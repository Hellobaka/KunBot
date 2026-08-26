using Another_Mirai_Native.Abstractions;
using Another_Mirai_Native.Abstractions.Attributes;
using me.cqp.luohuaming.iKun.PublicInfos;
using me.cqp.luohuaming.iKun.PublicInfos.Enums;
using me.cqp.luohuaming.iKun.PublicInfos.Items;
using me.cqp.luohuaming.iKun.PublicInfos.Models;
using me.cqp.luohuaming.iKun.PublicInfos.Models.Results;

namespace me.cqp.luohuaming.iKun;

[PluginInfo(
    appId: "me.cqp.luohuaming.iKun",
    name: "iKun",
    version: "2.0.0",
    description: "养鲲插件，移植自酷Q版 KunBot",
    author: "Hellobaka")]
public class Entry : PluginBase
{
    public override async Task OnEnableAsync(CancellationToken ct)
    {
        MainSave.API = API;
        MainSave.AppDirectory = API.AppApi.GetAppDirectory();
        MainSave.ImageDirectory = Path.Combine(Directory.GetCurrentDirectory(), "data", "image");

        API.Logger.Info("初始化", "加载配置");
        AppConfig appConfig = new(Path.Combine(MainSave.AppDirectory, "Config.json"));
        appConfig.LoadConfig();
        appConfig.EnableAutoReload();

        ItemConfig itemConfig = new(Path.Combine(MainSave.AppDirectory, "Items.json"));
        itemConfig.LoadConfig();
        itemConfig.EnableAutoReload();

        API.Logger.Info("初始化", "创建数据库");
        SQLHelper.CreateDB();
        Kun.InitiazlizeRandom();

        API.Logger.Info("初始化", "加载挂机列表");
        AutoPlay.AutoPlayFinished -= OnAutoPlayFinished;
        AutoPlay.AutoPlayFinished += OnAutoPlayFinished;
        AutoPlay.LoadAutoPlays(API.MessageApi);

        API.Logger.Info("初始化", "启动天罚线程");
        RandomPunish.Start();

        API.Logger.Info("初始化", "初始化完成");
        await Task.CompletedTask;
    }

    public override async Task OnDisableAsync(CancellationToken ct)
    {
        API.Logger.Info("卸载", "停止后台任务");
        RandomPunish.Stop();
        AutoPlay.StopAll();
        AutoPlay.AutoPlayFinished -= OnAutoPlayFinished;
        await Task.CompletedTask;
    }

    private static void OnAutoPlayFinished(AutoPlay autoPlay, AutoPlayResult autoPlayResult, Kun kun)
    {
        try
        {
            if (autoPlayResult == null || kun == null || kun.Level <= 0 || kun.Weight <= 0 || kun.Abandoned)
            {
                return;
            }
            if (AppConfig.Groups.Contains(autoPlay.GroupId))
            {
                string msg = "";
                switch (autoPlay.AutoPlayType)
                {
                    case AutoPlayType.Exp:
                        if (autoPlayResult.Dead)
                        {
                            msg = string.Format(CommonHelper.CQCode_At(kun.PlayerID) + AppConfig.ReplyAutoPlayFinishedButDead, kun.ToString(), autoPlayResult.Duration.TotalHours, autoPlayResult.Increment.ToShortNumber());
                        }
                        else
                        {
                            msg = string.Format(CommonHelper.CQCode_At(kun.PlayerID) + AppConfig.ReplyAutoPlayFinished, kun.ToString(), autoPlayResult.Duration.TotalHours, autoPlayResult.Increment.ToShortNumber(), kun.Weight.ToShortNumber());
                            if (autoPlayResult.WeightLimit)
                            {
                                msg += $"\n{AppConfig.ReplyWeightLimit}";
                            }
                        }
                        break;
                    case AutoPlayType.Coin:
                        if (!kun.Alive)
                        {
                            return;
                        }
                        var player = Player.GetPlayer(kun.PlayerID);
                        int currentCoin = player == null ? 0 : InventoryItem.GetItemCount(player, PublicInfos.Enums.Items.Coin);
                        msg = string.Format(CommonHelper.CQCode_At(kun.PlayerID) + AppConfig.ReplyWorkingFinished, kun.ToString(), autoPlayResult.Duration.TotalHours, (int)autoPlayResult.Increment, currentCoin);
                        break;
                    default:
                        break;
                }
                if (!string.IsNullOrEmpty(msg))
                {
                    MainSave.API.MessageApi.SendGroupMessage(autoPlay.GroupId, msg);
                }
            }
        }
        catch (Exception ex)
        {
            MainSave.API.Logger.Error("挂机结算", $"异常发生：{ex.Message}\n{ex.StackTrace}");
        }
    }
}
