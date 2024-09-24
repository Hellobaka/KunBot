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
            ID = Enums.Attribute.Shui;
            Name = "水";
            Description = [
                "◆成功攻击或吞噬后大量提升体重",
                "◆被攻击或吞噬时有大概率逃脱",
                "◇对「火」属性的对手有额外攻击加成",
            ];
        }

        public override (double, double) Attack(double source, double target, (double, double) baseAttack, double diff = 1)
        {
            // 成功 攻击和吞噬 额外增加敌人损失体重的10~30%体重
            baseAttack = base.Attack(source, target, baseAttack, diff);
            if (baseAttack.Item1 > 1)
            {
                double increment = source * (baseAttack.Item1 - 1);
                double decrement = target * (1 - baseAttack.Item2);
                
                increment += decrement * CommonHelper.Random.NextDouble(0.3, 0.5);

                return (1 + (increment / source), 1 - (decrement / target));
            }
            return baseAttack;
        }

        public override (double, double) Devour(double source, double target, double diff = 1)
        {
            // 成功 攻击和吞噬 额外增加敌人体重的10~30%体重
            var baseDevour = base.Devour(source, target, diff);
            if (baseDevour.Item1 > 1)
            {
                return (baseDevour.Item1 * (1 + CommonHelper.Random.NextDouble(0.1, 0.3)), baseDevour.Item2);
            }
            return baseDevour;
        }

        public override (double, double) BeingAttacked(double source, double target, (double, double) baseAttack)
        {
            // 被攻击或吞噬时 30% 的几率逃脱
            if (CommonHelper.Random.NextDouble() < 0.3)
            {
                return (1, 1);
            }
            return baseAttack;
        }
    }
}
