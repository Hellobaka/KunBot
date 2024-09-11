using me.cqp.luohuaming.iKun.PublicInfos.Enums;
using me.cqp.luohuaming.iKun.PublicInfos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace me.cqp.luohuaming.iKun.PublicInfos.PetAttribute.AttributeA
{
    [Pet(2)]
    public class Bei : IPetAttribute
    {
        public int ID => ((PetAttribute)Attribute.GetCustomAttribute(GetType(), typeof(PetAttribute))).ID;

        public string Name { get; set; }

        public string Description { get; set; }

        public void AfterAction(ActionType action, Kun kun)
        {
            throw new NotImplementedException();
        }

        public void AfterAttack(AttackType action, Kun origin, Kun target)
        {
            throw new NotImplementedException();
        }

        public bool BeforeAction(ActionType action, Kun kun, out string msg)
        {
            throw new NotImplementedException();
        }

        public bool BeforeAttack(AttackType action, Kun origin, Kun target, out string msg)
        {
            throw new NotImplementedException();
        }
    }
}
