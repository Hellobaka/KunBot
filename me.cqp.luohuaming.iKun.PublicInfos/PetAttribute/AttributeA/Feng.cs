namespace me.cqp.luohuaming.iKun.PublicInfos.PetAttribute.AttributeA
{
    public class Feng : IPetAttribute
    {
        public Feng()
        {
            ID = Enums.Attribute.Feng;
            Name = "风";
            Description = [
                "◆攻击时有大概率额外追击一次",
                "◆成功攻击或吞噬后巨量提升体重",
                "◆渡劫时巨幅度提升成功概率",
                "◇对「土、火」属性的对手有额外攻击加成",
                "◇对「水、金、木」属性的攻击额外防御加成",
            ];
        }

        private Logger Logger { get; set; } = new Logger("词缀_风");

        public override (double, double) Attack(double source, double target, (double, double) baseAttack, double diff = 1)
        {
            Logger.Info($"进入攻击词缀计算方法，攻击方体重={source}，目标方体重={target}，Diff={diff}，基础倍率={baseAttack.Item1}，{baseAttack.Item2}");
            // 攻击时 30% 概率 造成 200% 的攻击
            // 成功 攻击或吞噬 额外增加敌人失去体重 30~50%
            baseAttack = base.Attack(source, target, baseAttack, diff);
            Logger.Info($"基类方法计算结果={baseAttack.Item1}，{baseAttack.Item2}");
            if (baseAttack.Item1 > 1)
            {
                Logger.Info("攻击判定成功，进行加成");
                double change = 0;
                double random = CommonHelper.Random.NextDouble();
                Logger.Info($"双倍伤害随机数={random}，临界=0.3");
                if (random < 0.3)
                {
                    change = 1;
                }
                Logger.Info($"加成增量={change}");
                // 计算增量与减量
                double increment = source * (baseAttack.Item1 - 1);
                double decrement = target * (1 - baseAttack.Item2);
                Logger.Info($"攻方增量={increment}，被攻方减量={decrement}");
                // 额外造成200%攻击: 损失额外减去100%, 增量为损失
                decrement -= decrement * change;
                increment = decrement;

                Logger.Info($"使用加成计算后，攻方增量={increment}，被攻方减量={decrement}");
                // 额外增加: 增量额外添加损失的30~50%
                random = CommonHelper.Random.NextDouble(0.3, 0.5);
                increment += decrement * random;
                Logger.Info($"随机增量随机数={random}，攻方增量={increment}");
                baseAttack = (1 + (increment / source), 1 - (decrement / target));
            }
            Logger.Info($"退出词缀计算方法，计算结果={baseAttack.Item1}，{baseAttack.Item2}");

            return baseAttack;
        }

        public override (double, double) Devour(double source, double target, double diff = 1)
        {
            Logger.Info($"进入吞噬词缀计算方法，攻击方体重={source}，目标方体重={target}，Diff={diff}");

            // 成功 攻击或吞噬 额外增加敌人失去体重 30~50%
            var baseDevour = base.Devour(source, target, diff);
            Logger.Info($"基类方法计算结果={baseDevour.Item1}，{baseDevour.Item2}");
            if (baseDevour.Item1 > 0)
            {
                Logger.Info("吞噬判定成功，进行加成");
                double random = CommonHelper.Random.NextDouble(0.3, 0.5);
                Logger.Info($"随机增量随机数={random}");
                baseDevour = (baseDevour.Item1 * (1 + random), baseDevour.Item2);
            }
            Logger.Info($"退出词缀计算方法，计算结果={baseDevour.Item1}，{baseDevour.Item2}");
            return baseDevour;
        }

        public override double GetAscendSuccessRate(double value)
        {
            Logger.Info("进入渡劫成功率计算方法");
            // 渡劫时 提升 50%概率
            double rate = value * 1.5;
            Logger.Info($"退出渡劫成功率计算方法，成功率={rate}");
            return rate;
        }
    }
}