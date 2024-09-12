using me.cqp.luohuaming.iKun.PublicInfos.Enums;
using me.cqp.luohuaming.iKun.PublicInfos.Models;
using System;

namespace me.cqp.luohuaming.iKun.PublicInfos.PetAttribute.AttributeA
{
    public class Ao : IPetAttribute
    {
        public Ao()
        {
            ID = Attributes.Ao;
            Name = "";
            Description = "";
        }

        public override void Upgrade()
        {
            base.Upgrade();
        }
    }
}
