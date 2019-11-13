using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace xjplc.tanuo
{

    [Serializable]
    public class cmdRobot
    {
        string cmdName;
        public string CmdName
        {
            get { return cmdName; }
            set { cmdName = value; }
        }

        string valuePos;
        public string ValuePos
        {
            get { return valuePos; }
            set { valuePos = value; }
        }
        string posInCmd;
        public string PosInCmd
        {
            get { return posInCmd; }
            set { posInCmd = value; }
        }

        string cmdValue;
        public string CmdValue
        {
            get { return cmdValue; }
            set { cmdValue = value; }
        }

        public virtual void GetObjectData(
            SerializationInfo info,
            StreamingContext context)
        {
            info.AddValue("cmdName", cmdName);
            info.AddValue("valuePos", valuePos);
            info.AddValue("posInCmd", posInCmd);
            info.AddValue("cmdValue", cmdValue);
        }
        protected cmdRobot(SerializationInfo info, StreamingContext context)
        {
            cmdName      = info.GetString("cmdName");
            valuePos = info.GetString("valuePos");
            posInCmd = info.GetString("posInCmd");
            cmdValue = info.GetString("cmdValue");
        }

        //字节命令 哪一个字节 字节哪一位
        public static bool getCmd(ref byte[] bLst,cmdRobot cr)
        {
            cr.cmdName = "123";
            byte b;
            byte cmdValue = Convert.ToByte(cr.CmdValue,16);
            if (int.Parse(cr.PosInCmd) < bLst.Count())
            {

                b = bLst[int.Parse(cr.PosInCmd)-1];

            } else return false;

            List<int> posLst = new List<int>();

            string[] strPosLst = cr.ValuePos.Split(',');

            foreach (string s in strPosLst)
            {
                int pos = 0;
                if (int.TryParse(s, out pos))
                {
                    int i = ConstantMethod.getBitValueInByte(pos, cmdValue);
                    bool flag = i > 0 ? true : false;
                    b = ConstantMethod.set_bit(b, pos, flag);
                }
            }

          return true;
        }

        public cmdRobot()
        {
            
        }
        
        public static List<cmdRobot> createCmdRobotFromXml(string fileName)
        {
            if (!File.Exists(fileName)) return null;
            FileStream
            fs = new FileStream(fileName, FileMode.Open);

            XmlSerializer xs1 = new XmlSerializer(typeof(List<cmdRobot>));
            List<cmdRobot> listPers = xs1.Deserialize(fs) as List<cmdRobot>;
            fs.Close();
            return listPers;
        }

    }
}
