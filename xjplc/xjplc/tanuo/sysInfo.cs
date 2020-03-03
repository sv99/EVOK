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

namespace prjInfoSetting
{

    
    public partial class sysInfo : Form
    {
        public delegate void setSysdata(Dictionary<string,string> s);
        public setSysdata setParam;
        Queue<Control> allCtrls = new Queue<Control>();


        #region 参数名称常量        

        public static string[] param = 
            { "tkxs","splj", "tplj", "tplx",
            "qsip","qsport", "qsuser", "qspwd",
            "hsip", "hsport", "hsuser", "hspwd",
            "jxpip", "jxpport",
            "zdip", "zdport","carLen","stLen",
             "carIndex","zwIndex"
             };
        public static string[] defaultParam = { };
                             
        #endregion
        public sysInfo()
        {
            InitializeComponent();
            Init();
        }
        void Init()
        {
            ConstantMethod.CheckAllCtrls(this, allCtrls);
            
        }
        public void SaveLoadParam()
        {
            Dictionary<string, string> strLst = new Dictionary<string, string>();

            foreach (string key in param)
            {
                foreach (Control c in allCtrls)
                {
                    if (c.Tag!=null && key.Equals(c.Tag.ToString()))
                    {
                        strLst.Add(key, c.Text);
                    }
                }
            
            }
            if (strLst.Count == param.Count() && setParam !=null)
            {
                setParam(strLst);
            }
        }
        public void ShowSysParam(Dictionary<string,string> strLst)
        {                     
            foreach (Control c in allCtrls)
            {
                foreach (string key in strLst.Keys)
                {
                    if (c.Tag != null && key.Equals(c.Tag.ToString()))
                    {
                        c.Text = strLst[key];
                    }
                }
            }
        }

        private void skinButton1_Click(object sender, EventArgs e)
        {
            if (setParam != null)
            {
                SaveLoadParam();
                this.Hide();
            }
        }

        private void skinButton2_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void skinGroupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void sysInfo_Load(object sender, EventArgs e)
        {
          
        }
        public DeviceManageer devM;
        private void skinButton3_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dd = new FolderBrowserDialog();

            DialogResult dr = dd.ShowDialog();
            if (dr == DialogResult.OK)
            {
                textBox1.Text = dd.SelectedPath;
            }

        }

        private void skinButton4_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dd = new FolderBrowserDialog();

            DialogResult dr = dd.ShowDialog();
            if (dr == DialogResult.OK)
            {
                textBox2.Text = dd.SelectedPath;
            }
        }

        private void skinButton5_Click(object sender, EventArgs e)
        {

        }

        private void skinButton6_Click(object sender, EventArgs e)
        {
            if (devM != null)
                devM.CarReset();
        }

        private void skinButton7_Click(object sender, EventArgs e)
        {
            if (devM != null)
                devM.CarUnlock();
        }
    }
}
