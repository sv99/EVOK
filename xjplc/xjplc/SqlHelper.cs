using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
namespace xjplc
{

    public class SqlHelper
        {
            /// <summary>
            /// 获取连接字符串
            /// </summary>
            /// <returns>连接字符串</returns>
       public static string GetSqlConnectionString()
            {
                    
               return ConfigurationManager.ConnectionStrings["Sql"].ConnectionString;
            }

            /// <summary>
            /// 封装一个执行的sql 返回受影响的行数
            /// </summary>
            /// <param name="sqlText">执行的sql脚本</param>
            /// <param name="parameters">参数集合</param>
            /// <returns>受影响的行数</returns>
        public static int ExecuteNonQuery(string sqlText, params SqlParameter[] parameters)
            {
                using (SqlConnection conn = new SqlConnection(GetSqlConnectionString()))
                {
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        conn.Open();
                        cmd.CommandText = sqlText;
                        cmd.Parameters.AddRange(parameters);
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
        public static int ExecuteQuery(string sqlText, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(GetSqlConnectionString()))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    conn.Open();
                    cmd.CommandText = sqlText;
                    cmd.Parameters.AddRange(parameters);
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// 执行sql，返回查询结果中的第一行第一列的值
        /// </summary>
        /// <param name="sqlText">执行的sql脚本</param>
        /// <param name="parameters">参数集合</param>
        /// <returns>查询结果中的第一行第一列的值</returns>
        public static object ExecuteScalar(string sqlText, params SqlParameter[] parameters)
            {
                using (SqlConnection conn = new SqlConnection(GetSqlConnectionString()))
                {
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        conn.Open();
                        cmd.CommandText = sqlText;
                        cmd.Parameters.AddRange(parameters);
                        return cmd.ExecuteScalar();
                    }
                }
            }

        /// <summary>
            /// 执行sql 返回一个DataTable
            /// </summary>
            /// <param name="sqlText">执行的sql脚本</param>
            /// <param name="parameters">参数集合</param>
            /// <returns>返回一个DataTable</returns>
        public static DataTable ExecuteDataTable(string sqlText, params SqlParameter[] parameters)
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter(sqlText, GetSqlConnectionString()))
                {
                    DataTable dt = new DataTable();
                    adapter.SelectCommand.Parameters.AddRange(parameters);
                    adapter.Fill(dt);
                    return dt;
                }
            }

        #region  执行带参数的增删改命令： ExecuteNonQuery(string cmmText, SqlParameter[] para, CommandType cmmType)
        /// <summary>
        /// 执行带参数的增删改命令
        /// </summary>
        /// <param name="cmmText"></param>
        /// <param name="para"></param>
        /// <param name="cmmType"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string sqlText, SqlParameter[] para, CommandType cmmType)
        {
            using (SqlConnection conn = new SqlConnection(GetSqlConnectionString()))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    conn.Open();
                    cmd.CommandText = sqlText;
                    cmd.Parameters.AddRange(para);
                    return cmd.ExecuteNonQuery();
                }
            }
        }
        #endregion

        #region  执行不带参数的增删改命令：ExecuteNonQuery(string cmmText, CommandType cmmType)
        /// <summary>
        /// 执行不带参数的增删改命令
        /// </summary>
        /// <param name="cmmText"></param>
        /// <param name="cmmType"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string cmmText, CommandType cmmType)
        {
            using (SqlConnection conn = new SqlConnection(GetSqlConnectionString()))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    conn.Open();
                    cmd.CommandText = cmmText;
                    cmd.CommandType = cmmType; ;
                    return cmd.ExecuteNonQuery();
                }
            }
        }
        #endregion

        #region  执行带参数的查询命令：ExecuteQuery(string cmmText, SqlParameter[] para, CommandType cmmType)
        /// <summary>
        /// 执行带参数的查询命令
        /// </summary>
        /// <param name="cmmText"></param>
        /// <param name="para"></param>
        /// <param name="cmmType"></param>
        /// <returns></returns>
        public static DataTable ExecuteQuery(string cmmText, SqlParameter[] para, CommandType cmmType)
        {
            using (SqlConnection conn = new SqlConnection(GetSqlConnectionString()))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    DataTable dt = new DataTable();
                    cmd.CommandText = cmmText;
                    cmd.CommandType = cmmType;
                    cmd.Parameters.AddRange(para);
                    using (SqlDataReader sdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        dt.Load(sdr);
                        return dt;
                    }
                }
            }
        }
        #endregion

        #region 执行不带参数的查询命令： ExecuteQuery(string cmmText, CommandType cmmType)
        /// <summary>
        /// 执行不带参数的查询命令
        /// </summary>
        /// <param name="cmmText"></param>
        /// <param name="cmmType"></param>
        /// <returns></returns>
        public static DataTable ExecuteQuery(string cmmText, CommandType cmmType)
        {
            using (SqlConnection conn = new SqlConnection(GetSqlConnectionString()))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    conn.Open();
                    DataTable dt = new DataTable();
                    cmd.CommandText = cmmText;
                    cmd.CommandType = cmmType;
                    using (SqlDataReader sdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        dt.Load(sdr);
                        return dt;
                    }
                }
            }
        }
        #endregion
        #region 判断数据库是否存在
        /// <summary>
        /// 判断数据库是否存在
        /// </summary>
        /// <param name="db">数据库的名称</param>
        /// <param name="connKey">数据库的连接Key</param>
        /// <returns>true:表示数据库已经存在；false，表示数据库不存在</returns>
        public static Boolean IsDBExist(string db)
        {
             // string connToMaster = ConfigurationManager.ConnectionStrings[connKey].ToString();
            string createDbStr = " select * from master.dbo.sysdatabases where name " + "= '" + db + "'";

            DataTable dt = ExecuteQuery(createDbStr, CommandType.Text);
            if (dt.Rows.Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion

        #region 判断数据库中，指定表是否存在
        /// <summary>
        /// 判断数据库表是否存在
        /// </summary>
        /// <param name="db">数据库</param>
        /// <param name="tb">数据表名</param>
        /// <param name="connKey">连接数据库的key</param>
        /// <returns>true:表示数据表已经存在；false，表示数据表不存在</returns>
        public static Boolean IsTableExist(string db, string tb)
        {
           
           // string connToMaster = ConfigurationManager.ConnectionStrings[connKey].ToString();
            string createDbStr = "use " + db + " select 1 from  sysobjects where  id = object_id('" + tb + "') and type = 'U'";

            //在指定的数据库中  查找 该表是否存在
            DataTable dt = ExecuteQuery(createDbStr, CommandType.Text);
            if (dt.Rows.Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }

        }
        #endregion

        #region 创建数据库
        /// <summary>
        /// 创建数据库
        /// </summary>
        /// <param name="db">数据库名称</param>
        /// <param name="connKey">连接数据库的key</param>
        public static void CreateDataBase(string db)
        {
          
            //符号变量，判断数据库是否存在
            Boolean flag = IsDBExist(db);

            //如果数据库存在，则抛出
            if (flag == true)
            {
                throw new Exception("数据库已经存在！");
            }
            else
            {
                //数据库不存在，创建数据库
               // string connToMaster = ConfigurationManager.ConnectionStrings[connKey].ToString();
                string createDbStr = "Create database " + db;
                ExecuteNonQuery(createDbStr, CommandType.Text);
            }

        }
        #endregion

        #region 创建数据库表
        /// <summary>
        ///  在指定的数据库中，创建数据表
        /// </summary>
        /// <param name="db">指定的数据库</param>
        /// <param name="dt">要创建的数据表</param>
        /// <param name="dic">数据表中的字段及其数据类型</param>
        /// <param name="connKey">数据库的连接Key</param>
        public static void CreateDataTable(string db, string dt, Dictionary<string, string> dic)
        {           
            //string connToMaster = ConfigurationManager.ConnectionStrings[connKey].ToString();

            //判断数据库是否存在
            if (IsDBExist(db) == false)
            {
                throw new Exception("数据库不存在！");
            }

            //如果数据库表存在，则抛出错误
            if (IsTableExist(db, dt) == true)
            {
                throw new Exception("数据库表已经存在！");
            }
            else//数据表不存在，创建数据表
            {
                //拼接字符串，（该串为创建内容）
                string content = " ";
                //取出dic中的内容，进行拼接
                List<string> test = new List<string>(dic.Keys);
                for (int i = 0; i < dic.Count; i++)
                {
                    if(i==0)
                    content = content + test[i] + " " + dic[test[i]]+ " primary key ";
                    else
                    content = content + " , " + test[i] + " " + dic[test[i]];
                }

                //其后判断数据表是否存在，然后创建数据表
                string createTableStr = "use " + db + " create table " + dt + " (" + content + ")";

                ExecuteNonQuery(createTableStr, CommandType.Text);


            }
        }
        #endregion
        /// <summary>
        /// 判断数据库中名为tableName的表是否存在
        /// </summary>
        /// <param name="tableName">要查询的表名</param>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <returns></returns>
        public static bool IsSqlTableExist(string tableName)
        {
            bool bExist = false;
            SqlConnection _Connection = new SqlConnection(GetSqlConnectionString());
            try
            {
                _Connection.Open();
                using (DataTable dt = _Connection.GetSchema("Tables"))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (string.Equals(tableName, dr[2].ToString()))
                        {
                            bExist = true;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _Connection.Dispose();
            }

            return bExist;
        }
            
        /// <summary>
        /// 执行sql脚本
        /// </summary>
        /// <param name="sqlText">执行的sql脚本</param>
        /// <param name="parameters">参数集合</param>
        /// <returns>返回一个SqlDataReader</returns>
        public static SqlDataReader ExecuteReader(string sqlText, params SqlParameter[] parameters)
                {
                    //SqlDataReader要求，它读取数据的时候有，它独占它的SqlConnection对象，而且SqlConnection必须是Open状态
                    SqlConnection conn = new SqlConnection(GetSqlConnectionString());//不要释放连接，因为后面还需要连接打开状态
                    SqlCommand cmd = conn.CreateCommand();
                    conn.Open();
                    cmd.CommandText = sqlText;
                    cmd.Parameters.AddRange(parameters);
                    //CommandBehavior.CloseConnection当SqlDataReader释放的时候，顺便把SqlConnection对象也释放掉
                    return cmd.ExecuteReader(CommandBehavior.CloseConnection);
                }
        public static void UpdateFromDtByName(string tableName,DataTable dt)
            {
                SqlConnection conn = new SqlConnection(GetSqlConnectionString());
                string sql = "SELECT * FROM " + tableName.Trim();
                SqlDataAdapter adapter = new SqlDataAdapter(sql, conn);
                SqlCommandBuilder thisBuilder = new SqlCommandBuilder(adapter);
            
                DataTable table = new DataTable();
                adapter.Fill(table);
                table = dt.Copy();
                try 
                {
                    adapter.Update(table);
                }
                catch (Exception ex)
                {

                }
            }
            /// <summary>
            /// 适合插入表结构相同或者包含的情况
            /// </summary>
            /// <param name="dt"></param>
        public static void UpdateFromDeviceInfo(DataTable dt)
            {
                SqlConnection conn = new SqlConnection(GetSqlConnectionString());
                SqlDataAdapter adapter = new SqlDataAdapter(Constant.sqlGetDataTable, conn);
                SqlCommandBuilder thisBuilder = new SqlCommandBuilder(adapter);
                DataTable table = new DataTable();
                adapter.Fill(table);

                //已经有的给我删了
                for (int i = table.Rows.Count - 1; i >= 0; i--)
                {            
                       try
                       {
                        string s = table.Rows[i][Constant.sqlDeviceIp].ToString().Trim();
                        DataRow[] dr =
                        dt.Select(Constant.sqlDeviceIp + "=" + "'" + s + "'");
                                       
                        if (dr.Length > 0)
                        {
                            for (int colcount = 0; colcount < table.Rows[i].ItemArray.Length; colcount++)
                            {
                                if(colcount< dr[0].ItemArray.Length)
                                 table.Rows[i][colcount] = dr[0][colcount];
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
                //没有的给我加上
                for (int i = dt.Rows.Count - 1; i >= 0; i--)
                {
                    try
                    {
                        string s = dt.Rows[i][Constant.sqlDeviceIp].ToString().Trim();
                        DataRow[] dr =
                        table.Select(Constant.sqlDeviceIp + "=" + "'" + s + "'");

                        if (dr.Length > 0)
                        {
                        
                       
                        }
                        else
                        {
                            DataRow drNew = table.NewRow();
                            drNew.ItemArray = (object[])dt.Rows[i].ItemArray.Clone();
                            table.Rows.Add(drNew);
                                               
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }

                try
                {
                    adapter.Update(table);
                }
                catch (Exception ex)
                {
                
                }


            }
        public static void DataTableToSQLServer(DataTable dt)
            {

                using (SqlConnection destinationConnection = new SqlConnection(GetSqlConnectionString()))
                {
                    destinationConnection.Open();

                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(destinationConnection))
                    {
                        try
                        {
                            bulkCopy.DestinationTableName = "deviceinfo";//要插入的表的表名
                            bulkCopy.BatchSize = dt.Rows.Count;
                            bulkCopy.ColumnMappings.Add("日计划单号", "日计划单号");
                       
                            bulkCopy.ColumnMappings.Add("日期", "日期");
                     
                            bulkCopy.ColumnMappings.Add("车间", "车间");
                            bulkCopy.ColumnMappings.Add("图号", "图号");//映射字段名 DataTable列名 ,数据库 对应的列名  
                            bulkCopy.ColumnMappings.Add("名称", "名称");
                            bulkCopy.ColumnMappings.Add("工序", "工序");
                            bulkCopy.ColumnMappings.Add("工艺特性", "工艺特性");
                            bulkCopy.ColumnMappings.Add("姓名", "姓名");
                            bulkCopy.ColumnMappings.Add("人员特性", "人员特性");
                            bulkCopy.ColumnMappings.Add("设备大类", "设备大类");
                            bulkCopy.ColumnMappings.Add("设备编号", "设备编号");
                            bulkCopy.ColumnMappings.Add("设备地址", "设备地址");
                            bulkCopy.ColumnMappings.Add("设备特性", "设备特性");
                            bulkCopy.ColumnMappings.Add("图纸链接", "图纸链接");
                            bulkCopy.ColumnMappings.Add("调度说明", "调度说明");
                            bulkCopy.ColumnMappings.Add("排产量", "排产量");
                            bulkCopy.ColumnMappings.Add("节拍", "节拍");
                            bulkCopy.ColumnMappings.Add("机数", "机数");
                            bulkCopy.ColumnMappings.Add("工模具", "工模具");                                        
                            bulkCopy.WriteToServer(dt);
                        
                        }
                        catch (Exception ex)
                        {
                            System.Windows.Forms.MessageBox.Show(ex.Message);
                        }
                        finally
                        {

                        }
                    }


                }

            }


    }
    
}
