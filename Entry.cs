using Another_Mirai_Native.Abstractions;
using Another_Mirai_Native.Abstractions.Attributes;
using me.cqp.luohuaming.iKun.Background;
using me.cqp.luohuaming.iKun.Domain.Configuration;
using me.cqp.luohuaming.iKun.Infrastructure;
using me.cqp.luohuaming.iKun.Infrastructure.Logging;
using me.cqp.luohuaming.iKun.Infrastructure.Persistence;

namespace me.cqp.luohuaming.iKun;

/// <summary>
/// 插件入口：仅负责启动装配与停用清理。
/// 初始化顺序有依赖：App → 配置 → 数据库 → 挂机任务恢复 → 天罚线程。
/// </summary>
[PluginInfo(
    appId: "me.cqp.luohuaming.iKun",
    name: "iKun",
    version: "2.1.0",
    description: "养鲲插件，移植自酷Q版 KunBot（分层架构）",
    author: "Hellobaka")]
public class Entry : PluginBase
{
    private static readonly Log Log = Log.For("初始化");

    public override async Task OnEnableAsync(CancellationToken ct)
    {
        // 1. 运行环境
        Runtime.Init(API);

        // 2. 配置（含热重载）
        Log.Info("加载配置");
        CoreConfiguration.CreateCurrent(Path.Combine(Runtime.DataDirectory, "Config.json"));
        ItemConfiguration.CreateCurrent(Path.Combine(Runtime.DataDirectory, "Items.json"));

        // 3. 数据库
        Log.Info("创建数据库");
        Db.Initialize();

        // 4. 挂机调度器与结算通知
        Log.Info("恢复挂机任务");
        AutoPlaySettlementNotifier.Attach();
        IdleScheduler.Instance.ResumeFromDatabase(API.MessageApi);

        // 5. 天罚
        Log.Info("启动天罚服务");
        RandomPunishService.Start();

        Log.Info("初始化完成");
        await Task.CompletedTask;
    }

    public override async Task OnDisableAsync(CancellationToken ct)
    {
        Log.For("卸载").Info("停止后台任务");
        RandomPunishService.Stop();
        IdleScheduler.Instance.Shutdown();
        AutoPlaySettlementNotifier.Detach();
        await Task.CompletedTask;
    }
}