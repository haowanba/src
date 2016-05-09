using System;
using System.Data;
using System.Data.SqlClient;

namespace SqlServer
{
    public class DbManager
    {
        public static TResult ProcessInDb<TResult>(string connStr, bool needTransaction, Func<SqlCommand, TResult> process)
        {
            if (string.IsNullOrEmpty(connStr))
                throw new ArgumentNullException("connStr");
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    try
                    {
                        conn.Open();
                        cmd.CommandTimeout = 3600;
                        if (needTransaction)
                        {
                            var tran1 = conn.BeginTransaction(IsolationLevel.ReadCommitted);
                            cmd.Transaction = tran1;
                        }
                        TResult result = process(cmd);

                        if (cmd.Transaction != null)
                        {
                            cmd.Transaction.Commit();
                        }

                        return result;
                    }
                    catch (Exception)
                    {
                        if (cmd.Transaction != null)
                        {
                            cmd.Transaction.Rollback();
                        }
                        throw;
                    }
                }
            }
        }

        public static void ProcessInDb(string connStr, bool needTransaction, Action<SqlCommand> process)
        {
            if (string.IsNullOrEmpty(connStr))
                throw new ArgumentNullException("connStr");
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    try
                    {
                        conn.Open();
                        cmd.CommandTimeout = 3600;
                        if (needTransaction)
                        {
                            var tran1 = conn.BeginTransaction(IsolationLevel.ReadCommitted);
                            cmd.Transaction = tran1;
                        }
                        process(cmd);

                        if (cmd.Transaction != null)
                        {
                            cmd.Transaction.Commit();
                        }
                    }
                    catch (Exception)
                    {
                        if (cmd.Transaction != null)
                        {
                            cmd.Transaction.Rollback();
                        }
                        throw;
                    }
                }
            }
        }
    }
}
