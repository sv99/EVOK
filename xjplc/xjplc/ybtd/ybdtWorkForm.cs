using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace xjplc
{
    public partial class ybdtWorkForm : Form
    {
        YBDTWork ybdtWork;
        public xjplc.YBDTWork YbdtWork
        {
            get { return ybdtWork; }
            set { ybdtWork = value; }
        }
        public ybdtWorkForm()
        {
            InitializeComponent();
            
        }
        public void SetDataFormShow(DataTable dt)
        {
            dataGridView1.DataSource = dt;
        }
        public void SetYbtdWork(YBDTWork yw)
        {           
            YbdtWork = yw;
            dataGridView1.DataSource = YbdtWork.YbtdDevice.DataFormLst[0];
        }
     

        private void button1_Click(object sender, EventArgs e)
        {
            YbdtWork.TestSetMON();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            YbdtWork.TestSetMOFF();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            YbdtWork.ClrQuantity();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            
        }
    }
}
