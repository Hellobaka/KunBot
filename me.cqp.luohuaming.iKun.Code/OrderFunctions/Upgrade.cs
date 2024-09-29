using me.cqp.luohuaming.iKun.PublicInfos;
using me.cqp.luohuaming.iKun.PublicInfos.Models;
using me.cqp.luohuaming.iKun.Sdk.Cqp.EventArgs;
using System.Text;

namespace me.cqp.luohuaming.iKun.Code.OrderFunctions
{
    public class Upgrade : IOrderModel
    {
        public bool ImplementFlag { get; set; } = true;

        public string GetOrderStr() => AppConfig.CommandUpgrade;

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
            int currentCoin = InventoryItem.GetItemCount(player, PublicInfos.Enums.Items.Coin);
            int currentPill = InventoryItem.GetItemCount(player, PublicInfos.Enums.Items.UpgradePill);
            if (currentCoin < count * AppConfig.ValueTranmogifyCoinConsume)
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyItemLeak, Items.Coin().Name, count * AppConfig.ValueTranmogifyCoinConsume, currentCoin));
                return result;
            }
            if (currentPill < count * AppConfig.ValueTranmogifyPillConsume)
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyItemLeak, Items.UpgradePill().Name, count * AppConfig.ValueTranmogifyPillConsume, currentPill));
                return result;
            }
            InventoryItem.TryRemoveItem(player, PublicInfos.Enums.Items.Coin, count * AppConfig.ValueTranmogifyCoinConsume, out currentCoin);
            InventoryItem.TryRemoveItem(player, PublicInfos.Enums.Items.TransmogrifyPill, count * AppConfig.ValueTranmogifyPillConsume, out currentPill);

            kun.Initialize();
            var upgradeResult = kun.Upgrade(count);
            if (upgradeResult.Success is false)
            {
                sendText.MsgToSend.Add("强化方法过程发生异常，查看日志获取更多信息");
                return result;
            }
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(string.Format(AppConfig.ReplyUpgradeSuccess, upgradeResult.Increment.ToShortNumber(), upgradeResult.CurrentWeight.ToShortNumber()));
            if (upgradeResult.WeightLimit)
            {
                stringBuilder.AppendLine("·已达上限，需进行渡劫提高上限");
            }

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