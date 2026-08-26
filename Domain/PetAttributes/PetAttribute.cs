using me.cqp.luohuaming.iKun.Domain.Configuration;
using me.cqp.luohuaming.iKun.Domain.Enums;
using me.cqp.luohuaming.iKun.Infrastructure;
using me.cqp.luohuaming.iKun.Infrastructure.Logging;

namespace me.cqp.luohuaming.iKun.Domain.PetAttributes;

/// <summary>
/// 词缀基类：修饰鲲的各项数值计算。
/// 约定：
/// - 攻击与吞噬操作不可重复进行，防止更改既定结果
/// - 增加体重的操作在原始结果上乘算；降低伤害时乘算
/// - 单独增减伤害需先计算双方增减量，再乘算加成
/// </summary>
public abstract class PetAttribute
{
    protected Log Log { get; } = Log.For("词缀");

    /// <summary>展示用描述行</summary>
    public string[] Description { get; protected set; } = [];

    /// <summary>主词缀元素（副词缀恒为 Element.Affix）</summary>
    public Element Element { get; protected set; }

    /// <summary>展示名</summary>
    public string Name { get; protected set; } = "";

    /// <summary>副词缀编号（主词缀为 0）</summary>
    public int AffixId { get; protected set; }

    // ---- 渡劫 ----

    /// <summary>渡劫体重变化倍率。默认成功 +min~max%，失败 -min~max%</summary>
    public virtual double ModifyAscendWeight(double successRate, double multiplier = 1)
    {
        var config = CoreConfiguration.Current;
        var roll = Extensions.Rng.NextDouble();
        return (roll <= successRate
                ? 1 + (Extensions.Rng.NextDouble(config.AscendGainMinPercent, config.AscendGainMaxPercent) / 100)
                : 1 - (Extensions.Rng.NextDouble(config.AscendLossMinPercent, config.AscendLossMaxPercent) / 100))
            * multiplier;
    }

    /// <summary>修正渡劫成功率</summary>
    public virtual double ModifyAscendSuccessRate(double rate) => rate;

    /// <summary>修正幻化失败率</summary>
    public virtual double ModifyTransmogrifyFailRate(double failRate) => failRate;

    /// <summary>修正幻化失败后体重保留率</summary>
    public virtual double ModifyTransmogrifyFailKeepRate(double keepRate) => keepRate;

    // ---- 攻击 ----

    /// <summary>
    /// 攻击倍率计算。返回 (攻方倍率, 被攻方倍率)。
    /// 被攻方按比例损失体重，攻方获得等量。
    /// </summary>
    public virtual (double Attacker, double Defender) ModifyAttack(
        double attackerWeight, double defenderWeight, (double Attacker, double Defender) baseMultiplier, double diff = 1)
    {
        var config = CoreConfiguration.Current;
        var roll = Extensions.Rng.NextDouble(config.AttackDamageMinPercent / 100.0, config.AttackDamageMaxPercent / 100.0);
        var damage = Math.Min(attackerWeight * roll * diff, defenderWeight);
        return (baseMultiplier.Attacker + (damage / attackerWeight), baseMultiplier.Defender - (damage / defenderWeight));
    }

    /// <summary>受击时的倍率修正</summary>
    public virtual (double Attacker, double Defender) ModifyBeingAttacked(
        double attackerWeight, double defenderWeight, (double Attacker, double Defender) baseMultiplier) => baseMultiplier;

    /// <summary>
    /// 吞噬计算：体重接近时随机胜负。
    /// 返回 (攻方体重增量, 被攻方体重增量)。
    /// </summary>
    public virtual (double Attacker, double Defender) ModifyDevour(double attackerWeight, double defenderWeight, double diff = 1)
    {
        var config = CoreConfiguration.Current;
        bool success;
        if (Math.Abs((defenderWeight - attackerWeight) / attackerWeight * 100) < config.DevourDrawRangePercent)
        {
            success = Extensions.Rng.NextDouble() < 0.5;
        }
        else
        {
            success = attackerWeight > defenderWeight;
        }

        if (success)
        {
            var gain = Extensions.Rng.NextDouble(0.5, 1) * diff * defenderWeight;
            return (gain, -gain);
        }
        var loss = Extensions.Rng.NextDouble(0.5, 0.7) * diff * attackerWeight;
        return (-loss, loss);
    }

    /// <summary>被吞噬时的修正</summary>
    public virtual (double Attacker, double Defender) ModifyBeingDevoured(
        double devouredWeight, double devourerWeight, (double Attacker, double Defender) baseDevour) => baseDevour;

    // ---- 喂养/强化 ----

    /// <summary>喂养体重加成倍率（加算叠加）</summary>
    public virtual double ModifyFeed(int count, double multiplier = 1)
    {
        var config = CoreConfiguration.Current;
        double total = 0;
        for (int i = 0; i < count; i++)
        {
            total += Extensions.Rng.Next(config.FeedWeightMinBonusPercent, config.FeedWeightMaxBonusPercent) / 100.0 * multiplier;
        }
        return total;
    }

    /// <summary>强化体重倍率修正</summary>
    public virtual double ModifyUpgrade(int count, double multiplier = 1) => multiplier;
}