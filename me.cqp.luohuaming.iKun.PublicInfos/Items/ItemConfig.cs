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

        public static string AscendPillName { get; set; }

        public static string AscendPillDescription { get; set; }

        public static string LevelPillName { get; set; }

        public static string LevelPillDescription { get; set; }

        public static string WeightPillName { get; set; }

        public static string WeightPillDescription { get; set; }

        public static string UseLevelPill {  get; set; }

        public static string UseWeightPill {  get; set; }

        public static string UseItemException {  get; set; }

        public override void LoadConfig()
        {
            BlindBoxName = GetConfig("BlindBoxName", "盲盒");
            CoinName = GetConfig("CoinName", "金币");
            KunEggName = GetConfig("KunEggName", "鲲之蛋");
            ResurrectPillName = GetConfig("ResurrectPillName", "复活丸");
            TransmogrifyPillName = GetConfig("TransmogrifyPillName", "幻化丸");
            UpgradePillName = GetConfig("UpgradePillName", "强化丸");
            AscendPillName = GetConfig("AscendPillName", "渡劫丹");
            LevelPillName = GetConfig("LevelPillName", "快速等级丹");
            WeightPillName = GetConfig("WeightPillName", "快速体重丹");

            BlindBoxDescription = GetConfig("BlindBoxDescription", "能获得随机材料");
            CoinDescription = GetConfig("CoinDescription", "大陆上通用的货币");
            KunEggDescription = GetConfig("KunEggDescription", "可用于孵化、强化鲲");
            ResurrectPillDescription = GetConfig("ResurrectPillDescription", "用于复活的道具，能复活鲲");
            TransmogrifyPillDescription = GetConfig("TransmogrifyPillDescription", "用于幻化的道具，能够随机更改鲲的词缀");
            UpgradePillDescription = GetConfig("UpgradePillDescription", "用于强化的道具，能用于强化鲲");
            AscendPillDescription = GetConfig("AscendPillDescription", "能够临时提升渡劫成功率的道具");
            LevelPillDescription = GetConfig("LevelPillDescription", "能够迅速提升等级的道具");
            WeightPillDescription = GetConfig("WeightPillDescription", "能够迅速提升体重的道具");

            UseItemException = GetConfig("UseItemException", "物品使用过程发生异常，排查日志解决问题");
            UseLevelPill = GetConfig("UseLevelPill", "使用了 {0} 个快速等级丹，等级提升了 {1}，当前等级 {2}");
            UseWeightPill = GetConfig("UseWeightPill", "使用了快速体重丹，当前体重为 {0} kg");
        }
    }
}
