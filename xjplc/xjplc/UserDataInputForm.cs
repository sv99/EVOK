using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace xjplc
{
    public partial class UserDataInputForm : Form
    {

        static string len = "2440";
        static string dbc = "4.5";
        static string ltbc = "10";
        static string safe = "10";
        public UserDataInputForm()
        {
            InitializeComponent();
            Init();
        }
        

        OptSize op;
        EvokXJWork eok;
        public xjplc.EvokXJWork Eok
        {
            get { return eok; }
            set { eok = value; }
        }
        void Init()
        {
            textBox1.Text = len.ToString();

            textBox2.Text = ltbc.ToString();
            textBox3.Text = dbc.ToString();
            textBox4.Text = safe.ToString();

        }
        public xjplc.OptSize Op
        {
            get { return op; }
            set { op = value; }
        }
        public void SetData()
        {
            double len0 = 0;
            double ltbc0 = 0;
            double dbc0 = 0;
            double safe0 = 0;

            if (!double.TryParse(textBox1.Text,out len0))
            {
                return;
            }

            if (!double.TryParse(textBox2.Text, out ltbc0))
            {
                return;
            }

            if (!double.TryParse(textBox3.Text, out dbc0))
            {
                return;
            }

            if (!double.TryParse(textBox4.Text, out safe0))
            {
                return;
            }
            op.Len =(int) (len0*100);
            op.Dbc = (int)(dbc0 * 100);
            op.Ltbc = (int)(ltbc0 * 100);
            op.Safe = (int)(safe0 * 100);

            len = len0.ToString();
            dbc = dbc0.ToString();
            ltbc = ltbc0.ToString();
            safe = safe0.ToString();

            if (checkBox1.Checked)
            {
                Eok.PrintBarCodeMode = Constant.HandBarCode;
            }
            else
            {
                Eok.PrintBarCodeMode = Constant .NoPrintBarCode;
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            SetData();
            this.Close();
        }
    }
}
