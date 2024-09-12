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

        public virtual void Upgrade()
        {

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