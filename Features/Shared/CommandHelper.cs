using Another_Mirai_Native.Abstractions.Context;
using me.cqp.luohuaming.iKun.Background;
using me.cqp.luohuaming.iKun.Domain.Configuration;
using me.cqp.luohuaming.iKun.Domain.Enums;
using me.cqp.luohuaming.iKun.Domain.Models;
using me.cqp.luohuaming.iKun.Infrastructure.Persistence;

namespace me.cqp.luohuaming.iKun.Features.Shared;

/// <summary>
/// 指令处理公共辅助：白名单、参数解析、状态校验、消息回复。
/// </summary>
public static class CommandHelper
{
    /// <summary>群是否在启用列表中</summary>
    public static bool GroupEnabled(GroupMessageContext e) =>
        CoreConfiguration.Current.EnabledGroups.Contains(e.FromGroup.Id);

    /// <summary>解析整数参数。defaultToOne=true 时无参数/非法返回 1（兼容旧版行为），否则返回 null。</summary>
    public static int? ParseInt(string? input, bool defaultToOne = false)
    {
        input = input?.Trim();
        if (string.IsNullOrEmpty(input))
        {
            return defaultToOne ? 1 : null;
        }
        return int.TryParse(input, out var value) ? value : defaultToOne ? 1 : null;
    }

    /// <summary>指令格式错误提示</summary>
    public static string InvalidParams(string extra) => string.Format(CoreConfiguration.Current.Replies.ParamInvalid, extra);

    /// <summary>玩家与鲲的通用前置校验：未注册 / 未持有鲲 / 正在挂机或打工</summary>
    public static bool TryLoadPlayerAndKun(GroupMessageContext e, out Player player, out Kun kun)
    {
        var found = Player.Find(e.FromQQ.Id);
        if (found is null)
        {
            Reply(e, CoreConfiguration.Current.Replies.NoPlayer);
            player = null!;
            kun = null!;
            return false;
        }
        var kunFound = KunQuery.ActiveKunOf(found.QQ);
        if (kunFound is null)
        {
            Reply(e, CoreConfiguration.Current.Replies.NoKun);
            player = null!;
            kun = null!;
            return false;
        }
        kunFound.LoadAffixes();
        player = found;
        kun = kunFound;
        return true;
    }

    /// <summary>鲲是否正被挂机/打工占用，占用则回复并返回 true</summary>
    public static bool IsBusyReplyIfSo(GroupMessageContext e, Kun kun)
    {
        var replies = CoreConfiguration.Current.Replies;
        if (IdleScheduler.IsRunning(kun.Id, IdleType.Experience))
        {
            Reply(e, string.Format(replies.KunIdling, kun));
            return true;
        }
        if (IdleScheduler.IsRunning(kun.Id, IdleType.Coin))
        {
            Reply(e, string.Format(replies.KunWorking, kun));
            return true;
        }
        return false;
    }

    public static void Reply(GroupMessageContext e, string message) => e.SendMessage(message);
}

/// <summary>
/// 鲲数据查询（领域仓储门面）。
/// </summary>
public static class KunQuery
{
    /// <summary>玩家的当前存活鲲</summary>
    public static Kun? ActiveKunOf(long qq)
    {
        using var db = Db.CreateSession();
        return db.Queryable<Kun>().First(x => x.PlayerID == qq && !x.Abandoned && x.Alive);
    }

    public static Kun? ById(int id)
    {
        using var db = Db.CreateSession();
        return db.Queryable<Kun>().First(x => x.Id == id);
    }

    public static List<Kun> TopByWeight(int count)
    {
        using var db = Db.CreateSession();
        return db.Queryable<Kun>().Where(x => x.Alive && !x.Abandoned)
            .OrderByDescending(x => x.Weight).Take(count).ToList();
    }

    public static List<Kun> AliveAll()
    {
        using var db = Db.CreateSession();
        return db.Queryable<Kun>().Where(x => x.Alive && !x.Abandoned).ToList();
    }

    /// <summary>玩家可复活的死亡鲲（未超时限）</summary>
    public static List<Kun> ResurrectableOf(Player player)
    {
        using var db = Db.CreateSession();
        var dead = db.Queryable<Kun>()
            .Where(x => x.CanResurrect && !x.Alive && !x.Abandoned && x.PlayerID == player.QQ)
            .ToList();
        var limit = TimeSpan.FromHours(CoreConfiguration.Current.MaxResurrectHours);
        return dead.Where(x => DateTime.Now - x.DeadAt < limit).ToList();
    }
}