using Newtonsoft.Json;

namespace me.cqp.luohuaming.iKun.PublicInfos.Models
{
    public class Items
    {
        public int ID { get; set; }
        
        public string Name { get; set; }
        
        public string Description { get; set; }

        public bool Stackable { get; set; }

        [JsonIgnore]
        public int Count { get; set; }

        public static Items Coin(int count = 1) => new() { ID = 1, Name = "金钱", Count = count };

        public static Items KunEgg(int count = 1) => new() { ID = 2, Name = "鲲之蛋", Count = count };
    }
}
