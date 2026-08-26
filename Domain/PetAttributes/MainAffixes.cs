using me.cqp.luohuaming.iKun.Infrastructure;

namespace me.cqp.luohuaming.iKun.Domain.PetAttributes.MainAffixes;

/// <summary>无词缀</summary>
public sealed class NoneAffix : PetAttribute
{
    public NoneAffix()
    {
        Element = Enums.Element.None;
        Name = "无";
        Description = [];
    }
}

/// <summary>金：穿透/反弹/吞噬增重，渡劫降成功率；克木</summary>
public sealed class MetalAffix : PetAttribute
{
    public MetalAffix()
    {
        Element = Enums.Element.Metal;
        Name = "金";
        Description =
        [
            "◆攻击或吞噬时有概率穿透防御",
            "◆被攻击时有概率反弹伤害",
            "◆成功吞噬后大量提升体重",
            "◆渡劫时大幅减少成功概率",
            "◇对「木」属性的对手有额外攻击加成",
        ];
    }

    public override (double, double) ModifyAttack(double attacker, double defender, (double, double) baseMultiplier, double diff = 1)
    {
        // 20% 概率提高 10~30% 伤害
        var r = base.ModifyAttack(attacker, defender, baseMultiplier, diff);
        if (r.Item1 <= 1 || Extensions.Rng.NextDouble() > 0.2)
        {
            return r;
        }
        var boost = Extensions.Rng.NextDouble(0.1, 0.3);
        double attackerGain = attacker * (r.Item1 - 1);
        double defenderLoss = defender * (1 - r.Item2);
        attackerGain += defenderLoss * boost;
        defenderLoss += defenderLoss * boost;
        return (1 + (attackerGain / attacker), 1 - (defenderLoss / defender));
    }

    public override (double, double) ModifyDevour(double attacker, double defender, double diff = 1)
    {
        // 穿透：20% 概率提高 10~30% 伤害；成功吞噬额外 +10~30%
        var r = base.ModifyDevour(attacker, defender, diff);
        if (r.Item1 > 1 && Extensions.Rng.NextDouble() < 0.2)
        {
            var boost = Extensions.Rng.NextDouble(0.1, 0.3);
            double attackerGain = attacker * (r.Item1 - 1);
            double defenderLoss = defender * (1 - r.Item2);
            defenderLoss -= defenderLoss * boost;
            attackerGain = defenderLoss;
            r = (1 + (attackerGain / attacker), 1 - (defenderLoss / defender));
        }
        if (r.Item1 > 1)
        {
            var extra = Extensions.Rng.NextDouble(0.1, 0.3);
            r = (r.Item1 * (1 + extra), r.Item2);
        }
        return r;
    }

    public override (double, double) ModifyBeingAttacked(double attacker, double defender, (double, double) baseMultiplier)
    {
        // 被击时 20% 概率反弹 10~30% 所受伤害
        if (Extensions.Rng.NextDouble() > 0.2)
        {
            return baseMultiplier;
        }
        var reflect = Extensions.Rng.NextDouble(0.1, 0.3);
        double attackerGain = attacker * (baseMultiplier.Item1 - 1);
        double defenderLoss = defender * (1 - baseMultiplier.Item2);
        attackerGain -= defenderLoss * reflect;
        return (1 + (attackerGain / attacker), baseMultiplier.Item2);
    }

    public override double ModifyAscendSuccessRate(double rate) => rate * 0.7;
}

/// <summary>木：吞噬超量增重、逃脱、渡劫提成功率；克土</summary>
public sealed class WoodAffix : PetAttribute
{
    public WoodAffix()
    {
        Element = Enums.Element.Wood;
        Name = "木";
        Description =
        [
            "◆成功吞噬后超巨量增加体重",
            "◆被攻击时有概率逃脱",
            "◆渡劫时大幅提高成功概率",
            "◇对「土」属性的对手有额外攻击加成",
        ];
    }

    public override (double, double) ModifyDevour(double attacker, double defender, double diff = 1)
    {
        // 成功吞噬额外增加敌人体重的 50~100%
        var r = base.ModifyDevour(attacker, defender, diff);
        if (r.Item1 > 1)
        {
            var extra = Extensions.Rng.NextDouble(0.5, 1);
            r = (r.Item1 * (1 + extra), r.Item2);
        }
        return r;
    }

    public override (double, double) ModifyBeingAttacked(double attacker, double defender, (double, double) baseMultiplier) =>
        Extensions.Rng.NextDouble() < 0.2 ? (1, 1) : baseMultiplier;

    public override double ModifyAscendSuccessRate(double rate) => rate * 1.3;
}

/// <summary>水：攻/噬后增重、高概率逃脱；克火</summary>
public sealed class WaterAffix : PetAttribute
{
    public WaterAffix()
    {
        Element = Enums.Element.Water;
        Name = "水";
        Description =
        [
            "◆成功攻击或吞噬后大量提升体重",
            "◆被攻击或吞噬时有大概率逃脱",
            "◇对「火」属性的对手有额外攻击加成",
        ];
    }

    public override (double, double) ModifyAttack(double attacker, double defender, (double, double) baseMultiplier, double diff = 1)
    {
        // 攻击成功时额外获得对方损失体重的 30~50%
        var r = base.ModifyAttack(attacker, defender, baseMultiplier, diff);
        if (r.Item1 <= 1)
        {
            return r;
        }
        double attackerGain = attacker * (r.Item1 - 1);
        double defenderLoss = defender * (1 - r.Item2);
        attackerGain += defenderLoss * Extensions.Rng.NextDouble(0.3, 0.5);
        return (1 + (attackerGain / attacker), r.Item2);
    }

    public override (double, double) ModifyDevour(double attacker, double defender, double diff = 1)
    {
        var r = base.ModifyDevour(attacker, defender, diff);
        if (r.Item1 > 1)
        {
            r = (r.Item1 * (1 + Extensions.Rng.NextDouble(0.1, 0.3)), r.Item2);
        }
        return r;
    }

    public override (double, double) ModifyBeingAttacked(double attacker, double defender, (double, double) baseMultiplier) =>
        Extensions.Rng.NextDouble() < 0.3 ? (1, 1) : baseMultiplier;

    public override (double, double) ModifyBeingDevoured(double devoured, double devourer, (double, double) baseDevour) =>
        Extensions.Rng.NextDouble() < 0.3 ? (1, 1) : baseDevour;
}

/// <summary>火：高概率额外伤害、吞噬增重；克金</summary>
public sealed class FireAffix : PetAttribute
{
    public FireAffix()
    {
        Element = Enums.Element.Fire;
        Name = "火";
        Description =
        [
            "◆攻击时有大概率造成额外少量伤害",
            "◆成功吞噬后提升大量体重",
            "◇对「金」属性的对手有额外攻击加成",
        ];
    }

    public override (double, double) ModifyAttack(double attacker, double defender, (double, double) baseMultiplier, double diff = 1)
    {
        // 50% 概率提高 10~30% 伤害
        var r = base.ModifyAttack(attacker, defender, baseMultiplier, diff);
        if (r.Item1 > 1 && Extensions.Rng.NextDouble() < 0.5)
        {
            var boost = Extensions.Rng.NextDouble(0.1, 0.3);
            double attackerGain = attacker * (r.Item1 - 1);
            double defenderLoss = defender * (1 - r.Item2);
            attackerGain += defenderLoss * boost;
            defenderLoss += defenderLoss * boost;
            r = (1 + (attackerGain / attacker), 1 - (defenderLoss / defender));
        }
        return r;
    }

    public override (double, double) ModifyDevour(double attacker, double defender, double diff = 1)
    {
        var r = base.ModifyDevour(attacker, defender, diff);
        if (r.Item1 > 1)
        {
            r = (r.Item1 * (1 + Extensions.Rng.NextDouble(0.1, 0.3)), r.Item2);
        }
        return r;
    }
}

/// <summary>土：吞噬增重、减伤 50%、渡劫提成功率；克水</summary>
public sealed class EarthAffix : PetAttribute
{
    public EarthAffix()
    {
        Element = Enums.Element.Earth;
        Name = "土";
        Description =
        [
            "◆成功吞噬后大量提升体重",
            "◆被攻击时减少巨量伤害",
            "◆渡劫时大幅提高成功概率",
            "◇对「水」属性的对手有额外攻击加成",
        ];
    }

    public override (double, double) ModifyDevour(double attacker, double defender, double diff = 1)
    {
        var r = base.ModifyDevour(attacker, defender, diff);
        if (r.Item1 > 1)
        {
            r = (r.Item1 * (1 + Extensions.Rng.NextDouble(0.1, 0.3)), r.Item2);
        }
        return r;
    }

    public override (double, double) ModifyBeingAttacked(double attacker, double defender, (double, double) baseMultiplier)
    {
        if (baseMultiplier.Item2 >= 1)
        {
            return baseMultiplier;
        }
        double loss = defender * (1 - baseMultiplier.Item2) * 0.5;
        return (baseMultiplier.Item1, 1 - (loss / defender));
    }

    public override double ModifyAscendSuccessRate(double rate) => rate * 1.3;
}

/// <summary>风：追击、攻/噬巨量增重、渡劫大幅提升；克土火，弱于水金木</summary>
public sealed class WindAffix : PetAttribute
{
    public WindAffix()
    {
        Element = Enums.Element.Wind;
        Name = "风";
        Description =
        [
            "◆攻击时有大概率额外追击一次",
            "◆成功攻击或吞噬后巨量提升体重",
            "◆渡劫时巨幅度提升成功概率",
            "◇对「土火」属性的对手有额外攻击加成",
            "◇对「水金木」属性的攻击额外防御加成",
        ];
    }

    public override (double, double) ModifyAttack(double attacker, double defender, (double, double) baseMultiplier, double diff = 1)
    {
        // 30% 概率追击（额外 100% 伤害），并额外获得对方损失的 30~50%
        var r = base.ModifyAttack(attacker, defender, baseMultiplier, diff);
        if (r.Item1 <= 1)
        {
            return r;
        }
        double chase = Extensions.Rng.NextDouble() < 0.3 ? 1 : 0;
        double attackerGain = attacker * (r.Item1 - 1);
        double defenderLoss = defender * (1 - r.Item2);
        attackerGain += defenderLoss * chase;
        defenderLoss += defenderLoss * chase;
        attackerGain += defenderLoss * Extensions.Rng.NextDouble(0.3, 0.5);
        return (1 + (attackerGain / attacker), 1 - (defenderLoss / defender));
    }

    public override (double, double) ModifyDevour(double attacker, double defender, double diff = 1)
    {
        var r = base.ModifyDevour(attacker, defender, diff);
        if (r.Item1 > 0)
        {
            r = (r.Item1 * (1 + Extensions.Rng.NextDouble(0.3, 0.5)), r.Item2);
        }
        return r;
    }

    public override double ModifyAscendSuccessRate(double rate) => rate * 1.5;
}

/// <summary>雷：额外伤害、渡劫翻倍、小概率反伤大量；克水金木，弱于土火</summary>
public sealed class ThunderAffix : PetAttribute
{
    public ThunderAffix()
    {
        Element = Enums.Element.Thunder;
        Name = "雷";
        Description =
        [
            "◆攻击时有大概率造成额外少量伤害",
            "◆渡劫时超巨幅提升成功概率",
            "◆被攻击时小概率反弹大量伤害",
            "◇对「水金木」属性的对手有额外攻击加成",
            "◇对「土火」属性的攻击额外防御加成",
        ];
    }

    public override (double, double) ModifyAttack(double attacker, double defender, (double, double) baseMultiplier, double diff = 1)
    {
        // 50% 概率提高 10~30% 伤害
        var r = base.ModifyAttack(attacker, defender, baseMultiplier, diff);
        if (r.Item1 > 1 && Extensions.Rng.NextDouble() < 0.5)
        {
            var boost = Extensions.Rng.NextDouble(0.1, 0.3);
            double attackerGain = attacker * (r.Item1 - 1);
            double defenderLoss = defender * (1 - r.Item2);
            attackerGain += defenderLoss * boost;
            defenderLoss += defenderLoss * boost;
            r = (1 + (attackerGain / attacker), 1 - (defenderLoss / defender));
        }
        return r;
    }

    public override (double, double) ModifyBeingAttacked(double attacker, double defender, (double, double) baseMultiplier)
    {
        // 10% 概率反弹 30~50% 所受伤害
        if (Extensions.Rng.NextDouble() > 0.1)
        {
            return baseMultiplier;
        }
        var reflect = Extensions.Rng.NextDouble(0.3, 0.5);
        double attackerGain = attacker * (baseMultiplier.Item1 - 1);
        double defenderLoss = defender * (1 - baseMultiplier.Item2);
        attackerGain -= defenderLoss * reflect;
        return (1 + (attackerGain / attacker), baseMultiplier.Item2);
    }

    public override double ModifyAscendSuccessRate(double rate) => rate * 2;
}

/// <summary>阴：小概率巨额伤害、吞噬增重、45% 逃脱、渡劫减半；对阳超额克制</summary>
public sealed class YinAffix : PetAttribute
{
    public YinAffix()
    {
        Element = Enums.Element.Yin;
        Name = "阴";
        Description =
        [
            "◆攻击时有小概率造成巨量伤害",
            "◆成功吞噬后额外增加体重",
            "◆被攻击或吞噬时有几率逃脱",
            "◆渡劫时巨幅度降低成功概率",
            "◇对「阳」属性的对手有超额的攻击",
            "◇对「金木水火土风」属性的对手有大量攻击加成",
            "◇对「金木水火土风」属性的攻击有大量防御加成",
        ];
    }

    public override (double, double) ModifyAttack(double attacker, double defender, (double, double) baseMultiplier, double diff = 1)
    {
        // 先获得 130~150% 额外增量，再 10% 概率追加 200%
        var r = base.ModifyAttack(attacker, defender, baseMultiplier, diff);
        if (r.Item1 <= 1)
        {
            return r;
        }
        double attackerGain = attacker * (r.Item1 - 1);
        double defenderLoss = defender * (1 - r.Item2);
        attackerGain += attackerGain * (1 + Extensions.Rng.NextDouble(0.3, 0.5));
        if (Extensions.Rng.NextDouble() < 0.1)
        {
            attackerGain += defenderLoss * 2;
            defenderLoss += defenderLoss * 2;
        }
        return (1 + (attackerGain / attacker), 1 - (defenderLoss / defender));
    }

    public override (double, double) ModifyDevour(double attacker, double defender, double diff = 1)
    {
        var r = base.ModifyDevour(attacker, defender, diff);
        if (r.Item1 > 1)
        {
            r = (r.Item1 * (1 + Extensions.Rng.NextDouble(0.3, 0.5)), r.Item2);
        }
        return r;
    }

    public override (double, double) ModifyBeingAttacked(double attacker, double defender, (double, double) baseMultiplier) =>
        Extensions.Rng.NextDouble() < 0.45 ? (1, 1) : baseMultiplier;

    public override (double, double) ModifyBeingDevoured(double devoured, double devourer, (double, double) baseDevour) =>
        Extensions.Rng.NextDouble() < 0.45 ? (1, 1) : baseDevour;

    public override double ModifyAscendSuccessRate(double rate) => rate * 0.5;
}

/// <summary>阳：临时增重 30% 攻噬、减伤、渡劫成功增重 50%；对阴超额克制</summary>
public sealed class YangAffix : PetAttribute
{
    public YangAffix()
    {
        Element = Enums.Element.Yang;
        Name = "阳";
        Description =
        [
            "◆被攻击时有大概率大幅度降低受到伤害",
            "◆攻击或吞噬时提升较大成功概率",
            "◆渡劫成功时巨幅度提升获得的体重",
            "◆渡劫时巨幅度降低失败概率",
            "◇对「阴」属性的对手有超额的攻击",
            "◇对「金木水火土风」属性的对手有大量攻击加成",
            "◇对「金木水火土风」属性的攻击有大量防御加成",
        ];
    }

    public override (double, double) ModifyAttack(double attacker, double defender, (double, double) baseMultiplier, double diff = 1) =>
        base.ModifyAttack(attacker * 1.3, defender, baseMultiplier, diff);

    public override (double, double) ModifyDevour(double attacker, double defender, double diff = 1) =>
        base.ModifyDevour(attacker * 1.3, defender, diff);

    public override (double, double) ModifyBeingAttacked(double attacker, double defender, (double, double) baseMultiplier)
    {
        // 30% 概率减免 50% 伤害
        if (Extensions.Rng.NextDouble() > 0.3 || baseMultiplier.Item2 >= 1)
        {
            return baseMultiplier;
        }
        double loss = defender * (1 - baseMultiplier.Item2) * 0.5;
        return (baseMultiplier.Item1, 1 - (loss / defender));
    }

    public override double ModifyAscendWeight(double successRate, double multiplier = 1)
    {
        var result = base.ModifyAscendWeight(successRate, multiplier);
        return result > 1 ? result * 1.5 : result;
    }

    public override double ModifyAscendSuccessRate(double rate) => rate * 1.5;
}