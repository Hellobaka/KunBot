using Another_Mirai_Native.Abstractions.Models;
using me.cqp.luohuaming.iKun.PublicInfos.Enums;
using System.Text;

namespace me.cqp.luohuaming.iKun.PublicInfos
{
    public static class CommonHelper
    {
        /// <summary>
        /// 获取时间戳
        /// </summary>
        public static long GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds);
        }

        public static Random Random { get; set; } = new();

        public static string[] Units { get; set; } = ["万", "亿", "兆", "京", "垓", "秭", "穰", "沟", "涧", "正", "载", "极"];

        public static bool IsSameDay(DateTime dateTime1, DateTime dateTime2)
        {
            return dateTime1.Year == dateTime2.Year && dateTime1.Month == dateTime2.Month && dateTime1.Day == dateTime2.Day;
        }

        /// <summary>
        /// 随机范围小数
        /// </summary>
        /// <param name="random"></param>
        /// <param name="lower">0.x</param>
        /// <param name="upper">0.x</param>
        public static double NextDouble(this Random random, double lower, double upper)
        {
            return random.NextDouble() * (upper - lower) + lower;
        }

        public static (double, double) Multiple(this (double, double) item1, (double, double) item2)
        {
            return (item1.Item1 * item2.Item1, item1.Item2 * item2.Item2);
        }

        public static string ToShortNumber(this double value)
        {
            bool nagivate = value < 0;
            value /= AppConfig.WeightUnitBase;
            if (AppConfig.ShortNumberType == ShortNumberType.Normal)
            {
                return value.ToString("f2");
            }
            else if (AppConfig.ShortNumberType == ShortNumberType.Science)
            {
                return value <= 1000000 ? value.ToString("f2") : value.ToString("E2");
            }
            value = Math.Abs(value);
            int index = -1;
            while (value > 10000 && index < Units.Length)
            {
                value /= 10000;
                if (value > 1)
                {
                    index++;
                }
            }
            if (index < 0)
            {
                return (nagivate ? "-" : "") + value.ToString("f2");
            }
            else if (index < Units.Length)
            {
                return (nagivate ? "-" : "") + value.ToString("f2") + Units[index];
            }
            else
            {
                return (nagivate ? "-" : "") + value.ToString("f2") + Units.Last();
            }
        }

        /// <summary>
        /// 判断目标 QQ 是否在指定群内（带缓存）
        /// </summary>
        public static bool CheckSameGroup(Another_Mirai_Native.Abstractions.Services.IPluginApi api, long target, long fromGroup)
        {
            if (!MainSave.GroupMemberInfos.TryGetValue(fromGroup, out var infos) || infos is null || infos.Count == 0)
            {
                infos = api.GroupApi.GetGroupMembers(fromGroup);
                MainSave.GroupMemberInfos[fromGroup] = infos;
            }
            return infos.Any(x => x.QQ == target);
        }

        public static void RemoveNewLine(this StringBuilder stringBuilder)
        {
            if (stringBuilder.Length < Environment.NewLine.Length)
            {
                return;
            }
            stringBuilder.Remove(stringBuilder.Length - Environment.NewLine.Length, Environment.NewLine.Length);
        }

        public static int GetMaxItemValue() => Enum.GetValues(typeof(Enums.Items))
                           .Cast<Enums.Items>()
                           .Select(x => (int)x)
                           .Max();

        /// <summary>
        /// 构造 At 消息 CQ 码文本，用于拼接在纯文本消息中
        /// </summary>
        public static string CQCode_At(long qq) => $"[CQ:at,qq={qq}]";

        /// <summary>
        /// 获取群成员展示名：优先群名片，其次昵称，最后 QQ 号
        /// </summary>
        public static string GetMemberDisplayName(GroupMemberInfo info)
        {
            if (info is null)
            {
                return "";
            }
            return string.IsNullOrWhiteSpace(info.Card) ? (string.IsNullOrWhiteSpace(info.Nick) ? info.QQ.ToString() : info.Nick) : info.Card;
        }
    }
}
