using me.cqp.luohuaming.iKun.PublicInfos;
using me.cqp.luohuaming.iKun.PublicInfos.Models;
using me.cqp.luohuaming.iKun.Sdk.Cqp.EventArgs;
using System;

namespace me.cqp.luohuaming.iKun.Code.OrderFunctions
{
    public class Resurrect : IOrderModel
    {
        public bool ImplementFlag { get; set; } = true;

        public string GetOrderStr() => AppConfig.CommandResurrect;

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
            var param = e.Message.Text.Substring(GetOrderStr().Length).Trim();
            if (!int.TryParse(param, out int id))
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyParamInvalid, $"，示例：{GetOrderStr()} ID"));
                return result;
            }
            var player = Player.GetPlayer(e.FromQQ);
            if (player == null)
            {
                sendText.MsgToSend.Add(AppConfig.ReplyNoPlayer);
                return result;
            }
            var kun = Kun.GetKunByQQ(player.QQ);
            if (kun != null)
            {
                sendText.MsgToSend.Add(AppConfig.ReplyDuplicateResurrect);
                return result;
            }

            kun = Kun.GetKunByID(id);
            if (kun == null)
            {
                sendText.MsgToSend.Add(AppConfig.ReplyNoTargrtKun);
                return result;
            }
            if (kun.PlayerID != player.QQ)
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyKunOwnerNotMatch, AppConfig.ReplyResurrectFailed));
                return result;
            }
            if (kun.Abandoned)
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyKunAbandoned, AppConfig.ReplyResurrectFailed));
                return result;
            }
            if (kun.Alive)
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyKunAlive, AppConfig.ReplyResurrectFailed));
                return result;
            }
            if (kun.Weight <= 0)
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyKunWeightZero, AppConfig.ReplyResurrectFailed));
                return result;
            }
            if ((DateTime.Now - kun.DeadAt).TotalHours >= AppConfig.ValueMaxResurrectHour)
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyResurrectHourLimit, AppConfig.ValueMaxResurrectHour, (int)(DateTime.Now - kun.DeadAt).TotalHours));
                return result;
            }
            int consume = kun.ResurrectCount + 1;
            if (!InventoryItem.TryRemoveItem(player, PublicInfos.Enums.Items.ResurrectPill, consume, out int currentCount))
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyItemLeak, Items.ResurrectPill().Name, consume, currentCount));
                return result;
            }

            kun.Initialize();
            var r = kun.Resurrect();
            if (r.Success)
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyResurrectSuccess, kun.DeadAt.ToString("G"), r.CurrentResurrectCount, r.WeightLoss.ToShortNumber(), r.LevelLoss, consume, currentCount));
            }
            else
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyResurrectFail, consume, currentCount));
            }
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