namespace me.cqp.luohuaming.iKun.PublicInfos.PetAttribute.AttributeA
{
    public class Yang : IPetAttribute
    {
        public Yang()
        {
            ID = Enums.AttributeA.Yin;
            Name = "阳";
            Description = [
                "◆被攻击时有大概率大幅度降低受到伤害",
                "◆攻击或吞噬时提升较大成功概率",
                "◆渡劫成功时巨幅度提升获得的体重",
                "◆渡劫时巨幅度降低失败概率",
                "◇对「阴」属性的对手有超额的攻击",
                "◇对「金、木、水、火、土、风」属性的对手有大量攻击加成",
                "◇对「金、木、水、火、土、风」属性的攻击有大量防御加成",
            ];
        }

        public override (double, double) Attack(double source, double target, double diff = 1)
        {
            // 攻击或吞噬 临时提升30% 的体重
            return base.Attack(source * 1.3, target, diff);
        }

        public override (double, double) Devour(double source, double target, double diff = 1)
        {
            // 攻击或吞噬 临时提升30% 的体重
            return base.Devour(source * 1.3, target, diff);
        }

        public override (double, double) BeingAttacked(double source, double target, (double, double) baseAttack)
        {
            // 受到攻击时 30% 的几率降低 50% 受到的伤害
            if (CommonHelper.Random.NextDouble() < 0.3)
            {
                return (baseAttack.Item1 * 0.5, 1);
            }
            return baseAttack;
        }

        public override double Ascend(double success, double diff = 1)
        {
            // 渡劫成功时 提升 50% 体重
            // 渡劫时 提升 50% 成功概率
            var baseRate = base.Ascend(success * 1.5, diff);
            if (baseRate > 1)
            {
                return baseRate * 1.5;
            }

            return baseRate;
        }
    }
}
