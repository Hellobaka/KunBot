using SqlSugar;
using System;
using System.Collections.Generic;

namespace me.cqp.luohuaming.iKun.PublicInfos.Models
{
    [SugarTable]
    public class Player
    {
        [SugarColumn(IsPrimaryKey = true)]
        public int Id { get; set; }

        public long QQ { get; set; }

        public DateTime CreateAt { get; set; }
    }
}
