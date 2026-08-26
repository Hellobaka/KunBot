using System.Threading;
using System.Windows;
using Another_Mirai_Native.Abstractions.Attributes;
using Another_Mirai_Native.Abstractions.Context;
using Another_Mirai_Native.Abstractions.Handlers;
using me.cqp.luohuaming.iKun.Infrastructure.Logging;

namespace me.cqp.luohuaming.iKun.Admin;

/// <summary>
/// 框架菜单入口：点击"iKun管理面板"菜单项时由框架 UI 线程调用 OnMenu，
/// 在独立的 STA 线程上创建/复用 WPF 主窗口（llms.txt §8.4 WPF-in-plugin 模式）。
/// 框架通过反射实例化本类，共享状态全部使用静态成员。
/// </summary>
[Menu("iKun管理面板")]
public class AdminWindowEntry : IMenuHandler
{
    private static readonly Log Log = Log.For("管理面板");

    private static Application _app;
    private static AdminWindow _window;
    private static Thread _uiThread;
    private static volatile bool _shuttingDown;

    public void OnMenu(MenuContext e)
    {
        if (_shuttingDown)
        {
            Log.Warn("插件正在卸载，忽略本次菜单点击");
            return;
        }

        if (_window == null)
        {
            using var ready = new ManualResetEventSlim(false);
            _uiThread = new Thread(() => CreateUi(ready))
            { IsBackground = true };
            _uiThread.SetApartmentState(ApartmentState.STA);
            _uiThread.Start();
            ready.Wait();
            Log.Info("管理窗口线程已启动");
        }

        if (_window == null)
        {
            Log.Warn("管理窗口不可用（主题加载失败？），本次点击忽略");
            return;
        }

        _window.Dispatcher.Invoke(() => { _window.Show(); _window.Activate(); });
    }

    /// <summary>
    /// UI 线程主体：创建 Application、合并 HandyControl 主题、创建主窗口，
    /// 成功后用 app.Run() 驱动 Dispatcher。任何路径下 finally 都会 ready.Set()，
    /// 保证调用方（框架 UI 线程）不会永久阻塞。
    /// </summary>
    private static void CreateUi(ManualResetEventSlim ready)
    {
        try
        {
            // 1) Application 实例（HandyControl 依赖 Application.Current 找主题资源）
            _app = new Application();

            // 2) 合并 HandyControl 主题（官方 App.xaml 顺序：Theme 在前、Skin 在后）。
            //    AMN 构建流水线用 ILRepack 把 HandyControl 合并进本程序集（identity 仍为 iKun）：
            //    主题 BAML 存入 iKun.g.resources 且键带 "handycontrol/" 前缀，独立的
            //    "HandyControl" 程序集不复存在——首选合并后的 pack URI；
            //    同时保留原程序集名候选作为后备（部署未合并时）。
            //    注意：合并后 SkinDefault.xaml 内部的跳转仍指向不存在的 "HandyControl" 程序集，
            //    因此直接加载其跳转目标 Colors.xaml（官方 Skin 的等价内容）。
            bool themeOk = AddTheme(_app, "HandyControl/Themes/Theme.xaml", "iKun")
                         || AddTheme(_app, "Themes/Theme.xaml", "HandyControl");
            bool skinOk = AddTheme(_app, "HandyControl/Themes/Basic/Colors/Colors.xaml", "iKun")
                        || AddTheme(_app, "Themes/SkinDefault.xaml", "HandyControl");
            if (!themeOk || !skinOk)
            {
                Log.Error($"HandyControl 主题资源加载失败 theme={themeOk} skin={skinOk}，放弃创建管理窗口。请检查 ILRepack 合并后的资源。");
                try { _app.Shutdown(); } catch { }
                _app = null;
                return;
            }

            // 3) 创建主窗口（关窗=隐藏，实例复用）
            _window = new AdminWindow();
            _window.Closing += OnWindowClosing;
            Log.Info("管理窗口已创建");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "创建管理窗口失败");
            _app = null;
        }
        finally
        {
            ready.Set();
        }

        // 4) 仅当窗口创建成功才驱动 Dispatcher（失败路径 _app 已置空，线程直接结束）。
        //    Application 生命周期与线程绑定，插件卸载时由 Shutdown() 结束。
        _app?.Run();
    }

    private static void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        if (_shuttingDown)
        {
            return; // 插件卸载时允许真正关闭，避免 app.Shutdown() 被取消关窗卡住
        }
        e.Cancel = true;
        _window?.Hide();
    }

    /// <summary>
    /// 在 Application 资源中尝试加载主题资源字典。
    /// 采用 WPF pack URI 绝对形式：pack://application:,,,/{程序集名};component/{路径}
    /// （.xaml 后缀由 WPF 自动映射到 .g.resources 中的 .baml 条目，查找时不区分大小写）。
    /// </summary>
    private static bool AddTheme(Application app, string resourcePath, string assemblyName)
    {
        try
        {
            var dict = new ResourceDictionary
            {
                Source = new Uri($"pack://application:,,,/{assemblyName};component/{resourcePath}", UriKind.Absolute)
            };
            app.Resources.MergedDictionaries.Add(dict);
            Log.Info($"主题资源已加载: {assemblyName}/{resourcePath}");
            return true;
        }
        catch (Exception ex)
        {
            Log.Warn($"主题资源加载失败: {assemblyName}/{resourcePath}: {ex.Message}");
            return false;
        }
    }

    /// <summary>插件卸载时由 Entry.OnDisableAsync 调用：关闭窗口并结束 Dispatcher 线程。</summary>
    public static void Shutdown()
    {
        _shuttingDown = true;
        var app = _app;
        if (app == null)
        {
            return;
        }
        try
        {
            app.Dispatcher.Invoke(() => app.Shutdown(), TimeSpan.FromSeconds(3));
        }
        catch (Exception ex)
        {
            Log.Warn($"关闭管理窗口异常: {ex.Message}");
        }
        _app = null;
        _window = null;
        _uiThread = null;
        Log.Info("管理窗口已关闭");
    }
}
