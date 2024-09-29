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
            int consume = count;
            if (!InventoryItem.TryRemoveItem(player, PublicInfos.Enums.Items.KunEgg, consume, out int currentCount))
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyItemLeak, Items.KunEgg().Name, consume, currentCount));
                return result;
            }
            player.GiveItem([Items.KunEgg(consume * AppConfig.ValueKunEggToCoinRate)]);

            sendText.MsgToSend.Add(string.Format(AppConfig.ReplyOpenKunEgg, consume, consume * AppConfig.ValueKunEggToCoinRate));
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