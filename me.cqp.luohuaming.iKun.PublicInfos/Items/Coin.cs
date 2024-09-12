namespace me.cqp.luohuaming.iKun.PublicInfos.Items
{
    [Item(1)]
    public class Coin : Models.Items
    {
        public Coin(int count = 1)
        {
            ID = 1;
            Name = "金币";
            Description = "大陆上通用的货币";
            Stackable = true;
            Count = count;
        }
    }
}
