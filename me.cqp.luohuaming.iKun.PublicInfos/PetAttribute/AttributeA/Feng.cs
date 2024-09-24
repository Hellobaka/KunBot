namespace me.cqp.luohuaming.iKun.PublicInfos.PetAttribute.AttributeA
{
    public class Feng : IPetAttribute
    {
        public Feng()
        {
            ID = Enums.AttributeA.Feng;
            Name = "风";
            Description = [
                "◆攻击时有大概率额外追击一次",
                "◆成功攻击或吞噬后巨量提升体重",
                "◆渡劫时巨幅度提升成功概率",
                "◇对「土、火」属性的对手有额外攻击加成",
                "◇对「水、金、木」属性的攻击额外防御加成",
            ];
        }

        public override (double, double) Attack(double source, double target, double diff = 1)
        {
            // 攻击时 30% 概率 造成 200% 的攻击
            // 成功 攻击或吞噬 额外增加敌人失去体重 30~50%
            var baseAttack = base.Attack(source, target, diff);
            if (baseAttack.Item1 > 1)
            {
                double change = 0;
                if (CommonHelper.Random.NextDouble() < 0.3)
                {
                    change = 1;
                }
                // 计算增量与减量
                double increment = source * (baseAttack.Item1 - 1);
                double decrement = target * (1 - baseAttack.Item2);
                // 额外造成200%攻击: 损失额外减去100%, 增量为损失
                decrement -= decrement * change;
                increment = decrement;
                // 额外增加: 增量额外添加损失的30~50%
                increment += decrement * CommonHelper.Random.NextDouble(0.3, 0.5);

                return (1 + (increment / source), 1 - (decrement / target));
            }
            return baseAttack;
        }

        public override (double, double) Devour(double source, double target, double diff = 1)
        {
            // 成功 攻击或吞噬 额外增加敌人失去体重 30~50%
            var baseDevour = base.Devour(source, target, diff);
            if (baseDevour.Item1 > 0)
            {
                return (baseDevour.Item1 * (1 + CommonHelper.Random.NextDouble(0.3, 0.5)), baseDevour.Item2);
            }
            return baseDevour;
        }
    }
}
