using me.cqp.luohuaming.iKun.PublicInfos.PetAttribute;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace me.cqp.luohuaming.iKun.PublicInfos.Models
{
    [SugarTable]
    public class Kun
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        public int AttributeID { get; set; }

        public int PlayerID { get; set; }

        public double Weight { get; set; }

        public bool Abandoned { get; set; }

        public bool Alive { get; set; }

        [SugarColumn(IsIgnore = true)]
        public IPetAttribute PetAttribute { get; set; }

        public void Initialize()
        {

        }

        public double AddWeight(double weight)
        {
            return Weight + weight;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
