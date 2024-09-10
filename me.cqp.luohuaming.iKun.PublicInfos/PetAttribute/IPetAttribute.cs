using me.cqp.luohuaming.iKun.PublicInfos.Enums;
using me.cqp.luohuaming.iKun.PublicInfos.Models;

namespace me.cqp.luohuaming.iKun.PublicInfos.PetAttribute
{
    public interface IPetAttribute
    {
        public int ID { get; set; }
        
        public string Name { get; set; }
        
        public string Description { get; set; }

        public void BeforeAction(ActionType action, Kun kun);

        public void AfterAction(ActionType action, Kun kun);

        public void BeforeAttack(AttackType action, Kun origin, Kun target);

        public void AfterAttack(AttackType action, Kun origin, Kun target);
    }
}