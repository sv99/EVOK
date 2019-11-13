using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace evokNew0078
{
    public partial class WatchForm : Form
    {
        public WatchForm()
        {
            InitializeComponent();
        }

        public void SetShowDataTable(DataTable dt)
        {
            dgv.DataSource = dt;
        }
        private void watchForm_Load(object sender, EventArgs e)
        {

        }

        private void dgv_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
