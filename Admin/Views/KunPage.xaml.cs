using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using HandyControl.Controls;
using me.cqp.luohuaming.iKun.Domain.Models;
using me.cqp.luohuaming.iKun.Infrastructure;
using me.cqp.luohuaming.iKun.Infrastructure.Logging;
using static me.cqp.luohuaming.iKun.Admin.DataService;

namespace me.cqp.luohuaming.iKun.Admin.Views;

/// <summary>
/// 管理面板——鲲管理页：列表展示、搜索/状态过滤、编辑、删除。
/// 所有数据库访问经 DataService；列表查询在后台线程执行，避免阻塞 UI。
/// </summary>
public partial class KunPage
{
    private static readonly Log Log = Log.For("管理面板");

    /// <summary>全部鲲（未过滤）</summary>
    private List<KunRowDisplay> _allRows = new();

    /// <summary>带过滤器的视图（.NET 10 起 ICollectionView 位于 System.ComponentModel）</summary>
    private System.ComponentModel.ICollectionView _view;

    /// <summary>搜索关键字（归属QQ / 群 / 昵称 / 词缀名）</summary>
    private string _searchText = "";

    /// <summary>是否正在加载（防重复触发）</summary>
    private bool _isLoading;

    public KunPage()
    {
        InitializeComponent();
        StatusCombo.ItemsSource = new[] { "全部", "存活", "死亡", "弃置", "可复活" };
        StatusCombo.SelectedIndex = 0;
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _ = LoadAsync();
    }

    /// <summary>后台线程查询数据库并刷新列表</summary>
    private async Task LoadAsync()
    {
        if (_isLoading)
        {
            return;
        }
        _isLoading = true;
        RefreshButton.IsEnabled = false;
        try
        {
            List<KunRow> rows = await Task.Run(() => ListKuns());
            _allRows = rows.Select(r => new KunRowDisplay(r)).ToList();
            _view = new ListCollectionView(_allRows);
            _view.Filter = FilterPredicate;
            KunGrid.ItemsSource = _view;
            _view.Refresh();
            UpdateCount();
            Log.Info($"管理面板: 加载鲲列表成功，共 {_allRows.Count} 条");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "管理面板: 加载鲲列表失败");
            Growl.Error("加载鲲列表失败: " + ex.Message);
        }
        finally
        {
            _isLoading = false;
            RefreshButton.IsEnabled = true;
        }
    }

    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        _searchText = SearchBox.Text ?? "";
        _view?.Refresh();
        UpdateCount();
    }

    private void StatusCombo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        _view?.Refresh();
        UpdateCount();
    }

    private void RefreshButton_Click(object sender, RoutedEventArgs e)
    {
        _ = LoadAsync();
    }

    private void EditButton_Click(object sender, RoutedEventArgs e)
    {
        var d = KunGrid.SelectedItem as KunRowDisplay;
        if (d == null)
        {
            Growl.Warning("请先在列表中选择一条鲲");
            return;
        }
        // 模态对话框；确认保存后刷新列表
        bool? result = KunEditDialog.EditFor(d.Row, System.Windows.Window.GetWindow(this));
        if (result == true)
        {
            _ = LoadAsync();
        }
    }

    private async void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        var d = KunGrid.SelectedItem as KunRowDisplay;
        if (d == null)
        {
            Growl.Warning("请先在列表中选择一条鲲");
            return;
        }

        // 第一次确认：提示要删除的鲲及其归属
        if (System.Windows.MessageBox.Show(
                $"鲲 Id: {d.Id}\n归属QQ: {d.OwnerQQ}\n\n确定要删除这条鲲吗？",
                "删除鲲", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
        {
            return;
        }
        // 第二次确认：不可恢复警告
        if (System.Windows.MessageBox.Show(
                "确认删除? 将同时删除其挂机任务与出群记录。不可恢复!",
                "确认删除", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
        {
            return;
        }

        RefreshButton.IsEnabled = false;
        try
        {
            long kunId = d.Id;
            long ownerQQ = d.OwnerQQ;
            await Task.Run(() => DeleteKun(kunId));
            Log.Info($"管理面板: 已删除鲲 {kunId}（归属QQ {ownerQQ}）");
            Growl.Success($"鲲 {kunId} 已删除");
            await LoadAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"管理面板: 删除鲲 {d.Id} 失败");
            Growl.Error("删除鲲失败: " + ex.Message);
        }
        finally
        {
            RefreshButton.IsEnabled = true;
        }
    }

    /// <summary>过滤条件：状态下拉 + 关键字搜索（均实时生效）</summary>
    private bool FilterPredicate(object obj)
    {
        if (obj is not KunRowDisplay d)
        {
            return false;
        }

        switch (StatusCombo.SelectedIndex)
        {
            case 1:
                if (!d.Kun.Alive) return false;
                break;
            case 2:
                if (d.Kun.Alive) return false;
                break;
            case 3:
                if (!d.Kun.Abandoned) return false;
                break;
            case 4:
                // 可复活 = 当前死亡且可复活
                if (d.Kun.Alive || !d.Kun.CanResurrect) return false;
                break;
        }

        string text = _searchText.Trim();
        if (text.Length > 0)
        {
            bool hit =
                d.OwnerQQ.ToString().IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0 ||
                d.Groups.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0 ||
                (d.Kun.NickName ?? "").IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0 ||
                d.AffixA.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0 ||
                d.AffixB.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0 ||
                d.AffixC.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0;
            if (!hit)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>刷新底部数量提示</summary>
    private void UpdateCount()
    {
        if (_view == null)
        {
            return;
        }
        int filtered = 0;
        foreach (var _ in _view)
        {
            filtered++;
        }
        CountText.Text = filtered == _allRows.Count
            ? $"共 {_allRows.Count} 条"
            : $"共 {_allRows.Count} 条（匹配 {filtered} 条）";
    }
}

/// <summary>
/// 鲲列表展示模型：KunRow 的只读投影，供 DataGrid 直接绑定。
/// 体重/上限走 ToShortNumber 缩写格式；挂机/打工由运行标记+类型合成。
/// </summary>
public sealed class KunRowDisplay
{
    public KunRowDisplay(KunRow row)
    {
        Row = row;
    }

    /// <summary>原始数据行（编辑/删除时取 Row.Kun）</summary>
    public KunRow Row { get; }

    public Kun Kun => Row.Kun;

    public int Id => Kun.Id;

    public string AffixA => string.IsNullOrEmpty(Row.AffixAName) ? "—" : Row.AffixAName;

    public string AffixB => string.IsNullOrEmpty(Row.AffixBName) ? "—" : Row.AffixBName;

    public string AffixC => string.IsNullOrEmpty(Row.AffixCName) ? "—" : Row.AffixCName;

    public int Level => Kun.Level;

    public string WeightText => Kun.Weight.ToShortNumber();

    public string LimitText => Kun.WeightLimitOf(Kun.Level).ToShortNumber();

    public string AliveText => Kun.Alive ? "✓" : "✗";

    public string AbandonedText => Kun.Abandoned ? "✓" : "✗";

    public string CanResurrectText => Kun.CanResurrect ? "✓" : "✗";

    public int ResurrectCount => Kun.ResurrectCount;

    public string DeadAtText =>
        !Kun.Alive && Kun.DeadAt != default
            ? Kun.DeadAt.ToString("yyyy-MM-dd HH:mm")
            : "—";

    public string Groups => string.IsNullOrEmpty(Row.Groups) ? "—" : Row.Groups;

    public long OwnerQQ => Row.OwnerQQ;

    /// <summary>挂机中 / 打工中 / —</summary>
    public string IdleText =>
        Row.AutoPlayRunning
            ? (Row.AutoPlayType == 1 ? "打工中" : "挂机中")
            : "—";

    public string IdleEndText =>
        Row.AutoPlayRunning && Row.AutoPlayEndTime != default
            ? Row.AutoPlayEndTime.ToString("yyyy-MM-dd HH:mm")
            : "—";
}
