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
    public partial class OptParamSet : Form
    {
        public EvokXJWork evokWork;
        public OptParamSet()
        {
            InitializeComponent();
            Init();
        }

        void Init()
        {
            for (int i = 1; i < 10; i++)
                comboBox1.Items.Add(i.ToString());
        }
        private void skinButton18_Click(object sender, EventArgs e)
        {
            int splitCount = 0;

            if (int.TryParse(comboBox1.Text, out splitCount))
            {
                evokWork.SetSimiSplitCount(splitCount);
            }
            this.Hide();
                                                                   
        }

        private void OptParamSet_VisibleChanged(object sender, EventArgs e)
        {
            comboBox1.Text = evokWork.getOptSize().Simi_Splitcount.ToString();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            evokWork.SetUseRest(checkBox1.Checked);
            if (checkBox1.Checked)
            {
                MessageBox.Show("请堆放材料相同的余料，并扫码加入！");
            }
        }
    }
}
