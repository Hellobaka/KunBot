namespace me.cqp.luohuaming.iKun.PublicInfos.PetAttribute.AttributeA
{
    public class Yang : IPetAttribute
    {
        public Yang()
        {
            ID = Enums.Attribute.Yang;
            Name = "阳";
            Description = [
                "◆被攻击时有大概率大幅度降低受到伤害",
                "◆攻击或吞噬时提升较大成功概率",
                "◆渡劫成功时巨幅度提升获得的体重",
                "◆渡劫时巨幅度降低失败概率",
                "◇对「阴」属性的对手有超额的攻击",
                "◇对「金、木、水、火、土、风」属性的对手有大量攻击加成",
                "◇对「金、木、水、火、土、风」属性的攻击有大量防御加成",
            ];
        }

        private Logger Logger { get; set; } = new Logger("词缀_阳");

        public override (double, double) Attack(double source, double target, (double, double) baseAttack, double diff = 1)
        {
            Logger.Info($"进入攻击词缀计算方法，攻击方体重={source}，目标方体重={target}，Diff={diff}，基础倍率={baseAttack.Item1}，{baseAttack.Item2}");
            Logger.Info($"临时提升攻击方体重后，攻击方体重={source * 1.3}");

            // 攻击或吞噬 临时提升30% 的体重
            baseAttack = base.Attack(source * 1.3, target, baseAttack, diff);
            Logger.Info($"退出攻击词缀计算方法，最终倍率={baseAttack.Item1}，{baseAttack.Item2}");
            return baseAttack;
        }

        public override (double, double) Devour(double source, double target, double diff = 1)
        {
            Logger.Info($"进入吞噬词缀计算方法，攻击方体重={source}，目标方体重={target}，Diff={diff}");
            Logger.Info($"临时提升攻击方体重后，攻击方体重={source * 1.3}");

            // 攻击或吞噬 临时提升 30% 的体重
            var baseDevour = base.Devour(source * 1.3, target, diff);
            Logger.Info($"退出攻击词缀计算方法，最终倍率={baseDevour.Item1}，{baseDevour.Item2}");
            return baseDevour;
        }

        public override (double, double) BeingAttacked(double source, double target, (double, double) baseAttack)
        {
            Logger.Info($"进入被攻击词缀计算方法，攻击方体重={source}，目标方体重={target}，原始倍率={baseAttack.Item1}, {baseAttack.Item2}");
            double random = CommonHelper.Random.NextDouble();
            Logger.Info($"减少攻击加成随机数={random}，临界={0.3}");

            // 受到攻击时 30% 的几率降低 50% 受到的伤害
            if (random <= 0.3)
            {
                double diff = 1 + 0.5;
                Logger.Info($"加成增量={diff}");
                double increment = source * (baseAttack.Item1 - 1);
                double decrement = target * (1 - baseAttack.Item2) * diff;
                Logger.Info($"攻方增量={increment}，被攻方减量={decrement}");

                baseAttack = (1 + (increment - decrement) / source, baseAttack.Item2);
            }
            Logger.Info($"退出词缀计算方法，计算结果={baseAttack.Item1}，{baseAttack.Item2}");
            return baseAttack;
        }

        public override double Ascend(double success, double diff = 1)
        {
            Logger.Info($"进入渡劫词缀计算方法，成功率={success}，Diff={diff}");
            // 渡劫成功时 提升 50% 体重
            var baseRate = base.Ascend(success, diff);
            Logger.Info($"基类方法计算结果，成功率={success}，Diff={diff}");
            if (baseRate > 1)
            {
                Logger.Info($"渡劫成功，增加获得体重");
                baseRate = baseRate * 1.5;
            }
            Logger.Info($"退出渡劫词缀计算方法，倍率={diff}");

            return baseRate;
        }

        public override double GetAscendSuccessRate(double value)
        {
            // 渡劫时 提升 50% 概率
            Logger.Info($"进入渡劫成功率计算方法，成功率={value}");
            double rate = value * 1.5;
            Logger.Info($"退出渡劫成功率计算方法，成功率={rate}");
            return rate;
        }
    }
}