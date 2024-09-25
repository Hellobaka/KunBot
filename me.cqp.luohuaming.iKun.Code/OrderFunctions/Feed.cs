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
            var param = e.Message.Text.Replace(GetOrderStr(), "").Trim();
            if (!int.TryParse(param, out int count))
            {
                count = 1;
            }

            if (!InventoryItem.TryRemoveItem(player, Items.Coin().ID, count * AppConfig.ValueFeedCoinConsume, out int currentCoin))
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyItemLeak, Items.Coin().Name, count * AppConfig.ValueFeedCoinConsume, currentCoin));
                return result;
            }
            currentCoin -= count * AppConfig.ValueFeedCoinConsume;
            if (!InventoryItem.TryRemoveItem(player, Items.KunEgg().ID, count * AppConfig.ValueFeedKunEggConsume, out int currentKunEgg))
            {
                // 消耗了金币，进行补偿
                player.GiveItem([
                    Items.Coin(count * AppConfig.ValueFeedCoinConsume)
                ]);

                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyItemLeak, Items.KunEgg().Name, count * AppConfig.ValueFeedKunEggConsume, currentKunEgg));
                return result;
            }
            currentKunEgg -= count * AppConfig.ValueFeedKunEggConsume;

            double currentWeight = kun.Weight;
            for(int i = 0; i < count; i++)
            {
                double increase = CommonHelper.Random.Next(AppConfig.ValueFeedWeightMinimumIncrement, AppConfig.ValueFeedWeightMaximumIncrement) * 1.0;
                currentWeight *= (1 + increase / 100);
            }
            StringBuilder stringBuilder = new();
            stringBuilder.AppendLine($"你的「{kun}」体重增加了 {(currentWeight - kun.Weight):f2} 千克");
            stringBuilder.AppendLine($"现体重为 {kun.Weight:f2} 千克");
            stringBuilder.AppendLine("-------------------");
            stringBuilder.AppendLine($"剩余 {currentCoin} 枚金币，{currentKunEgg} 枚鲲蛋");
            stringBuilder.RemoveNewLine();

            kun.Weight = currentWeight;
            kun.Update();
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
