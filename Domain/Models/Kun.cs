using me.cqp.luohuaming.iKun.Domain.Configuration;
using me.cqp.luohuaming.iKun.Domain.Enums;
using me.cqp.luohuaming.iKun.Domain.PetAttributes;
using me.cqp.luohuaming.iKun.Domain.Results;
using me.cqp.luohuaming.iKun.Infrastructure;
using me.cqp.luohuaming.iKun.Infrastructure.Logging;
using me.cqp.luohuaming.iKun.Infrastructure.Persistence;
using SqlSugar;
using System.Text;

namespace me.cqp.luohuaming.iKun.Domain.Models;

/// <summary>
/// 鲲实体：数据库持久化 + 数值计算。
/// 计算方法只做计算与落库，返回 Result 对象由调用方格式化消息。
/// </summary>
[SugarTable]
public sealed class Kun
{
    private static readonly Log Log = Log.For("鲲");

    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }

    public bool Abandoned { get; set; }

    public bool Alive { get; set; }

    /// <summary>主词缀存储 ID</summary>
    public int AttributeAID { get; set; }

    /// <summary>副词缀1存储 ID</summary>
    public int AttributeBID { get; set; }

    /// <summary>副词缀2存储 ID</summary>
    public int AttributeCID { get; set; }

    public bool CanResurrect { get; set; } = true;

    public int Level { get; set; }

    public long PlayerID { get; set; }

    public int ResurrectCount { get; set; }

    public double Weight { get; set; }

    public DateTime DeadAt { get; set; }

    public string NickName { get; set; } = "";

    // ---- 运行期字段 ----

    private object SyncRoot { get; } = new();

    private PetAttribute? _mainAffix;

    private PetAttribute? _affix1;

    private PetAttribute? _affix2;

    /// <summary>渡劫丹带来的额外成功率（百分点）</summary>
    [SugarColumn(IsIgnore = true)]
    public double AscendBonusPercent { get; set; }

    /// <summary>等级对应的体重上限：10^level</summary>
    public static double WeightLimitOf(int level) => Math.Pow(10, level);

    // ---- 词缀解析 ----

    /// <summary>使用数值方法前必须调用；解析存储 ID 为词缀实例</summary>
    public void LoadAffixes()
    {
        _mainAffix = PetAttributeFactory.FromStoredId(true, AttributeAID);
        _affix1 = PetAttributeFactory.FromStoredId(false, AttributeBID);
        _affix2 = PetAttributeFactory.FromStoredId(false, AttributeCID);
    }

    private void EnsureAffixes()
    {
        if (_mainAffix is null || _affix1 is null || _affix2 is null)
        {
            throw new InvalidOperationException($"Kun#{Id} 未调用 LoadAffixes()");
        }
    }

    // ---- 渡劫 ----

    public AscendResult Ascend()
    {
        var config = CoreConfiguration.Current;
        try
        {
            Monitor.Enter(SyncRoot);
            Log.Info($"进入渡劫方法，ID={Id}，额外加成={AscendBonusPercent}");
            if (!Alive || Abandoned)
            {
                return Fail<AscendResult>("目标鲲已死亡或已被抛弃");
            }
            EnsureAffixes();
            double original = Weight;

            var rate = AscendBaseSuccessRate(Level);
            Log.Info($"基础成功率：{rate * 100}%");
            rate = _affix1!.ModifyAscendSuccessRate(_mainAffix!.ModifyAscendSuccessRate(rate));
            rate = _affix2!.ModifyAscendSuccessRate(rate);
            rate += AscendBonusPercent / 100;
            Log.Info($"最终成功率：{rate * 100}%");

            double multiplier = _mainAffix.ModifyAscendWeight(rate);
            multiplier = _affix1.ModifyAscendWeight(rate, multiplier);
            multiplier = _affix2.ModifyAscendWeight(rate, multiplier);

            Weight *= multiplier;
            bool success = multiplier >= 1;
            if (success)
            {
                Level++;
            }
            else
            {
                var deathRoll = Extensions.Rng.NextDouble();
                Log.Info($"渡劫失败，死亡判定：{deathRoll}，临界：{config.AscendFailDeathChance / 100}");
                if (deathRoll < config.AscendFailDeathChance / 100)
                {
                    Die("体重小于0");
                }
            }
            ClampWeight();
            Save();

            var result = new AscendResult
            {
                CurrentLevel = Level,
                CurrentWeight = Weight,
                WeightDelta = Weight - original,
                Died = !Alive,
            };
            Log.Info($"渡劫结束，倍率={multiplier}，体重={Weight}，等级={Level}，死亡={!Alive}");
            return result;
        }
        catch (Exception e)
        {
            Log.Error(e, "执行渡劫方法过程中发生异常");
            return new AscendResult { Success = false };
        }
        finally
        {
            Monitor.Exit(SyncRoot);
        }
    }

    /// <summary>渡劫基础成功率曲线</summary>
    private static double AscendBaseSuccessRate(int level) => level switch
    {
        <= 5 => 0.95 - (0.1 * (level - 1)),
        _ => 0.4 * Math.Exp(-0.4 * (level - 6)),
    };

    // ---- 攻击/吞噬 ----

    public AttackResult Attack(Kun target)
    {
        try
        {
            Monitor.Enter(SyncRoot);
            Monitor.Enter(target.SyncRoot);
            Log.Info($"进入攻击方法，ID={Id}，目标ID={target.Id}");
            if (!Alive || Abandoned || !target.Alive || target.Abandoned)
            {
                return Fail<AttackResult>("目标鲲已死亡或已被抛弃");
            }
            EnsureAffixes();
            target.EnsureAffixes();

            double original = Weight;
            double originalTarget = target.Weight;
            var elementMultiplier = ElementMultiplier(_mainAffix!.Element, target._mainAffix!.Element);

            var diff = _mainAffix.ModifyAttack(Weight, target.Weight, (1, 1), elementMultiplier);
            diff = target._mainAffix.ModifyBeingAttacked(Weight, target.Weight, diff);
            diff = _affix1!.ModifyAttack(Weight, target.Weight, diff, elementMultiplier);
            diff = target._affix1!.ModifyBeingAttacked(Weight, target.Weight, diff);
            diff = _affix2!.ModifyAttack(Weight, target.Weight, diff, elementMultiplier);
            diff = target._affix2!.ModifyBeingAttacked(Weight, target.Weight, diff);

            Weight *= diff.Item1;
            target.Weight *= diff.Item2;
            ClampWeight();
            target.ClampWeight();

            if (diff.Item2 < 1 && target.Weight < Weight * 0.1)
            {
                target.Die("被攻击方体重低于攻方10%");
            }
            if (Weight <= 0)
            {
                Die("攻击方体重小于0");
            }
            if (target.Weight <= 0)
            {
                target.Die("被攻击方体重小于0");
            }

            Save();
            target.Save();

            var result = new AttackResult
            {
                AttackerWeight = Weight,
                AttackerDied = !Alive,
                AttackerWeightDelta = Weight - original,
                HitWeightLimit = Weight == WeightLimitOf(Level),
                DefenderWeight = target.Weight,
                DefenderDied = !target.Alive,
                DefenderWeightDelta = originalTarget - target.Weight,
                Escaped = Math.Abs(diff.Item1 - 1) < 1e-9 && Math.Abs(diff.Item2 - 1) < 1e-9,
            };
            Log.Info($"攻击结束，{result}");
            return result;
        }
        catch (Exception e)
        {
            Log.Error(e, "执行攻击方法过程中发生异常");
            return new AttackResult { Success = false };
        }
        finally
        {
            Monitor.Exit(target.SyncRoot);
            Monitor.Exit(SyncRoot);
        }
    }

    public DevourResult Devour(Kun target)
    {
        try
        {
            Monitor.Enter(SyncRoot);
            Monitor.Enter(target.SyncRoot);
            Log.Info($"进入吞噬方法，ID={Id}，目标ID={target.Id}");
            if (!Alive || Abandoned || !target.Alive || target.Abandoned)
            {
                return Fail<DevourResult>("目标鲲已死亡或已被抛弃");
            }
            EnsureAffixes();
            target.EnsureAffixes();

            double original = Weight;
            double originalTarget = target.Weight;
            var elementMultiplier = ElementMultiplier(_mainAffix!.Element, target._mainAffix!.Element);

            var delta = _mainAffix.ModifyDevour(Weight, target.Weight, elementMultiplier);
            delta = target._mainAffix.ModifyBeingDevoured(target.Weight, Weight, delta);
            var affixNeutral = _affix1!.ModifyDevour(Weight, target.Weight).Multiply(delta);
            delta = target._affix1!.ModifyBeingDevoured(target.Weight, Weight, affixNeutral);
            affixNeutral = _affix2!.ModifyDevour(Weight, target.Weight).Multiply(delta);
            delta = target._affix2!.ModifyBeingDevoured(target.Weight, Weight, affixNeutral);

            Weight += delta.Item1;
            target.Weight += delta.Item2;
            ClampWeight();
            target.ClampWeight();

            if (delta.Item1 < 0)
            {
                // 吞噬失败概率死亡
                var deathRoll = Extensions.Rng.NextDouble();
                Log.Info($"吞噬失败，死亡判定：{deathRoll}，临界：{CoreConfiguration.Current.DevourFailDeathChance / 100}");
                if (deathRoll < CoreConfiguration.Current.DevourFailDeathChance / 100)
                {
                    Die("吞噬失败死亡");
                }
            }
            else if (Math.Abs(delta.Item1 - 1) < 1e-9 && Math.Abs(delta.Item2 - 1) < 1e-9)
            {
                Log.Info("对方鲲触发逃脱");
            }
            else
            {
                target.Die("吞噬成功");
            }
            if (Weight <= 0)
            {
                Die("吞噬方体重小于0");
            }
            if (target.Weight <= 0)
            {
                target.Die("被吞噬方体重小于0");
            }

            Save();
            target.Save();

            var result = new DevourResult
            {
                AttackerWeight = Weight,
                AttackerDied = !Alive,
                AttackerWeightDelta = Weight - original,
                HitWeightLimit = Weight == WeightLimitOf(Level),
                DefenderWeight = target.Weight,
                DefenderDied = !target.Alive,
                DefenderWeightDelta = target.Weight - originalTarget,
                Escaped = Math.Abs(delta.Item1 - 1) < 1e-9 && Math.Abs(delta.Item2 - 1) < 1e-9,
            };
            Log.Info($"吞噬结束，{result}");
            return result;
        }
        catch (Exception e)
        {
            Log.Error(e, "执行吞噬方法过程中发生异常");
            return new DevourResult { Success = false };
        }
        finally
        {
            Monitor.Exit(target.SyncRoot);
            Monitor.Exit(SyncRoot);
        }
    }

    /// <summary>主词缀元素克制倍率表</summary>
    private static double ElementMultiplier(Element attacker, Element defender)
    {
        Element[] basic = [Element.Metal, Element.Wood, Element.Water, Element.Fire, Element.Earth];
        int ai = Array.IndexOf(basic, attacker);
        int di = Array.IndexOf(basic, defender);

        // 五行相克：金克木→木克土→土克水→水克火→火克金
        if (ai >= 0 && di >= 0 && (di - ai + 5) % 5 == 1)
        {
            return 1.3;
        }
        // 风：克土火，弱于水金木
        if (attacker == Element.Wind && defender is Element.Earth or Element.Fire)
        {
            return 1.3;
        }
        if (attacker == Element.Wind && defender is Element.Water or Element.Metal or Element.Wood)
        {
            return 0.7;
        }
        if ((attacker == Element.Earth || attacker == Element.Fire) && defender == Element.Wind)
        {
            return 1.3;
        }
        // 雷：克水金木，弱于土火
        if (attacker == Element.Thunder && defender is Element.Water or Element.Metal or Element.Wood)
        {
            return 1.3;
        }
        if ((attacker == Element.Earth || attacker == Element.Fire) && defender == Element.Thunder)
        {
            return 0.7;
        }
        // 阴阳互克与对五行压制
        bool attackerIsBasic = basic.Contains(attacker) || attacker is Element.Wind or Element.Thunder;
        bool defenderIsBasic = basic.Contains(defender) || defender is Element.Wind or Element.Thunder;

        if (attacker == Element.Yin && defender == Element.Yang)
        {
            return 3;
        }
        if (attacker == Element.Yang && defender == Element.Yin)
        {
            return 3;
        }
        if (attacker is Element.Yin or Element.Yang && defenderIsBasic)
        {
            return attacker == Element.Yin ? 2 : 2;
        }
        if (defender is Element.Yin or Element.Yang && attackerIsBasic)
        {
            return 0.5;
        }
        // 有词缀打无词缀
        if (attacker != Element.None && defender == Element.None)
        {
            return 1.3;
        }
        return 1;
    }

    // ---- 喂养/强化 ----

    public FeedResult Feed(int count)
    {
        try
        {
            Monitor.Enter(SyncRoot);
            Log.Info($"进入喂养方法，ID={Id}，数量={count}");
            if (!Alive || Abandoned)
            {
                return Fail<FeedResult>("目标鲲已死亡或已被抛弃");
            }
            EnsureAffixes();
            double original = Weight;

            double bonusRate = _mainAffix!.ModifyFeed(count);
            bonusRate = _affix1!.ModifyFeed(count, bonusRate);
            bonusRate = _affix2!.ModifyFeed(count, bonusRate);

            Weight *= 1 + bonusRate;
            Weight += count * CoreConfiguration.Current.FeedWeightBaseIncrement;
            ClampWeight();
            Save();

            var result = new FeedResult
            {
                CurrentWeight = Weight,
                WeightDelta = Weight - original,
                HitWeightLimit = Weight == WeightLimitOf(Level),
            };
            Log.Info($"喂养结束，倍率={bonusRate}，体重={Weight}，增量={result.WeightDelta}");
            return result;
        }
        catch (Exception e)
        {
            Log.Error(e, "执行喂养方法过程中发生异常");
            return new FeedResult { Success = false };
        }
        finally
        {
            Monitor.Exit(SyncRoot);
        }
    }

    public UpgradeResult Upgrade(int count)
    {
        try
        {
            Monitor.Enter(SyncRoot);
            Log.Info($"进入强化方法，ID={Id}，数量={count}");
            if (!Alive || Abandoned)
            {
                return Fail<UpgradeResult>("目标鲲已死亡或已被抛弃");
            }
            EnsureAffixes();
            double original = Weight;

            var exp = IdleMath.ExperienceGainPerHour(Level) *
                      CoreConfiguration.Current.UpgradeExpHours * count;
            double multiplier = 1 + (exp / Weight);
            multiplier = _mainAffix!.ModifyUpgrade(count, multiplier);
            multiplier = _affix1!.ModifyUpgrade(count, multiplier);
            multiplier = _affix2!.ModifyUpgrade(count, multiplier);

            Weight *= multiplier;
            ClampWeight();
            Save();

            var result = new UpgradeResult
            {
                CurrentWeight = Weight,
                WeightDelta = Weight - original,
                HitWeightLimit = Weight == WeightLimitOf(Level),
            };
            Log.Info($"强化结束，倍率={multiplier}，体重={Weight}，增量={result.WeightDelta}");
            return result;
        }
        catch (Exception e)
        {
            Log.Error(e, "执行强化方法过程中发生异常");
            return new UpgradeResult { Success = false };
        }
        finally
        {
            Monitor.Exit(SyncRoot);
        }
    }

    // ---- 放生/复活 ----

    public bool Release()
    {
        try
        {
            Monitor.Enter(SyncRoot);
            Log.Info($"进入放生方法，ID={Id}");
            if (!Alive)
            {
                Log.Error("目标鲲已死亡");
                return false;
            }
            Abandoned = true;
            Save();
            return true;
        }
        catch (Exception e)
        {
            Log.Error(e, "执行放生方法过程中发生异常");
            return false;
        }
        finally
        {
            Monitor.Exit(SyncRoot);
        }
    }

    public ResurrectResult Resurrect()
    {
        try
        {
            Monitor.Enter(SyncRoot);
            Log.Info($"进入复活方法，ID={Id}，体重={Weight}，星级={Level}，死亡时间={DeadAt:G}");
            if (Abandoned)
            {
                return Fail<ResurrectResult>("目标鲲已被抛弃");
            }
            var config = CoreConfiguration.Current;
            Alive = true;
            ResurrectCount++;

            double originalWeight = Weight;
            int originalLevel = Level;
            double deadHours = (DateTime.Now - DeadAt).TotalHours;
            if (deadHours >= config.MaxResurrectHours)
            {
                Log.Error($"鲲死亡超过 {config.MaxResurrectHours} 小时，无法复活");
                return new ResurrectResult { Success = false };
            }

            int weightLossTicks = (int)(deadHours / 2);
            int levelLossTicks = (int)(deadHours / 18);
            for (int i = 0; i < weightLossTicks; i++)
            {
                Weight *= (100 - config.WeightLossPerTwoHoursPercent) / 100.0;
            }
            for (int i = 0; i < levelLossTicks; i++)
            {
                Level -= config.LevelLossPerEighteenHours;
            }
            Weight = Math.Max(WeightLimitOf(Level) * (config.ResurrectFloorPercent / 100.0), Weight);
            Level = Math.Max(Level, 1);
            Save();

            var result = new ResurrectResult
            {
                ResurrectCount = ResurrectCount,
                WeightLoss = originalWeight - Weight,
                LevelLoss = originalLevel - Level,
            };
            Log.Info($"退出复活方法，{result}");
            return result;
        }
        catch (Exception e)
        {
            Log.Error(e, "执行复活方法过程中发生异常");
            return new ResurrectResult { Success = false };
        }
        finally
        {
            Monitor.Exit(SyncRoot);
        }
    }

    // ---- 幻化 ----

    public TransmogrifyResult Transmogrify()
    {
        try
        {
            Monitor.Enter(SyncRoot);
            Log.Info($"进入幻化方法，ID={Id}");
            if (!Alive || Abandoned)
            {
                return FailTransmogrify("目标鲲已死亡或已被抛弃");
            }
            EnsureAffixes();
            var config = CoreConfiguration.Current;
            double originalWeight = Weight;

            var originalMain = PetAttributeFactory.FromStoredId(true, AttributeAID);
            var originalAffix1 = PetAttributeFactory.FromStoredId(false, AttributeBID);
            var originalAffix2 = PetAttributeFactory.FromStoredId(false, AttributeCID);

            double failRate = _affix1!.ModifyTransmogrifyFailRate(_mainAffix!.ModifyTransmogrifyFailRate(0.1));
            failRate = _affix2!.ModifyTransmogrifyFailRate(failRate);
            bool success = Extensions.Rng.NextDouble() > failRate;

            double keepRate = _affix1.ModifyTransmogrifyFailKeepRate(_mainAffix.ModifyTransmogrifyFailKeepRate(0.05));
            keepRate = _affix2.ModifyTransmogrifyFailKeepRate(keepRate);
            Weight *= keepRate;
            if (Weight < config.TransmogrifyDeathWeightLimit)
            {
                Die("幻化后体重低于临界点");
            }

            if (!success && Alive)
            {
                if (Extensions.Rng.NextDouble() < config.TransmogrifyFailDeathChance / 100)
                {
                    Die("幻化失败死亡判定");
                }
            }
            else if (Alive)
            {
                var newMain = PetAttributeFactory.CreateRandomMain();
                var newAffix1 = Affix.CreateRandom();
                var newAffix2 = Affix.CreateRandom();
                _mainAffix = newMain;
                _affix1 = newAffix1;
                _affix2 = newAffix2;
                AttributeAID = (int)newMain.Element;
                AttributeBID = newAffix1.AffixId;
                AttributeCID = newAffix2.AffixId;
            }
            Save();

            return new TransmogrifyResult
            {
                CurrentWeight = Weight,
                WeightLoss = originalWeight - Weight,
                Died = !Alive,
                CurrentMain = _mainAffix,
                CurrentAffix1 = _affix1,
                CurrentAffix2 = _affix2,
                OriginalMain = originalMain,
                OriginalAffix1 = originalAffix1,
                OriginalAffix2 = originalAffix2,
            };
        }
        catch (Exception e)
        {
            Log.Error(e, "执行幻化方法过程中发生异常");
            return FailTransmogrify(e.Message);
        }
        finally
        {
            Monitor.Exit(SyncRoot);
        }
    }

    private static TransmogrifyResult FailTransmogrify(string reason)
    {
        Log.Error(reason);
        return new TransmogrifyResult { Success = false, CurrentMain = PetAttributeFactory.FromStoredId(true, 0), OriginalMain = PetAttributeFactory.FromStoredId(true, 0) };
    }

    // ---- 内部辅助 ----

    private void ClampWeight() => Weight = Math.Min(Weight, WeightLimitOf(Level));

    private void Die(string reason)
    {
        Log.Info($"{reason}，ID={Id} 触发死亡");
        Alive = false;
        DeadAt = DateTime.Now;
    }

    public void Save()
    {
        using var db = Db.CreateSession();
        db.Updateable(this).ExecuteCommand();
    }

    private static T Fail<T>(string reason) where T : new()
    {
        Log.Error(reason);
        return new T { };
    }

    // ---- 展示 ----

    public override string ToString()
    {
        EnsureAffixes();
        var replies = CoreConfiguration.Current.Replies;
        var stars = new string('★', Math.Max(0, Level));
        if (!string.IsNullOrEmpty(NickName))
        {
            return replies.KunNickNameToString
                .Replace("%PetNickName%", NickName)
                .Replace("%LongLevel%", stars)
                .Replace("%ShortLevel%", $"{Level}★")
                .Replace("%Weight%", Weight.ToShortNumber());
        }
        return replies.KunToString
            .Replace("%PetAttributeA%", _mainAffix?.Name ?? "无")
            .Replace("%PetAttributeB%", _affix1?.Name ?? "")
            .Replace("%PetAttributeC%", _affix2?.Name ?? "")
            .Replace("%LongLevel%", stars)
            .Replace("%ShortLevel%", $"{Level}★")
            .Replace("%Weight%", Weight.ToShortNumber());
    }

    /// <summary>详细展示（含词缀描述与挂机/打工状态标签）</summary>
    public string ToDetailedString(bool showAttributes)
    {
        var replies = CoreConfiguration.Current.Replies;
        var builder = new StringBuilder();
        string idleTag = Background.IdleScheduler.IsRunning(Id, Enums.IdleType.Experience) ? $" {replies.RankingIdleTag}" : "";
        string workTag = Background.IdleScheduler.IsRunning(Id, Enums.IdleType.Coin) ? $" {replies.RankingWorkTag}" : "";
        builder.AppendLine(
            $"{this} {Weight.ToShortNumber()} {CoreConfiguration.Current.WeightUnit}{idleTag}{workTag}");
        if (showAttributes)
        {
            foreach (var line in (_mainAffix?.Description ?? []).Concat(_affix1?.Description ?? []).Concat(_affix2?.Description ?? []))
            {
                builder.AppendLine(line);
            }
        }
        builder.RemoveTrailingNewLine();
        return builder.ToString();
    }
}