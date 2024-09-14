using me.cqp.luohuaming.iKun.PublicInfos.Enums;
using me.cqp.luohuaming.iKun.PublicInfos.Models;
using me.cqp.luohuaming.iKun.PublicInfos.PetAttribute.AttributeA;
using System;
using System.Collections.Generic;
using System.Linq;

namespace me.cqp.luohuaming.iKun.PublicInfos.PetAttribute
{
    public abstract class IPetAttribute
    {
        public Attributes ID { get; set; }
        
        public string Name { get; set; }
        
        public string Description { get; set; }

        /// <summary>
        /// 默认完成了道具扣除
        /// 强化目标是体重的50%
        /// 每个次数提供10%的概率
        /// 超出100%的部分额外倍率提升
        /// 强化失败损失体重的20%
        /// 仅完成数值计算，不进行数值更改
        /// </summary>
        /// <returns>体重变化百分比</returns>
        public virtual double Upgrade(int count)
        {
            int successRandom = CommonHelper.Random.Next(0, 100);
            if (successRandom >= count * 10)
            {
                return 0.8;
            }
            if (count <= 10)
            {
                return 1.5;
            }
            return count / 10.0 * 1.5;
        }

        /// <summary>
        /// 默认完成了道具扣除
        /// 
        /// </summary>
        public virtual void Ascend()
        {
            throw new NotImplementedException();
        }

        public virtual void Devour()
        {

        }

        public virtual void Feed()
        {
            throw new NotImplementedException();
        }

        public virtual void Release()
        {
            throw new NotImplementedException();
        }

        public virtual void Resurrect()
        {
            throw new NotImplementedException();
        }

        public virtual void Transmogrify()
        {
            throw new NotImplementedException();
        }

        public virtual void Strike(Kun target)
        {
            throw new NotImplementedException();
        }

        public virtual void Attack(Kun target)
        {
            throw new NotImplementedException();
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

        public IPetAttribute GetInstanceByID(Attributes id)
        {
            return id switch
            {
                Attributes.Ao => new Ao(),
                Attributes.Bei => new Bei(),
                Attributes.Chan => new Chan(),
                Attributes.Du => new Du(),
                Attributes.Duo => new Duo(),
                Attributes.Nu => new Nu(),
                Attributes.Tan => new Tan(),
                Attributes.Yin => new Yin(),
                _ => new None(),
            };
        }
    }
}