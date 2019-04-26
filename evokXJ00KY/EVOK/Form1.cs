
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

            if (devMan.getDeviceByName(Constant.devSBJ) != null)
                devMan.getDeviceByName(Constant.devSBJ).SetShowDtData(sbjDgv);


            groupBox1.Visible = true;
            groupBox5.Visible = true;
            groupBox3.Visible = true;
            groupBox4.Visible = true;
            groupBox2.Visible = true;
            label6   .Visible = true;
            textBox1 .Visible = true;
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

            if (devMan.getDeviceByName(Constant.devSBJ) != null)
            {
                sbjLbl.Text = Constant.constantStatusStr[devMan.getDeviceByName(Constant.devSBJ).Status];
                devMan.checkSBJGotoSetData(); ;

            }



        }

       

       
       
        private void connectMachine_Click(object sender, EventArgs e)
        {
            Init();
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

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label54_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (devMan.getDeviceByName(Constant.devSBJ) == null) return;

            devMan.getDeviceByName(Constant.devSBJ).DtData.Rows.Add("2000", "800", "1980", "780", "45");
            devMan.getDeviceByName(Constant.devSBJ).DtData.Rows.Add("2020", "900", "2000", "880", "40");
            devMan.getDeviceByName(Constant.devSBJ).DtData.Rows.Add("2030", "920", "1990", "900", "45");
            devMan.getDeviceByName(Constant.devSBJ).DtData.Rows.Add("2050", "790", "1960", "760", "45");
            devMan.getDeviceByName(Constant.devSBJ).DtData.Rows.Add("2040", "800", "1990", "770", "45");
        }
    }
}
