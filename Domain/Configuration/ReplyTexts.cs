namespace me.cqp.luohuaming.iKun.Domain.Configuration;

/// <summary>
/// 回复文案段。键名与旧版 Config.json 完全兼容。
/// </summary>
public sealed class ReplyTexts
{
    // 注册/签到
    public string DuplicateRegister { get; private set; } = "你已经注册过了，不能重复注册";

    public string RegisterSuccess { get; private set; } = "注册成功，赠送 {0} 枚金币以及 {1} 枚鲲蛋";

    public string RegisterFailed { get; private set; } = "注册失败了，查看日志排查问题";

    public string DuplicateLogin { get; private set; } = "你今天已经签到过了，不能重复签到";

    public string LoginSuccess { get; private set; } = "签到成功，赠送 {0} 枚金币以及 {1} 枚鲲蛋";

    public string Menu { get; private set; } =
        "功能列表：\n{0}  {1}\n{2}  {3}\n{4}  {5}\n{6}  {7}\n{8}  {9}\n{10}  {11}\n{12}  {13}\n{14}  {15}\n{16}  {17}";

    // 通用
    public string NoPlayer { get; private set; } = "请先注册";

    public string NoKun { get; private set; } = "未持有鲲";

    public string TargetKunNotFound { get; private set; } = "目标所指的鲲不存在";

    public string TargetPlayerNotRegistered { get; private set; } = "目标指定的玩家未注册";

    public string TargetPlayerHasNoKun { get; private set; } = "目标指定的玩家未持有鲲";

    public string KunNotOwned { get; private set; } = "这只鲲不是你的{0}";

    public string KunAbandoned { get; private set; } = "鲲已被标记为弃置{0}";

    public string KunAlive { get; private set; } = "鲲未死亡{0}";

    public string KunDead { get; private set; } = "鲲已死亡{0}";

    public string ParamInvalid { get; private set; } = "指令格式错误{0}";

    public string ItemLeak { get; private set; } = "{0}数量不足，需要{1}个，现有{2}个";

    public string WeightLimitReached { get; private set; } = "体重已达上限，需进行渡劫提高体重上限";

    // 鲲展示
    public string RankingHeader { get; private set; } = "排行如下：";

    public string RankingGroupHeader { get; private set; } = "群排行如下：";

    public string KunToString { get; private set; } = "[%PetAttributeA%] %PetAttributeB%%PetAttributeC%鲲 %LongLevel%";

    public string KunNickNameToString { get; private set; } = "%PetNickName% %LongLevel%";

    public string RankingIdleTag { get; private set; } = "[挂机中]";

    public string RankingWorkTag { get; private set; } = "[打工中]";

    // 孵化
    public string DuplicateHatch { get; private set; } = "你已经有一只鲲了，不能重复孵化";

    public string HatchFailed { get; private set; } = "孵化失败\n-------------------\n剩余 {0} 颗鲲蛋";

    public string HatchSingleSuccess { get; private set; } = "恭喜你获得一只{0}\n体重 {1} 千克\n-------------------\n剩余 {2} 颗鲲蛋";

    public string HatchMultiSuccess { get; private set; } = "恭喜你获得一只{0}\n体重 {1} 千克\n-------------------\n共消耗 {2} 个蛋 剩余 {3} 颗鲲蛋";

    // 喂养/强化
    public string FeedResult { get; private set; } = "你的「{0}」体重增加了 {1} 千克\n现体重为 {2} 千克\n-------------------\n剩余 {3} 枚金币，{4} 枚鲲蛋";

    public string UpgradeSuccess { get; private set; } = "强化完成，体重增加了 {0} kg，当前体重 {1} kg\n剩余 {2} 颗强化丹 {3} 枚金币";

    public string UpgradeFailed { get; private set; } = "强化失败，体重减少了 {0} kg，当前体重 {1} kg\n剩余 {2} 颗强化丹 {3} 枚金币";

    // 渡劫
    public string AscendWeightBelowLimit { get; private set; } = "无法渡劫，由于体重未达到上限\n当前体重 {0} kg，上限体重 {1} kg";

    public string AscendSuccess { get; private set; } = "渡劫成功，体重增加了 {0} kg，等级加一\n当前体重 {1} kg，当前等级 {2}";

    public string AscendFailed { get; private set; } = "渡劫失败，体重减少了 {0} kg，当前体重 {1} kg";

    public string AscendDied { get; private set; } = "渡劫失败，你的鲲已死亡";

    public string ConsumeAscendPill { get; private set; } = "使用了 {0} 个渡劫丹，下次渡劫成功率 +{1}%";

    // 复活/放生
    public string DuplicateResurrect { get; private set; } = "你已经有一只鲲了，不能执行复活";

    public string ResurrectHourLimit { get; private set; } = "无法复活，由于鲲死亡已超过 {0} 小时，当前死亡 {1} 小时";

    public string ResurrectSuccess { get; private set; } = "鲲已复活，死亡时间 {0}，复活次数 {1}\n未复活期间，共损失了 {2} kg，{3} 星级\n消耗 {4} 个复活丸，还剩余 {5} 个复活丸";

    public string ResurrectFailed { get; private set; } = "复活失败，消耗 {0} 个复活丸，还剩余 {1} 个复活丸，查看日志查询原因";

    public string ReleaseSuccess { get; private set; } = "{0}放生成功";

    public string ReleaseFailed { get; private set; } = "放生失败，可能是鲲已死亡或不存在";

    public string DeadKunsHeader { get; private set; } = "还可复活的鲲列表如下：\n";

    // 幻化
    public string TransmogrifyLevelLimit { get; private set; } = "不能执行幻化，由于等级限制，当前等级 {0}，最低幻化等级：{1}";

    public string TransmogrifySuccess { get; private set; } = "幻化成功，{0} 转变为 {1}，体重减少 {2} kg，现 {3} kg\n剩余 {4} 颗幻化丹 {5} 枚金币";

    public string TransmogrifyFailed { get; private set; } = "幻化失败了，体重减少 {0} kg，现 {1} kg\n剩余 {2} 颗幻化丹 {3} 枚金币";

    public string TransmogrifyDied { get; private set; } = "幻化失败并且魂飞魄散\n剩余 {0} 颗幻化丹 {1} 枚金币";

    // 攻击
    public string AttackSelf { get; private set; } = "不能自己攻击自己";

    public string AttackCooldown { get; private set; } = "攻击冷却中，下次可攻击时间：{0}";

    public string AttackSuccess { get; private set; } = "{0} 对 {1}的{2} 发起攻击，攻击成功了！\n攻击方体重增长 {3} kg，现 {4} kg\n被攻击方体重减少 {5} kg，现 {6} kg";

    public string AttackFailed { get; private set; } = "{0} 对 {1}的{2} 发起攻击，攻击失败了！\n攻击方体重减少 {3} kg，现 {4} kg\n被攻击方体重增加 {5} kg，现 {6} kg";

    public string AttackEscaped { get; private set; } = "{0} 对 {1}的{2} 发起攻击，对方逃脱了！";

    public string AttackTargetDied { get; private set; } = "{0} 对 {1}的{2} 发起攻击，攻击成功了！被攻击方伤重致死\n攻击方体重增长 {3} kg，现 {4} kg";

    public string AttackerDied { get; private set; } = "{0} 对 {1}的{2} 发起攻击，攻击失败了！自身伤重致死\n被攻击方体重增加 {3} kg，现 {4} kg";

    // 吞噬
    public string DevourSelf { get; private set; } = "不能自己攻击自己";

    public string DevourCooldown { get; private set; } = "吞噬冷却中，下次可吞噬时间：{0}";

    public string DevourSuccess { get; private set; } = "{0} 吃掉了 {1}的{2}\n攻击方体重增长 {3} kg，现 {4} kg";

    public string DevourFailed { get; private set; } = "{0} 企图吃掉 {1}的{2}，但是失败了！\n攻击方体重减少 {3} kg，现 {4} kg\n被攻击方体重增加 {5} kg，现 {6} kg";

    public string DevourEscaped { get; private set; } = "{0} 企图吃掉 {1}的{2}，对方逃脱了！";

    public string DevouredByOther { get; private set; } = "{0} 企图吃掉 {1}的{2}，但是失败了！反倒被对方吃掉\n被攻击方体重增加 {3} kg，现 {4} kg";

    // 跨群广播
    public string CrossGroupAttacked { get; private set; } = "{0} 你的鲲在另一个群被攻击了，损失 {1} kg，还剩 {2} kg";

    public string CrossGroupAttackedToDeath { get; private set; } = "{0} 你的鲲在另一个群被攻击致死";

    public string CrossGroupAttackEscaped { get; private set; } = "{0} 你的鲲在另一个群被尝试攻击，但是你的鲲逃脱了";

    public string CrossGroupDevoured { get; private set; } = "{0} 你的鲲在另一个群被吞噬了";

    public string CrossGroupDevourEscaped { get; private set; } = "{0} 你的鲲在另一个群被尝试吞噬，但是你的鲲逃脱了";

    // 商店/物品
    public string ShopHeader { get; private set; } = "商店列表如下：\n-------------------";

    public string ShopEntry { get; private set; } = "%Index%：%CoinCount% %CoinName% => %ItemCount% %ItemName%";

    public string ShopIndexInvalid { get; private set; } = "购买失败，序号不存在";

    public string PurchaseSuccess { get; private set; } = "购买成功，消耗 {0} 枚{1}，获得 {2} 枚{3}\n-------------------\n剩余{1} {4} 枚，当前{3}共{5}枚";

    public string OpenEggResult { get; private set; } = "打开 {0} 个鲲蛋，获得了 {1} 个盲盒";

    public string BlindBoxOpened { get; private set; } = "打开 {0} 个盲盒，获得了以下物品：\n{1}";

    public string BlindBoxEmpty { get; private set; } = "打开 {0} 个盲盒，什么也没获得";

    public string InventoryEmpty { get; private set; } = "仓库为空";

    public string ItemCannotUse { get; private set; } = "此物品无法通过这种方式使用";

    public string ItemUseFailed { get; private set; } = "物品使用失败，数量已返还";

    // 挂机/打工
    public string IdleStarted { get; private set; } = "挂机开始！\n预计结束时间 {0} 预计获得体重 {1} kg";

    public string WorkStarted { get; private set; } = "打工开始！\n预计结束时间 {0} 预计获得金币 {1} 个";

    public string IdleFinished { get; private set; } = "挂机完成！\n你的 {0} 共挂机了 {1} 小时，获得了 {2} kg体重，当前体重 {3} kg";

    public string IdleFinishedButDead { get; private set; } = "挂机完成！\n你的 {0} 共挂机了 {1} 小时，获得了 {2} kg体重\n但是却因走火入魔而暴毙！";

    public string WorkFinished { get; private set; } = "打工完成！\n你的 {0} 共挂机了 {1} 小时，获得了 {2} 个金币，当前金币 {3} 个";

    public string KunIdling { get; private set; } = "{0} 正在挂机中";

    public string KunWorking { get; private set; } = "{0} 正在打工中";

    public string KunNotIdling { get; private set; } = "{0} 未在挂机";

    public string KunNotWorking { get; private set; } = "{0} 未在打工";

    public string IdleStartBlocked { get; private set; } = "，无法开始挂机";

    public string IdleCooldown { get; private set; } = "挂机暂时不可用，下次可挂机时间：{0}";

    public string WorkCooldown { get; private set; } = "打工暂时不可用，下次可打工时间：{0}";

    // 昵称
    public string NickNameApplied { get; private set; } = "自定义昵称已生效，你的鲲被命名为 {0}";

    public string NickNameDiscarded { get; private set; } = "自定义昵称抛弃，你的鲲名称恢复为 {0}";

    public string NickNameInvalid { get; private set; } = "昵称存在非法词汇，无法使用";

    // 天罚
    public string PunishInfo { get; private set; } = "每{0}会从所有鲲中抽选一个赐予天罚，体重越大抽到的概率更大\n下次天罚时间 {1}";

    public string PunishSkipped { get; private set; } = "本周天罚无事";

    public string PunishExecuted { get; private set; } = "天罚降下，{0} 被五雷轰顶，{1} 损失了 {2} kg，现在体重 {3} kg";

    public string PunishExecutedAndDied { get; private set; } = "天罚降下，{0} 被五雷轰顶，{1} 直接暴毙！";

    internal void ReloadFrom(CoreConfiguration config)
    {
        DuplicateRegister = config.Get("ReplyDuplicateRegister", DuplicateRegister);
        RegisterSuccess = config.Get("ReplyNewRegister", RegisterSuccess);
        RegisterFailed = config.Get("ReplyRegisterFailed", RegisterFailed);
        DuplicateLogin = config.Get("ReplyLoginDuplicate", DuplicateLogin);
        LoginSuccess = config.Get("ReplyLoginReward", LoginSuccess);
        Menu = config.Get("ReplyMenu", Menu);
        NoPlayer = config.Get("ReplyNoPlayer", NoPlayer);
        NoKun = config.Get("ReplyNoKun", NoKun);
        TargetKunNotFound = config.Get("ReplyNoTargrtKun", TargetKunNotFound);
        TargetPlayerNotRegistered = config.Get("ReplyNoTargetPlayer", TargetPlayerNotRegistered);
        TargetPlayerHasNoKun = config.Get("ReplyNoTargetKun", TargetPlayerHasNoKun);
        KunNotOwned = config.Get("ReplyKunOwnerNotMatch", KunNotOwned);
        KunAbandoned = config.Get("ReplyKunAbandoned", KunAbandoned);
        KunAlive = config.Get("ReplyKunAlive", KunAlive);
        KunDead = config.Get("ReplyKunNotAlive", KunDead);
        ParamInvalid = config.Get("ReplyParamInvalid", ParamInvalid);
        ItemLeak = config.Get("ReplyItemLeak", ItemLeak);
        WeightLimitReached = config.Get("ReplyWeightLimit", WeightLimitReached);

        RankingHeader = config.Get("ReplyRankingHeader", RankingHeader);
        RankingGroupHeader = config.Get("ReplyRankingGroupHeader", RankingGroupHeader);
        KunToString = config.Get("ReplyKunToString", KunToString);
        KunNickNameToString = config.Get("ReplyKunNickNameToString", KunNickNameToString);
        RankingIdleTag = config.Get("ReplyRankingAutoPlaying", RankingIdleTag);
        RankingWorkTag = config.Get("ReplyRankingWorking", RankingWorkTag);

        DuplicateHatch = config.Get("ReplyDuplicateHatch", DuplicateHatch);
        HatchFailed = config.Get("ReplyHatchFail", HatchFailed);
        HatchSingleSuccess = config.Get("ReplyHatchKun", HatchSingleSuccess);
        HatchMultiSuccess = config.Get("ReplyMultiHatchKun", HatchMultiSuccess);

        FeedResult = config.Get("ReplyFeed", FeedResult);
        UpgradeSuccess = config.Get("ReplyUpgradeSuccess", UpgradeSuccess);
        UpgradeFailed = config.Get("ReplyUpgradeFail", UpgradeFailed);

        AscendWeightBelowLimit = config.Get("ReplyAscendNoWeightLimit", AscendWeightBelowLimit);
        AscendSuccess = config.Get("ReplyAscendSuccess", AscendSuccess);
        AscendFailed = config.Get("ReplyAscendFail", AscendFailed);
        AscendDied = config.Get("ReplyAscendFailAndDead", AscendDied);
        ConsumeAscendPill = config.Get("ReplyConsumeAscendPill", ConsumeAscendPill);

        DuplicateResurrect = config.Get("ReplyDuplicateResurrect", DuplicateResurrect);
        ResurrectHourLimit = config.Get("ReplyResurrectHourLimit", ResurrectHourLimit);
        ResurrectSuccess = config.Get("ReplyResurrectSuccess", ResurrectSuccess);
        ResurrectFailed = config.Get("ReplyResurrectFail", ResurrectFailed);
        ReleaseSuccess = config.Get("ReplyReleaseSuccess", ReleaseSuccess);
        ReleaseFailed = config.Get("ReplyReleaseFail", ReleaseFailed);
        DeadKunsHeader = config.Get("ReplyQueryDeadKun", DeadKunsHeader);

        TransmogrifyLevelLimit = config.Get("ReplyTransmogrifyLevelLimit", TransmogrifyLevelLimit);
        TransmogrifySuccess = config.Get("ReplyTransmogrifySuccess", TransmogrifySuccess);
        TransmogrifyFailed = config.Get("ReplyTransmogrifyFail", TransmogrifyFailed);
        TransmogrifyDied = config.Get("ReplyTransmogrifyFailAndDead", TransmogrifyDied);

        AttackSelf = config.Get("ReplyAttackSelf", AttackSelf);
        AttackCooldown = config.Get("ReplyAttackInCD", AttackCooldown);
        AttackSuccess = config.Get("ReplyAttackSuccess", AttackSuccess);
        AttackFailed = config.Get("ReplyAttackFail", AttackFailed);
        AttackEscaped = config.Get("ReplyAttackEscaped", AttackEscaped);
        AttackTargetDied = config.Get("ReplyAttackSuccessAndTargetDead", AttackTargetDied);
        AttackerDied = config.Get("ReplyAttackFailAndDead", AttackerDied);

        DevourSelf = config.Get("ReplyDevourSelf", DevourSelf);
        DevourCooldown = config.Get("ReplyDevourInCD", DevourCooldown);
        DevourSuccess = config.Get("ReplyDevourSuccess", DevourSuccess);
        DevourFailed = config.Get("ReplyDevourFail", DevourFailed);
        DevourEscaped = config.Get("ReplyDevourEscaped", DevourEscaped);
        DevouredByOther = config.Get("ReplyDevourFailAndDead", DevouredByOther);

        CrossGroupAttacked = config.Get("ReplyAttackedNotSameGroup", CrossGroupAttacked);
        CrossGroupAttackedToDeath = config.Get("ReplyAttackedNotSameGroupAndDead", CrossGroupAttackedToDeath);
        CrossGroupAttackEscaped = config.Get("ReplyAttackedNotSameGroupButEscaped", CrossGroupAttackEscaped);
        CrossGroupDevoured = config.Get("ReplyDevouredNotSameGroup", CrossGroupDevoured);
        CrossGroupDevourEscaped = config.Get("ReplyDevouredNotSameGroupButEscaped", CrossGroupDevourEscaped);

        ShopHeader = config.Get("ReplyShoppingHeader", ShopHeader);
        ShopEntry = config.Get("ReplyShoppingDetail", ShopEntry);
        ShopIndexInvalid = config.Get("ReplyItemCannotBuy", ShopIndexInvalid);
        PurchaseSuccess = config.Get("ReplyBuyItem", PurchaseSuccess);
        OpenEggResult = config.Get("ReplyOpenKunEgg", OpenEggResult);
        BlindBoxOpened = config.Get("ReplyBlindBoxOpen", BlindBoxOpened);
        BlindBoxEmpty = config.Get("ReplyBlindBoxGetNothing", BlindBoxEmpty);
        InventoryEmpty = config.Get("ReplyEmptyInventory", InventoryEmpty);
        ItemCannotUse = config.Get("ReplyItemCannotUse", ItemCannotUse);
        ItemUseFailed = config.Get("ReplyItemUseFailed", ItemUseFailed);

        IdleStarted = config.Get("ReplyAutoPlayStarted", IdleStarted);
        WorkStarted = config.Get("ReplyWorkingStarted", WorkStarted);
        IdleFinished = config.Get("ReplyAutoPlayFinished", IdleFinished);
        IdleFinishedButDead = config.Get("ReplyAutoPlayFinishedButDead", IdleFinishedButDead);
        WorkFinished = config.Get("ReplyWorkingFinished", WorkFinished);
        KunIdling = config.Get("ReplyAutoPlaying", KunIdling);
        KunWorking = config.Get("ReplyWorking", KunWorking);
        KunNotIdling = config.Get("ReplyNotAutoPlaying", KunNotIdling);
        KunNotWorking = config.Get("ReplyNotWorking", KunNotWorking);
        IdleStartBlocked = config.Get("ReplyStartAutoPlayFailed", IdleStartBlocked);
        IdleCooldown = config.Get("ReplyAutoPlayInCD", IdleCooldown);
        WorkCooldown = config.Get("ReplyWorkingInCD", WorkCooldown);

        NickNameApplied = config.Get("ReplyCustomNickApplied", NickNameApplied);
        NickNameDiscarded = config.Get("ReplyCustomNickDiscarded", NickNameDiscarded);
        NickNameInvalid = config.Get("ReplyCustomNickInvalid", NickNameInvalid);

        PunishInfo = config.Get("ReplyRandomPunish", PunishInfo);
        PunishSkipped = config.Get("ReplyRandomPunishSkipped", PunishSkipped);
        PunishExecuted = config.Get("ReplyRandomPunishFinished", PunishExecuted);
        PunishExecutedAndDied = config.Get("ReplyRandomPunishFinishedAndDead", PunishExecutedAndDied);
    }
}