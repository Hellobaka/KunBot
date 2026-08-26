using me.cqp.luohuaming.iKun.Domain.Configuration;
using me.cqp.luohuaming.iKun.Infrastructure.Json;
using me.cqp.luohuaming.iKun.Infrastructure.Logging;

namespace me.cqp.luohuaming.iKun.Admin;

/// <summary>
/// 配置保存薄门面：管理面板各页通过本类写入配置。
/// 内部只调用 JsonConfigFile.SaveKeys（更新内存快照并原子重写文件），
/// 插件侧的 FileSystemWatcher 约 200ms 后热重载生效，此处不做其他操作。
/// </summary>
public static class ConfigEditor
{
    private static readonly Log Log = Log.For("管理面板");

    /// <summary>保存核心配置（Config.json）中的若干键</summary>
    /// <param name="values">JSON 键 → .NET 值（double/int/bool/DateTime/List&lt;long&gt;/List&lt;string&gt;）</param>
    /// <param name="error">失败原因；成功时为 null</param>
    public static bool TrySaveCore(IReadOnlyDictionary<string, object> values, out string error)
    {
        return TrySave(CoreConfiguration.Current, "核心配置", values, out error);
    }

    /// <summary>保存物品配置（Items.json）中的若干键</summary>
    /// <param name="values">JSON 键 → .NET 值</param>
    /// <param name="error">失败原因；成功时为 null</param>
    public static bool TrySaveItems(IReadOnlyDictionary<string, object> values, out string error)
    {
        return TrySave(ItemConfiguration.Current, "物品配置", values, out error);
    }

    private static bool TrySave(JsonConfigFile config, string name, IReadOnlyDictionary<string, object> values, out string error)
    {
        if (config is null)
        {
            error = $"{name}尚未初始化（插件可能未启用），无法保存";
            return false;
        }

        try
        {
            config.SaveKeys(values);
            Log.Info($"{name}保存成功，共 {values.Count} 项，插件将自动热重载生效");
            error = null;
            return true;
        }
        catch (Exception e)
        {
            Log.Error(e, $"{name}保存失败");
            error = $"保存失败：{e.Message}";
            return false;
        }
    }
}
