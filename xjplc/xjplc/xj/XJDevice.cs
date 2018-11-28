using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace xjplc
{
    /// <summary>
    /// 此类的说明
    /// 1.使用commmanager来进行数据接收发送
    /// 2.处理commmanager来的数据
    /// 3.接收用户界面提供的端口数据和需要读取寄存器内容的文件
    /// </summary>
    public class XJDevice 
    {
        string deviceId="无设备id";
        public string DeviceId
        {
            get { return deviceId; }
            set { deviceId = value; }
        }
        CsvStreamReader CSVData ;

        private List<DataTable> dataFormLst; //表格 
        public List<DataTable> DataFormLst  
        {
            get { return dataFormLst; }
            set { dataFormLst = value; }
        }

        private List<DataGridView> dgShowLst;//显示表格控件
        public List<DataGridView> DgShowLst
        {
            get { return dgShowLst; }
            set { dgShowLst = value; }
        }

        //List<List<PlcInfo>> DPlcInfoLst;
        //List<List<List<PlcInfo>>> MPlcInfoAllLst;
        //信捷使用 监控的数据都放在这里      
        public List<XJPlcInfo> DPlcInfo ;
        //这里为什么要这样？ 因为M值不连续的分一组 再整理起来
        public List<List<XJPlcInfo>> MPlcInfoAll;

        XJPLCPackCmdAndDataUnpack XJPLCcmd ;

        PortParam portParam ;

        XJCommManager comManager ;

        System.Timers.Timer WatchCommTimer ;// new System.Timers.Timer(500);

        int CommError = 0;  //只能在通讯上之后 清零
                            //机器状态
        int status;
        public int Status
        {
            get { return status; }
            set { status = value; }
        }
      
        public XJDevice()
        {

        }
        //切换监控的数据表格
        bool isShiftDataForm = false;    //对于流程：重新打包命令 comm发送 接收修改好 数据返回处理 修改好
        public bool IsShiftDataForm
        {
            get { return isShiftDataForm; }
            set { isShiftDataForm = value; }
        }

         bool isStopConnection;
        public bool IsStopConnection
        {
            get { return isStopConnection; }
            set {
                isStopConnection = value;
                comManager.IsNoConnection = value;
            }
        }

        public XJDevice(List<string> filestr,PortParam p0)
        {
            XJPLCcmd = new XJPLCPackCmdAndDataUnpack();
            DataFormLst = new List<DataTable>();
            CSVData = new CsvStreamReader();
            dgShowLst = new List<DataGridView>();

            //获取监控数据 dataformLst 填充
            GetPlcDataTableFromFile(filestr);

            //找一下串口 不存在就报错 退出          
           
            portParam = ConstantMethod.LoadPortParam(Constant.ConfigSerialportFilePath);

            if (!SerialPort.GetPortNames().Contains(portParam.m_portName))
            {
                MessageBox.Show(Constant.NoSerialPort+ portParam.m_portName);
                ConstantMethod.AppExit();
            }
            //监控第一个列表数据 考虑下 这个还要不要 因为已经有一个 shift在后面了
            if (dataFormLst.Count > 0)
                SetPlcReadDMData(dataFormLst[0]);

            //设置端口
            SetComm(p0);

            //监控通讯
            WatchCommTimer = new System.Timers.Timer(Constant.XJRestartTimeOut);  //这里1.5 秒别改 加到常量里 工控机性能不行 

            WatchCommTimer.Enabled = false;

            WatchCommTimer.AutoReset = true;

            WatchCommTimer.Elapsed += new System.Timers.ElapsedEventHandler(WatchTimerEvent);


        }

        public void setDeviceId(string idStr)
        {
            DeviceId = idStr;

        }
        /// <summary>
        /// 针对信捷PLC 进行设备的存在获取
        /// </summary>
        /// <returns></returns>

        public XJDevice(List<string> filestr)
        {
            XJPLCcmd    = new XJPLCPackCmdAndDataUnpack();
            DataFormLst = new List<DataTable>();
            CSVData     = new CsvStreamReader();
            dgShowLst   = new List<DataGridView>();

            //获取监控数据 dataformLst 填充
            GetPlcDataTableFromFile(filestr);

            //找一下串口 不存在就报错 退出
            if (!ConstantMethod.XJFindPort())
            {
               
                MessageBox.Show(DeviceId+Constant.ConnectMachineFail);
                //报错 在外面调试 需要隐藏
                ConstantMethod.AppExit();
            }

            portParam = ConstantMethod.LoadPortParam(Constant.ConfigSerialportFilePath);          

            //监控第一个列表数据 考虑下 这个还要不要 因为已经有一个 shift在后面了
            if (dataFormLst.Count>0)                
            SetPlcReadDMData(dataFormLst[0]);

            //设置端口
            SetComm(portParam);

            //监控通讯
            WatchCommTimer = new System.Timers.Timer(Constant.XJRestartTimeOut);  //这里1.5 秒别改 加到常量里 工控机性能不行 

            WatchCommTimer.Enabled = false;

            WatchCommTimer.AutoReset = true;

            WatchCommTimer.Elapsed += new System.Timers.ElapsedEventHandler(WatchTimerEvent);

           
        }
        #region 通讯错误
        //通讯错误引发的事件
        private void WatchTimerEvent(object sender, EventArgs e)
        {
            if(DeviceId=="门板机")
            {
                int s = 0;
            }
            if (!comManager.Status)
            {
                //先停了再说省的下一个定时事件又来
                WatchCommTimer.Enabled = false;
                status = Constant.DeviceNoConnection;
                CommError++;
                LogManager.WriteProgramLog(DeviceId+Constant.DeviceConnectionError);

                if (CommError < Constant.DeviceErrorConnCountMax)
                {
                    getDeviceData();                
                }
                else
                {
                   
                    MessageBox.Show("设备连接失败！");
                    return;
                }
               
                
            }
        }
        #endregion
        #region 正常通讯
        public void startRepack()
        {
            comManager.IsRePackCmdReadDMDataOut = true;
            comManager.IsRepackDone = false;

        }

        public void doneRepack()
        {
            comManager.IsRepackDone = true;

        }
        public bool shiftDataFormSplit(int formid,int rowSt,int count)
        {

            DataTable dt = dataFormLst[formid].Clone();
            startRepack();
            if ((rowSt >0) && ((rowSt+count-1)<= dataFormLst[formid].Rows.Count) && count>0)
            {
                for (int i = rowSt; i < rowSt+count-1; i++)
                {
                    
                    dt.ImportRow(dataFormLst[formid].Rows[i]);
                }
            }

            splitDataForm(dt,dataFormLst[formid]);
            isShiftDataForm = true;

            doneRepack();

            return true;
        }


        /// <summary>
        /// 切换监控数据表格
        /// </summary>
        /// <param name="formid"></param>
        /// <returns></returns>
        public bool shiftDataForm(int formid)
        {
            if (dataFormLst[formid] != null && dataFormLst[formid].Rows.Count > 0)
            {
                startRepack();

                isShiftDataForm = true;
                dataForm = dataFormLst[formid];
                int dAreaCount = 0;

                foreach(DataRow r in dataFormLst[formid].Rows)
                {
                    if (r["addr"].ToString().Contains(Constant.strDMArea[0]))
                       {
                          dAreaCount++;
                       }
                }

                if (dAreaCount > Constant.DataRowWatchMax)
                {
                    DataTable dt = dataFormLst[formid].Copy();
                    for (int i = dt.Rows.Count - 1; i >= 0; i--)
                    {
                        dt.Rows.RemoveAt(i);
                        if (dt.Rows.Count == Constant.DataRowWatchMax) break;
                    }

                    splitDataForm(dt, dataFormLst[formid]);
                } 
                else                                                        
                if (dataForm != null && dataForm.Rows.Count > 0)
                    PackCmdReadDMDataOut(dataForm);

                doneRepack();                                                                                                                                                                                  

                return true;

            }
            else return false;
                    
        }
        public bool splitDataForm(DataTable dataSplit,DataTable dataform1)
        {
            if (dataSplit != null && dataSplit.Rows.Count > 0)
            {

                SplitPackCmdReadDMDataOut(dataSplit, dataform1);
              
                return true;
            }
            else return false;

        }

        //重置端口 传入PLC数据读取文件和 端口
        public void RestartConneect(DataTable dt)
        {
            DeviceShutDown();

            
            ConstantMethod.Delay(50);

          //  portParam = ConstantMethod.LoadPortParam(Constant.ConfigSerialportFilePath);

            /*****
            if (!ConstantMethod.XJFindPort())
            {
                MessageBox.Show(DeviceId+Constant.ConnectMachineFail);
                ConstantMethod.AppExit();
            }
            *****/
            if (dataFormLst.Count>0)
            SetPlcReadDMData(dt);
            if (portParam.m_portName != null)
            SetComm(portParam);
            CommError = 0;
            
        }
        /// <summary>
        /// 开始进行通讯
        /// 如果在通讯中 突然中断了 commManager 进行reset 然后回调一个函数给device
        /// 下次如果要再进行通讯 需要重新连接
        /// 重新连接的话 1.端口要重新设置 SetPortParam(PortParam portParam0)
        ///             2.读取的文件也要在连接前确认好 SetPlcReadDataFile(string filename) 
        ///获取通讯的两个条件：1，端口 2.要读取的数据
        /// </summary>
        /// <returns></returns>
        public bool getDeviceData()
        {
            WatchCommTimer.Enabled = true;
            status = Constant.DeviceNoConnection;

            if (XJPLCcmd !=null)


            if (comManager.ConnectMachine())
            {
                    CommError = 0;
                    status = Constant.DeviceConnected;
                    return true;
             }

            comManager.Status = false;
            return false;
        }
        /// <summary>
        /// 设置端口 传入串口数据
        /// 如果以后要重新连接 或者更改串口了 可以使用这个函数
        /// </summary>
        /// <param name="portParam0"></param>
        public void SetComm(PortParam portParam0)
        {
            if (comManager != null)
            {
                comManager.Reset();
                comManager = null;
            }
                                 
            portParam = portParam0;
          

            comManager = new XJCommManager(portParam);
            //设置数据处理委托事件
            comManager.EventDataProcess += new xjplc.commDataProcess(Dataprocess);

            //命令打包类重新确认
            comManager.SetXJPLCcmd(XJPLCcmd);
           
            //GC.Collect();
            //GC.WaitForPendingFinalizers();
        }
        public void SetPlcReadDMData(DataTable dt)
        {
    
            //确认表格 //这个要隔离出来 方便以后可以单独调用 针对用户更改读取内容
                      
                if (dt != null && dt.Rows.Count > 0)
                {
                     dataForm = dt;
                     PackCmdReadDMDataOut(dataForm);              
                }           
        }
        /// <summary>
        /// 如果用户需要重新制定读取一些D区 M区的话可以先设置保存读取PLC 内容的文件
        /// 读取后生成命令
        /// </summary>
        public void SetPlcReadDMData(string filename)
        {
                      
            if (XJPLCcmd.CmdReadDMDataOut != null)
            {
                XJPLCcmd.CmdReadDMDataOut = null;
            }

            if (CSVData != null)
            {

                CSVData.Dispose();
                CSVData = null;
            }

            CSVData = new CsvStreamReader();
            string s = Path.GetExtension(filename);
            if (File.Exists(filename) && (Path.GetExtension(filename).Equals(Constant.CSVFileEX)))
                CSVData.FileName = filename;
            //确认表格 //这个要隔离出来 方便以后可以单独调用 针对用户更改读取内容
            if (CSVData.CheckCSVFile(Constant.PLCstrCol))
            {
                dataForm = CSVData.OpenCSV(CSVData.FileName);
                if (dataForm != null && dataForm.Rows.Count > 0)
                {
                    PackCmdReadDMDataOut(dataForm);
                    //在建立连接的时候 切换表格数据源            
                }

            }
            //GC.Collect();
            //GC.WaitForPendingFinalizers();
        }
        #endregion
       
        #region PLC数据需要采集的准备
        //机器数据 表格 具体信息见constan中列的信息
        DataTable dataForm;
        public void GetPlcDataTableFromFile(List<string> strfile)
        {
            if (strfile != null && strfile.Count > 0)
                foreach (string s in strfile)
                {
                    if (ConstantMethod.FileIsUsed(s))
                    {
                        MessageBox.Show(Constant.FileIsInUse);
                        ConstantMethod.AppExit();


                    }
                    if (File.Exists(s) && (Path.GetExtension(s).Equals(Constant.CSVFileEX)))
                        CSVData.FileName = s;
                    //确认表格 //这个要隔离出来 方便以后可以单独调用 针对用户更改读取内容
                    if (CSVData.CheckCSVFile(Constant.PLCstrCol))
                    {
                        DataTable dataForm0 = new DataTable();
                        dataForm0 = CSVData.OpenCSV(CSVData.FileName);
                        if (dataForm0 != null && dataForm0.Rows.Count > 0)
                            DataFormLst.Add(dataForm0);
                        else
                        {
                            MessageBox.Show(Constant.ReadPlcInfoFail);
                            ConstantMethod.AppExit();
                        }

                    }
                    else
                    {
                        MessageBox.Show(Constant.ReadPlcInfoFail);
                        Application.Exit();
                        System.Environment.Exit(0);
                    }
                }
             
           
        }
        public System.Data.DataTable DataForm
        {
            get { return dataForm; }
            set { dataForm = value; }
        }

        /// <summary>
        /// 本函数的思路：
        /// 1.根据需要读取的表格 进行分解成单个地址的单元下发指令 
        /// 2.信捷PLC 发送一条读取指令会包含D区起始地址和M区地址 并跟着相应的读取个数
        /// 3.相邻地址相差7以内的 可以一次性读完
        /// </summary>
        void PackCmdReadDMDataOut(DataTable dataForm0)
        {
            List<XJPlcInfo> plcInfoLst = new List<XJPlcInfo>();
            List<XJPlcInfo> MPlcInfo   = new List<XJPlcInfo>();
            foreach (DataRow row in dataForm0.Rows)
            {
                int mAddr = 0;
                int count = 0;
                string strSplit1;
                string strSplit2;
                string DSmode; //单字还是双字
                #region 获取地址 个数 区域 创建plcinfo类 并添加到集合
                if (row == null) return;
                

                if (row["addr"].ToString() == null || row["mode"].ToString() == null || row["count"].ToString() == null)
                {
                    return;
                };
                //添加各个单元
                //取数字 用替代的字符的方法 取数组就替换字母为空 取字母就替换数字
                strSplit1 = Regex.Replace(row["addr"].ToString(), "[A-Z]", "", RegexOptions.IgnoreCase);

                //取字母
                strSplit2 = Regex.Replace(row["addr"].ToString(), "[0-9]", "", RegexOptions.IgnoreCase);

                DSmode = row["mode"].ToString();

                if (!int.TryParse(row["count"].ToString(), out count))
                {
                    continue;
                }
                //地址超了 无效 暂且定XDM 最大69999
                if (!int.TryParse(strSplit1, out mAddr) || (mAddr<0) || (mAddr > Constant.XJMaxAddr))
                {
                    continue;
                }
                //字母大于4 无效地址
                if (strSplit2.Count() > 3) continue;
                //这里数组进行统计 
                if (DSmode.Equals(Constant.DoubleMode)&&(XJPLCPackCmdAndDataUnpack.AreaGetFromStr(strSplit2) <Constant.M_ID))
                {
                    count = count * 2;
                }
                //传入数据起始地址 个数 区域 模式
                XJPlcInfo[] tmpInfoLst = XJPLCcmd.GetPlcInfo(mAddr,count,strSplit2, DSmode);

                if (tmpInfoLst.Count() > 0) plcInfoLst.AddRange(tmpInfoLst);
                #endregion
            }
            #region 排序 去重复 统计DM 起始点
            //排序 按照绝对地址 
            plcInfoLst = plcInfoLst.OrderBy(x => x.AbsAddr).ToList();
            //去重复 
            plcInfoLst = plcInfoLst.Distinct(new ModelComparer()).ToList();
           
            //分离D 区 M区
            DPlcInfo = plcInfoLst.FindAll(t => t.IntArea < (Constant.HSD_ID + 1));
            MPlcInfo = plcInfoLst.FindAll(t => t.IntArea > (Constant.HSD_ID));

            DPlcInfo= InsertPlcInfo(DPlcInfo); //将D区分解出来 出来 变成一个一个单个的地址 尽量保持连续 
            MPlcInfo = InsertPlcInfo(MPlcInfo);


            plcInfoLst = DPlcInfo.Union(MPlcInfo).ToList<XJPlcInfo>();
            #endregion
            #region  根据断点 建立命令的表格缓冲lst 然后创建读取DM区域的命令
            //开始打包
            List<int> addrLst = new List<int>();//连续地址的起始地址
            List<int> idLst = new List<int>();  //地址是D xy HSD
            List<int> addrcount = new List<int>(); //起始地址开始 读取几个寄存器

            List<int> breakPoint = new List<int>(); //在
            //首先要确定断点
            breakPoint.Add(0);
            //获取不连续点的位置  D区域
            for (int i = 0; i < plcInfoLst.Count - 1; i++)
            {
                if (((plcInfoLst[i + 1].RelAddr - plcInfoLst[i].RelAddr) > 1) || (plcInfoLst[i + 1].IntArea != plcInfoLst[i].IntArea))
                {
                    int bp = i + 1;
                    breakPoint.Add(bp);                   
                }
            }

            breakPoint.Add(plcInfoLst.Count);  
                      
            //d区在前面，M区在后面 根据断点来区分
            //统计D区起始地址个数 统计M区起始地址个数 
            //D区返回数据可以根据Dplcinfo集合来预算
            //但M区需要知道M起始地址个数          
            for (int j = 0; j < breakPoint.Count; j++)
            {              
                if (breakPoint[j] < plcInfoLst.Count)
                {                
                    addrLst.Add(plcInfoLst[breakPoint[j]].AbsAddr);
                    idLst.Add(plcInfoLst[breakPoint[j]].IntArea);
                    addrcount.Add(plcInfoLst[breakPoint[j + 1] - 1].RelAddr - plcInfoLst[breakPoint[j]].RelAddr + 1);
                }
            }
            //这里d区的话 需要指定一下双字情况下的 另外一个字节
            FindHighPlcInfo(DPlcInfo);

            //这里M区麻烦一点 分成n个M单元组 每个单元组 有个起始地址
            MPlcInfoAll = new List<List<XJPlcInfo>>();
            for (int i = 0; i < addrLst.Count; i++)
            {
                List<XJPlcInfo> mplst = new List<XJPlcInfo>();
                if (idLst[i] > Constant.HSD_ID)
                {
                    for (int k = 0; k < addrcount[i]; k++)
                    {
                        XJPlcInfo p = new XJPlcInfo();
                        p.ValueMode = Constant.BitMode;
                        p.ByteValue = new byte[1];
                        p.IntArea = idLst[i];
                        p.AbsAddr = addrLst[i]+k;
                        p.Xuhao = k;                       
                        mplst.Add(p);
                    }

                }
                if (mplst.Count > 0) MPlcInfoAll.Add(mplst);
            }       

            int mCount = 0;          
            for (int i = 0; i < MPlcInfoAll.Count; i++)
            {
                double cntdb = (double)MPlcInfoAll[i].Count / 8;
                mCount = mCount +  
                    (int)Math.Ceiling(cntdb);
            }
            //dataform表格里面地址对应dplcinfo和mplcinfo的数据对应起来
            FindIndexInPlcInfo(dataForm0, DPlcInfo, MPlcInfoAll);

            
            XJPLCcmd.PackCmdReadDMDataOut(addrLst, idLst, addrcount, 5 + DPlcInfo.Count * 2 + mCount);
           
            #endregion
            addrLst = null;
            idLst = null;
            addrcount = null;
            breakPoint = null;
            //GC.Collect();
            //GC.WaitForPendingFinalizers();
        }
        /// <summary>
        /// 这里增加一个 就是 主form太大了 分割一个form出来 但是 绑定 还是在原来那个主form上
        /// </summary>
        /// <param name="dataForm0"></param>
        void SplitPackCmdReadDMDataOut(DataTable dataForm0, DataTable dataForm1)
        {
            List<XJPlcInfo> plcInfoLst = new List<XJPlcInfo>();
            List<XJPlcInfo> MPlcInfo = new List<XJPlcInfo>();
            foreach (DataRow row in dataForm0.Rows)
            {
                int mAddr = 0;
                int count = 0;
                string strSplit1;
                string strSplit2;
                string DSmode; //单字还是双字
                #region 获取地址 个数 区域 创建plcinfo类 并添加到集合
                if (row == null) return;


                if (row["addr"].ToString() == null || row["mode"].ToString() == null || row["count"].ToString() == null)
                {
                    return;
                };
                //添加各个单元
                //取数字 用替代的字符的方法 取数组就替换字母为空 取字母就替换数字
                strSplit1 = Regex.Replace(row["addr"].ToString(), "[A-Z]", "", RegexOptions.IgnoreCase);

                //取字母
                strSplit2 = Regex.Replace(row["addr"].ToString(), "[0-9]", "", RegexOptions.IgnoreCase);

                DSmode = row["mode"].ToString();

                if (!int.TryParse(row["count"].ToString(), out count))
                {
                    continue;
                }
                //地址超了 无效 暂且定XDM 最大69999
                if (!int.TryParse(strSplit1, out mAddr) || (mAddr < 0) || (mAddr > Constant.XJMaxAddr))
                {
                    continue;
                }
                //字母大于4 无效地址
                if (strSplit2.Count() > 3) continue;
                //这里数组进行统计 
                if (DSmode.Equals(Constant.DoubleMode) && (XJPLCPackCmdAndDataUnpack.AreaGetFromStr(strSplit2) < Constant.M_ID))
                {
                    count = count * 2;
                }
                //传入数据起始地址 个数 区域 模式
                XJPlcInfo[] tmpInfoLst = XJPLCcmd.GetPlcInfo(mAddr, count, strSplit2, DSmode);

                if (tmpInfoLst.Count() > 0) plcInfoLst.AddRange(tmpInfoLst);
                #endregion
            }
            #region 排序 去重复 统计DM 起始点
            //排序 按照绝对地址 
            plcInfoLst = plcInfoLst.OrderBy(x => x.AbsAddr).ToList();
            //去重复 
            plcInfoLst = plcInfoLst.Distinct(new ModelComparer()).ToList();

            //分离D 区 M区
            DPlcInfo = plcInfoLst.FindAll(t => t.IntArea < (Constant.HSD_ID + 1));
            MPlcInfo = plcInfoLst.FindAll(t => t.IntArea > (Constant.HSD_ID));

            DPlcInfo = InsertPlcInfo(DPlcInfo); //将D区分解出来 出来 变成一个一个单个的地址 尽量保持连续 
            MPlcInfo = InsertPlcInfo(MPlcInfo);


            plcInfoLst = DPlcInfo.Union(MPlcInfo).ToList<XJPlcInfo>();
            #endregion
            #region  根据断点 建立命令的表格缓冲lst 然后创建读取DM区域的命令
            //开始打包
            List<int> addrLst = new List<int>();//连续地址的起始地址
            List<int> idLst = new List<int>();  //地址是D xy HSD
            List<int> addrcount = new List<int>(); //起始地址开始 读取几个寄存器

            List<int> breakPoint = new List<int>(); //在
            //首先要确定断点
            breakPoint.Add(0);
            //获取不连续点的位置  D区域
            for (int i = 0; i < plcInfoLst.Count - 1; i++)
            {
                if (((plcInfoLst[i + 1].RelAddr - plcInfoLst[i].RelAddr) > 1) || (plcInfoLst[i + 1].IntArea != plcInfoLst[i].IntArea))
                {
                    int bp = i + 1;
                    breakPoint.Add(bp);
                }
            }

            breakPoint.Add(plcInfoLst.Count);

            //d区在前面，M区在后面 根据断点来区分
            //统计D区起始地址个数 统计M区起始地址个数 
            //D区返回数据可以根据Dplcinfo集合来预算
            //但M区需要知道M起始地址个数          
            for (int j = 0; j < breakPoint.Count; j++)
            {
                if (breakPoint[j] < plcInfoLst.Count)
                {
                    addrLst.Add(plcInfoLst[breakPoint[j]].AbsAddr);
                    idLst.Add(plcInfoLst[breakPoint[j]].IntArea);
                    addrcount.Add(plcInfoLst[breakPoint[j + 1] - 1].RelAddr - plcInfoLst[breakPoint[j]].RelAddr + 1);
                }
            }
            //这里d区的话 需要指定一下双字情况下的 另外一个字节
            FindHighPlcInfo(DPlcInfo);

            //这里M区麻烦一点 分成n个M单元组 每个单元组 有个起始地址
            MPlcInfoAll = new List<List<XJPlcInfo>>();
            for (int i = 0; i < addrLst.Count; i++)
            {
                List<XJPlcInfo> mplst = new List<XJPlcInfo>();
                if (idLst[i] > Constant.HSD_ID)
                {
                    for (int k = 0; k < addrcount[i]; k++)
                    {
                        XJPlcInfo p = new XJPlcInfo();
                        p.ValueMode = Constant.BitMode;
                        p.ByteValue = new byte[1];
                        p.IntArea = idLst[i];
                        p.AbsAddr = addrLst[i] + k;
                        p.Xuhao = k;
                        mplst.Add(p);
                    }

                }
                if (mplst.Count > 0) MPlcInfoAll.Add(mplst);
            }

            int mCount = 0;
            for (int i = 0; i < MPlcInfoAll.Count; i++)
            {
                double cntdb = (double)MPlcInfoAll[i].Count / 8;
                mCount = mCount +
                    (int)Math.Ceiling(cntdb);
            }
            //绑定主form
            FindIndexInPlcInfo(dataForm1, DPlcInfo, MPlcInfoAll);


            XJPLCcmd.PackCmdReadDMDataOut(addrLst, idLst, addrcount, 5 + DPlcInfo.Count * 2 + mCount);

            #endregion
            addrLst = null;
            idLst = null;
            addrcount = null;
            breakPoint = null;
            //GC.Collect();
            //GC.WaitForPendingFinalizers();
        }
        /// <summary>
        /// dataform中的数据 与 dplcinfo 和 mplcinfoall 集合对应起来 这样 更新速度会很快
        /// </summary>
        /// <param name="datafm"></param>
        /// <param name="dAll"></param>
        /// <param name="mAll"></param>
        /// <returns></returns>
        private bool FindIndexInPlcInfo(DataTable datafm, List<XJPlcInfo> dAll, List<List<XJPlcInfo>> mAll)
        {
            foreach (DataRow row in datafm.Rows)
            {
                if (row == null) continue;

                int mAddr = 0;  //地址
                int mArea;
                string strSplit1;
                string strSplit2;//地址区域
                string DSmode; //单字还是双字

                strSplit1 = Regex.Replace(row["addr"].ToString().Trim(), "[A-Z]", "", RegexOptions.IgnoreCase);

                //取字母
                strSplit2 = Regex.Replace(row["addr"].ToString().Trim(), "[0-9]", "", RegexOptions.IgnoreCase);
                strSplit1 = strSplit1.Trim();
                strSplit2= strSplit2.Trim();

                DSmode = row["mode"].ToString().Trim();

                //地址超了 无效 暂且定XDM 最大69999
                if (!int.TryParse(strSplit1, out mAddr) || (mAddr < 0) || (mAddr > Constant.XJMaxAddr))
                {
                    continue;
                }
                int count;
                if (!int.TryParse(row["count"].ToString().Trim(), out count))
                {
                    continue;
                }
             
                mArea = XJPLCPackCmdAndDataUnpack.AreaGetFromStr(strSplit2);

                if (mArea > Constant.HM_ID)
                {
                    mAddr = ConstantMethod.GetXYAddr8To10(mAddr);

                }
              
                mAddr = XJPLCPackCmdAndDataUnpack.AreaGetFromStr(mAddr, strSplit2);
               
                if (mArea < 0) continue;
                if (row["param1"] != null && row["param2"] != null)
                {
                    int[] s = FindValueIndexFromDPlcInfo(mAddr, mArea,  DSmode);
                    
                    row["param1"] = s[0];
                    row["param2"] = s[1];
                    //让集合 把dataform 也保存下 方便更新数据
                    if (mArea < Constant.M_ID)
                    {
                        if (!(s[0] > -1)) continue;
                        DPlcInfo[s[0]].BelongToDT = datafm;
                        DPlcInfo[s[0]].Row = datafm.Rows.IndexOf(row);
                    }
                    else
                    {
                        
                        if (!(s[0] > -1 && s[1] > -1)) continue;
                        MPlcInfoAll[s[0]][s[1]].BelongToDT = datafm;
                        MPlcInfoAll[s[0]][s[1]].Row = datafm.Rows.IndexOf(row);
                    }
                }

            }
            return true;
        }
        /// <summary>
        /// 
        /// 从Dplcinfo 和 mplcinfo 需要的地址的顺序 后面可以直接对应
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="area"></param>
        /// <param name="DSmode"></param>
        /// <returns></returns>
        public int[] FindValueIndexFromDPlcInfo(int addr, int area, string DSmode)
        {

            int[] result = new int[2];
            result[0] = -1;

            result[1] = -1;
            //寻找地址和字母都对的
            List<XJPlcInfo> dpResultLow = null;


            if (area < Constant.HSD_ID + 1)
            {
                dpResultLow = DPlcInfo.FindAll(
                delegate (XJPlcInfo pf)
                {
                    return (pf.AbsAddr == addr) && (pf.IntArea.Equals(area));
                });
                if (dpResultLow.Count > 0)
                    //单字的话就是D区和M区的情况了
                result[0] = DPlcInfo.IndexOf(dpResultLow[0]);
            }
            else
            {
                for (int i = 0; i < MPlcInfoAll.Count; i++)
                {

                    dpResultLow = MPlcInfoAll[i].FindAll(
                    delegate (XJPlcInfo pf)
                    {
                        return (pf.AbsAddr == addr) && (pf.IntArea.Equals(area));
                    });

                    //找到M了 

                    if (dpResultLow.Count > 0)
                    {
                        result[0] = i;;
                        result[1] = MPlcInfoAll[i].IndexOf(dpResultLow[0]);
                        break;
                    }

                }

            }
                    

            return result;

        }
        /// <summary>
        /// 在传入的集合里找到 高字节位置 针对双字寄存器
        /// </summary>
        /// <returns></returns>
        private bool FindHighPlcInfo(List<XJPlcInfo> dAll)
        {
            
            List<XJPlcInfo> dpResultHigh = null;
            if(dAll.Count>0)           
            for (int i = 0; i < dAll.Count; i++)
            {
                if (dAll[i].ValueMode.Equals(Constant.DoubleMode))
                {
                    int addr = dAll[i].AbsAddr + 1;
                    int area = dAll[i].IntArea;
                    dpResultHigh = dAll.FindAll(
                    delegate (XJPlcInfo pf)
                    {
                        return ((pf.AbsAddr == (addr)) && (pf.IntArea == area));
                    });

                        if (dpResultHigh.Count > 0)
                        {
                            dAll[i].DoubleModeHigh = dpResultHigh[0];
                        }                        
                }
            }
            return true;
        }
        /// <summary>
        //将所有地址 解析为一个一个地址 然后进行整理 比如D0 D3 那就读取D0 D1 D2 D3 将相邻地址进行连续化
        //因为信捷的PLC 发送命令需要区分D 区和M区 一条指令同时包含D区和M区
        //忘了XY 是 从0~7的 
        /// </summary>
        /// <param name="plcInfoLst0"></param>
        private List<XJPlcInfo> InsertPlcInfo(List<XJPlcInfo> plcInfoLst0)
        {
            List<XJPlcInfo> plcInfoLst1 = new List<XJPlcInfo>();
            if (plcInfoLst0.Count < 1) return plcInfoLst1;
            for (int i = 0; i < plcInfoLst0.Count - 1; i++)
            {
                //当相邻两个地址相差4的时候，那就连续性添加进去，针对D 区而言  这个7 是按照信捷编译器所获取的数据
                if (((plcInfoLst0[i + 1].RelAddr - plcInfoLst0[i].RelAddr) < 7) && (plcInfoLst0[i + 1].IntArea == plcInfoLst0[i].IntArea))
                {
                    int m = 0;
                    m = plcInfoLst0[i + 1].RelAddr - plcInfoLst0[i].RelAddr;
                    if (m > 1)
                    {
                        XJPlcInfo[] tmpPlcInfoLst = XJPLCcmd.GetPlcInfo(plcInfoLst0[i].RelAddr, m, plcInfoLst0[i].StrArea, plcInfoLst0[i].ValueMode);
                        if (tmpPlcInfoLst.Count() > 0)
                        {
                            plcInfoLst1.AddRange(tmpPlcInfoLst);                         
                        }
                    }
                    
                }
            }
                                  
            plcInfoLst1 = plcInfoLst1.Union(plcInfoLst0.ToArray()).ToList<XJPlcInfo>();

            //排序
            plcInfoLst1= plcInfoLst1.OrderBy(x => x.AbsAddr).ToList();
            //去重复
            plcInfoLst1 = plcInfoLst1.Distinct(new ModelComparer()).ToList();

            
            //GC.Collect();
            //GC.WaitForPendingFinalizers();

            return plcInfoLst1;

        }

        /// <summary>
        /// 给每个dataform加个控件
        /// </summary>
        public void SetShowDataGridViewLst()
        {

            dgShowLst.Clear();
            for (int i = 0; i < dataFormLst.Count; i++)
            {
                DataGridView dgvView = new DataGridView();
                dgShowLst.Add(dgvView);
                dgvView.DataSource = dataFormLst[i];
            }

        }

        #endregion PLC数据需要采集的准备

        #region 处理数据 及更新表格
       
     
        void  Dataprocess(object sender, CommEventArgs e)
        {
            isShiftDataForm = false;
            //数据处理 以及更新 datagridview
            XJPLCcmd.UnPackCmdReadDMDataIn(dataForm,e.Byte_buffer, DPlcInfo, MPlcInfoAll);            
            Application.DoEvents();                   
            e.Byte_buffer = null;
            //GC.Collect();
            //GC.WaitForPendingFinalizers();
        }      

        #endregion 处理数据
        #region 写数据
        //写D寄存器支持多个使用
        private bool SetMultipleDArea(int Addr, int count, int[] value, string Area)
        {
            return comManager.SetMultipleDArea(Addr, count, value, Area);
        }
        //写m寄存器支持多个使用
        private bool SetMultipleMArea(int Addr, int count, int[] value, string Area)
        {
            return comManager.SetMultipleMArea(Addr, count, value, Area);
        }
        /// <summary>
        /// 测试 D区域写
        /// </summary>
        /// <returns></returns>
        public bool WriteDarea(int addr,int[] value,string area)
        {           
            return SetMultipleDArea(addr, value.Count(), value, area);
        }
        /// <summary>
        /// 测试M区域写
        /// </summary>
        /// <returns></returns>
        public bool WriteMarea(int addr,int count ,int[] value,String area)
        {                              
            return SetMultipleMArea(addr, count, value, area);
        }
        public bool WriteSingleDData(int relAddr, int value, string area, string mode)
        {
            int[] valueInt = new int[1];
            valueInt[0] = value;
            return WriteMultiPleDMData(relAddr, valueInt, area, mode);
        }
        public bool WriteSingleMData(int relAddr, int value, string area, string mode)
        {
            int[] valueInt = new int[1];
            valueInt[0] = value;
            return WriteMultiPleDMData(relAddr, valueInt, area, mode);
        }
        public bool WriteMultiPleDMData(int relAddr,int[] value,string area,string mode)
        {
            int intArea = XJPLCPackCmdAndDataUnpack.AreaGetFromStr(area);
            List<int> mvalueLst = new List<int>();
            if (intArea < Constant.M_ID)
            {
                if (mode.Equals(Constant.DoubleMode))
                {
                    int dcount = value.Count() * 2;
                    List<int> valueLst = new List<int>();

                    foreach (int valuevaluesingle in value)
                    {
                        int addr_high = (int)((valuevaluesingle & 0xFFFF0000) >> 16);
                        int addr_low = valuevaluesingle & 0xFFFF;
                        valueLst.Add(addr_low);
                        valueLst.Add(addr_high);
                    }

                    return SetMultipleDArea(relAddr, dcount, valueLst.ToArray(), area);
                }
                else
                {
                    if (mode.Equals(Constant.SingleMode))
                    {
                        return SetMultipleDArea(relAddr, value.Count(), value, area);
                    }
                    else return false;
                }
            }
            else
            {

                string strvalue="";
                
                //8个数据一组
                for (int i = 0; i < value.Count(); i++)
                {
                    string s = value[i].ToString();
                    strvalue=strvalue.Insert(0,s);
                                     
                    if ((((i + 1) % 8) == 0)||(i== value.Count()-1))
                    {
                        mvalueLst.Add(Convert.ToInt32(strvalue,2));
                        strvalue = "";
                    }
                }                
              }
                //这里传入的数据 应该是               
                return SetMultipleMArea(relAddr, value.Count(), mvalueLst.ToArray(), area);
            }
           
       
        #endregion

        #region 开始更新表格数据
        /******
        Thread DataUpDateThread;

        bool UpdateFlag = true;

        //这里先不更新 在数据处理的时候更新
        public void StartUpdateUI()
        {
            if (DataUpDateThread != null)
            {
                UpdateFlag = false;
                DataUpDateThread.Abort();
                DataUpDateThread = null;
            }
            UpdateFlag = true;
            DataUpDateThread = new Thread(DataUpdateToDataFormThreadMethod);
            DataUpDateThread.Start();
        }

        public void CloseUpdateUI()
        {
            if (DataUpDateThread != null)
            {
                UpdateFlag = false;
                DataUpDateThread.Abort();
                DataUpDateThread.Join();
                DataUpDateThread = null;                
            }

        }

        

        private void DataUpdateToDataFormThreadMethod()
        {
            while (UpdateFlag)
            {
                Thread.Sleep(10);
                UpdateUI0();
                Application.DoEvents();
            }
        }

        private void UpdataUI1(int rowindex,int colindex,string value)
        {

        }
        /// <summary>
        /// 这种更新 从dataform触发 遍历dplcinfo和Mplcinfo 
        /// 但还有一种是在数据处理的时候实时更新dataform
        /// </summary>
        private void UpdateUI0()
        {
            SetUpdateUIBuffer(dgShow);
                      
            if (DPlcInfo == null && MPlcInfoAll == null) return;      
            
            foreach (DataRow row in DataForm.Rows)
            {
                string area; //单字还是双字
                int mArea;
                string strNewValue = "-1";
                int index = DataForm.Rows.IndexOf(row);
                //取字母
                area = Regex.Replace(row["addr"].ToString(), "[0-9]", "", RegexOptions.IgnoreCase);
                mArea = XJPLCPackCmdAndDataUnpack.AreaGetFromStr(area);
                int[] pos = new int[2];
                if (int.TryParse(row["param1"].ToString(), out pos[0]) && (int.TryParse(row["param2"].ToString(), out pos[1])))
                {
                    if (mArea < Constant.M_ID && mArea > -1)
                    {
                        if(pos[0]< DPlcInfo.Count())
                        strNewValue = DPlcInfo[pos[0]].PlcValue.ToString();                      
                    }
                    else
                    {
                        if(pos[0]< MPlcInfoAll.Count && pos[1]< MPlcInfoAll[pos[0]].Count())
                        strNewValue = MPlcInfoAll[pos[0]][pos[1]].PlcValue.ToString();
                    }
                    
                    if (row!=null && row["value"]!=null && !row["value"].ToString().Equals(strNewValue))
                    {                    
                        if (dgShow !=null && dgShow.SelectedCells.Count > 0 && dgShow.SelectedCells[0].RowIndex == index && dgShow.SelectedCells[0].IsInEditMode)
                        {
                                                       
                        }
                        else                      
                        row["value"] = strNewValue;//FindValueFromDPlcInfo(mAddr, mArea, DSmode);
                    }
                    Thread.Sleep(5);
                    Application.DoEvents();
                }

            }
        }
        
        /// <summary>
        /// device 这里重新打包监控数据命令 那就要更改下
        /// </summary>
        private void SetUpdateUIBuffer(DataGridView dataGridView1)
        {
            if (IsShiftDataForm)
            {
                if (dataGridView1 != null)
                {
                    dataGridView1.Invoke((EventHandler)(delegate
                    {
                      dataGridView1.DataSource = DataForm;             
                    }));
            }
                IsShiftDataForm = false;
            }
        }


       ****/

        public void DeviceShutDown()
        {
            WatchCommTimer.Enabled = false;

            //CloseUpdateUI();
            comManager.Reset();

            CommError = 0;
            status = -1;

            GC.Collect();
            GC.WaitForPendingFinalizers();

        }
        #endregion
    }
   
}
