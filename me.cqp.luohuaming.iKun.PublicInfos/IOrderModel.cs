using System.Collections.Generic;
using System.Text;
using me.cqp.luohuaming.iKun.Sdk.Cqp.EventArgs;

namespace me.cqp.luohuaming.iKun.PublicInfos
{
    public interface IOrderModel
    {
        bool ImplementFlag { get; set; }
        string GetOrderStr();
        bool Judge(string destStr);
        FunctionResult Execute(CQGroupMessageEventArgs e);
        FunctionResult Execute(CQPrivateMessageEventArgs e);
    }
}
