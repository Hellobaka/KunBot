using System.Windows.Input;

namespace me.cqp.luohuaming.iKun.Admin.ViewModels;

/// <summary>
/// 最小化 ICommand 实现，CanExecute 依赖 WPF CommandManager 自动刷新。
/// </summary>
public sealed class RelayCommand : ICommand
{
    private readonly Action _execute;
    private readonly Func<bool> _canExecute;

    public RelayCommand(Action execute, Func<bool> canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public bool CanExecute(object parameter) => _canExecute == null || _canExecute();

    public void Execute(object parameter) => _execute();

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
