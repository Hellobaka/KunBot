using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace me.cqp.luohuaming.iKun.PublicInfos.PetAttribute.AttributeA
{
    public class Lei : IPetAttribute
    {
        public Lei()
        {
            ID = Enums.Attribute.Lei;
            Name = "雷";
            Description = [
                "◆攻击时有大概率造成额外少量伤害",
                "◆渡劫时超巨幅提升成功概率",
                "◆被攻击时小概率反弹大量伤害",
                "◇对「水、金、木」属性的对手有额外攻击加成",
                "◇对「土、火」属性的攻击额外防御加成",
            ];
        }

        private Logger Logger { get; set; } = new Logger("词缀_雷");

        public override (double, double) Attack(double source, double target, (double, double) baseAttack, double diff = 1)
        {
            Logger.Info($"进入攻击词缀计算方法，攻击方体重={source}，目标方体重={target}，Diff={diff}，基础倍率={baseAttack.Item1}，{baseAttack.Item2}");

            // 攻击时 50% 的几率造成额外 10~30% 伤害
            baseAttack = base.Attack(source, target, baseAttack, diff);
            Logger.Info($"基类方法计算结果={baseAttack.Item1}，{baseAttack.Item2}");
            double random = CommonHelper.Random.NextDouble();
            Logger.Info($"攻击加成随机数={random}，临界={0.5}");
            if (baseAttack.Item1 > 1 && random < 0.5)
            {
                Logger.Info("攻击判定成功，进行加成");

                double change = CommonHelper.Random.NextDouble(0.1, 0.3);
                Logger.Info($"加成增量={change}");
                double increment = source * (baseAttack.Item1 - 1);
                double decrement = target * (1 - baseAttack.Item2);
                Logger.Info($"攻方增量={increment}，被攻方减量={decrement}");

                decrement -= decrement * change;
                increment += decrement;
                Logger.Info($"使用加成计算后，攻方增量={increment}，被攻方减量={decrement}");

                baseAttack = (1 + (increment / source), 1 - (decrement / target));
            }
            Logger.Info($"退出词缀计算方法，计算结果={baseAttack.Item1}，{baseAttack.Item2}");
            return baseAttack;
        }

        public override (double, double) BeingAttacked(double source, double target, (double, double) baseAttack)
        {
            Logger.Info($"进入被攻击词缀计算方法，攻击方体重={source}，目标方体重={target}，原始倍率={baseAttack.Item1}, {baseAttack.Item2}");
            double random = CommonHelper.Random.NextDouble();
            Logger.Info($"减少攻击加成随机数={random}，临界={0.1}");

            // 被攻击时 10% 概率 敌人受到 30~50% 我方受到的伤害
            if (random <= 0.1)
            {
                double diff = 1 + CommonHelper.Random.NextDouble(0.3, 0.5);
                Logger.Info($"加成增量={diff}");
                double increment = source * (baseAttack.Item1 - 1);
                double decrement = target * (1 - baseAttack.Item2) * diff;
                Logger.Info($"攻方增量={increment}，被攻方减量={decrement}");

                baseAttack = (1 + (increment - decrement) / source, baseAttack.Item2);
            }
            Logger.Info($"退出词缀计算方法，计算结果={baseAttack.Item1}，{baseAttack.Item2}");
            return baseAttack;
        }

        public override double GetAscendSuccessRate(double value)
        {
            Logger.Info("进入渡劫成功率计算方法");
            // 渡劫时 提升 100% 概率
            double rate = value * 2;
            Logger.Info($"退出渡劫成功率计算方法，成功率={rate}");
            return rate;
        }
    }
}
