using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace xjplc
{
    public static class MultiLanguage
    {
        public const string langStr = "Language";
        public static int getLangId()
        {
            int id = 0;
            ConfigFileManager langXml = new ConfigFileManager(Constant.ConfigParamFilePath);

            string idsstr = langXml.ReadConfig(langStr);

            if (!int.TryParse(idsstr,out id))
            {
                id = 0;
            }
            return id;
        }
        public static bool setLangId(int id,ConfigFileManager p)
        {

            ConfigFileManager langXml = p;// ConfigFileManager(Constant.ConfigParamFilePath);

            try
            {
                langXml.WriteConfig(langStr, id.ToString());                     
                
            }
            catch (Exception ex)
            {
                return false;
            }
                   
            return true;
        }
        public static void  saveId()
        {
            
        }
        //当前默认语言
        public static string DefaultLanguage = "ChineseSimplified";    
        /// <summary>
        /// 加载语言
        /// </summary>
        /// <param name="form">加载语言的窗口</param>
        public static void LoadLanguage(Form form, string FormName)
        {
            //根据用户选择的语言获得表的显示文字 
            Hashtable hashText = ReadXMLText(form.Name, FormName);
            Hashtable hashHeaderText = ReadXMLHeaderText(form.Name, FormName);
            if (hashText == null)
            {
                return;
            }
            //获取当前窗口的所有控件
            Control.ControlCollection sonControls = form.Controls;
            try
            {
                //遍历所有控件
                foreach (Control control in sonControls)
                {
                    if (control.GetType() == typeof(Panel))     //Panel
                    {
                        GetSetSubControls(control.Controls, hashText, hashHeaderText);
                    }
                    else if (control.GetType() == typeof(GroupBox))     //GroupBox
                    {
                        GetSetSubControls(control.Controls, hashText, hashHeaderText);
                    }
                    else if (control.GetType() == typeof(TabControl))       //TabControl
                    {
                        GetSetSubControls(control.Controls, hashText, hashHeaderText);
                    }
                    else if (control.GetType() == typeof(TabPage))      //TabPage
                    {
                        GetSetSubControls(control.Controls, hashText, hashHeaderText);
                    }
                    else if (control.GetType() == typeof(DataGridView))     //DataGridView
                    {
                        GetSetHeaderCell((DataGridView)control, hashHeaderText);
                    }
                    else if (control.GetType() == typeof(MenuStrip))     //DataGridView
                    {
                        foreach(ToolStripMenuItem item in ((MenuStrip)control).Items)
                        {
                            foreach (ToolStripMenuItem item0 in item.DropDownItems)
                            {
                                foreach (ToolStripMenuItem item1 in item0.DropDownItems)
                                {
                                    if (hashText.Contains(item1.Name))
                                    {
                                        item1.Text = (string)hashText[item1.Name];
                                    }
                                }

                                if (hashText.Contains(item0.Name))
                                {
                                    item0.Text = (string)hashText[item0.Name];
                                }
                            }

                            if (hashText.Contains(item.Name))
                            {
                                item.Text = (string)hashText[item.Name];
                            }


                        }
                    }
                    
                                                                                                                                                                              
                    if (hashText.Contains(control.Name))
                    {                      
                        control.Text = (string)hashText[control.Name];
                    }


                }
                //如果找到了控件，就将对应的名字赋值过去
                if (hashText.Contains(form.Name))
                {
                    form.Text = (string)hashText[form.Name];
                }
            }
            catch (Exception ex)
            {
                string s = ex.ToString();
            }
        }
        public static Hashtable hashString=null;
        public static string getStringFromXml(string frmName,string xmlName,string s)
        {
            if(hashString==null)
            hashString = MultiLanguage.ReadXMLText(frmName, xmlName);

            if (hashString == null || hashString.Count==0) return null;

            string s1 = (string)hashString[s];

            return s1;
           

        }
        private static void GetSetSubControls(Control.ControlCollection controls, Hashtable hashText, Hashtable hashHeaderText)
        {
            try
            {
                foreach (Control control in controls)
                {
                    if (control.GetType() == typeof(Panel))     //Panel
                    {
                        GetSetSubControls(control.Controls, hashText, hashHeaderText);
                    }
                    else if (control.GetType() == typeof(GroupBox))     //GroupBox
                    {
                        GetSetSubControls(control.Controls, hashText, hashHeaderText);
                    }
                    else if (control.GetType() == typeof(TabControl))       //TabControl
                    {
                        GetSetSubControls(control.Controls, hashText, hashHeaderText);
                    }
                    else if (control.GetType() == typeof(TabPage))      //TabPage
                    {
                        GetSetSubControls(control.Controls, hashText, hashHeaderText);
                    }
                    else if (control.GetType() == typeof(TableLayoutPanel))     //TableLayoutPanel
                    {
                        GetSetSubControls(control.Controls, hashText, hashHeaderText);
                    }
                    else if (control.GetType() == typeof(DataGridView))
                    {
                        GetSetHeaderCell((DataGridView)control, hashHeaderText);
                    }
                    

                    if (hashText.Contains(control.Name))
                    {
                        control.Text = (string)hashText[control.Name];
                    }



                }
            }
            catch(Exception ex)
            {

            }
         }
        private static Hashtable ReadXMLHeaderText(string frmName, string xmlName)
        {
            try
            {
                Hashtable hashResult = new Hashtable();
                XmlReader reader = null;
                //判断是否存在配置文件
                if (!File.Exists(xmlName))
                {
                    return null;
                }
                else
                {
                    reader = new XmlTextReader(xmlName);
                }
                XmlDocument doc = new XmlDocument();
                doc.Load(reader);
                XmlNode root = doc.DocumentElement;
               
                XmlNodeList nodeList = root.SelectNodes("Form[Name= '"+frmName+"']/Controls/DataGridViewCells/DataGridViewCell");
                foreach (XmlNode node in nodeList)
                {
                    try {
                        XmlNode node1 = node.SelectSingleNode("@name");
                        XmlNode node2 = node.SelectSingleNode("@HeaderText");
                        if (node1 != null)
                        {
                            hashResult.Add(node1.InnerText,node2.InnerText);
                        }
                    }
                    catch
                    {

                    }

                }

                reader.Close();
                return hashResult;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static Hashtable ReadXMLText(string formName, string xmlName)
        {
            try
            {
                Hashtable hashResult = new Hashtable();
                XmlReader reader = null;
                if (!File.Exists(xmlName)) return null;

                reader = new XmlTextReader(xmlName);

                XmlDocument doc = new XmlDocument();

                doc.Load(reader);

                XmlNode root = doc.DocumentElement;
                //获取XML文件中对应该窗口的内容
                XmlNodeList nodeList = root.SelectNodes("Form[Name = '"+formName+"']/Controls/Control");

                foreach (XmlNode node in nodeList )
                {
                    try
                    {
                        XmlNode node1 = node.SelectSingleNode("@name");
                        XmlNode node2 = node.SelectSingleNode("@text");
                        if(node!=null)
                        {
                            hashResult.Add(node1.InnerText, node2.InnerText);
                        }
                    }
                    catch (Exception  ex)
                    {

                    }

                }

                reader.Close();
                return hashResult;

            }
            catch { return null; }
        }
        /// <summary>
        /// 从XML文件中读取需要修改HeaderText的內容
        /// </summary>
        /// <param name="frmName">窗口名，用于获取对应窗口的那部分内容</param>
        /// <param name="xmlName">目标语言</param>
        /// <returns></returns>     
        /// <summary>
        /// 获取并设置DataGridView的列头
        /// </summary>
        /// <param name="dataGridView">DataGridView名</param>
        /// <param name="hashResult">哈希表</param>
        private static void GetSetHeaderCell(DataGridView dataGridView, Hashtable hashHeaderText)
        {
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                if (hashHeaderText.Contains(column.Name))
                {
                    column.HeaderText = (string)hashHeaderText[column.Name];
                }
            }
        }




    }
}
