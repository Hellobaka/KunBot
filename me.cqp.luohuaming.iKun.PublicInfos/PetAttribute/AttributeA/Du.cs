using me.cqp.luohuaming.iKun.PublicInfos.Enums;
using me.cqp.luohuaming.iKun.PublicInfos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace me.cqp.luohuaming.iKun.PublicInfos.PetAttribute.AttributeA
{
    public class Du : IPetAttribute
    {
        public Du()
        {
            ID = Attributes.Du;
            Name = "";
            Description = "";
        }

        public override void Upgrade()
        {
            base.Upgrade();
        }
    }
}
