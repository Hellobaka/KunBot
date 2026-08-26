using me.cqp.luohuaming.iKun.Domain.Models;
using me.cqp.luohuaming.iKun.Infrastructure.Logging;
using SqlSugar;

namespace me.cqp.luohuaming.iKun.Infrastructure.Persistence;

/// <summary>
/// SQLite 数据库入口：连接工厂与建表。
/// </summary>
public static class Db
{
    private static readonly Log Log = Log.For(nameof(Db));

    public static string DatabasePath => Path.Combine(Runtime.DataDirectory, "data.db");

    public static SqlSugarClient CreateSession() => new(new ConnectionConfig
    {
        ConnectionString = $"data source={DatabasePath}",
        DbType = DbType.Sqlite,
        IsAutoCloseConnection = false,
        InitKeyType = InitKeyType.Attribute,
    });

    public static void Initialize()
    {
        try
        {
            using var db = CreateSession();
            db.DbMaintenance.CreateDatabase(DatabasePath);
            db.CodeFirst.InitTables(typeof(InventoryItem));
            db.CodeFirst.InitTables(typeof(Kun));
            db.CodeFirst.InitTables(typeof(Player));
            db.CodeFirst.InitTables(typeof(AutoPlay));
            db.CodeFirst.InitTables(typeof(Record));
        }
        catch (Exception e)
        {
            Log.Error(e, "创建数据库过程发生异常");
        }
    }
}