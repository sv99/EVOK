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
    public partial class MSizeForm : Form
    {

        public EvokXJWork evokWork;

        public MSizeForm()
        {
            InitializeComponent();
        }

        private void skinButton18_Click(object sender, EventArgs e)
        {
            if (evokWork != null && evokWork.DeviceStatus)
            {
                int sizedata = 0;
                if (int.TryParse(textBox1.Text, out sizedata))
                {
                    evokWork.SetLen(sizedata);
                }
                else
                {
                    MessageBox.Show("料长错误！");
                }

                this.Hide();
            }
        }

        private void MSizeForm_VisibleChanged(object sender, EventArgs e)
        {
            textBox1.Text = evokWork.lcOutInPs.ShowValueDouble.ToString("0.00");
        }
    }
}
