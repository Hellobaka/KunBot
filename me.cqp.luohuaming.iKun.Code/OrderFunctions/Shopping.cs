using me.cqp.luohuaming.iKun.PublicInfos;
using me.cqp.luohuaming.iKun.PublicInfos.Models;
using me.cqp.luohuaming.iKun.Sdk.Cqp.EventArgs;
using System;
using System.Collections.Generic;
using System.Text;

namespace me.cqp.luohuaming.iKun.Code.OrderFunctions
{
    public class Shopping : IOrderModel
    {
        public bool ImplementFlag { get; set; } = true;

        public string GetOrderStr() => AppConfig.CommandShopping;

        public bool Judge(string destStr) => destStr.Replace("＃", "#").StartsWith(GetOrderStr());

        private static List<(Items, Items)> ShoppingList { get; set; } = [];

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
            var param = e.Message.Text.Substring(GetOrderStr().Length).Trim().Split([' '], StringSplitOptions.RemoveEmptyEntries);
            if (param.Length < 1)
            {
                GetShoppingList();
                StringBuilder stringBuilder = new();
                stringBuilder.AppendLine(AppConfig.ReplyShoppingHeader);
                for (int i = 1; i <= ShoppingList.Count; i++)
                {
                    var (item, coin) = ShoppingList[i - 1];
                    stringBuilder.AppendLine(AppConfig.ReplyShoppingDetail
                        .Replace("%ItemName%", item.Name)
                        .Replace("%ItemCount%", $"{item.Count}")
                        .Replace("%ItemDesc%", item.Description)
                        .Replace("%CoinName%", coin.Name)
                        .Replace("%CoinCount%", $"{coin.Count}")
                        .Replace("%CoinDesc%", coin.Description)
                        .Replace("%Index%", $"{i}"));
                }
                stringBuilder.Append($"示例：{GetOrderStr()} 序号 数量");
                sendText.MsgToSend.Add(stringBuilder.ToString());
                return result;
            }
            if (param.Length != 2 || !int.TryParse(param[0], out int index) || !int.TryParse(param[1], out int count))
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyParamInvalid, $"，示例：{GetOrderStr()} 序号 数量"));
                return result;
            }
            count = Math.Max(1, count);

            var player = Player.GetPlayer(e.FromQQ);
            if (player == null)
            {
                sendText.MsgToSend.Add(AppConfig.ReplyNoPlayer);
                return result;
            }

            GetShoppingList();
            if (index >= 1 && index <= ShoppingList.Count)
            {
                var (item, coin) = ShoppingList[index - 1];

                if (1.0 * count * coin.Count > int.MaxValue)
                {
                    sendText.MsgToSend.Add(string.Format(AppConfig.ReplyParamInvalid, ""));
                    return result;
                }
                int comsume = count * coin.Count;
                if (!InventoryItem.TryRemoveItem(player, coin.ID, comsume, out int currentCount))
                {
                    sendText.MsgToSend.Add(string.Format(AppConfig.ReplyItemLeak, coin.Name, comsume, currentCount));
                    return result;
                }
                item.Count *= count;
                player.GiveItem([
                    item
                ]);
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyBuyItem, comsume, coin.Name, item.Count, item.Name, currentCount, InventoryItem.GetItemCount(player, item.ID)));
                return result;
            }

            sendText.MsgToSend.Add(AppConfig.ReplyItemCannotBuy);
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

        private static void GetShoppingList()
        {
            ShoppingList = [];
            foreach (var trade in AppConfig.ShoppingList)
            {
                string[] split = trade.Split('|');
                int count = int.TryParse(split[0], out int intValue) ? intValue : -1;
                int itemIndex = int.TryParse(split[1], out intValue) ? intValue : -1;
                int price = int.TryParse(split[2], out intValue) ? intValue : -1;
                int coinIndex = int.TryParse(split[3], out intValue) ? intValue : -1;
                if (count > 0 && itemIndex > 0 && itemIndex <= CommonHelper.GetMaxItemValue() && price > 0 && coinIndex > 0 && coinIndex <= CommonHelper.GetMaxItemValue())
                {
                    var item = Items.GetItemByID((PublicInfos.Enums.Items)itemIndex);
                    item.Count = count;
                    var coin = Items.GetItemByID((PublicInfos.Enums.Items)coinIndex);
                    coin.Count = price;
                    ShoppingList.Add((item, coin));
                }
            }
        }
    }
}