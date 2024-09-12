using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace me.cqp.luohuaming.iKun.PublicInfos.Items
{
    [Item(3)]
    public class BlindBox : Models.Items
    {
        public BlindBox(int count = 1)
        {
            ID = 3;
            Name = "盲盒";
            Description = "能获得随机材料";
            Stackable = true;
            Count = count;
        }
    }
}
