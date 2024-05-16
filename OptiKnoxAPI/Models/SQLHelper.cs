using Microsoft.Data.SqlClient;
using System.Data;
namespace OptiKnoxAPI.Models
{
    public class SQLHelper
    {
        public static int ExecuteNonQuery(SqlCommand cmd, CommandType cmdType, string cmdText, string connectionstring)
        {
            using (SqlConnection conn = new SqlConnection(connectionstring.ToString()))
            {
                int val = 0;
                PrepareCommand(cmd, conn, null, cmdType, cmdText);
                try
                {
                    val = cmd.ExecuteNonQuery();
                }
                catch (Exception es)
                {
                    Console.Write(es);
                }
                return val;
            }
        }
        public static DataSet ExecuteAdapter(SqlCommand cmd, CommandType cmdType, string cmdText, string connectionstring)
        {
            using (SqlConnection conn = new SqlConnection(connectionstring.ToString()))
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText);
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                DataSet ds = new DataSet();
                try
                {
                    da.Fill(ds);
                }
                catch (Exception e)
                {
                    //ErrorHandler.ErrorsEntry(e.Message, "Class:clsVouchers;Method:getAccounts", 1);
                    return null;
                }
                cmd.Parameters.Clear();
                return ds;
            }
        }
        public static DataTable ExecuteAdapterTable(SqlCommand cmd, CommandType cmdType, string cmdText, string connectionstring)
        {
            using (SqlConnection conn = new SqlConnection(connectionstring.ToString()))
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText);
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                DataTable dt = new DataTable();
                try
                {
                    da.Fill(dt);
                }
                catch (Exception e)
                {
                    //ErrorHandler.ErrorsEntry(e.Message, "Class:clsVouchers;Method:getAccounts", 1);
                    return null;
                }
                cmd.Parameters.Clear();
                return dt;
            }
        }

        public static int ExecuteNonQuery(SqlTransaction trans, SqlCommand cmd, CommandType cmdType, string cmdText)
        {
            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText);
            int val = cmd.ExecuteNonQuery();
            //cmd.Parameters.Clear();
            return val;
        }

        public static SqlDataReader ExecuteReader(SqlCommand cmd, CommandType cmdType, string cmdText, string connectionstring)
        {
            SqlConnection conn = new SqlConnection(connectionstring);

            // we use a try/catch here because if the method throws an exception we want to 
            // close the connection throw code, because no datareader will exist, hence the 
            // commandBehaviour.CloseConnection will not work
            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText);
                SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                //cmd.Parameters.Clear();
                return rdr;
            }

            catch
            {
                conn.Close();
                throw;
            }
        }

        public static object ExecuteScalar(SqlCommand cmd, CommandType cmdType, string cmdText, string connectionstring)
        {
            using (SqlConnection conn = new SqlConnection(connectionstring.ToString()))
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText);
                object val = cmd.ExecuteScalar();
                //cmd.Parameters.Clear();
                return val;
            }
        }
        private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
                cmd.Transaction = trans;
            cmd.CommandType = cmdType;
        }

        #region "Method for ExecuteXmlReader"
        public static SqlDataReader ExecuteReader(SqlCommand cmd, CommandType cmdType, string cmdText, SqlConnection con)
        {
            SqlDataReader rdr;
            try
            {
                PrepareCommand(cmd, con, null, cmdType, cmdText);
                rdr = cmd.ExecuteReader();
                return rdr;
            }
            catch
            {
                con.Close();
                throw;
            }
        }
        #endregion
    }
}
