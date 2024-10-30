using System;

namespace me.cqp.luohuaming.iKun.PublicInfos.Models.Results
{
    public class AutoPlayResult
    {
        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public TimeSpan Duration { get; set; }

        public int CurrentCoin { get; set; }

        public double CurrentWeight { get; set; }

        public double Increment { get; set; }

        public bool WeightLimit { get; set; }

        public bool Dead { get; set; }
    }
}