using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace me.cqp.luohuaming.iKun.PublicInfos.PetAttribute.AttributeA
{
    public class Lei : IPetAttribute
    {
        public Lei()
        {
            ID = Enums.Attribute.Lei;
            Name = "雷";
            Description = [
                "◆攻击时有大概率造成额外少量伤害",
                "◆渡劫时超巨幅提升成功概率",
                "◆被攻击时小概率反弹大量伤害",
                "◇对「水、金、木」属性的对手有额外攻击加成",
                "◇对「土、火」属性的攻击额外防御加成",
            ];
        }

        public override (double, double) Attack(double source, double target, (double, double) baseAttack, double diff = 1)
        {
            // 攻击时 50% 的几率造成额外 10~30% 伤害
            baseAttack = base.Attack(source, target, baseAttack, diff);
            if (baseAttack.Item1 > 1 && CommonHelper.Random.NextDouble() < 0.5)
            {
                double change = CommonHelper.Random.NextDouble(0.1, 0.3);
                double increment = source * (baseAttack.Item1 - 1);
                double decrement = target * (1 - baseAttack.Item2);

                decrement -= decrement * change;
                increment += decrement;

                return (1 + (increment / source), 1 - (decrement / target));
            }
            return baseAttack;
        }

        public override (double, double) BeingAttacked(double source, double target, (double, double) baseAttack)
        {
            // 被攻击时 10% 概率 敌人受到 30~50% 我方受到的伤害
            if (CommonHelper.Random.NextDouble() <= 0.1)
            {
                double diff = 1 + CommonHelper.Random.NextDouble(0.3, 0.5);
                double increment = source * (baseAttack.Item1 - 1);
                double decrement = target * (1 - baseAttack.Item2) * diff;

                return (1 + (increment - decrement) / source, baseAttack.Item2);
            }
            return base.BeingAttacked(source, target, baseAttack);
        }

        public override double GetAscendSuccessRate(double value)
        {
            // 渡劫时 提升 100% 概率
            return value * 2;
        }
    }
}
