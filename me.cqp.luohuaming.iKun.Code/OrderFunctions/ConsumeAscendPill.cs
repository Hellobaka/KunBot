using me.cqp.luohuaming.iKun.PublicInfos;
using me.cqp.luohuaming.iKun.PublicInfos.Models;
using me.cqp.luohuaming.iKun.Sdk.Cqp.EventArgs;
using System;
using System.Text;

namespace me.cqp.luohuaming.iKun.Code.OrderFunctions
{
    public class ConsumeAscendPill : IOrderModel
    {
        public bool ImplementFlag { get; set; } = true;

        public string GetOrderStr() => AppConfig.CommandConsumeAscendPill;

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

            var player = Player.GetPlayer(e.FromQQ);
            if (player == null)
            {
                sendText.MsgToSend.Add(AppConfig.ReplyNoPlayer);
                return result;
            }

            var param = e.Message.Text.Substring(GetOrderStr().Length).Trim();
            if (!int.TryParse(param, out int count))
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyParamInvalid, $"，示例：{GetOrderStr()} 数量"));
                return result;
            }
            count = Math.Max(1, count);
            count = Math.Min(AppConfig.ValueAscendPillMaxConsumeCount, count);
            int currentPill = InventoryItem.GetItemCount(player, PublicInfos.Enums.Items.AscendPill);
            if (currentPill < count)
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyItemLeak, Items.AscendPill().Name, count, currentPill));
                return result;
            }
            player.AscendPillComsume = count;
            player.Update();

            sendText.MsgToSend.Add(string.Format(AppConfig.ReplyConsumeAscendPill, count, count * AppConfig.ValueAscendPillPerIncrement));
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