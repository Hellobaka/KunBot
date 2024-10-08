namespace me.cqp.luohuaming.iKun.PublicInfos.Items
{
    public class UpgradePill : Models.Items
    {
        public UpgradePill(int count = 1)
        {
            ID = Enums.Items.UpgradePill;
            Name = ItemConfig.UpgradePillName;
            Description = ItemConfig.UpgradePillDescription;
            Stackable = true;
            Count = count;
        }
    }
}