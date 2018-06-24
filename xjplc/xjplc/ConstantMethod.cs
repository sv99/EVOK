
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

namespace xjplc
{
    public class ConstantMethod
    {
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
                            string param3= dt.Rows[i]["param3"].ToString();
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
        public static bool XJFindPort() //如果可以连接 则写到config里
        {
            SerialPort m_serialPort = new SerialPort();

            string[] str = SerialPort.GetPortNames();

            List<string> portNameLst = new List<string>();

            PortParam portparam0 = ConstantMethod.LoadPortParam(Constant.ConfigFilePath);

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
                        ConstantMethod.SetPortParam(Constant.ConfigFilePath, Constant.PortName, m_serialPort.PortName);
                        return true ;
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
        #region 台达PLC 
        /// <summary>
        /// 这里检测台达PLC 是否存在 并发送 可以设置读取命令的数据
        /// </summary>
        /// <returns></returns>
        public static bool DTFindPort() //如果可以连接 则写到config里
        {
            SerialPort m_serialPort = new SerialPort();

            string[] str = SerialPort.GetPortNames();

            List<string> portNameLst = new List<string>();

            PortParam portparam0 = ConstantMethod.LoadPortParam(Constant.ConfigFilePath);

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
                    byte[] resultByte = new byte[Constant.DTExistByteOutIn.Count()];

                    m_serialPort.Write(Constant.DTExistByteOutIn, 0, Constant.DTExistByteOutIn.Length);

                    ConstantMethod.Delay(200);

                    m_serialPort.Read(resultByte, 0, Constant.DTExistByteOutIn.Count());


                    if (ConstantMethod.compareByteStrictly(resultByte, Constant.DTExistByteOutIn))
                    {
                        //rtbResult.AppendText("连接成功" + m_serialPort.PortName);
                        ConstantMethod.SetPortParam(Constant.ConfigFilePath, Constant.PortName, m_serialPort.PortName);
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
        public static void ShowInfo(RichTextBox r1, string s)
        {
            if (r1 != null && r1.IsHandleCreated )
            {
                r1.Invoke((EventHandler)(delegate
                {
                    r1.AppendText(s + "\n");
                }));
            }

        }
      
       public static void Optimize(int n, int[] w, int[] v, int[] x, int C)
      {

            int[,] V = new int[n+1,C+1];//前i个物品装入容量为j的背包中获得的最大价值
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
           for (i = n - 1;i > 0;i--)
			{
    
                if (V[i,j] > V[i - 1,j])
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
            if(int.TryParse(strAddr,out addr))
            {
                return addr;
            }
            return 0;
          
        }
        //比较两个字节数据 是否相等 以最小的字节数组为基础 局部相等 则返回true
        public static  bool compareByte(byte[] b1, byte[] b2)
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
                if (!b1[i] .Equals( b2[i]))
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
        public static bool SetPortParam(string filepath,string property,string value)
        {
            ConfigFileManager configManager = new ConfigFileManager();
            if (File.Exists(filepath))
                configManager.LoadFile(filepath);
            else
            {
                MessageBox.Show(Constant.ErrorConfigFile);
                Application.Exit();
                System.Environment.Exit(0);

            }

            configManager.WriteConfig(property, value);

            return true;
        }
        public static PortParam LoadPortParam(string filepath)
        {
            PortParam portparam0 = new PortParam();
            ConfigFileManager configManager = new ConfigFileManager() ;

            if (configManager != null)
            {
                if (File.Exists(filepath))
                    configManager.LoadFile(filepath);
                else
                {
                    MessageBox.Show(Constant.ErrorConfigFile);
                    Application.Exit();
                    System.Environment.Exit(0);
                }

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
            ReturnData[0] = CRC16Hi;       //CRC高位 
            ReturnData[1] = CRC16Lo;       //CRC低位 
            return ReturnData;
        } 
    }
}
