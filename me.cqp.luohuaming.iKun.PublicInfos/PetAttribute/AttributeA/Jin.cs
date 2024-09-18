namespace me.cqp.luohuaming.iKun.PublicInfos.PetAttribute.AttributeA
{
    public class Jin : IPetAttribute
    {
        public Jin()
        {
            ID = Enums.Attributes.Jin;
            Name = "金";
            Description = [
                "◆攻击或吞噬时 20% 的几率增加或受到 10%~30% 伤害",
                "◆被攻击时 15% 的几率反弹 10%~50% 伤害",
                "◆成功吞噬后额外增加 10%~30% 体重",
                "◇对“木”属性的对手有额外 30% 的攻击加成"
            ];
        }

        public override (double, double) Attack(double source, double target, double diff = 1)
        {
            var baseAttack = base.Attack(source, target, diff);
            if (CommonHelper.Random.NextDouble() <= 0.2)
            {
                diff = diff + CommonHelper.Random.NextDouble(0.1, 0.3);
                // recalc
                return base.Attack(source, target, diff);
            }
            return baseAttack;
        }

        public override (double, double) Devour(double source, double target, double diff = 1)
        {
            var baseDevour = base.Devour(source, target, diff);
            if (CommonHelper.Random.NextDouble() < 0.2)
            {
                diff = diff + CommonHelper.Random.NextDouble(0.1, 0.3);
                baseDevour = base.Devour(source, target, diff);
            }

            if(baseDevour.Item1 > 1)
            {
                diff = diff + CommonHelper.Random.NextDouble(0.1, 0.3);
                baseDevour = (baseDevour.Item1 * (1 + diff), baseDevour.Item2);
            }
            return baseDevour;
        }

        public override (double, double) BeingAttacked(double source, double target, (double, double) baseAttack)
        {
            if (CommonHelper.Random.NextDouble() <= 0.15)
            {
                double diff = 1 + CommonHelper.Random.NextDouble(0.1, 0.5);
                double increment = source * (baseAttack.Item1 - 1);
                double decrement = target * (1 - baseAttack.Item2) * diff;
                // increment - decrement = recalc increment
                // rate = recalc / source
                return (1 + (increment - decrement) / source, baseAttack.Item2);
            }
            return base.BeingAttacked(source, target, baseAttack);
        }

        public override (double, double) BeingDevoured(double source, double target, (double, double) baseDevour)
        {
            if (baseDevour.Item1 > 1)
            {
                return (baseDevour.Item1 * (1 + CommonHelper.Random.NextDouble(0.1, 0.3)), baseDevour.Item2);
            }

            return baseDevour;
        }
    }
}
