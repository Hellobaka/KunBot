using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace me.cqp.luohuaming.iKun.PublicInfos.PetAttribute.AttributeB
{
    public class None : IPetAttribute
    {
        public None()
        {
            ID = Enums.AttributeA.None;
            Name = "无属性";
            Description = [];
        }

        public override double Ascend(double success, double diff = 1)
        {
            return diff;
        }

        public override (double, double) Attack(double source, double target, double diff = 1)
        {
            return (diff, diff);
        }

        public override (double, double) BeingAttacked(double source, double target, (double, double) baseAttack)
        {
            return baseAttack;
        }

        public override (double, double) BeingDevoured(double source, double target, (double, double) baseDevour)
        {
            return baseDevour;
        }

        public override (double, double) Devour(double source, double target, double diff = 1)
        {
            return (diff, diff);
        }

        public override double Feed(int count, double diff = 1)
        {
            return diff;
        }

        public override double Upgrade(int count, double diff = 1)
        {
            return diff;
        }
    }
}
