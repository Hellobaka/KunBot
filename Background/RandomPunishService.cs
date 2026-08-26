using me.cqp.luohuaming.iKun.Domain.Configuration;
using me.cqp.luohuaming.iKun.Domain.Enums;
using me.cqp.luohuaming.iKun.Domain.Models;
using me.cqp.luohuaming.iKun.Infrastructure;
using me.cqp.luohuaming.iKun.Infrastructure.Logging;
using me.cqp.luohuaming.iKun.Infrastructure.Persistence;
using me.cqp.luohuaming.iKun.Infrastructure.WebQQ;

namespace me.cqp.luohuaming.iKun.Background;

/// <summary>
/// 每周天罚：按配置时间从存活鲲中按体重加权抽选并降罚。
/// </summary>
public static class RandomPunishService
{
    private static readonly Log Log = Log.For("天罚");
    private static readonly object Sync = new();

    private static CancellationTokenSource? _cancellation;
    private static Task? _loop;

    /// <summary>下次天罚时间（查询指令展示用）</summary>
    public static DateTime NextExecuteTime { get; private set; }

    public static void Start()
    {
        Stop();
        NextExecuteTime = ComputeNextTime();
        Log.Info($"下次天罚时间：{NextExecuteTime:G}");

        if (!CoreConfiguration.Current.EnableRandomPunish)
        {
            Log.Info("天罚功能未启用");
            return;
        }
        _cancellation = new CancellationTokenSource();
        _loop = Task.Run(async () =>
        {
            while (!_cancellation.Token.IsCancellationRequested)
            {
                if (DateTime.Now > NextExecuteTime)
                {
                    try
                    {
                        Execute();
                    }
                    catch (Exception e)
                    {
                        Log.Error(e, "执行天罚过程中发生异常");
                    }
                    NextExecuteTime = ComputeNextTime();
                    Log.Info($"天罚结束，下次天罚时间：{NextExecuteTime:G}");
                }
                await Task.Delay(1000, _cancellation.Token);
            }
        }, _cancellation.Token);
    }

    public static void Stop()
    {
        if (_cancellation is null)
        {
            return;
        }
        _cancellation.Cancel();
        try
        {
            _loop?.Wait(3000);
        }
        catch
        {
            // 取消属预期
        }
        _cancellation = null;
        _loop = null;
    }

    /// <summary>计算下一个执行时点（每周第 X 天的指定时刻）</summary>
    private static DateTime ComputeNextTime()
    {
        var config = CoreConfiguration.Current;
        var target = config.PunishExecuteDayOfWeek == 7 ? DayOfWeek.Sunday : (DayOfWeek)config.PunishExecuteDayOfWeek;
        var candidate = DateTime.Now;
        for (int i = 0; i <= 7 && candidate.DayOfWeek != target; i++)
        {
            candidate = candidate.AddDays(1);
        }
        var time = config.PunishExecuteTime;
        candidate = new DateTime(candidate.Year, candidate.Month, candidate.Day, time.Hour, time.Minute, time.Second);
        return candidate < DateTime.Now ? candidate.AddDays(7) : candidate;
    }

    private static void Execute()
    {
        Log.Info("天罚开始");
        var config = CoreConfiguration.Current;
        var replies = config.Replies;

        // 体重加权抽选
        var alive = QueryAliveKuns();
        if (alive.Count == 0)
        {
            Log.Info("没有存活鲲，跳过");
            return;
        }
        double totalWeight = alive.Sum(k => k.Weight);
        if (totalWeight <= 0)
        {
            Log.Info("总体重为 0，跳过");
            return;
        }
        Kun? target = null;
        Record? record = null;
        var roll = Extensions.Rng.NextDouble(0, totalWeight);
        double cumulative = 0;
        foreach (var kun in alive)
        {
            cumulative += kun.Weight;
            if (cumulative >= roll)
            {
                target = kun;
                break;
            }
        }
        if (target is null || (record = Record.ByKunId(target.Id)) is null)
        {
            Log.Error("未抽到目标鲲或缺少归属记录");
            return;
        }
        target.LoadAffixes();

        // 执行判定
        if (Extensions.Rng.NextDouble() >= config.PunishChancePercent / 100.0)
        {
            foreach (var group in config.EnabledGroups)
            {
                Messages.SendGroup(group, replies.PunishSkipped);
                Thread.Sleep(TimeSpan.FromMinutes(1));
            }
            Log.Info("天罚无事，未执行");
            return;
        }

        double originWeight = target.Weight;
        var lossRatio = Extensions.Rng.NextDouble(config.PunishLossMinPercent, config.PunishLossMaxPercent) / 100;
        target.Weight *= 1 - lossRatio;

        if (Extensions.Rng.NextDouble() < config.PunishDeathChancePercent / 100.0)
        {
            target.Alive = false;
            target.DeadAt = DateTime.Now;
            target.Save();
            ForceStopIdle(target.Id, IdleType.Experience);
            ForceStopIdle(target.Id, IdleType.Coin);
            Messages.SendGroup(record.Group,
                string.Format(replies.PunishExecutedAndDied, Messages.At(record.QQ), target));
            Log.Info("天罚结束，鲲已死亡");
            return;
        }

        target.Save();
        Messages.SendGroup(record.Group, string.Format(
            replies.PunishExecuted,
            Messages.At(record.QQ), target,
            (originWeight - target.Weight).ToShortNumber(), target.Weight.ToShortNumber()));
        Log.Info("天罚结束");
    }

    private static List<Kun> QueryAliveKuns()
    {
        using var db = Db.CreateSession();
        return db.Queryable<Kun>().Where(x => x.Alive && !x.Abandoned).ToList().OrderByDescending(x => x.Weight).ToList();
    }

    private static void ForceStopIdle(int kunId, IdleType type)
    {
        var record = IdleScheduler.LatestFor(kunId, type);
        if (record is { Running: true })
        {
            IdleScheduler.Instance.Stop(record);
        }
    }
}