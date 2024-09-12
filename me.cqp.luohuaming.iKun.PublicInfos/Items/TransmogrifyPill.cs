using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace me.cqp.luohuaming.iKun.PublicInfos.Items
{
    [Item(5)]
    public class TransmogrifyPill : Models.Items
    {
        public TransmogrifyPill(int count = 1)
        {
            ID = 5;
            Name = "羽化丸";
            Description = "收益风险并存，能大幅强化鲲，但是一旦失败将灰飞烟灭";
            Stackable = true;
            Count = count;
        }
    }
}
