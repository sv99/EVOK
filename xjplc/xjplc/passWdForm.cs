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
    public partial class passWdForm : Form
    {

        int lang = Constant.idChinese;

        public string Pwd;

        public int PwdCount;

        public string userInput;

        public bool IsStart;

        public bool IsUserClose;
        public passWdForm()
        {
            InitializeComponent();
            userInput = "";
            PwdCount = 0;
            Pwd = "0";
            IsStart = false;
            this.Text = Constant.pwdInput;
            button1.Text = Constant.confirm;

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
            if (File.Exists(Constant.ConfigPassWdFilePath))
                passWdFile.LoadFile(Constant.ConfigPassWdFilePath);
            else return;

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

        private void pwdTxt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                button1_Click(sender,e);
            }
        }

        private void passWdForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            IsUserClose = true;
        }
    }
}
