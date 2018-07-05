
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;

namespace Evok
{
    class Param
    {
        XmlDocument _xmldoc = null;
        string FilePath;
        //
        public Param()
        {
            _xmldoc = new XmlDocument();
        }
        //
        public Param(string file)
        {
            _xmldoc = new XmlDocument();
            FilePath = file;         
            _xmldoc.Load(file);
        }

        public String ReadConfig(string key)
        {
            XmlAttribute atrr;
            string str = "0";
            XmlNodeList nodes = _xmldoc.GetElementsByTagName("config");
            for (int i = 0; i < nodes.Count; i++)
            {
                
                atrr = nodes[i].Attributes[key];
                if (atrr!=null)
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
