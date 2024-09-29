namespace me.cqp.luohuaming.iKun.PublicInfos.PetAttribute.AttributeA
{
    public class Jin : IPetAttribute
    {
        public Jin()
        {
            ID = Enums.Attribute.Jin;
            Name = "金";
            Description = [
                "◆攻击或吞噬时有概率穿透防御",
                "◆被攻击时有概率反弹伤害",
                "◆成功吞噬后大量提升体重",
                "◆渡劫时大幅减少成功概率",
                "◇对「木」属性的对手有额外攻击加成"
            ];
        }

        private Logger Logger { get; set; } = new Logger("词缀_金");

        public override (double, double) Attack(double source, double target, (double, double) baseAttack, double diff = 1)
        {
            Logger.Info($"进入攻击词缀计算方法，攻击方体重={source}，目标方体重={target}，Diff={diff}，基础倍率={baseAttack.Item1}，{baseAttack.Item2}");

            // 攻击或吞噬时 20% 概率提高10~30%伤害
            baseAttack = base.Attack(source, target, baseAttack, diff);
            Logger.Info($"基类方法计算结果={baseAttack.Item1}，{baseAttack.Item2}");
            double random = CommonHelper.Random.NextDouble();
            Logger.Info($"攻击加成随机数={random}，临界={0.2}");
            if (baseAttack.Item1 > 1 && random <= 0.2)
            {
                Logger.Info("攻击判定成功，进行加成");

                double change = CommonHelper.Random.NextDouble(0.1, 0.3);
                Logger.Info($"加成增量={change}");
                double increment = source * (baseAttack.Item1 - 1);
                double decrement = target * (1 - baseAttack.Item2);
                Logger.Info($"攻方增量={increment}，被攻方减量={decrement}");

                decrement -= decrement * change;
                increment = decrement;
                Logger.Info($"使用加成计算后，攻方增量={increment}，被攻方减量={decrement}");

                baseAttack = (1 + (increment / source), 1 - (decrement / target));
            }
            Logger.Info($"退出词缀计算方法，计算结果={baseAttack.Item1}，{baseAttack.Item2}");
            return baseAttack;
        }

        public override (double, double) Devour(double source, double target, double diff = 1)
        {
            Logger.Info($"进入吞噬词缀计算方法，攻击方体重={source}，目标方体重={target}，Diff={diff}");

            // 攻击或吞噬时 20% 概率提高10~30%伤害
            // 成功吞噬 额外增加敌人体重的10~30%体重
            var baseDevour = base.Devour(source, target, diff);
            Logger.Info($"基类方法计算结果={baseDevour.Item1}，{baseDevour.Item2}");
            if (baseDevour.Item1 > 1 && CommonHelper.Random.NextDouble() < 0.2)
            {
                Logger.Info("吞噬判定成功，进行加成");

                double change = CommonHelper.Random.NextDouble(0.1, 0.3);
                Logger.Info($"加成增量={change}");
                double increment = source * (baseDevour.Item1 - 1);
                double decrement = target * (1 - baseDevour.Item2);
                Logger.Info($"攻方增量={increment}，被攻方减量={decrement}");

                decrement -= decrement * change;
                increment = decrement;
                Logger.Info($"使用加成计算后，攻方增量={increment}，被攻方减量={decrement}");

                baseDevour = (1 + (increment / source), 1 - (decrement / target));
            }

            if (baseDevour.Item1 > 1)
            {
                double random = CommonHelper.Random.NextDouble(0.1, 0.3);
                Logger.Info($"随机增量随机数={random}");
                baseDevour = (baseDevour.Item1 * (1 + random), baseDevour.Item2);
            }
            Logger.Info($"退出词缀计算方法，计算结果={baseDevour.Item1}，{baseDevour.Item2}");
            return baseDevour;
        }

        public override (double, double) BeingAttacked(double source, double target, (double, double) baseAttack)
        {
            Logger.Info($"进入被攻击词缀计算方法，攻击方体重={source}，目标方体重={target}，原始倍率={baseAttack.Item1}, {baseAttack.Item2}");
            double random = CommonHelper.Random.NextDouble();
            Logger.Info($"反弹随机数={random}，临界={0.2}");

            // 被攻击时 20 % 概率 敌人受到 10~30% 我方受到的伤害
            if (random <= 0.2)
            {
                Logger.Info("判定成功，进行加成");

                double change = CommonHelper.Random.NextDouble(0.1, 0.3);
                Logger.Info($"加成增量={change}");
                double increment = source * (baseAttack.Item1 - 1);
                double decrement = target * (1 - baseAttack.Item2);
                Logger.Info($"攻方增量={increment}，被攻方减量={decrement}");

                decrement -= increment * change;
                Logger.Info($"使用加成计算后，攻方增量={increment}，被攻方减量={decrement}");

                baseAttack = (1 + (decrement / source), baseAttack.Item2);
            }
            Logger.Info($"退出词缀计算方法，计算结果={baseAttack.Item1}，{baseAttack.Item2}");

            return baseAttack;
        }

        public override double GetAscendSuccessRate(double value)
        {
            Logger.Info("进入渡劫成功率计算方法");
            // 渡劫 减少 30 % 成功概率
            double rate = value * 0.7;
            Logger.Info($"退出渡劫成功率计算方法，成功率={rate}");
            return rate;
        }
    }
}