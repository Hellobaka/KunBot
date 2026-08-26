using System.IO;
using me.cqp.luohuaming.iKun.Infrastructure.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace me.cqp.luohuaming.iKun.Infrastructure.Json;

/// <summary>
/// JSON 配置文件基类：读取、写入默认值、FileSystemWatcher 热重载。
/// 子类在 <see cref="Load"/> 中用 <see cref="Get{T}"/> 逐键声明配置项。
/// </summary>
public abstract class JsonConfigFile
{
    private readonly object _readLock = new();
    private readonly object _writeLock = new();
    private readonly FileSystemWatcher _watcher = new();

    protected JsonConfigFile(string path)
    {
        Path = path;
        Reload();
    }

    /// <summary>配置文件完整路径</summary>
    protected string Path { get; }

    private JObject Root { get; set; } = [];

    /// <summary>读取配置项；不存在时写入并返回默认值</summary>
    internal T Get<T>(string key, T defaultValue)
    {
        lock (_readLock)
        {
            if (Root.TryGetValue(key, out var token))
            {
                return token.ToObject<T>() ?? defaultValue;
            }
        }
        Set(key, defaultValue);
        return defaultValue;
    }

    protected void Set<T>(string key, T value)
    {
        lock (_writeLock)
        {
            Root[key] = JToken.FromObject(value);
            File.WriteAllText(Path, Root.ToString(Formatting.Indented));
        }
    }

    /// <summary>批量更新多个配置键并原子重写整个文件（单次写盘）。若启用了自动重载，FileSystemWatcher 会随后热重载生效。</summary>
    public void SaveKeys(IReadOnlyDictionary<string, object> values)
    {
        lock (_writeLock)
        {
            foreach (var kv in values)
            {
                Root[kv.Key] = kv.Value is JToken t ? t : JToken.FromObject(kv.Value);
            }

            // 先写同目录临时文件再原子替换，读取方/监视器不会看到半截文件
            var tmp = Path + ".tmp";
            File.WriteAllText(tmp, Root.ToString(Formatting.Indented));
            try
            {
                ReplaceIntoPlace();
            }
            catch
            {
                // 目标文件可能正被短暂占用，稍等重试一次
                Thread.Sleep(100);
                try
                {
                    ReplaceIntoPlace();
                }
                catch
                {
                    try { File.Delete(tmp); } catch { /* 临时文件删除失败可忽略 */ }
                    throw;
                }
            }
            void ReplaceIntoPlace()
            {
                if (File.Exists(Path))
                {
                    File.Replace(tmp, Path, null);
                }
                else
                {
                    File.Move(tmp, Path);
                }
            }
        }
        // 不调用 Load()/Reload()：内存中的 Root 已更新，即便没有监视器缓存状态也一致；
        // 若启用了自动重载，FileSystemWatcher 会随后热重载生效
    }

    /// <summary>子类加载入口：文件重载与热重载时都会调用</summary>
    protected abstract void Load();

    private bool Reload()
    {
        try
        {
            if (!File.Exists(Path))
            {
                File.WriteAllText(Path, "{}");
            }
            Root = JObject.Parse(File.ReadAllText(Path));
            Load();
            return true;
        }
        catch (Exception e)
        {
            Log.For(nameof(JsonConfigFile)).Error(e, $"配置加载失败: {Path}");
            return false;
        }
    }

    /// <summary>开启文件变更热重载</summary>
    public void EnableAutoReload()
    {
        _watcher.Path = System.IO.Path.GetDirectoryName(Path)!;
        _watcher.Filter = System.IO.Path.GetFileName(Path);
        _watcher.NotifyFilter = NotifyFilters.LastWrite;
        _watcher.Changed -= OnFileChanged;
        _watcher.Changed += OnFileChanged;
        _watcher.EnableRaisingEvents = true;
    }

    private void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        try
        {
            // 编辑器常触发多次 Changed，稍作等待合并
            Thread.Sleep(200);
            if (e.ChangeType == WatcherChangeTypes.Changed && Reload())
            {
                Load();
            }
        }
        catch
        {
            // 忽略热重载异常，保持旧配置运行
        }
    }
}