using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            FilePath = file;
            _xmldoc.Load(file);
        }
        public void LoadFile(string file)
        {          
            FilePath = file;
            _xmldoc.Load(file);
        }

        public void Dispose()
        {
            _xmldoc.Save(FilePath);
            _xmldoc = null;
        }

        public String ReadConfig(string key)
            {
                XmlAttribute atrr;
                string str = "0";
                XmlNodeList nodes = _xmldoc.GetElementsByTagName("config");
                for (int i = 0; i < nodes.Count; i++)
                {

                    atrr = nodes[i].Attributes[key];
                    if (atrr != null)
                        str = atrr.Value.ToString();
                }
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
