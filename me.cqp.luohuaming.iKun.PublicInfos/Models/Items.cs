namespace me.cqp.luohuaming.iKun.PublicInfos.Models
{
    public class Items
    {
        public Enums.Items ID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool Stackable { get; set; } = true;

        public int Count { get; set; }

        public static Items Coin(int count = 1) => new PublicInfos.Items.Coin(count);

        public static Items KunEgg(int count = 1) => new PublicInfos.Items.KunEgg(count);

        public static Items TransmogrifyPill(int count = 1) => new PublicInfos.Items.TransmogrifyPill(count);

        public static Items BlindBox(int count = 1) => new PublicInfos.Items.BlindBox(count);

        public static Items ResurrectPill(int count = 1) => new PublicInfos.Items.ResurrectPill(count);

        public static Items UpgradePill(int count = 1) => new PublicInfos.Items.UpgradePill(count);

        public static Items AscendPill(int count = 1) => new PublicInfos.Items.AscendPill(count);

        public static Items GetItemByID(Enums.Items id)
        {
            switch (id)
            {
                case Enums.Items.Coin:
                    return new PublicInfos.Items.Coin();

                case Enums.Items.KunEgg:
                    return new PublicInfos.Items.KunEgg();

                case Enums.Items.BlindBox:
                    return new PublicInfos.Items.BlindBox();

                case Enums.Items.ResurrectPill:
                    return new PublicInfos.Items.ResurrectPill();

                case Enums.Items.TransmogrifyPill:
                    return new PublicInfos.Items.TransmogrifyPill();

                case Enums.Items.UpgradePill:
                    return new PublicInfos.Items.UpgradePill();

                case Enums.Items.AscendPill:
                    return new PublicInfos.Items.AscendPill();

                default:
                    break;
            }

            return null;
        }

        public override string ToString()
        {
            return $"{Name} {Count}";
        }
    }
}