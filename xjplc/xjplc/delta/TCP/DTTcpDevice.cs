using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace xjplc.delta.TCP
{
    public class DTTcpDevice
    {

        CsvStreamReader CSVData;

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

        public List<DTTcpPlcInfo> DPlcInfo;

        public List<List<DTTcpPlcInfo>> DPlcInfoAll;
        //这里为什么要这样？ 因为M值不连续的分一组 再整理起来
        public List<List<DTTcpPlcInfo>> MPlcInfoAll;

        public List<DTTcpPlcInfo> MPlcInfo;

        DTTcpClientManager tcpClientManager;

        System.Timers.Timer WatchCommTimer;// new System.Timers.Timer(500); int CommError = 0;  //只能在通讯上之后 清零
                                           //机器状态
        int status=-1;

        int CommError = 0;  //只能在通讯上之后 清零
        public int Status
        {
            get { return status; }
            set { status = value; }
        }

        ServerInfo serverParam;

        public DTTcpDevice(List<string> filepath)
        {

            Init(filepath);
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
            set
            {
                isStopConnection = value;
               // comManager.IsNoConnection = value;
            }
        }

        void Init(List<string> filestr)
        {

            DataFormLst = new List<DataTable>();
            CSVData = new CsvStreamReader();
            dgShowLst = new List<DataGridView>();

            //获取监控数据 dataformLst 填充
            GetPlcDataTableFromFile(filestr);

            MPlcInfo = new List<DTTcpPlcInfo>();
            DPlcInfo = new List<DTTcpPlcInfo>();

            serverParam = ConstantMethod.LoadServerParam(Constant.ConfigServerPortFilePath);

            //设置端口 放前面 因为SetPlcReadDMData 要用tcpclientManager
            SetComm(serverParam);
            //监控第一个列表数据
            if (dataFormLst.Count > 0)
                SetPlcReadDMData(dataFormLst[0]);      

            //监控通讯
            WatchCommTimer = new System.Timers.Timer(Constant.XJRestartTimeOut);  //这里1.5 秒别改 加到常量里 工控机性能不行 

            //测试先隐藏
            WatchCommTimer.Enabled = false;

            WatchCommTimer.AutoReset = true;

            WatchCommTimer.Elapsed += new System.Timers.ElapsedEventHandler(WatchTimerEvent);

        }

       
        #region 通讯错误
        //通讯错误引发的事件
        private void WatchTimerEvent(object sender, EventArgs e)
        {
            if (!tcpClientManager.Status && IsStopConnection == false)
            {
                Status = Constant.DeviceNoConnection;
                CommError++;
                LogManager.WriteProgramLog(Constant.DeviceConnectionError);
                //先停了再说省的下一个定时事件又来
                WatchCommTimer.Enabled = false;
                if (CommError < Constant.DeviceErrorConnCountMax)
                    getDeviceData();
                else
                {
                    MessageBox.Show("设备连接失败！");
                    return;
                }
            }
        }
        #endregion
        #region 正常通讯


        public bool shiftDataFormSplit(int formid, int rowSt, int count)
        {
            DataTable dt = dataFormLst[formid].Clone();

            if ((rowSt > 0) && ((rowSt + count - 1) < dataFormLst[formid].Rows.Count) && count > 0)
            {
                for (int i = rowSt; i < rowSt + count; i++)
                {
                    dt.ImportRow(dataFormLst[formid].Rows[i]);
                }
            }
           // splitDataForm(dt, dataFormLst[formid]);
            isShiftDataForm = true;

            tcpClientManager.IsRePackCmdReadDMDataOut = true; 

            return true;
        }
        public void startRepack()
        {
            tcpClientManager.IsRePackCmdReadDMDataOut = true;
            tcpClientManager.IsRepackDone = false;

        }

        public void doneRepack()
        {
            tcpClientManager.IsRePackCmdReadDMDataOut = false;
            tcpClientManager.IsRepackDone = true;

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
                dataForm = dataFormLst[formid];
                int dAreaCount = 0;

                foreach (DataRow r in dataFormLst[formid].Rows)
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
                    //屏幕分割
                    //splitDataForm(dt, dataFormLst[formid]);
                }
                else
                if (dataForm != null && dataForm.Rows.Count > 0)
                {                  
                    PackCmdReadDMDataOut(dataForm);
                }

                isShiftDataForm = true;

                doneRepack();

               return true;

            }
            else return false;

        }      

        //重置端口 传入PLC数据读取文件和 端口
        public void RestartConneect(DataTable dt)
        {
            DeviceShutDown();

            ConstantMethod.Delay(50);

            serverParam = ConstantMethod.LoadServerParam(Constant.ConfigServerPortFilePath);
            //这里设置 comm要在之前 不然 tcpclienManageer 在setcomm中 重新清空了
            if (serverParam.server_Ip != null)
                SetComm(serverParam);
            if (dataFormLst.Count > 0)
                SetPlcReadDMData(dt);
           

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
            Status = Constant.DeviceNoConnection;

            if (tcpClientManager.OpenTcpClient())
            {
                ConstantMethod.Delay(500);
                CommError = 0;
                Status = Constant.DeviceConnected;
                //开始设置数据先读取D区域吧
               
                return true;
            }

            return false;
        }
        /// <summary>
        /// 设置端口 传入串口数据
        /// 如果以后要重新连接 或者更改串口了 可以使用这个函数
        /// </summary>
        /// <param name="portParam0"></param>
        public void SetComm(ServerInfo  portParam0)
        {
            if (tcpClientManager != null)
            {
                tcpClientManager.Reset();
                tcpClientManager = null;
            }



            tcpClientManager = new DTTcpClientManager(portParam0);
            //设置数据处理委托事件
            tcpClientManager.EventDataProcess += new xjplc.socDataProcess(Dataprocess);

            //命令打包类重新确认
           // comManager.SetDTPLCcmd(DTPLCcmd);

            //GC.Collect();
            //GC.WaitForPendingFinalizers();
        }
        public void SetPlcReadDMData(DataTable dt)
        {

            //确认表格 //这个要隔离出来 方便以后可以单独调用 针对用户更改读取内容

            if (dt != null && dt.Rows.Count > 0)
            {
                dataForm = dt;
                //dgShow = dg;
                PackCmdReadDMDataOut(dataForm);
                //在建立连接的时候 切换表格数据源
                //if (dgShow != null)
                // dgShow.DataSource = dataForm;


            }
            //GC.Collect();
            //GC.WaitForPendingFinalizers();
        }
        /// <summary>
        /// 如果用户需要重新制定读取一些D区 M区的话可以先设置保存读取PLC 内容的文件
        /// 读取后生成命令
        /// </summary>
        public void SetPlcReadDMData(string filename)
        {


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
                        ConstantMethod.AppExit();
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
            
            List<DTTcpPlcInfo> plcInfoLst = new List<DTTcpPlcInfo>();
            DPlcInfo.Clear();
            MPlcInfo.Clear(); 
            
            foreach (DataRow row in dataForm0.Rows)
            {
                int mAddr = -1;
                int count = -1;
                int mArea = -1;           
                string strSplit2="";
                string DSmode=""; //单字还是双字
                #region 获取地址个数区域 创建plcinfo类 并添加到集合
                if (row == null) return;

                if(!getAddrAndArea(row,ref mAddr, ref mArea, ref DSmode,ref strSplit2, ref count)) continue;
                //这里数组进行统计                 
                //传入数据起始地址 个数 区域 模式
                
                DTTcpPlcInfo[] tmpInfoLst = DTTcpCmdPackAndDataUnpack.GetPlcInfo(mAddr, count, strSplit2, DSmode);


                if (tmpInfoLst.Count() > 0)
                {                  
                    plcInfoLst.AddRange(tmpInfoLst);
                }
                #endregion
            }
            #region 排序 去重复 统计DM 起始点
            //排序 按照绝对地址 
            plcInfoLst = plcInfoLst.OrderBy(x => x.AbsAddr).ToList();
            //去重复 
            plcInfoLst = plcInfoLst.Distinct(new ModelComparerDTTcpInfo()).ToList();

            //分离D 区 M区
            DPlcInfo = plcInfoLst.FindAll(t => t.IntArea > (Constant.MXAddrId));
            MPlcInfo = plcInfoLst.FindAll(t => t.IntArea < (Constant.QWAddrId));

            #endregion
            #region  根据断点 建立命令的表格缓冲lst 然后创建读取DM区域的命令 

           tcpClientManager.ClearCmd();
            
           DTTcpCmdPackAndDataUnpack. 
           PackReadDCmd(DPlcInfo.ToArray(), tcpClientManager.ReadDCmdOut,  tcpClientManager.ReadDCmdIn);

      
           DTTcpCmdPackAndDataUnpack.
           PackReadMCmd(MPlcInfo.ToArray(), tcpClientManager.ReadMCmdOut, tcpClientManager.ReadMCmdIn);

            //dataform表格里面地址对应dplcinfo和mplcinfo的数据对应起来
           FindIndexInPlcInfo(dataForm0, DPlcInfo, MPlcInfo);

          

        }
        
        //根据字符 获取地址和区域
        private string getAreaByStr(string strSplit1)
        {
            string area="";
            int addr=0;
           ConstantMethod. getAddrAndAreaByStr(strSplit1, ref addr, ref area);
            return area;
        }
        //根据字符 获取地址和区域
        //根据一行的数据 返回固定的 plc单元格参数 这个函数 是返回相对地址的 
        private int getAddrByStr(string strSplit1)
        {
            string area = "";
            int addr = 0;
            ConstantMethod.getAddrAndAreaByStr(strSplit1, ref addr, ref area);

            return addr;
        }
        //根据字符 获取地址和区域 返回相对地址 还有数量 还有区域编号
        private bool getAddrAndArea(DataRow row, ref int mAddr, ref int mArea, ref string DSmode,ref string mAreaStr,ref  int count)
        {

            DSmode = row["mode"].ToString().Trim();
         
            string area="";///地址区域          
            string valueStr = row["addr"].ToString().Trim();

            ConstantMethod.getAddrAndAreaByStr(valueStr, ref mAddr, ref area);

            //地址超了 无效 暂且定XDM 最大69999
            if ((mAddr < 0) || (mAddr > Constant.DTTCPMAXAddr))
            {
                return false;
            }               
            if (!int.TryParse(row["count"].ToString().Trim(), out count))
            {
                return false;
            }
            mArea = DTTcpCmdPackAndDataUnpack.GetIntAreaFromStr(area);

            mAreaStr = area;

            return true;
        }
        //一行中获取地址和区域 /根据一行的数据 返回固定的 plc单元格参数 这个函数 是返回绝对地址的 
        private bool  getAddrAndArea(DataRow  row,ref int mAddr, ref int mArea ,ref string DSmode)
        {
            DSmode = row["mode"].ToString().Trim();

            string area = "";///地址区域          
            string valueStr = row["addr"].ToString().Trim();
            int count = 0;
            ConstantMethod.getAddrAndAreaByStr(valueStr, ref mAddr, ref area);

            //地址超了 无效 暂且定XDM 最大69999
            if ((mAddr < 0) || (mAddr > Constant.DTTCPMAXAddr))
            {
                return false;
            }
            if (!int.TryParse(row["count"].ToString().Trim(), out count))
            {
                return false;
            }

            mArea = DTTcpCmdPackAndDataUnpack.GetIntAreaFromStr(area);

            mAddr = DTTcpCmdPackAndDataUnpack.GetAbsAddrFromStr(mAddr, area);

            return true;
        }      
        /// <summary>
        /// dataform中的数据 与 dplcinfo 和 mplcinfoall 集合对应起来 这样 更新速度会很快
        /// </summary>
        /// <param name="datafm"></param>
        /// <param name="dAll"></param>
        /// <param name="mAll"></param>
        /// <returns></returns>
        private bool FindIndexInPlcInfo(DataTable datafm, List<DTTcpPlcInfo> dAll, List<DTTcpPlcInfo> mAll)
        {
            foreach (DataRow row in datafm.Rows)
            {
                if (row == null) continue;

                int mAddr=-1;
                int mArea=-1;
                string DSmode = "";
                if (!getAddrAndArea(row, ref mAddr, ref mArea,ref DSmode)) continue;
                if (mArea < 0) continue;
                if (row["param1"] != null && row["param2"] != null)
                {
                    int[] s = FindValueIndexFromDPlcInfo(mAddr, mArea, DSmode);

                    row["param1"] = s[0].ToString();
                    row["param2"] = s[1].ToString();
                    //让集合 把dataform 也保存下 方便更新数据
                    if (mArea >Constant.MXAddrId)
                    {
                        if (!(s[0] > -1)) continue;
                        dAll[s[0]].BelongToDT = datafm;
                        dAll[s[0]].Row = datafm.Rows.IndexOf(row);
                    }
                    else
                    {

                        if (!(s[0] > -1))continue;
                        mAll[s[0]].BelongToDT = datafm;
                        mAll[s[0]].Row = datafm.Rows.IndexOf(row);
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
            List<DTTcpPlcInfo> dpResultLow = null;

            if (area > Constant.MXAddrId)
            {
                dpResultLow = DPlcInfo.FindAll(
                delegate (DTTcpPlcInfo pf)
                {
                    return (pf.AbsAddr == addr) && (pf.IntArea.Equals(area));
                });
                if (dpResultLow.Count > 0)
                    //单字的话就是D区和M区的情况了
                    result[0] = DPlcInfo.IndexOf(dpResultLow[0]);
            }
            else
            {
               
                    dpResultLow = MPlcInfo.FindAll(
                    delegate (DTTcpPlcInfo pf)
                    {
                        return (pf.AbsAddr == addr) && (pf.IntArea.Equals(area));
                    });

                    //找到M了 

                    if (dpResultLow.Count > 0)
                    {           
                        result[0] = MPlcInfo.IndexOf(dpResultLow[0]);                 
                    }
               

            }


            return result;

        }
        /// <summary>
        /// 在传入的集合里找到 高字节位置 针对双字寄存器
        /// </summary>
        /// <returns></returns>       
        
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

        void Dataprocess(object sender, SocEventArgs e)
        {

            if (isShiftDataForm)
            {
                isShiftDataForm = false;
                return;
            }
            if (e.Byte_buffer.Count() < 12) return;

            List<byte> data = new List<byte>();
            List<byte[]> value = new List<byte[]>();

            data.AddRange(e.Byte_buffer);
         
            byte[] s = data.Skip(7).Take(2).ToArray();
            if ((!ConstantMethod.compareByteStrictly(s, Constant.DTTcpFunctionReadBitCmd))
                && (!(ConstantMethod.compareByteStrictly(s, Constant.DTTcpFunctionReadByteCmd)))) return;
       
           byte[] s1 = data.Skip(9).Take(2).ToArray();
            Array.Reverse(s1);
           int ss = BitConverter.ToInt16(s1, 0);
           if (ss < 1) return; 
            if (data.Count>11)
            data.RemoveRange(0, 11);
            //这里执行一种数据获取当时直到数据数量取完
            while (data.Count > 0)
            {
                Application.DoEvents();
                if (data.Count > 1)
                {
                    byte[] dataArray = data.Skip(0).Take(2).ToArray();
                    Array.Reverse(dataArray);
                    int m = BitConverter.ToInt16(dataArray, 0);
                    data.RemoveRange(0, 2);
                    byte[] dataArray0 = data.Skip(0).Take(m).ToArray();
                  
                    value.Add(dataArray0);
                    data.RemoveRange(0, m);


                }
            }

            if (ConstantMethod.compareByteStrictly(s,Constant.DTTcpFunctionReadBitCmd))
            {
                //m
                if (MPlcInfo.Count() == value.Count())
                {
                    for (int i = 0; i < MPlcInfo.Count(); i++)
                    {
                        MPlcInfo[i].ByteValue = value[i];
                    }
                }
            }
            else
            {
                if (ConstantMethod.compareByteStrictly(s, Constant.DTTcpFunctionReadByteCmd))
                {
                    //D 
                    if (DPlcInfo.Count() == value.Count())
                    {
                        for (int i = 0; i < DPlcInfo.Count(); i++)
                        {
                            DPlcInfo[i].ByteValue = value[i];                           
                        }
                    }

                }
            }
            //update form
           updateData();
            //数据处理 以及更新 datagridview
            // DTPLCcmd.UnPackCmdReadDMDataIn(dataForm,e.Byte_buffer, DPlcInfo, MPlcInfoAll);            
            Application.DoEvents();
            e.Byte_buffer = null;
        
          
        }
        private void updateData()
        {
            updateForm(dataForm, DPlcInfo);
            updateForm(dataForm, MPlcInfo);
        }
     
        private void updateForm(DataTable datform,  List<DTTcpPlcInfo> mplcInfoLst)
        {
      
            int mCount = mplcInfoLst.Count;

            if ((mCount > 0))
            {           
                for (int i = 0; i < mCount; i++)
                {                                
                    if (!datform.Rows[mplcInfoLst[i].Row]["value"].ToString().Equals(mplcInfoLst[i].PlcValue)
                            && !mplcInfoLst[i].IsInEdit)
                    {
                                              
                        int m10addr = getAddrByStr(datform.Rows[mplcInfoLst[i].Row]["addr"].ToString());
                    
                        if (datform.Rows[mplcInfoLst[i].Row]["addr"].ToString().Contains(m10addr.ToString())
                            ||
                          datform.Rows[mplcInfoLst[i].Row]["addr"].ToString().Contains(mplcInfoLst[i].StrArea)
                            )
                        {
                            datform.Rows[mplcInfoLst[i].Row]["value"] = mplcInfoLst[i].PlcValue;
                            if (datform.Columns.Contains("param7"))
                            {
                                string ration = datform.Rows[mplcInfoLst[i].Row]["param7"].ToString();                                
                                double rationDouble = 1;
                                double valueDouble = 1;
                                double minDouble = -1000;
                                double maxDouble = 1000000;
                                                           
                                if (double.TryParse(ration, out rationDouble)
                                    && double.TryParse(mplcInfoLst[i].PlcValue, out valueDouble))
                                {

                                    valueDouble = valueDouble / rationDouble;
                                    //大小比较 如果没有参数8 9 那就直接显示数值 如果有大小那就显示比较结果
                                    if (datform.Columns.Contains("param8") && datform.Columns.Contains("param9"))
                                    {
                                        string minStr = datform.Rows[mplcInfoLst[i].Row]["param9"].ToString();
                                        string maxStr = datform.Rows[mplcInfoLst[i].Row]["param8"].ToString();

                                        if (double.TryParse(maxStr, out maxDouble) && (double.TryParse(minStr, out minDouble)))
                                        {
                                            if (valueDouble <= maxDouble && valueDouble > minDouble)
                                            {
                                                datform.Rows[mplcInfoLst[i].Row]["param6"] = valueDouble.ToString();
                                            }
                                            else
                                            {
                                                datform.Rows[mplcInfoLst[i].Row]["param6"] = Constant.dataOutOfRange;
                                            }
                                        }else datform.Rows[mplcInfoLst[i].Row]["param6"] = valueDouble.ToString();
                                    }
                                    else
                                    {
                                        datform.Rows[mplcInfoLst[i].Row]["param6"] = valueDouble.ToString();
                                    }                                                                      
                                }
                                else
                                {
                                    datform.Rows[mplcInfoLst[i].Row]["param6"] = mplcInfoLst[i].PlcValue;
                                }                              
                            }
                            
                        }

                    }                   
                }

            }
        }
        #endregion 处理数据
        #region 写数据


        #region 针对plcinfosimple 进行操作

        public bool SetDValue(DTPlcInfoSimple p, string[] value0)
        {          
            if (p != null && p.BelongToDataform != null)
            {
                return WriteMultiPleDMData(p.Addr, value0, p.Area, p.Mode);
            }
            return false;        
        }      
        public bool SetMValueON(DTPlcInfoSimple p)
        {
            string[] value0 = { "1" };         
            if (p != null && p.BelongToDataform != null)
            {
               return  WriteMultiPleDMData(p.Addr, value0, p.Area, p.Mode);
            }
            return false;          
        }
        public bool SetMValueOFF(DTPlcInfoSimple p)
        {
            string[] value0 = { "0" };
            if (p != null && p.BelongToDataform != null)
            {
                return WriteMultiPleDMData(p.Addr, value0, p.Area, p.Mode);
            }
            return false;
        }
        public void SetMValueON2OFF(DTPlcInfoSimple p)
        {
            SetMValueON(p);
           // ConstantMethod.Delay(500);
            SetMValueOFF(p);
        }
        public void SetMValueOFF2ON(DTPlcInfoSimple p)
        {
           SetMValueOFF(p);
          // ConstantMethod.Delay(1000);             
           SetMValueON(p);
        }
        public bool SetMultiPleDValue(DTPlcInfoSimple stPlcInfoSimple, string[] value0)
        {
          
            if (stPlcInfoSimple != null && stPlcInfoSimple.BelongToDataform != null)
            {
                return WriteMultiPleDMData(
                    stPlcInfoSimple.Addr,
                    value0,
                    stPlcInfoSimple.Area,
                    stPlcInfoSimple.Mode);             
            }
            return false;
        }

        #endregion
        //写D寄存器支持多个使用
              

       public byte[] getByteFromMode(string value,string Mode)
        {
            List<byte> valueByte = new List<byte>();
            Dictionary<string, int> modeByteCount = new Dictionary<string, int>();

            ConstantMethod.ArrayToDictionary(modeByteCount,Constant.tcpType,Constant.tcpTypeByteCount);

            //1字节 2 字节 4字节 8字节

            switch (modeByteCount[Mode])
            {

                case 1:
                    {
                        int data1Byte = 0;

                        if(int.TryParse(value,out data1Byte))
                        {
                            valueByte.Add((byte)(data1Byte));
                        }

                        break;
                    }
                case 2:
                    {                   
                        //uint
                        int data1Byte = 0;

                        if (int.TryParse(value, out data1Byte))
                        {
                            valueByte.AddRange(ConstantMethod.getDataLowHighByte(data1Byte));
                        }

                        break;
                    }
                case 4:
                    {
                        //int 占用4个 和单精度浮点
                        int data1Byte = 0;
                        if (Mode.Contains(Constant.REAL))
                        {
                            float dataFloat = 0;
                            if (float.TryParse(value, out dataFloat))
                            {

                                valueByte.AddRange(BitConverter.GetBytes(dataFloat));
                            }                           
                        }
                        else
                        if (int.TryParse(value, out data1Byte))
                        {
                            
                            valueByte.AddRange(BitConverter.GetBytes(data1Byte));
                        }

                        break;
                    }
                case 8:
                    {
                        //8字节 就双精度浮点 和LONG INT 
                        Int64 data1Byte = 0;
                        if (Mode.Contains(Constant.REAL))
                        {
                            double dataFloat = 0;
                            if (double.TryParse(value, out dataFloat))
                            {

                                valueByte.AddRange(BitConverter.GetBytes(dataFloat));
                            }
                        }
                        else
                        if (Int64.TryParse(value, out data1Byte))
                        {

                            valueByte.AddRange(BitConverter.GetBytes(data1Byte));
                        }


                        break;
                    }
            }

            return valueByte.ToArray();
        }   
           
       public bool WriteMultiPleDMData(int relAddr, string[] value, string area, string mode)
        {         
            List<byte[]> dvalueLst = new List<byte[]>();
            //根据数据空间 划分数据个数          
            for (int i = 0; i < value.Count(); i++)
            {
                dvalueLst.Add(getByteFromMode(value[i], mode));
            }
            //这里传入的数据 应该是  
            return tcpClientManager.SetMultipleDMArea(relAddr, value.Count(), dvalueLst, area);   
        }
        #endregion

        public void DeviceShutDown()
        {
            WatchCommTimer.Enabled = false;

            //CloseUpdateUI();
            tcpClientManager.Reset();

            CommError = 0;
            Status = -1;

            

        }
        #endregion
    }
}
