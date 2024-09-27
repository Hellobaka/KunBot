using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace me.cqp.luohuaming.iKun.PublicInfos.Models.Results
{
    public class AttackResult
    {
        public bool Success { get; set; } = true;

        public double Increment { get; set; }

        public double CurrentWeight { get; set; }

        public bool Dead { get; set; }

        public double TargetDecrement { get; set; }

        public double TargetCurrentWeight { get; set; }

        public bool TargetDead { get; set; }

        public override string ToString()
        {
            return $"执行成功={Success}，攻击方增量={Increment}，攻击方当前体重={CurrentWeight}，攻击方是否死亡={Dead}，" +
                $"被攻击方增量={TargetDecrement}，被攻击方当前体重={TargetCurrentWeight}，被攻击方是否死亡={TargetDead}";
        }
    }
}
