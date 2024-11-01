using me.cqp.luohuaming.iKun.Sdk.Cqp.Model;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace me.cqp.luohuaming.iKun.PublicInfos
{
    public static class CommonHelper
    {
        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        public static long GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds);
        }

        public static string GetAppImageDirectory()
        {
            var ImageDirectory = Path.Combine(Environment.CurrentDirectory, "data", "image\\");
            return ImageDirectory;
        }

        public static Random Random { get; set; } = new Random();

        public static string[] Units { get; set; } = ["万", "亿", "兆", "京", "垓"];

        public static string ParseLongNumber(int num)
        {
            string numStr = num.ToString();
            int step = 1;
            for (int i = numStr.Length - 1; i > 0; i--)
            {
                if (step % 3 == 0)
                {
                    numStr = numStr.Insert(i, ",");
                }
                step++;
            }
            return numStr;
        }

        public static string GetFileNameFromURL(this string url)
        {
            return url.Split('/').Last().Split('?').First();
        }

        public static string ParseNum2Chinese(this int num)
        {
            return num > 10000 ? $"{num / 10000.0:f1}万" : num.ToString();
        }

        public static bool CompareNumString(string a, string b)
        {
            if (a.Length != b.Length)
            {
                return a.Length > b.Length;
            }

            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                {
                    return a[i] > b[i];
                }
            }
            return false;
        }

        public static void RemoveNewLine(this StringBuilder stringBuilder)
        {
            if (stringBuilder.Length < Environment.NewLine.Length)
            {
                return;
            }
            stringBuilder.Remove(stringBuilder.Length - Environment.NewLine.Length, Environment.NewLine.Length);
        }

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
        /// <returns></returns>
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
            value /= AppConfig.WeightUnitBase;
            if (AppConfig.ShortNumberType == Enums.ShortNumberType.Normal)
            {
                return value.ToString("f2");
            }
            else if (AppConfig.ShortNumberType == Enums.ShortNumberType.Science)
            {
                return value <= 1000000 ? value.ToString("f2") : value.ToString("E2");
            }
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
                return value.ToString("f2");
            }
            else if (index < Units.Length)
            {
                return value.ToString("f2") + Units[index];
            }
            else
            {
                return value.ToString("f2") + Units.Last();
            }
        }

        public static bool CheckSameGroup(long target, long fromGroup)
        {
            if (!MainSave.GroupMemberInfos.TryGetValue(fromGroup, out var infos))
            {
                infos = MainSave.CQApi.GetGroupMemberList(fromGroup);
                MainSave.GroupMemberInfos[fromGroup] = infos;
            }
            return infos.Any(x => x.QQ == target);
        }
    }
}