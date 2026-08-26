namespace me.cqp.luohuaming.iKun.Domain.Configuration;

/// <summary>
/// 核心数值参数的配置加载实现。
/// </summary>
public sealed partial class CoreConfiguration
{
    private void LoadNumbers()
    {
        ProbabilityNone = Get("ProbablityNone", 70.0);
        ProbabilityJin = Get("ProbablityJin", 5.0);
        ProbabilityMu = Get("ProbablityMu", 5.0);
        ProbabilityShui = Get("ProbablityShui", 5.0);
        ProbabilityHuo = Get("ProbablityHuo", 5.0);
        ProbabilityTu = Get("ProbablityTu", 5.0);
        ProbabilityFeng = Get("ProbablityFeng", 2.0);
        ProbabilityLei = Get("ProbablityLei", 2.0);
        ProbabilityYin = Get("ProbablityYin", 0.5);
        ProbabilityYang = Get("ProbablityYang", 0.5);

        HatchRateMinPercent = Get("ValueHatchProbablityMin", 10);
        HatchRateMaxPercent = Get("ValueHatchProbablityMax", 50);
        HatchWeightMin = Get("ValueHatchWeightMin", 10);
        HatchWeightMax = Get("ValueHatchWeightMax", 10000);

        FeedCoinCostPerCount = Get("ValueFeedCoinConsume", 10);
        FeedEggCostPerCount = Get("ValueFeedKunEggConsume", 1);
        FeedWeightBaseIncrement = Get("ValueFeedWeightBaseIncrement", 10);
        FeedWeightMinBonusPercent = Get("ValueFeedWeightMinimumIncrement", 5);
        FeedWeightMaxBonusPercent = Get("ValueFeedWeightMaximumIncrement", 10);

        AttackDamageMinPercent = Get("ValueAttackWeightMinimumDecrement", 30);
        AttackDamageMaxPercent = Get("ValueAttackWeightMaximumDecrement", 100);
        RankingSize = Get("ValueRankingCount", 10);
        DevourDrawRangePercent = Get("ValueDevourDrawPercentage", 10);

        AscendFailDeathChance = Get("ValueAscendFailDeadProbablity", 10.0);
        DevourFailDeathChance = Get("ValueDevourFailDeadProbablity", 20.0);
        TransmogrifyDeathWeightLimit = Get("ValueTransmoirgifyFailDeadWeightLimit", 10.0);
        TransmogrifyFailDeathChance = Get("ValueTransmoirgifyFailDeadProbablity", 10.0);
        EggToBlindBoxRate = Get("ValueKunEggToCoinRate", 1);
        MaxResurrectHours = Get("ValueMaxDeadHour", 81);
        WeightLossPerTwoHoursPercent = Get("ValuePerTwoHourWeightLoss", 1);
        LevelLossPerEighteenHours = Get("ValuePerEighteenHourLevelLoss", 1);

        TransmogrifyCoinCost = Get("ValueTranmogifyCoinConsume", 100);
        TransmogrifyPillCost = Get("ValueTranmogifyPillConsume", 1);
        TransmogrifyLevelRequirement = Get("ValueTransmogrifyLevelLimit", 5);

        UpgradeCoinCost = Get("ValueUpgradeCoinConsume", 100);
        UpgradePillCost = Get("ValueUpgradePillConsume", 1);

        AscendGainMinPercent = Get("ValueAscendWeightMinimalIncrement", 10);
        AscendGainMaxPercent = Get("ValueAscendWeightMaximalIncrement", 400);
        AscendLossMinPercent = Get("ValueAscendWeightMinimalDecrement", 10);
        AscendLossMaxPercent = Get("ValueAscendWeightMaximalDecrement", 50);
        AscendCoinCost = Get("ValueAscendCoinConsume", 100);

        IdleDeathChancePercent = Get("ValueAutoPlayDeadProbablity", 5);
        MaxIdleDurationHours = Get("ValueMaxAutoPlayDuration", 24);

        PunishChancePercent = Get("ValueRandomPunishProbablity", 80);
        PunishLossMinPercent = Get("ValueRandomPunishMinimalDecrement", 50);
        PunishLossMaxPercent = Get("ValueRandomPunishMaximalDecrement", 80);
        PunishDeathChancePercent = Get("ValueRandomPunishDeadProbablity", 10);
        PunishExecuteDayOfWeek = Get("ValueRandomPunishExecuteDay", 4);
        PunishExecuteTime = Get("ValueRandomPunishExecuteTime", new DateTime());

        IdleCooldownHours = Get("ValueAutoPlayCDHour", 12.0);
        WorkCooldownHours = Get("ValueWorkingCDHour", 12.0);
        WorkCoinPerHour = Get("ValueWorkingCoinRewardPerHour", 10);

        MaxAscendPillConsume = Get("ValueAscendPillMaxConsumeCount", 3);
        AscendSuccessPerPillPercent = Get("ValueAscendPillPerIncrement", 10);
        CrossGroupNoticeMinLossPercent = Get("ValueNotSameGroupNoticeMinimalPercent", 10);
        ResurrectFloorPercent = Get("ValueResurrectWeightBase", 1);
        UpgradeExpHours = Get("ValueUpgradeExpHour", 24);
        WorkLevelBonusPercent = Get("ValueWorkLevelBouns", 5);

        ShoppingListRaw = Get("ShoppingList", new List<string> { "1|2|100|1" });
        BlindBoxContentsRaw = Get("BlindBoxContents", new List<string> { "0|75", "4|8", "5|8", "6|7", "7|1" });
        BlindBoxAllowMultiDraw = Get("BlindBoxEnableMultiContents", false);
        BlindBoxMultiDrawMustContainItem = Get("BlindBoxMultiContentMustHasItem", false);
        BlindBoxMultiDrawChancePercent = Get("BlindBoxMultiContentProbablity", 10);
    }
}