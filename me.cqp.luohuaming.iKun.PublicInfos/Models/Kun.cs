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

namespace me.cqp.luohuaming.iKun.PublicInfos.Models
{
    [SugarTable]
    public class Kun
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        public bool Abandoned { get; set; }

        public bool Alive { get; set; }

        public int AttributeAID { get; set; }

        public int AttributeBID { get; set; }

        public bool CanResurrect { get; set; }

        public int Level { get; set; }

        public long PlayerID { get; set; }

        public int ResurrectCount { get; set; }

        public double Weight { get; set; }

        public DateTime DeadAt { get; set; }

        private static Logger Logger { get; set; } = new Logger("鲲");

        [SugarColumn(IsIgnore = true)]
        public object LockObject { get; set; } = new object();

        [SugarColumn(IsIgnore = true)]
        public IPetAttribute PetAttributeA { get; set; }

        [SugarColumn(IsIgnore = true)]
        public IPetAttribute PetAttributeB { get; set; }

        private static PetAttributeRandomInsatantiator RandomInsatantiator { get; set; } = null;

        #region 数值实例

        // 数值方法应当进行数值计算，并将实例属性更新，同步进数据库
        // 反抛给调用方数值变化，由调用方进行文本拼接
        // 为每种方法定义独立的结果类，包含执行结果、以及所有需要的变化数值
        // 词缀进行计算时，在日志中记录所有的随机数并且需要在所有出口记录返回值
        // 进行计算前需要获取对象锁，当涉及两个对象的处理时，需要获取两个对象的锁
        // 部分方法需要检查Alive与Abandoned属性是否有效

        public static double GetLevelWeightLimit(int level) => Math.Pow(10, level);

        /// <summary>
        /// 渡劫
        /// 前置要求体重达到上限
        /// 进行渡劫成功之后，体重才能突破上限，等级加一
        /// 在词缀中有基础实现
        /// </summary>
        public AscendResult Ascend()
        {
            try
            {
                Monitor.Enter(LockObject);
                Logger.Info($"进入渡劫方法，ID={Id}");
                if (!Alive || Abandoned)
                {
                    Logger.Error("目标鲲已死亡或已被抛弃");
                    return new AscendResult { Success = false };
                }
                double original = Weight;
                double success = CalcAscendSuccessRate(Level);
                Logger.Info($"基础成功率：{success * 100}%");
                success = PetAttributeB.GetAscendSuccessRate(PetAttributeA.GetAscendSuccessRate(success));
                Logger.Info($"词缀加成后成功率：{success * 100}%");

                double diff = PetAttributeA.Ascend(success);
                diff = PetAttributeB.Ascend(success, diff);

                Weight *= diff;
                if (diff >= 1)
                {
                    Level++;
                }
                else
                {
                    double dead = CommonHelper.Random.NextDouble();
                    Logger.Info($"渡劫失败，死亡随机数判定：{dead}，临界：{AppConfig.ValueAscendFailDeadProbablity / 100}");
                    if (dead < AppConfig.ValueAscendFailDeadProbablity / 100)
                    {
                        Logger.Info($"判定成功，鲲触发死亡");
                        Alive = false;
                        DeadAt = DateTime.Now;
                    }
                    else
                    {
                        Logger.Info($"判定失败");
                    }
                }
                Weight = Math.Min(Weight, GetLevelWeightLimit(Level));
                if (Weight <= 0)
                {
                    Alive = false;
                    DeadAt = DateTime.Now;
                    Logger.Info($"体重小于0，鲲触发死亡");
                }
                Update();

                Logger.Info($"渡劫方法结束，倍率={diff}，结果={Weight}，原始值={original}，变化值={Weight - original}，当前等级={Level}，是否死亡={!Alive}");
                return new AscendResult
                {
                    CurrentLevel = Level,
                    CurrentWeight = Weight,
                    Increment = Weight - original,
                    Dead = !Alive
                };
            }
            catch (Exception e)
            {
                Logger.Error(e, "执行渡劫方法过程中发生异常");
                return new AscendResult { Success = false };
            }
            finally
            {
                Monitor.Exit(LockObject);
            }
        }

        /// <summary>
        /// 攻击
        /// 被攻击方损失概率体重，攻击方增加对应体重
        /// 被攻击方体重小于攻击方体重的10%时死亡
        /// 在词缀中有基础实现
        /// </summary>
        public AttackResult Attack(Kun target)
        {
            try
            {
                Monitor.Enter(LockObject);
                Monitor.Enter(target.LockObject);
                Logger.Info($"进入攻击方法，ID={Id}，目标ID={target.Id}");
                if (!Alive || Abandoned || !target.Alive || Abandoned)
                {
                    Logger.Error("目标鲲已死亡或已被抛弃");
                    return new AttackResult { Success = false };
                }
                double originalWeight = Weight;
                double originalTargetWeight = target.Weight;

                double baseAttackRate = GetBaseAttackRate(PetAttributeA, target.PetAttributeA);
                Logger.Info($"{PetAttributeA.Name}=>{target.PetAttributeA.Name}，基础伤害倍率={baseAttackRate}");

                var weightDiff = PetAttributeA.Attack(Weight, target.Weight, (1, 1), baseAttackRate);
                Logger.Info($"攻击主词缀计算，变化值={weightDiff.Item1}，{weightDiff.Item2}");
                weightDiff = target.PetAttributeA.BeingAttacked(Weight, target.Weight, weightDiff);
                Logger.Info($"被攻击方主词缀计算，变化值={weightDiff.Item1}，{weightDiff.Item2}");

                weightDiff = PetAttributeB.Attack(Weight, target.Weight, weightDiff, baseAttackRate);
                Logger.Info($"攻击方小词缀计算，变化值={weightDiff.Item1}，{weightDiff.Item2}");
                weightDiff = target.PetAttributeB.BeingAttacked(Weight, target.Weight, weightDiff);
                Logger.Info($"被攻击方小词缀计算，变化值={weightDiff.Item1}，{weightDiff.Item2}");

                Weight *= weightDiff.Item1;
                target.Weight *= weightDiff.Item2;
                Logger.Info($"攻击方变化后体重={Weight}，被攻击方变化后体重={target.Weight}");

                Weight = Math.Min(Weight, GetLevelWeightLimit(Level));
                target.Weight = Math.Min(target.Weight, GetLevelWeightLimit(target.Level));
                Logger.Info($"体重限制管理，攻击方体重={Weight}，被攻击方体重={target.Weight}");

                if (weightDiff.Item2 < 1 && target.Weight < Weight * 0.1)
                {
                    target.Alive = false;
                    target.DeadAt = DateTime.Now;
                    Logger.Info($"被攻击方变化后小于攻击方体重的10%，触发死亡");
                }

                if (Weight <= 0)
                {
                    Alive = false;
                    DeadAt = DateTime.Now;
                    Logger.Info($"攻击方变化后体重小于0，触发死亡");
                }
                if (target.Weight <= 0)
                {
                    target.Alive = false;
                    target.DeadAt = DateTime.Now;
                    Logger.Info($"被攻击方变化后体重小于0，触发死亡");
                }

                Update();
                target.Update();

                var r = new AttackResult
                {
                    CurrentWeight = Weight,
                    Dead = !Alive,
                    Increment = Weight - originalWeight,
                    TargetCurrentWeight = target.Weight,
                    TargetDead = !target.Alive,
                    TargetDecrement = originalTargetWeight - target.Weight,
                    WeightLimit = Weight == GetLevelWeightLimit(Level),
                    Escaped = weightDiff.Item1 == 1 && weightDiff.Item2 == 1,
                };
                Logger.Info($"攻击方法结束，{r}");
                return r;
            }
            catch (Exception e)
            {
                Logger.Error(e, "执行攻击方法过程中发生异常");
                return new AttackResult() { Success = false };
            }
            finally
            {
                Monitor.Exit(LockObject);
                Monitor.Exit(target.LockObject);
            }
        }

        /// <summary>
        /// 吞噬
        /// 体重相近时概率胜利
        /// 吞噬成功增加对方大部分体重，同时对方死亡
        /// 吞噬失败减少自身体重并概率死亡
        /// 在词缀中有基础实现
        /// </summary>
        public DevourResult Devour(Kun target)
        {
            try
            {
                Monitor.Enter(LockObject);
                Monitor.Enter(target.LockObject);
                Logger.Info($"进入吞噬方法，ID={Id}，目标ID={target.Id}");
                if (!Alive || Abandoned || !target.Alive || target.Abandoned)
                {
                    Logger.Error("目标鲲已死亡或已被抛弃");
                    return new DevourResult { Success = false };
                }
                double originalWeight = Weight;
                double originalTargetWeight = target.Weight;

                double baseAttackRate = GetBaseAttackRate(PetAttributeA, target.PetAttributeA);
                Logger.Info($"{PetAttributeA.Name}=>{target.PetAttributeA.Name}，基础伤害倍率={baseAttackRate}");

                var weightDiff = PetAttributeA.Devour(Weight, target.Weight, baseAttackRate);
                Logger.Info($"吞噬主词缀计算，变化值={weightDiff.Item1}，{weightDiff.Item2}");
                weightDiff = target.PetAttributeA.BeingDevoured(target.Weight, Weight, weightDiff);
                Logger.Info($"被吞噬主词缀计算，变化值={weightDiff.Item1}，{weightDiff.Item2}");

                weightDiff = PetAttributeB.Devour(Weight, target.Weight).Multiple(weightDiff);
                Logger.Info($"吞噬小词缀计算，变化值={weightDiff.Item1} ，{weightDiff.Item2}");
                weightDiff = target.PetAttributeB.BeingDevoured(target.Weight, Weight, weightDiff);
                Logger.Info($"被吞噬小词缀计算，变化值={weightDiff.Item1} ，{weightDiff.Item2}");

                Weight += weightDiff.Item1;
                target.Weight += weightDiff.Item2;
                Logger.Info($"攻击方变化后体重={Weight}，被攻击方变化后体重={target.Weight}");

                Weight = Math.Min(Weight, GetLevelWeightLimit(Level));
                target.Weight = Math.Min(target.Weight, GetLevelWeightLimit(target.Level));
                Logger.Info($"体重限制管理，攻击方体重={Weight}，被攻击方体重={target.Weight}");

                if (weightDiff.Item1 < 0)
                {
                    double dead = CommonHelper.Random.NextDouble();
                    Logger.Info($"吞噬失败，死亡随机数判定：{dead}，临界：{AppConfig.ValueDevourFailDeadProbablity / 100}");
                    if (dead < AppConfig.ValueDevourFailDeadProbablity / 100)
                    {
                        Logger.Info($"判定成功，鲲触发死亡");
                        Alive = false;
                        DeadAt = DateTime.Now;
                    }
                    else
                    {
                        Logger.Info($"判定失败");
                    }
                }
                else
                {
                    Logger.Info($"由于吞噬成功，被攻击方鲲触发死亡");
                    target.Alive = false;
                    target.DeadAt = DateTime.Now;
                }

                if (Weight <= 0)
                {
                    Alive = false;
                    DeadAt = DateTime.Now;
                    Logger.Info($"吞噬方变化后体重小于0，触发死亡");
                }
                if (target.Weight <= 0)
                {
                    target.Alive = false;
                    target.DeadAt = DateTime.Now;
                    Logger.Info($"被吞噬方变化后体重小于0，触发死亡");
                }

                Update();
                target.Update();

                var r = new DevourResult
                {
                    CurrentWeight = Weight,
                    Dead = !Alive,
                    Increment = Weight - originalWeight,
                    TargetCurrentWeight = target.Weight,
                    TargetDead = !target.Alive,
                    TargetDecrement = target.Weight - originalTargetWeight,
                    WeightLimit = Weight == GetLevelWeightLimit(Level),
                    Escaped = weightDiff.Item1 == 1 && weightDiff.Item2 == 1,
                };
                Logger.Info($"吞噬方法结束，{r}");
                return r;
            }
            catch (Exception e)
            {
                Logger.Error(e, "执行吞噬方法过程中发生异常");
                // 现有设计无法支持回滚操作
                return new DevourResult { Success = false };
            }
            finally
            {
                Monitor.Exit(target.LockObject);
                Monitor.Exit(LockObject);
            }
        }

        /// <summary>
        /// 喂养
        /// 在词缀中有基础实现
        /// 每个次数可增加10+(5%~10%)的体重
        /// </summary>
        public FeedResult Feed(int count)
        {
            try
            {
                Monitor.Enter(LockObject);
                double original = Weight;
                Logger.Info($"进入喂养方法，ID={Id}，数量={count}");
                if (!Alive || Abandoned)
                {
                    Logger.Error("目标鲲已死亡或已被抛弃");
                    return new FeedResult { Success = false };
                }
                double diff = PetAttributeA.Feed(count);
                diff = PetAttributeB.Feed(count, diff);

                Weight *= (1 + diff);
                Weight += count * AppConfig.ValueFeedWeightBaseIncrement;
                Weight = Math.Min(Weight, GetLevelWeightLimit(Level));
                Update();
                bool reachLimit = Weight == GetLevelWeightLimit(Level);
                Logger.Info($"喂养方法结束，倍率={diff}，基础增加={count * AppConfig.ValueFeedWeightBaseIncrement}，结果={Weight}，原始值={original}，变化值={Weight - original}，达到上限={reachLimit}");
                return new FeedResult
                {
                    CurrentWeight = Weight,
                    Increment = Weight - original,
                    WeightLimit = reachLimit
                };
            }
            catch (Exception e)
            {
                Logger.Error(e, "执行喂养方法过程中发生异常");
                return new FeedResult { Success = false };
            }
            finally
            {
                Monitor.Exit(LockObject);
            }
        }

        /// <summary>
        /// 放生
        /// </summary>
        public bool Release()
        {
            try
            {
                Monitor.Enter(LockObject);
                Logger.Info($"进入放生方法，ID={Id}");
                if (!Alive)
                {
                    Logger.Error("目标鲲已死亡");
                    return false;
                }
                Abandoned = true;
                Update();
                Logger.Info($"退出放生方法");
                return true;
            }
            catch (Exception e)
            {
                Logger.Error(e, "执行放生方法过程中发生异常");
                return false;
            }
            finally
            {
                Monitor.Exit(LockObject);
            }
        }

        /// <summary>
        /// 复活
        /// 默认完成道具扣除并满足条件
        /// 每复活一次，消耗的资源翻倍
        /// </summary>
        public ResurrectResult Resurrect()
        {
            try
            {
                Monitor.Enter(LockObject);
                Logger.Info($"进入复活方法，ID={Id}，体重={Weight}，星级={Level}，死亡时间={DeadAt:G}");
                if (Abandoned)
                {
                    Logger.Error("目标鲲已被抛弃");
                    return new ResurrectResult() { Success = false };
                }
                Alive = true;
                ResurrectCount++;
                double originalWeight = Weight;
                int originalLevel = Level;
                double deadHour = (DateTime.Now - DeadAt).TotalHours;
                Logger.Info($"死亡小时数={deadHour}");
                if (deadHour >= AppConfig.ValueMaxResurrectHour)
                {
                    Logger.Error($"鲲死亡超过 {AppConfig.ValueMaxResurrectHour} 小时，无法复活");
                    return new ResurrectResult() { Success = false };
                }
                int weightLossCount = (int)(deadHour / 2);
                int levelLossCount = (int)(deadHour / 18);
                Logger.Info($"体重缩水次数={weightLossCount}，倍率={AppConfig.ValuePerTwoHourWeightLoss}%");
                Logger.Info($"等级缩水次数={levelLossCount}，数量={AppConfig.ValuePerEighteenHourLevelLoss}%");
                for (int i = 0; i < weightLossCount; i++)
                {
                    Weight *= ((100 - AppConfig.ValuePerTwoHourWeightLoss) / 100.0);
                }
                for (int i = 0; i < levelLossCount; i++)
                {
                    Level -= AppConfig.ValuePerEighteenHourLevelLoss;
                }
                Level = Math.Max(Level, 1);
                Update();

                var r = new ResurrectResult
                {
                    CurrentResurrectCount = ResurrectCount,
                    WeightLoss = originalWeight - Weight,
                    LevelLoss = originalLevel - Level,
                };
                Logger.Info($"退出复活方法，{r}");
                return r;
            }
            catch (Exception e)
            {
                Logger.Error(e, "执行复活方法过程中发生异常");
                return new ResurrectResult() { Success = false };
            }
            finally
            {
                Monitor.Exit(LockObject);
            }
        }

        /// <summary>
        /// 幻化
        /// 默认实现了道具扣除并满足条件
        /// 随机主词缀与副词缀
        /// 每次扣除95%体重
        /// 小于10kg时死亡
        /// 10%概率失败，失败时死亡
        /// </summary>
        public TransmogrifyResult Transmogrify()
        {
            try
            {
                Monitor.Enter(LockObject);
                Logger.Info($"进入幻化方法，ID={Id}");
                if (!Alive || Abandoned)
                {
                    Logger.Error("目标鲲已死亡或已被抛弃");
                    return new TransmogrifyResult { Success = false };
                }
                var originalAttributeA = RandomInsatantiator.GetInstanceByID(true, AttributeAID);
                var originalAttributeB = RandomInsatantiator.GetInstanceByID(false, AttributeBID);
                var originalWeight = Weight;

                double random = CommonHelper.Random.NextDouble();
                double failRate = PetAttributeB.GetTransmogrifyFailRate(PetAttributeA.GetTransmogrifyFailRate(0.1));
                Logger.Info($"随机数={random}，成功临界={1 - failRate}");
                bool success = random > failRate;

                double loss = PetAttributeB.GetTransmogrifyFailWeightLostRate(PetAttributeA.GetTransmogrifyFailWeightLostRate(0.05));
                Logger.Info($"体重损失倍率={1 - loss}");
                Weight *= loss;
                Logger.Info($"计算后体重为={Weight}，死亡临界点={AppConfig.ValueTransmoirgifyDeadWeightLimit}");
                if (Weight < AppConfig.ValueTransmoirgifyDeadWeightLimit)
                {
                    Logger.Info($"体重小于临界点，直接死亡");
                    Alive = false;
                    DeadAt = DateTime.Now;
                }

                if (!success && Alive)
                {
                    Logger.Info($"幻化失败，判定是否死亡");
                    random = CommonHelper.Random.NextDouble();
                    Logger.Info($"随机数={random}，临界={AppConfig.ValueTransmoirgifyFailDeadProbablity / 100}");

                    if (random < AppConfig.ValueTransmoirgifyFailDeadProbablity / 100)
                    {
                        Logger.Info($"触发死亡");
                        Alive = false;
                        DeadAt = DateTime.Now;
                    }
                }
                else
                {
                    PetAttributeA = RandomInsatantiator.GetRandomInstance();
                    PetAttributeB = AttributeB.RandomCreate();

                    AttributeAID = (int)PetAttributeA.ID;
                    AttributeBID = PetAttributeB.AttrbiuteBID;
                    Logger.Info($"幻化成功，主词缀与小词缀均已变化");
                }
                Update();
                var r = new TransmogrifyResult
                {
                    CurrentAttributeA = PetAttributeA,
                    CurrentAttributeB = PetAttributeB,
                    CurrentWeight = Weight,
                    Decrement = originalWeight - Weight,
                    OriginalAttributeA = originalAttributeA,
                    OriginalAttributeB = originalAttributeB,
                    Dead = !Alive
                };
                Logger.Info($"幻化方法结束，{r}");
                return r;
            }
            catch (Exception e)
            {
                Logger.Error(e, "执行幻化方法过程中发生异常");
                return new TransmogrifyResult() { Success = false };
            }
            finally
            {
                Monitor.Exit(LockObject);
            }
        }

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
                if (!Alive || Abandoned)
                {
                    Logger.Error("目标鲲已死亡或已被抛弃");
                    return new UpgradeResult { Success = false };
                }
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
            if (source.ID != Enums.Attribute.None && target.ID == Enums.Attribute.None)
            {
                baseRate = 1.3;
            }
            return baseRate;
        }

        private static double CalcAscendSuccessRate(int level)
        {
            return level switch
            {
                <= 5 => 0.95 - 0.1 * (level - 1),
                _ => 0.4 * Math.Exp(-0.4 * (level - 6))
            };
        }

        #endregion 数值实例

        public static List<Kun> GetAllAliveKun()
        {
            var db = SQLHelper.GetInstance();
            return db.Queryable<Kun>().Where(x => x.Alive && !x.Abandoned).ToList();
        }

        public static List<Kun> GetRankingKun(int count)
        {
            var db = SQLHelper.GetInstance();
            return db.Queryable<Kun>().Where(x => x.Alive && !x.Abandoned).OrderByDescending(x => x.Weight).Take(count).ToList();
        }

        public static List<Kun> GetDeadKun(Player player)
        {
            var db = SQLHelper.GetInstance();
            return db.Queryable<Kun>().Where(x => x.CanResurrect && !x.Alive && !x.Abandoned && x.PlayerID == player.QQ)
                .ToList()
                .Where(x => (DateTime.Now - x.DeadAt) < TimeSpan.FromHours(AppConfig.ValueMaxResurrectHour))
                .ToList();
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
            return db.Queryable<Kun>().Where(x => ls.Contains(x.Id) && x.Alive && !x.Abandoned).ToList();
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
            RandomInsatantiator.AddImplementation<Yang>(AppConfig.ProbablityYang);
        }

        public static Kun RandomCreate(Player player)
        {
            IPetAttribute attributeA = RandomInsatantiator.GetRandomInstance();
            IPetAttribute attributeB = AttributeB.RandomCreate();

            Kun kun = new()
            {
                AttributeAID = (int)attributeA.ID,
                AttributeBID = attributeB.AttrbiuteBID,
                PlayerID = player.QQ,
                Weight = CommonHelper.Random.Next(AppConfig.ValueHatchWeightMin, AppConfig.ValueHatchWeightMax),
                Abandoned = false,
                Alive = true,
                CanResurrect = true,
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

        /// <summary>
        /// 使用数值方法前调用初始化
        /// </summary>
        public void Initialize()
        {
            PetAttributeA = RandomInsatantiator.GetInstanceByID(true, AttributeAID);
            PetAttributeB = RandomInsatantiator.GetInstanceByID(false, AttributeBID);
        }

        public override string ToString()
        {
            return AppConfig.ReplyKunToString
                .Replace("%PetAttributeA%", PetAttributeA.Name)
                .Replace("%PetAttributeB%", PetAttributeB.Name)
                .Replace("%LongLevel%", new string('★', Math.Max(0, Level)))
                .Replace("%ShortLevel%", $"{Level}★")
                .Replace("%Weight%", Weight.ToShortNumber());
        }

        public string ToStringFull()
        {
            StringBuilder stringBuilder = new();
            stringBuilder.AppendLine(this.ToString() + $" {Weight.ToShortNumber()} {AppConfig.WeightUnit}");
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

        public void Update()
        {
            UpdateKun(this);
        }
    }
}