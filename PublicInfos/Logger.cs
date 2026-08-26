namespace me.cqp.luohuaming.iKun.PublicInfos
{
    /// <summary>
    /// 框架日志封装
    /// </summary>
    public class Logger(string name)
    {
        private readonly string Name = name;

        public void Info(string message) => MainSave.API?.Logger?.Info(Name, message);

        public void Error(Exception e, string message) => MainSave.API?.Logger?.Error(Name, $"{message}\n{e}");

        public void Error(string message) => MainSave.API?.Logger?.Error(Name, message);

        public void Debug(string message) => MainSave.API?.Logger?.Debug(Name, message);

        public void Warn(string message) => MainSave.API?.Logger?.Warn(Name, message);
    }
}
