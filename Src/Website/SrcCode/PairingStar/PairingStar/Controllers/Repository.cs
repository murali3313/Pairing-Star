using System;
using System.Data;
using System.Data.SQLite;

namespace PairingStar.Controllers
{
    public class Repository
    {
        readonly String connString;
        private static Repository _repo;
        public static Repository GetRepository()
        {
            return _repo ?? (_repo = new Repository());
        }

        private  Repository()
        {
            connString = "Data Source=D:\\PS\\Pairing-Star\\Src\\Website\\Build\\DB\\PairingStar.db";
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