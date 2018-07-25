using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace xjplc.ybtd
{
    public partial class SetDeviceInfo : Form
    {

        YBDTWorkInfo ytdworkInfo;
        public xjplc.YBDTWorkInfo YtdworkInfo
        {
            get { return ytdworkInfo; }
            set { ytdworkInfo = value; }
        }
        public SetDeviceInfo()
        {
            InitializeComponent();
            YtdworkInfo = new YBDTWorkInfo();
        }

        public void ShowDeviceInfo()
        {
            textBox1.Text = YtdworkInfo.DanHao;
            textBox2.Text = YtdworkInfo.DateTimeDanhao;
            textBox3.Text = YtdworkInfo.Department;
            textBox6.Text = YtdworkInfo.TuHao;
            textBox5.Text = YtdworkInfo.ProdName;
            textBox4.Text = YtdworkInfo.GongXu;
            textBox16.Text = YtdworkInfo.SetProdQuantity;
            textBox17.Text = YtdworkInfo.Speed;
            textBox18.Text = YtdworkInfo.Jshu;


            textBox7.Text = YtdworkInfo.GyTx;
            textBox8.Text = YtdworkInfo.OperatorName;
            textBox9.Text = YtdworkInfo.OperatorTx;
            textBox10.Text = YtdworkInfo.DeviceClass;
            textBox11.Text = YtdworkInfo.DeviceId;
           

            textBox12.Text = YtdworkInfo.DeviceIP;
            textBox13.Text = YtdworkInfo.DeviceTx;
            textBox14.Text = YtdworkInfo.CadPath;
            textBox15.Text = YtdworkInfo.Ddsm;
            textBox19.Text = YtdworkInfo.Gmj;

        }

    }
}
