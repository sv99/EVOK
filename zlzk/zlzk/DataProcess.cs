
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

namespace zlzk
{
    class DataProcess
    {
        public static void ShowInfo(RichTextBox r1, string s)
        {

            r1.Invoke((EventHandler)(delegate
            {
                r1.AppendText(s);
            }));

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
