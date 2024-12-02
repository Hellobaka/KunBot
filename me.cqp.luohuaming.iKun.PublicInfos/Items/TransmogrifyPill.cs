namespace me.cqp.luohuaming.iKun.PublicInfos.Items
{
    public class TransmogrifyPill : Models.Items
    {
        public TransmogrifyPill(int count = 1)
        {
            ID = Enums.Items.TransmogrifyPill;
            Name = ItemConfig.TransmogrifyPillName;
            Description = ItemConfig.TransmogrifyPillDescription;
            Stackable = true;
            Count = count;
            Usable = true;
        }
    }
}