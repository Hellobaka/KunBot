using me.cqp.luohuaming.iKun.PublicInfos.PetAttribute;
using Newtonsoft.Json;
using System;
using System.Reflection;

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
