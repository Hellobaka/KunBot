using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using me.cqp.luohuaming.iKun.Sdk.Cqp.EventArgs;
using me.cqp.luohuaming.iKun.PublicInfos;
using me.cqp.luohuaming.iKun.PublicInfos.Models;

namespace me.cqp.luohuaming.iKun.Code.OrderFunctions
{
    public class Login : IOrderModel
    {
        public bool ImplementFlag { get; set; } = true;
        
        public string GetOrderStr() => AppConfig.CommandLogin;

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
            if (CommonHelper.IsSameDay(player.LoginAt, DateTime.Now))
            {
                sendText.MsgToSend.Add(AppConfig.ReplyDuplicateLogin);
                return result;
            }
            player.LoginAt = DateTime.Now;
            player.Update();
            int coinCount = AppConfig.ValueLoginCoinReward;
            int eggCount = AppConfig.ValueLoginEggReward;
            player.GiveItem([
                Items.Coin(coinCount),
                Items.KunEgg(eggCount)
            ]);

            sendText.MsgToSend.Add(string.Format(AppConfig.ReplyLoginReward, coinCount, eggCount));

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
