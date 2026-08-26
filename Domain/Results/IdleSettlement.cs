namespace me.cqp.luohuaming.iKun.Domain.Results;

/// <summary>挂机/打工结算结果</summary>
public sealed class IdleSettlement
{
    public DateTime StartTime { get; init; }

    public DateTime EndTime { get; init; }

    public TimeSpan Duration => EndTime - StartTime;

    public int CoinsEarned { get; init; }

    public double CurrentWeight { get; init; }

    public double WeightGained { get; init; }

    public bool HitWeightLimit { get; init; }

    public bool Died { get; init; }
}