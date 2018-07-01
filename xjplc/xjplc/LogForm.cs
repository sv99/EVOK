using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace xjplc
{
    public partial class LogForm : Form
    {
        public LogForm()
        {
            InitializeComponent();
        }
        public string fileName = "";

        public void LoadData()
        {
            if (File.Exists(fileName))
            {
                string str1 = File.ReadAllText(LogManager.LogFileName, Encoding.Default);
                rt1.Text = str1;
            }
        }
        private void Log_Load(object sender, EventArgs e)
        {
            Text = System.IO.Path.GetFileName(fileName);

        }
    }
}
