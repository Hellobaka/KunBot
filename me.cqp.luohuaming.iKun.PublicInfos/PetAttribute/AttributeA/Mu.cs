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

        public override (double, double) Devour(double source, double target, double diff = 1)
        {
            // 成功吞噬 额外增加敌人体重的50~100%体重
            var baseDevour = base.Devour(source, target, diff);
            if (baseDevour.Item1 > 1)
            {
                return (baseDevour.Item1 * (1 + CommonHelper.Random.NextDouble(0.5, 1)), baseDevour.Item2);
            }
            return baseDevour;
        }

        public override (double, double) BeingAttacked(double source, double target, (double, double) baseAttack)
        {
            // 被攻击时 20% 的几率逃脱
            if (CommonHelper.Random.NextDouble() < 0.2)
            {
                return (1, 1);
            }
            return baseAttack;
        }

        public override double GetAscendSuccessRate(double value)
        {
            // 渡劫时 提升 30% 概率
            return value * 1.3;
        }
    }
}
