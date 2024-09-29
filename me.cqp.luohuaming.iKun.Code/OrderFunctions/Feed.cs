using me.cqp.luohuaming.iKun.PublicInfos;
using me.cqp.luohuaming.iKun.PublicInfos.Models;
using me.cqp.luohuaming.iKun.Sdk.Cqp.EventArgs;
using System.Text;

namespace me.cqp.luohuaming.iKun.Code.OrderFunctions
{
    public class Feed : IOrderModel
    {
        public bool ImplementFlag { get; set; } = true;

        public string GetOrderStr() => AppConfig.CommandFeed;

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
            var kun = Kun.GetKunByQQ(player.QQ);
            if (kun == null)
            {
                sendText.MsgToSend.Add(AppConfig.ReplyNoKun);
                return result;
            }
            kun.Initialize();

            var param = e.Message.Text.Replace(GetOrderStr(), "").Trim();
            if (!int.TryParse(param, out int count))
            {
                count = 1;
            }

            int currentCoin = InventoryItem.GetItemCount(player, PublicInfos.Enums.Items.Coin);
            int currentEgg = InventoryItem.GetItemCount(player, PublicInfos.Enums.Items.KunEgg);
            if (currentCoin < count * AppConfig.ValueFeedCoinConsume)
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyItemLeak, Items.Coin().Name, count * AppConfig.ValueFeedCoinConsume, currentCoin));
                return result;
            }

            if (currentEgg < count * AppConfig.ValueFeedKunEggConsume)
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyItemLeak, Items.KunEgg().Name, count * AppConfig.ValueFeedKunEggConsume, currentEgg));
                return result;
            }
            InventoryItem.TryRemoveItem(player, PublicInfos.Enums.Items.Coin, count * AppConfig.ValueFeedCoinConsume, out currentCoin);
            InventoryItem.TryRemoveItem(player, PublicInfos.Enums.Items.KunEgg, count * AppConfig.ValueFeedKunEggConsume, out currentEgg);

            var r = kun.Feed(count);
            StringBuilder stringBuilder = new();
            stringBuilder.AppendLine(string.Format(AppConfig.ReplyFeed, kun.ToString(), r.Increment.ToShortNumber(), r.CurrentWeight.ToShortNumber()));
            if (r.WeightLimit)
            {
                stringBuilder.AppendLine(AppConfig.ReplyWeightLimit);
            }
            stringBuilder.AppendLine("-------------------");
            stringBuilder.AppendLine($"剩余 {currentCoin} 枚金币，{currentEgg} 枚鲲蛋");
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