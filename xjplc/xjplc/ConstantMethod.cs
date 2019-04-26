
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Diagnostics;
using System.Management;
using System.Net.Sockets;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;
using System.Reflection;
using Microsoft.Win32;
using System.Linq.Expressions;
using FastReport;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;

namespace xjplc
{
    public static class LogManager
    {

        static object locker = new object();

        public  static string LogFileName ;

        public static void WriteProgramLogProdData(params string[] logs)
        {
            lock (locker)
            {
                string LogAddress = ConstantMethod.GetAppPath() + "Log";
                if (!Directory.Exists(LogAddress))
                {
                    Directory.CreateDirectory(LogAddress);
                }
                LogAddress = string.Concat(LogAddress, "\\",
                DateTime.Now.Year, '-', DateTime.Now.Month, '-',
                DateTime.Now.Day, "生产.log");
                if (!File.Exists(LogAddress))
                {
                    var newFile = File.Create(LogAddress);
                    newFile.Close();
                }
                StreamWriter sw = new StreamWriter(LogAddress, true, System.Text.Encoding.Default);
                foreach (string log in logs)
                {
                    sw.WriteLine(string.Format("[{0}] {1}", DateTime.Now.ToString(), log));
                }
                LogFileName = LogAddress;
                sw.Close();
            }
        }


       
        /// <summary>
        /// 重要信息写入日志
        /// </summary>
        /// <param name="logs">日志列表，每条日志占一行</param>
        public static void WriteProgramLog(params string[] logs)
        {
            lock (locker)
            {
                string LogAddress = ConstantMethod.GetAppPath() + "Log";
                if (!Directory.Exists(LogAddress))
                {
                    Directory.CreateDirectory(LogAddress);
                }
                LogAddress = string.Concat(LogAddress,"\\",
                DateTime.Now.Year, '-', DateTime.Now.Month, '-',
                DateTime.Now.Day, ".log");
                if (!File.Exists(LogAddress))
                {
                  var newFile=  File.Create(LogAddress);
                    newFile.Close();
                }
                StreamWriter sw = new StreamWriter(LogAddress, true,System.Text.Encoding.Default);
                foreach (string log in logs)
                {
                    sw.WriteLine(string.Format("[{0}] {1}", DateTime.Now.ToString(), log));
                }
                LogFileName = LogAddress;
                sw.Close();
            }
        }
    }


    public class RegistryHelpers
    {

        public static RegistryKey GetRegistryKey()
        {
            return GetRegistryKey(null);
        }

        public static RegistryKey GetRegistryKey(string keyPath)
        {
            RegistryKey localMachineRegistry
                = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser,
                                          Environment.Is64BitOperatingSystem
                                              ? RegistryView.Registry64
                                              : RegistryView.Registry32);

            return string.IsNullOrEmpty(keyPath)
                ? localMachineRegistry
                : localMachineRegistry.OpenSubKey(keyPath);
        }
       
        public static object GetRegistryValue(string keyPath, string keyName)
        {
            RegistryKey registry = GetRegistryKey(keyPath);
            return registry.GetValue(keyName);
        }
    }


   
    public class ConstantMethod
    {


        public static string DesktopPath
        {
            get
            {
               return  Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

            }
        }
        //去掉字符串中的非数字
        public static string RemoveNotNumber(string key)
        {
            return Regex.Replace(key, @"[^\d]*", "");
        }

        //判断字符串是不是日期
        public static bool IsDateTimeStr(string s)
        {
            DateTime dStr = new DateTime();

            if (DateTime.TryParse(s, out dStr))
            {
                return true;
            }

            return false;

        }
        public static bool fileCopy(string source, string dest)
        {
            if (!File.Exists(source)) return false;
            bool isrewrite = true; // true=覆盖已存在的同名文件,false则反之

            System.IO.File.Copy(source, dest, isrewrite);
            /***         
            //filestram copy
            FileStream fsread = new FileStream(source, FileMode.OpenOrCreate, FileAccess.Read);

            FileStream fswrite = new FileStream(dest, FileMode.OpenOrCreate, FileAccess.Write);

            int r = 0;
            byte[] buffer = new byte[1024 * 1024 * 5];
            while (true)
            {
                r = fsread.Read(buffer, 0, buffer.Length);
                if (r == 0) break;
                fswrite.Write(buffer, 0, r);
            }

            fswrite.Close();
            fsread.Close();
            ****/
            return true;


        }
        public static DataTable getDataTableByString(string[] valueCol)
        {
         
            DataTable dt = new DataTable();

            for (int i = 0; i < valueCol.Length; i++)
            {
                dt.Columns.Add(valueCol[i]);
            }
            return dt;
        }

        //根据规则转换表格 得到数据表格
        public static DataTable convertDataTableByRule(DataTable UserDtTmp, List<int> valueCol)
        {
            DataTable dtOutPutTmp = new DataTable("file");

            for (int i = 0; i < valueCol.Count(); i++)
            {
                if (valueCol[i] >= UserDtTmp.Columns.Count) return null;
            }
            //收集数据 保存
            if (valueCol.Count > 3 && UserDtTmp.Columns.Count >= valueCol.Count)
            {

                DataColumn dtcolSize = new DataColumn("尺寸");

                DataColumn dtcolCnt = new DataColumn("设定数量");

                DataColumn dtcolCntDone = new DataColumn("已切数量");

                DataColumn dtcolBarCode = new DataColumn("条码");

                dtOutPutTmp.Columns.Add(dtcolSize);
                dtOutPutTmp.Columns.Add(dtcolCnt);
                dtOutPutTmp.Columns.Add(dtcolCntDone);
                dtOutPutTmp.Columns.Add(dtcolBarCode);

                //增加列  ConstantMethod.ShowInfo(rtbResult,"开始转换，转换规则如下");
                for (int i = 0; i < (valueCol.Count - 3); i++)
                {
                    DataColumn dtcolParm = new DataColumn("参数" + (i + 1).ToString());
                    dtOutPutTmp.Columns.Add(dtcolParm);
                }

                //增加行
                foreach (DataRow row in UserDtTmp.Rows)
                {
                    DataRow dr2 = dtOutPutTmp.NewRow();

                    for (int i = 0; i < dr2.ItemArray.Length; i++)
                    {
                        if (i == 2)
                        {
                            dr2[i] = "0";
                        }
                        else
                        {
                            if (i < 2)
                            {
                                dr2[i] = row[valueCol[i]];
                            }
                            else
                            {
                                dr2[i] = row[valueCol[i - 1]];
                            }
                        }

                    }

                    dtOutPutTmp.Rows.Add(dr2);
                }
            }

            return dtOutPutTmp;
        }

        //color convert
        public static Int32 ParseRGB(Color color)
        {
            return (Int32)(((uint)color.B << 16) | (ushort)(((ushort)color.G << 8) | color.R));
        }
        #region 信捷PLC 使用
        /// 变量与表格对应起来 可以实时读取数据
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="psLst"></param>
        public static void FindPos(DataTable dt, List<PlcInfoSimple> psLst)
        {
            if (dt != null && psLst.Count > 0)
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int j = 0; j < psLst.Count; j++)
                    {
                        if (dt.Rows[i]["bin"].ToString().Equals(psLst[j].Name))
                        {

                            psLst[j].Mode = dt.Rows[i]["mode"].ToString();
                            psLst[j].RowIndex = i;
                            psLst[j].BelongToDataform = dt;
                            int addrInt = 0;
                            string areaStr = "D";
                            string userdata = dt.Rows[i]["addr"].ToString();
                            string param3 = dt.Rows[i]["param3"].ToString();
                            string param4 = dt.Rows[i]["param4"].ToString();
                            if (!string.IsNullOrWhiteSpace(param3))
                            {
                                psLst[j].ShowStr.Add(param3);
                            }
                            if (!string.IsNullOrWhiteSpace(param4))
                            {
                                psLst[j].ShowStr.Add(param4);
                            }
                            ConstantMethod.SplitAreaAndAddr(userdata, ref addrInt, ref areaStr);
                            //区域符号在前面 后面地址就可以计算了
                            psLst[j].Area = areaStr;
                            psLst[j].Addr = addrInt;


                        }
                    }
                }

        }

        public static bool setControlInPlcSimple(PlcInfoSimple simple2, Control control)
        {
            //20181102 数据获取的唯一性 需要一致才行     XXX读  XXX读写 XXX写读
            string str1 = control.Tag.ToString() + Constant.Read;
            string str2 = control.Tag.ToString() + Constant.Write + Constant.Read;
            string str3 = control.Tag.ToString() + Constant.Read + Constant.Write;


            if (simple2.Name.Equals(str1) || simple2.Name.Equals(str2) || simple2.Name.Equals(str3))
            {
                simple2.ShowControl = control;
                return true;
            }

            return false;
        }

        public static bool setControlInPlcSimple(DTPlcInfoSimple simple2, Control control)
        {
            //20181102 数据获取的唯一性 需要一致才行     XXX读  XXX读写 XXX写读        

          
            string str1 = simple2.Name;
            string str2 = control.Tag.ToString();

            //如果带读 字样 那就去掉 然后去掉读写 看下 是不是可以在一起
            if (str1.Contains(Constant.Read))
            {
                str1= str1.Replace(Constant.Write, "");
                str1= str1.Replace(Constant.Read, "");

                if (str1.Equals(str2))
                {
                    simple2.ShowControl = control;
                    return true;
                }
            }

            return false;
        }
        public static bool XJFindPort() //如果可以连接 则写到config里
        {
            SerialPort m_serialPort = new SerialPort();

            string[] str = SerialPort.GetPortNames();

            List<string> portNameLst = new List<string>();
            PortParam portparam0 = new PortParam(); 

            try
            {
                if (File.Exists(Constant.ConfigSerialportFilePath))
                {
                    portparam0 = ConstantMethod.LoadPortParam(Constant.ConfigSerialportFilePath);
                    ConstantMethod.fileCopy(Constant.ConfigSerialportFilePath, Constant.ConfigSerialportFilePath_bak);
                }
                else
                {
                    if (!ConstantMethod.fileCopy(Constant.ConfigSerialportFilePath_bak, Constant.ConfigSerialportFilePath))
                    {
                        MessageBox.Show(Constant.ConfigSerialportFilePath_bak + Constant.ErrorSerialportConfigFile);

                        Application.Exit();

                        System.Environment.Exit(0);

                    }
                }

            }
            catch
            {

                if (!ConstantMethod.fileCopy(Constant.ConfigSerialportFilePath_bak, Constant.ConfigSerialportFilePath))
                {
                    MessageBox.Show(Constant.ConfigSerialportFilePath_bak+Constant.ErrorSerialportConfigFile);

                    Application.Exit();

                    System.Environment.Exit(0);

                }
                portparam0 = ConstantMethod.LoadPortParam(Constant.ConfigSerialportFilePath);
            }
            for (int i = 0; i < (str.Length + 1); i++)
            {
                try
                {
                    if (i == 0)
                        m_serialPort.PortName = portparam0.m_portName;
                    else
                    {
                        if (!str[i - 1].Equals(portparam0.m_portName))
                            m_serialPort.PortName = str[i - 1];
                        else continue;
                    }
                    m_serialPort.BaudRate = portparam0.m_baudRate;
                    m_serialPort.Parity = portparam0.m_parity;
                    m_serialPort.StopBits = portparam0.m_stopbits;
                    m_serialPort.Handshake = portparam0.m_handshake;
                    m_serialPort.DataBits = portparam0.m_dataBits;
                    m_serialPort.ReadBufferSize = 1024;
                    m_serialPort.WriteBufferSize = 1024;
                    m_serialPort.ReadTimeout = 100;
                    if (!m_serialPort.IsOpen)
                    {
                        m_serialPort.Open();
                    }
                    byte[] resultByte = new byte[Constant.XJExistByteIn.Count()];

                    m_serialPort.Write(Constant.XJExistByteOut, 0, Constant.XJExistByteOut.Length);

                    ConstantMethod.Delay(200);

                    m_serialPort.Read(resultByte, 0, Constant.XJExistByteIn.Count());

                    if (ConstantMethod.compareByteStrictly(resultByte, Constant.XJExistByteIn))
                    {
                        //rtbResult.AppendText("连接成功" + m_serialPort.PortName);
                        ConstantMethod.SetPortParam(Constant.ConfigSerialportFilePath, Constant.PortName, m_serialPort.PortName);
                        return true;
                    }

                }
                catch
                {
                    //rtbResult.AppendText("连接失败" + m_serialPort.PortName);
                    // throw new SerialPortException(
                    //string.Format("无法打开串口:{0}", m_serialPort.PortName));
                    //continue;

                }
                finally { m_serialPort.Close(); };

            }

            //GC.Collect();

            //GC.WaitForPendingFinalizers();

            return false;

        }
        #endregion
        public static int getModeCount(string type)
        {
            if (type.Equals(Constant.asPlcTcpType[0]))
            {
                return Constant.asPlcTcpTypeByteCount[0];
            }

            if (type.Equals(Constant.asPlcTcpType[1]))
               return Constant.asPlcTcpTypeByteCount[1];

            if (type.Equals(Constant.asPlcTcpType[2]))
                return Constant.asPlcTcpTypeByteCount[2];

            if (type.Equals(Constant.asPlcTcpType[3]))
                return Constant.asPlcTcpTypeByteCount[3];

            int count = 0;
            Dictionary<string, int> typerCount = new Dictionary<string, int>();
            ConstantMethod.ArrayToDictionary(typerCount,Constant.tcpType,Constant.tcpTypeByteCount);

            count = typerCount[type];
            return count;
        }
        //统计字符出现次数
        public static int CharNum(string str, string search)
        {
            int count = 0;
            if (!string.IsNullOrEmpty(str) || !string.IsNullOrEmpty(search))
            {
                string[] resultString = Regex.Split(str, search, RegexOptions.IgnoreCase);

                count = resultString.Length;
            }
            return count;

        }
        //切换两个控件位置大小 PIC1为大 PIC 为小
        public static void SwitchPic(Form f,ref PictureBox c1, ref PictureBox c2,int width0,int height0)
        {
            c1.Parent = null;
            c2.Parent = null;
            c2.Parent = f;
            int height = c1.Height;
            int width = c1.Width;
                      
            int top = c1.Top;
            int left = c1.Left;

            c2.Height = height;
            c2.Width = width;
            c2.Top = top;
            c2.Left = left;

            c1.Height = height0;
            c1.Width = width0;
            c1.Top = 0;
            c1.Left = 0;
                  

           

            c1.Parent = c2;
           

        }
        public static string GetParamPwd(int i)
        {
            string str = DateTime.Now.ToString("MMdd");
            int psswdInt = 0;
            int.TryParse(str, out psswdInt);
            psswdInt = psswdInt + Constant.PwdOffSet * i;
            return psswdInt.ToString();
        }
        public static string getDataMultipleZero(string str)
        {
            float s = 0;

            if (float.TryParse(str, out s))
            {
                s = s * 1000;
               // return s.ToString();
            }

            return s.ToString() ;
        }
        public static string getDataMultipleZero(string str,double ration)
        {
            double s = 0;
            if (ration < 2) ration = 1;

            if (double.TryParse(str, out s))
            {
                s = s * ration;
                // return s.ToString();
            }

            return s.ToString();
        }

        public static byte[] convertSingleToBytesFromString(string value)
        {
            float mc = 0;
            if (float.TryParse(value, out mc))
            {
                byte[] bytes = BitConverter.GetBytes(mc);
                return bytes;
            }
            else return null;

        }
        //4个字节 转化为2个字节 在4字节浮点数 转换为 2个寄存器单元过程中
        public static ushort[] convert4BytesTo2Bytes(byte[] sourceByte)
        {
            if (sourceByte == null) return null;
            if (sourceByte.Count() == 4)
            {
                byte[] desByteL = new byte[2];
                byte[] desByteH = new byte[2];
                Array.Copy(sourceByte,0, desByteL,0, 2);
                Array.Copy(sourceByte, 2, desByteH, 0, 2);


                ushort u16;
                u16 = (ushort)((desByteL[1] << 8) + desByteL[0]);
                ushort u17;
                u17 = (ushort)((desByteH[1] << 8) + desByteH[0]);
                List<ushort> uLst = new List<ushort>();
                uLst.Add(u16);
                uLst.Add(u17);
                return uLst.ToArray();
            }
            return null;

        }
        public static ushort convertBytesToShort(byte[] bytes)
        {
            if (bytes.Count() == 2)
            {
             
                ushort u16;
                u16 = (ushort)((bytes[0] << 8) + bytes[1]);
                return u16;
            }
            return 0;
        }

        public static string  getValueFromByte(string type,byte[] bytevalue)
        {
            if (type.Equals(Constant.asPlcTcpType[0]))
            {

                int v = BitConverter.ToInt16(bytevalue, 0);
                return v.ToString();

            }
            if (type.Equals(Constant.asPlcTcpType[1]))
            {
                int v = BitConverter.ToInt32(bytevalue, 0);
                return v.ToString();
            }
            if (type.Equals(Constant.asPlcTcpType[2]))
            {
                int v = BitConverter.ToInt32(bytevalue, 0);
                return v.ToString();
            }

            if (type.Equals(Constant.asPlcTcpType[3]))
            {
                if (bytevalue!=null && bytevalue.Length>0)
                return bytevalue[0].ToString();
            }
            //这里还有个浮点 不过应该不用

            string valueStr="";

            if (type.Equals(Constant.Bool) && bytevalue.Count() == Constant.BoolMemory)
            {
                sbyte v = (sbyte)bytevalue[0];
                valueStr = v.ToString();
            }
            if (type.Equals(Constant.SINT) && bytevalue.Count() == Constant.SINTMemory)
            {             
                sbyte v = (sbyte)bytevalue[0];               
            }

            if (type.Equals(Constant.USINT) && bytevalue.Count() == Constant.USINTMemory)
            {
                byte v = (byte)bytevalue[0];
                valueStr = v.ToString();            
            }
            if (type.Equals(Constant.INT) && bytevalue.Count() == Constant.INTMemory)
            {
                short v  = BitConverter.ToInt16(bytevalue, 0);
                valueStr = v.ToString();
            }

            if (type.Equals(Constant.UINT) && bytevalue.Count() == Constant.UINTMemory)
            {
                ushort v = BitConverter.ToUInt16(bytevalue, 0);
                valueStr = v.ToString();
            }

            if (type.Equals(Constant.DINT) && bytevalue.Count() == Constant.DINTMemory)
            {
                int v = BitConverter.ToInt32(bytevalue, 0);
                valueStr = v.ToString();
            }
            if (type.Equals(Constant.UDINT) && bytevalue.Count() == Constant.UDINTMemory)
            {
                uint v = BitConverter.ToUInt32(bytevalue, 0);
                valueStr = v.ToString();
            }

            if (type.Equals(Constant.LINT) && bytevalue.Count() == Constant.LINTMemory)
            {
                Int64 v = BitConverter.ToInt64(bytevalue, 0);
                valueStr = v.ToString();
            }
            if (type.Equals(Constant.ULINT) && bytevalue.Count() == Constant.ULINTMemory)
            {
                UInt64 v = BitConverter.ToUInt64(bytevalue, 0);
                valueStr = v.ToString();
            }

            if (type.Equals(Constant.REAL) && bytevalue.Count() == Constant.REALMemory)
            {
                float v = BitConverter.ToSingle(bytevalue, 0);
                valueStr = v.ToString();
            }

            if (type.Equals(Constant.LREAL) && bytevalue.Count() == Constant.LREALMemory)
            {
                double v = BitConverter.ToDouble(bytevalue, 0);
                valueStr = v.ToString();
            }

            return valueStr;
        }


        public static List<int> DeleteDataFromARefB(List<int> A, List<int> B)
        {

            foreach (int s in B)
            {
                A.Remove(s);
            }

            return A;
        }
        #region 台达PLC 

        public static DTPlcInfoSimple getDtPlcSimple(string name,List<DTPlcInfoSimple> pLst)
        {                                       
            foreach (DTPlcInfoSimple p in pLst)           
            {
                if (p.Name.Contains(name) )
                {
                     return p;
                }
            }

            return null;

        }
        public static PlcInfoSimple getPlcSimple(string name, List<PlcInfoSimple> pLst)
        {
            foreach (PlcInfoSimple p in pLst)
            {
                if (p.Name.Contains(name))
                {
                    return p;
                }
               
            }

            return new PlcInfoSimple(name); 

        }
        public static string GetAppPath()
        {
            string softName = Path.GetFileName(Application.ExecutablePath);
            string keyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run\";
            string keyName = softName;
            object connectionString = RegistryHelpers.GetRegistryValue(keyPath, keyName);
            String dir = "";
            try
            {
                dir = Path.GetDirectoryName(connectionString.ToString()) + "\\";
            }
            catch (Exception ex)
            {
                return System.AppDomain.CurrentDomain.BaseDirectory;
            }

            if (Directory.Exists(dir))
                return dir;
            else return System.AppDomain.CurrentDomain.BaseDirectory;
        }
        public static void AutoStart(bool isAuto)
        {

            try
            {
                if (isAuto == true)
                {
                    //RegistryKey R_local = Registry.LocalMachine;
                    RegistryKey R_local = Registry.CurrentUser;
                    RegistryKey R_run = R_local.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                    string s = System.IO.Path.GetFileName(Application.ExecutablePath);
                    R_run.SetValue(s, Application.ExecutablePath);
                    R_run.Close();
                    R_local.Close();
                }
                else
                {
                    RegistryKey R_local = Registry.LocalMachine;//RegistryKey R_local = Registry.CurrentUser;
                    RegistryKey R_run = R_local.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                    R_run.DeleteValue(System.IO.Path.GetFileName(Application.ExecutablePath), false);
                    R_run.Close();
                    R_local.Close();
                }

                //GlobalVariant.Instance.UserConfig.AutoStart = isAuto;
            }
            catch (Exception ex)
            {
                //MessageBoxDlg dlg = new MessageBoxDlg();
                //dlg.InitialData("您需要管理员权限修改", "提示", MessageBoxButtons.OK, MessageBoxDlgIcon.Error);
                //dlg.ShowDialog();
                MessageBox.Show("您需要管理员权限修改", "提示");
            }
        }
        public static FastReport.Report GetPrintReport()
        {
            FastReport.Report rp1 = new Report();

            string filter = "*.frx";
            string FilePath = System.AppDomain.CurrentDomain.BaseDirectory;
            string[] getbarcodepath;
            getbarcodepath = Directory.GetFiles(FilePath, filter);
            if (Directory.GetFiles(FilePath, filter).Length == 0)
            {
                MessageBox.Show(Constant.barCodeError);
            }
            else
            {
                if (Directory.GetFiles(FilePath, filter).Length > 1)
                {
                    MessageBox.Show(Constant.barCodeError1);
                }
                if (Directory.GetFiles(FilePath, filter).Length == 1)
                {
                    rp1.Load(getbarcodepath[0]);
                }
            }
            return rp1;
        }

        public static void FindPosTcp(DataTable dt, List<DTPlcInfoSimple> psLst)
        {
            if (dt != null && psLst.Count > 0)
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int j = 0; j < psLst.Count; j++)
                    {
                        if (dt.Rows[i]["bin"].ToString().Equals(psLst[j].Name))
                        {

                            psLst[j].Mode = dt.Rows[i]["mode"].ToString();
                            psLst[j].RowIndex = i;
                            psLst[j].BelongToDataform = dt;
                            int addrInt = 0;
                            string areaStr = "D";
                            string userdata = dt.Rows[i]["addr"].ToString();
                            string param3 = dt.Rows[i]["param3"].ToString();
                            string param4 = dt.Rows[i]["param4"].ToString();
                            if (!string.IsNullOrWhiteSpace(param3))
                            {
                                psLst[j].ShowStr.Add(param3);
                            }
                            if (!string.IsNullOrWhiteSpace(param4))
                            {
                                psLst[j].ShowStr.Add(param4);
                            }

                            ConstantMethod.getAddrAndAreaByStr(userdata, ref addrInt, ref areaStr);
                           // ConstantMethod.SplitAreaAndAddr(userdata, ref addrInt, ref areaStr);
                            //区域符号在前面 后面地址就可以计算了
                            psLst[j].Area = areaStr;
                            psLst[j].Addr = addrInt;


                        }
                    }
                }

        }
        public static void getAddrAndAreaByStrUseAsPlc(string strArea, ref int addr, ref string area)
        {
            string strSplit1;
            string strSplit2;//地址区域          

            strSplit1 = Regex.Replace(strArea.Trim(), "[A-Z]", "", RegexOptions.IgnoreCase);

            //取字母
            strSplit2 = Regex.Replace(strArea.Trim(), "[0-9]", "", RegexOptions.IgnoreCase);

            strSplit1 = strSplit1.Trim();
            strSplit2 = strSplit2.Trim();
            strSplit2 = strSplit2.Replace(".", "");

            area = strSplit2;


            if (strSplit1.Contains("."))
            {
                int z = 0;
                int x = 0;
                string[] value = strSplit1.Split('.');
                if (value.Count() == 2)
                {
                    if (int.TryParse(value[0], out z) && int.TryParse(value[1], out x))
                    {
                        addr = z * 16 + x;
                    }
                }
            }
            else
            {
                if (!int.TryParse(strSplit1, out addr))
                {

                }
            }
        }

        public static void getAddrAndAreaByStr(string strArea, ref int addr, ref string area)
        {
            string strSplit1;
            string strSplit2;//地址区域          

            strSplit1 = Regex.Replace(strArea.Trim(), "[A-Z]", "", RegexOptions.IgnoreCase);

            //取字母
            strSplit2 = Regex.Replace(strArea.Trim(), "[0-9]", "", RegexOptions.IgnoreCase);

            strSplit1 = strSplit1.Trim();
            strSplit2 = strSplit2.Trim();
            strSplit2 = strSplit2.Replace(".", "");

            area = strSplit2;


            if (strSplit1.Contains("."))
            {
                int z = 0;
                int x = 0;
                string[] value = strSplit1.Split('.');
                if (value.Count() == 2)
                {
                    if (int.TryParse(value[0], out z) && int.TryParse(value[1], out x))
                    {
                        addr = z * 8 + x;
                    }
                }
            }
            else
            {
                if (!int.TryParse(strSplit1, out addr))
                {

                }
            }
        }
        public static void FindPos(DataTable dt, List<DTPlcInfoSimple> psLst)
        {
            if (dt != null && psLst.Count > 0)
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int j = 0; j < psLst.Count; j++)
                    {
                        if (dt.Rows[i]["bin"].ToString().Equals(psLst[j].Name))
                        {

                            psLst[j].Mode = dt.Rows[i]["mode"].ToString();
                            psLst[j].RowIndex = i;
                            psLst[j].BelongToDataform = dt;
                            int addrInt = 0;
                            string areaStr = "D";
                            string userdata = dt.Rows[i]["addr"].ToString();
                            string param3 = dt.Rows[i]["param3"].ToString();
                            string param4 = dt.Rows[i]["param4"].ToString();
                            if (!string.IsNullOrWhiteSpace(param3))
                            {
                                psLst[j].ShowStr.Add(param3);
                            }
                            if (!string.IsNullOrWhiteSpace(param4))
                            {
                                psLst[j].ShowStr.Add(param4);
                            }
                            ConstantMethod.SplitAreaAndAddr(userdata, ref addrInt, ref areaStr);
                            //区域符号在前面 后面地址就可以计算了
                            psLst[j].Area = areaStr;
                            psLst[j].Addr = addrInt;


                        }
                    }
                }

        }

        /// <summary>
        /// 这里检测台达PLC 是否存在 并发送 可以设置读取命令的数据
        /// </summary>
        /// <returns></returns>
        public static bool DTFindPort() //如果可以连接 则写到config里
        {
            SerialPort m_serialPort = new SerialPort();

            string[] str = SerialPort.GetPortNames();

            List<string> portNameLst = new List<string>();

            PortParam portparam0 = ConstantMethod.LoadPortParam(Constant.ConfigSerialportFilePath);

            for (int i = 0; i < (str.Length + 1); i++)
            {
                try
                {
                    if (i == 0)
                        m_serialPort.PortName = portparam0.m_portName;
                    else
                    {
                        if (!str[i - 1].Equals(portparam0.m_portName))
                            m_serialPort.PortName = str[i - 1];
                        else continue;
                    }
                    m_serialPort.BaudRate = portparam0.m_baudRate;
                    m_serialPort.Parity = portparam0.m_parity;
                    m_serialPort.StopBits = portparam0.m_stopbits;
                    m_serialPort.Handshake = portparam0.m_handshake;
                    m_serialPort.DataBits = portparam0.m_dataBits;
                    m_serialPort.ReadBufferSize = 1024;
                    m_serialPort.WriteBufferSize = 1024;
                    m_serialPort.ReadTimeout = 100;
                    if (!m_serialPort.IsOpen)
                    {
                        m_serialPort.Open();
                    }
                    byte[] resultByte = new byte[Constant.DTExistByteOutIn0.Count()];

                    m_serialPort.Write(Constant.DTExistByteOutIn0, 0, Constant.DTExistByteOutIn0.Length);

                    ConstantMethod.Delay(200);

                    m_serialPort.Read(resultByte, 0, Constant.DTExistByteOutIn0.Count());

                    m_serialPort.Write(Constant.DTExistByteOutIn485, 0, Constant.DTExistByteOutIn485.Length);
                    if (ConstantMethod.compareByteStrictly(resultByte, Constant.DTExistByteOutIn0))
                    {
                        //rtbResult.AppendText("连接成功" + m_serialPort.PortName);
                        ConstantMethod.SetPortParam(Constant.ConfigSerialportFilePath, Constant.PortName, m_serialPort.PortName);
                        return true;
                    }

                }
                catch
                {
                    //rtbResult.AppendText("连接失败" + m_serialPort.PortName);
                    // throw new SerialPortException(
                    //string.Format("无法打开串口:{0}", m_serialPort.PortName));
                    //continue;
                }
                finally { m_serialPort.Close(); };

            }

            //GC.Collect();
            //GC.WaitForPendingFinalizers();

            return false;

        }

        #endregion

        //两个类之间相同的属性赋值 反射原理 要好好研究下
        public static D Mapper<D, S>(S s)
        {
            D d = Activator.CreateInstance<D>(); //构造新实例
            try
            {


                var Types = s.GetType();//获得类型  
                var Typed = typeof(D);

                foreach (PropertyInfo sp in Types.GetProperties())//获得类型的属性字段  
                {
                    foreach (PropertyInfo dp in Typed.GetProperties())
                    {
                        if (dp.Name == sp.Name && dp.PropertyType == sp.PropertyType && dp.Name != "Error" && dp.Name != "Item")//判断属性名是否相同  
                        {
                            dp.SetValue(d, sp.GetValue(s, null), null);//获得s对象属性的值复制给d对象的属性  
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return d;
        }

        public static void CheckAllCtrls(Control item, Queue<Control> allCtrls)
        {
          
            for (int i = 0; i < item.Controls.Count; i++)
            {
                if (item.Controls[i].HasChildren)
                {
                    CheckAllCtrls(item.Controls[i], allCtrls);
                }
                allCtrls.Enqueue(item.Controls[i]);
            }
        }
        public static void ShowInfo(RichTextBox r1, string s)
        {
            if (r1 != null && r1.IsHandleCreated)
            {
                r1.Invoke((EventHandler)(delegate
                {
                    r1.ScrollToCaret();
                    r1.AppendText(s + "\n");
                }));
            }

        }

        //字符串变量 根据语言文件来更新
        public static class MemberInfoGetting
        {
            public static string GetMemberName<T>(Expression<Func<T>> memberExpression)
            {
                MemberExpression expressionBody = (MemberExpression)memberExpression.Body;
                return expressionBody.Member.Name;
            }
        }
        public static string ShiftString(string s)
        {
            string result = "";
            result = s.Replace("[", "[\"[");
            result = result.Replace("]", "]\"]");

            return result;
        }
        public static void SaveDirectoryByFileDialog(OpenFileDialog op1)
        {
            if (op1 != null)
            {
                if (File.Exists(op1.FileName))
                    op1.InitialDirectory = Path.GetDirectoryName(op1.FileName);
            }

        }

        public static string getStrInKuoHao(string s)
        {
            string result = "";
            int pos0 = s.IndexOf("<");
            int pos1 = s.IndexOf(">");
            if (pos0 > -1 && pos1 > -1 && pos1 > pos0)
            {
                result = s.Substring(pos0 + 1, pos1 - pos0 - 1);
            }
            return result;
        }
        public static void SetText(Control r1, string s)
        {
            if (r1 != null && r1.IsHandleCreated)
            {
                r1.Invoke((EventHandler)(delegate
                {
                    r1.Text = s;
                }));
            }

        }
        public static void ShowInfoNoScrollEnd(RichTextBox r1, string s)
        {
            if (r1 != null && r1.IsHandleCreated)
            {
                r1.Invoke((EventHandler)(delegate
                {
                    r1.AppendText(s + "\n");
                }));
            }

        }

        #region 加密
        public static bool UserPassWd(int id)
        {
            passWdForm psswd = new passWdForm();
            psswd.ShowDialog();
            while (psswd.Visible)
            {
                Application.DoEvents();
            }

            if (psswd.userInput.Equals(ConstantMethod.GetParamPwd(id)))
            {
                psswd.Close();
                return true;
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(psswd.userInput))
                    MessageBox.Show(Constant.pwdWrong);
                psswd.Close();

            }
            return false;

        }

        /// <summary>
        /// 一般用户界面密码 动态密码 日期加1000
        /// </summary>
        /// <returns></returns>
        public static bool UserPassWd()
        {
            passWdForm psswd = new passWdForm();
            psswd.ShowDialog();
            while (psswd.Visible)
            {
                Application.DoEvents();
            }  

            if (psswd.userInput.Equals(ConstantMethod.GetParamPwd(1)))
            {
                psswd.Close();
                return true;
            }
            else
            {
                if(!string.IsNullOrWhiteSpace(psswd.userInput))
                MessageBox.Show(Constant.pwdWrong);
                psswd.Close();
                
            }
            return false;

        }
        //定义一个函数，返回字符串中的汉字个数
        public static int GetHanNumFromString(string str)
        {
            int count = 0;
            Regex regex = new Regex(@"^[\u4E00-\u9FA5]{0,}$");

            for (int i = 0; i < str.Length; i++)
            {
                if (regex.IsMatch(str[i].ToString()))
                {
                    count++;
                }
            }

            return count;
        }
        public static Bitmap RotateImg(Bitmap img, float angle)
        {
            //通过Png图片设置图片透明，修改旋转图片变黑问题。
            int width = img.Width;
            int height = img.Height;
            //角度
            Matrix mtrx = new Matrix();
            mtrx.RotateAt(angle, new PointF((width / 2), (height / 2)), MatrixOrder.Append);
            //得到旋转后的矩形
            GraphicsPath path = new GraphicsPath();
            path.AddRectangle(new RectangleF(0f, 0f, width, height));
            RectangleF rct = path.GetBounds(mtrx);
            //生成目标位图
            Bitmap devImage = new Bitmap((int)(rct.Width), (int)(rct.Height));
            Graphics g = Graphics.FromImage(devImage);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            //计算偏移量
            Point Offset = new Point((int)(rct.Width - width) / 2, (int)(rct.Height - height) / 2);
            //构造图像显示区域：让图像的中心与窗口的中心点一致
            Rectangle rect = new Rectangle(Offset.X, Offset.Y, (int)width, (int)height);

            Point center = new Point((int)(rect.X + rect.Width / 2), (int)(rect.Y + rect.Height / 2));
            g.TranslateTransform(center.X, center.Y);
            g.RotateTransform(angle);
            g.Clear(Color.White);
            //恢复图像在水平和垂直方向的平移
            g.TranslateTransform(-center.X, -center.Y);
            g.DrawImage(img, rect);
            //重至绘图的所有变换
            g.ResetTransform();

            g.Save();
            g.Dispose();
            path.Dispose();
            return devImage;
        }

        public static void rotatePic(PictureBox pb, int angle)
        {
            Graphics graphics = pb.CreateGraphics();
            graphics.Clear(pb.BackColor);
            //装入图片
            Bitmap image = new Bitmap(pb.Image);
            //获取当前窗口的中心点
            Rectangle rect = new Rectangle(0, 0, pb.Width, pb.Height);
            PointF center = new PointF(rect.Width / 2, rect.Height / 2);
            float offsetX = 0;
            float offsetY = 0;
            offsetX = center.X - image.Width / 2;
            offsetY = center.Y - image.Height / 2;
            //构造图片显示区域:让图片的中心点与窗口的中心点一致
            RectangleF picRect = new RectangleF(offsetX, offsetY, image.Width, image.Height);
            PointF Pcenter = new PointF(picRect.X + picRect.Width / 2,
                picRect.Y + picRect.Height / 2);
            // 绘图平面以图片的中心点旋转
            graphics.TranslateTransform(Pcenter.X, Pcenter.Y);
            graphics.RotateTransform(angle);
            //恢复绘图平面在水平和垂直方向的平移
            graphics.TranslateTransform(-Pcenter.X, -Pcenter.Y);
            //绘制图片
            graphics.DrawImage(image, picRect);
        }
        /// <summary>
        /// 程序 根据日期加密 日期乘以后面的count为密码
        /// </summary>
        /// <returns></returns>
        public static bool InitPassWd()
        {
            ConfigFileManager passWdFile = new ConfigFileManager();
            try
            {
                passWdFile.LoadFile(Constant.ConfigPassWdFilePath);
                ConstantMethod.fileCopy(Constant.ConfigPassWdFilePath, Constant.ConfigPassWdFilePath_Bak);
            }
            catch
            {
                if (!ConstantMethod.fileCopy(Constant.ConfigPassWdFilePath_Bak, Constant.ConfigPassWdFilePath))
                {
                    MessageBox.Show(Constant.ErrorPwdConfigFile);
                    ConstantMethod.AppExit();
                }
                passWdFile.LoadFile(Constant.ConfigPassWdFilePath);

            }


            string readTimeStr = passWdFile.ReadConfig(Constant.passwdTime);
            string readCountStr = passWdFile.ReadConfig(Constant.passwdCount);
            //统计下密码和设置时间 
            int readTimeInt = 0;
            int readCountInt = 0;
            passWdFile.Dispose();

            if (int.TryParse(readTimeStr, out readTimeInt) &&
                int.TryParse(readCountStr, out readCountInt))
            {
                DateTime dt = DateTime.ParseExact(readTimeStr, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);

                if (readCountInt > Constant.pwdCountMax) return true;

                if (dt <= DateTime.Now)
                {
                    passWdForm form = new passWdForm();
                    form.Pwd = (readCountInt * readTimeInt).ToString();
                    form.PwdCount = readCountInt;
                    form.IsStart = true;
                    form.ShowDialog();
                }
            }
            return true;
        }

        #endregion
        public static void Optimize(int n, int[] w, int[] v, int[] x, int C)
        {

            int[,] V = new int[n + 1, C + 1];//前i个物品装入容量为j的背包中获得的最大价值
            int i, j;
            for (i = 0; i <= n; i++)
            {
                V[i, 0] = 0;
            }
            for (j = 0; j <= C; j++)
            {
                V[0, j] = 0;
            }

            for (i = 1; i <= n - 1; i++)
                for (j = 1; j <= C; j++)
                {
                    if (j < w[i])

                        V[i, j] = V[i - 1, j];
                    else
                        V[i, j] = max(V[i - 1, j], V[i - 1, j - w[i]] + v[i]);
                }
            j = C;
            for (i = n - 1; i > 0; i--)
            {

                if (V[i, j] > V[i - 1, j])
                {
                    x[i] = 1;
                    j = j - w[i];
                }
                else
                    x[i] = 0;
            }

            V = null;

            //GC.Collect();
            //GC.WaitForPendingFinalizers();           
            return;
        }
        public static DataTable GetDgvToTable(DataGridView dgv)
        {
            DataTable dt = new DataTable();

            // 列强制转换
            for (int count = 0; count < dgv.Columns.Count; count++)
            {
                DataColumn dc = new DataColumn(dgv.Columns[count].Name.ToString());
                dt.Columns.Add(dc);
            }

            // 循环行
            for (int count = 0; count < dgv.Rows.Count; count++)
            {
                DataRow dr = dt.NewRow();
                for (int countsub = 0; countsub < dgv.Columns.Count; countsub++)
                {
                    dr[countsub] = Convert.ToString(dgv.Rows[count].Cells[countsub].Value);
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }
        //返回整型数据的 字节表示 低位在前 高位在后 配合insert range 的方式
        public static byte[] getDataLowHighByte(int addr)
        {


            List<byte> resultLst = new List<byte>();

            if (addr < 0) return resultLst.ToArray();

            int addr_low = addr & 0xFF;
            resultLst.Add((byte)addr_low);

            int addr_high = (addr & 0xFF00) >> 8;
            resultLst.Add((byte)addr_high);
            return resultLst.ToArray();
        }
        public static byte[] getDataHighLowByte(int addr)
        {
            if (addr < 0) return null;

            List<byte> resultLst = new List<byte>();
            int addr_high = (addr & 0xFF00) >> 8;
            resultLst.Add((byte)addr_high);

            int addr_low = addr & 0xFF;
            resultLst.Add((byte)addr_low);
            return resultLst.ToArray();


        }
        /// <summary>
        /// 判断是否八进制格式字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsOctal(string str)
        {
            const string PATTERN = @"[0-7]+$";
            return System.Text.RegularExpressions.Regex.IsMatch(str, PATTERN);
        }
        /// <summary>
        /// 8进制转十进制
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        public static int GetXYAddr8To10(int addr)
        {
            string strAddr = addr.ToString();
            if (IsOctal(strAddr))
                return Convert.ToInt32(strAddr, 8);
            else return 0;
        }
        /// <summary>
        /// 8进制转十进制
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        public static int GetXYAddr10To8(int addr)
        {
            string strAddr = Convert.ToString(addr, 8);
            if (int.TryParse(strAddr, out addr))
            {
                return addr;
            }
            return 0;

        }

        #region 打印机获取 静态函数
        private static PrintDocument fPrintDocument = new PrintDocument();
        List<string> printers = new List<string>();
        public static string DefaultPrinter
        {
            get { return fPrintDocument.PrinterSettings.PrinterName; }
        }
        /// <summary>
        ///  获取本地打印机的列表，第一项就是默认打印机
        /// </summary>
        public static List<string> GetLocalPrinter()
        {
            List<string> fPrinters = new List<string>();
            fPrinters.Add(DefaultPrinter);  //默认打印机出现在列表的第一项
            foreach (string fPrinterName in PrinterSettings.InstalledPrinters)
            {
                if (!fPrinters.Contains(fPrinterName))
                    fPrinters.Add(fPrinterName);
            }
            return fPrinters;
        }
        #endregion
        //比较两个字节数据 是否相等 以最小的字节数组为基础 局部相等 则返回true
        public static bool compareByte(byte[] b1, byte[] b2)
        {

            int b1cnt = b1.Count();
            int b2cnt = b2.Count();
            int j = b1cnt > b2cnt ? b2cnt : b1cnt;
            if (j < 1) return false;
            for (int i = 0; i < j; i++)
            {
                if (b1[i] != b2[i])
                { return false; }
            }
            return true;
        }
        public static bool compareString(string[] b1, string[] b2)
        {

            int b1cnt = b1.Count();
            int b2cnt = b2.Count();
            int j = b1cnt > b2cnt ? b2cnt : b1cnt;
            if (j < 1) return false;
            for (int i = 0; i < j; i++)
            {
                if (!b1[i].Equals(b2[i]))
                { return false; }
            }
            return true;
        }

        /// <summary>
        /// 根据datatable获得列名
        /// </summary>
        /// <param name="dt">表对象</param>
        /// <returns>返回结果的数据列数组</returns>
        public static string[] GetColumnsByDataTable(DataTable dt)
        {
            string[] strColumns = null;
            if (dt.Columns.Count > 0)
            {
                int columnNum = 0;
                columnNum = dt.Columns.Count;
                strColumns = new string[columnNum];
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    strColumns[i] = dt.Columns[i].ColumnName;
                }
            }

            return strColumns;
        }
        public static bool SetPortParam(string filepath, string property, string value)
        {
            ConfigFileManager configManager = new ConfigFileManager();
            if (File.Exists(filepath))
                configManager.LoadFile(filepath);
            else
            {
                MessageBox.Show(Constant.ErrorSerialportConfigFile);
                ConstantMethod.AppExit();

            }

            configManager.WriteConfig(property, value);

            return true;
        }
        //根据字符 将相应数组添加到字典
        public static void ArrayToDictionary(Dictionary<string, int> dt, string[] r1, int[] r2)
        {
            if (r1.Length != r2.Length) return;
            if (r1 == null || r2 == null || dt == null) return;
            for (int i = 0; i < r1.Count(); i++)
            {
                dt.Add(r1[i],r2[i]);
            }
        }

        public static void ArrayToDictionary(Dictionary<int, int> dt, int[] r1, int[] r2)
        {
            if (r1.Length != r2.Length) return;
            if (r1 == null || r2 == null || dt == null) return;
            for (int i = 0; i < r1.Count(); i++)
            {
                dt.Add(r1[i], r2[i]);
            }
        }

        public static ConfigFileManager  configFileBak(string filepath)
        {
            ConfigFileManager configManager = new ConfigFileManager();

            string ext = Path.GetExtension(filepath);

            string fileName = Path.GetFileNameWithoutExtension(filepath);
            string directory = Path.GetDirectoryName(filepath);

            string file_bak = directory + "\\" + fileName + "_bak" + ext;
            //configParam_default.xml
            string file_default= directory + "\\" + fileName + "_default" + ext;
            try
            {
                    if (configManager.LoadFile(filepath))
                    {
                        ConstantMethod.fileCopy(filepath, file_bak);
                    }
                    else
                    {
                        if (!ConstantMethod.fileCopy(file_bak, filepath))
                        {
                        MessageBox.Show(filepath + "错误！");

                        Application.Exit();

                        System.Environment.Exit(0);

                       }

                      if (!configManager.LoadFile(filepath))
                        {
                            if (!configManager.LoadFile(file_default))
                            {

                                MessageBox.Show(filepath + "错误！");

                                Application.Exit();

                                System.Environment.Exit(0);
                            }

                            ConstantMethod.fileCopy(file_default, filepath);
                        }
                    }                   
                }
                catch (Exception ex)
                {
                    if (!ConstantMethod.fileCopy(file_bak, filepath))
                    {
                        MessageBox.Show(filepath + "错误！");

                        Application.Exit();

                        System.Environment.Exit(0);

                    }
                    configManager.LoadFile(filepath);
                }

                      

            return configManager;

        }
        public static ServerInfo LoadServerParam(string filepath)
        {
            ServerInfo portparam0 = new ServerInfo();
            ConfigFileManager configManager = configFileBak(filepath);
           
            string ip = configManager.ReadConfig(Constant.ServerIp);

            string port= configManager.ReadConfig(Constant.ServerIpPort);

            portparam0.server_Ip = ip;
                 
            portparam0.server_Port = port;     

            
            return portparam0;
        }
        public static PortParam LoadPortParam(string filepath)
        {
            PortParam portparam0 = new PortParam();
            ConfigFileManager configManager = configFileBak(filepath);

            if (configManager != null)
            {              

                string portName = configManager.ReadConfig("PortName");
                int portBaud = int.Parse(configManager.ReadConfig("BaudRate"));
                string strParity = configManager.ReadConfig("Parity");
                string strStopBits = configManager.ReadConfig("StopBits");
                int strDataBits = int.Parse(configManager.ReadConfig("DataBits"));

                if (strParity == "Even")
                {
                    portparam0.m_parity = Parity.Even;
                }
                else if (strParity == "Odd")
                {
                    portparam0.m_parity = Parity.Odd;
                }
                else if (strParity == "None")
                {
                    portparam0.m_parity = Parity.None;
                }

                if (portName != "0")
                {
                    portparam0.m_portName = portName;
                }

                if (portBaud != 0)
                {
                    portparam0.m_baudRate = portBaud;
                }


                if (strStopBits == "1")
                {

                    portparam0.m_stopbits = StopBits.One;
                }
                else
                    if (strStopBits == "1.5")
                {
                    portparam0.m_stopbits = StopBits.OnePointFive;
                }
                else
                        if (strStopBits == "2")
                {
                    portparam0.m_stopbits = StopBits.Two;
                }

                if (strDataBits != 0)
                {
                    portparam0.m_dataBits = strDataBits;
                }

                portparam0.m_readBufferSize = 1024;
                portparam0.m_receivedBytesThreshold = 1;
                portparam0.m_receiveTimeout = 500;
                portparam0.m_sendInterval = 100;
                configManager = null;
                //GC.Collect();
                //GC.WaitForPendingFinalizers();

            }
            return portparam0;
        }
        public static void AppExit()
        {
            System.Environment.Exit(0);
        }
        public static int IsWhichFile(string filePath)
        {
            string extension = filePath.Substring(filePath.LastIndexOf(".")).ToString().ToLower();
            //判断是否是excel文件
            if (extension == ".xlsx" || extension == ".xls")
                return Constant.ExcelFile;

            if (extension == ".csv" )
                return Constant.CsvFile;

            return Constant.ErrorFile;
        }


        /// <summary>
        /// 返回指示文件是否已被其它程序使用的布尔值
        /// </summary>
        /// <param name="fileFullName">文件的完全限定名，例如：“C:\MyFile.txt”。</param>
        /// <returns>如果文件已被其它程序使用，则为 true；否则为 false。</returns>
        public static Boolean FileIsUsed(String fileFullName)
        {
            Boolean result = false;
            //判断文件是否存在，如果不存在，直接返回 false
            if (!System.IO.File.Exists(fileFullName))
            {
                result = false;
            }//end: 如果文件不存在的处理逻辑
            else
            {//如果文件存在，则继续判断文件是否已被其它程序使用
             //逻辑：尝试执行打开文件的操作，如果文件已经被其它程序使用，则打开失败，抛出异常，根据此类异常可以判断文件是否已被其它程序使用。
                System.IO.FileStream fileStream = null;
                try
                {
                    fileStream = System.IO.File.Open(fileFullName, System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite, System.IO.FileShare.None);
                    result = false;
                }
                catch (System.IO.IOException )
                {
                    result = true;
                }
                catch (System.Exception)
                {
                    result = true;
                }
                finally
                {
                    if (fileStream != null)
                    {
                        fileStream.Close();
                    }
                }
            }//end: 如果文件存在的处理逻辑
             //返回指示文件是否已被其它程序使用的值
            return result;
        }//end method FileIsUsed

        public static bool SplitAreaAndAddrTcp(string userdata, ref int addr, ref string area)
        {
            string strAddr;
            //取数字 用替代的字符的方法 取数组就替换字母为空 取字母就替换数字
            strAddr = Regex.Replace(userdata, "[A-Z]", "", RegexOptions.IgnoreCase);

            //取字母
            area = Regex.Replace(userdata, "[0-9]", "", RegexOptions.IgnoreCase);

            
            if (strAddr.Contains("."))
            {
                int z = 0;
                int x = 0;
                area = area.Replace(".", "");
                string[] value = strAddr.Split('.');
                if (value.Count() == 2)
                {
                    if (int.TryParse(value[0], out z) && int.TryParse(value[1], out x))
                    {
                        addr = z * 8 + x;
                    }
                }
            }
            else
            {                         
               if (!int.TryParse(strAddr, out addr)) return false;              
            }

            if (strAddr == null || area == null)
            {
                return false;
            };
            //地址超了 无效 暂且定XDM 最大69999
            if (!int.TryParse(strAddr, out addr) || (addr < 0) || (addr > Constant.XJMaxAddr))
            {
                return false;
            }



            return true;
        }
        public static bool IsReadBit(byte[] cmd)
        {
            List<byte> cmdLst = new List<byte>();
            cmdLst.AddRange(cmd);
            if (cmd.Count() > 8)
            {
                return ConstantMethod.compareByteStrictly(cmdLst.Skip(7).Take(2).ToArray(), Constant.DTTcpFunctionReadBitCmd);
            }

            return false;
        }
        public static bool IsReadByte(byte[] cmd)
        {
            List<byte> cmdLst = new List<byte>();
            cmdLst.AddRange(cmd);
            if (cmd.Count() > 8)
            {
                return ConstantMethod.compareByteStrictly(cmdLst.Skip(7).Take(2).ToArray(), Constant.DTTcpFunctionReadByteCmd);
            }

            return false;
        }
        public static bool SplitAreaAndAddr(string userdata,ref int addr,ref string area)
        {
            string strAddr;
            //取数字 用替代的字符的方法 取数组就替换字母为空 取字母就替换数字
            strAddr = Regex.Replace(userdata, "[A-Z]", "", RegexOptions.IgnoreCase);

            //取字母
            area = Regex.Replace(userdata, "[0-9]", "", RegexOptions.IgnoreCase);


            if (strAddr == null || area == null)
            {
                return false;
            };
            //地址超了 无效 暂且定XDM 最大69999
            if (!int.TryParse(strAddr, out addr) || (addr < 0) || (addr > Constant.XJMaxAddr))
            {
                return false;
            }

           

            return true;
        }
        public static int max(int a, int b)
        {
            if (a >= b) return a;
            else
                return b;
            //return a>b?a:b;
        }
        //数组比较 指定几个前面几个数据
        public static bool compareByteStrictly(byte[] b1, byte[] b2,int cnt)
        {

            int b1cnt = b1.Count();
            int b2cnt = b2.Count();
            if (cnt > b1cnt || cnt > b2cnt) return false;
            if (b1cnt != b2cnt) return false;
            //  int j = b1cnt > b2cnt ? b2cnt : b1cnt;
            int j = cnt;
            if (j < 1) return false;
            for (int i = 0; i < j; i++)
            {
                if (b1[i] != b2[i])
                { return false; }
            }
            return true;

        }
        //比较两个字节数据 是否相等 以最小的字节数组为基础 局部相等 则返回true
        public static bool compareByteStrictly(byte[] b1, byte[] b2)
        {

            int b1cnt = b1.Count();
            int b2cnt = b2.Count();
            if (b1cnt != b2cnt) return false;
            int j = b1cnt > b2cnt ? b2cnt : b1cnt;
            if (j < 1) return false;
            for (int i = 0; i < j; i++)
            {
                if (b1[i] != b2[i])
                { return false; }
            }
            return true;

        }
        public static byte[] StrToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2]; 
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }
  
        public static string GetLocalIP()
        {
            try
            {
                string HostName = Dns.GetHostName(); //得到主机名
                IPHostEntry IpEntry = Dns.GetHostEntry(HostName);
                for (int i = 0; i < IpEntry.AddressList.Length; i++)
                {
                    //从IP地址列表中筛选出IPv4类型的IP地址
                    //AddressFamily.InterNetwork表示此IP为IPv4,
                    //AddressFamily.InterNetworkV6表示此地址为IPv6类型
                    if (IpEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    {
                        return IpEntry.AddressList[i].ToString();
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                MessageBox.Show("获取本机IP出错:" + ex.Message);
                return "";
            }
        }

        public static Boolean KillProcess(string proName)
        {
            
            Process[] p = Process.GetProcessesByName(proName);
            if (p.Count() > 0)
            {
                foreach (Process p1 in p)
                {
                    p1.Kill();
                    //MessageBox.Show("EXCEL进程关闭成功！");

                }
                return true;
            }
            else return false;

        }
        //判断程序是否在执行
        public static Boolean IsRuning()
        {
            Process current = Process.GetCurrentProcess();
            Process[] processes = Process.GetProcessesByName(current.ProcessName);
            foreach (Process process in processes)
            {
                if (process.Id != current.Id)
                {
                    if (process.MainModule.FileName
                    == current.MainModule.FileName)
                    {
                        MessageBox.Show(current.ProcessName + "程序已经运行！", Application.ProductName,
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return true;
                    }
                }
            }
            return false;
        }
        //字节数组转十六进制字符
        public static string byteToHexStr(byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }
            return returnStr;
        }
       
        public static void Delay(int milliSecond)
        {
            int start = Environment.TickCount;
            while (Math.Abs(Environment.TickCount - start) < milliSecond)
            {
                Application.DoEvents();
            }
        }
        //带引用传递的延时 可提前退出循环  这个用于设备是否存在 从false 变到true
        public static void Delay(int milliSecond,ref bool jump)
        {
            int start = Environment.TickCount;
            while ((Math.Abs(Environment.TickCount - start) < milliSecond)&&(!jump))
            {               
                Application.DoEvents();
            }
        }
        //带引用传递的延时 可提前退出循环  这个用于设备发送命令是否成功 从true 变到false
        public static void DelayWriteCmd(int milliSecond, ref bool jump)
        {
            int start = Environment.TickCount;
            while ((Math.Abs(Environment.TickCount - start) < milliSecond) && (jump))
            {
                Application.DoEvents();
            }
        }
        public static void DelayWriteCmdOk(int milliSecond, ref int valueOld, ref PlcInfoSimple p,ref PlcInfoSimple emg)
        {
            int start = Environment.TickCount;

            while ((Math.Abs(Environment.TickCount - start) < milliSecond))
            {
                if(emg.ShowValue>0)
                {
                    break;
                }
                if (valueOld == p.ShowValue)
                {
                 
                    break;
                }
                Application.DoEvents();
            }
        }

        //写数据的时候判断下 不等 或者 超时退出
        public static void DelayWriteCmdOk(int milliSecond,ref int valueOld, ref PlcInfoSimple p)
        {
            int start = Environment.TickCount;

            while ((Math.Abs(Environment.TickCount - start) < milliSecond))
            {
                if (valueOld  ==  p.ShowValue )
                {
                    break;
                }
                Application.DoEvents();
            }
        }
        //写数据的时候判断下 不等 或者 超时退出
        public static void DelayWriteCmdOk(int milliSecond, ref int valueOld, ref PlcInfoSimple p,ref bool  IsWriteSuccess)
        {
            int start = Environment.TickCount;
            IsWriteSuccess = false;
            while ((Math.Abs(Environment.TickCount - start) < milliSecond))
            {
                if (valueOld == p.ShowValue)
                {
                    IsWriteSuccess = true;
                    break;
                }
                Application.DoEvents();
            }          
        }
        public static void DelayWriteCmdOk(int milliSecond, ref int valueOld, ref DTPlcInfoSimple p)
        {
            int start = Environment.TickCount;

            while ((Math.Abs(Environment.TickCount - start) < milliSecond))
            {
                if (valueOld == p.ShowValue)
                {
                    break;
                }
                Application.DoEvents();
            }
        }
        public static void DelayMeasure(int milliSecond, ref int valueOld, ref DTPlcInfoSimple p, ref DTPlcInfoSimple emg, ref bool jump)
        {
            int start = Environment.TickCount;

            while ((Math.Abs(Environment.TickCount - start) < milliSecond))
            {
                if (valueOld == p.ShowValue || (emg.ShowValue == 1) || !jump)  //测量好 或者 急停 都退出
                {
                    break;
                }
                Application.DoEvents();
            }
        }

        public static void DelayMeasure(int milliSecond, ref int valueOld, ref PlcInfoSimple p, ref PlcInfoSimple emg,ref bool jump)
        {
            int start = Environment.TickCount;

            while ((Math.Abs(Environment.TickCount - start) < milliSecond))
            {
                if (valueOld == p.ShowValue || (emg.ShowValue==1) || !jump)  //测量好 或者 急停 都退出
                {
                    break;
                }
                Application.DoEvents();
            }
        }
        public Boolean  SearchFile(string path,string file)
        {
            return true;
        }
        //返回字符串 输入整形数值 除以100
        public static string TwoPoint(int cnt)
        {
            string value = "0.00";
            double d = (double)cnt / 100;
            value = d.ToString("f2");
            return value;
        }

        public static int getBitValueInByte(int pos,byte byte1)
        {
            int i = 0;
           
            switch (pos)
            {

                case 1:
                    {
                        i=(byte1 & 0x01) == 0x01 ? 1 : 0;
                        break;
                    }
                case 2:
                    {
                        i = (byte1 & 0x02) == 0x02 ? 1 : 0;
                        break;
                    }
                case 3:
                    {
                        i = (byte1 & 0x04) == 0x04 ? 1 : 0;
                        break;
                    }
                case 4:
                    {
                        i = (byte1 & 0x08) == 0x08 ? 1 : 0;
                        break;
                    }
                case 5:
                    {
                        i = (byte1 & 0x10) == 0x10 ? 1 : 0;
                        break;
                    }
                case 6:
                    {
                        i = (byte1 & 0x20) == 0x20 ? 1 : 0;
                        break;
                    }
                case 7:
                    {
                        i = (byte1 & 0x40) == 0x40 ? 1 : 0;
                        break;
                    }
                case 8:
                    {
                        i = (byte1 & 0x80) == 0x80 ? 1 : 0;
                        break;
                    }
            }

            return i;
            
        }
        public static int Pack4BytesToInt(int Low, int High)
        {
            int valueLow = Low;
            int valueHigh = High << 16;

            int valueLong = valueHigh | valueLow;
            return valueLong;
        }
        //crc 校验
        public static byte[] CRC16_C(byte[] data)
        {
            byte CRC16Lo;
            byte CRC16Hi;   //CRC寄存器 
            byte CL; byte CH;       //多项式码&HA001 
            byte SaveHi; byte SaveLo;
            byte[] tmpData;

            int Flag;
            CRC16Lo = 0xFF;
            CRC16Hi = 0xFF;
            CL = 0x01;
            CH = 0xA0;
            tmpData = data;
            for (int i = 0; i < tmpData.Length; i++)
            {
                CRC16Lo = (byte)(CRC16Lo ^ tmpData[i]); //每一个数据与CRC寄存器进行异或 
                for (Flag = 0; Flag <= 7; Flag++)
                {
                    SaveHi = CRC16Hi;
                    SaveLo = CRC16Lo;
                    CRC16Hi = (byte)(CRC16Hi >> 1);      //高位右移一位 
                    CRC16Lo = (byte)(CRC16Lo >> 1);      //低位右移一位 
                    if ((SaveHi & 0x01) == 0x01) //如果高位字节最后一位为1 
                    {
                        CRC16Lo = (byte)(CRC16Lo | 0x80);   //则低位字节右移后前面补1 
                    }             //否则自动补0 
                    if ((SaveLo & 0x01) == 0x01) //如果LSB为1，则与多项式码进行异或 
                    {
                        CRC16Hi = (byte)(CRC16Hi ^ CH);
                        CRC16Lo = (byte)(CRC16Lo ^ CL);
                    }
                }
            }
            byte[] ReturnData = new byte[2];
            ReturnData[0] =CRC16Lo;       //CRC高位 
            ReturnData[1] = CRC16Hi;       //CRC低位 
            return ReturnData;

        }
        public static byte[] DeltaByteToCmd(byte[] cmd)
        {
            List<byte> bLst = new List<byte>();
            bLst.Add((byte)Constant.DTHeader);

            bLst.AddRange(cmd);

            string str = System.Text.Encoding.Default.GetString(cmd);

            return bLst.ToArray();

        }
        /// <summary>
        /// 台达数据处理 串口缓冲区 
        /// </summary>
        /// <param name="sLst"></param>
        /// <returns></returns>
        public static List<byte> DeltaBufferPro(List<byte> sLst)
        {
            List<byte> bLst = new List<byte>();
            sLst.RemoveAt(0);
            string str = System.Text.Encoding.Default.GetString(sLst.ToArray()).Trim();
            byte[] byteArray = ConstantMethod.StrToHexByte(str);
            bLst.AddRange(byteArray);
            return bLst;

        }

        public static string getCharacter(string s )
        {
            return Regex.Match(s, @"[a-zA-Z]+").ToString();
        }

        public static string getNumber(string s)
        {
            return Regex.Match(s, @"\d+").ToString();
        }
        //台达上层数据转换为底层数据
        public static byte[] DeltaCmdPro(byte[] sLst)
        {
            string str = ConstantMethod.byteToHexStr(sLst);

            List<byte> bLst = new List<byte>();
            bLst.Add(Constant.DTHeader);

            byte[] byteArray = System.Text.Encoding.ASCII.GetBytes(str);
            bLst.AddRange(byteArray);

            bLst.AddRange(Constant.DTEnd.ToArray());
            
            return bLst.ToArray();

        }
        //台达底层数据转换为上层数据
        public static byte[] DeltaBufferPro(byte[] sLst)
        {
            List<byte> bLst = new List<byte>();
            bLst.AddRange(sLst);
            bLst = ConstantMethod.DeltaBufferPro(bLst);
            return bLst.ToArray();

        }

    }
}
