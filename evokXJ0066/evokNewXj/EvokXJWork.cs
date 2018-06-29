﻿using FastReport;
using FastReport.Barcode;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using xjplc;

namespace evokNew0066
{
    public class EvokXJWork
    {
        //用户排版数据
        DataTable userDataTable;

        OptSize optSize;
        //设备类
        EvokXJDevice evokDevice;

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
        List<List<PlcInfoSimple>> AllPlcSimpleLst;
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
        public void SetEvokDevice(EvokXJDevice evokDevice0)
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
                        PlcInfoSimple p = new PlcInfoSimple(name);
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

        public void ChangePrintMode(int value)
        {
           paramFile.WriteConfig(Constant.printBarcodeMode, value.ToString());

             printBarCodeMode = value;//

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
        List<PlcInfoSimple> psLstAuto;
        public List<xjplc.PlcInfoSimple> PsLstAuto
        {
            get { return psLstAuto; }
            set { psLstAuto = value; }
        }
        //定义后要加入集合  //忽略寄存器的影响直接匹配参数名     
        
        public PlcInfoSimple autoMesOutInPs = new PlcInfoSimple("自动测长标志读写");
        public PlcInfoSimple dbcOutInPs     = new PlcInfoSimple("刀补偿读写");
        public PlcInfoSimple ltbcOutInPs    = new PlcInfoSimple("料头补偿读写");
        public PlcInfoSimple safeOutInPs    = new PlcInfoSimple("安全距离读写");
        public PlcInfoSimple prodOutInPs    = new PlcInfoSimple("总产量读写"); 
        public PlcInfoSimple lcOutInPs      = new PlcInfoSimple("料长读写");
        public PlcInfoSimple stopOutInPs    = new PlcInfoSimple("停止读写");
        public PlcInfoSimple cutDoneOutInPs = new PlcInfoSimple("切割完毕读写");
        public PlcInfoSimple plcHandlebarCodeOutInPs = new PlcInfoSimple("条码打印读写");

        public PlcInfoSimple pauseOutPs     = new PlcInfoSimple("暂停写");
        public PlcInfoSimple startOutPs     = new PlcInfoSimple("启动写");             
        public PlcInfoSimple resetOutPs     = new PlcInfoSimple("复位写");
        public PlcInfoSimple autoSLOutPs    = new PlcInfoSimple("自动上料写");
        public PlcInfoSimple pageShiftOutPs = new PlcInfoSimple("页面切换写");
        

        public PlcInfoSimple emgStopInPs    = new PlcInfoSimple("急停读");
        public PlcInfoSimple startInPs      = new PlcInfoSimple("启动读");
        public PlcInfoSimple resetInPs      = new PlcInfoSimple("复位读");
        public PlcInfoSimple pauseInPs      = new PlcInfoSimple("暂停读");
        public PlcInfoSimple autoSLInPs     = new PlcInfoSimple("自动上料读");
        public PlcInfoSimple autoCCInPs     = new PlcInfoSimple("自动测长读");
        public PlcInfoSimple clInPs         = new PlcInfoSimple("出料读");
        public PlcInfoSimple slInPs         = new PlcInfoSimple("送料读");
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


        #endregion
        #region 手动
        List<PlcInfoSimple> psLstHand;
        public PlcInfoSimple slzOutPs = new PlcInfoSimple("送料左写");
        public PlcInfoSimple slyOutPs = new PlcInfoSimple("送料右写");
        public PlcInfoSimple clzOutPs = new PlcInfoSimple("出料左写");
        public PlcInfoSimple clyOutPs = new PlcInfoSimple("出料右写");
        public PlcInfoSimple jlzOutPs = new PlcInfoSimple("锯料正写");
        public PlcInfoSimple jlfOutPs = new PlcInfoSimple("检测正写");
        public PlcInfoSimple jcfOutPs = new PlcInfoSimple("检测负写");
        public PlcInfoSimple sldjjOutPs = new PlcInfoSimple("上料电机写");
        public PlcInfoSimple sldjOutPs = new PlcInfoSimple("送料侧压写");
        public PlcInfoSimple qlqgOutPs = new PlcInfoSimple("切料气缸写");
        public PlcInfoSimple tmzkxffOutPs = new PlcInfoSimple("条码真空吸附阀写");
        public PlcInfoSimple qddjOutPs = new PlcInfoSimple("切刀电机写");
        public PlcInfoSimple qlcyzOutPs = new PlcInfoSimple("切料侧压左写");
        public PlcInfoSimple slksOutPs = new PlcInfoSimple("上料靠栅写");
        public PlcInfoSimple sljsjOutPs = new PlcInfoSimple("上料架升降写");
        public PlcInfoSimple qlylOutPs = new PlcInfoSimple("切料压料写");
        public PlcInfoSimple qlcyyOutPs = new PlcInfoSimple("切料侧压右写");
        public PlcInfoSimple sfslwOutPs = new PlcInfoSimple("伺服上料位写");
        public PlcInfoSimple tmccfOutPs = new PlcInfoSimple("条码吹尘阀写");
        public PlcInfoSimple cldjOutPs = new PlcInfoSimple("出料电机写");
        public PlcInfoSimple tmtgcqfOutPs = new PlcInfoSimple("条码铜管吹气阀写");
        public PlcInfoSimple cljlOutPs = new PlcInfoSimple("出料夹料写");
        public PlcInfoSimple tmxyqgOutPs = new PlcInfoSimple("条码下压气缸写");
        public PlcInfoSimple tmspjcqgOutPs = new PlcInfoSimple("条码水平进出气缸写");
        public PlcInfoSimple sljccOutPs = new PlcInfoSimple("上料架检测吹尘写");
        public PlcInfoSimple slsyOutPs = new PlcInfoSimple("送料上压写");

        public PlcInfoSimple slsyInPs = new PlcInfoSimple("送料上压读");
        public PlcInfoSimple sljccInPs = new PlcInfoSimple("上料架检测吹尘读");
        public PlcInfoSimple slInPs0 = new PlcInfoSimple("送料读");      
        public PlcInfoSimple clInPs0 = new PlcInfoSimple("出料读");   
        public PlcInfoSimple jlInPs = new PlcInfoSimple("锯料读");
        public PlcInfoSimple jcInPs = new PlcInfoSimple("检测读");     
        public PlcInfoSimple sldjjInPs = new PlcInfoSimple("上料电机读");
        public PlcInfoSimple sldjInPs = new PlcInfoSimple("送料侧压读");
        public PlcInfoSimple qlqgInPs = new PlcInfoSimple("切料气缸读");
        public PlcInfoSimple tmzkxffInPs = new PlcInfoSimple("条码真空吸附阀读");
        public PlcInfoSimple qddjInPs = new PlcInfoSimple("切刀电机读");
        public PlcInfoSimple qlcyzInPs = new PlcInfoSimple("切料侧压左读");
        public PlcInfoSimple slksInPs = new PlcInfoSimple("上料靠栅读");
        public PlcInfoSimple sljsjInPs = new PlcInfoSimple("上料架升降读");
        public PlcInfoSimple qlylInPs = new PlcInfoSimple("切料压料读");
        public PlcInfoSimple qlcyyInPs = new PlcInfoSimple("切料侧压右读");
        public PlcInfoSimple sfslwInPs = new PlcInfoSimple("伺服上料位读");
        public PlcInfoSimple tmccfInPs = new PlcInfoSimple("条码吹尘阀读");
        public PlcInfoSimple cldjInPs = new PlcInfoSimple("出料电机读");
        public PlcInfoSimple tmtgcqfInPs = new PlcInfoSimple("条码铜管吹气阀读");
        public PlcInfoSimple cljlInPs = new PlcInfoSimple("出料夹料读");
        public PlcInfoSimple tmxyqgInPs = new PlcInfoSimple("条码下压气缸读");
        public PlcInfoSimple tmspjcqgInPs = new PlcInfoSimple("条码水平进出气缸读");

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
        public EvokXJWork()
        {
            PsLstAuto = new List<PlcInfoSimple>();
            PsLstAuto.Add(autoMesOutInPs);
            PsLstAuto.Add(dbcOutInPs);
            PsLstAuto.Add(ltbcOutInPs);
            PsLstAuto.Add(safeOutInPs);
            PsLstAuto.Add(prodOutInPs);
            PsLstAuto.Add(lcOutInPs);
            PsLstAuto.Add(stopOutInPs);
            PsLstAuto.Add(cutDoneOutInPs);
            PsLstAuto.Add(plcHandlebarCodeOutInPs);
           
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


            PsLstHand = new List<PlcInfoSimple>();            


            PsLstParam = new List<PlcInfoSimple>();
            PsLstIO = new List<PlcInfoSimple>();
            UserDataTable = new DataTable();

            AllPlcSimpleLst=  new List<List<PlcInfoSimple>>();

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
        public void start()
        {
            evokDevice.SetMValueOFF2ON(startOutPs);
            //切割类
            CutThreadStart = new ThreadStart(CutWork0);
            //初始化Thread的新实例，并通过构造方法将委托ts做为参数赋初始值。
            CutThread = new Thread(CutThreadStart);   //需要引入System.Threading命名空间
            RunFlag = true;

            rtbWork.Clear();

        }
        public void stop()
        {
            RunFlag = false;
            evokDevice.SetMValueOFF2ON(stopOutInPs);
            optSize.SingleSizeLst.Clear();
            optSize.ProdInfoLst.Clear();
        }

        public bool IsInEmg {
            get
            {
                if (emgStopInPs.ShowValue == Constant.M_ON) return true; else return false;
            }
        }
        //停止 
        public void pause()
        {
            evokDevice.SetMValueOFF2ON(pauseOutPs);
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
        private void CutWork0()
        {
            int CutProCnt = 0;
            List<string[]> lstStr = new List<string[]>();
            string[] s1 = new string[19];           

            if (optSize.ProdInfoLst.Count > 0)
            {             
                for (int i = CutProCnt; i < optSize.ProdInfoLst.Count; i++)
                {
                    //plc 计数器 清零
                    evokDevice.SetDValue(cutDoneOutInPs, 0);

                    ConstantMethod.ShowInfo(rtbWork, "第" + (i + 1).ToString() + "根木料开始切割");
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

                    evokDevice.SetMultiPleDValue(lcOutInPs, DataList.ToArray());

                    //打第一条条码
                    if ( optSize.SingleSizeLst[i].Count>0)                                                            
                    printBarcode(printReport,optSize.SingleSizeLst[i][0].ParamStrLst.ToArray());

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

                
            }
            else
            {
                MessageBox.Show(Constant.noData);
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
        public void CutStartMeasure()
        {
         
            if (IsInEmg)
            {
                MessageBox.Show(Constant.emgStopTip);
                return;
            }

            //启动
            start();

            //等待 测量
            while (mRunFlag)
            {
                                
                int valueOld = 1;

                ConstantMethod.DelayMeasure(Constant.MeaSureMaxTime, ref valueOld, ref autoCCInPs,ref emgStopInPs,ref mRunFlag);

                if (IsInEmg)
                {
                    stop();
                }

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
                    if(CutThreadStart ==null)            
                    CutThreadStart = new ThreadStart(CutWork0);
                    //初始化Thread的新实例，并通过构造方法将委托ts做为参数赋初始值。
                    if (CutThread == null)
                        CutThread = new Thread(CutThreadStart);   //需要引入System.Threading命名空间

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
                ConstantMethod.ShowInfo(rtbWork,Constant.NextOpt);
            }

            stop();
            MessageBox.Show(Constant.CutEnd);
        }
        public void CutStartNormal()
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
            start();
                        
            try
            {               
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
                stop();
                MessageBox.Show(Constant.CutEnd);
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
                    evokDevice.SetDValue(pageShiftOutPs, 2);
                }
                if (pageid == Constant.HandPage)
                {
                    evokDevice.SetDValue(pageShiftOutPs, 3);
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

        #region 寄存器操作部分
        private PlcInfoSimple getPsFromPslLst(string tag0, string str0, List<PlcInfoSimple> pslLst)
        {
            foreach (PlcInfoSimple simple in pslLst)
            {
                if (simple.Name.ToString().Contains(tag0) && simple.Name.Contains(str0))
                {
                    return simple;
                }
            }
            return null;
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
     
        public void SetDValue(string str1, string str2, List<PlcInfoSimple> pLst,int num)
        {
            PlcInfoSimple p = getPsFromPslLst(str1, str2, pLst);
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
        private void FindPlcInfo(PlcInfoSimple p, List<PlcInfo> dplc, List<List<PlcInfo>> mplc)
        {
            if (p.Area == null) return;
            if (dplc == null || 
                mplc == null || 
                dplc.Count == 0 || 
                mplc.Count == 0  
                ) return;
            foreach (PlcInfo p0 in dplc)
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
