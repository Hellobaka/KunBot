using me.cqp.luohuaming.iKun.PublicInfos;
using me.cqp.luohuaming.iKun.PublicInfos.Models;
using me.cqp.luohuaming.iKun.Sdk.Cqp.EventArgs;
using System;

namespace me.cqp.luohuaming.iKun.Code.OrderFunctions
{
    public class StopAutoPlay : IOrderModel
    {
        public bool ImplementFlag { get; set; } = true;

        public string GetOrderStr() => AppConfig.CommandStopAutoPlay;

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
            if (!AutoPlay.CheckKunAutoPlay(kun))
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyNotAutoPlaying, kun));
                return result;
            }

            var autoPlay = AutoPlay.GetKunAutoPlay(kun, PublicInfos.Enums.AutoPlayType.Exp);
            if (autoPlay == null)
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyNotAutoPlaying, kun));
                return result;
            }
            var r = autoPlay.Stop();
            if (r == null)
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyKunNotAlive, kun));
                return result;
            }
            string msg = "";
            if (r.Dead)
            {
                msg = string.Format(AppConfig.ReplyAutoPlayFinishedButDead, kun, r.Duration.TotalHours.ToString("f2"), r.Increment.ToShortNumber());
            }
            else
            {
                msg = string.Format(AppConfig.ReplyAutoPlayFinished, kun, r.Duration.TotalHours.ToString("f2"), r.Increment.ToShortNumber(), r.CurrentWeight.ToShortNumber());
                if (r.WeightLimit)
                {
                    msg += $"\n{AppConfig.ReplyWeightLimit}";
                }
            }
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