using me.cqp.luohuaming.iKun.PublicInfos;
using me.cqp.luohuaming.iKun.PublicInfos.Models;
using me.cqp.luohuaming.iKun.Sdk.Cqp.EventArgs;
using System.Linq;

namespace me.cqp.luohuaming.iKun.Code.OrderFunctions
{
    public class UseNickName : IOrderModel
    {
        public bool ImplementFlag { get; set; } = true;

        public string GetOrderStr() => AppConfig.CommandUseCustomNickName;

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
            string nickName = e.Message.Text.Replace(GetOrderStr(), "").Trim();
            if (string.IsNullOrEmpty(nickName))
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyParamInvalid, $"，示例：{GetOrderStr()} 昵称"));
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
            if (AppConfig.NickNameFilter.Any(x => nickName.ToLower().Contains(x)))
            {
                sendText.MsgToSend.Add(AppConfig.ReplyCustomNickInvalid);
                return result;
            }
            kun.NickName = nickName;
            kun.Update();

            sendText.MsgToSend.Add(string.Format(AppConfig.ReplyCustomNickApplied, kun));
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