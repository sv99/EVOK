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

namespace HySkPrj
{
    public partial class Form1 : Form
    {
     
        public Form1()
        {
            InitializeComponent();
            Init();
        }

        XJPlcWork devA;

        XJPlcWork devB;
        void Init()
        {
            List<string> plcfileA = new List<string>();

            plcfileA.Add(Constant.PlcDataFilePathAuto);

           // devA = new  XJPlcWork(plcfileA.ToArray(),Constant.ConfigSerialportFilePath);

            List<string> plcfileB = new List<string>();

            plcfileB.Add(Constant.PlcDataFilePathAuto1);

            devB = new XJPlcWork(plcfileB.ToArray(), Constant.ConfigSerialportFilePath1);



        }
       

    }
}
