using me.cqp.luohuaming.iKun.Sdk.Cqp;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace me.cqp.luohuaming.iKun.PublicInfos.Models
{
    public class RandomPunish
    {
        public RandomPunish()
        {
            TargetTime = CalcNextPunishDateTime();
            MainSave.CQLog.Info("天罚线程", $"下次天罚时间：{TargetTime:G}");
            Instance = this;
        }

        private Task RunningTask { get; set; }

        private CancellationTokenSource RunningTaskCancellationToken { get; set; } = new();

        public DateTime TargetTime { get; set; }

        public static Logger Logger { get; set; } = new Logger("天罚");

        public static RandomPunish Instance { get; set; }

        public void Start()
        {
            Stop();
            RunningTaskCancellationToken = new();
            if (!AppConfig.EnableRandomPunish)
            {
                return;
            }
            RunningTask = new Task(async () =>
            {
                while (!RunningTaskCancellationToken.IsCancellationRequested)
                {
                    if (DateTime.Now > TargetTime)
                    {
                        ExecutePunishment();
                        TargetTime = CalcNextPunishDateTime();
                        MainSave.CQLog.Info("天罚线程", $"天罚生效，下次天罚时间：{TargetTime:G}");
                        Logger.Info($"天罚生效，下次天罚时间：{TargetTime:G}");
                    }
                    await Task.Delay(1000);
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
            }
        }

        private DateTime CalcNextPunishDateTime()
        {
            DateTime dateTime = DateTime.Now;
            DayOfWeek dayOfWeek = AppConfig.ValueRandomPunishExecuteDay == 7 ? DayOfWeek.Sunday
                                 : (DayOfWeek)AppConfig.ValueRandomPunishExecuteDay;
            for (int i = 0; i <= 7; i++)
            {
                if (dateTime.DayOfWeek == dayOfWeek)
                {
                    break;
                }
                dateTime = dateTime.AddDays(1);
            }
            var target = AppConfig.ValueRandomPunishExecuteTime;
            dateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, target.Hour, target.Minute, target.Second);
            if (dateTime < DateTime.Now)
            {
                return dateTime.AddDays(7);
            }
            return dateTime;
        }

        private void ExecutePunishment()
        {
            Logger.Info("天罚开始");

            Kun targetKun = GetTargetKun();
            if (targetKun == null)
            {
                Logger.Error("未抽到目标鲲");
                return;
            }
            if (!GetKunParentGroupAndQQ(targetKun, out long groupId, out long qq))
            {
                Logger.Error($"ID={targetKun.Id}，未能获取鲲所属的群 ID 与 QQ");
                return;
            }

            targetKun.Initialize();
            double random = CommonHelper.Random.NextDouble();
            Logger.Info($"天罚判定随机数={random}，临界={AppConfig.ValueRandomPunishProbablity / 100.0}");
            if (random >= AppConfig.ValueRandomPunishProbablity / 100.0)
            {
                SendEnableGroupMessage(string.Format(AppConfig.ReplyRandomPunishSkipped));
                Logger.Info("天罚结束，未执行");

                return;
            }
            double originWeight = targetKun.Weight;
            random = CommonHelper.Random.NextDouble(AppConfig.ValueRandomPunishMinimalDecrement, AppConfig.ValueRandomPunishMaximalDecrement) / 100;
            targetKun.Weight *= (1 - random);
            Logger.Info($"体重损失百分比={random}，减后体重为={targetKun.Weight}");

            random = CommonHelper.Random.NextDouble();
            Logger.Info($"死亡判定随机数={random}，临界={AppConfig.ValueRandomPunishDeadProbablity / 100.0}");
            if (random < AppConfig.ValueRandomPunishDeadProbablity / 100.0)
            {
                targetKun.Alive = false;
                targetKun.DeadAt = DateTime.Now;
                targetKun.Update();
                SendGroupMessage(groupId, string.Format(AppConfig.ReplyRandomPunishFinishedAndDead,
                    CQApi.CQCode_At(qq), targetKun));
                Logger.Info("天罚结束，鲲已死亡");

                return;
            }
            targetKun.Update();
            SendGroupMessage(groupId, string.Format(AppConfig.ReplyRandomPunishFinished,
                    CQApi.CQCode_At(qq), targetKun, (originWeight - targetKun.Weight).ToShortNumber(), targetKun.Weight.ToShortNumber()));
            Logger.Info($"天罚结束");
        }

        private void SendGroupMessage(long groupId, string msg)
        {
            MainSave.CQApi.SendGroupMessage(groupId, msg);
        }

        private void SendEnableGroupMessage(string msg)
        {
            foreach (var item in AppConfig.Groups)
            {
                MainSave.CQApi.SendGroupMessage(item, msg);
                Thread.Sleep(1000 * 60);
            }
        }

        private static Kun GetTargetKun()
        {
            var list = Kun.GetAllAliveKun().OrderByDescending(x => x.Weight).ToList();
            var totalWeight = list.Sum(x => x.Weight);
            var random = CommonHelper.Random.NextDouble(0, totalWeight);

            double sumWeight = 0;
            Kun targetKun = null;
            foreach (var item in list)
            {
                sumWeight += item.Weight;
                if (sumWeight >= random)
                {
                    targetKun = item;
                    break;
                }
            }

            return targetKun;
        }

        private static bool GetKunParentGroupAndQQ(Kun kun, out long groupId, out long qq)
        {
            groupId = 0;
            qq = 0;
            var record = Record.GetRecordByKunID(kun.Id);
            if (record == null)
            {
                return false;
            }
            groupId = record.Group;
            qq = record.QQ;
            return true;
        }
    }
}
