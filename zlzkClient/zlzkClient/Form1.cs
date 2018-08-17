using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using xjplc.ybtd;

namespace zlzkClient
{
    public partial class Form1 : Form
    {

        sqlClient sClient;
        public Form1()
        {
            InitializeComponent();
            InitSql();
            timer1.Enabled = true;
        }
        void InitSql()
        {
            sClient = new sqlClient();
        }
       
        private void UpdateRation()
        {
            foreach (DataGridViewRow dr in dataGridView1.Rows)
            {
                int fenzi;
                int fenmu;
                try
                {
                    this.Invoke((EventHandler)(delegate
                    {
                        if (int.TryParse(dr.Cells["当前产量"].Value.ToString(), out fenzi) && (int.TryParse(dr.Cells["排产量"].Value.ToString(), out fenmu)))
                        {

                            if (fenzi <= fenmu)
                            {
                                dr.Cells["完成率"].Value = ((double)fenzi / fenmu) * 100;
                            }
                        }
                    }));
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            sClient.GetDataFromSql(dataGridView1);
            UpdateRation();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
           

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
           
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.CurrentRow.Index < 0) return;

            SetDeviceInfo sform = new SetDeviceInfo();
            sform.YtdworkInfo.DanHao = dataGridView1.CurrentRow.Cells[1].Value.ToString();
           // sform.YtdworkInfo.DateTimeDanhao = dataGridView1.CurrentRow.Cells[2].Value.ToString();
            sform.YtdworkInfo.Department = dataGridView1.CurrentRow.Cells[3].Value.ToString();
            sform.YtdworkInfo.TuHao = dataGridView1.CurrentRow.Cells[4].Value.ToString();
            sform.YtdworkInfo.ProdName = dataGridView1.CurrentRow.Cells[5].Value.ToString();
            sform.YtdworkInfo.GongXu = dataGridView1.CurrentRow.Cells[6].Value.ToString();
            sform.YtdworkInfo.GyTx = dataGridView1.CurrentRow.Cells[7].Value.ToString();
            sform.YtdworkInfo.OperatorName = dataGridView1.CurrentRow.Cells[8].Value.ToString();
            sform.YtdworkInfo.OperatorTx = dataGridView1.CurrentRow.Cells[9].Value.ToString();
            sform.YtdworkInfo.DeviceClass = dataGridView1.CurrentRow.Cells[10].Value.ToString();
            sform.YtdworkInfo.DeviceId = dataGridView1.CurrentRow.Cells[11].Value.ToString();
            sform.YtdworkInfo.DeviceIP = dataGridView1.CurrentRow.Cells[12].Value.ToString();
            sform.YtdworkInfo.DeviceTx = dataGridView1.CurrentRow.Cells[13].Value.ToString();
            sform.YtdworkInfo.CadPath = dataGridView1.CurrentRow.Cells[14].Value.ToString();
            sform.YtdworkInfo.Ddsm = dataGridView1.CurrentRow.Cells[15].Value.ToString();
            sform.YtdworkInfo.SetProdQuantity = dataGridView1.CurrentRow.Cells[16].Value.ToString();
            sform.YtdworkInfo.Speed = dataGridView1.CurrentRow.Cells[17].Value.ToString();
            sform.YtdworkInfo.Jshu = dataGridView1.CurrentRow.Cells[18].Value.ToString();
            sform.YtdworkInfo.Gmj = dataGridView1.CurrentRow.Cells[19].Value.ToString();
            sform.ShowDeviceInfo();
            sform.Show();
        }
    }
}
