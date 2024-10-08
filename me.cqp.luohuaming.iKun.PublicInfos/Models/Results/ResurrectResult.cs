using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace me.cqp.luohuaming.iKun.PublicInfos.Models.Results
{
    public class ResurrectResult
    {
        public bool Success { get; set; } = true;

        public int CurrentResurrectCount { get; set; }

        public double WeightLoss { get; set; }

        public double LevelLoss { get; set; }
    }
}
