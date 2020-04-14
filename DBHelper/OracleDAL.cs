using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Configuration;
using System.Collections;
using System.Data.Sql;

namespace DBHelper
{
    class OracleDAL : IDataAccess
    {
        private string _connStr;
         public  OracleDAL(string connStr)
        {
            _connStr = connStr;
        }

         public int ExecuteNonQuery(System.Data.CommandType cmdType, string cmdText)
         {
             OleDbCommand cmd = new OleDbCommand();

             using (OleDbConnection conn = new OleDbConnection(_connStr))
             {
                 PrepareCommand(cmd, conn, null, cmdType, cmdText, null);
                 int val = cmd.ExecuteNonQuery();
                 //清除cmd的参数 
                 cmd.Parameters.Clear();
                 if (conn.State == ConnectionState.Open)
                 {
                     conn.Close();
                 }
                 return val;
             }
             
         }

         public int ExecuteNonQuery(string cmdText)
         {
             return ExecuteNonQuery(CommandType.Text, cmdText);
         }

         public int ExecuteNonQuery(System.Data.CommandType cmdType, string cmdText, params QueryParameter[] commandParameters)
         {
             OleDbCommand cmd = new OleDbCommand();

             using (OleDbConnection conn = new OleDbConnection(_connStr))
             {
                 PrepareCommand(cmd, conn, null, CommandType.Text, cmdText, commandParameters);
                 int val = cmd.ExecuteNonQuery();
                 //清除cmd的参数 
                 cmd.Parameters.Clear();
                 if (conn.State == ConnectionState.Open)
                 {
                     conn.Close();
                 }
                 return val;
             }
         }

         public System.Data.DataTable GetTable(string cmdText, params QueryParameter[] para)
         {
             using (OleDbConnection cn = new OleDbConnection(_connStr))
             {

                 cn.Open();

                 //创建一个OleDbCommand对象，并对其进行初始化 
                 OleDbCommand cmd = new OleDbCommand();
                 PrepareCommand(cmd, cn, (OleDbTransaction)null, CommandType.Text, cmdText, para);

                 //创建OleDbDataAdapter对象以及DataSet 
                 OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                 DataSet ds = new DataSet();

                 //填充ds 
                 da.Fill(ds);

                 // 清除cmd的参数集合 
                 cmd.Parameters.Clear();
                 if (cmd.Connection.State == ConnectionState.Open)
                 {
                     cmd.Connection.Close();
                 }
                 //前往ds 
                 return ds.Tables[0];
             }
         }

         public System.Data.DataTable GetTable(string cmdText)
         {
             return GetTable(cmdText, null);
         }

         public System.Data.DataSet GetDataSet(string cmdText, string TableName)
         {
             using (OleDbConnection cn = new OleDbConnection(_connStr))
             {
                 cn.Open();
                 //创建一个OleDbCommand对象，并对其进行初始化 

                 OleDbCommand cmd = new OleDbCommand();
                 PrepareCommand(cmd, cn, (OleDbTransaction)null, CommandType.Text, cmdText, null);

                 //创建OleDbDataAdapter对象以及DataSet 
                 OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                 DataSet ds = new DataSet();

                 //填充ds 
                 da.Fill(ds);

                 // 清除cmd的参数集合 
                 cmd.Parameters.Clear();
                 if (cmd.Connection.State == ConnectionState.Open)
                 {
                     cmd.Connection.Close();
                 }
                 //前往ds 
                 return ds;
             }

         }

         public System.Data.DataSet GetDataSet(string cmdText)
         {
            return GetDataSet(cmdText, "");
         }

         public System.Data.Common.DbDataReader ExecuteReader(string cmdText)
         {
             OleDbCommand cmd = new OleDbCommand();
             OleDbConnection conn = new OleDbConnection(_connStr);
             try
             {
                 PrepareCommand(cmd, conn, null, CommandType.Text, cmdText, null);
                 OleDbDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                 cmd.Parameters.Clear();
                 if (conn.State != ConnectionState.Open)
                 {
                     try
                     {
                         conn.Open();
                     }
                     catch (System.Exception ex)
                     {
                         throw ex;
                     }
                 }
                 return rdr;
             }
             catch
             {
                 conn.Close();
                 throw;
             }
         }


         public static void PrepareCommand(OleDbCommand cmd, OleDbConnection conn, OleDbTransaction trans, CommandType cmdType, string cmdText, params QueryParameter[] cmdParms)
         {
             //判断连接的状态。如果是关闭状态，则打开 
             if (conn.State != ConnectionState.Open)
                 conn.Open();
             //cmd属性赋值 
             cmd.Connection = conn;
             cmd.CommandText = cmdText;
             //能否需要用到事务处理 
             if (trans != null)
                 cmd.Transaction = trans;
             cmd.CommandType = cmdType;
             //添加cmd需要的存储过程参数 
             if (cmdParms != null)
             {
                 foreach (QueryParameter parm in cmdParms)
                     cmd.Parameters.Add(parm);
             }
         }
    }
}
