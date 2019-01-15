
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
using xjplc.ky;

namespace EVOK
{
    public partial class Form1 : Form
    {

        DeviceManager devMan;
        public Form1()
        {
            InitializeComponent();
        }

        void Init()
        {
            devMan = new DeviceManager();
            timer1.Enabled = true;

            if(devMan.getDeviceByName(Constant.devSTHY) !=null)
            devMan.getDeviceByName(Constant.devSTHY).SetShowDtData(sthyDgv);
            if (devMan.getDeviceByName(Constant.devSCLS) != null)
            devMan.getDeviceByName(Constant.devSCLS).SetShowDtData(sclsDgv);
            if (devMan.getDeviceByName(Constant.devSC) != null)
            devMan.getDeviceByName(Constant.devSC).SetShowDtData(scDgv);

            groupBox1.Visible= true;
            groupBox5.Visible = true;
            groupBox3.Visible = true;
            groupBox4.Visible = true;

        }
        private void button1_Click(object sender, EventArgs e)
        {
            Init();
        }

        public new void Dispose()
        {
            devMan.Dispose();
            base.Dispose();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            //if (!ylcTxt.Focused)
            // ylcTxt.Text = devMan.ylc();
            //if(devMan.DevLst[0].Status< Constant.constantStatusStr.Length)
            // label6.Text = Constant.constantStatusStr[devMan.DevLst[0].Status];
            // if (button10.Text.Equals("扫码开"))
            // {
            //   devMan.checkSbjGotoSetData(ylcTxt.Text, ylkTxt.Text, jgcTxt.Text, jgkTxt.Text);
            // }
            if (devMan.getDeviceByName(Constant.devSTHY) != null)
            {
                sthyLbl.Text = Constant.constantStatusStr[devMan.getDeviceByName(Constant.devSTHY).Status];

                /***
                Device dev = devMan.getDeviceByName(Constant.devSTHY);

                sthyLbl.Text = Constant.constantStatusStr[devMan.getDeviceByName(Constant.devSTHY).Status];

                sthyMCTxt.Text = dev.DtData.Rows[0][0].ToString();
                sthyMKTxt.Text = dev.DtData.Rows[0][1].ToString();
                sthyMHTxt.Text = dev.DtData.Rows[0][2].ToString();
                sthyJDFTxt.Text = dev.DtData.Rows[0][3].ToString();
                sthyPFTxt.Text = dev.DtData.Rows[0][4].ToString();
               ***/
                devMan.checkSTHYGotoSetData();

            }
            if (devMan.getDeviceByName(Constant.devSCLS) != null)
            {
                sclsLbl.Text = Constant.constantStatusStr[devMan.getDeviceByName(Constant.devSCLS).Status];
                devMan.checkSCLSGotoSetData();

            }
            if (devMan.getDeviceByName(Constant.devSC) != null)
            {
                scLbl.Text = Constant.constantStatusStr[devMan.getDeviceByName(Constant.devSC).Status];
                devMan.checkSCGotoSetData();
            }
                
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            int v = -1;// int.
            if (e.KeyChar == '\r')              
            {
                if (int.TryParse(ylcTxt.Text, out v))
                {
                    devMan.setylc((ushort)v);
                   
                }
                groupBox1.Focus();
            }         

        }

        private void button2_Click(object sender, EventArgs e)
        {
            devMan.setljtest(0);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            /***
            if (!button10.Text.Equals("扫码开"))
                devMan.setsbjDATA(ylcTxt.Text, ylkTxt.Text, jgcTxt.Text, jgkTxt.Text);
            else
            {
                MessageBox.Show("扫码模式，无法手动发送！");
            }
            ***/
        }

        private void button10_Click(object sender, EventArgs e)
        {
            /***
            if (button10.Text.Equals("扫码开"))
            {
                button10.Text = "扫码关";
            }
            else
            {
                button10.Text = "扫码开";
            }
            ***/
        }

        private void connectMachine_Click(object sender, EventArgs e)
        {
            Init();
        }

        private void sclsLbl_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (devMan.getDeviceByName(Constant.devSTHY) == null) return;

            devMan.getDeviceByName(Constant.devSTHY).DtData.Rows.Add("2000", "50", "500", "1", "1");
            devMan.getDeviceByName(Constant.devSTHY).DtData.Rows.Add("2020", "60", "500", "2", "2");
            devMan.getDeviceByName(Constant.devSTHY).DtData.Rows.Add("2030", "60", "500", "3", "3");
            devMan.getDeviceByName(Constant.devSTHY).DtData.Rows.Add("2040", "60", "500", "4", "4");

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (devMan.getDeviceByName(Constant.devSCLS) == null) return;

            devMan.getDeviceByName(Constant.devSCLS).DtData.Rows.Add("2000", "60", "500", "1", "1");
            devMan.getDeviceByName(Constant.devSCLS).DtData.Rows.Add("2020", "60", "500", "2", "2");
            devMan.getDeviceByName(Constant.devSCLS).DtData.Rows.Add("2030", "60", "500", "3", "3");
            devMan.getDeviceByName(Constant.devSCLS).DtData.Rows.Add("2040", "60", "500", "4", "4");
        }

        private void bu(object sender, EventArgs e)
        {
            if (devMan.getDeviceByName(Constant.devSC) == null) return;

            devMan.getDeviceByName(Constant.devSC).DtData.Rows.Add("2000", "60", "607", "1");
            devMan.getDeviceByName(Constant.devSC).DtData.Rows.Add("2020", "60", "506", "2");
            devMan.getDeviceByName(Constant.devSC).DtData.Rows.Add("2030", "60", "502", "3");
            devMan.getDeviceByName(Constant.devSC).DtData.Rows.Add("2040", "60", "503", "4");

        }
    }
}
