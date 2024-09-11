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

        public int Level {  get; set; }

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

        public double AddWeight(double weight)
        {
            return Weight + weight;
        }

        public override string ToString()
        {
            return base.ToString();
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
            IPetAttribute attributeA = RandomInsatantiatorA.GetRandomInstance();
            IPetAttribute attributeB = RandomInsatantiatorA.GetRandomInstance();
            Kun kun = new()
            {
                AttributeAID = attributeA.ID,
                AttributeBID = attributeB.ID,
                PlayerID = player.QQ,
                Weight = CommonHelper.Random.Next(AppConfig.ValueHatchWeightMin, AppConfig.ValueHatchWeightMax),
                Level = RandomLevel(),
                Abandoned = false,
                Alive = true,
            };
            return kun;
        }

        public static void SaveKun(Kun kun)
        {
            var db = SQLHelper.GetInstance();
            db.Insertable(kun).ExecuteCommand();
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
