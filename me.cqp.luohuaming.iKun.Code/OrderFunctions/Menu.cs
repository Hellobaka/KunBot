using me.cqp.luohuaming.iKun.PublicInfos;
using me.cqp.luohuaming.iKun.Sdk.Cqp.EventArgs;

namespace me.cqp.luohuaming.iKun.Code.OrderFunctions
{
    public class Menu : IOrderModel
    {
        public bool ImplementFlag { get; set; } = true;

        public string GetOrderStr() => AppConfig.CommandMenu;

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

            sendText.MsgToSend.Add(string.Format(AppConfig.ReplyMenu, AppConfig.CommandRegister, AppConfig.CommandLogin
                , AppConfig.CommandFeed, AppConfig.CommandUpgrade
                , AppConfig.CommandHatch, AppConfig.CommandInventory
                , AppConfig.CommandShopping, AppConfig.CommandOpenBlindBox
                , AppConfig.CommandOpenEgg, AppConfig.CommandTransmogrify
                , AppConfig.CommandAttack, AppConfig.CommandDevour
                , AppConfig.CommandQueryDeadKuns, AppConfig.CommandReleaseKun
                , AppConfig.CommandResurrect, AppConfig.CommandRanking
                , AppConfig.CommandAscend, AppConfig.CommandMenu));
            result.SendObject.Add(sendText);
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