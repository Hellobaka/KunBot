using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace me.cqp.luohuaming.iKun.PublicInfos.Items
{
    [Item(6)]
    public class ResurrectPill : Models.Items
    {
        public ResurrectPill(int count = 1)
        {
            ID = 6;
            Name = "复活丸";
            Description = "能复活鲲，但是不能复活因幻化而死去的鲲";
            Stackable = true;
            Count = count;
        }
    }
}
