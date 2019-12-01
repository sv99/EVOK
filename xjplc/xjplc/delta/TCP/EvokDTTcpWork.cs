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
using xjplc.delta;
using xjplc.delta.TCP;

namespace xjplc
{
    public class EvokDTTcpWork
    {
        //用户排版数据
        DataTable userDataTable;

        OptSize optSize;
        //设备类
        //EvokDTDevice evokDevice;
        DTTcpDevice evokDevice;
        //显示工作信息
        RichTextBox rtbWork;

        //打印的报表
        FastReport.Report printReport;

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


        public ConfigFileManager ParamFile
        {
            get { return paramFile; }

            set
            {
                paramFile = value;
            }
        }

        //
        List<List<DTPlcInfoSimple>> allPlcSimpleLst;
        public System.Collections.Generic.List<System.Collections.Generic.List<xjplc.DTPlcInfoSimple>> AllPlcSimpleLst
        {
            get { return allPlcSimpleLst; }
            set { allPlcSimpleLst = value; }
        }

        public DataTable UserDataTable
        {
            get { return userDataTable; }
            set { userDataTable = value; }
        }
        public DataTable GetDataForm(int id)
        {
            if (id < DataFormCount)
            {
                return evokDevice.DataFormLst[id];
            }
            else return null;
        }

        public void ShowBarCode(Report rp1, int rowindex)
        {
            List<string> valuestr = new List<string>();

            if (optSize.DtData != null && optSize.DtData.Rows.Count > 0)
            {            
                DataRow dr = optSize.DtData.Rows[rowindex];
                for (int j = 3; j < optSize.DtData.Columns.Count; j++)
                {
                    valuestr.Add(dr[j].ToString());
                }
                printBarcode(rp1, valuestr.ToArray(), 0);
            }
            else
            {
                MessageBox.Show("无数据，请先导出数据！");
            }
        }
        public int DataFormCount
        {
            get { return evokDevice.DataFormLst.Count; }
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
        public bool AutoMes
        { 
            get {

                if (autoMesOutInPs.ShowValue ==  Constant.M_OFF)
                {
                    return true;
                }
                else return false;
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
        bool mRunFlag ;
        public bool RunFlag
        {
            get { return mRunFlag; }
            set { mRunFlag = value; }
        }
        ThreadStart CutThreadStart;
        //初始化Thread的新实例，并通过构造方法将委托ts做为参数赋初始值。
        Thread CutThread;   //需要引入System.Threading命名空间

        List<string> errorList;
        public System.Collections.Generic.List<string> ErrorList
        {
            get { return errorList; }
            set { errorList = value; }
        }
        public void SetUserDataGridView(DataGridView dgv1)
        {
            optSize.UserDataView = dgv1;
        }

        public void SetOptSize(OptSize optSize0)
        {
            optSize = optSize0;
        }

        public void SetRtbWork(RichTextBox  richrtbWork0)
        {
            rtbWork = richrtbWork0;
        }

        public void SetRtbResult(RichTextBox richrtbWork0)
        {
            rtbResult = richrtbWork0;
        }

        public bool DeviceStatus {
            get {

                return evokDevice.Status==Constant.DeviceConnected;
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
                MessageBox.Show("条码文件不存在");
            }
            else
            {
                if (Directory.GetFiles(FilePath, filter).Length > 1)
                {
                    MessageBox.Show("多个条码文件，请点击条码查看选择");
                }
                if (Directory.GetFiles(FilePath, filter).Length == 1)
                {
                    printReport.Load(getbarcodepath[0]);
                }
            }

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

        public void SetPrintReport(FastReport.Report r1)
        {
            if (r1 != null) printReport = r1;
            string filter = "*.frx";
            string FilePath = System.AppDomain.CurrentDomain.BaseDirectory;
            string[] getbarcodepath;
            getbarcodepath = Directory.GetFiles(FilePath, filter);
            if (Directory.GetFiles(FilePath, filter).Length == 0)
            {
                MessageBox.Show("条码文件不存在");
            }
            else
            {
                if (Directory.GetFiles(FilePath, filter).Length > 1)
                {
                    MessageBox.Show("多个条码文件，请点击条码查看选择");
                }
                if (Directory.GetFiles(FilePath,filter).Length == 1)
                {
                    printReport.Load(getbarcodepath[0]);
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
        public void ChangePrintMode(int value)
        {
            if (PrinterSettings.InstalledPrinters.Count == 0)
            {
                value = 0;
                LogManager.WriteProgramLog(Constant.DeviceNoPrinter);
            }

            paramFile.WriteConfig(Constant.printBarcodeMode, value.ToString());

             printBarCodeMode = value;

            if (printBarCodeMode == Constant.AutoBarCode)
            {
              evokDevice.SetMValueON(plcHandlebarCodeOutInPs);               
            }
            else
            {
              evokDevice.SetMValueOFF(plcHandlebarCodeOutInPs);
            }

                        
        }


        int deviceId;
        public int DeviceId
        {
            get { return deviceId; }
            set { deviceId = value; }
        }
        #region 自动
        //自动页面
        List<DTPlcInfoSimple> psLstAuto;
        public List<xjplc.DTPlcInfoSimple> PsLstAuto
        {
            get { return psLstAuto; }
            set { psLstAuto = value; }
        }
        //定义后要加入集合  //忽略寄存器的影响直接匹配参数名     

        #region 自动页面通用信号集合

        public DTPlcInfoSimple startOutPs = new DTPlcInfoSimple("启动写");
        public DTPlcInfoSimple startInPs = new DTPlcInfoSimple("启动读");

        public DTPlcInfoSimple resetOutPs = new DTPlcInfoSimple("复位写");
        public DTPlcInfoSimple resetInPs = new DTPlcInfoSimple("复位读");

        public DTPlcInfoSimple emgStopInPs = new DTPlcInfoSimple("急停读");

        public DTPlcInfoSimple pauseInPs = new DTPlcInfoSimple("暂停读");  
        public DTPlcInfoSimple pauseOutPs = new DTPlcInfoSimple("暂停写");

        public DTPlcInfoSimple YLCOutInPs = new DTPlcInfoSimple("原料长度读写");
        public DTPlcInfoSimple YLKOutInPs = new DTPlcInfoSimple("原料宽度读写");

        public DTPlcInfoSimple sizeLengthOutInPs = new DTPlcInfoSimple("加工长度读写");
        public DTPlcInfoSimple sizeWidthOutInPs = new DTPlcInfoSimple("加工宽度读写");

        public DTPlcInfoSimple prodOutInPs = new DTPlcInfoSimple("总产量读写");

        public DTPlcInfoSimple stopInPs = new DTPlcInfoSimple("停止读");
        public DTPlcInfoSimple stopOutPs = new DTPlcInfoSimple("停止写");

        public DTPlcInfoSimple cutSizeRangeChangOutInPs = new DTPlcInfoSimple("加工限制长读写");
        public DTPlcInfoSimple cutSizeRangeKuanOutInPs = new DTPlcInfoSimple("加工限制宽读写");
        #endregion

        #region 门锁机

        DataTable dtScHyShow = new DataTable();
    
        public DTPlcInfoSimple gwShiftInOutPs = new DTPlcInfoSimple("工位指示读写");
        public DTPlcInfoSimple knifeSelectInOutPs = new DTPlcInfoSimple("刀具选择读写");
        public DTPlcInfoSimple programNoShiftInOutPs = new DTPlcInfoSimple("程序号读写");
        public DTPlcInfoSimple openProgramOutPs = new DTPlcInfoSimple("打开选定程序写");
        public DTPlcInfoSimple openProgramBarCodeOutPs = new DTPlcInfoSimple("扫码选定程序写");
        public DTPlcInfoSimple selectProgramOutPs = new DTPlcInfoSimple("当前程序写");
        public DTPlcInfoSimple selectProgramInPs = new DTPlcInfoSimple("当前程序读");
        public DTPlcInfoSimple scProgramOutPs = new DTPlcInfoSimple("锁槽写");
        public DTPlcInfoSimple hyProgramOutPs = new DTPlcInfoSimple("合页写");
        public DTPlcInfoSimple scSureProgramOutPs = new DTPlcInfoSimple("锁槽确认写");
        public DTPlcInfoSimple hySureProgramOutPs = new DTPlcInfoSimple("合页确认写");
        public DTPlcInfoSimple doorWidthInOutPs = new DTPlcInfoSimple("门宽读写");
        public DTPlcInfoSimple doorHeightInOutPs = new DTPlcInfoSimple("门长读写");
        public DTPlcInfoSimple doorThicknessInOutPs = new DTPlcInfoSimple("合页侧厚度读写");
        public DTPlcInfoSimple hyModeInOutPs = new DTPlcInfoSimple("合页模式读写");
        public DTPlcInfoSimple hyCountInOutPs = new DTPlcInfoSimple("合页数量读写");
        public System.Data.DataTable DtScHyShow
        {
            get { return dtScHyShow; }
            set { dtScHyShow = value; }
        }
        public List<DTPlcInfoSimple> ProgramConfigPsLst = new List<DTPlcInfoSimple>();

        public DTPlcInfoSimple errorResetOutPs = new DTPlcInfoSimple("报警清除写");



        public List<DTPlcInfoSimple> hyDataInLst = new List<DTPlcInfoSimple>();
        public List<DTPlcInfoSimple> scLightInLst = new List<DTPlcInfoSimple>();
        public List<DTPlcInfoSimple> scDataInLst = new List<DTPlcInfoSimple>();
        public List<DTPlcInfoSimple> hyLightInLst = new List<DTPlcInfoSimple>();





        #endregion

        #region 门套机

        public DTPlcInfoSimple hyViewOutInPs = new DTPlcInfoSimple("合页显示画面读写");
        public DTPlcInfoSimple hyCountViewOutInPs = new DTPlcInfoSimple("三合页读写");
        public DTPlcInfoSimple kaoshanViewOutInPs = new DTPlcInfoSimple("左靠山读写");
       
        #endregion
        public DTPlcInfoSimple handModeOutInPs = new DTPlcInfoSimple("手动模式读写");
        public DTPlcInfoSimple autoModeOutInPs = new DTPlcInfoSimple("自动模式读写");      
        public DTPlcInfoSimple errorResetOutInPs = new DTPlcInfoSimple("报警清除读写");
        public DTPlcInfoSimple angleModeOutInPs = new DTPlcInfoSimple("角度模式选择读写");
        public DTPlcInfoSimple autoModeLightInPs = new DTPlcInfoSimple("自动运行读");

        public DTPlcInfoSimple alarm1InPs = new DTPlcInfoSimple("报警1");
        public DTPlcInfoSimple alarm2InPs = new DTPlcInfoSimple("报警2");
        public DTPlcInfoSimple alarm3InPs = new DTPlcInfoSimple("报警3");
        public DTPlcInfoSimple alarm4InPs = new DTPlcInfoSimple("报警4");
        public DTPlcInfoSimple alarm5InPs = new DTPlcInfoSimple("报警5");
        public DTPlcInfoSimple alarm6InPs = new DTPlcInfoSimple("报警6");
        public DTPlcInfoSimple alarm7InPs = new DTPlcInfoSimple("报警7");
        public DTPlcInfoSimple alarm8InPs = new DTPlcInfoSimple("报警8");
        public DTPlcInfoSimple alarm9InPs = new DTPlcInfoSimple("报警9");
        public DTPlcInfoSimple alarm10InPs = new DTPlcInfoSimple("报警10");
        public DTPlcInfoSimple alarm11InPs = new DTPlcInfoSimple("报警11");
        public DTPlcInfoSimple alarm12InPs = new DTPlcInfoSimple("报警12");
        public DTPlcInfoSimple alarm13InPs = new DTPlcInfoSimple("报警13");
        public DTPlcInfoSimple alarm14InPs = new DTPlcInfoSimple("报警14");
        public DTPlcInfoSimple alarm15InPs = new DTPlcInfoSimple("报警15");
        public DTPlcInfoSimple alarm16InPs = new DTPlcInfoSimple("报警16");
        public DTPlcInfoSimple alarm17InPs = new DTPlcInfoSimple("报警17");
        public DTPlcInfoSimple alarm18InPs = new DTPlcInfoSimple("报警18");
        public DTPlcInfoSimple alarm19InPs = new DTPlcInfoSimple("报警19");
        public DTPlcInfoSimple alarm20InPs = new DTPlcInfoSimple("报警20");
        public DTPlcInfoSimple alarm21InPs = new DTPlcInfoSimple("报警21");
        public DTPlcInfoSimple alarm22InPs = new DTPlcInfoSimple("报警22");
        public DTPlcInfoSimple alarm23InPs = new DTPlcInfoSimple("报警23");
        public DTPlcInfoSimple alarm24InPs = new DTPlcInfoSimple("报警24");
        public DTPlcInfoSimple alarm25InPs = new DTPlcInfoSimple("报警25");
        public DTPlcInfoSimple alarm26InPs = new DTPlcInfoSimple("报警26");
        public DTPlcInfoSimple alarm27InPs = new DTPlcInfoSimple("报警27");
        public DTPlcInfoSimple alarm28InPs = new DTPlcInfoSimple("报警28");
        public DTPlcInfoSimple alarm29InPs = new DTPlcInfoSimple("报警29");
        public DTPlcInfoSimple alarm30InPs = new DTPlcInfoSimple("报警30");
        public DTPlcInfoSimple alarm31InPs = new DTPlcInfoSimple("报警31");
        public DTPlcInfoSimple alarm32InPs = new DTPlcInfoSimple("报警32");
        public DTPlcInfoSimple alarm33InPs = new DTPlcInfoSimple("报警33");
        public DTPlcInfoSimple alarm34InPs = new DTPlcInfoSimple("报警34");
        public DTPlcInfoSimple alarm35InPs = new DTPlcInfoSimple("报警35");
        public DTPlcInfoSimple alarm36InPs = new DTPlcInfoSimple("报警36");
        public DTPlcInfoSimple alarm37InPs = new DTPlcInfoSimple("报警37");
        public DTPlcInfoSimple alarm38InPs = new DTPlcInfoSimple("报警38");
        public DTPlcInfoSimple alarm39InPs = new DTPlcInfoSimple("报警39");
        public DTPlcInfoSimple alarm40InPs = new DTPlcInfoSimple("报警40");
        public DTPlcInfoSimple alarm41InPs = new DTPlcInfoSimple("报警41");
        public DTPlcInfoSimple alarm42InPs = new DTPlcInfoSimple("报警42");
        public DTPlcInfoSimple alarm43InPs = new DTPlcInfoSimple("报警43");
        public DTPlcInfoSimple alarm44InPs = new DTPlcInfoSimple("报警44");
        public DTPlcInfoSimple alarm45InPs = new DTPlcInfoSimple("报警45");
        public DTPlcInfoSimple alarm46InPs = new DTPlcInfoSimple("报警46");
        public DTPlcInfoSimple alarm47InPs = new DTPlcInfoSimple("报警47");
        public DTPlcInfoSimple alarm48InPs = new DTPlcInfoSimple("报警48");
        public DTPlcInfoSimple alarm49InPs = new DTPlcInfoSimple("报警49");
        public DTPlcInfoSimple alarm50InPs = new DTPlcInfoSimple("报警50");
        public DTPlcInfoSimple alarm51InPs = new DTPlcInfoSimple("报警51");

        public DTPlcInfoSimple autoMesOutInPs = new DTPlcInfoSimple("自动测长标志读写");
        public DTPlcInfoSimple dbcOutInPs = new DTPlcInfoSimple("刀补偿读写");
        public DTPlcInfoSimple ltbcOutInPs = new DTPlcInfoSimple("料头补偿读写");
        public DTPlcInfoSimple safeOutInPs = new DTPlcInfoSimple("安全距离读写");

        public DTPlcInfoSimple lcOutInPs = new DTPlcInfoSimple("料长读写");

        public DTPlcInfoSimple cutDoneOutInPs = new DTPlcInfoSimple("切割完毕读写");
        public DTPlcInfoSimple plcHandlebarCodeOutInPs = new DTPlcInfoSimple("条码打印读写");
        public DTPlcInfoSimple startCountInOutPs = new DTPlcInfoSimple("开始计数读写");
        public DTPlcInfoSimple ldsCountInOutPs = new DTPlcInfoSimple("料段数读写");
        public DTPlcInfoSimple autoSLOutPs    = new DTPlcInfoSimple("自动上料写");
        public DTPlcInfoSimple pageShiftInOutPs = new DTPlcInfoSimple("页面切换读写");          
        public DTPlcInfoSimple autoSLInPs     = new DTPlcInfoSimple("自动上料读");
        public DTPlcInfoSimple autoCCInPs     = new DTPlcInfoSimple("自动测长读");
        public DTPlcInfoSimple clInPs         = new DTPlcInfoSimple("出料读");
        public DTPlcInfoSimple slInPs         = new DTPlcInfoSimple("送料读");

        
        #endregion
        #region 手动
        List<DTPlcInfoSimple> psLstHand;
        public System.Collections.Generic.List<xjplc.DTPlcInfoSimple> PsLstHand
        {
            get { return psLstHand; }
            set { psLstHand = value; }
        }
        #endregion
        #region 参数
        List<DTPlcInfoSimple> psLstParam;
        public System.Collections.Generic.List<xjplc.DTPlcInfoSimple> PsLstParam
        {
            get { return psLstParam;  }
            set { psLstParam = value; }
        }

        #endregion
        #region IO监控
        List<DTPlcInfoSimple> psLstIO;
        public System.Collections.Generic.List<xjplc.DTPlcInfoSimple> PsLstIO
        {
            get { return psLstIO; }
            set { psLstIO = value; }
        }
        #endregion
        #region 参数1
        List<DTPlcInfoSimple> psLstParam1;
        public System.Collections.Generic.List<xjplc.DTPlcInfoSimple> PsLstParam1
        {
            get { return psLstParam1; }
            set { psLstParam1 = value; }
        }
        #endregion

        //页面切换要读写的吧
        public void ShiftShowPage(int id)
        {
            string[] valurStr = { id.ToString() };
            evokDevice.SetDValue(pageShiftInOutPs, valurStr);
        }
        public void SetPage(int id)
        {
            if (evokDevice.DataFormLst.Count > 1 && evokDevice.DataFormLst[id].Rows.Count > 0 && id< evokDevice.DataFormLst.Count && id < AllPlcSimpleLst.Count)
            {

                AllPlcSimpleLst[id].Clear();
                foreach (DataRow dr in evokDevice.DataFormLst[id].Rows)
                {
                    if (dr == null) continue;
                    string name = dr["bin"].ToString();
                    //获取比率 最大值 最小值

                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        DTPlcInfoSimple p = new DTPlcInfoSimple(name);
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
                            
                            ConstantMethod.getAddrAndAreaByStr(userdata, ref addrInt, ref areaStr,DeviceId);
                            // ConstantMethod.SplitAreaAndAddr(userdata, ref addrInt, ref areaStr);
                            //区域符号在前面 后面地址就可以计算了
                            p.Area = areaStr;
                            p.Addr = addrInt;

                            if (evokDevice.DataFormLst[id].Columns.Contains("param7"))
                            {
                                string ration = dr["param7"].ToString();
                                if (!string.IsNullOrWhiteSpace(ration)) p.Ration = int.Parse(ration);

                            }
                            if (evokDevice.DataFormLst[id].Columns.Contains("param8"))
                            {
                                string max = dr["param8"].ToString();
                                if (!string.IsNullOrWhiteSpace(max)) p.MaxValue = int.Parse(max);
                            }
                            if (evokDevice.DataFormLst[id].Columns.Contains("param9"))
                            {
                                string min = dr["param9"].ToString();
                                if (!string.IsNullOrWhiteSpace(min)) p.MinValue = int.Parse(min);
                            }

                            AllPlcSimpleLst[id].Add(p);
                        }
                        catch { }    
                        
                    }
                }
            }
        }        
        public void InitUsual()
        {

            PsLstHand = new List<DTPlcInfoSimple>();
            PsLstAuto = new List<DTPlcInfoSimple>();
            PsLstParam = new List<DTPlcInfoSimple>();
            PsLstIO = new List<DTPlcInfoSimple>();
            AllPlcSimpleLst = new List<List<DTPlcInfoSimple>>();

            AllPlcSimpleLst.Add(psLstAuto);
            AllPlcSimpleLst.Add(psLstHand);
            AllPlcSimpleLst.Add(psLstParam);
            AllPlcSimpleLst.Add(PsLstIO);

            paramFile = ConstantMethod.configFileBak(Constant.ConfigParamFilePath);

            #region 自动页面数据处理

            SetPage(Constant.AutoPage);
            
            #endregion
            #region handpage

            //手动自动读取
            SetPage(Constant.HandPage);

            #endregion
            //参数页面
            SetPage(Constant.ParamPage);

            #region 设备处理
            #region 门锁机

            if (deviceId == Constant.msjDeivceId)
            {

                errorResetOutPs = ConstantMethod.getDtPlcSimple(errorResetOutPs.Name, psLstAuto);
                pageShiftInOutPs = ConstantMethod.getDtPlcSimple(pageShiftInOutPs.Name, psLstAuto);
                openProgramOutPs = ConstantMethod.getDtPlcSimple(openProgramOutPs.Name, psLstAuto);
                openProgramBarCodeOutPs = ConstantMethod.getDtPlcSimple(openProgramBarCodeOutPs.Name, psLstAuto);
                selectProgramOutPs = ConstantMethod.getDtPlcSimple(selectProgramOutPs.Name, psLstAuto);
                selectProgramInPs = ConstantMethod.getDtPlcSimple(selectProgramInPs.Name, psLstAuto);
                scProgramOutPs = ConstantMethod.getDtPlcSimple(scProgramOutPs.Name, psLstAuto);
                hyProgramOutPs = ConstantMethod.getDtPlcSimple(hyProgramOutPs.Name, psLstAuto);
                doorWidthInOutPs = ConstantMethod.getDtPlcSimple(doorWidthInOutPs.Name, psLstAuto);
                doorHeightInOutPs = ConstantMethod.getDtPlcSimple(doorHeightInOutPs.Name, psLstAuto);
                doorThicknessInOutPs = ConstantMethod.getDtPlcSimple(doorThicknessInOutPs.Name, psLstAuto);
                hyModeInOutPs = ConstantMethod.getDtPlcSimple(hyModeInOutPs.Name, psLstAuto);
                hyCountInOutPs = ConstantMethod.getDtPlcSimple(hyCountInOutPs.Name, psLstAuto);
                     
                //添加一个页面
                AllPlcSimpleLst.Add(ProgramConfigPsLst);
                SetPage(Constant.ScarPage);

                knifeSelectInOutPs = ConstantMethod.getDtPlcSimple(knifeSelectInOutPs.Name, ProgramConfigPsLst);
                gwShiftInOutPs = ConstantMethod.getDtPlcSimple(gwShiftInOutPs.Name, ProgramConfigPsLst);
                programNoShiftInOutPs = ConstantMethod.getDtPlcSimple(programNoShiftInOutPs.Name, ProgramConfigPsLst);
                scSureProgramOutPs = ConstantMethod.getDtPlcSimple(scSureProgramOutPs.Name, ProgramConfigPsLst);
                hySureProgramOutPs = ConstantMethod.getDtPlcSimple(hySureProgramOutPs.Name, ProgramConfigPsLst);
               

                DtScHyShow.Columns.Add("步序");
                DtScHyShow.Columns.Add("锁槽工位");
                DtScHyShow.Columns.Add("合页工位");

                string userScHytCount = ParamFile.ReadConfig(Constant.SchyCount);

                int schyCount = 0;
                if (int.TryParse(userScHytCount, out schyCount))
                {

                    if(schyCount>12)
                    for (int i = 0; i < schyCount; i++)
                    {
                        DataRow dr = DtScHyShow.NewRow();
                        dr[0] = (i + 1).ToString();
                        DtScHyShow.Rows.Add(dr);
                    }

                }
                else
                    for (int i = 0; i < 12; i++)
                    {
                        DataRow dr = DtScHyShow.NewRow();
                        dr[0] = (i + 1).ToString();
                        DtScHyShow.Rows.Add(dr);
                    }


            }
            #endregion

            #region 四边锯处理
            if (deviceId == Constant.sbjDeivceId)
            {
                startOutPs = ConstantMethod.getDtPlcSimple(startOutPs.Name, psLstAuto);

                resetOutPs = ConstantMethod.getDtPlcSimple(resetOutPs.Name, psLstAuto);

                angleModeOutInPs = ConstantMethod.getDtPlcSimple(angleModeOutInPs.Name, psLstAuto);
                YLCOutInPs = ConstantMethod.getDtPlcSimple(YLCOutInPs.Name, psLstAuto);
                YLKOutInPs = ConstantMethod.getDtPlcSimple(YLKOutInPs.Name, psLstAuto);
                sizeLengthOutInPs = ConstantMethod.getDtPlcSimple(sizeLengthOutInPs.Name, psLstAuto);
                sizeWidthOutInPs = ConstantMethod.getDtPlcSimple(sizeWidthOutInPs.Name, psLstAuto);
                handModeOutInPs = ConstantMethod.getDtPlcSimple(handModeOutInPs.Name, psLstAuto);
                autoModeOutInPs = ConstantMethod.getDtPlcSimple(autoModeOutInPs.Name, psLstAuto);
                cutSizeRangeChangOutInPs = ConstantMethod.getDtPlcSimple(cutSizeRangeChangOutInPs.Name, psLstAuto);
                cutSizeRangeKuanOutInPs = ConstantMethod.getDtPlcSimple(cutSizeRangeKuanOutInPs.Name, psLstAuto);
            }
            #endregion

            #region 双端锯
            if (deviceId == Constant.xzjDeivceId)
            {
                sizeLengthOutInPs = ConstantMethod.getDtPlcSimple(sizeLengthOutInPs.Name, psLstAuto);
                handModeOutInPs = ConstantMethod.getDtPlcSimple(handModeOutInPs.Name, psLstAuto);
                autoModeOutInPs = ConstantMethod.getDtPlcSimple(autoModeOutInPs.Name, psLstAuto);
                hyViewOutInPs = ConstantMethod.getDtPlcSimple(hyViewOutInPs.Name, psLstAuto);
                hyCountViewOutInPs = ConstantMethod.getDtPlcSimple(hyCountViewOutInPs.Name, psLstAuto);
                kaoshanViewOutInPs = ConstantMethod.getDtPlcSimple(kaoshanViewOutInPs.Name, psLstAuto);
                     
           }
            #endregion


            UserDataTable = new DataTable();        
          
            if (!int.TryParse(paramFile.ReadConfig(Constant.printBarcodeMode), out printBarCodeMode))
            {
                MessageBox.Show(Constant.ErrorParamConfigFile);

                Application.Exit();

                System.Environment.Exit(0);
            }


            LogManager.WriteProgramLog(Constant.Start);
       
            buildCmd(Constant.AutoPage);

            if (!evokDevice.getDeviceData()) 
            {             
                MessageBox.Show(DeviceId + Constant.ConnectMachineFail);
                Environment.Exit(0);
            }
            
            //切换
            evokDevice.SetMValueON(autoModeOutInPs);

            ErrorList = new List<string>();

            optSize = new OptSize();

            ConstantMethod.Delay(200);

            openProgram(selectProgramInPs.ShowValue);

        }

        #region 双端锯

        #region 门套机
        public  int GetScView()
        {
            if (kaoshanViewOutInPs != null)
                return kaoshanViewOutInPs.ShowValue;
            else return 0;
        }
        public int  GetHyCountView()
        {
            if (hyCountViewOutInPs != null)
                return hyCountViewOutInPs.ShowValue;
            else return 0;
        }

        public  int GetHyView
        {

            get{
                if (hyViewOutInPs != null) return hyViewOutInPs.ShowValue;
                else return 0;
            }
        }
        #endregion
        public void SetJgCd(string  c)
        {
            double temp = 0;
            if(double.TryParse(c,out temp))
            SetDValue(sizeLengthOutInPs,c);
        }
        #endregion
        public bool checkCutChang(double c)
        {
            if (DeviceId == Constant.sbjDeivceId)
                if ((YLCOutInPs.ShowValueFloat - c) <= cutSizeRangeChangOutInPs.ShowValue                                    
                    &&
                    YLCOutInPs.ShowValueFloat > 0                                  
                    &&
                    YLCOutInPs.ShowValueFloat - c > 0
                    )

                {
                    return true;
                }
            return false;
        }
        public bool checkCutKuan(double k)
        {
            if (DeviceId == Constant.sbjDeivceId)
                if (
                    (YLKOutInPs.ShowValueFloat - k) <= cutSizeRangeKuanOutInPs.ShowValue
                    &&
                   
                    YLKOutInPs.ShowValueFloat > 0
                    &&
                    YLKOutInPs.ShowValueFloat - k > 0                  
                    )

                {
                    return true;
                }
            return false;
        }
        //四边锯数据判断 
        public bool checkDataUseful(double c,double k)
        {
            if(DeviceId ==Constant.sbjDeivceId )
            if ((YLCOutInPs.ShowValueFloat - c) <= cutSizeRangeChangOutInPs.ShowValue
                &&
                (YLKOutInPs.ShowValueFloat - k) <= cutSizeRangeKuanOutInPs.ShowValue
                &&
                YLCOutInPs.ShowValueFloat>0
                &&
                YLKOutInPs.ShowValueFloat>0
                &&
                YLKOutInPs.ShowValueFloat - k >0
                &&
                YLCOutInPs.ShowValueFloat -c>0
                )

            {
                return true;
            }
            return false;
        }
        public EvokDTTcpWork(int id)
        {

            //初始化设备
            List<string> strDataFormPath = new List<string>();

            strDataFormPath.Add(Constant.PlcDataFilePathAuto);
            strDataFormPath.Add(Constant.PlcDataFilePathHand);
            strDataFormPath.Add(Constant.PlcDataFilePathParam);
            strDataFormPath.Add(Constant.PlcDataFilePathIO);

            if (File.Exists(Constant.PlcDataFilePathScar))
                strDataFormPath.Add(Constant.PlcDataFilePathScar);

            if (File.Exists(Constant.PlcDataFilePathParam1))
                strDataFormPath.Add(Constant.PlcDataFilePathParam1);

            for (int i = strDataFormPath.Count - 1; i >= 0; i--)
            {
                if (!File.Exists(strDataFormPath[i]))
                {

                    strDataFormPath.RemoveAt(i);
                    MessageBox.Show(strDataFormPath[i] + Constant.ErrorPlcFile);
                    Environment.Exit(0);
                }
            }

            DeviceId = id;

            evokDevice = new DTTcpDevice(strDataFormPath,DeviceId);
           
           
            InitUsual();          

        }
        public bool RestartDevice(int id)
        {

            evokDevice.RestartConneect(evokDevice.DataFormLst[id]);
            FindPlcSimpleInPlcInfoLst(id);
            return evokDevice.getDeviceData();

        }
        #region 运行部分
        public bool KeyPress_AutoPage(object sender, KeyPressEventArgs e)
        {
            return KeyPress_Page(sender, e, Constant.AutoPage);
        }
        //201901172122测试类似于信捷这种的数据测试 一个起始地址 然后单双字 地址 加上去 可以批量写数据 便于以后可以发送批量加工数据
        public bool SetMultiPleTest()//--通过
        {
            
            List<string> value = new List<string>();
            value.Add("200");
            value.Add("200");
            value.Add("200");
            value.Add("200");
            SetDValue("门宽",Constant.Write, AllPlcSimpleLst[0],value.ToArray());

            return true;
        }

        public bool KeyPress_Page(object sender, KeyPressEventArgs e,int id)
        {
            if (e.KeyChar == '\r')
            {
                SetDValue(((TextBox)sender).Tag.ToString(), Constant.Write, AllPlcSimpleLst[id], ((TextBox)sender).Text);
                return true;
            }
            return false;
        }
        public bool KeyPress_Param1Page(object sender, KeyPressEventArgs e)
        {
            return KeyPress_Page(sender, e, Constant.ScarPage);
            
        }
        public bool KeyPress_HandPage(object sender, KeyPressEventArgs e)
        {
            return KeyPress_Page(sender, e, Constant.HandPage);
           
        }
        public bool mouseDown(object sender, MouseEventArgs e,int id)
        {
          
                SetMPsOn(((Button)sender).Tag.ToString(), Constant.Write, AllPlcSimpleLst[id]);
                return true;
            
        }

        public DTPlcInfoSimple getDtPlcSimple(int id,string name)
        {
            if (id < AllPlcSimpleLst.Count)
                return ConstantMethod.getDtPlcSimple(name, AllPlcSimpleLst[id]);
            else return null;
        }
        public bool mouseUp(object sender, MouseEventArgs e, int id)
        {
        
                SetMPsOff(((Button)sender).Tag.ToString(), Constant.Write, AllPlcSimpleLst[id]);
                return true;

        }
        public void dgvParam_CellEndEdit(DataGridView dgvParam, object sender, DataGridViewCellEventArgs e)
        {


            string s = dgvParam.SelectedCells[0].Value.ToString();

            int rowIndex = dgvParam.SelectedCells[0].RowIndex;
            try
            {

                DgvValueEdit(rowIndex, s);
            }
            catch (Exception ex)
            { }
            finally { DgvInOutEdit(rowIndex, false); }
            /***
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

            ***/
        }
        public void ProClr()
        {
            string[] valurStr = { "0" };
            evokDevice.SetDValue(prodOutInPs, valurStr);
            optSize.prodClear();
        }
        //启动
        public void start(int id)
        {
            if (checkDataUseful(sizeLengthOutInPs.ShowValueFloat,sizeWidthOutInPs.ShowValueFloat))
            {
                evokDevice.SetMValueON2OFF(startOutPs);

                RunFlag = true;

                rtbWork.Clear();

                LogManager.WriteProgramLog(Constant.DeviceStartCut);


            }
            else
            {
                MessageBox.Show("加工数据错误！");
            }

        }
       
        public void bitOff2On(string str1, string str2, List<DTPlcInfoSimple> pLst)
        {
            DTPlcInfoSimple p = getPsFromPslLst(str1, str2, pLst);
            if (p != null)
            {
                evokDevice.SetMValueON2OFF(p);
            }
            else
            {
                MessageBox.Show(Constant.SetDataFail);
            }
        }

        public void oppositeBitClick(string str1, string str2, int id)
        {
            if (id < AllPlcSimpleLst.Count)
            {
                DTPlcInfoSimple p = getPsFromPslLst(str1, str2, AllPlcSimpleLst[id]);
                if (p != null)
                {
                    oppositeBit(p);
                }
                else
                {
                    MessageBox.Show(Constant.SetDataFail);
                }
            }

        }
        public void bitOnClick(string str1, string str2, int id)
        {
            if (id < AllPlcSimpleLst.Count)
            {
                DTPlcInfoSimple p = getPsFromPslLst(str1, str2, AllPlcSimpleLst[id]);
                if (p != null)
                {
                    bitOn(p);
                }
                else
                {
                    MessageBox.Show(Constant.SetDataFail);
                }
            }

        }
        private void bitOn(DTPlcInfoSimple p)
        {          
            evokDevice.SetMValueON(p);      
        }
        private void bitOff(DTPlcInfoSimple p)
        {
            evokDevice.SetMValueOFF(p);
        }
        public void oppositeBitClick(string str1, string str2, List<DTPlcInfoSimple> pLst)
        {
            DTPlcInfoSimple p = getPsFromPslLst(str1, str2, pLst);
            if (p != null)
            {
                oppositeBit(p);
            }
            else
            {
                MessageBox.Show(Constant.SetDataFail);
            }
            
        }
        //点位取反
        private void oppositeBit(DTPlcInfoSimple p)
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
        public void stop()
        {
            RunFlag = false;
            evokDevice.SetMValueOFF2ON(stopInPs);
            optSize.SingleSizeLst.Clear();
            optSize.ProdInfoLst.Clear();
            LogManager.WriteProgramLog(Constant.DeviceStop);
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
        public void pause()
        {
            evokDevice.SetMValueOFF2ON(pauseOutPs);
            LogManager.WriteProgramLog(Constant.DevicePause);
        }
        //自动上料
        public void autoSL()
        {
            evokDevice.SetMValueOFF2ON(autoSLOutPs);
        }

        #region 门锁机
 

        public void setScanParam(string width, string height,string thickness,int hymode ,string hycnt)
        {
       
            SetDValue(doorWidthInOutPs, width);
            ConstantMethod.Delay(100);

            SetDValue(doorHeightInOutPs, height);
  
            ConstantMethod.Delay(100);


            SetDValue(doorThicknessInOutPs, thickness);

            ConstantMethod.Delay(100);
            if (hymode > 0) bitOn(hyModeInOutPs);
            else bitOff(hyModeInOutPs);
            ConstantMethod.Delay(100);

            SetDValue(hyCountInOutPs, hycnt);

          //  MessageBox.Show("门宽"+ width+"门长"+ height+"门厚" + thickness +"合页数量"+ hycnt);



        }
        public void openProgram(int id,int hy)
        {
            if (!(id == scProgramOutPs.ShowValue))
            {
                string[] value = { id.ToString() };
                evokDevice.SetDValue(scProgramOutPs, value);
                ConstantMethod.Delay(100);
            }
            if (!(hy == hyProgramOutPs.ShowValue))
            {
                string[] value = { hy.ToString() };
                evokDevice.SetDValue(hyProgramOutPs, value);
                ConstantMethod.Delay(100);
            }
            evokDevice.SetMValueON2OFF(openProgramOutPs);
        }
        public void openProgramBarCode(int id, int hy)
        {
            if (!(id == scProgramOutPs.ShowValue))
            {
                string[] value = { id.ToString() };
                evokDevice.SetDValue(scProgramOutPs, value);
                ConstantMethod.Delay(100);
            }
            if (!(hy == hyProgramOutPs.ShowValue))
            {
                string[] value = { hy.ToString() };
                evokDevice.SetDValue(hyProgramOutPs, value);
                ConstantMethod.Delay(100);
            }
            evokDevice.SetMValueON2OFF(openProgramBarCodeOutPs);
        }
        public void openProgram(int id)
        {
            string[] value = { id.ToString() };
            evokDevice.SetDValue(selectProgramOutPs,value);
            ConstantMethod.Delay(100);
            evokDevice.SetMValueON2OFF(openProgramOutPs);
        }
        //多个轴复位
       
        public void selectKnife(int value)
        {
            List<string> valuestr = new List<string>();
            valuestr.Add(value.ToString());
            evokDevice.SetDValue(knifeSelectInOutPs, valuestr.ToArray());
        }
        public void selectSc(int value)
        {
            List<string> valuestr = new List<string>();
            valuestr.Add(value.ToString());
            evokDevice.SetDValue(scProgramOutPs, valuestr.ToArray());
            ConstantMethod.Delay(100);
            evokDevice.SetMValueON2OFF(scSureProgramOutPs);
        }
        public void selectHy(int value)
        {
            List<string> valuestr = new List<string>();
            valuestr.Add(value.ToString());
            evokDevice.SetDValue(hyProgramOutPs, valuestr.ToArray());
            ConstantMethod.Delay(100);
            evokDevice.SetMValueON2OFF(hySureProgramOutPs);
        }
        //工位双击后 要给下面发数据
        public void selectGw(int value)
        {
            List<string> valuestr = new List<string>();
            valuestr.Add(value.ToString());
            evokDevice.SetDValue(gwShiftInOutPs, valuestr.ToArray());
            ConstantMethod.Delay(100);
        }
        public void selectProgramNo(int value)
        {
            List<string> valuestr = new List<string>();
            valuestr.Add(value.ToString());
            evokDevice.SetDValue(programNoShiftInOutPs, valuestr.ToArray());
        }
        #endregion
        //复位
        public void reset()
        {
            //stop();
            evokDevice.SetMValueON2OFF(resetOutPs);
            LogManager.WriteProgramLog(Constant.DeviceReset);
        }

        #endregion
        public void SaveFile()
        {
            if(this !=null && optSize !=null)
            optSize.SaveCsv();
            optSize.SaveExcel();
        }

        #region 条码部分

        public void printBarcode(Report rp1, object s2, int show)
        {
            string[] s1 = (string[])s2;
            if (s1 != null && printReport != null)
            {
                try
                {
                    //在遇到结巴的情况下 保存下当前打印模式
                    //OldPrintBarCodeMode = PrintBarCodeMode;         

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
        public void printBarcode(Report rp1, object s2)
        {

            string[] s1 = (string[])s2;
            if (s1 != null && printReport != null && IsPrintBarCode)
            {
                Application.DoEvents();
                if (rp1.FindObject("barcode1") != null)
                    (rp1.FindObject("barcode1") as BarcodeObject).Text = s1[0];

                if (rp1.FindObject("Text1") != null)
                    (rp1.FindObject("Text1") as TextObject).Text = s1[1];

                if (rp1.FindObject("Text2") != null)
                    (rp1.FindObject("Text2") as TextObject).Text = s1[2];

                if (rp1.FindObject("Text3") != null)
                    (rp1.FindObject("Text3") as TextObject).Text = s1[3];

                if (rp1.FindObject("Text4") != null)
                    (rp1.FindObject("Text4") as TextObject).Text = s1[4];

                if (rp1.FindObject("Text5") != null)
                    (rp1.FindObject("Text5") as TextObject).Text = s1[5];

                if (rp1.FindObject("Text6") != null)
                    (rp1.FindObject("Text6") as TextObject).Text = s1[6];

                if (rp1.FindObject("Text7") != null)
                    (rp1.FindObject("Text7") as TextObject).Text = s1[7];

                if (rp1.FindObject("Text8") != null)
                    (rp1.FindObject("Text8") as TextObject).Text = s1[8];

                if (rp1.FindObject("Text9") != null)
                    (rp1.FindObject("Text9") as TextObject).Text = s1[9];
                if (rp1.FindObject("Text10") != null)
                    (rp1.FindObject("Text10") as TextObject).Text = s1[10];
                if (rp1.FindObject("Text11") != null)
                    (rp1.FindObject("Text11") as TextObject).Text = s1[11];
                if (rp1.FindObject("Text12") != null)
                    (rp1.FindObject("Text12") as TextObject).Text = s1[12];
                if (rp1.FindObject("Text13") != null)
                    (rp1.FindObject("Text13") as TextObject).Text = s1[13];
                if (rp1.FindObject("Text14") != null)
                    (rp1.FindObject("Text14") as TextObject).Text = s1[14];
                if (rp1.FindObject("Text15") != null)
                    (rp1.FindObject("Text15") as TextObject).Text = s1[15];
                if (rp1.FindObject("Text16") != null)
                    (rp1.FindObject("Text16") as TextObject).Text = s1[16];
                if (rp1.FindObject("Text17") != null)
                    (rp1.FindObject("Text17") as TextObject).Text = s1[17];
                if (rp1.FindObject("Text18") != null)
                    (rp1.FindObject("Text18") as TextObject).Text = s1[18];

                rp1.Prepare();
                rp1.PrintSettings.ShowDialog = false;
                rp1.Print();
            }
        }

        public void btnDown(string str1,string str2, List<DTPlcInfoSimple> pLst)
        {
            DTPlcInfoSimple p = getPsFromPslLst(str1, str2, pLst);
            if (p != null)
            {
                evokDevice.SetMValueON(p);
            }
            else
            {
                MessageBox.Show(Constant.SetDataFail);
            }
        }

        public void btnUp(string str1, string str2, List<DTPlcInfoSimple> pLst)
        {
            DTPlcInfoSimple p = getPsFromPslLst(str1, str2, pLst);
            if (p != null)
            {
                evokDevice.SetMValueOFF(p);
            }
            else
            {
                MessageBox.Show(Constant.SetDataFail);
            }
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
        public void CutRotateWithHoleThread()
        {
            //从哪一根开始切 暂定 从第一根 开始
            int CutProCnt = 0;

            if (optSize.ProdInfoLst.Count > 0)
            {
                for (int i = CutProCnt; i < optSize.ProdInfoLst.Count; i++)
                {
                    ConstantMethod.ShowInfo(rtbWork, "第" + (i + 1).ToString() + "根木料开始切割");

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
        private void DownLoadDataWithHoleAngle(int i)
        {
            List<int> DataList = new List<int>();
            List<ProdInfo> prod = optSize.ProdInfoLst;


            //先提取孔参数  当前这根料的数据 提取孔参数 角度参数 角度没有默认为90度          
            for (int n = 0; n < optSize.SingleSizeLst[i].Count; n++)
            {

                SingleSizeWithHoleAngle p = new SingleSizeWithHoleAngle(
                    optSize.SingleSizeLst[i][n].DtUser, optSize.SingleSizeLst[i][n].Xuhao
                    );

                p = ConstantMethod.Mapper<SingleSizeWithHoleAngle, SingleSize>(optSize.SingleSizeLst[i][n]);

                optSize.ProdInfoLst[i].hole.Add(p.Hole);
                optSize.ProdInfoLst[i].angle.Add(p.Angle);
            }

            for (int m = 0; m < 6; m++)
            {
                #region 带孔的参数下发

                //段数为起始地址 ：数据格式 D3000段数	D3002段长	D3004是否打印	前角度	孔数	孔位置	边长	深度
                DataList.Add(optSize.ProdInfoLst[i].Cut.Count);  //段数
                                                                 //保存下地址
                int ldsCountInOutPsAddr = ldsCountInOutPs.Addr;
                #region 开始下发孔和角度 尺寸等数据
                if (prod[i].hole.Count > 0 && prod[i].angle.Count > 0)
                    for (int sizeid = 0; sizeid < prod[i].Cut.Count; sizeid++)
                    {
                        DataList.Add(prod[i].Cut[sizeid]);  //段长
                        DataList.Add(1);  //条码打印标志
                        int holecount0 = 0;
                        //总共10个孔 取前面 5个
                        //前角度
                        for (int holecount = 0; holecount < prod[i].hole[sizeid].Count() / 2; holecount = holecount + 3)
                        {
                            if (prod[i].hole[sizeid][holecount] > 0)
                                holecount0++;
                        }
                        DataList.Add(prod[i].angle[sizeid][0]);
                        DataList.Add(holecount0);

                        for (int addhole = 0; addhole < holecount0 * 3; addhole++)
                        {
                            DataList.Add(prod[i].hole[sizeid][addhole]);
                        }
                        //后角度
                        int holecount1 = 0;
                        for (int holecount = 15; holecount < prod[i].hole[sizeid].Count(); holecount = holecount + 3)
                        {
                            if (prod[i].hole[sizeid][holecount] > 0)
                                holecount1++;
                        }

                        DataList.Add(prod[i].angle[sizeid][1]);
                        DataList.Add(holecount1);

                        for (int addhole = 15; addhole < 15 + holecount1 * 3; addhole++)
                        {
                            DataList.Add(prod[i].hole[sizeid][addhole]);

                        }                                         
                        //evokDevice.SetMultiPleDValue(ldsCountInOutPs, DataList.ToArray());
                                              
                        LogManager.WriteProgramLog(Constant.DataDownLoad + ldsCountInOutPs.Addr.ToString());                       

                        DataList.Clear();

                        //地址偏移 按照约定的表格协议来
                        if (sizeid == 0)
                        {
                            ldsCountInOutPs.Addr += 134;
                        }
                        else
                            ldsCountInOutPs.Addr += 132;

                    }

                //恢复地址
                ldsCountInOutPs.Addr = ldsCountInOutPsAddr;
                //检验一下第一组数据就得了 因为其他地址在变 根本没法读取
                if (ldsCountInOutPs.ShowValue == prod[i].Cut.Count)
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
                    //MessageBox.Show(Constant.PlcReadDataError);
                    LogManager.WriteProgramLog(Constant.PlcReadDataError);
                    RunFlag = false;
                    ConstantMethod.AppExit();
                    return;
                } 

           
        }
        private void DownLoadDataNormal(int i)
        {
            List<int> DataList = new List<int>();
            //添加料长
            DataList.Add(optSize.ProdInfoLst[i].Len);
            //D4998-》0
            int value = 1;
            DataList.Add(value);
            DataList.Add(optSize.ProdInfoLst[i].WL);
            //添加段数
            DataList.Add(optSize.ProdInfoLst[i].Cut.Count);

            DataList.AddRange(optSize.ProdInfoLst[i].Cut);

            
            //数据下发 确保正确 下位机需要给一个M16 高电平 我这边来置OFF
            //发数据三次 M16 如果还没有给高电平
            bool plcgetData = false;

            for (int m = 0; m < 6; m++)
            {              
                // 料段数为0 下发
                if (ldsCountInOutPs.ShowValue == 0)
                {
                    LogManager.WriteProgramLog(Constant.DataDownLoad + m.ToString());
                    /***
                    if (evokDevice.SetMultiPleDValue(lcOutInPs, DataList.ToArray()))
                    {
                         //发送是料长 但料长不清零 要读取清零的D5000数据 所以只能加延时
                        ConstantMethod.Delay(200);
                        //料段数大于0  代表写成功了 
                        if (ldsCountInOutPs.ShowValue > 0)
                        {
                            //然后 设置M16 为高 写成功了 就退出来
                            if (evokDevice.SetMValueON(startCountInOutPs)) break;
                        }
                    }
                    ****/
                 }
            }

            //数据下发完成 等待数据接收 M16 为OFF
            int valueWriteOk = 0;
            //使用下测长的延时函数 起始和测长差不多的 就是数据下发 等机器确认
            ConstantMethod.DelayMeasure(Constant.PlcCountTimeOut,
                     ref valueWriteOk,
                     ref startCountInOutPs,
                     ref emgStopInPs, ref mRunFlag);

            if (startCountInOutPs.ShowValue != valueWriteOk)
            {
                plcgetData = true;
                //MessageBox.Show(Constant.PlcReadDataError);
                LogManager.WriteProgramLog(Constant.PlcReadDataError);
                RunFlag = false;
                ConstantMethod.AppExit();
                return;
                               
            }

        }
        private void CutLoop(int i)
        {
            //打第一条条码
            if (optSize.SingleSizeLst[i].Count > 0)
                printBarcode(printReport, optSize.SingleSizeLst[i][0].ParamStrLst.ToArray());

            int oldcCount = 0;//保存的老计数值

            while (RunFlag)
            {
                Application.DoEvents();

                Thread.Sleep(10);
                int newCount = cutDoneOutInPs.ShowValue;

                //这里整理成函数
                if ((!RunFlag || IsInEmg))
                {
                    ConstantMethod.ShowInfo(rtbWork, Constant.emgStopTip);
                    LogManager.WriteProgramLog(Constant.emgStopTip);
                    stop();
                    return;
                }

                if (newCount != oldcCount && oldcCount < optSize.ProdInfoLst[i].Cut.Count)
                {
                    int oldCutCount = 0;

                    if (int.TryParse(optSize.SingleSizeLst[i][oldcCount].DtUser.Rows[optSize.SingleSizeLst[i][oldcCount].Xuhao]["已切数量"].ToString(), out oldCutCount))
                    {
                        oldCutCount++;
                        optSize.SingleSizeLst[i][oldcCount].DtUser.Rows[optSize.SingleSizeLst[i][oldcCount].Xuhao]["已切数量"] = oldCutCount;
                    }

                    ConstantMethod.ShowInfo(rtbWork, "第" + (oldcCount + 1).ToString() + "段尺寸：" + optSize.ProdInfoLst[i].Cut[oldcCount].ToString() + "-----完成");

                    oldcCount = newCount;
                    if (newCount < optSize.SingleSizeLst[i].Count)
                    {
                        //打条码
                        printBarcode(printReport, optSize.SingleSizeLst[i][newCount].ParamStrLst.ToArray());
                    }
                }
                if (newCount >= optSize.ProdInfoLst[i].Cut.Count) break;
            }
        }
        private void  CountClr()
        {
            string[] valueStr = {"0"};
            evokDevice.SetDValue(cutDoneOutInPs, valueStr);
        }
        /// <summary>
        /// 正常测长切割
        /// </summary>
        private void CutWorkThread()
        {
            //从哪一根开始切 暂定 从第一根 开始
            int CutProCnt = 0;          

            if (optSize.ProdInfoLst.Count > 0)
            {             
                for (int i = CutProCnt; i < optSize.ProdInfoLst.Count; i++)
                {
                    ConstantMethod.ShowInfo(rtbWork, "第" + (i + 1).ToString() + "根木料开始切割");
                    
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
            optSize.Len = lcOutInPs.ShowValue;
            optSize.Dbc = dbcOutInPs.ShowValue;
            optSize.Ltbc = ltbcOutInPs.ShowValue;
            optSize.Safe = safeOutInPs.ShowValue;
            return optSize.LoadCsvData(filename);
        }
        public bool LoadExcelData(string filename)
        {
            optSize.Len = lcOutInPs.ShowValue;
            optSize.Dbc = dbcOutInPs.ShowValue;
            optSize.Ltbc = ltbcOutInPs.ShowValue;
            optSize.Safe = safeOutInPs.ShowValue;
            return optSize.LoadExcelData(filename);
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

                        if (!CutThread.IsAlive)
                            CutThread.Start();

                       
                        

                        break;
                    }
                case Constant.CutMeasureMode:
                    {
                        if (CutThreadStart == null)
                            CutThreadStart = new ThreadStart(CutWorkThread);
                        //初始化Thread的新实例，并通过构造方法将委托ts做为参数赋初始值。
                        if (CutThread == null)
                            CutThread = new Thread(CutThreadStart);   //需要引入System.Threading命名空间

                        if (!CutThread.IsAlive)
                            CutThread.Start();
                       
                       
                        break;
                    }
                case Constant.CutMeasureRotateWithHoleMode:
                    {

                        if (CutThreadStart == null)
                            CutThreadStart = new ThreadStart(CutRotateWithHoleThread);
                        //初始化Thread的新实例，并通过构造方法将委托ts做为参数赋初始值。
                        if (CutThread == null)
                            CutThread = new Thread(CutThreadStart);   //需要引入System.Threading命名空间

                        if (!CutThread.IsAlive)
                            CutThread.Start();                       
                       
                        break;
                    }
                case Constant.CutNormalWithHoleMode:
                    {

                        if (CutThreadStart == null)
                            CutThreadStart = new ThreadStart(CutRotateWithHoleThread);
                        //初始化Thread的新实例，并通过构造方法将委托ts做为参数赋初始值。
                        if (CutThread == null)
                            CutThread = new Thread(CutThreadStart);   //需要引入System.Threading命名空间

                        if (!CutThread.IsAlive)
                            CutThread.Start();                 
                       
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }
        public void CutStartMeasure(int cutid)
        {
         
            if (IsInEmg)
            {
                MessageBox.Show(Constant.emgStopTip);
                return;
            }

            LogManager.WriteProgramLog(Constant.AutoMeasureMode);

            //启动
            start(cutid);

            //等待 测量
            while (mRunFlag)
            {
                                
                int valueOld = 1;
                LogManager.WriteProgramLog(Constant.MeasureSt);
                ConstantMethod.DelayMeasure(Constant.MeaSureMaxTime, ref valueOld, ref autoCCInPs,ref emgStopInPs,ref mRunFlag);
               
                if (IsInEmg)
                {
                    stop();
                }
                LogManager.WriteProgramLog(Constant.MeasureEd);
                if (autoCCInPs.ShowValue ==Constant.M_ON)
                {
                    evokDevice.SetMValueOFF(autoCCInPs);

                    optSize.Len = lcOutInPs.ShowValue;
                    //开始优化 
                    optSize.OptMeasure(rtbResult);                   
                    if (optSize.ProdInfoLst.Count < 1)
                    {
                        break;
                    }                 
                }
                else
                {
                    MessageBox.Show(Constant.measureOutOfTime);
                    return;
                }

                try
                {

                    SelectCutThread(cutid);

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
                ConstantMethod.ShowInfo(rtbWork,Constant.NextOpt);
            }

            stop();
           //测试先隐藏
          // MessageBox.Show(Constant.CutEnd);
        }
        public void CutStartNormal(int cutid)
        {

            if (IsInEmg)
            {
                MessageBox.Show(Constant.emgStopTip);
                return;
            }
            //正常模式需要优化
            if (optSize.ProdInfoLst.Count < 1)
            {
                MessageBox.Show(Constant.noData);
                return;
            }

            LogManager.WriteProgramLog(Constant.NormalMode);

            start(cutid);
                                   
            try
            {

                SelectCutThread(cutid);
                while (CutThread.IsAlive)
                {
                    Application.DoEvents();
                }
            }
            finally
            {
                CutThread = null;
                CutThreadStart = null;            
                stop();
               // MessageBox.Show(Constant.CutEnd);
            }
        }
        //自动测长开
        public void autoMesON()
        {
            evokDevice.SetMValueOFF(autoMesOutInPs);
        }
        public void angleModeChoose()
        {
            oppositeBit(angleModeOutInPs);
        }
        public void autoMesOFF()
        {
            evokDevice.SetMValueON(autoMesOutInPs);
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
            printReport.Dispose();
            if (CutThread != null && CutThread.IsAlive)
            {
                CutThread.Join();
            }

            LogManager.WriteProgramLog(Constant.Quit);

        }
        public void InitDgvParam(DataGridView dgvParam)
        {
            if (evokDevice.DataFormLst.Count > 2)
            {
                dgvParam.AutoGenerateColumns = false;
                dgvParam.DataSource = evokDevice.DataFormLst[2];
                dgvParam.Columns[0].DataPropertyName = evokDevice.DataFormLst[2].Columns["bin"].ToString();
                dgvParam.Columns[1].DataPropertyName = evokDevice.DataFormLst[2].Columns["param6"].ToString();
            }
        }
      
        public void buildCmd(int pageid)
        {
            evokDevice.shiftDataForm(pageid);

            FindPlcSimpleInPlcInfoLst(pageid);

            ConstantMethod.Delay(50);

        }
        
        public bool ShiftPage(int pageid)
        {
            if (evokDevice.Status == Constant.DeviceConnected)
            {
                //页面切换需要告诉下位机
                if (pageid == Constant.AutoPage)
                {
                    //切换
                    ShiftShowPage(pageid+1);                  
                    evokDevice.SetMValueON(autoModeOutInPs);
                                                                           
                }
                if (pageid == Constant.HandPage)
                {
                    ShiftShowPage(pageid + 1);
                    evokDevice.SetMValueON(handModeOutInPs);               
                }

                if (pageid == Constant.ParamPage)
                {
                    if (!ConstantMethod.UserPassWd())
                    {
                        return false;
                    }                  
                    ShiftShowPage(5);
                }
                if (pageid == Constant.IOPage)
                {
                    ShiftShowPage(6);
                }
                if (pageid == Constant.ScarPage)
                {
                    if (!ConstantMethod.UserPassWd())
                    {
                        return false;
                    }                 
                   ShiftShowPage(10);
                }
                evokDevice.shiftDataForm(pageid);
                                                                            
                FindPlcSimpleInPlcInfoLst(pageid);

                ConstantMethod.Delay(100);

                return true;
            }
          
            
           return false;       

        }

        public bool shiftDataFormSplit(int formid, int rowSt, int count)
        {
            evokDevice.shiftDataFormSplit(formid, rowSt, count);
            return true;
        }

        #region 寄存器操作部分

        public void ClearError()
        {
            string[] valurStr = {Constant.errClrValue };
            evokDevice.SetDValue(errorResetOutPs, valurStr);
            
        }
        private DTPlcInfoSimple getPsFromPslLst(string tag0, string str0, List<DTPlcInfoSimple> pslLst)
        {
            foreach (DTPlcInfoSimple simple in pslLst)
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
        public void SetInEdit(string str1, string str2, int id)
        {
            DTPlcInfoSimple p = getPsFromPslLst(str1, str2, AllPlcSimpleLst[id]);
            if (p != null)
            {
                p.IsInEdit = true;
            }
            else
            {
                MessageBox.Show(Constant.SetDataFail);
            }
        }
        public void SetOutEdit(string str1, string str2, int id)
        {
            DTPlcInfoSimple p = getPsFromPslLst(str1, str2, AllPlcSimpleLst[id]);
            if (p != null)
            {
                p.IsInEdit = false;
            }
            else
            {
                MessageBox.Show(Constant.SetDataFail);
            }
        }
        public void SetMPsONToOFF(string str1, string str2, int id)
        {
            if (id >= AllPlcSimpleLst.Count) return;
            DTPlcInfoSimple p = getPsFromPslLst(str1, str2, AllPlcSimpleLst[id]);
            if (p != null)
            {
                evokDevice.SetMValueON2OFF(p);
            }
            else
            {
                MessageBox.Show(Constant.SetDataFail);
            }
        }
        public void SetMPsONToOFF(string str1, string str2, List<DTPlcInfoSimple> pLst)
        {

            DTPlcInfoSimple p = getPsFromPslLst(str1, str2, pLst);
            if (p != null)
            {
                evokDevice.SetMValueON2OFF(p);
            }
            else
            {
                MessageBox.Show(Constant.SetDataFail);
            }
        }
        public void  SetMPsOFFToOn(string str1,string str2 ,List<DTPlcInfoSimple> pLst)
        {

            DTPlcInfoSimple p = getPsFromPslLst(str1,str2, pLst);
            if (p != null)
            {
                evokDevice.SetMValueOFF2ON(p);
            }
            else
            {
                MessageBox.Show(Constant.SetDataFail);
            }
        }
        public void SetMPsOn(string str1, string str2, List<DTPlcInfoSimple> pLst)
        {
            DTPlcInfoSimple p = getPsFromPslLst(str1, str2, pLst);
            if (p != null)
            {
                evokDevice.SetMValueON(p);
            }
            else
            {
                MessageBox.Show(Constant.SetDataFail);
            }
        }

        public void SetInEdit(string str1, string str2, List<DTPlcInfoSimple> pLst)
        {
            DTPlcInfoSimple p = getPsFromPslLst(str1, str2, pLst);
            if (p != null)
            {
                p.IsInEdit = true;
            }
            else
            {
                MessageBox.Show(Constant.SetDataFail);
            }
        }
        public void SetOutEdit(string str1, string str2, List<DTPlcInfoSimple> pLst)
        {
            DTPlcInfoSimple p = getPsFromPslLst(str1, str2, pLst);
            if (p != null)
            {
                p.IsInEdit = false;
            }
            else
            {
                MessageBox.Show(Constant.SetDataFail);
            }
        }

        public void SetDValue(string str1, string str2, List<DTPlcInfoSimple> pLst, string[] num)
        {
            DTPlcInfoSimple p = getPsFromPslLst(str1, str2, pLst);
            if (p != null)
            {
                evokDevice.SetDValue(p, num);
            }
            else
            {
                MessageBox.Show(Constant.SetDataFail);
            }

        }
        public void SetDValue(DTPlcInfoSimple p, string num)
        {
            List<string> valueStr = new List<string>();

            double valueShift = 0;
            if (double.TryParse(num, out valueShift))
            {

                if (!(valueShift <= p.MaxValue && valueShift >= p.MinValue))
                {
                    MessageBox.Show(Constant.dataOutOfRange + p.MinValue.ToString() + "--" + p.MaxValue.ToString());
                    return;
                }
                valueShift = valueShift * p.Ration;
                valueStr.Add(valueShift.ToString());
            }
            if (p != null)
            {
                evokDevice.SetDValue(p, valueStr.ToArray());
            }
            else
            {
                MessageBox.Show(Constant.SetDataFail);
            }
        }
        //这里输入数据要筛选下 是否大于最大值
        //不知道数据类型 就传字节
        public void SetDValue(string str1, string str2, List<DTPlcInfoSimple> pLst, string num)
        {
            DTPlcInfoSimple p = getPsFromPslLst(str1, str2, pLst);
            List<string> valueStr = new List<string>();
            
            double valueShift = 0;
            if (double.TryParse(num, out valueShift))
            {
                
                if (!(valueShift <= p.MaxValue && valueShift >= p.MinValue))
                {
                    MessageBox.Show(Constant.dataOutOfRange+p.MinValue.ToString()+"--"+p.MaxValue.ToString());
                    return;
                }
                valueShift = valueShift * p.Ration;
                valueStr.Add(valueShift.ToString());
            }
            if (p != null)
            {               
                evokDevice.SetDValue(p, valueStr.ToArray());
            }
            else
            {
                MessageBox.Show(Constant.SetDataFail);
            }

        }
        
        public void SetMPsOff(string str1, string str2, List<DTPlcInfoSimple> pLst)
        {
            DTPlcInfoSimple p = getPsFromPslLst(str1, str2, pLst);
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
               // evokDevice.WriteSingleDData(addr, num3, area, mode);
            }
        }
        public void DgvValueEdit(int rowIndex, string num3)
        {

            string userdata = evokDevice.DataForm.Rows[rowIndex]["addr"].ToString();
            int addr = 0;
            string area = "D";
            string mode = evokDevice.DataForm.Rows[rowIndex]["mode"].ToString();

            string name = evokDevice.DataForm.Rows[rowIndex]["bin"].ToString();
            //ConstantMethod.getAddrAndAreaByStr(userdata,ref addr,ref area);

            DTPlcInfoSimple dp = ConstantMethod.getDtPlcSimple(name,PsLstParam);

            dp.Mode = mode;
            dp.BelongToDataform = evokDevice.DataForm;
            List<string> value = new List<string>();

            double valueDouble = 0;

            if (double.TryParse(num3,out valueDouble))
            {

                if (valueDouble <= dp.MaxValue && valueDouble >= dp.MinValue)
                {

                    valueDouble = valueDouble * dp.Ration;
                    value.Add(valueDouble.ToString());
                    evokDevice.SetDValue(dp, value.ToArray());
                }
            }
                      
            

        }
        public void InitDgvIO(DataGridView dgvIO)
        {
            if (evokDevice.DataFormLst.Count > 3)
            {
                dgvIO.AutoGenerateColumns = false;
                dgvIO.DataSource = evokDevice.DataFormLst[3];
                dgvIO.Columns["bin0"].DataPropertyName = evokDevice.DataFormLst[2].Columns["bin"].ToString();
                dgvIO.Columns["value0"].DataPropertyName = evokDevice.DataFormLst[2].Columns["value"].ToString();
                dgvIO.ReadOnly = true;
            }
        }
        public void DgvInOutEdit(int rowIndex,bool editEnable)
        {
            string s = evokDevice.DataForm.Rows[rowIndex]["param1"].ToString();         
            string userdata = evokDevice.DataForm.Rows[rowIndex]["addr"].ToString();
            string area = "D";
            int addr = 0;
            ConstantMethod.getAddrAndAreaByStr(userdata,ref addr,ref area,DeviceId);          
            int result = 0;

            if (DeviceId == Constant.xzjDeivceId)
            {
                if (int.TryParse(s, out result))
                {
                    if (DTTcpCmdPackAndDataUnpack.GetIntAreaFromStr(area) < Constant.HSD_ID)
                    {
                        evokDevice.DPlcInfo[result].IsInEdit = editEnable;
                    }
                    else
                    {
                        evokDevice.MPlcInfo[result].IsInEdit = editEnable;
                    }
                }
            }
            else 
            if (int.TryParse(s, out result))
            {
                if (DTTcpCmdPackAndDataUnpack.GetIntAreaFromStr(area)>Constant.MXAddrId)
                {
                    evokDevice.DPlcInfo[result].IsInEdit = editEnable;
                }
                else
                {
                    evokDevice.MPlcInfo[result].IsInEdit = editEnable;
                }
            }
        }
        #endregion;

        #region 缓冲区中有个plcinfo类 存储了 PLC 的实时数据 DTPlcInfoSimple 则是用户进行对接的操作对象 两者进行连接
        /// <summary>
        /// plcsimple 与缓冲区中的类绑定 便于后续读取值 缓冲区的类 实时更新数据 
        /// plcsimpele 进行与用户的操作绑定
        /// </summary>
        /// <param name="m"></param>
        private void FindPlcSimpleInPlcInfoLst(int m)
        {
            List<DTPlcInfoSimple> pLst = AllPlcSimpleLst[m];
          //  foreach (List<DTPlcInfoSimple> pLst in AllPlcSimpleLst)
          //  {
                foreach (DTPlcInfoSimple p in pLst)
                {
                    FindPlcInfo(p, evokDevice.DPlcInfo, evokDevice.MPlcInfo);
                }
           // }
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
        private void FindPlcInfo(DTPlcInfoSimple p, List<DTTcpPlcInfo> dplc, List<DTTcpPlcInfo> mplc)
        {
            if (p.Area == null) return;
            if(dplc.Count>0)
            foreach (DTTcpPlcInfo p0 in dplc)
            {
                if ((p0.RelAddr == p.Addr) && (p0.StrArea.Equals(p.Area.Trim())))
                {
                    p.SetPlcInfo(p0);
                    return;
                }
            }
            if (mplc.Count > 0)
                for (int j = 0; j < mplc.Count; j++)
                {
                    
                    if ((mplc[j].RelAddr == p.Addr) && (mplc[j].StrArea.Equals(p.Area.Trim())))
                    {
                        p.SetPlcInfo(mplc[j]);
                        return;
                    }

                }
            //}
        }
        private void FindPlcInfo(DTPlcInfoSimple p, List<DTTcpPlcInfo> dplc, List<List<DTTcpPlcInfo>> mplc)
        {
            if (p.Area == null) return;
            if (dplc == null || 
                mplc == null || 
                dplc.Count == 0 || 
                mplc.Count == 0  
                ) return;
            foreach (DTTcpPlcInfo p0 in dplc)
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
}
