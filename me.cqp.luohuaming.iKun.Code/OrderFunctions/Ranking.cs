using me.cqp.luohuaming.iKun.PublicInfos;
using me.cqp.luohuaming.iKun.PublicInfos.Models;
using me.cqp.luohuaming.iKun.Sdk.Cqp.EventArgs;
using System;
using System.Linq;
using System.Text;

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
            stringBuilder.AppendLine(AppConfig.ReplyRankingHeader);
            var records = Record.GetRecordsByGroupID(e.FromGroup);
            var kuns = Kun.GetKunByRecords(records).OrderByDescending(x => x.Weight).ToList();
            for (int i = 0; i < Math.Min(AppConfig.ValueRankingCount, kuns.Count); i++)
            {
                kuns[i].Initialize();
                var info = e.FromGroup.GetGroupMemberInfo(kuns[i].PlayerID);
                string name = string.IsNullOrWhiteSpace(info.Card) ? info.Nick : info.Card;
                stringBuilder.AppendLine($"{i + 1}. [{name}] {kuns[i]} {kuns[i].Weight.ToShortNumber()} {AppConfig.WeightUnit}");
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