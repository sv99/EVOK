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
    public class EvokDTWork
    {
        //用户排版数据
        DataTable userDataTable;

        OptSize optSize;
        //设备类
        EvokDTDevice evokDevice;

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
        //
        List<List<DTPlcInfoSimple>> AllPlcSimpleLst;
        public DataTable UserDataTable
        {
            get { return userDataTable; }
            set { userDataTable = value; }
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
        public void SetEvokDevice(EvokDTDevice evokDevice0)
        {
            evokDevice = evokDevice0;
            if (evokDevice.DataFormLst.Count > 1 && evokDevice.DataFormLst[1].Rows.Count > 0)
            {
                psLstHand.Clear();
                foreach (DataRow dr in evokDevice.DataFormLst[1].Rows)
                {
                    if (dr == null) continue;
                    string name = dr["bin"].ToString();
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        DTPlcInfoSimple p = new DTPlcInfoSimple(name);
                        psLstHand.Add(p);
                    }
                }
            }
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
        public void SetPrintReport(FastReport.Report r1)
        {
            if (r1 != null)
                printReport = r1;
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
        #region 自动
        //自动页面
        List<DTPlcInfoSimple> psLstAuto;
        public List<xjplc.DTPlcInfoSimple> PsLstAuto
        {
            get { return psLstAuto; }
            set { psLstAuto = value; }
        }
        //定义后要加入集合  //忽略寄存器的影响直接匹配参数名     
        
        public DTPlcInfoSimple autoMesOutInPs = new DTPlcInfoSimple("自动测长标志读写");
        public DTPlcInfoSimple dbcOutInPs     = new DTPlcInfoSimple("刀补偿读写");
        public DTPlcInfoSimple ltbcOutInPs    = new DTPlcInfoSimple("料头补偿读写");
        public DTPlcInfoSimple safeOutInPs    = new DTPlcInfoSimple("安全距离读写");
        public DTPlcInfoSimple prodOutInPs    = new DTPlcInfoSimple("总产量读写"); 
        public DTPlcInfoSimple lcOutInPs      = new DTPlcInfoSimple("料长读写");
        public DTPlcInfoSimple stopOutInPs    = new DTPlcInfoSimple("停止读写");
        public DTPlcInfoSimple cutDoneOutInPs = new DTPlcInfoSimple("切割完毕读写");
        public DTPlcInfoSimple plcHandlebarCodeOutInPs = new DTPlcInfoSimple("条码打印读写");
        public DTPlcInfoSimple startCountInOutPs = new DTPlcInfoSimple("开始计数读写");
        public DTPlcInfoSimple ldsCountInOutPs = new DTPlcInfoSimple("料段数读写");

        public DTPlcInfoSimple pauseOutPs     = new DTPlcInfoSimple("暂停写");
        public DTPlcInfoSimple startOutPs     = new DTPlcInfoSimple("启动写");             
        public DTPlcInfoSimple resetOutPs     = new DTPlcInfoSimple("复位写");
        public DTPlcInfoSimple autoSLOutPs    = new DTPlcInfoSimple("自动上料写");
        public DTPlcInfoSimple pageShiftOutPs = new DTPlcInfoSimple("页面切换写");

        

        public DTPlcInfoSimple emgStopInPs    = new DTPlcInfoSimple("急停读");
        public DTPlcInfoSimple startInPs      = new DTPlcInfoSimple("启动读");
        public DTPlcInfoSimple resetInPs      = new DTPlcInfoSimple("复位读");
        public DTPlcInfoSimple pauseInPs      = new DTPlcInfoSimple("暂停读");
        public DTPlcInfoSimple autoSLInPs     = new DTPlcInfoSimple("自动上料读");
        public DTPlcInfoSimple autoCCInPs     = new DTPlcInfoSimple("自动测长读");
        public DTPlcInfoSimple clInPs         = new DTPlcInfoSimple("出料读");
        public DTPlcInfoSimple slInPs         = new DTPlcInfoSimple("送料读");
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


        #endregion
        #region 手动
        List<DTPlcInfoSimple> psLstHand;
        public DTPlcInfoSimple slzOutPs = new DTPlcInfoSimple("送料左写");
        public DTPlcInfoSimple slyOutPs = new DTPlcInfoSimple("送料右写");
        public DTPlcInfoSimple clzOutPs = new DTPlcInfoSimple("出料左写");
        public DTPlcInfoSimple clyOutPs = new DTPlcInfoSimple("出料右写");
        public DTPlcInfoSimple jlzOutPs = new DTPlcInfoSimple("锯料正写");
        public DTPlcInfoSimple jlfOutPs = new DTPlcInfoSimple("检测正写");
        public DTPlcInfoSimple jcfOutPs = new DTPlcInfoSimple("检测负写");
        public DTPlcInfoSimple sldjjOutPs = new DTPlcInfoSimple("上料电机写");
        public DTPlcInfoSimple sldjOutPs = new DTPlcInfoSimple("送料侧压写");
        public DTPlcInfoSimple qlqgOutPs = new DTPlcInfoSimple("切料气缸写");
        public DTPlcInfoSimple tmzkxffOutPs = new DTPlcInfoSimple("条码真空吸附阀写");
        public DTPlcInfoSimple qddjOutPs = new DTPlcInfoSimple("切刀电机写");
        public DTPlcInfoSimple qlcyzOutPs = new DTPlcInfoSimple("切料侧压左写");
        public DTPlcInfoSimple slksOutPs = new DTPlcInfoSimple("上料靠栅写");
        public DTPlcInfoSimple sljsjOutPs = new DTPlcInfoSimple("上料架升降写");
        public DTPlcInfoSimple qlylOutPs = new DTPlcInfoSimple("切料压料写");
        public DTPlcInfoSimple qlcyyOutPs = new DTPlcInfoSimple("切料侧压右写");
        public DTPlcInfoSimple sfslwOutPs = new DTPlcInfoSimple("伺服上料位写");
        public DTPlcInfoSimple tmccfOutPs = new DTPlcInfoSimple("条码吹尘阀写");
        public DTPlcInfoSimple cldjOutPs = new DTPlcInfoSimple("出料电机写");
        public DTPlcInfoSimple tmtgcqfOutPs = new DTPlcInfoSimple("条码铜管吹气阀写");
        public DTPlcInfoSimple cljlOutPs = new DTPlcInfoSimple("出料夹料写");
        public DTPlcInfoSimple tmxyqgOutPs = new DTPlcInfoSimple("条码下压气缸写");
        public DTPlcInfoSimple tmspjcqgOutPs = new DTPlcInfoSimple("条码水平进出气缸写");
        public DTPlcInfoSimple sljccOutPs = new DTPlcInfoSimple("上料架检测吹尘写");
        public DTPlcInfoSimple slsyOutPs = new DTPlcInfoSimple("送料上压写");

        public DTPlcInfoSimple slsyInPs = new DTPlcInfoSimple("送料上压读");
        public DTPlcInfoSimple sljccInPs = new DTPlcInfoSimple("上料架检测吹尘读");
        public DTPlcInfoSimple slInPs0 = new DTPlcInfoSimple("送料读");      
        public DTPlcInfoSimple clInPs0 = new DTPlcInfoSimple("出料读");   
        public DTPlcInfoSimple jlInPs = new DTPlcInfoSimple("锯料读");
        public DTPlcInfoSimple jcInPs = new DTPlcInfoSimple("检测读");     
        public DTPlcInfoSimple sldjjInPs = new DTPlcInfoSimple("上料电机读");
        public DTPlcInfoSimple sldjInPs = new DTPlcInfoSimple("送料侧压读");
        public DTPlcInfoSimple qlqgInPs = new DTPlcInfoSimple("切料气缸读");
        public DTPlcInfoSimple tmzkxffInPs = new DTPlcInfoSimple("条码真空吸附阀读");
        public DTPlcInfoSimple qddjInPs = new DTPlcInfoSimple("切刀电机读");
        public DTPlcInfoSimple qlcyzInPs = new DTPlcInfoSimple("切料侧压左读");
        public DTPlcInfoSimple slksInPs = new DTPlcInfoSimple("上料靠栅读");
        public DTPlcInfoSimple sljsjInPs = new DTPlcInfoSimple("上料架升降读");
        public DTPlcInfoSimple qlylInPs = new DTPlcInfoSimple("切料压料读");
        public DTPlcInfoSimple qlcyyInPs = new DTPlcInfoSimple("切料侧压右读");
        public DTPlcInfoSimple sfslwInPs = new DTPlcInfoSimple("伺服上料位读");
        public DTPlcInfoSimple tmccfInPs = new DTPlcInfoSimple("条码吹尘阀读");
        public DTPlcInfoSimple cldjInPs = new DTPlcInfoSimple("出料电机读");
        public DTPlcInfoSimple tmtgcqfInPs = new DTPlcInfoSimple("条码铜管吹气阀读");
        public DTPlcInfoSimple cljlInPs = new DTPlcInfoSimple("出料夹料读");
        public DTPlcInfoSimple tmxyqgInPs = new DTPlcInfoSimple("条码下压气缸读");
        public DTPlcInfoSimple tmspjcqgInPs = new DTPlcInfoSimple("条码水平进出气缸读");

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
            get { return psLstParam; }
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
        public EvokDTWork()
        {
            PsLstAuto = new List<DTPlcInfoSimple>();
            PsLstAuto.Add(autoMesOutInPs);
            PsLstAuto.Add(dbcOutInPs);
            PsLstAuto.Add(ltbcOutInPs);
            PsLstAuto.Add(safeOutInPs);
            PsLstAuto.Add(prodOutInPs);
            PsLstAuto.Add(lcOutInPs);
            PsLstAuto.Add(stopOutInPs);
            PsLstAuto.Add(cutDoneOutInPs);
            PsLstAuto.Add(plcHandlebarCodeOutInPs);
            PsLstAuto.Add(ldsCountInOutPs);
            
            PsLstAuto.Add(pauseOutPs);
            PsLstAuto.Add(startOutPs);
            PsLstAuto.Add(resetOutPs);
            PsLstAuto.Add(autoSLOutPs);
            PsLstAuto.Add(pageShiftOutPs);

            PsLstAuto.Add(emgStopInPs);
            PsLstAuto.Add(startInPs);
            PsLstAuto.Add(resetInPs);
            PsLstAuto.Add(pauseInPs);
            PsLstAuto.Add(autoSLInPs);
            PsLstAuto.Add(autoCCInPs);
            PsLstAuto.Add(clInPs);
            PsLstAuto.Add(slInPs);
            PsLstAuto.Add(alarm1InPs);
            PsLstAuto.Add(alarm2InPs);
            PsLstAuto.Add(alarm3InPs);
            PsLstAuto.Add(alarm4InPs);
            PsLstAuto.Add(alarm5InPs);
            PsLstAuto.Add(alarm6InPs);
            PsLstAuto.Add(alarm7InPs);
            PsLstAuto.Add(alarm8InPs);
            PsLstAuto.Add(alarm9InPs);
            PsLstAuto.Add(alarm10InPs);
            PsLstAuto.Add(alarm11InPs);
            PsLstAuto.Add(alarm12InPs);
            PsLstAuto.Add(alarm13InPs);
            PsLstAuto.Add(alarm14InPs);
            PsLstAuto.Add(alarm15InPs);
            PsLstAuto.Add(alarm16InPs);

            PsLstAuto.Add(startCountInOutPs);


            PsLstHand = new List<DTPlcInfoSimple>();            


            PsLstParam = new List<DTPlcInfoSimple>();
            PsLstIO = new List<DTPlcInfoSimple>();
            UserDataTable = new DataTable();

            AllPlcSimpleLst=  new List<List<DTPlcInfoSimple>>();

            AllPlcSimpleLst.Add(psLstAuto);
            AllPlcSimpleLst.Add(psLstHand);
            AllPlcSimpleLst.Add(psLstParam);
            AllPlcSimpleLst.Add(PsLstIO);

            paramFile = new ConfigFileManager();

            if (File.Exists(Constant.ConfigParamFilePath))
            {
                paramFile.LoadFile(Constant.ConfigParamFilePath);

                if (!int.TryParse(paramFile.ReadConfig(Constant.printBarcodeMode), out printBarCodeMode))
                {
                    MessageBox.Show(Constant.ErrorParamConfigFile);

                    Application.Exit();

                    System.Environment.Exit(0);
                }
            }
            else
            {
                MessageBox.Show(Constant.ErrorParamConfigFile);
                Application.Exit();
                System.Environment.Exit(0);
            }

            LogManager.WriteProgramLog(Constant.Start);

        }
        public bool RestartDevice(int id)
        {

            evokDevice.RestartConneect(evokDevice.DataFormLst[id]);
            return evokDevice.getDeviceData();

        }
        #region 运行部分

        public void ProClr()
        {
            evokDevice.SetDValue(prodOutInPs,0);
            optSize.prodClear();
        }
        //启动
        public void start(int id)
        {
            evokDevice.SetMValueOFF2ON(startOutPs);
           
            
            RunFlag = true;

            rtbWork.Clear();

            LogManager.WriteProgramLog(Constant.DeviceStartCut);

        }
        public void stop()
        {
            RunFlag = false;
            evokDevice.SetMValueOFF2ON(stopOutInPs);
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
        //复位
        public void reset()
        {
            stop();
            evokDevice.SetMValueOFF2ON(resetOutPs);
            LogManager.WriteProgramLog(Constant.DeviceReset);
        }
        #endregion
        public void SaveFile()
        {
            optSize.SaveCsv();
            optSize.SaveExcel();
        }
       
        #region 条码部分
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
                        evokDevice.SetMultiPleDValue(ldsCountInOutPs, DataList.ToArray());
                                              
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
                    Environment.Exit(0);
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
                Environment.Exit(0);
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
            evokDevice.SetDValue(cutDoneOutInPs, 0);
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
        public void LoadCsvData(string filename)
        {
            optSize.Len = lcOutInPs.ShowValue;
            optSize.Dbc = dbcOutInPs.ShowValue;
            optSize.Ltbc = ltbcOutInPs.ShowValue;
            optSize.Safe = safeOutInPs.ShowValue;
            optSize.LoadCsvData(filename);
        }
        public void LoadExcelData(string filename)
        {
            optSize.Len = lcOutInPs.ShowValue;
            optSize.Dbc = dbcOutInPs.ShowValue;
            optSize.Ltbc = ltbcOutInPs.ShowValue;
            optSize.Safe = safeOutInPs.ShowValue;
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

        public void InitControl()
        {

            if ((evokDevice.DataFormLst.Count > 0) && (evokDevice.DataFormLst[0] != null))
            {
                ConstantMethod.FindPos(evokDevice.DataFormLst[0], PsLstAuto);
            }
            if ((evokDevice.DataFormLst.Count > 0) && (evokDevice.DataFormLst[1] != null))
            {
                ConstantMethod.FindPos(evokDevice.DataFormLst[1], PsLstHand);
            }
            if ((evokDevice.DataFormLst.Count > 0) && (evokDevice.DataFormLst[2] != null))
            {
                ConstantMethod.FindPos(evokDevice.DataFormLst[2], PsLstParam);
            }
        }
        public bool ShiftPage(int pageid)
        {
            if (evokDevice.Status == Constant.DeviceConnected)
            {
                //页面切换需要告诉下位机
                if (pageid == Constant.AutoPage)
                {
                  //  evokDevice.SetDValue(pageShiftOutPs, 2);
                }
                if (pageid == Constant.HandPage)
                {
                    //evokDevice.SetDValue(pageShiftOutPs, 3);
                }

                if (pageid == Constant.ParamPage)
                {
                    passWdForm psswd = new passWdForm();
                    psswd.ShowDialog();

                    while (psswd.Visible)
                    {
                        Application.DoEvents();
                    }
                    string str = DateTime.Now.ToString("MMdd");
                    int psswdInt=0;
                    int.TryParse(str, out psswdInt);
                    psswdInt = psswdInt + 1000;
                    if (psswd.userInput.Equals(psswdInt.ToString()))
                    {

                    }
                    else
                    {
                        MessageBox.Show(Constant.pwdWrong);
                        return false;
                    }
                    psswd.Close();
                }

                evokDevice.shiftDataForm(pageid);
                                                                            
                FindPlcSimpleInPlcInfoLst(pageid);

                ConstantMethod.Delay(50);

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
        private DTPlcInfoSimple getPsFromPslLst(string tag0, string str0, List<DTPlcInfoSimple> pslLst)
        {
            foreach (DTPlcInfoSimple simple in pslLst)
            {
                if (simple.Name.ToString().Contains(tag0) && simple.Name.Contains(str0))
                {
                    return simple;
                }
            }
            return null;
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
     
        public void SetDValue(string str1, string str2, List<DTPlcInfoSimple> pLst,int num)
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
        public void InitDgvParam(DataGridView dgvParam)
        {
            if (evokDevice.DataFormLst.Count > 2)
            {
                dgvParam.AutoGenerateColumns = false;
                dgvParam.DataSource = evokDevice.DataFormLst[2];
                dgvParam.Columns["bin"].DataPropertyName = evokDevice.DataFormLst[2].Columns["bin"].ToString();
                dgvParam.Columns["value"].DataPropertyName = evokDevice.DataFormLst[2].Columns["value"].ToString();
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
                    evokDevice.DPlcInfo[result].IsInEdit = editEnable;
                }
                else
                {
                    evokDevice.MPlcInfoAll[result][num4].IsInEdit = editEnable;
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

            foreach (List<DTPlcInfoSimple> pLst in AllPlcSimpleLst)
            {
                foreach (DTPlcInfoSimple p in pLst)
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
        private void FindPlcInfo(DTPlcInfoSimple p, List<DTPlcInfo> dplc, List<List<DTPlcInfo>> mplc)
        {
            if (p.Area == null) return;
            if (dplc == null || 
                mplc == null || 
                dplc.Count == 0 || 
                mplc.Count == 0  
                ) return;
            foreach (DTPlcInfo p0 in dplc)
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
