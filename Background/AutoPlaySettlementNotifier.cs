using me.cqp.luohuaming.iKun.Domain.Configuration;
using me.cqp.luohuaming.iKun.Domain.Enums;
using me.cqp.luohuaming.iKun.Domain.Models;
using me.cqp.luohuaming.iKun.Domain.Results;
using me.cqp.luohuaming.iKun.Infrastructure;
using me.cqp.luohuaming.iKun.Infrastructure.Logging;
using me.cqp.luohuaming.iKun.Infrastructure.WebQQ;

namespace me.cqp.luohuaming.iKun.Background;

/// <summary>
/// 挂机/打工完成通知：把 IdleScheduler 的结算事件转换为群消息。
/// </summary>
public static class AutoPlaySettlementNotifier
{
    private static readonly Log Log = Log.For("挂机结算");

    /// <summary>订阅调度器结算事件（Entry 启用时调用）</summary>
    public static void Attach()
    {
        IdleScheduler.Instance.IdleFinished -= OnIdleFinished;
        IdleScheduler.Instance.IdleFinished += OnIdleFinished;
    }

    public static void Detach()
    {
        IdleScheduler.Instance.IdleFinished -= OnIdleFinished;
    }

    private static void OnIdleFinished(AutoPlay record, IdleSettlement? result, Kun? kun)
    {
        try
        {
            if (result is null || kun is null || !CoreConfiguration.Current.EnabledGroups.Contains(record.GroupId))
            {
                return;
            }
            var replies = CoreConfiguration.Current.Replies;
            string message;
            if (record.IdleType == IdleType.Coin)
            {
                // 打工：鲲死亡时不播报
                if (!kun.Alive)
                {
                    return;
                }
                var player = Player.Find(kun.PlayerID);
                int coins = player is null ? 0 : InventoryItem.CountOf(player, ItemId.Coin);
                message = string.Format(
                    Messages.At(kun.PlayerID) + replies.WorkFinished,
                    kun, result.Duration.TotalHours, result.CoinsEarned, coins);
            }
            else
            {
                message = result.Died
                    ? string.Format(
                        Messages.At(kun.PlayerID) + replies.IdleFinishedButDead,
                        kun, result.Duration.TotalHours, result.WeightGained.ToShortNumber())
                    : BuildIdleFinishedMessage(replies, kun, result);
            }
            Messages.SendGroup(record.GroupId, message);
        }
        catch (Exception e)
        {
            Log.Error(e, "发送挂机完成消息异常");
        }
    }

    private static string BuildIdleFinishedMessage(ReplyTexts replies, Kun kun, IdleSettlement result)
    {
        var message = string.Format(
            Messages.At(kun.PlayerID) + replies.IdleFinished,
            kun, result.Duration.TotalHours, result.WeightGained.ToShortNumber(), result.CurrentWeight.ToShortNumber());
        return result.HitWeightLimit ? $"{message}\n{replies.WeightLimitReached}" : message;
    }
}