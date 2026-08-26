namespace me.cqp.luohuaming.iKun.Domain.Results;

/// <summary>渡劫结果</summary>
public sealed class AscendResult
{
    public bool Success { get; init; } = true;

    public double WeightDelta { get; init; }

    public double CurrentWeight { get; init; }

    public int CurrentLevel { get; init; }

    public bool Died { get; init; }
}

/// <summary>攻击结果</summary>
public sealed class AttackResult
{
    public bool Success { get; init; } = true;

    public double AttackerWeightDelta { get; init; }

    public double AttackerWeight { get; init; }

    public bool AttackerDied { get; init; }

    public bool HitWeightLimit { get; init; }

    public double DefenderWeightDelta { get; init; }

    public double DefenderWeight { get; init; }

    public bool DefenderDied { get; init; }

    public bool Escaped { get; init; }

    public override string ToString() =>
        $"执行成功={Success}，被逃脱={Escaped}，攻方增量={AttackerWeightDelta}，攻方体重={AttackerWeight}，攻方死亡={AttackerDied}，上限={HitWeightLimit}，" +
        $"被攻方减量={DefenderWeightDelta}，被攻方体重={DefenderWeight}，被攻方死亡={DefenderDied}";
}

/// <summary>吞噬结果</summary>
public sealed class DevourResult
{
    public bool Success { get; init; } = true;

    public double AttackerWeightDelta { get; init; }

    public double AttackerWeight { get; init; }

    public bool AttackerDied { get; init; }

    public bool HitWeightLimit { get; init; }

    public double DefenderWeightDelta { get; init; }

    public double DefenderWeight { get; init; }

    public bool DefenderDied { get; init; }

    public bool Escaped { get; init; }

    public override string ToString() =>
        $"执行成功={Success}，被逃脱={Escaped}，攻方增量={AttackerWeightDelta}，攻方体重={AttackerWeight}，攻方死亡={AttackerDied}，上限={HitWeightLimit}，" +
        $"被攻方增量={DefenderWeightDelta}，被攻方体重={DefenderWeight}，被攻方死亡={DefenderDied}";
}

/// <summary>喂养结果</summary>
public sealed class FeedResult
{
    public bool Success { get; init; } = true;

    public double CurrentWeight { get; init; }

    public double WeightDelta { get; init; }

    public bool HitWeightLimit { get; init; }
}

/// <summary>复活结果</summary>
public sealed class ResurrectResult
{
    public bool Success { get; init; } = true;

    public int ResurrectCount { get; init; }

    public double WeightLoss { get; init; }

    public int LevelLoss { get; init; }

    public override string ToString() => $"复活次数={ResurrectCount}，体重丢失={WeightLoss}，星级丢失={LevelLoss}";
}

/// <summary>强化结果</summary>
public sealed class UpgradeResult
{
    public bool Success { get; init; } = true;

    public double CurrentWeight { get; init; }

    public double WeightDelta { get; init; }

    public bool HitWeightLimit { get; init; }
}