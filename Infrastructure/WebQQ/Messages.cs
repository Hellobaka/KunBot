namespace me.cqp.luohuaming.iKun.Infrastructure.WebQQ;

/// <summary>
/// 消息片段构造辅助：CQ 码文本、消息发送封装。
/// </summary>
public static class Messages
{
    /// <summary>At 码文本，用于拼进 Reply 模板格式的纯文本消息</summary>
    public static string At(long qq) => $"[CQ:at,qq={qq}]";

    /// <summary>向群发送文本（后台线程安全）</summary>
    public static void SendGroup(long groupId, string message) => me.cqp.luohuaming.iKun.Infrastructure.Runtime.Api.MessageApi.SendGroupMessage(groupId, message);
}