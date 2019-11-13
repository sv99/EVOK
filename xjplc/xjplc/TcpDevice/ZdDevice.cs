using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xjplc.tanuo;

namespace xjplc.TcpDevice
{
    public class ZdDevice : TcpDevice
    {
        public byte[] cmdZDUsualOut = { 0xb0, 0x00, 0x00, 0x00, 0x33, 0x00, 0x0b };

        string configCmdFile = ConstantMethod.GetAppPath() + "config\\configZdCmd.xml";

        List<cmdRobot> cmdLst;

        #region 终端参数
        //复位是否完毕
        bool resetOk = false;
        public bool ResetOk
        {
            get { return resetOk; }
            set { resetOk = value; }
        }
        //小车倾斜角度
        int carQxValue = 0;
        public int CarQxValue
        {
            get { return carQxValue; }
            set { carQxValue = value; }
        }
        //远光灯亮度
        int farlight = 3;
        public int Farlight
        {
            get { return farlight; }
            set { farlight = value; }
        }

        //近光灯亮度
        int nearlight = 3;
        public int Nearlight
        {
            get { return nearlight; }
            set { nearlight = value; }
        }

       //小车倾斜方向
        int carQxDir = 0;
        public int CarQxDir
        {
            get { return carQxDir; }
            set { carQxDir = value; }
        }
        //小车翻滚方向
        int carFgDir = 0;
        public int CarFgDir
        {
            get { return carFgDir; }
            set { carFgDir = value; }
        }
        //小车翻滚值
        int carFgValue = 0;
        public int CarFgValue
        {
            get { return carFgValue; }
            set { carFgValue = value; }
        }

        //小车速度
        int speed = 5;
        public int Speed
        {
            get { return speed; }
            set { speed = value; }
        }


        //是否除雾
        int cwEnable = 0;
        public int CwEnable
        {
            get { return cwEnable; }
            set { cwEnable = value; }
        }

        //旋转倾斜值  
        int rotateQxValue = 0;
        public int RotateQxValue
        {
            get { return rotateQxValue; }
            set { rotateQxValue = value; }
        }

        //旋转翻滚值
        int rotateFgValue = 0;
        public int RotateFgValue
        {
            get { return rotateFgValue; }
            set { rotateFgValue = value; }
        }

        //当前倾斜值
        int currentQxValue;
        public int CurrentQxValue
        {
            get { return currentQxValue; }
            set { currentQxValue = value; }
        }
        //当前翻滚值
        int currentFgValue;
        public int CurrentFgValue
        {
            get { return currentFgValue; }
            set { currentFgValue = value; }
        }

        

        #endregion
       
        #region 参数设置

        //远光灯加1
        public void incFarLight()
        {
            SetLightCTRL(Farlight + 1, Nearlight);
        }

        //近光灯加1
        public void incNearLight()
        {
            SetLightCTRL(Farlight , Nearlight + 1);
        }

        //远光灯减1
        public void decFarLight()
        {
            SetLightCTRL(Farlight -1 , Nearlight);
        }

        //近光灯减1
        public void decNearLight()
        {
            SetLightCTRL(Farlight, Nearlight - 1);
        }

        public void SetSpeed(int spvalue)
        {
            if (Speed <= 10 && Speed > 0)
            {
                Speed = spvalue;

                byte dirByte = ((byte)(CmdOutReadyGo[2] & 0xf0));
                dirByte = (byte)(dirByte | (Speed));
                CmdOutReadyGo[2] = dirByte;

                //取出速度值 给默认的指令
                dirByte = (byte)(dirByte & 0x0f);
                cmdZDUsualOut[2] = dirByte;

               
                
            }
        }
        //单独设置远近光灯设置
        public void SetLightCTRL(int farLight, int nearLight)
        {
            if (farLight > 10 || farLight<1) return;
            if (nearLight > 10 || nearlight <1 ) return;

            byte dirByte = (byte)(farLight << 4 | nearLight);

            CmdOutReadyGo[4] = dirByte;
            cmdZDUsualOut[4] = dirByte;
            this.Farlight = farLight;
            this.Nearlight = nearLight;
                      
        }
        //速度加1
        public void incSpeed()
        {
            byte[] cmdOutTemp = CmdOutReadyGo.ToArray();
            Speed++;
            if (Speed >= 9) Speed = 9;

            byte dirByte = ((byte)(cmdOutTemp[2] & 0xf0));
            dirByte = (byte)(dirByte | (Speed));

            cmdOutTemp[2] = dirByte;
            cmdZDUsualOut[2] = dirByte;
            CmdOutReadyGo = cmdOutTemp;

        }
        //速度减1
        public void decSpeed()
        {
            byte[] cmdOutTemp = CmdOutReadyGo.ToArray();
            Speed--;
            if (Speed <= 0) Speed = 0;

            byte dirByte = ((byte)(cmdOutTemp[2] & 0xf0));
            dirByte = (byte)(dirByte | (Speed));

            cmdOutTemp[2] = dirByte;
            cmdZDUsualOut[2] = dirByte;
            CmdOutReadyGo = cmdOutTemp;

        }
        #endregion
        #region 运动控制
        #region 云台运动

        public bool  CamPTStart(int id)
        {
            if (!Status ) return false;
            switch (id)
            {
                case 1:
                    {
                        if(!IsPTMoving) 
                        PackPTUp();
                        break;
                    }
                case 2:
                    {
                        if (!IsPTMoving)
                            PackPTDown();
                        break;
                    }
                case 3:
                    {
                        if (!IsPTMoving)
                            PackPTLeft();

                        break;
                    }
                case 4:
                    {
                        if (!IsPTMoving)
                            PackPTRight();
                        break;
                    }
                case 5:
                    {
                        if (!IsPTMoving)
                            PTReset();
                        break;
                    }
                default:
                    {
                        if (IsPTMoving)
                            StopPT();
                        break;
                    }

            }
            return true;
        }
       
        public bool IsPTMoving
        {
            get
            {
                return PTUp || PTDown || PTLeft || PTRight || PTInRst;
            }
        }
        //云台前后左右
        bool PTUp = false;
        bool PTDown = false;
        bool PTLeft = false;
        bool PTRight = false;
        bool PTInRst = false;       

        //停止摄像头运动 第4个字节 
        void StopPT()
        {
            PTUp = false;
            PTDown = false;
            PTLeft = false;
            PTRight = false;
            PTInRst = false;

            byte dirByte = CmdOutReadyGo[3];// (byte)(0x20 | CwEnable << 3 );

            dirByte = ConstantMethod.set_bit(dirByte, 5, false);

            dirByte = ConstantMethod.set_bit(dirByte, 6, false);

            dirByte = ConstantMethod.set_bit(dirByte, 7, false);

            dirByte = ConstantMethod.set_bit(dirByte, 8, false);

            CmdOutReadyGo[3] = dirByte;


        }

        //摄像头云台复位
        void  PTReset()
        {
         

            byte dirByte = CmdOutReadyGo[3];// (byte)(0x20 | CwEnable << 3 );

            dirByte = ConstantMethod.set_bit(dirByte, 5, true);

            dirByte = ConstantMethod.set_bit(dirByte, 6, false);

            dirByte = ConstantMethod.set_bit(dirByte, 7, false);

            dirByte = ConstantMethod.set_bit(dirByte, 8, false);

            CmdOutReadyGo[3] = dirByte;

            PTInRst = true;
            /***
            int cwEn = cwEnable;
            byte[] cmdOutTemp = CmdOutReadyGo.ToArray();
            byte dirByte = (byte)(0x10 | cwEn << 3);
            cmdOutTemp[3] = dirByte;

            CmdOutReadyGo = cmdOutTemp;

            PTInRst = true;
            ***/
            while (PTInRst)
            {                            
                ConstantMethod.Delay(100);
                
                if (ResetOk)
                {
                    StopPT();
                    break;
                }
            }
        }
        //摄像头云台上
         void PackPTUp()
        {                   

            byte dirByte = CmdOutReadyGo[3];// (byte)(0x20 | CwEnable << 3 );

            dirByte = ConstantMethod.set_bit(dirByte, 5, false);

            dirByte = ConstantMethod.set_bit(dirByte, 6, true);

            dirByte = ConstantMethod.set_bit(dirByte, 7, false);

            dirByte = ConstantMethod.set_bit(dirByte, 8, false);

            CmdOutReadyGo[3] = dirByte;
                   
            PTUp = true;

        }              
        //摄像头云台下     
        void PackPTDown()
        {

            byte dirByte = CmdOutReadyGo[3];// (byte)(0x20 | CwEnable << 3 );

            dirByte = ConstantMethod.set_bit(dirByte, 5, false);

            dirByte = ConstantMethod.set_bit(dirByte, 6, false);

            dirByte = ConstantMethod.set_bit(dirByte, 7, true);

            dirByte = ConstantMethod.set_bit(dirByte, 8, false);

            CmdOutReadyGo[3] = dirByte;

            PTDown = true;
            /***
            byte[] cmdOutTemp = CmdOutReadyGo.ToArray();
            byte dirByte = (byte)(0x40 | CwEnable << 3);
            cmdOutTemp[3] = dirByte;
            CmdOutReadyGo = cmdOutTemp;
            PTDown = true;   
            **/

        }
        //摄像头云台左
         void PackPTLeft()
        {

            byte dirByte = CmdOutReadyGo[3];// (byte)(0x20 | CwEnable << 3 );

            dirByte = ConstantMethod.set_bit(dirByte, 5, false);

            dirByte = ConstantMethod.set_bit(dirByte, 6, true);

            dirByte = ConstantMethod.set_bit(dirByte, 7, true);

            dirByte = ConstantMethod.set_bit(dirByte, 8, false);

            CmdOutReadyGo[3] = dirByte;

            PTLeft = true;


            /***
            byte[] cmdOutTemp = CmdOutReadyGo.ToArray();
            byte dirByte = (byte)(0x60 | CwEnable << 3);
            cmdOutTemp[3] = dirByte;
            CmdOutReadyGo = cmdOutTemp;
            PTLeft = true;
            ***/

        }
        //摄像头云台右
         void PackPTRight()
        {
            byte dirByte = CmdOutReadyGo[3];// (byte)(0x20 | CwEnable << 3 );

            dirByte = ConstantMethod.set_bit(dirByte, 5, false);

            dirByte = ConstantMethod.set_bit(dirByte, 6, false);

            dirByte = ConstantMethod.set_bit(dirByte, 7, false);

            dirByte = ConstantMethod.set_bit(dirByte, 8, true);

            CmdOutReadyGo[3] = dirByte;

            PTRight = true;

            /****
            byte[] cmdOutTemp = CmdOutReadyGo.ToArray();
            byte dirByte = (byte)(0x80 | cwEn << 3);
            cmdOutTemp[3] = dirByte;
            PTRight = true;
            CmdOutReadyGo = cmdOutTemp;  
            ***/

        }
        #endregion
        #region 杆的运动

       
        bool IsGanMovingFlag { get { return CamRise || CamDescend; } }
        public bool StartGan(int id)
        {
            if (!Status) return false;

            switch (id)
            {
                case 1:
                    {
                        if (CamRise) return true;
                        PackTaiGanCmd();
                        break;
                    }
                case 2:
                    {
                        if (CamDescend) return true;
                        PackJiangGanCmd();
                        break;
                    }
                default:
                    {
                        if(IsGanMovingFlag)
                        PackStopGanCmd();
                        break;
                    }
            }

            return true;
        }
               
        //抬杆 降杆
        bool CamRise = false;
        bool CamDescend = false;
         void PackTaiGanCmd()
        {
            
            CmdOutReadyGo[1] = 0x01;         
            CamRise = true;
        }
        //降杆
         void PackJiangGanCmd()
        {
            CmdOutReadyGo[1] = 0x02;
            CamDescend = true;

        }
        //停止杆
        void PackStopGanCmd()
        {
            CmdOutReadyGo[1] = 0x00;
            CamRise = false;
            CamDescend = false;
        }

        #endregion

        #region 小车运动
        bool IsCarMoving
        {
            get { return CarMoveBackward || CarMoveForward || CarTurnLeft || CarTurnRight || GetLineHouTui; }
        }
        public void CarStart(int id)
        {
            if (!Status) return;
        
            switch (id)
            {
                case 1: //上
                    {
                        if (IsCarMoving) return;
                        //持续运动检测器开启
                        PackQianCmd();
                        break;
                    }
                case 2:
                    {
                        if (IsCarMoving) return;
                        PackHouCmd();
                        break;
                    }
                case 3:
                    {
                        if (IsCarMoving) return;
                        PackZuoCmd();
                        break;
                    }
                case 4:
                    {
                        if (IsCarMoving) return;
                        PackYouCmd();
                        break;
                    }
                case 5:
                    {

                        ZdGetLineHouTuiStart();
                        break;
                    }
                default:
                    {
                        if (IsCarMoving)
                        StopCar();
                        break;
                    }
            }
        }

        //小车前进
        bool CarMoveForward = false;
        void PackQianCmd()
        {
                        
            byte dirByte = (byte)(0x20 | Speed);
            CmdOutReadyGo[2] = dirByte;
            CarMoveForward = true;

        }


        //小车后退
        bool CarMoveBackward = false;
        void PackHouCmd()
        {
           
            byte dirByte = (byte)(0x40 | Speed);
            CmdOutReadyGo[2] = dirByte;          

            CarMoveBackward = true;
        }

        //小车左转
        bool CarTurnLeft = false;
         void PackZuoCmd()
        {         
            byte dirByte = (byte)(0x60 | Speed);
            CmdOutReadyGo[2] = dirByte;           
            CarTurnLeft = true;
        }

        //小车右转
        bool CarTurnRight = false;
         void PackYouCmd()
        {
                       
            byte dirByte = (byte)(0x80 | Speed);
            CmdOutReadyGo[2] = dirByte;         
            CarTurnRight = true;
        }

        //后退收线 卷线盘
        bool GetLineHouTui = false;
        void ZdGetLineHouTuiStart()
        {          
            byte dirByte = (byte)(0x10 |Speed);
            CmdOutReadyGo[2] = dirByte;
            GetLineHouTui = true;          
        }
      
        //停止终端
        void StopCar()
        {
            CmdOutReadyGo = cmdZDUsualOut.ToArray(); 
            CarMoveForward = false;
            CarMoveBackward = false;
            CarTurnLeft = false;
            CarTurnRight = false;
            GetLineHouTui = false;
        }
        #endregion
        public void CwOpposite()
        {
            if (CwEnable == 1)
            {
                CwOnOff(false);
            }
            else
            {
                CwOnOff(true);
            }
        }
        //打开除雾
        public void CwOnOff(bool value)
        {
            //第3个字节 第3位
            if (value)
            {              
                CwEnable = 1;
                byte dirByte = (byte)(CmdOutReadyGo[3] | 0x08);
                CmdOutReadyGo[3] = dirByte;
            }
            else
            {
                CwEnable = 0;
                byte dirByte = (byte)(CmdOutReadyGo[3] & 0xF7);
                CmdOutReadyGo[3] = dirByte;

            }
        }      
        #endregion

        public ZdDevice(ServerInfo p0) :base(p0)
        {
            EventDataProcess += new xjplc.socDataProcess(ZDDataProcess);

            cmdLst = cmdRobot.createCmdRobotFromXml(configCmdFile);
     
            CmdOutReadyGo = cmdZDUsualOut.ToArray();

            CmdInReadyCome = cmdZDUsualOut.ToArray();

        }

        //倾斜翻滚角度运算
        int getValueByDir(int olddir, int oldangle)
        {
            if (olddir == 1)
            {
                oldangle = (127-oldangle) * (-1);
            }

            return oldangle;
        }

        //数据处理
        void ZDDataProcess(object sender, SocEventArgs e)
        {
            if (DeviceId == Constant.devicePropertyB)
            {
                if (e.Byte_buffer.Length == 5 && e.Byte_buffer[0] == 0xb0 && e.Byte_buffer[4] == 0x0b)
                {
                    int oldQxValue = CarQxValue;
                    int oldQxDir = CarQxDir;

                    ResetOk = ConstantMethod.getBitValueInByte(8, e.Byte_buffer[1]) == 1 ? true : false;

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
