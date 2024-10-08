namespace me.cqp.luohuaming.iKun.PublicInfos.Items
{
    public class ResurrectPill : Models.Items
    {
        public ResurrectPill(int count = 1)
        {
            ID = Enums.Items.ResurrectPill;
            Name = ItemConfig.ResurrectPillName;
            Description = ItemConfig.ResurrectPillDescription;
            Stackable = true;
            Count = count;
        }
    }
}