using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace me.cqp.luohuaming.iKun.PublicInfos.PetAttribute.AttributeA
{
    public class Feng : IPetAttribute
    {
        public Feng()
        {
            ID = Enums.AttributeA.Feng;
            Name = "风";
            Description = [
                "◆攻击时 15% 的几率造成 200% 的伤害",
                "◆成功吞噬后 15% 的几率增加自身 10%~30% 体重",
                "◇对“土、火”属性的对手有额外 30% 的攻击加成",
                "◇对“水、金、木”属性的攻击减少 30% 的伤害",
            ];
        }

        public override (double, double) Attack(double source, double target, double diff = 1)
        {
            var baseAttack = base.Attack(source, target, diff);
            if (baseAttack.Item1 > 1 && CommonHelper.Random.NextDouble() < 0.15)
            {
                double change = 1;
                double increment = source * (baseAttack.Item1 - 1);
                double decrement = target * (1 - baseAttack.Item2);

                decrement -= increment * change;
                increment += increment * change;

                return (1 + (increment / source), 1 - (decrement / target));
            }
            return baseAttack;
        }

        public override (double, double) Devour(double source, double target, double diff = 1)
        {
            var baseDevour = base.Devour(source, target, diff);
            if (baseDevour.Item1 > 1 && CommonHelper.Random.NextDouble() < 0.15)
            {
                return (baseDevour.Item1 * (1 + CommonHelper.Random.NextDouble(0.1, 0.3)), baseDevour.Item2);
            }
            return baseDevour;
        }
    }
}
