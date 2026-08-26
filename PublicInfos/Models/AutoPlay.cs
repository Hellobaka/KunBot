using Another_Mirai_Native.Abstractions.Services;
using me.cqp.luohuaming.iKun.PublicInfos.Enums;
using me.cqp.luohuaming.iKun.PublicInfos.Models.Results;
using SqlSugar;

namespace me.cqp.luohuaming.iKun.PublicInfos.Models
{
    [SugarTable]
    public class AutoPlay
    {
        private static readonly object loadLock = new();

        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int ID { get; set; }

        public int KunID { get; set; }

        public long GroupId { get; set; }

        public double Duration { get; set; } = 8;

        public DateTime StartTime { get; set; } = DateTime.Now;

        public DateTime EndTime { get; set; } = new();

        public bool Running { get; set; }

        public AutoPlayType AutoPlayType { get; set; } = AutoPlayType.Exp;

        [SugarColumn(IsIgnore = true)]
        private static Logger Logger { get; set; } = new("挂机管理");

        [SugarColumn(IsIgnore = true)]
        private Task RunningTask { get; set; }

        [SugarColumn(IsIgnore = true)]
        private CancellationTokenSource RunningTaskCancellationToken { get; set; }

        /// <summary>
        /// 挂机完成事件参数：挂机任务、结算结果（可能为 null）、鲲
        /// </summary>
        public static event Action<AutoPlay, AutoPlayResult, Kun> AutoPlayFinished;

        private static List<AutoPlay> RunningAutoPlay { get; set; } = null;

        /// <summary>
        /// 用于在后台线程发送群消息
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        private static IMessageApi MessageApi { get; set; }

        public static List<AutoPlay> GetAllRunningAutoPlay()
        {
            var db = SQLHelper.GetInstance();
            return db.Queryable<AutoPlay>().Where(x => x.Running).ToList();
        }

        public static List<AutoPlay> GetAllRunningAutoPlayByGroupId(long groupId, AutoPlayType autoPlayType)
        {
            var db = SQLHelper.GetInstance();
            return db.Queryable<AutoPlay>().Where(x => x.Running && x.GroupId == groupId && x.AutoPlayType == autoPlayType).ToList();
        }

        public static AutoPlay GetKunAutoPlay(Kun kun, AutoPlayType autoPlayType, bool fromDB = false)
        {
            if (!fromDB && RunningAutoPlay != null)
            {
                return RunningAutoPlay.FirstOrDefault(x => x.KunID == kun.Id && x.AutoPlayType == autoPlayType);
            }
            var db = SQLHelper.GetInstance();
            return db.Queryable<AutoPlay>().Where(x => x.KunID == kun.Id && x.AutoPlayType == autoPlayType).OrderByDescending(x => x.StartTime).First();
        }

        public static void LoadAutoPlays(IMessageApi messageApi)
        {
            MessageApi = messageApi;
            RunningAutoPlay = GetAllRunningAutoPlay();
            lock (loadLock)
            {
                foreach (var autoPlay in RunningAutoPlay)
                {
                    autoPlay.Start();
                }
            }
            // 已超时但仍在运行的任务，立即触发结算
            foreach (var autoPlay in RunningAutoPlay.ToList())
            {
                if (DateTime.Now > autoPlay.StartTime.AddHours(autoPlay.Duration))
                {
                    SetTaskRunning(autoPlay, false);
                    SettleAndNotify(autoPlay);
                }
            }
        }

        public void Start()
        {
            Stop();
            SetTaskRunning(this, true);
            RunningTaskCancellationToken = new CancellationTokenSource();
            RunningTask = new Task(async () =>
            {
                while (!RunningTaskCancellationToken.IsCancellationRequested)
                {
                    if (DateTime.Now > (StartTime + TimeSpan.FromHours(Duration)))
                    {
                        SettleAndNotify(this);
                        break;
                    }
                    await Task.Delay(1000, RunningTaskCancellationToken.Token).ContinueWith(_ => { });
                }
            }, RunningTaskCancellationToken.Token);
            RunningTask.Start();
        }

        private static void SettleAndNotify(AutoPlay autoPlay)
        {
            SetTaskRunning(autoPlay, false);
            var r = autoPlay.SettleAutoPlayResult(out Kun kun);
            Logger.Info($"挂机完成，ID={autoPlay.ID}，时长={autoPlay.Duration}");
            try
            {
                AutoPlayFinished?.Invoke(autoPlay, r, kun);
            }
            catch (Exception e)
            {
                Logger.Error(e, "处理挂机完成事件过程中发生异常");
            }
        }

        public AutoPlayResult Stop()
        {
            if (RunningTask != null)
            {
                RunningTaskCancellationToken.Cancel();
                try
                {
                    RunningTask.Wait(5000);
                }
                catch
                {
                }
                SetTaskRunning(this, false);

                return SettleAutoPlayResult(out _);
            }
            else
            {
                return null;
            }
        }

        public static void StopAll()
        {
            lock (loadLock)
            {
                foreach (var autoPlay in RunningAutoPlay?.ToList() ?? [])
                {
                    try
                    {
                        autoPlay.RunningTaskCancellationToken?.Cancel();
                    }
                    catch
                    {
                    }
                }
                RunningAutoPlay?.Clear();
            }
        }

        public AutoPlayResult SettleAutoPlayResult(out Kun kun)
        {
            kun = null;
            return AutoPlayType switch
            {
                AutoPlayType.Exp => SettleAutoPlayExpResult(out kun),
                AutoPlayType.Coin => SettleAutoPlayCoinResult(out kun),
                _ => null,
            };
        }

        private AutoPlayResult SettleAutoPlayCoinResult(out Kun kun)
        {
            kun = Kun.GetKunByID(KunID);
            if (kun == null || !kun.Alive || kun.Abandoned)
            {
                Logger.Info($"目标鲲不存在或已死亡或已被抛弃");
                return null;
            }
            kun.Initialize();
            int increment = (int)CalcAutoPlayCoin(kun.Level, StartTime, EndTime);

            var player = Player.GetPlayer(kun.PlayerID);
            if (player == null)
            {
                Logger.Info($"未找到鲲对应的玩家");
                return null;
            }
            player.GiveItem([Items.Coin(increment)]);
            AutoPlayResult r = new()
            {
                CurrentCoin = InventoryItem.GetItemCount(player, Enums.Items.Coin),
                Duration = EndTime - StartTime,
                StartTime = StartTime,
                EndTime = EndTime,
                Increment = increment,
            };
            Logger.Info($"打工结束，开始时间={r.StartTime}，时长={r.Duration.TotalHours:f2}h，金币增加={r.Increment}");
            return r;
        }

        private AutoPlayResult SettleAutoPlayExpResult(out Kun kun)
        {
            kun = Kun.GetKunByID(KunID);
            if (kun == null || !kun.Alive || kun.Abandoned)
            {
                Logger.Info($"目标鲲不存在或已死亡或已被抛弃");
                return null;
            }
            kun.Initialize();
            var originalWeight = kun.Weight;
            var exp = CalcAutoPlayExp(kun.Level, StartTime, EndTime);

            kun.Weight += exp;
            kun.Weight = Math.Min(kun.Weight, Kun.GetLevelWeightLimit(kun.Level));
            double random = CommonHelper.Random.NextDouble();
            Logger.Info($"随机数={random}，死亡概率={AppConfig.ValueAutoPlayDeadProbablity}%");
            if (random < AppConfig.ValueAutoPlayDeadProbablity / 100)
            {
                Logger.Info("判定成功，鲲已死亡");
                kun.Alive = false;
                kun.DeadAt = DateTime.Now;
            }
            kun.Update();

            var r = new AutoPlayResult
            {
                CurrentWeight = kun.Weight,
                Duration = EndTime - StartTime,
                StartTime = StartTime,
                EndTime = EndTime,
                Dead = !kun.Alive,
                Increment = kun.Weight - originalWeight,
                WeightLimit = kun.Weight == Kun.GetLevelWeightLimit(kun.Level)
            };

            Logger.Info($"挂机结束，开始时间={r.StartTime}，时长={r.Duration.TotalHours:f2}h，经验增加={r.Increment}，是否死亡={r.Dead}");
            return r;
        }

        public static void SetTaskRunning(AutoPlay task, bool running)
        {
            if (task == null)
            {
                return;
            }
            task.Running = running;
            if (!running)
            {
                var taskEnd = task.StartTime.AddHours(task.Duration);
                if (taskEnd < DateTime.Now)
                {
                    task.EndTime = taskEnd;
                }
                else
                {
                    task.EndTime = DateTime.Now;
                }
                lock (loadLock)
                {
                    RunningAutoPlay?.Remove(task);
                }
            }
            else
            {
                lock (loadLock)
                {
                    RunningAutoPlay ??= [];
                    if (!RunningAutoPlay.Any(x => x.ID == task.ID))
                    {
                        RunningAutoPlay.Add(task);
                    }
                }
            }
            var db = SQLHelper.GetInstance();
            db.Updateable(task).ExecuteCommand();
        }

        public static void AddAutoPlay(AutoPlay autoPlay)
        {
            var db = SQLHelper.GetInstance();
            autoPlay.ID = db.Insertable(autoPlay).ExecuteReturnIdentity();

            autoPlay.Start();
            autoPlay.Running = true;
        }

        public static bool CheckKunAutoPlay(Kun kun, AutoPlayType autoPlayType = AutoPlayType.Exp)
        {
            lock (loadLock)
            {
                if (RunningAutoPlay != null)
                {
                    return RunningAutoPlay.Any(x => x.KunID == kun.Id && x.AutoPlayType == autoPlayType && x.Running);
                }
            }
            var db = SQLHelper.GetInstance();
            return db.Queryable<AutoPlay>().Where(x => x.KunID == kun.Id && x.AutoPlayType == autoPlayType).OrderByDescending(x => x.StartTime).First()?.Running ?? false;
        }

        public static double CalcAutoPlayExp(int level, DateTime startTime, DateTime endTime)
        {
            double expSpeed = level switch
            {
                <= 0 => 0,
                1 => 10,
                2 => 100,
                3 => 1000,
                4 => 7000,
                5 => 30000,
                >= 6 and < 8 => Math.Pow(10, level - 1) / 10,
                >= 8 => Math.Pow(10, level - 1) / 20,
            };
            Logger.Info($"星级={level}，挂机经验速度={expSpeed}");
            return expSpeed * (endTime - startTime).TotalHours;
        }

        public static double CalcAutoPlayCoin(int level, DateTime startTime, DateTime endTime)
        {
            double increment = (endTime - startTime).TotalHours * AppConfig.ValueWorkingCoinRewardPerHour;
            increment += increment * level * (AppConfig.ValueWorkLevelBouns / 100.0);

            Logger.Info($"星级={level}，挂机金币速度={increment}");
            return increment;
        }

        public static bool CheckAutoPlayInCD(Kun kun, AutoPlayType autoPlayType, out DateTime availableTime)
        {
            var autoPlay = GetKunAutoPlay(kun, autoPlayType, true);
            availableTime = DateTime.Now;
            if (autoPlay == null)
            {
                return true;
            }
            availableTime = autoPlay.EndTime.AddHours(autoPlayType == AutoPlayType.Exp ? AppConfig.ValueAutoPlayCDHour : AppConfig.ValueWorkingCDHour);
            return availableTime < DateTime.Now;
        }
    }
}
