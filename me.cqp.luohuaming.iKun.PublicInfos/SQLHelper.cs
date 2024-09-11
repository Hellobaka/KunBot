using me.cqp.luohuaming.iKun.PublicInfos.Models;
using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.IO;

namespace me.cqp.luohuaming.iKun.PublicInfos
{
    public static class SQLHelper
    {
        private static string DBPath => Path.Combine(MainSave.AppDirectory, "data.db");

        public static SqlSugarClient GetInstance()
        {
            return new(new ConnectionConfig()
            {
                ConnectionString = $"data source={DBPath}",
                DbType = DbType.Sqlite,
                IsAutoCloseConnection = false,
                InitKeyType = InitKeyType.Attribute,
            });
        }

        public static bool CreateDB()
        {
            try
            {
                using var db = GetInstance();
                db.DbMaintenance.CreateDatabase(DBPath);
                db.CodeFirst.InitTables(typeof(InventoryItem));
                db.CodeFirst.InitTables(typeof(Kun));
                db.CodeFirst.InitTables(typeof(Player));
                return true;
            }
            catch (Exception e)
            {
                MainSave.CQLog.Error("创建数据库", "创建数据库过程发生异常：" + e.Message + e.StackTrace);
                return false;
            }
        }

        public static void CreateItems()
        {
            string path = Path.Combine(MainSave.AppDirectory, "items.json");
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("物品列表未找到，请放置 items.json 至数据目录");
            }
            List<Items> items = JsonConvert.DeserializeObject<List<Items>>(File.ReadAllText(path));
            var db = GetInstance();

            foreach (var item in items)
            {
                if (!db.Queryable<Items>().Any(x => x.ID == item.ID))
                {
                    db.Insertable(item).ExecuteCommand();
                }
            }
        }
    }
}
