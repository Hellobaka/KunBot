namespace me.cqp.luohuaming.iKun.PublicInfos.Items
{
    public class UpgradePill : Models.Items
    {
        public UpgradePill(int count = 1)
        {
            ID = Enums.Items.UpgradePill;
            Name = "强化丸";
            Description = "能用于强化鲲";
            Stackable = true;
            Count = count;
        }
    }
}