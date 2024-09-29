namespace me.cqp.luohuaming.iKun.PublicInfos.Items
{
    public class KunEgg : Models.Items
    {
        public KunEgg(int count = 1)
        {
            ID = Enums.Items.KunEgg;
            Name = "鲲之蛋";
            Description = "可用于孵化、强化鲲";
            Stackable = true;
            Count = count;
        }
    }
}