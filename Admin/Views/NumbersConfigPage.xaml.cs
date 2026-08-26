using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using HandyControl.Controls;
using me.cqp.luohuaming.iKun.Domain.Configuration;

// HandyControl 3.5.1 没有 Button/CheckBox/GroupBox/Expander 类（用原生控件 + 主题样式），
// 数值/日期/输入框使用 HandyControl 控件：
using HcNud = HandyControl.Controls.NumericUpDown;
using HcComboBox = HandyControl.Controls.ComboBox;
using HcDatePicker = HandyControl.Controls.DatePicker;
using HcTextBox = HandyControl.Controls.TextBox;

namespace me.cqp.luohuaming.iKun.Admin.Views;

/// <summary>
/// 数值参数配置页：编辑 Config.json 中 81 个数值/开关/列表键（含旧版键名）。
/// 分组用 Expander（默认展开）；数值用 NumericUpDown，列表用 ListBox + 添加/删除。
/// 保存时全部校验（NaN/越界/非整数/时间格式），任一无效则不保存。经 ConfigEditor.TrySaveCore。
/// </summary>
public partial class NumbersConfigPage : UserControl
{
    /// <summary>字段控件类型</summary>
    private enum Kind
    {
        Int,
        Double,
        Bool,
        String,
        /// <summary>ShortNumberStyle 枚举下拉</summary>
        Style,
        /// <summary>天罚星期下拉（1=周一 … 7=周日）</summary>
        PunishDay,
        /// <summary>天罚执行时间（日期 + HH:mm）</summary>
        PunishTime,
        /// <summary>List&lt;string&gt; 列表编辑器</summary>
        ListString,
        /// <summary>List&lt;long&gt; 列表编辑器</summary>
        ListLong,
    }

    private sealed class Field
    {
        public string Label;
        public string Key;
        public Kind Kind;
        public int Section;
        public double Min;
        public double Max;
        public double Step;
        public string Hint;
        public Func<CoreConfiguration, object> Get;
    }

    private static readonly string[] SectionTitles =
    {
        "开关", "显示", "奖励/CD", "概率", "孵化", "喂养", "攻击/吞噬", "渡劫",
        "幻化", "强化", "复活/死亡", "挂机/打工", "天罚", "跨群通知", "商店", "盲盒",
        "群白名单", "管理员",
    };

    private const double Big = 1_000_000_000;
    private const double Big2 = 1_000_000;

    /// <summary>全部字段（Section 对应 SectionTitles 下标）；JSON 键保留旧版拼写，不可"纠正"</summary>
    private static readonly Field[] Fields =
    {
        // ---- 开关 ----
        new() { Label = "启用 @ 目标", Key = "EnableAt", Kind = Kind.Bool, Section = 0, Hint = "战斗消息中 @ 目标玩家", Get = c => c.EnableAt },
        new() { Label = "启用天罚", Key = "EnableRandomPunish", Kind = Kind.Bool, Section = 0, Hint = "每周按天罚时间执行一次", Get = c => c.EnableRandomPunish },
        new() { Label = "跨群攻击通知", Key = "EnableNotSameGroupAttackBoardcast", Kind = Kind.Bool, Section = 0, Hint = "鲲被其他群攻击时通知其所在群", Get = c => c.BroadcastCrossGroupAttack },
        new() { Label = "跨群吞噬通知", Key = "EnableNotSameGroupDevourBoardcast", Kind = Kind.Bool, Section = 0, Hint = "鲲被其他群吞噬时通知其所在群", Get = c => c.BroadcastCrossGroupDevour },
        new() { Label = "跨群攻击逃脱通知", Key = "EnableNotSameGroupAttackEscapeBoardcast", Kind = Kind.Bool, Section = 0, Hint = "鲲在其他群逃脱攻击时通知", Get = c => c.BroadcastCrossGroupAttackEscape },
        new() { Label = "跨群吞噬逃脱通知", Key = "EnableNotSameGroupDevourEscapeBoardcast", Kind = Kind.Bool, Section = 0, Hint = "鲲在其他群逃脱吞噬时通知", Get = c => c.BroadcastCrossGroupDevourEscape },

        // ---- 显示 ----
        new() { Label = "体重缩写风格", Key = "ShortNumberType", Kind = Kind.Style, Section = 1, Hint = "普通 / 中文字（万/亿…）/ 科学计数", Get = c => c.ShortNumberStyle },
        new() { Label = "体重单位基数", Key = "WeightUnitBase", Kind = Kind.Int, Section = 1, Min = 1, Max = Big, Step = 1, Hint = "显示体重 = 数据库体重 ÷ 该值", Get = c => c.WeightUnitBase },
        new() { Label = "体重单位", Key = "WeightUnit", Kind = Kind.String, Section = 1, Hint = "数字后的单位后缀，如 kg", Get = c => c.WeightUnit },
        new() { Label = "昵称过滤词", Key = "NickNameFilter", Kind = Kind.ListString, Section = 1, Hint = "昵称中不允许出现的子串，逐项添加", Get = c => c.NickNameFilter },

        // ---- 奖励/CD ----
        new() { Label = "注册奖励金币", Key = "ValueRegisterCoinReward", Kind = Kind.Int, Section = 2, Min = 0, Max = Big, Step = 1, Get = c => c.RegisterRewardCoins },
        new() { Label = "注册奖励鲲蛋", Key = "ValueRegisterEggReward", Kind = Kind.Int, Section = 2, Min = 0, Max = Big, Step = 1, Get = c => c.RegisterRewardEggs },
        new() { Label = "签到奖励金币", Key = "ValueLoginCoinReward", Kind = Kind.Int, Section = 2, Min = 0, Max = Big, Step = 1, Get = c => c.LoginRewardCoins },
        new() { Label = "签到奖励鲲蛋", Key = "ValueLoginEggReward", Kind = Kind.Int, Section = 2, Min = 0, Max = Big, Step = 1, Get = c => c.LoginRewardEggs },
        new() { Label = "攻击冷却", Key = "ValueAttackCD", Kind = Kind.Double, Section = 2, Min = 0, Max = Big2, Step = 1, Hint = "分钟", Get = c => c.AttackCooldownMinutes },
        new() { Label = "吞噬冷却", Key = "ValueDevourCD", Kind = Kind.Double, Section = 2, Min = 0, Max = Big2, Step = 1, Hint = "分钟", Get = c => c.DevourCooldownMinutes },

        // ---- 概率 ----
        new() { Label = "词缀概率：无", Key = "ProbablityNone", Kind = Kind.Double, Section = 3, Min = 0, Max = 100, Step = 0.1, Hint = "百分比", Get = c => c.ProbabilityNone },
        new() { Label = "词缀概率：金", Key = "ProbablityJin", Kind = Kind.Double, Section = 3, Min = 0, Max = 100, Step = 0.1, Hint = "百分比", Get = c => c.ProbabilityJin },
        new() { Label = "词缀概率：木", Key = "ProbablityMu", Kind = Kind.Double, Section = 3, Min = 0, Max = 100, Step = 0.1, Hint = "百分比", Get = c => c.ProbabilityMu },
        new() { Label = "词缀概率：水", Key = "ProbablityShui", Kind = Kind.Double, Section = 3, Min = 0, Max = 100, Step = 0.1, Hint = "百分比", Get = c => c.ProbabilityShui },
        new() { Label = "词缀概率：火", Key = "ProbablityHuo", Kind = Kind.Double, Section = 3, Min = 0, Max = 100, Step = 0.1, Hint = "百分比", Get = c => c.ProbabilityHuo },
        new() { Label = "词缀概率：土", Key = "ProbablityTu", Kind = Kind.Double, Section = 3, Min = 0, Max = 100, Step = 0.1, Hint = "百分比", Get = c => c.ProbabilityTu },
        new() { Label = "词缀概率：风", Key = "ProbablityFeng", Kind = Kind.Double, Section = 3, Min = 0, Max = 100, Step = 0.1, Hint = "百分比", Get = c => c.ProbabilityFeng },
        new() { Label = "词缀概率：雷", Key = "ProbablityLei", Kind = Kind.Double, Section = 3, Min = 0, Max = 100, Step = 0.1, Hint = "百分比", Get = c => c.ProbabilityLei },
        new() { Label = "词缀概率：阴", Key = "ProbablityYin", Kind = Kind.Double, Section = 3, Min = 0, Max = 100, Step = 0.1, Hint = "百分比", Get = c => c.ProbabilityYin },
        new() { Label = "词缀概率：阳", Key = "ProbablityYang", Kind = Kind.Double, Section = 3, Min = 0, Max = 100, Step = 0.1, Hint = "百分比", Get = c => c.ProbabilityYang },

        // ---- 孵化 ----
        new() { Label = "孵化成功率下限", Key = "ValueHatchProbablityMin", Kind = Kind.Int, Section = 4, Min = 0, Max = 100, Step = 1, Hint = "百分比", Get = c => c.HatchRateMinPercent },
        new() { Label = "孵化成功率上限", Key = "ValueHatchProbablityMax", Kind = Kind.Int, Section = 4, Min = 0, Max = 100, Step = 1, Hint = "百分比", Get = c => c.HatchRateMaxPercent },
        new() { Label = "孵化体重下限", Key = "ValueHatchWeightMin", Kind = Kind.Int, Section = 4, Min = 0, Max = Big, Step = 1, Get = c => c.HatchWeightMin },
        new() { Label = "孵化体重上限", Key = "ValueHatchWeightMax", Kind = Kind.Int, Section = 4, Min = 0, Max = Big, Step = 1, Get = c => c.HatchWeightMax },

        // ---- 喂养 ----
        new() { Label = "喂养金币消耗", Key = "ValueFeedCoinConsume", Kind = Kind.Int, Section = 5, Min = 0, Max = Big, Step = 1, Get = c => c.FeedCoinCostPerCount },
        new() { Label = "喂养鲲蛋消耗", Key = "ValueFeedKunEggConsume", Kind = Kind.Int, Section = 5, Min = 0, Max = Big, Step = 1, Get = c => c.FeedEggCostPerCount },
        new() { Label = "喂养基础体重增量", Key = "ValueFeedWeightBaseIncrement", Kind = Kind.Int, Section = 5, Min = 0, Max = Big, Step = 1, Get = c => c.FeedWeightBaseIncrement },
        new() { Label = "喂养体重加成下限", Key = "ValueFeedWeightMinimumIncrement", Kind = Kind.Int, Section = 5, Min = 0, Max = 100, Step = 1, Hint = "百分比", Get = c => c.FeedWeightMinBonusPercent },
        new() { Label = "喂养体重加成上限", Key = "ValueFeedWeightMaximumIncrement", Kind = Kind.Int, Section = 5, Min = 0, Max = 100, Step = 1, Hint = "百分比", Get = c => c.FeedWeightMaxBonusPercent },

        // ---- 攻击/吞噬 ----
        new() { Label = "攻击减重下限", Key = "ValueAttackWeightMinimumDecrement", Kind = Kind.Int, Section = 6, Min = 0, Max = 100, Step = 1, Hint = "百分比", Get = c => c.AttackDamageMinPercent },
        new() { Label = "攻击减重上限", Key = "ValueAttackWeightMaximumDecrement", Kind = Kind.Int, Section = 6, Min = 0, Max = 100, Step = 1, Hint = "百分比", Get = c => c.AttackDamageMaxPercent },
        new() { Label = "排行榜显示数量", Key = "ValueRankingCount", Kind = Kind.Int, Section = 6, Min = 1, Max = 1000, Step = 1, Get = c => c.RankingSize },
        new() { Label = "吞噬抽选范围", Key = "ValueDevourDrawPercentage", Kind = Kind.Int, Section = 6, Min = 0, Max = 100, Step = 1, Hint = "百分比", Get = c => c.DevourDrawRangePercent },
        new() { Label = "吞噬失败死亡概率", Key = "ValueDevourFailDeadProbablity", Kind = Kind.Double, Section = 6, Min = 0, Max = 100, Step = 0.1, Hint = "百分比", Get = c => c.DevourFailDeathChance },

        // ---- 渡劫 ----
        new() { Label = "渡劫失败死亡概率", Key = "ValueAscendFailDeadProbablity", Kind = Kind.Double, Section = 7, Min = 0, Max = 100, Step = 0.1, Hint = "百分比", Get = c => c.AscendFailDeathChance },
        new() { Label = "渡劫成功增重下限", Key = "ValueAscendWeightMinimalIncrement", Kind = Kind.Int, Section = 7, Min = 0, Max = Big2, Step = 1, Hint = "百分比", Get = c => c.AscendGainMinPercent },
        new() { Label = "渡劫成功增重上限", Key = "ValueAscendWeightMaximalIncrement", Kind = Kind.Int, Section = 7, Min = 0, Max = Big2, Step = 1, Hint = "百分比", Get = c => c.AscendGainMaxPercent },
        new() { Label = "渡劫失败减重下限", Key = "ValueAscendWeightMinimalDecrement", Kind = Kind.Int, Section = 7, Min = 0, Max = Big2, Step = 1, Hint = "百分比", Get = c => c.AscendLossMinPercent },
        new() { Label = "渡劫失败减重上限", Key = "ValueAscendWeightMaximalDecrement", Kind = Kind.Int, Section = 7, Min = 0, Max = Big2, Step = 1, Hint = "百分比", Get = c => c.AscendLossMaxPercent },
        new() { Label = "渡劫金币消耗", Key = "ValueAscendCoinConsume", Kind = Kind.Int, Section = 7, Min = 0, Max = Big, Step = 1, Get = c => c.AscendCoinCost },
        new() { Label = "渡劫丹最多使用数", Key = "ValueAscendPillMaxConsumeCount", Kind = Kind.Int, Section = 7, Min = 0, Max = Big2, Step = 1, Get = c => c.MaxAscendPillConsume },
        new() { Label = "每颗渡劫丹成功率加成", Key = "ValueAscendPillPerIncrement", Kind = Kind.Int, Section = 7, Min = 0, Max = 100, Step = 1, Hint = "百分比", Get = c => c.AscendSuccessPerPillPercent },

        // ---- 幻化 ----
        new() { Label = "幻化金币消耗", Key = "ValueTranmogifyCoinConsume", Kind = Kind.Int, Section = 8, Min = 0, Max = Big, Step = 1, Get = c => c.TransmogrifyCoinCost },
        new() { Label = "幻化丹消耗", Key = "ValueTranmogifyPillConsume", Kind = Kind.Int, Section = 8, Min = 0, Max = Big, Step = 1, Get = c => c.TransmogrifyPillCost },
        new() { Label = "幻化最低等级", Key = "ValueTranmogifyLevelLimit", Kind = Kind.Int, Section = 8, Min = 0, Max = Big2, Step = 1, Get = c => c.TransmogrifyLevelRequirement },
        new() { Label = "幻化失败致死体重下限", Key = "ValueTransmoirgifyFailDeadWeightLimit", Kind = Kind.Double, Section = 8, Min = 0, Max = Big2, Step = 1, Hint = "幻化后体重低于该值即死亡", Get = c => c.TransmogrifyDeathWeightLimit },
        new() { Label = "幻化失败死亡概率", Key = "ValueTransmoirgifyFailDeadProbablity", Kind = Kind.Double, Section = 8, Min = 0, Max = 100, Step = 0.1, Hint = "百分比", Get = c => c.TransmogrifyFailDeathChance },

        // ---- 强化 ----
        new() { Label = "强化金币消耗", Key = "ValueUpgradeCoinConsume", Kind = Kind.Int, Section = 9, Min = 0, Max = Big, Step = 1, Get = c => c.UpgradeCoinCost },
        new() { Label = "强化丹消耗", Key = "ValueUpgradePillConsume", Kind = Kind.Int, Section = 9, Min = 0, Max = Big, Step = 1, Get = c => c.UpgradePillCost },
        new() { Label = "强化获得经验所需时间", Key = "ValueUpgradeExpHour", Kind = Kind.Int, Section = 9, Min = 0, Max = Big2, Step = 1, Hint = "小时", Get = c => c.UpgradeExpHours },

        // ---- 复活/死亡 ----
        new() { Label = "最晚复活时间", Key = "ValueMaxDeadHour", Kind = Kind.Int, Section = 10, Min = 0, Max = Big2, Step = 1, Hint = "小时（超过后不可复活）", Get = c => c.MaxResurrectHours },
        new() { Label = "每 2 小时体重损失", Key = "ValuePerTwoHourWeightLoss", Kind = Kind.Int, Section = 10, Min = 0, Max = 100, Step = 1, Hint = "百分比/每2小时（死亡期间）", Get = c => c.WeightLossPerTwoHoursPercent },
        new() { Label = "每 18 小时等级损失", Key = "ValuePerEighteenHourLevelLoss", Kind = Kind.Int, Section = 10, Min = 0, Max = Big2, Step = 1, Hint = "每18小时（死亡期间）", Get = c => c.LevelLossPerEighteenHours },
        new() { Label = "复活体重保底", Key = "ValueResurrectWeightBase", Kind = Kind.Int, Section = 10, Min = 0, Max = 100, Step = 1, Hint = "百分比（体重保底 = 等级上限 × 该比例）", Get = c => c.ResurrectFloorPercent },

        // ---- 挂机/打工 ----
        new() { Label = "挂机死亡概率", Key = "ValueAutoPlayDeadProbablity", Kind = Kind.Int, Section = 11, Min = 0, Max = 100, Step = 1, Hint = "百分比", Get = c => c.IdleDeathChancePercent },
        new() { Label = "最长挂机时长", Key = "ValueMaxAutoPlayDuration", Kind = Kind.Int, Section = 11, Min = 0, Max = Big2, Step = 1, Hint = "小时", Get = c => c.MaxIdleDurationHours },
        new() { Label = "挂机冷却", Key = "ValueAutoPlayCDHour", Kind = Kind.Double, Section = 11, Min = 0, Max = Big2, Step = 1, Hint = "小时", Get = c => c.IdleCooldownHours },
        new() { Label = "打工冷却", Key = "ValueWorkingCDHour", Kind = Kind.Double, Section = 11, Min = 0, Max = Big2, Step = 1, Hint = "小时", Get = c => c.WorkCooldownHours },
        new() { Label = "打工每小时金币奖励", Key = "ValueWorkingCoinRewardPerHour", Kind = Kind.Int, Section = 11, Min = 0, Max = Big, Step = 1, Get = c => c.WorkCoinPerHour },
        new() { Label = "打工等级加成", Key = "ValueWorkLevelBouns", Kind = Kind.Int, Section = 11, Min = 0, Max = Big2, Step = 1, Hint = "百分比（旧拼写 Bouns）", Get = c => c.WorkLevelBonusPercent },

        // ---- 天罚 ----
        new() { Label = "天罚执行概率", Key = "ValueRandomPunishProbablity", Kind = Kind.Int, Section = 12, Min = 0, Max = 100, Step = 1, Hint = "百分比（抽中后是否执行）", Get = c => c.PunishChancePercent },
        new() { Label = "天罚减重下限", Key = "ValueRandomPunishMinimalDecrement", Kind = Kind.Int, Section = 12, Min = 0, Max = 100, Step = 1, Hint = "百分比", Get = c => c.PunishLossMinPercent },
        new() { Label = "天罚减重上限", Key = "ValueRandomPunishMaximalDecrement", Kind = Kind.Int, Section = 12, Min = 0, Max = 100, Step = 1, Hint = "百分比", Get = c => c.PunishLossMaxPercent },
        new() { Label = "天罚死亡概率", Key = "ValueRandomPunishDeadProbablity", Kind = Kind.Int, Section = 12, Min = 0, Max = 100, Step = 1, Hint = "百分比", Get = c => c.PunishDeathChancePercent },
        new() { Label = "天罚执行星期", Key = "ValueRandomPunishExecuteDay", Kind = Kind.PunishDay, Section = 12, Hint = "1=周一 … 7=周日", Get = c => c.PunishExecuteDayOfWeek },
        new() { Label = "天罚执行时间", Key = "ValueRandomPunishExecuteTime", Kind = Kind.PunishTime, Section = 12, Hint = "日期 + HH:mm（24 小时制）", Get = c => c.PunishExecuteTime },

        // ---- 跨群通知 ----
        new() { Label = "跨群通知最低损失比例", Key = "ValueNotSameGroupNoticeMinimalPercent", Kind = Kind.Int, Section = 13, Min = 0, Max = 100, Step = 1, Hint = "百分比（损失超过该比例才通知）", Get = c => c.CrossGroupNoticeMinLossPercent },

        // ---- 商店 ----
        new() { Label = "商店列表", Key = "ShoppingList", Kind = Kind.ListString, Section = 14, Hint = "格式: 金币数|物品ID|数量|价格倍率，逐项添加", Get = c => c.ShoppingListRaw },

        // ---- 盲盒 ----
        new() { Label = "盲盒内容", Key = "BlindBoxContents", Kind = Kind.ListString, Section = 15, Hint = "格式: 物品ID|权重，逐项添加", Get = c => c.BlindBoxContentsRaw },
        new() { Label = "启用多物品抽取", Key = "BlindBoxEnableMultiContents", Kind = Kind.Bool, Section = 15, Hint = "一个盲盒可开出多件物品", Get = c => c.BlindBoxAllowMultiDraw },
        new() { Label = "多物品必含物品", Key = "BlindBoxMultiContentMustHasItem", Kind = Kind.Bool, Section = 15, Hint = "多物品抽取时保证含固定物品", Get = c => c.BlindBoxMultiDrawMustContainItem },
        new() { Label = "多物品抽取概率", Key = "BlindBoxMultiContentProbablity", Kind = Kind.Int, Section = 15, Min = 0, Max = 100, Step = 1, Hint = "百分比", Get = c => c.BlindBoxMultiDrawChancePercent },
        new() { Label = "鲲蛋换盲盒比率", Key = "ValueKunEggToCoinRate", Kind = Kind.Int, Section = 15, Min = 0, Max = Big, Step = 1, Hint = "每开 1 颗鲲蛋获得的盲盒数", Get = c => c.EggToBlindBoxRate },

        // ---- 群白名单 ----
        new() { Label = "允许使用的群号", Key = "Groups", Kind = Kind.ListLong, Section = 16, Hint = "白名单群号，逐项添加", Get = c => c.EnabledGroups },

        // ---- 管理员 ----
        new() { Label = "管理员 QQ 号", Key = "Admins", Kind = Kind.ListLong, Section = 17, Hint = "管理员 QQ，逐项添加", Get = c => c.Admins },
    };

    private readonly Dictionary<string, HcNud> _nums = new();
    private readonly Dictionary<string, CheckBox> _bools = new();
    private readonly Dictionary<string, HcTextBox> _strings = new();
    private readonly Dictionary<string, ListBox> _lists = new();
    private HcComboBox _styleCombo;
    private HcComboBox _dayCombo;
    private HcDatePicker _punishDate;
    private HcTextBox _punishTime;

    public NumbersConfigPage()
    {
        InitializeComponent();
        BuildSections();
        ReloadValues();
    }

    // ---------- 构建 ----------

    private void BuildSections()
    {
        for (int i = 0; i < SectionTitles.Length; i++)
        {
            var content = new StackPanel { Margin = new Thickness(8, 4, 4, 4) };
            ContentPanel.Children.Add(new Expander
            {
                Header = SectionTitles[i],
                IsExpanded = true,
                Margin = new Thickness(0, 0, 0, 6),
                Content = content,
            });

            foreach (var f in Fields.Where(x => x.Section == i))
            {
                AddFieldRow(content, f);
            }
        }
    }

    private void AddFieldRow(StackPanel section, Field f)
    {
        switch (f.Kind)
        {
            case Kind.Int:
            case Kind.Double:
            {
                var nud = new HcNud
                {
                    Value = 0,
                    Minimum = f.Min,
                    Maximum = f.Max,
                    Increment = f.Step,
                    Width = 110,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                _nums[f.Key] = nud;
                AddSimpleRow(section, f, nud);
                break;
            }
            case Kind.Bool:
            {
                var chk = new CheckBox { VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(6, 0, 0, 0) };
                _bools[f.Key] = chk;
                AddSimpleRow(section, f, chk);
                break;
            }
            case Kind.String:
            {
                var box = new HcTextBox
                {
                    Width = 220,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                _strings[f.Key] = box;
                AddSimpleRow(section, f, box);
                break;
            }
            case Kind.Style:
            {
                _styleCombo = new HcComboBox { Width = 160, HorizontalAlignment = HorizontalAlignment.Left };
                _styleCombo.Items.Add(new ComboBoxItem { Content = "普通", Tag = (int)ShortNumberStyle.Normal });
                _styleCombo.Items.Add(new ComboBoxItem { Content = "中文字（万/亿…）", Tag = (int)ShortNumberStyle.ChineseCharacter });
                _styleCombo.Items.Add(new ComboBoxItem { Content = "科学计数", Tag = (int)ShortNumberStyle.Science });
                _styleCombo.SelectedIndex = 0;
                AddSimpleRow(section, f, _styleCombo);
                break;
            }
            case Kind.PunishDay:
            {
                string[] days = ["周一", "周二", "周三", "周四", "周五", "周六", "周日"];
                _dayCombo = new HcComboBox { Width = 160, HorizontalAlignment = HorizontalAlignment.Left };
                for (int i = 0; i < days.Length; i++)
                {
                    _dayCombo.Items.Add(new ComboBoxItem { Content = days[i], Tag = i + 1 });
                }
                _dayCombo.SelectedIndex = -1;
                AddSimpleRow(section, f, _dayCombo);
                break;
            }
            case Kind.PunishTime:
            {
                _punishDate = new HcDatePicker { Width = 140, HorizontalAlignment = HorizontalAlignment.Left };
                _punishTime = new HcTextBox
                {
                    Width = 80,
                    Margin = new Thickness(6, 0, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Left,
                };
                var timePanel = new StackPanel { Orientation = Orientation.Horizontal };
                timePanel.Children.Add(_punishDate);
                timePanel.Children.Add(_punishTime);
                AddSimpleRow(section, f, timePanel);
                break;
            }
            case Kind.ListString:
            case Kind.ListLong:
                AddListRow(section, f, f.Kind == Kind.ListLong);
                break;
        }
    }

    private static void AddSimpleRow(StackPanel section, Field f, UIElement control)
    {
        var row = new Grid { Margin = new Thickness(2, 3, 2, 3) };
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(170) });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        var label = new TextBlock
        {
            Text = f.Label,
            VerticalAlignment = VerticalAlignment.Center,
            TextTrimming = TextTrimming.CharacterEllipsis,
        };
        Grid.SetColumn(label, 0);

        Grid.SetColumn(control, 1);

        var hint = new TextBlock
        {
            Text = f.Hint ?? string.Empty,
            Foreground = Brushes.Gray,
            FontSize = 12,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(10, 0, 0, 0),
            TextWrapping = TextWrapping.Wrap,
        };
        Grid.SetColumn(hint, 2);

        row.Children.Add(label);
        row.Children.Add(control);
        row.Children.Add(hint);
        section.Children.Add(row);
    }

    private void AddListRow(StackPanel section, Field f, bool longs)
    {
        var list = new ListBox
        {
            Height = 76,
            Margin = new Thickness(0, 4, 0, 0),
            HorizontalContentAlignment = HorizontalAlignment.Stretch,
        };
        _lists[f.Key] = list;

        var input = new HcTextBox { Width = 240, HorizontalAlignment = HorizontalAlignment.Left };
        var addButton = new Button { Content = "添加", Width = 56, Height = 26, Margin = new Thickness(6, 0, 0, 0) };
        var removeButton = new Button { Content = "删除", Width = 56, Height = 26, Margin = new Thickness(6, 0, 0, 0) };

        addButton.Click += (_, _) => AddListItem(f, list, input, longs);
        removeButton.Click += (_, _) =>
        {
            if (list.SelectedItem is not null)
            {
                list.Items.Remove(list.SelectedItem);
            }
        };

        var buttons = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 4, 0, 0) };
        buttons.Children.Add(input);
        buttons.Children.Add(addButton);
        buttons.Children.Add(removeButton);

        var block = new StackPanel { Margin = new Thickness(2, 4, 2, 8) };
        var header = new TextBlock
        {
            Text = f.Label,
            Margin = new Thickness(2, 0, 0, 0),
        };
        var hint = new TextBlock
        {
            Text = f.Hint ?? string.Empty,
            Foreground = Brushes.Gray,
            FontSize = 12,
            Margin = new Thickness(2, 0, 0, 0),
        };
        block.Children.Add(header);
        block.Children.Add(hint);
        block.Children.Add(buttons);
        block.Children.Add(list);
        section.Children.Add(block);
    }

    private static void AddListItem(Field f, ListBox list, HcTextBox input, bool longs)
    {
        var text = (input.Text ?? string.Empty).Trim();
        if (text.Length == 0)
        {
            Growl.Warning("请先输入要添加的内容");
            return;
        }

        if (longs)
        {
            if (!TryParseLong(text, out var value))
            {
                Growl.Error($"输入无效: {f.Label}");
                return;
            }
            if (list.Items.Contains(value))
            {
                Growl.Warning("列表中已存在该值");
                return;
            }
            list.Items.Add(value);
        }
        else
        {
            if (list.Items.Contains(text))
            {
                Growl.Warning("列表中已存在该项");
                return;
            }
            list.Items.Add(text);
        }

        input.Text = string.Empty;
    }

    /// <summary>先按当前区域文化解析 long，再按不变文化</summary>
    private static bool TryParseLong(string text, out long value)
    {
        return long.TryParse(text, NumberStyles.Integer, CultureInfo.CurrentCulture, out value)
            || long.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
    }

    /// <summary>解析 "HH:mm" 或 "HH:mm:ss"（区域文化感知的数字）</summary>
    private static bool TryParseTime(string text, out int hour, out int minute)
    {
        hour = 0;
        minute = 0;
        var parts = (text ?? string.Empty).Trim().Split(':');
        if (parts.Length is not (2 or 3))
        {
            return false;
        }

        bool okHour = int.TryParse(parts[0], NumberStyles.Integer, CultureInfo.CurrentCulture, out hour)
                      || int.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out hour);
        bool okMinute = int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.CurrentCulture, out minute)
                        || int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out minute);
        return okHour && okMinute && hour is >= 0 and <= 23 && minute is >= 0 and <= 59;
    }

    // ---------- 读值 / 保存 ----------

    /// <summary>把当前配置快照重新读入所有控件（热重载后刷新用）</summary>
    private void ReloadValues()
    {
        var config = CoreConfiguration.Current;
        if (config is null)
        {
            return;
        }

        foreach (var f in Fields)
        {
            var raw = f.Get(config);
            switch (f.Kind)
            {
                case Kind.Int:
                case Kind.Double:
                    _nums[f.Key].Value = Convert.ToDouble(raw, CultureInfo.InvariantCulture);
                    break;
                case Kind.Bool:
                    _bools[f.Key].IsChecked = (bool)raw;
                    break;
                case Kind.String:
                    _strings[f.Key].Text = (string)raw ?? string.Empty;
                    break;
                case Kind.Style:
                    _styleCombo.SelectedIndex = (int)(ShortNumberStyle)raw;
                    break;
                case Kind.PunishDay:
                {
                    var day = Convert.ToInt32(raw, CultureInfo.InvariantCulture);
                    _dayCombo.SelectedIndex = day is >= 1 and <= 7 ? day - 1 : -1;
                    break;
                }
                case Kind.PunishTime:
                {
                    var t = (DateTime)raw;
                    if (t > new DateTime(1900, 1, 1))
                    {
                        _punishDate.SelectedDate = t.Date;
                        _punishTime.Text = t.ToString("HH:mm");
                    }
                    else
                    {
                        _punishDate.SelectedDate = DateTime.Today;
                        _punishTime.Text = "00:00";
                    }
                    break;
                }
                case Kind.ListString:
                case Kind.ListLong:
                {
                    var list = _lists[f.Key];
                    list.Items.Clear();
                    foreach (var item in (System.Collections.IEnumerable)raw)
                    {
                        list.Items.Add(item);
                    }
                    break;
                }
            }
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
        string invalid = null;

        foreach (var f in Fields)
        {
            if (!TryReadField(f, out var value, ref invalid))
            {
                break;
            }
            values[f.Key] = value;
        }

        if (invalid is not null)
        {
            Growl.Error($"输入无效: {invalid}");
            return;
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

    private bool TryReadField(Field f, out object value, ref string invalid)
    {
        value = null;
        switch (f.Kind)
        {
            case Kind.Int:
            {
                var v = _nums[f.Key].Value;
                if (double.IsNaN(v) || double.IsInfinity(v) || v != Math.Truncate(v) || v < f.Min || v > f.Max)
                {
                    invalid = f.Label;
                    return false;
                }
                value = (int)v;
                break;
            }
            case Kind.Double:
            {
                var v = _nums[f.Key].Value;
                if (double.IsNaN(v) || double.IsInfinity(v) || v < f.Min || v > f.Max)
                {
                    invalid = f.Label;
                    return false;
                }
                value = v;
                break;
            }
            case Kind.Bool:
                value = _bools[f.Key].IsChecked;
                break;
            case Kind.String:
                value = _strings[f.Key].Text ?? string.Empty;
                break;
            case Kind.Style:
                if (_styleCombo.SelectedIndex < 0)
                {
                    invalid = f.Label;
                    return false;
                }
                value = (ShortNumberStyle)_styleCombo.SelectedIndex;
                break;
            case Kind.PunishDay:
                if (_dayCombo.SelectedIndex < 0)
                {
                    invalid = f.Label;
                    return false;
                }
                value = _dayCombo.SelectedIndex + 1;
                break;
            case Kind.PunishTime:
            {
                var date = _punishDate.SelectedDate ?? DateTime.Today;
                if (!TryParseTime(_punishTime.Text, out var hour, out var minute))
                {
                    invalid = f.Label;
                    return false;
                }
                value = new DateTime(date.Year, date.Month, date.Day, hour, minute, 0);
                break;
            }
            case Kind.ListString:
                value = _lists[f.Key].Items.Cast<object>().Select(o => o?.ToString() ?? string.Empty).ToList();
                break;
            case Kind.ListLong:
                value = _lists[f.Key].Items.Cast<object>().Select(o => Convert.ToInt64(o, CultureInfo.InvariantCulture)).ToList();
                break;
        }
        return true;
    }
}
