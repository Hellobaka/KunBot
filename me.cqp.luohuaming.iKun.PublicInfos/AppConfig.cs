using me.cqp.luohuaming.iKun.PublicInfos.Enums;
using System;
using System.Collections.Generic;
using System.IO;

namespace me.cqp.luohuaming.iKun.PublicInfos
{
    public class AppConfig : ConfigBase
    {
        public AppConfig(string path)
            : base(path)
        {
            LoadConfig();
            Instance = this;
        }

        public static AppConfig Instance { get; private set; }

        public static bool EnableAt { get; set; }

        public static bool EnableRandomPunish { get; set; }

        public static ShortNumberType ShortNumberType { get; set; }

        public static int WeightUnitBase { get; set; } = 1;

        public static string WeightUnit { get; set; } = "kg";

        public static string CommandRegister { get; set; } = "";

        public static string CommandLogin { get; set; } = "";

        public static string CommandMenu { get; set; } = "";

        public static string CommandRanking { get; set; } = "";

        public static string CommandRankingGroup { get; set; } = "";

        public static string CommandInventory { get; set; } = "";

        public static string CommandHatch { get; set; } = "";

        public static string CommandFeed { get; set; } = "";

        public static string CommandUpgrade { get; set; } = "";

        public static string CommandTransmogrify { get; set; } = "";

        public static string CommandQueryDeadKuns { get; set; } = "";

        /// <summary>
        /// 渡劫
        /// </summary>
        public static string CommandAscend { get; set; } = "";

        /// <summary>
        /// 复活
        /// </summary>
        public static string CommandResurrect { get; set; } = "";

        public static string CommandReleaseKun { get; set; } = "";

        /// <summary>
        /// 吞噬
        /// </summary>
        public static string CommandDevour { get; set; } = "";

        public static string CommandAttack { get; set; } = "";

        public static string CommandBuyEgg { get; set; } = "";

        public static string CommandOpenEgg { get; set; } = "";

        public static string CommandOpenBlindBox { get; set; } = "";

        public static string CommandStartAutoPlay { get; set; } = "";

        public static string CommandStopAutoPlay { get; set; } = "";

        public static string CommandStartWorking { get; set; } = "";

        public static string CommandStopWorking { get; set; } = "";

        public static string CommandRandomPunish { get; set; } = "";

        public static string ReplyDuplicateRegister { get; set; } = "";

        public static string ReplyNewRegister { get; set; } = "";

        public static string ReplyRegisterFailed { get; set; } = "";

        public static string ReplyMenu { get; set; } = "";

        public static string ReplyNoPlayer { get; set; } = "";

        public static string ReplyNoTargetPlayer { get; set; } = "";

        public static string ReplyNoKun { get; set; } = "";

        public static string ReplyNoTargrtKun { get; set; } = "";

        public static string ReplyKunOwnerNotMatch { get; set; } = "";

        public static string ReplyKunAbandoned { get; set; } = "";

        public static string ReplyKunAlive { get; set; } = "";

        public static string ReplyKunWeightZero { get; set; } = "";

        public static string ReplyKunNotAlive { get; set; } = "";

        public static string ReplyNoTargetPlayerKun { get; set; } = "";

        public static string ReplyAscendNoWeightLimit { get; set; } = "";

        public static string ReplyAscendFailAndDead { get; set; } = "";

        public static string ReplyAscendFail { get; set; } = "";

        public static string ReplyAscendSuccess { get; set; } = "";

        public static string ReplyParamInvalid { get; set; } = "";

        public static string ReplyAttackInCD { get; set; } = "";

        public static string ReplyDevourInCD { get; set; } = "";

        public static string ReplyAttackSuccess { get; set; } = "";

        public static string ReplyAttackFail { get; set; } = "";

        public static string ReplyAttackEscaped { get; set; } = "";

        public static string ReplyAttackSuccessAndTargetDead { get; set; } = "";

        public static string ReplyAttackFailAndDead { get; set; } = "";

        public static string ReplyDevourSuccess { get; set; } = "";

        public static string ReplyDevourFail { get; set; } = "";

        public static string ReplyDevourFailAndDead { get; set; } = "";

        public static string ReplyDevourEscaped { get; set; } = "";

        public static string ReplyDuplicateLogin { get; set; } = "";

        public static string ReplyLoginReward { get; set; } = "";

        public static string ReplyItemLeak { get; set; } = "";

        public static string ReplyMultiHatchKun { get; set; } = "";

        public static string ReplyHatchKun { get; set; } = "";

        public static string ReplyDuplicateHatch { get; set; } = "";

        public static string ReplyDuplicateResurrect { get; set; } = "";

        public static string ReplyBuyEgg { get; set; } = "";

        public static string ReplyHatchFail { get; set; } = "";

        public static string ReplyFeed { get; set; } = "";

        public static string ReplyBlindBoxOpen { get; set; } = "";

        public static string ReplyBlindBoxGetNothing { get; set; } = "";

        public static string ReplyOpenKunEgg { get; set; } = "";

        public static string ReplyReleaseSuccess { get; set; } = "";

        public static string ReplyReleaseFail { get; set; } = "";

        public static string ReplyQueryDeadKun { get; set; } = "";

        public static string ReplyTransmogrifySuccess { get; set; } = "";

        public static string ReplyTransmogrifyFail { get; set; } = "";

        public static string ReplyTransmogrifyFailAndDead { get; set; } = "";

        public static string ReplyTransmogrifyLevelLimit { get; set; } = "";

        public static string ReplyUpgradeSuccess { get; set; } = "";

        public static string ReplyUpgradeFail { get; set; } = "";

        public static string ReplyResurrectHourLimit { get; set; } = "";

        public static string ReplyResurrectSuccess { get; set; } = "";

        public static string ReplyResurrectFail { get; set; } = "";

        public static string ReplyWeightLimit { get; set; } = "";

        public static string ReplyResurrectFailed { get; set; } = "";

        public static string ReplyStartAutoPlayFailed { get; set; } = "";

        public static string ReplyStopAutoPlayFailed { get; set; } = "";

        public static string ReplyRankingHeader { get; set; } = "";

        public static string ReplyRankingGroupHeader { get; set; } = "";

        public static string ReplyEmptyInventory { get; set; } = "";

        public static string ReplyAttackSelf { get; set; } = "";

        public static string ReplyDevourSelf { get; set; } = "";

        public static string ReplyKunToString { get; set; } = "";

        public static string ReplyAutoPlayStarted { get; set; } = "";

        public static string ReplyWorkingStarted { get; set; } = "";

        public static string ReplyAutoPlayFinished { get; set; } = "";
       
        public static string ReplyAutoPlayFinishedButDead { get; set; } = "";

        public static string ReplyAutoPlaying { get; set; } = "";

        public static string ReplyNotAutoPlaying { get; set; } = "";

        public static string ReplyRandomPunish { get; set; } = "";
        
        public static string ReplyRandomPunishSkipped { get; set; } = "";
        
        public static string ReplyRandomPunishFinished { get; set; } = "";
       
        public static string ReplyRandomPunishFinishedAndDead { get; set; } = "";

        public static string ReplyRankingAutoPlaying { get; set; } = "";

        public static string ReplyRankingWorking { get; set; } = "";

        public static string ReplyAutoPlayInCD { get; set; } = "";

        public static string ReplyWorkingInCD { get; set; } = "";

        public static string ReplyWorking { get; set; } = "";

        public static string ReplyNotWorking { get; set; } = "";

        public static string ReplyWorkingFinished { get; set; } = "";

        public static double ProbablityNone { get; set; }

        public static double ProbablityJin { get; set; }

        public static double ProbablityHuo { get; set; }

        public static double ProbablityFeng { get; set; }

        public static double ProbablityTu { get; set; }

        public static double ProbablityLei { get; set; }

        public static double ProbablityShui { get; set; }

        public static double ProbablityYin { get; set; }

        public static double ProbablityYang { get; set; }

        public static double ProbablityMu { get; set; }

        public static int ValueHatchProbablityMin { get; set; }

        public static int ValueHatchProbablityMax { get; set; }

        public static int ValueHatchWeightMin { get; set; }

        public static int ValueHatchWeightMax { get; set; }

        public static int ValueEggValue { get; set; }

        public static int ValueAscendCoinConsume { get; set; }

        public static int ValueFeedCoinConsume { get; set; }

        public static int ValueFeedKunEggConsume { get; set; }

        public static int ValueTranmogifyCoinConsume { get; set; }

        public static int ValueTranmogifyPillConsume { get; set; }

        public static int ValueUpgradeCoinConsume { get; set; }

        public static int ValueUpgradePillConsume { get; set; }

        public static int ValueFeedWeightBaseIncrement { get; set; }

        public static int ValueFeedWeightMinimumIncrement { get; set; }

        public static int ValueFeedWeightMaximumIncrement { get; set; }

        public static int ValueRankingCount { get; set; }

        public static int ValueDevourDrawPercentage { get; set; }

        public static int ValueAttackWeightMinimumDecrement { get; set; }

        public static int ValueAttackWeightMaximumDecrement { get; set; }

        public static int ValueLoginCoinReward { get; set; }

        public static int ValueLoginEggReward { get; set; }

        public static double ValueAscendFailDeadProbablity { get; set; } = 10;

        public static double ValueDevourFailDeadProbablity { get; set; } = 20;

        public static double ValueTransmoirgifyDeadWeightLimit { get; set; } = 10;

        public static double ValueTransmoirgifyFailDeadProbablity { get; set; } = 10;

        public static double ValueAttackCD { get; set; } = 30;

        public static double ValueDevourCD { get; set; } = 30;

        public static int ValueKunEggToBlindBoxRate { get; set; } = 1;

        public static int ValueMaxResurrectHour { get; set; } = 81;

        public static int ValuePerTwoHourWeightLoss { get; set; } = 1;

        public static int ValuePerEighteenHourLevelLoss { get; set; } = 1;

        public static int ValueTransmogrifyLevelLimit { get; set; } = 5;

        public static int ValueAscendWeightMinimalIncrement { get; set; }

        public static int ValueAscendWeightMaximalIncrement { get; set; }

        public static int ValueAscendWeightMinimalDecrement { get; set; }

        public static int ValueAscendWeightMaximalDecrement { get; set; }

        public static int ValueRegisterCoinReward { get; set; }

        public static int ValueRegisterEggReward { get; set; }

        public static int ValueAutoPlayDeadProbablity { get; set; }

        public static int ValueMaxAutoPlayDuration { get; set; }

        public static int ValueRandomPunishProbablity { get; set; }

        public static int ValueRandomPunishMinimalDecrement { get; set; }
       
        public static int ValueRandomPunishMaximalDecrement { get; set; }
       
        public static int ValueRandomPunishDeadProbablity { get; set; }
       
        public static int ValueRandomPunishExecuteDay { get; set; }

        public static DateTime ValueRandomPunishExecuteTime {  get; set; }

        public static int ValueAutoPlayCDHour {  get; set; }

        public static int ValueWorkingCDHour {  get; set; }

        public static int ValueWorkingCoinRewardPerHour {  get; set; }

        public static List<string> BlindBoxContents { get; set; } = [];

        public static bool BlindBoxEnableMultiContents {  get; set; }

        public static int BlindBoxMultiContentProbablity { get; set; } = 10;

        public static bool BlindBoxMultiContentMustHasItem {  get; set; }

        public static List<long> Groups { get; set; } = [];

        public static List<long> Admins { get; set; } = [];

        public override void LoadConfig()
        {
            EnableAt = GetConfig("EnableAt", false);
            EnableRandomPunish = GetConfig("EnableRandomPunish", false);
            WeightUnitBase = GetConfig("WeightUnitBase", 1);
            WeightUnit = GetConfig("WeightUnit", "kg");
            ShortNumberType = GetConfig("ShortNumberType", ShortNumberType.Normal);

            CommandAscend = GetConfig("CommandAscend", "#渡劫");
            CommandAttack = GetConfig("CommandAttack", "#攻击");
            CommandBuyEgg = GetConfig("CommandBuyEgg", "#买鲲蛋");
            CommandDevour = GetConfig("CommandDevour", "#吞噬");
            CommandFeed = GetConfig("CommandFeed", "#喂养");
            CommandHatch = GetConfig("CommandHatch", "#孵蛋");
            CommandInventory = GetConfig("CommandInventory", "#背包");
            CommandLogin = GetConfig("CommandLogin", "#签到");
            CommandMenu = GetConfig("CommandMenu", "#鲲菜单");
            CommandOpenBlindBox = GetConfig("CommandOpenBlindBox", "#开盲盒");
            CommandOpenEgg = GetConfig("CommandOpenEgg", "#开鲲蛋");
            CommandRanking = GetConfig("CommandRanking", "#排行");
            CommandRankingGroup = GetConfig("CommandRankingGroup", "#群排行");
            CommandReleaseKun = GetConfig("CommandReleaseKun", "#放生");
            CommandResurrect = GetConfig("CommandResurrect", "#复活");
            CommandTransmogrify = GetConfig("CommandTransmogrify", "#幻化");
            CommandUpgrade = GetConfig("CommandUpgrade", "#强化");
            CommandRegister = GetConfig("CommandRegister", "#注册");
            CommandQueryDeadKuns = GetConfig("CommandQueryDeadKuns", "#查询已死亡鲲");
            CommandStartAutoPlay = GetConfig("CommandStartAutoPlay", "#开始挂机");
            CommandStopAutoPlay = GetConfig("CommandStopAutoPlay", "#停止挂机");
            CommandStartWorking = GetConfig("CommandStartWorking", "#开始打工");
            CommandStopWorking = GetConfig("CommandStopWorking", "#停止打工");
            CommandRandomPunish = GetConfig("CommandRandomPunish", "#天罚");

            ReplyDuplicateRegister = GetConfig("ReplyDuplicateRegister", "你已经注册过了，不能重复注册");
            ReplyDuplicateLogin = GetConfig("ReplyLoginDuplicate", "你今天已经签到过了，不能重复签到");
            ReplyDuplicateHatch = GetConfig("ReplyDuplicateHatch", "你已经有一只鲲了，不能重复孵化");
            ReplyDuplicateResurrect = GetConfig("ReplyDuplicateResurrect", "你已经有一只鲲了，不能执行复活");
            ReplyNewRegister = GetConfig("ReplyNewRegister", "注册成功，赠送 {0} 枚金币以及 {1} 枚鲲蛋");
            ReplyRegisterFailed = GetConfig("ReplyRegisterFailed", "注册失败了，查看日志排查问题");
            ReplyMenu = GetConfig("ReplyMenu", "功能列表：\n{0}  {1}\n{2}  {3}\n{4}  {5}\n{6}  {7}\n{8}  {9}\n{10}  {11}\n{12}  {13}\n{14}  {15}\n{16}  {17}");
            ReplyNoPlayer = GetConfig("ReplyNoPlayer", "请先注册");
            ReplyNoKun = GetConfig("ReplyNoKun", "未持有鲲");
            ReplyNoTargrtKun = GetConfig("ReplyNoTargrtKun", "目标所指的鲲不存在");
            ReplyKunOwnerNotMatch = GetConfig("ReplyKunOwnerNotMatch", "这只鲲不是你的{0}");
            ReplyKunAbandoned = GetConfig("ReplyKunAbandoned", "鲲已被标记为弃置{0}");
            ReplyKunAlive = GetConfig("ReplyKunAlive", "鲲未死亡{0}");
            ReplyKunWeightZero = GetConfig("ReplyKunWeightZero", "鲲体重小于0{0}");
            ReplyKunNotAlive = GetConfig("ReplyKunNotAlive", "鲲已死亡{0}");
            ReplyLoginReward = GetConfig("ReplyLoginReward", "签到成功，赠送 {0} 枚金币以及 {1} 枚鲲蛋");
            ReplyItemLeak = GetConfig("ReplyItemLeak", "{0}数量不足，需要{1}个，现有{2}个");
            ReplyHatchFail = GetConfig("ReplyHatchFail", "孵化失败\n-------------------\n剩余 {0} 颗鲲蛋");
            ReplyHatchKun = GetConfig("ReplyHatchKun", "恭喜你获得一只{0}\n体重 {1} 千克\n-------------------\n剩余 {2} 颗鲲蛋");
            ReplyMultiHatchKun = GetConfig("ReplyMultiHatchKun", "恭喜你获得一只{0}\n体重 {1} 千克\n-------------------\n共消耗 {2} 个蛋 剩余 {3} 颗鲲蛋");
            ReplyBuyEgg = GetConfig("ReplyBuyEgg", "购买成功，消耗金币 {0} 枚，获得 {1} 枚鲲蛋\n-------------------\n剩余金币 {2} 枚，鲲蛋 {3} 枚");
            ReplyAscendNoWeightLimit = GetConfig("ReplyAscendNoWeightLimit", "无法渡劫，由于体重未达到上限\n当前体重 {0} kg，上限体重 {1} kg");
            ReplyAscendSuccess = GetConfig("ReplyAscendSuccess", "渡劫成功，体重增加了 {0} kg，等级加一\n当前体重 {1} kg，当前等级 {2}");
            ReplyAscendFailAndDead = GetConfig("ReplyAscendFailAndDead", "渡劫失败，你的鲲已死亡");
            ReplyAscendFail = GetConfig("ReplyAscendFail", "渡劫失败，体重减少了 {0} kg，当前体重 {1} kg");
            ReplyParamInvalid = GetConfig("ReplyParamInvalid", "指令格式错误{0}");
            ReplyNoTargetPlayer = GetConfig("ReplyNoTargetPlayer", "目标指定的玩家未注册");
            ReplyNoTargetPlayerKun = GetConfig("ReplyNoTargetKun", "目标指定的玩家未持有鲲");
            ReplyAttackInCD = GetConfig("ReplyAttackInCD", "攻击冷却中，下次可攻击时间：{0}");
            ReplyDevourInCD = GetConfig("ReplyDevourInCD", "吞噬冷却中，下次可吞噬时间：{0}");
            ReplyAttackSuccess = GetConfig("ReplyAttackSuccess", "{0} 对 {1}的{2} 发起攻击，攻击成功了！\n攻击方体重增长 {3} kg，现 {4} kg\n被攻击方体重减少 {5} kg，现 {6} kg");
            ReplyAttackFail = GetConfig("ReplyAttackFail", "{0} 对 {1}的{2} 发起攻击，攻击失败了！\n攻击方体重减少 {3} kg，现 {4} kg\n被攻击方体重增加 {5} kg，现 {6} kg");
            ReplyAttackEscaped = GetConfig("ReplyAttackEscaped", "{0} 对 {1}的{2} 发起攻击，对方逃脱了！");
            ReplyAttackSuccessAndTargetDead = GetConfig("ReplyAttackSuccessAndTargetDead", "{0} 对 {1}的{2} 发起攻击，攻击成功了！被攻击方伤重致死\n攻击方体重增长 {3} kg，现 {4} kg");
            ReplyAttackFailAndDead = GetConfig("ReplyAttackFailAndDead", "{0} 对 {1}的{2} 发起攻击，攻击失败了！自身伤重致死\n被攻击方体重增加 {3} kg，现 {4} kg");
            ReplyDevourSuccess = GetConfig("ReplyDevourSuccess", "{0} 吃掉了 {1}的{2}\n攻击方体重增长 {3} kg，现 {4} kg");
            ReplyDevourFail = GetConfig("ReplyDevourFail", "{0} 企图吃掉 {1}的{2}，但是失败了！\n攻击方体重减少 {3} kg，现 {4} kg\n被攻击方体重增加 {5} kg，现 {6} kg");
            ReplyDevourFailAndDead = GetConfig("ReplyDevourFailAndDead", "{0} 企图吃掉 {1}的{2}，但是失败了！反倒被对方吃掉\n被攻击方体重增加 {3} kg，现 {4} kg");
            ReplyDevourEscaped = GetConfig("ReplyAttackEscaped", "{0} 企图吃掉 {1}的{2}，对方逃脱了！");
            ReplyFeed = GetConfig("ReplyFeed", "你的「{0}」体重增加了 {1} 千克\n现体重为 {2} 千克\n-------------------\n剩余 {3} 枚金币，{4} 枚鲲蛋");
            ReplyBlindBoxOpen = GetConfig("ReplyBlindBoxOpen", "打开 {0} 个盲盒，获得了以下物品：\n{1}");
            ReplyBlindBoxGetNothing = GetConfig("ReplyBlindBoxGetNothing", "打开 {0} 个盲盒，什么也没获得");
            ReplyOpenKunEgg = GetConfig("ReplyOpenKunEgg", "打开 {0} 个鲲蛋，获得了 {1} 个盲盒");
            ReplyReleaseSuccess = GetConfig("ReplyReleaseSuccess", "{0}放生成功");
            ReplyReleaseFail = GetConfig("ReplyReleaseFail", "放生失败，可能是鲲已死亡或不存在");
            ReplyQueryDeadKun = GetConfig("ReplyQueryDeadKun", "还可复活的鲲列表如下：\n");
            ReplyTransmogrifySuccess = GetConfig("ReplyTransmogrifySuccess", "幻化成功，{0} 转变为 {1}，体重减少 {2} kg，现 {3} kg\n剩余 {4} 颗幻化丹 {5} 枚金币");
            ReplyTransmogrifyFail = GetConfig("ReplyTransmogrifyFail", "幻化失败了，体重减少 {0} kg，现 {1} kg\n剩余 {2} 颗幻化丹 {3} 枚金币");
            ReplyTransmogrifyFailAndDead = GetConfig("ReplyTransmogrifyFailAndDead", "幻化失败并且魂飞魄散\n剩余 {0} 颗幻化丹 {1} 枚金币");
            ReplyTransmogrifyLevelLimit = GetConfig("ReplyTransmogrifyLevelLimit", "不能执行幻化，由于等级限制，当前等级 {0}，最低幻化等级：{1}");
            ReplyUpgradeSuccess = GetConfig("ReplyUpgradeSuccess", "强化完成，体重增加了 {0} kg，当前体重 {1} kg\n剩余 {2} 颗强化丹 {3} 枚金币");
            ReplyUpgradeFail = GetConfig("ReplyUpgradeFail", "强化失败，体重减少了 {0} kg，当前体重 {1} kg\n剩余 {2} 颗强化丹 {3} 枚金币");
            ReplyResurrectHourLimit = GetConfig("ReplyResurrectHourLimit", "无法复活，由于鲲死亡已超过 {0} 小时，当前死亡 {1} 小时");
            ReplyResurrectSuccess = GetConfig("ReplyResurrectSuccess", "鲲已复活，死亡时间 {0}，复活次数 {1}\n未复活期间，共损失了 {2} kg，{3} 星级\n消耗 {4} 个复活丸，还剩余 {5} 个复活丸");
            ReplyResurrectFail = GetConfig("ReplyResurrectFail", "复活失败，消耗 {0} 个复活丸，还剩余 {1} 个复活丸，查看日志查询原因");
            ReplyWeightLimit = GetConfig("ReplyWeightLimit", "体重已达上限，需进行渡劫提高体重上限");
            ReplyResurrectFailed = GetConfig("ReplyResurrectFailed", "，无法复活");
            ReplyRankingHeader = GetConfig("ReplyRankingHeader", "排行如下：");
            ReplyRankingGroupHeader = GetConfig("ReplyRankingGroupHeader", "群排行如下：");
            ReplyEmptyInventory = GetConfig("ReplyEmptyInventory", "仓库为空");
            ReplyAttackSelf = GetConfig("ReplyAttackSelf", "不能自己攻击自己");
            ReplyDevourSelf = GetConfig("ReplyDevourSelf", "不能自己攻击自己");
            ReplyKunToString = GetConfig("ReplyKunToString", "[%PetAttributeA%] %PetAttributeB%鲲 %LongLevel%");
            ReplyAutoPlayFinished = GetConfig("ReplyAutoPlayFinished", "挂机完成！\n你的 {0} 共挂机了 {1} 小时，获得了 {2} kg体重，当前体重 {3} kg");
            ReplyWorkingFinished = GetConfig("ReplyWorkingFinished", "打工完成！\n你的 {0} 共挂机了 {1} 小时，获得了 {2} 个金币，当前金币 {3} 个");
            ReplyAutoPlayFinishedButDead = GetConfig("ReplyAutoPlayFinishedButDead", "挂机完成！\n你的 {0} 共挂机了 {1} 小时，获得了 {2} kg体重\n但是却因走火入魔而暴毙！");
            ReplyAutoPlayStarted = GetConfig("ReplyAutoPlayStarted", "挂机开始！\n预计结束时间 {0} 预计获得体重 {1} kg");
            ReplyWorkingStarted = GetConfig("ReplyWorkingStarted", "打工开始！\n预计结束时间 {0} 预计获得金币 {1} 个");
            ReplyAutoPlaying = GetConfig("ReplyAutoPlaying", "{0} 正在挂机中");
            ReplyWorking = GetConfig("ReplyWorking", "{0} 正在打工中");
            ReplyNotAutoPlaying = GetConfig("ReplyNotAutoPlaying", "{0} 未在挂机");
            ReplyNotWorking = GetConfig("ReplyNotWorking", "{0} 未在打工");
            ReplyStartAutoPlayFailed = GetConfig("ReplyStartAutoPlayFailed", "，无法开始挂机");
            ReplyStopAutoPlayFailed = GetConfig("ReplyStopAutoPlayFailed", "，无法结束挂机");
            ReplyRandomPunish = GetConfig("ReplyRandomPunish", "每{0}会从所有鲲中抽选一个赐予天罚，体重越大抽到的概率更大\n下次天罚时间 {1}");
            ReplyRandomPunishFinished = GetConfig("ReplyRandomPunishFinished", "天罚降下，{0} 被五雷轰顶，{1} 损失了 {2} kg，现在体重 {3} kg");
            ReplyRandomPunishSkipped = GetConfig("ReplyRandomPunishSkipped", "本周天罚无事");
            ReplyRandomPunishFinishedAndDead = GetConfig("ReplyRandomPunishFinishedAndDead", "天罚降下，{0} 被五雷轰顶，{1} 直接暴毙！");
            ReplyRankingAutoPlaying = GetConfig("ReplyRankingAutoPlaying", "[挂机中]");
            ReplyRankingWorking = GetConfig("ReplyRankingWorking", "[打工中]");
            ReplyAutoPlayInCD = GetConfig("ReplyAutoPlayInCD", "挂机暂时不可用，下次可挂机时间：{0}");
            ReplyWorkingInCD = GetConfig("ReplyWorkingInCD", "打工暂时不可用，下次可打工时间：{0}");

            ProbablityNone = GetConfig("ProbablityNone", 70);
            ProbablityJin = GetConfig("ProbablityJin", 5);
            ProbablityMu = GetConfig("ProbablityMu", 5);
            ProbablityShui = GetConfig("ProbablityShui", 5);
            ProbablityTu = GetConfig("ProbablityTu", 5);
            ProbablityHuo = GetConfig("ProbablityHuo", 5);
            ProbablityFeng = GetConfig("ProbablityFeng", 2);
            ProbablityLei = GetConfig("ProbablityLei", 2);
            ProbablityYin = GetConfig("ProbablityYin", 0.5);
            ProbablityYang = GetConfig("ProbablityYang", 0.5);

            ValueHatchProbablityMin = GetConfig("ValueHatchProbablityMin", 10);
            ValueHatchProbablityMax = GetConfig("ValueHatchProbablityMax", 50);
            ValueHatchWeightMin = GetConfig("ValueHatchWeightMin", 10);
            ValueHatchWeightMax = GetConfig("ValueHatchWeightMax", 10000);
            ValueEggValue = GetConfig("ValueEggValue", 100);
            ValueFeedCoinConsume = GetConfig("ValueFeedCoinConsume", 10);
            ValueAscendCoinConsume = GetConfig("ValueAscendCoinConsume", 100);
            ValueFeedKunEggConsume = GetConfig("ValueFeedKunEggConsume", 1);
            ValueFeedWeightBaseIncrement = GetConfig("ValueFeedWeightBaseIncrement", 10);
            ValueFeedWeightMinimumIncrement = GetConfig("ValueFeedWeightMinimumIncrement", 5);
            ValueFeedWeightMaximumIncrement = GetConfig("ValueFeedWeightMaximumIncrement", 10);
            ValueAttackWeightMinimumDecrement = GetConfig("ValueAttackWeightMinimumDecrement", 5);
            ValueAttackWeightMaximumDecrement = GetConfig("ValueAttackWeightMaximumDecrement", 10);
            ValueRankingCount = GetConfig("ValueRankingCount", 10);
            ValueDevourDrawPercentage = GetConfig("ValueDevourDrawPercentage", 10);
            ValueLoginCoinReward = GetConfig("ValueLoginCoinReward", 100);
            ValueLoginEggReward = GetConfig("ValueLoginEggReward", 10);
            ValueAscendFailDeadProbablity = GetConfig("ValueAscendFailDeadProbablity", 10);
            ValueDevourFailDeadProbablity = GetConfig("ValueDevourFailDeadProbablity", 20);
            ValueTransmoirgifyDeadWeightLimit = GetConfig("ValueTransmoirgifyFailDeadWeightLimit", 10);
            ValueTransmoirgifyFailDeadProbablity = GetConfig("ValueTransmoirgifyFailDeadProbablity", 10);
            ValueAttackCD = GetConfig("ValueAttackCD", 30);
            ValueDevourCD = GetConfig("ValueDevourCD", 30);
            ValueKunEggToBlindBoxRate = GetConfig("ValueKunEggToCoinRate", 1);
            ValueMaxResurrectHour = GetConfig("ValueMaxDeadHour", 81);
            ValuePerTwoHourWeightLoss = GetConfig("ValuePerTwoHourWeightLoss", 1);
            ValuePerEighteenHourLevelLoss = GetConfig("ValuePerEighteenHourLevelLoss", 1);
            ValueTranmogifyCoinConsume = GetConfig("ValueTranmogifyCoinConsume", 100);
            ValueTranmogifyPillConsume = GetConfig("ValueTranmogifyPillConsume", 1);
            ValueTransmogrifyLevelLimit = GetConfig("ValueTransmogrifyLevelLimit", 5);
            ValueUpgradeCoinConsume = GetConfig("ValueUpgradeCoinConsume", 100);
            ValueUpgradePillConsume = GetConfig("ValueUpgradePillConsume", 1);
            ValueAscendWeightMinimalIncrement = GetConfig("ValueAscendWeightMinimalIncrement", 10);
            ValueAscendWeightMaximalIncrement = GetConfig("ValueAscendWeightMaximalIncrement", 400);
            ValueAscendWeightMinimalDecrement = GetConfig("ValueAscendWeightMinimalDecrement", 10);
            ValueAscendWeightMaximalDecrement = GetConfig("ValueAscendWeightMaximalDecrement", 50);
            ValueRegisterCoinReward = GetConfig("ValueRegisterCoinReward", 500);
            ValueRegisterEggReward = GetConfig("ValueRegisterEggReward", 50);
            ValueAutoPlayDeadProbablity = GetConfig("ValueAutoPlayDeadProbablity", 5);
            ValueMaxAutoPlayDuration = GetConfig("ValueMaxAutoPlayDuration", 24);
            ValueRandomPunishProbablity = GetConfig("ValueRandomPunishProbablity", 80);
            ValueRandomPunishMinimalDecrement = GetConfig("ValueRandomPunishMinimalDecrement", 50);
            ValueRandomPunishMaximalDecrement = GetConfig("ValueRandomPunishMaximalDecrement", 80);
            ValueRandomPunishDeadProbablity = GetConfig("ValueRandomPunishDeadProbablity", 10);
            ValueRandomPunishExecuteDay = GetConfig("ValueRandomPunishExecuteDay", 4);
            ValueRandomPunishExecuteTime = GetConfig("ValueRandomPunishExecuteTime", new DateTime());
            ValueAutoPlayCDHour = GetConfig("ValueAutoPlayCDHour", 12);
            ValueWorkingCDHour = GetConfig("ValueWorkingCDHour", 12);
            ValueWorkingCoinRewardPerHour = GetConfig("ValueWorkingCoinRewardPerHour", 10);
            
            BlindBoxContents = GetConfig("BlindBoxContents", new List<string>() { "0|75", "4|8", "5|8", "6|8" });
            BlindBoxEnableMultiContents = GetConfig("BlindBoxEnableMultiContents", false);
            BlindBoxMultiContentMustHasItem = GetConfig("BlindBoxMultiContentMustHasItem", false);
            BlindBoxMultiContentProbablity = GetConfig("BlindBoxMultiContentProbablity", 10);

            Groups = GetConfig("Groups", new List<long>());
            Admins = GetConfig("Admins", new List<long>());
        }
    }
}