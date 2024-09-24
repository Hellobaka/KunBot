using me.cqp.luohuaming.iKun.PublicInfos.Items;
using me.cqp.luohuaming.iKun.PublicInfos.PetAttribute;
using me.cqp.luohuaming.iKun.PublicInfos.PetAttribute.AttributeA;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        private static PetAttributeRandomInsatantiator RandomInsatantiatorA { get; set; } = null;

        private static PetAttributeRandomInsatantiator RandomInsatantiatorB { get; set; } = null;

        #region 数值实例
        /// <summary>
        /// 强化
        /// 在词缀中有基础实现
        /// </summary>
        public void Upgrade(int count)
        {
            double diff = PetAttributeA.Upgrade(count);
            diff *= PetAttributeB.Upgrade(count);

            Weight *= diff;
            Weight = Math.Min(Weight, GetLevelWeightLimit(Level));
            Update();
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
            double diff = PetAttributeA.Ascend(success);
            diff *= PetAttributeB.Ascend(success);

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
        /// 在词缀中有基础实现
        /// </summary>
        public void Devour(Kun target)
        {
            double baseAttackRate = GetBaseAttackRate(PetAttributeA, target.PetAttributeA);
            var weightDiff = PetAttributeA.Devour(Weight, target.Weight, baseAttackRate);
            weightDiff = target.PetAttributeA.BeingDevoured(target.Weight, Weight, weightDiff);

            weightDiff = target.PetAttributeB.Devour(Weight, target.Weight).Multiple(weightDiff);
            weightDiff = target.PetAttributeB.BeingDevoured(target.Weight, Weight, weightDiff).Multiple(weightDiff);

            Weight *= weightDiff.Item1;
            target.Weight *= weightDiff.Item2;
            
            Weight = Math.Min(Weight, GetLevelWeightLimit(Level));
            target.Weight = Math.Min(target.Weight, GetLevelWeightLimit(target.Level));

            Update();
            target.Update();
        }

        /// <summary>
        /// 喂养
        /// 在词缀中有基础实现
        /// </summary>
        public void Feed(int count)
        {
            double diff = PetAttributeA.Feed(count);
            diff *= PetAttributeB.Feed(count);

            Weight *= diff;
            Weight = Math.Min(Weight, GetLevelWeightLimit(Level));
            Update();
        }

        /// <summary>
        /// 放生
        /// </summary>
        public void Release()
        {
        }

        /// <summary>
        /// 复活
        /// </summary>
        public void Resurrect()
        {
        }

        /// <summary>
        /// 幻化
        /// </summary>
        public void Transmogrify()
        {
        }

        /// <summary>
        /// 攻击
        /// 在词缀中有基础实现
        /// </summary>
        public void Attack(Kun target)
        {
            double baseAttackRate = GetBaseAttackRate(PetAttributeA, target.PetAttributeA);
            var weightDiff = PetAttributeA.Attack(Weight, target.Weight, baseAttackRate);
            weightDiff = target.PetAttributeA.BeingAttacked(target.Weight, Weight, weightDiff);

            weightDiff = target.PetAttributeB.Attack(Weight, target.Weight).Multiple(weightDiff);
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
            if (source.ID == Enums.AttributeA.Jin && target.ID == Enums.AttributeA.Mu)
            {
                baseRate = 1.3;
            }
            else if (source.ID == Enums.AttributeA.Mu && target.ID == Enums.AttributeA.Tu)
            {
                baseRate = 1.3;
            }
            else if (source.ID == Enums.AttributeA.Tu && target.ID == Enums.AttributeA.Shui)
            {
                baseRate = 1.3;
            }
            else if (source.ID == Enums.AttributeA.Shui && target.ID == Enums.AttributeA.Huo)
            {
                baseRate = 1.3;
            }
            else if (source.ID == Enums.AttributeA.Huo && target.ID == Enums.AttributeA.Jin)
            {
                baseRate = 1.3;
            }
            // 风
            else if (source.ID == Enums.AttributeA.Feng && (target.ID == Enums.AttributeA.Shui || target.ID == Enums.AttributeA.Jin || target.ID == Enums.AttributeA.Mu))
            {
                baseRate = 0.7;
            }
            else if ((source.ID == Enums.AttributeA.Tu || source.ID == Enums.AttributeA.Huo) && target.ID == Enums.AttributeA.Feng)
            {
                baseRate = 1.3;
            }
            // 雷
            else if (source.ID == Enums.AttributeA.Lei && (target.ID == Enums.AttributeA.Shui || target.ID == Enums.AttributeA.Jin || target.ID == Enums.AttributeA.Mu))
            {
                baseRate = 1.3;
            }
            else if ((source.ID == Enums.AttributeA.Tu || source.ID == Enums.AttributeA.Huo) && target.ID == Enums.AttributeA.Lei)
            {
                baseRate = 0.7;
            }
            // 阴
            else if (source.ID == Enums.AttributeA.Yin && target.ID == Enums.AttributeA.Yang)
            {
                baseRate = 3;
            }
            else if ((source.ID == Enums.AttributeA.Jin || source.ID == Enums.AttributeA.Mu || source.ID == Enums.AttributeA.Shui || source.ID == Enums.AttributeA.Huo || source.ID == Enums.AttributeA.Tu || source.ID == Enums.AttributeA.Feng) && target.ID == Enums.AttributeA.Yin)
            {
                baseRate = 0.5;
            }
            else if (source.ID == Enums.AttributeA.Yin && (target.ID == Enums.AttributeA.Jin || target.ID == Enums.AttributeA.Mu || target.ID == Enums.AttributeA.Shui || target.ID == Enums.AttributeA.Huo || target.ID == Enums.AttributeA.Tu || target.ID == Enums.AttributeA.Feng))
            {
                baseRate = 2;
            }
            // 阳
            else if (source.ID == Enums.AttributeA.Yang && target.ID == Enums.AttributeA.Yin)
            {
                baseRate = 3;
            }
            else if ((source.ID == Enums.AttributeA.Jin || source.ID == Enums.AttributeA.Mu || source.ID == Enums.AttributeA.Shui || source.ID == Enums.AttributeA.Huo || source.ID == Enums.AttributeA.Tu || source.ID == Enums.AttributeA.Feng) && target.ID == Enums.AttributeA.Yang)
            {
                baseRate = 0.5;
            }
            else if (source.ID == Enums.AttributeA.Yang && (target.ID == Enums.AttributeA.Jin || target.ID == Enums.AttributeA.Mu || target.ID == Enums.AttributeA.Shui || target.ID == Enums.AttributeA.Huo || target.ID == Enums.AttributeA.Tu || target.ID == Enums.AttributeA.Feng))
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
            PetAttributeA = RandomInsatantiatorA.GetInstanceByID(true, AttributeAID);
            PetAttributeB = RandomInsatantiatorB.GetInstanceByID(false, AttributeBID);
        }

        public void Update()
        {
            var db = SQLHelper.GetInstance();
            db.Updateable(this).ExecuteCommand();
        }

        public override string ToString()
        {
            return $"[{PetAttributeA.Name}] {PetAttributeB.Name}鲲 {new string('★', Level)}";
        }

        public static void InitiazlizeRandom()
        {
            RandomInsatantiatorA = new();
            RandomInsatantiatorA.AddImplementation<Jin>(AppConfig.ProbablityJin);
            RandomInsatantiatorA.AddImplementation<Mu>(AppConfig.ProbablityMu);
            RandomInsatantiatorA.AddImplementation<Shui>(AppConfig.ProbablityShui);
            RandomInsatantiatorA.AddImplementation<Huo>(AppConfig.ProbablityHuo);
            RandomInsatantiatorA.AddImplementation<Tu>(AppConfig.ProbablityTu);
            RandomInsatantiatorA.AddImplementation<Feng>(AppConfig.ProbablityFeng);
            RandomInsatantiatorA.AddImplementation<Lei>(AppConfig.ProbablityLei);
            RandomInsatantiatorA.AddImplementation<Yin>(AppConfig.ProbablityYin);

            RandomInsatantiatorB = new();
            RandomInsatantiatorB.AddImplementation<PetAttribute.AttributeB.None>(1);
        }

        public static Kun RandomCreate(Player player)
        {
            IPetAttribute attributeB = RandomInsatantiatorA.GetRandomInstance();
            Kun kun = new()
            {
                AttributeBID = (int)attributeB.ID,
                PlayerID = player.QQ,
                Weight = CommonHelper.Random.Next(AppConfig.ValueHatchWeightMin, AppConfig.ValueHatchWeightMax),
                Abandoned = false,
                Alive = true,
            };
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
    }
}
