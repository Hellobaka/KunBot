using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace me.cqp.luohuaming.iKun.PublicInfos.Models.Results
{
    public class UpgradeResult
    {
        public bool Success { get; set; } = true;
        
        public double CurrentWeight { get; set; }

        public double Increment { get; set; }

        public bool WeightLimit { get; set; }
    }
}
