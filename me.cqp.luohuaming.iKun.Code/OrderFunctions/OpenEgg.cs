using me.cqp.luohuaming.iKun.PublicInfos;
using me.cqp.luohuaming.iKun.PublicInfos.Models;
using me.cqp.luohuaming.iKun.Sdk.Cqp.EventArgs;

namespace me.cqp.luohuaming.iKun.Code.OrderFunctions
{
    public class OpenEgg : IOrderModel
    {
        public bool ImplementFlag { get; set; } = true;

        public string GetOrderStr() => AppConfig.CommandOpenEgg;

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

            var param = e.Message.Text.Replace(GetOrderStr(), "").Trim();
            if (!int.TryParse(param, out int count))
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyParamInvalid, $"，示例：{GetOrderStr()} 数量"));
                return result;
            }
            var player = Player.GetPlayer(e.FromQQ);
            if (player == null)
            {
                sendText.MsgToSend.Add(AppConfig.ReplyNoPlayer);
                return result;
            }
            int currentCoin = InventoryItem.GetItemCount(player, PublicInfos.Enums.Items.Coin);
            int currentEgg = InventoryItem.GetItemCount(player, PublicInfos.Enums.Items.KunEgg);
            if (currentCoin < count * 10)
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyItemLeak, Items.Coin().Name, count * 10, currentCoin));
                return result;
            }

            if (currentEgg < count)
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyItemLeak, Items.KunEgg().Name, count, currentEgg));
                return result;
            }
            InventoryItem.TryRemoveItem(player, PublicInfos.Enums.Items.Coin, count * 10, out currentCoin);
            InventoryItem.TryRemoveItem(player, PublicInfos.Enums.Items.KunEgg, count, out currentEgg);
            player.GiveItem([Items.BlindBox(count * AppConfig.ValueKunEggToBlindBoxRate)]);

            sendText.MsgToSend.Add(string.Format(AppConfig.ReplyOpenKunEgg, count, count * AppConfig.ValueKunEggToBlindBoxRate));
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