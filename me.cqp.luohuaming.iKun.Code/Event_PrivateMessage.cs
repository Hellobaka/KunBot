using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using me.cqp.luohuaming.iKun.Sdk.Cqp.EventArgs;
using me.cqp.luohuaming.iKun.PublicInfos;

namespace me.cqp.luohuaming.iKun.Code
{
    public class Event_PrivateMessage
    {
        public static FunctionResult PrivateMessage(CQPrivateMessageEventArgs e)
        {
            FunctionResult result = new FunctionResult()
            {
                SendFlag = false
            };
            try
            {
                foreach (var item in MainSave.Instances.Where(item => item.Judge(e.Message.Text)))
                {
                    return item.Execute(e);
                }
                return result;
            }
            catch (Exception exc)
            {
                MainSave.CQLog.Info("异常抛出", exc.Message + exc.StackTrace);
                return result;
            }
        }
    }
}
