using Another_Mirai_Native.Abstractions.Models;

namespace me.cqp.luohuaming.iKun.Infrastructure.WebQQ;

/// <summary>
/// 群成员缓存：按群缓存成员列表，用于同群判定与昵称/群名片解析。
/// </summary>
public static class GroupMemberCache
{
    private static readonly Dictionary<long, List<GroupMemberInfo>> Cache = [];
    private static readonly object Lock = new();

    public static IReadOnlyList<GroupMemberInfo> GetMembers(long groupId)
    {
        lock (Lock)
        {
            if (Cache.TryGetValue(groupId, out var members) && members.Count > 0)
            {
                return members;
            }
            members = me.cqp.luohuaming.iKun.Infrastructure.Runtime.Api.GroupApi.GetGroupMembers(groupId);
            Cache[groupId] = members;
            return members;
        }
    }

    /// <summary>目标 QQ 是否在指定群内</summary>
    public static bool Contains(long groupId, long qq) => GetMembers(groupId).Any(x => x.QQ == qq);

    /// <summary>按昵称或群名片模糊查找成员</summary>
    public static GroupMemberInfo? FindByName(long groupId, string keyword) =>
        GetMembers(groupId).FirstOrDefault(x =>
            (x.Nick?.Contains(keyword) ?? false) || (x.Card?.Contains(keyword) ?? false));

    public static void Invalidate(long groupId)
    {
        lock (Lock)
        {
            Cache.Remove(groupId);
        }
    }
}