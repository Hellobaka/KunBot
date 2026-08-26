using Newtonsoft.Json.Linq;
using System.IO;

namespace me.cqp.luohuaming.iKun.PublicInfos
{
    /// <summary>
    /// 配置读取帮助类
    /// </summary>
    public class ConfigBase
    {
        public ConfigBase(string configPath)
        {
            ConfigPath = configPath;
            Load();
        }

        public FileSystemWatcher ConfigChangeWatcher { get; set; } = new();

        /// <summary>
        /// 配置文件路径
        /// </summary>
        public string ConfigPath { get; set; }

        public object ReadLock { get; set; } = new();

        public object WriteLock { get; set; } = new();

        public JObject CurrentJObject { get; set; }

        /// <summary>
        /// 读取配置
        /// </summary>
        /// <param name="sectionName">需要读取的配置键名</param>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>目标类型的配置</returns>
        public T GetConfig<T>(string sectionName, T defaultValue = default)
        {
            lock (ReadLock)
            {
                if (CurrentJObject != null && CurrentJObject.ContainsKey(sectionName))
                {
                    return CurrentJObject[sectionName].ToObject<T>();
                }

                if (defaultValue != null)
                {
                    SetConfig(sectionName, defaultValue);
                    return defaultValue;
                }
                return defaultValue;
            }
        }

        public void SetConfig<T>(string sectionName, T value)
        {
            lock (WriteLock)
            {
                CurrentJObject ??= [];
                if (CurrentJObject.ContainsKey(sectionName))
                {
                    CurrentJObject[sectionName] = JToken.FromObject(value);
                }
                else
                {
                    CurrentJObject.Add(sectionName, JToken.FromObject(value));
                }

                File.WriteAllText(ConfigPath, CurrentJObject.ToString(Newtonsoft.Json.Formatting.Indented));
            }
        }

        public bool Load()
        {
            try
            {
                if (File.Exists(ConfigPath) is false)
                {
                    File.WriteAllText(ConfigPath, "{}");
                }
                CurrentJObject = JObject.Parse(File.ReadAllText(ConfigPath));
                return true;
            }
            catch (Exception e)
            {
                MainSave.API?.Logger?.Debug("配置热重载", $"LoadFail: {e.Message}");
                return false;
            }
        }

        public void EnableAutoReload()
        {
            ConfigChangeWatcher.Path = Path.GetDirectoryName(ConfigPath);
            ConfigChangeWatcher.Filter = Path.GetFileName(ConfigPath);
            ConfigChangeWatcher.NotifyFilter = NotifyFilters.LastWrite;
            ConfigChangeWatcher.Changed -= ConfigChangeWatcher_Changed;
            ConfigChangeWatcher.Changed += ConfigChangeWatcher_Changed;
            ConfigChangeWatcher.EnableRaisingEvents = true;
        }

        public void DisableAutoReload()
        {
            ConfigChangeWatcher.EnableRaisingEvents = false;
        }

        public virtual void LoadConfig()
        {
        }

        private void ConfigChangeWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            try
            {
                if (e.ChangeType == WatcherChangeTypes.Changed && Load())
                {
                    LoadConfig();
                }
            }
            catch
            {
            }
        }
    }
}
