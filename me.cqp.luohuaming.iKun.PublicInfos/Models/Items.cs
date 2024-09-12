using me.cqp.luohuaming.iKun.PublicInfos.PetAttribute;
using Newtonsoft.Json;
using System;
using System.Reflection;

namespace me.cqp.luohuaming.iKun.PublicInfos.Models
{
    public class Items
    {
        public int ID { get; set; }
        
        public string Name { get; set; }
        
        public string Description { get; set; }

        public bool Stackable { get; set; } = true;

        public int Count { get; set; }

        public static Items Coin(int count = 1) => new PublicInfos.Items.Coin(count);

        public static Items KunEgg(int count = 1) => new PublicInfos.Items.KunEgg(count);

        public static Items GetItemByID(int id)
        {
            foreach (var item in Assembly.GetAssembly(typeof(PublicInfos.Items.Coin)).GetTypes())
            {
                if (item.IsInterface)
                    continue;
                if(item.Namespace != "me.cqp.luohuaming.iKun.PublicInfos.Items")
                {
                    continue;
                }
                var attribute = item.GetCustomAttribute<ItemAttribute>();
                if(attribute != null && attribute.ID == id)
                {
                    return (Items)Activator.CreateInstance(item, args: 1);
                }
            }

            return null;
        }

        public override string ToString()
        {
            return $"{Name} {Count}";
        }

        [AttributeUsage(AttributeTargets.Class)]
        public class ItemAttribute(int id) : Attribute
        {
            public int ID { get; set; } = id;
        }
    }
}
