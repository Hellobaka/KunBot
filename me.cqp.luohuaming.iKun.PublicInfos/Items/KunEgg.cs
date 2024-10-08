namespace me.cqp.luohuaming.iKun.PublicInfos.Items
{
    public class KunEgg : Models.Items
    {
        public KunEgg(int count = 1)
        {
            ID = Enums.Items.KunEgg;
            Name = ItemConfig.KunEggName;
            Description = ItemConfig.KunEggDescription;
            Stackable = true;
            Count = count;
        }
    }
}