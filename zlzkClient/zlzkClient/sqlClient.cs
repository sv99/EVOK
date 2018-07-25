using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using xjplc;
namespace zlzkClient
{
    class sqlClient
    {
        DataTable sqlShowDataTable;
        public System.Data.DataTable SqlShowDataTable
        {
            get { return sqlShowDataTable; }
            set { sqlShowDataTable = value; }
        }
        public sqlClient()
        {
            SqlShowDataTable = new DataTable();
        }
        public void GetDataFromSql(DataGridView dg1)
        {
            string sqlStr = "SELECT * FROM deviceinfo";

            SqlShowDataTable = SqlHelper.ExecuteDataTable(sqlStr);

            dg1.DataSource = SqlShowDataTable;


        }
    }
}
