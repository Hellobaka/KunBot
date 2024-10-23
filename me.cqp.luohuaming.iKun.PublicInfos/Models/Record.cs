using SqlSugar;
using System.Collections.Generic;

namespace me.cqp.luohuaming.iKun.PublicInfos.Models
{
    [SugarTable]
    public class Record
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int ID { get; set; }

        public long Group { get; set; }

        public long QQ { get; set; }

        public int KunID { get; set; }

        public static void AddRecord(Record record)
        {
            var db = SQLHelper.GetInstance();
            db.Insertable(record).ExecuteCommand();
        }

        public static List<Record> GetRecordsByGroupID(long groupID)
        {
            var db = SQLHelper.GetInstance();
            return db.Queryable<Record>().Where(x => x.Group == groupID).ToList();
        }

        public static Record? GetRecordByKunID(int kunId)
        {
            var db = SQLHelper.GetInstance();
            return db.Queryable<Record>().Where(x => x.KunID == kunId).First();
        }
    }
}