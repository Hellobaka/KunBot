using me.cqp.luohuaming.iKun.PublicInfos;
using me.cqp.luohuaming.iKun.PublicInfos.Models;
using me.cqp.luohuaming.iKun.Sdk.Cqp.EventArgs;

namespace me.cqp.luohuaming.iKun.Code.OrderFunctions
{
    public class ReleaseKun : IOrderModel
    {
        public bool ImplementFlag { get; set; } = true;

        public string GetOrderStr() => AppConfig.CommandReleaseKun;

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
            if (kun.Release())
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyReleaseSuccess, kun.ToString()));
            }
            else
            {
                sendText.MsgToSend.Add(AppConfig.ReplyReleaseFail);
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