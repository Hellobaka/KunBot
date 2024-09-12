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

        public int Level { get; set; }

        public bool Abandoned { get; set; }

        public bool Alive { get; set; }

        [SugarColumn(IsIgnore = true)]
        public IPetAttribute PetAttributeA { get; set; }

        [SugarColumn(IsIgnore = true)]
        public IPetAttribute PetAttributeB { get; set; }

        private static PetAttributeRandomInsatantiator RandomInsatantiatorA { get; set; } = null;

        private static PetAttributeRandomInsatantiator RandomInsatantiatorB { get; set; } = null;

        public void Initialize()
        {
            PetAttributeA = RandomInsatantiatorA.GetInstanceByID(AttributeAID);
            PetAttributeB = RandomInsatantiatorB.GetInstanceByID(AttributeBID);
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
            RandomInsatantiatorA.AddImplementation<Ao>(AppConfig.ProbablityAo);
            RandomInsatantiatorA.AddImplementation<Bei>(AppConfig.ProbablityBei);
            RandomInsatantiatorA.AddImplementation<Chan>(AppConfig.ProbablityChan);
            RandomInsatantiatorA.AddImplementation<Du>(AppConfig.ProbablityDu);
            RandomInsatantiatorA.AddImplementation<Duo>(AppConfig.ProbablityDuo);
            RandomInsatantiatorA.AddImplementation<Nu>(AppConfig.ProbablityNu);
            RandomInsatantiatorA.AddImplementation<Tan>(AppConfig.ProbablityTan);
            RandomInsatantiatorA.AddImplementation<Yin>(AppConfig.ProbablityYin);

        }

        public static Kun RandomCreate(Player player)
        {
            IPetAttribute attributeB = RandomInsatantiatorA.GetRandomInstance();
            Kun kun = new()
            {
                AttributeBID = attributeB.ID,
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

        private static int RandomLevel()
        {
            double sumProbablity = AppConfig.ProbablityHatchLevel.Sum();
            double randomValue = CommonHelper.Random.NextDouble() * sumProbablity;

            double value = 0;
            for (int i = 1; i <= AppConfig.ProbablityHatchLevel.Count; i++)
            {
                value += AppConfig.ProbablityHatchLevel[i];
                if (value <= randomValue)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
