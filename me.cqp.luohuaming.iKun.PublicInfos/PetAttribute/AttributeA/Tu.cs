using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace me.cqp.luohuaming.iKun.PublicInfos.PetAttribute.AttributeA
{
    public class Tu : IPetAttribute
    {
        public Tu()
        {
            ID = Enums.Attribute.Tu;
            Name = "土";
            Description = [
                "◆成功吞噬后大量提升体重",
                "◆被攻击时减少巨量伤害",
                "◆渡劫时大幅提高成功概率",
                "◇对「水」属性的对手有额外攻击加成",
            ];
        }

        private Logger Logger { get; set; } = new Logger("词缀_土");

        public override (double, double) Devour(double source, double target, double diff = 1)
        {
            Logger.Info($"进入吞噬词缀计算方法，攻击方体重={source}，目标方体重={target}，Diff={diff}");

            // 成功 吞噬 额外增加敌人体重 10~30%体重
            var baseDevour = base.Devour(source, target, diff);
            Logger.Info($"基类方法计算结果={baseDevour.Item1}，{baseDevour.Item2}");
            if (baseDevour.Item1 > 1)
            {
                Logger.Info("吞噬判定成功，进行加成");
                double random = CommonHelper.Random.NextDouble(0.1, 0.3);
                Logger.Info($"随机增量随机数={random}");
                baseDevour = (baseDevour.Item1 * (1 + random), baseDevour.Item2);
            }
            Logger.Info($"退出词缀计算方法，计算结果={baseDevour.Item1}，{baseDevour.Item2}");
            return baseDevour;
        }

        public override (double, double) BeingAttacked(double source, double target, (double, double) baseAttack)
        {
            // 被攻击时减少 50% 的伤害
            Logger.Info($"进入被攻击词缀计算方法，攻击方体重={source}，目标方体重={target}，原始倍率={baseAttack.Item1}, {baseAttack.Item2}");
            if (baseAttack.Item2 < 1)
            {
                double change = 0.5;
                Logger.Info($"减伤={change}");
                double decrement = target * (1 - baseAttack.Item2);
                Logger.Info($"被攻方减量={decrement}");

                decrement *= change;
                Logger.Info($"使用减伤计算后，被攻方减量={decrement}");

                baseAttack = (baseAttack.Item1, 1 - (decrement / target));
            }
            Logger.Info($"退出词缀计算方法，计算结果={baseAttack.Item1}，{baseAttack.Item2}");
            return baseAttack;
        }

        public override double GetAscendSuccessRate(double value)
        {
            Logger.Info("进入渡劫成功率计算方法");
            // 渡劫时 提升 30% 概率
            double rate = value * 1.3;
            Logger.Info($"退出渡劫成功率计算方法，成功率={rate}");
            return rate;
        }
    }
}
