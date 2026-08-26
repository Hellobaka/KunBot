using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace me.cqp.luohuaming.iKun.Admin.ViewModels;

/// <summary>
/// 最小化 INotifyPropertyChanged 基类（不引入 MVVM 库）。
/// </summary>
public abstract class ObservableObject : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    protected bool Set<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
