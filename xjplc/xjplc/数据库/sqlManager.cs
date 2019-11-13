using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace xjplc.数据库
{
    public class sqlManager
    {

        SqlConnection conn = new SqlConnection();

        string dbName;
        string tableName;
        string connStr;
        string servierName;

        SqlCommand command = new SqlCommand();

        List<string> colNameLst;
        public bool Status
        {
            get
            {
                if (conn.State == ConnectionState.Open)
                {
                    return true;
                }
                return false;
            }
        }
        public sqlManager(string serverName,string dbname, string tname, int id)
        {
            colNameLst = new List<string>();
            //连接本地服务器数据库
            if (id == 0)
            {
                connStr = string.Format("server={0};database={1};integrated security = SSPI", serverName,dbname);
                dbName = dbname;
                tableName = tname;
                conn.ConnectionString = connStr;
            }
            if (open())
            {

                command.Connection = conn;            // 绑定SqlConnection对象
                command.CommandText = "select  *  from " + tname;   //sql语句
                SqlDataReader reader = command.ExecuteReader();

                //输出列名
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    colNameLst.Add(reader.GetName(i));
                }
                reader.Close();
            }
            else
            {
                MessageBox.Show("数据库打开错误！");
            }

        }

        public bool open()
        {
            if (conn == null) return false;
            if (conn.State == ConnectionState.Closed)
            {
                try
                {
                    conn.OpenAsync();
                    ConstantMethod.Delay(3000);
                   
                    return Status;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            return false;
        }

        public void close()
        {
            conn.Close();
            conn.Dispose();
        }

        private bool IsColExist(string s)
        {
            return colNameLst.Contains(s);

        }
        public DataTable getData(Dictionary<string, string> condition)
        {
            string conditionStr = getSelectStr(condition);

            if (Status && conditionStr != null)
            {
                command.CommandText = conditionStr;
                SqlDataReader reader = command.ExecuteReader();
                return ConvertToDataTable(reader);
            }

            return null;

        }
        public string getSelectStr(Dictionary<string, string> condition)
        {

            if (getConditionStr(condition) == null) return null;

            string setExeStr = "select  *  from " + tableName + getConditionStr(condition);

            return setExeStr;
        }

        //传入字符串更新数据集
        public string getUpdateStr(Dictionary<string, string> setcondition, Dictionary<string, string> condition)
        {
            string setExeStr = "update " + tableName + " set ";

            List<string> keys = setcondition.Keys.ToList();

            for (int i = 0; i < keys.Count; i++)
            {
                if (i != keys.Count - 1)
                    setExeStr += string.Format("{0}='{1}' and ", keys[i], setcondition[keys[i]]);
                else
                    setExeStr += string.Format("{0}='{1}' ", keys[i], setcondition[keys[i]]);

                if (!IsColExist(keys[i])) return null;

            }

            if (getConditionStr(condition) == null) return null;

            return setExeStr + getConditionStr(condition);
        }
        public string getConditionStr(Dictionary<string, string> condition)
        {
            string conditionStr = " where ";
            List<string> keys = condition.Keys.ToList();
            for (int i = 0; i < keys.Count; i++)
            {
                if (i != keys.Count - 1)
                    conditionStr += string.Format("{0}='{1}' and ", keys[i], condition[keys[i]]);
                else
                    conditionStr += string.Format("{0}='{1}' ", keys[i], condition[keys[i]]);

                if (!IsColExist(keys[i])) return null;
            }


            return conditionStr;

        }
        //string s1 = "update line_table set opTime='20190504'  where barCode='111905000216402102' and pheight='1620'";
        public bool setData(Dictionary<string, string> setExe, Dictionary<string, string> condition)
        {

            string setExeStr = getUpdateStr(setExe, condition);

            if (Status && setExeStr != null)
            {
                
                DataTable dt = getData(condition);
                if (getData(condition) != null && getData(condition).Rows.Count == 1)

                {
                    command.CommandText = setExeStr;
                    int count = command.ExecuteNonQuery();
                    return (count == 1);
                }

                else return false;
            }
            return false;


        }
        public DataTable ConvertToDataTable(SqlDataReader dataReader)
        {
            DataTable dt = new DataTable();
            DataTable schemaTable = dataReader.GetSchemaTable();

            try
            {
                //动态构建表，添加列
                foreach (DataRow dr in schemaTable.Rows)
                {
                    DataColumn dc = new DataColumn();
                    //设置列的数据类型
                    dc.DataType = dr[0].GetType();
                    //设置列的名称
                    dc.ColumnName = dr[0].ToString();
                    //将该列添加进构造的表中
                    dt.Columns.Add(dc);
                }
                //读取数据添加进表中
                while (dataReader.Read())
                {
                    DataRow row = dt.NewRow();
                    //填充一行数据
                    for (int i = 0; i < schemaTable.Rows.Count; i++)
                    {
                        row[i] = dataReader[i].ToString();

                    }
                    dt.Rows.Add(row);
                    row = null;
                }
                dataReader.Close();
                schemaTable = null;
                return dt;
            }
            catch (Exception ex)
            {

                return null;
            }

        }

    }
}
