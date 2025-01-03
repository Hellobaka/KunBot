using me.cqp.luohuaming.iKun.PublicInfos;
using me.cqp.luohuaming.iKun.PublicInfos.Models;
using me.cqp.luohuaming.iKun.Sdk.Cqp.EventArgs;

namespace me.cqp.luohuaming.iKun.Code.OrderFunctions
{
    public class Register : IOrderModel
    {
        public bool ImplementFlag { get; set; } = true;

        public string GetOrderStr() => AppConfig.CommandRegister;

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

            if (Player.Exists(e.FromQQ))
            {
                sendText.MsgToSend.Add(AppConfig.ReplyDuplicateRegister);
                return result;
            }
            var player = Player.Create(e.FromQQ);
            if (player != null)
            {
                int coinCount = AppConfig.ValueRegisterCoinReward;
                int eggCount = AppConfig.ValueRegisterEggReward;
                player.GiveItem([
                    Items.Coin(coinCount),
                    Items.KunEgg(eggCount)
                ]);

                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyNewRegister, coinCount, eggCount));
            }
            else
            {
                sendText.MsgToSend.Add(AppConfig.ReplyRegisterFailed);
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