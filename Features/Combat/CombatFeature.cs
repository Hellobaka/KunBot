using Another_Mirai_Native.Abstractions.Context;
using me.cqp.luohuaming.iKun.Domain.Configuration;
using me.cqp.luohuaming.iKun.Domain.Models;
using me.cqp.luohuaming.iKun.Features.Shared;
using me.cqp.luohuaming.iKun.Infrastructure;
using me.cqp.luohuaming.iKun.Infrastructure.WebQQ;

namespace me.cqp.luohuaming.iKun.Features.Combat;

/// <summary>
/// 战斗指令：攻击、吞噬。共享目标解析与结算播报逻辑。
/// </summary>
public sealed class CombatFeature
{
    public static CombatFeature Instance { get; } = new();

    private CombatFeature()
    { }

    public void Attack(GroupMessageContext e, string args) => Run(e, args, isAttack: true);

    public void Devour(GroupMessageContext e, string args) => Run(e, args, isAttack: false);

    private void Run(GroupMessageContext e, string args, bool isAttack)
    {
        var config = CoreConfiguration.Current;
        var replies = config.Replies;
        string command = isAttack ? config.CommandAttack : config.CommandDevour;

        // 解析目标：At / QQ / 昵称 / 群名片
        long target = ResolveTarget(e, args);
        if (target < 10000)
        {
            CommandHelper.Reply(e, CommandHelper.InvalidParams($"或无法找到目标，示例：{command} [QQ|At|昵称|卡片]"));
            return;
        }
        if (target == e.FromQQ.Id)
        {
            CommandHelper.Reply(e, isAttack ? replies.AttackSelf : replies.DevourSelf);
            return;
        }

        var player = Player.Find(e.FromQQ.Id);
        if (player is null)
        {
            CommandHelper.Reply(e, replies.NoPlayer);
            return;
        }
        if (Player.Find(target) is null)
        {
            CommandHelper.Reply(e, replies.TargetPlayerNotRegistered);
            return;
        }
        var kun = KunQuery.ActiveKunOf(player.QQ);
        if (kun is null)
        {
            CommandHelper.Reply(e, replies.NoKun);
            return;
        }
        kun.LoadAffixes();
        if (CommandHelper.IsBusyReplyIfSo(e, kun))
        {
            return;
        }
        var targetKun = KunQuery.ActiveKunOf(target);
        if (targetKun is null)
        {
            CommandHelper.Reply(e, replies.TargetPlayerHasNoKun);
            return;
        }
        targetKun.LoadAffixes();
        if (CommandHelper.IsBusyReplyIfSo(e, targetKun))
        {
            return;
        }

        // 冷却
        double cooldown = isAttack ? config.AttackCooldownMinutes : config.DevourCooldownMinutes;
        DateTime lastAction = isAttack ? player.AttackAt : player.DevourAt;
        string cooldownText = isAttack ? replies.AttackCooldown : replies.DevourCooldown;
        if (DateTime.Now - lastAction < TimeSpan.FromMinutes(cooldown))
        {
            CommandHelper.Reply(e, string.Format(cooldownText, lastAction.AddMinutes(cooldown).ToString("G")));
            return;
        }

        bool sameGroup = GroupMemberCache.Contains(e.FromGroup.Id, target);
        long otherGroupId = 0;
        if (!sameGroup)
        {
            otherGroupId = Record.ByKunId(targetKun.Id)?.Group ?? 0;
        }

        // 目标展示名
        string targetName;
        if (config.EnableAt)
        {
            targetName = Messages.At(target);
        }
        else
        {
            var info = sameGroup
                ? e.FromGroup.GetGroupMemberInfo(target)
                : otherGroupId > 0 ? Runtime.Api.GroupApi.GetGroupMemberInfo(otherGroupId, target) : null;
            targetName = string.IsNullOrWhiteSpace(info?.Card) ? target.ToString() : info.Card!;
        }

        // 执行
        dynamic result = isAttack ? ExecuteAttack(kun, targetKun, e, replies, targetName, sameGroup, otherGroupId, target)
                                  : ExecuteDevour(kun, targetKun, e, replies, targetName, sameGroup, otherGroupId, target);

        // 更新时间戳
        if (isAttack)
        {
            player.AttackAt = DateTime.Now;
        }
        else
        {
            player.DevourAt = DateTime.Now;
        }
        player.Save();
        _ = result; // 消息已在 Execute* 内发送
    }

    private Domain.Results.AttackResult ExecuteAttack(
        Kun kun, Kun targetKun, GroupMessageContext e, ReplyTexts replies,
        string targetName, bool sameGroup, long otherGroupId, long targetQQ)
    {
        var result = kun.Attack(targetKun);
        if (!result.Success)
        {
            CommandHelper.Reply(e, "攻击方法过程发生异常，查看日志获取更多信息");
            return result;
        }
        string message;
        if (result.AttackerDied)
        {
            message = string.Format(replies.AttackerDied, kun.ToString(), targetName, targetKun.ToString(),
                result.DefenderWeightDelta.ToShortNumber(), result.DefenderWeight.ToShortNumber());
        }
        else if (result.DefenderDied)
        {
            message = string.Format(replies.AttackTargetDied, kun.ToString(), targetName, targetKun.ToString(),
                result.AttackerWeightDelta.ToShortNumber(), result.AttackerWeight.ToShortNumber());
        }
        else if (result.AttackerWeightDelta < 0)
        {
            message = string.Format(replies.AttackFailed, kun.ToString(), targetName, targetKun.ToString(),
                (-result.AttackerWeightDelta).ToShortNumber(), result.AttackerWeight.ToShortNumber(),
                result.DefenderWeightDelta.ToShortNumber(), result.DefenderWeight.ToShortNumber());
        }
        else if (result.Escaped)
        {
            message = string.Format(replies.AttackEscaped, kun.ToString(), targetName, targetKun.ToString());
        }
        else
        {
            message = string.Format(replies.AttackSuccess, kun.ToString(), targetName, targetKun.ToString(),
                result.AttackerWeightDelta.ToShortNumber(), result.AttackerWeight.ToShortNumber(),
                result.DefenderWeightDelta.ToShortNumber(), result.DefenderWeight.ToShortNumber());
            if (result.HitWeightLimit)
            {
                message += $"\n{replies.WeightLimitReached}";
            }
        }
        CommandHelper.Reply(e, message);

        BroadcastCrossGroup(otherGroupId, sameGroup, e, result.AttackerWeightDelta > 0, result.Escaped, result.DefenderDied,
            result.DefenderWeightDelta, result.DefenderWeight);
        return result;
    }

    private Domain.Results.DevourResult ExecuteDevour(
        Kun kun, Kun targetKun, GroupMessageContext e, ReplyTexts replies,
        string targetName, bool sameGroup, long otherGroupId, long targetQQ)
    {
        var config = CoreConfiguration.Current;
        var result = kun.Devour(targetKun);
        if (!result.Success)
        {
            CommandHelper.Reply(e, "吞噬方法过程发生异常，查看日志获取更多信息");
            return result;
        }
        string message;
        if (result.AttackerDied)
        {
            message = string.Format(replies.DevouredByOther, kun.ToString(), targetName, targetKun.ToString(),
                result.DefenderWeightDelta.ToShortNumber(), result.DefenderWeight.ToShortNumber());
        }
        else if (result.AttackerWeightDelta < 0)
        {
            message = string.Format(replies.DevourFailed, kun.ToString(), targetName, targetKun.ToString(),
                (-result.AttackerWeightDelta).ToShortNumber(), result.AttackerWeight.ToShortNumber(),
                result.DefenderWeightDelta.ToShortNumber(), result.DefenderWeight.ToShortNumber());
        }
        else if (result.Escaped)
        {
            message = string.Format(replies.DevourEscaped, kun.ToString(), targetName, targetKun.ToString());
        }
        else
        {
            message = string.Format(replies.DevourSuccess, kun.ToString(), targetName, targetKun.ToString(),
                result.AttackerWeightDelta.ToShortNumber(), result.AttackerWeight.ToShortNumber());
            if (result.HitWeightLimit)
            {
                message += $"\n{replies.WeightLimitReached}";
            }
        }
        CommandHelper.Reply(e, message);

        // 跨群广播（吞噬无损失百分比门槛）
        if (!sameGroup && config.BroadcastCrossGroupDevour && otherGroupId > 0)
        {
            var text = result.Escaped
                ? (config.BroadcastCrossGroupDevourEscape
                    ? string.Format(replies.CrossGroupDevourEscaped, Messages.At(targetQQ)) : null)
                : result.AttackerWeightDelta > 0
                    ? string.Format(replies.CrossGroupDevoured, Messages.At(targetQQ)) : null;
            if (text is not null)
            {
                Messages.SendGroup(otherGroupId, text);
            }
        }
        return result;
    }

    /// <summary>攻击的跨群广播（带损失百分比门槛）</summary>
    private void BroadcastCrossGroup(long groupId, bool sameGroup, GroupMessageContext e,
        bool attackerWon, bool escaped, bool defenderDied, double loss, double remaining)
    {
        var config = CoreConfiguration.Current;
        if (sameGroup || groupId <= 0 || !config.BroadcastCrossGroupAttack)
        {
            return;
        }
        var replies = config.Replies;
        string? text = escaped
            ? config.BroadcastCrossGroupAttackEscape
                ? string.Format(replies.CrossGroupAttackEscaped, Messages.At(e.FromQQ.Id)) : null
            : defenderDied
                ? string.Format(replies.CrossGroupAttackedToDeath, Messages.At(e.FromQQ.Id))
                : attackerWon && loss / (remaining + loss) * 100 > config.CrossGroupNoticeMinLossPercent
                    ? string.Format(replies.CrossGroupAttacked, Messages.At(e.FromQQ.Id), loss.ToShortNumber(), remaining.ToShortNumber())
                    : null;
        if (text is not null)
        {
            Messages.SendGroup(groupId, text);
        }
    }

    /// <summary>从消息链或文本解析目标 QQ（At → QQ号 → 昵称/群名片模糊匹配）</summary>
    private static long ResolveTarget(GroupMessageContext e, string args)
    {
        foreach (var item in e.Message.MessageChain)
        {
            if (item is Another_Mirai_Native.Abstractions.Models.MessageItem.At { AllTarget: false } at)
            {
                return at.Target;
            }
        }
        args = args.Trim();
        if (string.IsNullOrEmpty(args))
        {
            return -1;
        }
        if (long.TryParse(args, out long qq))
        {
            return qq;
        }
        return GroupMemberCache.FindByName(e.FromGroup.Id, args)?.QQ ?? -1;
    }
}