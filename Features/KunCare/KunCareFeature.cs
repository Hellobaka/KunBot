using Another_Mirai_Native.Abstractions.Context;
using me.cqp.luohuaming.iKun.Domain.Configuration;
using me.cqp.luohuaming.iKun.Domain.Enums;
using me.cqp.luohuaming.iKun.Domain.Models;
using me.cqp.luohuaming.iKun.Domain.PetAttributes;
using me.cqp.luohuaming.iKun.Features.Shared;

using me.cqp.luohuaming.iKun.Infrastructure;
using me.cqp.luohuaming.iKun.Infrastructure.Persistence;

namespace me.cqp.luohuaming.iKun.Features.KunCare;

/// <summary>
/// 鲲养成指令：孵化、喂养、强化、幻化、渡劫、复活、放生、查询死亡鲲、我的鲲。
/// </summary>
public sealed class KunCareFeature
{
    public static KunCareFeature Instance { get; } = new();

    private KunCareFeature()
    { }

    // ---- 我的鲲 ----

    public void MyKun(GroupMessageContext e)
    {
        if (!CommandHelper.TryLoadPlayerAndKun(e, out _, out var kun))
        {
            return;
        }
        CommandHelper.Reply(e, kun.ToDetailedString(true));
    }

    // ---- 孵化 ----

    public void Hatch(GroupMessageContext e, string args)
    {
        var config = CoreConfiguration.Current;
        var countOrNull = CommandHelper.ParseInt(args);
        if (countOrNull is null)
        {
            CommandHelper.Reply(e, CommandHelper.InvalidParams($"，示例：{config.CommandHatch} 数量"));
            return;
        }
        int count = Math.Max(1, countOrNull.Value);

        // 孵化前置：已注册且当前无存活鲲
        var player = Player.Find(e.FromQQ.Id);
        if (player is null)
        {
            CommandHelper.Reply(e, config.Replies.NoPlayer);
            return;
        }
        if (KunQuery.ActiveKunOf(player.QQ) is not null)
        {
            CommandHelper.Reply(e, config.Replies.DuplicateHatch);
            return;
        }
        if (!ConsumeEggsAndHatch(e, player, count))
        {
            return;
        }
    }

    private bool ConsumeEggsAndHatch(GroupMessageContext e, Player player, int count)
    {
        var config = CoreConfiguration.Current;
        var replies = config.Replies;
        var eggName = ItemCatalog.Definition(ItemId.KunEgg)!.Name;

        if (!InventoryItem.TryRemove(player, ItemId.KunEgg, count, out int remainingEggs))
        {
            CommandHelper.Reply(e, string.Format(replies.ItemLeak, eggName, count, remainingEggs));
            return false;
        }

        // 逐蛋判定孵化成功率
        int successThreshold = Extensions.Rng.Next(config.HatchRateMinPercent, config.HatchRateMaxPercent);
        int consumed = 0;
        bool hatched = false;
        for (consumed = 1; consumed <= count; consumed++)
        {
            if (Extensions.Rng.Next(100) > successThreshold)
            {
                continue;
            }
            hatched = true;
            break;
        }
        if (!hatched)
        {
            CommandHelper.Reply(e, string.Format(replies.HatchFailed, remainingEggs));
            return false;
        }

        // 退还多余蛋
        int refund = count - consumed;
        if (refund > 0)
        {
            player.GrantItems([Item.KunEgg(refund)]);
        }

        // 生成新鲲
        var mainAffix = PetAttributeFactory.CreateRandomMain();
        var affix1 = Affix.CreateRandom();
        var affix2 = Affix.CreateRandom();
        var kun = new Kun
        {
            AttributeAID = (int)mainAffix.Element,
            AttributeBID = affix1.AffixId,
            AttributeCID = affix2.AffixId,
            PlayerID = player.QQ,
            Weight = Extensions.Rng.Next(config.HatchWeightMin, config.HatchWeightMax),
            Alive = true,
        };
        kun.Level = (int)Math.Log10(kun.Weight) + 1;

        using (var db = Db.CreateSession())
        {
            kun.Id = db.Insertable(kun).ExecuteReturnIdentity();
        }
        kun.LoadAffixes();
        Record.Add(new Record { Group = e.FromGroup.Id, QQ = e.FromQQ.Id, KunID = kun.Id });

        CommandHelper.Reply(e, count > 1
            ? string.Format(replies.HatchMultiSuccess, kun.ToString(), kun.Weight.ToShortNumber(), consumed, remainingEggs + refund)
            : string.Format(replies.HatchSingleSuccess, kun.ToString(), kun.Weight.ToShortNumber(), remainingEggs));
        return true;
    }

    // ---- 喂养 ----

    public void Feed(GroupMessageContext e, string args)
    {
        var config = CoreConfiguration.Current;
        var replies = config.Replies;
        if (!CommandHelper.TryLoadPlayerAndKun(e, out var player, out var kun))
        {
            return;
        }
        if (CommandHelper.IsBusyReplyIfSo(e, kun))
        {
            return;
        }

        var countOrNull = CommandHelper.ParseInt(args, defaultToOne: true);
        if (countOrNull is null)
        {
            CommandHelper.Reply(e, CommandHelper.InvalidParams($"，示例：{config.CommandFeed} 数量"));
            return;
        }
        int count = Math.Max(1, countOrNull.Value);

        // 达到体重上限后拒绝喂养
        if (kun.Weight >= Kun.WeightLimitOf(kun.Level))
        {
            CommandHelper.Reply(e, replies.WeightLimitReached);
            return;
        }

        int coinCost = count * config.FeedCoinCostPerCount;
        int eggCost = count * config.FeedEggCostPerCount;
        int coins = InventoryItem.CountOf(player, ItemId.Coin);
        int eggs = InventoryItem.CountOf(player, ItemId.KunEgg);
        if (coins < coinCost)
        {
            CommandHelper.Reply(e, string.Format(replies.ItemLeak, ItemCatalog.Definition(ItemId.Coin)!.Name, coinCost, coins));
            return;
        }
        if (eggs < eggCost)
        {
            CommandHelper.Reply(e, string.Format(replies.ItemLeak, ItemCatalog.Definition(ItemId.KunEgg)!.Name, eggCost, eggs));
            return;
        }
        InventoryItem.TryRemove(player, ItemId.Coin, coinCost, out coins);
        InventoryItem.TryRemove(player, ItemId.KunEgg, eggCost, out eggs);

        var result = kun.Feed(count);
        if (!result.Success)
        {
            CommandHelper.Reply(e, "喂养方法过程发生异常，查看日志获取更多信息");
            return;
        }
        var builder = new System.Text.StringBuilder();
        builder.AppendLine(string.Format(
            replies.FeedResult, kun.ToString(), result.WeightDelta.ToShortNumber(),
            result.CurrentWeight.ToShortNumber(), coins, eggs));
        if (result.HitWeightLimit)
        {
            builder.AppendLine(replies.WeightLimitReached);
        }
        builder.RemoveTrailingNewLine();
        CommandHelper.Reply(e, builder.ToString());
    }

    // ---- 强化 ----

    public void Upgrade(GroupMessageContext e, string args)
    {
        var config = CoreConfiguration.Current;
        var replies = config.Replies;
        if (!CommandHelper.TryLoadPlayerAndKun(e, out var player, out var kun))
        {
            return;
        }
        if (CommandHelper.IsBusyReplyIfSo(e, kun))
        {
            return;
        }
        var countOrNull = CommandHelper.ParseInt(args, defaultToOne: true);
        if (countOrNull is null)
        {
            CommandHelper.Reply(e, CommandHelper.InvalidParams($"，示例：{config.CommandUpgrade} 数量"));
            return;
        }
        int count = Math.Max(1, countOrNull.Value);

        int coinCost = count * config.UpgradeCoinCost;
        int pillCost = count * config.UpgradePillCost;
        int coins = InventoryItem.CountOf(player, ItemId.Coin);
        int pills = InventoryItem.CountOf(player, ItemId.UpgradePill);
        if (coins < coinCost)
        {
            CommandHelper.Reply(e, string.Format(replies.ItemLeak, ItemCatalog.Definition(ItemId.Coin)!.Name, coinCost, coins));
            return;
        }
        if (pills < pillCost)
        {
            CommandHelper.Reply(e, string.Format(replies.ItemLeak, ItemCatalog.Definition(ItemId.UpgradePill)!.Name, pillCost, pills));
            return;
        }
        InventoryItem.TryRemove(player, ItemId.Coin, coinCost, out coins);
        InventoryItem.TryRemove(player, ItemId.UpgradePill, pillCost, out pills);

        var result = kun.Upgrade(count);
        if (!result.Success)
        {
            CommandHelper.Reply(e, "强化方法过程发生异常，查看日志获取更多信息");
            return;
        }
        var builder = new System.Text.StringBuilder();
        string upgradeText = result.WeightDelta > 0
            ? string.Format(replies.UpgradeSuccess, result.WeightDelta.ToShortNumber(), result.CurrentWeight.ToShortNumber(), pills, coins)
            : string.Format(replies.UpgradeFailed, (-result.WeightDelta).ToShortNumber(), result.CurrentWeight.ToShortNumber(), pills, coins);
        builder.AppendLine(upgradeText);
        if (result.HitWeightLimit)
        {
            builder.AppendLine(replies.WeightLimitReached);
        }
        builder.RemoveTrailingNewLine();
        CommandHelper.Reply(e, builder.ToString());
    }

    // ---- 幻化 ----

    public void Transmogrify(GroupMessageContext e)
    {
        var config = CoreConfiguration.Current;
        var replies = config.Replies;
        if (!CommandHelper.TryLoadPlayerAndKun(e, out var player, out var kun))
        {
            return;
        }
        if (kun.Level < config.TransmogrifyLevelRequirement)
        {
            CommandHelper.Reply(e, string.Format(replies.TransmogrifyLevelLimit, kun.Level, config.TransmogrifyLevelRequirement));
            return;
        }
        if (CommandHelper.IsBusyReplyIfSo(e, kun))
        {
            return;
        }
        int coins = InventoryItem.CountOf(player, ItemId.Coin);
        int pills = InventoryItem.CountOf(player, ItemId.TransmogrifyPill);
        if (coins < config.TransmogrifyCoinCost)
        {
            CommandHelper.Reply(e, string.Format(replies.ItemLeak, ItemCatalog.Definition(ItemId.Coin)!.Name, config.TransmogrifyCoinCost, coins));
            return;
        }
        if (pills < config.TransmogrifyPillCost)
        {
            CommandHelper.Reply(e, string.Format(replies.ItemLeak, ItemCatalog.Definition(ItemId.TransmogrifyPill)!.Name, config.TransmogrifyPillCost, pills));
            return;
        }
        InventoryItem.TryRemove(player, ItemId.Coin, config.TransmogrifyCoinCost, out coins);
        InventoryItem.TryRemove(player, ItemId.TransmogrifyPill, config.TransmogrifyPillCost, out pills);

        var result = kun.Transmogrify();
        if (!result.Success || result.CurrentMain is null || result.OriginalMain is null)
        {
            CommandHelper.Reply(e, "幻化方法过程发生异常，查看日志获取更多信息");
            return;
        }
        if (result.Died)
        {
            CommandHelper.Reply(e, string.Format(replies.TransmogrifyDied, pills, coins));
            return;
        }
        bool unchanged = result.CurrentMain.Element == result.OriginalMain.Element &&
                         result.CurrentAffix1!.AffixId == result.OriginalAffix1!.AffixId &&
                         result.CurrentAffix2!.AffixId == result.OriginalAffix2!.AffixId;
        if (unchanged)
        {
            CommandHelper.Reply(e, string.Format(replies.TransmogrifyFailed, result.WeightLoss.ToShortNumber(), result.CurrentWeight.ToShortNumber(), pills, coins));
            return;
        }
        string Describe(PetAttribute main, PetAttribute a1, PetAttribute a2) =>
            $"[{main.Name}]{a1.Name}{a2.Name}鲲";
        CommandHelper.Reply(e, string.Format(
            replies.TransmogrifySuccess,
            Describe(result.OriginalMain, result.OriginalAffix1!, result.OriginalAffix2!),
            Describe(result.CurrentMain, result.CurrentAffix1!, result.CurrentAffix2!),
            result.WeightLoss.ToShortNumber(), result.CurrentWeight.ToShortNumber(), pills, coins));
    }

    // ---- 渡劫 ----

    public void Ascend(GroupMessageContext e)
    {
        var config = CoreConfiguration.Current;
        var replies = config.Replies;
        if (!CommandHelper.TryLoadPlayerAndKun(e, out var player, out var kun))
        {
            return;
        }
        if (CommandHelper.IsBusyReplyIfSo(e, kun))
        {
            return;
        }
        // 挂起渡劫丹校验与加成
        if (player.AscendPillComsume > 0 &&
            InventoryItem.CountOf(player, ItemId.AscendPill) < player.AscendPillComsume)
        {
            CommandHelper.Reply(e, string.Format(
                replies.ItemLeak, ItemCatalog.Definition(ItemId.AscendPill)!.Name,
                player.AscendPillComsume, InventoryItem.CountOf(player, ItemId.AscendPill)));
            return;
        }
        kun.AscendBonusPercent = player.AscendPillComsume * config.AscendSuccessPerPillPercent;

        if (kun.Weight < Kun.WeightLimitOf(kun.Level))
        {
            CommandHelper.Reply(e, string.Format(
                replies.AscendWeightBelowLimit,
                kun.Weight.ToShortNumber(), Kun.WeightLimitOf(kun.Level).ToShortNumber()));
            return;
        }
        if (!InventoryItem.TryRemove(player, ItemId.Coin, config.AscendCoinCost, out int coins))
        {
            CommandHelper.Reply(e, string.Format(replies.ItemLeak, ItemCatalog.Definition(ItemId.Coin)!.Name, config.AscendCoinCost, coins));
            return;
        }

        var result = kun.Ascend();
        if (!result.Success)
        {
            CommandHelper.Reply(e, "渡劫方法过程发生异常，查看日志获取更多信息");
            return;
        }
        // 扣除挂起的渡劫丹
        if (player.AscendPillComsume > 0)
        {
            if (!InventoryItem.TryRemove(player, ItemId.AscendPill, player.AscendPillComsume, out int pillsLeft))
            {
                CommandHelper.Reply(e, string.Format(replies.ItemLeak, ItemCatalog.Definition(ItemId.AscendPill)!.Name, player.AscendPillComsume, pillsLeft));
                return;
            }
            player.AscendPillComsume = 0;
            player.Save();
        }
        if (result.Died)
        {
            CommandHelper.Reply(e, replies.AscendDied);
        }
        else if (result.WeightDelta > 0)
        {
            CommandHelper.Reply(e, string.Format(replies.AscendSuccess, result.WeightDelta.ToShortNumber(), result.CurrentWeight.ToShortNumber(), result.CurrentLevel));
        }
        else
        {
            CommandHelper.Reply(e, string.Format(replies.AscendFailed, result.WeightDelta.ToShortNumber(), result.CurrentWeight.ToShortNumber()));
        }
    }

    /// <summary>挂起渡劫丹（下次渡劫生效）</summary>
    public void ConsumeAscendPills(GroupMessageContext e, string args)
    {
        var config = CoreConfiguration.Current;
        var replies = config.Replies;
        var player = Player.Find(e.FromQQ.Id);
        if (player is null)
        {
            CommandHelper.Reply(e, replies.NoPlayer);
            return;
        }
        var countOrNull = CommandHelper.ParseInt(args, defaultToOne: true);
        if (countOrNull is null)
        {
            CommandHelper.Reply(e, CommandHelper.InvalidParams($"，示例：{config.CommandConsumeAscendPill} 数量"));
            return;
        }
        int count = Math.Clamp(countOrNull.Value, 1, config.MaxAscendPillConsume);
        int pills = InventoryItem.CountOf(player, ItemId.AscendPill);
        if (pills < count)
        {
            CommandHelper.Reply(e, string.Format(replies.ItemLeak, ItemCatalog.Definition(ItemId.AscendPill)!.Name, count, pills));
            return;
        }
        player.AscendPillComsume = count;
        player.Save();
        CommandHelper.Reply(e, string.Format(replies.ConsumeAscendPill, count, count * config.AscendSuccessPerPillPercent));
    }

    // ---- 复活/放生/查询 ----

    public void Resurrect(GroupMessageContext e, string args)
    {
        var config = CoreConfiguration.Current;
        var replies = config.Replies;
        var idOrNull = CommandHelper.ParseInt(args);
        if (idOrNull is null)
        {
            CommandHelper.Reply(e, CommandHelper.InvalidParams($"，示例：{config.CommandResurrect} ID"));
            return;
        }
        var player = Player.Find(e.FromQQ.Id);
        if (player is null)
        {
            CommandHelper.Reply(e, replies.NoPlayer);
            return;
        }
        if (KunQuery.ActiveKunOf(player.QQ) is not null)
        {
            CommandHelper.Reply(e, replies.DuplicateResurrect);
            return;
        }
        var kun = KunQuery.ById(idOrNull.Value);
        if (kun is null)
        {
            CommandHelper.Reply(e, replies.TargetKunNotFound);
            return;
        }
        if (kun.PlayerID != player.QQ)
        {
            CommandHelper.Reply(e, string.Format(replies.KunNotOwned, replies.ResurrectFailed));
            return;
        }
        if (kun.Abandoned)
        {
            CommandHelper.Reply(e, string.Format(replies.KunAbandoned, ""));
            return;
        }
        if (kun.Alive)
        {
            CommandHelper.Reply(e, string.Format(replies.KunAlive, ""));
            return;
        }
        double deadHours = (DateTime.Now - kun.DeadAt).TotalHours;
        if (deadHours >= config.MaxResurrectHours)
        {
            CommandHelper.Reply(e, string.Format(replies.ResurrectHourLimit, config.MaxResurrectHours, (int)deadHours));
            return;
        }
        int cost = kun.ResurrectCount + 1;
        if (!InventoryItem.TryRemove(player, ItemId.ResurrectPill, cost, out int remaining))
        {
            CommandHelper.Reply(e, string.Format(replies.ItemLeak, ItemCatalog.Definition(ItemId.ResurrectPill)!.Name, cost, remaining));
            return;
        }
        kun.LoadAffixes();
        var result = kun.Resurrect();
        if (result.Success)
        {
            CommandHelper.Reply(e, string.Format(
                replies.ResurrectSuccess, kun.DeadAt.ToString("G"), result.ResurrectCount,
                result.WeightLoss.ToShortNumber(), result.LevelLoss, cost, remaining));
        }
        else
        {
            CommandHelper.Reply(e, string.Format(replies.ResurrectFailed, cost, remaining));
        }
    }

    public void QueryDeadKuns(GroupMessageContext e)
    {
        var replies = CoreConfiguration.Current.Replies;
        var player = Player.Find(e.FromQQ.Id);
        if (player is null)
        {
            CommandHelper.Reply(e, replies.NoPlayer);
            return;
        }
        var builder = new System.Text.StringBuilder();
        foreach (var kun in KunQuery.ResurrectableOf(player))
        {
            kun.LoadAffixes();
            builder.AppendLine($"{kun.Id}. {kun}");
        }
        CommandHelper.Reply(e, replies.DeadKunsHeader + builder);
    }

    public void Release(GroupMessageContext e)
    {
        var replies = CoreConfiguration.Current.Replies;
        if (!CommandHelper.TryLoadPlayerAndKun(e, out _, out var kun))
        {
            return;
        }
        if (CommandHelper.IsBusyReplyIfSo(e, kun))
        {
            return;
        }
        CommandHelper.Reply(e, kun.Release()
            ? string.Format(replies.ReleaseSuccess, kun.ToString())
            : replies.ReleaseFailed);
    }
}