using me.cqp.luohuaming.iKun.PublicInfos.PetAttribute.AttributeA;
using System;
using System.Collections.Generic;
using System.Linq;

namespace me.cqp.luohuaming.iKun.PublicInfos.PetAttribute
{
    /// <summary>
    /// 词缀系统
    /// 通常情况下，攻击与吞噬操作不可重复进行，防止更改既定结果
    /// 所有增加体重操作在原始结果上进行乘算
    /// 所有单独增加伤害操作需要计算出双方增减量之后，进行加成乘算，再计算出倍率
    /// 降低伤害时，进行乘算
    /// 同时造成与受到伤害的可直接再次进行伤害计算操作
    /// </summary>
    public abstract class IPetAttribute
    {
        public string[] Description { get; set; }

        public Enums.Attribute ID { get; set; }

        public string Name { get; set; }

        private Logger Logger { get; set; } = new Logger("基础词缀");

        /// <summary>
        /// 默认完成了道具扣除
        /// 成功增加500%，失败扣除50%，10%概率死亡
        /// </summary>
        /// <param name="success">基础成功率(0.x)</param>
        /// <param name="diff">最终值变化附加数值(%)(乘算)</param>
        /// <returns>体重变化(0.x)</returns>
        public virtual double Ascend(double success, double diff = 1)
        {
            Logger.Info($"进入渡劫词缀计算方法，成功率={success}，Diff={diff}");
            double random = CommonHelper.Random.NextDouble();
            Logger.Info($"判定随机数={random}");
            diff = (random <= success ? 5 : 0.5) * diff;
            Logger.Info($"退出渡劫词缀计算方法，倍率={diff}");
            return diff;
        }

        /// <summary>
        /// 被攻击方损失概率体重，攻击方增加对应体重
        /// </summary>
        /// <param name="source">攻击方体重</param>
        /// <param name="target">目标方体重</param>
        /// <param name="diff">攻击附加变化(%)(乘算)</param>
        /// <returns>攻击方体重变化(0.x)，目标方体重变化(0.x)</returns>
        public virtual (double, double) Attack(double source, double target, (double, double) baseAttack, double diff = 1)
        {
            Logger.Info($"进入攻击词缀计算方法，攻击方体重={source}，目标方体重={target}，Diff={diff}，基础倍率={baseAttack.Item1}，{baseAttack.Item2}");
            Logger.Info($"攻击体重损失下限={AppConfig.ValueAttackWeightMinimumDecrement}，上限={AppConfig.ValueAttackWeightMaximumDecrement}");
            double random = CommonHelper.Random.NextDouble(AppConfig.ValueAttackWeightMinimumDecrement / 100.0, AppConfig.ValueAttackWeightMaximumDecrement / 100.0);
            double decrement = random * diff;
            double value = source * decrement;
            double increment = value / target;
            Logger.Info($"倍率随机数={random}，被攻击方损失倍率={decrement}，攻击方增量={increment}");
            var r = (baseAttack.Item1 + increment, baseAttack.Item2 - decrement);
            Logger.Info($"退出攻击词缀计算方法，最终倍率={r.Item1}，{r.Item2}");
            return r;
        }

        /// <summary>
        /// 被攻击时伤害倍率变化
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="diff"></param>
        /// <returns>攻击方体重变化(0.x)，目标方体重变化(0.x)</returns>
        public virtual (double, double) BeingAttacked(double source, double target, (double, double) baseAttack)
        {
            Logger.Info($"进入被攻击词缀计算方法，攻击方体重={source}，目标方体重={target}，原始倍率={baseAttack.Item1}, {baseAttack.Item2}");
            Logger.Info($"无处理，返回原始倍率");

            return baseAttack;
        }

        public virtual (double, double) BeingDevoured(double source, double target, (double, double) baseDevour)
        {
            Logger.Info($"进入被吞噬词缀计算方法，攻击方体重={source}，目标方体重={target}，原始倍率={baseDevour.Item1}, {baseDevour.Item2}");
            Logger.Info($"无处理，返回原始倍率");
            return baseDevour;
        }

        /// <summary>
        /// 目标方体重在攻击方的±百分比范围内时，双方50%胜负
        /// 不满足上述条件时:
        /// - 若目标方体重大于攻击方，吞噬失败
        /// - 目标方体重小于攻击方，吞噬成功
        /// 成功时攻击方增加目标方的 50%~100% 体重，目标方死亡
        /// 失败时目标方增加攻击方的 50%~70% 体重，攻击方概率死亡
        /// </summary>
        /// <param name="source">攻击方体重</param>
        /// <param name="target">目标方体重</param>
        /// <param name="diff">最终值变化附加数值(%)(乘算)</param>
        /// <returns>攻击方体重变化(值)，目标方体重变化(值)</returns>
        public virtual (double, double) Devour(double source, double target, double diff = 1)
        {
            Logger.Info($"进入吞噬词缀计算方法，攻击方体重={source}，目标方体重={target}，Diff={diff}");

            bool attackSuccess = false;
            Logger.Info($"体重相差百分比={Math.Abs((target - source) / source * 100)}%，随机胜负临界={AppConfig.ValueDevourDrawPercentage}%");
            if (Math.Abs((target - source) / source * 100) < AppConfig.ValueDevourDrawPercentage)
            {
                double random = CommonHelper.Random.NextDouble();
                attackSuccess = random < 0.5;
                Logger.Info($"判定随机数={random}，攻击是否成功={attackSuccess}");
            }
            else if (source > target)
            {
                Logger.Info($"攻击方体重大于被攻击方，攻击成功");
                attackSuccess = true;
            }
            else
            {
                Logger.Info($"攻击方体重小于于被攻击方，攻击失败");
                attackSuccess = false;
            }

            if (attackSuccess)
            {
                double loss = CommonHelper.Random.NextDouble(0.5, 1) * diff;
                double increment = target * loss;
                Logger.Info($"退出吞噬词缀计算方法，攻击方增加倍率={increment}，被攻击方失去倍率={loss}");
                return (increment, -1 * increment);
            }
            else
            {
                double loss = CommonHelper.Random.NextDouble(0.5, 0.7) * diff;
                double decrement = source * loss;
                Logger.Info($"退出吞噬词缀计算方法，攻击方失去倍率={decrement}，被攻击方增加倍率={loss}");
                return (-1 * decrement, decrement);
            }
        }

        /// <summary>
        /// 默认完成了道具扣除
        /// 每个次数可增加5%~10%的体重(加算)
        /// </summary>
        /// <param name="diff">最终值变化附加数值(%)(乘算)</param>
        /// <returns>体重变化(0.x)</returns>
        public virtual double Feed(int count, double diff = 1)
        {
            Logger.Info($"进入喂食词缀计算方法，数量={count}，Diff={diff}");

            double increment = 0;
            for (int i = 0; i < count; i++)
            {
                double random = CommonHelper.Random.Next(5, 10);
                double newIncrement = random / 100.0 * diff;
                increment += newIncrement;
                Logger.Info($"次数={i + 1}，随机数={random}，增量={newIncrement}，当前总增量={increment}");
            }
            Logger.Info($"退出喂食词缀计算方法，倍率={increment}");
            return increment;
        }

        public virtual double GetAscendSuccessRate(double value)
        {
            Logger.Info("进入渡劫成功率计算方法");
            Logger.Info($"退出渡劫成功率计算方法，成功率={value}");
            return value;
        }

        public virtual double GetTransmogrifyFailRate(double fail)
        {
            Logger.Info("进入幻化失败率计算方法");
            Logger.Info($"退出幻化失败率计算方法，失败率={fail}");

            return fail;
        }

        public virtual double GetTransmogrifyFailWeightLostRate(double lost)
        {
            Logger.Info("进入幻化失败后体重损失率计算方法");
            Logger.Info($"退出幻化失败后体重损失率计算方法，体重损失率={lost}");

            return lost;
        }

        /// <summary>
        /// 默认完成了道具扣除
        /// 强化目标是体重的50%
        /// 每个次数提供10%的概率
        /// 超出100%的部分额外倍率提升
        /// 强化失败损失体重的20%-50%
        /// 仅完成数值计算，不进行数值更改
        /// </summary>
        /// <param name="diff">最终值变化附加数值(%)(乘算)</param>
        /// <returns>体重变化(0.x)</returns>
        public virtual double Upgrade(int count, double diff = 1)
        {
            Logger.Info($"进入强化词缀计算方法，数量={count}，Diff={diff}");
            int successRandom = CommonHelper.Random.Next(0, 100);
            Logger.Info($"成功判定随机数：{successRandom}，临界：{count * 10}");
            if (successRandom >= count * 10)
            {
                int rd = CommonHelper.Random.Next(50, 80);
                Logger.Info($"判定失败，结果随机数：{rd}");
                diff = rd / 100.0 * diff;
                Logger.Info($"退出强化词缀计算方法，计算结果：{diff}");
                return diff;
            }
            Logger.Info($"判定成功");
            if (count <= 10)
            {
                Logger.Info($"基础倍率");

                diff = 1.5 * diff;
                Logger.Info($"退出强化词缀计算方法，计算结果：{diff}");
                return diff;
            }
            Logger.Info($"额外倍率");
            diff = count / 10.0 * 1.5 * diff;
            Logger.Info($"退出强化词缀计算方法，计算结果：{diff}");
            return diff;
        }
    }

    public class PetAttributeRandomInsatantiator
    {
        private Dictionary<Type, double> Implementations { get; set; } = [];

        public void AddImplementation<T>(double probability) where T : IPetAttribute
        {
            Implementations.Add(typeof(T), probability);
        }

        public IPetAttribute GetInstanceByID(bool attrbuteId, int id)
        {
            if (attrbuteId)
            {
                return (Enums.Attribute)id switch
                {
                    Enums.Attribute.Jin => new Jin(),
                    Enums.Attribute.Mu => new Mu(),
                    Enums.Attribute.Shui => new Shui(),
                    Enums.Attribute.Huo => new Huo(),
                    Enums.Attribute.Tu => new Tu(),
                    Enums.Attribute.Feng => new Feng(),
                    Enums.Attribute.Lei => new Lei(),
                    Enums.Attribute.Yin => new Yin(),
                    _ => new None(),
                };
            }
            else
            {
                return new AttributeB.AttributeB(id);
            }
        }

        public IPetAttribute GetRandomInstance()
        {
            double totalProbability = Implementations.Values.Sum();
            double randomValue = CommonHelper.Random.NextDouble() * totalProbability;

            double cumulativeProbability = 0.0;
            for (int i = 0; i < Implementations.Count; i++)
            {
                cumulativeProbability += Implementations.Values.ElementAt(i);
                if (randomValue <= cumulativeProbability)
                {
                    return (IPetAttribute)Activator.CreateInstance(Implementations.Keys.ElementAt(i));
                }
            }
            throw new InvalidOperationException("No implementation selected. This should never happen.");
        }
    }
}