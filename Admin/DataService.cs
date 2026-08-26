using me.cqp.luohuaming.iKun.Domain.Configuration;
using me.cqp.luohuaming.iKun.Domain.Enums;
using me.cqp.luohuaming.iKun.Domain.Models;
using me.cqp.luohuaming.iKun.Domain.PetAttributes;
using me.cqp.luohuaming.iKun.Infrastructure.Logging;
using me.cqp.luohuaming.iKun.Infrastructure.Persistence;
using SqlSugar;

namespace me.cqp.luohuaming.iKun.Admin;

/// <summary>
/// 管理面板数据访问：玩家/鲲/背包查询、保存与级联删除。供 Admin 窗口在后台线程调用。
/// 只做纯 SqlSugar 实体读写，不调用领域业务方法。
/// </summary>
public static class DataService
{
    private static readonly Log Log = Log.For("管理面板");

    // ---- 返回模型 ----

    /// <summary>玩家列表行</summary>
    public sealed class PlayerSummary
    {
        public long QQ { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime LoginAt { get; set; }
        public DateTime AttackAt { get; set; }
        public DateTime DevourAt { get; set; }
        public int AscendPillComsume { get; set; }
        public int Coin { get; set; }
        public int KunEgg { get; set; }
        public int KunCount { get; set; }
        public int AliveKunCount { get; set; }
    }

    /// <summary>玩家详情：玩家 + 背包（带展示名）+ 鲲</summary>
    public sealed class PlayerDetail
    {
        public Player Player { get; set; }
        public List<ItemEntry> Items { get; set; } = new();
        public List<Kun> Kuns { get; set; } = new();
    }

    /// <summary>背包条目（InventoryItem 为 sealed 实体，展示名经包装类附加）</summary>
    public sealed class ItemEntry
    {
        public InventoryItem Item { get; set; }
        public string DisplayName { get; set; } = "";
    }

    /// <summary>鲲列表行</summary>
    public sealed class KunRow
    {
        public Kun Kun { get; set; }
        public string AffixAName { get; set; } = "";
        public string AffixBName { get; set; } = "";
        public string AffixCName { get; set; } = "";
        public string Groups { get; set; } = "";
        public long OwnerQQ { get; set; }
        public bool AutoPlayRunning { get; set; }
        public int AutoPlayType { get; set; }
        public DateTime AutoPlayEndTime { get; set; }
    }

    // ---- 查询 ----

    /// <summary>玩家列表：一行一玩家，含金币/鲲蛋数量与鲲统计（SQL 分组，无 N+1）</summary>
    public static List<PlayerSummary> ListPlayers()
    {
        using var db = Db.CreateSession();
        var players = db.Queryable<Player>().ToList();
        if (players.Count == 0)
        {
            return new List<PlayerSummary>();
        }

        // 背包：按 (PlayerID, ItemID) 分组求和，一次 SQL
        var itemAgg = db.Queryable<InventoryItem>()
            .Where(x => !x.Deleted && (x.ItemID == (int)ItemId.Coin || x.ItemID == (int)ItemId.KunEgg))
            .GroupBy(x => new { x.PlayerID, x.ItemID })
            .Select(x => new { x.PlayerID, x.ItemID, Total = SqlFunc.AggregateSum(x.Count) })
            .ToList();
        var itemsByPlayer = itemAgg
            .GroupBy(x => x.PlayerID)
            .ToDictionary(g => g.Key, g => g.ToList());

        // 鲲：按 PlayerID 分组统计总数/存活数，一次 SQL
        var kunAgg = db.Queryable<Kun>()
            .GroupBy(x => x.PlayerID)
            .Select(x => new
            {
                x.PlayerID,
                KunCount = SqlFunc.AggregateCount(x.Id),
                AliveKunCount = SqlFunc.AggregateSum(SqlFunc.IIF(x.Alive, 1, 0)),
            })
            .ToList();
        var kunsByPlayer = kunAgg.ToDictionary(x => x.PlayerID);

        return players.Select(p =>
        {
            var coin = 0;
            var kunEgg = 0;
            if (itemsByPlayer.TryGetValue(p.QQ, out var entries))
            {
                foreach (var entry in entries)
                {
                    if (entry.ItemID == (int)ItemId.Coin)
                    {
                        coin += entry.Total;
                    }
                    else
                    {
                        kunEgg += entry.Total;
                    }
                }
            }
            kunsByPlayer.TryGetValue(p.QQ, out var kunStat);
            return new PlayerSummary
            {
                QQ = p.QQ,
                CreateAt = p.CreateAt,
                LoginAt = p.LoginAt,
                AttackAt = p.AttackAt,
                DevourAt = p.DevourAt,
                AscendPillComsume = p.AscendPillComsume,
                Coin = coin,
                KunEgg = kunEgg,
                KunCount = kunStat?.KunCount ?? 0,
                AliveKunCount = kunStat?.AliveKunCount ?? 0,
            };
        }).ToList();
    }

    /// <summary>玩家详情；玩家不存在时返回空集合</summary>
    public static PlayerDetail GetPlayer(long qq)
    {
        using var db = Db.CreateSession();
        var detail = new PlayerDetail();
        var player = db.Queryable<Player>().First(x => x.QQ == qq);
        if (player is null)
        {
            return detail;
        }
        detail.Player = player;
        detail.Items = db.Queryable<InventoryItem>()
            .Where(x => x.PlayerID == qq && !x.Deleted)
            .ToList()
            .Select(x => new ItemEntry { Item = x, DisplayName = ItemDisplayName(x.ItemID) })
            .ToList();
        detail.Kuns = db.Queryable<Kun>().Where(x => x.PlayerID == qq).ToList();
        return detail;
    }

    /// <summary>鲲列表：词缀名、归属群、最新一条挂机/打工记录</summary>
    public static List<KunRow> ListKuns()
    {
        using var db = Db.CreateSession();
        var kuns = db.Queryable<Kun>().ToList();
        if (kuns.Count == 0)
        {
            return new List<KunRow>();
        }

        // 归属群：按 KunID 分组取 distinct 群号
        var records = db.Queryable<Record>().Select(x => new { x.Group, x.KunID }).ToList();
        var groupsByKun = records
            .GroupBy(x => x.KunID)
            .ToDictionary(g => g.Key, g => string.Join(",", g.Select(x => x.Group).Distinct()));

        // 挂机：每个 KunID 取 ID 最大的一条
        var latestByKun = db.Queryable<AutoPlay>()
            .GroupBy(x => x.KunID)
            .Select(x => new { x.KunID, MaxId = SqlFunc.AggregateMax(x.ID) })
            .ToList();
        var latestIds = latestByKun.Select(x => x.MaxId).ToList();
        var autoPlayByKun = latestIds.Count == 0
            ? new Dictionary<int, AutoPlay>()
            : db.Queryable<AutoPlay>()
                .Where(x => latestIds.Contains(x.ID))
                .ToList()
                .ToDictionary(x => x.KunID);

        return kuns.Select(kun =>
        {
            autoPlayByKun.TryGetValue(kun.Id, out var ap);
            return new KunRow
            {
                Kun = kun,
                AffixAName = AffixName(true, kun.AttributeAID),
                AffixBName = AffixName(false, kun.AttributeBID),
                AffixCName = AffixName(false, kun.AttributeCID),
                Groups = groupsByKun.TryGetValue(kun.Id, out var groups) ? groups : "",
                OwnerQQ = kun.PlayerID,
                AutoPlayRunning = ap?.Running ?? false,
                AutoPlayType = ap is null ? 0 : (int)ap.AutoPlayType,
                AutoPlayEndTime = ap?.EndTime ?? default,
            };
        }).ToList();
    }

    // ---- 保存 ----

    /// <summary>保存鲲的全部标量列（按 Id 更新）</summary>
    public static void SaveKun(Kun kun)
    {
        using var db = Db.CreateSession();
        db.Updateable(kun).UpdateColumns(x => new
        {
            x.Abandoned,
            x.Alive,
            x.AttributeAID,
            x.AttributeBID,
            x.AttributeCID,
            x.CanResurrect,
            x.Level,
            x.PlayerID,
            x.ResurrectCount,
            x.Weight,
            x.DeadAt,
            x.NickName,
        }).ExecuteCommand();
    }

    /// <summary>保存玩家可变字段（按 QQ 更新）</summary>
    public static void SavePlayer(Player p)
    {
        using var db = Db.CreateSession();
        db.Updateable(p).UpdateColumns(x => new
        {
            x.LoginAt,
            x.AttackAt,
            x.DevourAt,
            x.AscendPillComsume,
        }).ExecuteCommand();
    }

    /// <summary>
    /// 保存背包条目（对齐游戏语义）：同 (玩家, 物品) 已存在未删除行 → 更新 Count；
    /// 新物品 → 插入新行；Count &lt;= 0 → 视为移除，软删除该行（Deleted=true）。
    /// </summary>
    public static void SaveInventoryItem(InventoryItem item)
    {
        using var db = Db.CreateSession();
        var existing = db.Queryable<InventoryItem>()
            .First(x => x.PlayerID == item.PlayerID && x.ItemID == item.ItemID && !x.Deleted);
        if (item.Count <= 0)
        {
            if (existing is not null)
            {
                existing.Deleted = true;
                db.Updateable(existing).ExecuteCommand();
            }
            return;
        }
        if (existing is not null)
        {
            existing.Count = item.Count;
            db.Updateable(existing).ExecuteCommand();
        }
        else
        {
            db.Insertable(new InventoryItem
            {
                PlayerID = item.PlayerID,
                ItemID = item.ItemID,
                Count = item.Count,
            }).ExecuteCommand();
        }
    }

    // ---- 级联删除（管理面板物理清理）----

    /// <summary>删除鲲：归属记录 → 挂机记录 → 鲲本体</summary>
    public static void DeleteKun(long kunId)
    {
        using var db = Db.CreateSession();
        db.Ado.BeginTran();
        try
        {
            db.Deleteable<Record>().Where(x => x.KunID == kunId).ExecuteCommand();
            db.Deleteable<AutoPlay>().Where(x => x.KunID == kunId).ExecuteCommand();
            db.Deleteable<Kun>().Where(x => x.Id == kunId).ExecuteCommand();
            db.Ado.CommitTran();
        }
        catch
        {
            db.Ado.RollbackTran();
            throw;
        }
        Log.Info($"管理面板删除鲲 {kunId} 及其关联数据");
    }

    /// <summary>删除玩家：挂机 → 归属记录 → 背包 → 鲲 → 玩家</summary>
    public static void DeletePlayer(long qq)
    {
        using var db = Db.CreateSession();
        var kunIds = db.Queryable<Kun>()
            .Where(x => x.PlayerID == qq)
            .Select(x => x.Id)
            .ToList();
        db.Ado.BeginTran();
        try
        {
            if (kunIds.Count > 0)
            {
                db.Deleteable<AutoPlay>().Where(x => kunIds.Contains(x.KunID)).ExecuteCommand();
            }
            db.Deleteable<Record>().Where(x => x.QQ == qq).ExecuteCommand();
            db.Deleteable<InventoryItem>().Where(x => x.PlayerID == qq).ExecuteCommand();
            db.Deleteable<Kun>().Where(x => x.PlayerID == qq).ExecuteCommand();
            db.Deleteable<Player>().Where(x => x.QQ == qq).ExecuteCommand();
            db.Ado.CommitTran();
        }
        catch
        {
            db.Ado.RollbackTran();
            throw;
        }
        Log.Info($"管理面板删除玩家 {qq} 及其关联数据（鲲 {kunIds.Count} 条）");
    }

    // ---- 内部辅助 ----

    /// <summary>存储 ID → 词缀展示名；0/79 无属性显示“无”，解析失败显示“未知”</summary>
    private static string AffixName(bool isMain, int id)
    {
        try
        {
            var name = PetAttributeFactory.FromStoredId(isMain, id).Name;
            return string.IsNullOrEmpty(name) ? "无" : name;
        }
        catch (Exception e)
        {
            Log.Debug($"{(isMain ? "主" : "副")}词缀 ID {id} 解析失败：{e.Message}");
            return "未知";
        }
    }

    /// <summary>物品展示名（ItemConfiguration.Current），未知物品回退“物品{ItemId}”</summary>
    private static string ItemDisplayName(int itemId)
    {
        var config = ItemConfiguration.Current;
        return itemId switch
        {
            (int)ItemId.Coin => config.CoinName,
            (int)ItemId.KunEgg => config.KunEggName,
            (int)ItemId.BlindBox => config.BlindBoxName,
            (int)ItemId.ResurrectPill => config.ResurrectPillName,
            (int)ItemId.TransmogrifyPill => config.TransmogrifyPillName,
            (int)ItemId.UpgradePill => config.UpgradePillName,
            (int)ItemId.AscendPill => config.AscendPillName,
            (int)ItemId.LevelPill => config.LevelPillName,
            (int)ItemId.WeightPill => config.WeightPillName,
            _ => $"物品{itemId}",
        };
    }
}
