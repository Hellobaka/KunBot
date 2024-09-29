namespace me.cqp.luohuaming.iKun.PublicInfos.Models.Results
{
    public class AscendResult
    {
        public bool Success { get; set; } = true;

        public double Increment { get; set; }

        public double CurrentWeight { get; set; }

        public int CurrentLevel { get; set; }

        public bool Dead { get; set; }
    }
}