using System.IO;
using NLog;

namespace me.cqp.luohuaming.iKun.Infrastructure.Logging;

/// <summary>
/// 带分类标签的日志门面，同时输出到两个目标：
/// 1. NLog 文件日志——按原插件样式，写入插件数据目录 Logs\log.txt，按天归档到 Logs\Archives；
/// 2. 框架日志——转发到框架 ILogger，便于在框架控制台/日志中查看。
/// </summary>
public sealed class Log(string category)
{
    private readonly NLog.Logger _file = LogManager.GetLogger(category);

    public static Log For(string category) => new(category);

    /// <summary>
    /// 初始化 NLog 文件日志（样式与原插件一致）：
    /// <paramref name="dataDirectory"/>\Logs\log.txt，按天压缩归档至 Logs\Archives\log_{日期}.zip，保留最近 30 份。
    /// </summary>
    public static void InitFileLogging(string dataDirectory)
    {
        var logDir = Path.Combine(dataDirectory, "Logs");
        Directory.CreateDirectory(Path.Combine(logDir, "Archives"));

        var config = new NLog.Config.LoggingConfiguration();
        var logfile = new NLog.Targets.FileTarget
        {
            FileName = Path.Combine(logDir, "log.txt"),
            Layout = "[${longdate}][${logger}][${uppercase:${level}}] ${message}${exception:format=tostring}",
            ArchiveFileName = Path.Combine(logDir, "Archives", "log_{#}.zip"),
            ArchiveEvery = NLog.Targets.FileArchivePeriod.Day,
            ArchiveNumbering = NLog.Targets.ArchiveNumberingMode.Date,
            MaxArchiveFiles = 30,
            ConcurrentWrites = true,
            KeepFileOpen = true,
            EnableArchiveFileCompression = true,
        };
        config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);
        LogManager.Configuration = config;
    }

    /// <summary>关闭 NLog 文件日志（插件卸载时调用，刷盘并释放文件句柄）。</summary>
    public static void ShutdownFileLogging() => LogManager.Shutdown();

    public void Debug(string message) => Write(LogLevel.Debug, message);

    public void Info(string message) => Write(LogLevel.Info, message);

    public void Warn(string message) => Write(LogLevel.Warn, message);

    public void Error(string message) => Write(LogLevel.Error, message);

    public void Error(Exception exception, string message)
    {
        _file.Error(exception, message);
        WriteToFramework("Error", $"{message}\n{exception}");
    }

    private void Write(LogLevel level, string message)
    {
        switch (level.ToString())
        {
            case "Debug": _file.Debug(message); break;
            case "Info": _file.Info(message); break;
            case "Warn": _file.Warn(message); break;
            default: _file.Error(message); break;
        }
        WriteToFramework(level.ToString(), message);
    }

    private void WriteToFramework(string level, string message)
    {
        var api = Runtime.TryGetApi();
        if (api is null)
        {
            return;
        }
        switch (level)
        {
            case "Warn": api.Logger.Warn(category, message);
                break;
            case "Error": api.Logger.Error(category, message);
                break;
        }
    }
}