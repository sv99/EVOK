using System;
using System.Collections.Generic;
using System.IO;
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
        public bool LoadFile(string file)
        {          
            FilePath = file;
            if (!File.Exists(file)) return false;
            try
            {
                _xmldoc.Load(file);
            }
            catch (Exception ex)
            {

                return false;
               //throw new Exception(ex.Message);
                //MessageBox.Show("加载参数文件错误！请重新配置或者还原文件");
               //ConstantMethod.AppExit();
            }

            return true;
        }
        

        public void Dispose()
        {
            _xmldoc.Save(FilePath);
            _xmldoc = null;
        }
        public bool WriteConfig(string s1, string s2, string value)
        {
            string str = "";
            XmlNode root = _xmldoc.DocumentElement;
            XmlNodeList nodes = root.SelectNodes(s1);

            if (nodes.Count > 0)
            {
                foreach (XmlNode node in nodes)
                {
                    try
                    {
                        XmlNode node1 = node.SelectSingleNode(s2);

                        if (node1 != null)
                        {
                            node1.InnerText = value;
                            _xmldoc.Save(FilePath);
                            return true;
                            // hashResult.Add(node1.InnerText.ToLower(), node2.InnerText);
                        }
                        else
                            return false;
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }

                }
            }
            return false;
        }
        public string ReadConfig(string s1,string s2)
        {
            string str = "";
            XmlNode root = _xmldoc.DocumentElement;
            XmlNodeList nodes = root.SelectNodes(s1);

            if (nodes.Count > 0)
            {
                foreach (XmlNode node in nodes)
                {
                    try
                    {
                        XmlNode node1 = node.SelectSingleNode(s2);
                
                        if (node1 != null)
                        {
                            str = node1.InnerText;
                            // hashResult.Add(node1.InnerText.ToLower(), node2.InnerText);
                        }
                    }
                    catch (Exception ex)
                    {

                    }

                }
            
            }
            return str;
        }
        public string ReadConfig(string root,string tagname,string key)
        {
            XmlAttribute atrr;
            string str = "";

            XmlNodeList nodes = _xmldoc.GetElementsByTagName(root);

            if (nodes.Count > 0)
            {
                XmlNode n =
                nodes[0].SelectSingleNode(tagname);
                return n.Attributes[key].Value.ToString();
            }

            return "";
        }
        public bool WriteConfig(string root, string tagname, string key,string value)
        {
            XmlAttribute atrr;
            string str = "";

            XmlNodeList nodes = _xmldoc.GetElementsByTagName(root);

            try
            {
                if (nodes.Count > 0)
                {
                    XmlNode n =
                    nodes[0].SelectSingleNode(tagname);
                    n.Attributes[key].Value = value;
                    _xmldoc.Save(FilePath);
                    return true;
                }
                else return false;
            }
            catch
            {
                XmlNode n =
                nodes[0].SelectSingleNode(tagname);
                XmlAttribute ra = _xmldoc.CreateAttribute(key);
                ra.Value = value;
                n.Attributes.Append(ra);
                _xmldoc.Save(FilePath);
                return false;
            }
            
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
                //_xmldoc.Save(FilePath);
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
