using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace me.cqp.luohuaming.iKun.PublicInfos.PetAttribute.AttributeA
{
    public class Tu : IPetAttribute
    {
        public Tu()
        {
            ID = Enums.Attribute.Tu;
            Name = "土";
            Description = [
                "◆成功吞噬后大量提升体重",
                "◆被攻击时减少巨量伤害",
                "◆渡劫时大幅提高成功概率",
                "◇对「水」属性的对手有额外攻击加成",
            ];
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

        public override (double, double) BeingAttacked(double source, double target, (double, double) baseAttack)
        {
            // 被攻击时减少 50% 的伤害
            if (baseAttack.Item2 < 1)
            {
                double change = 0.5;
                double decrement = target * (1 - baseAttack.Item2);

                decrement *= change;

                return (baseAttack.Item1, 1 - (decrement / target));
            }
            return base.BeingAttacked(source, target, baseAttack);
        }

        public override double GetAscendSuccessRate(double value)
        {
            // 渡劫时 提升 30% 概率
            return value * 1.3;
        }
    }
}
