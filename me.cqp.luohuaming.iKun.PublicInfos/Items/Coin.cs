using me.cqp.luohuaming.iKun.PublicInfos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace me.cqp.luohuaming.iKun.PublicInfos.Items
{
    public class Coin : IItem
    {
        public int ID { get; set; } = 1;

        public string Name { get; set; } = "";

        public string Description { get; set; } = "";

        public bool Stackable { get; set; } = true;

        public int Use()
        {
            throw new NotImplementedException();
        }

        public int Use(Kun target)
        {
            throw new NotImplementedException();
        }

        public int Use(Player player)
        {
            throw new NotImplementedException();
        }
    }
}
