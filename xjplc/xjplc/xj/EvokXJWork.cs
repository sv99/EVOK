using FastReport;
using FastReport.Barcode;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace xjplc
{
    public class EvokXJWork
    {
        //用户排版数据
        DataTable userDataTable;

        List<string> errorList;
        public System.Collections.Generic.List<string> ErrorList
        {
            get { return errorList; }
            set { errorList = value; }
        }
        OptSize optSize;
        //设备类
        EvokXJDevice evokDevice;

        int deviceProperty;
        public int DeviceProperty
        {
            get { return deviceProperty; }
            set { deviceProperty = value; }
        }
        string deviceName;
        public string DeviceName
        {
            get { return deviceName; }
            set {
                evokDevice.setDeviceId(value);
                deviceName = value;
            }
        }
        int currentPageId = -1;
        public int CurrentPageId
        {
            get { return currentPageId; }
            set { currentPageId = value; }
        }
        //显示工作信息
        RichTextBox rtbWork;

        //label 显示工作的状态信息
        Label lblStatus;

        //打印的报表
        FastReport.Report printReport;

        //print
        int minPrintSize=0;
        string minPrinterName="";
        //显示优化文本框
        RichTextBox rtbResult;

        ConfigFileManager paramFile;

        //打条码模式
        int printBarCodeMode = 0;

        public int PrintBarCodeMode
        {
            get { return printBarCodeMode; }
            set { printBarCodeMode = value; }
        }

        //
        List<List<PlcInfoSimple>> allPlcSimpleLst;
        public System.Collections.Generic.List<System.Collections.Generic.List<xjplc.PlcInfoSimple>> AllPlcSimpleLst
        {
            get { return allPlcSimpleLst; }
            set { allPlcSimpleLst = value; }
        }
        public DataTable UserDataTable
        {
            get { return userDataTable; }
            set { userDataTable = value; }
        }
        public bool AutoMes
        {
            get {

                if (autoMesOutInPs.ShowValue == Constant.M_OFF)
                {
                    return true;
                }
                else return false;
            }
        }
        public bool lliao
        {
            get
            {

                if (lliaoOutInPs.ShowValue == Constant.M_ON)
                {
                    return true;
                }
                else return false;
            }
        }
        private bool scarMode;
        public bool ScarMode
        {
            get
            {
                if (scarInPs.ShowValue > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

        }

        public int DeviceMode
        {
          get{  if(modeSelectOutInPs!=null)           
                return modeSelectOutInPs.ShowValue;
                return 0;

            }
        }
        public bool IsPrintBarCode
        {
            get
            {
                if (PrintBarCodeMode != Constant.NoPrintBarCode)
                {
                    return true;
                }
                else return false;

            }
        }

        private bool isSaveProdLog;
        public bool IsSaveProdLog
        {
            get { return isSaveProdLog; }
            set { isSaveProdLog = value; }
        }
        public int DataFormCount
        {
            get { return evokDevice.DataFormLst.Count; }
        }


        bool sfslw;

        public bool Sfslw
        {
            get { return sfslw; }
        }

        bool mRunFlag = false;
        public bool RunFlag
        {
            get { return mRunFlag; }
            set { mRunFlag = value; }
        }
        ThreadStart CutThreadStart;
        //初始化Thread的新实例，并通过构造方法将委托ts做为参数赋初始值。
        Thread CutThread;   //需要引入System.Threading命名空间
        public void SetEvokDevice(EvokXJDevice evokDevice0)
        {
            evokDevice = evokDevice0;
        }
        /// <summary>
        ///20181104这里本来initcontrol 可以去掉 但是现在的话 不好去 因为这个机制和台达TCP 那个不一样 自动页面还有自己创建的数据 后期需要修改
        /// 设定手动页面 通过表格bin字符进行自动生成plcinfosimple变量
        /// </summary>
        /// <param name="evokDevice0"></param>
        //根据表格创建plcsimple
        public void SetPage(int id)
        {
            if (evokDevice.DataFormLst.Count > 1 && evokDevice.DataFormLst[id].Rows.Count > 0)
            {
                AllPlcSimpleLst[id].Clear();

                foreach (DataRow dr in evokDevice.DataFormLst[id].Rows)
                {
                    if (dr == null) continue;
                    string name = dr["bin"].ToString();

                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        PlcInfoSimple p = new PlcInfoSimple(name);
                        try
                        {

                            p.Mode = dr["mode"].ToString();
                            p.RowIndex = evokDevice.DataFormLst[id].Rows.IndexOf(dr);
                            p.BelongToDataform = evokDevice.DataFormLst[id];
                            int addrInt = 0;
                            string areaStr = "D";
                            string userdata = dr["addr"].ToString();
                            string param3 = dr["param3"].ToString();
                            string param4 = dr["param4"].ToString();
                            if (!string.IsNullOrWhiteSpace(param3))
                            {
                                p.ShowStr.Add(param3);
                            }
                            if (!string.IsNullOrWhiteSpace(param4))
                            {
                                p.ShowStr.Add(param4);
                            }
                            ConstantMethod.SplitAreaAndAddr(userdata, ref addrInt, ref areaStr);
                            //区域符号在前面 后面地址就可以计算了
                            p.Area = areaStr;
                            p.Addr = addrInt;


                            if (dr.Table.Columns.Contains("param7"))
                            {
                                string ration = dr["param7"].ToString();
                                if (!string.IsNullOrWhiteSpace(ration)) p.Ration = int.Parse(ration);
                            }
                            if (dr.Table.Columns.Contains("param8"))
                            {
                                string max = dr["param8"].ToString();
                                if (!string.IsNullOrWhiteSpace(max)) p.MaxValue = int.Parse(max);
                            }
                            if (dr.Table.Columns.Contains("param9"))
                            {
                                string min = dr["param9"].ToString();
                                if (!string.IsNullOrWhiteSpace(min)) p.MinValue = int.Parse(min);
                            }

                            AllPlcSimpleLst[id].Add(p);
                        }
                        catch (Exception ex)
                        {

                        }

                    }
                }
            }
        }
        //优化料根数 有几根 设置下
        public void SetShowCnt(ComboBox cb1)
        {
            if (cb1 != null) optSize.CbResultCnt = cb1;
        }
        public void SetOptSize(OptSize optSize0)
        {
            optSize = optSize0;
        }
        public void optReady(int OPTID)
        {                   
            optSize.Len = lcOutInPs.ShowValue;
            //20181201 门皮 门板机器 由于带有宽度 在料长
            if (DeviceProperty == Constant.devicePropertyB )
            {
                optSize.Len = lcOutInPs.ShowValue + 5000;
            }

            if(dbcOutInPs!=null)
            optSize.Dbc = dbcOutInPs.ShowValue;

            if (ltbcOutInPs != null)
            optSize.Ltbc = ltbcOutInPs.ShowValue;

            if (safeOutInPs != null)
            optSize.Safe = safeOutInPs.ShowValue;

            if(wlMiniSizeOutInPs!=null)
            optSize.WlMiniValue = wlMiniSizeOutInPs.ShowValue;

            switch (OPTID)
            {
                case Constant.optNormal:
                    {
                        ConstantMethod.ShowInfo(rtbResult, optSize.OptNormal(rtbResult));
                        break;
                    }
                case Constant.optNormalExcel:
                    {
                        ConstantMethod.ShowInfo(rtbResult, optSize.OptNormal(rtbResult, Constant.optNormalExcel));
                        break;
                    }
                case Constant.optNo:
                    {
                        ConstantMethod.ShowInfo(rtbResult, optSize.OptNormal(rtbResult, Constant.optNo));
                        break;
                    }
                case Constant.optShuChi:
                    {
                        ConstantMethod.ShowInfo(rtbResult, optSize.OptNormal(rtbResult, Constant.optShuChi));
                        break;
                    }
            }         
        }

        public void shiftToLine()
        {
            PlcInfoSimple temp = new PlcInfoSimple(0,"D");
            temp = startOutPs;
            startOutPs = lineStartOutPs;
            lineStartOutPs = temp;

            temp = startInPs;
            startInPs = lineStartInPs;
            lineStartInPs = temp;

        }
        public void SetRtbWork(RichTextBox richrtbWork0)
        {
            rtbWork = richrtbWork0;
            //rtbWork.Clear();
        }
        public void SetUserDataGridView(DataGridView dgv1)
        {
            optSize.UserDataView = dgv1;
        }

        public void SetRtbResult(RichTextBox richrtbWork0)
        {
            rtbResult = richrtbWork0;
        }

        public void SetLblStatus(Label lblstatus0)
        {
            lblStatus = lblstatus0;
        }
        //显示状态
        public void ShowLblStatus()
        {
            if (lblStatus != null)
            {
                if (deviceStatusId < Constant.constantStatusStr.Count())
                {
                    lblStatus.Text = Constant.constantStatusStr[deviceStatusId];
                    lblStatus.BackColor = Constant.colorRgb[deviceStatusId];
                }
            }
        }
        public bool DeviceStatus {
            get {

                return evokDevice.Status == Constant.DeviceConnected;
            }
        }
        public void SetPrintReport()
        {
            if (printReport == null)
            {
                printReport = new FastReport.Report();
            }
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
                    printReport.Load(getbarcodepath[0]);
                    printReport.PrintSettings.ShowDialog = false;
                }
            }

        }

        public void ShowNowLog(string filename0)
        {
            if (!File.Exists(filename0))
            {
                MessageBox.Show(Constant.DeviceNoLogFile);
                return;
            }

            LogForm log1 = new LogForm();
            log1.fileName = filename0;
            log1.LoadData();
            log1.ShowDialog();
        }
        public void SaveProdDataLog(ProdInfo p, int id)
        {
            if (IsSaveProdLog)
            {
                if (optSize.ProdInfoLst.Count > 0)
                {
                    string savestr = "";
                    string header = String.Format("料长:{0}  料头补偿:{1}  刀补偿:{2}  安全距离:{3} 尾料:{4}", optSize.Len, optSize.Ltbc, optSize.Dbc, optSize.Safe, p.WL);
                    //for (int i = 0; i < optSize.ProdInfoLst.Count; i++)
                    // {
                    savestr = savestr + "  第" + (id + 1).ToString() + "根:";
                    for (int j = 0; j < p.Cut.Count; j++)
                    {
                        savestr = savestr + "  第" + (j + 1).ToString() + "刀尺寸" + p.Cut[j].ToString() + "\n";
                    }

                    LogManager.WriteProgramLogProdData(String.Concat(header, savestr));
                    //}
                }
            }

        }
        public void ChangePrintMode(int value)
        {

            if (PrinterSettings.InstalledPrinters.Count == 0)
            {
                value = 0;
                LogManager.WriteProgramLog(DeviceName + Constant.DeviceNoPrinter);
            }

            paramFile.WriteConfig(Constant.printBarcodeMode, value.ToString());

            OldPrintBarCodeMode = PrintBarCodeMode;

            printBarCodeMode = value;

            if (printBarCodeMode == Constant.AutoBarCode)
            {
                if(plcHandlebarCodeOutInPs !=null)
                evokDevice.SetMValueON(plcHandlebarCodeOutInPs);
            }
            else
            {
                if (plcHandlebarCodeOutInPs != null)
                    evokDevice.SetMValueOFF(plcHandlebarCodeOutInPs);
            }

            string issaveProdLog = "0";
            issaveProdLog = paramFile.ReadConfig(Constant.IsSaveProdLog);
            int save = 0;
            if (int.TryParse(issaveProdLog, out save))
            {
                if (save == 0) IsSaveProdLog = false;
                else
                {
                    IsSaveProdLog = true;
                }
            }
        }
        
        public DataTable GetDataForm(int id)
        {
            
            if (id < DataFormCount)
            {
                return evokDevice.DataForm;
            }
            else return null;
        }
        #region 自动
        //自动页面
        List<PlcInfoSimple> psLstAuto;
        public List<xjplc.PlcInfoSimple> PsLstAuto
        {
            get { return psLstAuto; }
            set { psLstAuto = value; }
        }
        //打印标签过程当中，要实现切换打印模式 
        int oldPrintBarCodeMode = Constant.NoPrintBarCode;
        public int OldPrintBarCodeMode
        {
            get { return oldPrintBarCodeMode; }
            set { oldPrintBarCodeMode = value; }
        }
        //定义后要加入集合  //忽略寄存器的影响直接匹配参数名     
        public PlcInfoSimple printMiniSizeOutInPs = new PlcInfoSimple("打印最小尺寸读写");
        public PlcInfoSimple wlMiniSizeOutInPs = new PlcInfoSimple("尾料尺寸限制读写");
        public PlcInfoSimple autoMesOutInPs = new PlcInfoSimple("自动测长标志读写");
        public PlcInfoSimple dbcOutInPs = new PlcInfoSimple("刀补偿读写");
        public PlcInfoSimple ltbcOutInPs = new PlcInfoSimple("料头补偿读写");
        public PlcInfoSimple ltbcDefaultOutInPs = new PlcInfoSimple("料头固定补偿读写");
        public PlcInfoSimple safeOutInPs = new PlcInfoSimple("安全距离读写");
        public PlcInfoSimple prodOutInPs = new PlcInfoSimple("总产量读写");
        public PlcInfoSimple noSizeToCutOutInPs = new PlcInfoSimple("无匹配读写");
        public PlcInfoSimple ldsOutInPs = new PlcInfoSimple("料段数读写");
        public PlcInfoSimple lcOutInPs = new PlcInfoSimple("料长读写");
        public PlcInfoSimple lkOutInPs = new PlcInfoSimple("料宽读写");
        public PlcInfoSimple yljxOutInPs = new PlcInfoSimple("预留间隙读写");
        public PlcInfoSimple stopOutPs = new PlcInfoSimple("停止写");
        public PlcInfoSimple stopInPs = new PlcInfoSimple("停止读");
        public PlcInfoSimple cutDoneOutInPs = new PlcInfoSimple("切割完毕读写");
        public PlcInfoSimple plcHandlebarCodeOutInPs = new PlcInfoSimple("条码打印读写");
        public PlcInfoSimple startCountInOutPs = new PlcInfoSimple("开始计数读写");
        public PlcInfoSimple wlInOutPs = new PlcInfoSimple("尾料读写");
        public PlcInfoSimple wlExistOutInPs = new PlcInfoSimple("H0读写");
        public PlcInfoSimple wlWidthOutInPs = new PlcInfoSimple("H100读写");
        public PlcInfoSimple wlHeightOutInPs = new PlcInfoSimple("H101读写");
        public PlcInfoSimple lliaoOutInPs = new PlcInfoSimple("拉料开关读写");
        public PlcInfoSimple widthCountInOutPs = new PlcInfoSimple("尺寸宽段数读写");
        public PlcInfoSimple heightCountInOutPs = new PlcInfoSimple("尺寸长段数读写");
        public PlcInfoSimple modeSelectOutInPs = new PlcInfoSimple("模式选择读写");
        public PlcInfoSimple deviceStatusOutInPs = new PlcInfoSimple("设备状态读写");

        public PlcInfoSimple lineStartTipInPs = new PlcInfoSimple("启动提醒读");
        public PlcInfoSimple lineStartOutPs = new PlcInfoSimple("H1写");
        public PlcInfoSimple lineStartInPs = new PlcInfoSimple("H1读");

        public PlcInfoSimple pauseOutPs = new PlcInfoSimple("暂停写");
        public PlcInfoSimple startOutPs = new PlcInfoSimple("启动写");
        public PlcInfoSimple resetOutPs = new PlcInfoSimple("复位写");
        public PlcInfoSimple scarModeOutPs = new PlcInfoSimple("结疤测量写");
        public PlcInfoSimple pageShiftOutPs = new PlcInfoSimple("页面切换写");
        public PlcInfoSimple pressOutInPs = new PlcInfoSimple("压料读写");
        public PlcInfoSimple doorCutCntOutInPs = new PlcInfoSimple("门刀数读写");
        public PlcInfoSimple dataNotEnoughOutInPs = new PlcInfoSimple("数据达到限位读写");
        public PlcInfoSimple dataNotEnoughValueOutInPs = new PlcInfoSimple("数据限位值读写");
        public PlcInfoSimple gjOutInPs = new PlcInfoSimple("过胶读写");
        public PlcInfoSimple GWOutInPs = new PlcInfoSimple("工位读写");
        public PlcInfoSimple PJOutPs = new PlcInfoSimple("喷胶写");
        public PlcInfoSimple PJInPs = new PlcInfoSimple("喷胶读");
        public PlcInfoSimple ZQInPs = new PlcInfoSimple("纵切读");
        public PlcInfoSimple KSInPs = new PlcInfoSimple("靠栅读");
        public PlcInfoSimple LMInPs = new PlcInfoSimple("横切刀读");
        public PlcInfoSimple LMSLInPs = new PlcInfoSimple("横切送料读");
        public PlcInfoSimple doorTypeCutCountOutInPs = new PlcInfoSimple("门型刀数读写");
        public PlcInfoSimple sfslwInPs = new PlcInfoSimple("伺服上料位读");
        public PlcInfoSimple emgStopInPs = new PlcInfoSimple("急停读");
        public PlcInfoSimple emgStopOutPs = new PlcInfoSimple("急停写");
        public PlcInfoSimple startInPs = new PlcInfoSimple("启动读");
        public PlcInfoSimple resetInPs = new PlcInfoSimple("复位读");
        public PlcInfoSimple pauseInPs = new PlcInfoSimple("暂停读");
        public PlcInfoSimple scarModeInPs = new PlcInfoSimple("结疤测量读");
        public PlcInfoSimple scarInPs = new PlcInfoSimple("结疤读");
        public PlcInfoSimple autoCCInPs = new PlcInfoSimple("自动测长读");
        public PlcInfoSimple clInPs = new PlcInfoSimple("出料读");
        public PlcInfoSimple slInPs = new PlcInfoSimple("送料读");
        public PlcInfoSimple alarm1InPs = new PlcInfoSimple("报警1");
        public PlcInfoSimple alarm2InPs = new PlcInfoSimple("报警2");
        public PlcInfoSimple alarm3InPs = new PlcInfoSimple("报警3");
        public PlcInfoSimple alarm4InPs = new PlcInfoSimple("报警4");
        public PlcInfoSimple alarm5InPs = new PlcInfoSimple("报警5");
        public PlcInfoSimple alarm6InPs = new PlcInfoSimple("报警6");
        public PlcInfoSimple alarm7InPs = new PlcInfoSimple("报警7");
        public PlcInfoSimple alarm8InPs = new PlcInfoSimple("报警8");
        public PlcInfoSimple alarm9InPs = new PlcInfoSimple("报警9");
        public PlcInfoSimple alarm10InPs = new PlcInfoSimple("报警10");
        public PlcInfoSimple alarm11InPs = new PlcInfoSimple("报警11");
        public PlcInfoSimple alarm12InPs = new PlcInfoSimple("报警12");
        public PlcInfoSimple alarm13InPs = new PlcInfoSimple("报警13");
        public PlcInfoSimple alarm14InPs = new PlcInfoSimple("报警14");
        public PlcInfoSimple alarm15InPs = new PlcInfoSimple("报警15");
        public PlcInfoSimple alarm16InPs = new PlcInfoSimple("报警16");
        public PlcInfoSimple alarm17InPs = new PlcInfoSimple("报警17");
        public PlcInfoSimple alarm18InPs = new PlcInfoSimple("报警18");
        public PlcInfoSimple alarm19InPs = new PlcInfoSimple("报警19");
        public PlcInfoSimple alarm20InPs = new PlcInfoSimple("报警20");
        public PlcInfoSimple alarm21InPs = new PlcInfoSimple("报警21");
        public PlcInfoSimple alarm22InPs = new PlcInfoSimple("报警22");
        public PlcInfoSimple alarm23InPs = new PlcInfoSimple("报警23");
        public PlcInfoSimple alarm24InPs = new PlcInfoSimple("报警24");
        public PlcInfoSimple alarm25InPs = new PlcInfoSimple("报警25");
        public PlcInfoSimple alarm26InPs = new PlcInfoSimple("报警26");
        public PlcInfoSimple alarm27InPs = new PlcInfoSimple("报警27");
        public PlcInfoSimple alarm28InPs = new PlcInfoSimple("报警28");
        public PlcInfoSimple alarm29InPs = new PlcInfoSimple("报警29");
        public PlcInfoSimple alarm30InPs = new PlcInfoSimple("报警30");

        #endregion
        #region 手动
        List<PlcInfoSimple> psLstHand;

        public System.Collections.Generic.List<xjplc.PlcInfoSimple> PsLstHand
        {
            get { return psLstHand; }
            set { psLstHand = value; }
        }
        #endregion
        #region 参数
        List<PlcInfoSimple> psLstParam;
        public System.Collections.Generic.List<xjplc.PlcInfoSimple> PsLstParam
        {
            get { return psLstParam; }
            set { psLstParam = value; }
        }
        #endregion
        #region IO监控
        List<PlcInfoSimple> psLstIO;
        public System.Collections.Generic.List<xjplc.PlcInfoSimple> PsLstIO
        {
            get { return psLstIO; }
            set { psLstIO = value; }
        }
        #endregion
        #region 扩展页面
        
        List<PlcInfoSimple> psLstEx1;
        public System.Collections.Generic.List<xjplc.PlcInfoSimple> PsLstEx1
        {
            get { return psLstEx1; }
            set { psLstEx1 = value; }
        }
        List<PlcInfoSimple> psLstEx2;
        public System.Collections.Generic.List<xjplc.PlcInfoSimple> PsLstEx2
        {
            get { return psLstEx2; }
            set { psLstEx2 = value; }
        }
        List<PlcInfoSimple> psLstEx3;
        public System.Collections.Generic.List<xjplc.PlcInfoSimple> PsLstEx3
        {
            get { return psLstEx3; }
            set { psLstEx3 = value; }
        }
        List<PlcInfoSimple> psLstEx4;
        public System.Collections.Generic.List<xjplc.PlcInfoSimple> PsLstEx4
        {
            get { return psLstEx4; }
            set { psLstEx4 = value; }
        }
        List<PlcInfoSimple> psLstEx5;
        public System.Collections.Generic.List<xjplc.PlcInfoSimple> PsLstEx5
        {
            get { return psLstEx5; }
            set { psLstEx5 = value; }
        }
        List<PlcInfoSimple> psLstEx6;
        public System.Collections.Generic.List<xjplc.PlcInfoSimple> PsLstEx6
        {
            get { return psLstEx6; }
            set { psLstEx6 = value; }
        }
        List<PlcInfoSimple> psLstEx7;
        public System.Collections.Generic.List<xjplc.PlcInfoSimple> PsLstEx7
        {
            get { return psLstEx7; }
            set { psLstEx7 = value; }
        }
        List<PlcInfoSimple> psLstEx8;
        public System.Collections.Generic.List<xjplc.PlcInfoSimple> PsLstEx8
        {
            get { return psLstEx8; }
            set { psLstEx8 = value; }
        }
        List<PlcInfoSimple> psLstEx9;
        public System.Collections.Generic.List<xjplc.PlcInfoSimple> PsLstEx9
        {
            get { return psLstEx9; }
            set { psLstEx9 = value; }
        }
        #endregion
        public EvokXJWork(List<string> strDataFormPath, PortParam p0)
        {
            for (int i = strDataFormPath.Count - 1; i >= 0; i--)
            {
                if (!File.Exists(strDataFormPath[i]))
                {
                    strDataFormPath.RemoveAt(i);
                    MessageBox.Show(strDataFormPath[i] + Constant.ErrorPlcFile);
                    Environment.Exit(0);
                }
            }

            evokDevice = new EvokXJDevice(strDataFormPath, p0);

            InitUsualTest();

        }
        //可以选择默认读取端口和自动读取端口
        public EvokXJWork(int id)
        {
            PortParam p0 = new PortParam();
            List<string> strDataFormPath = new List<string>();
            switch (id)
            {
                case Constant.evokGetDefaultMode:
                    {
                        p0 = ConstantMethod.LoadPortParam(Constant.ConfigSerialportFilePath);
                        strDataFormPath.Add(Constant.PlcDataFilePathAuto);
                        strDataFormPath.Add(Constant.PlcDataFilePathHand);
                        strDataFormPath.Add(Constant.PlcDataFilePathParam);
                        strDataFormPath.Add(Constant.PlcDataFilePathIO);
                        break;
                    }
                case Constant.evokGetAutoMode:
                    {
                        //选择工厂模式
                        for (int i = strDataFormPath.Count - 1; i >= 0; i--)
                        {
                            if (!File.Exists(strDataFormPath[i]))
                            {
                                strDataFormPath.RemoveAt(i);
                                MessageBox.Show(strDataFormPath[i] + Constant.ErrorPlcFile);
                                Environment.Exit(0);
                            }
                        }
                        break;
                    }
            }

            evokDevice = new EvokXJDevice(strDataFormPath, p0);

            InitUsualTest();

        }
        public bool IsPause()
        {
            return (pauseInPs.ShowValue == 1);
        }
        ComboBox cbOptParam1;
        public void SetOptParamShowCombox(ComboBox cbOptParam0)
        {
            cbOptParam1 = cbOptParam0;
            //优化系数
            int optParamInt = 0;
            try
            {
                if (!int.TryParse(paramFile.ReadConfig(Constant.optParam1), out optParamInt))
                {
                    MessageBox.Show(Constant.ErrorParamConfigFile);

                    Application.Exit();

                    System.Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {

            }
            if (cbOptParam1 != null && optParamInt > -1)
            { cbOptParam1.Text = optParamInt.ToString(); }
        }
        
        public bool startTip
        {
            
            get {

                if (lineStartTipInPs != null)
                    return
                    lineStartTipInPs.ShowValue == 1 ? true : false;

                else return false;
            }
        }
        public void InitUsualTest()
        {

            PsLstAuto = new List<PlcInfoSimple>();

            PsLstHand = new List<PlcInfoSimple>();

            PsLstParam = new List<PlcInfoSimple>();

            PsLstIO = new List<PlcInfoSimple>();
      
            UserDataTable = new DataTable();

            AllPlcSimpleLst = new List<List<PlcInfoSimple>>();

            AllPlcSimpleLst.Add(psLstAuto);
            AllPlcSimpleLst.Add(psLstHand);
            AllPlcSimpleLst.Add(psLstParam);
            AllPlcSimpleLst.Add(PsLstIO);

            

            #region 自动
            SetPage(Constant.AutoPage);
           
            printMiniSizeOutInPs = ConstantMethod.getPlcSimple(printMiniSizeOutInPs.Name, psLstAuto);
            wlMiniSizeOutInPs = ConstantMethod.getPlcSimple(wlMiniSizeOutInPs.Name, psLstAuto);
            autoMesOutInPs = ConstantMethod.getPlcSimple(autoMesOutInPs.Name, psLstAuto);
            dbcOutInPs = ConstantMethod.getPlcSimple(dbcOutInPs.Name, psLstAuto);
            ltbcOutInPs = ConstantMethod.getPlcSimple(ltbcOutInPs.Name, psLstAuto);
            ltbcDefaultOutInPs = ConstantMethod.getPlcSimple(ltbcDefaultOutInPs.Name, psLstAuto);
            safeOutInPs = ConstantMethod.getPlcSimple(safeOutInPs.Name, psLstAuto);
            prodOutInPs = ConstantMethod.getPlcSimple(prodOutInPs.Name, psLstAuto);
            prodOutInPs.IsParam = false;

            noSizeToCutOutInPs = ConstantMethod.getPlcSimple(noSizeToCutOutInPs.Name, psLstAuto);
            ldsOutInPs = ConstantMethod.getPlcSimple(ldsOutInPs.Name, psLstAuto);
            lcOutInPs = ConstantMethod.getPlcSimple(lcOutInPs.Name, psLstAuto);
            lkOutInPs = ConstantMethod.getPlcSimple(lkOutInPs.Name, psLstAuto);
            yljxOutInPs = ConstantMethod.getPlcSimple(yljxOutInPs.Name, psLstAuto);
            stopOutPs = ConstantMethod.getPlcSimple(stopOutPs.Name, psLstAuto);
            stopInPs = ConstantMethod.getPlcSimple(stopInPs.Name, psLstAuto);
            cutDoneOutInPs = ConstantMethod.getPlcSimple(cutDoneOutInPs.Name, psLstAuto);
            plcHandlebarCodeOutInPs = ConstantMethod.getPlcSimple(plcHandlebarCodeOutInPs.Name, psLstAuto);
            startCountInOutPs = ConstantMethod.getPlcSimple(startCountInOutPs.Name, psLstAuto);
            wlInOutPs = ConstantMethod.getPlcSimple(wlInOutPs.Name, psLstAuto);
            wlExistOutInPs = ConstantMethod.getPlcSimple(wlExistOutInPs.Name, psLstAuto);
            wlWidthOutInPs = ConstantMethod.getPlcSimple(wlWidthOutInPs.Name, psLstAuto);
            wlHeightOutInPs = ConstantMethod.getPlcSimple(wlHeightOutInPs.Name, psLstAuto);
            lliaoOutInPs = ConstantMethod.getPlcSimple(lliaoOutInPs.Name, psLstAuto);
            widthCountInOutPs = ConstantMethod.getPlcSimple(widthCountInOutPs.Name, psLstAuto);
            heightCountInOutPs = ConstantMethod.getPlcSimple(heightCountInOutPs.Name, psLstAuto);
            modeSelectOutInPs = ConstantMethod.getPlcSimple(modeSelectOutInPs.Name, psLstAuto);
            deviceStatusOutInPs = ConstantMethod.getPlcSimple(deviceStatusOutInPs.Name, psLstAuto);

            lineStartTipInPs = ConstantMethod.getPlcSimple(lineStartTipInPs.Name, psLstAuto);
            lineStartOutPs = ConstantMethod.getPlcSimple(lineStartOutPs.Name, psLstAuto);
            lineStartInPs = ConstantMethod.getPlcSimple(lineStartInPs.Name, psLstAuto);
      
            pauseOutPs = ConstantMethod.getPlcSimple(pauseOutPs.Name, psLstAuto);
            startOutPs = ConstantMethod.getPlcSimple(startOutPs.Name, psLstAuto);
            resetOutPs = ConstantMethod.getPlcSimple(resetOutPs.Name, psLstAuto);
            scarModeOutPs = ConstantMethod.getPlcSimple(scarModeOutPs.Name, psLstAuto);
            pageShiftOutPs = ConstantMethod.getPlcSimple(pageShiftOutPs.Name, psLstAuto);
            pressOutInPs = ConstantMethod.getPlcSimple(pressOutInPs.Name, psLstAuto);
            doorCutCntOutInPs = ConstantMethod.getPlcSimple(doorCutCntOutInPs.Name, psLstAuto);
            dataNotEnoughOutInPs = ConstantMethod.getPlcSimple(dataNotEnoughOutInPs.Name, psLstAuto);
            dataNotEnoughValueOutInPs = ConstantMethod.getPlcSimple(dataNotEnoughValueOutInPs.Name, psLstAuto);
            gjOutInPs = ConstantMethod.getPlcSimple(gjOutInPs.Name, psLstAuto);
            GWOutInPs = ConstantMethod.getPlcSimple(GWOutInPs.Name, psLstAuto);
            PJOutPs = ConstantMethod.getPlcSimple(PJOutPs.Name, psLstAuto);
            PJInPs = ConstantMethod.getPlcSimple(PJInPs.Name, psLstAuto);
            ZQInPs = ConstantMethod.getPlcSimple(ZQInPs.Name, psLstAuto);
            KSInPs = ConstantMethod.getPlcSimple(KSInPs.Name, psLstAuto);
            LMInPs = ConstantMethod.getPlcSimple(LMInPs.Name, psLstAuto);
            LMSLInPs = ConstantMethod.getPlcSimple(LMSLInPs.Name, psLstAuto);
            doorTypeCutCountOutInPs = ConstantMethod.getPlcSimple(doorTypeCutCountOutInPs.Name, psLstAuto);
            sfslwInPs = ConstantMethod.getPlcSimple(sfslwInPs.Name, psLstAuto);
            emgStopInPs = ConstantMethod.getPlcSimple(emgStopInPs.Name, psLstAuto);
            emgStopOutPs = ConstantMethod.getPlcSimple(emgStopOutPs.Name, psLstAuto);
            startInPs = ConstantMethod.getPlcSimple(startInPs.Name, psLstAuto);
            resetInPs = ConstantMethod.getPlcSimple(resetInPs.Name, psLstAuto);
            pauseInPs = ConstantMethod.getPlcSimple(pauseInPs.Name, psLstAuto);
            scarModeInPs = ConstantMethod.getPlcSimple(scarModeInPs.Name, psLstAuto);
            scarInPs = ConstantMethod.getPlcSimple(scarInPs.Name, psLstAuto);
            autoCCInPs = ConstantMethod.getPlcSimple(autoCCInPs.Name, psLstAuto);
            clInPs = ConstantMethod.getPlcSimple(clInPs.Name, psLstAuto);
            slInPs = ConstantMethod.getPlcSimple(slInPs.Name, psLstAuto);

            #endregion
            #region Hand
            SetPage(Constant.HandPage);
            #endregion
            #region 参数
            SetPage(Constant.ParamPage);
            #endregion
            #region IO监控

            #endregion

            paramFile =  ConstantMethod.configFileBak(Constant.ConfigParamFilePath);          

            if (!int.TryParse(paramFile.ReadConfig(Constant.printBarcodeMode), out printBarCodeMode))
               {
                    
                    MessageBox.Show(Constant.ErrorParamConfigFile);

                    Application.Exit();

                    System.Environment.Exit(0);

               }
            #region 打印机和贴码尺寸设置
                       
            minPrinterName = paramFile.ReadConfig(Constant.minPrinterName);
             #endregion
            LogManager.WriteProgramLog(DeviceName + Constant.Start);

            InitControl();

            ShiftPage(Constant.AutoPage);

            if (!evokDevice.getDeviceData())
            {

                MessageBox.Show(DeviceName + Constant.ConnectMachineFail);
                Environment.Exit(0);
            }

            ErrorList = new List<string>();

            optSize = new OptSize();


            //条码测试 线程 定时检测定时器
            InitBarCodeTimer();

        }
        public void SetSmallSizePrinter(string printerName)
        {
            List<string> printerLst = ConstantMethod.GetLocalPrinter();
            if (printerLst.Contains(printerName))
            {
                paramFile.WriteConfig(Constant.minPrinterName, printerName);
                minPrinterName = printerName;
            }

        }
        public void ListSmallSizePrinter(ComboBox cb)
        {
            List<string> printerLst = ConstantMethod.GetLocalPrinter();
            cb.DataSource = printerLst;// ConstantMethod.GetLocalPrinter();
            if (printerLst.Contains(minPrinterName))
            cb.Text = minPrinterName;
            else cb.Text = ConstantMethod.DefaultPrinter;

        }
        public bool SaveOptParam1(string id)
        {
            int x = 0;
            if (int.TryParse(id, out x))
                if (x > -1)
                {
                    try
                    {
                        paramFile.WriteConfig(Constant.optParam1, id);
                    }
                    catch (Exception ex)
                    { }
                    return true;
                }
            return false;
        }

        //默认自动读取设备端口
        public EvokXJWork()
        {
            //初始化设备
            List<string> strDataFormPath = new List<string>();

            strDataFormPath.Add(Constant.PlcDataFilePathAuto);
            strDataFormPath.Add(Constant.PlcDataFilePathHand);
            strDataFormPath.Add(Constant.PlcDataFilePathParam);
            strDataFormPath.Add(Constant.PlcDataFilePathIO);
         

            if (File.Exists(Constant.PlcDataFilePathScar))
                strDataFormPath.Add(Constant.PlcDataFilePathScar);

            for (int i = strDataFormPath.Count - 1; i >= 0; i--)
            {
                if (!File.Exists(strDataFormPath[i]))
                {
                    strDataFormPath.RemoveAt(i);
                    MessageBox.Show(strDataFormPath[i] + Constant.ErrorPlcFile);
                    Environment.Exit(0);
                }
            }

            evokDevice = new EvokXJDevice(strDataFormPath);
            evokDevice.setDeviceId(DeviceName);
            InitUsualTest();

        }

        #region 梳齿机
        public void addDataForm(List<string> filestr)
        {
            if (DeviceName.Equals(Constant.scjDeivceName))
            {
                PsLstEx1 = new List<PlcInfoSimple>();
                PsLstEx2 = new List<PlcInfoSimple>();
                PsLstEx3 = new List<PlcInfoSimple>();
                PsLstEx4 = new List<PlcInfoSimple>();
                PsLstEx5 = new List<PlcInfoSimple>();
                PsLstEx6 = new List<PlcInfoSimple>();
                PsLstEx7 = new List<PlcInfoSimple>();
                PsLstEx8 = new List<PlcInfoSimple>();
                PsLstEx9 = new List<PlcInfoSimple>();
                AllPlcSimpleLst.Add(PsLstEx1);
                AllPlcSimpleLst.Add(PsLstEx2);
                AllPlcSimpleLst.Add(PsLstEx3);
                AllPlcSimpleLst.Add(PsLstEx4);
                AllPlcSimpleLst.Add(PsLstEx5);
                AllPlcSimpleLst.Add(PsLstEx6);
                AllPlcSimpleLst.Add(PsLstEx7);
                AllPlcSimpleLst.Add(PsLstEx8);
                AllPlcSimpleLst.Add(PsLstEx9);

            }

            if ( filestr.Count > 0 )
            evokDevice.GetPlcDataTableFromFile(filestr);
            for (int i =  4; i < evokDevice.DataFormLst.Count; i++)
            {
                
                SetPage(i);
                psLstHand.AddRange(AllPlcSimpleLst[i]);
            }
        }

        private void DownLoadDataNormalWithShuchi(int i)
        {

            List<int> DataList = new List<int>();
            List<int> DataListGW = new List<int>();
            DataList.Add(1);
            //添加段数
            DataList.Add(optSize.ProdInfoLst[i].Cut.Count);

            //采用信捷XG2 精度提高后需要增加一个比率 201901142054
            if (DeviceName.Equals(Constant.scjDeivceName))
            {
                
                foreach (int s in optSize.ProdInfoLst[i].Cut)
                {
                    DataList.Add(s * 1000);
                }
            }
            else
                DataList.AddRange(optSize.ProdInfoLst[i].Cut);

            //添加工位号
            DataListGW.Add(optSize.ProdInfoLst[i].Cut.Count);

            int[] intArray = null;
            List<int> intGW = new List<int>();
            //C# 3.0下用此句
            try
            {
                foreach (string param12 in optSize.ProdInfoLst[i].Param12)
                {
                    int value = 1;
                    if (intGW.Count > 0)
                        value = intGW.Last();
                    if (int.TryParse(param12, out value))
                    {

                    }
                    intGW.Add(value);
                }
            }
            catch (Exception ex)
            {

            }
            intArray = intGW.ToArray();
            //if(DataListGW.Count == DataList.Count)
            if (intArray != null && intArray.Length > 0)
                DataListGW.AddRange(intArray);
            //数据下发 确保正确 下位机需要给一个M16 高电平 我这边来置OFF
            //发数据三次 M16 如果还没有给高电平

            if (DeviceName.Equals(Constant.scjDeivceName))
            {
                if (DataListGW.Count() > 1)
                    DataList.Insert(2, DataListGW[1]);
            }

            for (int m = 0; m < 30; m++)
            {
                if (!RunFlag) break;   
                LogManager.WriteProgramLog(DeviceName + Constant.DataDownLoad + m.ToString());

               
                if (evokDevice.SetMultiPleDValue(wlInOutPs, DataList.ToArray()))
                {
                    //发送是料长但料长不清零要读取清零的D5000数据 所以只能加延时
                    //20190108 根据老胡的建议 增加一个点位告诉下位机 数据发好了
                    ConstantMethod.Delay(200);
                    //如果不是以前的机器 就不发工位号了
                    if(!DeviceName.Equals(Constant.scjDeivceName))
                    if (evokDevice.SetMultiPleDValue(GWOutInPs, DataListGW.ToArray()))
                    {
                        ConstantMethod.Delay(20);

                        LogManager.WriteProgramLog(DeviceName + Constant.DataDownLoadSuccess);
                    }
                    // 料段数大于0  代表写成功了 
                    if (wlInOutPs.ShowValue != 0)
                    {
                        //然后设置M16为高 写成功了 就退出来
                        if (evokDevice.SetMValueON(startCountInOutPs)) break;
                    }
                }
            }         
        }


        #endregion
        public void pressShift()
        {
            evokDevice.opposite(pressOutInPs);
        }
        public bool RestartDevice(int id)
        {
           
            evokDevice.RestartConneect(evokDevice.DataFormLst[id]);
            //在重新启动后 plcsimple 和PLCinfoLst 重新寻找 因为重启后 plcinfolst重新生成了
            FindPlcSimpleInPlcInfoLst(id);
            return evokDevice.getDeviceData();

        }
        #region 运行部分


        public bool testGetScarData(DataTable scarTable)
        {
            int scarCount = scarInPs.ShowValue;

            if (scarCount % 2 != 0 || scarCount < 1 || scarCount > Constant.MaxScarCount) return false;

            scarCount = scarCount / 2;

            scarTable.Rows.Clear();

            int srAddress = Constant.ScarStartAddress;

            for (int i = 0; i < scarCount; i++)
            {
                srAddress += 2;

                DataRow dr = scarTable.NewRow();
                dr[0] = Constant.strDMArea[0] + srAddress.ToString();
                dr[1] = Constant.DoubleMode;
                dr[2] = Constant.ScarName + i.ToString() + "-0";
                dr[3] = "1";

                srAddress += 2;
                DataRow dr0 = scarTable.NewRow();
                dr0[0] = Constant.strDMArea[0] + srAddress.ToString();
                dr0[1] = Constant.DoubleMode;
                dr0[2] = Constant.ScarName + i.ToString() + "-1";
                dr0[3] = "1";

                scarTable.Rows.Add(dr);
                scarTable.Rows.Add(dr0);
            }

            ShiftPage(Constant.ScarPage);

            ConstantMethod.Delay(1000);

            ShiftPage(Constant.AutoPage);

            return true;

        }
        public void ProClr()
        {
            evokDevice.SetDValue(prodOutInPs, 0);
           
            optSize.prodClear();
        }
        public void SetLtbc()
        {
            evokDevice.SetDValue(ltbcOutInPs, optSize.Ltbc);
        }
        /// <summary>
        /// 门生产线 启动使用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool startCutDoor(int id)
        {
            ConstantMethod.Delay(300);  
            evokDevice.SetMValueOFF2ON(startOutPs);
            CutReady(); //临时 以后要改到通用的地方去
            OldPrintBarCodeMode = PrintBarCodeMode;
            RunFlag = true;                      

            LogManager.WriteProgramLog(DeviceName + Constant.DeviceStartCut);

            return GetStartStatus();

        }
        public bool GetStartStatus()
        {
            int valueold = Constant.M_ON;
            ConstantMethod.DelayWriteCmdOk(Constant.StartWaitMaxTime, ref valueold, ref startInPs,ref emgStopInPs);

            return startInPs.ShowValue == Constant.M_ON ? true : false;
        }
        //启动
        public bool start(int id)
        {
            evokDevice.SetMValueOFF2ON2OFF(startOutPs);
            ConstantMethod.Delay(200);
            CutReady(); //临时以后要改到通用的地方去
            OldPrintBarCodeMode = PrintBarCodeMode;  //登记下用户打印初始值      
            RunFlag = true;           

            LogManager.WriteProgramLog(DeviceName + Constant.DeviceStartCut);
            tCheckPrint.Enabled = true;

            return GetStartStatus();
        }

        void stopOperation()
        {
            RunFlag = false;
            CloseDataNotEnough();
            ConstantMethod.Delay(200);
            if (CutThread != null && CutThread.IsAlive)
            {
                CutThread.Abort();
            }
            optSize.SingleSizeLst.Clear();
            optSize.ProdInfoLst.Clear();

            tCheckPrint.Enabled = false;

        }
        public bool stop()
        {
        
            // 如果已停止 那就不需要停止了
            if (deviceStatusId == Constant.constantStatusId[1]
                ||
                deviceStatusId == Constant.constantStatusId[3] ||
                deviceStatusId == Constant.constantStatusId[4] ||
                deviceStatusId == Constant.constantStatusId[6] ||
                deviceStatusId == Constant.constantStatusId[7]
                )
            {
                stopOperation();              
                showWorkInfo();
                return true;
            }

            int id = 0;
            while (!evokDevice.SetMValueON(stopOutPs))
            {
                id++;
                Application.DoEvents();
                if (id > 30) break;
            }
            int old = 1;
            ConstantMethod.DelayWriteCmdOk(3000, ref old, ref stopInPs);
            LogManager.WriteProgramLog(DeviceName + Constant.DeviceStop);
            try
            {
                if (stopInPs.ShowValue == old)
                {
                    showWorkInfo(Constant.stopOk);
                    return true;
                }
                else
                {
                    showWorkInfo(Constant.stopWrong);
                    
                }
            }

            catch (Exception ex)
            {

            }
            finally
            {
                stopOperation();
                LogManager.WriteProgramLog(DeviceName + Constant.DeviceStop);
            }
            return false;
        }
        public bool emgStop()
        {

            int id = 0;
            //操作30次 如果发送错误 就一直发送
            while (!evokDevice.SetMValueON(emgStopOutPs))
            {
                id++;
                Application.DoEvents();
                if (id > 5) break;
            }

            int old = 1;
            ConstantMethod.DelayWriteCmdOk(500, ref old, ref emgStopInPs);
            try
            {
                if (emgStopInPs.ShowValue == old)
                {
                    showWorkInfo(Constant.emgstopOk);
                    return true;
                }
                else
                {
                    showWorkInfo(Constant.emgstopWrong);

                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                stopOperation();

                LogManager.WriteProgramLog(DeviceName + Constant.DeviceEmgStop);
            }
            return false;
        }

        public bool IsInEmg {
            get
            {
                if (emgStopInPs.ShowValue == Constant.M_ON)
                {
                    return true;
                }
                else return false;
            }
        }



        //停止 
        public bool pause()
        {           
            //如果是在报警中 复位中 停止中  
            if (deviceStatusId   == Constant.constantStatusId[1]
               || deviceStatusId == Constant.constantStatusId[3]
               || deviceStatusId == Constant.constantStatusId[4]
               || deviceStatusId == Constant.constantStatusId[6]
               || deviceStatusId == Constant.constantStatusId[7]
                && deviceStatusId< Constant.constantStatusStr.Count())
            {                
                showWorkInfo(); return false;
            }

            if (!RunFlag)
            {
                showWorkInfo(Constant.IsWorking);
                return false;
            }

            if (pauseOutPs.ShowValue == 0)
            {

                evokDevice.opposite(pauseOutPs);
                int old = 1;
                ConstantMethod.DelayWriteCmdOk(10000,ref old,ref pauseInPs,ref emgStopInPs);
                LogManager.WriteProgramLog(DeviceName + Constant.DevicePause);

                if (pauseInPs.ShowValue == old)
                {
                    showWorkInfo(Constant.pauseOk);
                    return true;
                }
                else
                {
                    showWorkInfo(Constant.pauseWrong);
                    return false;
                } 
                                                            
            }
            else
            {
                int old = 0;
               
                evokDevice.opposite(pauseOutPs);

                ConstantMethod.DelayWriteCmdOk(10000, ref old, ref pauseInPs);

                if (pauseInPs.ShowValue == old)
                {
                    showWorkInfo(Constant.contiOk);
                    return true;
                }
                else
                {
                    showWorkInfo(Constant.contiWrong);
                    return false;
                }
            }
        }
        //自动上料
        public void autoSL()
        {
            evokDevice.SetMValueOFF2ON(scarModeOutPs);
        }
         
        //复位
        public bool reset()
        {
           
            if (
                   deviceStatusId == Constant.constantStatusId[1]
                || deviceStatusId == Constant.constantStatusId[2]
                || deviceStatusId == Constant.constantStatusId[3]
                || deviceStatusId == Constant.constantStatusId[4]
                || deviceStatusId == Constant.constantStatusId[5]
        
               && deviceStatusId < Constant.constantStatusStr.Count())
            {
                
                showWorkInfo();
                return false;
            }

            int id = 0;
            evokDevice.SetMValueOFF2ON2OFF(resetOutPs);
            //复位等待2秒
            ConstantMethod.Delay(1000);
            int old = 0;
            ConstantMethod.DelayWriteCmdOk(60000, ref old, ref resetInPs,ref emgStopInPs);

            LogManager.WriteProgramLog(DeviceName+Constant.DeviceReset);


            if (resetInPs.ShowValue == old)
            {
                showWorkInfo(Constant.resetOk);
                stopOperation();
                return true;
            }
            else
            {
                showWorkInfo(Constant.resetWrong);
                return false;
            }

           
        }


        #endregion
        public void SaveFile()
        {
         
            optSize.SaveCsv();
            optSize.SaveExcel();
        }
        public bool SetOptSizeData(DataTable dtSize)
        {
            if (dtSize != null && dtSize.Rows.Count > 0)
            {
                optSize.DtData = dtSize;
                return true;
            }
            return false;
        }
        public int ReadCSVDataDefault()
        {
            if (!LoadCsvData(Constant.userdata))
            {
                optSize.buildDefaultCsvFile(Constant.userdata);
                LoadCsvData(Constant.userdata);
            }
            return 0;
        }
        public void ShowBarCode(int rowindex)
        {
            if (printReport != null)
            {
                List<string> valuestr = new List<string>();

                if (optSize.DtData != null && optSize.DtData.Rows.Count > 0)
                {


                    DataRow dr = optSize.DtData.Rows[rowindex];
                    for (int j = 3; j < optSize.DtData.Columns.Count; j++)
                    {
                        valuestr.Add(dr[j].ToString());
                    }


                    printBarcode(printReport, valuestr.ToArray(), 0);
                }
                else
                {
                    MessageBox.Show("无数据，请先导出数据！");
                }
            }
            else
            {
                MessageBox.Show("条码加载错误");
            }
        }
        public void ShowBarCode(Report rp1,int rowindex)
        {
            List<string> valuestr = new List<string>();

            if (optSize.DtData != null && optSize.DtData.Rows.Count > 0)
            {

             
                DataRow dr = optSize.DtData.Rows[rowindex];
                for (int j = 3; j < optSize.DtData.Columns.Count; j++)
                {
                    valuestr.Add(dr[j].ToString());
                }

             

                printBarcode(rp1, valuestr.ToArray(),0);
            }
            else
            {
                MessageBox.Show("无数据，请先导出数据！");
            }
        }
        //显示条码 不打印
        public void printBarcode(Report rp1, object s2,int show)
        {

            string[] s1 = (string[])s2;
            if (s1 != null && printReport != null)
            {
                try
                {

                    //在遇到结巴的情况下 保存下当前打印模式
                    OldPrintBarCodeMode = PrintBarCodeMode;         

                    Application.DoEvents();

                    if (rp1.FindObject("Barcode1") != null)
                        (rp1.FindObject("Barcode1") as BarcodeObject).Text = s1[0];


                    for (int i = 1; i < s1.Length; i++)
                    {
                        if (rp1.FindObject("Text" + (i).ToString()) != null && string.IsNullOrWhiteSpace(s1[i]))
                        {
                            (rp1.FindObject("Text" + (i).ToString()) as TextObject).Text = "";

                            continue;
                        }
                        //其他参数另外选

                        if (rp1.FindObject("Text" + (i).ToString()) != null && (!string.IsNullOrWhiteSpace(s1[i])))
                        {
                            /***
                             if (s1[i].Contains('['))
                             {
                                 s1[i] = s1[i].Replace('[', ' ');
                             }
                             if (s1[i].Contains(']'))
                             {
                                 s1[i] = s1[i].Replace(']', ' ');
                             }
                           ***/
                            s1[i] = ConstantMethod.ShiftString(s1[i]);
                            (rp1.FindObject("Text" + (i).ToString()) as TextObject).Text = s1[i];
                        }
                    }
                    
                    rp1.Prepare();
                    rp1.Show();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }

            }
        }
              
        #region 条码部分


        #region 条码线程测试
        // List<Thread> printThreadLst = new List<Thread>();
        Dictionary<Thread, barCodeManger> printThreadLst;
        System.Timers.Timer tCheckPrint = new System.Timers.Timer(1000);

        
        private void InitBarCodeTimer()
        {
            (new FastReport.EnvironmentSettings()).ReportSettings.ShowProgress = false;

            tCheckPrint = new System.Timers.Timer(200);  //这里0.2 秒别改 加到常量里 工控机性能不行 

            tCheckPrint.Enabled = false;

            tCheckPrint.AutoReset = true;

            tCheckPrint.Elapsed += new System.Timers.ElapsedEventHandler(printTimerCheck);

            printThreadLst = new Dictionary<Thread, barCodeManger>();
        }
        //采用线程启动
        private void printBarCode(object obj)
        {
            barCodeManger cm = (barCodeManger)obj;
            string[] s1 = cm.ParamCode;
            Report rp1 =ConstantMethod. GetPrintReport(); //这里每次加载一个fasreport 文档 之前使用同一个report 会报错 线程间减少访问同一个report
            if (s1.Length < 1) return;
            printBarcode(rp1, s1);//
          
        }
       //这里使用线程集合来控制 打印进度  定时每隔一定时间来查看 让最前面的线程先打印掉 先走
        private void printTimerCheck(object sender, EventArgs e)
        {

            if (printThreadLst.Count >0)
            {
                List<Thread> tLst = new List<Thread>(printThreadLst.Keys);
                if (tLst[0].ThreadState == ThreadState.Unstarted)
                {
                    tLst[0].Start(printThreadLst[tLst[0]]);
                }
                else
                if (tLst[0].ThreadState == ThreadState.Aborted || (!tLst[0].IsAlive))
                {
                    printThreadLst.Remove(tLst[0]);
                }
              
            }
        }
   
       //生成一个线程 放到打印集合里等待定时器事件去判断
        public void printBarcode(Report rp1, object s2, bool threadStart)
        {          
                barCodeManger
                bCodeManager = new barCodeManger(); 
                bCodeManager.ParamCode = (string[])s2;

                Thread printThread;

                printThread = new Thread(new ParameterizedThreadStart(printBarCode));
    
                printThreadLst.Add(printThread, bCodeManager);

        }
        #endregion

        public void printBarcode(Report rp1, object s2)
        {
            string[] s1 = (string[])s2;
            if (s1.Length < 1) return;
            try
            {
                if (s1 != null && printReport != null && IsPrintBarCode)
                {
                    
                    Application.DoEvents();

                    if (rp1.FindObject("Barcode1") != null)
                        (rp1.FindObject("Barcode1") as BarcodeObject).Text = s1[0];


                    for (int i = 1; i < s1.Length; i++)
                    {
                        if (rp1.FindObject("Text" + (i).ToString()) != null && string.IsNullOrWhiteSpace(s1[i]))
                        {
                            (rp1.FindObject("Text" + (i).ToString()) as TextObject).Text = "";
                            continue;
                        }
                        //其他参数另外选
                        if (rp1.FindObject("Text" + (i).ToString()) != null && (!string.IsNullOrWhiteSpace(s1[i])))
                        {
                            s1[i] = ConstantMethod.ShiftString(s1[i]);
                            (rp1.FindObject("Text" + (i).ToString()) as TextObject).Text = s1[i];
                        }
                    }
                   
                    //ConstantMethod.ShowInfo(rtbResult, "参数3：" + s1[3].ToString()+ "参数4：" + s1[4].ToString()+"参数8：" + s1[8].ToString());
                    rp1.Prepare();                                       
                    //ConstantMethod.ShowInfo(rtbWork, "打印发送！");
                    rp1.Print();
                    
                }
            }
            catch (Exception ex)
            {
                RunFlag = false;
                //MessageBox.Show(Constant.startTips8+ ex.Message);
            }
            if(!printReport.PrintSettings.Printer.Equals(ConstantMethod.DefaultPrinter))
            printReport.PrintSettings.Printer = ConstantMethod.DefaultPrinter;
        }
        //打印条码打开
        public void plcHandleBarCodeON()
        {
            evokDevice.SetMValueON(plcHandlebarCodeOutInPs);
        }
        public void plcHandleBarCodeOFF()
        {
            evokDevice.SetMValueOFF(plcHandlebarCodeOutInPs);
        }

        #endregion
        #region 切割过程
        int CutProCnt = 0; //切割的起始根数

        public bool SetCutProCnt(int cnt)
        {
            if (CutProCnt >= optSize.ProdInfoLst.Count)
            {
                CutProCnt = 0;
            }
            
            if (cnt <= optSize.ProdInfoLst.Count)
            {
                CutProCnt = cnt-1;
                return true;
            }
            else
            {
                MessageBox.Show(Constant.startTips7);
            }

            return false;
        }
        //暂时没有反馈 先按照回复错误运行下去
     
      
       

        public bool DataJoin()
        {
            return optSize.dataGetTogether();
        }
        //先把宽的发完
        public void CutDoorBanThread()
        {
            //从哪一根开始切 暂定 从第一根 开始
            showWorkInfo("宽度数据下发!");

            DownLoadDataWithDoorBanWidth(0);
            //数据下发后 启动 
            if (!startCutDoor(0))
            {
                MessageBox.Show(DeviceName+Constant.DeviceStartFailed);
                return;
            }
            showWorkInfo(" 启动成功!");
            if (optSize.ProdInfoLst.Count > 0 && CutProCnt < optSize.ProdInfoLst.Count)
            {
                for (int i = CutProCnt; i < optSize.ProdInfoLst.Count; i++)
                {

                    SaveProdDataLog(optSize.ProdInfoLst[i], i);
                    ConstantMethod.ShowInfo(rtbWork, Constant.resultTip5 + (i + 1).ToString() + Constant.startTips4);

                    // 每根数据下发                   
                    DownLoadDataNormal(i);
                   
                    //plc 计数器 清零
                    CountClr();
                    //开始切割进程
                    CutLoop(i);

                }
            }
            else
            {
                MessageBox.Show(Constant.noData);
            }
        }
        public void CutDoorShellThread()
        {
            //从哪一根开始切 暂定 从第一根 开始         
            showWorkInfo("准备数据下发！");
            if (optSize.ProdInfoLst.Count > 0 && CutProCnt< optSize.ProdInfoLst.Count)
            {
                for (int i = CutProCnt; i < optSize.ProdInfoLst.Count; i++)
                {

                    SaveProdDataLog(optSize.ProdInfoLst[i],i);

                    ConstantMethod.ShowInfo(rtbWork, Constant.resultTip5 + (i + 1).ToString() + Constant.startTips4);
                               
                    DownLoadDataWithDoorShell(i);
                    //每根数据下发
                    showWorkInfo(Constant.startTips3);
                    //数据下发后启动 
                    if (!startCutDoor(0))
                    {
                        MessageBox.Show(DeviceName+Constant.DeviceStartFailed);
                        return ;
                    }
                    //plc 计数器 清零
                    CountClr();
                    //开始切割进程
                    CutLoop(i);

                    break;

                }
            }
            else
            {
                MessageBox.Show(Constant.noData);
            }
        }

        public void CutRotateWithHoleThread()
        {
            //从哪一根开始切 暂定 从第一根 开始
         
            if (optSize.ProdInfoLst.Count > 0 && CutProCnt < optSize.ProdInfoLst.Count)
            {
                for (int i = CutProCnt; i < optSize.ProdInfoLst.Count; i++)
                {
                    ConstantMethod.ShowInfo(rtbWork, Constant.resultTip5 + (i + 1).ToString() + Constant.startTips4);

                    //plc 计数器 清零
                    CountClr();
                    // 每根数据下发                   
                    DownLoadDataWithHoleAngle(i);
                    //开始切割进程
                    CutLoop(i);

                }
            }
            else
            {
                MessageBox.Show(Constant.noData);
            }
        }
         private void DownLoadDataNormal(int i)
        {

            SetLtbc();

            List<int> DataList = new List<int>();
            //添加料长 20170727 料长不发送了
            // DataList.Add(optSize.ProdInfoLst[i].Len);
            //D4998-》0
            // int value = 1;
            //DataList.Add(value);
            DataList.Add(optSize.ProdInfoLst[i].WL);
            //添加段数
            DataList.Add(optSize.ProdInfoLst[i].Cut.Count);

            DataList.AddRange(optSize.ProdInfoLst[i].Cut);


            //数据下发 确保正确 下位机需要给一个M16 高电平 我这边来置OFF
            //发数据三次 M16 如果还没有给高电平
            LogManager.WriteProgramLog(DeviceName + "数据下发");
            for (int m = 0; m < 30; m++)
            {
                if (!RunFlag) break;
                // 料段数为0 下发
                //if (wlInOutPs.ShowValue == 0)
                //{                
                LogManager.WriteProgramLog(DeviceName + Constant.DataDownLoad + m.ToString());

                    if (evokDevice.SetMultiPleDValue(wlInOutPs, DataList.ToArray()))
                    {
                        LogManager.WriteProgramLog(DeviceName + Constant.DataDownLoadSuccess);
                        //发送是料长 但料长不清零 要读取清零的D5000数据 所以只能加延时
                        //ConstantMethod.Delay(200);
                        break;
                        /****
                        //料段数大于0  代表写成功了 
                        if (wlInOutPs.ShowValue > 0)
                        {
                            //然后 设置M16 为高 写成功了 就退出来
                            if (evokDevice.SetMValueON(startCountInOutPs)) break;
                        }
                        ****/
                   }
               // }
                if (m == 29)
                {
                    LogManager.WriteProgramLog(DeviceName + Constant.DataDownLoadFail);
                }
            }          
        }

        private void DownLoadDataWithHoleAngle(int i)
        {
            List<int> DataList = new List<int>();
            List<ProdInfo> prod = optSize.ProdInfoLst;


            //先提取孔参数  当前这根料的数据 提取孔参数 角度参数 角度没有默认为90度          
            for (int n = 0; n < optSize.SingleSizeLst[i].Count; n++)
            {
                if (!RunFlag) break;
                SingleSizeWithHoleAngle p = new SingleSizeWithHoleAngle(
                    optSize.SingleSizeLst[i][n].DtUser, optSize.SingleSizeLst[i][n].Xuhao
                    );

                p = ConstantMethod.Mapper<SingleSizeWithHoleAngle, SingleSize>(optSize.SingleSizeLst[i][n]);

                optSize.ProdInfoLst[i].hole.Add(p.Hole);
                optSize.ProdInfoLst[i].angle.Add(p.Angle);
            }

            for (int m = 0; m < 6; m++)
            {
                if (!RunFlag) break;
                #region 带孔的参数下发

                //段数为起始地址 ：数据格式 D3000段数	D3002段长	D3004是否打印	前角度	孔数	孔位置	边长	深度
                DataList.Add(optSize.ProdInfoLst[i].Cut.Count);  //段数
                                                                 //保存下地址
                int ldsCountInOutPsAddr = wlInOutPs.Addr;
                #region 开始下发孔和角度 尺寸等数据
                if (prod[i].hole.Count > 0 && prod[i].angle.Count > 0)
                    for (int sizeid = 0; sizeid < prod[i].Cut.Count; sizeid++)
                    {
                        DataList.Add(prod[i].Cut[sizeid]);  //段长
                        DataList.Add(1);  //条码打印标志
                        int holecount0 = 0;
                        //总共10个孔 取前面 5个
                        //前角度 前角度和孔数30 要填满
                        for (int holecount = 0; holecount < prod[i].hole[sizeid].Count() / 2; holecount = holecount + 3)
                        {
                            if (prod[i].hole[sizeid][holecount] > 0)
                                holecount0++;
                        }
                        DataList.Add(prod[i].angle[sizeid][0]);
                        DataList.Add(holecount0);

                        for (int addhole = 0; addhole < 10 * 3; addhole++)
                        {
                           
                            DataList.Add(prod[i].hole[sizeid][addhole]);
                        }
                        //后角度 第三十个数据才是后角度孔的开始 后面不需要填满
                        int holecount1 = 0;
                        for (int holecount = 30; holecount < prod[i].hole[sizeid].Count(); holecount = holecount + 3)
                        {
                            if (prod[i].hole[sizeid][holecount] > 0)
                                holecount1++;
                        }

                        DataList.Add(prod[i].angle[sizeid][1]);
                        DataList.Add(holecount1);
                        //默认取后面三个个数据
                        for (int addhole = 30; addhole < 30 + holecount1 * 3; addhole++)
                        {
                            DataList.Add(prod[i].hole[sizeid][addhole]);

                        }                                         
                        evokDevice.SetMultiPleDValue(wlInOutPs, DataList.ToArray());
                                              
                        LogManager.WriteProgramLog(DeviceName + Constant.DataDownLoad + wlInOutPs.Addr.ToString());                       

                        DataList.Clear();

                        //地址偏移 按照约定的表格协议来
                        if (sizeid == 0)
                        {
                            wlInOutPs.Addr += 134;
                        }
                        else
                            wlInOutPs.Addr += 132;

                    }

                //恢复地址
                wlInOutPs.Addr = ldsCountInOutPsAddr;
                //检验一下第一组数据就得了 因为其他地址在变 根本没法读取
                if (wlInOutPs.ShowValue == prod[i].Cut.Count)
                {
                    if (evokDevice.SetMValueON(startCountInOutPs)) break;
                }               
            }
            #endregion

            #endregion

                //数据下发 确保正确 下位机需要给一个M16 高电平 我这边来置OFF
                //发数据三次 M16 如果还没有给高电平 就退出
                int valueWriteOk = 0;
                //使用下测长的延时函数 起始和测长差不多的 就是数据下发 等机器确认
                ConstantMethod.DelayMeasure(Constant.PlcCountTimeOut,
                         ref valueWriteOk,
                         ref startCountInOutPs,
                         ref emgStopInPs, ref mRunFlag);

                if (startCountInOutPs.ShowValue != valueWriteOk)
                {
                    MessageBox.Show(Constant.PlcReadDataError);
                    LogManager.WriteProgramLog(DeviceName + Constant.PlcReadDataError);
                    RunFlag = false;
                    //Environment.Exit(0);
                    return;
                }            
        }
        private void DownLoadDataWithDoorShell(int i)
        {

            //数据先分组
            List<List<int>> DataList = new List<List<int>>();
            List<List<int>> DataWidth = new List<List<int>>();
            List<int> sizeInt = new List<int>();
            List<int> sizeIntWidth = new List<int>();
            int ptr = 0;
            if (optSize.ProdInfoLst.Count == 1)
            {

                while (ptr < optSize.ProdInfoLst[0].Cut.Count)
                {
                    if (!RunFlag) break;
                    if (ptr % 50 == 0)
                    {
                        sizeInt = new List<int>();
                        sizeIntWidth = new List<int>();
                        DataList.Add(sizeInt);
                        DataWidth.Add(sizeIntWidth);
                    }

                    sizeInt.Add(optSize.ProdInfoLst[0].Cut[ptr]);
                    int width = 0;
                    if (!int.TryParse(optSize.ProdInfoLst[0].Param1[ptr], out width))
                    {
                        MessageBox.Show("宽度输入错误！");
                        break;
                    }

                    width = width * Constant.dataMultiple;

                    sizeIntWidth.Add(width);
                    ptr++;
                    Application.DoEvents();
                }
            }


            int wlAddr = heightCountInOutPs.Addr;
            int widthAddr = widthCountInOutPs.Addr;
            DataList[0].Insert(0, optSize.ProdInfoLst[0].Cut.Count);
            DataWidth[0].Insert(0, optSize.ProdInfoLst[0].Cut.Count);
            //分段下发数据        
            for (int j = 0; j < DataList.Count(); j++)
            {
               
                if (evokDevice.SetMultiPleDValue(heightCountInOutPs, DataList[j].ToArray()) &&
                evokDevice.SetMultiPleDValue(widthCountInOutPs, DataWidth[j].ToArray()))
                {
                    //发送是料长 但料长不清零 要读取清零的D5000数据 所以只能加延时
                    ConstantMethod.Delay(200);
                    heightCountInOutPs.Addr = heightCountInOutPs.Addr + 2 * DataList[j].Count;
                    //料段数大于0  代表写成功了 
                    widthCountInOutPs.Addr = widthCountInOutPs.Addr + 2 * DataWidth[j].Count;

                }

            }
            heightCountInOutPs.Addr = wlAddr;
            widthCountInOutPs.Addr = widthAddr;
            
            //最后给个启动信号
           // evokDevice.SetMValueOFF2ON(startCountInOutPs);
            
        }
        private void DownLoadDataWithDoorBanWidth(int i)
        {
          
            //数据先分组
            List<List<int>> DataWidth = new List<List<int>>();
            List<int> sizeIntWidth = new List<int>();
            DataWidth.Add(sizeIntWidth);
            for (int j = 0; j < optSize.ProdInfoLst.Count; j++)
            {
                if (!RunFlag) break;
                int ptr = 0;
                while (ptr < optSize.ProdInfoLst[j].Cut.Count)
                {
                    if (!RunFlag) break;
                    if (DataWidth.Count>0 && DataWidth.Last().Count>49 )
                    {

                        sizeIntWidth = new List<int>();

                        DataWidth.Add(sizeIntWidth);
                    }

                    int width = 0;
                    if (!int.TryParse(optSize.ProdInfoLst[j].Param1[ptr], out width))
                    {
                        MessageBox.Show("宽度输入错误！");
                        break;
                    }

                    width = width * Constant.dataMultiple;
                    sizeIntWidth.Add(width);
                    ptr++;
                    Application.DoEvents();
                }
            }
            
            int widthAddr = widthCountInOutPs.Addr;
            int widthCnt = 0;
            for (int m = 0; m < DataWidth.Count; m++)
            {
                widthCnt = widthCnt + DataWidth[m].Count;
            }    
                
            DataWidth[0].Insert(0, widthCnt);
            //分段下发数据        
            for (int j = 0; j < DataWidth.Count(); j++)
            {
                if (!RunFlag) break;
                if (evokDevice.SetMultiPleDValue(widthCountInOutPs, DataWidth[j].ToArray()))
                {
                    //发送是料长 但料长不清零 要读取清零的D5000数据 所以只能加延时
                    ConstantMethod.Delay(100);                 
                    //料段数大于0  代表写成功了 
                    widthCountInOutPs.Addr = widthCountInOutPs.Addr + 2 * DataWidth[j].Count;

                }

            }
            widthCountInOutPs.Addr=  widthAddr;
            if (wlInOutPs.ShowValue > 0)
            {
                //最后给个启动信号
                evokDevice.SetMValueOFF2ON(startCountInOutPs);
            }          
        }
         
        string CurrentDoorType = "123456789";
        private int DoorTypeCount(string doorType,string[] doorNextTypeLst)
        {
            int sum = 0;
            for (int i = 0; i < doorNextTypeLst.Count(); i++)
            {
                if (doorType != doorNextTypeLst[i]) break;
                sum = sum + 1;
            }

            return sum;

        }
        private void SetDoorTypeCutCount(int value)
        {
            List<int> valLst = new List<int>();
            
            valLst.Add(value);
            valLst.Add(1);
            valLst.Add(1);
            evokDevice.SetMultiPleDValue(doorTypeCutCountOutInPs, valLst.ToArray());
        }
        //门型刀数为订单号 参数11 201212231358 修改
        private void CutLoop(int i,int printdMode)
        {
            //打第一条条码 参数10 是门型
            if (optSize.SingleSizeLst[i].Count > 0 && !CurrentDoorType.Equals(optSize.SingleSizeLst[i][0].ParamStrLst[Constant.doorId]))
            {
                //20181015打印第一个尺寸 但是小于最小尺寸 也不打印
                /****
                ChangePrintMode(Constant.AutoBarCode);               
                printBarcode(printReport, optSize.SingleSizeLst[i][0].ParamStrLst.ToArray());
                ****/
                CurrentDoorType = optSize.SingleSizeLst[i][0].ParamStrLst[Constant.doorId];
                PrintBarCheck(optSize.SingleSizeLst[i][0]);
                //下发门型要切的刀数
                if (!DeviceName.Equals(Constant.scjDeivceName))
                {
                    List<string> nextDoorType = new List<string>();
                    for (int m = i; m < optSize.ProdInfoLst.Count(); m++) //把后面的门型 都添加进来
                    {
                        nextDoorType.AddRange(optSize.ProdInfoLst[m].Param10);
                    }
                    int cutCntDoorType = DoorTypeCount(CurrentDoorType, nextDoorType.ToArray());
                    if (cutCntDoorType > 0) SetDoorTypeCutCount(cutCntDoorType);
                }
            }
        
          
            int oldcCount = 0;//保存的老计数值        

            while (RunFlag)
            {
                Application.DoEvents();

                Thread.Sleep(10);
                int newCount = cutDoneOutInPs.ShowValue;

                //这里整理成函数 急停 报错 或者有错误
                if ((!RunFlag || IsInEmg || errorList.Count>0))
                {
                    ConstantMethod.ShowInfo(rtbWork, Constant.emgStopTip);
                    LogManager.WriteProgramLog(DeviceName + Constant.emgStopTip);
                    stopOperation();
                   //stop();
                    return;
                }

                if (newCount != oldcCount && oldcCount < optSize.ProdInfoLst[i].Cut.Count)
                {
                    int oldCutCount = 0;
                    if (!optSize.SingleSizeLst[i][oldcCount].Barc.Equals(Constant.ScarId))
                        if (int.TryParse(optSize.SingleSizeLst[i][oldcCount].DtUser.Rows[optSize.SingleSizeLst[i][oldcCount].Xuhao][2].ToString(), out oldCutCount))
                        {
                            oldCutCount++;
                            //梳齿机有要求 在发现门型变化时 打印一下 后面不再进入到这里 关掉PLC信号
                            if (DeviceProperty == Constant.scjDeivceId)
                            {
                                if (plcHandlebarCodeOutInPs != null)
                                    evokDevice.SetMValueOFF(plcHandlebarCodeOutInPs);
                                // ChangePrintMode(Constant.AutoBarCode);
                            }
                            optSize.SingleSizeLst[i][oldcCount].DtUser.Rows[optSize.SingleSizeLst[i][oldcCount].Xuhao][2] = oldCutCount;
                            optSize.checkIsDone(optSize.SingleSizeLst[i][oldcCount].Xuhao);
                        }

                    ConstantMethod.ShowInfo(rtbWork, Constant.resultTip5 + (oldcCount + 1).ToString() + Constant.size + optSize.ProdInfoLst[i].Cut[oldcCount].ToString() + Constant.startTips5);
                    ConstantMethod.ShowInfo(rtbWork, CurrentDoorType);
                    oldcCount = newCount;
                    if (newCount < optSize.SingleSizeLst[i].Count)
                    {
                        if (!CurrentDoorType.Equals(optSize.SingleSizeLst[i][newCount].ParamStrLst[Constant.doorId]))
                        {
                   
                            CurrentDoorType = optSize.SingleSizeLst[i][newCount].ParamStrLst[Constant.doorId];
                            if (newCount < optSize.SingleSizeLst[i].Count)
                            PrintBarCheck(optSize.SingleSizeLst[i][newCount]);

                            //下发门型要切的刀数
                            if (!DeviceName.Equals(Constant.scjDeivceName))
                            {
                                List<string> nextDoorType = new List<string>();
                                nextDoorType.AddRange(optSize.ProdInfoLst[i].Param10.Skip(newCount).Take(optSize.ProdInfoLst[i].Param10.Count - newCount));
                                for (int m = i + 1; m < optSize.ProdInfoLst.Count(); m++) //把后面的门型 都添加进来
                                {
                                    nextDoorType.AddRange(optSize.ProdInfoLst[m].Param10);
                                }
                                int cutCntDoorType = DoorTypeCount(CurrentDoorType, nextDoorType.ToArray());
                                if (cutCntDoorType > 0) SetDoorTypeCutCount(cutCntDoorType);
                            }
                        }
                    }
                }
                if (newCount >= optSize.ProdInfoLst[i].Cut.Count)
                {
                    break;
                }
            }
            
        }
        
        //如果尺寸小于最小打印尺寸 且是自动打印模式  那就不打印
        private void PrintBarCheck(SingleSize ss) //说明下 这个在参数设置里有保存打印与不打印的标志位 仅用于要打印的情况下 不打印
        {
           // ConstantMethod.ShowInfo(rtbWork, "debug" + minPrinterName+""+printMiniSizeOutInPs.ShowValue+"  "+ss.Cut+"  "+PrintBarCodeMode.ToString()+"\n");

            if (printMiniSizeOutInPs!=null && printMiniSizeOutInPs.ShowValue > ss.Cut && PrintBarCodeMode == Constant.AutoBarCode)
            {
               // ConstantMethod.ShowInfo(rtbWork, "debug ENTER  ");

                //如果选择了 自动打印条码 客户又指定了第二台打印机 那就打印呗
                if (minPrinterName != "" && printMiniSizeOutInPs.ShowValue > ss.Cut && ConstantMethod.GetLocalPrinter().Contains(minPrinterName))
                {
                    printReport.PrintSettings.Printer = minPrinterName;
                    printBarcode(printReport, ss.ParamStrLst.ToArray());
                }
                else printReport.PrintSettings.Printer = ConstantMethod.DefaultPrinter;

                evokDevice.SetMValueOFF
                (plcHandlebarCodeOutInPs);
                //不打印 关掉HM4
                return;
            }
            if (ss.ParamStrLst.ToArray().Length > 0 && ss.ParamStrLst.ToArray()[0].ToString().Equals(Constant.ScarId))
            {
                evokDevice.SetMValueOFF
               (plcHandlebarCodeOutInPs);
                //不打印 关掉HM4
                return;
            }
            //如果之前是需要打印的 恢复PLC 输出
            if ( OldPrintBarCodeMode == Constant.AutoBarCode)
            {
               evokDevice.SetMValueON
              (plcHandlebarCodeOutInPs);
            }

            //1.打印测试
            //在D4880 计数过快的地方 容易统计已切数量不准 改用线程的方式
            //printBarcode(printReport, ss.ParamStrLst.ToArray(), true);

            //2.
            //使用下面这个打印 在D4880 计数太快时 容易统计漏掉已切数量 
            //但在实际当中不会那么快 测试时在 打印间隔包含3秒以上就可以
           

            printBarcode(printReport, ss.ParamStrLst.ToArray());
        }
        private int CutLoop(int i)
        {
            //打第一条条码
            if (optSize.SingleSizeLst[i].Count > 0)
            {
                PrintBarCheck(optSize.SingleSizeLst[i][0]);
            }              

            int oldcCount = 0;//保存的老计数值
            while (RunFlag)
            {
                Application.DoEvents();

                Thread.Sleep(10);

                int newCount  = cutDoneOutInPs.ShowValue;
                
                //这里整理成函数
                if ((!RunFlag || IsInEmg))
                {
                    ConstantMethod.ShowInfo(rtbWork, Constant.emgStopTip);
                    LogManager.WriteProgramLog(DeviceName + Constant.emgStopTip);
                    //  stop();
                    stopOperation();
                    return -1;
                }

                if (ErrorList.Count > 0)
                {
                    RunFlag = false;
                    //stop();
                    stopOperation();
                    return -2;
                }
                if (newCount != oldcCount && oldcCount < optSize.ProdInfoLst[i].Cut.Count)
                {
                    int oldCutCount = 0;
                    if(!optSize.SingleSizeLst[i][oldcCount].Barc.Equals(Constant.ScarId))
                    if (int.TryParse(optSize.SingleSizeLst[i][oldcCount].DtUser.Rows[optSize.SingleSizeLst[i][oldcCount].Xuhao][2].ToString(), out oldCutCount))
                    {
                        oldCutCount++;
                        optSize.SingleSizeLst[i][oldcCount].DtUser.Rows[optSize.SingleSizeLst[i][oldcCount].Xuhao][2] = oldCutCount;
                        optSize.checkIsDone(optSize.SingleSizeLst[i][oldcCount].Xuhao);
                    }
                    if (optSize.ProdInfoLst[i].Param1.Count > 0)
                        ConstantMethod.ShowInfo(rtbWork, Constant.resultTip5 + (oldcCount + 1).ToString() + Constant.size + optSize.ProdInfoLst[i].Cut[oldcCount].ToString() +
                            Constant.startTips6 + optSize.ProdInfoLst[i].Param1[oldcCount].ToString() + Constant.startTips5);
                    else
                    {
                        ConstantMethod.ShowInfo(rtbWork, Constant.resultTip5 + (oldcCount + 1).ToString() + Constant.size + optSize.ProdInfoLst[i].Cut[oldcCount].ToString() +
                              Constant.startTips5);
                    }
                    oldcCount = newCount;

                    
                    //尺寸太短 就不打印 索菲亚增加 20181015 适用于全部版本 
                    if(newCount<optSize.SingleSizeLst[i].Count)
                    PrintBarCheck(optSize.SingleSizeLst[i][newCount]);
                                               
                    
                } 
                              
                if (newCount >= optSize.ProdInfoLst[i].Cut.Count) break;
            }

            return 0;
        }
        private void  CountClr()
        {
            int i = 0;
            while ((!evokDevice.SetDValue(cutDoneOutInPs, 0)&&(i<10)))
            {
                i++;
            }               
        }

        private void CutWorkThreadWithShuchi()
        {
            //从哪一根开始切 暂定 从第一根 开始           
            CurrentDoorType = "1----";
            if (optSize.ProdInfoLst.Count > 0)
            {
                for (int i = CutProCnt; i < optSize.ProdInfoLst.Count; i++)
                {
                    ConstantMethod.ShowInfo(rtbWork, Constant.resultTip5 + (i + 1).ToString() + Constant.startTips4);
                    //plc 计数器 清零
                    CountClr();
                    // 每根数据下发                   
                    DownLoadDataNormalWithShuchi(i);
                    //开始切割进程
                    CutLoop(i,0);

                }
            }
            else
            {
                MessageBox.Show(Constant.noData);
            }

        }
        //当数据数量小于某个数时 开灯提醒 20181019 索菲亚建议
        void CheckIsDataNotEnough(int id)
        {
            int count = 0;

            //测长模式 考虑sizeleft  否则是手动排版的
            if (AutoMes)
            {
                if (optSize.SizeLeft < dataNotEnoughValueOutInPs.ShowValue)
                {
                    OpenDataNotEnough();
                }
            }
            else
            {
                if (id < optSize.ProdInfoLst.Count)
                {
                    for (int i = id; i < optSize.ProdInfoLst.Count; i++)
                    {
                        count = count+optSize.ProdInfoLst[i].Cut.Count;
                    }
                }
                if (count < dataNotEnoughValueOutInPs.ShowValue)
                    OpenDataNotEnough();
                else
                    CloseDataNotEnough();
            }
        }
        /// <summary>
        /// 正常测长切割
        /// </summary>
        private void CutWorkThread()
        {
            //从哪一根开始切 暂定 从第一根 开始                   
            if (optSize.ProdInfoLst.Count > 0)
            {             
                for (int i = CutProCnt; i < optSize.ProdInfoLst.Count; i++)
                {
                   
                    CheckIsDataNotEnough(i);
                    //索菲亚要求 增加生产记录 根据用户配置参数来改
                    SaveProdDataLog(optSize.ProdInfoLst[i], i);
                    ConstantMethod.ShowInfo(rtbWork, Constant.resultTip5 + (i + 1).ToString() + Constant.startTips4);                                      
                    //plc 计数器 清零
                    CountClr();
                    
                    // 每根数据下发                   
                    DownLoadDataNormal(i);
                    //开始切割进程                   
                    CutLoop(i);

                }                
            }
            else
            {
                MessageBox.Show(Constant.noData);
            }

        }

        /// <summary>
        /// 数据发送完成  可以一起同步计数了哦
        /// </summary>
        public void StartCountClr()
        {
            if (!evokDevice.SetMValueOFF(startCountInOutPs))
            {
                ConstantMethod.Delay(100);  //延时一下 判断是否没读到
            }
        }
        #endregion
        #region 优化
        public bool LoadCsvData(string filename)
        {
            if(lcOutInPs!=null)
            optSize.Len = lcOutInPs.ShowValue;
            if (dbcOutInPs != null)
                optSize.Dbc = dbcOutInPs.ShowValue;
            if (ltbcOutInPs != null)
                optSize.Ltbc = ltbcOutInPs.ShowValue;

            if (safeOutInPs != null)
                optSize.Safe = safeOutInPs.ShowValue;

            optSize.WlMiniValue = wlMiniSizeOutInPs.ShowValue;
            return optSize.LoadCsvData(filename);
        }
        //分号分隔符
        public void LoadCsvData0(string filename)
        {
            optSize.Len = lcOutInPs.ShowValue;
            optSize.Dbc = dbcOutInPs.ShowValue;
            optSize.Ltbc = ltbcOutInPs.ShowValue;
            optSize.Safe = safeOutInPs.ShowValue;
            
            optSize.WlMiniValue = wlMiniSizeOutInPs.ShowValue;
            optSize.LoadCsvData0(filename);
        }
        public void LoadExcelData(string filename)
        {
            optSize.Len = lcOutInPs.ShowValue;
            optSize.Dbc = dbcOutInPs.ShowValue;
            optSize.Ltbc = ltbcOutInPs.ShowValue;
            optSize.Safe = safeOutInPs.ShowValue;
            if (wlMiniSizeOutInPs != null)
                optSize.WlMiniValue = wlMiniSizeOutInPs.ShowValue;
            optSize.LoadExcelData(filename);
        }
        #endregion

        #region 自动测长

        private void SelectCutThread(int cutid)
        {
            switch (cutid)
            {
                case Constant.CutNormalMode:
                    {
                        if (CutThreadStart == null)
                            CutThreadStart = new ThreadStart(CutWorkThread);
                        //初始化Thread的新实例，并通过构造方法将委托ts做为参数赋初始值。
                        if (CutThread == null)
                            CutThread = new Thread(CutThreadStart);   //需要引入System.Threading命名空间                 
                        break;
                    }
                case Constant.CutMeasureWithScarSplitNoSize:
                    {
                        if (CutThreadStart == null)
                            CutThreadStart = new ThreadStart(CutWorkThread);
                        //初始化Thread的新实例，并通过构造方法将委托ts做为参数赋初始值。
                        if (CutThread == null)
                            CutThread = new Thread(CutThreadStart);   //需要引入System.Threading命名空间                 
                        break;
                    }
                case Constant.CutMeasureMode:
                    {
                        if (CutThreadStart == null)
                            CutThreadStart = new ThreadStart(CutWorkThread);
                        //初始化Thread的新实例，并通过构造方法将委托ts做为参数赋初始值。
                        if (CutThread == null)
                            CutThread = new Thread(CutThreadStart);   //需要引入System.Threading命名空间                
                        break;
                    }
                case Constant.CutMeasureRotateWithHoleMode:
                    {

                        if (CutThreadStart == null)
                            CutThreadStart = new ThreadStart(CutRotateWithHoleThread);
                        //初始化Thread的新实例，并通过构造方法将委托ts做为参数赋初始值。
                        if (CutThread == null)
                            CutThread = new Thread(CutThreadStart);   //需要引入System.Threading命名空间

                        break;
                    }
                case Constant.CutNormalWithHoleMode:
                    {

                        if (CutThreadStart == null)
                            CutThreadStart = new ThreadStart(CutRotateWithHoleThread);
                        //初始化Thread的新实例，并通过构造方法将委托ts做为参数赋初始值。
                        if (CutThread == null)
                            CutThread = new Thread(CutThreadStart);   //需要引入System.Threading命名空间

              
                        break;
                    }
                case Constant.CutNormalDoorShellMode:
                    {

                        if (CutThreadStart == null)
                            CutThreadStart = new ThreadStart(CutDoorShellThread);
                        //初始化Thread的新实例，并通过构造方法将委托ts做为参数赋初始值。
                        if (CutThread == null)
                            CutThread = new Thread(CutThreadStart);   //需要引入System.Threading命名空间


                        break;
                    }
                case Constant.CutNormalDoorBanMode:
                    {

                        if (CutThreadStart == null)
                            CutThreadStart = new ThreadStart(CutDoorBanThread);
                        //初始化Thread的新实例，并通过构造方法将委托ts做为参数赋初始值。
                        if (CutThread == null)
                            CutThread = new Thread(CutThreadStart);   //需要引入System.Threading命名空间


                        break;
                    }
                case Constant.CutNormalWithShuChiMode:
                    {

                        if (CutThreadStart == null)
                            CutThreadStart = new ThreadStart(CutWorkThreadWithShuchi);
                        //初始化Thread的新实例，并通过构造方法将委托ts做为参数赋初始值。
                        if (CutThread == null)
                            CutThread = new Thread(CutThreadStart);   //需要引入System.Threading命名空间


                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }
        //索菲亚尺寸 优化出来 最好不要一刀 可以设置多刀
        public int CutStartMeasure(bool split, int cutid,int cutCount)
        {
            //先获取默认补偿
            int defaultLtbc = ltbcDefaultOutInPs.ShowValue;

            if (IsInEmg)
            {
                MessageBox.Show(Constant.emgStopTip);
                return -1;
            }
            LogManager.WriteProgramLog(DeviceName + Constant.AutoMeasureMode);

            //启动
            start(cutid);

            //等待 测量
            while (mRunFlag)
            {
                int valueOld = 1;

                LogManager.WriteProgramLog(DeviceName + Constant.MeasureSt);

                ConstantMethod.DelayMeasure(Constant.MeaSureMaxTime, ref valueOld, ref autoCCInPs, ref emgStopInPs, ref mRunFlag);

                if (IsInEmg)
                {
                    //  stop();
                    stopOperation();
                    MessageBox.Show("急停！");
                    return -2;
                }

                LogManager.WriteProgramLog(DeviceName + Constant.MeasureEd);

                if (autoCCInPs.ShowValue == Constant.M_ON)
                {

                    evokDevice.SetMValueOFF(autoCCInPs);

                    optSize.Len = lcOutInPs.ShowValue;

                    if (scarInPs.ShowValue > 0)
                    {

                        //开始优化 结巴 还是测长   
                        if (GetScar(optSize, scarInPs.ShowValue) == Constant.GetScarSuccess)
                        {
                            optSize.Ltbc = defaultLtbc;
                            //进行选择 尺寸与结疤分离 还是单独去除结疤
                            if (cutid == Constant.CutMeasureWithScarSplitNoSize)
                            {
                                optSize.OptMeasureWithScarCheckAndNoSize(split, rtbResult, optSize.DtData);
                            }
                            else //开始优化进行          
                                optSize.OptMeasureWithScarCheck(split, rtbResult, optSize.DtData);
                        }
                        else
                        {
                            MessageBox.Show(Constant.GetScarError);
                            return -2;
                        }
                    }
                    else
                    {
                        optSize.Ltbc = defaultLtbc;
                        optSize.OptMeasure(rtbResult);
                    }

                    if (optSize.ProdInfoLst.Count < 1)
                    {
                        //20180922当有数据的时候 没有合适的料长 则要告诉PLC 进行处理 然后继续等待下一根料
                        if (optSize.ValueAbleRow.Count > 0)
                        {
                            noSizeToCut();
                            goto NEXTOPT;
                        }
                        else
                        {
                            //MessageBox.Show(Constant.noData);
                            break;
                        }

                    }
                }
                else
                {
                    MessageBox.Show(Constant.measureOutOfTime);
                    return -3;
                }

                try
                {

                    SelectCutThread(cutid);

                    if (!CutThread.IsAlive)
                        CutThread.Start();
                    while (CutThread.IsAlive)
                    {
                        Application.DoEvents();
                    }
                }
                finally
                {
                    CutThread = null;
                    CutThreadStart = null;
                }

                NEXTOPT:
                ConstantMethod.ShowInfo(rtbWork, Constant.NextOpt);


            }

            stopOperation();
            //测试先隐藏
            MessageBox.Show(Constant.CutEnd);
            return 0;
        }
        public void SetOptSizeParam1(string value1)
        {

            if (SaveOptParam1(value1.ToString()))
            {
                optSize.OptParam1 = int.Parse(value1);
            }           
        }      
        public void SetOptSizeParam1(int value1)
        {
            if (value1 > -1)
            {
                SaveOptParam1(value1.ToString());
                optSize.OptParam1 = value1;
            }
        }
        /// <summary>
        /// 进行结巴数据的获取 需要实际调试 检查结巴数量是否正确
        /// </summary>
        /// <returns></returns>
        private int GetScar(OptSize op,int scarCount)
        {
            if (rtbResult != null) rtbResult.Clear();

            if (!testGetScarData(evokDevice.DataFormLst[Constant.ScarPage])) return Constant.GetScarWrongScar;

            if (scarCount == evokDevice.DataFormLst[Constant.ScarPage].Rows.Count)
            {
                //清空数据
                op.ScarLst.Clear();

                ConstantMethod.ShowInfo(rtbResult,Constant.ScarName+"数量："+(scarCount/2).ToString());

                foreach (DataRow dr in evokDevice.DataFormLst[Constant.ScarPage].Rows)
                {
                    string scarvalueStr = dr[4].ToString();

                    int scarValue = 0;

                    if (int.TryParse(scarvalueStr, out scarValue))
                    {
                        if (scarValue > 0)
                        {
                            ConstantMethod.ShowInfo(rtbResult, Constant.ScarName+"位置：" + scarValue.ToString());
                            op.ScarLst.Insert(0, scarValue);
                        }
                        else
                        {
                            return Constant.GetScarWrongScar;
                        }
                    }
                    else
                    {
                        return Constant.GetScarWrongScar;
                    }
                }                                               
            }
          // ConstantMethod.ShowInfo(rtbResult, "\n");

            return Constant.GetScarSuccess;
        }
        /// <summary>
        /// 在测长过程中 需要检测结巴
        /// </summary>
        /// <param name="cutid"></param>
        /// 
        //增加反馈信号 便于循环实施
        public int CutStartMeasure(bool split,int cutid)
        {
            //先获取默认补偿
            int defaultLtbc = ltbcDefaultOutInPs.ShowValue;
            optSize.Len = lcOutInPs.ShowValue;
            optSize.Dbc = dbcOutInPs.ShowValue;
            optSize.Ltbc = ltbcOutInPs.ShowValue;
            optSize.Safe = safeOutInPs.ShowValue;
            if (wlMiniSizeOutInPs != null)
                optSize.WlMiniValue = wlMiniSizeOutInPs.ShowValue;

            if (IsInEmg)
            {
                MessageBox.Show(Constant.emgStopTip);
                return -1;
            }
            LogManager.WriteProgramLog(DeviceName + Constant.AutoMeasureMode);

            //启动
            start(cutid);

            //等待 测量
            while (mRunFlag)
            {                               
                int valueOld = 1;

                LogManager.WriteProgramLog(DeviceName + Constant.MeasureSt);

                ConstantMethod.DelayMeasure(Constant.MeaSureMaxTime, ref valueOld, ref autoCCInPs,ref emgStopInPs,ref mRunFlag);
               
                if (IsInEmg)
                {
                    stopOperation();

                }

                LogManager.WriteProgramLog(DeviceName + Constant.MeasureEd);

                if (autoCCInPs.ShowValue ==Constant.M_ON)
                {

                    evokDevice.SetMValueOFF(autoCCInPs);

                    optSize.Len = lcOutInPs.ShowValue;
                   
                    if (scarInPs.ShowValue > 0)
                    {
                        
                        //开始优化 结巴 还是测长   
                        if (GetScar(optSize, scarInPs.ShowValue) == Constant.GetScarSuccess)
                        {
                            optSize.Ltbc = defaultLtbc;
                            //进行选择 尺寸与结疤分离 还是单独去除结疤
                            if (cutid == Constant.CutMeasureWithScarSplitNoSize)
                            {
                                optSize.OptMeasureWithScarCheckAndNoSize(split, rtbResult, optSize.DtData);
                            }
                            else //开始优化进行          
                                optSize.OptMeasureWithScarCheck(split, rtbResult, optSize.DtData);
                        }
                        else
                        {
                            MessageBox.Show(Constant.GetScarError);
                            return -2;
                        }
                    }
                    else
                    {
                        optSize.Ltbc = defaultLtbc;
                        optSize.OptMeasure(rtbResult);                        
                    }
                                                       
                    if (optSize.ProdInfoLst.Count < 1)
                    {
                        //20180922当有数据的时候 没有合适的料长 则要告诉PLC 进行处理 然后继续等待下一根料
                        if (optSize.ValueAbleRow.Count>0)
                        {                          
                            noSizeToCut();
                            goto NEXTOPT;
                        }
                        else
                        {
                            //MessageBox.Show(Constant.noData);
                            break;
                        }
                       
                    }                 
                }
                else
                {
                    MessageBox.Show(Constant.measureOutOfTime);
                    return -3;
                }

                try
                {

                    SelectCutThread(cutid);

                    if (!CutThread.IsAlive)
                        CutThread.Start();
                    while (CutThread.IsAlive)
                    {
                        Application.DoEvents();
                    }
                }
                finally
                {
                    CutThread = null;
                    CutThreadStart = null;
                }

               NEXTOPT:
               ConstantMethod.ShowInfo(rtbWork,Constant.NextOpt);
               
                                                           
            }

           // stop();
            stopOperation();
           // emgStop();
            //测试先隐藏
           MessageBox.Show(Constant.CutEnd);
           return 0;
        }
        //有尺寸 但是和长料匹配不上
        public void noSizeToCut()
        {
            evokDevice.SetMValueON(noSizeToCutOutInPs);
            //20180922 根据PLC 要求 给料段数这里发个1
            evokDevice.SetDValue(ldsOutInPs, 1);
        }
        //切割前 获取所有原料信息
        public void CutReady()
        {
            optSize.Len = lcOutInPs.ShowValue;
            optSize.Dbc = dbcOutInPs.ShowValue;
            optSize.Ltbc = ltbcOutInPs.ShowValue;
            optSize.Safe = safeOutInPs.ShowValue;
        }
        //启动
        public bool CutDoorStartNormal(int cutid)
        {
            //启动反馈 //数据监控 //完成反馈
            if (IsInEmg)
            {
                MessageBox.Show(DeviceName + Constant.emgStopTip);
                return false;
            }
            //报错也不启动
            if (errorList.Count > 0)
            {
                MessageBox.Show(DeviceName + Constant.Alarm);
                return false;
            }
            //正常模式需要优化
            if (optSize.ProdInfoLst.Count < 1)
            {
                MessageBox.Show(DeviceName + Constant.noData);
                return false;
            }
            LogManager.WriteProgramLog(DeviceName + Constant.NormalMode);
            

            try
            {
                SelectCutThread(cutid);

                if (!CutThread.IsAlive)
                    CutThread.Start();
                
                while (CutThread.IsAlive)
                {
                    Application.DoEvents();
                }
               
            }
            finally
            {
                CutThread = null;
                CutThreadStart = null;
                //结束了 PLC 要求不发信号           
                // stop();
                stopOperation();
                MessageBox.Show(DeviceName+Constant.CutEnd);
            }

            return true;

        }
        public void showWorkInfo()
        {
            if(deviceStatusId<Constant.constantStatusStr.Count())
            ConstantMethod.ShowInfo(rtbWork, DeviceName + Constant.constantStatusStr[deviceStatusId]);
        }
        public void showWorkInfo(string str)
        {
            ConstantMethod.ShowInfo(rtbWork, DeviceName +str);
        }
        public void CutStartNormal(int cutid)
        {
            showWorkInfo(Constant.startTips0);
            if (RunFlag)
            {
                MessageBox.Show(DeviceName + Constant.alreadyStart);
                return;
         
            }
            if (IsInEmg)
            {
                MessageBox.Show(DeviceName + Constant.emgStopTip);
                return;
            }

            //正常模式需要优化
            if (optSize.ProdInfoLst.Count < 1)
            {
                MessageBox.Show(DeviceName + Constant.noData);
                return;
            }

            if (errorList.Count > 0)
            {
                MessageBox.Show(DeviceName + Constant.Alarm);
                return ;
            }

            LogManager.WriteProgramLog(DeviceName + Constant.ShuChiMode);

            if (!start(cutid))
            {
               
                MessageBox.Show(DeviceName+Constant.DeviceStartFailed);
                return;
            }
           showWorkInfo(Constant.startTips1);
            try
            {

                SelectCutThread(cutid);

                if (!CutThread.IsAlive)
                    CutThread.Start();

                while (CutThread.IsAlive)
                {
                    Application.DoEvents();
                  
                }               
            }
            finally
            {
                CutThread = null;
                CutThreadStart = null;
                stopOperation();
                //结束了 PLC 要求不发信号 
                //20190227 索菲亚要求添加 
               // emgStop();
                //stop();
                MessageBox.Show(DeviceName + Constant.CutEnd);
            }
        }
        //自动测长开
        public void autoMesON()
        {
            evokDevice.SetMValueOFF(autoMesOutInPs);
        }

        public void autoMesOFF()
        {
            evokDevice.SetMValueON(autoMesOutInPs);
        }
        public void lliaoON()
        {
            evokDevice.SetMValueON(lliaoOutInPs);
        }

        public void lliaoOFF()
        {
            evokDevice.SetMValueOFF(lliaoOutInPs);
        }
        #endregion      
        public void Dispose()
        {
            RunFlag = false;
            ConstantMethod.Delay(100);
            //保存文件
            SaveFile();
            if (evokDevice != null)
                evokDevice.DeviceShutDown();
            if(printReport != null)
            printReport.Dispose();
            if (CutThread != null && CutThread.IsAlive)
            {
                CutThread.Join();
            }
            SetOptSizeParam1(optSize.OptParam1);
            LogManager.WriteProgramLog(DeviceName + Constant.Quit);
        }
        //数据不够的时候 就报警
        public void OpenDataNotEnough()
        {
            if(dataNotEnoughOutInPs!=null)
            evokDevice.SetMValueON(dataNotEnoughOutInPs);
        }
        public void CloseDataNotEnough()
        {
            if(dataNotEnoughOutInPs!=null)
            evokDevice.SetMValueOFF(dataNotEnoughOutInPs);
        }
        public void InitControl()
        {


            if ((evokDevice.DataFormLst.Count > 0) && (evokDevice.DataFormLst[Constant.AutoPage] != null))
            {
                ConstantMethod.FindPos(evokDevice.DataFormLst[Constant.AutoPage], PsLstAuto);
            }
            if ((evokDevice.DataFormLst.Count > 0) && (evokDevice.DataFormLst[Constant.HandPage] != null))
            {
                ConstantMethod.FindPos(evokDevice.DataFormLst[Constant.HandPage], PsLstHand);
            }
            if ((evokDevice.DataFormLst.Count > 0) && (evokDevice.DataFormLst[Constant.ParamPage] != null))
            {
                ConstantMethod.FindPos(evokDevice.DataFormLst[Constant.ParamPage], PsLstParam);
            }
        }

        public int deviceStatusId
        {
            get {
                if (deviceStatusOutInPs == null) return 0;
                return deviceStatusOutInPs.ShowValue; }
        }
        public void ChangtToAuto()
        {
            evokDevice.SetDValue(pageShiftOutPs, Constant.AutoPageID);
        }
        public bool ShiftPage(int pageid)
        {
            if (CurrentPageId == pageid)
            {              
                return true;
            }
            if (evokDevice.Status == Constant.DeviceConnected)
            {
                //页面切换需要告诉下位机
                if (pageid == Constant.AutoPage)
                {
                    evokDevice.SetDValue(pageShiftOutPs, Constant.AutoPageID);
                }

                if (pageid == Constant.HandPage)
                {
                    evokDevice.SetDValue(pageShiftOutPs, Constant.HandPageID);
                }
                                              
                if (pageid == Constant.ParamPage)
                {
                    if (!ConstantMethod.UserPassWd()) return false;
                }

                evokDevice.shiftDataForm(pageid);

                FindPlcSimpleInPlcInfoLst(pageid);

                CurrentPageId = pageid;

                ConstantMethod.Delay(50);

                return true;

            }
                     
           return false;       
        }

        public bool shiftDataFormSplit(int formid, int rowSt, int count)
        {
            //20180905 消除datagridview 自增后 ，在count这里加了1          
            evokDevice.shiftDataFormSplit(formid, rowSt, count+1);
            return true;
        }

        #region 寄存器操作部分
        private PlcInfoSimple getPsFromPslLst(string tag0, string str0, List<PlcInfoSimple> pslLst)
        {

            foreach (PlcInfoSimple simple in pslLst)
            {
                string str1 = tag0;
                string str2 = simple.Name;
                if (str2.Contains(str0))
                {
                    str2 = str2.Replace(Constant.Write, "");
                    str2 = str2.Replace(Constant.Read, "");

                    if (str1.Equals(str2))
                    {
                        return simple;
                    }
                }
            }
            return null;

        
        }
        public void oppositeBit(PlcInfoSimple p)
        {
            if (p.ShowValue > 0)
            {
                evokDevice.SetMValueOFF(p);
            }
            else
            {
                evokDevice.SetMValueON(p);
            }
        }
        public void oppositeBitClick(string str1, string str2, List<PlcInfoSimple> pLst)
        {
            PlcInfoSimple p = getPsFromPslLst(str1, str2, pLst);
            if (p != null)
            {
                oppositeBit(p);
            }
            else
            {
                MessageBox.Show(Constant.SetDataFail);
            }

        }
        public void  SetMPsOFFToOn(string str1,string str2 ,List<PlcInfoSimple> pLst)
        {
            PlcInfoSimple p = getPsFromPslLst(str1,str2, pLst);
            if (p != null)
            {
                evokDevice.SetMValueOFF2ON(p);
            }
            else
            {
                MessageBox.Show(Constant.SetDataFail);
            }
        }
        public void SetMPsOn(string str1, string str2, List<PlcInfoSimple> pLst)
        {
            PlcInfoSimple p = getPsFromPslLst(str1, str2, pLst);
            if (p != null)
            {
                evokDevice.SetMValueON(p);
            }
            else
            {
                MessageBox.Show(Constant.SetDataFail);
            }
        }

        public bool ParamParamTxt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                double num;

                if (double.TryParse(((TextBox)sender).Text, out num) && num > -1)
                {
                    //  num = num * Constant.dataMultiple;
                    SetDValue(((TextBox)sender).Tag.ToString(), Constant.Write, PsLstParam, ((TextBox)sender).Text);
                }

                return true;
            }
            return false;
        }


        public void SetInEdit(string str1, string str2, List<PlcInfoSimple> pLst)
        {
            PlcInfoSimple p = getPsFromPslLst(str1, str2, pLst);
            if (p != null)
            {
                p.IsInEdit = true;
            }
            else
            {
                MessageBox.Show(Constant.SetDataFail);
            }
        }
        public void SetOutEdit(string str1, string str2, List<PlcInfoSimple> pLst)
        {
            PlcInfoSimple p = getPsFromPslLst(str1, str2, pLst);
            if (p != null)
            {
                p.IsInEdit = false;
            }
            else
            {
                MessageBox.Show(Constant.SetDataFail);
            }
        }
        public bool KeyPressSetValue(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                return SetDValue(((TextBox)sender).Tag.ToString(), Constant.Write, PsLstAuto, ((TextBox)sender).Text); 
            }
            return false;
        }
        public bool HandParamTxt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                double num;

                if (double.TryParse(((TextBox)sender).Text, out num) && num > -1)
                {
                  //  num = num * Constant.dataMultiple;
                    SetDValue(((TextBox)sender).Tag.ToString(), Constant.Write, PsLstHand, ((TextBox)sender).Text);
                }
                return true;
            }
            return false;
        }
        public bool AutoParamTxt_KeyPress0(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                double num;

                if (double.TryParse(((TextBox)sender).Text, out num) && num > -1)
                {
                    SetDValue(((TextBox)sender).Tag.ToString(), Constant.Write, PsLstAuto, ((TextBox)sender).Text);
                }
                return true;
            }
            return false;
        }

        public bool AutoParamTxt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                double num;

                if (double.TryParse(((TextBox)sender).Text, out num) && num > -1)
                {
                    num = num * Constant.dataMultiple;
                    
                    SetDValue(((TextBox)sender).Tag.ToString(), Constant.Write, PsLstAuto, (int)num);
                }
                return true;              
            }
            return false;
        }
        //设置适用于 人工设置 比例关系
        public bool SetDValue(string str1, string str2, List<PlcInfoSimple> pLst, string num)
        {
            PlcInfoSimple p = getPsFromPslLst(str1, str2, pLst);

            double valueShift = 0;

            if (double.TryParse(num, out valueShift))
            {

                if (!(valueShift <= p.MaxValue && valueShift >= p.MinValue))
                {
                    MessageBox.Show(Constant.dataOutOfRange + p.MinValue.ToString() + "--" + p.MaxValue.ToString());
                    return false;
                }
                valueShift = valueShift * p.Ration;
                                                             
            }

         

            if (p != null)
            {

                return evokDevice.SetDValue(p, (int)valueShift);

            }
            else
            {
                MessageBox.Show(Constant.SetDataFail);
             
            }
            return false;
        }
        public void SetDValue(string str1, string str2, List<PlcInfoSimple> pLst,int num)
        {
            PlcInfoSimple p = getPsFromPslLst(str1, str2, pLst);

            if (!(num <= p.MaxValue && num >= p.MinValue))
            {
                MessageBox.Show(Constant.dataOutOfRange + p.MinValue.ToString() + "--" + p.MaxValue.ToString());
                return;
            }
            if (p != null)
            {
              
              evokDevice.SetDValue(p, num);
                
            }
            else
            {
                MessageBox.Show(Constant.SetDataFail);
            }

        }
        public void SetMPsOff(string str1, string str2, List<PlcInfoSimple> pLst)
        {
            PlcInfoSimple p = getPsFromPslLst(str1, str2, pLst);
            if (p != null)
            {
                evokDevice.SetMValueOFF(p);
            }
            else
            {
                MessageBox.Show(Constant.SetDataFail);
            }
        }
        #endregion

        #region 关于参数设置表格的设定
        public void DgvValueEdit(int rowIndex,int num3)
        {
            string userdata = evokDevice.DataForm.Rows[rowIndex]["addr"].ToString();
            int addr = 0;
            string area = "D";
            string mode = evokDevice.DataForm.Rows[rowIndex]["mode"].ToString();
            ConstantMethod.SplitAreaAndAddr(userdata, ref addr, ref area);
            if ( ((XJPLCPackCmdAndDataUnpack.AreaGetFromStr(area) > -1) && (XJPLCPackCmdAndDataUnpack.AreaGetFromStr(area) < 3)))
            {
                evokDevice.WriteSingleDData(addr, num3, area, mode);
            }
        }

        public void dgvParam_CellEndEdit(DataGridView dgvParam,object sender, DataGridViewCellEventArgs e)
        {
            double num3;

            string s = dgvParam.SelectedCells[0].Value.ToString();

            int rowIndex = dgvParam.SelectedCells[0].RowIndex;

            try
            {
                if (double.TryParse(s, out num3))
                {
                    int value = (int)(num3 * Constant.dataMultiple);
                    DgvValueEdit(rowIndex, value);
                }
            }
            catch { }
            finally { DgvInOutEdit(rowIndex, false); }


        }
        public void ShiftDgvParamLang(DataGridView dgvParam,int id)
        {
            if(id == Constant.idChinese)
            dgvParam.Columns[0].DataPropertyName = evokDevice.DataFormLst[2].Columns[Constant.Bin].ToString();
            else
            dgvParam.Columns[0].DataPropertyName = evokDevice.DataFormLst[2].Columns[Constant.strParam10].ToString();
        }
        public void InitDgvParam(DataGridView dgvParam)
        {
            if (evokDevice.DataFormLst.Count > 2)
            {
                dgvParam.AutoGenerateColumns = false;
                dgvParam.DataSource = evokDevice.DataFormLst[2];

                dgvParam.Columns[0].DataPropertyName = evokDevice.DataFormLst[2].Columns[Constant.Bin].ToString();
                dgvParam.Columns[1].DataPropertyName = evokDevice.DataFormLst[2].Columns[Constant.strParam6].ToString();
            }
        }
        DataGridView dgvIO;
        public System.Windows.Forms.DataGridView DgvIO
        {
            get { return dgvIO; }
            set { dgvIO = value; }
        }
        public void InitDgvIO(DataGridView dgvIO0)
        {
            DgvIO = dgvIO0;
            if (evokDevice.DataFormLst.Count > 3)
            {
                dgvIO.AutoGenerateColumns = false;
                dgvIO.DataSource = evokDevice.DataFormLst[3];
                dgvIO.Columns[0].DataPropertyName = evokDevice.DataFormLst[2].Columns[Constant.Bin].ToString();
                dgvIO.Columns[1].DataPropertyName = evokDevice.DataFormLst[2].Columns[Constant.strValue].ToString();
                dgvIO.ReadOnly = true;
            }
        }

        public void DgvInOutEdit(int rowIndex,bool editEnable)
        {        
            string s = evokDevice.DataForm.Rows[rowIndex]["param1"].ToString();
            string str2 = evokDevice.DataForm.Rows[rowIndex]["param2"].ToString();
            string userdata = evokDevice.DataForm.Rows[rowIndex]["addr"].ToString();
            string area = "D";
            int addr = 0;
            ConstantMethod.SplitAreaAndAddr(userdata, ref addr, ref area);
            int result = 0;
            int num4 = 0;
            if (int.TryParse(s, out result) && int.TryParse(str2, out num4))
            {
                
                if (XJPLCPackCmdAndDataUnpack.AreaGetFromStr(area) < 3)
                {
                    if (result < evokDevice.DPlcInfo.Count)
                    evokDevice.DPlcInfo[result].IsInEdit = editEnable;
                }
                else
                {
                    if (result < evokDevice.MPlcInfoAll.Count)
                        evokDevice.MPlcInfoAll[result][num4].IsInEdit = editEnable;
                }
            }
        }
        #endregion;

        #region 缓冲区中有个plcinfo类 存储了 PLC 的实时数据 PlcInfoSimple 则是用户进行对接的操作对象 两者进行连接
        /// <summary>
        /// plcsimple 与缓冲区中的类绑定 便于后续读取值 缓冲区的类 实时更新数据 
        /// plcsimpele 进行与用户的操作绑定
        /// </summary>
        /// <param name="m"></param>
        private void FindPlcSimpleInPlcInfoLst(int m)
        {

            foreach (List<PlcInfoSimple> pLst in AllPlcSimpleLst)
            {
                foreach (PlcInfoSimple p in pLst)
                {
                    FindPlcInfo(p, evokDevice.DPlcInfo, evokDevice.MPlcInfoAll);
                }
            }
            /****
            if (m == 0)
                for (int i = 0; i <  PsLstAuto.Count; i++)
                {
                    FindPlcInfo( PsLstAuto[i], evokDevice.DPlcInfo, evokDevice.MPlcInfoAll);
                }
            if (m == 1)
                for (int i = 0; i <  PsLstHand.Count; i++)
                {
                    FindPlcInfo(PsLstHand[i], evokDevice.DPlcInfo, evokDevice.MPlcInfoAll);
                }
            if (m == 2)
                for (int i = 0; i <  PsLstParam.Count; i++)
                {
                    FindPlcInfo(PsLstParam[i], evokDevice.DPlcInfo, evokDevice.MPlcInfoAll);
                }
                ***/

        }
        private void FindPlcInfo(PlcInfoSimple p, List<XJPlcInfo> dplc, List<List<XJPlcInfo>> mplc)
        {
            if (p.Area == null) return;
            if (dplc == null || 
                mplc == null || 
                dplc.Count == 0 || 
                mplc.Count == 0  
                ) return;
            foreach (XJPlcInfo p0 in dplc)
            {
                if ((p0.RelAddr == p.Addr) && (p0.StrArea.Equals(p.Area.Trim())))
                {
                    p.SetPlcInfo(p0);
                    return;
                }
            }
                    
            for (int i = 0; i < mplc.Count; i++)
            {
                for (int j = 0; j < mplc[i].Count; j++)
                {
                    
                    if ((mplc[i][j].RelAddr == p.Addr) && (mplc[i][j].StrArea.Equals(p.Area.Trim())))
                    {
                        p.SetPlcInfo(mplc[i][j]);
                        return;
                    }

                }
            }
        }
        #endregion
    }


    public class barCodeManger
    {
        public barCodeManger()
        {

        }
        public barCodeManger(FastReport.Report rp0)
        {
            Rp1 = rp0;
        }

        private Report rp1;
        public FastReport.Report Rp1
        {
            get { return rp1; }
            set { rp1 = value; }
        }
        private string[] paramCode;
        public string[] ParamCode
        {
            get { return paramCode; }
            set { paramCode = value; }
        }
    }
}
