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
