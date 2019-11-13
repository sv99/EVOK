using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using xjplc;

namespace evokNew0081
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (ConstantMethod.IsRuning())
            {
                return;
            }
           // ConstantMethod.InitPassWd();
            //ConstantMethod.AutoStart(true);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new WorkForm());
        }
    }
}
