using Another_Mirai_Native.Abstractions;
using Another_Mirai_Native.Abstractions.Attributes;
using Another_Mirai_Native.Abstractions.Context;
using Another_Mirai_Native.Abstractions.Enums;
using me.cqp.luohuaming.iKun.Domain.Configuration;
using me.cqp.luohuaming.iKun.Features.Account;
using me.cqp.luohuaming.iKun.Features.Combat;
using me.cqp.luohuaming.iKun.Features.Economy;
using me.cqp.luohuaming.iKun.Features.KunCare;
using me.cqp.luohuaming.iKun.Features.Ranking;
using me.cqp.luohuaming.iKun.Features.Shared;
using me.cqp.luohuaming.iKun.Features.Work;
using System.Text.RegularExpressions;

namespace me.cqp.luohuaming.iKun.Features;

/// <summary>
/// 指令路由：把 [CommandRoute] 元数据翻译为框架 [DynamicCommand]。
/// 各业务指令实现在 Features/* 的 Feature 单例中，本类不含业务逻辑。
/// </summary>
public class CommandRouter : CommandHandlerBase
{
    // ---- 触发词属性（框架每次调度时读取，配置热重载即时生效）----

    private static CoreConfiguration Config => CoreConfiguration.Current;

    // Account
    public string RouteRegister => BuildPattern(Config.CommandRegister, withArgs: false);

    public string RouteLogin => BuildPattern(Config.CommandLogin, false);

    public string RouteMenu => BuildPattern(Config.CommandMenu, false);

    public string RouteInventory => BuildPattern(Config.CommandInventory, false);

    public string RouteNickName => BuildPattern(Config.CommandSetNickName, true);

    public string RouteClearNickName => BuildPattern(Config.CommandClearNickName, false);

    // KunCare
    public string RouteHatch => BuildPattern(Config.CommandHatch, true);

    public string RouteFeed => BuildPattern(Config.CommandFeed, true);

    public string RouteUpgrade => BuildPattern(Config.CommandUpgrade, true);

    public string RouteTransmogrify => BuildPattern(Config.CommandTransmogrify, false);

    public string RouteAscend => BuildPattern(Config.CommandAscend, false);

    public string RouteAscendPill => BuildPattern(Config.CommandConsumeAscendPill, true);

    public string RouteResurrect => BuildPattern(Config.CommandResurrect, true);

    public string RouteDeadKuns => BuildPattern(Config.CommandQueryDeadKuns, false);

    public string RouteRelease => BuildPattern(Config.CommandRelease, false);

    public string RouteMyKun => BuildPattern(Config.CommandMyKun, false);

    // Combat
    public string RouteAttack => BuildPattern(Config.CommandAttack, true);

    public string RouteDevour => BuildPattern(Config.CommandDevour, true);

    // Economy
    public string RouteShop => BuildPattern(Config.CommandShopping, true);

    public string RouteOpenEgg => BuildPattern(Config.CommandOpenEgg, true);

    public string RouteBlindBox => BuildPattern(Config.CommandOpenBlindBox, true);

    public string RouteUseItem => BuildPattern(Config.CommandUseItem, true);

    // Ranking
    public string RouteRanking => BuildPattern(Config.CommandRanking, false);

    public string RouteRankingGroup => BuildPattern(Config.CommandRankingGroup, false);

    public string RoutePunishInfo => BuildPattern(Config.CommandRandomPunishInfo, false);

    // Work
    public string RouteStartIdle => BuildPattern(Config.CommandStartIdle, true);

    public string RouteStopIdle => BuildPattern(Config.CommandStopIdle, false);

    public string RouteStartWork => BuildPattern(Config.CommandStartWork, true);

    public string RouteStopWork => BuildPattern(Config.CommandStopWork, false);

    /// <summary>触发词 → 正则缓存</summary>
    private static readonly Dictionary<string, string> PatternCache = [];

    /// <summary>
    /// 生成指令正则：^[＃#]命令[ \t]*(args 命名组)?$
    /// </summary>
    private static string BuildPattern(string template, bool withArgs)
    {
        var key = $"{template}|{withArgs}";
        if (PatternCache.TryGetValue(key, out var cached))
        {
            return cached;
        }
        var builder = new System.Text.StringBuilder("^");
        foreach (var ch in template)
        {
            builder.Append(ch == '#' ? "[＃#]" : Regex.Escape(ch.ToString()));
        }
        builder.Append(withArgs ? @"[ \t]*(?<args>.*?)\s*$" : @"[ \t]*$");
        cached = builder.ToString();
        PatternCache[key] = cached;
        return cached;
    }

    // ---- 路由到 Feature 单例（全部仅群聊生效）----

    // Account
    [DynamicCommand(nameof(RouteRegister), MatchMode.Regex, MessageScope.Group)]
    public void Register(GroupMessageContext e)
    {
        if (Guard(e))
        {
            AccountFeature.Instance.Register(e);
        }
    }

    [DynamicCommand(nameof(RouteLogin), MatchMode.Regex, MessageScope.Group)]
    public void Login(GroupMessageContext e)
    {
        if (Guard(e))
        {
            AccountFeature.Instance.Login(e);
        }
    }

    [DynamicCommand(nameof(RouteMenu), MatchMode.Regex, MessageScope.Group)]
    public void Menu(GroupMessageContext e)
    {
        if (Guard(e))
        {
            AccountFeature.Instance.Menu(e);
        }
    }

    [DynamicCommand(nameof(RouteInventory), MatchMode.Regex, MessageScope.Group)]
    public void Inventory(GroupMessageContext e)
    {
        if (Guard(e))
        {
            AccountFeature.Instance.Inventory(e);
        }
    }

    [DynamicCommand(nameof(RouteNickName), MatchMode.Regex, MessageScope.Group)]
    public void NickName(GroupMessageContext e, string args)
    {
        if (Guard(e))
        {
            AccountFeature.Instance.SetNickName(e, args);
        }
    }

    [DynamicCommand(nameof(RouteClearNickName), MatchMode.Regex, MessageScope.Group)]
    public void ClearNickName(GroupMessageContext e)
    {
        if (Guard(e))
        {
            AccountFeature.Instance.ClearNickName(e);
        }
    }

    // KunCare
    [DynamicCommand(nameof(RouteHatch), MatchMode.Regex, MessageScope.Group)]
    public void Hatch(GroupMessageContext e, string args)
    {
        if (Guard(e))
        {
            KunCareFeature.Instance.Hatch(e, args);
        }
    }

    [DynamicCommand(nameof(RouteFeed), MatchMode.Regex, MessageScope.Group)]
    public void Feed(GroupMessageContext e, string args)
    {
        if (Guard(e))
        {
            KunCareFeature.Instance.Feed(e, args);
        }
    }

    [DynamicCommand(nameof(RouteUpgrade), MatchMode.Regex, MessageScope.Group)]
    public void Upgrade(GroupMessageContext e, string args)
    {
        if (Guard(e))
        {
            KunCareFeature.Instance.Upgrade(e, args);
        }
    }

    [DynamicCommand(nameof(RouteTransmogrify), MatchMode.Regex, MessageScope.Group)]
    public void Transmogrify(GroupMessageContext e)
    {
        if (Guard(e))
        {
            KunCareFeature.Instance.Transmogrify(e);
        }
    }

    [DynamicCommand(nameof(RouteAscend), MatchMode.Regex, MessageScope.Group)]
    public void Ascend(GroupMessageContext e)
    {
        if (Guard(e))
        {
            KunCareFeature.Instance.Ascend(e);
        }
    }

    [DynamicCommand(nameof(RouteAscendPill), MatchMode.Regex, MessageScope.Group)]
    public void AscendPill(GroupMessageContext e, string args)
    {
        if (Guard(e))
        {
            KunCareFeature.Instance.ConsumeAscendPills(e, args);
        }
    }

    [DynamicCommand(nameof(RouteResurrect), MatchMode.Regex, MessageScope.Group)]
    public void Resurrect(GroupMessageContext e, string args)
    {
        if (Guard(e))
        {
            KunCareFeature.Instance.Resurrect(e, args);
        }
    }

    [DynamicCommand(nameof(RouteDeadKuns), MatchMode.Regex, MessageScope.Group)]
    public void DeadKuns(GroupMessageContext e)
    {
        if (Guard(e))
        {
            KunCareFeature.Instance.QueryDeadKuns(e);
        }
    }

    [DynamicCommand(nameof(RouteRelease), MatchMode.Regex, MessageScope.Group)]
    public void Release(GroupMessageContext e)
    {
        if (Guard(e))
        {
            KunCareFeature.Instance.Release(e);
        }
    }

    [DynamicCommand(nameof(RouteMyKun), MatchMode.Regex, MessageScope.Group)]
    public void MyKun(GroupMessageContext e)
    {
        if (Guard(e))
        {
            KunCareFeature.Instance.MyKun(e);
        }
    }

    // Combat
    [DynamicCommand(nameof(RouteAttack), MatchMode.Regex, MessageScope.Group)]
    public void Attack(GroupMessageContext e, string args)
    {
        if (Guard(e))
        {
            CombatFeature.Instance.Attack(e, args);
        }
    }

    [DynamicCommand(nameof(RouteDevour), MatchMode.Regex, MessageScope.Group)]
    public void Devour(GroupMessageContext e, string args)
    {
        if (Guard(e))
        {
            CombatFeature.Instance.Devour(e, args);
        }
    }

    // Economy
    [DynamicCommand(nameof(RouteShop), MatchMode.Regex, MessageScope.Group)]
    public void Shop(GroupMessageContext e, string args)
    {
        if (Guard(e))
        {
            EconomyFeature.Instance.Shop(e, args);
        }
    }

    [DynamicCommand(nameof(RouteOpenEgg), MatchMode.Regex, MessageScope.Group)]
    public void OpenEgg(GroupMessageContext e, string args)
    {
        if (Guard(e))
        {
            EconomyFeature.Instance.OpenEgg(e, args);
        }
    }

    [DynamicCommand(nameof(RouteBlindBox), MatchMode.Regex, MessageScope.Group)]
    public void BlindBox(GroupMessageContext e, string args)
    {
        if (Guard(e))
        {
            EconomyFeature.Instance.OpenBlindBox(e, args);
        }
    }

    [DynamicCommand(nameof(RouteUseItem), MatchMode.Regex, MessageScope.Group)]
    public void UseItem(GroupMessageContext e, string args)
    {
        if (Guard(e))
        {
            EconomyFeature.Instance.UseItem(e, args);
        }
    }

    // Ranking
    [DynamicCommand(nameof(RouteRanking), MatchMode.Regex, MessageScope.Group)]
    public void Ranking(GroupMessageContext e)
    {
        if (Guard(e))
        {
            RankingFeature.Instance.Global(e);
        }
    }

    [DynamicCommand(nameof(RouteRankingGroup), MatchMode.Regex, MessageScope.Group)]
    public void GroupRanking(GroupMessageContext e)
    {
        if (Guard(e))
        {
            RankingFeature.Instance.Group(e);
        }
    }

    [DynamicCommand(nameof(RoutePunishInfo), MatchMode.Regex, MessageScope.Group)]
    public void PunishInfo(GroupMessageContext e)
    {
        if (Guard(e))
        {
            RankingFeature.Instance.PunishInfo(e);
        }
    }

    // Work
    [DynamicCommand(nameof(RouteStartIdle), MatchMode.Regex, MessageScope.Group)]
    public void StartIdle(GroupMessageContext e, string args)
    {
        if (Guard(e))
        {
            WorkFeature.Instance.StartIdle(e, args);
        }
    }

    [DynamicCommand(nameof(RouteStopIdle), MatchMode.Regex, MessageScope.Group)]
    public void StopIdle(GroupMessageContext e)
    {
        if (Guard(e))
        {
            WorkFeature.Instance.StopIdle(e);
        }
    }

    [DynamicCommand(nameof(RouteStartWork), MatchMode.Regex, MessageScope.Group)]
    public void StartWork(GroupMessageContext e, string args)
    {
        if (Guard(e))
        {
            WorkFeature.Instance.StartWork(e, args);
        }
    }

    [DynamicCommand(nameof(RouteStopWork), MatchMode.Regex, MessageScope.Group)]
    public void StopWork(GroupMessageContext e)
    {
        if (Guard(e))
        {
            WorkFeature.Instance.StopWork(e);
        }
    }

    /// <summary>群白名单守卫：未启用返回 false 跳过处理</summary>
    private static bool Guard(GroupMessageContext e) => CommandHelper.GroupEnabled(e);
}