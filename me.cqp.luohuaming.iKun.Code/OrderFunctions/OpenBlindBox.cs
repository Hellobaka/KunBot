using me.cqp.luohuaming.iKun.PublicInfos;
using me.cqp.luohuaming.iKun.PublicInfos.Models;
using me.cqp.luohuaming.iKun.Sdk.Cqp.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace me.cqp.luohuaming.iKun.Code.OrderFunctions
{
    public class OpenBlindBox : IOrderModel
    {
        public bool ImplementFlag { get; set; } = true;

        public string GetOrderStr() => AppConfig.CommandOpenBlindBox;

        public bool Judge(string destStr) => destStr.Replace("＃", "#").StartsWith(GetOrderStr());

        private static List<(PublicInfos.Enums.Items, double)> BlindBoxes { get; set; } = null;

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
            int consume = count;
            if (!InventoryItem.TryRemoveItem(player, PublicInfos.Enums.Items.BlindBox, consume, out int currentCount))
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyItemLeak, PublicInfos.Models.Items.BlindBox().Name, consume, currentCount));
                return result;
            }
            List<Items> contents = [];
            StringBuilder stringBuilder = new();
            for (int i = 0; i < count; i++)
            {
                var item = GetBlindBox();
                if (item == PublicInfos.Enums.Items.Nothing)
                {
                    continue;
                }
                contents.Add(Items.GetItemByID(item));
            }
            foreach(var item in contents.GroupBy(x => x.Name, (key, groups) => new { Key = key, Items = groups.ToList() }))
            {
                stringBuilder.AppendLine("·" + item.Key + $" {item.Items.Count} 个");
            }
            stringBuilder.RemoveNewLine();
            if (contents.Count > 0)
            {
                player.GiveItem(contents);
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyBlindBoxOpen, consume, stringBuilder.ToString()));
                return result;
            }
            else
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyBlindBoxGetNothing, consume));
                return result;
            }
        }

        private static PublicInfos.Enums.Items GetBlindBox()
        {
            if (BlindBoxes == null)
            {
                BlindBoxes = [];
                foreach (var item in AppConfig.BlindBoxContents)
                {
                    string[] split = item.Split('|');
                    int index = int.TryParse(split[0], out int value) ? value : -1;
                    double probablity = double.TryParse(split[1], out double doubleValue) ? doubleValue : -1;
                    if (index >= 0 && index <= 6)
                    {
                        BlindBoxes.Add(((PublicInfos.Enums.Items)index, probablity));
                    }
                }
            }

            double probablityTotal = BlindBoxes.Sum(x => x.Item2);
            double random = probablityTotal * CommonHelper.Random.NextDouble();
            double p = 0;
            foreach (var item in BlindBoxes)
            {
                p += item.Item2;
                if (random < p)
                {
                    return item.Item1;
                }
            }
            return PublicInfos.Enums.Items.Nothing;
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