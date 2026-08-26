using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using HandyControl.Controls;
using me.cqp.luohuaming.iKun.Domain.Configuration;

// HandyControl 无 Button/CheckBox 类（3.5.1 中为原生控件 + 主题样式），此处 TextBox 用 hc: 前缀
using HcTextBox = HandyControl.Controls.TextBox;

namespace me.cqp.luohuaming.iKun.Admin.Views;

/// <summary>
/// 指令触发词配置页：编辑 Config.json 中 29 个 Command* 键（含旧版键名）。
/// 行由代码按 Fields 表构建（标签 + TextBox 两列 UniformGrid），保存经 ConfigEditor.TrySaveCore。
/// </summary>
public partial class CommandsConfigPage : UserControl
{
    /// <summary>（界面标签，JSON 键，读取当前值）——键名与 CoreConfiguration.Load 中的旧版拼写保持一致</summary>
    private static readonly (string Label, string Key, Func<CoreConfiguration, string> Get)[] Fields =
    {
        ("注册", "CommandRegister", c => c.CommandRegister),
        ("签到", "CommandLogin", c => c.CommandLogin),
        ("菜单", "CommandMenu", c => c.CommandMenu),
        ("我的鲲", "CommandMyKun", c => c.CommandMyKun),
        ("排行", "CommandRanking", c => c.CommandRanking),
        ("群排行", "CommandRankingGroup", c => c.CommandRankingGroup),
        ("背包", "CommandInventory", c => c.CommandInventory),
        ("孵蛋", "CommandHatch", c => c.CommandHatch),
        ("喂养", "CommandFeed", c => c.CommandFeed),
        ("强化", "CommandUpgrade", c => c.CommandUpgrade),
        ("幻化", "CommandTransmogrify", c => c.CommandTransmogrify),
        ("查询已死亡鲲", "CommandQueryDeadKuns", c => c.CommandQueryDeadKuns),
        ("渡劫", "CommandAscend", c => c.CommandAscend),
        ("复活", "CommandResurrect", c => c.CommandResurrect),
        ("放生", "CommandReleaseKun", c => c.CommandRelease),
        ("吞噬", "CommandDevour", c => c.CommandDevour),
        ("攻击", "CommandAttack", c => c.CommandAttack),
        ("购物", "CommandShopping", c => c.CommandShopping),
        ("开鲲蛋", "CommandOpenEgg", c => c.CommandOpenEgg),
        ("开盲盒", "CommandOpenBlindBox", c => c.CommandOpenBlindBox),
        ("开始挂机", "CommandStartAutoPlay", c => c.CommandStartIdle),
        ("停止挂机", "CommandStopAutoPlay", c => c.CommandStopIdle),
        ("开始打工", "CommandStartWorking", c => c.CommandStartWork),
        ("停止打工", "CommandStopWorking", c => c.CommandStopWork),
        ("天罚", "CommandRandomPunish", c => c.CommandRandomPunishInfo),
        ("使用渡劫丹", "CommandConsumeAscendPill", c => c.CommandConsumeAscendPill),
        ("自定义名称", "CommandUseCustomNickName", c => c.CommandSetNickName),
        ("恢复名称", "CommandUnuseCustomNickName", c => c.CommandClearNickName),
        ("使用物品", "CommandUseItem", c => c.CommandUseItem),
    };

    private readonly Dictionary<string, HcTextBox> _boxes = new();

    public CommandsConfigPage()
    {
        InitializeComponent();
        BuildRows();
        ReloadValues();
    }

    private void BuildRows()
    {
        var grid = new UniformGrid { Columns = 2 };
        foreach (var (label, key, _) in Fields)
        {
            var box = new HcTextBox { VerticalAlignment = VerticalAlignment.Center, MinWidth = 100 };
            _boxes[key] = box;

            var row = new Grid { Margin = new Thickness(4) };
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(96) });
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            var text = new TextBlock
            {
                Text = label,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(2, 0, 8, 0)
            };
            Grid.SetColumn(text, 0);
            Grid.SetColumn(box, 1);
            row.Children.Add(text);
            row.Children.Add(box);
            grid.Children.Add(row);
        }

        ContentPanel.Children.Add(grid);
    }

    /// <summary>把当前配置快照重新读入所有输入框（热重载后刷新用）</summary>
    private void ReloadValues()
    {
        var config = CoreConfiguration.Current;
        if (config is null)
        {
            return;
        }

        foreach (var (_, key, get) in Fields)
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
        foreach (var (_, key, _) in Fields)
        {
            values[key] = _boxes[key].Text ?? string.Empty;
        }

        if (ConfigEditor.TrySaveCore(values, out var error))
        {
            Growl.Success("已保存，插件将自动热重载生效");
        }
        else
        {
            Growl.Error(error);
        }
    }
}
