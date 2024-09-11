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

        public static string CommandStrike { get; set; } = "";

        public static string CommandBuyEgg { get; set; } = "";

        public static string CommandOpenEgg { get; set; } = "";

        public static string CommandOpenBlindBox { get; set; } = "";

        public static string ReplyDuplicateRegister { get; set; } = "";

        public static string ReplyNewRegister { get; set; } = "";

        public static string ReplyRegisterFailed { get; set; } = "";

        public static string ReplyMenu { get; set; } = "";

        public static string ReplyNoPlayer { get; set; } = "";

        public static string ReplyLoginReward { get; set; } = "";

        public static string ReplyItemLeak { get; set; } = "";

        public static string ReplyHatchKun { get; set; } = "";

        public static string ReplyBuyEgg { get; set; } = "";

        public static string ReplyHatchFail { get; set; } = "";

        public static double ProbablityAo { get; set; }
       
        public static double ProbablityDu { get; set; }
       
        public static double ProbablityNu { get; set; }
       
        public static double ProbablityDuo { get; set; }
       
        public static double ProbablityTan { get; set; }
       
        public static double ProbablityChan { get; set; }
       
        public static double ProbablityYin { get; set; }
       
        public static double ProbablityBei { get; set; }

        public static List<double> ProbablityHatchLevel { get; set; } = [];

        public static int ValueHatchProbablityMin { get; set; }

        public static int ValueHatchProbablityMax { get; set; }

        public static int ValueHatchWeightMin { get; set; }

        public static int ValueHatchWeightMax { get; set; }

        public static int ValueEggValue { get; set; }

        public static List<long> Groups { get; set; } = [];

        public static List<long> Admins { get; set; } = [];

        public static void LoadConfig()
        {
            RankingCount = ConfigHelper.GetConfig("RankingCount", 10);

            CommandAscend = ConfigHelper.GetConfig("CommandAscend", "");
            CommandAttack = ConfigHelper.GetConfig("CommandAttack", "");
            CommandBuyEgg = ConfigHelper.GetConfig("CommandBuyEgg", "");
            CommandDevour = ConfigHelper.GetConfig("CommandDevour", "");
            CommandFeed = ConfigHelper.GetConfig("CommandFeed", "");
            CommandHatch = ConfigHelper.GetConfig("CommandHatch", "");
            CommandInventory = ConfigHelper.GetConfig("CommandInventory", "");
            CommandLogin = ConfigHelper.GetConfig("CommandLogin", "");
            CommandMenu = ConfigHelper.GetConfig("CommandMenu", "");
            CommandOpenBlindBox = ConfigHelper.GetConfig("CommandOpenBlindBox", "");
            CommandOpenEgg = ConfigHelper.GetConfig("CommandOpenEgg", "");
            CommandRanking = ConfigHelper.GetConfig("CommandRanking", "");
            CommandReleaseKun = ConfigHelper.GetConfig("CommandReleaseKun", "");
            CommandResurrect = ConfigHelper.GetConfig("CommandResurrect", "");
            CommandStrike = ConfigHelper.GetConfig("CommandStrike", "");
            CommandTransmogrify = ConfigHelper.GetConfig("CommandTransmogrify", "");
            CommandUpgrade = ConfigHelper.GetConfig("CommandUpgrade", "");
            CommandRegister = ConfigHelper.GetConfig("CommandRegister", "");

            ReplyDuplicateRegister = ConfigHelper.GetConfig("ReplyDuplicateRegister", "你已经注册过了，不能重复注册");
            ReplyNewRegister = ConfigHelper.GetConfig("ReplyNewRegister", "注册成功，赠送 {0} 枚金币以及 {1} 枚鲲蛋");
            ReplyRegisterFailed = ConfigHelper.GetConfig("ReplyRegisterFailed", "注册失败了");
            ReplyMenu = ConfigHelper.GetConfig("ReplyMenu", "菜单：");
            ReplyNoPlayer = ConfigHelper.GetConfig("ReplyNoPlayer", "请先注册");
            ReplyLoginReward = ConfigHelper.GetConfig("ReplyLoginReward", "签到成功，赠送 {0} 枚金币以及 {1} 枚鲲蛋");
            ReplyItemLeak = ConfigHelper.GetConfig("ReplyItemLeak", "道具数量不足，需要{0}个，现有{1}个");
            ReplyHatchFail = ConfigHelper.GetConfig("ReplyHatchFail", "孵化失败\n-------------------\n剩余 {0} 颗鲲蛋");
            ReplyHatchKun = ConfigHelper.GetConfig("ReplyHatchKun", "恭喜你获得一只{0}\n体重 {1} 千克\n-------------------\n剩余 {2} 颗鲲蛋");
            ReplyBuyEgg = ConfigHelper.GetConfig("ReplyBuyEgg", "购买成功，消耗金币 {0} 枚，获得 {1} 枚鲲蛋\n-------------------\n剩余金币 {2} 枚，鲲蛋 {3} 枚");

            ProbablityAo = ConfigHelper.GetConfig("ProbablityAo", 1);
            ProbablityBei = ConfigHelper.GetConfig("ProbablityBei", 1);
            ProbablityChan = ConfigHelper.GetConfig("ProbablityChan", 1);
            ProbablityDuo = ConfigHelper.GetConfig("ProbablityDuo", 1);
            ProbablityDu = ConfigHelper.GetConfig("ProbablityDu", 1);
            ProbablityNu = ConfigHelper.GetConfig("ProbablityNu", 1);
            ProbablityTan = ConfigHelper.GetConfig("ProbablityTan", 1);
            ProbablityYin = ConfigHelper.GetConfig("ProbablityYin", 1);
            ProbablityHatchLevel = ConfigHelper.GetConfig("ProbablityHatchLevel", new List<double>() { 1, 1, 1, 1, 1, 1, 1, 1 });

            ValueHatchProbablityMin = ConfigHelper.GetConfig("ValueHatchProbablityMin", 1);
            ValueHatchProbablityMax = ConfigHelper.GetConfig("ValueHatchProbablityMax", 1);
            ValueHatchWeightMin = ConfigHelper.GetConfig("ValueHatchWeightMin", 1);
            ValueHatchWeightMax = ConfigHelper.GetConfig("ValueHatchWeightMax", 1);
            ValueEggValue = ConfigHelper.GetConfig("ValueEggValue", 100);

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
