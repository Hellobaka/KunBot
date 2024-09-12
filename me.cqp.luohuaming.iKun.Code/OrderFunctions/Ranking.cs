using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using me.cqp.luohuaming.iKun.Sdk.Cqp.EventArgs;
using me.cqp.luohuaming.iKun.PublicInfos;
using me.cqp.luohuaming.iKun.PublicInfos.Models;

namespace me.cqp.luohuaming.iKun.Code.OrderFunctions
{
    public class Ranking : IOrderModel
    {
        public bool ImplementFlag { get; set; } = true;

        public string GetOrderStr() => AppConfig.CommandRanking;

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
            StringBuilder stringBuilder = new StringBuilder();
            var records = Record.GetRecordsByGroupID(e.FromGroup);
            var kuns = Kun.GetKunByRecords(records).OrderByDescending(x => x.Weight).ToList();
            for (int i = 0; i < Math.Min(AppConfig.ValueRankingCount, kuns.Count); i++)
            {
                string name = e.FromGroup.GetGroupMemberInfo(e.FromQQ).Card;
                stringBuilder.AppendLine($"{i + 1}. [{name}] {kuns[i]} {kuns[i].Weight:f2} 千克");
            }
            stringBuilder.RemoveNewLine();

            sendText.MsgToSend.Add(stringBuilder.ToString());
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
