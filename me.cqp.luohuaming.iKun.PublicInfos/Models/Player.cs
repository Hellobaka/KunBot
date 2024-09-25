using SqlSugar;
using System;
using System.Collections.Generic;

namespace me.cqp.luohuaming.iKun.PublicInfos.Models
{
    [SugarTable]
    public class Player
    {
        [SugarColumn(IsPrimaryKey = true)]
        public long QQ { get; set; }

        public DateTime CreateAt { get; set; }

        public DateTime LoginAt { get; set; }

        public static bool Exists(long qq) => SQLHelper.GetInstance().Queryable<Player>().Any(p => p.QQ == qq);

        public static Player? Create(long qq)
        {
            var db = SQLHelper.GetInstance();
            var player = new Player()
            {
                QQ = qq,
                CreateAt = DateTime.Now,
            };
            if (db.Insertable(player).ExecuteCommand() > 0)
            {
                return player;
            }
            return null;
        }

        public void GiveItem(List<Items> items)
        {
            var db = SQLHelper.GetInstance();
            foreach (var item in items) 
            {
                var query = db.Queryable<InventoryItem>().First(x => x.ItemID == (int)item.ID && x.PlayerID == QQ);
                if (query == null || !item.Stackable)
                {
                    db.Insertable(new InventoryItem
                    {
                        Count = item.Count,
                        ItemID = (int)item.ID,
                        PlayerID = QQ
                    }).ExecuteCommand();
                    continue;
                }

                if (item.Stackable)
                {
                    query.Count += item.Count;
                    db.Updateable(query).ExecuteCommand();
                }
            }
        }

        public void Update()
        {
            var db = SQLHelper.GetInstance();
            db.Updateable(this).ExecuteCommand();
        }


        public static Player? GetPlayer(long qq)
        {
            var db = SQLHelper.GetInstance();
            return db.Queryable<Player>().First(x=> x.QQ == qq);
        }
    }
}
