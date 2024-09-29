namespace me.cqp.luohuaming.iKun.PublicInfos.Items
{
    public class BlindBox : Models.Items
    {
        public BlindBox(int count = 1)
        {
            ID = Enums.Items.BlindBox;
            Name = "盲盒";
            Description = "能获得随机材料";
            Stackable = true;
            Count = count;
        }
    }
}