using me.cqp.luohuaming.iKun.Domain.Enums;
using me.cqp.luohuaming.iKun.Infrastructure.Logging;
using me.cqp.luohuaming.iKun.Infrastructure.Persistence;
using SqlSugar;

namespace me.cqp.luohuaming.iKun.Domain.Models;

/// <summary>背包条目（可堆叠物品一行计数，不可堆叠物品逐行标记删除）</summary>
[SugarTable]
public sealed class InventoryItem
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }

    public long PlayerID { get; set; }

    /// <summary>物品 ID（Item 枚举值）</summary>
    public int ItemID { get; set; }

    public int Count { get; set; }

    /// <summary>不可堆叠物品的软删除标记</summary>
    public bool Deleted { get; set; }

    public static int CountOf(Player player, ItemId itemId)
    {
        var definition = ItemCatalog.Definition(itemId);
        if (definition is null)
        {
            Log.For("背包").Error($"无法查询到ID为 {itemId} 的物资");
            return -1;
        }
        using var db = Db.CreateSession();
        return definition.Stackable
            ? db.Queryable<InventoryItem>().First(x => x.PlayerID == player.QQ && x.ItemID == (int)itemId && !x.Deleted)?.Count ?? 0
            : db.Queryable<InventoryItem>().Count(x => x.PlayerID == player.QQ && x.ItemID == (int)itemId && !x.Deleted);
    }

    /// <summary>
    /// 尝试扣除物品。成功返回 true 并输出扣除后的剩余数量。
    /// </summary>
    public static bool TryRemove(Player player, ItemId itemId, int count, out int remaining)
    {
        remaining = 0;
        count = Math.Max(1, count);
        var definition = ItemCatalog.Definition(itemId);
        if (definition is null)
        {
            Log.For("背包").Error($"无法查询到ID为 {itemId} 的物资");
            return false;
        }
        using var db = Db.CreateSession();
        if (definition.Stackable)
        {
            var entry = db.Queryable<InventoryItem>()
                .First(x => x.PlayerID == player.QQ && x.ItemID == (int)itemId && !x.Deleted);
            if (entry is null || entry.Count < count)
            {
                remaining = entry?.Count ?? 0;
                return false;
            }
            entry.Count -= count;
            db.Updateable(entry).ExecuteCommand();
            remaining = entry.Count;
            return true;
        }

        var entries = db.Queryable<InventoryItem>()
            .Where(x => x.PlayerID == player.QQ && x.ItemID == (int)itemId && !x.Deleted).ToList();
        if (entries.Count < count)
        {
            remaining = entries.Count;
            return false;
        }
        for (int i = 0; i < count; i++)
        {
            entries[i].Deleted = true;
            db.Updateable(entries[i]).ExecuteCommand();
        }
        remaining = entries.Count - count;
        return true;
    }

    public static List<InventoryItem> AllOf(long qq)
    {
        using var db = Db.CreateSession();
        return db.Queryable<InventoryItem>().Where(x => x.PlayerID == qq && !x.Deleted).ToList();
    }

    public override string ToString()
    {
        var definition = ItemCatalog.Definition((ItemId)ItemID);
        return $"{definition?.Name} {Count} 个 {definition?.Description}";
    }
}