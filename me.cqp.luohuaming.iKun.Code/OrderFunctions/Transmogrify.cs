using me.cqp.luohuaming.iKun.PublicInfos;
using me.cqp.luohuaming.iKun.PublicInfos.Models;
using me.cqp.luohuaming.iKun.Sdk.Cqp.EventArgs;

namespace me.cqp.luohuaming.iKun.Code.OrderFunctions
{
    public class Transmogrify : IOrderModel
    {
        public bool ImplementFlag { get; set; } = true;

        public string GetOrderStr() => AppConfig.CommandTransmogrify;

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
            if (kun.Level < AppConfig.ValueTransmogrifyLevelLimit)
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyTransmogrifyLevelLimit, kun.Level, AppConfig.ValueTransmogrifyLevelLimit));
                return result;
            }
            kun.Initialize();
            if (AutoPlay.CheckKunAutoPlay(kun))
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyAutoPlaying, kun));
                return result;
            }
            int currentCoin = InventoryItem.GetItemCount(player, PublicInfos.Enums.Items.Coin);
            int currentPill = InventoryItem.GetItemCount(player, PublicInfos.Enums.Items.TransmogrifyPill);
            if (currentCoin < AppConfig.ValueTranmogifyCoinConsume)
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyItemLeak, Items.Coin().Name, AppConfig.ValueTranmogifyCoinConsume, currentCoin));
                return result;
            }
            if (currentPill < AppConfig.ValueTranmogifyPillConsume)
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyItemLeak, Items.TransmogrifyPill().Name, AppConfig.ValueTranmogifyPillConsume, currentPill));
                return result;
            }
            InventoryItem.TryRemoveItem(player, PublicInfos.Enums.Items.Coin, AppConfig.ValueTranmogifyCoinConsume, out currentCoin);
            InventoryItem.TryRemoveItem(player, PublicInfos.Enums.Items.TransmogrifyPill, AppConfig.ValueTranmogifyPillConsume, out currentPill);

            var r = kun.Transmogrify();
            if (r.Success is false)
            {
                sendText.MsgToSend.Add("幻化方法过程发生异常，查看日志获取更多信息");
                return result;
            }
            if (r.Dead)
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyTransmogrifyFailAndDead, currentPill, currentCoin));
            }
            else if (r.CurrentAttributeA.ID == r.OriginalAttributeA.ID && r.CurrentAttributeB.ID == r.OriginalAttributeB.ID)
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyTransmogrifyFail, r.Decrement.ToShortNumber(), r.CurrentWeight.ToShortNumber(), currentPill, currentCoin));
            }
            else
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyTransmogrifySuccess, $"[{r.OriginalAttributeA.Name}]{r.OriginalAttributeB.Name}鲲", $"[{r.CurrentAttributeA.Name}]{r.CurrentAttributeB.Name}鲲", r.Decrement.ToShortNumber(), r.CurrentWeight.ToShortNumber(), currentPill, currentCoin));
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