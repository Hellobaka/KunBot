using me.cqp.luohuaming.iKun.PublicInfos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace me.cqp.luohuaming.iKun.PublicInfos.Items
{
    public interface IItem
    {
        public int ID { get; set; }
        
        public string Name { get; set; }
        
        public string Description { get; set; }

        public bool Stackable { get; set; }

        public int Use();

        public int Use(Kun target);

        public int Use(Player player);
    }
}
