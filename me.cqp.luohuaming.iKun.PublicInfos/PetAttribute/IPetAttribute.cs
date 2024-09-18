using me.cqp.luohuaming.iKun.PublicInfos.Enums;
using me.cqp.luohuaming.iKun.PublicInfos.Models;
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
    /// 同时造成与受到伤害的可直接再次进行伤害计算操作
    /// </summary>
    public abstract class IPetAttribute
    {
        public Enums.AttributeA ID { get; set; }
        
        public string Name { get; set; }
        
        public string[] Description { get; set; }

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
            int successRandom = CommonHelper.Random.Next(0, 100);
            if (successRandom >= count * 10)
            {
                return CommonHelper.Random.Next(50, 80) / 100.0 * diff;
            }
            if (count <= 10)
            {
                return 1.5 * diff;
            }
            return count / 10.0 * 1.5 * diff;
        }

        /// <summary>
        /// 默认完成了道具扣除
        /// 成功增加50%，失败扣除50%
        /// </summary>
        /// <param name="success">基础成功率(0.x)</param>
        /// <param name="diff">最终值变化附加数值(%)(乘算)</param>
        /// <returns>体重变化(0.x)</returns>
        public virtual double Ascend(double success, double diff = 1)
        {
            return (CommonHelper.Random.NextDouble() <= success ? 1.5 : 0.5) * diff;
        }

        /// <summary>
        /// 目标方体重在攻击方的±百分比范围内时，双方50%胜负
        /// 目标方体重大于攻击方，攻击失败
        /// 目标方体重小于攻击方，攻击成功
        /// 成功时攻击方增加目标方失去的体重，目标方失去10%~50%体重
        /// 失败时目标方增加攻击方失去的体重，攻击方失去10%~50%体重
        /// </summary>
        /// <param name="source">攻击方体重</param>
        /// <param name="target">目标方体重</param>
        /// <param name="diff">最终值变化附加数值(%)(乘算)</param>
        /// <returns>攻击方体重变化(0.x)，目标方体重变化(0.x)</returns>
        public virtual (double, double) Devour(double source, double target, double diff = 1)
        {
            bool attackSuccess = false;
            if (Math.Abs((target - source) / source * 100) < AppConfig.ValueDevourDrawPercentage)
            {
                attackSuccess = CommonHelper.Random.NextDouble() < 0.5;
            }
            else if(source > target)
            {
                attackSuccess = true;
            }
            else
            {
                attackSuccess = false;
            }

            double loss = CommonHelper.Random.Next(10, 50) / 100 * diff;
            if (attackSuccess)
            {
                return (1 + loss, 1 - loss);
            }
            else
            {
                return (1 - loss, 1 + loss);
            }
        }

        public virtual (double, double) BeingDevoured(double source, double target, (double, double) baseDevour)
        {
            return baseDevour;
        }

        /// <summary>
        /// 默认完成了道具扣除
        /// 每个次数可增加5%~10%的体重(加算)
        /// </summary>
        /// <param name="diff">最终值变化附加数值(%)(乘算)</param>
        /// <returns>体重变化(0.x)</returns>
        public virtual double Feed(int count, double diff = 1)
        {
            double increasement = 0;
            for (int i = 0; i < count; i++)
            {
                increasement += CommonHelper.Random.Next(5, 10) / 100.0 * diff;
            }

            return increasement;
        }

        /// <summary>
        /// 被攻击方损失概率体重，攻击方增加对应体重
        /// </summary>
        /// <param name="source">攻击方体重</param>
        /// <param name="target">目标方体重</param>
        /// <param name="diff">攻击附加变化(%)(乘算)</param>
        /// <returns>攻击方体重变化(0.x)，目标方体重变化(0.x)</returns>
        public virtual (double, double) Attack(double source, double target, double diff = 1)
        {
            double decrement = CommonHelper.Random.NextDouble(AppConfig.ValueAttackWeightMinimumDecrement / 100.0, AppConfig.ValueAttackWeightMaximumDecrement / 100.0)
                                    * diff;
            double value = source * decrement;

            double increment = value / target;

            return (1 + increment, 1 - decrement);
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
            return baseAttack;
        }
    }

    public class PetAttributeRandomInsatantiator
    {
        private Dictionary<Type, double> Implementations { get; set; }

        private Random Random { get; set; }

        public void AddImplementation<T>(double probability) where T : IPetAttribute
        {
            Implementations.Add(typeof(T), probability);
        }

        public IPetAttribute GetRandomInstance()
        {
            double totalProbability = Implementations.Values.Sum();
            double randomValue = Random.NextDouble() * totalProbability;

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

        public IPetAttribute GetInstanceByID(bool attrbuteA, int id)
        {
            if (attrbuteA) 
            {
                return (Enums.AttributeA)id switch
                {
                    Enums.AttributeA.Jin => new Jin(),
                    Enums.AttributeA.Mu => new Mu(),
                    Enums.AttributeA.Shui => new Shui(),
                    Enums.AttributeA.Huo => new Huo(),
                    Enums.AttributeA.Tu => new Tu(),
                    Enums.AttributeA.Feng => new Feng(),
                    Enums.AttributeA.Lei => new Lei(),
                    Enums.AttributeA.Yin => new Yin(),
                    _ => new None(),
                };
            }
            else
            {
                return (Enums.AttributeB)id switch
                {
                    _ => new None(),
                };
            }            
        }
    }
}