using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace me.cqp.luohuaming.iKun.PublicInfos.PetAttribute.AttributeA
{
    public class Shui : IPetAttribute
    {
        public Shui()
        {
            ID = Enums.Attributes.Shui;
            Name = "水";
            Description = [
                "◆成功吞噬后额外增加 10%~30% 体重",
                "◆被攻击或吞噬时 20% 的几率逃脱",
                "◇对“火”属性的对手有额外 30% 的攻击加成",
            ];
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

        public override (double, double) BeingAttacked(double source, double target, (double, double) baseAttack)
        {
            if (CommonHelper.Random.NextDouble() < 0.2)
            {
                return (1, 1);
            }
            return baseAttack;
        }
    }
}
