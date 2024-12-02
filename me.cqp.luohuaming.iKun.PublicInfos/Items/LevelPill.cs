using me.cqp.luohuaming.iKun.PublicInfos.Models;
using System;

namespace me.cqp.luohuaming.iKun.PublicInfos.Items
{
    public class LevelPill : Models.Items
    {
        public LevelPill(int count = 1)
        {
            ID = Enums.Items.LevelPill;
            Name = ItemConfig.LevelPillName;
            Description = ItemConfig.LevelPillDescription;
            Stackable = true;
            Count = count;
            Usable = true;
        }

        public override (bool, string) UseItem(int count, Player player, Kun kun)
        {
            try
            {
                kun.Level += count;
                kun.Update();

                return (true, string.Format(ItemConfig.UseLevelPill, count, count, kun.Level));
            }
            catch (Exception e)
            {
                MainSave.CQLog.Error("使用等级丹", $"异常发生：{e.Message}{e.StackTrace}");
                return (false, ItemConfig.UseItemException);
            }
        }
    }
}