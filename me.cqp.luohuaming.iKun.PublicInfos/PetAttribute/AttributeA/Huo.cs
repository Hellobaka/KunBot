namespace me.cqp.luohuaming.iKun.PublicInfos.PetAttribute.AttributeA
{
    public class Huo : IPetAttribute
    {
        public Huo()
        {
            ID = Enums.Attribute.Huo;
            Name = "火";
            Description = [
                "◆攻击时有大概率造成额外少量伤害",
                "◆成功吞噬后提升大量体重",
                "◇对「金」属性的对手有额外攻击加成",
            ];
        }

        private Logger Logger { get; set; } = new Logger("词缀_火");

        public override (double, double) Attack(double source, double target, (double, double) baseAttack, double diff = 1)
        {
            Logger.Info($"进入攻击词缀计算方法，攻击方体重={source}，目标方体重={target}，Diff={diff}，基础倍率={baseAttack.Item1}，{baseAttack.Item2}");

            // 攻击时 50% 的概率提高 10~30%伤害
            baseAttack = base.Attack(source, target, baseAttack, diff);
            Logger.Info($"基类方法计算结果={baseAttack.Item1}，{baseAttack.Item2}");
            double random = CommonHelper.Random.NextDouble();
            Logger.Info($"攻击加成随机数={random}，临界={0.5}");

            if (baseAttack.Item1 > 1 && random < 0.5)
            {
                Logger.Info("攻击判定成功，进行加成");

                double change = CommonHelper.Random.NextDouble(0.1, 0.3);
                Logger.Info($"加成增量={change}");
                double increment = source * (baseAttack.Item1 - 1);
                double decrement = target * (1 - baseAttack.Item2);
                Logger.Info($"攻方增量={increment}，被攻方减量={decrement}");

                increment += decrement * change;
                decrement += decrement * change;
                Logger.Info($"使用加成计算后，攻方增量={increment}，被攻方减量={decrement}");

                baseAttack = (1 + (increment / source), 1 - (decrement / target));
            }
            Logger.Info($"退出词缀计算方法，计算结果={baseAttack.Item1}，{baseAttack.Item2}");
            return baseAttack;
        }

        public override (double, double) Devour(double source, double target, double diff = 1)
        {
            Logger.Info($"进入吞噬词缀计算方法，攻击方体重={source}，目标方体重={target}，Diff={diff}");

            // 成功 吞噬 额外增加敌人体重 10~30%体重
            var baseDevour = base.Devour(source, target, diff);
            Logger.Info($"基类方法计算结果={baseDevour.Item1}，{baseDevour.Item2}");
            if (baseDevour.Item1 > 1)
            {
                Logger.Info("吞噬判定成功，进行加成");
                double random = CommonHelper.Random.NextDouble(0.1, 0.3);
                Logger.Info($"随机增量随机数={random}");
                baseDevour = (baseDevour.Item1 * (1 + random), baseDevour.Item2);
            }
            Logger.Info($"退出词缀计算方法，计算结果={baseDevour.Item1}，{baseDevour.Item2}");

            return baseDevour;
        }
    }
}