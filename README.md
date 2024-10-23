# KunBot
## 配置
### 核心配置
| 键                                 | 描述                                       | 默认值                                      |
|----------------------------------|------------------------------------------|-------------------------------------------|
| EnableAt                         | 是否启用@功能                              | false                                     |
|ShortNumberType|短数字类型（0: 默认小数 1: 单位制 2: 科学记数法）|0|
| CommandAscend                    | 渡劫命令                                   | #渡劫                                      |
| CommandAttack                    | 攻击命令                                   | #攻击                                      |
| CommandBuyEgg                    | 买鲲蛋命令                                 | #买鲲蛋                                    |
| CommandDevour                    | 吞噬命令                                   | #吞噬                                      |
| CommandFeed                      | 喂养命令                                   | #喂养                                      |
| CommandHatch                     | 孵蛋命令                                   | #孵蛋                                      |
| CommandInventory                 | 背包命令                                   | #背包                                      |
| CommandLogin                     | 签到命令                                   | #签到                                      |
| CommandMenu                      | 菜单命令                                   | #鲲菜单                                    |
| CommandOpenBlindBox              | 开盲盒命令                                 | #开盲盒                                    |
| CommandOpenEgg                   | 开鲲蛋命令                                 | #开鲲蛋                                    |
| CommandRanking                   | 排行命令                                   | #排行                                      |
| CommandReleaseKun                | 放生命令                                   | #放生                                      |
| CommandResurrect                 | 复活命令                                   | #复活                                      |
| CommandTransmogrify              | 幻化命令                                   | #幻化                                      |
| CommandUpgrade                   | 强化命令                                   | #强化                                      |
| CommandRegister                  | 注册命令                                   | #注册                                      |
| CommandQueryDeadKuns             | 查询已死亡鲲命令                            | #查询已死亡鲲                               |
| ReplyDuplicateRegister           | 重复注册回复                               | 你已经注册过了，不能重复注册                           |
| ReplyDuplicateLogin              | 重复签到回复                               | 你今天已经签到过了，不能重复签到                         |
| ReplyDuplicateHatch              | 重复孵化回复                               | 你已经有一只鲲了，不能重复孵化                         |
| ReplyDuplicateResurrect          | 重复复活回复                               | 你已经有一只鲲了，不能执行复活                         |
| ReplyNewRegister                 | 新注册回复                                 | 注册成功，赠送 {0} 枚金币以及 {1} 枚鲲蛋             |
| ReplyRegisterFailed              | 注册失败回复                               | 注册失败了，查看日志排查问题                           |
| ReplyMenu                        | 菜单回复                                   | 功能列表：\n{0}  {1}\n{2}  {3}\n{4}  {5}\n{6}  {7}\n{8}  {9}\n{10}  {11}\n{12}  {13}\n{14}  {15}\n{16}  {17} |
| ReplyNoPlayer                    | 无玩家回复                                 | 请先注册                                     |
| ReplyNoKun                       | 无鲲回复                                   | 未持有鲲                                     |
| ReplyNoTargrtKun                 | 目标鲲不存在回复                             | 目标所指的鲲不存在                               |
| ReplyKunOwnerNotMatch            | 鲲主人不匹配回复                             | 这只鲲不是你的{0}                               |
| ReplyKunAbandoned                | 鲲被弃置回复                               | 鲲已被标记为弃置{0}                             |
| ReplyKunAlive                    | 鲲未死亡回复                               | 鲲未死亡{0}                                  |
| ReplyKunWeightZero               | 鲲体重为零回复                             | 鲲体重小于0{0}                                |
| ReplyKunNotAlive                 | 鲲已死亡回复                               | 鲲已死亡{0}                                  |
| ReplyLoginReward                 | 签到奖励回复                               | 签到成功，赠送 {0} 枚金币以及 {1} 枚鲲蛋         |
| ReplyItemLeak                    | 物品不足回复                               | {0}数量不足，需要{1}个，现有{2}个                |
| ReplyHatchFail                   | 孵化失败回复                               | 孵化失败\n-------------------\n剩余 {0} 颗鲲蛋    |
| ReplyHatchKun                    | 孵化成功回复                               | 恭喜你获得一只{0}\n体重 {1} 千克\n-------------------\n剩余 {2} 颗鲲蛋|
| ReplyMultiHatchKun               | 多次孵化成功回复                             | 恭喜你获得一只{0}\n体重 {1} 千克\n-------------------\n共消耗 {2} 个蛋 剩余 {3} 颗鲲蛋|
| ReplyBuyEgg                      | 购买鲲蛋回复                               | 购买成功，消耗金币 {0} 枚，获得 {1} 枚鲲蛋\n...   |
| ReplyAscendNoWeightLimit         | 渡劫失败体重未达上限回复                      | 无法渡劫，由于体重未达到上限\n当前体重 {0} kg，上限体重 {1} kg |
| ReplyAscendSuccess               | 渡劫成功回复                               | 购买成功，消耗金币 {0} 枚，获得 {1} 枚鲲蛋\n-------------------\n剩余金币 {2} 枚，鲲蛋 {3} 枚 |
| ReplyAscendFailAndDead           | 渡劫失败鲲死亡回复                           | 渡劫失败，你的鲲已死亡                             |
| ReplyAscendFail                  | 渡劫失败回复                               | 渡劫失败，体重减少了 {0} kg，当前体重 {1} kg      |
| ReplyParamInvalid                | 参数无效回复                               | 指令格式错误{0}                                 |
| ReplyNoTargetPlayer              | 目标玩家未注册回复                           | 目标指定的玩家未注册                               |
| ReplyNoTargetPlayerKun           | 目标玩家无鲲回复                             | 目标指定的玩家未持有鲲                             |
| ReplyAttackInCD                  | 攻击冷却中回复                             | 攻击冷却中，下次可攻击时间：{0}                     |
| ReplyDevourInCD                  | 吞噬冷却中回复                             | 吞噬冷却中，下次可吞噬时间：{0}                     |
| ReplyAttackSuccess               | 攻击成功回复                               | {0} 对 {1}的{2} 发起攻击，攻击成功了！\n攻击方体重增长 {3} kg，现 {4} kg\n被攻击方体重减少 {5} kg，现 {6} kg|
| ReplyAttackFail                  | 攻击失败回复                               | {0} 对 {1}的{2} 发起攻击，攻击失败了！\n攻击方体重减少 {3} kg，现 {4} kg\n被攻击方体重增加 {5} kg，现 {6} kg|
| ReplyAttackEscaped               | 攻击逃脱回复                               | {0} 对 {1}的{2} 发起攻击，对方逃脱了！             |
| ReplyAttackSuccessAndTargetDead  | 攻击成功目标死亡回复                         | {0} 对 {1}的{2} 发起攻击，攻击成功了！被攻击方伤重致死\n攻击方体重增长 {3} kg，现 {4} kg|
| ReplyAttackFailAndDead           | 攻击失败自身死亡回复                         | {0} 对 {1}的{2} 发起攻击，攻击失败了！自身伤重致死\n被攻击方体重增加 {3} kg，现 {4} kg|
| ReplyDevourSuccess               | 吞噬成功回复                               | {0} 吃掉了 {1}的{2}\n攻击方体重增长 {3} kg，现 {4} kg |
| ReplyDevourFail                  | 吞噬失败回复                               | {0} 企图吃掉 {1}的{2}，但是失败了！\n攻击方体重减少 {3} kg，现 {4} kg\n被攻击方体重增加 {5} kg，现 {6} kg |
| ReplyDevourFailAndDead           | 吞噬失败自身死亡回复                         | {0} 企图吃掉 {1}的{2}，但是失败了！反倒被对方吃掉\n被攻击方体重增加 {3} kg，现 {4} kg|
| ReplyDevourEscaped               | 吞噬逃脱回复                               | {0} 企图吃掉 {1}的{2}，对方逃脱了！                 |
| ReplyFeed                        | 喂养回复                                   | 你的「{0}」体重增加了 {1} 千克\n现体重为 {2} 千克\n-------------------\n剩余 {3} 枚金币，{4} 枚鲲蛋   |
| ReplyBlindBoxOpen                | 打开盲盒回复                               | 打开 {0} 个盲盒，获得了以下物品：\n{1}              |
| ReplyBlindBoxGetNothing          | 打开盲盒无物品回复                           | 打开 {0} 个盲盒，什么也没获得                       |
| ReplyOpenKunEgg                  | 打开鲲蛋回复                               | 打开 {0} 个鲲蛋，获得了 {1} 个盲盒                   |
| ReplyReleaseSuccess              | 放生成功回复                               | {0}放生成功                                    |
| ReplyReleaseFail                 | 放生失败回复                               | 放生失败，可能是鲲已死亡或不存在                      |
| ReplyQueryDeadKun                | 查询已死亡鲲回复                             | 还可复活的鲲列表如下：\n                           |
| ReplyTransmogrifySuccess         | 幻化成功回复                               | 幻化成功，{0} 转变为 {1}，体重减少 {2} kg，现 {3} kg\n剩余 {4} 颗幻化丹 {5} 枚金币 |
| ReplyTransmogrifyFail            | 幻化失败回复                               | 幻化失败了，体重减少 {0} kg，现 {1} kg\n剩余 {2} 颗幻化丹 {3} 枚金币|
| ReplyTransmogrifyFailAndDead     | 幻化失败自身死亡回复                         | 幻化失败并且魂飞魄散\n剩余 {0} 颗幻化丹 {1} 枚金币     |
| ReplyTransmogrifyLevelLimit      | 幻化等级限制回复                             | 不能执行幻化，由于等级限制，当前等级 {0}，最低幻化等级：{1} |
| ReplyUpgradeSuccess              | 强化成功回复                               | 强化完成，体重增加了 {0} kg，当前体重 {1} kg\n剩余 {2} 颗强化丹 {3} 枚金币|
| ReplyUpgradeFail                 | 强化失败回复                               | 强化失败，体重减少了 {0} kg，当前体重 {1} kg\n剩余 {2} 颗强化丹 {3} 枚金币 |
| ReplyResurrectHourLimit          | 复活时间限制回复                             | 无法复活，由于鲲死亡已超过 {0} 小时，当前死亡 {1} 小时     |
| ReplyResurrectSuccess            | 复活成功回复                               | 鲲已复活，死亡时间 {0}，复活次数 {1}\n未复活期间，共损失了 {2} kg，{3} 星级\n消耗 {4} 个复活丸，还剩余 {5} 个复活丸|
| ReplyResurrectFail               | 复活失败回复                               | 复活失败，消耗 {0} 个复活丸，还剩余 {1} 个复活丸，查看日志查询原因 |
| ReplyWeightLimit                 | 体重上限回复                               | 体重已达上限，需进行渡劫提高体重上限                    |
| ProbablityNone                   | 主词缀_无概率                                     | 70                                        |
| ProbablityJin                    | 主词缀_金概率                                     | 5                                         |
| ProbablityMu                     | 主词缀_木概率                                     | 5                                         |
| ProbablityShui                   | 主词缀_水概率                                     | 5                                         |
| ProbablityTu                     | 主词缀_土概率                                     | 5                                         |
| ProbablityHuo                    | 主词缀_火概率                                     | 5                                         |
| ProbablityFeng                   | 主词缀_风概率                                     | 2                                         |
| ProbablityLei                    | 主词缀_雷概率                                     | 2                                         |
| ProbablityYin                    | 主词缀_阴概率                                     | 0.5                                       |
| ProbablityYang                   | 主词缀_阳概率                                     | 0.5                                       |
| ValueHatchProbablityMin          | 孵化概率最小值                               | 10                                        |
| ValueHatchProbablityMax          | 孵化概率最大值                               | 50                                        |
| ValueHatchWeightMin              | 孵化体重最小值                               | 10                                        |
| ValueHatchWeightMax              | 孵化体重最大值                               | 10000                                     |
| ValueEggValue                    | 鲲蛋价值                                   | 100                                       |
| ValueFeedCoinConsume             | 喂养金币消耗                                | 10                                        |
| ValueAscendCoinConsume           | 渡劫金币消耗                                | 100                                       |
| ValueFeedKunEggConsume           | 喂养鲲蛋消耗                                | 1                                         |
| ValueFeedWeightBaseIncrement     | 喂养体重基础增量                             | 10                                        |
| ValueFeedWeightMinimumIncrement  | 喂养体重最小增量                             | 5                                         |
| ValueFeedWeightMaximumIncrement  | 喂养体重最大增量                             | 10                                        |
| ValueAttackWeightMinimumDecrement| 攻击体重最小减量                             | 5                                         |
| ValueAttackWeightMaximumDecrement| 攻击体重最大减量                             | 10                                        |
| ValueRankingCount                | 排行榜数量                                 | 10                                        |
| ValueDevourDrawPercentage        | 吞噬抽取百分比                               | 10                                        |
| ValueLoginCoinReward             | 登录金币奖励                                | 100                                       |
| ValueLoginEggReward              | 登录鲲蛋奖励                                | 10                                        |
| ValueAscendFailDeadProbablity    | 渡劫失败死亡概率                             | 10                                        |
| ValueDevourFailDeadProbablity    | 吞噬失败死亡概率                             | 20                                        |
| ValueTransmoirgifyDeadWeightLimit| 幻化失败死亡体重限制                          | 10                                        |
| ValueTransmoirgifyFailDeadProbablity | 幻化失败死亡概率                             | 10                                        |
| ValueAttackCD                    | 攻击冷却时间                                | 30                                        |
| ValueDevourCD                    | 吞噬冷却时间                                | 30                                        |
| ValueKunEggToBlindBoxRate        | 鲲蛋兑换盲盒比例                              | 1                                         |
| ValueMaxResurrectHour            | 最大复活时间                                | 81                                        |
| ValuePerTwoHourWeightLoss        | 复活时每两小时体重损失（%）                             | 1                                         |
| ValuePerEighteenHourLevelLoss    | 复活时每十八小时等级损失                           | 1                                         |
| ValueTranmogifyCoinConsume       | 幻化金币消耗                                | 100                                       |
| ValueTranmogifyPillConsume       | 幻化丹消耗                                  | 1                                         |
| ValueTransmogrifyLevelLimit      | 幻化等级限制                                | 5                                         |
| ValueUpgradeCoinConsume          | 强化金币消耗                                | 100                                       |
| ValueUpgradePillConsume          | 强化丹消耗                                  | 1                                         |
| ValueAscendWeightMinimalIncrement| 渡劫体重最小增量                             | 10                                        |
| ValueAscendWeightMaximalIncrement| 渡劫体重最大增量                             | 400                                       |
| ValueAscendWeightMinimalDecrement| 渡劫体重最小减量                             | 10                                        |
| ValueAscendWeightMaximalDecrement| 渡劫体重最大减量                             | 50                                        |
| BlindBoxContents                 | 盲盒内容(ID\|概率)                                   | ["0\|75", "4\|8", "5\|8", "6\|8"]             |
| Groups                           | 群组列表                                   | []                                        |
| Admins                           | 管理员列表                                 | []                                        |
|ReplyResurrectFailed|无法复活的文本|，无法复活|
|ReplyRankingHeader|体重排名的前置文本|排行如下：|
|ReplyEmptyInventory|仓库为空时的文本|仓库为空|
|ReplyAttackSelf|攻击自己时的文本|不能自己攻击自己|
|ReplyDevourSelf|吞噬自己时的文本|不能自己攻击自己|
|ReplyKunToString|输出鲲文本|[%PetAttributeA%] %PetAttributeB%鲲 %LongLevel%|
|ValueRegisterCoinReward|注册时给予的金币数量|500|
|ValueRegisterEggReward|注册时给予的鲲蛋数量|50|
|BlindBoxEnableMultiContents|盲盒可获得多个奖励开关|false|
|BlindBoxMultiContentMustHasItem|盲盒多个奖励必定获取物品|false|
|BlindBoxMultiContentProbablity|盲盒多个奖励触发概率|10|
|CommandStartAutoPlay|开始挂机的指令|#开始挂机|
|CommandStopAutoPlay|停止挂机的指令|#停止挂机|
|ReplyAutoPlayFinished|挂机完成的回复|挂机完成！\n你的 {0} 共挂机了 {1} 小时，获得了 {2} kg体重，当前体重 {3} kg|
|ReplyAutoPlayFinishedButDead|挂机结束但是暴毙的回复|挂机完成！\n你的 {0} 共挂机了 {1} 小时，获得了 {2} kg体重\n但是却因走火入魔而暴毙！|
|ReplyAutoPlayStarted|开始挂机的回复|挂机开始！\n预计结束时间 {0} 预计获得体重 {1} kg|
|ReplyAutoPlaying|正在挂机的回复|{0} 正在挂机中|
|ReplyNotAutoPlaying|未在挂机的回复|{0} 未在挂机|
|ReplyStartAutoPlayFailed|开始挂机失败的附加回复|，无法开始挂机|
|ReplyStopAutoPlayFailed|停止挂机失败的附加回复|，无法结束挂机|
|ValueAutoPlayDeadProbablity|挂机后暴毙的概率|5|
|ValueMaxAutoPlayDuration|最大可挂机时长(h)|24|
|EnableRandomPunish|是否启用天罚功能|False|
|WeightUnitBase|重量单位的基数，输出数值会除以这个值|1|
|WeightUnit|重量单位的名称|kg|
|CommandRandomPunish|获取天罚的描述|#天罚|
|ReplyRandomPunish|天罚指令的描述|每{0}会从所有鲲中抽选一个赐予天罚，体重越大抽到的概率更大\n下次天罚时间 {1}|
|ReplyRandomPunishFinished|天罚完成的回复|天罚降下，{0} 被五雷轰顶，{1} 损失了 {2} kg，现在体重 {3} kg|
|ReplyRandomPunishSkipped|天罚未发生的群发|本周天罚无事|
|ReplyRandomPunishFinishedAndDead|天罚致死的回复|天罚降下，{0} 被五雷轰顶，{1} 直接暴毙！|
|ValueRandomPunishProbablity|天罚成功的概率|80|
|ValueRandomPunishMinimalDecrement|天罚损失体重的最小百分比|50|
|ValueRandomPunishMaximalDecrement|天罚损失体重的最大百分比|80|
|ValueRandomPunishDeadProbablity|天罚致死的概率|10|
|ValueRandomPunishExecuteDay|天罚每周发生的周期（星期n）|4|
|ValueRandomPunishExecuteTime|天罚发生的时间（需保证格式不变，两位数字，仅T之后的时间部分生效）|0001-01-01T00:00:00|

### 物品配置
| 键                        | 描述                       | 默认值           |
|---------------------------|----------------------------|------------------|
| BlindBoxName              | 盲盒名称                   | 盲盒             |
| CoinName                  | 金币名称                   | 金币             |
| KunEggName                | 鲲之蛋名称                 | 鲲之蛋           |
| ResurrectPillName         | 复活丸名称                 | 复活丸           |
| TransmogrifyPillName      | 幻化丸名称                 | 幻化丸           |
| UpgradePillName           | 强化丸名称                 | 强化丸           |
| BlindBoxDescription       | 盲盒描述                   | 能获得随机材料   |
| CoinDescription           | 金币描述                   | 大陆上通用的货币 |
| KunEggDescription         | 鲲之蛋描述                 | 可用于孵化、强化鲲 |
| ResurrectPillDescription  | 复活丸描述                 | 用于复活的道具，能复活鲲 |
| TransmogrifyPillDescription| 幻化丸描述                 | 用于幻化的道具，能够随机更改鲲的词缀 |
| UpgradePillDescription    | 强化丸描述                 | 用于强化的道具，能用于强化鲲 |

### 物品ID
| ID                       | 物品名称                       |
|---------------------------|----------------------------|
|0|空|
|1|金币|
|2|鲲之蛋|
|3|盲盒|
|4|复活丸|
|5|幻化丸|
|6|强化丸|

### 格式化鲲输出
| 格式                                 | 描述                                       | 
|----------------------------------|------------------------------------------|
|%PetAttributeA%|主词缀名称|
|%PetAttributeB%|副词缀名称|
|%LongLevel%|完整星级|
|%ShortLevel%|数字+星级|
|%Weight%|体重|

### 挂机经验
| 等级                                | 每小时经验                                       | 满级所需时间|
|----------------------------------|------------------------------------------|----|
|1|10|1|
|2|100|1|
|3|1000|1|
|4|7000|1.42|
|5|30000|3.33|
|>=6 and < 8|0.05 * (10 ^ (等级 - 1))|200|
|>=8|0.01 * (10 ^ (等级 - 1))|1000|
