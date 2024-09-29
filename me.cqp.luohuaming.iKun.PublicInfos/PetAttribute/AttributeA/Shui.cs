using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace me.cqp.luohuaming.iKun.PublicInfos.PetAttribute.AttributeA
{
    public class Shui : IPetAttribute
    {
        public Shui()
        {
            ID = Enums.Attribute.Shui;
            Name = "水";
            Description = [
                "◆成功攻击或吞噬后大量提升体重",
                "◆被攻击或吞噬时有大概率逃脱",
                "◇对「火」属性的对手有额外攻击加成",
            ];
        }

        private Logger Logger { get; set; } = new Logger("词缀_水");

        public override (double, double) Attack(double source, double target, (double, double) baseAttack, double diff = 1)
        {
            Logger.Info($"进入攻击词缀计算方法，攻击方体重={source}，目标方体重={target}，Diff={diff}，基础倍率={baseAttack.Item1}，{baseAttack.Item2}");

            // 成功 攻击和吞噬 额外增加敌人损失体重的10~30%体重
            baseAttack = base.Attack(source, target, baseAttack, diff);
            Logger.Info($"基类方法计算结果={baseAttack.Item1}，{baseAttack.Item2}");
            if (baseAttack.Item1 > 1)
            {
                double increment = source * (baseAttack.Item1 - 1);
                double decrement = target * (1 - baseAttack.Item2);
                Logger.Info($"攻方增量={increment}，被攻方减量={decrement}");
                double random = CommonHelper.Random.NextDouble(0.3, 0.5);
                increment += decrement * random;
                Logger.Info($"加成增量={random}");
                Logger.Info($"使用加成计算后，攻方增量={increment}");

                baseAttack = (1 + (increment / source), 1 - (decrement / target));
            }
            Logger.Info($"退出词缀计算方法，计算结果={baseAttack.Item1}，{baseAttack.Item2}");
            return baseAttack;
        }

        public override (double, double) Devour(double source, double target, double diff = 1)
        {
            Logger.Info($"进入吞噬词缀计算方法，攻击方体重={source}，目标方体重={target}，Diff={diff}");

            // 成功 攻击和吞噬 额外增加敌人体重的10~30%体重
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
            // 被攻击或吞噬时 30% 的几率逃脱
            Logger.Info($"进入被攻击词缀计算方法，攻击方体重={source}，目标方体重={target}，原始倍率={baseAttack.Item1}, {baseAttack.Item2}");
            double random = CommonHelper.Random.NextDouble();
            Logger.Info($"逃脱随机数={random}，临界={0.3}");
            if (CommonHelper.Random.NextDouble() < 0.3)
            {
                Logger.Info("判定成功");
                baseAttack = (1, 1);
            }
            Logger.Info($"退出词缀计算方法，计算结果={baseAttack.Item1}，{baseAttack.Item2}");
            return baseAttack;
        }

        public override (double, double) BeingDevoured(double source, double target, (double, double) baseDevour)
        {
            // 被攻击或吞噬时 30% 的几率逃脱
            Logger.Info($"进入被吞噬词缀计算方法，攻击方体重={source}，目标方体重={target}，原始倍率={baseDevour.Item1}, {baseDevour.Item2}");
            double random = CommonHelper.Random.NextDouble();
            Logger.Info($"逃脱随机数={random}，临界={0.3}");
            if (CommonHelper.Random.NextDouble() < 0.3)
            {
                Logger.Info("判定成功");
                baseDevour = (1, 1);
            }
            Logger.Info($"退出词缀计算方法，计算结果={baseDevour.Item1}，{baseDevour.Item2}");
            return baseDevour;
        }
    }
}
