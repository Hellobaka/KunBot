using me.cqp.luohuaming.iKun.PublicInfos;
using me.cqp.luohuaming.iKun.PublicInfos.Models;
using me.cqp.luohuaming.iKun.Sdk.Cqp.EventArgs;

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
            var kun = Kun.GetKunByQQ(player.QQ);
            if (kun != null)
            {
                sendText.MsgToSend.Add(AppConfig.ReplyDuplicateHatch);
                return result;
            }
            int hatchConsume = 1;
            if (!InventoryItem.TryRemoveItem(player, Items.KunEgg().ID, hatchConsume, out int currentCount))
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyItemLeak, Items.KunEgg().Name, hatchConsume, currentCount));
                return result;
            }
            int hatchSuccess = CommonHelper.Random.Next(AppConfig.ValueHatchProbablityMin, AppConfig.ValueHatchProbablityMax);
            if (CommonHelper.Random.Next(100) > hatchSuccess)
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyHatchFail, currentCount - hatchConsume));
                return result;
            }

            kun = Kun.RandomCreate(player);
            kun.Initialize();
            int id = Kun.SaveKun(kun);

            Record.AddRecord(new Record { Group = e.FromGroup, QQ = e.FromQQ, KunID = id });
            sendText.MsgToSend.Add(string.Format(AppConfig.ReplyHatchKun, kun.ToString(), kun.Weight, currentCount));
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