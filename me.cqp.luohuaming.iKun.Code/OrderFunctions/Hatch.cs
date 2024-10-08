using me.cqp.luohuaming.iKun.PublicInfos;
using me.cqp.luohuaming.iKun.PublicInfos.Models;
using me.cqp.luohuaming.iKun.Sdk.Cqp.EventArgs;
using System;

namespace me.cqp.luohuaming.iKun.Code.OrderFunctions
{
    public class Hatch : IOrderModel
    {
        public bool ImplementFlag { get; set; } = true;

        public string GetOrderStr() => AppConfig.CommandHatch;

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
                count = 1;
            }
            count = Math.Max(1, count);

            var player = Player.GetPlayer(e.FromQQ);
            if (player == null)
            {
                sendText.MsgToSend.Add(AppConfig.ReplyNoPlayer);
                return result;
            }
            var kun = Kun.GetKunByQQ(player.QQ);
            if (kun != null)
            {
                sendText.MsgToSend.Add(AppConfig.ReplyDuplicateHatch);
                return result;
            }
            int hatchConsume = count;
            if (!InventoryItem.TryRemoveItem(player, Items.KunEgg().ID, hatchConsume, out int currentCount))
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyItemLeak, Items.KunEgg().Name, hatchConsume, currentCount));
                return result;
            }
            int hatchSuccess = CommonHelper.Random.Next(AppConfig.ValueHatchProbablityMin, AppConfig.ValueHatchProbablityMax);
            int consume = 0; 
            bool success = false;
            for (consume = 1; consume <= count; consume++)
            {
                if (CommonHelper.Random.Next(100) > hatchSuccess)
                {
                    continue;
                }
                success = true;
                break;
            }
            if (!success)
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyHatchFail, currentCount));
                return result;
            }
            int diff = count - consume;
            player.GiveItem([Items.KunEgg(diff)]);

            kun = Kun.RandomCreate(player);
            kun.Initialize();
            int id = Kun.SaveKun(kun);

            Record.AddRecord(new Record { Group = e.FromGroup, QQ = e.FromQQ, KunID = id });
            if (count > 1)
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyMultiHatchKun, kun.ToString(), kun.Weight, consume, currentCount + diff));
            }
            else
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyHatchKun, kun.ToString(), kun.Weight, currentCount));
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