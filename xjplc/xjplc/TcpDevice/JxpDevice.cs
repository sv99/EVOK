using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xjplc.TcpDevice
{
    public class JxpDevice : TcpDevice
    {
        public byte[] cmdJXPUsualOut = { 0xc0 ,0x02, 0x00, 0x0c };

        #region 卷线盘property
        double encoderData = 0;
        public double EncoderData
        {
            get { return encoderData; }
            set { encoderData = value; }
        }


        //复位
        bool resetOk = false;
        public bool ResetOk
        {
            get { return resetOk; }
            set { resetOk = value; }
        }

        //轮径识别
        bool tyreCheck  = false;
        public bool TyreCheck
        {
            get { return tyreCheck; }
            set { tyreCheck = value; }
        }

        //轮径
        int tyreSize = 0;


        //方向
        int dir = 0;
        public int Dir
        {
            get { return dir; }
            set { dir = value; }
        }

        #endregion
        public JxpDevice(ServerInfo p0) : base(p0)
        {

            EventDataProcess += new xjplc.socDataProcess(JxpDataProcess);
            CmdOut= cmdJXPUsualOut;
            CmdOutReadyGo = cmdJXPUsualOut;
            CmdInReadyCome = cmdJXPUsualOut;                      
                                                          
        }
        #region 动作操作
        public void SetLineGetLevel(int level)
        {
            byte val = (byte)(level<<2);
            val =(byte)( val | 0x02);
            byte[] cmdOutTemp = cmdJXPUsualOut.ToArray();

            cmdOutTemp[1] = val;
            CmdOutLst.Add(cmdOutTemp.ToList<byte>());
            CmdInLst.Add(cmdOutTemp.ToList<byte>());
        }

        //清除计数器     
        public void ClrMeter()
        {
            if (!Status) return;
            byte[] cmdOutTemp = CmdOutReadyGo.ToArray();

            cmdOutTemp[1] = 0x82;

            CmdOutLst.Add(cmdOutTemp.ToList<byte>());
            CmdInLst.Add(cmdOutTemp.ToList<byte>());

            CmdOutReadyGo = cmdOutTemp;

            ConstantMethod.Delay(200);

            CmdOutReadyGo = cmdJXPUsualOut;

        }
        //收线
        public void JxpGetLine()
        {
            if (IsGetingLine)
            {
                PackStopGetLineCmd();
            }
            else
            {
                PackGetLineCmd();
            }
        }


        public bool IsGetingLine = false;
        public void PackGetLineCmd()
        {
            IsGetingLine = true;
            byte[] cmdOutTemp = CmdOutReadyGo.ToArray();
            cmdOutTemp[1] = 0x42;
            CmdOutReadyGo = cmdOutTemp;                     
        }

        public void PackStopGetLineCmd()
        {
            IsGetingLine = false;
            CmdOutLst.Clear();
            CmdInLst.Clear();
            CmdOutReadyGo = cmdJXPUsualOut;

        }

        #endregion
        void JxpDataProcess(object sender, SocEventArgs e)
        {
            if (DeviceId == Constant.devicePropertyA)
            {
                int codeH = 0;
                int codeL = 0;

                if (e.Byte_buffer.Length == 8 && e.Byte_buffer[0]==0xa0 && e.Byte_buffer[7]==0x0a)
                {
                    //符号 字节
                    Dir = e.Byte_buffer[1];

                    //编码器数据
                    byte[] codeHbyte = {  e.Byte_buffer[3],e.Byte_buffer[2] };

                    codeH = BitConverter.ToInt16(codeHbyte,0);

                    byte[] codeLbyte = { e.Byte_buffer[5] ,e.Byte_buffer[4] };

                    codeL = BitConverter.ToInt16(codeLbyte, 0);

                    string dirStr = "";
                    if (Dir > 0) dirStr = "-";
                    string sCode = dirStr+codeH.ToString() + "." + codeL.ToString();
                    double enc = 0;
                    if (double.TryParse(sCode, out enc))
                    {
                        EncoderData =enc;
                    }

                    //第7字节
                    ResetOk = ConstantMethod.getBitValueInByte(8, e.Byte_buffer[6])==1?true:false;

                    TyreCheck = ConstantMethod.getBitValueInByte(7, e.Byte_buffer[6]) == 1 ? true : false;

                    tyreSize = (e.Byte_buffer[6] & 0x38);


                }
            }


        }
    }


    
}
