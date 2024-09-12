namespace me.cqp.luohuaming.iKun.PublicInfos.Items
{
    [Item(2)]
    public class KunEgg : Models.Items
    {
        public KunEgg(int count = 1)
        {
            ID = 2;
            Name = "鲲之蛋";
            Description = "可用于孵化、强化鲲";
            Stackable = true;
            Count = count;
        }
    }
}
