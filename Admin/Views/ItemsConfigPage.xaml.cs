using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using HandyControl.Controls;
using me.cqp.luohuaming.iKun.Domain.Configuration;

// HandyControl 无 Expander 类（3.5.1 中为原生控件 + 主题样式），此处 TextBox 用 hc: 前缀
using HcTextBox = HandyControl.Controls.TextBox;

namespace me.cqp.luohuaming.iKun.Admin.Views;

/// <summary>
/// 物品配置页：编辑 Items.json 中 21 个键（9 名称 + 9 描述 + 3 使用文案）。
/// 名称/描述为单行 TextBox，使用文案为多行 TextBox。保存经 ConfigEditor.TrySaveItems。
/// </summary>
public partial class ItemsConfigPage : UserControl
{
    /// <summary>（分组，JSON 键，界面标签，是否多行，读取当前值）</summary>
    private static readonly (string Group, string Key, string Label, bool MultiLine, Func<ItemConfiguration, string> Get)[] Fields =
    {
        // 名称
        ("名称", "CoinName", "金币", false, c => c.CoinName),
        ("名称", "KunEggName", "鲲之蛋", false, c => c.KunEggName),
        ("名称", "BlindBoxName", "盲盒", false, c => c.BlindBoxName),
        ("名称", "ResurrectPillName", "复活丸", false, c => c.ResurrectPillName),
        ("名称", "TransmogrifyPillName", "幻化丸", false, c => c.TransmogrifyPillName),
        ("名称", "UpgradePillName", "强化丸", false, c => c.UpgradePillName),
        ("名称", "AscendPillName", "渡劫丹", false, c => c.AscendPillName),
        ("名称", "LevelPillName", "快速等级丹", false, c => c.LevelPillName),
        ("名称", "WeightPillName", "快速体重丹", false, c => c.WeightPillName),
        // 描述
        ("描述", "CoinDescription", "金币", false, c => c.CoinDescription),
        ("描述", "KunEggDescription", "鲲之蛋", false, c => c.KunEggDescription),
        ("描述", "BlindBoxDescription", "盲盒", false, c => c.BlindBoxDescription),
        ("描述", "ResurrectPillDescription", "复活丸", false, c => c.ResurrectPillDescription),
        ("描述", "TransmogrifyPillDescription", "幻化丸", false, c => c.TransmogrifyPillDescription),
        ("描述", "UpgradePillDescription", "强化丸", false, c => c.UpgradePillDescription),
        ("描述", "AscendPillDescription", "渡劫丹", false, c => c.AscendPillDescription),
        ("描述", "LevelPillDescription", "快速等级丹", false, c => c.LevelPillDescription),
        ("描述", "WeightPillDescription", "快速体重丹", false, c => c.WeightPillDescription),
        // 使用文案
        ("使用文案", "UseItemException", "使用物品发生异常", true, c => c.UseItemException),
        ("使用文案", "UseLevelPill", "快速等级丹使用结果（{0} 个数 {1} 提升 {2} 当前等级）", true, c => c.UseLevelPill),
        ("使用文案", "UseWeightPill", "快速体重丹使用结果（{0} 当前体重）", true, c => c.UseWeightPill),
    };

    private readonly Dictionary<string, HcTextBox> _boxes = new();

    public ItemsConfigPage()
    {
        InitializeComponent();
        BuildGroups();
        ReloadValues();
    }

    private void BuildGroups()
    {
        string currentGroup = null;
        StackPanel groupContent = null;

        foreach (var (group, key, label, multiLine, _) in Fields)
        {
            if (group != currentGroup)
            {
                currentGroup = group;
                groupContent = new StackPanel { Margin = new Thickness(8, 4, 4, 4) };
                ContentPanel.Children.Add(new Expander
                {
                    Header = group,
                    IsExpanded = true,
                    Margin = new Thickness(0, 0, 0, 6),
                    Content = groupContent,
                });
            }

            var box = new HcTextBox
            {
                AcceptsReturn = multiLine,
                TextWrapping = TextWrapping.Wrap,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Height = multiLine ? 60 : 28,
                Margin = new Thickness(0, 4, 0, 10),
            };
            _boxes[key] = box;

            var row = new StackPanel { Margin = new Thickness(2) };
            row.Children.Add(new TextBlock { Text = label, Margin = new Thickness(2, 2, 0, 0) });
            row.Children.Add(box);
            groupContent.Children.Add(row);
        }
    }

    /// <summary>把当前配置快照重新读入所有输入框（热重载后刷新用）</summary>
    private void ReloadValues()
    {
        var config = ItemConfiguration.Current;
        if (config is null)
        {
            return;
        }

        foreach (var (_, key, _, _, get) in Fields)
        {
            _boxes[key].Text = get(config) ?? string.Empty;
        }
    }

    private void ReloadButton_Click(object sender, RoutedEventArgs e)
    {
        ReloadValues();
        Growl.Info("已重新加载当前配置");
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        var values = new Dictionary<string, object>(Fields.Length);
        foreach (var (_, key, _, _, _) in Fields)
        {
            values[key] = _boxes[key].Text ?? string.Empty;
        }

        if (ConfigEditor.TrySaveItems(values, out var error))
        {
            Growl.Success("已保存，插件将自动热重载生效");
        }
        else
        {
            Growl.Error(error);
        }
    }
}
