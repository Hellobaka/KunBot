using me.cqp.luohuaming.iKun.Domain.Configuration;
using me.cqp.luohuaming.iKun.Domain.Enums;
using me.cqp.luohuaming.iKun.Infrastructure;
using me.cqp.luohuaming.iKun.Infrastructure.Logging;

namespace me.cqp.luohuaming.iKun.Domain.Models;

/// <summary>
/// 物品定义：名称/描述/可堆叠/可用。文案来自 ItemConfiguration。
/// 可用物品通过 <see cref="ItemCatalog"/> 返回带 Use 行为的子类型。
/// </summary>
public record Item
{
    private static readonly Log Log = Log.For(nameof(Item));

    public ItemId Id { get; init; }
    public string Name { get; init; } = "";
    public string Description { get; init; } = "";
    public bool Stackable { get; init; } = true;
    public bool Usable { get; init; }

    /// <summary>本次操作的数量（发放/购买时使用）</summary>
    public int Count { get; set; } = 1;

    public static Item Coin(int count = 1) => FromId(ItemId.Coin)! with { Count = count };
    public static Item KunEgg(int count = 1) => FromId(ItemId.KunEgg)! with { Count = count };
    public static Item BlindBox(int count = 1) => FromId(ItemId.BlindBox)! with { Count = count };

    /// <summary>按 ID 构造定义；可用物品直接返回带使用行为的子类型</summary>
    public static Item? FromId(ItemId id)
    {
        var config = ItemConfiguration.Current;
        return id switch
        {
            ItemId.Coin => new Item { Id = id, Name = config.CoinName, Description = config.CoinDescription },
            ItemId.KunEgg => new Item { Id = id, Name = config.KunEggName, Description = config.KunEggDescription },
            ItemId.BlindBox => new Item { Id = id, Name = config.BlindBoxName, Description = config.BlindBoxDescription },
            ItemId.ResurrectPill => new Item { Id = id, Name = config.ResurrectPillName, Description = config.ResurrectPillDescription },
            ItemId.TransmogrifyPill => new Item { Id = id, Name = config.TransmogrifyPillName, Description = config.TransmogrifyPillDescription },
            ItemId.UpgradePill => new Item { Id = id, Name = config.UpgradePillName, Description = config.UpgradePillDescription },
            ItemId.AscendPill => new Item { Id = id, Name = config.AscendPillName, Description = config.AscendPillDescription },
            ItemId.LevelPill => new LevelPillItem(id, config.LevelPillName, config.LevelPillDescription),
            ItemId.WeightPill => new WeightPillItem(id, config.WeightPillName, config.WeightPillDescription),
            _ => null,
        };
    }

    /// <summary>按名称查找（用于 #使用物品 名称）</summary>
    public static Item? FromName(string name)
    {
        var config = ItemConfiguration.Current;
        if (name == config.CoinName)
        {
            return FromId(ItemId.Coin);
        }

        if (name == config.KunEggName)
        {
            return FromId(ItemId.KunEgg);
        }

        if (name == config.BlindBoxName)
        {
            return FromId(ItemId.BlindBox);
        }

        if (name == config.ResurrectPillName)
        {
            return FromId(ItemId.ResurrectPill);
        }

        if (name == config.TransmogrifyPillName)
        {
            return FromId(ItemId.TransmogrifyPill);
        }

        if (name == config.UpgradePillName)
        {
            return FromId(ItemId.UpgradePill);
        }

        if (name == config.AscendPillName)
        {
            return FromId(ItemId.AscendPill);
        }

        if (name == config.LevelPillName)
        {
            return FromId(ItemId.LevelPill);
        }

        if (name == config.WeightPillName)
        {
            return FromId(ItemId.WeightPill);
        }

        return null;
    }

    /// <summary>
    /// 使用物品。默认完成扣除，调用方需先校验可用状态与数量。
    /// 返回 (是否成功, 反馈文本)。
    /// </summary>
    public virtual (bool Success, string Reply) Use(int count, Player player, Kun kun) =>
        (false, "物品没有使用效果");

    public override string ToString() => $"{Name} {Count}";
}

/// <summary>快速等级丹：直接加等级</summary>
public sealed record LevelPillItem : Item
{
    public LevelPillItem(ItemId id, string name, string description)
    {
        Id = id;
        Name = name;
        Description = description;
        Usable = true;
    }

    public override (bool Success, string Reply) Use(int count, Player player, Kun kun)
    {
        try
        {
            kun.Level += count;
            kun.Save();
            var config = ItemConfiguration.Current;
            return (true, string.Format(config.UseLevelPill, count, count, kun.Level));
        }
        catch (Exception e)
        {
            Log.For("使用物品").Error(e, "使用异常");
            return (false, ItemConfiguration.Current.UseItemException);
        }
    }
}

/// <summary>快速体重丹：体重拉满至当前等级上限</summary>
public sealed record WeightPillItem : Item
{
    public WeightPillItem(ItemId id, string name, string description)
    {
        Id = id;
        Name = name;
        Description = description;
        Usable = true;
    }

    public override (bool Success, string Reply) Use(int count, Player player, Kun kun)
    {
        try
        {
            kun.LoadAffixes();
            kun.Weight = Kun.WeightLimitOf(kun.Level);
            kun.Save();
            return (true, string.Format(ItemConfiguration.Current.UseWeightPill, kun.Weight.ToShortNumber()));
        }
        catch (Exception e)
        {
            Log.For("使用物品").Error(e, "使用异常");
            return (false, ItemConfiguration.Current.UseItemException);
        }
    }
}

/// <summary>物品目录：统一入口解析定义</summary>
public static class ItemCatalog
{
    private static readonly Log Log = Log.For(nameof(ItemCatalog));

    /// <summary>枚举最大值（商店/盲盒配置解析用）</summary>
    public static int MaxId => Enum.GetValues<ItemId>().Cast<int>().Max();

    /// <summary>按 ID 获取定义（已包含可用子类型）</summary>
    public static Item? Definition(ItemId id) => Item.FromId(id);

    /// <summary>按名称获取定义</summary>
    public static Item? ByName(string name) => Item.FromName(name);

    /// <summary>
    /// 解析输入（ID 或名称）为物品实例；失败返回 null。
    /// </summary>
    public static Item? Resolve(string input)
    {
        var definition = ByName(input);
        if (definition is null && int.TryParse(input, out var numeric))
        {
            definition = Item.FromId((ItemId)numeric);
        }
        if (definition is null)
        {
            Log.Debug($"无法解析物品: {input}");
        }
        return definition;
    }
}