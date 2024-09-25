using me.cqp.luohuaming.iKun.PublicInfos.Items;
using me.cqp.luohuaming.iKun.PublicInfos.Models.Results;
using me.cqp.luohuaming.iKun.PublicInfos.PetAttribute;
using me.cqp.luohuaming.iKun.PublicInfos.PetAttribute.AttributeA;
using me.cqp.luohuaming.iKun.PublicInfos.PetAttribute.AttributeB;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace me.cqp.luohuaming.iKun.PublicInfos.Models
{
    [SugarTable]
    public class Kun
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        public int AttributeAID { get; set; }

        public int AttributeBID { get; set; }

        public long PlayerID { get; set; }

        public double Weight { get; set; }

        public bool Abandoned { get; set; }

        public int Level { get; set; }

        public bool Alive { get; set; }

        public int ResurrectCount { get; set; }

        public bool CanResurrect { get; set; }

        [SugarColumn(IsIgnore = true)]
        public IPetAttribute PetAttributeA { get; set; }

        [SugarColumn(IsIgnore = true)]
        public IPetAttribute PetAttributeB { get; set; }

        [SugarColumn(IsIgnore = true)]
        public object LockObject { get; set; } = new object();

        private static PetAttributeRandomInsatantiator RandomInsatantiator { get; set; } = null;

        private static Logger Logger { get; set; } = new Logger("鲲");

        #region 数值实例
        // 数值方法应当进行数值计算，并将实例属性更新，同步进数据库
        // 反抛给调用方数值变化，由调用方进行文本拼接
        // 为每种方法定义独立的结果类，包含执行结果、以及所有需要的变化数值
        // 词缀进行计算时，在日志中记录所有的随机数并且需要在所有出口记录返回值
        // 进行计算前需要获取对象锁，当涉及两个对象的处理时，需要获取两个对象的锁

        /// <summary>
        /// 强化
        /// 次数可以叠加成功率
        /// 成功率大于1时提升强化效果
        /// 在词缀中有基础实现
        /// </summary>
        public UpgradeResult Upgrade(int count)
        {
            try
            {
                Monitor.Enter(LockObject);
                Logger.Info($"进入强化方法，ID={Id}，数量={count}");
                double original = Weight;
                double diff = PetAttributeA.Upgrade(count);
                diff = PetAttributeB.Upgrade(count, diff);

                Weight *= diff;
                Weight = Math.Min(Weight, GetLevelWeightLimit(Level));
                Update();

                bool reachLimit = Weight == GetLevelWeightLimit(Level);
                Logger.Info($"强化方法结束，倍率={diff}，结果={Weight}，原始值={original}，变化值={Weight - original}，达到上限={reachLimit}");
                return new UpgradeResult
                {
                    CurrentWeight = Weight,
                    Increment = Weight - original,
                    WeightLimit = reachLimit
                };
            }
            catch (Exception e)
            {
                Logger.Error(e, "执行强化方法过程中发生异常");
                return new UpgradeResult { Success = false };
            }
            finally
            {
                Monitor.Exit(LockObject);
            }
        }

        /// <summary>
        /// 渡劫
        /// 前置要求体重达到上限
        /// 进行渡劫成功之后，体重才能突破上限，等级加一
        /// 在词缀中有基础实现
        /// </summary>
        public void Ascend()
        {
            double success = Level switch
            {
                1 => 0.95,
                2 => 0.90,
                3 => 0.85,
                4 => 0.75,
                5 => 0.65,
                6 => 0.50,
                7 => 0.35,
                8 => 0.20,
                _ => 0.10,
            };
            success = PetAttributeB.GetAscendSuccessRate(PetAttributeA.GetAscendSuccessRate(success));

            double diff = PetAttributeA.Ascend(success);
            diff = PetAttributeB.Ascend(success, diff);

            Weight *= diff;
            if (diff > 1)
            {
                Level++;
            }
            Weight = Math.Min(Weight, GetLevelWeightLimit(Level));
            Update();
        }

        /// <summary>
        /// 吞噬
        /// 体重相近时概率胜利
        /// 吞噬成功增加对方部分体重
        /// 吞噬失败减少自身体重并概率死亡
        /// 在词缀中有基础实现
        /// </summary>
        public void Devour(Kun target)
        {
            double baseAttackRate = GetBaseAttackRate(PetAttributeA, target.PetAttributeA);
            var weightDiff = PetAttributeA.Devour(Weight, target.Weight, baseAttackRate);
            weightDiff = target.PetAttributeA.BeingDevoured(target.Weight, Weight, weightDiff);

            weightDiff = target.PetAttributeB.Devour(Weight, target.Weight).Multiple(weightDiff);
            weightDiff = target.PetAttributeB.BeingDevoured(target.Weight, Weight, weightDiff).Multiple(weightDiff);

            Weight += weightDiff.Item1;
            target.Weight += weightDiff.Item2;

            Weight = Math.Min(Weight, GetLevelWeightLimit(Level));
            target.Weight = Math.Min(target.Weight, GetLevelWeightLimit(target.Level));

            Update();
            target.Update();
        }

        /// <summary>
        /// 喂养
        /// 在词缀中有基础实现
        /// 每个次数可增加5%~10%的体重
        /// </summary>
        public void Feed(int count)
        {
            double diff = PetAttributeA.Feed(count);
            diff = PetAttributeB.Feed(count, diff);

            Weight *= diff;
            Weight = Math.Min(Weight, GetLevelWeightLimit(Level));
            Update();
        }

        /// <summary>
        /// 放生
        /// </summary>
        public void Release()
        {
            Abandoned = true;
            Update();
        }

        /// <summary>
        /// 复活
        /// 默认完成道具扣除并满足条件
        /// 每复活一次，消耗的资源翻倍
        /// </summary>
        public void Resurrect()
        {
            Alive = true;
            ResurrectCount++;
            Update();
        }

        /// <summary>
        /// 幻化
        /// 默认实现了道具扣除并满足条件
        /// 随机主词缀与副词缀
        /// 每次扣除95%体重
        /// 小于10kg时死亡
        /// 10%概率失败，失败时死亡
        /// </summary>
        public void Transmogrify()
        {
            bool success = CommonHelper.Random.NextDouble() > PetAttributeB.GetTransmogrifyFailRate(PetAttributeA.GetTransmogrifyFailRate(0.1));
            Weight *= PetAttributeB.GetTransmogrifyFailWeightLostRate(PetAttributeA.GetTransmogrifyFailWeightLostRate(0.05));
            if (Weight < 10)
            {
                Alive = false;
                Update();
            }
            if (!success)
            {
                Alive = false;
            }
            else
            {
                PetAttributeA = RandomInsatantiator.GetRandomInstance();
                PetAttributeB = AttributeB.RandomCreate();

                AttributeAID = (int)PetAttributeA.ID;
                AttributeBID = (int)PetAttributeB.ID;
            }
            Update();
        }

        /// <summary>
        /// 攻击
        /// 被攻击方损失概率体重，攻击方增加对应体重
        /// 在词缀中有基础实现
        /// </summary>
        public void Attack(Kun target)
        {
            double baseAttackRate = GetBaseAttackRate(PetAttributeA, target.PetAttributeA);
            var weightDiff = PetAttributeA.Attack(Weight, target.Weight, (1, 1), baseAttackRate);
            weightDiff = target.PetAttributeA.BeingAttacked(target.Weight, Weight, weightDiff);

            weightDiff = target.PetAttributeB.Attack(Weight, target.Weight, weightDiff, baseAttackRate).Multiple(weightDiff);
            weightDiff = target.PetAttributeB.BeingAttacked(target.Weight, Weight, weightDiff).Multiple(weightDiff);

            Weight *= weightDiff.Item1;
            target.Weight *= weightDiff.Item2;

            Weight = Math.Min(Weight, GetLevelWeightLimit(Level));
            target.Weight = Math.Min(target.Weight, GetLevelWeightLimit(target.Level));

            Update();
            target.Update();
        }

        private double GetBaseAttackRate(IPetAttribute source, IPetAttribute target)
        {
            double baseRate = 1;
            // 基础克制
            if (source.ID == Enums.Attribute.Jin && target.ID == Enums.Attribute.Mu)
            {
                baseRate = 1.3;
            }
            else if (source.ID == Enums.Attribute.Mu && target.ID == Enums.Attribute.Tu)
            {
                baseRate = 1.3;
            }
            else if (source.ID == Enums.Attribute.Tu && target.ID == Enums.Attribute.Shui)
            {
                baseRate = 1.3;
            }
            else if (source.ID == Enums.Attribute.Shui && target.ID == Enums.Attribute.Huo)
            {
                baseRate = 1.3;
            }
            else if (source.ID == Enums.Attribute.Huo && target.ID == Enums.Attribute.Jin)
            {
                baseRate = 1.3;
            }
            // 风
            else if (source.ID == Enums.Attribute.Feng && (target.ID == Enums.Attribute.Shui || target.ID == Enums.Attribute.Jin || target.ID == Enums.Attribute.Mu))
            {
                baseRate = 0.7;
            }
            else if ((source.ID == Enums.Attribute.Tu || source.ID == Enums.Attribute.Huo) && target.ID == Enums.Attribute.Feng)
            {
                baseRate = 1.3;
            }
            // 雷
            else if (source.ID == Enums.Attribute.Lei && (target.ID == Enums.Attribute.Shui || target.ID == Enums.Attribute.Jin || target.ID == Enums.Attribute.Mu))
            {
                baseRate = 1.3;
            }
            else if ((source.ID == Enums.Attribute.Tu || source.ID == Enums.Attribute.Huo) && target.ID == Enums.Attribute.Lei)
            {
                baseRate = 0.7;
            }
            // 阴
            else if (source.ID == Enums.Attribute.Yin && target.ID == Enums.Attribute.Yang)
            {
                baseRate = 3;
            }
            else if ((source.ID == Enums.Attribute.Jin || source.ID == Enums.Attribute.Mu || source.ID == Enums.Attribute.Shui || source.ID == Enums.Attribute.Huo || source.ID == Enums.Attribute.Tu || source.ID == Enums.Attribute.Feng) && target.ID == Enums.Attribute.Yin)
            {
                baseRate = 0.5;
            }
            else if (source.ID == Enums.Attribute.Yin && (target.ID == Enums.Attribute.Jin || target.ID == Enums.Attribute.Mu || target.ID == Enums.Attribute.Shui || target.ID == Enums.Attribute.Huo || target.ID == Enums.Attribute.Tu || target.ID == Enums.Attribute.Feng))
            {
                baseRate = 2;
            }
            // 阳
            else if (source.ID == Enums.Attribute.Yang && target.ID == Enums.Attribute.Yin)
            {
                baseRate = 3;
            }
            else if ((source.ID == Enums.Attribute.Jin || source.ID == Enums.Attribute.Mu || source.ID == Enums.Attribute.Shui || source.ID == Enums.Attribute.Huo || source.ID == Enums.Attribute.Tu || source.ID == Enums.Attribute.Feng) && target.ID == Enums.Attribute.Yang)
            {
                baseRate = 0.5;
            }
            else if (source.ID == Enums.Attribute.Yang && (target.ID == Enums.Attribute.Jin || target.ID == Enums.Attribute.Mu || target.ID == Enums.Attribute.Shui || target.ID == Enums.Attribute.Huo || target.ID == Enums.Attribute.Tu || target.ID == Enums.Attribute.Feng))
            {
                baseRate = 2;
            }


            return baseRate;
        }

        public static int GetLevelWeightLimit(int level) => (int)Math.Pow(10, level);
        #endregion

        /// <summary>
        /// 使用数值方法前调用初始化
        /// </summary>
        public void Initialize()
        {
            PetAttributeA = RandomInsatantiator.GetInstanceByID(true, AttributeAID);
            PetAttributeB = RandomInsatantiator.GetInstanceByID(false, AttributeBID);
        }

        public void Update()
        {
            UpdateKun(this);
        }

        public override string ToString()
        {
            return $"[{PetAttributeA.Name}] {PetAttributeB.Name}鲲 {new string('★', Level)} {Weight:f2} 千克";
        }

        public string ToStringFull()
        {
            StringBuilder stringBuilder = new();
            stringBuilder.AppendLine(this.ToString());
            foreach (var item in PetAttributeA.Description)
            {
                stringBuilder.AppendLine(item.ToString());
            }
            foreach (var item in PetAttributeB.Description)
            {
                stringBuilder.AppendLine(item.ToString());
            }
            stringBuilder.RemoveNewLine();
            return stringBuilder.ToString();
        }

        public static void InitiazlizeRandom()
        {
            RandomInsatantiator = new();
            RandomInsatantiator.AddImplementation<None>(AppConfig.ProbablityNone);
            RandomInsatantiator.AddImplementation<Jin>(AppConfig.ProbablityJin);
            RandomInsatantiator.AddImplementation<Mu>(AppConfig.ProbablityMu);
            RandomInsatantiator.AddImplementation<Shui>(AppConfig.ProbablityShui);
            RandomInsatantiator.AddImplementation<Huo>(AppConfig.ProbablityHuo);
            RandomInsatantiator.AddImplementation<Tu>(AppConfig.ProbablityTu);
            RandomInsatantiator.AddImplementation<Feng>(AppConfig.ProbablityFeng);
            RandomInsatantiator.AddImplementation<Lei>(AppConfig.ProbablityLei);
            RandomInsatantiator.AddImplementation<Yin>(AppConfig.ProbablityYin);
        }

        public static Kun RandomCreate(Player player)
        {
            IPetAttribute attributeA = RandomInsatantiator.GetRandomInstance();
            IPetAttribute attributeB = AttributeB.RandomCreate();

            Kun kun = new()
            {
                AttributeAID = (int)attributeA.ID,
                AttributeBID = (int)attributeB.ID,
                PlayerID = player.QQ,
                Weight = CommonHelper.Random.Next(AppConfig.ValueHatchWeightMin, AppConfig.ValueHatchWeightMax),
                Abandoned = false,
                Alive = true,
            };
            kun.Level = (int)Math.Log10(kun.Weight) + 1;
            return kun;
        }

        public static int SaveKun(Kun kun)
        {
            var db = SQLHelper.GetInstance();
            return db.Insertable(kun).ExecuteReturnIdentity();
        }

        public static void UpdateKun(Kun kun)
        {
            var db = SQLHelper.GetInstance();
            db.Updateable(kun).ExecuteCommand();
        }

        public static Kun GetKunByID(int id)
        {
            var db = SQLHelper.GetInstance();
            return db.Queryable<Kun>().First(x => x.Id == id);
        }

        public static Kun GetKunByQQ(long qq)
        {
            var db = SQLHelper.GetInstance();
            return db.Queryable<Kun>().First(x => x.PlayerID == qq && !x.Abandoned && x.Alive);
        }

        public static List<Kun> GetKunByRecords(List<Record> records)
        {
            var db = SQLHelper.GetInstance();
            var ls = records.Select(x => x.KunID).ToList();
            return db.Queryable<Kun>().Where(x => ls.Any(o => x.Id == o)).ToList();
        }

        public static List<Kun> GetDeadKun(Player player)
        {
            var db = SQLHelper.GetInstance();
            return db.Queryable<Kun>().Where(x => x.CanResurrect && !x.Alive && !x.Abandoned && x.PlayerID == player.QQ).ToList();
        }
    }
}
