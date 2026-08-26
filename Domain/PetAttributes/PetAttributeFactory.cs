using me.cqp.luohuaming.iKun.Domain.Configuration;
using me.cqp.luohuaming.iKun.Domain.Enums;
using me.cqp.luohuaming.iKun.Infrastructure;

namespace me.cqp.luohuaming.iKun.Domain.PetAttributes;

/// <summary>
/// 主词缀工厂：按配置概率随机实例化，或按元素/副词缀编号精确实例化。
/// </summary>
public static class PetAttributeFactory
{
    private static IReadOnlyList<(Element Element, double Weight, Func<PetAttribute> Create)>? _weightedTable;

    private static IReadOnlyList<(Element, double, Func<PetAttribute>)> BuildWeightedTable()
    {
        var c = CoreConfiguration.Current;
        return
        [
            (Element.None, c.ProbabilityNone, () => new MainAffixes.NoneAffix()),
            (Element.Metal, c.ProbabilityJin, () => new MainAffixes.MetalAffix()),
            (Element.Wood, c.ProbabilityMu, () => new MainAffixes.WoodAffix()),
            (Element.Water, c.ProbabilityShui, () => new MainAffixes.WaterAffix()),
            (Element.Fire, c.ProbabilityHuo, () => new MainAffixes.FireAffix()),
            (Element.Earth, c.ProbabilityTu, () => new MainAffixes.EarthAffix()),
            (Element.Wind, c.ProbabilityFeng, () => new MainAffixes.WindAffix()),
            (Element.Thunder, c.ProbabilityLei, () => new MainAffixes.ThunderAffix()),
            (Element.Yin, c.ProbabilityYin, () => new MainAffixes.YinAffix()),
            (Element.Yang, c.ProbabilityYang, () => new MainAffixes.YangAffix()),
        ];
    }

    /// <summary>按配置权重随机主词缀</summary>
    public static PetAttribute CreateRandomMain()
    {
        _weightedTable ??= BuildWeightedTable();
        double total = 0;
        foreach (var entry in _weightedTable)
        {
            total += entry.Weight;
        }

        var roll = Extensions.Rng.NextDouble() * total;
        var cumulative = 0.0;
        foreach (var entry in _weightedTable)
        {
            cumulative += entry.Weight;
            if (roll <= cumulative)
            {
                return entry.Create();
            }
        }
        return new MainAffixes.NoneAffix();
    }

    /// <summary>按存储 ID 精确实例化（main=true 解析主词缀，否则解析副词缀编号）</summary>
    public static PetAttribute FromStoredId(bool isMain, int id) => isMain ? FromElement((Element)id) : new Affix(id);

    private static PetAttribute FromElement(Element element) => element switch
    {
        Element.Metal => new MainAffixes.MetalAffix(),
        Element.Wood => new MainAffixes.WoodAffix(),
        Element.Water => new MainAffixes.WaterAffix(),
        Element.Fire => new MainAffixes.FireAffix(),
        Element.Earth => new MainAffixes.EarthAffix(),
        Element.Wind => new MainAffixes.WindAffix(),
        Element.Thunder => new MainAffixes.ThunderAffix(),
        Element.Yin => new MainAffixes.YinAffix(),
        Element.Yang => new MainAffixes.YangAffix(),
        _ => new MainAffixes.NoneAffix(),
    };
}