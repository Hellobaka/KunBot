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
            ID = Enums.Attributes.Lei;
            Name = "雷";
            Description = [
                "◆攻击时 25% 的几率造成额外 10%~30% 伤害",
                "◆被攻击时 15% 的几率反弹 10%~30% 伤害",
                "◇对“阴”属性的对手有额外 100% 的攻击加成",
                "◇对“水、金、木”属性的对手有额外 30% 的攻击加成",
                "◇对“土、火”属性的对手有额外 30% 的防御加成",
            ];
        }

        public override (double, double) Attack(double source, double target, double diff = 1)
        {
            var baseAttack = base.Attack(source, target, diff);
            if (baseAttack.Item1 > 1 && CommonHelper.Random.NextDouble() < 0.25)
            {
                double change = CommonHelper.Random.NextDouble(0.1, 0.3);
                double increment = source * (baseAttack.Item1 - 1);
                double decrement = target * (1 - baseAttack.Item2);

                decrement -= increment * change;
                increment += increment * change;

                return (1 + (increment / source), 1 - (decrement / target));
            }
            return baseAttack;
        }

        public override (double, double) BeingAttacked(double source, double target, (double, double) baseAttack)
        {
            if (CommonHelper.Random.NextDouble() <= 0.15)
            {
                double diff = 1 + CommonHelper.Random.NextDouble(0.1, 0.3);
                double increment = source * (baseAttack.Item1 - 1);
                double decrement = target * (1 - baseAttack.Item2) * diff;

                return (1 + (increment - decrement) / source, baseAttack.Item2);
            }
            return base.BeingAttacked(source, target, baseAttack);
        }
    }
}
