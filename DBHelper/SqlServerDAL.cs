using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Data.Common;
using System.Windows.Forms;

namespace DBHelper
{
   public class SqlServerDAL:IDataAccess
    {
        private string _connStr;
        public  SqlServerDAL(string connStr)
        {
            _connStr = connStr;
        }

        public int ExecuteNonQuery( System.Data.CommandType cmdType, string cmdText)
        {
            return ExecuteNonQuery(cmdType, cmdText);
        }
        public int ExecuteNonQuery(string cmdText)
        {
            return ExecuteNonQuery(CommandType.Text, cmdText);
        }
        public int ExecuteNonQuery(System.Data.CommandType cmdType, string cmdText, params QueryParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            using (SqlConnection conn = new SqlConnection(_connStr))
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
        }

        public System.Data.DataTable GetTable(string cmdText, params QueryParameter[] para)
        {
            using (SqlConnection conn = new SqlConnection(_connStr))
            {
                SqlCommand cmd = new SqlCommand();
                PrepareCommand(cmd,conn, null,CommandType.Text, cmdText, para);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                cmd.Parameters.Clear();
                return dt;
                
            }

        }

        public static DataTable gettable(string sql)
        {
            string connstr = "Password=123;Persist Security Info=True;User ID=sa;Initial Catalog=DQLREPORTDB  ;Data Source=.;integrated security=true";
            using (SqlConnection conn = new SqlConnection(connstr))
            {

                DbDataAdapter Adpt = new SqlDataAdapter(sql, conn);
                DataTable dt = new DataTable();

                Adpt.Fill(dt);
                
                return dt;
            }
        }

        public System.Data.DataTable GetTable(string cmdText)
        {
            using (SqlConnection conn = new SqlConnection(_connStr))
            {
              
              
                

                DbDataAdapter Adpt = new SqlDataAdapter(cmdText, conn);
                DataTable dt = new DataTable();

                Adpt.Fill(dt);
                /**
                try
                {
                    Adpt.Fill(dt);
                }
                catch(Exception e)
                {
                    MessageBox.Show("出现错误: " + e.ToString());
                    return null;
                }**/
                return dt;
            }
        }
        public static  int ExecuteNonQueryy(string sqlstr)
        {
            string connectionStr = "Password=123;Persist Security Info=True;User ID=sa;Initial Catalog=DQLREPORTDB  ;Data Source=.;integrated security=true";
            using (SqlConnection sqlCon = new SqlConnection(connectionStr))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sqlstr, sqlCon))
                { 
                    sqlCon.Open();
                    return sqlCmd.ExecuteNonQuery();
                }
            }
        }
        public System.Data.DataSet GetDataSet(string cmdText, string TableName)
        {
            using (SqlConnection conn = new SqlConnection(_connStr))
            {
                //创建SqlDataAdapter对象以及DataSet
                DbDataAdapter Adpt = new SqlDataAdapter(cmdText, conn);
                DataSet ds = new DataSet();
                try
                {
                    //填充ds
                    Adpt.Fill(ds, TableName);
                    //返回ds
                    return ds;
                }
                catch
                {
                    //关闭连接，抛出异常
                    conn.Close();
                    throw;
                }
            }
        }
        public System.Data.DataSet GetDataSet(string cmdText)
        {
            using (SqlConnection conn = new SqlConnection(_connStr))
            {
                //创建SqlDataAdapter对象以及DataSet
                DbDataAdapter Adpt = new SqlDataAdapter(cmdText, conn);
                DataSet ds = new DataSet();
                try
                {
                    //填充ds
                    Adpt.Fill(ds);
                    //返回ds
                    return ds;
                }
                catch
                {
                    //关闭连接，抛出异常
                    conn.Close();
                    throw;
                }
            }
        }



        public static void SqlBulkCopyColumnMapping(string dataTableName, DataTable sourceDataTable)
        {
            string connectionStr = "Password=123;Persist Security Info=True;User ID=sa;Initial Catalog=DQLREPORTDB  ;Data Source=.;integrated security=true";
            int batchSize = 100000;
            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(connectionStr, SqlBulkCopyOptions.UseInternalTransaction))
                {
                    try
                    {
                        sqlBulkCopy.DestinationTableName = dataTableName;
                        sqlBulkCopy.BatchSize = batchSize;
                        //for (int i = 0; i < sourceDataTable.Columns.Count; i++)
                        //{
                        //    //sqlBulkCopy.ColumnMappings.Add(sourceDataTable.Columns[i].ColumnName, sourceDataTable.Columns[i].ColumnName);
                        //    //会报类型转换的错误。
                        //    sqlBulkCopy.WriteToServer(sourceDataTable);
                        //}
                        sqlBulkCopy.WriteToServer(sourceDataTable);
                    }
                    catch (Exception ex)
                    {

                        throw ex;
                    }
                }
            }
        }





        public DbDataReader ExecuteReader(string cmdText)
        {
            SqlConnection conn = new SqlConnection(_connStr);
                SqlCommand cmd = new SqlCommand(cmdText, conn);
                try
                {
                    if (conn.State != ConnectionState.Open)
                        conn.Open();

                    cmd.Connection = conn;
                    return cmd.ExecuteReader(CommandBehavior.CloseConnection);
                }
                catch (Exception ex)
                {
                    
                    throw ex;
                }
          
        }

            /// <summary>
        /// Prepare a command for execution
        /// </summary>
        /// <param name="cmd">SqlCommand object</param>
        /// <param name="conn">SqlConnection object</param>
        /// <param name="trans">SqlTransaction object</param>
        /// <param name="cmdType">Cmd type e.g. stored procedure or text</param>
        /// <param name="cmdText">Command text, e.g. Select * from Products</param>
        /// <param name="cmdParms">SqlParameters to use in the command</param>
        private void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, QueryParameter[] cmdParms) {

            if (conn.State != ConnectionState.Open)
                conn.Open();

            cmd.Connection = conn;
            cmd.CommandText = cmdText;

            if (trans != null)
                cmd.Transaction = trans;

            cmd.CommandType = cmdType;

            if (cmdParms != null) {
                foreach (QueryParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }
    }
}
