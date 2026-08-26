namespace me.cqp.luohuaming.iKun.Infrastructure.Logging;

/// <summary>
/// 带分类标签的日志门面，转发到框架 ILogger。
/// </summary>
public sealed class Log(string category)
{
    public static Log For(string category) => new(category);

    public void Debug(string message) => Write("Debug", message);

    public void Info(string message) => Write("Info", message);

    public void Warn(string message) => Write("Warn", message);

    public void Error(string message) => Write("Error", message);

    public void Error(Exception exception, string message) => Write("Error", $"{message}\n{exception}");

    private void Write(string level, string message)
    {
        var api = Runtime.TryGetApi();
        if (api is null)
        {
            return;
        }
        switch (level)
        {
            case "Debug": api.Logger.Debug(category, message); break;
            case "Info": api.Logger.Info(category, message); break;
            case "Warn": api.Logger.Warn(category, message); break;
            case "Error": api.Logger.Error(category, message); break;
        }
    }
}