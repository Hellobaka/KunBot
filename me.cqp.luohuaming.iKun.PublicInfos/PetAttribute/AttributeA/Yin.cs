namespace me.cqp.luohuaming.iKun.PublicInfos.PetAttribute.AttributeA
{
    public class Yin : IPetAttribute
    {
        public Yin()
        {
            ID = Enums.AttributeA.Yin;
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

        public override (double, double) Attack(double source, double target, double diff = 1)
        {
            // 攻击时 10% 概率 造成 300% 的攻击
            // 成功 攻击或吞噬 额外增加敌人体重 30~50%体重
            var baseAttack = base.Attack(source, target, diff);
            if (baseAttack.Item1 > 1)
            {
                double change = (1 + CommonHelper.Random.NextDouble(0.3, 0.5));
                double increment = source * (baseAttack.Item1 - 1);
                double decrement = target * (1 - baseAttack.Item2);

                increment += increment * change;
                if (CommonHelper.Random.NextDouble() < 0.1)
                {
                    change = 2;
                    decrement -= increment * change;
                    increment += increment * change;
                }
                return (1 + (increment / source), 1 - (decrement / target));
            }
            return baseAttack;
        }

        public override (double, double) Devour(double source, double target, double diff = 1)
        {
            // 成功 攻击或吞噬 额外增加敌人体重 30~50%体重
            var baseDevour = base.Devour(source, target, diff);
            if (baseDevour.Item1 > 1)
            {
                return (baseDevour.Item1 * (1 + CommonHelper.Random.NextDouble(0.3, 0.5)), baseDevour.Item2);
            }
            return baseDevour;
        }

        public override (double, double) BeingAttacked(double source, double target, (double, double) baseAttack)
        {
            // 被攻击或吞噬时 45% 的几率逃脱
            if (CommonHelper.Random.NextDouble() < 0.45)
            {
                return (1, 1);
            }
            return baseAttack;
        }

        public override (double, double) BeingDevoured(double source, double target, (double, double) baseDevour)
        {
            // 被攻击或吞噬时 45% 的几率逃脱
            if (CommonHelper.Random.NextDouble() < 0.3)
            {
                return (1, 1);
            }
            return baseDevour;
        }

        public override double Ascend(double success, double diff = 1)
        {
            // 渡劫时 减少成功 50%概率
            return base.Ascend(success * 0.5, diff);
        }
    }
}
