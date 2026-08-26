using me.cqp.luohuaming.iKun.Domain.Configuration;

using me.cqp.luohuaming.iKun.Infrastructure.Logging;

namespace me.cqp.luohuaming.iKun.Domain.Models;

/// <summary>
/// 挂机/打工纯数值计算（无状态）。
/// </summary>
public static class IdleMath
{
    private static readonly Log Log = Log.For("挂机管理");

    /// <summary>每小时经验收益：随星级指数增长</summary>
    public static double ExperienceGainPerHour(int level) => level switch
    {
        <= 0 => 0,
        1 => 10,
        2 => 100,
        3 => 1000,
        4 => 7000,
        5 => 30000,
        >= 6 and < 8 => Math.Pow(10, level - 1) / 10,
        _ => Math.Pow(10, level - 1) / 20,
    };

    /// <summary>时间段内经验总收益</summary>
    public static double TotalExperience(int level, DateTime start, DateTime end)
    {
        var gain = ExperienceGainPerHour(level) * (end - start).TotalHours;
        Log.Info($"星级={level}，挂机经验速度={ExperienceGainPerHour(level)}");
        return gain;
    }

    /// <summary>时间段内金币总收益：基础时薪 × 等级加成</summary>
    public static double TotalCoins(int level, DateTime start, DateTime end)
    {
        var config = CoreConfiguration.Current;
        var gain = (end - start).TotalHours * config.WorkCoinPerHour;
        gain += gain * level * (config.WorkLevelBonusPercent / 100.0);
        Log.Info($"星级={level}，打工金币={gain}");
        return gain;
    }
}