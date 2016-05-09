using System;
using System.Data.SqlClient;
using Common.Define;

namespace SqlServer
{
    public class ShakeItemDbHelper
    {
        private const string ConnStr = "Server=tcp:x80axq4pow.database.chinacloudapi.cn,1433;Database=DB_VB5KBFQ6HI;User ID=wxcs@x80axq4pow;Password=fa450@flse;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;";

        internal static bool DbExist(string dbName)
        {
            if (string.IsNullOrEmpty(dbName))
                throw new ArgumentNullException("dbName");
            return DbManager.ProcessInDb(ConnStr, false, command =>
            {
                command.CommandText = string.Format("select COUNT(1) from sysdatabases where name = '{0}'", dbName);
                if (Int32.Parse(command.ExecuteScalar().ToString()) <= 0)
                {
                    return false;
                }
                return true;
            });
        }

        public static void Insert(ShakeItem item)
        {
            if (String.IsNullOrEmpty(item.Id))
            {
                item.Id = Guid.NewGuid().ToString();
            }
            DbManager.ProcessInDb(ConnStr, false, command =>
            {
                command.CommandText = GetInsertSql(item);
                command.ExecuteNonQuery();
            });
        }

        public static void Update(ShakeItem item)
        {
            DbManager.ProcessInDb(ConnStr, false, command =>
            {
                command.CommandText = GetUpdateSql(item);
                command.ExecuteNonQuery();
            });
        }

        public static void Update(string id, string score)
        {
            DbManager.ProcessInDb(ConnStr, false, command =>
            {
                command.CommandText = GetUpdateScoreSql(id, score);
                command.ExecuteNonQuery();
            });
        }

        public static ShakeItem GetById(string id)
        {
            return DbManager.ProcessInDb(ConnStr, false, command =>
            {
                command.CommandText = GetSelectByIdSql(id);
                SqlDataReader reader = command.ExecuteReader();
                ShakeItem item = new ShakeItem { Id = id };
                if (reader.Read())
                {
                    item.Name = reader["NAME"].ToString();
                    item.Telephone = reader["TELEPHONE"].ToString();
                    item.Area = reader["AREA"].ToString();
                    item.Score = Convert.ToInt32(reader["SCORE"]);
                }
                return item;
            });
        }

        public static ShakeItem GetByTelephone(string tel)
        {
            return DbManager.ProcessInDb(ConnStr, false, command =>
            {
                command.CommandText = GetSelectByTelephoneSql(tel);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    ShakeItem item = new ShakeItem
                    {
                        Id = reader["ID"].ToString(),
                        Name = reader["NAME"].ToString(),
                        Telephone = reader["TELEPHONE"].ToString(),
                        Area = reader["AREA"].ToString(),
                        Score = Convert.ToInt32(reader["SCORE"])
                    };
                    return item;
                }
                return null;
            });
        }

        private static string GetInsertSql(ShakeItem item)
        {
            return String.Format("INSERT INTO HWB (ID,NAME,TELEPHONE,AREA,SCORE) VALUES('{0}','{1}','{2}','{3}',{4})", item.Id, item.Name, item.Telephone, item.Area, item.Score);
        }

        private static string GetUpdateSql(ShakeItem item)
        {
            return String.Format("UPDATE HWB SET NAME='{1}',TELEPHONE='{2}',AREA = '{3}',SCORE = {4} WHERE ID='{0}'", item.Id, item.Name, item.Telephone, item.Area, item.Score);
        }

        private static string GetUpdateScoreSql(string id, string score)
        {
            return String.Format("UPDATE HWB SET SCORE = {1} WHERE ID='{0}'", id, score);
        }

        private static string GetSelectByIdSql(string id)
        {
            return String.Format("SELECT * from HWB WHERE ID='{0}'", id);
        }

        private static string GetSelectByTelephoneSql(string tel)
        {
            return String.Format("SELECT * from HWB WHERE TELEPHONE='{0}'", tel);
        }
    }
}
