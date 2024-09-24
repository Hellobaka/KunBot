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

        public override (double, double) Attack(double source, double target, (double, double) baseAttack, double diff = 1)
        {
            // 攻击或吞噬时 20% 概率提高10~30%伤害
            baseAttack = base.Attack(source, target, baseAttack, diff);
            if (CommonHelper.Random.NextDouble() <= 0.2)
            {
                double change = CommonHelper.Random.NextDouble(0.1, 0.3);
                double increment = source * (baseAttack.Item1 - 1);
                double decrement = target * (1 - baseAttack.Item2);

                decrement -= decrement * change;
                increment = decrement;

                return (1 + (increment / source), 1 - (decrement / target));
            }
            return baseAttack;
        }

        public override (double, double) Devour(double source, double target, double diff = 1)
        {
            // 攻击或吞噬时 20% 概率提高10~30%伤害
            // 成功吞噬 额外增加敌人体重的10~30%体重
            var baseDevour = base.Devour(source, target, diff);
            if (CommonHelper.Random.NextDouble() < 0.2)
            {
                double change = CommonHelper.Random.NextDouble(0.1, 0.3);
                double increment = source * (baseDevour.Item1 - 1);
                double decrement = target * (1 - baseDevour.Item2);

                decrement -= decrement * change;
                increment = decrement;

                return (1 + (increment / source), 1 - (decrement / target));
            }

            if (baseDevour.Item1 > 1)
            {
                return (baseDevour.Item1 * (1 + CommonHelper.Random.NextDouble(0.1, 0.3)), baseDevour.Item2);
            }
            return baseDevour;
        }

        public override (double, double) BeingAttacked(double source, double target, (double, double) baseAttack)
        {
            // 被攻击时 20 % 概率 敌人受到 10~30% 我方受到的伤害
            if (CommonHelper.Random.NextDouble() <= 0.2)
            {
                double change = CommonHelper.Random.NextDouble(0.1, 0.3);
                double increment = source * (baseAttack.Item1 - 1);
                double decrement = target * (1 - baseAttack.Item2);

                decrement -= increment * change;

                return (1 + (decrement / source), baseAttack.Item2);
            }
            return base.BeingAttacked(source, target, baseAttack);
        }

        public override double GetAscendSuccessRate(double value)
        {
            // 渡劫 减少 30 % 成功概率
            return value * 0.7;
        }
    }
}
