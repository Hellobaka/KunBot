# Copilot Instructions for iKun

AMN2 (Another-Mirai-Native2) QQ 机器人插件「iKun 养鲲」，由酷Q 版 KunBot 移植而来。C# / .NET 10.0-windows 类库，构建产物为 `Native_iKun.dll` + 自动生成的 `Native_iKun.json` 清单。

## 构建与部署

```powershell
dotnet build          # 无测试、无 lint 配置；验证方式就是构建 + 运行时测试
```

- 构建成功标志：0 错误；输出位于 `bin\Debug\net10.0-windows\Native_iKun.dll|json`
- NU1903（SQLitePCLRaw 漏洞警告）来自框架锁定的依赖版本，属预期，勿升级修复
- 部署/调试：用 AMN2 测试 MCP 服务器工具，**不要手动复制文件**：`AMN2-add_plugin`（传入 bin 下的两个 Native_* 文件）→ `AMN2-enable_plugin`/`AMN2-reload_plugin` → `AMN2-send_message` 发 NoConnection 协议消息验证 → `AMN2-get_latest_logs` 查结果与异常堆栈
- 插件数据目录在框架侧：`<框架根>\data\app\me.cqp.luohuaming.iKun\`（Config.json / Items.json / data.db）

## 权威文档

**`llms.txt` 是框架开发指南**（AMN2 API 参考、错误模式清单）。改动涉及框架 API（PluginBase、CommandHandlerBase、Context、MessageBuilder、I*Api）前先查它。

## 架构（大图）

三层结构，跨文件才能看清：

1. **入口层** `Entry.cs` — 唯一的 `PluginBase` 子类。`OnEnableAsync` 按固定顺序初始化：`Runtime.Init`(API/路径装配)→加载双配置(热重载)→`Db.Initialize()` 建库→挂机任务恢复(IdleScheduler.ResumeFromDatabase + AutoPlaySettlementNotifier 订阅结算事件)→天罚服务(RandomPunishService)。所有框架访问经 `Infrastructure.Runtime.Api`。
2. **指令层** `Features/CommandRouter.cs` — 单个 `CommandHandlerBase` 承载全部指令。每个指令是 `[DynamicCommand(nameof(RouteXxx), MatchMode.Regex, MessageScope.Group)]` 方法；`RouteXxx` 属性每次调度时调用 `BuildPattern(CoreConfiguration.CommandXxx, withArgs)` 把配置中的触发词转成正则——这就是配置热重载后指令立即生效的机制。参数通过正则命名组 `args` 注入方法。Router 只做守卫(`Guard`)与转发，业务在 `Features/*Feature.cs`（单例静态 Instance）。
3. **领域层** `Domain/` — 与框架解耦的业务逻辑：
   - `Models/Kun.cs` 核心数值引擎（渡劫/攻击/吞噬/喂养/复活/幻化），只做计算+落库并返回 `Results/*Result` 对象，**不发送消息**；消息拼装在 Feature 层用 `Replies.*` 格式化
   - `PetAttributes/` 词缀系统：主词缀(Element 金木水火土风雷阴阳无)经 `AttributeAID` 存储、副词缀1/2 经 `AttributeBID/CID` 存储，链式修饰基础数值计算
   - `Background/IdleScheduler.cs` 挂机/打工计时循环 + 静态事件 `IdleFinished`；`Background/RandomPunishService.cs` 每周定时器
4. **基础设施层** `Infrastructure/` — `Persistence.Db`(SqlSugar 会话工厂+建表)、`Runtime`(API 与目录)、`Logging.Log`、`WebQQ/`（消息发送与群成员缓存）。

## 关键约定（易踩坑）

- **数据库结构必须与原版 KunBot_old 一致**：表名/列名不可擅改——`Player(QQ, CreateAt, LoginAt, AttackAt, DevourAt, AscendPillComsume)`、`Kun(Id, AttributeAID, AttributeBID, AttributeCID, PlayerID, ...)`、`InventoryItem(Id, PlayerID, ItemID, Count, Deleted)`、`AutoPlay(ID, KunID, GroupId, Duration, StartTime, EndTime, Running, AutoPlayType[int枚举 Exp=0])`、`Record(ID, Group, QQ, KunID)`。属性名即列名，改属性=改列，旧库直接失效。新表在 `Db.Initialize()` 注册 InitTables
- **群白名单前置检查**：每个群聊指令方法第一行必须经过 `if (!Guard(e)) return;`（内部 `CommandHelper.GroupEnabled(e)`）
- **命名组叫 `args`，不叫 `raw`**：框架中参数名 `raw` 是保留名（注入完整原始消息而非捕获组）；带参指令签名为 `(GroupMessageContext e, string args)`
- **框架类禁止有参构造**：`PluginBase`/`CommandHandlerBase` 及 handler 接口实现类由框架反射实例化，只能有无参构造；跨 handler 共享状态用静态属性（见 CoreConfiguration.Current、各 Feature.Instance）
- **文件路径**：插件自身文件一律 `Runtime.DataDirectory`（来自 `API.AppApi.GetAppDirectory()`），绝不写 CWD 或程序目录
- **At 码拼接**：纯文本消息中的 @ 用字符串拼接 CQ 码（保持 Reply 模板格式）；构造独立消息段才用 `MessageBuilder`
- **数值方法的锁纪律**：`Kun` 的计算方法先获取对象锁（双对象操作按序锁两把），所有随机数判定写入日志，出口统一落库并返回 Result 对象
- **词缀计算顺序不可变**：主词缀→副词缀1→副词缀2 依次调用，倍率乘算语义见 `IPetAttribute` 头部注释
- **SqlSugar 映射**：运行期字段必须 `[SugarColumn(IsIgnore = true)]`；表模型加 `[SugarTable]`
- **文案全部走配置**：回复文本来自 `CoreConfiguration.Replies` / `ItemConfiguration`（含 `{0}` 占位符或 `%Token%` 替换），不在代码里硬编码中文回复；新增配置键需同时在 `LoadConfig()` 提供 `Get(key, 默认值)` 以生成默认配置
- **NuGet 版本锁定**：必须精确版本且程序集版本与框架 `DependencyManifest-dotnet9.json` 完全一致才会被 ILRepack 去重。SqlSugar 必须用 `SqlSugarCore 5.1.4.211`（其程序集名为 SqlSugar 5.1.4.211 与框架一致）；升到 5.1.4.217 会导致 SqlSugar 整个内嵌进插件 DLL，框架反射扫描类型时因 Npgsql/MySqlConnector 等悬空引用抛 ReflectionTypeLoadException、插件加载失败
- **日志**：用 `Infrastructure.Logging.Log.For(tag)` 封装，不要直接引用框架 ILogger
