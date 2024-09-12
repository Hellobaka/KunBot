using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace me.cqp.luohuaming.iKun.PublicInfos.Items
{
    [Item(4)]
    public class UpgradePill : Models.Items
    {
        public UpgradePill(int count = 1)
        {
            ID = 4;
            Name = "强化丸";
            Description = "能用于强化鲲";
            Stackable = true;
            Count = count;
        }
    }
}
