using me.cqp.luohuaming.iKun.Sdk.Cqp;
using me.cqp.luohuaming.iKun.Sdk.Cqp.Model;
using System.Collections.Generic;

namespace me.cqp.luohuaming.iKun.PublicInfos
{
    public static class MainSave
    {
        /// <summary>
        /// 保存各种事件的数组
        /// </summary>
        public static List<IOrderModel> Instances { get; set; } = new List<IOrderModel>();

        public static CQLog CQLog { get; set; }

        public static CQApi CQApi { get; set; }

        public static string AppDirectory { get; set; }

        public static string ImageDirectory { get; set; }

        public static Dictionary<long, GroupMemberInfoCollection> GroupMemberInfos { get; set; } = [];
    }
}