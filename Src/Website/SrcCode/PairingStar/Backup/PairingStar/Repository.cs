using System;
using System.Data;
using System.Data.SQLite;
using System.Web;

namespace PairingStar.Controllers
{
    public class Repository
    {
        private static readonly String connString= string.Format("Data Source={0}Data\\PairingStar.db",HttpContext.Current.Server.MapPath("..//"));
            

        private static Repository _repo;
        public static Repository GetRepository()
        {
            return _repo ?? (_repo = new Repository());
        }

        private  Repository()
        {
            try
            {
                var dataTable = LoadData("select * from t_user");

            }
            catch (Exception)
            {

                ExecuteQuery(
                    "CREATE TABLE \"t_user\"(\"PK_ID\" INTEGER PRIMARY KEY AUTOINCREMENT,\"USERNAME\" TEXT NOT NULL collate nocase,\"ROLE\" TEXT NOT NULL DEFAULT \'DEV\' collate nocase,\"PHOTO\" BLOB,\"GENDER\" TEXT NOT NULL DEFAULT \'Male\' collate nocase)");
                ExecuteQuery(
                    "CREATE TABLE \"t_pairingmatrix\"(\"PK_ID\" INTEGER PRIMARY KEY AUTOINCREMENT,\"PAIRONE\" TEXT NOT NULL collate nocase,\"PAIRTWO\" TEXT NOT NULL collate nocase,\"PAIRDATE\" TEXT NOT NULL collate nocase,\"PAIRTIME\" REAL NOT NULL)");
            }
            
        }

        public void ExecuteQuery(params string[] queries)
        {
            using (var conn = new SQLiteConnection(connString))
            {
                foreach (var query in queries)
                {
                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }

                }

            }
        }


        public void ExecuteQuery(string query, SQLiteParameter[] queryParams)
        {
            
            using (var conn = new SQLiteConnection(connString))
            {
                using (var cmd = new SQLiteCommand(query, conn))
                    {
                        conn.Open();
                        cmd.Parameters.AddRange(queryParams);
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
            }
        }


        public DataTable LoadData(string query)
        {
            var startRecord = new DataTable();
            using (var conn = new SQLiteConnection(connString))
            {
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    conn.Open();
                    var dataAdapter=new SQLiteDataAdapter(cmd);
                    dataAdapter.Fill(startRecord);
                    conn.Close();
                }
            }

            return startRecord;

        }

        public T ExecuteScalar<T>(string query)
        {
            T returnValue;
            using (var conn = new SQLiteConnection(connString))
            {
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    conn.Open();
                    var executeScalar = cmd.ExecuteScalar();
                    returnValue = (T)Convert.ChangeType(executeScalar, typeof (T));
                    conn.Close();
                }
            }
            return returnValue;
        }
    }
}