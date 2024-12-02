using me.cqp.luohuaming.iKun.PublicInfos.Models;
using System;

namespace me.cqp.luohuaming.iKun.PublicInfos.Items
{
    public class WeightPill : Models.Items
    {
        public WeightPill(int count = 1)
        {
            ID = Enums.Items.WeightPill;
            Name = ItemConfig.WeightPillName;
            Description = ItemConfig.WeightPillDescription;
            Stackable = true;
            Count = count;
            Usable = true;
        }

        public override (bool, string) UseItem(int count, Player player, Kun kun)
        {
            try
            {
                kun.Weight = Kun.GetLevelWeightLimit(kun.Level);
                kun.Update();

                return (true, string.Format(ItemConfig.UseWeightPill, kun.Weight));
            }
            catch (Exception e)
            {
                MainSave.CQLog.Error("使用体重丹", $"异常发生：{e.Message}{e.StackTrace}");
                return (false, ItemConfig.UseItemException);
            }
        }
    }
}