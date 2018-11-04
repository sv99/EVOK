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
    public partial class prodStyleCreateForm : Form
    {
        public prodStyleCreateForm()
        {
            InitializeComponent();
        }

        private void stbtn_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        public void Init()
        {
            if(Constant.proCutType.Count() == Constant.proCutTypeImage.Count())
            {
                comboBox1.Items.Clear();
                imageList1.Images.Clear();

                foreach (string imageName in Constant.proCutTypeImage)
                {
                    imageList1.Images.Add(Image.FromFile(Constant.ConfigSource + imageName + ".png"));                    
                }
                foreach (string proCutTypeName in Constant.proCutType)
                {
                    comboBox1.Items.Add(proCutTypeName);
                }
            }

        }
        private void prodStyleCreateForm_Load(object sender, EventArgs e)
        {
            Init();
        }
    }
}
