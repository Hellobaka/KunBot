using System.ComponentModel;
using System.Globalization;
using System.Threading;
using HandyControl.Controls;
using me.cqp.luohuaming.iKun.Domain.Configuration;
using me.cqp.luohuaming.iKun.Domain.Enums;
using me.cqp.luohuaming.iKun.Domain.Models;
using me.cqp.luohuaming.iKun.Domain.PetAttributes;
using me.cqp.luohuaming.iKun.Infrastructure;
using me.cqp.luohuaming.iKun.Infrastructure.Logging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace me.cqp.luohuaming.iKun.Admin.Views;

/// <summary>
/// 管理面板 - 玩家管理页：
/// 左侧玩家列表（QQ 筛选/刷新），右侧详情（签到/冷却/渡劫丹可编辑、背包数量可编辑、名下鲲只读），
/// 支持保存修改与级联删除玩家。所有数据库访问经 DataService 且在后台线程执行。
/// </summary>
public partial class PlayerPage : UserControl
{
    private static readonly Log Log = Log.For("管理面板");

    private const string TimestampFormat = "yyyy-MM-dd HH:mm:ss";

    private List<DataService.PlayerSummary> _allPlayers = new();
    private long? _selectedQq;
    private DataService.PlayerDetail? _currentDetail;
    private List<InventoryDisplayRow> _inventoryRows = new();
    private List<int> _inventoryOriginalCounts = new();
    private int _detailLoadToken;
    private bool _listLoaded;
    private bool _busy;

    public PlayerPage()
    {
        InitializeComponent();
    }

    // ---- 玩家列表 ----

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (_listLoaded) return;
        _listLoaded = true;
        await LoadListAsync();
    }

    /// <summary>重载玩家列表（后台线程访问数据库），完成后尽量恢复原选中项</summary>
    private async Task LoadListAsync()
    {
        long? restoreQq = _selectedQq;
        try
        {
            var list = await Task.Run(() => DataService.ListPlayers());
            _allPlayers = list ?? new List<DataService.PlayerSummary>();
            ApplyFilter();
            if (restoreQq is long qq)
            {
                var row = _allPlayers.FirstOrDefault(p => p.QQ == qq);
                if (row != null)
                {
                    PlayerGrid.SelectedItem = row;
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "加载玩家列表失败");
            Growl.Error("加载玩家列表失败：" + ex.Message);
        }
    }

    /// <summary>按 QQ 子串即时筛选列表</summary>
    private void ApplyFilter()
    {
        string keyword = SearchBox.Text?.Trim() ?? "";
        var view = keyword.Length == 0
            ? _allPlayers
            : _allPlayers.Where(p => p.QQ.ToString().Contains(keyword, StringComparison.Ordinal)).ToList();
        PlayerGrid.ItemsSource = view;
    }

    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e) => ApplyFilter();

    private async void RefreshButton_Click(object sender, RoutedEventArgs e) => await LoadListAsync();

    private async void PlayerGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (PlayerGrid.SelectedItem is DataService.PlayerSummary summary)
        {
            _selectedQq = summary.QQ;
            await LoadDetailAsync(summary.QQ);
        }
        else
        {
            _selectedQq = null;
            ShowEmptyDetail();
        }
    }

    // ---- 玩家详情 ----

    /// <summary>加载所选玩家详情（后台线程访问数据库；token 防止快速切换时旧结果覆盖新结果）</summary>
    private async Task LoadDetailAsync(long qq)
    {
        int token = Interlocked.Increment(ref _detailLoadToken);
        try
        {
            var detail = await Task.Run(() => DataService.GetPlayer(qq));
            if (detail is null || token != Volatile.Read(ref _detailLoadToken)) return;
            if (detail.Player is null)
            {
                // 玩家不存在（可能刚被删除）：清空详情并刷新列表
                Log.Warn($"玩家 {qq} 不存在或已被删除");
                Growl.Warning($"玩家 {qq} 不存在或已被删除");
                _selectedQq = null;
                PlayerGrid.SelectedItem = null;
                ShowEmptyDetail();
                await LoadListAsync();
                return;
            }
            _currentDetail = detail;
            PopulateDetail(qq, detail);
        }
        catch (Exception ex)
        {
            if (token != Volatile.Read(ref _detailLoadToken)) return;
            Log.Error(ex, $"加载玩家 {qq} 详情失败");
            Growl.Error($"加载玩家 {qq} 详情失败：" + ex.Message);
        }
    }

    private void PopulateDetail(long qq, DataService.PlayerDetail detail)
    {
        Player p = detail.Player;
        DetailHeader.Text = $"玩家 {qq}　注册时间：{FormatTimestamp(p.CreateAt)}";
        LoginAtBox.Text = FormatTimestampInput(p.LoginAt);
        AttackAtBox.Text = FormatTimestampInput(p.AttackAt);
        DevourAtBox.Text = FormatTimestampInput(p.DevourAt);
        AscendPillBox.Text = p.AscendPillComsume.ToString();

        // 背包：金币与鲲蛋行恒显示（数量为 0 也显示）
        var rows = (detail.Items ?? new List<DataService.ItemEntry>())
            .Where(x => x != null && x.Item != null && !x.Item.Deleted)
            .Select(x => new InventoryDisplayRow(
                x.Item,
                string.IsNullOrWhiteSpace(x.DisplayName) ? ItemDisplayName(x.Item.ItemID) : x.DisplayName))
            .ToList();
        if (rows.All(r => r.Source.ItemID != (int)ItemId.Coin))
        {
            rows.Add(new InventoryDisplayRow(
                new InventoryItem { PlayerID = qq, ItemID = (int)ItemId.Coin, Count = 0, Deleted = false },
                ItemDisplayName((int)ItemId.Coin)));
        }
        if (rows.All(r => r.Source.ItemID != (int)ItemId.KunEgg))
        {
            rows.Add(new InventoryDisplayRow(
                new InventoryItem { PlayerID = qq, ItemID = (int)ItemId.KunEgg, Count = 0, Deleted = false },
                ItemDisplayName((int)ItemId.KunEgg)));
        }
        _inventoryRows = rows;
        _inventoryOriginalCounts = rows.Select(r => r.Count).ToList();
        InventoryGrid.ItemsSource = rows;

        KunGrid.ItemsSource = (detail.Kuns ?? new List<Kun>())
            .Where(k => k != null)
            .Select(k => new KunDisplayRow
            {
                Level = k.Level,
                Weight = FormatWeight(k.Weight),
                MainAffix = ResolveAffixName(true, k.AttributeAID),
                Affix1 = ResolveAffixName(false, k.AttributeBID),
                Affix2 = ResolveAffixName(false, k.AttributeCID),
                Alive = k.Alive ? "是" : "否",
                Abandoned = k.Abandoned ? "是" : "否",
                CanResurrect = k.CanResurrect ? "是" : "否",
                DeadAt = FormatTimestamp(k.DeadAt),
                NickName = string.IsNullOrWhiteSpace(k.NickName) ? "—" : k.NickName,
            })
            .ToList();

        EmptyHint.Visibility = Visibility.Collapsed;
        DetailHost.Visibility = Visibility.Visible;
    }

    private void ShowEmptyDetail()
    {
        _currentDetail = null;
        _inventoryRows = new List<InventoryDisplayRow>();
        _inventoryOriginalCounts = new List<int>();
        InventoryGrid.ItemsSource = null;
        KunGrid.ItemsSource = null;
        DetailHost.Visibility = Visibility.Collapsed;
        EmptyHint.Visibility = Visibility.Visible;
    }

    // ---- 保存 / 删除 ----

    private async void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        if (_busy) return;
        if (PlayerGrid.SelectedItem is not DataService.PlayerSummary summary)
        {
            Growl.Warning("请先在左侧列表中选择一个玩家");
            return;
        }
        long qq = summary.QQ;
        if (_currentDetail is null || _currentDetail.Player.QQ != qq)
        {
            Growl.Warning("玩家详情尚未加载完成，请稍候再保存");
            return;
        }
        if (!TryParseTimestamp(LoginAtBox.Text, out DateTime loginAt) ||
            !TryParseTimestamp(AttackAtBox.Text, out DateTime attackAt) ||
            !TryParseTimestamp(DevourAtBox.Text, out DateTime devourAt))
        {
            Growl.Warning("时间格式不正确，应为 yyyy-MM-dd HH:mm:ss，留空表示清空");
            return;
        }
        if (!int.TryParse(AscendPillBox.Text.Trim(), out int pills) || pills < 0)
        {
            Growl.Warning("挂起渡劫丹数量必须是不小于 0 的整数");
            return;
        }

        _busy = true;
        SetButtonsEnabled(false);
        try
        {
            await Task.Run(() =>
            {
                Player p = _currentDetail.Player;
                p.LoginAt = loginAt;
                p.AttackAt = attackAt;
                p.DevourAt = devourAt;
                p.AscendPillComsume = pills;
                DataService.SavePlayer(p);

                // 背包：仅保存数量发生变化的行
                for (int i = 0; i < _inventoryRows.Count; i++)
                {
                    if (_inventoryRows[i].Count != _inventoryOriginalCounts[i])
                    {
                        if (_inventoryRows[i].Count < 0) _inventoryRows[i].Count = 0;
                        DataService.SaveInventoryItem(_inventoryRows[i].Source);
                    }
                }
            });
            Log.Info($"管理面板保存玩家 {qq} 修改");
            Growl.Success($"玩家 {qq} 的修改已保存");
            // 保存后重载详情与列表，确保数字刷新
            await LoadListAsync();
            await LoadDetailAsync(qq);
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"保存玩家 {qq} 修改失败");
            Growl.Error("保存失败：" + ex.Message);
        }
        finally
        {
            _busy = false;
            SetButtonsEnabled(true);
        }
    }

    private async void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        if (_busy) return;
        if (PlayerGrid.SelectedItem is not DataService.PlayerSummary summary)
        {
            Growl.Warning("请先在左侧列表中选择一个玩家");
            return;
        }
        long qq = summary.QQ;

        // 双重确认：先确认删除玩家本身，再确认级联删除的不可恢复性
        MessageBoxResult first = System.Windows.MessageBox.Show(
            $"确认删除玩家 {qq}？此操作不可恢复！",
            "确认删除玩家",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);
        if (first != MessageBoxResult.Yes) return;
        MessageBoxResult second = System.Windows.MessageBox.Show(
            $"最终确认：将同时永久删除玩家 {qq} 名下的所有鲲、背包物品与记录。\n\n仍要继续删除吗？",
            "再次确认",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);
        if (second != MessageBoxResult.Yes) return;

        _busy = true;
        SetButtonsEnabled(false);
        try
        {
            await Task.Run(() => DataService.DeletePlayer(qq));
            Log.Info($"管理面板删除玩家 {qq}（级联删除鲲/背包/记录）");
            Growl.Success($"玩家 {qq} 已删除");
            _selectedQq = null;
            _currentDetail = null;
            PlayerGrid.SelectedItem = null;
            ShowEmptyDetail();
            await LoadListAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"删除玩家 {qq} 失败");
            Growl.Error("删除失败：" + ex.Message);
        }
        finally
        {
            _busy = false;
            SetButtonsEnabled(true);
        }
    }

    private void SetButtonsEnabled(bool enabled)
    {
        SaveButton.IsEnabled = enabled;
        DeleteButton.IsEnabled = enabled;
        RefreshButton.IsEnabled = enabled;
    }

    // ---- 显示辅助 ----

    /// <summary>体重格式化：复用 Extensions.ToShortNumber 缩写风格，异常时回退原始值</summary>
    private static string FormatWeight(double weight)
    {
        try { return Extensions.ToShortNumber(weight); }
        catch { return weight.ToString("f2"); }
    }

    /// <summary>词缀显示名：主词缀按元素、副词缀按编号解析；空名显示「无」，异常显示「未知」</summary>
    private static string ResolveAffixName(bool isMain, int storedId)
    {
        try
        {
            string name = PetAttributeFactory.FromStoredId(isMain, storedId).Name;
            return string.IsNullOrWhiteSpace(name) ? "无" : name;
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"解析{(isMain ? "主" : "副")}词缀 ID={storedId} 失败");
            return "未知";
        }
    }

    /// <summary>物品显示名：优先读取配置名称，异常时回退默认文案</summary>
    private static string ItemDisplayName(int itemId)
    {
        try
        {
            return ItemCatalog.Definition((ItemId)itemId)?.Name ?? itemId.ToString();
        }
        catch
        {
            return itemId switch
            {
                (int)ItemId.Coin => "金币",
                (int)ItemId.KunEgg => "鲲之蛋",
                _ => itemId.ToString(),
            };
        }
    }

    private static string FormatTimestamp(DateTime value) => value == default ? "—" : value.ToString(TimestampFormat);

    private static string FormatTimestampInput(DateTime value) => value == default ? "" : value.ToString(TimestampFormat);

    private static bool TryParseTimestamp(string text, out DateTime value)
    {
        text = (text ?? "").Trim();
        if (text.Length == 0)
        {
            value = default;
            return true;
        }
        return DateTime.TryParseExact(text, TimestampFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out value);
    }
}

/// <summary>背包条目展示行：名称只读，数量双向绑定并写回原 InventoryItem</summary>
public sealed class InventoryDisplayRow
{
    private readonly InventoryItem _item;

    public InventoryDisplayRow(InventoryItem item, string name)
    {
        _item = item;
        Name = name;
    }

    /// <summary>原始背包实体（保存时使用）</summary>
    public InventoryItem Source => _item;

    public string Name { get; }

    public int Count
    {
        get => _item.Count;
        set => _item.Count = value;
    }
}

/// <summary>名下鲲只读展示行（词缀/体重/时间已预格式化为文本）</summary>
public sealed class KunDisplayRow
{
    public int Level { get; init; }
    public string Weight { get; init; } = "";
    public string MainAffix { get; init; } = "";
    public string Affix1 { get; init; } = "";
    public string Affix2 { get; init; } = "";
    public string Alive { get; init; } = "";
    public string Abandoned { get; init; } = "";
    public string CanResurrect { get; init; } = "";
    public string DeadAt { get; init; } = "";
    public string NickName { get; init; } = "";
}

/// <summary>DateTime 显示转换：空/MinValue → 「—」，否则 yyyy-MM-dd HH:mm:ss</summary>
public sealed class DateTimeDashConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is DateTime dt && dt != default ? dt.ToString("yyyy-MM-dd HH:mm:ss") : "—";

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

/// <summary>大数显示转换：计数类数值超过 10 万时按「万/亿/兆…」缩写（风格同 Extensions.ToShortNumber，但不做体重单位换算）</summary>
public sealed class BigNumberConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is int i ? NumberFormat.FormatBigNumber(i)
        : value is long l ? NumberFormat.FormatBigNumber(l)
        : value?.ToString() ?? "";

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

/// <summary>计数类大数缩写（10 万以下原样显示）</summary>
public static class NumberFormat
{
    public static string FormatBigNumber(long value)
    {
        if (value < 100_000) return value.ToString();
        try
        {
            var style = CoreConfiguration.Current?.ShortNumberStyle ?? ShortNumberStyle.Normal;
            if (style == ShortNumberStyle.Science)
            {
                double d = value;
                return d <= 1_000_000 ? d.ToString("f2") : d.ToString("E2");
            }
            string[] units = ["万", "亿", "兆", "京", "垓", "秭", "穰", "沟", "涧", "正", "载", "极"];
            double v = Math.Abs((double)value);
            int index = -1;
            while (v > 10000 && index < units.Length)
            {
                v /= 10000;
                if (v > 1) index++;
            }
            string prefix = value < 0 ? "-" : "";
            return index < 0 ? prefix + v.ToString("f2") : prefix + v.ToString("f2") + units[Math.Min(index, units.Length - 1)];
        }
        catch
        {
            return value.ToString();
        }
    }
}
