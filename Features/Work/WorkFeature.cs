using Another_Mirai_Native.Abstractions.Context;
using me.cqp.luohuaming.iKun.Background;
using me.cqp.luohuaming.iKun.Domain.Configuration;
using me.cqp.luohuaming.iKun.Domain.Enums;
using me.cqp.luohuaming.iKun.Domain.Models;
using me.cqp.luohuaming.iKun.Features.Shared;

using me.cqp.luohuaming.iKun.Infrastructure;

namespace me.cqp.luohuaming.iKun.Features.Work;

/// <summary>
/// 挂机/打工指令：开始、停止（两类共用流程，仅类型与文案不同）。
/// </summary>
public sealed class WorkFeature
{
    public static WorkFeature Instance { get; } = new();

    private WorkFeature()
    { }

    public void StartIdle(GroupMessageContext e, string args) => Start(e, args, IdleType.Experience);

    public void StartWork(GroupMessageContext e, string args) => Start(e, args, IdleType.Coin);

    public void StopIdle(GroupMessageContext e) => Stop(e, IdleType.Experience);

    public void StopWork(GroupMessageContext e) => Stop(e, IdleType.Coin);

    // ---- 开始 ----

    private void Start(GroupMessageContext e, string args, IdleType type)
    {
        var config = CoreConfiguration.Current;
        var replies = config.Replies;
        bool isIdle = type == IdleType.Experience;

        var hoursOrNull = CommandHelper.ParseInt(args);
        if (hoursOrNull is null)
        {
            string example = isIdle ? config.CommandStartIdle : config.CommandStartWork;
            CommandHelper.Reply(e, CommandHelper.InvalidParams($"，示例：{example} 整数小时"));
            return;
        }
        int duration = Math.Max(1, hoursOrNull.Value);
        if (duration > config.MaxIdleDurationHours)
        {
            CommandHelper.Reply(e, CommandHelper.InvalidParams($"，参数最大为 {config.MaxIdleDurationHours}"));
            return;
        }
        if (!CommandHelper.TryLoadPlayerAndKun(e, out _, out var kun))
        {
            return;
        }

        // 状态校验
        string startBlocked = isIdle ? replies.IdleStartBlocked : "";
        if (!kun.Alive)
        {
            CommandHelper.Reply(e, string.Format(replies.KunDead, startBlocked));
            return;
        }
        if (kun.Abandoned)
        {
            CommandHelper.Reply(e, string.Format(replies.KunAbandoned, startBlocked));
            return;
        }
        if (isIdle && kun.Weight >= Kun.WeightLimitOf(kun.Level))
        {
            CommandHelper.Reply(e, replies.WeightLimitReached);
            return;
        }
        if (CommandHelper.IsBusyReplyIfSo(e, kun))
        {
            return;
        }
        if (!IdleScheduler.IsOffCooldown(kun.Id, type, out DateTime availableAt))
        {
            string cooldownText = isIdle ? replies.IdleCooldown : replies.WorkCooldown;
            CommandHelper.Reply(e, string.Format(cooldownText, availableAt.ToString("G")));
            return;
        }

        var record = IdleScheduler.Instance.Launch(kun.Id, e.FromGroup.Id, duration, type);
        if (isIdle)
        {
            var exp = IdleMath.TotalExperience(kun.Level, record.StartTime, record.EndTime);
            CommandHelper.Reply(e, string.Format(replies.IdleStarted, record.EndTime.ToString("G"), exp.ToShortNumber()));
        }
        else
        {
            var coins = (int)IdleMath.TotalCoins(kun.Level, record.StartTime, record.EndTime);
            CommandHelper.Reply(e, string.Format(replies.WorkStarted, record.EndTime.ToString("G"), coins));
        }
    }

    // ---- 停止 ----

    private void Stop(GroupMessageContext e, IdleType type)
    {
        var replies = CoreConfiguration.Current.Replies;
        bool isIdle = type == IdleType.Experience;
        if (!CommandHelper.TryLoadPlayerAndKun(e, out _, out var kun))
        {
            return;
        }
        if (!IdleScheduler.IsRunning(kun.Id, type))
        {
            CommandHelper.Reply(e, string.Format(isIdle ? replies.KunNotIdling : replies.KunNotWorking, kun));
            return;
        }
        var record = IdleScheduler.LatestFor(kun.Id, type);
        if (record is null)
        {
            CommandHelper.Reply(e, string.Format(isIdle ? replies.KunNotIdling : replies.KunNotWorking, kun));
            return;
        }
        var result = IdleScheduler.Instance.Stop(record);
        if (result is null)
        {
            CommandHelper.Reply(e, string.Format(replies.KunDead, kun));
            return;
        }
        if (isIdle)
        {
            CommandHelper.Reply(e, result.Died
                ? string.Format(replies.IdleFinishedButDead, kun, result.Duration.TotalHours.ToString("f2"), result.WeightGained.ToShortNumber())
                : BuildIdleStopMessage(replies, kun, result));
        }
        else
        {
            CommandHelper.Reply(e, string.Format(
                replies.WorkFinished, kun, result.Duration.TotalHours.ToString("f2"), result.CoinsEarned, result.CoinsEarned));
        }
    }

    private static string BuildIdleStopMessage(ReplyTexts replies, Kun kun, Domain.Results.IdleSettlement result)
    {
        var message = string.Format(
            replies.IdleFinished, kun, result.Duration.TotalHours.ToString("f2"),
            result.WeightGained.ToShortNumber(), result.CurrentWeight.ToShortNumber());
        return result.HitWeightLimit ? $"{message}\n{replies.WeightLimitReached}" : message;
    }
}