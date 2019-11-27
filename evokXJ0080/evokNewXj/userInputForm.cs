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

namespace evokNewXJ
{
    public partial class userInputForm : Form
    {

        EvokXJWork wk;
        public xjplc.EvokXJWork Wk
        {
            get { return wk; }
            set { wk = value; }
        }
        public userInputForm()
        {
            InitializeComponent();
        }

        private void label10_Click(object sender, EventArgs e)
        {
           

        }
        bool IsExit = false;

        private void button1_Click(object sender, EventArgs e)
        {
            
           
            /***
           
            foreach (DataTable dt in wk.getOptSize().DtLst)
            {
                if (dt.TableName.Equals("手动输入"))
                {
                    IsExit                                   = true;
                    wk.getOptSize().DtData                   = dt; 
                    wk.getOptSize().UserDataView .DataSource = dt;
                    
                }
            }
            ***/
            if (!IsExit)
            {

                DataTable dt = ConstantMethod.getDataTableByString(Constant.strformatZh);

                dt.TableName = comboBox1.Text;

                wk.getOptSize().DtData = dt;

                wk.getOptSize().UserDataView.DataSource = dt;

                IsExit = true;
            }

            DataRow drt = wk.getOptSize().DtData.NewRow();

            double usersize = 0;

            double leftAngle = 0;

            double rightAngle = 0;

            if (!double.TryParse(textBox1.Text, out usersize))
            {
                MessageBox.Show("用户尺寸错误");
                return;
            }
            if (!double.TryParse(textBox2.Text, out leftAngle))
            {
                MessageBox.Show("左角度错误");
                return;
            }
            if (!double.TryParse(textBox3.Text, out rightAngle))
            {
                MessageBox.Show("右角度错误");
                return;
            }

           

            drt[Constant.strformatZh[1]] = "1";
            drt[Constant.strformatZh[2]] = "0";
            drt[Constant.strformatZh[3]] = "补料";

            drt[Constant.strformatZh[4]] = textBox2.Text;

            drt[Constant.strformatZh[5]] = textBox3.Text;
            drt[Constant.strformatZh[7]] = textBox1.Text;

            string oppositeSize = "0";

            string maxSize = "0";
            wk.getOptSize().DtData.TableName = comboBox1.Text;

            drt[19] = wk.getOptSize().SimiM.Width.ToString();
            wk.getOptSize().SetSimiMaterial();

            double sized = wk.getOptSize().SimiM.calculateSize(
                usersize.ToString(),
                drt[Constant.strformatZh[4]].ToString(),
                drt[Constant.strformatZh[5]].ToString(),
                ref oppositeSize,
                ref maxSize);
            if (sized > 0)
            {
                drt[Constant.strformatZh[0]] = maxSize;//排版需要大尺寸
                drt[Constant.strformatZh[8]] = sized.ToString("0.00");
                drt[Constant.strformatZh[9]] = oppositeSize;
                drt[6] = comboBox1.Text;
                drt[Constant.strformatZh[19]] = wk.getOptSize().SimiM.Width.ToString();

                wk.getOptSize().DtData.Rows.Add(drt);
            }
             
        }

        private void userInputForm_Shown(object sender, EventArgs e)
        {
            if (wk != null)
            {
                comboBox1.DataSource = wk.getOptSize().MaterialLst;
                comboBox1.Text = wk.getOptSize().MaterialLst[0];
            }
        }
    }
    
}

