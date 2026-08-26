using me.cqp.luohuaming.iKun.Infrastructure.Persistence;
using SqlSugar;

namespace me.cqp.luohuaming.iKun.Domain.Models;

/// <summary>玩家：注册信息、签到/攻击/吞噬时间戳、挂起渡劫丹数量</summary>
[SugarTable]
public sealed class Player
{
    [SugarColumn(IsPrimaryKey = true)]
    public long QQ { get; set; }

    public DateTime CreateAt { get; set; }

    public DateTime LoginAt { get; set; }

    public DateTime AttackAt { get; set; }

    public DateTime DevourAt { get; set; }

    /// <summary>挂起的渡劫丹消耗数量</summary>
    public int AscendPillComsume { get; set; }

    public static bool Exists(long qq)
    {
        using var db = Db.CreateSession();
        return db.Queryable<Player>().Any(p => p.QQ == qq);
    }

    public static Player? Create(long qq)
    {
        using var db = Db.CreateSession();
        var player = new Player { QQ = qq, CreateAt = DateTime.Now, LoginAt = DateTime.Now };
        return db.Insertable(player).ExecuteCommand() > 0 ? player : null;
    }

    public static Player? Find(long qq)
    {
        using var db = Db.CreateSession();
        return db.Queryable<Player>().First(x => x.QQ == qq);
    }

    public void Save()
    {
        using var db = Db.CreateSession();
        db.Updateable(this).ExecuteCommand();
    }

    // ---- 背包 ----

    public void GrantItems(IEnumerable<Item> items)
    {
        using var db = Db.CreateSession();
        foreach (var item in items)
        {
            var existing = db.Queryable<InventoryItem>()
                .First(x => x.ItemID == (int)item.Id && x.PlayerID == QQ);
            if (existing is null || !item.Stackable)
            {
                db.Insertable(new InventoryItem
                {
                    PlayerID = QQ,
                    ItemID = (int)item.Id,
                    Count = item.Count,
                }).ExecuteCommand();
            }
            else
            {
                existing.Count += item.Count;
                db.Updateable(existing).ExecuteCommand();
            }
        }
    }
}