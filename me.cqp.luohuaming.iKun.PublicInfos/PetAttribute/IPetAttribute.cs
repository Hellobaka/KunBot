using me.cqp.luohuaming.iKun.PublicInfos.Enums;
using me.cqp.luohuaming.iKun.PublicInfos.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace me.cqp.luohuaming.iKun.PublicInfos.PetAttribute
{
    public interface IPetAttribute
    {
        public int ID { get; }
        
        public string Name { get; set; }
        
        public string Description { get; set; }

        public bool BeforeAction(ActionType action, Kun kun, out string msg);

        public void AfterAction(ActionType action, Kun kun);

        public bool BeforeAttack(AttackType action, Kun origin, Kun target, out string msg);

        public void AfterAttack(AttackType action, Kun origin, Kun target);
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class PetAttribute(int id) : Attribute
    {
        public int ID { get; set; } = id;
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

        public IPetAttribute GetInstanceByID(int id)
        {
            var item = Implementations.FirstOrDefault(x => ((PetAttribute)Attribute.GetCustomAttribute(x.Key, typeof(PetAttribute))).ID == id);
            if (item.Key == null)
            {
                return null;
            }

            return (IPetAttribute)Activator.CreateInstance(item.Key);
        }
    }
}