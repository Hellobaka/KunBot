using me.cqp.luohuaming.iKun.PublicInfos;
using me.cqp.luohuaming.iKun.PublicInfos.Models;
using me.cqp.luohuaming.iKun.Sdk.Cqp.EventArgs;

namespace me.cqp.luohuaming.iKun.Code.OrderFunctions
{
    public class Ascend : IOrderModel
    {
        public bool ImplementFlag { get; set; } = true;

        public string GetOrderStr() => AppConfig.CommandAscend;

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
            if (kun.Weight < Kun.GetLevelWeightLimit(kun.Level))
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyAscendNoWeightLimit, kun.Weight.ToShortNumber(), Kun.GetLevelWeightLimit(kun.Level)));
                return result;
            }
            if (!InventoryItem.TryRemoveItem(player, Items.Coin().ID, AppConfig.ValueAscendCoinConsume, out int currentCoin))
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyItemLeak, Items.Coin().Name, AppConfig.ValueAscendCoinConsume, currentCoin));
                return result;
            }
            var r = kun.Ascend();
            if (r.Success is false)
            {
                sendText.MsgToSend.Add("渡劫方法过程发生异常，查看日志获取更多信息");
                return result;
            }
            if (r.Dead)
            {
                sendText.MsgToSend.Add(AppConfig.ReplyAscendFailAndDead);
            }
            else if(r.Increment > 0)
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyAscendSuccess, r.Increment.ToShortNumber(), r.CurrentWeight.ToShortNumber(), r.CurrentLevel));
            }
            else
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyAscendFail, r.Increment.ToShortNumber(), r.CurrentWeight.ToShortNumber()));
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