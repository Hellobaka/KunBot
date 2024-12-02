namespace me.cqp.luohuaming.iKun.PublicInfos.Items
{
    public class Coin : Models.Items
    {
        public Coin(int count = 1)
        {
            ID = Enums.Items.Coin;
            Name = ItemConfig.CoinName;
            Description = ItemConfig.CoinDescription;
            Stackable = true;
            Count = count;
            Usable = false;
        }
    }
}