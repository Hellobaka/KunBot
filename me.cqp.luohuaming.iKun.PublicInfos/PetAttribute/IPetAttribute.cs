using me.cqp.luohuaming.iKun.PublicInfos.Enums;
using me.cqp.luohuaming.iKun.PublicInfos.Models;

namespace me.cqp.luohuaming.iKun.PublicInfos.PetAttribute
{
    public interface IPetAttribute
    {
        public int ID { get; set; }
        
        public string Name { get; set; }
        
        public string Description { get; set; }

        public bool BeforeAction(ActionType action, Kun kun, out string msg);

        public void AfterAction(ActionType action, Kun kun);

        public bool BeforeAttack(AttackType action, Kun origin, Kun target, out string msg);

        public void AfterAttack(AttackType action, Kun origin, Kun target);
    }
}