using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace me.cqp.luohuaming.iKun.PublicInfos.PetAttribute.AttributeA
{
    public class Yin : IPetAttribute
    {
        public Yin()
        {
            ID = Enums.Attributes.Yin;
            Name = "阴";
            Description = [
                "◆攻击时 30% 的几率造成 200% 的伤害",
                "◆成功吞噬后额外增加 10%~30% 体重",
                "◆被攻击或吞噬时有 30% 的几率逃脱",
                "◇对“金、木、水、火、土、风”属性的对手有额外 50% 的攻击",
                "◇受到“金、木、水、火、土、风”属性的攻击减少 50% 的伤害",
            ];
        }

        public override (double, double) Attack(double source, double target, double diff = 1)
        {
            var baseAttack = base.Attack(source, target, diff);
            if (baseAttack.Item1 > 1 && CommonHelper.Random.NextDouble() < 0.3)
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
            if (baseDevour.Item1 > 1)
            {
                return (baseDevour.Item1 * (1 + CommonHelper.Random.NextDouble(0.1, 0.3)), baseDevour.Item2);
            }
            return baseDevour;
        }

        public override (double, double) BeingAttacked(double source, double target, (double, double) baseAttack)
        {
            if (CommonHelper.Random.NextDouble() < 0.3)
            {
                return (1, 1);
            }
            return baseAttack;
        }

        public override (double, double) BeingDevoured(double source, double target, (double, double) baseDevour)
        {
            if (CommonHelper.Random.NextDouble() < 0.3)
            {
                return (1, 1);
            }
            return baseDevour;
        }
    }
}
