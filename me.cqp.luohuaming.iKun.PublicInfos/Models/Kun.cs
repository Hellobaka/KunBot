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

        public bool Alive { get; set; }

        [SugarColumn(IsIgnore = true)]
        public IPetAttribute PetAttributeA { get; set; }

        [SugarColumn(IsIgnore = true)]
        public IPetAttribute PetAttributeB { get; set; }

        [SugarColumn(IsIgnore = true)]
        public int Level => Weight switch
        {
            < 400 => 1,
            < 1100 => 2,
            < 2100 => 3,
            < 3400 => 4,
            < 5100 => 5,
            < 7300 => 6,
            < 9900 => 7,
            < 13000 => 8,
            _ => 9,
        };

        private static PetAttributeRandomInsatantiator RandomInsatantiatorA { get; set; } = null;

        private static PetAttributeRandomInsatantiator RandomInsatantiatorB { get; set; } = null;

        #region 数值实例
        public void Upgrade(int count)
        {
            PetAttributeA.Upgrade(count);
            PetAttributeB.Upgrade(count);
        }

        public void Ascend()
        {
        }

        public void Devour()
        {
        }

        public void Feed()
        {
        }

        public void Release()
        {
        }

        public void Resurrect()
        {
        }

        public void Transmogrify()
        {
        }

        public void Attack(Kun target)
        {
            double baseAttackRate = CalcBaseAttackRate(PetAttributeA, target.PetAttributeA);
            var weightDiff = PetAttributeA.Attack(Weight, target.Weight, baseAttackRate);
            weightDiff = target.PetAttributeA.BeingAttacked(target.Weight, Weight, weightDiff);

            weightDiff = target.PetAttributeB.Attack(Weight, target.Weight).Multiple(weightDiff);
            weightDiff = target.PetAttributeB.BeingAttacked(target.Weight, Weight, weightDiff).Multiple(weightDiff);

            Weight *= weightDiff.Item1;
            target.Weight *= weightDiff.Item2;

            Update();
            target.Update();
        }

        private double CalcBaseAttackRate(IPetAttribute source, IPetAttribute target)
        {
            double baseRate = 1;
            if (source.ID == Enums.Attributes.Jin && target.ID == Enums.Attributes.Mu)
            {
                baseRate = 1.3;
            }
            else if (source.ID == Enums.Attributes.Jin && target.ID == Enums.Attributes.Feng)
            {
                baseRate = 0.7;
            }
            else if (source.ID == Enums.Attributes.Mu && target.ID == Enums.Attributes.Tu)
            {
                baseRate = 1.3;
            }
            else if (source.ID == Enums.Attributes.Mu && target.ID == Enums.Attributes.Feng)
            {
                baseRate = 0.7;
            }
            else if (source.ID == Enums.Attributes.Tu && target.ID == Enums.Attributes.Shui)
            {
                baseRate = 1.3;
            }
            else if (source.ID == Enums.Attributes.Tu && target.ID == Enums.Attributes.Lei)
            {
                baseRate = 0.7;
            }
            else if (source.ID == Enums.Attributes.Shui && target.ID == Enums.Attributes.Huo)
            {
                baseRate = 1.3;
            }
            else if (source.ID == Enums.Attributes.Shui && target.ID == Enums.Attributes.Feng)
            {
                baseRate = 0.7;
            }
            else if (source.ID == Enums.Attributes.Huo && target.ID == Enums.Attributes.Jin)
            {
                baseRate = 1.3;
            }
            else if (source.ID == Enums.Attributes.Huo && target.ID == Enums.Attributes.Lei)
            {
                baseRate = 0.7;
            }
            else if (source.ID == Enums.Attributes.Feng && (target.ID == Enums.Attributes.Tu || target.ID == Enums.Attributes.Huo))
            {
                baseRate = 1.3;
            }
            else if (source.ID == Enums.Attributes.Lei && target.ID == Enums.Attributes.Yin)
            {
                baseRate = 2;
            }
            else if (source.ID == Enums.Attributes.Lei && (target.ID == Enums.Attributes.Shui || target.ID == Enums.Attributes.Jin || target.ID == Enums.Attributes.Mu))
            {
                baseRate = 1.3;
            }
            else if (source.ID == Enums.Attributes.Yin && target.ID != Enums.Attributes.Yin)
            {
                baseRate = 1.5;
            }
            else if(source.ID != Enums.Attributes.Yin && target.ID == Enums.Attributes.Yin)
            {
                baseRate = 0.5;
            }

            return baseRate;
        }

        #endregion

        /// <summary>
        /// 使用数值方法前调用初始化
        /// </summary>
        public void Initialize()
        {
            PetAttributeA = RandomInsatantiatorA.GetInstanceByID((Enums.Attributes)AttributeAID);
            PetAttributeB = RandomInsatantiatorB.GetInstanceByID((Enums.Attributes)AttributeBID);
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
