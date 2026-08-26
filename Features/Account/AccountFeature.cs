using Another_Mirai_Native.Abstractions.Context;
using me.cqp.luohuaming.iKun.Domain.Configuration;
using me.cqp.luohuaming.iKun.Domain.Enums;
using me.cqp.luohuaming.iKun.Domain.Models;
using me.cqp.luohuaming.iKun.Features.Shared;

using me.cqp.luohuaming.iKun.Infrastructure;

namespace me.cqp.luohuaming.iKun.Features.Account;

/// <summary>
/// 账号相关指令：注册、签到、菜单、背包。
/// </summary>
public sealed class AccountFeature
{
    public static AccountFeature Instance { get; } = new();

    private AccountFeature()
    { }

    public void Register(GroupMessageContext e)
    {
        if (Player.Exists(e.FromQQ.Id))
        {
            CommandHelper.Reply(e, CoreConfiguration.Current.Replies.DuplicateRegister);
            return;
        }
        var config = CoreConfiguration.Current;
        var player = Player.Create(e.FromQQ.Id);
        if (player is null)
        {
            CommandHelper.Reply(e, config.Replies.RegisterFailed);
            return;
        }
        player.GrantItems([Item.Coin(config.RegisterRewardCoins), Item.KunEgg(config.RegisterRewardEggs)]);
        CommandHelper.Reply(e, string.Format(config.Replies.RegisterSuccess, config.RegisterRewardCoins, config.RegisterRewardEggs));
    }

    public void Login(GroupMessageContext e)
    {
        var player = Player.Find(e.FromQQ.Id);
        if (player is null)
        {
            CommandHelper.Reply(e, CoreConfiguration.Current.Replies.NoPlayer);
            return;
        }
        var config = CoreConfiguration.Current;
        if (Extensions.IsSameDay(player.LoginAt, DateTime.Now))
        {
            CommandHelper.Reply(e, config.Replies.DuplicateLogin);
            return;
        }
        player.LoginAt = DateTime.Now;
        player.Save();
        player.GrantItems([Item.Coin(config.LoginRewardCoins), Item.KunEgg(config.LoginRewardEggs)]);
        CommandHelper.Reply(e, string.Format(config.Replies.LoginSuccess, config.LoginRewardCoins, config.LoginRewardEggs));
    }

    public void Menu(GroupMessageContext e)
    {
        var c = CoreConfiguration.Current;
        var r = c.Replies;
        CommandHelper.Reply(e, string.Format(r.Menu,
            c.CommandRegister, c.CommandLogin,
            c.CommandFeed, c.CommandUpgrade,
            c.CommandHatch, c.CommandInventory,
            c.CommandShopping, c.CommandOpenBlindBox,
            c.CommandOpenEgg, c.CommandTransmogrify,
            c.CommandAttack, c.CommandDevour,
            c.CommandQueryDeadKuns, c.CommandRelease,
            c.CommandResurrect, c.CommandRanking,
            c.CommandAscend, c.CommandMenu));
    }

    public void Inventory(GroupMessageContext e)
    {
        if (!CommandHelper.TryLoadPlayerAndKun(e, out var player, out var kun))
        {
            // 未持有鲲时仍展示背包
            var foundPlayer = Player.Find(e.FromQQ.Id);
            if (foundPlayer is null)
            {
                return;
            }
            player = foundPlayer;
        }
        var replies = CoreConfiguration.Current.Replies;
        var builder = new System.Text.StringBuilder();
        builder.AppendLine(kun is null ? replies.NoKun : kun.ToDetailedString(false));
        builder.AppendLine("----");

        var items = InventoryItem.AllOf(e.FromQQ.Id).Where(x => x.Count > 0).ToList();
        if (items.Count == 0)
        {
            builder.AppendLine(replies.InventoryEmpty);
        }
        else
        {
            foreach (var entry in items)
            {
                if (ItemCatalog.Definition((ItemId)entry.ItemID) is not null)
                {
                    builder.AppendLine(entry.ToString());
                }
            }
        }
        builder.RemoveTrailingNewLine();
        CommandHelper.Reply(e, builder.ToString());
    }

    /// <summary>自定义鲲昵称</summary>
    public void SetNickName(GroupMessageContext e, string args)
    {
        var nickName = args.Trim();
        if (string.IsNullOrEmpty(nickName))
        {
            CommandHelper.Reply(e, CommandHelper.InvalidParams($"，示例：{CoreConfiguration.Current.CommandSetNickName} 昵称"));
            return;
        }
        if (!CommandHelper.TryLoadPlayerAndKun(e, out _, out var kun))
        {
            return;
        }
        if (CoreConfiguration.Current.NickNameFilter.Any(x => nickName.ToLower().Contains(x)))
        {
            CommandHelper.Reply(e, CoreConfiguration.Current.Replies.NickNameInvalid);
            return;
        }
        kun.NickName = nickName;
        kun.Save();
        CommandHelper.Reply(e, string.Format(CoreConfiguration.Current.Replies.NickNameApplied, kun));
    }

    /// <summary>恢复默认名称</summary>
    public void ClearNickName(GroupMessageContext e)
    {
        if (!CommandHelper.TryLoadPlayerAndKun(e, out _, out var kun))
        {
            return;
        }
        kun.LoadAffixes();
        kun.NickName = "";
        kun.Save();
        CommandHelper.Reply(e, string.Format(CoreConfiguration.Current.Replies.NickNameDiscarded, kun.ToString()));
    }
}