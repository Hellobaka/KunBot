using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using HandyControl.Controls;
using me.cqp.luohuaming.iKun.Domain.Configuration;

// HandyControl 无 Expander 类（3.5.1 中为原生控件 + 主题样式），此处 TextBox 用 hc: 前缀
using HcTextBox = HandyControl.Controls.TextBox;

namespace me.cqp.luohuaming.iKun.Admin.Views;

/// <summary>
/// 回复文案配置页：编辑 Config.json 中 94 个 Reply* 键（分组同 Domain/Configuration/ReplyTexts.cs）。
/// 每组一个 Expander（默认展开），行 = 键名(灰色小字) + 中文说明 + 多行 TextBox。
/// 键名保留旧版拼写（如 ReplyNoTargrtKun），不可"纠正"。保存经 ConfigEditor.TrySaveCore。
/// </summary>
public partial class RepliesConfigPage : UserControl
{
    /// <summary>（分组，JSON 键，中文说明，读取当前值）</summary>
    private static readonly (string Group, string Key, string Desc, Func<CoreConfiguration, string> Get)[] Fields =
    {
        // 注册/签到
        ("注册/签到", "ReplyDuplicateRegister", "重复注册提示", c => c.Replies.DuplicateRegister),
        ("注册/签到", "ReplyNewRegister", "注册成功（{0} 金币 {1} 鲲蛋）", c => c.Replies.RegisterSuccess),
        ("注册/签到", "ReplyRegisterFailed", "注册失败", c => c.Replies.RegisterFailed),
        ("注册/签到", "ReplyLoginDuplicate", "重复签到提示", c => c.Replies.DuplicateLogin),
        ("注册/签到", "ReplyLoginReward", "签到成功（{0} 金币 {1} 鲲蛋）", c => c.Replies.LoginSuccess),
        ("注册/签到", "ReplyMenu", "功能菜单列表（{0}~{17} 为指令触发词）", c => c.Replies.Menu),
        // 通用
        ("通用", "ReplyNoPlayer", "未注册", c => c.Replies.NoPlayer),
        ("通用", "ReplyNoKun", "未持有鲲", c => c.Replies.NoKun),
        ("通用", "ReplyNoTargrtKun", "目标鲲不存在（旧拼写 Targrt）", c => c.Replies.TargetKunNotFound),
        ("通用", "ReplyNoTargetPlayer", "目标玩家未注册", c => c.Replies.TargetPlayerNotRegistered),
        ("通用", "ReplyNoTargetKun", "目标玩家未持有鲲", c => c.Replies.TargetPlayerHasNoKun),
        ("通用", "ReplyKunOwnerNotMatch", "这只鲲不是你的", c => c.Replies.KunNotOwned),
        ("通用", "ReplyKunAbandoned", "鲲已被标记为弃置", c => c.Replies.KunAbandoned),
        ("通用", "ReplyKunAlive", "鲲未死亡", c => c.Replies.KunAlive),
        ("通用", "ReplyKunNotAlive", "鲲已死亡", c => c.Replies.KunDead),
        ("通用", "ReplyParamInvalid", "指令格式错误", c => c.Replies.ParamInvalid),
        ("通用", "ReplyItemLeak", "物品数量不足", c => c.Replies.ItemLeak),
        ("通用", "ReplyWeightLimit", "体重已达上限", c => c.Replies.WeightLimitReached),
        // 鲲展示
        ("鲲展示", "ReplyRankingHeader", "排行列表头部", c => c.Replies.RankingHeader),
        ("鲲展示", "ReplyRankingGroupHeader", "群排行列表头部", c => c.Replies.RankingGroupHeader),
        ("鲲展示", "ReplyKunToString", "鲲展示格式（%PetAttributeA%/%PetAttributeB%%PetAttributeC%/LongLevel 占位）", c => c.Replies.KunToString),
        ("鲲展示", "ReplyKunNickNameToString", "鲲昵称展示格式（%PetNickName%/LongLevel 占位）", c => c.Replies.KunNickNameToString),
        ("鲲展示", "ReplyRankingAutoPlaying", "排行中挂机状态标签", c => c.Replies.RankingIdleTag),
        ("鲲展示", "ReplyRankingWorking", "排行中打工状态标签", c => c.Replies.RankingWorkTag),
        // 孵化
        ("孵化", "ReplyDuplicateHatch", "已有鲲时重复孵化提示", c => c.Replies.DuplicateHatch),
        ("孵化", "ReplyHatchFail", "孵化失败", c => c.Replies.HatchFailed),
        ("孵化", "ReplyHatchKun", "单蛋孵化成功", c => c.Replies.HatchSingleSuccess),
        ("孵化", "ReplyMultiHatchKun", "多蛋孵化成功", c => c.Replies.HatchMultiSuccess),
        // 喂养/强化
        ("喂养/强化", "ReplyFeed", "喂养结果", c => c.Replies.FeedResult),
        ("喂养/强化", "ReplyUpgradeSuccess", "强化成功", c => c.Replies.UpgradeSuccess),
        ("喂养/强化", "ReplyUpgradeFail", "强化失败", c => c.Replies.UpgradeFailed),
        // 渡劫
        ("渡劫", "ReplyAscendNoWeightLimit", "体重未达上限无法渡劫", c => c.Replies.AscendWeightBelowLimit),
        ("渡劫", "ReplyAscendSuccess", "渡劫成功", c => c.Replies.AscendSuccess),
        ("渡劫", "ReplyAscendFail", "渡劫失败", c => c.Replies.AscendFailed),
        ("渡劫", "ReplyAscendFailAndDead", "渡劫失败并死亡", c => c.Replies.AscendDied),
        ("渡劫", "ReplyConsumeAscendPill", "使用渡劫丹提示", c => c.Replies.ConsumeAscendPill),
        // 复活/放生
        ("复活/放生", "ReplyDuplicateResurrect", "已有鲲时重复复活提示", c => c.Replies.DuplicateResurrect),
        ("复活/放生", "ReplyResurrectHourLimit", "死亡超时无法复活", c => c.Replies.ResurrectHourLimit),
        ("复活/放生", "ReplyResurrectSuccess", "复活成功", c => c.Replies.ResurrectSuccess),
        ("复活/放生", "ReplyResurrectFail", "复活失败", c => c.Replies.ResurrectFailed),
        ("复活/放生", "ReplyReleaseSuccess", "放生成功", c => c.Replies.ReleaseSuccess),
        ("复活/放生", "ReplyReleaseFail", "放生失败", c => c.Replies.ReleaseFailed),
        ("复活/放生", "ReplyQueryDeadKun", "可复活鲲列表头部", c => c.Replies.DeadKunsHeader),
        // 幻化
        ("幻化", "ReplyTransmogrifyLevelLimit", "幻化受等级限制", c => c.Replies.TransmogrifyLevelLimit),
        ("幻化", "ReplyTransmogrifySuccess", "幻化成功", c => c.Replies.TransmogrifySuccess),
        ("幻化", "ReplyTransmogrifyFail", "幻化失败", c => c.Replies.TransmogrifyFailed),
        ("幻化", "ReplyTransmogrifyFailAndDead", "幻化失败并死亡", c => c.Replies.TransmogrifyDied),
        // 攻击
        ("攻击", "ReplyAttackSelf", "不能攻击自己", c => c.Replies.AttackSelf),
        ("攻击", "ReplyAttackInCD", "攻击冷却中", c => c.Replies.AttackCooldown),
        ("攻击", "ReplyAttackSuccess", "攻击成功", c => c.Replies.AttackSuccess),
        ("攻击", "ReplyAttackFail", "攻击失败", c => c.Replies.AttackFailed),
        ("攻击", "ReplyAttackEscaped", "攻击被对方逃脱", c => c.Replies.AttackEscaped),
        ("攻击", "ReplyAttackSuccessAndTargetDead", "攻击成功且目标死亡", c => c.Replies.AttackTargetDied),
        ("攻击", "ReplyAttackFailAndDead", "攻击失败且自身死亡", c => c.Replies.AttackerDied),
        // 吞噬
        ("吞噬", "ReplyDevourSelf", "不能吞噬自己", c => c.Replies.DevourSelf),
        ("吞噬", "ReplyDevourInCD", "吞噬冷却中", c => c.Replies.DevourCooldown),
        ("吞噬", "ReplyDevourSuccess", "吞噬成功", c => c.Replies.DevourSuccess),
        ("吞噬", "ReplyDevourFail", "吞噬失败", c => c.Replies.DevourFailed),
        ("吞噬", "ReplyDevourEscaped", "吞噬被对方逃脱", c => c.Replies.DevourEscaped),
        ("吞噬", "ReplyDevourFailAndDead", "吞噬失败反被对方吃掉", c => c.Replies.DevouredByOther),
        // 跨群广播
        ("跨群广播", "ReplyAttackedNotSameGroup", "跨群被攻击广播", c => c.Replies.CrossGroupAttacked),
        ("跨群广播", "ReplyAttackedNotSameGroupAndDead", "跨群被攻击致死", c => c.Replies.CrossGroupAttackedToDeath),
        ("跨群广播", "ReplyAttackedNotSameGroupButEscaped", "跨群攻击被逃脱", c => c.Replies.CrossGroupAttackEscaped),
        ("跨群广播", "ReplyDevouredNotSameGroup", "跨群被吞噬广播", c => c.Replies.CrossGroupDevoured),
        ("跨群广播", "ReplyDevouredNotSameGroupButEscaped", "跨群吞噬被逃脱", c => c.Replies.CrossGroupDevourEscaped),
        // 商店/物品
        ("商店/物品", "ReplyShoppingHeader", "商店列表头部", c => c.Replies.ShopHeader),
        ("商店/物品", "ReplyShoppingDetail", "商店条目格式（%Index%/%CoinCount%/%CoinName%/%ItemCount%/%ItemName% 占位）", c => c.Replies.ShopEntry),
        ("商店/物品", "ReplyItemCannotBuy", "购买失败（序号不存在）", c => c.Replies.ShopIndexInvalid),
        ("商店/物品", "ReplyBuyItem", "购买成功", c => c.Replies.PurchaseSuccess),
        ("商店/物品", "ReplyOpenKunEgg", "开鲲蛋结果", c => c.Replies.OpenEggResult),
        ("商店/物品", "ReplyBlindBoxOpen", "开盲盒结果", c => c.Replies.BlindBoxOpened),
        ("商店/物品", "ReplyBlindBoxGetNothing", "开盲盒未获得物品", c => c.Replies.BlindBoxEmpty),
        ("商店/物品", "ReplyEmptyInventory", "仓库为空", c => c.Replies.InventoryEmpty),
        ("商店/物品", "ReplyItemCannotUse", "物品无法以此方式使用", c => c.Replies.ItemCannotUse),
        ("商店/物品", "ReplyItemUseFailed", "物品使用失败", c => c.Replies.ItemUseFailed),
        // 挂机/打工
        ("挂机/打工", "ReplyAutoPlayStarted", "挂机开始", c => c.Replies.IdleStarted),
        ("挂机/打工", "ReplyWorkingStarted", "打工开始", c => c.Replies.WorkStarted),
        ("挂机/打工", "ReplyAutoPlayFinished", "挂机完成", c => c.Replies.IdleFinished),
        ("挂机/打工", "ReplyAutoPlayFinishedButDead", "挂机完成但暴毙", c => c.Replies.IdleFinishedButDead),
        ("挂机/打工", "ReplyWorkingFinished", "打工完成", c => c.Replies.WorkFinished),
        ("挂机/打工", "ReplyAutoPlaying", "正在挂机中", c => c.Replies.KunIdling),
        ("挂机/打工", "ReplyWorking", "正在打工中", c => c.Replies.KunWorking),
        ("挂机/打工", "ReplyNotAutoPlaying", "未在挂机", c => c.Replies.KunNotIdling),
        ("挂机/打工", "ReplyNotWorking", "未在打工", c => c.Replies.KunNotWorking),
        ("挂机/打工", "ReplyStartAutoPlayFailed", "无法开始挂机（后缀文案）", c => c.Replies.IdleStartBlocked),
        ("挂机/打工", "ReplyAutoPlayInCD", "挂机冷却中", c => c.Replies.IdleCooldown),
        ("挂机/打工", "ReplyWorkingInCD", "打工冷却中", c => c.Replies.WorkCooldown),
        // 昵称
        ("昵称", "ReplyCustomNickApplied", "自定义昵称已生效", c => c.Replies.NickNameApplied),
        ("昵称", "ReplyCustomNickDiscarded", "自定义昵称已抛弃", c => c.Replies.NickNameDiscarded),
        ("昵称", "ReplyCustomNickInvalid", "昵称含非法词汇", c => c.Replies.NickNameInvalid),
        // 天罚
        ("天罚", "ReplyRandomPunish", "天罚信息（{0} 星期 {1} 下次时间）", c => c.Replies.PunishInfo),
        ("天罚", "ReplyRandomPunishSkipped", "本周天罚无事", c => c.Replies.PunishSkipped),
        ("天罚", "ReplyRandomPunishFinished", "天罚已执行", c => c.Replies.PunishExecuted),
        ("天罚", "ReplyRandomPunishFinishedAndDead", "天罚暴毙", c => c.Replies.PunishExecutedAndDied),
    };

    private readonly Dictionary<string, HcTextBox> _boxes = new();

    public RepliesConfigPage()
    {
        InitializeComponent();
        BuildGroups();
        ReloadValues();
    }

    private void BuildGroups()
    {
        string currentGroup = null;
        StackPanel groupContent = null;

        foreach (var (group, key, desc, _) in Fields)
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
                    Content = groupContent
                });
            }

            var box = new HcTextBox
            {
                AcceptsReturn = true,
                TextWrapping = TextWrapping.Wrap,
                Height = 60,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Margin = new Thickness(0, 4, 0, 10)
            };
            _boxes[key] = box;

            var header = new TextBlock
            {
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(2, 2, 0, 0)
            };
            header.Inlines.Add(new Run(key) { FontSize = 11, Foreground = Brushes.Gray });
            header.Inlines.Add(new Run("　" + desc));

            var row = new StackPanel { Margin = new Thickness(2) };
            row.Children.Add(header);
            row.Children.Add(box);
            groupContent.Children.Add(row);
        }
    }

    /// <summary>把当前配置快照重新读入所有输入框（热重载后刷新用）</summary>
    private void ReloadValues()
    {
        var config = CoreConfiguration.Current;
        if (config is null)
        {
            return;
        }

        foreach (var (_, key, _, get) in Fields)
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
        foreach (var (_, key, _, _) in Fields)
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
