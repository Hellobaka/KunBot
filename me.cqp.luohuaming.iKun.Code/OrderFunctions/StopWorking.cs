using me.cqp.luohuaming.iKun.PublicInfos;
using me.cqp.luohuaming.iKun.PublicInfos.Models;
using me.cqp.luohuaming.iKun.Sdk.Cqp.EventArgs;
using System;

namespace me.cqp.luohuaming.iKun.Code.OrderFunctions
{
    public class StopWorking : IOrderModel
    {
        public bool ImplementFlag { get; set; } = true;

        public string GetOrderStr() => AppConfig.CommandStopWorking;

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
            if (!kun.Alive)
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyKunNotAlive, AppConfig.ReplyStartAutoPlayFailed));
                return result;
            }
            if (kun.Abandoned)
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyKunAbandoned, AppConfig.ReplyStartAutoPlayFailed));
                return result;
            }
            kun.Initialize();
            if (!AutoPlay.CheckKunAutoPlay(kun, PublicInfos.Enums.AutoPlayType.Coin))
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyNotWorking, kun));
                return result;
            }

            var autoPlay = AutoPlay.GetKunAutoPlay(kun, PublicInfos.Enums.AutoPlayType.Coin);
            if (autoPlay == null)
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyNotWorking, kun));
                return result;
            }
            var r = autoPlay.Stop();
            string msg = string.Format(AppConfig.ReplyWorkingFinished, kun, r.Duration.TotalHours.ToString("f2"), (int)r.Increment, r.CurrentCoin);
            sendText.MsgToSend.Add(msg);
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