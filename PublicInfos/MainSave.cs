using Another_Mirai_Native.Abstractions.Models;
using Another_Mirai_Native.Abstractions.Services;
using me.cqp.luohuaming.iKun.PublicInfos.Models;

namespace me.cqp.luohuaming.iKun.PublicInfos
{
    public static class MainSave
    {
        public static IPluginApi API { get; set; }

        public static string AppDirectory { get; set; }

        public static string ImageDirectory { get; set; }

        public static Dictionary<long, List<GroupMemberInfo>> GroupMemberInfos { get; set; } = [];
    }
}
