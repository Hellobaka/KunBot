using Another_Mirai_Native.Abstractions.Services;

namespace me.cqp.luohuaming.iKun.Infrastructure;

/// <summary>
/// 插件运行环境：框架 API 与各目录路径的唯一持有者。
/// 在 Entry 启动时初始化一次，全插件只读访问。
/// </summary>
public static class Runtime
{
    private static bool _initialized;

    public static IPluginApi Api { get; private set; } = null!;

    /// <summary>插件专属数据目录（配置、数据库等）</summary>
    public static string DataDirectory { get; private set; } = "";

    /// <summary>框架媒体图片目录（data\image）</summary>
    public static string ImageDirectory { get; private set; } = "";

    public static void Init(IPluginApi api)
    {
        if (_initialized)
        {
            return;
        }
        Api = api;
        DataDirectory = api.AppApi.GetAppDirectory();
        ImageDirectory = Path.Combine(Directory.GetCurrentDirectory(), "data", "image");
        _initialized = true;
    }

    /// <summary>
    /// 获取尚未初始化时的安全 API 引用（用于静态字段初始化阶段的日志等场景）。
    /// </summary>
    internal static IPluginApi? TryGetApi() => _initialized ? Api : null;
}