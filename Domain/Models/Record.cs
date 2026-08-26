using me.cqp.luohuaming.iKun.Infrastructure.Persistence;
using SqlSugar;

namespace me.cqp.luohuaming.iKun.Domain.Models;

/// <summary>鲲与群/QQ 的归属记录（ 孵化时写入）</summary>
[SugarTable]
public sealed class Record
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int ID { get; set; }

    public long Group { get; set; }

    public long QQ { get; set; }

    public int KunID { get; set; }

    public static void Add(Record record)
    {
        using var db = Db.CreateSession();
        db.Insertable(record).ExecuteCommand();
    }

    public static List<Record> ByGroup(long groupId)
    {
        using var db = Db.CreateSession();
        return db.Queryable<Record>().Where(x => x.Group == groupId).ToList();
    }

    public static List<Record> ByQQs(List<long> qqs)
    {
        using var db = Db.CreateSession();
        return db.Queryable<Record>().Where(x => qqs.Contains(x.QQ)).ToList();
    }

    public static Record? ByKunId(int kunId)
    {
        using var db = Db.CreateSession();
        return db.Queryable<Record>().Where(x => x.KunID == kunId).First();
    }
}
