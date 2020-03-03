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


        public bool IsHandControl = false;
        
        //近光灯亮度
        int nearlight = 3;
        public int Nearlight
        {
            get { return nearlight; }
            set { nearlight = value; }
        }

        //后视灯亮度

        int backlight = 3;
        public int Backlight
        {
            get { return backlight; }
            set { backlight = value; }
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
        int speed = 3;
        public int Speed
        {
            get { return speed; }
            set { speed = value; }
        }

        //小车抬杆状态
        int carTaiGanValue = 0;
        public int CarTaiGanValue
        {
            get { return carTaiGanValue; }
            set { carTaiGanValue = value; }
        }
        //小车气压1
        double carPressure1 = 0;
        public double CarPressure1
        {
            get { return carPressure1; }
            set { carPressure1 = value; }
        }
        //小车气压2
        double carPressure2 = 0;
        public double CarPressure2
        {
            get { return carPressure2; }
            set { carPressure2 = value; }
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
        public Action<int> FarLightChange;
        public Action<int> NearLightChange;
        public Action<int> ZdSpeedChange;
        public Action<int> BackLightChange;
        //远光灯加1
        public void incFarLight()
        {
            SetLightFar(Farlight + 1);
        }

        //近光灯加1
        public void incNearLight()
        {
            SetLightNear(Nearlight + 1);
        }

        //远光灯减1
        public void decFarLight()
        {
            SetLightFar(Farlight -1);
        }

        //近光灯减1
        public void decNearLight()
        {
            SetLightNear(Nearlight - 1);
        }

        public void SetSpeed(int spvalue)
        {
            Speed = spvalue;

            if (Speed > 10)Speed = 10;
            if (Speed < 0)Speed = 0;

           
            byte dirByte = ((byte)(CmdOutReadyGo[2] & 0xf0));
            dirByte = (byte)(dirByte | (Speed));
                CmdOutReadyGo[2] = dirByte;

                //取出速度值 给默认的指令
                dirByte = (byte)(dirByte & 0x0f);
                cmdZDUsualOut[2] = dirByte;

            if (ZdSpeedChange != null  && IsHandControl)
            {
                ZdSpeedChange(Speed);
            }


        }
        //单独设置远近光灯设置
        public void SetLightCTRL(int farLight, int nearLight)
        {
            if (farLight > 10 || farLight<0) return;

            if (nearLight > 10 || nearlight <0) return;


            byte dirByte = (byte)(farLight << 4 | nearLight);

            CmdOutReadyGo[4] = dirByte;
            cmdZDUsualOut[4] = dirByte;

            this.Farlight = farLight;
            this.Nearlight = nearLight;
                      
        }
        public void SetLightNear(int near)
        {

            if (near > 10 || near < 0) return;


            byte dirByte = (byte)(this.Farlight << 4 | near);

            CmdOutReadyGo[4] = dirByte;
            cmdZDUsualOut[4] = dirByte;

            this.Nearlight = near;

            if (NearLightChange != null && IsHandControl)
            {
                NearLightChange(Nearlight);
            }


        }
        public void SetLightFar(int far)
        {
            if (far > 10 || far < 0) return;

           

            byte dirByte = (byte)(far << 4 | this.Nearlight);

            CmdOutReadyGo[4] = dirByte;
            cmdZDUsualOut[4] = dirByte;

            this.Farlight = far;
            if (FarLightChange != null  && IsHandControl)
                FarLightChange(Farlight);

        }

        public bool SetCarReset()
        {

            byte dirByte = CmdOutReadyGo[5];// (byte)(0x20 | CwEnable << 3 );

            dirByte = ConstantMethod.set_bit(dirByte, 5, true);

           
            CmdOutReadyGo[5] = dirByte;

            CarResetOk = false;
            CarInRst = true;
            /***
            int cwEn = cwEnable;
            byte[] cmdOutTemp = CmdOutReadyGo.ToArray();
            byte dirByte = (byte)(0x10 | cwEn << 3);
            cmdOutTemp[3] = dirByte;

            CmdOutReadyGo = cmdOutTemp;

            PTInRst = true;
            ***/

            ConstantMethod.Delay(3000, ref CarResetOk);

            CarInRst = false;
            dirByte = ConstantMethod.set_bit(dirByte, 5, false);

  
            CmdOutReadyGo[5] = dirByte;
             

            
            return CarResetOk;




        }

        bool UnLock = true;
        public bool SetCarUnlock()
        {

            byte dirByte = CmdOutReadyGo[5];// (byte)(0x20 | CwEnable << 3 );                   

            if(UnLock)
               UnLock = false;
            else
                UnLock = true;

            dirByte = ConstantMethod.set_bit(dirByte, 6, UnLock);

            CmdOutReadyGo[5] = dirByte;

       
            return UnLock;




        }
        public void SetBackLight(int near)
        {
            if (near > 10 || near < 0) return;


            byte dirByte = (byte)(0x0F & near);

            CmdOutReadyGo[5] = dirByte;
            cmdZDUsualOut[5] = dirByte;

            this.Backlight = near;

            if (BackLightChange != null && IsHandControl)
                BackLightChange(Backlight);


        }
        public void IncLight(int near)
        {
            SetBackLight(Backlight + 1);         
        }
        public void DecLight(int near)
        {
            SetBackLight(Backlight- 1);

        }
        //速度加1
        public void incSpeed()
        {
            
            Speed++;

            if (Speed >= 10) Speed = 10;

            SetSpeed(Speed);

        }
        //速度减1
        public void decSpeed()
        {
            
            Speed--;
            if (Speed <= 0) Speed = 0;

            SetSpeed(Speed);

        }
        #endregion
        #region 运动控制
        #region 云台运动
        public Action<int> YunTaiShowChange;
        public bool  CamPTStart(int id)
        {
            if (!Status ) return false;
            if (YunTaiShowChange != null) YunTaiShowChange(id);
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
                        if (!IsPTMoving )
                            return PTReset();
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
        bool CarInRst = false;
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
        bool  PTReset()
        {
         

            byte dirByte = CmdOutReadyGo[3];// (byte)(0x20 | CwEnable << 3 );

            dirByte = ConstantMethod.set_bit(dirByte, 5, true);

            dirByte = ConstantMethod.set_bit(dirByte, 6, false);

            dirByte = ConstantMethod.set_bit(dirByte, 7, false);

            dirByte = ConstantMethod.set_bit(dirByte, 8, false);

            CmdOutReadyGo[3] = dirByte;

            ResetOk = false;

            PTInRst = true;
            /***
            int cwEn = cwEnable;
            byte[] cmdOutTemp = CmdOutReadyGo.ToArray();
            byte dirByte = (byte)(0x10 | cwEn << 3);
            cmdOutTemp[3] = dirByte;

            CmdOutReadyGo = cmdOutTemp;

            PTInRst = true;
            ***/

            ConstantMethod.Delay(5000, ref resetOk);
            PTInRst = false;

            StopPT();

            return resetOk;



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
        public Action<int> CarStartShowChange;
        public void CarStart(int id)
        {
            if (!Status) return;
        
            switch (id)
            {
                case 1: //上
                    {
                        if (IsCarMoving) return;
                        if (CarStartShowChange != null)
                        {
                            CarStartShowChange(1);
                        }
                        //持续运动检测器开启
                        PackQianCmd();
                        break;
                    }
                case 2:
                    {
                        if (IsCarMoving) return;
                        if (CarStartShowChange != null)
                        {
                            CarStartShowChange(2);
                        }
                        PackHouCmd();
                   
                        break;
                    }
                case 3:
                    {
                        if (IsCarMoving) return;
                        if (CarStartShowChange != null)
                        {
                            CarStartShowChange(3);
                        }
                        PackZuoCmd();
                        break;
                    }
                case 4:
                    {
                        if (IsCarMoving) return;
                        if (CarStartShowChange != null)
                        {
                            CarStartShowChange(4);
                        }
                        PackYouCmd();
                        break;
                    }
                case 5:
                    {
                        ZdGetLineHouTuiStart();
                        if (CarStartShowChange != null)
                        {
                            CarStartShowChange(2);
                        }
                        break;
                    }
                default:
                    {
                        if (IsCarMoving)
                            if (CarStartShowChange != null)
                            {
                                CarStartShowChange(0);
                            }
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
        public bool CarResetOk = false;
        //数据处理
        void ZDDataProcess(object sender, SocEventArgs e)
        {
            if (DeviceId == Constant.devicePropertyB)
            {
                if (e.Byte_buffer.Length == 8 && e.Byte_buffer[0] == 0xb0 && e.Byte_buffer[7] == 0x0b)
                {
                    int oldQxValue = CarQxValue;
                    int oldQxDir = CarQxDir;

                    ResetOk = ConstantMethod.getBitValueInByte(8, e.Byte_buffer[1]) == 1 ;
                    CarResetOk = ConstantMethod.getBitValueInByte(7, e.Byte_buffer[1]) == 1;
                    
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
                    CarFgDir = ConstantMethod.getBitValueInByte(8, e.Byte_buffer[3]);
                    CarFgValue = e.Byte_buffer[3] & 0x7F;



                    //新计算方式
                    int oldCurrentFgValue = CurrentFgValue;
                    CurrentFgValue = getValueByDir(CarFgDir, CarFgValue); ;// (int)e.Byte_buffer[3];
                    //if (RotateFgValue == 0)
                    RotateFgValue = CurrentFgValue-oldCurrentFgValue ;// getRotateValue(oldFgDir, oldFgValue, CarFgDir, CarFgValue);




                    #endregion

                    #region 抬杆高度气压1气压2
                    CarTaiGanValue = e.Byte_buffer[4];
                

                    CarPressure1 = ((e.Byte_buffer[5]- 98.333)/ -46.865);

                    CarPressure2 = ((e.Byte_buffer[6] - 98.333) / -46.865);

                    if (CarPressure1 <= 0) CarPressure1 = 0;
                    if (CarPressure2 <= 0) CarPressure2 = 0;
                    #endregion
                }
            }
        }

    }
}
