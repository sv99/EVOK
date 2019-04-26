using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xjplc.TcpDevice
{
    public class ZdDevice : TcpDevice
    {
        public byte[] cmdZDUsualOut = { 0xb0, 0x00, 0x00, 0x00,  0x33, 0x00, 0x0b };


        #region 终端property
        bool resetOk = false;
        int carQxValue = 0;


        public int CarQxValue
        {
            get { return carQxValue; }
            set { carQxValue = value; }
        }
        int carQxDir = 0;
        public int CarQxDir
        {
            get { return carQxDir; }
            set { carQxDir = value; }
        }
        int carFgDir = 0;
        public int CarFgDir
        {
            get { return carFgDir; }
            set { carFgDir = value; }
        }
        int carFgValue = 0;
        public int CarFgValue
        {
            get { return carFgValue; }
            set { carFgValue = value; }
        }
        int speed = 5;
        int cw=0;
        public int Cw
        {
            get { return cw; }
            set { cw = value; }
        }
        public int Speed
        {
            get { return speed; }
            set { speed = value; }
        }
        public void SetLightCTRL(int farLight,int nearLight)
        {
            if (farLight > 10) farLight = 3;
            if (nearLight > 10) nearLight = 3;

            byte dirByte = (byte)(farLight << 4 | nearLight);

            cmdZDUsualOut[4] = dirByte;
         
            /***
            if (farLight > 10) farLight = 3;
            if (nearLight > 10) nearLight = 3;
            byte[] cmdOutTemp = cmdZDUsualOut.ToArray();
            byte dirByte = (byte)(farLight << 4  | nearLight);

            cmdOutTemp[4] = dirByte;
             
            CmdOutLst.Add(cmdOutTemp.ToList<byte>());
            CmdInLst.Add(cmdOutTemp.ToList<byte>());
            ***/

        }

        int rotateQxValue = 0;
        public int RotateQxValue
        {
            get { return rotateQxValue; }
            set { rotateQxValue = value; }
        }
        int rotateFgValue = 0;
        public int RotateFgValue
        {
            get { return rotateFgValue; }
            set { rotateFgValue = value; }
        }

        int currentQxValue;
        public int CurrentQxValue
        {
            get { return currentQxValue; }
            set { currentQxValue = value; }
        }
        int currentFgValue;
        public int CurrentFgValue
        {
            get { return currentFgValue; }
            set { currentFgValue = value; }
        }

        public void SetCameraCTRL(int dir,int resetEn,int cwEn)
        {

            byte[] cmdOutTemp = cmdZDUsualOut.ToArray();
            switch (dir)
            {
                case 0:
                    {

                        byte dirByte = (byte)(0x00 | cwEn<<3 | resetEn<<4);
                        cmdOutTemp[3] = dirByte;

                        break;
                    }
                case 1:
                    {
                        byte dirByte = (byte)(0x20 | cwEn << 3 | resetEn << 4);
                        cmdOutTemp[3] = dirByte;
                        break;
                    }
                case 2:
                    {

                        byte dirByte = (byte)(0x40 | cwEn <<3 | resetEn << 4);
                        cmdOutTemp[3] = dirByte;
                        
                        break;
                    }
                case 3:
                    {
                        byte dirByte = (byte)(0x60 | cwEn << 3 | resetEn << 4);
                        cmdOutTemp[3] = dirByte;

                        break;
                    }
                case 4:
                    {
                        byte dirByte = (byte)(0x80 | cwEn << 3 | resetEn << 4);
                        cmdOutTemp[3] = dirByte;
                        break;
                    }
                default:
                    {
                        byte dirByte = (byte)(0x00 | cwEn << 3 | resetEn << 4);
                        cmdOutTemp[3] = dirByte;
                        break;
                    }
            }

            CmdOutLst.Add(cmdOutTemp.ToList<byte>());
            CmdInLst.Add(cmdOutTemp.ToList<byte>());

        }

        bool camqian = false;
        bool camhou = false;
        bool camzuo = false;
        bool camyou = false;
        bool camrst = false;
        public void stopCam()
        {
            camqian = false;
            camhou = false;
            camzuo = false;
            camyou = false;
            camrst = false;
            CmdOutLst.Clear();
            CmdInLst.Clear();
        }
        public void CamerReset(int cwEn)
        {
            cwEn = cw;
            byte[] cmdOutTemp = cmdZDUsualOut.ToArray();
            byte dirByte = (byte)(0x10 | cwEn << 3);
            cmdOutTemp[3] = dirByte;

            camrst = true;

            while (camrst)
            {
                CmdOutLst.Add(cmdOutTemp.ToList<byte>());
                CmdInLst.Add(cmdOutTemp.ToList<byte>());
                ConstantMethod.Delay(10);
                if (resetOk)
                {
                    stopCam();
                    break;
                }
            }
        }
        public void CamerQian(int cwEn)
        {
            byte[] cmdOutTemp = cmdZDUsualOut.ToArray();
            byte dirByte = (byte)(0x20 | cwEn << 3 );
            cmdOutTemp[3] = dirByte;

            camqian = true;

            while (camqian)
            {
                CmdOutLst.Add(cmdOutTemp.ToList<byte>());
                CmdInLst.Add(cmdOutTemp.ToList<byte>());
                ConstantMethod.Delay(10);
            }

        }

       
        public void CamerHou(int cwEn)
        {
            cwEn = Cw;
            byte[] cmdOutTemp = cmdZDUsualOut.ToArray();
            byte dirByte = (byte)(0x40 | cwEn << 3);
            cmdOutTemp[3] = dirByte;
            
            camhou = true;

            while (camhou)
            {
                CmdOutLst.Add(cmdOutTemp.ToList<byte>());
                CmdInLst.Add(cmdOutTemp.ToList<byte>());
                ConstantMethod.Delay(10);
            }

        }
        public void CamerZuo(int cwEn)
        {
            byte[] cmdOutTemp = cmdZDUsualOut.ToArray();
            byte dirByte = (byte)(0x60 | cwEn << 3);
            cmdOutTemp[3] = dirByte;

            camzuo = true;

            while (camzuo)
            {
                CmdOutLst.Add(cmdOutTemp.ToList<byte>());
                CmdInLst.Add(cmdOutTemp.ToList<byte>());
                ConstantMethod.Delay(10);
            }

        }
        public void CamerYou(int cwEn)
        {
            byte[] cmdOutTemp = cmdZDUsualOut.ToArray();
            byte dirByte = (byte)(0x80 | cwEn << 3);
            cmdOutTemp[3] = dirByte;

            camyou = true;

            while (camyou)
            {
                CmdOutLst.Add(cmdOutTemp.ToList<byte>());
                CmdInLst.Add(cmdOutTemp.ToList<byte>());
                ConstantMethod.Delay(10);
            }

        }
        public void SetMotion(int dir, int speed)
        {
            byte[] cmdOutTemp = cmdZDUsualOut.ToArray();

            if (speed < 10)
                switch (dir)
                {
                    case 0:  //停止
                        {
                            byte dirByte = (byte)(0x00 | speed);
                            cmdOutTemp[2] = dirByte;

                            break;
                        }
                    case 1://前进
                        {
                            byte dirByte = (byte)(0x20 | speed);
                            cmdOutTemp[2] = dirByte;
                            break;
                        }
                    case 2://后退
                        {
                            byte dirByte = (byte)(0x40 | speed);
                            cmdOutTemp[2] = dirByte;

                            break;
                        }
                    case 3://左转
                        {
                            byte dirByte = (byte)(0x60 | speed);
                            cmdOutTemp[2] = dirByte;
                            break;
                        }
                    case 4://右转
                        {

                            byte dirByte = (byte)(0x80 | speed);
                            cmdOutTemp[2] = dirByte;
                            break;
                        }
                    default:
                        {

                            byte dirByte = (byte)(0x00 | speed);
                            cmdOutTemp[2] = dirByte;
                            break;
                        }

                }

            CmdOutLst.Add(cmdOutTemp.ToList<byte>());
            CmdInLst.Add(cmdOutTemp.ToList<byte>());


        }

        bool taigan = false;
        bool jianggan = false;
        public void StartTaiGan()
        {
            byte[] cmdOutTemp = cmdZDUsualOut.ToArray();
            cmdOutTemp[1] = 0x01;
            taigan = true;
            while (taigan)
            {
                CmdOutLst.Add(cmdOutTemp.ToList<byte>());
                CmdInLst.Add(cmdOutTemp.ToList<byte>());
                ConstantMethod.Delay(10);
            }
                                                     
        }

        bool qian = false;
        public void StartQian(int speed)
        {
            byte[] cmdOutTemp = cmdZDUsualOut.ToArray();
            byte dirByte = (byte)(0x20 | speed);
            cmdOutTemp[2] = dirByte;
            qian = true;
            while (qian)
            {
                CmdOutLst.Add(cmdOutTemp.ToList<byte>());
                CmdInLst.Add(cmdOutTemp.ToList<byte>());
                ConstantMethod.Delay(10);
            }

        }
        bool hou = false;
        public void StartHou(int speed)
        {
            byte[] cmdOutTemp = cmdZDUsualOut.ToArray();
            byte dirByte = (byte)(0x40 | speed);
            cmdOutTemp[2] = dirByte;
            hou = true;
            while (hou)
            {
                CmdOutLst.Add(cmdOutTemp.ToList<byte>());
                CmdInLst.Add(cmdOutTemp.ToList<byte>());
                ConstantMethod.Delay(10);
            }

        }
        bool zuo = false;
        public void StartZuo(int speed)
        {
            byte[] cmdOutTemp = cmdZDUsualOut.ToArray();
            byte dirByte = (byte)(0x60 | speed);
            cmdOutTemp[2] = dirByte;
            zuo = true;
            while (zuo)
            {
                CmdOutLst.Add(cmdOutTemp.ToList<byte>());
                CmdInLst.Add(cmdOutTemp.ToList<byte>());
                ConstantMethod.Delay(10);
            }

        }

        bool you = false;
        public void StartYou(int speed)
        {
            byte[] cmdOutTemp = cmdZDUsualOut.ToArray();
            byte dirByte = (byte)(0x80 | speed);
            cmdOutTemp[2] = dirByte;
            you = true;
            while (you)
            {
                CmdOutLst.Add(cmdOutTemp.ToList<byte>());
                CmdInLst.Add(cmdOutTemp.ToList<byte>());
                ConstantMethod.Delay(10);
            }

        }
        bool GetLineHouTui = false;

        public void ZdGetLineHouTui()
        {
            byte[] cmdOutTemp = cmdZDUsualOut.ToArray();
            byte dirByte = (byte)(0x10 | 8);
            cmdOutTemp[2] = dirByte;
            GetLineHouTui = true;
            while (GetLineHouTui)
            {
                CmdOutLst.Add(cmdOutTemp.ToList<byte>());
                CmdInLst.Add(cmdOutTemp.ToList<byte>());
                ConstantMethod.Delay(10);
            }
            CmdOutLst.Clear();
            CmdInLst.Clear();

        }


        public void stopZd()
        {
      
            qian = false;
            hou = false;
            zuo = false;
            you = false;
            GetLineHouTui = false;
            CmdOutLst.Clear();
            CmdInLst.Clear();
        }
        public void StartJiangGan()
        {
            byte[] cmdOutTemp = cmdZDUsualOut.ToArray();
            cmdOutTemp[1] = 0x02;
            jianggan = true;
            while (jianggan)
            {
                CmdOutLst.Add(cmdOutTemp.ToList<byte>());
                CmdInLst.Add(cmdOutTemp.ToList<byte>());
                ConstantMethod.Delay(10);
            }

        }

        public void StopGan()
        {
            taigan = false;
            jianggan = false;
            CmdOutLst.Clear();
            CmdInLst.Clear();

        }
        public void SetTaiGanStatus(int id)
        {
            byte[] cmdOutTemp = cmdZDUsualOut.ToArray();
           
            switch (id)
            {
                case 0:
                    {
                        cmdOutTemp[1] = 0x00;
                        break;
                    }
                case 1:
                    {

                        cmdOutTemp[1] = 0x01;
                        break;
                    }
                case 2:
                    {

                        cmdOutTemp[1] = 0x10;
                        break;
                    }
                default :
                    {
                        cmdOutTemp[1] = 0x00;
                        break;
                    }
            }


            CmdOutLst.Add(cmdOutTemp.ToList<byte>());
            CmdInLst.Add(cmdOutTemp.ToList<byte>());


        }
        #endregion
        public void OpenCw()
        {
            Cw = 1;
            CmdOutDefault[3]=0x08;
        }
        public void CloseCw()
        {
            Cw = 0;
            CmdOutDefault[3] = 0x00;
        }
        public ZdDevice(ServerInfo p0) :base(p0)
        {
            EventDataProcess += new xjplc.socDataProcess(ZDDataProcess);

            CmdOut = cmdZDUsualOut;
                 
            CmdOutDefault = cmdZDUsualOut;
            CmdInDefault = cmdZDUsualOut;
            //OpenTcpClient();
        }

        int getValueByDir(int olddir, int oldangle)
        {
            if (olddir == 1)
            {
                oldangle = (127-oldangle) * (-1);
            }

            return oldangle;
        }
        int getRotateValue(int olddir,int oldangle,int newdir,int newangle)
        {           

            return getValueByDir(newdir, newangle) - getValueByDir(olddir,oldangle); 

        }
        void ZDDataProcess(object sender, SocEventArgs e)
        {
            if (DeviceId == Constant.devicePropertyB)
            {
                if (e.Byte_buffer.Length == 5 && e.Byte_buffer[0] == 0xb0 && e.Byte_buffer[4] == 0x0b)
                {
                    int oldQxValue = CarQxValue;
                    int oldQxDir = CarQxDir;


                    resetOk = ConstantMethod.getBitValueInByte(8, e.Byte_buffer[1]) == 1 ? true : false;

                    #region 计算倾斜
                    //老的计算方式
                    CarQxDir = ConstantMethod.getBitValueInByte(8, e.Byte_buffer[2]);
                    CarQxValue = e.Byte_buffer[2] & 0x7F;

                    //新计算方式

                    int oldCurrentQxValue = CurrentQxValue;


                    CurrentQxValue = getValueByDir(CarQxDir, CarQxValue);// (int)e.Byte_buffer[2];

                    // if(RotateQxValue ==0)
                    RotateQxValue = CurrentQxValue-oldCurrentQxValue;// getRotateValue(oldQxDir, oldQxValue, CarQxDir, CarQxValue);

                    #endregion
                    #region 翻滚角度
                    //老的计算方式
                    int oldFgValue = CarFgValue;
                    int oldFgDir = CarFgDir;
                    CarFgDir = ConstantMethod.getBitValueInByte(8, e.Byte_buffer[3]);
                    CarFgValue = e.Byte_buffer[3] & 0x7F;



                    //新计算方式
                    int oldCurrentFgValue = CurrentFgValue;
                    CurrentFgValue = getValueByDir(CarFgDir, CarFgValue); ;// (int)e.Byte_buffer[3];
                    //if (RotateFgValue == 0)
                    RotateFgValue = CurrentFgValue-oldCurrentFgValue ;// getRotateValue(oldFgDir, oldFgValue, CarFgDir, CarFgValue);
                   #endregion
                }
            }
        }

    }
}
