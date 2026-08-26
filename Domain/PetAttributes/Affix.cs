using me.cqp.luohuaming.iKun.Domain.Enums;
using me.cqp.luohuaming.iKun.Infrastructure;

namespace me.cqp.luohuaming.iKun.Domain.PetAttributes;

/// <summary>
/// 副词缀：编号 1~80，每种对应一个效果类型与幅度。
/// </summary>
public sealed class Affix : PetAttribute
{
    private const int FallbackId = 79; // 无属性

    private static IReadOnlyDictionary<int, (string Name, AffixEffect Effect, double Value)>? _table;

    private static IReadOnlyDictionary<AffixEffect, string> _effectNames = new Dictionary<AffixEffect, string>
    {
        [AffixEffect.UpgradeWeightGainUp] = "◇{0}提升洗髓时获取的体重",
        [AffixEffect.AttackWeightGainUp] = "◇{0}提升攻击后获得的体重",
        [AffixEffect.AttackDamageUp] = "◇{0}提升攻击时造成的伤害",
        [AffixEffect.ReduceIncomingDamage] = "◇{0}降低攻击时受到的伤害",
        [AffixEffect.AscendWeightGainUp] = "◇{0}增加渡劫成功时提升的体重",
        [AffixEffect.AscendSuccessRateUp] = "◇{0}提升渡劫成功率",
        [AffixEffect.ReduceAscendFailLoss] = "◇{0}降低渡劫失败时损失的体重",
        [AffixEffect.FeedWeightGainUp] = "◇{0}提升喂食时获得的体重",
        [AffixEffect.TransmogrifySuccessRateUp] = "◇{0}提升蜕变成功率",
        [AffixEffect.ReduceTransmogrifyFailLoss] = "◇{0}降低蜕变失败时损失的体重",
        [AffixEffect.None] = "◇无属性",
    };

    private static IReadOnlyDictionary<double, string> _magnitudeNames = new Dictionary<double, string>
    {
        [0] = "",
        [0.01] = "微小幅度",
        [0.05] = "小幅度",
        [0.1] = "中幅度",
        [0.15] = "大幅度",
    };

    /// <summary>编号 →（名称、效果、幅度）映射表</summary>
    private static IReadOnlyDictionary<int, (string, AffixEffect, double)> Table =>
        _table ??= new Dictionary<int, (string, AffixEffect, double)>
        {
            { 1, ("白", AffixEffect.UpgradeWeightGainUp, 0.01) },
            { 2, ("灰", AffixEffect.UpgradeWeightGainUp, 0.05) },
            { 3, ("黑", AffixEffect.UpgradeWeightGainUp, 0.1) },
            { 4, ("墨", AffixEffect.UpgradeWeightGainUp, 0.15) },
            { 5, ("晶", AffixEffect.UpgradeWeightGainUp, 0.15) },
            { 6, ("锐", AffixEffect.AttackWeightGainUp, 0.01) },
            { 7, ("密", AffixEffect.AttackWeightGainUp, 0.05) },
            { 8, ("厚", AffixEffect.AttackWeightGainUp, 0.1) },
            { 9, ("重", AffixEffect.AttackWeightGainUp, 0.15) },
            { 10, ("野", AffixEffect.AttackDamageUp, 0.01) },
            { 11, ("勇", AffixEffect.AttackDamageUp, 0.05) },
            { 12, ("雪", AffixEffect.AttackDamageUp, 0.05) },
            { 13, ("霜", AffixEffect.AttackDamageUp, 0.05) },
            { 14, ("风", AffixEffect.AttackDamageUp, 0.05) },
            { 15, ("雷", AffixEffect.AttackDamageUp, 0.05) },
            { 16, ("电", AffixEffect.AttackDamageUp, 0.05) },
            { 17, ("金", AffixEffect.AttackDamageUp, 0.05) },
            { 18, ("木", AffixEffect.AttackDamageUp, 0.05) },
            { 19, ("水", AffixEffect.AttackDamageUp, 0.05) },
            { 20, ("火", AffixEffect.AttackDamageUp, 0.05) },
            { 21, ("烈", AffixEffect.AttackDamageUp, 0.05) },
            { 22, ("油", AffixEffect.AttackDamageUp, 0.05) },
            { 23, ("幽", AffixEffect.AttackDamageUp, 0.05) },
            { 24, ("刺", AffixEffect.AttackDamageUp, 0.05) },
            { 25, ("鳞", AffixEffect.AttackDamageUp, 0.05) },
            { 26, ("腐", AffixEffect.AttackDamageUp, 0.05) },
            { 27, ("牙", AffixEffect.AttackDamageUp, 0.05) },
            { 28, ("尖", AffixEffect.AttackDamageUp, 0.05) },
            { 29, ("刺", AffixEffect.AttackDamageUp, 0.05) },
            { 30, ("土", AffixEffect.AttackDamageUp, 0.05) },
            { 31, ("强", AffixEffect.AttackDamageUp, 0.1) },
            { 32, ("猛", AffixEffect.AttackDamageUp, 0.1) },
            { 33, ("傲", AffixEffect.AttackDamageUp, 0.15) },
            { 34, ("奇", AffixEffect.AttackDamageUp, 0.15) },
            { 35, ("毒", AffixEffect.AttackDamageUp, 0.15) },
            { 36, ("聪", AffixEffect.ReduceIncomingDamage, 0.01) },
            { 37, ("狡", AffixEffect.ReduceIncomingDamage, 0.05) },
            { 38, ("怒", AffixEffect.ReduceIncomingDamage, 0.1) },
            { 39, ("凶", AffixEffect.ReduceIncomingDamage, 0.15) },
            { 40, ("稳", AffixEffect.ReduceIncomingDamage, 0.15) },
            { 41, ("肉", AffixEffect.ReduceIncomingDamage, 0.15) },
            { 42, ("盾", AffixEffect.ReduceIncomingDamage, 0.15) },
            { 43, ("硬", AffixEffect.ReduceIncomingDamage, 0.15) },
            { 44, ("巨", AffixEffect.ReduceIncomingDamage, 0.15) },
            { 45, ("大", AffixEffect.ReduceIncomingDamage, 0.15) },
            { 46, ("蓝", AffixEffect.AscendWeightGainUp, 0.01) },
            { 47, ("粉", AffixEffect.AscendWeightGainUp, 0.01) },
            { 48, ("绿", AffixEffect.AscendWeightGainUp, 0.05) },
            { 49, ("黄", AffixEffect.AscendWeightGainUp, 0.1) },
            { 50, ("橙", AffixEffect.AscendWeightGainUp, 0.15) },
            { 51, ("碧", AffixEffect.AscendSuccessRateUp, 0.01) },
            { 52, ("敏", AffixEffect.AscendSuccessRateUp, 0.01) },
            { 53, ("紫", AffixEffect.AscendSuccessRateUp, 0.05) },
            { 54, ("红", AffixEffect.AscendSuccessRateUp, 0.1) },
            { 55, ("朱", AffixEffect.AscendSuccessRateUp, 0.1) },
            { 56, ("彩", AffixEffect.AscendSuccessRateUp, 0.15) },
            { 57, ("傻", AffixEffect.AscendSuccessRateUp, 0.15) },
            { 58, ("鸣", AffixEffect.ReduceAscendFailLoss, 0.01) },
            { 59, ("懒", AffixEffect.ReduceAscendFailLoss, 0.01) },
            { 60, ("游", AffixEffect.ReduceAscendFailLoss, 0.05) },
            { 61, ("琥", AffixEffect.ReduceAscendFailLoss, 0.1) },
            { 62, ("苍", AffixEffect.ReduceAscendFailLoss, 0.15) },
            { 63, ("铜", AffixEffect.FeedWeightGainUp, 0.01) },
            { 64, ("铁", AffixEffect.FeedWeightGainUp, 0.05) },
            { 65, ("银", AffixEffect.FeedWeightGainUp, 0.1) },
            { 66, ("金", AffixEffect.FeedWeightGainUp, 0.15) },
            { 67, ("贪", AffixEffect.FeedWeightGainUp, 0.15) },
            { 68, ("骄", AffixEffect.TransmogrifySuccessRateUp, 0.01) },
            { 69, ("炫", AffixEffect.TransmogrifySuccessRateUp, 0.01) },
            { 70, ("典", AffixEffect.TransmogrifySuccessRateUp, 0.05) },
            { 71, ("幽", AffixEffect.TransmogrifySuccessRateUp, 0.1) },
            { 72, ("古", AffixEffect.TransmogrifySuccessRateUp, 0.15) },
            { 73, ("蠢", AffixEffect.TransmogrifySuccessRateUp, 0.15) },
            { 74, ("迅", AffixEffect.ReduceTransmogrifyFailLoss, 0.01) },
            { 75, ("柔", AffixEffect.ReduceTransmogrifyFailLoss, 0.05) },
            { 76, ("乖", AffixEffect.ReduceTransmogrifyFailLoss, 0.05) },
            { 77, ("灵", AffixEffect.ReduceTransmogrifyFailLoss, 0.1) },
            { 78, ("耀", AffixEffect.ReduceTransmogrifyFailLoss, 0.15) },
            { 79, ("无", AffixEffect.None, 0) },
            { 80, ("菜虚", AffixEffect.ReduceIncomingDamage, 0.15) },
        };

    public AffixEffect Effect { get; }

    /// <summary>效果幅度（0 / 0.01 / 0.05 / 0.1 / 0.15）</summary>
    public double Magnitude { get; }

    public Affix(int id)
    {
        Element = Enums.Element.Affix;
        AffixId = id;
        if (!Table.TryGetValue(id, out var entry))
        {
            entry = Table[FallbackId];
        }
        Effect = entry.Item2;
        Magnitude = entry.Item3;
        Name = entry.Item1;
        Description = [string.Format(_effectNames[Effect], _magnitudeNames.TryGetValue(Magnitude, out var magnitudeName) ? magnitudeName : "")];
        if (Name == "无")
        {
            Name = "";
        }
    }

    /// <summary>随机副词缀</summary>
    public static Affix CreateRandom() => new(Extensions.Rng.Next(1, Table.Count + 1));

    // ---- 效果实现 ----

    public override double ModifyAscendWeight(double successRate, double multiplier = 1)
    {
        if (Effect == AffixEffect.ReduceAscendFailLoss && multiplier < 1)
        {
            multiplier = 1 - ((1 - multiplier) * (Magnitude + 1));
        }
        else if (Effect == AffixEffect.AscendWeightGainUp && multiplier > 1)
        {
            multiplier = 1 + ((multiplier - 1) * (Magnitude + 1));
        }
        return multiplier;
    }

    public override double ModifyAscendSuccessRate(double rate)
    {
        if (Effect == AffixEffect.AscendSuccessRateUp)
        {
            rate *= 1 + Magnitude;
        }
        return rate;
    }

    public override (double, double) ModifyAttack(double attacker, double defender, (double, double) baseMultiplier, double diff = 1)
    {
        if (baseMultiplier.Item1 <= 1)
        {
            return baseMultiplier;
        }
        if (Effect == AffixEffect.AttackDamageUp)
        {
            double attackerGain = attacker * (baseMultiplier.Item1 - 1);
            double defenderLoss = defender * (1 - baseMultiplier.Item2);
            attackerGain += defenderLoss * Magnitude;
            defenderLoss += defenderLoss * Magnitude;
            return (1 + (attackerGain / attacker), 1 - (defenderLoss / defender));
        }
        if (Effect == AffixEffect.AttackWeightGainUp)
        {
            return (baseMultiplier.Item1 * (1 + Magnitude), baseMultiplier.Item2);
        }
        return baseMultiplier;
    }

    public override (double, double) ModifyBeingAttacked(double attacker, double defender, (double, double) baseMultiplier)
    {
        if (Effect == AffixEffect.ReduceIncomingDamage && baseMultiplier.Item2 < 1)
        {
            return (baseMultiplier.Item1, 1 - ((1 - baseMultiplier.Item2) * Magnitude));
        }
        return baseMultiplier;
    }

    public override double ModifyFeed(int count, double multiplier = 1)
    {
        if (Effect == AffixEffect.FeedWeightGainUp)
        {
            multiplier *= 1 + Magnitude;
        }
        return multiplier;
    }

    public override double ModifyUpgrade(int count, double multiplier = 1)
    {
        if (Effect == AffixEffect.UpgradeWeightGainUp)
        {
            multiplier *= 1 + Magnitude;
        }
        return multiplier;
    }

    public override double ModifyTransmogrifyFailRate(double failRate)
    {
        if (Effect == AffixEffect.ReduceTransmogrifyFailLoss)
        {
            failRate *= 1 - Magnitude;
        }
        return failRate;
    }

    public override double ModifyTransmogrifyFailKeepRate(double keepRate)
    {
        if (Effect == AffixEffect.TransmogrifySuccessRateUp)
        {
            keepRate *= 1 - Magnitude;
        }
        return keepRate;
    }
}