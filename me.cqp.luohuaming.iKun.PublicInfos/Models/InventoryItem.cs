using SqlSugar;
using System.Collections.Generic;

namespace me.cqp.luohuaming.iKun.PublicInfos.Models
{
    [SugarTable]
    public class InventoryItem
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        public long PlayerID { get; set; }

        public int ItemID { get; set; }

        public int Count { get; set; }

        public bool Deleted { get; set; }

        public static int GetItemCount(Player player, Enums.Items itemID)
        {
            var db = SQLHelper.GetInstance();
            var item = Items.GetItemByID(itemID);
            if (item == null)
            {
                MainSave.CQLog.Error("获取物资信息", $"无法查询到ID为 {itemID} 的物资");
                return -1;
            }
            if (item.Stackable)
            {
                return db.Queryable<InventoryItem>().First(x => x.PlayerID == player.QQ && x.ItemID == (int)itemID && !x.Deleted)?.Count ?? 0;
            }
            else
            {
                return db.Queryable<InventoryItem>().Count(x => x.PlayerID == player.QQ && x.ItemID == (int)itemID && !x.Deleted);
            }
        }

        public static bool TryRemoveItem(Player player, Enums.Items itemID, int count, out int currentCount)
        {
            var db = SQLHelper.GetInstance();
            var item = Items.GetItemByID(itemID);
            currentCount = 0;
            if (item == null)
            {
                MainSave.CQLog.Error("获取物资信息", $"无法查询到ID为 {itemID} 的物资");
                return false;
            }
            if (item.Stackable)
            {
                var query = db.Queryable<InventoryItem>().Where(x => x.PlayerID == player.QQ && x.ItemID == (int)itemID && !x.Deleted).First();
                currentCount = query.Count;
                if (query.Count >= count)
                {
                    query.Count -= count;
                    db.Updateable(query).ExecuteCommand();
                    currentCount = query.Count;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                var items = db.Queryable<InventoryItem>().Where(x => x.PlayerID == player.QQ && x.ItemID == (int)itemID && !x.Deleted).ToList();
                currentCount = items.Count;
                if (items.Count >= count)
                {
                    for (int i = 0; i < items.Count - count; i++)
                    {
                        items[i].Deleted = true;
                        db.Updateable(items[i]).ExecuteCommand();
                        currentCount = 0;
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static List<InventoryItem> GetItemsByQQ(long qq)
        {
            var db = SQLHelper.GetInstance();
            return db.Queryable<InventoryItem>().Where(x => x.PlayerID == qq && !x.Deleted).ToList();
        }

        public override string ToString()
        {
            var item = Items.GetItemByID((Enums.Items)ItemID);
            return $"{item.Name} {Count}个 {item.Description}";
        }
    }
}