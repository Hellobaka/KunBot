using me.cqp.luohuaming.iKun.PublicInfos.Models.Results;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace me.cqp.luohuaming.iKun.PublicInfos.Models
{
    [SugarTable]
    public class AutoPlay
    {
        private static object loadLock = new();

        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int ID { get; set; }

        public int KunID { get; set; }

        public long GroupId { get; set; }

        public double Duration { get; set; } = 8;

        public DateTime StartTime { get; set; } = DateTime.Now;

        public DateTime EndTime { get; set; } = new();

        public bool Running { get; set; }

        [SugarColumn(IsIgnore = true)]
        public static Logger Logger { get; set; } = new Logger("挂机管理");

        [SugarColumn(IsIgnore = true)]
        private Task RunningTask { get; set; }

        [SugarColumn(IsIgnore = true)]
        private CancellationTokenSource RunningTaskCancellationToken { get; set; }

        public static event Action<AutoPlay, AutoPlayResult, Kun> AutoPlayFinished;

        private static List<AutoPlay> RunningAutoPlay { get; set; } = null;

        public static List<AutoPlay> GetAllRunningAutoPlay()
        {
            var db = SQLHelper.GetInstance();
            return db.Queryable<AutoPlay>().Where(x => x.Running).ToList();
        }

        public static List<AutoPlay> GetAllRunningAutoPlayByGroupId(long groupId)
        {
            var db = SQLHelper.GetInstance();
            return db.Queryable<AutoPlay>().Where(x => x.Running && x.GroupId == groupId).ToList();
        }

        public static AutoPlay? GetKunAutoPlay(Kun kun, bool fromDB = false)
        {
            if (!fromDB && RunningAutoPlay != null)
            {
                return RunningAutoPlay.FirstOrDefault(x => x.KunID == kun.Id);
            }
            var db = SQLHelper.GetInstance();
            return db.Queryable<AutoPlay>().Where(x => x.KunID == kun.Id).OrderByDescending(x => x.StartTime).First();
        }

        public static void LoadAutoPlays()
        {
            RunningAutoPlay = GetAllRunningAutoPlay();
            lock (loadLock)
            {
                foreach (var autoPlay in RunningAutoPlay)
                {
                    autoPlay.Start();
                }
            }
        }

        public void Start()
        {
            Stop();
            SetTaskRunning(this, true);
            RunningTaskCancellationToken = new();
            RunningTask = new Task(async () =>
            {
                while (!RunningTaskCancellationToken.IsCancellationRequested)
                {
                    if (DateTime.Now > (StartTime + TimeSpan.FromHours(Duration)))
                    {
                        SetTaskRunning(this, false);
                        AutoPlayFinished?.Invoke(this, SettleAutoPlayResult(out Kun kun), kun);
                        Logger.Info($"挂机完成，ID={ID}，时长={Duration}");
                        break;
                    }
                    await Task.Delay(1000);
                }
            }, RunningTaskCancellationToken.Token);
            RunningTask.Start();
        }

        public AutoPlayResult Stop()
        {
            if (RunningTask != null)
            {
                RunningTaskCancellationToken.Cancel();
                RunningTask.Wait();
                SetTaskRunning(this, false);

                return SettleAutoPlayResult(out _);
            }
            else
            {
                return null;
            }
        }

        public AutoPlayResult SettleAutoPlayResult(out Kun kun)
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
                    RunningAutoPlay.Remove(task);
                }
            }
            else
            {
                if (!RunningAutoPlay.Any(x => x.ID == task.ID))
                {
                    lock (loadLock)
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

        public static bool CheckKunAutoPlay(Kun kun)
        {
            if (RunningAutoPlay != null)
            {
                return RunningAutoPlay.Any(x => x.KunID == kun.Id);
            }
            var db = SQLHelper.GetInstance();
            return db.Queryable<AutoPlay>().Where(x => x.KunID == kun.Id).OrderByDescending(x => x.StartTime).First()?.Running ?? false;
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
                >= 6 and < 8 => Math.Pow(10, level - 1) / 20,
                >= 8 => Math.Pow(10, level - 1) / 100,
            };
            Logger.Info($"星级={level}，挂机经验速度={expSpeed}");
            return expSpeed * (endTime - startTime).TotalHours;
        }

        public static bool CheckAutoPlayInCD(Kun kun, out DateTime availableTime)
        {
            var autoPlay = GetKunAutoPlay(kun, true);
            availableTime = DateTime.Now;
            if (autoPlay == null)
            {
                return true;
            }
            availableTime = autoPlay.EndTime.AddHours(AppConfig.ValueAutoPlayCDHour);
            return availableTime < DateTime.Now;
        }
    }
}