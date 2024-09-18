﻿using me.cqp.luohuaming.iKun.PublicInfos.Items;
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
        /// <summary>
        /// 强化
        /// 在词缀中有基础实现
        /// </summary>
        public void Upgrade(int count)
        {
            double diff = PetAttributeA.Upgrade(count);
            diff *= PetAttributeB.Upgrade(count);

            Weight *= diff;
            Update();
        }

        /// <summary>
        /// 渡劫
        /// 在词缀中有基础实现
        /// </summary>
        public void Ascend()
        {
            double success = Level switch
            {
                1 => 0.7,
                2 => 0.65,
                3 => 0.6,
                4 => 0.55,
                5 => 0.5,
                6 => 0.45,
                7 => 0.4,
                8 => 0.35,
                _ => 0.3,
            };
            double diff = PetAttributeA.Ascend(success);
            diff *= PetAttributeB.Ascend(success);

            Weight *= diff;
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

            Update();
            target.Update();
        }

        private double GetBaseAttackRate(IPetAttribute source, IPetAttribute target)
        {
            double baseRate = 1;
            if (source.ID == Enums.AttributeA.Jin && target.ID == Enums.AttributeA.Mu)
            {
                baseRate = 1.3;
            }
            else if (source.ID == Enums.AttributeA.Jin && target.ID == Enums.AttributeA.Feng)
            {
                baseRate = 0.7;
            }
            else if (source.ID == Enums.AttributeA.Mu && target.ID == Enums.AttributeA.Tu)
            {
                baseRate = 1.3;
            }
            else if (source.ID == Enums.AttributeA.Mu && target.ID == Enums.AttributeA.Feng)
            {
                baseRate = 0.7;
            }
            else if (source.ID == Enums.AttributeA.Tu && target.ID == Enums.AttributeA.Shui)
            {
                baseRate = 1.3;
            }
            else if (source.ID == Enums.AttributeA.Tu && target.ID == Enums.AttributeA.Lei)
            {
                baseRate = 0.7;
            }
            else if (source.ID == Enums.AttributeA.Shui && target.ID == Enums.AttributeA.Huo)
            {
                baseRate = 1.3;
            }
            else if (source.ID == Enums.AttributeA.Shui && target.ID == Enums.AttributeA.Feng)
            {
                baseRate = 0.7;
            }
            else if (source.ID == Enums.AttributeA.Huo && target.ID == Enums.AttributeA.Jin)
            {
                baseRate = 1.3;
            }
            else if (source.ID == Enums.AttributeA.Huo && target.ID == Enums.AttributeA.Lei)
            {
                baseRate = 0.7;
            }
            else if (source.ID == Enums.AttributeA.Feng && (target.ID == Enums.AttributeA.Tu || target.ID == Enums.AttributeA.Huo))
            {
                baseRate = 1.3;
            }
            else if (source.ID == Enums.AttributeA.Lei && target.ID == Enums.AttributeA.Yin)
            {
                baseRate = 2;
            }
            else if (source.ID == Enums.AttributeA.Lei && (target.ID == Enums.AttributeA.Shui || target.ID == Enums.AttributeA.Jin || target.ID == Enums.AttributeA.Mu))
            {
                baseRate = 1.3;
            }
            else if (source.ID == Enums.AttributeA.Yin && target.ID != Enums.AttributeA.Yin)
            {
                baseRate = 1.5;
            }
            else if(source.ID != Enums.AttributeA.Yin && target.ID == Enums.AttributeA.Yin)
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
