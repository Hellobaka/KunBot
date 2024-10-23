using me.cqp.luohuaming.iKun.PublicInfos;
using me.cqp.luohuaming.iKun.PublicInfos.Models;
using me.cqp.luohuaming.iKun.Sdk.Cqp.EventArgs;
using System;
using System.Linq;
using System.Text;

namespace me.cqp.luohuaming.iKun.Code.OrderFunctions
{
    public class RandomPunish : IOrderModel
    {
        public bool ImplementFlag { get; set; } = true;

        public string GetOrderStr() => AppConfig.CommandRandomPunish;

        public bool Judge(string destStr) => destStr.Replace("＃", "#").StartsWith(GetOrderStr());

        public FunctionResult Execute(CQGroupMessageEventArgs e)
        {
            FunctionResult result = new FunctionResult
            {
                Result = true,
                SendFlag = true,
            };
            SendText sendText = new SendText
            {
                SendID = e.FromGroup,
            };
            result.SendObject.Add(sendText);
            DayOfWeek dayOfWeek = AppConfig.ValueRandomPunishExecuteDay == 7 ? DayOfWeek.Sunday
                                 : (DayOfWeek)AppConfig.ValueRandomPunishExecuteDay;
            string dayOfWeekString = dayOfWeek switch
            {
                DayOfWeek.Sunday => "周日",
                DayOfWeek.Monday => "周一",
                DayOfWeek.Tuesday => "周二",
                DayOfWeek.Wednesday => "周三",
                DayOfWeek.Thursday => "周四",
                DayOfWeek.Friday => "周五",
                DayOfWeek.Saturday => "周六",
            };
            sendText.MsgToSend.Add(string.Format(AppConfig.ReplyRandomPunish, dayOfWeekString, PublicInfos.Models.RandomPunish.Instance.TargetTime.ToString("G")));
            return result;
        }

        public FunctionResult Execute(CQPrivateMessageEventArgs e)
        {
            FunctionResult result = new FunctionResult
            {
                Result = false,
                SendFlag = false,
            };
            return result;
        }
    }
}