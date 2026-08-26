using me.cqp.luohuaming.iKun.Infrastructure.Json;

namespace me.cqp.luohuaming.iKun.Domain.Configuration;

/// <summary>
/// 物品名称/描述/使用文案配置。
/// </summary>
public sealed class ItemConfiguration : JsonConfigFile
{
    public ItemConfiguration(string path) : base(path)
    {
        EnableAutoReload();
        Load();
    }

    /// <summary>热重载后的最新快照</summary>
    public static ItemConfiguration Current { get; private set; } = null!;

    internal static ItemConfiguration CreateCurrent(string path)
    {
        var instance = new ItemConfiguration(path);
        Current = instance;
        return instance;
    }

    // ---- 名称 ----
    public string CoinName { get; private set; } = "金币";

    public string KunEggName { get; private set; } = "鲲之蛋";

    public string BlindBoxName { get; private set; } = "盲盒";

    public string ResurrectPillName { get; private set; } = "复活丸";

    public string TransmogrifyPillName { get; private set; } = "幻化丸";

    public string UpgradePillName { get; private set; } = "强化丸";

    public string AscendPillName { get; private set; } = "渡劫丹";

    public string LevelPillName { get; private set; } = "快速等级丹";

    public string WeightPillName { get; private set; } = "快速体重丹";

    // ---- 描述 ----
    public string CoinDescription { get; private set; } = "大陆上通用的货币";

    public string KunEggDescription { get; private set; } = "可用于孵化、强化鲲";

    public string BlindBoxDescription { get; private set; } = "能获得随机材料";

    public string ResurrectPillDescription { get; private set; } = "用于复活的道具，能复活鲲";

    public string TransmogrifyPillDescription { get; private set; } = "用于幻化的道具，能够随机更改鲲的词缀";

    public string UpgradePillDescription { get; private set; } = "用于强化的道具，能用于强化鲲";

    public string AscendPillDescription { get; private set; } = "能够临时提升渡劫成功率的道具";

    public string LevelPillDescription { get; private set; } = "能够迅速提升等级的道具";

    public string WeightPillDescription { get; private set; } = "能够迅速提升体重的道具";

    // ---- 使用文案 ----
    public string UseItemException { get; private set; } = "物品使用过程发生异常，排查日志解决问题";

    public string UseLevelPill { get; private set; } = "使用了 {0} 个快速等级丹，等级提升了 {1}，当前等级 {2}";

    public string UseWeightPill { get; private set; } = "使用了快速体重丹，当前体重为 {0} kg";

    protected override void Load()
    {
        CoinName = Get("CoinName", "金币");
        KunEggName = Get("KunEggName", "鲲之蛋");
        BlindBoxName = Get("BlindBoxName", "盲盒");
        ResurrectPillName = Get("ResurrectPillName", "复活丸");
        TransmogrifyPillName = Get("TransmogrifyPillName", "幻化丸");
        UpgradePillName = Get("UpgradePillName", "强化丸");
        AscendPillName = Get("AscendPillName", "渡劫丹");
        LevelPillName = Get("LevelPillName", "快速等级丹");
        WeightPillName = Get("WeightPillName", "快速体重丹");

        CoinDescription = Get("CoinDescription", "大陆上通用的货币");
        KunEggDescription = Get("KunEggDescription", "可用于孵化、强化鲲");
        BlindBoxDescription = Get("BlindBoxDescription", "能获得随机材料");
        ResurrectPillDescription = Get("ResurrectPillDescription", "用于复活的道具，能复活鲲");
        TransmogrifyPillDescription = Get("TransmogrifyPillDescription", "用于幻化的道具，能够随机更改鲲的词缀");
        UpgradePillDescription = Get("UpgradePillDescription", "用于强化的道具，能用于强化鲲");
        AscendPillDescription = Get("AscendPillDescription", "能够临时提升渡劫成功率的道具");
        LevelPillDescription = Get("LevelPillDescription", "能够迅速提升等级的道具");
        WeightPillDescription = Get("WeightPillDescription", "能够迅速提升体重的道具");

        UseItemException = Get("UseItemException", "物品使用过程发生异常，排查日志解决问题");
        UseLevelPill = Get("UseLevelPill", "使用了 {0} 个快速等级丹，等级提升了 {1}，当前等级 {2}");
        UseWeightPill = Get("UseWeightPill", "使用了快速体重丹，当前体重为 {0} kg");
    }
}