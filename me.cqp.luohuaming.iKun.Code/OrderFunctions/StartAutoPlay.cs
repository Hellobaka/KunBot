using me.cqp.luohuaming.iKun.PublicInfos;
using me.cqp.luohuaming.iKun.PublicInfos.Models;
using me.cqp.luohuaming.iKun.Sdk.Cqp.EventArgs;
using System;

namespace me.cqp.luohuaming.iKun.Code.OrderFunctions
{
    public class StartAutoPlay : IOrderModel
    {
        public bool ImplementFlag { get; set; } = true;

        public string GetOrderStr() => AppConfig.CommandStartAutoPlay;

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
            var param = e.Message.Text.Substring(GetOrderStr().Length).Trim();
            if (!int.TryParse(param, out int duration))
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyParamInvalid, $"，示例：{GetOrderStr()} 整数小时"));
                return result;
            }
            duration = Math.Max(1, duration);
            if (duration > AppConfig.ValueMaxAutoPlayDuration)
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyParamInvalid, $"，参数最大为 {AppConfig.ValueMaxAutoPlayDuration}"));
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
            if (kun.Weight == Kun.GetLevelWeightLimit(kun.Level))
            {
                sendText.MsgToSend.Add(AppConfig.ReplyWeightLimit);
                return result;
            }
            kun.Initialize();
            if (AutoPlay.CheckKunAutoPlay(kun))
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyAutoPlaying, kun));
                return result;
            }
            var start = DateTime.Now;
            var end = start.AddHours(duration);
            var autoPlay = new AutoPlay
            {
                Duration = duration,
                GroupId = e.FromGroup,
                KunID = kun.Id,
                StartTime = start,
                EndTime = end,
            };
            AutoPlay.AddAutoPlay(autoPlay);
            var exp = AutoPlay.CalcAutoPlayExp(kun.Level, start, end);
            sendText.MsgToSend.Add(string.Format(AppConfig.ReplyAutoPlayStarted, end.ToString("G"), exp.ToShortNumber()));
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