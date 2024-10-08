namespace me.cqp.luohuaming.iKun.PublicInfos.Items
{
    public class ItemConfig : ConfigBase
    {
        public ItemConfig(string path)
            : base(path)
        {
            LoadConfig();
            Instance = this;
        }

        public static ItemConfig Instance { get; private set; }

        public static string BlindBoxName { get; set; }

        public static string CoinName { get; set; }

        public static string KunEggName { get; set; }

        public static string ResurrectPillName { get; set; }

        public static string TransmogrifyPillName { get; set; }

        public static string UpgradePillName { get; set; }

        public static string BlindBoxDescription { get; set; }

        public static string CoinDescription { get; set; }

        public static string KunEggDescription { get; set; }

        public static string ResurrectPillDescription { get; set; }

        public static string TransmogrifyPillDescription { get; set; }

        public static string UpgradePillDescription { get; set; }

        public override void LoadConfig()
        {
            BlindBoxName = GetConfig("BlindBoxName", "盲盒");
            CoinName = GetConfig("CoinName", "金币");
            KunEggName = GetConfig("KunEggName", "鲲之蛋");
            ResurrectPillName = GetConfig("ResurrectPillName", "复活丸");
            TransmogrifyPillName = GetConfig("TransmogrifyPillName", "幻化丸");
            UpgradePillName = GetConfig("UpgradePillName", "强化丸");

            BlindBoxDescription = GetConfig("BlindBoxDescription", "能获得随机材料");
            CoinDescription = GetConfig("CoinDescription", "大陆上通用的货币");
            KunEggDescription = GetConfig("KunEggDescription", "可用于孵化、强化鲲");
            ResurrectPillDescription = GetConfig("ResurrectPillDescription", "用于复活的道具，能复活鲲");
            TransmogrifyPillDescription = GetConfig("TransmogrifyPillDescription", "用于幻化的道具，能够随机更改鲲的词缀");
            UpgradePillDescription = GetConfig("UpgradePillDescription", "用于强化的道具，能用于强化鲲");
        }
    }
}
