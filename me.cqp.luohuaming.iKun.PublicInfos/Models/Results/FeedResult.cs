namespace me.cqp.luohuaming.iKun.PublicInfos.Models.Results
{
    public class FeedResult
    {
        public bool Success { get; set; } = true;

        public double CurrentWeight { get; set; }

        public double Increment { get; set; }

        public bool WeightLimit { get; set; }
    }
}