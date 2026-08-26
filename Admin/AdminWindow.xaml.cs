using System.Windows;
using System.Windows.Controls;
using HandyControl.Controls;

namespace me.cqp.luohuaming.iKun.Admin;

/// <summary>
/// 管理面板主窗口：左侧 SideMenu 导航 + 中间 ContentControl 页面宿主。
/// 页面懒加载并缓存（用户在某页的编辑状态在切页时保留，各页自带"重新加载"按钮）。
/// 无 Application 类——Application 由 AdminWindowEntry 在 UI 线程上创建。
/// </summary>
public partial class AdminWindow
{
    private readonly Dictionary<int, UserControl> _pageCache = new();

    /// <summary>菜单项 Tag(0..5) → 页面工厂。</summary>
    private static readonly Func<UserControl>[] _pageFactories =
    {
        () => new Views.PlayerPage(),
        () => new Views.KunPage(),
        () => new Views.CommandsConfigPage(),
        () => new Views.RepliesConfigPage(),
        () => new Views.NumbersConfigPage(),
        () => new Views.ItemsConfigPage(),
    };

    public AdminWindow()
    {
        InitializeComponent();
    }

    /// <summary>
    /// 窗口加载后默认展示"用户概况"（索引 0）：
    /// 通过 SideMenuItem.SelectedEvent 触发选中，使 SideMenu 内部选中状态
    /// （高亮/互斥）与代码保持一致，并经由 OnMenuItemSelected 切页。
    /// </summary>
    private void OnWindowLoaded(object sender, RoutedEventArgs e)
    {
        if (SideMenuCtl.Items.Count > 0 && SideMenuCtl.Items[0] is SideMenuItem first)
        {
            first.RaiseEvent(new RoutedEventArgs(SideMenuItem.SelectedEvent, first));
        }
        ShowPage(0);
    }

    /// <summary>侧边菜单项被选中（RoutedEvent SideMenuItem.SelectedEvent）。</summary>
    private void OnMenuItemSelected(object sender, RoutedEventArgs e)
    {
        if (sender is SideMenuItem { Tag: string tag } && int.TryParse(tag, out int index))
        {
            ShowPage(index);
        }
    }

    /// <summary>按索引显示页面：首次访问时创建并缓存，之后直接复用实例。</summary>
    private void ShowPage(int index)
    {
        if (index < 0 || index >= _pageFactories.Length)
        {
            return;
        }
        if (!_pageCache.TryGetValue(index, out UserControl page))
        {
            page = _pageFactories[index]();
            _pageCache[index] = page;
        }
        PageHost.Content = page;
    }
}
