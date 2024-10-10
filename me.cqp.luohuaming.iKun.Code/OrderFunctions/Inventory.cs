using me.cqp.luohuaming.iKun.PublicInfos;
using me.cqp.luohuaming.iKun.PublicInfos.Models;
using me.cqp.luohuaming.iKun.Sdk.Cqp.EventArgs;
using System.Text;

namespace me.cqp.luohuaming.iKun.Code.OrderFunctions
{
    public class Inventory : IOrderModel
    {
        public bool ImplementFlag { get; set; } = true;

        public string GetOrderStr() => AppConfig.CommandInventory;

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
            StringBuilder stringBuilder = new();
            var kun = Kun.GetKunByQQ(player.QQ);
            if (kun == null)
            {
                stringBuilder.AppendLine(AppConfig.ReplyNoKun);
            }
            else
            {
                kun.Initialize();
                stringBuilder.AppendLine(kun.ToStringFull());
            }
            stringBuilder.AppendLine("=============");
            var list = InventoryItem.GetItemsByQQ(e.FromQQ);
            if (list == null || list.Count == 0)
            {
                sendText.MsgToSend.Add(AppConfig.ReplyEmptyInventory);
            }
            else
            {
                foreach (var item in list)
                {
                    var items = Items.GetItemByID((PublicInfos.Enums.Items)item.ItemID);
                    if (items == null)
                    {
                        continue;
                    }
                    stringBuilder.AppendLine(item.ToString());
                }
                stringBuilder.RemoveNewLine();

                sendText.MsgToSend.Add(stringBuilder.ToString());
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