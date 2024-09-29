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
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int ID { get; set; }

        public int KunID { get; set; }

        public long GroupId { get; set; }

        public double Duration { get; set; } = 8;

        public DateTime StartTime { get; set; } = DateTime.Now;

        public DateTime EndTime { get; set; } = new();

        public bool Running { get; set; }

        [SugarColumn(IsIgnore = true)]
        private Task RunningTask { get; set; }

        [SugarColumn(IsIgnore = true)]
        private CancellationTokenSource RunningTaskCancellationToken { get; set; }

        public static event Action<AutoPlay> AutoPlayFinished;

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

        public static void LoadAutoPlays()
        {
            RunningAutoPlay = GetAllRunningAutoPlay();
            foreach (var autoPlay in RunningAutoPlay)
            {
                autoPlay.Start();
            }
        }

        public void Start()
        {
            Stop();
            SetTaskRunning(this, true);
            RunningTask = new Task(() =>
            {
                while (!RunningTaskCancellationToken.IsCancellationRequested)
                {
                    if (DateTime.Now > (StartTime + TimeSpan.FromHours(Duration)))
                    {
                        AutoPlayFinished?.Invoke(this);
                        SetTaskRunning(this, false);
                        break;
                    }
                    Task.Delay(1000);
                }
            }, RunningTaskCancellationToken.Token);
            RunningTask.Start();
        }

        public void Stop()
        {
            if (RunningTask != null)
            {
                RunningTaskCancellationToken.Cancel();
                RunningTask.Wait();
                SetTaskRunning(this, false);
            }
        }

        public static void SetTaskRunning(AutoPlay task, bool running)
        {
            task.Running = running;
            if (!running)
            {
                task.EndTime = DateTime.Now;
                RunningAutoPlay.Remove(task);
            }
            else
            {
                task.StartTime = DateTime.Now;
                task.EndTime = new();
                RunningAutoPlay.Add(task);
            }
            var db = SQLHelper.GetInstance();
            db.Updateable(task).ExecuteCommand();
        }

        public static void AddAutoPlay(AutoPlay autoPlay)
        {
            var db = SQLHelper.GetInstance();
            db.Insertable(autoPlay).ExecuteCommand();

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

        public double CalcAutoPlayExp()
        {
            Kun kun = Kun.GetKunByID(KunID);
            int level = kun.Level;
            int expSpeed = level switch
            {
                1 => 10,
                2 => 100,
                3 => 1000,
                4 => 7000,
                5 => 30000,
                6 => 100000,
                _ => 0
            };
            return expSpeed * ((EndTime - StartTime).TotalHours / 24.0);
        }
    }
}