using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace xjplc
{
    public class ConfigFileManager
    {    
            XmlDocument _xmldoc = null;
            string FilePath;
            //
            public ConfigFileManager()
            {
                _xmldoc = new XmlDocument();
            }
        //
        
        public ConfigFileManager(string file)
        {
            _xmldoc = new XmlDocument();
            LoadFile(file);
        }
        public void LoadFile(string file)
        {          
            FilePath = file;
            try
            {
                _xmldoc.Load(file);
            }
            catch (Exception ex)
            {
                MessageBox.Show("加载参数文件错误！请重新配置或者还原文件");
                ConstantMethod.AppExit();
            }
        }

        public void Dispose()
        {
            _xmldoc.Save(FilePath);
            _xmldoc = null;
        }

        public String ReadConfig(string key)
            {
                XmlAttribute atrr;
                string str = "";
                XmlNodeList nodes = _xmldoc.GetElementsByTagName("config");
                for (int i = 0; i < nodes.Count; i++)
                {

                    atrr = nodes[i].Attributes[key];
                    if (atrr != null)
                        str = atrr.Value.ToString();
                }
                _xmldoc.Save(FilePath);
                return str;
            }

            public int WriteConfig(string key, string value)
            {

                XmlAttribute atrr;

                XmlNodeList nodes = _xmldoc.GetElementsByTagName("config");
                for (int i = 0; i < nodes.Count; i++)
                {
                    atrr = nodes[i].Attributes[key];
                    atrr.Value = value;
                }
                _xmldoc.Save(FilePath);
     
                return 0;
            }
        
    }
}
