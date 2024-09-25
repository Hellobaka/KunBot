using NLog;
using System;
using System.IO;

namespace me.cqp.luohuaming.iKun.PublicInfos
{
    public class Logger(string name)
    {
        private NLog.Logger CurrentLogger { get; set; } = LogManager.GetLogger(name);

        public static void Init()
        {
            var config = new NLog.Config.LoggingConfiguration();
            var logfile = new NLog.Targets.FileTarget()
            {
                FileName = Path.Combine(MainSave.AppDirectory, "Logs", "log.txt"),
                Layout = "[${longdate}][${logger}][${uppercase:${level}}] ${message}${exception:format=tostring}",
                ArchiveFileName = Path.Combine(MainSave.AppDirectory, "Logs", "Archives", "log_{#}.zip"),
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

        public void Info(string message)
        {
            CurrentLogger.Info(message);
        }

        public void Error(Exception e, string message)
        {
            CurrentLogger.Error(e, message);
        }

        public void Error(string message)
        {
            CurrentLogger.Error(message);
        }
    }
}
