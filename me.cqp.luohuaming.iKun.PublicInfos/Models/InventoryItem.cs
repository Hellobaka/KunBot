using SqlSugar;

namespace me.cqp.luohuaming.iKun.PublicInfos.Models
{
    [SugarTable]
    public class InventoryItem
    {
        public int Id { get; set; }

        public int PlayerID { get; set; }

        public int ItemID { get; set; }

        public int Count { get; set; }
    }
}
