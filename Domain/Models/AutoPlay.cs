using SqlSugar;

namespace me.cqp.luohuaming.iKun.Domain.Models;

/// <summary>
/// 挂机/打工记录（持久化实体）。任务调度与运行时状态在 Background.IdleScheduler。
/// </summary>
[SugarTable]
public sealed class AutoPlay
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int ID { get; set; }

    public int KunID { get; set; }

    public long GroupId { get; set; }

    /// <summary>时长（小时）</summary>
    public double Duration { get; set; } = 8;

    public DateTime StartTime { get; set; } = DateTime.Now;

    public DateTime EndTime { get; set; }

    /// <summary>数据库侧的运行标记</summary>
    public bool Running { get; set; }

    /// <summary>Exp=挂机(经验/体重)，Coin=打工(金币)</summary>
    public Enums.AutoPlayType AutoPlayType { get; set; } = Enums.AutoPlayType.Exp;

    [SugarColumn(IsIgnore = true)]
    public Domain.Enums.IdleType IdleType =>
        AutoPlayType == Enums.AutoPlayType.Coin ? Domain.Enums.IdleType.Coin : Domain.Enums.IdleType.Experience;
}
