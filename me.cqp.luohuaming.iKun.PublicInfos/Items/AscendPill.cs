namespace me.cqp.luohuaming.iKun.PublicInfos.Items
{
    public class AscendPill : Models.Items
    {
        public AscendPill(int count = 1)
        {
            ID = Enums.Items.AscendPill;
            Name = ItemConfig.AscendPillName;
            Description = ItemConfig.AscendPillDescription;
            Stackable = true;
            Count = count;
        }
    }
}