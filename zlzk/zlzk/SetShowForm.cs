using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using xjplc;

namespace zlzk
{
    public partial class SetShowForm : Form
    {
        public event userShowItemChanged showItemChanged;

        ExcelNpoi excelop;
        DataTable showItemDataTable;
        List<string> itemAll;
        List<string> selectItem;
       
        public SetShowForm()
        {
            InitializeComponent();

            excelop = new ExcelNpoi();
            showItemDataTable = excelop.ImportExcel(Constant.showItemPath);
            itemAll = new List<string>();
            selectItem = new List<string>();
            if (showItemDataTable != null && showItemDataTable.Rows.Count > 0)
            {
                foreach (DataRow dr in showItemDataTable.Rows)
                {
                    if(!string.IsNullOrWhiteSpace(dr[0].ToString()))
                    itemAll.Add(dr[0].ToString());
                    if (!string.IsNullOrWhiteSpace(dr[1].ToString()))
                        selectItem.Add(dr[1].ToString());
                };
            }

            UpdateListBox();
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ItemChangedArgs itemArgs = new ItemChangedArgs();
            itemArgs.ItemAll = itemAll;
            itemArgs.ItemSelect = selectItem;
            showItemChanged(sender, itemArgs);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string s = itemAll[listBox1.SelectedIndex];
            if (!selectItem.Contains(s))
            {
                selectItem.Add(s);
                
            }
            UpdateListBox();


        }
        private void UpdateListBox()
        {
            listBox2.DataSource = null;
            listBox2.DataSource = selectItem;
            listBox1.DataSource = null;
            listBox1.DataSource = itemAll;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            string s = selectItem[listBox2.SelectedIndex];
            if (selectItem.Contains(s))
            {
                selectItem.Remove(s);
            }
            UpdateListBox();
        }
    }
}
