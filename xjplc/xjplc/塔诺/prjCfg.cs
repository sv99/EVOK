using CCWin.SkinControl;
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

namespace prjInfoSetting
{

    //视频参数设置类
    //启动 用户之前参数恢复
    //输入数据后改变之前参数
    //保存参数 更新当前参数

    public partial class prjCfg : Form
    {
        public delegate void setPrjdata(string[] str);

      

        #region 字符常量
        public static string[] infoStr = {
        "gcmc", "jcry", "jcsb", "jcdw", "jcfx", "jcdz",
         "qdjh", "zdjh", "jkgc", "jkjd", "jkwd",
         "gdlx", "gdcz", "gdzj", "gdcd", "zgcd",
         "qsdw", "sjpd","mssd", "jsrq",
         "pgbz", "dqzyx", "tzyxd", "fhzk","bz",
        };
       
        public static string prjInfo = @"PrjInfo/";
        public static string demoStr = @"PrjInfo/demoStr";
        public static string extextion = ".mp4";
        public static  char splitChar = '/';

        public static string prjDirLst = "prjDir";
        public static string guanDuanTypeLst = "guanDuanType";
        public static string guanDuanMaterialLst = "guanDuanMaterial";
        public static string evalIdLst = "evalId";
        public static string areaImportanceLst = "areaImportance";
        public static string soilQuantityLst = "soilQuantity";
        public static string payLoadLst = "payLoad";
        public static string ListStr = "@ListStr";
        #endregion

        public string fileName = "";
             
        ConfigFileManager cfg;
        
        public setPrjdata callBackDataUpLoad;
              

        public prjCfg()
        {
            InitializeComponent();
            Init();
        }
        void SetCombBx(ComboBox cb,string keyName)
        {
            string s1 = cfg.ReadConfig(prjInfo + keyName, ListStr);
            cb.Items.AddRange(s1.Split(splitChar));
           //cb.Text = cb.Items[0].ToString();

        }
        //此处可以使用委托
        void SetTxt(Dictionary<string,string>  s2)
        {
            Queue<Control> allCtrls = new Queue<Control>();
            ConstantMethod.CheckAllCtrls(this, allCtrls);
          
            foreach (Control c in allCtrls)
            {
                foreach (string s in s2.Keys)
                {
                    if (c.Tag != null && c.Tag.ToString().Equals(s))
                    {
                        if (c.GetType().Equals(typeof(SkinTextBox)))
                        {
                            c.Text = s2[s];
                        }
                        if (c.GetType().Equals(typeof(SkinComboBox)))
                        {
                            c.Text = s2[s];  //这里奇怪了 
                        }
                        if (c.GetType().Equals(typeof(RichTextBox)))
                        {
                            c.Text = s2[s];  //这里奇怪了 
                        }
                        if (c.GetType().Equals(typeof(SkinDateTimePicker)))
                        {
                            if(ConstantMethod.IsDateTimeStr(s2[s]))
                            ((DateTimePicker)c).Text = s2[s];
                        }
                    }                    
                }               
            }
        }
        public void ShowData(string[] s2)
        {
            Dictionary<string, string> dicStr = new Dictionary<string, string>();

            if (s2.Length == infoStr.Length)
            {
                for (int i = 0; i < s2.Length; i++)
                {
                    dicStr.Add(infoStr[i], s2[i]);
                }
                SetTxt(dicStr);
            }
        }
        void Init()
        {
            cfg = new ConfigFileManager(Constant.cfgPrjFile);

            SetCombBx(dirCb, prjDirLst);
            SetCombBx(gdTypeCb, guanDuanTypeLst);
            SetCombBx(gdMaterialCb, guanDuanMaterialLst);
            SetCombBx(pgbzCb, evalIdLst);
            SetCombBx(areaImportanceCb, areaImportanceLst);
            SetCombBx(soilQuantityCb, soilQuantityLst);
            SetCombBx(payLoadCb, payLoadLst);
                 
           
        }
                
        private void skinButton2_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void skinButton1_Click(object sender, EventArgs e)
        {
            if (callBackDataUpLoad != null)
            {
                List<string> pstr = new List<string>();
                //工程信息 6
                pstr.Add(prjTxt.Text);
                pstr.Add(skinTextBox1.Text);
                pstr.Add(skinTextBox2.Text);
                pstr.Add(skinTextBox3.Text);
                pstr.Add(dirCb.Text);
                pstr.Add(skinTextBox5.Text);

                //检测井号5
                pstr.Add(skinTextBox9.Text);
                pstr.Add(skinTextBox8.Text);
                pstr.Add(skinTextBox7.Text);
                pstr.Add(skinTextBox6.Text);
                pstr.Add(skinTextBox4.Text);

                //管道信息5
                pstr.Add(gdTypeCb.Text);
                pstr.Add(gdMaterialCb.Text);
                pstr.Add(skinTextBox12.Text);
                pstr.Add(skinTextBox10.Text);
                pstr.Add(skinTextBox11.Text);

                //建设信息4
                pstr.Add(skinTextBox15.Text);
                pstr.Add(skinTextBox13.Text);
                pstr.Add(skinTextBox16.Text);
                pstr.Add(dateTimePicker1.Text);

                //管道标准 4
                pstr.Add(pgbzCb.Text);
                pstr.Add(areaImportanceCb.Text);
                pstr.Add(soilQuantityCb.Text);
                pstr.Add(payLoadCb.Text);

                //备注信息 1
                pstr.Add(richTextBox1.Text);

                callBackDataUpLoad(pstr.ToArray());


                this.Hide();
            }


        }

        private void prjCfg_Load(object sender, EventArgs e)
        {
           
        }
    }
}
