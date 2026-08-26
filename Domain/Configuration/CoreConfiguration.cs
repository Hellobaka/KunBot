using me.cqp.luohuaming.iKun.Infrastructure.Json;

namespace me.cqp.luohuaming.iKun.Domain.Configuration;

/// <summary>
/// 核心配置：指令、回复文案、概率与数值参数。
/// 文件由 Infrastructure 的 JsonConfigFile 读写；本类只承载键名与默认值。
/// </summary>
public sealed partial class CoreConfiguration : JsonConfigFile
{
    public CoreConfiguration(string path) : base(path)
    {
        EnableAutoReload();
        Load();
    }

    /// <summary>热重载后的最新快照（调度侧读取）</summary>
    public static CoreConfiguration Current { get; private set; } = null!;

    internal static CoreConfiguration CreateCurrent(string path)
    {
        var instance = new CoreConfiguration(path);
        Current = instance;
        return instance;
    }

    // ---- 开关 ----
    public bool EnableAt { get; private set; }

    public bool EnableRandomPunish { get; private set; }

    public bool BroadcastCrossGroupAttack { get; private set; } = true;

    public bool BroadcastCrossGroupDevour { get; private set; } = true;

    public bool BroadcastCrossGroupAttackEscape { get; private set; }

    public bool BroadcastCrossGroupDevourEscape { get; private set; }

    // ---- 展示 ----
    public ShortNumberStyle ShortNumberStyle { get; private set; }

    public int WeightUnitBase { get; private set; } = 1;

    public string WeightUnit { get; private set; } = "kg";

    public List<string> NickNameFilter { get; private set; } = ["[cq:"];

    // ---- 指令触发词 ----
    public string CommandRegister { get; private set; } = "#注册";

    public string CommandLogin { get; private set; } = "#签到";

    public string CommandMenu { get; private set; } = "#鲲菜单";

    public string CommandMyKun { get; private set; } = "#我的鲲";

    public string CommandRanking { get; private set; } = "#排行";

    public string CommandRankingGroup { get; private set; } = "#群排行";

    public string CommandInventory { get; private set; } = "#背包";

    public string CommandHatch { get; private set; } = "#孵蛋";

    public string CommandFeed { get; private set; } = "#喂养";

    public string CommandUpgrade { get; private set; } = "#强化";

    public string CommandTransmogrify { get; private set; } = "#幻化";

    public string CommandQueryDeadKuns { get; private set; } = "#查询已死亡鲲";

    public string CommandAscend { get; private set; } = "#渡劫";

    public string CommandResurrect { get; private set; } = "#复活";

    public string CommandRelease { get; private set; } = "#放生";

    public string CommandDevour { get; private set; } = "#吞噬";

    public string CommandAttack { get; private set; } = "#攻击";

    public string CommandShopping { get; private set; } = "#购物";

    public string CommandOpenEgg { get; private set; } = "#开鲲蛋";

    public string CommandOpenBlindBox { get; private set; } = "#开盲盒";

    public string CommandStartIdle { get; private set; } = "#开始挂机";

    public string CommandStopIdle { get; private set; } = "#停止挂机";

    public string CommandStartWork { get; private set; } = "#开始打工";

    public string CommandStopWork { get; private set; } = "#停止打工";

    public string CommandRandomPunishInfo { get; private set; } = "#天罚";

    public string CommandConsumeAscendPill { get; private set; } = "#使用渡劫丹";

    public string CommandSetNickName { get; private set; } = "#自定义名称";

    public string CommandClearNickName { get; private set; } = "#恢复名称";

    public string CommandUseItem { get; private set; } = "#使用物品";

    protected override void Load()
    {
        EnableAt = Get("EnableAt", false);
        EnableRandomPunish = Get("EnableRandomPunish", false);
        BroadcastCrossGroupAttack = Get("EnableNotSameGroupAttackBoardcast", true);
        BroadcastCrossGroupDevour = Get("EnableNotSameGroupDevourBoardcast", true);
        BroadcastCrossGroupAttackEscape = Get("EnableNotSameGroupAttackEscapeBoardcast", false);
        BroadcastCrossGroupDevourEscape = Get("EnableNotSameGroupDevourEscapeBoardcast", false);
        WeightUnitBase = Get("WeightUnitBase", 1);
        WeightUnit = Get("WeightUnit", "kg");
        ShortNumberStyle = Get("ShortNumberType", ShortNumberStyle.Normal);
        NickNameFilter = Get("NickNameFilter", new List<string> { "[cq:" });

        CommandRegister = Get("CommandRegister", "#注册");
        CommandLogin = Get("CommandLogin", "#签到");
        CommandMenu = Get("CommandMenu", "#鲲菜单");
        CommandMyKun = Get("CommandMyKun", "#我的鲲");
        CommandRanking = Get("CommandRanking", "#排行");
        CommandRankingGroup = Get("CommandRankingGroup", "#群排行");
        CommandInventory = Get("CommandInventory", "#背包");
        CommandHatch = Get("CommandHatch", "#孵蛋");
        CommandFeed = Get("CommandFeed", "#喂养");
        CommandUpgrade = Get("CommandUpgrade", "#强化");
        CommandTransmogrify = Get("CommandTransmogrify", "#幻化");
        CommandQueryDeadKuns = Get("CommandQueryDeadKuns", "#查询已死亡鲲");
        CommandAscend = Get("CommandAscend", "#渡劫");
        CommandResurrect = Get("CommandResurrect", "#复活");
        CommandRelease = Get("CommandReleaseKun", "#放生");
        CommandDevour = Get("CommandDevour", "#吞噬");
        CommandAttack = Get("CommandAttack", "#攻击");
        CommandShopping = Get("CommandShopping", "#购物");
        CommandOpenEgg = Get("CommandOpenEgg", "#开鲲蛋");
        CommandOpenBlindBox = Get("CommandOpenBlindBox", "#开盲盒");
        CommandStartIdle = Get("CommandStartAutoPlay", "#开始挂机");
        CommandStopIdle = Get("CommandStopAutoPlay", "#停止挂机");
        CommandStartWork = Get("CommandStartWorking", "#开始打工");
        CommandStopWork = Get("CommandStopWorking", "#停止打工");
        CommandRandomPunishInfo = Get("CommandRandomPunish", "#天罚");
        CommandConsumeAscendPill = Get("CommandConsumeAscendPill", "#使用渡劫丹");
        CommandSetNickName = Get("CommandUseCustomNickName", "#自定义名称");
        CommandClearNickName = Get("CommandUnuseCustomNickName", "#恢复名称");
        CommandUseItem = Get("CommandUseItem", "#使用物品");

        RegisterRewardCoins = Get("ValueRegisterCoinReward", 500);
        RegisterRewardEggs = Get("ValueRegisterEggReward", 50);
        LoginRewardCoins = Get("ValueLoginCoinReward", 100);
        LoginRewardEggs = Get("ValueLoginEggReward", 10);
        AttackCooldownMinutes = Get("ValueAttackCD", 30.0);
        DevourCooldownMinutes = Get("ValueDevourCD", 30.0);

        EnabledGroups = Get("Groups", new List<long>());
        Admins = Get("Admins", new List<long>());

        LoadNumbers();
        Replies.ReloadFrom(this);
    }

    // ---- 数值参数（Load 中赋值）----
    public int RegisterRewardCoins { get; private set; }

    public int RegisterRewardEggs { get; private set; }

    public int LoginRewardCoins { get; private set; }

    public int LoginRewardEggs { get; private set; }

    public double AttackCooldownMinutes { get; private set; }

    public double DevourCooldownMinutes { get; private set; }

    public List<long> EnabledGroups { get; private set; } = [];

    public List<long> Admins { get; private set; } = [];

    /// <summary>回复文案子集</summary>
    public ReplyTexts Replies { get; } = new();

    /// <summary>
    /// 兼容旧配置键的完整数值段。为控制篇幅拆分到 partial：
    /// CoreConfiguration.Numbers.cs（数值）/ CoreConfiguration.Replies.cs（回复）。
    /// </summary>
}

/// <summary>
/// 核心数值参数段。
/// </summary>
public sealed partial class CoreConfiguration
{
    public double ProbabilityNone { get; private set; } = 70.0;

    public double ProbabilityJin { get; private set; } = 5.0;

    public double ProbabilityMu { get; private set; } = 5.0;

    public double ProbabilityShui { get; private set; } = 5.0;

    public double ProbabilityHuo { get; private set; } = 5.0;

    public double ProbabilityTu { get; private set; } = 5.0;

    public double ProbabilityFeng { get; private set; } = 2.0;

    public double ProbabilityLei { get; private set; } = 2.0;

    public double ProbabilityYin { get; private set; } = 0.5;

    public double ProbabilityYang { get; private set; } = 0.5;

    public int HatchRateMinPercent { get; private set; }

    public int HatchRateMaxPercent { get; private set; }

    public int HatchWeightMin { get; private set; }

    public int HatchWeightMax { get; private set; }

    public int FeedCoinCostPerCount { get; private set; }

    public int FeedEggCostPerCount { get; private set; }

    public int FeedWeightBaseIncrement { get; private set; }

    public int FeedWeightMinBonusPercent { get; private set; }

    public int FeedWeightMaxBonusPercent { get; private set; }

    public int RankingSize { get; private set; }

    public int DevourDrawRangePercent { get; private set; }

    public int AttackDamageMinPercent { get; private set; }

    public int AttackDamageMaxPercent { get; private set; }

    public double AscendFailDeathChance { get; private set; } = 10.0;

    public double DevourFailDeathChance { get; private set; } = 20.0;

    public double TransmogrifyDeathWeightLimit { get; private set; } = 10.0;

    public double TransmogrifyFailDeathChance { get; private set; } = 10.0;

    public int EggToBlindBoxRate { get; private set; } = 1;

    public int MaxResurrectHours { get; private set; } = 81;

    public int WeightLossPerTwoHoursPercent { get; private set; } = 1;

    public int LevelLossPerEighteenHours { get; private set; } = 1;

    public int TransmogrifyLevelRequirement { get; private set; } = 5;

    public int AscendCoinCost { get; private set; }

    public int TransmogrifyCoinCost { get; private set; }

    public int TransmogrifyPillCost { get; private set; }

    public int UpgradeCoinCost { get; private set; }

    public int UpgradePillCost { get; private set; }

    public int AscendGainMinPercent { get; private set; }

    public int AscendGainMaxPercent { get; private set; }

    public int AscendLossMinPercent { get; private set; }

    public int AscendLossMaxPercent { get; private set; }

    public int IdleDeathChancePercent { get; private set; }

    public int MaxIdleDurationHours { get; private set; } = 24;

    public int PunishChancePercent { get; private set; }

    public int PunishLossMinPercent { get; private set; }

    public int PunishLossMaxPercent { get; private set; }

    public int PunishDeathChancePercent { get; private set; }

    public int PunishExecuteDayOfWeek { get; private set; }

    public DateTime PunishExecuteTime { get; private set; }

    public double IdleCooldownHours { get; private set; }

    public double WorkCooldownHours { get; private set; }

    public int WorkCoinPerHour { get; private set; }

    public int MaxAscendPillConsume { get; private set; } = 3;

    public int AscendSuccessPerPillPercent { get; private set; } = 10;

    public double CrossGroupNoticeMinLossPercent { get; private set; }

    public int ResurrectFloorPercent { get; private set; } = 1;

    public int UpgradeExpHours { get; private set; } = 24;

    public int WorkLevelBonusPercent { get; private set; } = 10;

    public List<string> ShoppingListRaw { get; private set; } = ["1|2|100|1"];

    public List<string> BlindBoxContentsRaw { get; private set; } = ["0|75", "4|8", "5|8", "6|7", "7|1"];

    public bool BlindBoxAllowMultiDraw { get; private set; }

    public bool BlindBoxMultiDrawMustContainItem { get; private set; }

    public int BlindBoxMultiDrawChancePercent { get; private set; } = 10;
}