using me.cqp.luohuaming.iKun.PublicInfos.Items;

namespace me.cqp.luohuaming.iKun.PublicInfos.Models
{
    public class Items
    {
        public Enums.Items ID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool Stackable { get; set; } = true;

        public int Count { get; set; }

        public bool Usable { get; set; }

        public static Items Coin(int count = 1) => new Coin(count);

        public static Items KunEgg(int count = 1) => new KunEgg(count);

        public static Items TransmogrifyPill(int count = 1) => new TransmogrifyPill(count);

        public static Items BlindBox(int count = 1) => new BlindBox(count);

        public static Items ResurrectPill(int count = 1) => new ResurrectPill(count);

        public static Items UpgradePill(int count = 1) => new UpgradePill(count);

        public static Items AscendPill(int count = 1) => new AscendPill(count);

        public static Items LevelPill(int count = 1) => new LevelPill(count);

        public static Items WeightPill(int count = 1) => new WeightPill(count);

        public static Items GetItemByName(string name)
        {
            if (name == ItemConfig.CoinName)
            {
                return Coin();
            }
            else if (name == ItemConfig.KunEggName)
            {
                return KunEgg();
            }
            else if (name == ItemConfig.TransmogrifyPillName)
            {
                return TransmogrifyPill();
            }
            else if (name == ItemConfig.BlindBoxName)
            {
                return BlindBox();
            }
            else if (name == ItemConfig.ResurrectPillName)
            {
                return ResurrectPill();
            }
            else if (name == ItemConfig.UpgradePillName)
            {
                return UpgradePill();
            }
            else if (name == ItemConfig.AscendPillName)
            {
                return AscendPill();
            }
            else if (name == ItemConfig.LevelPillName)
            {
                return LevelPill();
            }
            else if (name == ItemConfig.WeightPillName)
            {
                return WeightPill();
            }

            return null;
        }

        public static Items GetItemByID(Enums.Items id)
        {
            return id switch
            {
                Enums.Items.Coin => new Coin(),
                Enums.Items.KunEgg => new KunEgg(),
                Enums.Items.BlindBox => new BlindBox(),
                Enums.Items.ResurrectPill => new ResurrectPill(),
                Enums.Items.TransmogrifyPill => new TransmogrifyPill(),
                Enums.Items.UpgradePill => new UpgradePill(),
                Enums.Items.AscendPill => new AscendPill(),
                Enums.Items.LevelPill => new LevelPill(),
                Enums.Items.WeightPill => new WeightPill(),
                _ => null,
            };
        }

        /// <summary>
        /// 使用物品
        /// 默认完成了道具扣除
        /// 要求对象可用，使用前检查对象状态
        /// 方法内不针对对象进行二次检查
        /// </summary>
        /// <returns>使用是否成功，使用反馈</returns>
        public virtual (bool, string) UseItem(int count, Player player, Kun kun)
        {
            return (false, "物品没有使用效果");
        }

        public override string ToString()
        {
            return $"{Name} {Count}";
        }
    }
}
