# Copilot Instructions for iKun

AMN2 (Another-Mirai-Native2) QQ 机器人插件「iKun 养鲲」，由酷Q 版 KunBot 移植而来。C# / .NET 10.0-windows 类库，构建产物为 `Native_iKun.dll` + 自动生成的 `Native_iKun.json` 清单。

## 构建与部署

```powershell
dotnet build          # 无测试、无 lint 配置；验证方式就是构建 + 运行时测试
```

- 构建成功标志：0 错误；输出位于 `bin\Debug\net10.0-windows\Native_iKun.dll|json`
- NU1903（SQLitePCLRaw 漏洞警告）来自框架锁定的依赖版本，属预期，勿升级修复
- 部署/调试：将两个 Native_* 文件放入框架 `data\plugins\`，或直接用 AMN2 测试 MCP 服务器工具（`AMN2-add_plugin` → `AMN2-reload_plugin` → `AMN2-send_message` 发 NoConnection 协议消息验证）
- 插件数据目录在框架侧：`<框架根>\data\app\me.cqp.luohuaming.iKun\`（Config.json / Items.json / data.db）

## 权威文档

**`llms.txt` 是框架开发指南**（AMN2 API 参考、错误模式清单）。改动涉及框架 API（PluginBase、CommandHandlerBase、Context、MessageBuilder、I*Api）前先查它。

## 架构（大图）

三层结构，跨文件才能看清：

1. **入口层** `Entry.cs` — 唯一的 `PluginBase` 子类。`OnEnableAsync` 按固定顺序初始化：加载双配置(热重载)→建库(SQLHelper)→`Kun.InitiazlizeRandom()`(词缀概率表)→挂起任务恢复(AutoPlay.LoadAutoPlays)→天罚线程(RandomPunish)。`MainSave.API` 是全局 `IPluginApi` 服务定位器，其余代码一律经它访问框架。
2. **指令层** `Commands.cs` — 单个 `CommandHandlerBase` 承载全部 29 个指令。每个指令是 `[DynamicCommand(nameof(TXxx), MatchMode.Regex, MessageScope.Group)]` 方法；`TXxx` 属性每次调度时调用 `BuildPattern(AppConfig.CommandXxx, withArgs)` 把配置中的触发词转成正则——这就是配置热重载后指令立即生效的机制。参数通过正则命名组 `args` 注入方法。
3. **领域层** `PublicInfos/` — 与框架解耦的业务逻辑：
   - `Models/Kun.cs` 核心数值引擎（渡劫/攻击/吞噬/喂养/复活/幻化/强化），只做计算+落库并返回 `Models/Results/*Result` 对象，**不发送消息**；消息拼装在 Commands 层用 `AppConfig.Reply*` 格式化
   - `PetAttribute/IPetAttribute.cs` 词缀系统：主词缀(AttributeA，金木水火土风雷阴阳无)与副词缀(AttributeB，80条映射表)通过虚方法链式修饰基础数值计算；`PetAttributeRandomInsatantiator` 按 AppConfig 概率随机实例化
   - `Models/AutoPlay.cs` 挂机/打工后台任务 + 静态事件 `AutoPlayFinished`（Entry 订阅并发群消息）；`RandomPunish.cs` 每周定时器线程

## 关键约定（易踩坑）

- **群白名单前置检查**：每个群聊指令方法第一行必须 `if (!GroupEnabled(e)) return Task.FromResult(EventHandleResult.Pass);`——Pass 让事件继续传播，Block 表示已处理
- **命名组叫 `args`，不叫 `raw`**：框架中参数名 `raw` 是保留名（注入完整原始消息而非捕获组）；带参指令签名为 `(GroupMessageContext e, string args)`
- **框架类禁止有参构造**：`PluginBase`/`CommandHandlerBase` 及 handler 接口实现类由框架反射实例化，只能有无参构造；跨 handler 共享状态用静态属性（见 MainSave、AppConfig）
- **文件路径**：插件自身文件一律 `MainSave.AppDirectory`（来自 `API.AppApi.GetAppDirectory()`），绝不写 CWD 或程序目录
- **At 码拼接**：纯文本消息中的 @ 用 `CommonHelper.CQCode_At(qq)` 字符串拼接（保持原版 Reply 模板格式）；构造独立消息段才用 `MessageBuilder`
- **数值方法的锁纪律**：`Kun` 的计算方法先 `Monitor.Enter(LockObject)`（双对象操作按序锁两把），所有随机数判定写入日志，出口统一 `Update()` 落库并返回 Result 对象
- **词缀计算顺序不可变**：主词缀→副词缀1→副词缀2 依次调用，倍率乘算语义见 `IPetAttribute` 头部注释
- **SqlSugar 映射**：运行期字段（LockObject、词缀对象等）必须 `[SugarColumn(IsIgnore = true)]`；表模型加 `[SugarTable]`，新表要在 `SQLHelper.CreateDB()` 注册 InitTables
- **文案全部走配置**：回复文本来自 `AppConfig.Reply*` / `ItemConfig.*`（含 `{0}` 占位符或 `%Token%` 替换），不在代码里硬编码中文回复；新增配置键需同时在 `LoadConfig()` 提供 `GetConfig(key, 默认值)` 以生成默认配置
- **NuGet 版本锁定**：必须精确版本且尽量与框架 DependencyManifest 一致（如 SqlSugarCore 5.1.4.211 程序集名 SqlSugar 与框架同名同版本→ILRepack 时去重），否则依赖被合并进插件 DLL
- **日志**：用 `PublicInfos.Logger` 封装（单参数），它内部转发 `MainSave.API.Logger(tag, message)`；不要直接引用框架 ILogger
