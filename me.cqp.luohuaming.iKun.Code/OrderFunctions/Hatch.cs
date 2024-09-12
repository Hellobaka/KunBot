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

            var player = Player.GetPlayer(e.FromQQ);
            if (player == null)
            {
                sendText.MsgToSend.Add(AppConfig.ReplyNoPlayer);
                return result;
            }
            int hatchComsume = 1;
            if (!InventoryItem.TryRemoveItem(player, Items.KunEgg().ID, hatchComsume, out int currentCount))
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyItemLeak, hatchComsume, currentCount));
                return result;
            }
            int hatchSuccess = CommonHelper.Random.Next(AppConfig.ValueHatchProbablityMin, AppConfig.ValueHatchProbablityMax);
            if (CommonHelper.Random.Next(100) > hatchSuccess)
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyHatchFail, currentCount - hatchComsume));
                return result;
            }

            var kun = Kun.RandomCreate(player);
            int id = Kun.SaveKun(kun);

            Record.AddRecord(new Record { Group = e.FromGroup, QQ = e.FromQQ, KunID = id });
            sendText.MsgToSend.Add(string.Format(AppConfig.ReplyHatchKun, kun.ToString(), kun.Weight, currentCount - hatchComsume));
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
