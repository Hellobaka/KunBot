using me.cqp.luohuaming.iKun.PublicInfos;
using me.cqp.luohuaming.iKun.PublicInfos.Models;
using me.cqp.luohuaming.iKun.Sdk.Cqp.EventArgs;
using System;

namespace me.cqp.luohuaming.iKun.Code.OrderFunctions
{
    public class BuyEgg : IOrderModel
    {
        public bool ImplementFlag { get; set; } = true;

        public string GetOrderStr() => AppConfig.CommandBuyEgg;

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
            if (!int.TryParse(param, out int count))
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyParamInvalid, $"，示例：{GetOrderStr()} 数量"));
                return result;
            }
            count = Math.Max(1, count);

            var player = Player.GetPlayer(e.FromQQ);
            if (player == null)
            {
                sendText.MsgToSend.Add(AppConfig.ReplyNoPlayer);
                return result;
            }

            int eggComsume = count * AppConfig.ValueEggValue;
            if (!InventoryItem.TryRemoveItem(player, Items.Coin().ID, eggComsume, out int currentCount))
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyItemLeak, Items.Coin().Name, eggComsume, currentCount));
                return result;
            }
            player.GiveItem([
                Items.KunEgg(count)
            ]);
            sendText.MsgToSend.Add(string.Format(AppConfig.ReplyBuyEgg, eggComsume, count, currentCount, InventoryItem.GetItemCount(player, Items.KunEgg().ID)));
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