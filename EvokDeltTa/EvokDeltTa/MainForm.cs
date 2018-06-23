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
namespace EvokDeltTa
{
    public partial class MainForm : Form
    {
        DTDevice EvokDtDevice;
        private static Queue<Control> allCtrls = new Queue<Control>();

        private EvokWork evokWork;
        private OptSize optSize;

        private List<string> strDataFormPath;

        private WatchForm wForm;

        //测试      


        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (ConstantMethod.DTFindPort())
            {
                ConstantMethod.ShowInfo(richTextBox1,Constant.ConnectMachineSuccess);
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}
