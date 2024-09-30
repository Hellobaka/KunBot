using System.Collections.Generic;
using System.IO;

namespace me.cqp.luohuaming.iKun.PublicInfos
{
    public static class AppConfig
    {
        private static FileSystemWatcher ConfigChangeWatcher { get; set; } = new();

        public static int RankingCount { get; set; } = 10;

        public static bool EnableAt { get; set; }

        public static bool EnableShortNumber { get; set; }

        public static string CommandRegister { get; set; } = "";

        public static string CommandLogin { get; set; } = "";

        public static string CommandMenu { get; set; } = "";

        public static string CommandRanking { get; set; } = "";

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

        public static string ReplyKunNotAlive { get; set; } = "";

        public static string ReplyNoTargetPlayerKun { get; set; } = "";

        public static string ReplyAscendNoWeightLimit { get; set; } = "";

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

        public static List<string> BlindBoxContents { get; set; } = [];

        public static List<long> Groups { get; set; } = [];

        public static List<long> Admins { get; set; } = [];

        public static void LoadConfig()
        {
            RankingCount = ConfigHelper.GetConfig("RankingCount", 10);
            EnableAt = ConfigHelper.GetConfig("EnableAt", false);
            EnableShortNumber = ConfigHelper.GetConfig("EnableShortNumber", false);

            CommandAscend = ConfigHelper.GetConfig("CommandAscend", "#渡劫");
            CommandAttack = ConfigHelper.GetConfig("CommandAttack", "#攻击");
            CommandBuyEgg = ConfigHelper.GetConfig("CommandBuyEgg", "#买鲲蛋");
            CommandDevour = ConfigHelper.GetConfig("CommandDevour", "#吞噬");
            CommandFeed = ConfigHelper.GetConfig("CommandFeed", "#喂养");
            CommandHatch = ConfigHelper.GetConfig("CommandHatch", "#孵蛋");
            CommandInventory = ConfigHelper.GetConfig("CommandInventory", "#背包");
            CommandLogin = ConfigHelper.GetConfig("CommandLogin", "#签到");
            CommandMenu = ConfigHelper.GetConfig("CommandMenu", "#菜单");
            CommandOpenBlindBox = ConfigHelper.GetConfig("CommandOpenBlindBox", "#开盲盒");
            CommandOpenEgg = ConfigHelper.GetConfig("CommandOpenEgg", "#开鲲蛋");
            CommandRanking = ConfigHelper.GetConfig("CommandRanking", "#排行");
            CommandReleaseKun = ConfigHelper.GetConfig("CommandReleaseKun", "#放生");
            CommandResurrect = ConfigHelper.GetConfig("CommandResurrect", "#复活");
            CommandTransmogrify = ConfigHelper.GetConfig("CommandTransmogrify", "#幻化");
            CommandUpgrade = ConfigHelper.GetConfig("CommandUpgrade", "#强化");
            CommandRegister = ConfigHelper.GetConfig("CommandRegister", "#注册");
            CommandQueryDeadKuns = ConfigHelper.GetConfig("CommandQueryDeadKuns", "#查询已死亡鲲");

            ReplyDuplicateRegister = ConfigHelper.GetConfig("ReplyDuplicateRegister", "你已经注册过了，不能重复注册");
            ReplyDuplicateLogin = ConfigHelper.GetConfig("ReplyLoginDuplicate", "你今天已经签到过了，不能重复签到");
            ReplyDuplicateHatch = ConfigHelper.GetConfig("ReplyDuplicateHatch", "你已经有一只鲲了，不能重复孵化");
            ReplyDuplicateResurrect = ConfigHelper.GetConfig("ReplyDuplicateResurrect", "你已经有一只鲲了，不能执行复活");
            ReplyNewRegister = ConfigHelper.GetConfig("ReplyNewRegister", "注册成功，赠送 {0} 枚金币以及 {1} 枚鲲蛋");
            ReplyRegisterFailed = ConfigHelper.GetConfig("ReplyRegisterFailed", "注册失败了");
            ReplyMenu = ConfigHelper.GetConfig("ReplyMenu", "菜单：");
            ReplyNoPlayer = ConfigHelper.GetConfig("ReplyNoPlayer", "请先注册");
            ReplyNoKun = ConfigHelper.GetConfig("ReplyNoKun", "未持有鲲");
            ReplyNoTargrtKun = ConfigHelper.GetConfig("ReplyNoTargrtKun", "目标所指的鲲不存在");
            ReplyKunOwnerNotMatch = ConfigHelper.GetConfig("ReplyKunOwnerNotMatch", "这只鲲不是你的{0}");
            ReplyKunAbandoned = ConfigHelper.GetConfig("ReplyKunAbandoned", "鲲已被标记为弃置{0}");
            ReplyKunAlive = ConfigHelper.GetConfig("ReplyKunAlive", "鲲未死亡{0}");
            ReplyKunNotAlive = ConfigHelper.GetConfig("ReplyKunNotAlive", "鲲已死亡{0}");
            ReplyLoginReward = ConfigHelper.GetConfig("ReplyLoginReward", "签到成功，赠送 {0} 枚金币以及 {1} 枚鲲蛋");
            ReplyItemLeak = ConfigHelper.GetConfig("ReplyItemLeak", "{0}数量不足，需要{1}个，现有{2}个");
            ReplyHatchFail = ConfigHelper.GetConfig("ReplyHatchFail", "孵化失败\n-------------------\n剩余 {0} 颗鲲蛋");
            ReplyHatchKun = ConfigHelper.GetConfig("ReplyHatchKun", "恭喜你获得一只{0}\n体重 {1} 千克\n-------------------\n剩余 {2} 颗鲲蛋");
            ReplyBuyEgg = ConfigHelper.GetConfig("ReplyBuyEgg", "购买成功，消耗金币 {0} 枚，获得 {1} 枚鲲蛋\n-------------------\n剩余金币 {2} 枚，鲲蛋 {3} 枚");
            ReplyAscendNoWeightLimit = ConfigHelper.GetConfig("ReplyAscendNoWeightLimit", "无法渡劫，由于体重未达到上限\n当前体重 {0} kg，上限体重 {1} kg");
            ReplyAscendSuccess = ConfigHelper.GetConfig("ReplyAscendSuccess", "渡劫成功，体重增加了 {0} kg，等级加一\n当前体重 {1} kg，当前等级 {2}");
            ReplyAscendFail = ConfigHelper.GetConfig("ReplyAscendFail", "渡劫失败，你的鲲已死亡");
            ReplyParamInvalid = ConfigHelper.GetConfig("ReplyParamInvalid", "指令格式错误{0}");
            ReplyNoTargetPlayer = ConfigHelper.GetConfig("ReplyNoTargetPlayer", "目标指定的玩家未注册");
            ReplyNoTargetPlayerKun = ConfigHelper.GetConfig("ReplyNoTargetKun", "目标指定的玩家未持有鲲");
            ReplyAttackInCD = ConfigHelper.GetConfig("ReplyAttackInCD", "攻击冷却中，下次可攻击时间：{0}");
            ReplyDevourInCD = ConfigHelper.GetConfig("ReplyDevourInCD", "吞噬冷却中，下次可吞噬时间：{0}");
            ReplyAttackSuccess = ConfigHelper.GetConfig("ReplyAttackSuccess", "{0} 对 {1}的{2} 发起攻击，攻击成功了！\n攻击方体重增长 {3} kg，现 {4} kg\n被攻击方体重减少 {5} kg，现 {6} kg");
            ReplyAttackFail = ConfigHelper.GetConfig("ReplyAttackFail", "{0} 对 {1}的{2} 发起攻击，攻击失败了！\n攻击方体重减少 {3} kg，现 {4} kg\n被攻击方体重增加 {5} kg，现 {6} kg");
            ReplyAttackEscaped = ConfigHelper.GetConfig("ReplyAttackEscaped", "{0} 对 {1}的{2} 发起攻击，对方逃脱了！");
            ReplyAttackSuccessAndTargetDead = ConfigHelper.GetConfig("ReplyAttackSuccessAndTargetDead", "{0} 对 {1}的{2} 发起攻击，攻击成功了！被攻击方伤重致死\n攻击方体重增长 {3} kg，现 {4} kg");
            ReplyAttackFailAndDead = ConfigHelper.GetConfig("ReplyAttackFailAndDead", "{0} 对 {1}的{2} 发起攻击，攻击失败了！自身伤重致死\n被攻击方体重增加 {3} kg，现 {4} kg");
            ReplyDevourSuccess = ConfigHelper.GetConfig("ReplyDevourSuccess", "{0} 吃掉了 {1}的{2}\n攻击方体重增长 {3} kg，现 {4} kg");
            ReplyDevourFail = ConfigHelper.GetConfig("ReplyDevourFail", "{0} 企图吃掉 {1}的{2}，但是失败了！\n攻击方体重减少 {3} kg，现 {4} kg\n被攻击方体重增加 {5} kg，现 {6} kg");
            ReplyDevourFailAndDead = ConfigHelper.GetConfig("ReplyDevourFailAndDead", "{0} 企图吃掉 {1}的{2}，但是失败了！反倒被对方吃掉\n被攻击方体重增加 {3} kg，现 {4} kg");
            ReplyDevourEscaped = ConfigHelper.GetConfig("ReplyAttackEscaped", "{0} 企图吃掉 {1}的{2}，对方逃脱了！");
            ReplyFeed = ConfigHelper.GetConfig("ReplyFeed", "你的「{0}」体重增加了 {1} 千克\n现体重为 {2} 千克");
            ReplyBlindBoxOpen = ConfigHelper.GetConfig("ReplyBlindBoxOpen", "打开 {0} 个盲盒，获得了以下物品：\n{1}");
            ReplyBlindBoxGetNothing = ConfigHelper.GetConfig("ReplyBlindBoxGetNothing", "打开 {0} 个盲盒，什么也没获得");
            ReplyOpenKunEgg = ConfigHelper.GetConfig("ReplyOpenKunEgg", "打开 {0} 个鲲蛋，获得了 {1} 个盲盒");
            ReplyReleaseSuccess = ConfigHelper.GetConfig("ReplyReleaseSuccess", "{0}放生成功");
            ReplyReleaseFail = ConfigHelper.GetConfig("ReplyReleaseFail", "放生失败，可能是鲲已死亡或不存在");
            ReplyQueryDeadKun = ConfigHelper.GetConfig("ReplyQueryDeadKun", "还可复活的鲲列表如下：\n");
            ReplyTransmogrifySuccess = ConfigHelper.GetConfig("ReplyTransmogrifySuccess", "幻化成功，{0} 转变为 {1}，体重减少 {2} kg，现 {3} kg\n剩余 {4} 颗幻化丹 {5} 枚金币");
            ReplyTransmogrifyFail = ConfigHelper.GetConfig("ReplyTransmogrifyFail", "幻化失败了，体重减少 {0} kg，现 {1} kg\n剩余 {2} 颗幻化丹 {3} 枚金币");
            ReplyTransmogrifyFailAndDead = ConfigHelper.GetConfig("ReplyTransmogrifyFailAndDead", "幻化失败并且魂飞魄散\n剩余 {0} 颗幻化丹 {1} 枚金币");
            ReplyTransmogrifyLevelLimit = ConfigHelper.GetConfig("ReplyTransmogrifyLevelLimit", "不能执行幻化，由于等级限制，当前等级 {0}，最低幻化等级：{1}");
            ReplyUpgradeSuccess = ConfigHelper.GetConfig("ReplyUpgradeSuccess", "强化完成，体重增加了 {0} kg，当前体重 {1} kg\n剩余 {2} 颗强化丹 {3} 枚金币");
            ReplyUpgradeFail = ConfigHelper.GetConfig("ReplyUpgradeFail", "强化失败，体重减少了 {0} kg，当前体重 {1} kg\n剩余 {2} 颗强化丹 {3} 枚金币");
            ReplyResurrectHourLimit = ConfigHelper.GetConfig("ReplyResurrectHourLimit", "无法复活，由于鲲死亡已超过 {0} 小时，当前死亡 {1} 小时");
            ReplyResurrectSuccess = ConfigHelper.GetConfig("ReplyResurrectSuccess", "鲲已复活");
            ReplyResurrectFail = ConfigHelper.GetConfig("ReplyResurrectFail", "复活失败，查看日志查询原因");
            ReplyWeightLimit = ConfigHelper.GetConfig("ReplyWeightLimit", "体重已达上限，需进行渡劫提高体重上限");

            ProbablityNone = ConfigHelper.GetConfig("ProbablityNone", 70);
            ProbablityJin = ConfigHelper.GetConfig("ProbablityJin", 5);
            ProbablityMu = ConfigHelper.GetConfig("ProbablityMu", 5);
            ProbablityShui = ConfigHelper.GetConfig("ProbablityShui", 5);
            ProbablityTu = ConfigHelper.GetConfig("ProbablityTu", 5);
            ProbablityHuo = ConfigHelper.GetConfig("ProbablityHuo", 5);
            ProbablityFeng = ConfigHelper.GetConfig("ProbablityFeng", 2);
            ProbablityLei = ConfigHelper.GetConfig("ProbablityLei", 2);
            ProbablityYin = ConfigHelper.GetConfig("ProbablityYin", 0.5);
            ProbablityYang = ConfigHelper.GetConfig("ProbablityYang", 0.5);

            ValueHatchProbablityMin = ConfigHelper.GetConfig("ValueHatchProbablityMin", 10);
            ValueHatchProbablityMax = ConfigHelper.GetConfig("ValueHatchProbablityMax", 50);
            ValueHatchWeightMin = ConfigHelper.GetConfig("ValueHatchWeightMin", 10);
            ValueHatchWeightMax = ConfigHelper.GetConfig("ValueHatchWeightMax", 10000);
            ValueEggValue = ConfigHelper.GetConfig("ValueEggValue", 100);
            ValueFeedCoinConsume = ConfigHelper.GetConfig("ValueFeedCoinConsume", 10);
            ValueAscendCoinConsume = ConfigHelper.GetConfig("ValueAscendCoinConsume", 100);
            ValueFeedKunEggConsume = ConfigHelper.GetConfig("ValueFeedKunEggConsume", 1);
            ValueFeedWeightMinimumIncrement = ConfigHelper.GetConfig("ValueFeedWeightMinimumIncrement", 10);
            ValueFeedWeightMaximumIncrement = ConfigHelper.GetConfig("ValueFeedWeightMaximumIncrement", 40);
            ValueAttackWeightMinimumDecrement = ConfigHelper.GetConfig("ValueAttackWeightMinimumDecrement", 5);
            ValueAttackWeightMaximumDecrement = ConfigHelper.GetConfig("ValueAttackWeightMaximumDecrement", 10);
            ValueRankingCount = ConfigHelper.GetConfig("ValueRankingCount", 10);
            ValueDevourDrawPercentage = ConfigHelper.GetConfig("ValueDevourDrawPercentage", 10);
            ValueLoginCoinReward = ConfigHelper.GetConfig("ValueLoginCoinReward", 100);
            ValueLoginEggReward = ConfigHelper.GetConfig("ValueLoginEggReward", 10);
            ValueAscendFailDeadProbablity = ConfigHelper.GetConfig("ValueAscendFailDeadProbablity", 10);
            ValueDevourFailDeadProbablity = ConfigHelper.GetConfig("ValueDevourFailDeadProbablity", 20);
            ValueTransmoirgifyDeadWeightLimit = ConfigHelper.GetConfig("ValueTransmoirgifyFailDeadWeightLimit", 10);
            ValueTransmoirgifyFailDeadProbablity = ConfigHelper.GetConfig("ValueTransmoirgifyFailDeadProbablity", 10);
            ValueAttackCD = ConfigHelper.GetConfig("ValueAttackCD", 30);
            ValueDevourCD = ConfigHelper.GetConfig("ValueDevourCD", 30);
            ValueKunEggToBlindBoxRate = ConfigHelper.GetConfig("ValueKunEggToCoinRate", 1);
            ValueMaxResurrectHour = ConfigHelper.GetConfig("ValueMaxDeadHour", 81);
            ValuePerTwoHourWeightLoss = ConfigHelper.GetConfig("ValuePerTwoHourWeightLoss", 1);
            ValuePerEighteenHourLevelLoss = ConfigHelper.GetConfig("ValuePerEighteenHourLevelLoss", 1);
            ValueTranmogifyCoinConsume = ConfigHelper.GetConfig("ValueTranmogifyCoinConsume", 100);
            ValueTranmogifyPillConsume = ConfigHelper.GetConfig("ValueTranmogifyPillConsume", 1);
            ValueTransmogrifyLevelLimit = ConfigHelper.GetConfig("ValueTransmogrifyLevelLimit", 5);
            ValueUpgradeCoinConsume = ConfigHelper.GetConfig("ValueUpgradeCoinConsume", 100);
            ValueUpgradePillConsume = ConfigHelper.GetConfig("ValueUpgradePillConsume", 1);
            
            BlindBoxContents = ConfigHelper.GetConfig("BlindBoxContents", new List<string>() { "0|75", "4|8", "5|8", "6|8" });

            Groups = ConfigHelper.GetConfig("Groups", new List<long>());
            Admins = ConfigHelper.GetConfig("Admins", new List<long>());
        }

        public static void EnableAutoReload()
        {
            ConfigChangeWatcher.Path = Path.GetDirectoryName(ConfigHelper.ConfigPath);
            ConfigChangeWatcher.Filter = Path.GetFileName(ConfigHelper.ConfigPath);
            ConfigChangeWatcher.NotifyFilter = NotifyFilters.LastWrite;
            ConfigChangeWatcher.Changed -= ConfigChangeWatcher_Changed;
            ConfigChangeWatcher.Changed += ConfigChangeWatcher_Changed;
            ConfigChangeWatcher.EnableRaisingEvents = true;
        }

        public static void DisableAutoReload()
        {
            ConfigChangeWatcher.EnableRaisingEvents = false;
        }

        private static void ConfigChangeWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            try
            {
                if (e.ChangeType == WatcherChangeTypes.Changed && ConfigHelper.Load())
                {
                    LoadConfig();
                }
            }
            catch
            {
            }
        }
    }
}