using Another_Mirai_Native.Abstractions.Context;
using me.cqp.luohuaming.iKun.Domain.Configuration;
using me.cqp.luohuaming.iKun.Domain.Enums;
using me.cqp.luohuaming.iKun.Domain.Models;
using me.cqp.luohuaming.iKun.Features.Shared;

using me.cqp.luohuaming.iKun.Infrastructure;

namespace me.cqp.luohuaming.iKun.Features.Economy;

/// <summary>
/// 经济指令：商店、开鲲蛋、开盲盒、使用物品。
/// </summary>
public sealed class EconomyFeature
{
    private List<(ItemId Id, double Weight)>? _blindBoxTable;

    private List<(Item Goods, Item Price)> _shopEntries = [];

    private EconomyFeature()
    { }

    public static EconomyFeature Instance { get; } = new();
    // ---- 开盲盒 ----
    public void OpenBlindBox(GroupMessageContext e, string args)
    {
        var config = CoreConfiguration.Current;
        var replies = config.Replies;
        var countOrNull = CommandHelper.ParseInt(args, defaultToOne: true);
        if (countOrNull is null)
        {
            CommandHelper.Reply(e, CommandHelper.InvalidParams($"，示例：{config.CommandOpenBlindBox} 数量"));
            return;
        }
        int count = Math.Max(1, countOrNull.Value);
        var player = Player.Find(e.FromQQ.Id);
        if (player is null)
        {
            CommandHelper.Reply(e, replies.NoPlayer);
            return;
        }
        if (!InventoryItem.TryRemove(player, ItemId.BlindBox, count, out int remaining))
        {
            CommandHelper.Reply(e, string.Format(replies.ItemLeak, ItemCatalog.Definition(ItemId.BlindBox)!.Name, count, remaining));
            return;
        }
        BuildBlindBoxTable();

        var obtained = new List<Item>();
        for (int i = 0; i < count; i++)
        {
            foreach (var id in DrawBlindBox())
            {
                if (id != ItemId.Nothing)
                {
                    obtained.Add(ItemCatalog.Definition(id)!);
                }
            }
        }
        if (obtained.Count == 0)
        {
            CommandHelper.Reply(e, string.Format(replies.BlindBoxEmpty, count));
            return;
        }
        var summary = new System.Text.StringBuilder();
        foreach (var group in obtained.GroupBy(x => x.Name))
        {
            summary.AppendLine($"· {group.Key} {group.Count()} 个");
        }
        summary.RemoveTrailingNewLine();
        player.GrantItems(obtained);
        CommandHelper.Reply(e, string.Format(replies.BlindBoxOpened, count, summary));
    }

    public void OpenEgg(GroupMessageContext e, string args)
    {
        var config = CoreConfiguration.Current;
        var replies = config.Replies;
        var countOrNull = CommandHelper.ParseInt(args, defaultToOne: true);
        if (countOrNull is null)
        {
            CommandHelper.Reply(e, CommandHelper.InvalidParams($"，示例：{config.CommandOpenEgg} 数量"));
            return;
        }
        int count = Math.Max(1, countOrNull.Value);
        var player = Player.Find(e.FromQQ.Id);
        if (player is null)
        {
            CommandHelper.Reply(e, replies.NoPlayer);
            return;
        }
        const int coinPerEgg = 10;
        int coins = InventoryItem.CountOf(player, ItemId.Coin);
        int eggs = InventoryItem.CountOf(player, ItemId.KunEgg);
        if (coins < count * coinPerEgg)
        {
            CommandHelper.Reply(e, string.Format(replies.ItemLeak, ItemCatalog.Definition(ItemId.Coin)!.Name, count * coinPerEgg, coins));
            return;
        }
        if (eggs < count)
        {
            CommandHelper.Reply(e, string.Format(replies.ItemLeak, ItemCatalog.Definition(ItemId.KunEgg)!.Name, count, eggs));
            return;
        }
        InventoryItem.TryRemove(player, ItemId.Coin, count * coinPerEgg, out _);
        InventoryItem.TryRemove(player, ItemId.KunEgg, count, out _);
        int blindBoxes = count * config.EggToBlindBoxRate;
        player.GrantItems([Item.BlindBox(blindBoxes)]);
        CommandHelper.Reply(e, string.Format(replies.OpenEggResult, count, blindBoxes));
    }

    // ---- 商店 ----
    public void Shop(GroupMessageContext e, string args)
    {
        var config = CoreConfiguration.Current;
        var replies = config.Replies;
        var parts = args.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        // 无参数：展示列表
        if (parts.Length == 0)
        {
            RebuildShop();
            var builder = new System.Text.StringBuilder();
            builder.AppendLine(replies.ShopHeader);
            for (int i = 1; i <= _shopEntries.Count; i++)
            {
                var shopEntry = _shopEntries[i - 1];
                builder.AppendLine(replies.ShopEntry
                    .Replace("%Index%", $"{i}")
                    .Replace("%CoinCount%", $"{shopEntry.Price.Count}")
                    .Replace("%CoinName%", shopEntry.Price.Name)
                    .Replace("%ItemCount%", $"{shopEntry.Goods.Count}")
                    .Replace("%ItemName%", shopEntry.Goods.Name));
            }
            builder.Append($"示例：{config.CommandShopping} 序号 数量");
            CommandHelper.Reply(e, builder.ToString());
            return;
        }

        if (parts.Length != 2 || !int.TryParse(parts[0], out int index) || !int.TryParse(parts[1], out int count))
        {
            CommandHelper.Reply(e, CommandHelper.InvalidParams($"，示例：{config.CommandShopping} 序号 数量"));
            return;
        }
        count = Math.Max(1, count);

        var player = Player.Find(e.FromQQ.Id);
        if (player is null)
        {
            CommandHelper.Reply(e, replies.NoPlayer);
            return;
        }
        RebuildShop();
        if (index < 1 || index > _shopEntries.Count)
        {
            CommandHelper.Reply(e, replies.ShopIndexInvalid);
            return;
        }
        var (entryGoods, entryPrice) = _shopEntries[index - 1];
        long totalCost = 1L * count * entryPrice.Count;
        if (totalCost > int.MaxValue)
        {
            CommandHelper.Reply(e, CommandHelper.InvalidParams(""));
            return;
        }
        if (!InventoryItem.TryRemove(player, entryPrice.Id, (int)totalCost, out int remaining))
        {
            CommandHelper.Reply(e, string.Format(replies.ItemLeak, entryPrice.Name, totalCost, remaining));
            return;
        }
        var granted = entryGoods with { Count = entryGoods.Count * count };
        player.GrantItems([granted]);
        CommandHelper.Reply(e, string.Format(
            replies.PurchaseSuccess, totalCost, entryPrice.Name,
            granted.Count, granted.Name, remaining, InventoryItem.CountOf(player, granted.Id)));
    }

    public void UseItem(GroupMessageContext e, string args)
    {
        var config = CoreConfiguration.Current;
        var replies = config.Replies;
        var parts = args.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length is not (1 or 2))
        {
            CommandHelper.Reply(e, CommandHelper.InvalidParams($"，示例：{config.CommandUseItem} 物品ID/名称 数量"));
            return;
        }
        var definition = ItemCatalog.Resolve(parts[0]);
        if (definition is null)
        {
            CommandHelper.Reply(e, CommandHelper.InvalidParams("，指定的物品 ID 或名称无效"));
            return;
        }
        int count = parts.Length == 2 && int.TryParse(parts[1], out var parsedCount) ? Math.Max(1, parsedCount) : 1;
        if (!definition.Usable)
        {
            CommandHelper.Reply(e, replies.ItemCannotUse);
            return;
        }
        if (!CommandHelper.TryLoadPlayerAndKun(e, out var player, out var kun))
        {
            return;
        }
        if (CommandHelper.IsBusyReplyIfSo(e, kun))
        {
            return;
        }
        if (!InventoryItem.TryRemove(player, definition.Id, count, out int remaining))
        {
            CommandHelper.Reply(e, string.Format(replies.ItemLeak, definition.Name, count, remaining));
            return;
        }
        var (success, replyText) = definition.Use(count, player, kun);
        if (!success)
        {
            CommandHelper.Reply(e, replies.ItemUseFailed);
            // 返还
            player.GrantItems([definition with { Count = count }]);
            return;
        }
        CommandHelper.Reply(e, replyText);
    }

    private static int ParseIntOr(string? input, int fallback) => int.TryParse(input, out var v) ? v : fallback;

    private void BuildBlindBoxTable()
    {
        _blindBoxTable ??= [];
        if (_blindBoxTable.Count > 0)
        {
            return;
        }
        var table = new List<(ItemId, double)>();
        foreach (var raw in CoreConfiguration.Current.BlindBoxContentsRaw)
        {
            var parts = raw.Split('|');
            int id = int.TryParse(parts.ElementAtOrDefault(0), out var i) ? i : -1;
            double weight = double.TryParse(parts.ElementAtOrDefault(1), out var w) ? w : -1;
            if (id >= 0 && id <= ItemCatalog.MaxId)
            {
                table.Add(((ItemId)id, weight));
            }
        }
        _blindBoxTable = table;
    }

    // ---- 开鲲蛋 → 盲盒 ----
    /// <summary>单次抽取（含多连抽逻辑）</summary>
    private List<ItemId> DrawBlindBox(bool redraw = false)
    {
        var config = CoreConfiguration.Current;
        var table = redraw && config.BlindBoxMultiDrawMustContainItem
            ? _blindBoxTable!.Where(x => x.Id != ItemId.Nothing).ToList()
            : _blindBoxTable!;
        double totalWeight = table.Sum(x => x.Weight);
        var result = new List<ItemId>();
        double roll = Extensions.Rng.NextDouble() * totalWeight;
        double cumulative = 0;
        foreach (var entry in table)
        {
            cumulative += entry.Weight;
            if (roll < cumulative)
            {
                result.Add(entry.Id);
                break;
            }
        }
        // 多内容重抽
        if (!redraw && config.BlindBoxAllowMultiDraw &&
            Extensions.Rng.NextDouble() < config.BlindBoxMultiDrawChancePercent / 100.0)
        {
            result.AddRange(DrawBlindBox(redraw: true));
        }
        return result;
    }

    /// <summary>从配置解析商店条目："数量|物品ID|单价|货币ID"</summary>
    private void RebuildShop()
    {
        _shopEntries = [];
        foreach (var trade in CoreConfiguration.Current.ShoppingListRaw)
        {
            var parts = trade.Split('|');
            int count = ParseIntOr(parts.ElementAtOrDefault(0), -1);
            int itemId = ParseIntOr(parts.ElementAtOrDefault(1), -1);
            int price = ParseIntOr(parts.ElementAtOrDefault(2), -1);
            int currencyId = ParseIntOr(parts.ElementAtOrDefault(3), -1);
            if (count <= 0 || itemId <= 0 || itemId > ItemCatalog.MaxId ||
                price <= 0 || currencyId <= 0 || currencyId > ItemCatalog.MaxId)
            {
                continue;
            }
            var goods = ItemCatalog.Definition((ItemId)itemId)! with { Count = count };
            var currency = ItemCatalog.Definition((ItemId)currencyId)! with { Count = price };
            _shopEntries.Add((goods, currency));
        }
    }
}