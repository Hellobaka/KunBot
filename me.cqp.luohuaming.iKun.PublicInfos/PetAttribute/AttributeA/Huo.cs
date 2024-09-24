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

        public override (double, double) Attack(double source, double target, (double, double) baseAttack, double diff = 1)
        {
            // 攻击时 50% 的概率提高 10~30%伤害
            baseAttack = base.Attack(source, target, baseAttack, diff);
            if (baseAttack.Item1 > 1 && CommonHelper.Random.NextDouble() < 0.5)
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
            // 成功 吞噬 额外增加敌人体重 10~30%体重
            var baseDevour = base.Devour(source, target, diff);
            if (baseDevour.Item1 > 1)
            {
                return (baseDevour.Item1 * (1 + CommonHelper.Random.NextDouble(0.1, 0.3)), baseDevour.Item2);
            }

            return baseDevour;
        }
    }
}
