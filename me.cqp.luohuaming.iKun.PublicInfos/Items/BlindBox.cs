namespace me.cqp.luohuaming.iKun.PublicInfos.Items
{
    public class BlindBox : Models.Items
    {
        public BlindBox(int count = 1)
        {
            ID = Enums.Items.BlindBox;
            Name = ItemConfig.BlindBoxName;
            Description = ItemConfig.BlindBoxDescription;
            Stackable = true;
            Count = count;
        }
    }
}