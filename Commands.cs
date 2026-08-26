using Another_Mirai_Native.Abstractions;
using Another_Mirai_Native.Abstractions.Attributes;
using Another_Mirai_Native.Abstractions.Context;
using Another_Mirai_Native.Abstractions.Enums;
using Another_Mirai_Native.Abstractions.Models;
using me.cqp.luohuaming.iKun.PublicInfos;
using me.cqp.luohuaming.iKun.PublicInfos.Enums;
using me.cqp.luohuaming.iKun.PublicInfos.Items;
using me.cqp.luohuaming.iKun.PublicInfos.Models;
using System.Text;
using System.Text.RegularExpressions;

namespace me.cqp.luohuaming.iKun;

using Items = PublicInfos.Models.Items;
using ItemEnum = PublicInfos.Enums.Items;

public class Commands : CommandHandlerBase
{
    #region 指令模板（从 AppConfig 动态读取，支持配置热重载）

    // 说明：DynamicCommand 的成员名指向本类属性，属性 Getter 转发 AppConfig 的当前值，
    // 配置文件热重载后无需重启即可生效。
    // DynamicCommand 的成员名指向本类属性，属性 Getter 每次调度时读取 AppConfig 当前值，
    // 因此配置热重载后指令触发词立即生效。
    // 生成的正则形如 ^[＃#]<命令>(?<args>...)$，命名组 args 即指令参数。

    public string TRegister => BuildPattern(AppConfig.CommandRegister, false);
    public string TLogin => BuildPattern(AppConfig.CommandLogin, false);
    public string TMenu => BuildPattern(AppConfig.CommandMenu, false);
    public string TRanking => BuildPattern(AppConfig.CommandRanking, false);
    public string TRankingGroup => BuildPattern(AppConfig.CommandRankingGroup, false);
    public string TInventory => BuildPattern(AppConfig.CommandInventory, false);
    public string THatch => BuildPattern(AppConfig.CommandHatch, true);
    public string TFeed => BuildPattern(AppConfig.CommandFeed, true);
    public string TUpgrade => BuildPattern(AppConfig.CommandUpgrade, true);
    public string TTransmogrify => BuildPattern(AppConfig.CommandTransmogrify, false);
    public string TQueryDeadKuns => BuildPattern(AppConfig.CommandQueryDeadKuns, false);
    public string TAscend => BuildPattern(AppConfig.CommandAscend, false);
    public string TResurrect => BuildPattern(AppConfig.CommandResurrect, true);
    public string TReleaseKun => BuildPattern(AppConfig.CommandReleaseKun, false);
    public string TDevour => BuildPattern(AppConfig.CommandDevour, true);
    public string TAttack => BuildPattern(AppConfig.CommandAttack, true);
    public string TShopping => BuildPattern(AppConfig.CommandShopping, true);
    public string TOpenEgg => BuildPattern(AppConfig.CommandOpenEgg, true);
    public string TOpenBlindBox => BuildPattern(AppConfig.CommandOpenBlindBox, true);
    public string TStartAutoPlay => BuildPattern(AppConfig.CommandStartAutoPlay, true);
    public string TStopAutoPlay => BuildPattern(AppConfig.CommandStopAutoPlay, false);
    public string TStartWorking => BuildPattern(AppConfig.CommandStartWorking, true);
    public string TStopWorking => BuildPattern(AppConfig.CommandStopWorking, false);
    public string TRandomPunish => BuildPattern(AppConfig.CommandRandomPunish, false);
    public string TConsumeAscendPill => BuildPattern(AppConfig.CommandConsumeAscendPill, true);
    public string TUseNickName => BuildPattern(AppConfig.CommandUseCustomNickName, true);
    public string TUnuseNickName => BuildPattern(AppConfig.CommandUnuseCustomNickName, false);
    public string TMyKun => BuildPattern(AppConfig.CommandMyKun, false);
    public string TUseItem => BuildPattern(AppConfig.CommandUseItem, true);

    private static readonly Dictionary<string, string> PatternCache = [];

    /// <summary>
    /// 生成指令匹配正则：^[＃#]<命令>[ \t]*(?<args>...)\s*$
    /// </summary>
    /// <param name="template">配置中的指令（以 # 开头）</param>
    /// <param name="withArgs">是否捕获参数命名组</param>
    private static string BuildPattern(string template, bool withArgs)
    {
        string key = $"{template}|{withArgs}";
        if (PatternCache.TryGetValue(key, out var cached))
        {
            return cached;
        }
        var sb = new StringBuilder("^");
        foreach (var ch in template)
        {
            sb.Append(ch == '#' ? @"[＃#]" : Regex.Escape(ch.ToString()));
        }
        if (withArgs)
        {
            sb.Append(@"[ \t]*(?<args>.*?)\s*$");
        }
        else
        {
            sb.Append(@"[ \t]*$");
        }
        cached = sb.ToString();
        PatternCache[key] = cached;
        return cached;
    }

    /// <summary>
    /// 解析指令后的整数参数
    /// </summary>
    /// <param name="raw">指令后文本</param>
    /// <param name="defaultToOne">无参数或非法时返回 1（原版行为），否则返回 null</param>
    private static int? ParseIntParam(string raw, bool defaultToOne = false)
    {
        raw = raw?.Trim();
        if (string.IsNullOrEmpty(raw))
        {
            return defaultToOne ? 1 : null;
        }
        if (int.TryParse(raw, out int v))
        {
            return v;
        }
        return defaultToOne ? 1 : null;
    }

    /// <summary>
    /// 指令格式错误回复
    /// </summary>
    private static string ParamInvalid(string extra) => string.Format(AppConfig.ReplyParamInvalid, extra);

    /// <summary>
    /// 群白名单检查
    /// </summary>
    private static bool GroupEnabled(GroupMessageContext e) => AppConfig.Groups.Any(x => x == e.FromGroup.Id);

    #endregion

    #region 基础功能

    [DynamicCommand(nameof(TRegister), MatchMode.Regex, MessageScope.Group)]
    public Task<EventHandleResult> Register(GroupMessageContext e)
    {
        if (!GroupEnabled(e))
        {
            return Task.FromResult(EventHandleResult.Pass);
        }
        if (Player.Exists(e.FromQQ.Id))
        {
            e.SendMessage(AppConfig.ReplyDuplicateRegister);
            return Task.FromResult(EventHandleResult.Block);
        }
        var player = Player.Create(e.FromQQ.Id);
        if (player != null)
        {
            int coinCount = AppConfig.ValueRegisterCoinReward;
            int eggCount = AppConfig.ValueRegisterEggReward;
            player.GiveItem([Items.Coin(coinCount), Items.KunEgg(eggCount)]);

            e.SendMessage(string.Format(AppConfig.ReplyNewRegister, coinCount, eggCount));
        }
        else
        {
            e.SendMessage(AppConfig.ReplyRegisterFailed);
        }
        return Task.FromResult(EventHandleResult.Block);
    }

    [DynamicCommand(nameof(TLogin), MatchMode.Regex, MessageScope.Group)]
    public Task<EventHandleResult> Login(GroupMessageContext e)
    {
        if (!GroupEnabled(e))
        {
            return Task.FromResult(EventHandleResult.Pass);
        }
        var player = Player.GetPlayer(e.FromQQ.Id);
        if (player == null)
        {
            e.SendMessage(AppConfig.ReplyNoPlayer);
            return Task.FromResult(EventHandleResult.Block);
        }
        if (CommonHelper.IsSameDay(player.LoginAt, DateTime.Now))
        {
            e.SendMessage(AppConfig.ReplyDuplicateLogin);
            return Task.FromResult(EventHandleResult.Block);
        }
        player.LoginAt = DateTime.Now;
        player.Update();
        int coinCount = AppConfig.ValueLoginCoinReward;
        int eggCount = AppConfig.ValueLoginEggReward;
        player.GiveItem([Items.Coin(coinCount), Items.KunEgg(eggCount)]);

        e.SendMessage(string.Format(AppConfig.ReplyLoginReward, coinCount, eggCount));
        return Task.FromResult(EventHandleResult.Block);
    }

    [DynamicCommand(nameof(TMenu), MatchMode.Regex, MessageScope.Group)]
    public Task<EventHandleResult> Menu(GroupMessageContext e)
    {
        if (!GroupEnabled(e))
        {
            return Task.FromResult(EventHandleResult.Pass);
        }
        e.SendMessage(string.Format(AppConfig.ReplyMenu, AppConfig.CommandRegister, AppConfig.CommandLogin
            , AppConfig.CommandFeed, AppConfig.CommandUpgrade
            , AppConfig.CommandHatch, AppConfig.CommandInventory
            , AppConfig.CommandShopping, AppConfig.CommandOpenBlindBox
            , AppConfig.CommandOpenEgg, AppConfig.CommandTransmogrify
            , AppConfig.CommandAttack, AppConfig.CommandDevour
            , AppConfig.CommandQueryDeadKuns, AppConfig.CommandReleaseKun
            , AppConfig.CommandResurrect, AppConfig.CommandRanking
            , AppConfig.CommandAscend, AppConfig.CommandMenu));
        return Task.FromResult(EventHandleResult.Block);
    }

    [DynamicCommand(nameof(TMyKun), MatchMode.Regex, MessageScope.Group)]
    public Task<EventHandleResult> MyKun(GroupMessageContext e)
    {
        if (!GroupEnabled(e))
        {
            return Task.FromResult(EventHandleResult.Pass);
        }
        var player = Player.GetPlayer(e.FromQQ.Id);
        if (player == null)
        {
            e.SendMessage(AppConfig.ReplyNoPlayer);
            return Task.FromResult(EventHandleResult.Block);
        }
        var kun = Kun.GetKunByQQ(player.QQ);
        if (kun == null)
        {
            e.SendMessage(AppConfig.ReplyNoKun);
        }
        else
        {
            kun.Initialize();
            e.SendMessage(kun.ToStringFull(true));
        }
        return Task.FromResult(EventHandleResult.Block);
    }

    #endregion

    #region 排行与查询

    [DynamicCommand(nameof(TRanking), MatchMode.Regex, MessageScope.Group)]
    public Task<EventHandleResult> Ranking(GroupMessageContext e)
    {
        if (!GroupEnabled(e))
        {
            return Task.FromResult(EventHandleResult.Pass);
        }
        StringBuilder stringBuilder = new();
        stringBuilder.AppendLine(AppConfig.ReplyRankingHeader);
        var kuns = Kun.GetRankingKun(AppConfig.ValueRankingCount);
        for (int i = 0; i < kuns.Count; i++)
        {
            kuns[i].Initialize();
            var record = Record.GetRecordByKunID(kuns[i].Id);
            if (record == null)
            {
                continue;
            }
            try
            {
                var info = MainSave.API.GroupApi.GetGroupMemberInfo(record.Group, record.QQ);
                bool autoPlaying = AutoPlay.CheckKunAutoPlay(kuns[i], AutoPlayType.Exp);
                bool working = AutoPlay.CheckKunAutoPlay(kuns[i], AutoPlayType.Coin);
                stringBuilder.AppendLine($"{i + 1}. [{info.Card ?? info.Nick}] {kuns[i]} {kuns[i].Weight.ToShortNumber()}" +
                    $" {AppConfig.WeightUnit}" +
                    $"{(autoPlaying ? $" {AppConfig.ReplyRankingAutoPlaying}" : "")}" +
                    $"{(working ? $" {AppConfig.ReplyRankingWorking}" : "")}");
            }
            catch (Exception exc)
            {
                MainSave.API.Logger.Info("获取成员名片", $"获取失败，群={record.Group}，QQ={record.QQ}\n{exc.Message}，{exc.StackTrace}");
                continue;
            }
        }
        stringBuilder.RemoveNewLine();

        e.SendMessage(stringBuilder.ToString());
        return Task.FromResult(EventHandleResult.Block);
    }

    [DynamicCommand(nameof(TRankingGroup), MatchMode.Regex, MessageScope.Group)]
    public Task<EventHandleResult> RankingGroup(GroupMessageContext e)
    {
        if (!GroupEnabled(e))
        {
            return Task.FromResult(EventHandleResult.Pass);
        }
        StringBuilder stringBuilder = new();
        stringBuilder.AppendLine(AppConfig.ReplyRankingGroupHeader);
        var memberList = e.FromGroup.GetGroupMemberList();
        if (memberList == null || memberList.Count == 0)
        {
            e.SendMessage("获取群成员列表失败");
            return Task.FromResult(EventHandleResult.Block);
        }
        var records = Record.GetRecordsByQQList(memberList.Select(x => x.QQ).ToList());
        var kuns = Kun.GetKunByRecords(records).OrderByDescending(x => x.Weight).ToList();
        for (int i = 0; i < Math.Min(AppConfig.ValueRankingCount, kuns.Count); i++)
        {
            kuns[i].Initialize();
            try
            {
                var info = e.FromGroup.GetGroupMemberInfo(kuns[i].PlayerID);
                bool autoPlaying = AutoPlay.CheckKunAutoPlay(kuns[i], AutoPlayType.Exp);
                bool working = AutoPlay.CheckKunAutoPlay(kuns[i], AutoPlayType.Coin);
                stringBuilder.AppendLine($"{i + 1}. [{CommonHelper.GetMemberDisplayName(info)}] {kuns[i]} {kuns[i].Weight.ToShortNumber()}" +
                    $" {AppConfig.WeightUnit}" +
                    $"{(autoPlaying ? $" {AppConfig.ReplyRankingAutoPlaying}" : "")}" +
                    $"{(working ? $" {AppConfig.ReplyRankingWorking}" : "")}");
            }
            catch (Exception exc)
            {
                MainSave.API.Logger.Info("获取成员名片", $"获取失败，群={e.FromGroup.Id}，QQ={kuns[i].PlayerID}\n{exc.Message}，{exc.StackTrace}");
                continue;
            }
        }
        stringBuilder.RemoveNewLine();

        e.SendMessage(stringBuilder.ToString());
        return Task.FromResult(EventHandleResult.Block);
    }

    [DynamicCommand(nameof(TQueryDeadKuns), MatchMode.Regex, MessageScope.Group)]
    public Task<EventHandleResult> QueryDeadKuns(GroupMessageContext e)
    {
        if (!GroupEnabled(e))
        {
            return Task.FromResult(EventHandleResult.Pass);
        }
        var player = Player.GetPlayer(e.FromQQ.Id);
        if (player == null)
        {
            e.SendMessage(AppConfig.ReplyNoPlayer);
            return Task.FromResult(EventHandleResult.Block);
        }
        var list = Kun.GetDeadKun(player);
        StringBuilder stringBuilder = new();
        foreach (var item in list)
        {
            item.Initialize();
            stringBuilder.AppendLine($"{item.Id}. " + item.ToString());
        }
        e.SendMessage(AppConfig.ReplyQueryDeadKun + stringBuilder.ToString());
        return Task.FromResult(EventHandleResult.Block);
    }

    [DynamicCommand(nameof(TRandomPunish), MatchMode.Regex, MessageScope.Group)]
    public Task<EventHandleResult> RandomPunishQuery(GroupMessageContext e)
    {
        if (!GroupEnabled(e))
        {
            return Task.FromResult(EventHandleResult.Pass);
        }
        DayOfWeek dayOfWeek = AppConfig.ValueRandomPunishExecuteDay == 7 ? DayOfWeek.Sunday
                             : (DayOfWeek)AppConfig.ValueRandomPunishExecuteDay;
        string dayOfWeekString = dayOfWeek switch
        {
            DayOfWeek.Sunday => "周日",
            DayOfWeek.Monday => "周一",
            DayOfWeek.Tuesday => "周二",
            DayOfWeek.Wednesday => "周三",
            DayOfWeek.Thursday => "周四",
            DayOfWeek.Friday => "周五",
            DayOfWeek.Saturday => "周六",
        };
        e.SendMessage(string.Format(AppConfig.ReplyRandomPunish, dayOfWeekString, RandomPunish.TargetTime.ToString("G")));
        return Task.FromResult(EventHandleResult.Block);
    }

    #endregion

    #region 孵化、喂养、强化、幻化

    [DynamicCommand(nameof(THatch), MatchMode.Regex, MessageScope.Group)]
    public Task<EventHandleResult> Hatch(GroupMessageContext e, string args)
    {
        if (!GroupEnabled(e))
        {
            return Task.FromResult(EventHandleResult.Pass);
        }
        var countOrNull = ParseIntParam(args, true);
        if (countOrNull == null)
        {
            e.SendMessage(ParamInvalid($"，示例：{AppConfig.CommandHatch} 数量"));
            return Task.FromResult(EventHandleResult.Block);
        }
        int count = Math.Max(1, countOrNull.Value);

        var player = Player.GetPlayer(e.FromQQ.Id);
        if (player == null)
        {
            e.SendMessage(AppConfig.ReplyNoPlayer);
            return Task.FromResult(EventHandleResult.Block);
        }
        var kun = Kun.GetKunByQQ(player.QQ);
        if (kun != null)
        {
            e.SendMessage(AppConfig.ReplyDuplicateHatch);
            return Task.FromResult(EventHandleResult.Block);
        }
        int hatchConsume = count;
        if (!InventoryItem.TryRemoveItem(player, Items.KunEgg().ID, hatchConsume, out int currentCount))
        {
            e.SendMessage(string.Format(AppConfig.ReplyItemLeak, Items.KunEgg().Name, hatchConsume, currentCount));
            return Task.FromResult(EventHandleResult.Block);
        }
        int hatchSuccess = CommonHelper.Random.Next(AppConfig.ValueHatchProbablityMin, AppConfig.ValueHatchProbablityMax);
        int consume = 0;
        bool success = false;
        for (consume = 1; consume <= count; consume++)
        {
            if (CommonHelper.Random.Next(100) > hatchSuccess)
            {
                continue;
            }
            success = true;
            break;
        }
        if (!success)
        {
            e.SendMessage(string.Format(AppConfig.ReplyHatchFail, currentCount));
            return Task.FromResult(EventHandleResult.Block);
        }
        int diff = count - consume;
        if (diff > 0)
        {
            player.GiveItem([Items.KunEgg(diff)]);
        }

        kun = Kun.RandomCreate(player);
        kun.Initialize();
        int id = Kun.SaveKun(kun);

        Record.AddRecord(new Record { Group = e.FromGroup.Id, QQ = e.FromQQ.Id, KunID = id });
        if (count > 1)
        {
            e.SendMessage(string.Format(AppConfig.ReplyMultiHatchKun, kun.ToString(), kun.Weight.ToShortNumber(), consume, currentCount + diff));
        }
        else
        {
            e.SendMessage(string.Format(AppConfig.ReplyHatchKun, kun.ToString(), kun.Weight.ToShortNumber(), currentCount));
        }
        return Task.FromResult(EventHandleResult.Block);
    }

    [DynamicCommand(nameof(TFeed), MatchMode.Regex, MessageScope.Group)]
    public Task<EventHandleResult> Feed(GroupMessageContext e, string args)
    {
        if (!GroupEnabled(e))
        {
            return Task.FromResult(EventHandleResult.Pass);
        }
        var player = Player.GetPlayer(e.FromQQ.Id);
        if (player == null)
        {
            e.SendMessage(AppConfig.ReplyNoPlayer);
            return Task.FromResult(EventHandleResult.Block);
        }
        var kun = Kun.GetKunByQQ(player.QQ);
        if (kun == null)
        {
            e.SendMessage(AppConfig.ReplyNoKun);
            return Task.FromResult(EventHandleResult.Block);
        }
        kun.Initialize();
        if (AutoPlay.CheckKunAutoPlay(kun))
        {
            e.SendMessage(string.Format(AppConfig.ReplyAutoPlaying, kun));
            return Task.FromResult(EventHandleResult.Block);
        }

        var countOrNull = ParseIntParam(args, true);
        if (countOrNull == null)
        {
            e.SendMessage(ParamInvalid($"，示例：{AppConfig.CommandFeed} 数量"));
            return Task.FromResult(EventHandleResult.Block);
        }
        int count = Math.Max(1, countOrNull.Value);
        // 体重达到上限后拒绝喂养。
        if (kun.Weight < Kun.GetLevelWeightLimit(kun.Level))
        {
            int currentCoin = InventoryItem.GetItemCount(player, ItemEnum.Coin);
            int currentEgg = InventoryItem.GetItemCount(player, ItemEnum.KunEgg);
            if (currentCoin < count * AppConfig.ValueFeedCoinConsume)
            {
                e.SendMessage(string.Format(AppConfig.ReplyItemLeak, Items.Coin().Name, count * AppConfig.ValueFeedCoinConsume, currentCoin));
                return Task.FromResult(EventHandleResult.Block);
            }

            if (currentEgg < count * AppConfig.ValueFeedKunEggConsume)
            {
                e.SendMessage(string.Format(AppConfig.ReplyItemLeak, Items.KunEgg().Name, count * AppConfig.ValueFeedKunEggConsume, currentEgg));
                return Task.FromResult(EventHandleResult.Block);
            }
            InventoryItem.TryRemoveItem(player, ItemEnum.Coin, count * AppConfig.ValueFeedCoinConsume, out currentCoin);
            InventoryItem.TryRemoveItem(player, ItemEnum.KunEgg, count * AppConfig.ValueFeedKunEggConsume, out currentEgg);

            var r = kun.Feed(count);
            StringBuilder stringBuilder = new();
            stringBuilder.AppendLine(string.Format(AppConfig.ReplyFeed, kun.ToString(), r.Increment.ToShortNumber(), r.CurrentWeight.ToShortNumber(), currentCoin, currentEgg));
            if (r.WeightLimit)
            {
                stringBuilder.AppendLine(AppConfig.ReplyWeightLimit);
            }
            stringBuilder.RemoveNewLine();

            e.SendMessage(stringBuilder.ToString());
        }
        else
        {
            e.SendMessage(AppConfig.ReplyWeightLimit);
        }
        return Task.FromResult(EventHandleResult.Block);
    }

    [DynamicCommand(nameof(TUpgrade), MatchMode.Regex, MessageScope.Group)]
    public Task<EventHandleResult> Upgrade(GroupMessageContext e, string args)
    {
        if (!GroupEnabled(e))
        {
            return Task.FromResult(EventHandleResult.Pass);
        }
        var player = Player.GetPlayer(e.FromQQ.Id);
        if (player == null)
        {
            e.SendMessage(AppConfig.ReplyNoPlayer);
            return Task.FromResult(EventHandleResult.Block);
        }
        var kun = Kun.GetKunByQQ(player.QQ);
        if (kun == null)
        {
            e.SendMessage(AppConfig.ReplyNoKun);
            return Task.FromResult(EventHandleResult.Block);
        }
        kun.Initialize();
        if (AutoPlay.CheckKunAutoPlay(kun))
        {
            e.SendMessage(string.Format(AppConfig.ReplyAutoPlaying, kun));
            return Task.FromResult(EventHandleResult.Block);
        }
        if (AutoPlay.CheckKunAutoPlay(kun, AutoPlayType.Coin))
        {
            e.SendMessage(string.Format(AppConfig.ReplyWorking, kun));
            return Task.FromResult(EventHandleResult.Block);
        }
        var countOrNull = ParseIntParam(args, true);
        if (countOrNull == null)
        {
            e.SendMessage(ParamInvalid($"，示例：{AppConfig.CommandUpgrade} 数量"));
            return Task.FromResult(EventHandleResult.Block);
        }
        int count = Math.Max(1, countOrNull.Value);
        int currentCoin = InventoryItem.GetItemCount(player, ItemEnum.Coin);
        int currentPill = InventoryItem.GetItemCount(player, ItemEnum.UpgradePill);
        if (currentCoin < count * AppConfig.ValueUpgradeCoinConsume)
        {
            e.SendMessage(string.Format(AppConfig.ReplyItemLeak, Items.Coin().Name, count * AppConfig.ValueUpgradeCoinConsume, currentCoin));
            return Task.FromResult(EventHandleResult.Block);
        }
        if (currentPill < count * AppConfig.ValueUpgradePillConsume)
        {
            e.SendMessage(string.Format(AppConfig.ReplyItemLeak, Items.UpgradePill().Name, count * AppConfig.ValueUpgradePillConsume, currentPill));
            return Task.FromResult(EventHandleResult.Block);
        }
        InventoryItem.TryRemoveItem(player, ItemEnum.Coin, count * AppConfig.ValueUpgradeCoinConsume, out currentCoin);
        InventoryItem.TryRemoveItem(player, ItemEnum.UpgradePill, count * AppConfig.ValueUpgradePillConsume, out currentPill);

        var upgradeResult = kun.Upgrade(count);
        if (upgradeResult.Success is false)
        {
            e.SendMessage("强化方法过程发生异常，查看日志获取更多信息");
            return Task.FromResult(EventHandleResult.Block);
        }
        StringBuilder stringBuilder = new();
        if (upgradeResult.Increment > 0)
        {
            stringBuilder.AppendLine(string.Format(AppConfig.ReplyUpgradeSuccess, upgradeResult.Increment.ToShortNumber(), upgradeResult.CurrentWeight.ToShortNumber(), currentPill, currentCoin));
        }
        else
        {
            stringBuilder.AppendLine(string.Format(AppConfig.ReplyUpgradeFail, upgradeResult.Increment.ToShortNumber(), upgradeResult.CurrentWeight.ToShortNumber(), currentPill, currentCoin));
        }
        if (upgradeResult.WeightLimit)
        {
            stringBuilder.AppendLine(AppConfig.ReplyWeightLimit);
        }

        e.SendMessage(stringBuilder.ToString());
        return Task.FromResult(EventHandleResult.Block);
    }

    [DynamicCommand(nameof(TTransmogrify), MatchMode.Regex, MessageScope.Group)]
    public Task<EventHandleResult> Transmogrify(GroupMessageContext e)
    {
        if (!GroupEnabled(e))
        {
            return Task.FromResult(EventHandleResult.Pass);
        }
        var player = Player.GetPlayer(e.FromQQ.Id);
        if (player == null)
        {
            e.SendMessage(AppConfig.ReplyNoPlayer);
            return Task.FromResult(EventHandleResult.Block);
        }
        var kun = Kun.GetKunByQQ(player.QQ);
        if (kun == null)
        {
            e.SendMessage(AppConfig.ReplyNoKun);
            return Task.FromResult(EventHandleResult.Block);
        }
        if (kun.Level < AppConfig.ValueTransmogrifyLevelLimit)
        {
            e.SendMessage(string.Format(AppConfig.ReplyTransmogrifyLevelLimit, kun.Level, AppConfig.ValueTransmogrifyLevelLimit));
            return Task.FromResult(EventHandleResult.Block);
        }
        kun.Initialize();
        if (AutoPlay.CheckKunAutoPlay(kun))
        {
            e.SendMessage(string.Format(AppConfig.ReplyAutoPlaying, kun));
            return Task.FromResult(EventHandleResult.Block);
        }
        if (AutoPlay.CheckKunAutoPlay(kun, AutoPlayType.Coin))
        {
            e.SendMessage(string.Format(AppConfig.ReplyWorking, kun));
            return Task.FromResult(EventHandleResult.Block);
        }
        int currentCoin = InventoryItem.GetItemCount(player, ItemEnum.Coin);
        int currentPill = InventoryItem.GetItemCount(player, ItemEnum.TransmogrifyPill);
        if (currentCoin < AppConfig.ValueTranmogifyCoinConsume)
        {
            e.SendMessage(string.Format(AppConfig.ReplyItemLeak, Items.Coin().Name, AppConfig.ValueTranmogifyCoinConsume, currentCoin));
            return Task.FromResult(EventHandleResult.Block);
        }
        if (currentPill < AppConfig.ValueTranmogifyPillConsume)
        {
            e.SendMessage(string.Format(AppConfig.ReplyItemLeak, Items.TransmogrifyPill().Name, AppConfig.ValueTranmogifyPillConsume, currentPill));
            return Task.FromResult(EventHandleResult.Block);
        }
        InventoryItem.TryRemoveItem(player, ItemEnum.Coin, AppConfig.ValueTranmogifyCoinConsume, out currentCoin);
        InventoryItem.TryRemoveItem(player, ItemEnum.TransmogrifyPill, AppConfig.ValueTranmogifyPillConsume, out currentPill);

        var r = kun.Transmogrify();
        if (r.Success is false)
        {
            e.SendMessage("幻化方法过程发生异常，查看日志获取更多信息");
            return Task.FromResult(EventHandleResult.Block);
        }
        if (r.Dead)
        {
            e.SendMessage(string.Format(AppConfig.ReplyTransmogrifyFailAndDead, currentPill, currentCoin));
        }
        else if (r.CurrentAttributeA.ID == r.OriginalAttributeA.ID && r.CurrentAttributeB.AttrbiuteBID == r.OriginalAttributeB.AttrbiuteBID && r.OriginalAttributeC.AttrbiuteBID == r.CurrentAttributeC.AttrbiuteBID)
        {
            e.SendMessage(string.Format(AppConfig.ReplyTransmogrifyFail, r.Decrement.ToShortNumber(), r.CurrentWeight.ToShortNumber(), currentPill, currentCoin));
        }
        else
        {
            e.SendMessage(string.Format(AppConfig.ReplyTransmogrifySuccess, $"[{r.OriginalAttributeA.Name}]{r.OriginalAttributeB.Name}{r.OriginalAttributeC.Name}鲲", $"[{r.CurrentAttributeA.Name}]{r.CurrentAttributeB.Name}{r.CurrentAttributeC.Name}鲲", r.Decrement.ToShortNumber(), r.CurrentWeight.ToShortNumber(), currentPill, currentCoin));
        }
        return Task.FromResult(EventHandleResult.Block);
    }

    #endregion

    #region 渡劫、复活、放生

    [DynamicCommand(nameof(TAscend), MatchMode.Regex, MessageScope.Group)]
    public Task<EventHandleResult> Ascend(GroupMessageContext e)
    {
        if (!GroupEnabled(e))
        {
            return Task.FromResult(EventHandleResult.Pass);
        }
        var player = Player.GetPlayer(e.FromQQ.Id);
        if (player == null)
        {
            e.SendMessage(AppConfig.ReplyNoPlayer);
            return Task.FromResult(EventHandleResult.Block);
        }
        if (player.AscendPillComsume > 0)
        {
            int currentCount = InventoryItem.GetItemCount(player, ItemEnum.AscendPill);
            if (currentCount < player.AscendPillComsume)
            {
                e.SendMessage(string.Format(AppConfig.ReplyItemLeak, Items.AscendPill().Name, player.AscendPillComsume, currentCount));
                return Task.FromResult(EventHandleResult.Block);
            }
        }

        var kun = Kun.GetKunByQQ(player.QQ);
        if (kun == null)
        {
            e.SendMessage(AppConfig.ReplyNoKun);
            return Task.FromResult(EventHandleResult.Block);
        }
        kun.Initialize();

        kun.AscendProbablityIncrement = player.AscendPillComsume * AppConfig.ValueAscendPillPerIncrement;

        if (AutoPlay.CheckKunAutoPlay(kun))
        {
            e.SendMessage(string.Format(AppConfig.ReplyAutoPlaying, kun));
            return Task.FromResult(EventHandleResult.Block);
        }
        if (AutoPlay.CheckKunAutoPlay(kun, AutoPlayType.Coin))
        {
            e.SendMessage(string.Format(AppConfig.ReplyWorking, kun));
            return Task.FromResult(EventHandleResult.Block);
        }
        if (kun.Weight < Kun.GetLevelWeightLimit(kun.Level))
        {
            e.SendMessage(string.Format(AppConfig.ReplyAscendNoWeightLimit, kun.Weight.ToShortNumber(), Kun.GetLevelWeightLimit(kun.Level).ToShortNumber()));
            return Task.FromResult(EventHandleResult.Block);
        }
        if (!InventoryItem.TryRemoveItem(player, Items.Coin().ID, AppConfig.ValueAscendCoinConsume, out int currentCoin))
        {
            e.SendMessage(string.Format(AppConfig.ReplyItemLeak, Items.Coin().Name, AppConfig.ValueAscendCoinConsume, currentCoin));
            return Task.FromResult(EventHandleResult.Block);
        }
        var r = kun.Ascend();
        if (r.Success is false)
        {
            e.SendMessage("渡劫方法过程发生异常，查看日志获取更多信息");
            return Task.FromResult(EventHandleResult.Block);
        }
        if (player.AscendPillComsume > 0)
        {
            if (!InventoryItem.TryRemoveItem(player, ItemEnum.AscendPill, player.AscendPillComsume, out int currentPill))
            {
                e.SendMessage(string.Format(AppConfig.ReplyItemLeak, Items.AscendPill().Name, player.AscendPillComsume, currentPill));
                return Task.FromResult(EventHandleResult.Block);
            }
            player.AscendPillComsume = 0;
            player.Update();
        }
        if (r.Dead)
        {
            e.SendMessage(AppConfig.ReplyAscendFailAndDead);
        }
        else if (r.Increment > 0)
        {
            e.SendMessage(string.Format(AppConfig.ReplyAscendSuccess, r.Increment.ToShortNumber(), r.CurrentWeight.ToShortNumber(), r.CurrentLevel));
        }
        else
        {
            e.SendMessage(string.Format(AppConfig.ReplyAscendFail, r.Increment.ToShortNumber(), r.CurrentWeight.ToShortNumber()));
        }
        return Task.FromResult(EventHandleResult.Block);
    }

    [DynamicCommand(nameof(TResurrect), MatchMode.Regex, MessageScope.Group)]
    public Task<EventHandleResult> Resurrect(GroupMessageContext e, string args)
    {
        if (!GroupEnabled(e))
        {
            return Task.FromResult(EventHandleResult.Pass);
        }
        var idOrNull = ParseIntParam(args);
        if (idOrNull == null)
        {
            e.SendMessage(ParamInvalid($"，示例：{AppConfig.CommandResurrect} ID"));
            return Task.FromResult(EventHandleResult.Block);
        }
        var player = Player.GetPlayer(e.FromQQ.Id);
        if (player == null)
        {
            e.SendMessage(AppConfig.ReplyNoPlayer);
            return Task.FromResult(EventHandleResult.Block);
        }
        var kun = Kun.GetKunByQQ(player.QQ);
        if (kun != null)
        {
            e.SendMessage(AppConfig.ReplyDuplicateResurrect);
            return Task.FromResult(EventHandleResult.Block);
        }

        kun = Kun.GetKunByID(idOrNull.Value);
        if (kun == null)
        {
            e.SendMessage(AppConfig.ReplyNoTargrtKun);
            return Task.FromResult(EventHandleResult.Block);
        }
        if (kun.PlayerID != player.QQ)
        {
            e.SendMessage(string.Format(AppConfig.ReplyKunOwnerNotMatch, AppConfig.ReplyResurrectFailed));
            return Task.FromResult(EventHandleResult.Block);
        }
        if (kun.Abandoned)
        {
            e.SendMessage(string.Format(AppConfig.ReplyKunAbandoned, AppConfig.ReplyResurrectFailed));
            return Task.FromResult(EventHandleResult.Block);
        }
        if (kun.Alive)
        {
            e.SendMessage(string.Format(AppConfig.ReplyKunAlive, AppConfig.ReplyResurrectFailed));
            return Task.FromResult(EventHandleResult.Block);
        }
        if ((DateTime.Now - kun.DeadAt).TotalHours >= AppConfig.ValueMaxResurrectHour)
        {
            e.SendMessage(string.Format(AppConfig.ReplyResurrectHourLimit, AppConfig.ValueMaxResurrectHour, (int)(DateTime.Now - kun.DeadAt).TotalHours));
            return Task.FromResult(EventHandleResult.Block);
        }
        int consume = kun.ResurrectCount + 1;
        if (!InventoryItem.TryRemoveItem(player, ItemEnum.ResurrectPill, consume, out int currentCount))
        {
            e.SendMessage(string.Format(AppConfig.ReplyItemLeak, Items.ResurrectPill().Name, consume, currentCount));
            return Task.FromResult(EventHandleResult.Block);
        }

        kun.Initialize();
        var r = kun.Resurrect();
        if (r.Success)
        {
            e.SendMessage(string.Format(AppConfig.ReplyResurrectSuccess, kun.DeadAt.ToString("G"), r.CurrentResurrectCount, r.WeightLoss.ToShortNumber(), r.LevelLoss, consume, currentCount));
        }
        else
        {
            e.SendMessage(string.Format(AppConfig.ReplyResurrectFail, consume, currentCount));
        }
        return Task.FromResult(EventHandleResult.Block);
    }

    [DynamicCommand(nameof(TReleaseKun), MatchMode.Regex, MessageScope.Group)]
    public Task<EventHandleResult> ReleaseKun(GroupMessageContext e)
    {
        if (!GroupEnabled(e))
        {
            return Task.FromResult(EventHandleResult.Pass);
        }
        var player = Player.GetPlayer(e.FromQQ.Id);
        if (player == null)
        {
            e.SendMessage(AppConfig.ReplyNoPlayer);
            return Task.FromResult(EventHandleResult.Block);
        }
        var kun = Kun.GetKunByQQ(player.QQ);
        if (kun == null)
        {
            e.SendMessage(AppConfig.ReplyNoKun);
            return Task.FromResult(EventHandleResult.Block);
        }
        kun.Initialize();
        if (AutoPlay.CheckKunAutoPlay(kun))
        {
            e.SendMessage(string.Format(AppConfig.ReplyAutoPlaying, kun));
            return Task.FromResult(EventHandleResult.Block);
        }
        if (AutoPlay.CheckKunAutoPlay(kun, AutoPlayType.Coin))
        {
            e.SendMessage(string.Format(AppConfig.ReplyWorking, kun));
            return Task.FromResult(EventHandleResult.Block);
        }
        if (kun.Release())
        {
            e.SendMessage(string.Format(AppConfig.ReplyReleaseSuccess, kun.ToString()));
        }
        else
        {
            e.SendMessage(AppConfig.ReplyReleaseFail);
        }
        return Task.FromResult(EventHandleResult.Block);
    }

    #endregion

    #region 攻击、吞噬

    /// <summary>
    /// 从消息链或文本中解析目标 QQ（支持 At / QQ号 / 昵称 / 群名片）
    /// </summary>
    private static long ResolveTarget(GroupMessageContext e, string args)
    {
        foreach (var item in e.Message.MessageChain)
        {
            if (item is Another_Mirai_Native.Abstractions.Models.MessageItem.At at && !at.AllTarget)
            {
                return at.Target;
            }
        }
        args = args?.Trim();
        if (string.IsNullOrEmpty(args))
        {
            return -1;
        }
        if (long.TryParse(args, out long target))
        {
            return target;
        }
        // 按昵称与卡片匹配
        List<GroupMemberInfo> infos;
        if (!MainSave.GroupMemberInfos.TryGetValue(e.FromGroup.Id, out infos) || infos is null || infos.Count == 0)
        {
            infos = MainSave.API.GroupApi.GetGroupMembers(e.FromGroup.Id);
            MainSave.GroupMemberInfos[e.FromGroup.Id] = infos;
        }
        var info = infos.FirstOrDefault(x => (x.Nick?.Contains(args) ?? false) || (x.Card?.Contains(args) ?? false));
        return info?.QQ ?? -1;
    }

    private Task<EventHandleResult> AttackOrDevourCore(GroupMessageContext e, string args, bool isAttack)
    {
        if (!GroupEnabled(e))
        {
            return Task.FromResult(EventHandleResult.Pass);
        }
        string commandStr = isAttack ? AppConfig.CommandAttack : AppConfig.CommandDevour;

        long target = ResolveTarget(e, args);
        if (target < 10000)
        {
            e.SendMessage(ParamInvalid($"或无法找到目标，示例：{commandStr} [QQ|At|昵称|卡片]"));
            return Task.FromResult(EventHandleResult.Block);
        }
        if (target == e.FromQQ.Id)
        {
            e.SendMessage(isAttack ? AppConfig.ReplyAttackSelf : AppConfig.ReplyDevourSelf);
            return Task.FromResult(EventHandleResult.Block);
        }

        var player = Player.GetPlayer(e.FromQQ.Id);
        if (player == null)
        {
            e.SendMessage(AppConfig.ReplyNoPlayer);
            return Task.FromResult(EventHandleResult.Block);
        }
        var targetPlayer = Player.GetPlayer(target);
        if (targetPlayer == null)
        {
            e.SendMessage(AppConfig.ReplyNoTargetPlayer);
            return Task.FromResult(EventHandleResult.Block);
        }
        var kun = Kun.GetKunByQQ(player.QQ);
        if (kun == null)
        {
            e.SendMessage(AppConfig.ReplyNoKun);
            return Task.FromResult(EventHandleResult.Block);
        }
        kun.Initialize();
        if (AutoPlay.CheckKunAutoPlay(kun))
        {
            e.SendMessage(string.Format(AppConfig.ReplyAutoPlaying, kun));
            return Task.FromResult(EventHandleResult.Block);
        }
        if (AutoPlay.CheckKunAutoPlay(kun, AutoPlayType.Coin))
        {
            e.SendMessage(string.Format(AppConfig.ReplyWorking, kun));
            return Task.FromResult(EventHandleResult.Block);
        }
        var targetKun = Kun.GetKunByQQ(targetPlayer.QQ);
        if (targetKun == null)
        {
            e.SendMessage(AppConfig.ReplyNoTargetPlayerKun);
            return Task.FromResult(EventHandleResult.Block);
        }
        targetKun.Initialize();
        if (AutoPlay.CheckKunAutoPlay(targetKun))
        {
            e.SendMessage(string.Format(AppConfig.ReplyAutoPlaying, targetKun));
            return Task.FromResult(EventHandleResult.Block);
        }

        double cdMinutes = isAttack ? AppConfig.ValueAttackCD : AppConfig.ValueDevourCD;
        DateTime lastActionAt = isAttack ? player.AttackAt : player.DevourAt;
        if (DateTime.Now - lastActionAt < TimeSpan.FromMinutes(cdMinutes))
        {
            e.SendMessage(string.Format(isAttack ? AppConfig.ReplyAttackInCD : AppConfig.ReplyDevourInCD, lastActionAt.AddMinutes(cdMinutes).ToString("G")));
            return Task.FromResult(EventHandleResult.Block);
        }

        bool sameGroup = CommonHelper.CheckSameGroup(MainSave.API, target, e.FromGroup.Id);
        long notSameGroupId = 0;
        if (!sameGroup)
        {
            var record = Record.GetRecordByKunID(targetKun.Id);
            if (record != null)
            {
                notSameGroupId = record.Group;
            }
        }
        string playerInfo;
        if (AppConfig.EnableAt)
        {
            playerInfo = CommonHelper.CQCode_At(target);
        }
        else
        {
            var info = sameGroup
                ? e.FromGroup.GetGroupMemberInfo(target)
                : (notSameGroupId > 0 ? MainSave.API.GroupApi.GetGroupMemberInfo(notSameGroupId, target) : null);
            playerInfo = string.IsNullOrWhiteSpace(info?.Card) ? target.ToString() : info.Card;
        }

        if (isAttack)
        {
            var r = kun.Attack(targetKun);
            if (r.Success is false)
            {
                e.SendMessage("攻击方法过程发生异常，查看日志获取更多信息");
                return Task.FromResult(EventHandleResult.Block);
            }
            player.AttackAt = DateTime.Now;
            player.Update();
            if (r.Dead)
            {
                e.SendMessage(string.Format(AppConfig.ReplyAttackFailAndDead, kun.ToString(), playerInfo, targetKun.ToString(), r.TargetDecrement.ToShortNumber(), r.TargetCurrentWeight.ToShortNumber()));
            }
            else if (r.TargetDead)
            {
                e.SendMessage(string.Format(AppConfig.ReplyAttackSuccessAndTargetDead, kun.ToString(), playerInfo, targetKun.ToString(), r.Increment.ToShortNumber(), r.CurrentWeight.ToShortNumber()));
            }
            else if (r.Increment < 0)
            {
                e.SendMessage(string.Format(AppConfig.ReplyAttackFail, kun.ToString(), playerInfo, targetKun.ToString(), r.Increment.ToShortNumber(), r.CurrentWeight.ToShortNumber(), r.TargetDecrement.ToShortNumber(), r.TargetCurrentWeight.ToShortNumber()));
            }
            else if (r.Escaped)
            {
                e.SendMessage(string.Format(AppConfig.ReplyAttackEscaped, kun.ToString(), playerInfo, targetKun.ToString()));
            }
            else
            {
                string send = string.Format(AppConfig.ReplyAttackSuccess, kun.ToString(), playerInfo, targetKun.ToString(), r.Increment.ToShortNumber(), r.CurrentWeight.ToShortNumber(), r.TargetDecrement.ToShortNumber(), r.TargetCurrentWeight.ToShortNumber());
                if (r.WeightLimit)
                {
                    send += $"\n{AppConfig.ReplyWeightLimit}";
                }
                e.SendMessage(send);
            }

            if (!sameGroup && AppConfig.EnableNotSameGroupAttackBoardcast)
            {
                if (r.Escaped && AppConfig.EnableNotSameGroupAttackEscapeBoardcast)
                {
                    MainSave.API.MessageApi.SendGroupMessage(notSameGroupId, string.Format(AppConfig.ReplyAttackedNotSameGroupButEscaped, CommonHelper.CQCode_At(target)));
                }
                else if (r.TargetDead)
                {
                    MainSave.API.MessageApi.SendGroupMessage(notSameGroupId, string.Format(AppConfig.ReplyAttackedNotSameGroupAndDead, CommonHelper.CQCode_At(target)));
                }
                else if (r.Increment > 0)
                {
                    double loss = r.TargetDecrement / (r.TargetCurrentWeight + r.TargetDecrement) * 100;
                    if (loss > AppConfig.ValueNotSameGroupNoticeMinimalPercent)
                    {
                        MainSave.API.MessageApi.SendGroupMessage(notSameGroupId, string.Format(AppConfig.ReplyAttackedNotSameGroup, CommonHelper.CQCode_At(target), r.TargetDecrement.ToShortNumber(), r.TargetCurrentWeight.ToShortNumber()));
                    }
                }
            }
        }
        else
        {
            var r = kun.Devour(targetKun);
            if (r.Success is false)
            {
                e.SendMessage("吞噬方法过程发生异常，查看日志获取更多信息");
                return Task.FromResult(EventHandleResult.Block);
            }
            player.DevourAt = DateTime.Now;
            player.Update();
            if (r.Dead)
            {
                e.SendMessage(string.Format(AppConfig.ReplyDevourFailAndDead, kun.ToString(), playerInfo, targetKun.ToString(), r.TargetDecrement.ToShortNumber(), r.TargetCurrentWeight.ToShortNumber()));
            }
            else if (r.Increment < 0)
            {
                e.SendMessage(string.Format(AppConfig.ReplyDevourFail, kun.ToString(), playerInfo, targetKun.ToString(), r.Increment.ToShortNumber(), r.CurrentWeight.ToShortNumber(), r.TargetDecrement.ToShortNumber(), r.TargetCurrentWeight.ToShortNumber()));
            }
            else if (r.Escaped)
            {
                e.SendMessage(string.Format(AppConfig.ReplyDevourEscaped, kun.ToString(), playerInfo, targetKun.ToString()));
            }
            else
            {
                string send = string.Format(AppConfig.ReplyDevourSuccess, kun.ToString(), playerInfo, targetKun.ToString(), r.Increment.ToShortNumber(), r.CurrentWeight.ToShortNumber(), r.TargetDecrement.ToShortNumber(), r.TargetCurrentWeight.ToShortNumber());
                if (r.WeightLimit)
                {
                    send += $"\n{AppConfig.ReplyWeightLimit}";
                }
                e.SendMessage(send);
            }

            if (!sameGroup && AppConfig.EnableNotSameGroupDevourBoardcast)
            {
                if (r.Escaped)
                {
                    MainSave.API.MessageApi.SendGroupMessage(notSameGroupId, string.Format(AppConfig.ReplyDevouredNotSameGroupButEscaped, CommonHelper.CQCode_At(target)));
                }
                else if (r.Increment > 0)
                {
                    MainSave.API.MessageApi.SendGroupMessage(notSameGroupId, string.Format(AppConfig.ReplyDevouredNotSameGroup, CommonHelper.CQCode_At(target)));
                }
            }
        }
        return Task.FromResult(EventHandleResult.Block);
    }

    [DynamicCommand(nameof(TAttack), MatchMode.Regex, MessageScope.Group)]
    public Task<EventHandleResult> Attack(GroupMessageContext e, string args) => AttackOrDevourCore(e, args, true);

    [DynamicCommand(nameof(TDevour), MatchMode.Regex, MessageScope.Group)]
    public Task<EventHandleResult> Devour(GroupMessageContext e, string args) => AttackOrDevourCore(e, args, false);

    #endregion

    #region 商店与物品

    private static List<(Items, Items)> ShoppingList { get; set; } = [];

    private static void GetShoppingList()
    {
        ShoppingList = [];
        foreach (var trade in AppConfig.ShoppingList)
        {
            string[] split = trade.Split('|');
            int count = int.TryParse(split.ElementAtOrDefault(0), out int intValue) ? intValue : -1;
            int itemIndex = int.TryParse(split.ElementAtOrDefault(1), out intValue) ? intValue : -1;
            int price = int.TryParse(split.ElementAtOrDefault(2), out intValue) ? intValue : -1;
            int coinIndex = int.TryParse(split.ElementAtOrDefault(3), out intValue) ? intValue : -1;
            if (count > 0 && itemIndex > 0 && itemIndex <= CommonHelper.GetMaxItemValue() && price > 0 && coinIndex > 0 && coinIndex <= CommonHelper.GetMaxItemValue())
            {
                var item = Items.GetItemByID((ItemEnum)itemIndex);
                item.Count = count;
                var coin = Items.GetItemByID((ItemEnum)coinIndex);
                coin.Count = price;
                ShoppingList.Add((item, coin));
            }
        }
    }

    [DynamicCommand(nameof(TShopping), MatchMode.Regex, MessageScope.Group)]
    public Task<EventHandleResult> Shopping(GroupMessageContext e, string args)
    {
        if (!GroupEnabled(e))
        {
            return Task.FromResult(EventHandleResult.Pass);
        }
        var param = (args ?? "").Trim().Split([' '], StringSplitOptions.RemoveEmptyEntries);
        if (param.Length < 1)
        {
            GetShoppingList();
            StringBuilder stringBuilder = new();
            stringBuilder.AppendLine(AppConfig.ReplyShoppingHeader);
            for (int i = 1; i <= ShoppingList.Count; i++)
            {
                var (item, coin) = ShoppingList[i - 1];
                stringBuilder.AppendLine(AppConfig.ReplyShoppingDetail
                    .Replace("%ItemName%", item.Name)
                    .Replace("%ItemCount%", $"{item.Count}")
                    .Replace("%ItemDesc%", item.Description)
                    .Replace("%CoinName%", coin.Name)
                    .Replace("%CoinCount%", $"{coin.Count}")
                    .Replace("%CoinDesc%", coin.Description)
                    .Replace("%Index%", $"{i}"));
            }
            stringBuilder.Append($"示例：{AppConfig.CommandShopping} 序号 数量");
            e.SendMessage(stringBuilder.ToString());
            return Task.FromResult(EventHandleResult.Block);
        }
        if (param.Length != 2 || !int.TryParse(param[0], out int index) || !int.TryParse(param[1], out int count))
        {
            e.SendMessage(ParamInvalid($"，示例：{AppConfig.CommandShopping} 序号 数量"));
            return Task.FromResult(EventHandleResult.Block);
        }
        count = Math.Max(1, count);

        var player = Player.GetPlayer(e.FromQQ.Id);
        if (player == null)
        {
            e.SendMessage(AppConfig.ReplyNoPlayer);
            return Task.FromResult(EventHandleResult.Block);
        }

        GetShoppingList();
        if (index >= 1 && index <= ShoppingList.Count)
        {
            var (item, coin) = ShoppingList[index - 1];

            if (1.0 * count * coin.Count > int.MaxValue)
            {
                e.SendMessage(ParamInvalid(""));
                return Task.FromResult(EventHandleResult.Block);
            }
            int comsume = count * coin.Count;
            if (!InventoryItem.TryRemoveItem(player, coin.ID, comsume, out int currentCount))
            {
                e.SendMessage(string.Format(AppConfig.ReplyItemLeak, coin.Name, comsume, currentCount));
                return Task.FromResult(EventHandleResult.Block);
            }
            item.Count *= count;
            player.GiveItem([item]);
            e.SendMessage(string.Format(AppConfig.ReplyBuyItem, comsume, coin.Name, item.Count, item.Name, currentCount, InventoryItem.GetItemCount(player, item.ID)));
            return Task.FromResult(EventHandleResult.Block);
        }

        e.SendMessage(AppConfig.ReplyItemCannotBuy);
        return Task.FromResult(EventHandleResult.Block);
    }

    private static List<(ItemEnum, double)> BlindBoxes { get; set; } = null;

    private static List<ItemEnum> GetBlindBox(bool redraw = false)
    {
        List<ItemEnum> result = [];

        double probablityTotal = BlindBoxes.Sum(x => x.Item2);
        double random = probablityTotal * CommonHelper.Random.NextDouble();
        double p = 0;

        if (redraw && AppConfig.BlindBoxMultiContentMustHasItem)
        {
            probablityTotal = BlindBoxes.Where(x => x.Item1 != ItemEnum.Nothing).Sum(x => x.Item2);
            random = probablityTotal * CommonHelper.Random.NextDouble();
            foreach (var item in BlindBoxes.Where(x => x.Item1 != ItemEnum.Nothing))
            {
                p += item.Item2;
                if (random < p)
                {
                    result.Add(item.Item1);
                    break;
                }
            }
        }
        else
        {
            foreach (var item in BlindBoxes)
            {
                p += item.Item2;
                if (random < p)
                {
                    result.Add(item.Item1);
                    break;
                }
            }
        }

        if (AppConfig.BlindBoxEnableMultiContents && CommonHelper.Random.NextDouble() < AppConfig.BlindBoxMultiContentProbablity / 100.0)
        {
            result = [.. result, .. GetBlindBox(true)];
        }

        return result;
    }

    private static void BuildBlindBox()
    {
        BlindBoxes = [];
        foreach (var item in AppConfig.BlindBoxContents)
        {
            string[] split = item.Split('|');
            int index = int.TryParse(split.ElementAtOrDefault(0), out int value) ? value : -1;
            double probablity = double.TryParse(split.ElementAtOrDefault(1), out double doubleValue) ? doubleValue : -1;
            if (index >= 0 && index <= CommonHelper.GetMaxItemValue())
            {
                BlindBoxes.Add(((ItemEnum)index, probablity));
            }
        }
    }

    [DynamicCommand(nameof(TOpenBlindBox), MatchMode.Regex, MessageScope.Group)]
    public Task<EventHandleResult> OpenBlindBox(GroupMessageContext e, string args)
    {
        if (!GroupEnabled(e))
        {
            return Task.FromResult(EventHandleResult.Pass);
        }
        var countOrNull = ParseIntParam(args);
        if (countOrNull == null)
        {
            e.SendMessage(ParamInvalid($"，示例：{AppConfig.CommandOpenBlindBox} 数量"));
            return Task.FromResult(EventHandleResult.Block);
        }
        int count = Math.Max(1, countOrNull.Value);
        var player = Player.GetPlayer(e.FromQQ.Id);
        if (player == null)
        {
            e.SendMessage(AppConfig.ReplyNoPlayer);
            return Task.FromResult(EventHandleResult.Block);
        }
        int consume = count;
        if (!InventoryItem.TryRemoveItem(player, ItemEnum.BlindBox, consume, out int currentCount))
        {
            e.SendMessage(string.Format(AppConfig.ReplyItemLeak, Items.BlindBox().Name, consume, currentCount));
            return Task.FromResult(EventHandleResult.Block);
        }
        BuildBlindBox();

        List<Items> contents = [];
        StringBuilder stringBuilder = new();
        for (int i = 0; i < count; i++)
        {
            var blindBox = GetBlindBox();
            foreach (var item in blindBox)
            {
                if (item == ItemEnum.Nothing)
                {
                    continue;
                }
                contents.Add(Items.GetItemByID(item));
            }
        }
        foreach (var item in contents.GroupBy(x => x.Name, (key, groups) => new { Key = key, Count = groups.ToList().Count }))
        {
            stringBuilder.AppendLine("· " + item.Key + $" {item.Count} 个");
        }
        stringBuilder.RemoveNewLine();
        if (contents.Count > 0)
        {
            player.GiveItem(contents);
            e.SendMessage(string.Format(AppConfig.ReplyBlindBoxOpen, consume, stringBuilder.ToString()));
        }
        else
        {
            e.SendMessage(string.Format(AppConfig.ReplyBlindBoxGetNothing, consume));
        }
        return Task.FromResult(EventHandleResult.Block);
    }

    [DynamicCommand(nameof(TOpenEgg), MatchMode.Regex, MessageScope.Group)]
    public Task<EventHandleResult> OpenEgg(GroupMessageContext e, string args)
    {
        if (!GroupEnabled(e))
        {
            return Task.FromResult(EventHandleResult.Pass);
        }
        var countOrNull = ParseIntParam(args);
        if (countOrNull == null)
        {
            e.SendMessage(ParamInvalid($"，示例：{AppConfig.CommandOpenEgg} 数量"));
            return Task.FromResult(EventHandleResult.Block);
        }
        int count = Math.Max(1, countOrNull.Value);
        var player = Player.GetPlayer(e.FromQQ.Id);
        if (player == null)
        {
            e.SendMessage(AppConfig.ReplyNoPlayer);
            return Task.FromResult(EventHandleResult.Block);
        }
        int currentCoin = InventoryItem.GetItemCount(player, ItemEnum.Coin);
        int currentEgg = InventoryItem.GetItemCount(player, ItemEnum.KunEgg);
        if (currentCoin < count * 10)
        {
            e.SendMessage(string.Format(AppConfig.ReplyItemLeak, Items.Coin().Name, count * 10, currentCoin));
            return Task.FromResult(EventHandleResult.Block);
        }

        if (currentEgg < count)
        {
            e.SendMessage(string.Format(AppConfig.ReplyItemLeak, Items.KunEgg().Name, count, currentEgg));
            return Task.FromResult(EventHandleResult.Block);
        }
        InventoryItem.TryRemoveItem(player, ItemEnum.Coin, count * 10, out currentCoin);
        InventoryItem.TryRemoveItem(player, ItemEnum.KunEgg, count, out currentEgg);
        player.GiveItem([Items.BlindBox(count * AppConfig.ValueKunEggToBlindBoxRate)]);

        e.SendMessage(string.Format(AppConfig.ReplyOpenKunEgg, count, count * AppConfig.ValueKunEggToBlindBoxRate));
        return Task.FromResult(EventHandleResult.Block);
    }

    [DynamicCommand(nameof(TUseItem), MatchMode.Regex, MessageScope.Group)]
    public Task<EventHandleResult> UseItem(GroupMessageContext e, string args)
    {
        if (!GroupEnabled(e))
        {
            return Task.FromResult(EventHandleResult.Pass);
        }
        var param = (args ?? "").Trim().Split([' '], StringSplitOptions.RemoveEmptyEntries);
        if (param.Length != 1 && param.Length != 2)
        {
            e.SendMessage(ParamInvalid($"，示例：{AppConfig.CommandUseItem} 物品ID/名称 数量"));
            return Task.FromResult(EventHandleResult.Block);
        }

        int count = 1;
        Items items;
        if ((items = Items.GetItemByName(param[0])) == null
            && (!int.TryParse(param[0], out int value)
                || (items = Items.GetItemByID((ItemEnum)value)) == null))
        {
            e.SendMessage(ParamInvalid("，指定的物品 ID 或名称无效"));
            return Task.FromResult(EventHandleResult.Block);
        }
        if (param.Length == 2)
        {
            count = int.TryParse(param[1], out value) ? value : count;
            count = Math.Max(count, 1);
        }
        if (!items.Usable)
        {
            e.SendMessage(AppConfig.ReplyItemCannotUse);
            return Task.FromResult(EventHandleResult.Block);
        }
        var player = Player.GetPlayer(e.FromQQ.Id);
        if (player == null)
        {
            e.SendMessage(AppConfig.ReplyNoPlayer);
            return Task.FromResult(EventHandleResult.Block);
        }
        var kun = Kun.GetKunByQQ(player.QQ);
        if (kun == null)
        {
            e.SendMessage(AppConfig.ReplyNoKun);
            return Task.FromResult(EventHandleResult.Block);
        }
        kun.Initialize();
        if (AutoPlay.CheckKunAutoPlay(kun))
        {
            e.SendMessage(string.Format(AppConfig.ReplyAutoPlaying, kun));
            return Task.FromResult(EventHandleResult.Block);
        }
        if (AutoPlay.CheckKunAutoPlay(kun, AutoPlayType.Coin))
        {
            e.SendMessage(string.Format(AppConfig.ReplyWorking, kun));
            return Task.FromResult(EventHandleResult.Block);
        }

        if (!InventoryItem.TryRemoveItem(player, items.ID, count, out var currentItem))
        {
            e.SendMessage(string.Format(AppConfig.ReplyItemLeak, items.Name, count, currentItem));
            return Task.FromResult(EventHandleResult.Block);
        }
        var reply = items.UseItem(count, player, kun);
        if (!reply.Item1)
        {
            e.SendMessage(AppConfig.ReplyItemUseFailed);
            items.Count = count;
            player.GiveItem([items]);
            return Task.FromResult(EventHandleResult.Block);
        }
        e.SendMessage(reply.Item2);
        return Task.FromResult(EventHandleResult.Block);
    }

    [DynamicCommand(nameof(TConsumeAscendPill), MatchMode.Regex, MessageScope.Group)]
    public Task<EventHandleResult> ConsumeAscendPill(GroupMessageContext e, string args)
    {
        if (!GroupEnabled(e))
        {
            return Task.FromResult(EventHandleResult.Pass);
        }
        var player = Player.GetPlayer(e.FromQQ.Id);
        if (player == null)
        {
            e.SendMessage(AppConfig.ReplyNoPlayer);
            return Task.FromResult(EventHandleResult.Block);
        }

        var countOrNull = ParseIntParam(args);
        if (countOrNull == null)
        {
            e.SendMessage(ParamInvalid($"，示例：{AppConfig.CommandConsumeAscendPill} 数量"));
            return Task.FromResult(EventHandleResult.Block);
        }
        int count = Math.Max(1, countOrNull.Value);
        count = Math.Min(AppConfig.ValueAscendPillMaxConsumeCount, count);
        int currentPill = InventoryItem.GetItemCount(player, ItemEnum.AscendPill);
        if (currentPill < count)
        {
            e.SendMessage(string.Format(AppConfig.ReplyItemLeak, Items.AscendPill().Name, count, currentPill));
            return Task.FromResult(EventHandleResult.Block);
        }
        player.AscendPillComsume = count;
        player.Update();

        e.SendMessage(string.Format(AppConfig.ReplyConsumeAscendPill, count, count * AppConfig.ValueAscendPillPerIncrement));
        return Task.FromResult(EventHandleResult.Block);
    }

    [DynamicCommand(nameof(TInventory), MatchMode.Regex, MessageScope.Group)]
    public Task<EventHandleResult> Inventory(GroupMessageContext e, string args)
    {
        if (!GroupEnabled(e))
        {
            return Task.FromResult(EventHandleResult.Pass);
        }
        var player = Player.GetPlayer(e.FromQQ.Id);
        if (player == null)
        {
            e.SendMessage(AppConfig.ReplyNoPlayer);
            return Task.FromResult(EventHandleResult.Block);
        }
        StringBuilder stringBuilder = new();
        var kun = Kun.GetKunByQQ(player.QQ);
        if (kun == null)
        {
            stringBuilder.AppendLine(AppConfig.ReplyNoKun);
        }
        else
        {
            kun.Initialize();
            stringBuilder.AppendLine(kun.ToStringFull(false));
        }
        stringBuilder.AppendLine("----");
        var list = InventoryItem.GetItemsByQQ(e.FromQQ.Id).Where(x => x.Count > 0).ToList();
        if (list == null || list.Count == 0)
        {
            stringBuilder.AppendLine(AppConfig.ReplyEmptyInventory);
        }
        else
        {
            foreach (var item in list)
            {
                var items = Items.GetItemByID((ItemEnum)item.ItemID);
                if (items == null)
                {
                    continue;
                }
                stringBuilder.AppendLine(item.ToString());
            }
        }
        stringBuilder.RemoveNewLine();
        e.SendMessage(stringBuilder.ToString());
        return Task.FromResult(EventHandleResult.Block);
    }

    #endregion

    #region 挂机与打工

    [DynamicCommand(nameof(TStartAutoPlay), MatchMode.Regex, MessageScope.Group)]
    public Task<EventHandleResult> StartAutoPlay(GroupMessageContext e, string args)
    {
        if (!GroupEnabled(e))
        {
            return Task.FromResult(EventHandleResult.Pass);
        }
        var durationOrNull = ParseIntParam(args);
        if (durationOrNull == null)
        {
            e.SendMessage(ParamInvalid($"，示例：{AppConfig.CommandStartAutoPlay} 整数小时"));
            return Task.FromResult(EventHandleResult.Block);
        }
        int duration = Math.Max(1, durationOrNull.Value);
        if (duration > AppConfig.ValueMaxAutoPlayDuration)
        {
            e.SendMessage(ParamInvalid($"，参数最大为 {AppConfig.ValueMaxAutoPlayDuration}"));
            return Task.FromResult(EventHandleResult.Block);
        }
        var player = Player.GetPlayer(e.FromQQ.Id);
        if (player == null)
        {
            e.SendMessage(AppConfig.ReplyNoPlayer);
            return Task.FromResult(EventHandleResult.Block);
        }
        var kun = Kun.GetKunByQQ(player.QQ);
        if (kun == null)
        {
            e.SendMessage(AppConfig.ReplyNoKun);
            return Task.FromResult(EventHandleResult.Block);
        }
        if (!kun.Alive)
        {
            e.SendMessage(string.Format(AppConfig.ReplyKunNotAlive, AppConfig.ReplyStartAutoPlayFailed));
            return Task.FromResult(EventHandleResult.Block);
        }
        if (kun.Abandoned)
        {
            e.SendMessage(string.Format(AppConfig.ReplyKunAbandoned, AppConfig.ReplyStartAutoPlayFailed));
            return Task.FromResult(EventHandleResult.Block);
        }
        if (kun.Weight == Kun.GetLevelWeightLimit(kun.Level))
        {
            e.SendMessage(AppConfig.ReplyWeightLimit);
            return Task.FromResult(EventHandleResult.Block);
        }
        kun.Initialize();
        if (AutoPlay.CheckKunAutoPlay(kun, AutoPlayType.Exp))
        {
            e.SendMessage(string.Format(AppConfig.ReplyAutoPlaying, kun));
            return Task.FromResult(EventHandleResult.Block);
        }
        if (AutoPlay.CheckKunAutoPlay(kun, AutoPlayType.Coin))
        {
            e.SendMessage(string.Format(AppConfig.ReplyWorking, kun));
            return Task.FromResult(EventHandleResult.Block);
        }
        if (!AutoPlay.CheckAutoPlayInCD(kun, AutoPlayType.Exp, out DateTime availableTime))
        {
            e.SendMessage(string.Format(AppConfig.ReplyAutoPlayInCD, availableTime.ToString("G")));
            return Task.FromResult(EventHandleResult.Block);
        }
        var start = DateTime.Now;
        var end = start.AddHours(duration);
        var autoPlay = new AutoPlay
        {
            Duration = duration,
            GroupId = e.FromGroup.Id,
            KunID = kun.Id,
            StartTime = start,
            EndTime = end,
            AutoPlayType = AutoPlayType.Exp
        };
        AutoPlay.AddAutoPlay(autoPlay);
        var exp = AutoPlay.CalcAutoPlayExp(kun.Level, start, end);
        e.SendMessage(string.Format(AppConfig.ReplyAutoPlayStarted, end.ToString("G"), exp.ToShortNumber()));
        return Task.FromResult(EventHandleResult.Block);
    }

    [DynamicCommand(nameof(TStopAutoPlay), MatchMode.Regex, MessageScope.Group)]
    public Task<EventHandleResult> StopAutoPlay(GroupMessageContext e)
    {
        if (!GroupEnabled(e))
        {
            return Task.FromResult(EventHandleResult.Pass);
        }
        var player = Player.GetPlayer(e.FromQQ.Id);
        if (player == null)
        {
            e.SendMessage(AppConfig.ReplyNoPlayer);
            return Task.FromResult(EventHandleResult.Block);
        }
        var kun = Kun.GetKunByQQ(player.QQ);
        if (kun == null)
        {
            e.SendMessage(AppConfig.ReplyNoKun);
            return Task.FromResult(EventHandleResult.Block);
        }
        if (!kun.Alive)
        {
            e.SendMessage(string.Format(AppConfig.ReplyKunNotAlive, AppConfig.ReplyStartAutoPlayFailed));
            return Task.FromResult(EventHandleResult.Block);
        }
        if (kun.Abandoned)
        {
            e.SendMessage(string.Format(AppConfig.ReplyKunAbandoned, AppConfig.ReplyStartAutoPlayFailed));
            return Task.FromResult(EventHandleResult.Block);
        }
        kun.Initialize();
        if (!AutoPlay.CheckKunAutoPlay(kun, AutoPlayType.Exp))
        {
            e.SendMessage(string.Format(AppConfig.ReplyNotAutoPlaying, kun));
            return Task.FromResult(EventHandleResult.Block);
        }

        var autoPlay = AutoPlay.GetKunAutoPlay(kun, AutoPlayType.Exp);
        if (autoPlay == null)
        {
            e.SendMessage(string.Format(AppConfig.ReplyNotAutoPlaying, kun));
            return Task.FromResult(EventHandleResult.Block);
        }
        var r = autoPlay.Stop();
        if (r == null)
        {
            e.SendMessage(string.Format(AppConfig.ReplyKunNotAlive, kun));
            return Task.FromResult(EventHandleResult.Block);
        }
        string msg;
        if (r.Dead)
        {
            msg = string.Format(AppConfig.ReplyAutoPlayFinishedButDead, kun, r.Duration.TotalHours.ToString("f2"), r.Increment.ToShortNumber());
        }
        else
        {
            msg = string.Format(AppConfig.ReplyAutoPlayFinished, kun, r.Duration.TotalHours.ToString("f2"), r.Increment.ToShortNumber(), r.CurrentWeight.ToShortNumber());
            if (r.WeightLimit)
            {
                msg += $"\n{AppConfig.ReplyWeightLimit}";
            }
        }
        e.SendMessage(msg);
        return Task.FromResult(EventHandleResult.Block);
    }

    [DynamicCommand(nameof(TStartWorking), MatchMode.Regex, MessageScope.Group)]
    public Task<EventHandleResult> StartWorking(GroupMessageContext e, string args)
    {
        if (!GroupEnabled(e))
        {
            return Task.FromResult(EventHandleResult.Pass);
        }
        var durationOrNull = ParseIntParam(args);
        if (durationOrNull == null)
        {
            e.SendMessage(ParamInvalid($"，示例：{AppConfig.CommandStartWorking} 整数小时"));
            return Task.FromResult(EventHandleResult.Block);
        }
        int duration = Math.Max(1, durationOrNull.Value);
        if (duration > AppConfig.ValueMaxAutoPlayDuration)
        {
            e.SendMessage(ParamInvalid($"，参数最大为 {AppConfig.ValueMaxAutoPlayDuration}"));
            return Task.FromResult(EventHandleResult.Block);
        }
        var player = Player.GetPlayer(e.FromQQ.Id);
        if (player == null)
        {
            e.SendMessage(AppConfig.ReplyNoPlayer);
            return Task.FromResult(EventHandleResult.Block);
        }
        var kun = Kun.GetKunByQQ(player.QQ);
        if (kun == null)
        {
            e.SendMessage(AppConfig.ReplyNoKun);
            return Task.FromResult(EventHandleResult.Block);
        }
        if (!kun.Alive)
        {
            e.SendMessage(string.Format(AppConfig.ReplyKunNotAlive, AppConfig.ReplyStartAutoPlayFailed));
            return Task.FromResult(EventHandleResult.Block);
        }
        if (kun.Abandoned)
        {
            e.SendMessage(string.Format(AppConfig.ReplyKunAbandoned, AppConfig.ReplyStartAutoPlayFailed));
            return Task.FromResult(EventHandleResult.Block);
        }
        kun.Initialize();
        if (AutoPlay.CheckKunAutoPlay(kun, AutoPlayType.Coin))
        {
            e.SendMessage(string.Format(AppConfig.ReplyWorking, kun));
            return Task.FromResult(EventHandleResult.Block);
        }
        if (AutoPlay.CheckKunAutoPlay(kun, AutoPlayType.Exp))
        {
            e.SendMessage(string.Format(AppConfig.ReplyAutoPlaying, kun));
            return Task.FromResult(EventHandleResult.Block);
        }
        if (!AutoPlay.CheckAutoPlayInCD(kun, AutoPlayType.Coin, out DateTime availableTime))
        {
            e.SendMessage(string.Format(AppConfig.ReplyWorkingInCD, availableTime.ToString("G")));
            return Task.FromResult(EventHandleResult.Block);
        }
        var start = DateTime.Now;
        var end = start.AddHours(duration);
        var autoPlay = new AutoPlay
        {
            Duration = duration,
            GroupId = e.FromGroup.Id,
            KunID = kun.Id,
            StartTime = start,
            EndTime = end,
            AutoPlayType = AutoPlayType.Coin
        };
        AutoPlay.AddAutoPlay(autoPlay);
        int increment = (int)AutoPlay.CalcAutoPlayCoin(kun.Level, start, end);
        e.SendMessage(string.Format(AppConfig.ReplyWorkingStarted, end.ToString("G"), increment));
        return Task.FromResult(EventHandleResult.Block);
    }

    [DynamicCommand(nameof(TStopWorking), MatchMode.Regex, MessageScope.Group)]
    public Task<EventHandleResult> StopWorking(GroupMessageContext e)
    {
        if (!GroupEnabled(e))
        {
            return Task.FromResult(EventHandleResult.Pass);
        }
        var player = Player.GetPlayer(e.FromQQ.Id);
        if (player == null)
        {
            e.SendMessage(AppConfig.ReplyNoPlayer);
            return Task.FromResult(EventHandleResult.Block);
        }
        var kun = Kun.GetKunByQQ(player.QQ);
        if (kun == null)
        {
            e.SendMessage(AppConfig.ReplyNoKun);
            return Task.FromResult(EventHandleResult.Block);
        }
        if (!kun.Alive)
        {
            e.SendMessage(string.Format(AppConfig.ReplyKunNotAlive, AppConfig.ReplyStartAutoPlayFailed));
            return Task.FromResult(EventHandleResult.Block);
        }
        if (kun.Abandoned)
        {
            e.SendMessage(string.Format(AppConfig.ReplyKunAbandoned, AppConfig.ReplyStartAutoPlayFailed));
            return Task.FromResult(EventHandleResult.Block);
        }
        kun.Initialize();
        if (!AutoPlay.CheckKunAutoPlay(kun, AutoPlayType.Coin))
        {
            e.SendMessage(string.Format(AppConfig.ReplyNotWorking, kun));
            return Task.FromResult(EventHandleResult.Block);
        }

        var autoPlay = AutoPlay.GetKunAutoPlay(kun, AutoPlayType.Coin);
        if (autoPlay == null)
        {
            e.SendMessage(string.Format(AppConfig.ReplyNotWorking, kun));
            return Task.FromResult(EventHandleResult.Block);
        }
        var r = autoPlay.Stop();
        if (r == null)
        {
            e.SendMessage(string.Format(AppConfig.ReplyKunNotAlive, kun));
            return Task.FromResult(EventHandleResult.Block);
        }
        string msg = string.Format(AppConfig.ReplyWorkingFinished, kun, r.Duration.TotalHours.ToString("f2"), (int)r.Increment, r.CurrentCoin);
        e.SendMessage(msg);
        return Task.FromResult(EventHandleResult.Block);
    }

    #endregion

    #region 自定义昵称

    [DynamicCommand(nameof(TUseNickName), MatchMode.Regex, MessageScope.Group)]
    public Task<EventHandleResult> UseNickName(GroupMessageContext e, string args)
    {
        if (!GroupEnabled(e))
        {
            return Task.FromResult(EventHandleResult.Pass);
        }
        string nickName = (args ?? "").Trim();
        if (string.IsNullOrEmpty(nickName))
        {
            e.SendMessage(ParamInvalid($"，示例：{AppConfig.CommandUseCustomNickName} 昵称"));
            return Task.FromResult(EventHandleResult.Block);
        }
        var player = Player.GetPlayer(e.FromQQ.Id);
        if (player == null)
        {
            e.SendMessage(AppConfig.ReplyNoPlayer);
            return Task.FromResult(EventHandleResult.Block);
        }

        var kun = Kun.GetKunByQQ(player.QQ);
        if (kun == null)
        {
            e.SendMessage(AppConfig.ReplyNoKun);
            return Task.FromResult(EventHandleResult.Block);
        }
        kun.Initialize();
        if (AppConfig.NickNameFilter.Any(x => nickName.ToLower().Contains(x)))
        {
            e.SendMessage(AppConfig.ReplyCustomNickInvalid);
            return Task.FromResult(EventHandleResult.Block);
        }
        kun.NickName = nickName;
        kun.Update();

        e.SendMessage(string.Format(AppConfig.ReplyCustomNickApplied, kun));
        return Task.FromResult(EventHandleResult.Block);
    }

    [DynamicCommand(nameof(TUnuseNickName), MatchMode.Regex, MessageScope.Group)]
    public Task<EventHandleResult> UnuseNickName(GroupMessageContext e)
    {
        if (!GroupEnabled(e))
        {
            return Task.FromResult(EventHandleResult.Pass);
        }
        var player = Player.GetPlayer(e.FromQQ.Id);
        if (player == null)
        {
            e.SendMessage(AppConfig.ReplyNoPlayer);
            return Task.FromResult(EventHandleResult.Block);
        }

        var kun = Kun.GetKunByQQ(player.QQ);
        if (kun == null)
        {
            e.SendMessage(AppConfig.ReplyNoKun);
            return Task.FromResult(EventHandleResult.Block);
        }
        kun.Initialize();
        kun.NickName = "";
        kun.Update();

        e.SendMessage(string.Format(AppConfig.ReplyCustomNickDiscarded, kun.ToString()));
        return Task.FromResult(EventHandleResult.Block);
    }

    #endregion
}
