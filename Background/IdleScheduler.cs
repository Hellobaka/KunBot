using Another_Mirai_Native.Abstractions.Services;
using me.cqp.luohuaming.iKun.Domain.Configuration;
using me.cqp.luohuaming.iKun.Domain.Enums;
using me.cqp.luohuaming.iKun.Domain.Models;
using me.cqp.luohuaming.iKun.Domain.Results;
using me.cqp.luohuaming.iKun.Infrastructure;
using me.cqp.luohuaming.iKun.Infrastructure.Logging;
using me.cqp.luohuaming.iKun.Infrastructure.Persistence;

namespace me.cqp.luohuaming.iKun.Background;

/// <summary>
/// 挂机/打工调度器：管理运行中任务的计时线程、持久化与结算事件。
/// </summary>
public sealed class IdleScheduler
{
    private static readonly Log Log = Log.For(nameof(IdleScheduler));
    private static readonly object Sync = new();

    private readonly List<RunningIdle> _running = [];
    private IMessageApi _messageApi = null!;

    /// <summary>任务完成事件：(任务, 结算结果, 鲲)。结果可能为 null（鲲已死亡等）。</summary>
    public event Action<AutoPlay, IdleSettlement?, Kun?>? IdleFinished;

    private sealed class RunningIdle
    {
        public AutoPlay Record { get; init; }

        public CancellationTokenSource Cancellation { get; init; }

        public Task? Loop { get; set; }
    }

    // ---- 生命周期 ----

    private static IdleScheduler? _instance;

    /// <summary>全局单例（Entry 启动时装配）</summary>
    public static IdleScheduler Instance => _instance ??= new IdleScheduler();

    /// <summary>恢复数据库中标记为运行中的任务；已超时的立即结算</summary>
    public void ResumeFromDatabase(IMessageApi messageApi)
    {
        _messageApi = messageApi;
        using var db = Db.CreateSession();
        var persisted = db.Queryable<AutoPlay>().Where(x => x.Running).ToList();
        foreach (var record in persisted)
        {
            if (DateTime.Now > record.StartTime.AddHours(record.Duration))
            {
                MarkStopped(record);
                SettleAndNotify(record);
            }
            else
            {
                StartLoop(record);
            }
        }
        Log.Info($"恢复挂机任务 {persisted.Count} 条");
    }

    /// <summary>插件停用时停止全部计时循环（不结算）</summary>
    public void Shutdown()
    {
        lock (Sync)
        {
            foreach (var running in _running)
            {
                running.Cancellation.Cancel();
            }
            _running.Clear();
        }
    }

    // ---- 查询 ----

    /// <summary>鲲是否正在指定类型的任务中</summary>
    public static bool IsRunning(int kunId, IdleType type) =>
        Instance.IsRunningCore(kunId, type);

    private bool IsRunningCore(int kunId, IdleType type)
    {
        lock (Sync)
        {
            return _running.Any(x => x.Record.KunID == kunId && x.Record.IdleType == type && x.Record.Running);
        }
    }

    /// <summary>查询鲲最近一次同类型记录</summary>
    public static AutoPlay? LatestFor(int kunId, IdleType type)
    {
        using var db = Db.CreateSession();
        return db.Queryable<AutoPlay>()
            .Where(x => x.KunID == kunId && (int)x.AutoPlayType == (int)type)
            .OrderByDescending(x => x.StartTime)
            .First();
    }

    /// <summary>是否处于冷却。返回 false 时输出可开始时间。</summary>
    public static bool IsOffCooldown(int kunId, IdleType type, out DateTime availableAt)
    {
        availableAt = DateTime.Now;
        var latest = LatestFor(kunId, type);
        if (latest is null || (!latest.Running && latest.EndTime == default))
        {
            return true;
        }
        var config = CoreConfiguration.Current;
        double cooldownHours = type == IdleType.Experience ? config.IdleCooldownHours : config.WorkCooldownHours;
        availableAt = (latest.Running ? latest.StartTime.AddHours(latest.Duration) : latest.EndTime).AddHours(cooldownHours);
        return availableAt < DateTime.Now;
    }

    // ---- 控制 ----

    /// <summary>创建并启动新任务（调用前应完成全部校验）</summary>
    public AutoPlay Launch(int kunId, long groupId, int durationHours, IdleType type)
    {
        var record = new AutoPlay
        {
            KunID = kunId,
            GroupId = groupId,
            Duration = durationHours,
            StartTime = DateTime.Now,
            EndTime = DateTime.Now.AddHours(durationHours),
            AutoPlayType = type == IdleType.Coin ? Domain.Enums.AutoPlayType.Coin : Domain.Enums.AutoPlayType.Exp,
        };
        using (var db = Db.CreateSession())
        {
            record.ID = db.Insertable(record).ExecuteReturnIdentity();
        }
        StartLoop(record);
        return record;
    }

    /// <summary>
    /// 停止任务并结算。返回结算结果；任务不存在时返回 null。
    /// </summary>
    public IdleSettlement? Stop(AutoPlay record)
    {
        RunningIdle? running;
        lock (Sync)
        {
            running = _running.FirstOrDefault(x => x.Record.ID == record.ID);
        }
        if (running is null)
        {
            return null;
        }
        running.Cancellation.Cancel();
        try
        {
            running.Loop?.Wait(5000);
        }
        catch
        {
            // 计时循环被取消属预期
        }
        MarkStopped(running.Record);
        return Settle(running.Record, out _);
    }

    private void StartLoop(AutoPlay record)
    {
        MarkStarted(record);
        var cts = new CancellationTokenSource();
        var running = new RunningIdle { Record = record, Cancellation = cts };
        lock (Sync)
        {
            _running.Add(running);
        }
        running.Loop = Task.Run(async () =>
        {
            try
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    if (DateTime.Now > record.StartTime.AddHours(record.Duration))
                    {
                        MarkStopped(record);
                        SettleAndNotify(record);
                        break;
                    }
                    await Task.Delay(1000, cts.Token);
                }
            }
            catch (OperationCanceledException)
            {
                // 外部取消（Stop / Shutdown）
            }
        }, cts.Token);
    }

    // ---- 持久化状态 ----

    private static void MarkStarted(AutoPlay record)
    {
        record.Running = true;
        using var db = Db.CreateSession();
        db.Updateable(record).ExecuteCommand();
    }

    private static void MarkStopped(AutoPlay record)
    {
        record.Running = false;
        var scheduledEnd = record.StartTime.AddHours(record.Duration);
        record.EndTime = scheduledEnd < DateTime.Now ? scheduledEnd : DateTime.Now;
        using var db = Db.CreateSession();
        db.Updateable(record).ExecuteCommand();
        lock (Sync)
        {
            Instance._running.RemoveAll(x => x.Record.ID == record.ID);
        }
    }

    // ---- 结算 ----

    private void SettleAndNotify(AutoPlay record)
    {
        var result = Settle(record, out var kun);
        Log.Info($"挂机结束，ID={record.ID}，类型={record.AutoPlayType}");
        try
        {
            IdleFinished?.Invoke(record, result, kun);
        }
        catch (Exception e)
        {
            Log.Error(e, "处理挂机完成事件过程中发生异常");
        }
    }

    /// <summary>结算：发放收益、更新鲲状态，返回结果对象</summary>
    private IdleSettlement? Settle(AutoPlay record, out Kun? kun)
    {
        kun = KunRepository.FindById(record.KunID);
        if (kun is null || !kun.Alive || kun.Abandoned)
        {
            Log.Info("目标鲲不存在或已死亡或已被抛弃");
            return null;
        }
        kun.LoadAffixes();

        if (record.IdleType == IdleType.Coin)
        {
            return SettleCoins(record, kun);
        }
        return SettleExperience(record, kun);
    }

    private IdleSettlement SettleCoins(AutoPlay record, Kun kun)
    {
        var coins = (int)IdleMath.TotalCoins(kun.Level, record.StartTime, record.EndTime);
        var player = Player.Find(kun.PlayerID);
        if (player is null)
        {
            Log.Info("未找到鲲对应的玩家");
            return Blank(record);
        }
        player.GrantItems([Item.Coin(coins)]);
        return new IdleSettlement
        {
            StartTime = record.StartTime,
            EndTime = record.EndTime,
            CoinsEarned = coins,
            CurrentWeight = kun.Weight,
        };
    }

    private IdleSettlement SettleExperience(AutoPlay record, Kun kun)
    {
        var config = CoreConfiguration.Current;
        double originalWeight = kun.Weight;
        kun.Weight += IdleMath.TotalExperience(kun.Level, record.StartTime, record.EndTime);
        kun.Weight = Math.Min(kun.Weight, Kun.WeightLimitOf(kun.Level));

        if (Extensions.Rng.NextDouble() < config.IdleDeathChancePercent / 100.0)
        {
            Log.Info("走火入魔判定成功，鲲已死亡");
            kun.Alive = false;
            kun.DeadAt = DateTime.Now;
        }
        kun.Save();

        return new IdleSettlement
        {
            StartTime = record.StartTime,
            EndTime = record.EndTime,
            WeightGained = kun.Weight - originalWeight,
            CurrentWeight = kun.Weight,
            HitWeightLimit = kun.Weight == Kun.WeightLimitOf(kun.Level),
            Died = !kun.Alive,
        };
    }

    private static IdleSettlement Blank(AutoPlay record) => new()
    {
        StartTime = record.StartTime,
        EndTime = record.EndTime,
    };

    /// <summary>鲲数据访问（调度器内部使用，避免依赖 Feature 层）</summary>
    private static class KunRepository
    {
        public static Kun? FindById(int id)
        {
            using var db = Db.CreateSession();
            return db.Queryable<Kun>().First(x => x.Id == id);
        }
    }
}