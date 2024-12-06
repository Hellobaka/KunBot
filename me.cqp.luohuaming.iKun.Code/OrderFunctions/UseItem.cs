using me.cqp.luohuaming.iKun.PublicInfos;
using me.cqp.luohuaming.iKun.PublicInfos.Models;
using me.cqp.luohuaming.iKun.Sdk.Cqp.EventArgs;
using System;

namespace me.cqp.luohuaming.iKun.Code.OrderFunctions
{
    public class UseItem : IOrderModel
    {
        public bool ImplementFlag { get; set; } = true;

        public string GetOrderStr() => AppConfig.CommandUseItem;

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
            var param = e.Message.Text.Substring(GetOrderStr().Length).Trim().Split([' '], StringSplitOptions.RemoveEmptyEntries);
            if (param.Length != 1 && param.Length != 2)
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyParamInvalid, $"，示例：{GetOrderStr()} 物品ID/名称 数量"));
                return result;
            }

            int count = 1;
            Items items;
            if ((items = Items.GetItemByName(param[0])) == null
                && (!int.TryParse(param[0], out int value)
                    || (items = Items.GetItemByID((PublicInfos.Enums.Items)value)) == null))
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyParamInvalid, $"，指定的物品 ID 或名称无效"));
                return result;
            }
            if (param.Length == 2)
            {
                count = int.TryParse(param[1], out value) ? value : count;
                count = Math.Max(count, 1);
            }
            if (!items.Usable)
            {
                sendText.MsgToSend.Add(AppConfig.ReplyItemCannotUse);
                return result;
            }
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
            if (AutoPlay.CheckKunAutoPlay(kun))
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyAutoPlaying, kun));
                return result;
            }
            if (AutoPlay.CheckKunAutoPlay(kun, PublicInfos.Enums.AutoPlayType.Coin))
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyWorking, kun));
                return result;
            }

            if (!InventoryItem.TryRemoveItem(player, items.ID, count, out var currentItem))
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyItemLeak, items.Name, count, currentItem));
                return result;
            }
            var reply = items.UseItem(count, player, kun);
            if (!reply.Item1)
            {
                sendText.MsgToSend.Add(AppConfig.ReplyItemUseFailed);
                items.Count = count;
                player.GiveItem([items]);
                return result;
            }
            sendText.MsgToSend.Add(reply.Item2);
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