namespace me.cqp.luohuaming.iKun.PublicInfos.PetAttribute.AttributeA
{
    public class Yin : IPetAttribute
    {
        public Yin()
        {
            ID = Enums.Attribute.Yin;
            Name = "阴";
            Description = [
                "◆攻击时有小概率造成巨量伤害",
                "◆成功吞噬后额外增加体重",
                "◆被攻击或吞噬时有几率逃脱",
                "◆渡劫时巨幅度降低成功概率",
                "◇对「阳」属性的对手有超额的攻击",
                "◇对「金、木、水、火、土、风」属性的对手有大量攻击加成",
                "◇对「金、木、水、火、土、风」属性的攻击有大量防御加成",
            ];
        }

        private Logger Logger { get; set; } = new Logger("词缀_阴");

        public override (double, double) Attack(double source, double target, (double, double) baseAttack, double diff = 1)
        {
            Logger.Info($"进入攻击词缀计算方法，攻击方体重={source}，目标方体重={target}，Diff={diff}，基础倍率={baseAttack.Item1}，{baseAttack.Item2}");

            // 攻击时 10% 概率 造成 300% 的攻击
            // 成功 攻击或吞噬 额外增加敌人体重 30~50%体重
            baseAttack = base.Attack(source, target, baseAttack, diff);
            Logger.Info($"基类方法计算结果={baseAttack.Item1}，{baseAttack.Item2}");
            if (baseAttack.Item1 > 1)
            {
                Logger.Info("攻击判定成功，进行加成");

                double change = (1 + CommonHelper.Random.NextDouble(0.3, 0.5));
                Logger.Info($"加成增量={change}");
                double increment = source * (baseAttack.Item1 - 1);
                double decrement = target * (1 - baseAttack.Item2);
                Logger.Info($"攻方增量={increment}，被攻方减量={decrement}");

                increment += increment * change;
                Logger.Info($"使用加成计算后，攻方增量={increment}，被攻方减量={decrement}");
                change = CommonHelper.Random.NextDouble();
                Logger.Info($"双倍伤害随机数={change}，临界=0.1");

                if (change < 0.1)
                {
                    change = 2;
                    decrement -= increment * change;
                    increment += increment * change;
                    Logger.Info($"使用加成计算后，攻方增量={increment}，被攻方减量={decrement}");
                }

                baseAttack = (1 + (increment / source), 1 - (decrement / target));
            }
            Logger.Info($"退出词缀计算方法，计算结果={baseAttack.Item1}，{baseAttack.Item2}");
            return baseAttack;
        }

        public override (double, double) Devour(double source, double target, double diff = 1)
        {
            Logger.Info($"进入吞噬词缀计算方法，攻击方体重={source}，目标方体重={target}，Diff={diff}");

            // 成功 攻击或吞噬 额外增加敌人体重 30~50%体重
            var baseDevour = base.Devour(source, target, diff);
            Logger.Info($"基类方法计算结果={baseDevour.Item1}，{baseDevour.Item2}");
            if (baseDevour.Item1 > 1)
            {
                Logger.Info("吞噬判定成功，进行加成");
                double random = CommonHelper.Random.NextDouble(0.3, 0.5);
                Logger.Info($"随机增量随机数={random}");
                baseDevour = (baseDevour.Item1 * (1 + random), baseDevour.Item2);
            }
            Logger.Info($"退出词缀计算方法，计算结果={baseDevour.Item1}，{baseDevour.Item2}");

            return baseDevour;
        }

        public override (double, double) BeingAttacked(double source, double target, (double, double) baseAttack)
        {
            // 被攻击或吞噬时 45% 的几率逃脱
            Logger.Info($"进入被攻击词缀计算方法，攻击方体重={source}，目标方体重={target}，原始倍率={baseAttack.Item1}, {baseAttack.Item2}");
            double random = CommonHelper.Random.NextDouble();
            Logger.Info($"逃脱随机数={random}，临界={0.45}");
            if (CommonHelper.Random.NextDouble() < 0.45)
            {
                Logger.Info("判定成功");
                baseAttack = (1, 1);
            }
            Logger.Info($"退出词缀计算方法，计算结果={baseAttack.Item1}，{baseAttack.Item2}");
            return baseAttack;
        }

        public override (double, double) BeingDevoured(double source, double target, (double, double) baseDevour)
        {
            // 被攻击或吞噬时 45% 的几率逃脱
            Logger.Info($"进入被吞噬词缀计算方法，攻击方体重={source}，目标方体重={target}，原始倍率={baseDevour.Item1}, {baseDevour.Item2}");
            double random = CommonHelper.Random.NextDouble();
            Logger.Info($"逃脱随机数={random}，临界={0.45}");
            if (CommonHelper.Random.NextDouble() < 0.45)
            {
                Logger.Info("判定成功");
                baseDevour = (1, 1);
            }
            Logger.Info($"退出词缀计算方法，计算结果={baseDevour.Item1}，{baseDevour.Item2}");
            return baseDevour;
        }

        public override double GetAscendSuccessRate(double value)
        {
            // 渡劫时 减少成功 50%概率
            Logger.Info($"进入渡劫成功率计算方法，成功率={value}");
            double rate = value * 0.5;
            Logger.Info($"退出渡劫成功率计算方法，成功率={rate}");
            return rate;
        }
    }
}