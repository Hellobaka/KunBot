using me.cqp.luohuaming.iKun.PublicInfos;
using me.cqp.luohuaming.iKun.PublicInfos.Models;
using me.cqp.luohuaming.iKun.Sdk.Cqp.EventArgs;
using System;
using System.Linq;
using System.Text;

namespace me.cqp.luohuaming.iKun.Code.OrderFunctions
{
    public class RankingGroup : IOrderModel
    {
        public bool ImplementFlag { get; set; } = true;

        public string GetOrderStr() => AppConfig.CommandRankingGroup;

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
            stringBuilder.AppendLine(AppConfig.ReplyRankingGroupHeader);
            var memberList = e.FromGroup.GetGroupMemberList();
            if (memberList == null || memberList.Count == 0)
            {
                sendText.MsgToSend.Add("获取群成员列表失败");
                return result;
            }
            var records = Record.GetRecordsByQQList(memberList.Select(x => x.QQ.Id).ToList());
            var kuns = Kun.GetKunByRecords(records).OrderByDescending(x => x.Weight).ToList();
            for (int i = 0; i < Math.Min(AppConfig.ValueRankingCount, kuns.Count); i++)
            {
                kuns[i].Initialize();
                bool autoPlaying = AutoPlay.CheckKunAutoPlay(kuns[i]);
                bool working = AutoPlay.CheckKunAutoPlay(kuns[i], PublicInfos.Enums.AutoPlayType.Coin);
                try
                {
                    var info = e.FromGroup.GetGroupMemberInfo(kuns[i].PlayerID);
                    string name = string.IsNullOrWhiteSpace(info.Card) ? info.Nick : info.Card;
                    stringBuilder.AppendLine($"{i + 1}. [{name}] {kuns[i]} {kuns[i].Weight.ToShortNumber()}" +
                        $" {AppConfig.WeightUnit}" +
                        $"{(autoPlaying ? $" {AppConfig.ReplyRankingAutoPlaying}" : "")}" +
                        $"{(working ? $" {AppConfig.ReplyRankingWorking}" : "")}");
                }
                catch (Exception exc)
                {
                    e.CQLog.Info("获取成员名片", $"获取失败，群={e.FromGroup}，QQ={kuns[i].PlayerID}\n{exc.Message}，{exc.StackTrace}");
                    continue;
                }
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