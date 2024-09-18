using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using me.cqp.luohuaming.iKun.Sdk.Cqp.Model;

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
            stringBuilder.Remove(stringBuilder.Length - Environment.NewLine.Length, Environment.NewLine.Length);
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
            return random.Next((int)(lower * 100000), (int)(upper * 100000)) / 100000.0;
        }

        public static (double, double) Multiple(this (double, double) item1, (double, double) item2)
        {
            return (item1.Item1 * item2.Item1, item1.Item2 * item2.Item2);
        }
    }
}
