using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace xjplc.报警
{
    public partial class Alarm : Form
    {
        public Alarm()
        {
            InitializeComponent();
            Init();
        }
        void Init()
        {
          //  ConstantMethod.Delay(2000);
        }
        public void showAlram(List<string> lst)
        {
            //看下能否用数据库更新的方式进行更新 这样有点麻烦
            foreach (string s in lst)
            {
                if (!listBox1.Items.Contains(s)) listBox1.Items.Add(s);
            }
            for(int i= listBox1.Items.Count-1;i>-1;i--)
            
            {
                if (!lst.Contains(listBox1.Items[i])) listBox1.Items.RemoveAt(i);
            }
        }
    }
}
