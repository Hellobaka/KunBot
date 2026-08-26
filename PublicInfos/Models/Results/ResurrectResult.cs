namespace me.cqp.luohuaming.iKun.PublicInfos.Models.Results
{
    public class ResurrectResult
    {
        public bool Success { get; set; } = true;

        public int CurrentResurrectCount { get; set; }

        public double WeightLoss { get; set; }

        public int LevelLoss { get; set; }

        public override string ToString()
        {
            return $"当前复活次数={CurrentResurrectCount}，体重丢失={WeightLoss}，星级丢失={LevelLoss}";
        }
    }
}
