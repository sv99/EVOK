using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace xjplc
{
    public partial class passWdForm : Form
    {
        public string Pwd;

        public int PwdCount;

        public string userInput;

        public bool IsStart;
        public passWdForm()
        {
            InitializeComponent();
            userInput = "0";
            PwdCount = 0;
            Pwd = "0";
            IsStart = false;
        }
    
        private void button1_Click(object sender, EventArgs e)
        {
            userInput = pwdTxt.Text;

            if (!IsStart)
            {
                this.Hide();
                return;
            }

            ConfigFileManager passWdFile = new ConfigFileManager();

            passWdFile.LoadFile(Constant.ConfigPassWdFilePath);

            string pwdTime = "0";

            pwdTime = DateTime.Now.ToString("yyyyMMdd");

          
            if (userInput.Equals(Pwd))
            {

                //修改下次时间
                DateTime nowAdd = DateTime.Now;

                nowAdd = nowAdd.AddMonths(PwdCount);

                pwdTime = nowAdd.ToString("yyyyMMdd");

                passWdFile.WriteConfig(Constant.passwdTime, pwdTime);

                //修改密码输入的次数
                int pwdInt = 0;

                if (int.TryParse(pwdTime, out pwdInt))
                {
                    PwdCount++;
                    passWdFile.WriteConfig(Constant.passwdCount, PwdCount.ToString());
                }
                this.Close();
            }
            else
            {
                MessageBox.Show(Constant.pwdWrong);                          
                passWdFile.WriteConfig(Constant.passwdTime, pwdTime);
                Environment.Exit(0);                           
                this.Close();
               
            }           

            passWdFile.Dispose();
            pwdTxt.Text = "";

        }
                              
        private void passWdForm_Load(object sender, EventArgs e)
        {

            
        }
    }
}
