namespace me.cqp.luohuaming.iKun.PublicInfos.PetAttribute.AttributeA
{
    public class Huo : IPetAttribute
    {
        public Huo()
        {
            ID = Enums.AttributeA.Huo;
            Name = "火";
            Description = [
                "◆攻击时 25% 的几率造成额外 10%~30% 伤害",
                "◆成功吞噬后提升自身 10%~30% 体重",
                "◇对“金”属性的对手有额外 30% 的攻击加成",
            ];
        }

        public override (double, double) Attack(double source, double target, double diff = 1)
        {
            var baseAttack = base.Attack(source, target, diff);
            if (baseAttack.Item1 > 1 && CommonHelper.Random.NextDouble() < 0.25)
            {
                // 变化率
                double change = CommonHelper.Random.NextDouble(0.1, 0.3);
                // 计算原始增量: 原始体重乘以原始倍率
                double increment = source * (baseAttack.Item1 - 1);
                // 计算原始减量: 原始体重乘以原始倍率
                double decrement = target * (1 - baseAttack.Item2);

                // 计算额外减量: 原始增量 - 新的增量
                decrement -= increment * change;
                // 计算额外增量: 新的增量
                increment += increment * change;

                // 计算结果: increment / source = 新的增量倍率
                // decrement / target = 新的减量倍率
                return (1 + (increment / source), 1 - (decrement / target));
            }
            return baseAttack;
        }

        public override (double, double) Devour(double source, double target, double diff = 1)
        {
            var baseDevour = base.Devour(source, target, diff);
            if (baseDevour.Item1 > 1)
            {
                return (baseDevour.Item1 * (1 + CommonHelper.Random.NextDouble(0.1, 0.3)), baseDevour.Item2);
            }

            return baseDevour;
        }
    }
}
