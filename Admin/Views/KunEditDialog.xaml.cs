using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using HandyControl.Controls;
using me.cqp.luohuaming.iKun.Domain.Models;
using me.cqp.luohuaming.iKun.Domain.PetAttributes;
using me.cqp.luohuaming.iKun.Infrastructure;
using me.cqp.luohuaming.iKun.Infrastructure.Logging;
using static me.cqp.luohuaming.iKun.Admin.DataService;

namespace me.cqp.luohuaming.iKun.Admin.Views;

/// <summary>
/// 管理面板——编辑鲲对话框（模态）。
/// 在本地 Kun 副本上编辑，点击"保存"校验通过后经 DataService.SaveKun 落库；
/// 只写入所选鲲自身的行，不触碰其他玩家数据。
/// </summary>
public partial class KunEditDialog
{
    private static readonly Log Log = Log.For("管理面板");

    /// <summary>主词缀展示名（与 Element 枚举 0..9 一一对应：无/金/木/水/火/土/风/雷/阴/阳）</summary>
    private static readonly string[] MainAffixNames = ["无", "金", "木", "水", "火", "土", "风", "雷", "阴", "阳"];

    private static readonly Brush HintNormalBrush = CreateFrozenBrush(0x88, 0x88, 0x88);
    private static readonly Brush HintWarnBrush = CreateFrozenBrush(0xE7, 0x4C, 0x3C);

    /// <summary>下拉选项：词缀编号 + 展示名</summary>
    private sealed class AffixOption
    {
        public AffixOption(int id, string display)
        {
            Id = id;
            Display = display;
        }

        public int Id { get; }

        public string Display { get; }

        public override string ToString() => Display;
    }

    /// <summary>鲲实体本地副本（编辑只改副本，保存时才落库）</summary>
    private readonly Kun _kun;

    /// <summary>预填完成后才启用实时校验提示</summary>
    private bool _prefilled;

    /// <summary>是否已确认保存（ShowDialog 返回后可读）</summary>
    public bool Accepted => DialogResult == true;

    public KunEditDialog(KunRow row)
    {
        InitializeComponent();

        // 拷贝标量字段到本地副本（Kun 为 sealed 类，逐字段拷贝）
        Kun src = row.Kun;
        _kun = new Kun
        {
            Id = src.Id,
            Abandoned = src.Abandoned,
            Alive = src.Alive,
            AttributeAID = src.AttributeAID,
            AttributeBID = src.AttributeBID,
            AttributeCID = src.AttributeCID,
            CanResurrect = src.CanResurrect,
            Level = src.Level,
            PlayerID = src.PlayerID,
            ResurrectCount = src.ResurrectCount,
            Weight = src.Weight,
            DeadAt = src.DeadAt,
            NickName = src.NickName,
            AscendBonusPercent = src.AscendBonusPercent,
        };

        InitAffixCombos();
        Prefill();
        _prefilled = true;
        // 预填完成后同步实时上限提示与死亡时间控件启用状态
        UpdateLimitHint();
        SyncDeadFieldsEnabled();
    }

    /// <summary>模态编辑入口：owner 为主窗口，返回 true 表示已保存</summary>
    public static bool? EditFor(KunRow row, System.Windows.Window owner)
    {
        var dialog = new KunEditDialog(row);
        dialog.Owner = owner;
        return dialog.ShowDialog();
    }

    /// <summary>
    /// 构建词缀下拉项：
    /// 主词缀 0..9（无/金/木/水/火/土/风/雷/阴/阳）；
    /// 副词缀 0=无(默认)，1..80 名称取自 Affix 编号表（编号 79 "无" 显示为"无属性"）。
    /// </summary>
    private void InitAffixCombos()
    {
        MainCombo.ItemsSource = MainAffixNames
            .Select((name, id) => new AffixOption(id, $"{id} {name}"))
            .ToList();

        var subOptions = new List<AffixOption> { new(0, "0 无(默认)") };
        for (int id = 1; id <= 80; id++)
        {
            string name = new Affix(id).Name;
            if (string.IsNullOrEmpty(name))
            {
                name = "无属性";
            }
            subOptions.Add(new AffixOption(id, $"{id} {name}"));
        }
        SubBCombo.ItemsSource = subOptions;
        SubCCombo.ItemsSource = subOptions;
    }

    /// <summary>用副本字段预填所有控件</summary>
    private void Prefill()
    {
        LevelBox.Text = _kun.Level.ToString(CultureInfo.InvariantCulture);
        WeightBox.Text = _kun.Weight.ToString("0.##########", CultureInfo.InvariantCulture);
        AliveCheck.IsChecked = _kun.Alive;
        AbandonedCheck.IsChecked = _kun.Abandoned;
        CanResurrectCheck.IsChecked = _kun.CanResurrect;
        ResurrectCountBox.Text = _kun.ResurrectCount.ToString(CultureInfo.InvariantCulture);
        NickNameBox.Text = _kun.NickName ?? "";

        if (_kun.DeadAt != default)
        {
            DeadDate.SelectedDate = _kun.DeadAt.Date;
            DeadTimeBox.Text = _kun.DeadAt.ToString("HH:mm", CultureInfo.InvariantCulture);
        }

        SelectOption(MainCombo, _kun.AttributeAID);
        SelectOption(SubBCombo, _kun.AttributeBID);
        SelectOption(SubCCombo, _kun.AttributeCID);
    }

    private static void SelectOption(System.Windows.Controls.ComboBox combo, int id)
    {
        foreach (object item in combo.Items)
        {
            if (item is AffixOption opt && opt.Id == id)
            {
                combo.SelectedItem = item;
                return;
            }
        }
        // 编号越界（脏数据），回退第一项"无"
        combo.SelectedIndex = 0;
    }

    private void LevelOrWeight_Changed(object sender, TextChangedEventArgs e)
    {
        UpdateLimitHint();
    }

    private void AliveCheck_Changed(object sender, RoutedEventArgs e)
    {
        if (!_prefilled)
        {
            return;
        }
        SyncDeadFieldsEnabled();
    }

    /// <summary>死亡时间仅在鲲已死亡时有意义：同步日期/时间控件启用状态</summary>
    private void SyncDeadFieldsEnabled()
    {
        bool dead = AliveCheck.IsChecked != true;
        DeadDate.IsEnabled = dead;
        DeadTimeBox.IsEnabled = dead;
    }

    /// <summary>实时更新体重上限提示："上限 10^Level = X"，超限时红色警告</summary>
    private void UpdateLimitHint()
    {
        if (!_prefilled)
        {
            return;
        }
        bool levelOk = int.TryParse(LevelBox.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int level)
                       && level >= 0;
        bool weightOk = TryParseDouble(WeightBox.Text, out double weight);
        if (!levelOk)
        {
            LimitHint.Text = "等级需为 ≥0 的整数";
            LimitHint.Foreground = HintWarnBrush;
            return;
        }
        double limit = Kun.WeightLimitOf(level);
        if (weightOk && weight > limit)
        {
            LimitHint.Text = $"上限 10^{level} = {limit.ToShortNumber()}　⚠ 当前体重已超过上限";
            LimitHint.Foreground = HintWarnBrush;
        }
        else
        {
            LimitHint.Text = $"上限 10^{level} = {limit.ToShortNumber()}";
            LimitHint.Foreground = HintNormalBrush;
        }
    }

    private async void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        // ---- 校验与解析 ----
        if (!int.TryParse(LevelBox.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int level)
            || level < 0)
        {
            Growl.Error("等级需为 ≥0 的整数");
            return;
        }
        if (!TryParseDouble(WeightBox.Text, out double weight) || weight < 0)
        {
            Growl.Error("体重需为 ≥0 的数字");
            return;
        }
        if (!int.TryParse(ResurrectCountBox.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int resurrectCount)
            || resurrectCount < 0)
        {
            Growl.Error("复活次数需为 ≥0 的整数");
            return;
        }

        if (MainCombo.SelectedItem is not AffixOption main
            || SubBCombo.SelectedItem is not AffixOption subB
            || SubCCombo.SelectedItem is not AffixOption subC)
        {
            Growl.Error("请选择词缀");
            return;
        }

        // 死亡时间：日期与时间都留空 → 保持原值不变
        DateTime? deadAt = null;
        DateTime? date = DeadDate.SelectedDate;
        string timeText = (DeadTimeBox.Text ?? "").Trim();
        if (date == null && timeText.Length == 0)
        {
            deadAt = null; // 保持不变
        }
        else
        {
            date ??= DateTime.Today;
            TimeSpan time = TimeSpan.Zero;
            if (timeText.Length > 0
                && !TimeSpan.TryParseExact(timeText, new[] { "hh\\:mm", "h\\:mm" }, CultureInfo.InvariantCulture, out time))
            {
                Growl.Error("死亡时间格式应为 HH:mm，例如 08:30");
                return;
            }
            deadAt = date.Value.Date + time;
        }

        // ---- 写入本地副本 ----
        _kun.Level = level;
        _kun.Weight = weight;
        _kun.Alive = AliveCheck.IsChecked == true;
        _kun.Abandoned = AbandonedCheck.IsChecked == true;
        _kun.CanResurrect = CanResurrectCheck.IsChecked == true;
        _kun.ResurrectCount = resurrectCount;
        _kun.NickName = (NickNameBox.Text ?? "").Trim();
        _kun.AttributeAID = main.Id;
        _kun.AttributeBID = subB.Id;
        _kun.AttributeCID = subC.Id;
        if (deadAt != null)
        {
            _kun.DeadAt = deadAt.Value;
        }

        SaveButton.IsEnabled = false;
        CancelButton.IsEnabled = false;
        try
        {
            Kun kun = _kun;
            bool alive = kun.Alive;
            int id = kun.Id;
            // 落库放后台线程，避免阻塞 UI
            await Task.Run(() => SaveKun(kun));
            Log.Info($"管理面板: 已保存鲲 {id}（等级 {level}，体重 {weight}，存活 {alive}）");
            Growl.Success($"鲲 {id} 已保存");
            DialogResult = true;
            Close();
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"管理面板: 保存鲲 {_kun.Id} 失败");
            Growl.Error("保存失败: " + ex.Message);
            SaveButton.IsEnabled = true;
            CancelButton.IsEnabled = true;
        }
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    /// <summary>兼容不变/当前文化的十进制解析</summary>
    private static bool TryParseDouble(string text, out double value)
    {
        text = (text ?? "").Trim();
        return double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value)
               || double.TryParse(text, NumberStyles.Float, CultureInfo.CurrentCulture, out value);
    }

    private static Brush CreateFrozenBrush(byte r, byte g, byte b)
    {
        var brush = new SolidColorBrush(Color.FromRgb(r, g, b));
        brush.Freeze();
        return brush;
    }
}
