using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows.Forms;

namespace xjplc.delta
{
    //管理一堆设备的集合类  
    public class YBDTDevice 
    {

        CsvStreamReader CSVData;
        DataTable dataForm;

        SocServerManager socManager;
        public xjplc.SocServerManager SocManager
        {
            get { return socManager; }
            set { socManager = value; }
        }

        int status ; //
        public int Status
        {
            get { return status; }
            set { status = value; }
        }
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
        public List<DTPlcInfo> DPlcInfo;
        //这里为什么要这样？ 因为M值不连续的分一组 再整理起来
        public List<List<DTPlcInfo>> MPlcInfoAll;

        Socket socClient;
        public System.Net.Sockets.Socket SocClient
        {
            get { return socClient; }
            set { socClient = value; }
        }

        private DTPLCPackCmdAndDataUnpack dTPLCcmd;
        private bool isShiftDataForm;

        public xjplc.DTPLCPackCmdAndDataUnpack DTPLCcmd
        {
            get { return dTPLCcmd; }
            set { dTPLCcmd = value; }
        }

        public System.Timers.Timer WatchCommTimer { get; private set; }

        void InitCmd(List<string> filestr, Socket soc)
        {
            DataFormLst = new List<DataTable>();
            CSVData = new CsvStreamReader();
            dgShowLst = new List<DataGridView>();
            SocClient = soc;
            //获取监控数据 dataformLst 填充
            GetPlcDataTableFromFile(filestr);

            //监控第一个列表数据
            if (dataFormLst.Count > 0)
                SetPlcReadDMData(dataFormLst[0]);

            SetSocManager();

            //监控通讯
            WatchCommTimer = new System.Timers.Timer(Constant.XJRestartTimeOut);  //这里1.5 秒别改 加到常量里 工控机性能不行 

            //测试先隐藏
            WatchCommTimer.Enabled = false;

            WatchCommTimer.AutoReset = true;

            WatchCommTimer.Elapsed += new System.Timers.ElapsedEventHandler(WatchTimerEvent);

        }
        public YBDTDevice(List<string> filestr, Socket soc)
        {
            DTPLCcmd = new DTPLCPackCmdAndDataUnpack();
            InitCmd(filestr, soc);

        }
        public void SetConnectMode(int i)
        {
            if (i < 2)
            {
                DTPLCcmd.ConnectMode = i;
            }
        }
        public YBDTDevice(List<string> filestr, Socket soc,int conneMode)
        {
            DTPLCcmd = new DTPLCPackCmdAndDataUnpack();
            SetConnectMode(conneMode);
            InitCmd(filestr, soc);

        }

        public bool shiftDataForm(int formid)
        {
            if (dataFormLst[formid] != null && dataFormLst[formid].Rows.Count > 0)
            {
                dataForm = dataFormLst[formid];
                int dAreaCount = 0;

                foreach (DataRow r in dataFormLst[formid].Rows)
                {
                    if (r["addr"].ToString().Contains(Constant.strDMArea[0]))
                    {
                        dAreaCount++;
                    }
                }
               
                if (dataForm != null && dataForm.Rows.Count > 0)
                    PackCmdReadDMDataOut(dataForm);

                isShiftDataForm = true;

                SocManager.IsSetReadDDataOut = true;

                return true;

            }
            else return false;

        }
        public bool getDeviceData()
        {
            //WatchCommTimer.Enabled = true;
            Status = Constant.DeviceNoConnection;

            if (DTPLCcmd != null)

                if (SocManager.ConnectMachine())
                {

                    Status = Constant.DeviceConnected;
                    //开始设置数据先读取D区域吧
                    return true;
                }

            return false;
        }
        private void WatchTimerEvent(object sender, ElapsedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void SetSocManager()
        {
            SocManager = new SocServerManager(SocClient);
            SocManager.SetDTPLCcmd(DTPLCcmd);
           
            SocManager.EventDataProcess += new socDataProcess(Dataprocess);

            SocManager.Status = true; //能够建立 就代表状态是好的


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
        void PackCmdReadDMDataOut(DataTable dataForm0)
        {
            List<DTPlcInfo> plcInfoLst = new List<DTPlcInfo>();
            List<DTPlcInfo> MPlcInfo = new List<DTPlcInfo>();
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
                if (DSmode.Equals(Constant.DoubleMode) && (DTPLCPackCmdAndDataUnpack.AreaGetFromStr(strSplit2) < Constant.M_ID))
                {
                    count = count * 1; //默认都是双字了
                }
                //传入数据起始地址 个数 区域 模式
                DTPlcInfo[] tmpInfoLst = DTPLCcmd.GetPlcInfo(mAddr, count, strSplit2, DSmode);

                if (tmpInfoLst.Count() > 0) plcInfoLst.AddRange(tmpInfoLst);
                #endregion
            }
            #region 排序 去重复 统计DM 起始点
            //排序 按照绝对地址 
            plcInfoLst = plcInfoLst.OrderBy(x => x.AbsAddr).ToList();
            //去重复 
            plcInfoLst = plcInfoLst.Distinct(new ModelComparerDT()).ToList();

            //分离D 区 M区
            DPlcInfo = plcInfoLst.FindAll(t => t.IntArea < (Constant.HSD_ID + 1));
            MPlcInfo = plcInfoLst.FindAll(t => t.IntArea > (Constant.HSD_ID));

            


            plcInfoLst = DPlcInfo.Union(MPlcInfo).ToList<DTPlcInfo>();
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
            //FindHighPlcInfo(DPlcInfo);
            //台达PLC 默认是双字 那双字 就要自己创建了
            CreateHighPlcInfo(DPlcInfo);
            //这里M区麻烦一点 分成n个M单元组 每个单元组 有个起始地址
            MPlcInfoAll = new List<List<DTPlcInfo>>();
            for (int i = 0; i < addrLst.Count; i++)
            {
                List<DTPlcInfo> mplst = new List<DTPlcInfo>();
                if (idLst[i] > Constant.HSD_ID)
                {
                    for (int k = 0; k < addrcount[i]; k++)
                    {
                        DTPlcInfo p = new DTPlcInfo();
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
            //dataform表格里面地址对应dplcinfo和mplcinfo的数据对应起来
            FindIndexInPlcInfo(dataForm0, DPlcInfo, MPlcInfoAll);
            DTPLCcmd.PackSetCmdReadDDataOut(DPlcInfo);
            DTPLCcmd.PackSetCmdReadMDataOut(MPlcInfoAll);

            // DTPLCcmd.PackCmdReadDMDataOut(addrLst, idLst, addrcount, 5 + DPlcInfo.Count * 2 + mCount);

            #endregion
            addrLst = null;
            idLst = null;
            addrcount = null;
            breakPoint = null;
            
        }

          
        private bool FindIndexInPlcInfo(DataTable datafm, List<DTPlcInfo> dAll, List<List<DTPlcInfo>> mAll)
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
                strSplit2 = strSplit2.Trim();

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

                mArea = DTPLCPackCmdAndDataUnpack.AreaGetFromStr(strSplit2);

                if (mArea > Constant.HM_ID)
                {
                    mAddr = ConstantMethod.GetXYAddr8To10(mAddr);

                }

                mAddr = DTPLCPackCmdAndDataUnpack.AreaGetFromStr(mAddr, strSplit2);

                if (mArea < 0) continue;
                if (row["param1"] != null && row["param2"] != null)
                {
                    int[] s = FindValueIndexFromDPlcInfo(mAddr, mArea, DSmode);

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
            List<DTPlcInfo> dpResultLow = null;


            if (area < Constant.HSD_ID + 1)
            {
                dpResultLow = DPlcInfo.FindAll(
                delegate (DTPlcInfo pf)
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
                    delegate (DTPlcInfo pf)
                    {
                        return (pf.AbsAddr == addr) && (pf.IntArea.Equals(area));
                    });

                    //找到M了 

                    if (dpResultLow.Count > 0)
                    {
                        result[0] = i; ;
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
        private bool FindHighPlcInfo(List<DTPlcInfo> dAll)
        {

            List<DTPlcInfo> dpResultHigh = null;
            if (dAll.Count > 0)
                for (int i = 0; i < dAll.Count; i++)
                {
                    if (dAll[i].ValueMode.Equals(Constant.DoubleMode))
                    {
                        int addr = dAll[i].AbsAddr + 1;
                        int area = dAll[i].IntArea;
                        dpResultHigh = dAll.FindAll(
                        delegate (DTPlcInfo pf)
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
        private bool CreateHighPlcInfo(List<DTPlcInfo> dAll)
        {


            if (dAll.Count > 0)
                for (int i = 0; i < dAll.Count; i++)
                {
                    DTPlcInfo tmpInfoHigh = new DTPlcInfo((dAll[i].RelAddr + 1), dAll[i].StrArea, dAll[i].ValueMode);
                    tmpInfoHigh.Xuhao = -1;
                    dAll[i].DoubleModeHigh = tmpInfoHigh;
                }
            return true;
        }
        void Dataprocess(object sender, SocEventArgs e)
        {
            isShiftDataForm = false;

            if ((DTPLCcmd.CmdReadDDataOut != null))
            {

                if ((SocManager.IsReadingD) && (!SocManager.IsReadingM))
                {
                    DTPLCcmd.UnPackCmdReadDDataIn(dataForm, e.Byte_buffer, DPlcInfo);
                }
            }

            if ((DTPLCcmd.CmdReadMDataOut != null))
            {

                if ((SocManager.IsReadingM) && (!SocManager.IsReadingD))
                {
                    DTPLCcmd.UnPackCmdReadMDataIn(dataForm, e.Byte_buffer, MPlcInfoAll);
                }

            }
            //数据处理 以及更新 datagridview
            // DTPLCcmd.UnPackCmdReadDMDataIn(dataForm,e.Byte_buffer, DPlcInfo, MPlcInfoAll);            
            Application.DoEvents();
            e.Byte_buffer = null;
            //GC.Collect();
            //GC.WaitForPendingFinalizers();
        }


        public void GetPlcDataTableFromFile(List<string> strfile)
        {
            if (strfile != null && strfile.Count > 0)
                foreach (string s in strfile)
                {
                    if (ConstantMethod.FileIsUsed(s))
                    {
                        MessageBox.Show(Constant.FileIsInUse);
                     //   ConstantMethod.AppExit();


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
                            //ConstantMethod.AppExit();
                        }
                    }
                    else
                    {
                        MessageBox.Show(Constant.ReadPlcInfoFail);
                        //Application.Exit();
                        //System.Environment.Exit(0);
                    }
                }


        }

        #region 写数据操作

        //写D寄存器支持多个使用
        private bool SetMultipleDArea(int Addr, int count, int[] value, string Area)
        {
            return SocManager.SetMultipleDArea(Addr, count, value, Area);
        }
        //写m寄存器支持多个使用
        private bool SetMultipleMArea(int Addr, int count, int[] value, string Area)
        {
            return SocManager.SetMultipleMArea(Addr, count, value, Area);
        }
        /// <summary>
        /// 测试 D区域写
        /// </summary>
        /// <returns></returns>
        public bool WriteDarea(int addr, int[] value, string area)
        {
            return SetMultipleDArea(addr, value.Count(), value, area);
        }

        /// <summary>
        /// 测试M区域写
        /// </summary>
        /// <returns></returns>
        public bool WriteMarea(int addr, int count, int[] value, String area)
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
        public bool WriteMultiPleDMData(int relAddr, int[] value, string area, string mode)
        {
            int intArea = DTPLCPackCmdAndDataUnpack.AreaGetFromStr(area);
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

                string strvalue = "";

                //8个数据一组
                for (int i = 0; i < value.Count(); i++)
                {
                    string s = value[i].ToString();
                    strvalue = strvalue.Insert(0, s);

                    if ((((i + 1) % 8) == 0) || (i == value.Count() - 1))
                    {
                        mvalueLst.Add(Convert.ToInt32(strvalue, 2));
                        strvalue = "";
                    }
                }
            }
            //这里传入的数据 应该是               
            return SetMultipleMArea(relAddr, value.Count(), mvalueLst.ToArray(), area);
        }

        public bool SetDValue(DTPlcInfoSimple p, int value0)
        {

            if (p != null && p.BelongToDataform != null)
            {
                WriteSingleDData(p.Addr, value0, p.Area, p.Mode);
            }

            ConstantMethod.DelayWriteCmdOk(Constant.XJConnectTimeOut, ref value0, ref p);

            if (value0 == p.ShowValue) return true; else return false;
        }

        public bool SetMValueON(DTPlcInfoSimple p)
        {
            int value0 = 1;
            if (p != null && p.BelongToDataform != null)
            {
                WriteSingleMData(p.Addr, value0, p.Area, p.Mode);
            }

            ConstantMethod.DelayWriteCmdOk(Constant.XJConnectTimeOut, ref value0, ref p);

            if (value0 == p.ShowValue) return true; else return false;
        }
        public bool SetMValueOFF(DTPlcInfoSimple p)
        {
            int value0 = 0;
            if (p != null && p.BelongToDataform != null)
            {
                WriteSingleMData(p.Addr, value0, p.Area, p.Mode);
            }

            ConstantMethod.DelayWriteCmdOk(Constant.XJConnectTimeOut, ref value0, ref p);

            if (value0 == p.ShowValue) return true; else return false;
        }
        public void SetMValueON2OFF(DTPlcInfoSimple p)
        {
            SetMValueON(p);
            SetMValueOFF(p);
        }
        public void SetMValueOFF2ON(DTPlcInfoSimple p)
        {
            SetMValueOFF(p);
            SetMValueON(p);
        }
        public bool SetMultiPleDValue(DTPlcInfoSimple stDTPlcInfoSimple, int[] value0)
        {
            if (stDTPlcInfoSimple != null && stDTPlcInfoSimple.BelongToDataform != null)
            {
                WriteMultiPleDMData(
                    stDTPlcInfoSimple.Addr,
                    value0,
                    stDTPlcInfoSimple.Area,
                    stDTPlcInfoSimple.Mode);
                ConstantMethod.DelayWriteCmdOk(Constant.WriteCommTimeOut, ref value0[0], ref stDTPlcInfoSimple);
                //可能时间太久 要等下 
                if (value0[0] == stDTPlcInfoSimple.ShowValue) return true; else return false;

            }
            return false;
        }




        #endregion
    }
}