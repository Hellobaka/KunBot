using System.Collections.Generic;
using System.IO;

namespace me.cqp.luohuaming.iKun.PublicInfos
{
    public static class AppConfig
    {
        private static FileSystemWatcher ConfigChangeWatcher { get; set; } = new();

        public static int RankingCount { get; set; } = 10;

        public static string CommandRegister { get; set; } = "";

        public static string CommandLogin { get; set; } = "";
        
        public static string CommandMenu { get; set; } = "";
        
        public static string CommandRanking { get; set; } = "";

        public static string CommandInventory { get; set; } = "";

        public static string CommandHatch { get; set; } = "";

        public static string CommandFeed { get; set; } = "";

        public static string CommandUpgrade { get; set; } = "";

        public static string CommandTransmogrify { get; set; } = "";

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

        public static string ReplyNoKun { get; set; } = "";

        public static string ReplyLoginReward { get; set; } = "";

        public static string ReplyItemLeak { get; set; } = "";

        public static string ReplyHatchKun { get; set; } = "";

        public static string ReplyBuyEgg { get; set; } = "";

        public static string ReplyHatchFail { get; set; } = "";

        public static double ProbablityNone { get; set; }

        public static double ProbablityJin { get; set; }
       
        public static double ProbablityHuo { get; set; }
       
        public static double ProbablityFeng { get; set; }
       
        public static double ProbablityTu { get; set; }
       
        public static double ProbablityLei { get; set; }
       
        public static double ProbablityShui { get; set; }
       
        public static double ProbablityYin { get; set; }
       
        public static double ProbablityMu { get; set; }

        public static int ValueHatchProbablityMin { get; set; }

        public static int ValueHatchProbablityMax { get; set; }

        public static int ValueHatchWeightMin { get; set; }

        public static int ValueHatchWeightMax { get; set; }

        public static int ValueEggValue { get; set; }

        public static int ValueFeedCoinConsume { get; set; }

        public static int ValueFeedKunEggConsume { get; set; }

        public static int ValueFeedWeightMinimumIncrement { get; set; }

        public static int ValueFeedWeightMaximumIncrement { get; set; }

        public static int ValueRankingCount { get; set; }

        public static int ValueDevourDrawPercentage { get; set; }

        public static int ValueAttackWeightMinimumDecrement { get; set; }

        public static int ValueAttackWeightMaximumDecrement { get; set; }

        public static List<long> Groups { get; set; } = [];

        public static List<long> Admins { get; set; } = [];

        public static void LoadConfig()
        {
            RankingCount = ConfigHelper.GetConfig("RankingCount", 10);

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

            ReplyDuplicateRegister = ConfigHelper.GetConfig("ReplyDuplicateRegister", "你已经注册过了，不能重复注册");
            ReplyNewRegister = ConfigHelper.GetConfig("ReplyNewRegister", "注册成功，赠送 {0} 枚金币以及 {1} 枚鲲蛋");
            ReplyRegisterFailed = ConfigHelper.GetConfig("ReplyRegisterFailed", "注册失败了");
            ReplyMenu = ConfigHelper.GetConfig("ReplyMenu", "菜单：");
            ReplyNoPlayer = ConfigHelper.GetConfig("ReplyNoPlayer", "请先注册");
            ReplyNoKun = ConfigHelper.GetConfig("ReplyNoKun", "未持有鲲");
            ReplyLoginReward = ConfigHelper.GetConfig("ReplyLoginReward", "签到成功，赠送 {0} 枚金币以及 {1} 枚鲲蛋");
            ReplyItemLeak = ConfigHelper.GetConfig("ReplyItemLeak", "{0}数量不足，需要{1}个，现有{2}个");
            ReplyHatchFail = ConfigHelper.GetConfig("ReplyHatchFail", "孵化失败\n-------------------\n剩余 {0} 颗鲲蛋");
            ReplyHatchKun = ConfigHelper.GetConfig("ReplyHatchKun", "恭喜你获得一只{0}\n体重 {1} 千克\n-------------------\n剩余 {2} 颗鲲蛋");
            ReplyBuyEgg = ConfigHelper.GetConfig("ReplyBuyEgg", "购买成功，消耗金币 {0} 枚，获得 {1} 枚鲲蛋\n-------------------\n剩余金币 {2} 枚，鲲蛋 {3} 枚");

            ProbablityNone = ConfigHelper.GetConfig("ProbablityNone", 1);
            ProbablityJin = ConfigHelper.GetConfig("ProbablityJin", 1);
            ProbablityMu = ConfigHelper.GetConfig("ProbablityMu", 1);
            ProbablityShui = ConfigHelper.GetConfig("ProbablityShui", 1);
            ProbablityTu = ConfigHelper.GetConfig("ProbablityTu", 1);
            ProbablityHuo = ConfigHelper.GetConfig("ProbablityHuo", 1);
            ProbablityFeng = ConfigHelper.GetConfig("ProbablityFeng", 1);
            ProbablityLei = ConfigHelper.GetConfig("ProbablityLei", 1);
            ProbablityYin = ConfigHelper.GetConfig("ProbablityYin", 1);

            ValueHatchProbablityMin = ConfigHelper.GetConfig("ValueHatchProbablityMin", 1);
            ValueHatchProbablityMax = ConfigHelper.GetConfig("ValueHatchProbablityMax", 1);
            ValueHatchWeightMin = ConfigHelper.GetConfig("ValueHatchWeightMin", 1);
            ValueHatchWeightMax = ConfigHelper.GetConfig("ValueHatchWeightMax", 1);
            ValueEggValue = ConfigHelper.GetConfig("ValueEggValue", 100);
            ValueFeedCoinConsume = ConfigHelper.GetConfig("ValueFeedCoinConsume", 100);
            ValueFeedKunEggConsume = ConfigHelper.GetConfig("ValueFeedKunEggConsume", 10);
            ValueFeedWeightMinimumIncrement = ConfigHelper.GetConfig("ValueFeedWeightMinimumIncrement", 10);
            ValueFeedWeightMaximumIncrement = ConfigHelper.GetConfig("ValueFeedWeightMaximumIncrement", 40);
            ValueAttackWeightMinimumDecrement = ConfigHelper.GetConfig("ValueAttackWeightMinimumDecrement", 5);
            ValueAttackWeightMaximumDecrement = ConfigHelper.GetConfig("ValueAttackWeightMaximumDecrement", 10);
            ValueRankingCount = ConfigHelper.GetConfig("ValueRankingCount", 10);
            ValueDevourDrawPercentage = ConfigHelper.GetConfig("ValueDevourDrawPercentage", 10);

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
