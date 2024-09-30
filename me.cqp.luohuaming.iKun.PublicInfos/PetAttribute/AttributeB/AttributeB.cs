using System.Collections.Generic;

namespace me.cqp.luohuaming.iKun.PublicInfos.PetAttribute.AttributeB
{
    public class AttributeB : IPetAttribute
    {
        public AttributeB(int id)
        {
            ID = Enums.Attribute.AttributeB;
            AttrbiuteBID = id;
            if (!AttributeBMap.TryGetValue(id, out var item))
            {
                item = AttributeBMap[79];
            }
            AttributeBAction = item.Item2;
            AppendValue = item.Item3;
            Name = item.Item1;
            Description = [
                string.Format(ActionName[AttributeBAction], AppendValueName[AppendValue])
            ];
            Logger = new Logger($"小词缀_{Name}");
        }

        private Logger Logger { get; set; }

        private static Dictionary<int, (string, Enums.AttributeBAction, double)> AttributeBMap { get; set; } = new()
        {
            { 1, ("白", Enums.AttributeBAction.UpgradeWeightGainUpper, 0.01) },
            { 2, ("灰", Enums.AttributeBAction.UpgradeWeightGainUpper, 0.05) },
            { 3, ("黑", Enums.AttributeBAction.UpgradeWeightGainUpper, 0.1) },
            { 4, ("墨", Enums.AttributeBAction.UpgradeWeightGainUpper, 0.15) },
            { 5, ("晶", Enums.AttributeBAction.UpgradeWeightGainUpper, 0.15) },
            { 6, ("锐", Enums.AttributeBAction.AttackWeightGainUpper, 0.01) },
            { 7, ("密", Enums.AttributeBAction.AttackWeightGainUpper, 0.05) },
            { 8, ("厚", Enums.AttributeBAction.AttackWeightGainUpper, 0.1) },
            { 9, ("重", Enums.AttributeBAction.AttackWeightGainUpper, 0.15) },
            { 10, ("野", Enums.AttributeBAction.AttackDamageUpper, 0.01) },
            { 11, ("勇", Enums.AttributeBAction.AttackDamageUpper, 0.05) },
            { 12, ("雪", Enums.AttributeBAction.AttackDamageUpper, 0.05) },
            { 13, ("霜", Enums.AttributeBAction.AttackDamageUpper, 0.05) },
            { 14, ("风", Enums.AttributeBAction.AttackDamageUpper, 0.05) },
            { 15, ("雷", Enums.AttributeBAction.AttackDamageUpper, 0.05) },
            { 16, ("电", Enums.AttributeBAction.AttackDamageUpper, 0.05) },
            { 17, ("金", Enums.AttributeBAction.AttackDamageUpper, 0.05) },
            { 18, ("木", Enums.AttributeBAction.AttackDamageUpper, 0.05) },
            { 19, ("水", Enums.AttributeBAction.AttackDamageUpper, 0.05) },
            { 20, ("火", Enums.AttributeBAction.AttackDamageUpper, 0.05) },
            { 21, ("烈", Enums.AttributeBAction.AttackDamageUpper, 0.05) },
            { 22, ("油", Enums.AttributeBAction.AttackDamageUpper, 0.05) },
            { 23, ("幽", Enums.AttributeBAction.AttackDamageUpper, 0.05) },
            { 24, ("刺", Enums.AttributeBAction.AttackDamageUpper, 0.05) },
            { 25, ("鳞", Enums.AttributeBAction.AttackDamageUpper, 0.05) },
            { 26, ("腐", Enums.AttributeBAction.AttackDamageUpper, 0.05) },
            { 27, ("牙", Enums.AttributeBAction.AttackDamageUpper, 0.05) },
            { 28, ("尖", Enums.AttributeBAction.AttackDamageUpper, 0.05) },
            { 29, ("刺", Enums.AttributeBAction.AttackDamageUpper, 0.05) },
            { 30, ("土", Enums.AttributeBAction.AttackDamageUpper, 0.05) },
            { 31, ("强", Enums.AttributeBAction.AttackDamageUpper, 0.1) },
            { 32, ("猛", Enums.AttributeBAction.AttackDamageUpper, 0.1) },
            { 33, ("傲", Enums.AttributeBAction.AttackDamageUpper, 0.15) },
            { 34, ("奇", Enums.AttributeBAction.AttackDamageUpper, 0.15) },
            { 35, ("毒", Enums.AttributeBAction.AttackDamageUpper, 0.15) },
            { 36, ("聪", Enums.AttributeBAction.BeingAttackedDamageLower, 0.01) },
            { 37, ("狡", Enums.AttributeBAction.BeingAttackedDamageLower, 0.05) },
            { 38, ("怒", Enums.AttributeBAction.BeingAttackedDamageLower, 0.1) },
            { 39, ("凶", Enums.AttributeBAction.BeingAttackedDamageLower, 0.15) },
            { 40, ("稳", Enums.AttributeBAction.BeingAttackedDamageLower, 0.15) },
            { 41, ("肉", Enums.AttributeBAction.BeingAttackedDamageLower, 0.15) },
            { 42, ("盾", Enums.AttributeBAction.BeingAttackedDamageLower, 0.15) },
            { 43, ("硬", Enums.AttributeBAction.BeingAttackedDamageLower, 0.15) },
            { 44, ("巨", Enums.AttributeBAction.BeingAttackedDamageLower, 0.15) },
            { 45, ("大", Enums.AttributeBAction.BeingAttackedDamageLower, 0.15) },
            { 46, ("蓝", Enums.AttributeBAction.AscendWeightGainUpper, 0.01) },
            { 47, ("粉", Enums.AttributeBAction.AscendWeightGainUpper, 0.01) },
            { 48, ("绿", Enums.AttributeBAction.AscendWeightGainUpper, 0.05) },
            { 49, ("黄", Enums.AttributeBAction.AscendWeightGainUpper, 0.1) },
            { 50, ("橙", Enums.AttributeBAction.AscendWeightGainUpper, 0.15) },
            { 51, ("碧", Enums.AttributeBAction.AscendSuccessRateUpper, 0.01) },
            { 52, ("敏", Enums.AttributeBAction.AscendSuccessRateUpper, 0.01) },
            { 53, ("紫", Enums.AttributeBAction.AscendSuccessRateUpper, 0.05) },
            { 54, ("红", Enums.AttributeBAction.AscendSuccessRateUpper, 0.1) },
            { 55, ("朱", Enums.AttributeBAction.AscendSuccessRateUpper, 0.1) },
            { 56, ("彩", Enums.AttributeBAction.AscendSuccessRateUpper, 0.15) },
            { 57, ("傻", Enums.AttributeBAction.AscendSuccessRateUpper, 0.15) },
            { 58, ("鸣", Enums.AttributeBAction.AscendFailWeightLostLower, 0.01) },
            { 59, ("懒", Enums.AttributeBAction.AscendFailWeightLostLower, 0.01) },
            { 60, ("游", Enums.AttributeBAction.AscendFailWeightLostLower, 0.05) },
            { 61, ("琥", Enums.AttributeBAction.AscendFailWeightLostLower, 0.1) },
            { 62, ("苍", Enums.AttributeBAction.AscendFailWeightLostLower, 0.15) },
            { 63, ("铜", Enums.AttributeBAction.FeedWeightGainUpper, 0.01) },
            { 64, ("铁", Enums.AttributeBAction.FeedWeightGainUpper, 0.05) },
            { 65, ("银", Enums.AttributeBAction.FeedWeightGainUpper, 0.1) },
            { 66, ("金", Enums.AttributeBAction.FeedWeightGainUpper, 0.15) },
            { 67, ("贪", Enums.AttributeBAction.FeedWeightGainUpper, 0.15) },
            { 68, ("骄", Enums.AttributeBAction.TransmogrifySuccessRateUpper, 0.01) },
            { 69, ("炫", Enums.AttributeBAction.TransmogrifySuccessRateUpper, 0.01) },
            { 70, ("典", Enums.AttributeBAction.TransmogrifySuccessRateUpper, 0.05) },
            { 71, ("幽", Enums.AttributeBAction.TransmogrifySuccessRateUpper, 0.1) },
            { 72, ("古", Enums.AttributeBAction.TransmogrifySuccessRateUpper, 0.15) },
            { 73, ("蠢", Enums.AttributeBAction.TransmogrifySuccessRateUpper, 0.15) },
            { 74, ("迅", Enums.AttributeBAction.TransmogrifyFailWeightLostLower, 0.01) },
            { 75, ("柔", Enums.AttributeBAction.TransmogrifyFailWeightLostLower, 0.05) },
            { 76, ("乖", Enums.AttributeBAction.TransmogrifyFailWeightLostLower, 0.05) },
            { 77, ("灵", Enums.AttributeBAction.TransmogrifyFailWeightLostLower, 0.1) },
            { 78, ("耀", Enums.AttributeBAction.TransmogrifyFailWeightLostLower, 0.15) },
            { 79, ("无", Enums.AttributeBAction.None, 0) },
            { 80, ("菜虚", Enums.AttributeBAction.BeingAttackedDamageLower, 0.15) },
        };

        private static Dictionary<Enums.AttributeBAction, string> ActionName { get; set; } = new()
        {
            { Enums.AttributeBAction.UpgradeWeightGainUpper, "◇{0}提升洗髓时获取的体重"},
            { Enums.AttributeBAction.AttackWeightGainUpper, "◇{0}提升攻击后获得的体重"},
            { Enums.AttributeBAction.AttackDamageUpper, "◇{0}提升攻击时造成的伤害"},
            { Enums.AttributeBAction.BeingAttackedDamageLower, "◇{0}降低攻击时受到的伤害"},
            { Enums.AttributeBAction.AscendWeightGainUpper, "◇{0}增加渡劫成功时提升的体重"},
            { Enums.AttributeBAction.AscendSuccessRateUpper, "◇{0}提升渡劫成功率"},
            { Enums.AttributeBAction.AscendFailWeightLostLower, "◇{0}降低渡劫失败时损失的体重"},
            { Enums.AttributeBAction.FeedWeightGainUpper, "◇{0}提升喂食时获得的体重"},
            { Enums.AttributeBAction.TransmogrifySuccessRateUpper, "◇{0}提升蜕变成功率"},
            { Enums.AttributeBAction.TransmogrifyFailWeightLostLower, "◇{0}降低蜕变失败时损失的体重" },
            { Enums.AttributeBAction.None, "◇无属性" }
        };

        private static Dictionary<double, string> AppendValueName { get; set; } = new()
        {
            { 0, "" },
            { 0.01, "微小幅度" },
            { 0.05, "小幅度" },
            { 0.1, "中幅度" },
            { 0.15, "大幅度" },
        };

        private Enums.AttributeBAction AttributeBAction { get; set; }

        private double AppendValue { get; set; }

        public static AttributeB RandomCreate() => new AttributeB(CommonHelper.Random.Next(1, AttributeBMap.Count + 1));

        public override double Ascend(double success, double diff = 1)
        {
            if (AttributeBAction == Enums.AttributeBAction.AscendFailWeightLostLower)
            {
                if (diff < 1)
                {
                    diff = 1 - ((1 - diff) * AppendValue);
                    Logger.Info($"降低渡劫失败损失体重，倍率={AppendValue}，处理后倍率={diff}");
                }
            }
            else if (AttributeBAction == Enums.AttributeBAction.AscendWeightGainUpper)
            {
                if (diff > 1)
                {
                    diff = 1 + ((diff - 1) * AppendValue);
                    Logger.Info($"提升渡劫成功后获得体重，倍率={AppendValue}，处理后倍率={diff}");
                }
            }
            return diff;
        }

        public override double GetAscendSuccessRate(double value)
        {
            if (AttributeBAction == Enums.AttributeBAction.AscendSuccessRateUpper)
            {
                value *= AppendValue;
                Logger.Info($"提升渡劫成功率，倍率={AppendValue}，处理后成功率={value}");
            }
            return value;
        }

        public override (double, double) Attack(double source, double target, (double, double) baseAttack, double diff = 1)
        {
            if (baseAttack.Item1 > 1 && AttributeBAction == Enums.AttributeBAction.AttackDamageUpper)
            {
                double change = AppendValue;
                double increment = source * (baseAttack.Item1 - 1);
                double decrement = target * (1 - baseAttack.Item2);

                increment += decrement * change;
                decrement += decrement * change;

                baseAttack = (1 + (increment / source), 1 - (decrement / target));
                Logger.Info($"提升攻击成功时造成的伤害，倍率={AppendValue}，处理后倍率={baseAttack.Item1}，{baseAttack.Item2}");
            }
            else if (baseAttack.Item1 > 1 && AttributeBAction == Enums.AttributeBAction.AttackWeightGainUpper)
            {
                baseAttack = (baseAttack.Item1 * (1 + AppendValue), baseAttack.Item2);
                Logger.Info($"提升攻击成功时获得的体重，倍率={AppendValue}，处理后倍率={baseAttack.Item1}，{baseAttack.Item2}");
            }
            return baseAttack;
        }

        public override (double, double) BeingAttacked(double source, double target, (double, double) baseAttack)
        {
            if (AttributeBAction == Enums.AttributeBAction.BeingAttackedDamageLower)
            {
                if (baseAttack.Item2 < 1)
                {
                    baseAttack = (baseAttack.Item1, 1 - ((1 - baseAttack.Item2) * AppendValue));
                    Logger.Info($"降低受到攻击时的伤害，倍率={AppendValue}，处理后倍率={baseAttack.Item1}，{baseAttack.Item2}");
                }
            }
            return baseAttack;
        }

        public override (double, double) BeingDevoured(double source, double target, (double, double) baseDevour)
        {
            return baseDevour;
        }

        public override (double, double) Devour(double source, double target, double diff = 1)
        {
            return (1, 1);
        }

        public override double Feed(int count, double diff = 1)
        {
            if (AttributeBAction == Enums.AttributeBAction.FeedWeightGainUpper)
            {
                diff = diff * (1 + AppendValue);
                Logger.Info($"提升喂养时获得的体重，倍率={AppendValue}，处理后倍率={diff}");
            }
            return diff;
        }

        public override double Upgrade(int count, double diff = 1)
        {
            if (AttributeBAction == Enums.AttributeBAction.UpgradeWeightGainUpper)
            {
                diff = diff * (1 + AppendValue);
                Logger.Info($"提升升级时获得的体重，倍率={AppendValue}，处理后倍率={diff}");
            }
            return diff;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="fail">0.1</param>
        /// <returns></returns>
        public override double GetTransmogrifyFailRate(double fail)
        {
            if (AttributeBAction == Enums.AttributeBAction.TransmogrifyFailWeightLostLower)
            {
                fail = fail * (1 - AppendValue);
                Logger.Info($"降低幻化失败率，倍率={AppendValue}，处理后失败率={fail}");
            }
            return fail;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="lost">0.95</param>
        /// <returns></returns>
        public override double GetTransmogrifyFailWeightLostRate(double lost)
        {
            if (AttributeBAction == Enums.AttributeBAction.TransmogrifySuccessRateUpper)
            {
                lost = lost * (1 - AppendValue);
                Logger.Info($"降低幻化失败后损失体重比率，倍率={AppendValue}，处理后损失率={lost}");
            }
            return lost;
        }
    }
}