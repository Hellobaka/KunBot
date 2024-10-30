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
            var kuns = Kun.GetRankingKun(AppConfig.ValueRankingCount);
            for (int i = 0; i < kuns.Count; i++)
            {
                kuns[i].Initialize();
                bool autoPlaying = AutoPlay.CheckKunAutoPlay(kuns[i], PublicInfos.Enums.AutoPlayType.Exp);
                bool working = AutoPlay.CheckKunAutoPlay(kuns[i], PublicInfos.Enums.AutoPlayType.Coin);
                var record = Record.GetRecordByKunID(kuns[i].Id);
                if (record == null)
                {
                    continue;
                }
                try
                {
                    var info = e.CQApi.GetGroupMemberInfo(record.Group, record.QQ);
                    string name = string.IsNullOrWhiteSpace(info.Card) ? info.Nick : info.Card;
                    stringBuilder.AppendLine($"{i + 1}. [{name}] {kuns[i]} {kuns[i].Weight.ToShortNumber()}" +
                        $" {AppConfig.WeightUnit}" +
                        $"{(autoPlaying ? $" {AppConfig.ReplyRankingAutoPlaying}" : "")}" +
                        $"{(working ? $" {AppConfig.ReplyRankingWorking}" : "")}");
                }
                catch (Exception exc)
                {
                    e.CQLog.Info("获取成员名片", $"获取失败，群={record.Group}，QQ={record.QQ}\n{exc.Message}，{exc.StackTrace}");
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