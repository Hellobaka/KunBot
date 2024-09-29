namespace me.cqp.luohuaming.iKun.PublicInfos.PetAttribute.AttributeA
{
    public class Mu : IPetAttribute
    {
        public Mu()
        {
            ID = Enums.Attribute.Mu;
            Name = "木";
            Description = [
                "◆成功吞噬后超巨量增加体重",
                "◆被攻击时有概率逃脱",
                "◆渡劫时大幅提高成功概率",
                "◇对「土」属性的对手有额外攻击加成",
            ];
        }

        private Logger Logger { get; set; } = new Logger("词缀_木");

        public override (double, double) Devour(double source, double target, double diff = 1)
        {
            Logger.Info($"进入吞噬词缀计算方法，攻击方体重={source}，目标方体重={target}，Diff={diff}");

            // 成功吞噬 额外增加敌人体重的50~100%体重
            var baseDevour = base.Devour(source, target, diff);
            Logger.Info($"基类方法计算结果={baseDevour.Item1}，{baseDevour.Item2}");
            if (baseDevour.Item1 > 1)
            {
                Logger.Info("吞噬判定成功，进行加成");
                double random = CommonHelper.Random.NextDouble(0.5, 1);
                Logger.Info($"随机增量随机数={random}");
                baseDevour = (baseDevour.Item1 * (1 + random), baseDevour.Item2);
            }
            Logger.Info($"退出词缀计算方法，计算结果={baseDevour.Item1}，{baseDevour.Item2}");

            return baseDevour;
        }

        public override (double, double) BeingAttacked(double source, double target, (double, double) baseAttack)
        {
            // 被攻击时 20% 的几率逃脱
            Logger.Info($"进入被攻击词缀计算方法，攻击方体重={source}，目标方体重={target}，原始倍率={baseAttack.Item1}, {baseAttack.Item2}");
            double random = CommonHelper.Random.NextDouble();
            Logger.Info($"逃脱随机数={random}，临界={0.2}");

            if (CommonHelper.Random.NextDouble() < 0.2)
            {
                Logger.Info("判定成功");
                baseAttack = (1, 1);
            }
            Logger.Info($"退出词缀计算方法，计算结果={baseAttack.Item1}，{baseAttack.Item2}");
            return baseAttack;
        }

        public override double GetAscendSuccessRate(double value)
        {
            Logger.Info("进入渡劫成功率计算方法");
            // 渡劫时 提升 30% 概率
            double rate = value * 1.3;
            Logger.Info($"退出渡劫成功率计算方法，成功率={rate}");
            return rate;
        }
    }
}
