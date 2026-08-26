using me.cqp.luohuaming.iKun.Domain.Configuration;
using System.Text;

namespace me.cqp.luohuaming.iKun.Infrastructure;

/// <summary>
/// 通用工具扩展：随机数、数值缩写、StringBuilder 等。
/// </summary>
public static class Extensions
{
    /// <summary>全局随机源</summary>
    public static Random Rng { get; } = new();

    /// <summary>[lower, upper) 范围小数（0.x 形式）</summary>
    public static double NextDouble(this Random random, double lower, double upper) =>
        (random.NextDouble() * (upper - lower)) + lower;

    public static (double, double) Multiply(this (double, double) left, (double, double) right) =>
        (left.Item1 * right.Item1, left.Item2 * right.Item2);

    /// <summary>按权重单位与缩写风格格式化体重数值</summary>
    public static string ToShortNumber(this double value)
    {
        bool negative = value < 0;
        value /= CoreConfiguration.Current.WeightUnitBase;
        var style = CoreConfiguration.Current.ShortNumberStyle;
        switch (style)
        {
            case ShortNumberStyle.Normal:
                return value.ToString("f2");

            case ShortNumberStyle.Science:
                return value <= 1_000_000 ? value.ToString("f2") : value.ToString("E2");
        }

        string[] units = ["万", "亿", "兆", "京", "垓", "秭", "穰", "沟", "涧", "正", "载", "极"];
        value = Math.Abs(value);
        int index = -1;
        while (value > 10000 && index < units.Length)
        {
            value /= 10000;
            if (value > 1)
            {
                index++;
            }
        }
        string prefix = negative ? "-" : "";
        return index switch
        {
            < 0 => prefix + value.ToString("f2"),
            < 12 => prefix + value.ToString("f2") + units[index],
            _ => prefix + value.ToString("f2") + units[^1],
        };
    }

    /// <summary>移除末尾换行</summary>
    public static void RemoveTrailingNewLine(this StringBuilder builder)
    {
        if (builder.Length >= Environment.NewLine.Length &&
            builder.ToString()[^Environment.NewLine.Length..] == Environment.NewLine)
        {
            builder.Remove(builder.Length - Environment.NewLine.Length, Environment.NewLine.Length);
        }
    }

    /// <summary>两个时间是否同一天</summary>
    public static bool IsSameDay(DateTime left, DateTime right) =>
        left.Year == right.Year && left.Month == right.Month && left.Day == right.Day;
}