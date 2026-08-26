using Another_Mirai_Native.Abstractions.Context;
using me.cqp.luohuaming.iKun.Background;
using me.cqp.luohuaming.iKun.Domain.Configuration;
using me.cqp.luohuaming.iKun.Domain.Enums;
using me.cqp.luohuaming.iKun.Domain.Models;
using me.cqp.luohuaming.iKun.Features.Shared;
using me.cqp.luohuaming.iKun.Infrastructure;
using me.cqp.luohuaming.iKun.Infrastructure.Logging;

namespace me.cqp.luohuaming.iKun.Features.Ranking;

/// <summary>
/// 排行指令：全服排行、本群排行、天罚信息。
/// </summary>
public sealed class RankingFeature
{
    public static RankingFeature Instance { get; } = new();

    private RankingFeature()
    { }

    /// <summary>全服体重排行</summary>
    public void Global(GroupMessageContext e)
    {
        var replies = CoreConfiguration.Current.Replies;
        var builder = new System.Text.StringBuilder();
        builder.AppendLine(replies.RankingHeader);
        int rank = 1;
        foreach (var kun in KunQuery.TopByWeight(CoreConfiguration.Current.RankingSize))
        {
            kun.LoadAffixes();
            var record = Record.ByKunId(kun.Id);
            if (record is null)
            {
                continue;
            }
            try
            {
                var info = Runtime.Api.GroupApi.GetGroupMemberInfo(record.Group, record.QQ);
                string name = string.IsNullOrWhiteSpace(info.Card) ? info.Nick : info.Card!;
                AppendRankLine(builder, rank, name, kun);
                rank++;
            }
            catch (Exception exc)
            {
                Log.For("获取成员名片").Info($"获取失败，群={record.Group}，QQ={record.QQ}\n{exc.Message}，{exc.StackTrace}");
            }
        }
        builder.RemoveTrailingNewLine();
        CommandHelper.Reply(e, builder.ToString());
    }

    /// <summary>本群排行</summary>
    public void Group(GroupMessageContext e)
    {
        var replies = CoreConfiguration.Current.Replies;
        var members = e.FromGroup.GetGroupMemberList();
        if (members is null || members.Count == 0)
        {
            CommandHelper.Reply(e, "获取群成员列表失败");
            return;
        }
        var records = Record.ByQQs(members.Select(x => x.QQ).ToList());
        var kunIds = records.Select(x => x.KunID).ToHashSet();
        var kuns = KunQuery.TopByWeight(int.MaxValue).Where(k => kunIds.Contains(k.Id)).Take(CoreConfiguration.Current.RankingSize).ToList();
        var builder = new System.Text.StringBuilder();
        builder.AppendLine(replies.RankingGroupHeader);
        int rank = 1;
        foreach (var kun in kuns)
        {
            kun.LoadAffixes();
            try
            {
                var info = e.FromGroup.GetGroupMemberInfo(kun.PlayerID);
                string name = string.IsNullOrWhiteSpace(info.Card) ? info.Nick : info.Card!;
                AppendRankLine(builder, rank, name, kun);
                rank++;
            }
            catch (Exception exc)
            {
                Log.For("获取成员名片").Info($"获取失败，群={e.FromGroup.Id}，QQ={kun.PlayerID}\n{exc.Message}，{exc.StackTrace}");
            }
        }
        builder.RemoveTrailingNewLine();
        CommandHelper.Reply(e, builder.ToString());
    }

    private static void AppendRankLine(System.Text.StringBuilder builder, int rank, string name, Kun kun)
    {
        bool idling = IdleScheduler.IsRunning(kun.Id, IdleType.Experience);
        bool working = IdleScheduler.IsRunning(kun.Id, IdleType.Coin);
        var replies = CoreConfiguration.Current.Replies;
        builder.AppendLine(
            $"{rank}. [{name}] {kun} {kun.Weight.ToShortNumber()} {CoreConfiguration.Current.WeightUnit}" +
            $"{(idling ? $" {replies.RankingIdleTag}" : "")}{(working ? $" {replies.RankingWorkTag}" : "")}");
    }

    /// <summary>天罚时间查询</summary>
    public void PunishInfo(GroupMessageContext e)
    {
        var config = CoreConfiguration.Current;
        var day = config.PunishExecuteDayOfWeek == 7 ? DayOfWeek.Sunday : (DayOfWeek)config.PunishExecuteDayOfWeek;
        var dayName = day switch
        {
            DayOfWeek.Sunday => "周日",
            DayOfWeek.Monday => "周一",
            DayOfWeek.Tuesday => "周二",
            DayOfWeek.Wednesday => "周三",
            DayOfWeek.Thursday => "周四",
            DayOfWeek.Friday => "周五",
            _ => "周六",
        };
        CommandHelper.Reply(e, string.Format(
            config.Replies.PunishInfo, dayName, Background.RandomPunishService.NextExecuteTime.ToString("G")));
    }
}