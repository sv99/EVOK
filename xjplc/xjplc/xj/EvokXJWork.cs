namespace xjplc
{
    using evokNewXJ;
    using FastReport;
    using FastReport.Barcode;
    using simi;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Drawing;
    using System.Drawing.Printing;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Timers;
    using System.Windows.Forms;

    public class EvokXJWork
    {
        private List<List<PlcInfoSimple>> allPlcSimpleLst;
        public PlcInfoSimple autoCCInPs;
        public PlcInfoSimple autoMesOutInPs;
        private ComboBox cbOptParam1;
        public PlcInfoSimple clInPs;
        private bool comIsDownLoading;
        private int currentDoorSizeCount;
        private string CurrentDoorType;
        private int currentPageId;
        public PlcInfoSimple cutDoneOutInPs;
        private int CutProCnt;
        private int cutSelMode;
        private Thread CutThread;
        private ThreadStart CutThreadStart;
        public PlcInfoSimple dataNotEnoughOutInPs;
        public PlcInfoSimple dataNotEnoughValueOutInPs;
        public PlcInfoSimple dataOutPs;
        public PlcInfoSimple dbcOutInPs;
        private string deviceName;
        private int deviceProperty;
        public PlcInfoSimple deviceStatusOutInPs;
        private DataGridView dgvIO;
        public PlcInfoSimple doorbanCurrentDoorIdInPs;
        public PlcInfoSimple doorbanCurrentStatusInPs;
        public PlcInfoSimple doorbanDataChangDownLoadSuccessOutPs;
        public PlcInfoSimple doorbanDataChangRequestInPs;
        public PlcInfoSimple doorbanDataKuanDownLoadSuccessOutPs;
        public PlcInfoSimple doorbanDataKuanRequestInPs;
        public PlcInfoSimple doorbanDoorCountInOutPs;
        public PlcInfoSimple doorBanLen;
        public PlcInfoSimple doorbanLenthChangInOutPs;
        public PlcInfoSimple doorbanLenthKuanInOutPs;
        private string[] doorBanStatus0;
        public PlcInfoSimple doorBanWidth;
        public PlcInfoSimple doorCutCntOutInPs;
        public PlcInfoSimple doorDownLoadCountInOutPs;
        public PlcInfoSimple doorshellCurrentDoorIdInPs;
        public PlcInfoSimple doorshellCurrentStatusInPs;
        public PlcInfoSimple doorshellDataChangDownLoadSuccessOutPs;
        public PlcInfoSimple doorshellDataChangRequestInPs;
        public PlcInfoSimple doorshellDataDoorIdDownLoadSuccessOutPs;
        public PlcInfoSimple doorshellDataDoorIdRequestInPs;
        public PlcInfoSimple doorshellDataKuanDownLoadSuccessOutPs;
        public PlcInfoSimple doorshellDataKuanRequestInPs;
        public PlcInfoSimple doorshellDoorCountInOutPs;
        public PlcInfoSimple doorshellDoorIdInOutPs;
        public PlcInfoSimple doorShellLen;
        public PlcInfoSimple doorshellLenthChangInOutPs;
        public PlcInfoSimple doorshellLenthKuanInOutPs;
        private string[] doorShellStatus0;
        public PlcInfoSimple doorShellWidth;
        public PlcInfoSimple doorSize;
        public PlcInfoSimple doorTypeCutCountOutInPs;
        private const int downLoadIdDoorBanChang = 2;
        private const int downLoadIdDoorBanKuan = 3;
        private const int downLoadIdShellChang = 4;
        private const int downLoadIdShellDoorId = 6;
        private const int downLoadIdShellKuan = 5;
        private const int downLoadIdXiaLiaoChang = 1;
        private int downLoadSizeId;
        public PlcInfoSimple emgStopInPs;
        public PlcInfoSimple emgStopOutPs;
        private List<string> errorList;
        private EvokXJDevice evokDevice;
        public PlcInfoSimple gjOutInPs;
        public PlcInfoSimple GWOutInPs;
        public PlcInfoSimple heightCountInOutPs;
        private List<DataTable> holeDataLst;
        private List<DataTable> grooveDataLst;
        public PlcInfoSimple inspectPatternDoneInOutPs;
        public PlcInfoSimple inspectPatternModeInOutPs;
        public PlcInfoSimple inspectPatternPosInOutPs;
        private bool IsRuninng;
        private bool isSaveProdLog;
        public PlcInfoSimple KSInPs;
        private Label lblStatus;
        public PlcInfoSimple lcOutInPs;
        public PlcInfoSimple ldsOutInPs;
        public PlcInfoSimple leftRightShowInPs;
        public PlcInfoSimple leftRightShowOutPs;
        public PlcInfoSimple lineEmgStopInPs;
        public PlcInfoSimple lineEmgStopOutPs;
        public PlcInfoSimple linePauseInPs;
        public PlcInfoSimple linePauseOutPs;
        public PlcInfoSimple lineResetInPs;
        public PlcInfoSimple lineResetOutPs;
        public PlcInfoSimple lineStartInPs;
        public PlcInfoSimple lineStartOutPs;
        public PlcInfoSimple lineStartTipInPs;
        public PlcInfoSimple lineStopInPs;
        public PlcInfoSimple lineStopOutPs;
        public PlcInfoSimple liWaiInPs;
        public PlcInfoSimple liWaiOutPs;
        public PlcInfoSimple lkOutInPs;
        public PlcInfoSimple lliaoOutInPs;
        public PlcInfoSimple LMInPs;
        public PlcInfoSimple LMSLInPs;
        public PlcInfoSimple ltbcDefaultOutInPs;
        public PlcInfoSimple ltbcOutInPs;
        private Form mainForm;
        public PlcInfoSimple materialIdInOutPs;
        public PlcInfoSimple materialSetEnInOutPs;
        private string minPrinterName;
        private int minPrintSize;
        public PlcInfoSimple modeSelectOutInPs;
        private bool mRunFlag;
        public PlcInfoSimple muxiaoHoleOutPs;
        public PlcInfoSimple mxkShowInPs;
        public PlcInfoSimple mxkShowOutPs;
        public PlcInfoSimple mxkStringShowInPs;
        public PlcInfoSimple noSizeToCutOutInPs;
        public PlcInfoSimple thisLenGoOutOutInPs;
        private int oldPrintBarCodeMode;
        private OptSize optSize;
        public PlcInfoSimple pageShiftOutPs;
        private ConfigFileManager paramFile;
        public PlcInfoSimple pauseInPs;
        public PlcInfoSimple pauseOutPs;
        public PlcInfoSimple PJInPs;
        public PlcInfoSimple PJOutPs;
        public PlcInfoSimple plcHandlebarCodeOutInPs;
        public PlcInfoSimple pos1EnInPs;
        public PlcInfoSimple pos1EnOutPs;
        public PlcInfoSimple pos1OutInPs;
        public PlcInfoSimple pos2EnInPs;
        public PlcInfoSimple pos2EnOutPs;
        public PlcInfoSimple pos2OutInPs;
        public PlcInfoSimple posMode;
        public PlcInfoSimple pressOutInPs;
        public PlcInfoSimple AutoPosDataOutInPs;
        private int printBarCodeMode;
        public PlcInfoSimple printMiniSizeOutInPs;
        private Report printReport;
      //  private Dictionary<Thread, barCodeManger> printThreadLst;
        public PlcInfoSimple prodOutInPs;
        private patternSize pSize;
        private List<PlcInfoSimple> psLstAuto;
        private List<PlcInfoSimple> psLstEx1;
        private List<PlcInfoSimple> psLstEx2;
        private List<PlcInfoSimple> psLstEx3;
        private List<PlcInfoSimple> psLstEx4;
        private List<PlcInfoSimple> psLstEx5;
        private List<PlcInfoSimple> psLstEx6;
        private List<PlcInfoSimple> psLstEx7;
        private List<PlcInfoSimple> psLstEx8;
        private List<PlcInfoSimple> psLstEx9;
        private List<PlcInfoSimple> psLstHand;
        private List<PlcInfoSimple> psLstIO;
        private List<PlcInfoSimple> psLstParam;
        public PlcInfoSimple resetInPs;
        public PlcInfoSimple resetOutPs;
        public PlcInfoSimple restMaterialRangeInOutPs;
        private RichTextBox rtbResult;
        private RichTextBox rtbWork;
        private bool runflag_DoorBanChang;
        private bool runflag_DoorBanKuan;
        private bool runflag_DoorShellChang;
        private bool runflag_DoorShellDoorId;
        private bool runflag_DoorShellKuan;
        private bool runflag_XialiaoJu;
        public PlcInfoSimple safeOutInPs;
        public PlcInfoSimple scarInPs;
        private bool scarMode;
        public PlcInfoSimple scarModeInPs;
        public PlcInfoSimple scarModeOutPs;
        public PlcInfoSimple sfslwInPs;
        public Label showFilePathLabel;
        public PlcInfoSimple slInPs;
        public PlcInfoSimple smxShowInPs;
        public PlcInfoSimple smxShowOutPs;
        public PlcInfoSimple startCountInOutPs;
        public PlcInfoSimple startInPs;
        public PlcInfoSimple startOutPs;
        public PlcInfoSimple stopInPs;
        public PlcInfoSimple stopOutPs;
      
        private System.Timers.Timer tCheckPrint;
        public Action<string> updateData;
        private DataTable userDataTable;
        public PlcInfoSimple widht1InOutPs;
        public PlcInfoSimple widht2InOutPs;
        public PlcInfoSimple widthCountInOutPs;
        public PlcInfoSimple wlExistOutInPs;
        public PlcInfoSimple wlHeightOutInPs;
        public PlcInfoSimple wlInOutPs;
        public PlcInfoSimple wlMiniSizeOutInPs;
        public PlcInfoSimple wlWidthOutInPs;
        public PlcInfoSimple xialiaoCurrentDoorIdInPs;
        public PlcInfoSimple xialiaoCurrentStatusInPs;
        public PlcInfoSimple xialiaoDataDownLoadSuccessOutPs;
        public PlcInfoSimple xialiaoDataRequestInPs;
        public PlcInfoSimple xialiaoDoorCntInPs;
        public PlcInfoSimple xialiaoDoorCountInOutPs;
        public PlcInfoSimple xialiaojuOutPs;
        private string[] xialiaojuStatus0;
        public PlcInfoSimple yljxOutInPs;
        public PlcInfoSimple zkShowInPs;
        public PlcInfoSimple zkShowOutPs;
        public PlcInfoSimple ZQInPs;
        public PlcInfoSimple zxShowInPs;
        public PlcInfoSimple zxShowOutPs;
        #region ͨ��ʽ˫�˾�
        public PlcInfoSimple   sdjDataDownLoadEnd;
        public PlcInfoSimple   sdjLeftEnable;
        public PlcInfoSimple   sdjRightEnable;
        public PlcInfoSimple   sdjLeftDataAddress;
        public PlcInfoSimple sdjRightDataAddress;
        #endregion

        public SqlConnection lo_conn;
        public EvokXJWork()
        {
            deviceName = "";
            currentPageId = -1;
            minPrintSize = 0;
            minPrinterName = "";
            cutSelMode = 0;
            printBarCodeMode = 0;
            mRunFlag = false;
            oldPrintBarCodeMode = Constant.NoPrintBarCode;
            printMiniSizeOutInPs = new PlcInfoSimple("��ӡ��С�ߴ��д");
            wlMiniSizeOutInPs = new PlcInfoSimple("β�ϳߴ����ƶ�д");
            autoMesOutInPs = new PlcInfoSimple("�Զ��ⳤ��־��д");
            dbcOutInPs = new PlcInfoSimple("��������д");
            ltbcOutInPs = new PlcInfoSimple("��ͷ������д");
            ltbcDefaultOutInPs = new PlcInfoSimple("��ͷ�̶�������д");
            safeOutInPs = new PlcInfoSimple("��ȫ�����д");
            prodOutInPs = new PlcInfoSimple("�ܲ�����д");
            noSizeToCutOutInPs = new PlcInfoSimple("��ƥ���д");
            thisLenGoOutOutInPs = new PlcInfoSimple("���϶�д");
            AutoPosDataOutInPs = new PlcInfoSimple("�Զ����ȶ�д");
            ldsOutInPs = new PlcInfoSimple("�϶�����д");
            lcOutInPs = new PlcInfoSimple("�ϳ���д");
            lkOutInPs = new PlcInfoSimple("�Ͽ��д");
            yljxOutInPs = new PlcInfoSimple("Ԥ����϶��д");
            stopOutPs = new PlcInfoSimple("ֹͣд");
            stopInPs = new PlcInfoSimple("ֹͣ��");
            cutDoneOutInPs = new PlcInfoSimple("�и���϶�д");
            plcHandlebarCodeOutInPs = new PlcInfoSimple("�����ӡ��д");
            startCountInOutPs = new PlcInfoSimple("��ʼ������д");
            wlInOutPs = new PlcInfoSimple("β�϶�д");
            wlExistOutInPs = new PlcInfoSimple("H0��д");
            wlWidthOutInPs = new PlcInfoSimple("H100��д");
            wlHeightOutInPs = new PlcInfoSimple("H101��д");
            lliaoOutInPs = new PlcInfoSimple("���Ͽ��ض�д");
            widthCountInOutPs = new PlcInfoSimple("�ߴ�������д");
            heightCountInOutPs = new PlcInfoSimple("�ߴ糤������д");
            modeSelectOutInPs = new PlcInfoSimple("ģʽѡ���д");
            deviceStatusOutInPs = new PlcInfoSimple("�豸״̬��д");
            lineStartTipInPs = new PlcInfoSimple("�������Ѷ�");
            lineStartOutPs = new PlcInfoSimple("������д");
            lineStartInPs = new PlcInfoSimple("��������");
            lineResetOutPs = new PlcInfoSimple("�߸�λд");
            lineResetInPs = new PlcInfoSimple("�߸�λ��");
            lineStopOutPs = new PlcInfoSimple("��ֹͣд");
            lineStopInPs = new PlcInfoSimple("��ֹͣ��");
            linePauseOutPs = new PlcInfoSimple("����ͣд");
            linePauseInPs = new PlcInfoSimple("����ͣ��");
            doorSize = new PlcInfoSimple("���Ͼ��ϳ���д");
            doorBanLen = new PlcInfoSimple("�ݺ���ϳ���д");
            doorBanWidth = new PlcInfoSimple("�ݺ���Ͽ��д");
            doorShellLen = new PlcInfoSimple("��Ƥ���ϳ���д");
            doorShellWidth = new PlcInfoSimple("��Ƥ���Ͽ��д");
            doorDownLoadCountInOutPs = new PlcInfoSimple("ÿ���·�������д");
            xialiaojuOutPs = new PlcInfoSimple("���Ͼⳤ�ȶ�д");
            xialiaoDataRequestInPs = new PlcInfoSimple("���Ͼ��������ݶ�");
            xialiaoDataDownLoadSuccessOutPs = new PlcInfoSimple("���Ͼ��·��ɹ�д");
            xialiaoCurrentDoorIdInPs = new PlcInfoSimple("���Ͼ⵱ǰ�źŶ�");
            xialiaoCurrentStatusInPs = new PlcInfoSimple("���Ͼ�״̬��");
            xialiaoDoorCountInOutPs = new PlcInfoSimple("���Ͼ��·�����������д");
            xialiaoDoorCntInPs = new PlcInfoSimple("���Ͼ������");
            doorbanLenthChangInOutPs = new PlcInfoSimple("�Ű�ⳤ�ȶ�д");
            doorbanLenthKuanInOutPs = new PlcInfoSimple("�Ű���ȶ�д");
            doorbanDataKuanRequestInPs = new PlcInfoSimple("�Ű����������ݶ�");
            doorbanDataChangRequestInPs = new PlcInfoSimple("�Ű���������ݶ�");
            doorbanDataKuanDownLoadSuccessOutPs = new PlcInfoSimple("�Ű����·��ɹ�д");
            doorbanDataChangDownLoadSuccessOutPs = new PlcInfoSimple("�Ű�ⳤ�·��ɹ�д");
            doorbanCurrentDoorIdInPs = new PlcInfoSimple("�Ű�⵱ǰ�źŶ�");
            doorbanCurrentStatusInPs = new PlcInfoSimple("�Ű��״̬��");
            doorbanDoorCountInOutPs = new PlcInfoSimple("�Ű���·�����������д");
            doorshellLenthChangInOutPs = new PlcInfoSimple("��Ƥ�ⳤ�ȶ�д");
            doorshellLenthKuanInOutPs = new PlcInfoSimple("��Ƥ���ȶ�д");
            doorshellDataKuanRequestInPs = new PlcInfoSimple("��Ƥ����������ݶ�");
            doorshellDataKuanDownLoadSuccessOutPs = new PlcInfoSimple("��Ƥ����·��ɹ�д");
            doorshellDataChangRequestInPs = new PlcInfoSimple("��Ƥ���������ݶ�");
            doorshellDataChangDownLoadSuccessOutPs = new PlcInfoSimple("��Ƥ�ⳤ�·��ɹ�д");
            doorshellCurrentDoorIdInPs = new PlcInfoSimple("��Ƥ�⵱ǰ�źŶ�");
            doorshellCurrentStatusInPs = new PlcInfoSimple("��Ƥ��״̬��");
            doorshellDoorIdInOutPs = new PlcInfoSimple("��Ƥ����Id��д");
            doorshellDataDoorIdRequestInPs = new PlcInfoSimple("��Ƥ����Id�������ݶ�");
            doorshellDataDoorIdDownLoadSuccessOutPs = new PlcInfoSimple("��Ƥ����Id�·��ɹ�д");
            doorshellDoorCountInOutPs = new PlcInfoSimple("��Ƥ���·�����������д");
            lineEmgStopInPs = new PlcInfoSimple("�߼�ͣ��");
            lineEmgStopOutPs = new PlcInfoSimple("�߼�ͣд");
            dataOutPs = new PlcInfoSimple("�������ض�д");
            muxiaoHoleOutPs = new PlcInfoSimple("ľ��������д");
            mxkStringShowInPs = new PlcInfoSimple("����ľ����ѡ����ʾ��");
            zxShowInPs = new PlcInfoSimple("����ľм��");
            zkShowInPs = new PlcInfoSimple("�п�˫ľм��");
            smxShowInPs = new PlcInfoSimple("˫ľм��");
            mxkShowInPs = new PlcInfoSimple("ľ�ҿ�ѡ���");
            leftRightShowInPs = new PlcInfoSimple("����ģʽ��");
            liWaiInPs = new PlcInfoSimple("����ģʽ��");
            zxShowOutPs = new PlcInfoSimple("����ľмд");
            zkShowOutPs = new PlcInfoSimple("�п�˫ľмд");
            smxShowOutPs = new PlcInfoSimple("˫ľмд");
            mxkShowOutPs = new PlcInfoSimple("ľ�ҿ�ѡ��д");
            leftRightShowOutPs = new PlcInfoSimple("����ģʽд");
            liWaiOutPs = new PlcInfoSimple("����ģʽд");
            widht1InOutPs = new PlcInfoSimple("�Ͽ�1��д");
            widht2InOutPs = new PlcInfoSimple("�Ͽ�2��д");
            restMaterialRangeInOutPs = new PlcInfoSimple("�������ö�д");
            inspectPatternModeInOutPs = new PlcInfoSimple("����ģʽ��д");
            inspectPatternDoneInOutPs = new PlcInfoSimple("���Ʋ�����ɶ�д");
            inspectPatternPosInOutPs = new PlcInfoSimple("���ƾ������ö�д");
            materialSetEnInOutPs = new PlcInfoSimple("��������ʹ�ܶ�д");
            materialIdInOutPs = new PlcInfoSimple("���ϴ��Ŷ�д");
            pos1OutInPs = new PlcInfoSimple("����1���ȶ�д");
            pos2OutInPs = new PlcInfoSimple("����2���ȶ�д");
            pos1EnOutPs = new PlcInfoSimple("����1ʹ��д");
            pos2EnOutPs = new PlcInfoSimple("����2ʹ��д");
            pos1EnInPs = new PlcInfoSimple("����1ʹ�ܶ�");
            pos2EnInPs = new PlcInfoSimple("����2ʹ�ܶ�");
            posMode = new PlcInfoSimple("ģʽѡ��λ�ö�д");
            pauseOutPs = new PlcInfoSimple("��ͣд");
            startOutPs = new PlcInfoSimple("����д");
            resetOutPs = new PlcInfoSimple("��λд");
            scarModeOutPs = new PlcInfoSimple("��̲���д");
            pageShiftOutPs = new PlcInfoSimple("ҳ���л�д");
            pressOutInPs = new PlcInfoSimple("ѹ�϶�д");
            doorCutCntOutInPs = new PlcInfoSimple("�ŵ�����д");
            dataNotEnoughOutInPs = new PlcInfoSimple("���ݴﵽ��λ��д");
            dataNotEnoughValueOutInPs = new PlcInfoSimple("������λֵ��д");
            gjOutInPs = new PlcInfoSimple("������д");
            GWOutInPs = new PlcInfoSimple("��λ��д");
            PJOutPs = new PlcInfoSimple("�罺д");
            PJInPs = new PlcInfoSimple("�罺��");
            ZQInPs = new PlcInfoSimple("���ж�");
            KSInPs = new PlcInfoSimple("��դ��");
            LMInPs = new PlcInfoSimple("���е���");
            LMSLInPs = new PlcInfoSimple("�������϶�");
            doorTypeCutCountOutInPs = new PlcInfoSimple("���͵�����д");
            sfslwInPs = new PlcInfoSimple("�ŷ�����λ��");
            emgStopInPs = new PlcInfoSimple("��ͣ��");
            emgStopOutPs = new PlcInfoSimple("��ͣд");
            startInPs = new PlcInfoSimple("������");
            resetInPs = new PlcInfoSimple("��λ��");
            pauseInPs = new PlcInfoSimple("��ͣ��");
            scarModeInPs = new PlcInfoSimple("��̲�����");
            scarInPs = new PlcInfoSimple("��̶�");
            autoCCInPs = new PlcInfoSimple("�Զ��ⳤ��");
            clInPs = new PlcInfoSimple("���϶�");
            slInPs = new PlcInfoSimple("���϶�");

            sdjDataDownLoadEnd = new PlcInfoSimple("�����·���϶�д");
            sdjLeftEnable = new PlcInfoSimple("��1ʹ�ܶ�д");
            sdjRightEnable = new PlcInfoSimple("��1ʹ�ܶ�д");
            sdjLeftDataAddress = new PlcInfoSimple("��1�����·���ַ��д");
            sdjRightDataAddress = new PlcInfoSimple("��1�����·���ַ��д");

        xialiaojuStatus0 = new string[] { "������", "׼������", "��ͣ��", "��ͣ��", "������", "ͨѶ����", "��λ��" };
            doorBanStatus0 = new string[] { "������", "׼������", "��ͣ��", "��ͣ��", "������", "ͨѶ����", "��λ��" };
            doorShellStatus0 = new string[] { "������", "׼������", "��ͣ��", "��ͣ��", "������", "ͨѶ����", "��λ��" };
            comIsDownLoading = false;
            downLoadSizeId = 0;
            pSize = new patternSize();
            tCheckPrint = new System.Timers.Timer(1000.0);
            CutProCnt = 0;
            CurrentDoorType = "123456789";
            List<string> strfile = new List<string> {
                Constant.PlcDataFilePathAuto,
                Constant.PlcDataFilePathHand,
                Constant.PlcDataFilePathParam,
                Constant.PlcDataFilePathIO
            };
            if (File.Exists(Constant.PlcDataFilePathScar))
            {
                strfile.Add(Constant.PlcDataFilePathScar);
            }
            for (int i = strfile.Count - 1; i >= 0; i--)
            {
                if (!File.Exists(strfile[i]))
                {
                    MessageBox.Show(strfile[i] + Constant.ErrorPlcFile);
                    strfile.RemoveAt(i);
                }
            }
            evokDevice = new EvokXJDevice(strfile);
            evokDevice.setDeviceId(DeviceName);
            InitUsualTest();
        }

      
        public void ErrorListSave()
        {

        }
        List<string> warningList = new List<string>();
        public System.Collections.Generic.List<string> WarningList
        {
            get { return warningList; }
            set { warningList = value; }
        }
        public void errorListUpdate(int id)
        {
            List<PlcInfoSimple> plst = new List<PlcInfoSimple>();

            if (id >= 0 && id < AllPlcSimpleLst.Count)
             plst = AllPlcSimpleLst[id];

            if (plst.Count < 1) return;

            foreach (PlcInfoSimple p in plst)
            {
                if (p.Name.Contains(Constant.Alarm) && p.ShowStr != null && p.ShowStr.Count > 0)
                {
                    for (int i = 0; i < p.ShowStr.Count; i++)
                    {
                        int index = errorList.IndexOf(p.ShowStr[i]);

                        if (p.ShowValue == Constant.M_ON && index < 0)
                        {
                            errorList.Add(p.ShowStr[i]);
                        }
                        if (p.ShowValue == Constant.M_OFF && index > -1 && index < errorList.Count)
                        {
                            errorList.RemoveAt(index);
                        }
                    }

                }
              
                  if (p.Name.Contains(Constant.Warning) && p.ShowStr != null && p.ShowStr.Count > 0)
                    {
                        for (int i = 0; i < p.ShowStr.Count; i++)
                        {
                            int index = warningList.IndexOf(p.ShowStr[i]);
                            if (p.ShowValue == Constant.M_ON && index < 0)
                            {
                               warningList.Add(p.ShowStr[i]);
                            }
                            if (p.ShowValue == Constant.M_OFF && index > 0 && index < p.ShowStr.Count)
                            {
                              warningList.RemoveAt(index);
                            }
                        }
                    }               
            }
        }

        RestMaterial rsm;
        public xjplc.simi.RestMaterial Rsm
        {
            get { return rsm; }
            set
            {
                rsm = value;
                optSize.Rsm = value;
            }
        }


        public EvokXJWork(int id)
        {
            deviceName = "";
            currentPageId = -1;
            minPrintSize = 0;
            minPrinterName = "";
            cutSelMode = 0;
            printBarCodeMode = 0;
            mRunFlag = false;
            oldPrintBarCodeMode = Constant.NoPrintBarCode;
            printMiniSizeOutInPs = new PlcInfoSimple("��ӡ��С�ߴ��д");
            wlMiniSizeOutInPs = new PlcInfoSimple("β�ϳߴ����ƶ�д");
            autoMesOutInPs = new PlcInfoSimple("�Զ��ⳤ��־��д");
            dbcOutInPs = new PlcInfoSimple("��������д");
            ltbcOutInPs = new PlcInfoSimple("��ͷ������д");
            ltbcDefaultOutInPs = new PlcInfoSimple("��ͷ�̶�������д");
            safeOutInPs = new PlcInfoSimple("��ȫ�����д");
            prodOutInPs = new PlcInfoSimple("�ܲ�����д");
            noSizeToCutOutInPs = new PlcInfoSimple("��ƥ���д");
            ldsOutInPs = new PlcInfoSimple("�϶�����д");
            lcOutInPs = new PlcInfoSimple("�ϳ���д");
            lkOutInPs = new PlcInfoSimple("�Ͽ��д");
            yljxOutInPs = new PlcInfoSimple("Ԥ����϶��д");
            stopOutPs = new PlcInfoSimple("ֹͣд");
            stopInPs = new PlcInfoSimple("ֹͣ��");
            cutDoneOutInPs = new PlcInfoSimple("�и���϶�д");
            plcHandlebarCodeOutInPs = new PlcInfoSimple("�����ӡ��д");
            startCountInOutPs = new PlcInfoSimple("��ʼ������д");
            wlInOutPs = new PlcInfoSimple("β�϶�д");
            wlExistOutInPs = new PlcInfoSimple("H0��д");
            wlWidthOutInPs = new PlcInfoSimple("H100��д");
            wlHeightOutInPs = new PlcInfoSimple("H101��д");
            lliaoOutInPs = new PlcInfoSimple("���Ͽ��ض�д");
            widthCountInOutPs = new PlcInfoSimple("�ߴ�������д");
            heightCountInOutPs = new PlcInfoSimple("�ߴ糤������д");
            modeSelectOutInPs = new PlcInfoSimple("ģʽѡ���д");
            deviceStatusOutInPs = new PlcInfoSimple("�豸״̬��д");
            lineStartTipInPs = new PlcInfoSimple("�������Ѷ�");
            lineStartOutPs = new PlcInfoSimple("������д");
            lineStartInPs = new PlcInfoSimple("��������");
            lineResetOutPs = new PlcInfoSimple("�߸�λд");
            lineResetInPs = new PlcInfoSimple("�߸�λ��");
            lineStopOutPs = new PlcInfoSimple("��ֹͣд");
            lineStopInPs = new PlcInfoSimple("��ֹͣ��");
            linePauseOutPs = new PlcInfoSimple("����ͣд");
            linePauseInPs = new PlcInfoSimple("����ͣ��");
            doorSize = new PlcInfoSimple("���Ͼ��ϳ���д");
            doorBanLen = new PlcInfoSimple("�ݺ���ϳ���д");
            doorBanWidth = new PlcInfoSimple("�ݺ���Ͽ��д");
            doorShellLen = new PlcInfoSimple("��Ƥ���ϳ���д");
            doorShellWidth = new PlcInfoSimple("��Ƥ���Ͽ��д");
            doorDownLoadCountInOutPs = new PlcInfoSimple("ÿ���·�������д");
            xialiaojuOutPs = new PlcInfoSimple("���Ͼⳤ�ȶ�д");
            xialiaoDataRequestInPs = new PlcInfoSimple("���Ͼ��������ݶ�");
            xialiaoDataDownLoadSuccessOutPs = new PlcInfoSimple("���Ͼ��·��ɹ�д");
            xialiaoCurrentDoorIdInPs = new PlcInfoSimple("���Ͼ⵱ǰ�źŶ�");
            xialiaoCurrentStatusInPs = new PlcInfoSimple("���Ͼ�״̬��");
            xialiaoDoorCountInOutPs = new PlcInfoSimple("���Ͼ��·�����������д");
            xialiaoDoorCntInPs = new PlcInfoSimple("���Ͼ������");
            doorbanLenthChangInOutPs = new PlcInfoSimple("�Ű�ⳤ�ȶ�д");
            doorbanLenthKuanInOutPs = new PlcInfoSimple("�Ű���ȶ�д");
            doorbanDataKuanRequestInPs = new PlcInfoSimple("�Ű����������ݶ�");
            doorbanDataChangRequestInPs = new PlcInfoSimple("�Ű���������ݶ�");
            doorbanDataKuanDownLoadSuccessOutPs = new PlcInfoSimple("�Ű����·��ɹ�д");
            doorbanDataChangDownLoadSuccessOutPs = new PlcInfoSimple("�Ű�ⳤ�·��ɹ�д");
            doorbanCurrentDoorIdInPs = new PlcInfoSimple("�Ű�⵱ǰ�źŶ�");
            doorbanCurrentStatusInPs = new PlcInfoSimple("�Ű��״̬��");
            doorbanDoorCountInOutPs = new PlcInfoSimple("�Ű���·�����������д");
            doorshellLenthChangInOutPs = new PlcInfoSimple("��Ƥ�ⳤ�ȶ�д");
            doorshellLenthKuanInOutPs = new PlcInfoSimple("��Ƥ���ȶ�д");
            doorshellDataKuanRequestInPs = new PlcInfoSimple("��Ƥ����������ݶ�");
            doorshellDataKuanDownLoadSuccessOutPs = new PlcInfoSimple("��Ƥ����·��ɹ�д");
            doorshellDataChangRequestInPs = new PlcInfoSimple("��Ƥ���������ݶ�");
            doorshellDataChangDownLoadSuccessOutPs = new PlcInfoSimple("��Ƥ�ⳤ�·��ɹ�д");
            doorshellCurrentDoorIdInPs = new PlcInfoSimple("��Ƥ�⵱ǰ�źŶ�");
            doorshellCurrentStatusInPs = new PlcInfoSimple("��Ƥ��״̬��");
            doorshellDoorIdInOutPs = new PlcInfoSimple("��Ƥ����Id��д");
            doorshellDataDoorIdRequestInPs = new PlcInfoSimple("��Ƥ����Id�������ݶ�");
            doorshellDataDoorIdDownLoadSuccessOutPs = new PlcInfoSimple("��Ƥ����Id�·��ɹ�д");
            doorshellDoorCountInOutPs = new PlcInfoSimple("��Ƥ���·�����������д");
            lineEmgStopInPs = new PlcInfoSimple("�߼�ͣ��");
            lineEmgStopOutPs = new PlcInfoSimple("�߼�ͣд");
            dataOutPs = new PlcInfoSimple("�������ض�д");
            muxiaoHoleOutPs = new PlcInfoSimple("ľ��������д");
            mxkStringShowInPs = new PlcInfoSimple("����ľ����ѡ����ʾ��");
            zxShowInPs = new PlcInfoSimple("����ľм��");
            zkShowInPs = new PlcInfoSimple("�п�˫ľм��");
            smxShowInPs = new PlcInfoSimple("˫ľм��");
            mxkShowInPs = new PlcInfoSimple("ľ�ҿ�ѡ���");
            leftRightShowInPs = new PlcInfoSimple("����ģʽ��");
            liWaiInPs = new PlcInfoSimple("����ģʽ��");
            zxShowOutPs = new PlcInfoSimple("����ľмд");
            zkShowOutPs = new PlcInfoSimple("�п�˫ľмд");
            smxShowOutPs = new PlcInfoSimple("˫ľмд");
            mxkShowOutPs = new PlcInfoSimple("ľ�ҿ�ѡ��д");
            leftRightShowOutPs = new PlcInfoSimple("����ģʽд");
            liWaiOutPs = new PlcInfoSimple("����ģʽд");
            widht1InOutPs = new PlcInfoSimple("�Ͽ�1��д");
            widht2InOutPs = new PlcInfoSimple("�Ͽ�2��д");
            restMaterialRangeInOutPs = new PlcInfoSimple("�������ö�д");
            inspectPatternModeInOutPs = new PlcInfoSimple("����ģʽ��д");
            inspectPatternDoneInOutPs = new PlcInfoSimple("���Ʋ�����ɶ�д");
            inspectPatternPosInOutPs = new PlcInfoSimple("���ƾ������ö�д");
            materialSetEnInOutPs = new PlcInfoSimple("��������ʹ�ܶ�д");
            materialIdInOutPs = new PlcInfoSimple("���ϴ��Ŷ�д");
            pos1OutInPs = new PlcInfoSimple("����1���ȶ�д");
            pos2OutInPs = new PlcInfoSimple("����2���ȶ�д");
            pos1EnOutPs = new PlcInfoSimple("����1ʹ��д");
            pos2EnOutPs = new PlcInfoSimple("����2ʹ��д");
            pos1EnInPs = new PlcInfoSimple("����1ʹ�ܶ�");
            pos2EnInPs = new PlcInfoSimple("����2ʹ�ܶ�");
            posMode = new PlcInfoSimple("ģʽѡ��λ�ö�д");
            pauseOutPs = new PlcInfoSimple("��ͣд");
            startOutPs = new PlcInfoSimple("����д");
            resetOutPs = new PlcInfoSimple("��λд");
            scarModeOutPs = new PlcInfoSimple("��̲���д");
            pageShiftOutPs = new PlcInfoSimple("ҳ���л�д");
            pressOutInPs = new PlcInfoSimple("ѹ�϶�д");
            doorCutCntOutInPs = new PlcInfoSimple("�ŵ�����д");
            dataNotEnoughOutInPs = new PlcInfoSimple("���ݴﵽ��λ��д");
            dataNotEnoughValueOutInPs = new PlcInfoSimple("������λֵ��д");
            gjOutInPs = new PlcInfoSimple("������д");
            GWOutInPs = new PlcInfoSimple("��λ��д");
            PJOutPs = new PlcInfoSimple("�罺д");
            PJInPs = new PlcInfoSimple("�罺��");
            ZQInPs = new PlcInfoSimple("���ж�");
            KSInPs = new PlcInfoSimple("��դ��");
            AutoPosDataOutInPs = new PlcInfoSimple("�Զ����ȶ�д");
            LMInPs = new PlcInfoSimple("���е���");
            LMSLInPs = new PlcInfoSimple("�������϶�");
            doorTypeCutCountOutInPs = new PlcInfoSimple("���͵�����д");
            sfslwInPs = new PlcInfoSimple("�ŷ�����λ��");
            emgStopInPs = new PlcInfoSimple("��ͣ��");
            emgStopOutPs = new PlcInfoSimple("��ͣд");
            startInPs = new PlcInfoSimple("������");
            resetInPs = new PlcInfoSimple("��λ��");
            pauseInPs = new PlcInfoSimple("��ͣ��");
            scarModeInPs = new PlcInfoSimple("��̲�����");
            scarInPs = new PlcInfoSimple("��̶�");
            autoCCInPs = new PlcInfoSimple("�Զ��ⳤ��");
            clInPs = new PlcInfoSimple("���϶�");
            slInPs = new PlcInfoSimple("���϶�");
            xialiaojuStatus0 = new string[] { "������", "׼������", "��ͣ��", "��ͣ��", "������", "ͨѶ����", "��λ��" };
            doorBanStatus0 = new string[] { "������", "׼������", "��ͣ��", "��ͣ��", "������", "ͨѶ����", "��λ��" };
            doorShellStatus0 = new string[] { "������", "׼������", "��ͣ��", "��ͣ��", "������", "ͨѶ����", "��λ��" };
            comIsDownLoading = false;
            downLoadSizeId = 0;
            pSize = new patternSize();
            tCheckPrint = new System.Timers.Timer(1000.0);
            CutProCnt = 0;
            CurrentDoorType = "123456789";
            PortParam param = new PortParam();
            List<string> strfile = new List<string>();
            switch (id)
            {
                case 0:
                    param = ConstantMethod.LoadPortParam(Constant.ConfigSerialportFilePath);
                    strfile.Add(Constant.PlcDataFilePathAuto);
                    strfile.Add(Constant.PlcDataFilePathHand);
                    strfile.Add(Constant.PlcDataFilePathParam);
                    strfile.Add(Constant.PlcDataFilePathIO);
                    evokDevice = new EvokXJDevice(strfile, param);
                    break;

                case 1:
                    for (int i = strfile.Count - 1; i >= 0; i--)
                    {
                        if (!File.Exists(strfile[i]))
                        {
                            strfile.RemoveAt(i);
                            MessageBox.Show(strfile[i] + Constant.ErrorPlcFile);
                            Environment.Exit(0);
                        }
                    }
                    evokDevice = new EvokXJDevice(strfile, param);
                    break;

                case 2:
                    {
                        strfile.Add(Constant.PlcDataFilePathAuto);
                        strfile.Add(Constant.PlcDataFilePathHand);
                        strfile.Add(Constant.PlcDataFilePathParam);
                        strfile.Add(Constant.PlcDataFilePathIO);
                        ServerInfo info = new ServerInfo();
                        info = ConstantMethod.LoadServerParam(Constant.ConfigServerPortFilePath);
                        evokDevice = new EvokXJDevice(strfile, info);
                        break;
                    }
            }
            InitUsualTest();
        }

        public EvokXJWork(List<string> strDataFormPath, PortParam p0)
        {
            deviceName = "";
            currentPageId = -1;
            minPrintSize = 0;
            minPrinterName = "";
            cutSelMode = 0;
            printBarCodeMode = 0;
            mRunFlag = false;
            oldPrintBarCodeMode = Constant.NoPrintBarCode;
            printMiniSizeOutInPs = new PlcInfoSimple("��ӡ��С�ߴ��д");
            wlMiniSizeOutInPs = new PlcInfoSimple("β�ϳߴ����ƶ�д");
            autoMesOutInPs = new PlcInfoSimple("�Զ��ⳤ��־��д");
            dbcOutInPs = new PlcInfoSimple("��������д");
            ltbcOutInPs = new PlcInfoSimple("��ͷ������д");
            ltbcDefaultOutInPs = new PlcInfoSimple("��ͷ�̶�������д");
            safeOutInPs = new PlcInfoSimple("��ȫ�����д");
            prodOutInPs = new PlcInfoSimple("�ܲ�����д");
            noSizeToCutOutInPs = new PlcInfoSimple("��ƥ���д");
            ldsOutInPs = new PlcInfoSimple("�϶�����д");
            lcOutInPs = new PlcInfoSimple("�ϳ���д");
            lkOutInPs = new PlcInfoSimple("�Ͽ��д");
            yljxOutInPs = new PlcInfoSimple("Ԥ����϶��д");
            stopOutPs = new PlcInfoSimple("ֹͣд");
            stopInPs = new PlcInfoSimple("ֹͣ��");
            cutDoneOutInPs = new PlcInfoSimple("�и���϶�д");
            plcHandlebarCodeOutInPs = new PlcInfoSimple("�����ӡ��д");
            startCountInOutPs = new PlcInfoSimple("��ʼ������д");
            wlInOutPs = new PlcInfoSimple("β�϶�д");
            wlExistOutInPs = new PlcInfoSimple("H0��д");
            wlWidthOutInPs = new PlcInfoSimple("H100��д");
            wlHeightOutInPs = new PlcInfoSimple("H101��д");
            lliaoOutInPs = new PlcInfoSimple("���Ͽ��ض�д");
            widthCountInOutPs = new PlcInfoSimple("�ߴ�������д");
            heightCountInOutPs = new PlcInfoSimple("�ߴ糤������д");
            modeSelectOutInPs = new PlcInfoSimple("ģʽѡ���д");
            deviceStatusOutInPs = new PlcInfoSimple("�豸״̬��д");
            lineStartTipInPs = new PlcInfoSimple("�������Ѷ�");
            lineStartOutPs = new PlcInfoSimple("������д");
            lineStartInPs = new PlcInfoSimple("��������");
            lineResetOutPs = new PlcInfoSimple("�߸�λд");
            lineResetInPs = new PlcInfoSimple("�߸�λ��");
            lineStopOutPs = new PlcInfoSimple("��ֹͣд");
            lineStopInPs = new PlcInfoSimple("��ֹͣ��");
            linePauseOutPs = new PlcInfoSimple("����ͣд");
            linePauseInPs = new PlcInfoSimple("����ͣ��");
            doorSize = new PlcInfoSimple("���Ͼ��ϳ���д");
            doorBanLen = new PlcInfoSimple("�ݺ���ϳ���д");
            doorBanWidth = new PlcInfoSimple("�ݺ���Ͽ��д");
            doorShellLen = new PlcInfoSimple("��Ƥ���ϳ���д");
            doorShellWidth = new PlcInfoSimple("��Ƥ���Ͽ��д");
            doorDownLoadCountInOutPs = new PlcInfoSimple("ÿ���·�������д");
            xialiaojuOutPs = new PlcInfoSimple("���Ͼⳤ�ȶ�д");
            xialiaoDataRequestInPs = new PlcInfoSimple("���Ͼ��������ݶ�");
            xialiaoDataDownLoadSuccessOutPs = new PlcInfoSimple("���Ͼ��·��ɹ�д");
            xialiaoCurrentDoorIdInPs = new PlcInfoSimple("���Ͼ⵱ǰ�źŶ�");
            xialiaoCurrentStatusInPs = new PlcInfoSimple("���Ͼ�״̬��");
            xialiaoDoorCountInOutPs = new PlcInfoSimple("���Ͼ��·�����������д");
            xialiaoDoorCntInPs = new PlcInfoSimple("���Ͼ������");
            doorbanLenthChangInOutPs = new PlcInfoSimple("�Ű�ⳤ�ȶ�д");
            doorbanLenthKuanInOutPs = new PlcInfoSimple("�Ű���ȶ�д");
            doorbanDataKuanRequestInPs = new PlcInfoSimple("�Ű����������ݶ�");
            doorbanDataChangRequestInPs = new PlcInfoSimple("�Ű���������ݶ�");
            doorbanDataKuanDownLoadSuccessOutPs = new PlcInfoSimple("�Ű����·��ɹ�д");
            doorbanDataChangDownLoadSuccessOutPs = new PlcInfoSimple("�Ű�ⳤ�·��ɹ�д");
            doorbanCurrentDoorIdInPs = new PlcInfoSimple("�Ű�⵱ǰ�źŶ�");
            doorbanCurrentStatusInPs = new PlcInfoSimple("�Ű��״̬��");
            doorbanDoorCountInOutPs = new PlcInfoSimple("�Ű���·�����������д");
            doorshellLenthChangInOutPs = new PlcInfoSimple("��Ƥ�ⳤ�ȶ�д");
            doorshellLenthKuanInOutPs = new PlcInfoSimple("��Ƥ���ȶ�д");
            doorshellDataKuanRequestInPs = new PlcInfoSimple("��Ƥ����������ݶ�");
            doorshellDataKuanDownLoadSuccessOutPs = new PlcInfoSimple("��Ƥ����·��ɹ�д");
            doorshellDataChangRequestInPs = new PlcInfoSimple("��Ƥ���������ݶ�");
            doorshellDataChangDownLoadSuccessOutPs = new PlcInfoSimple("��Ƥ�ⳤ�·��ɹ�д");
            doorshellCurrentDoorIdInPs = new PlcInfoSimple("��Ƥ�⵱ǰ�źŶ�");
            doorshellCurrentStatusInPs = new PlcInfoSimple("��Ƥ��״̬��");
            doorshellDoorIdInOutPs = new PlcInfoSimple("��Ƥ����Id��д");
            doorshellDataDoorIdRequestInPs = new PlcInfoSimple("��Ƥ����Id�������ݶ�");
            doorshellDataDoorIdDownLoadSuccessOutPs = new PlcInfoSimple("��Ƥ����Id�·��ɹ�д");
            doorshellDoorCountInOutPs = new PlcInfoSimple("��Ƥ���·�����������д");
            lineEmgStopInPs = new PlcInfoSimple("�߼�ͣ��");
            lineEmgStopOutPs = new PlcInfoSimple("�߼�ͣд");
            dataOutPs = new PlcInfoSimple("�������ض�д");
            muxiaoHoleOutPs = new PlcInfoSimple("ľ��������д");
            mxkStringShowInPs = new PlcInfoSimple("����ľ����ѡ����ʾ��");
            zxShowInPs = new PlcInfoSimple("����ľм��");
            zkShowInPs = new PlcInfoSimple("�п�˫ľм��");
            smxShowInPs = new PlcInfoSimple("˫ľм��");
            mxkShowInPs = new PlcInfoSimple("ľ�ҿ�ѡ���");
            leftRightShowInPs = new PlcInfoSimple("����ģʽ��");
            liWaiInPs = new PlcInfoSimple("����ģʽ��");
            zxShowOutPs = new PlcInfoSimple("����ľмд");
            zkShowOutPs = new PlcInfoSimple("�п�˫ľмд");
            smxShowOutPs = new PlcInfoSimple("˫ľмд");
            mxkShowOutPs = new PlcInfoSimple("ľ�ҿ�ѡ��д");
            leftRightShowOutPs = new PlcInfoSimple("����ģʽд");
            liWaiOutPs = new PlcInfoSimple("����ģʽд");
            widht1InOutPs = new PlcInfoSimple("�Ͽ�1��д");
            widht2InOutPs = new PlcInfoSimple("�Ͽ�2��д");
            restMaterialRangeInOutPs = new PlcInfoSimple("�������ö�д");
            inspectPatternModeInOutPs = new PlcInfoSimple("����ģʽ��д");
            inspectPatternDoneInOutPs = new PlcInfoSimple("���Ʋ�����ɶ�д");
            inspectPatternPosInOutPs = new PlcInfoSimple("���ƾ������ö�д");
            materialSetEnInOutPs = new PlcInfoSimple("��������ʹ�ܶ�д");
            materialIdInOutPs = new PlcInfoSimple("���ϴ��Ŷ�д");
            pos1OutInPs = new PlcInfoSimple("����1���ȶ�д");
            pos2OutInPs = new PlcInfoSimple("����2���ȶ�д");
            pos1EnOutPs = new PlcInfoSimple("����1ʹ��д");
            pos2EnOutPs = new PlcInfoSimple("����2ʹ��д");
            pos1EnInPs = new PlcInfoSimple("����1ʹ�ܶ�");
            pos2EnInPs = new PlcInfoSimple("����2ʹ�ܶ�");
            posMode = new PlcInfoSimple("ģʽѡ��λ�ö�д");
            pauseOutPs = new PlcInfoSimple("��ͣд");
            startOutPs = new PlcInfoSimple("����д");
            resetOutPs = new PlcInfoSimple("��λд");
            scarModeOutPs = new PlcInfoSimple("��̲���д");
            pageShiftOutPs = new PlcInfoSimple("ҳ���л�д");
            pressOutInPs = new PlcInfoSimple("ѹ�϶�д");
            doorCutCntOutInPs = new PlcInfoSimple("�ŵ�����д");
            dataNotEnoughOutInPs = new PlcInfoSimple("���ݴﵽ��λ��д");
            dataNotEnoughValueOutInPs = new PlcInfoSimple("������λֵ��д");
            gjOutInPs = new PlcInfoSimple("������д");
            GWOutInPs = new PlcInfoSimple("��λ��д");
            PJOutPs = new PlcInfoSimple("�罺д");
            PJInPs = new PlcInfoSimple("�罺��");
            ZQInPs = new PlcInfoSimple("���ж�");
            KSInPs = new PlcInfoSimple("��դ��");
            LMInPs = new PlcInfoSimple("���е���");
            LMSLInPs = new PlcInfoSimple("�������϶�");
            doorTypeCutCountOutInPs = new PlcInfoSimple("���͵�����д");
            sfslwInPs = new PlcInfoSimple("�ŷ�����λ��");
            emgStopInPs = new PlcInfoSimple("��ͣ��");
            emgStopOutPs = new PlcInfoSimple("��ͣд");
            startInPs = new PlcInfoSimple("������");
            resetInPs = new PlcInfoSimple("��λ��");
            pauseInPs = new PlcInfoSimple("��ͣ��");
            scarModeInPs = new PlcInfoSimple("��̲�����");
            scarInPs = new PlcInfoSimple("��̶�");
            autoCCInPs = new PlcInfoSimple("�Զ��ⳤ��");
            clInPs = new PlcInfoSimple("���϶�");
            slInPs = new PlcInfoSimple("���϶�");
            xialiaojuStatus0 = new string[] { "������", "׼������", "��ͣ��", "��ͣ��", "������", "ͨѶ����", "��λ��" };
            doorBanStatus0 = new string[] { "������", "׼������", "��ͣ��", "��ͣ��", "������", "ͨѶ����", "��λ��" };
            doorShellStatus0 = new string[] { "������", "׼������", "��ͣ��", "��ͣ��", "������", "ͨѶ����", "��λ��" };
            comIsDownLoading = false;
            downLoadSizeId = 0;
            pSize = new patternSize();
            tCheckPrint = new System.Timers.Timer(1000.0);
            CutProCnt = 0;
            CurrentDoorType = "123456789";
            AutoPosDataOutInPs= new PlcInfoSimple("�Զ����ȶ�д");
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
            if (filestr.Count > 0)
            {
                evokDevice.GetPlcDataTableFromFile(filestr);
            }
            for (int i = 4; i < evokDevice.DataFormLst.Count; i++)
            {
                SetPage(i);
                psLstHand.AddRange(AllPlcSimpleLst[i]);
            }
        }

        public void addDataForm(List<string> filestr, int id)
        {
            if (DeviceName.Equals(Constant.scjDeivceName) || DeviceName.Equals(Constant.sdjDeivceName))
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
            if (filestr.Count > 0)
            {
                evokDevice.GetPlcDataTableFromFile(filestr);
            }
            for (int i = 4; i < evokDevice.DataFormLst.Count; i++)
            {
                SetPage(i);
                AllPlcSimpleLst[id].AddRange(AllPlcSimpleLst[i]);
            }
        }       

        public void autoMesOFF()
        {
            evokDevice.SetMValueON(autoMesOutInPs);
        }

        public void autoMesON()
        {
            evokDevice.SetMValueOFF(autoMesOutInPs);
        }

        public bool AutoParamTxt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                double num;
                if (double.TryParse(((TextBox)sender).Text, out num) && (num > -1.0))
                {
                    num *= Constant.dataMultiple;
                    SetDValue(((TextBox)sender).Tag.ToString(), Constant.Write, PsLstAuto, (int)num);
                }
                return true;
            }
            return false;
        }

        public bool AutoParamTxt_KeyPressWithRatio(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                double num;
                if (double.TryParse(((TextBox)sender).Text, out num) && (num > -1.0))
                {
                    SetDValue(((TextBox)sender).Tag.ToString(), Constant.Write, PsLstAuto, ((TextBox)sender).Text);
                }
                return true;
            }
            return false;
        }

        public bool AutoParamTxt_KeyPressWithRatioWithId(object sender, KeyPressEventArgs e, int id)
        {
            if (e.KeyChar == '\r')
            {
                double num;
                List<PlcInfoSimple> pLst = new List<PlcInfoSimple>();
                if ((id < AllPlcSimpleLst.Count<List<PlcInfoSimple>>()) && (id >= 0))
                {
                    pLst = AllPlcSimpleLst[id];
                }
                else
                {
                    return false;
                }
                if (double.TryParse(((TextBox)sender).Text, out num) && (num > -1.0))
                {
                    SetDValue(((TextBox)sender).Tag.ToString(), Constant.Write, pLst, ((TextBox)sender).Text);
                }
                return true;
            }
            return false;
        }

        public void ChangeCutMode(int value)
        {
            ParamFile.WriteConfig(Constant.cutSelMode, value.ToString());
            cutSelMode = value;
        }

        public void ChangePrintMode(int value)
        {
            if (PrinterSettings.InstalledPrinters.Count == 0)
            {
                value = 0;
                string[] logs = new string[] { DeviceName + Constant.DeviceNoPrinter };
                LogManager.WriteProgramLog(logs);
            }
            ParamFile.WriteConfig(Constant.printBarcodeMode, value.ToString());
            OldPrintBarCodeMode = PrintBarCodeMode;
            printBarCodeMode = value;
            if (printBarCodeMode == Constant.AutoBarCode)
            {
                if (plcHandlebarCodeOutInPs !=null)
                {
                    evokDevice.SetMValueON(plcHandlebarCodeOutInPs);
                }
            }
            else if (plcHandlebarCodeOutInPs !=null)
            {
                evokDevice.SetMValueOFF(plcHandlebarCodeOutInPs);
            }
            string s = "0";
            s = ParamFile.ReadConfig(Constant.IsSaveProdLog);
            int result = 0;
            if (int.TryParse(s, out result))
            {
                if (result == 0)
                {
                    IsSaveProdLog = false;
                }
                else
                {
                    IsSaveProdLog = true;
                }
            }
        }

        public void ChangtToAuto()
        {
            evokDevice.SetDValue(pageShiftOutPs, Constant.AutoPageID);
        }

        private void CheckIsDataNotEnough(int id)
        {
            int num = 0;
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
                        num += optSize.ProdInfoLst[i].Cut.Count;
                    }
                }
                if (num < dataNotEnoughValueOutInPs.ShowValue)
                {
                    OpenDataNotEnough();
                }
                else
                {
                    CloseDataNotEnough();
                }
            }
        }

        public void CloseDataNotEnough()
        {
            if (dataNotEnoughOutInPs !=null)
            {
                evokDevice.SetMValueOFF(dataNotEnoughOutInPs);
            }
        }

        private void collectDeviceData()
        {
            SetSimiReady();

            if (lcOutInPs.Ration > 1.0)
            {
                if ((DeviceName == null) || !DeviceName.Equals(Constant.scjDeivceName))
                {
                    optSize.Len = lcOutInPs.ShowValue * Constant.dataMultiple;
                }
            }
            else
            {
                optSize.Len = lcOutInPs.ShowValue;
            }
            if (DeviceProperty == 1)
            {
                optSize.Len = lcOutInPs.ShowValue + 0x1388;
            }
            if (dbcOutInPs !=null)
            {
                if ((dbcOutInPs != null) && (dbcOutInPs.Ration > 1.0))
                {
                    optSize.Dbc = (int)(dbcOutInPs.ShowValueDouble * Constant.dataMultiple);
                }
                else
                {
                    optSize.Dbc = dbcOutInPs.ShowValue;
                }
            }
            if (ltbcOutInPs !=null)
            {
                if (ltbcOutInPs.Ration > 1.0)
                {
                    optSize.Ltbc = (int)(ltbcOutInPs.ShowValueDouble * Constant.dataMultiple);
                }
                else
                {
                    optSize.Ltbc = ltbcOutInPs.ShowValue;
                }
            }
            if (safeOutInPs !=null)
            {
                if (safeOutInPs.Ration > 1.0)
                {
                    if (DeviceName.Equals(Constant.simiDeivceName))
                    {
                        optSize.Safe = ((int)((safeOutInPs.ShowValueDouble * Constant.dataMultiple) + (optSize.Dbc * 15)));
                    }
                    else
                    {
                        optSize.Safe = (int)(safeOutInPs.ShowValueDouble * Constant.dataMultiple);
                    }
                }
                else
                {
                    optSize.Safe = safeOutInPs.ShowValue;
                }
            }
            if (wlMiniSizeOutInPs !=null)
            {
                if (wlMiniSizeOutInPs.Ration > 1.0)
                {
                    optSize.WlMiniValue = (int)(wlMiniSizeOutInPs.ShowValueDouble * Constant.dataMultiple);
                }
                else
                {
                    optSize.WlMiniValue = wlMiniSizeOutInPs.ShowValue;
                }
            }
        }

        private void collectUserInputData()
        {

            new UserDataInputForm { Op = optSize,Eok=this }.ShowDialog();
        }

        private void CountClr()
        {
            for (int i = 0; !evokDevice.SetDValue(cutDoneOutInPs, 0) && (i < 2); i++)
            {
                Application.DoEvents();
            }
        }
        private void CountClr(int id)
        {
            for (int i = 0; !evokDevice.SetDValue(cutDoneOutInPs, id) && (i < 2); i++)
            {
                Application.DoEvents();
            }
        }
        public void currentIdUpdate(DataGridView dgv, int doorId, int deviceid, int startrow, OptSize op)
        {
            if ((((deviceid != 0) || getXialiaoJuStatus().Equals(xialiaojuStatus0[0])) && ((doorId >= 1) && (doorId <= dgv.Rows.Count))) && (currentDoorSizeCount != xialiaoDoorCntInPs.ShowValue))
            {
                if ((xialiaoDoorCntInPs.ShowValue < 1) || (xialiaoDoorCntInPs.ShowValue > 5))
                {
                    currentDoorSizeCount = 0;
                }
                else
                {
                    currentDoorSizeCount = xialiaoDoorCntInPs.ShowValue;
                    int num = 0;
                    foreach (DataRow row in op.DtData.Rows)
                    {
                        int result = 0;
                        if ((row[13] != null) && int.TryParse(row[13].ToString(), out result))
                        {
                            string str = row[2].ToString();
                            string str2 = row[1].ToString();
                            if ((result == doorId) && !str.Equals(str2))
                            {
                                row[2] = row[1];
                                dgv.Rows[num].DefaultCellStyle.BackColor = Color.Green;
                                if ((num + 4) < dgv.Rows.Count)
                                {
                                    dgv.CurrentCell = dgv.Rows[num + 4].Cells[0];
                                }
                                else
                                {
                                    dgv.CurrentCell = dgv.Rows[num].Cells[0];
                                }
                                break;
                            }
                        }
                        num++;
                    }
                }
            }
        }

        public void currentIdUpdateBanAndShell(DataGridView dgv, int doorId, int deviceid, int idcount, OptSize op)
        {
            if ((((deviceid != 1) || getDoorBanStatus().Equals(xialiaojuStatus0[0])) && ((deviceid != 2) || getDoorShellStatus().Equals(xialiaojuStatus0[0]))) && (((deviceid != 0) || getXialiaoJuStatus().Equals(xialiaojuStatus0[0])) && ((doorId >= 1) && (doorId <= dgv.Rows.Count))))
            {
                int num = 0;
                foreach (DataRow row in op.DtData.Rows)
                {
                    int result = 0;
                    if ((row[13] != null) && int.TryParse(row[13].ToString(), out result))
                    {
                        string str = row[2].ToString();
                        string str2 = row[1].ToString();
                        if ((result <= doorId) && !str.Equals(str2))
                        {
                            row[2] = row[1];
                            dgv.Rows[num].DefaultCellStyle.BackColor = Color.Green;
                            if ((num + 4) < dgv.Rows.Count)
                            {
                                dgv.CurrentCell = dgv.Rows[num + 4].Cells[0];
                            }
                            else
                            {
                                dgv.CurrentCell = dgv.Rows[num].Cells[0];
                            }
                        }
                        else if (result > doorId)
                        {
                            break;
                        }
                    }
                    num++;
                }
            }
        }

        public void CutDoorBanThread()
        {
            showWorkInfo("��������·�!");
            DownLoadDataWithDoorBanWidth(0);
            if (!startCutDoor(0))
            {
                MessageBox.Show(DeviceName + Constant.DeviceStartFailed);
            }
            else
            {
                showWorkInfo(" �����ɹ�!");
                if ((optSize.ProdInfoLst.Count > 0) && (CutProCnt < optSize.ProdInfoLst.Count))
                {
                    for (int i = CutProCnt; i < optSize.ProdInfoLst.Count; i++)
                    {
                        SaveProdDataLog(optSize.ProdInfoLst[i], i);
                        ConstantMethod.ShowInfo(rtbWork, Constant.resultTip5 + ((i + 1)).ToString() + Constant.startTips4);
                        DownLoadDataNormal(i);
                        CountClr();
                        CutLoop(i);
                    }
                }
                else
                {
                    MessageBox.Show(Constant.noData);
                }
            }
        }

        public void CutDoorShellThread()
        {
            showWorkInfo("׼�������·���");
            if ((optSize.ProdInfoLst.Count > 0) && (CutProCnt < optSize.ProdInfoLst.Count))
            {
                int cutProCnt = CutProCnt;
                while (cutProCnt < optSize.ProdInfoLst.Count)
                {
                    SaveProdDataLog(optSize.ProdInfoLst[cutProCnt], cutProCnt);
                    ConstantMethod.ShowInfo(rtbWork, Constant.resultTip5 + ((cutProCnt + 1)).ToString() + Constant.startTips4);
                    DownLoadDataWithDoorShell(cutProCnt);
                    showWorkInfo(Constant.startTips3);
                    if (!startCutDoor(0))
                    {
                        MessageBox.Show(DeviceName + Constant.DeviceStartFailed);
                    }
                    else
                    {
                        CountClr();
                        CutLoop(cutProCnt);
                    }
                    break;
                }
            }
            else
            {
                MessageBox.Show(Constant.noData);
            }
        }

        public bool CutDoorStartNormal(int cutid)
        {
            if (IsInNoSafe)
            {
                MessageBox.Show(DeviceName + Constant.emgStopTip);
                return false;
            }
            if (errorList.Count > 0)
            {
                MessageBox.Show(DeviceName + Constant.Alarm);
                return false;
            }
            if (optSize.ProdInfoLst.Count < 1)
            {
                MessageBox.Show(DeviceName + Constant.noData);
                return false;
            }
            string[] logs = new string[] { DeviceName + Constant.NormalMode };
            LogManager.WriteProgramLog(logs);
            try
            {
                SelectCutThread(cutid);
                if (!CutThread.IsAlive)
                {
                    CutThread.Start();
                }
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
                MessageBox.Show(DeviceName + Constant.CutEnd);
            }
            return true;
        }
        void SimiUpdateData(
            string WorkUnitType, 
            string Barcode,
            string WorkPlace,
            string PDTRStatus,
            bool ProcessingStatus
            )
        {
            try
            {
                string datestr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                SqlCommand com = lo_conn.CreateCommand();
                /*****
                WorkUnitType 
                Barcode 
                DateTime 
                WorkPlace 
                PDTRStatus 
                ProcessingStatus
                ****/
                // com.CommandText = "insert into "+textBox5.Text+"(2, 3)"+" values(123, 123)";

                com.CommandText = "insert into " + simi_SQL_DataTableName +
                    " ( WorkUnitType, Barcode, DateTime, WorkPlace, PDTRStatus ,ProcessingStatus )"
                    + " values( '"
                    + WorkUnitType + "' , '" + Barcode
                    + "' , '" + datestr
                    + "' , '" + WorkPlace
                    + "' , '" + PDTRStatus
                    + "' , '" + ProcessingStatus.ToString() +
                    "')";

                // com.CommandText = "insert into " +"("+textBox5.Text + " values (2, 3,1,1,1,1,1)";

                com.ExecuteNonQuery();
                ConstantMethod.ShowInfo(rtbWork,"�����ɹ���");
            }
            catch (Exception ex)
            {
                ConstantMethod.ShowInfo(rtbWork, "����ʧ�ܣ�");
            
             }
            finally
            {
               
            }
        }
        private void SetAutoPosValue(int size)
        {

            List<int> sizeLst = new List<int>();

            sizeLst.Add(size);
            sizeLst.Add(1);
            double strd = ((double)size) / 1000;
            SetAutoPosTipLabelText(strd.ToString("0.00")+"mm");
            evokDevice.SetMultiPleDValue(AutoPosDataOutInPs, sizeLst.ToArray());
            
        }
        private int CutLoopWithAutoPos(int i)
        {
           
            if (optSize.SingleSizeLst[i].Count > 0)
            {
                PrintBarCheck(optSize.SingleSizeLst[i][0]);
            }

          
           int num = 0;
            bool First = true;
            while (RunFlag)
            {
                
                Application.DoEvents();

                Thread.Sleep(10);

                int showValue = cutDoneOutInPs.ShowValue;

                if (!RunFlag || IsInNoSafe)
                {
                    ConstantMethod.ShowInfo(rtbWork, Constant.emgStopTip);
                    string[] logs = new string[] { DeviceName + Constant.emgStopTip };
                    LogManager.WriteProgramLog(logs);
                    stopOperation();
                    return -1;
                }
                if (ErrorList.Count > 0)
                {
                    RunFlag = false;
                    stopOperation();
                    return -2;
                }

                if (showValue == 0 && num ==0 && First)
                {
                    SetAutoPosValue(optSize.ProdInfoLst[i].Cut[0] * 10);
                    First = false;
                }
                else
                if ((showValue>0)&&(showValue != num) && (num < optSize.ProdInfoLst[i].Cut.Count))
                {

                    if((num + 1)< optSize.ProdInfoLst[i].Cut.Count)
                    SetAutoPosValue(optSize.ProdInfoLst[i].Cut[num+1] * 10);

                    int num5;
                    int result = 0;
                    if (optSize.SingleSizeLst[i][num].DtUser != null)
                        if (!optSize.SingleSizeLst[i][num].Barc.Equals(Constant.ScarId)
                            &&
                            int.TryParse(optSize.SingleSizeLst[i][num].DtUser.Rows[optSize.SingleSizeLst[i][num].Xuhao][2].ToString(), out result))
                        {
                            result++;

                            int cntDoneCount = 0;
                            //�趨���� һ��Ҫ���ڵ�����������
                            if (int.TryParse(optSize.SingleSizeLst[i][num].DtUser.Rows[optSize.SingleSizeLst[i][num].Xuhao][1].ToString(), out cntDoneCount))
                                if (result <= cntDoneCount)
                                {
                                    optSize.SingleSizeLst[i][num].DtUser.Rows[optSize.SingleSizeLst[i][num].Xuhao][2] = result;


                                    Point point = new Point(optSize.SingleSizeLst[i][num].Xuhao, 2);
                                    optSize.checkIsDone(optSize.SingleSizeLst[i][num].Xuhao);
                                }
                        }

                    if (optSize.ProdInfoLst[i].Param1.Count > 0)
                    {
                        string[] textArray2 = new string[7];
                        textArray2[0] = Constant.resultTip5;
                        num5 = num + 1;
                        textArray2[1] = num5.ToString();
                        textArray2[2] = Constant.size;
                        num5 = optSize.ProdInfoLst[i].Cut[num];
                        textArray2[3] = num5.ToString();
                        textArray2[4] = Constant.startTips6;
                        textArray2[5] = optSize.ProdInfoLst[i].Param1[num].ToString();
                        textArray2[6] = Constant.startTips5;
                        ConstantMethod.ShowInfo(rtbWork, string.Concat(textArray2));
                    }
                    else
                    {
                        string[] textArray3 = new string[5];
                        textArray3[0] = Constant.resultTip5;
                        num5 = num + 1;
                        textArray3[1] = num5.ToString();
                        textArray3[2] = Constant.size;
                        textArray3[3] = optSize.ProdInfoLst[i].Cut[num].ToString();
                        textArray3[4] = Constant.startTips5;
                        ConstantMethod.ShowInfo(rtbWork, string.Concat(textArray3));
                    }
                   
                    num = showValue;

                    /***
                    if (showValue < optSize.SingleSizeLst[i].Count)
                    {                     
                        PrintBarCheck(optSize.SingleSizeLst[i][showValue]);
                    }
                    ****/

                }

                if (showValue >= optSize.ProdInfoLst[i].Cut.Count)
                {
                   
                    break;
                }
            }

            return 0;
        }

        private int CutLoop(int i)
        {
            if (optSize.SingleSizeLst[i].Count > 0)
            {
                PrintBarCheck(optSize.SingleSizeLst[i][0]);
            }

         
            int num = 0;

            while (RunFlag)
            {
                Application.DoEvents();

                Thread.Sleep(10);

                int showValue = cutDoneOutInPs.ShowValue;

                if (!RunFlag || IsInNoSafe)
                {
                    ConstantMethod.ShowInfo(rtbWork, Constant.emgStopTip);
                    string[] logs = new string[] { DeviceName + Constant.emgStopTip };
                    LogManager.WriteProgramLog(logs);
                    stopOperation();
                    return -1;
                }
                if (ErrorList.Count > 0)
                {
                    RunFlag = false;
                    stopOperation();
                    return -2;
                }
                
                if ((showValue != num) && (num < optSize.ProdInfoLst[i].Cut.Count))
                {
                    
                    int num5;
                    int result = 0;
                    if(optSize.SingleSizeLst[i][num].DtUser !=null)
                    if (!optSize.SingleSizeLst[i][num].Barc.Equals(Constant.ScarId) 
                        && 
                        int.TryParse(optSize.SingleSizeLst[i][num].DtUser.Rows[optSize.SingleSizeLst[i][num].Xuhao][2].ToString(), out result))
                    {
                        result++;

                        int cntDoneCount = 0;
                        //�趨���� һ��Ҫ���ڵ�����������
                        if (int.TryParse(optSize.SingleSizeLst[i][num].DtUser.Rows[optSize.SingleSizeLst[i][num].Xuhao][1].ToString(), out cntDoneCount))
                                if (result <= cntDoneCount)
                                {
                                    optSize.SingleSizeLst[i][num].DtUser.Rows[optSize.SingleSizeLst[i][num].Xuhao][2] = result;


                                    Point point = new Point(optSize.SingleSizeLst[i][num].Xuhao, 2);
                                    optSize.checkIsDone(optSize.SingleSizeLst[i][num].Xuhao);
                                }                     
                    }
                        
                    if (optSize.ProdInfoLst[i].Param1.Count > 0)
                    {
                        string[] textArray2 = new string[7];
                        textArray2[0] = Constant.resultTip5;
                        num5 = num + 1;
                        textArray2[1] = num5.ToString();
                        textArray2[2] = Constant.size;
                        num5 = optSize.ProdInfoLst[i].Cut[num];
                        textArray2[3] = num5.ToString();
                        textArray2[4] = Constant.startTips6;
                        textArray2[5] = optSize.ProdInfoLst[i].Param1[num].ToString();
                        textArray2[6] = Constant.startTips5;
                        ConstantMethod.ShowInfo(rtbWork, string.Concat(textArray2));
                    }
                    else
                    {
                        string[] textArray3 = new string[5];
                        textArray3[0] = Constant.resultTip5;
                        num5 = num + 1;
                        textArray3[1] = num5.ToString();
                        textArray3[2] = Constant.size;
                        textArray3[3] = optSize.ProdInfoLst[i].Cut[num].ToString();
                        textArray3[4] = Constant.startTips5;
                        ConstantMethod.ShowInfo(rtbWork, string.Concat(textArray3));
                    }
                    //���ݿ����
                    Simi_SQLReturn(optSize.SingleSizeLst[i][num].Barc);

                    num = showValue;

                    if (showValue < optSize.SingleSizeLst[i].Count )
                    {
                      
                        PrintBarCheck(optSize.SingleSizeLst[i][showValue]);
                    }
                    

                }

                if (showValue >= optSize.ProdInfoLst[i].Cut.Count)
                {
                    /**
                    ConstantMethod.ShowInfo(rtbWork, "����ѭ���и2" +
                    "showValue" + showValue.ToString() + "num" +
                    num.ToString() + "Ҫ�е���"
                    + optSize.ProdInfoLst[i].Cut.Count.ToString()
                    );
                ***/
                    break;
                }
            }

            return 0;
        }
        void Simi_SQLReturn(string barc)
        {
            if (DeviceName.Equals(Constant.simiDeivceName))
            {
                if (lo_conn.State == ConnectionState.Open)
                {
                    SimiUpdateData(
                        "C",
                        barc,
                        "0210",
                        "999",
                         true);
                }
                else ConstantMethod.ShowInfo(rtbWork, "���ݿ�δ���ӣ�");
            }
           
        }
        private void CutLoop(int i, int printdMode)
        {
            if ((optSize.SingleSizeLst[i].Count > 0) && !CurrentDoorType.Equals(optSize.SingleSizeLst[i][0].ParamStrLst[10]))
            {
                CurrentDoorType = optSize.SingleSizeLst[i][0].ParamStrLst[10];
                PrintBarCheck(optSize.SingleSizeLst[i][0]);
                if (!DeviceName.Equals(Constant.scjDeivceName))
                {
                    List<string> list = new List<string>();
                    for (int j = i; j < optSize.ProdInfoLst.Count<ProdInfo>(); j++)
                    {
                        list.AddRange(optSize.ProdInfoLst[j].Param10);
                    }
                    int num2 = DoorTypeCount(CurrentDoorType, list.ToArray());
                    if (num2 > 0)
                    {
                        SetDoorTypeCutCount(num2);
                    }
                }
            }
            int num = 0;
            while (RunFlag)
            {
                Application.DoEvents();
                Thread.Sleep(10);
                int showValue = cutDoneOutInPs.ShowValue;
                if ((!RunFlag || IsInNoSafe) || (errorList.Count > 0))
                {
                    ConstantMethod.ShowInfo(rtbWork, Constant.emgStopTip);
                    string[] logs = new string[] { DeviceName + Constant.emgStopTip };
                    LogManager.WriteProgramLog(logs);
                    stopOperation();
                    break;
                }
                if ((showValue != num) && (num < optSize.ProdInfoLst[i].Cut.Count))
                {
                    int result = 0;
                    if (!optSize.SingleSizeLst[i][num].Barc.Equals(Constant.ScarId) && int.TryParse(optSize.SingleSizeLst[i][num].DtUser.Rows[optSize.SingleSizeLst[i][num].Xuhao][2].ToString(), out result))
                    {
                        result++;
                        if ((DeviceProperty == Constant.scjDeivceId) && (plcHandlebarCodeOutInPs !=null))
                        {
                            evokDevice.SetMValueOFF(plcHandlebarCodeOutInPs);
                        }
                        optSize.SingleSizeLst[i][num].DtUser.Rows[optSize.SingleSizeLst[i][num].Xuhao][2] = result;
                        optSize.checkIsDone(optSize.SingleSizeLst[i][num].Xuhao);
                    }
                    string[] textArray2 = new string[5];
                    textArray2[0] = Constant.resultTip5;
                    int num6 = num + 1;
                    textArray2[1] = num6.ToString();
                    textArray2[2] = Constant.size;
                    textArray2[3] = optSize.ProdInfoLst[i].Cut[num].ToString();
                    textArray2[4] = Constant.startTips5;
                    ConstantMethod.ShowInfo(rtbWork, string.Concat(textArray2));
                    ConstantMethod.ShowInfo(rtbWork, CurrentDoorType);
                    num = showValue;
                    if ((showValue < optSize.SingleSizeLst[i].Count) && !CurrentDoorType.Equals(optSize.SingleSizeLst[i][showValue].ParamStrLst[10]))
                    {
                        CurrentDoorType = optSize.SingleSizeLst[i][showValue].ParamStrLst[10];
                        if (showValue < optSize.SingleSizeLst[i].Count)
                        {
                            PrintBarCheck(optSize.SingleSizeLst[i][showValue]);
                        }
                        if (!DeviceName.Equals(Constant.scjDeivceName))
                        {
                            List<string> list2 = new List<string>();
                            list2.AddRange(optSize.ProdInfoLst[i].Param10.Skip<string>(showValue).Take<string>(optSize.ProdInfoLst[i].Param10.Count - showValue));
                            for (int k = i + 1; k < optSize.ProdInfoLst.Count<ProdInfo>(); k++)
                            {
                                list2.AddRange(optSize.ProdInfoLst[k].Param10);
                            }
                            int num7 = DoorTypeCount(CurrentDoorType, list2.ToArray());

                            if (num7 > 0)
                            {
                                SetDoorTypeCutCount(num7);
                            }
                        }
                    }
                }
                if (showValue >= optSize.ProdInfoLst[i].Cut.Count)
                {
                    break;
                }
            }
        }

        private int CutLoopWithDevice(int i)
        {
            if (!optSize.SingleSizeLst[i][0].Barc.Contains("�ü�"))
                printBarcode(printReport, optSize.SingleSizeLst[i][0].ParamStrLst.ToArray());
            int num = 0;
            while (RunFlag)
            {
                Application.DoEvents();
               
                Simi_Show(i, num);
                int result = 0;
                if(optSize.SingleSizeLst[i][num].DtUser!=null)
                if (!optSize.SingleSizeLst[i][num].Barc.Equals(Constant.ScarId) && int.TryParse(optSize.SingleSizeLst[i][num].DtUser.Rows[optSize.SingleSizeLst[i][num].Xuhao][2].ToString(), out result))
                {
                    result++;
                    optSize.SingleSizeLst[i][num].DtUser.Rows[optSize.SingleSizeLst[i][num].Xuhao][2] = result;
                    Point point = new Point(optSize.SingleSizeLst[i][num].Xuhao, 2);
                    optSize.checkIsDone(optSize.SingleSizeLst[i][num].Xuhao);
                    
                 }
                ConstantMethod.Delay(1000);
                string[] textArray1 = new string[7];
                textArray1[0] = Constant.resultTip5;
                int num2 = num + 1;
                textArray1[1] = num2.ToString();
                textArray1[2] = Constant.size;
                textArray1[3] = optSize.ProdInfoLst[i].Cut[num].ToString();
                textArray1[4] = Constant.startTips6;
                textArray1[5] = optSize.ProdInfoLst[i].Param1[num].ToString();
                textArray1[6] = Constant.startTips5;
                ConstantMethod.ShowInfo(rtbWork, string.Concat(textArray1));

                //���ݿ����
                Simi_SQLReturn(optSize.SingleSizeLst[i][num].Barc);
                num++;

               
                if (num >= optSize.ProdInfoLst[i].Cut.Count)
                {
                    break;
                }
                
                if (!optSize.SingleSizeLst[i][num].Barc.Contains("�ü�"))
                    printBarcode(printReport, optSize.SingleSizeLst[i][num].ParamStrLst.ToArray());

            }
            ConstantMethod.Delay(1000);
            Simi_Show(i, num);
            return 0;
        }
        public void CutReady(int id)
        {
            if (id == Constant.CutNormalWithAngle)
            {
                optSize.Len = (int)(lcOutInPs.ShowValue * Constant.dataMultiple);
                optSize.Dbc = (int)(dbcOutInPs.ShowValue * Constant.dataMultiple);
                optSize.Ltbc = (int)(ltbcOutInPs.ShowValue * Constant.dataMultiple);
                optSize.Safe = (int)(safeOutInPs.ShowValue * Constant.dataMultiple);
            }
            else
            {
                CutReady();
            }
        }
        public void CutReady()
        {
            optSize.Len = lcOutInPs.ShowValue;
            optSize.Dbc = dbcOutInPs.ShowValue;
            optSize.Ltbc = ltbcOutInPs.ShowValue;
            optSize.Safe = safeOutInPs.ShowValue;
        }

        public void CutRotateWithHoleThread()
        {
            if ((optSize.ProdInfoLst.Count > 0) && (CutProCnt < optSize.ProdInfoLst.Count))
            {
                for (int i = CutProCnt; i < optSize.ProdInfoLst.Count; i++)
                {
                    ConstantMethod.ShowInfo(rtbWork, Constant.resultTip5 + ((i + 1)).ToString() + Constant.startTips4);
                    CountClr();
                    DownLoadDataWithHoleAngle(i);
                    CutLoop(i);
                }
            }
            else
            {
                MessageBox.Show(Constant.noData);
            }
        }
        //�������Զ���λ ��ʾ ����ͷ���ǳߴ�
        private Label autoPosTipLabel;
        public System.Windows.Forms.Label AutoPosTipLabel
        {
            get { return autoPosTipLabel; }
            set { autoPosTipLabel = value; }
        }

        void SetAutoPosTipLabelText(string str)
        {
            if (AutoPosTipLabel != null)
            {
                AutoPosTipLabel.Invoke((EventHandler)(delegate
                {
                    AutoPosTipLabel.Text = str;// Constant.startTips10;
                }));
                
            }
        }
        private void CutSimenSiPlcThread_AutoPos()
        {
            showWorkInfo(optSize.ProdInfoLst.Count.ToString());
            if (optSize.ProdInfoLst.Count > 0)
            {
                for (int i = CutProCnt; i < optSize.ProdInfoLst.Count; i++)
                {
                    SaveProdDataLog(optSize.ProdInfoLst[i], i);
                    ConstantMethod.ShowInfo(rtbWork, Constant.resultTip5 + ((i + 1)).ToString() + Constant.startTips4);
                    CountClr(-1);
                    // ConstantMethod.ShowInfo(rtbWork, Constant.startTips10);
                    SetAutoPosTipLabelText(Constant.startTips10);
                    //DownLoadDataNormalWithSimenSiPlc2(i);
                    CutLoopWithAutoPos(i);
                }
            }
            else
            {
                MessageBox.Show(Constant.noData);
            }
        }

        private void CutSimenSiPlcThread()
        {
            showWorkInfo(optSize.ProdInfoLst.Count.ToString());
            if (optSize.ProdInfoLst.Count > 0)
            {
                for (int i = CutProCnt; i < optSize.ProdInfoLst.Count; i++)
                {
                    SaveProdDataLog(optSize.ProdInfoLst[i], i);
                    ConstantMethod.ShowInfo(rtbWork, Constant.resultTip5 + ((i + 1)).ToString() + Constant.startTips4);
                    CountClr();
                    DownLoadDataNormalWithSimenSiPlc2(i);
                    CutLoop(i);
                }
            }
            else
            {
                MessageBox.Show(Constant.noData);
            }
        }
        //�Ƿ����Ű�
        bool isArrangeOpt = false;
        public bool IsArrangeOpt
        {
            get { return isArrangeOpt; }
            set { isArrangeOpt = value; }
        }
        public int CutStartMeasure(bool split, int cutid)
        {
            int showValue = ltbcDefaultOutInPs.ShowValue;
            optSize.Len = lcOutInPs.ShowValue;
            optSize.Dbc = dbcOutInPs.ShowValue;
            optSize.Ltbc = ltbcOutInPs.ShowValue;
            optSize.Safe = safeOutInPs.ShowValue;
            if (wlMiniSizeOutInPs !=null)
            {
                optSize.WlMiniValue = wlMiniSizeOutInPs.ShowValue;
            }
            if (IsInNoSafe)
            {
                MessageBox.Show(Constant.emgStopTip);
                return -1;
            }
            string[] logs = new string[] { DeviceName + Constant.AutoMeasureMode };
            LogManager.WriteProgramLog(logs);
            start(cutid);
            while (mRunFlag)
            {
                int valueOld = 1;
                string[] textArray2 = new string[] { DeviceName + Constant.MeasureSt };
                LogManager.WriteProgramLog(textArray2);
                if (IsInNoSafe || optSize.CheckIsNoData(optSize.DtData))
                {
                    break;
                }
                                            
                ConstantMethod.DelayMeasure(Constant.MeaSureMaxTime, ref valueOld, ref autoCCInPs, ref emgStopInPs, ref mRunFlag);
                string[] textArray3 = new string[] { DeviceName + Constant.MeasureEd };
                LogManager.WriteProgramLog(textArray3);
                if (autoCCInPs.ShowValue == Constant.M_ON)
                {
                    evokDevice.SetMValueOFF(autoCCInPs);
                    optSize.Len = lcOutInPs.ShowValue;
                    if (scarInPs.ShowValue > 0)
                    {
                        if (GetScar(optSize, scarInPs.ShowValue) != 0)
                        {
                            MessageBox.Show(Constant.GetScarError);
                            return -2;
                        }
                        optSize.Ltbc = showValue;
                        if (cutid ==Constant.CutMeasureWithScarSplitNoSize)
                        {
                            optSize.OptMeasureWithScarCheckAndNoSize(split, rtbResult, optSize.DtData);
                        }
                        else
                        {
                            optSize.OptMeasureWithScarCheck(split, rtbResult, optSize.DtData);
                        }
                    }
                    else
                    {
                        optSize.Ltbc = showValue;
                        if (IsArrangeOpt)
                        {
                            optSize.OptMeasureNoOpt(rtbResult);
                        }
                        else
                        optSize.OptMeasure(rtbResult);
                       
                        KeFanLastProcess(3000);
                                               
                    }
                    if (optSize.ProdInfoLst.Count >= 1)
                    {
                        goto Label_02B6;
                    }
                    if (optSize.ValueAbleRow.Count > 0)
                    {
                        noSizeToCut();
                        goto Label_030F;
                    }
                    break;
                }
                MessageBox.Show(Constant.measureOutOfTime);
                return -3;
                Label_02B6:;
                try
                {
                    SelectCutThread(cutid);
                    if (!CutThread.IsAlive)
                    {
                        CutThread.Start();
                    }
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
                Label_030F:
                ConstantMethod.ShowInfo(rtbWork, Constant.NextOpt);
            }
            stopOperation();
            MessageBox.Show(Constant.CutEnd);
            return 0;
        }

        public int CutStartMeasure(bool split, int cutid, int cutCount)
        {
            int showValue = ltbcDefaultOutInPs.ShowValue;
            if (IsInNoSafe)
            {
                MessageBox.Show(Constant.emgStopTip);
                return -1;
            }
            string[] logs = new string[] { DeviceName + Constant.AutoMeasureMode };
            LogManager.WriteProgramLog(logs);
            start(cutid);
            while (mRunFlag)
            {
                int valueOld = 1;
                string[] textArray2 = new string[] { DeviceName + Constant.MeasureSt };
                LogManager.WriteProgramLog(textArray2);
                ConstantMethod.DelayMeasure(Constant.MeaSureMaxTime, ref valueOld, ref autoCCInPs, ref emgStopInPs, ref mRunFlag);
                if (IsInNoSafe)
                {
                    stopOperation();
                    MessageBox.Show("��ͣ��");
                    return -2;
                }
                string[] textArray3 = new string[] { DeviceName + Constant.MeasureEd };
                LogManager.WriteProgramLog(textArray3);
                if (autoCCInPs.ShowValue == Constant.M_ON)
                {
                    evokDevice.SetMValueOFF(autoCCInPs);
                    optSize.Len = lcOutInPs.ShowValue;
                    if (scarInPs.ShowValue > 0)
                    {
                        if (GetScar(optSize, scarInPs.ShowValue) != 0)
                        {
                            MessageBox.Show(Constant.GetScarError);
                            return -2;
                        }
                        optSize.Ltbc = showValue;
                        if (cutid == 4)
                        {
                            optSize.OptMeasureWithScarCheckAndNoSize(split, rtbResult, optSize.DtData);
                        }
                        else
                        {
                            optSize.OptMeasureWithScarCheck(split, rtbResult, optSize.DtData);
                        }
                    }
                    else
                    {
                        optSize.Ltbc = showValue;
                        optSize.OptMeasure(rtbResult);
                    }
                    if (optSize.ProdInfoLst.Count >= 1)
                    {
                        goto Label_0247;
                    }
                    if (optSize.ValueAbleRow.Count > 0)
                    {
                        noSizeToCut();
                        goto Label_02A0;
                    }
                    break;
                }
                MessageBox.Show(Constant.measureOutOfTime);
                return -3;
                Label_0247:;
                try
                {
                    SelectCutThread(cutid);
                    if (!CutThread.IsAlive)
                    {
                        CutThread.Start();
                    }
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
                Label_02A0:
                ConstantMethod.ShowInfo(rtbWork, Constant.NextOpt);
            }
            stopOperation();
            MessageBox.Show(Constant.CutEnd);
            return 0;
        }
        
        public void CutStartNormal(int cutid)
        {
                                 
            showWorkInfo(Constant.startTips0);
            if (RunFlag)
            {
                MessageBox.Show(DeviceName + Constant.alreadyStart);
            }
            else if (IsInNoSafe)
            {
                MessageBox.Show(DeviceName + Constant.emgStopTip);
            }
            else if (optSize.ProdInfoLst.Count < 1)
            {
                MessageBox.Show(DeviceName + Constant.noData);
            }
            else if (errorList.Count > 0)
            {
                MessageBox.Show(DeviceName + Constant.Alarm);
            }
            else if (!start(cutid))
            {
                RunFlag = false;
                MessageBox.Show(DeviceName + Constant.DeviceStartFailed);
            }
            else
            {
                showWorkInfo(Constant.startTips1);

                try
                {

                    SelectCutThread(cutid);

                    if (!CutThread.IsAlive)
                    {
                        CutThread.Start();
                    }

                    while (CutThread.IsAlive)
                    {
                        Application.DoEvents();
                    }
                }
                finally
                {
                    CutThread      = null;
                    CutThreadStart = null;
                    stopOperation();
                    MessageBox.Show(DeviceName + Constant.CutEnd);
                }
            }
        }
        public void CutStartNormalWithBarCodeScan_Simensi(int cutid)
        {

            showWorkInfo(Constant.startTips0);
            if (RunFlag)
            {
                MessageBox.Show(DeviceName + Constant.alreadyStart);
            }
            else if (IsInNoSafe)
            {
                MessageBox.Show(DeviceName + Constant.emgStopTip);
            }           
            else if (errorList.Count > 0)
            {
                MessageBox.Show(DeviceName + Constant.Alarm);
            }
            else if (!start(cutid))
            {
                RunFlag = false;
                MessageBox.Show(DeviceName + Constant.DeviceStartFailed);
            }
            else
            {

                showWorkInfo(Constant.startTips1);
                while (RunFlag)
                {

                    optReady(Constant.optNormal);

                    if(optSize.ProdInfoLst.Count<=0)
                    {
                        break;
                    }

                    try
                    {

                        SelectCutThread(cutid);

                        if (!CutThread.IsAlive)
                        {
                            CutThread.Start();
                        }

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
                }

               
                stopOperation();
                MessageBox.Show(DeviceName + Constant.CutEnd);
            }
        }
        //д�˺���Ŀ������ �Զ�ģʽPLC ������ ��������߾��Զ������ֶ����� ����һ������
        //
        public void CutStartNormalWithSimensiMode(int cutid)
        {

            showWorkInfo(Constant.startTips0);
            if (RunFlag)
            {
                MessageBox.Show(DeviceName + Constant.alreadyStart);
            }
            else if (IsInNoSafe)
            {
                MessageBox.Show(DeviceName + Constant.emgStopTip);
            }
            else if (errorList.Count > 0)
            {
                MessageBox.Show(DeviceName + Constant.Alarm);
            }
            else if (!start(cutid))
            {
                RunFlag = false;
                MessageBox.Show(DeviceName + Constant.DeviceStartFailed);
            }
            else
            {

                showWorkInfo(Constant.startTips1);
                while (RunFlag)
                {

                    optReady(Constant.optNormal);

                    if (optSize.ProdInfoLst.Count <= 0)
                    {
                        break;
                    }

                    try
                    {

                        SelectCutThread(cutid);

                        if (!CutThread.IsAlive)
                        {
                            CutThread.Start();
                        }

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
                }


                stopOperation();
                MessageBox.Show(DeviceName + Constant.CutEnd);
            }
        }

        public void CutStartSimiPatternMode(int cutid)
        {

            SetSimiReady();
            
            showWorkInfo(Constant.startTips0);

            if (RunFlag)
            {
                MessageBox.Show(DeviceName + Constant.alreadyStart);
            }
            else if (IsInNoSafe)
            {
                MessageBox.Show(DeviceName + Constant.emgStopTip);
            }
            else if (errorList.Count > 0)
            {
                MessageBox.Show(DeviceName + Constant.Alarm);
            }
            else if (!start(cutid))
            {

                RunFlag = false;

                MessageBox.Show(DeviceName + Constant.DeviceStartFailed);

            }
            else
            {

                showWorkInfo(Constant.startTips1);

                int num = 0;
                double xoffset = 0.0;
                while (RunFlag)
                {
                    double pos = 0.0;
                    int valueOld = Constant.M_ON;
                    showWorkInfo(Constant.startTips9);
                    ConstantMethod.DelayMeasure(Constant.MeaSureMaxTime, ref valueOld, ref inspectPatternDoneInOutPs, ref emgStopInPs, ref mRunFlag);

                    if (IsInNoSafe)
                    {
                        stopOperation();
                        MessageBox.Show("�����˳��������豸��");
                        return;
                    }

                    if (inspectPatternDoneInOutPs.ShowValue == Constant.M_ON)
                    {
                        pos = inspectPatternPosInOutPs.ShowValue;
                        if ((pos > 0.0) && (pos < 30.0))
                        {
                            evokDevice.SetMValueOFF(inspectPatternDoneInOutPs);
                            if (cutid == Constant.CutNormalWithAngle)
                            {
                                optSize.OptMeasureWithSimiPattern(rtbResult, optSize.DtData,
                                GetPatternAllPos(
                                        pos*Constant.dataMultiple, 
                                        optSize.Len, 
                                        pSize), ref xoffset);
                            }
                        }
                        else
                        {
                            MessageBox.Show("���Ʋ������� ��Χ0~100"+ pos.ToString());
                            return;
                        }
                    }
                    if (optSize.ProdInfoLst.Count < 1)
                    {
                        MessageBox.Show(DeviceName + Constant.noData);
                        stopOperation();
                        return;
                    }
                    try
                    {
                        SelectCutThread(cutid);
                        if (!CutThread.IsAlive)
                        {
                            CutThread.Start();
                        }
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
                    num++;
                    ConstantMethod.ShowInfo(rtbWork, Constant.NextOpt);
                }

                stopOperation();
                MessageBox.Show(Constant.CutEnd);
            }
        }

        //û�������� Ҫ��һ���ź�
        //�Ʒ�������Ҫ���� PLC �������û������ �Ǿ�Ҫ��ǰ������λ
        void KeFanLastProcess(int i )
        {
            if (!DeviceUser.Equals(Constant.DeviceUserKeFan)) return;

            if (AutoMes && i == 3000)
            {
                if (optSize.CheckNextDataIsNoData(optSize.DtData))
                {
                    evokDevice.SetMValueON(noSizeToCutOutInPs);
                }
                return;
            }
            if(!AutoMes)
            if ( (i == optSize.ProdInfoLst.Count - 1) || i==2000
                )
            {
                evokDevice.SetMValueON(noSizeToCutOutInPs);
            }
        }
        private void CutWorkThread()
        {
            if (optSize.ProdInfoLst.Count > 0)
            {
                for (int i = CutProCnt; i < optSize.ProdInfoLst.Count; i++)
                {

                    Simi_Show(i);
                    SaveProdDataLog(optSize.ProdInfoLst[i], i);
                    ConstantMethod.ShowInfo(rtbWork, Constant.resultTip5 + ((i + 1)).ToString() + Constant.startTips4);
                    CountClr();
                    KeFanLastProcess(i);
                    DownLoadDataNormal(i);
                    CutLoop(i);
                }
            }
            else
            {
                MessageBox.Show(Constant.noData);
            }
        }

        //��ӡ���ϱ�ǩ ɾ����ʹ������ ID>1000
        void SimicutoffProcess(ProdInfo  prod)
        {

            List<string> list = new List<string>();
            double num2 = 0.0;
            num2 = ((double)prod.WL) / ((double)Constant.dataMultiple);

            if ((prod.ID == Constant.RestMaterialId))
            {
                Rsm.DeleteMaterial(optSize.SimiM.MaterialName, (prod.Len / Constant.dataMultiple).ToString());
                ConstantMethod.ShowInfo(rtbWork, "ɾ����ʹ�����ϣ�");

            }
            //����������� �ǾͿ�ʼ��ӡ����

            if ((prod.Barc.Last().Contains("�ü�")))
            {

                ConstantMethod.ShowInfo(rtbWork, "����������β�ϣ�");

                list.Add(optSize.MaterialName);
                list.Add(num2.ToString("0.00"));
                list.Add(optSize.SimiM.Width.ToString());
                list.Add(optSize.SimiM.Height.ToString());

                foreach (string ss in optSize.SimiM.ParamLst)
                {
                    if (!string.IsNullOrWhiteSpace(ss))
                    {
                        list.Add(ss);
                    }
                }


                string s = "";
                foreach (string ss in list)
                {
                    s += ss + "/";
                }

                list.Insert(0, s);

                printRestMaterial(list.ToArray());


            }


        }
        private void CutWorkThreadWithAngle()
        {
            CurrentDoorType = "1----";
            if (optSize.ProdInfoLst.Count > 0)
            {
                for (int i = CutProCnt; i < optSize.ProdInfoLst.Count; i++)
                {
                    Simi_Show(i);
                    ConstantMethod.ShowInfo(rtbWork, Constant.resultTip5 + ((i + 1)).ToString() + Constant.startTips4);
                    SaveProdDataLog(optSize.ProdInfoLst[i], i);
                    CountClr();
                    DownLoadDataNormalWithAngle(i);
                    CutLoop(i);
                    SimicutoffProcess(optSize.ProdInfoLst[i]);


                }
            }
            else
            {
                MessageBox.Show(Constant.noData);
            }
        }
        public void SetUseRest(bool ok)
        {
            optSize.UseRestMaterial = ok;
        }
        private void CutWorkThreadWithShuchi()
        {
            CurrentDoorType = "1----";
            if (optSize.ProdInfoLst.Count > 0)
            {
                for (int i = CutProCnt; i < optSize.ProdInfoLst.Count; i++)
                {
                    ConstantMethod.ShowInfo(rtbWork, Constant.resultTip5 + ((i + 1)).ToString() + Constant.startTips4);
                    SaveProdDataLog(optSize.ProdInfoLst[i], i);
                    CountClr();
                    DownLoadDataNormalWithShuchi(i);
                    CutLoop(i, 0);
                }
            }
            else
            {
                MessageBox.Show(Constant.noData);
            }
        }

        public bool DataJoin() { return optSize.dataGetTogether(); }


        public void dataLoad(int[] datavalue, int[] bitvalue)
        {
            evokDevice.SetMultiPleDValue(dataOutPs, datavalue);
            evokDevice.SetMultiPleDValue(muxiaoHoleOutPs, bitvalue);
        }


        public void DgvInOutEdit(int rowIndex, bool editEnable)
        {
            string s = evokDevice.DataForm.Rows[rowIndex]["param1"].ToString();
            string str2 = evokDevice.DataForm.Rows[rowIndex]["param2"].ToString();
            string userdata = evokDevice.DataForm.Rows[rowIndex]["addr"].ToString();
            string area = "D";
            int addr = 0;
            ConstantMethod.SplitAreaAndAddr(userdata, ref addr, ref area);
            int result = 0;
            int num3 = 0;
            if (int.TryParse(s, out result) && int.TryParse(str2, out num3))
            {
                if (XJPLCPackCmdAndDataUnpack.AreaGetFromStr(area) < 3)
                {
                    if (result < evokDevice.DPlcInfo.Count)
                    {
                        evokDevice.DPlcInfo[result].IsInEdit = editEnable;
                    }
                }
                else if (result < evokDevice.MPlcInfoAll.Count)
                {
                    evokDevice.MPlcInfoAll[result][num3].IsInEdit = editEnable;
                }
            }
        }

        public void dgvParam_CellEndEdit(DataGridView dgvParam, object sender, DataGridViewCellEventArgs e)
        {
            string s = dgvParam.SelectedCells[0].Value.ToString();
            int rowIndex = dgvParam.SelectedCells[0].RowIndex;
            double dataMultiple = Constant.dataMultiple;
            if (!double.TryParse(evokDevice.DataForm.Rows[rowIndex][Constant.strParam7].ToString(), out dataMultiple))
            {
                dataMultiple = Constant.dataMultiple;
            }
            try
            {
                double num;
                if (double.TryParse(s, out num))
                {
                    int num4 = (int)(num * dataMultiple);
                    DgvValueEdit(rowIndex, num4);
                }
            }
            catch
            {
            }
            finally
            {
                DgvInOutEdit(rowIndex, false);
            }
        }

        public void DgvValueEdit(int rowIndex, int num3)
        {
            string userdata = evokDevice.DataForm.Rows[rowIndex]["addr"].ToString();
            int addr = 0;
            string area = "D";
            string mode = evokDevice.DataForm.Rows[rowIndex]["mode"].ToString();
            ConstantMethod.SplitAreaAndAddr(userdata, ref addr, ref area);
            if ((XJPLCPackCmdAndDataUnpack.AreaGetFromStr(area) > -1) && (XJPLCPackCmdAndDataUnpack.AreaGetFromStr(area) < 3))
            {
                evokDevice.WriteSingleDData(addr, num3, area, mode);
            }
        }

        public void Dispose()
        {
            RunFlag = false;
            ConstantMethod.Delay(100);
            SaveFile();
            if (evokDevice !=null)
            {
                evokDevice.DeviceShutDown();
            }
            if (printReport !=null)
            {
                printReport.Dispose();
            }
            if ((CutThread != null) && CutThread.IsAlive)
            {
                CutThread.Join();
            }
            //SetOptSizeParam1(optSize.OptParam1);
            string[] logs = new string[] { DeviceName + Constant.Quit };
            LogManager.WriteProgramLog(logs);
        }

        public void doorBanSingleStart(OptSize op)
        {
            if (!op.IsDataValueAble)
            {
                MessageBox.Show(Constant.tataLineDeviceName[1] + Constant.optResultNoData);
            }
            else if (!getDeviceStatusIsReady(1))
            {
                MessageBox.Show(Constant.tataLineDeviceName[1] + Constant.errorDeviceStatus);
            }
            else
            {
                doorBanStart(op);
            }
        }

        public void doorBanSingleStop()
        {
            runflag_DoorBanKuan = false;
            runflag_DoorBanChang = false;
        }

        private void doorBanStart(OptSize op)
        {
            string[] logs = new string[] { "��ʼ�����Ű����������߳�" };
            LogManager.WriteProgramLog(logs);
            new Thread(new ParameterizedThreadStart(downLoadDoorBanChang)).Start(op);
            new Thread(new ParameterizedThreadStart(downLoadDoorBanKuan)).Start(op);
        }

        public void doorShellSingleStart(OptSize op)
        {
            if (!op.IsDataValueAble)
            {
                MessageBox.Show(Constant.tataLineDeviceName[2] + Constant.optResultNoData);
            }
            else if (!getDeviceStatusIsReady(2))
            {
                MessageBox.Show(Constant.tataLineDeviceName[2] + Constant.errorDeviceStatus);
            }
            else
            {
                doorShellStart(op);
            }
        }

        public void doorShellSingleStop()
        {
            runflag_DoorShellChang = false;
            runflag_DoorShellDoorId = false;
            runflag_DoorShellKuan = false;
        }

        private void doorShellStart(OptSize op)
        {
            string[] logs = new string[] { "��ʼ������Ƥ���������߳�" };
            LogManager.WriteProgramLog(logs);
            new Thread(new ParameterizedThreadStart(downLoadDoorShellChang)).Start(op);
            new Thread(new ParameterizedThreadStart(downLoadDoorShellKuan)).Start(op);
            new Thread(new ParameterizedThreadStart(downLoadDoorShellDoorId)).Start(op);
        }

        private int DoorTypeCount(string doorType, string[] doorNextTypeLst)
        {
            int num = 0;
            for (int i = 0; i < doorNextTypeLst.Count<string>(); i++)
            {
                if (doorType != doorNextTypeLst[i])
                {
                    return num;
                }
                num++;
            }
            return num;
        }

        private void DownLoadDataNormal(int i)
        {
            List<int> list = new List<int>();
            int num = 1;
            list.Add(optSize.ProdInfoLst[i].WL * num);
            list.Add(optSize.ProdInfoLst[i].Cut.Count);
            foreach (int num2 in optSize.ProdInfoLst[i].Cut)
            {
                list.Add(num2 * num);
            }
            string[] logs = new string[] { DeviceName + "�����·�" };
            LogManager.WriteProgramLog(logs);
            for (int j = 0; j < 30; j++)
            {
                if (!RunFlag)
                {
                    break;
                }
                string[] textArray2 = new string[] { DeviceName + Constant.DataDownLoad + j.ToString() };
                LogManager.WriteProgramLog(textArray2);
                if (evokDevice.SetMultiPleDValue(wlInOutPs, list.ToArray()))
                {
                    string[] textArray3 = new string[] { DeviceName + Constant.DataDownLoadSuccess };
                    LogManager.WriteProgramLog(textArray3);
                    evokDevice.SetMValueON(startCountInOutPs);
                    break;
                }
                if (j == 0x1d)
                {
                    string[] textArray4 = new string[] { DeviceName + Constant.DataDownLoadFail };
                    LogManager.WriteProgramLog(textArray4);
                }
            }
        }

        private void DownLoadDataNormalWithAngle(int i)
        {
            List<int> list = new List<int> {
                optSize.ProdInfoLst[i].Len * 10,
                optSize.ProdInfoLst[i].WL * 10,
                optSize.ProdInfoLst[i].Cut.Count,
                0x15f90
            };
            for (int j = 0; j < optSize.ProdInfoLst[i].Cut.Count; j++)
            {
                double result = 0.0;
                double num3 = 0.0;
                if (double.TryParse(optSize.ProdInfoLst[i].Param5[j], out result) && double.TryParse(optSize.ProdInfoLst[i].Param6[j], out num3))
                {
                    if (downLoadSizeId == Constant.downLoadBottomSizeId)
                    {
                        list.Add((int)(num3 * 1000.0));
                    }
                    else
                    {
                        list.Add((int)(result * 1000.0));
                    }
                }
                float num4 = 0f;
                float num5 = 0f;
                if (float.TryParse(optSize.ProdInfoLst[i].Param1[j], out num4) && float.TryParse(optSize.ProdInfoLst[i].Param2[j], out num5))
                {
                    num4 *= 1000f;
                    num5 *= 1000f;
                    if (num4 == 90000f)
                    {
                        num4 = 0f;
                    }
                    if (num5 == 90000f)
                    {
                        num5 = 0f;
                    }
                    list.Add((int)num4);
                    list.Add((int)num5);
                }
            }
            string[] logs = new string[] { DeviceName + "�����·�" };
            LogManager.WriteProgramLog(logs);
            for (int k = 0; k < 30; k++)
            {
                if (!RunFlag)
                {
                    break;
                }
                string[] textArray2 = new string[] { DeviceName + Constant.DataDownLoad + k.ToString() };
                LogManager.WriteProgramLog(textArray2);
                if (evokDevice.SetMultiPleDValue(wlInOutPs, list.ToArray()))
                {
                    string[] textArray3 = new string[] { DeviceName + Constant.DataDownLoadSuccess };
                    LogManager.WriteProgramLog(textArray3);
                    if (evokDevice.SetMValueON(startCountInOutPs))
                    {
                    }
                    break;
                }
                if (k == 0x1d)
                {
                    string[] textArray4 = new string[] { DeviceName + Constant.DataDownLoadFail };
                    LogManager.WriteProgramLog(textArray4);
                }
            }
        }

        private void DownLoadDataNormalWithShuchi(int i)
        {
            List<int> list = new List<int>();
            List<int> source = new List<int>();
            list.Add(1);
            list.Add(optSize.ProdInfoLst[i].Cut.Count);
            if (DeviceName.Equals(Constant.scjDeivceName))
            {
                foreach (int num in optSize.ProdInfoLst[i].Cut)
                {
                    list.Add(num * 0x3e8);
                }
            }
            else
            {
                list.AddRange(optSize.ProdInfoLst[i].Cut);
            }
            source.Add(optSize.ProdInfoLst[i].Cut.Count);
            int[] collection = null;
            List<int> list3 = new List<int>();
            try
            {
                foreach (string str in optSize.ProdInfoLst[i].Param12)
                {
                    int result = 1;
                    if (list3.Count > 0)
                    {
                        result = list3.Last<int>();
                    }
                    if (int.TryParse(str, out result))
                    {
                    }
                    list3.Add(result);
                }
            }
            catch (Exception)
            {
            }
            collection = list3.ToArray();
            if ((collection != null) && (collection.Length > 0))
            {
                source.AddRange(collection);
            }
            if (DeviceName.Equals(Constant.scjDeivceName) && (source.Count<int>() > 1))
            {
                list.Insert(2, source[1]);
            }
            for (int j = 0; j < 30; j++)
            {
                if (!RunFlag)
                {
                    break;
                }
                string[] logs = new string[] { DeviceName + Constant.DataDownLoad + j.ToString() };
                LogManager.WriteProgramLog(logs);
                if (evokDevice.SetMultiPleDValue(wlInOutPs, list.ToArray()))
                {
                    ConstantMethod.Delay(200);
                    if (!DeviceName.Equals(Constant.scjDeivceName) && evokDevice.SetMultiPleDValue(GWOutInPs, source.ToArray()))
                    {
                        ConstantMethod.Delay(20);
                        string[] textArray2 = new string[] { DeviceName + Constant.DataDownLoadSuccess };
                        LogManager.WriteProgramLog(textArray2);
                    }
                    if ((wlInOutPs.ShowValue > 0) && evokDevice.SetMValueON(startCountInOutPs))
                    {
                        break;
                    }
                }
            }
        }

        private void DownLoadDataNormalWithSimenSiPlc(int i)
        {
            List<int> list = new List<int>();
            int num = 10;
            int count = optSize.ProdInfoLst[i].Cut.Count;
            list.Add(optSize.ProdInfoLst[i].Cut[count - 1] * num);
            list.Add(optSize.ProdInfoLst[i].Cut.Count);
            for (int j = 0; j < (optSize.ProdInfoLst[i].Cut.Count - 1); j++)
            {
                list.Add(optSize.ProdInfoLst[i].Cut[j] * num);
            }
            string[] logs = new string[] { DeviceName + "�����·�" };
            LogManager.WriteProgramLog(logs);
            for (int k = 0; k < 30; k++)
            {
                if (!RunFlag)
                {
                    break;
                }
                string[] textArray2 = new string[] { DeviceName + Constant.DataDownLoad + k.ToString() };
                LogManager.WriteProgramLog(textArray2);
                if (evokDevice.SetMultiPleDValue(wlInOutPs, list.ToArray()))
                {
                    string[] textArray3 = new string[] { DeviceName + Constant.DataDownLoadSuccess };
                    LogManager.WriteProgramLog(textArray3);
                    evokDevice.SetMValueON(startCountInOutPs);
                    break;
                }
                if (k == 0x1d)
                {
                    string[] textArray4 = new string[] { DeviceName + Constant.DataDownLoadFail };
                    LogManager.WriteProgramLog(textArray4);
                }
            }
        }
        private void DownLoadDataNormalWithSimenSiPlc2(int i)
        {
            List<int> list = new List<int>();
            int num = 10;
            int count = optSize.ProdInfoLst[i].Cut.Count;
            list.Add(optSize.ProdInfoLst[i].Cut.Count);
            for (int j = 0; j < (optSize.ProdInfoLst[i].Cut.Count); j++)
            {
                list.Add(optSize.ProdInfoLst[i].Cut[j] * num);
            }
            string[] logs = new string[] { DeviceName + "�����·�" };
            LogManager.WriteProgramLog(logs);
            for (int k = 0; k < 30; k++)
            {
                if (!RunFlag)
                {
                    break;
                }
                string[] textArray2 = new string[] { DeviceName + Constant.DataDownLoad + k.ToString() };
                LogManager.WriteProgramLog(textArray2);
                if (evokDevice.SetMultiPleDValue(ldsOutInPs, list.ToArray()))
                {
                    string[] textArray3 = new string[] { DeviceName + Constant.DataDownLoadSuccess };
                    LogManager.WriteProgramLog(textArray3);
                    evokDevice.SetMValueON(startCountInOutPs);
                    break;
                }
                if (k == 0x1d)
                {
                    string[] textArray4 = new string[] { DeviceName + Constant.DataDownLoadFail };
                    LogManager.WriteProgramLog(textArray4);
                }
            }
        }

        private void DownLoadDataWithDoorBanWidth(int i)
        {
            List<List<int>> source = new List<List<int>>();
            List<int> item = new List<int>();
            source.Add(item);
            for (int j = 0; j < optSize.ProdInfoLst.Count; j++)
            {
                if (!RunFlag)
                {
                    break;
                }
                int num4 = 0;
                while (num4 < optSize.ProdInfoLst[j].Cut.Count)
                {
                    if (!RunFlag)
                    {
                        break;
                    }
                    if ((source.Count > 0) && (source.Last<List<int>>().Count > 0x31))
                    {
                        item = new List<int>();
                        source.Add(item);
                    }
                    int result = 0;
                    if (!int.TryParse(optSize.ProdInfoLst[j].Param1[num4], out result))
                    {
                        MessageBox.Show("����������");
                        break;
                    }
                    result *= Constant.dataMultiple;
                    item.Add(result);
                    num4++;
                    Application.DoEvents();
                }
            }
            int addr = widthCountInOutPs.Addr;
            int num2 = 0;
            for (int k = 0; k < source.Count; k++)
            {
                num2 += source[k].Count;
            }
            source[0].Insert(0, num2);
            for (int m = 0; m < source.Count<List<int>>(); m++)
            {
                if (!RunFlag)
                {
                    break;
                }
                if (evokDevice.SetMultiPleDValue(widthCountInOutPs, source[m].ToArray()))
                {
                    ConstantMethod.Delay(100);
                    widthCountInOutPs.Addr += 2 * source[m].Count;
                }
            }
            widthCountInOutPs.Addr = addr;
            if (wlInOutPs.ShowValue > 0)
            {
                evokDevice.SetMValueOFF2ON(startCountInOutPs);
            }
        }

        private void DownLoadDataWithDoorShell(int i)
        {
            List<List<int>> source = new List<List<int>>();
            List<List<int>> list2 = new List<List<int>>();
            List<int> item = new List<int>();
            List<int> list4 = new List<int>();
            int num = 0;
            if (optSize.ProdInfoLst.Count == 1)
            {
                while (num < optSize.ProdInfoLst[0].Cut.Count)
                {
                    if (!RunFlag)
                    {
                        break;
                    }
                    if ((num % 50) == 0)
                    {
                        item = new List<int>();
                        list4 = new List<int>();
                        source.Add(item);
                        list2.Add(list4);
                    }
                    item.Add(optSize.ProdInfoLst[0].Cut[num]);
                    int result = 0;
                    if (!int.TryParse(optSize.ProdInfoLst[0].Param1[num], out result))
                    {
                        MessageBox.Show("����������");
                        break;
                    }
                    result *= Constant.dataMultiple;
                    list4.Add(result);
                    num++;
                    Application.DoEvents();
                }
            }
            int addr = heightCountInOutPs.Addr;
            int num3 = widthCountInOutPs.Addr;
            source[0].Insert(0, optSize.ProdInfoLst[0].Cut.Count);
            list2[0].Insert(0, optSize.ProdInfoLst[0].Cut.Count);
            for (int j = 0; j < source.Count<List<int>>(); j++)
            {
                if (evokDevice.SetMultiPleDValue(heightCountInOutPs, source[j].ToArray()) && evokDevice.SetMultiPleDValue(widthCountInOutPs, list2[j].ToArray()))
                {
                    ConstantMethod.Delay(200);
                    heightCountInOutPs.Addr += 2 * source[j].Count;
                    widthCountInOutPs.Addr += 2 * list2[j].Count;
                }
            }
            heightCountInOutPs.Addr = addr;
            widthCountInOutPs.Addr = num3;
        }

        private void DownLoadDataWithHoleAngle(int i)
        {
            List<int> list = new List<int>();
            List<ProdInfo> prodInfoLst = optSize.ProdInfoLst;
            for (int j = 0; j < optSize.SingleSizeLst[i].Count; j++)
            {
                if (!RunFlag)
                {
                    break;
                }
                SingleSizeWithHoleAngle angle = new SingleSizeWithHoleAngle(optSize.SingleSizeLst[i][j].DtUser, optSize.SingleSizeLst[i][j].Xuhao);
                angle = ConstantMethod.Mapper<SingleSizeWithHoleAngle, SingleSize>(optSize.SingleSizeLst[i][j]);
                optSize.ProdInfoLst[i].hole.Add(angle.Hole);
                optSize.ProdInfoLst[i].angle.Add(angle.Angle);
            }
            for (int k = 0; k < 6; k++)
            {
                if (!RunFlag)
                {
                    break;
                }
                list.Add(optSize.ProdInfoLst[i].Cut.Count);
                int addr = wlInOutPs.Addr;
                if ((prodInfoLst[i].hole.Count > 0) && (prodInfoLst[i].angle.Count > 0))
                {
                    for (int m = 0; m < prodInfoLst[i].Cut.Count; m++)
                    {
                        list.Add(prodInfoLst[i].Cut[m]);
                        list.Add(1);
                        int item = 0;
                        for (int n = 0; n < (prodInfoLst[i].hole[m].Count<int>() / 2); n += 3)
                        {
                            if (prodInfoLst[i].hole[m][n] > 0)
                            {
                                item++;
                            }
                        }
                        list.Add(prodInfoLst[i].angle[m][0]);
                        list.Add(item);
                        for (int num9 = 0; num9 < 30; num9++)
                        {
                            list.Add(prodInfoLst[i].hole[m][num9]);
                        }
                        int num7 = 0;
                        for (int num10 = 30; num10 < prodInfoLst[i].hole[m].Count<int>(); num10 += 3)
                        {
                            if (prodInfoLst[i].hole[m][num10] > 0)
                            {
                                num7++;
                            }
                        }
                        list.Add(prodInfoLst[i].angle[m][1]);
                        list.Add(num7);
                        for (int num11 = 30; num11 < (30 + (num7 * 3)); num11++)
                        {
                            list.Add(prodInfoLst[i].hole[m][num11]);
                        }
                        evokDevice.SetMultiPleDValue(wlInOutPs, list.ToArray());
                        string[] logs = new string[] { DeviceName + Constant.DataDownLoad + wlInOutPs.Addr.ToString() };
                        LogManager.WriteProgramLog(logs);
                        list.Clear();
                        if (m == 0)
                        {
                            wlInOutPs.Addr += 0x86;
                        }
                        else
                        {
                            wlInOutPs.Addr += 0x84;
                        }
                    }
                }
                wlInOutPs.Addr = addr;
                if ((wlInOutPs.ShowValue == prodInfoLst[i].Cut.Count) && evokDevice.SetMValueON(startCountInOutPs))
                {
                    break;
                }
            }
            int valueOld = 0;
            ConstantMethod.DelayMeasure(Constant.PlcCountTimeOut, ref valueOld, ref startCountInOutPs, ref emgStopInPs, ref mRunFlag);
            if (startCountInOutPs.ShowValue != valueOld)
            {
                MessageBox.Show(Constant.PlcReadDataError);
                string[] textArray2 = new string[] { DeviceName + Constant.PlcReadDataError };
                LogManager.WriteProgramLog(textArray2);
                RunFlag = false;
            }
        }

        private void downLoadDoorBanChang(object opt)
        {
            if (opt != null)
            {
                OptSize op = (OptSize)opt;
                int pos = 0;
                int perCount = 10;
                string[] logs = new string[] { "�Ű峤�������ݿ�ʼ��" };
                LogManager.WriteProgramLog(logs);
                if (doorDownLoadCountInOutPs.ShowValue > 0)
                {
                    perCount = doorDownLoadCountInOutPs.ShowValue;
                }
                runflag_DoorBanChang = true;
                downThread(2, op, pos, perCount, ref runflag_DoorBanChang, doorbanDataChangRequestInPs, doorbanLenthChangInOutPs, doorbanDataChangDownLoadSuccessOutPs, doorbanDoorCountInOutPs);
                runflag_DoorBanChang = false;
            }
        }

        private void downLoadDoorBanKuan(object opt)
        {
            if (opt != null)
            {
                OptSize op = (OptSize)opt;
                int pos = 0;
                int perCount = 10;
                if (doorDownLoadCountInOutPs.ShowValue > 0)
                {
                    perCount = doorDownLoadCountInOutPs.ShowValue;
                }
                string[] logs = new string[] { "�Ű���������ݿ�ʼ��" };
                LogManager.WriteProgramLog(logs);
                runflag_DoorBanKuan = true;
                downThread(3, op, pos, perCount, ref runflag_DoorBanKuan, doorbanDataKuanRequestInPs, doorbanLenthKuanInOutPs, doorbanDataKuanDownLoadSuccessOutPs, null);
                runflag_DoorBanKuan = false;
            }
        }

        private void downLoadDoorShellChang(object opt)
        {
            if (opt != null)
            {
                OptSize op = (OptSize)opt;
                int pos = 0;
                int perCount = 10;
                string[] logs = new string[] { "��Ƥ���������ݿ�ʼ��" };
                LogManager.WriteProgramLog(logs);
                if (doorDownLoadCountInOutPs.ShowValue > 0)
                {
                    perCount = doorDownLoadCountInOutPs.ShowValue;
                }
                runflag_DoorShellChang = true;
                downThread(4, op, pos, perCount, ref runflag_DoorShellChang, doorshellDataChangRequestInPs, doorshellLenthChangInOutPs, doorshellDataChangDownLoadSuccessOutPs, doorshellDoorCountInOutPs);
                runflag_DoorShellChang = false;
            }
        }

        private void downLoadDoorShellDoorId(object opt)
        {
            if (opt != null)
            {
                OptSize op = (OptSize)opt;
                int pos = 0;
                int perCount = 10;
                string[] logs = new string[] { "��ƤID�������ݿ�ʼ��" };
                LogManager.WriteProgramLog(logs);
                if (doorDownLoadCountInOutPs.ShowValue > 0)
                {
                    perCount = doorDownLoadCountInOutPs.ShowValue;
                }
                runflag_DoorShellDoorId = true;
                downThread(6, op, pos, perCount, ref runflag_DoorShellDoorId, doorshellDataDoorIdRequestInPs, doorshellDoorIdInOutPs, doorshellDataDoorIdDownLoadSuccessOutPs, null);
                runflag_DoorShellDoorId = false;
            }
        }

        private void downLoadDoorShellKuan(object opt)
        {
            if (opt != null)
            {
                OptSize op = (OptSize)opt;
                int pos = 0;
                int perCount = 10;
                string[] logs = new string[] { "��Ƥ���������ݿ�ʼ��" };
                LogManager.WriteProgramLog(logs);
                if (doorDownLoadCountInOutPs.ShowValue > 0)
                {
                    perCount = doorDownLoadCountInOutPs.ShowValue;
                }
                runflag_DoorShellKuan = true;
                downThread(5, op, pos, perCount, ref runflag_DoorShellKuan, doorshellDataKuanRequestInPs, doorshellLenthKuanInOutPs, doorshellDataKuanDownLoadSuccessOutPs, null);
                runflag_DoorShellKuan = false;
            }
        }

        public void downLoadTest(List<int> devLst, OptSize op, OptSize op1, OptSize op2)
        {
            string[] logs = new string[] { "��ʼ�������������߳�" };
            LogManager.WriteProgramLog(logs);
            List<bool> list = new List<bool>();
            RunFlag = true;
            if (devLst.Contains(0))
            {
                list.Add(runflag_XialiaoJu);
                xialiaoStart(op);
            }
            if (devLst.Contains(1))
            {
                list.Add(runflag_DoorBanChang);
                list.Add(runflag_DoorBanKuan);
                doorBanStart(op1);
            }
            if (devLst.Contains(2))
            {
                list.Add(runflag_DoorShellChang);
                list.Add(runflag_DoorShellKuan);
                doorShellStart(op2);
            }
            LineStart();
            while ((((runflag_XialiaoJu || runflag_DoorBanChang) || (runflag_DoorBanKuan || runflag_DoorShellChang)) || runflag_DoorShellKuan) || runflag_DoorShellDoorId)
            {
                Application.DoEvents();
                Thread.Sleep(100);
            }
            RunFlag = false;
        }

        private void downLoadXiaLiaoju(object opt)
        {
            if (opt != null)
            {
                OptSize op = (OptSize)opt;
                int pos = 0;
                int num2 = Constant.M_OFF;
                int perCount = 10;
                string[] logs = new string[] { "���Ͼ��������ݿ�ʼ��" };
                LogManager.WriteProgramLog(logs);
                if (doorDownLoadCountInOutPs.ShowValue > 0)
                {
                    perCount = doorDownLoadCountInOutPs.ShowValue;
                }
                runflag_XialiaoJu = true;
                downThread(1, op, pos, perCount, ref runflag_XialiaoJu, xialiaoDataRequestInPs, xialiaojuOutPs, xialiaoDataDownLoadSuccessOutPs, xialiaoDoorCountInOutPs);
                runflag_XialiaoJu = false;
            }
        }

        private bool downThread(int id, OptSize op, int pos, int perCount, ref bool runflag_temp, PlcInfoSimple reqPs, PlcInfoSimple stp, PlcInfoSimple downLoadSuccess, PlcInfoSimple finishPs)
        {
            int showValue = Constant.M_OFF;
            while (runflag_temp)
            {
                Thread.Sleep(100);
                if (needBreak(id))
                {
                    runflag_temp = false;
                    break;
                }
                if ((pos == op.ProdInfoLst.Count) || (lineEmgStopInPs.ShowValue == Constant.M_ON))
                {
                    if (finishPs !=null)
                    {
                        while (comIsDownLoading)
                        {
                            Application.DoEvents();
                        }
                        comIsDownLoading = true;
                        string[] logs = new string[] { id.ToString() + "�豸�������ݽ�����д-1��" };
                        LogManager.WriteProgramLog(logs);
                        evokDevice.SetDValue(finishPs, -1);
                        comIsDownLoading = false;
                    }
                    break;
                }
                if ((showValue == Constant.M_OFF) && (reqPs.ShowValue == Constant.M_ON))
                {
                    if (comIsDownLoading)
                    {
                        string[] textArray2 = new string[] { id.ToString() + "���������ź��У������ظ�������ߵȴ���" };
                        LogManager.WriteProgramLog(textArray2);
                    }
                    while (comIsDownLoading)
                    {
                        Application.DoEvents();
                    }
                    comIsDownLoading = true;
                    string[] textArray3 = new string[] { id.ToString() + "���������ź���Ч" };
                    LogManager.WriteProgramLog(textArray3);
                    if ((pos >= op.ProdInfoLst.Count) || (op.ProdInfoLst.Count <= 0))
                    {
                        break;
                    }
                    if (LinedownLoadData(stp, op, ref pos, perCount, id))
                    {
                        ConstantMethod.Delay(100);
                        evokDevice.SetMValueON(downLoadSuccess);
                    }
                    comIsDownLoading = false;
                }
                showValue = reqPs.ShowValue;
                Application.DoEvents();
            }
            return true;
        }


        public bool emgStop()
        {
            int num = 0;
            if (emgStopOutPs.Area !=null)
            {
                while (!evokDevice.SetMValueON(emgStopOutPs))
                {
                    num++;
                    Application.DoEvents();
                    if (num > 5)
                    {
                        break;
                    }
                }
            }
            int valueOld = 1;
            ConstantMethod.DelayWriteCmdOk(500, ref valueOld, ref emgStopInPs);
            try
            {
                if (emgStopInPs.ShowValue == valueOld)
                {
                    showWorkInfo(Constant.emgstopOk);
                    return true;
                }
                showWorkInfo(Constant.emgstopWrong);
            }
            catch (Exception)
            {
            }
            finally
            {
                stopOperation();
                string[] logs = new string[] { DeviceName + Constant.DeviceEmgStop };
                LogManager.WriteProgramLog(logs);
            }
            return false;
        }

        private void enterRunning()
        {
            int showValue = -1;
            CountClr();
            while (IsRuninng)
            {
                Application.DoEvents();
                if ((showValue != cutDoneOutInPs.ShowValue) && (cutDoneOutInPs.ShowValue < HoleDataLst.Count))
                {
                    showValue = cutDoneOutInPs.ShowValue;
                    if (updateData !=null)
                    {
                        updateData(HoleDataLst[showValue].TableName);
                    }
                }
                if (cutDoneOutInPs.ShowValue == HoleDataLst.Count)
                {
                    IsRuninng = false;
                    if (updateData !=null)
                    {
                        updateData("");
                    }
                    break;
                }
            }
        }

        private bool errorStatus(string str) { return (str.Contains(xialiaojuStatus0[4]) || str.Contains(xialiaojuStatus0[3])); } 


        private string FindBarCodeFile(string barcode)
        {
            string path = ReadBarCodeSourceFolder();
            if (!Directory.Exists(path))
            {
                MessageBox.Show(Constant.barCodeError);
                return null;
            }
            string[] files = Directory.GetFiles(path, "*.nc");
            foreach (string str3 in files)
            {
               // string str4 = Path.GetFileName(str3);
               // str4=str4.Replace('_',' ');
               // str4=str4.Replace('-', ' ').Trim();
               // str4 = str4.Replace(" ", "");
                if (str3.Contains(barcode))
                {
                    return str3;
                }
            }
            MessageBox.Show(Constant.barCodeError);
            return "";
        }

        private void FindPlcInfo(PlcInfoSimple p, List<XJPlcInfo> dplc, List<List<XJPlcInfo>> mplc)
        {
            if ((p.Area != null) && ((dplc != null) && (mplc != null)))
            {
                foreach (XJPlcInfo info in dplc)
                {
                    if ((info.RelAddr == p.Addr) && info.StrArea.Equals(p.Area.Trim()))
                    {
                        p.SetPlcInfo(info);
                        return;
                    }
                }
                for (int i = 0; i < mplc.Count; i++)
                {
                    for (int j = 0; j < mplc[i].Count; j++)
                    {
                        if ((mplc[i][j].RelAddr == p.Addr) && mplc[i][j].StrArea.Equals(p.Area.Trim()))
                        {
                            p.SetPlcInfo(mplc[i][j]);
                            break;
                        }
                    }
                }
            }
        }

        private void FindPlcSimpleInPlcInfoLst(int m)
        {
            foreach (List<PlcInfoSimple> list in AllPlcSimpleLst)
            {
                foreach (PlcInfoSimple simple in list)
                {
                    FindPlcInfo(simple, evokDevice.DPlcInfo, evokDevice.MPlcInfoAll);
                }
            }
        }

        public DataTable GetDataForm(int id)
        {
            if (id < DataFormCount)
            {
                return evokDevice.DataForm;
            }
            return null;
        }

        private bool getDeviceStatusIsReady(int id)
        {
            switch (id)
            {
                case 0:
                    return (getXialiaoJuStatus().Equals(xialiaojuStatus0[1]) && !isDownLoading(id));

                case 1:
                    return (getDoorBanStatus().Equals(xialiaojuStatus0[1]) && !isDownLoading(id));

                case 2:
                    return (getDoorShellStatus().Equals(xialiaojuStatus0[1]) && !isDownLoading(id));
            }
            return false;
        }

        public string getDoorBanStatus() { return GetStatus(doorbanCurrentStatusInPs, doorBanStatus0); }


        public string getDoorShellStatus() { return GetStatus(doorshellCurrentStatusInPs, doorShellStatus0); }
     

        public OptSize getOptSize() { return optSize; }   

        private List<SimiPatternPoint> GetPatternAllPos(double pos, double len, patternSize pS)
        {
            List<SimiPatternPoint> list = new List<SimiPatternPoint>();

            double xiepoWidth = pS.xiepoWidth * Constant.dataMultiple; 
            double patternWith = pS.patternWith * Constant.dataMultiple; 
            double patternHeight = pS.patternHeight * Constant.dataMultiple; 
            double xBottomMargin = pS.XBottomMargin * Constant.dataMultiple; 
            double yBottomMargin = pS.YBottomMargin * Constant.dataMultiple; 
            double xNOPatternWidth = pS.XNOPatternWidth * Constant.dataMultiple; 
            PointDouble num7 = new PointDouble
            {
                X =getOptSize().Ltbc+pos - xiepoWidth,
                Y = yBottomMargin
            };
            while (true)
            {
                SimiPatternPoint item = new SimiPatternPoint {
                    leftDown = {
                        X = num7.X,
                        Y = num7.Y
                    }
                };
                item.leftUp.X = item.leftDown.X;
                item.leftUp.Y = (item.leftDown.Y + patternHeight) + (xiepoWidth * 2.0);
                item.rightUp.X = (item.leftUp.X + patternWith) + (xiepoWidth * 2.0);
                item.rightUp.Y = item.leftUp.Y;
                item.rightDown.X = item.rightUp.X;
                item.rightDown.Y = item.leftDown.Y;
                if (item.rightDown.X >= len)
                {
                    return list;
                }
                list.Add(item);
                num7.X = xNOPatternWidth + item.rightDown.X;
            }
        }

        private PlcInfoSimple getPsFromPslLst(string tag0, string str0, List<PlcInfoSimple> pslLst)
        {
            foreach (PlcInfoSimple simple in pslLst)
            {
                string str = tag0;
                string name = simple.Name;
                if (name.Contains(str0))
                {
                    name = name.Replace(Constant.Write, "").Replace(Constant.Read, "");
                    if (str.Equals(name))
                    {
                        return simple;
                    }
                }
            }
            return null;
        }

        private int GetScar(OptSize op, int scarCount)
        {
            if (rtbResult !=null)
            {
                rtbResult.Clear();
            }
            if (!testGetScarData(evokDevice.DataFormLst[Constant.ScarPage]))
            {
                return 1;
            }
            if (scarCount == evokDevice.DataFormLst[Constant.ScarPage].Rows.Count)
            {
                op.ScarLst.Clear();
                ConstantMethod.ShowInfo(rtbResult, Constant.ScarName + Constant.sumStr + ":  " + ((scarCount / 2)).ToString());
                foreach (DataRow row in evokDevice.DataFormLst[Constant.ScarPage].Rows)
                {
                    string s = row[4].ToString();
                    int result = 0;
                    if (int.TryParse(s, out result))
                    {
                        if (result <= 0)
                        {
                            return 1;
                        }
                        ConstantMethod.ShowInfo(rtbResult, Constant.ScarName + Constant.posStr + ":  " + result.ToString());
                        op.ScarLst.Insert(0, result);
                    }
                    else
                    {
                        return 1;
                    }
                }
            }
            return 0;
        }

        public bool GetStartStatus()
        {
            if (startInPs !=null)
            {
                int valueOld = Constant.M_ON;
                ConstantMethod.DelayWriteCmdOk(Constant.StartWaitMaxTime, ref valueOld, ref startInPs, ref emgStopInPs);
                return (startInPs.ShowValue == Constant.M_ON);
            }
            return true;
        }

        public string GetStatus(PlcInfoSimple ps, string[] statusLst)
        {
            BitArray array = ConstantMethod.getBitValueInByteLst(ps.ShowValue, 0, 0);
            if (array.Count >= statusLst.Count<string>())
            {
                for (int i = 0; i < statusLst.Count<string>(); i++)
                {
                    if (array[i])
                    {
                        return statusLst[i];
                    }
                }
            }
            return Constant.noData;
        }

        public string getXialiaoJuStatus() { return GetStatus(xialiaoCurrentStatusInPs, xialiaojuStatus0); }


        public bool HandParamTxt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                double num;
                if (double.TryParse(((TextBox)sender).Text, out num) && (num > -1.0))
                {
                    SetDValue(((TextBox)sender).Tag.ToString(), Constant.Write, PsLstHand, ((TextBox)sender).Text);
                }
                return true;
            }
            return false;
        }

        private void InitBarCodeTimer()
        {
            new EnvironmentSettings().ReportSettings.ShowProgress = false;
            tCheckPrint = new System.Timers.Timer(200.0);
            tCheckPrint.Enabled = false;
            tCheckPrint.AutoReset = true;
           // tCheckPrint.Elapsed += new ElapsedEventHandler(printTimerCheck);
        }

        public void InitControl()
        {
            if ((evokDevice.DataFormLst.Count > 0) && (evokDevice.DataFormLst[0] !=null))
            {
                ConstantMethod.FindPos(evokDevice.DataFormLst[0], PsLstAuto);
            }
            if ((evokDevice.DataFormLst.Count > 0) && (evokDevice.DataFormLst[1] !=null))
            {
                ConstantMethod.FindPos(evokDevice.DataFormLst[1], PsLstHand);
            }
            if ((evokDevice.DataFormLst.Count > 0) && (evokDevice.DataFormLst[2] !=null))
            {
                ConstantMethod.FindPos(evokDevice.DataFormLst[2], PsLstParam);
            }
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
            SetPage(0);
            printMiniSizeOutInPs = ConstantMethod.getPlcSimple(printMiniSizeOutInPs.Name, psLstAuto);
            wlMiniSizeOutInPs = ConstantMethod.getPlcSimple(wlMiniSizeOutInPs.Name, psLstAuto);
            autoMesOutInPs = ConstantMethod.getPlcSimple(autoMesOutInPs.Name, psLstAuto);
            dbcOutInPs = ConstantMethod.getPlcSimple(dbcOutInPs.Name, psLstAuto);
            ltbcOutInPs = ConstantMethod.getPlcSimple(ltbcOutInPs.Name, psLstAuto);
            ltbcDefaultOutInPs = ConstantMethod.getPlcSimple(ltbcDefaultOutInPs.Name, psLstAuto);
            safeOutInPs = ConstantMethod.getPlcSimple(safeOutInPs.Name, psLstAuto);
            prodOutInPs = ConstantMethod.getPlcSimple(prodOutInPs.Name, psLstAuto);
            prodOutInPs.IsParam = false;
            if(thisLenGoOutOutInPs!=null)
            thisLenGoOutOutInPs= ConstantMethod.getPlcSimple(thisLenGoOutOutInPs.Name, psLstAuto);
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
            widht1InOutPs = ConstantMethod.getPlcSimple(widht1InOutPs.Name, psLstAuto);
            widht2InOutPs = ConstantMethod.getPlcSimple(widht2InOutPs.Name, psLstAuto);
            restMaterialRangeInOutPs = ConstantMethod.getPlcSimple(restMaterialRangeInOutPs.Name, psLstAuto);
            inspectPatternModeInOutPs = ConstantMethod.getPlcSimple(inspectPatternModeInOutPs.Name, psLstAuto);
            inspectPatternPosInOutPs = ConstantMethod.getPlcSimple(inspectPatternPosInOutPs.Name, psLstAuto);
            inspectPatternDoneInOutPs = ConstantMethod.getPlcSimple(inspectPatternDoneInOutPs.Name, psLstAuto);
            materialSetEnInOutPs = ConstantMethod.getPlcSimple(materialSetEnInOutPs.Name, psLstAuto);
            materialIdInOutPs = ConstantMethod.getPlcSimple(materialIdInOutPs.Name, psLstAuto);
            lineStartTipInPs = ConstantMethod.getPlcSimple(lineStartTipInPs.Name, psLstAuto);
            lineStartOutPs = ConstantMethod.getPlcSimple(lineStartOutPs.Name, psLstAuto);
            lineStartInPs = ConstantMethod.getPlcSimple(lineStartInPs.Name, psLstAuto);
            xialiaoCurrentDoorIdInPs = ConstantMethod.getPlcSimple(xialiaoCurrentDoorIdInPs.Name, psLstAuto);
            xialiaoCurrentStatusInPs = ConstantMethod.getPlcSimple(xialiaoCurrentStatusInPs.Name, psLstAuto);
            xialiaoDoorCntInPs = ConstantMethod.getPlcSimple(xialiaoDoorCntInPs.Name, psLstAuto);
            doorSize = ConstantMethod.getPlcSimple(doorSize.Name, psLstAuto);
            doorBanLen = ConstantMethod.getPlcSimple(doorBanLen.Name, psLstAuto);
            doorBanWidth = ConstantMethod.getPlcSimple(doorBanWidth.Name, psLstAuto);
            doorShellLen = ConstantMethod.getPlcSimple(doorShellLen.Name, psLstAuto);
            doorShellWidth = ConstantMethod.getPlcSimple(doorShellWidth.Name, psLstAuto);
            lineResetOutPs = ConstantMethod.getPlcSimple(lineResetOutPs.Name, psLstAuto);
            lineResetInPs = ConstantMethod.getPlcSimple(lineResetInPs.Name, psLstAuto);
            lineStopOutPs = ConstantMethod.getPlcSimple(lineStopOutPs.Name, psLstAuto);
            lineStopInPs = ConstantMethod.getPlcSimple(lineStopInPs.Name, psLstAuto);
            linePauseOutPs = ConstantMethod.getPlcSimple(linePauseOutPs.Name, psLstAuto);
            linePauseInPs = ConstantMethod.getPlcSimple(linePauseInPs.Name, psLstAuto);
            lineEmgStopInPs = ConstantMethod.getPlcSimple(lineEmgStopInPs.Name, psLstAuto);
            lineEmgStopOutPs = ConstantMethod.getPlcSimple(lineEmgStopOutPs.Name, psLstAuto);
            dataOutPs = ConstantMethod.getPlcSimple(dataOutPs.Name, psLstAuto);
            muxiaoHoleOutPs = ConstantMethod.getPlcSimple(muxiaoHoleOutPs.Name, psLstAuto);
            mxkStringShowInPs = ConstantMethod.getPlcSimple(mxkStringShowInPs.Name, psLstAuto);
            zxShowInPs = ConstantMethod.getPlcSimple(zxShowInPs.Name, psLstAuto);
            zkShowInPs = ConstantMethod.getPlcSimple(zkShowInPs.Name, psLstAuto);
            mxkShowInPs = ConstantMethod.getPlcSimple(mxkShowInPs.Name, psLstAuto);
            smxShowInPs = ConstantMethod.getPlcSimple(smxShowInPs.Name, psLstAuto);
            leftRightShowInPs = ConstantMethod.getPlcSimple(leftRightShowInPs.Name, psLstAuto);
            liWaiInPs = ConstantMethod.getPlcSimple(liWaiInPs.Name, psLstAuto);
            zxShowOutPs = ConstantMethod.getPlcSimple(zxShowOutPs.Name, psLstAuto);
            zkShowOutPs = ConstantMethod.getPlcSimple(zkShowOutPs.Name, psLstAuto);
            mxkShowOutPs = ConstantMethod.getPlcSimple(mxkShowOutPs.Name, psLstAuto);
            smxShowOutPs = ConstantMethod.getPlcSimple(smxShowOutPs.Name, psLstAuto);
            leftRightShowOutPs = ConstantMethod.getPlcSimple(leftRightShowOutPs.Name, psLstAuto);
            liWaiOutPs = ConstantMethod.getPlcSimple(liWaiOutPs.Name, psLstAuto);
            doorDownLoadCountInOutPs = ConstantMethod.getPlcSimple(doorDownLoadCountInOutPs.Name, psLstAuto);
            xialiaojuOutPs = ConstantMethod.getPlcSimple(xialiaojuOutPs.Name, psLstAuto);
            xialiaoDataRequestInPs = ConstantMethod.getPlcSimple(xialiaoDataRequestInPs.Name, psLstAuto);
            xialiaoDataDownLoadSuccessOutPs = ConstantMethod.getPlcSimple(xialiaoDataDownLoadSuccessOutPs.Name, psLstAuto);
            xialiaoDoorCountInOutPs = ConstantMethod.getPlcSimple(xialiaoDoorCountInOutPs.Name, psLstAuto);
            doorbanLenthChangInOutPs = ConstantMethod.getPlcSimple(doorbanLenthChangInOutPs.Name, psLstAuto);
            doorbanLenthKuanInOutPs = ConstantMethod.getPlcSimple(doorbanLenthKuanInOutPs.Name, psLstAuto);
            doorbanDataChangRequestInPs = ConstantMethod.getPlcSimple(doorbanDataChangRequestInPs.Name, psLstAuto);
            doorbanDataKuanRequestInPs = ConstantMethod.getPlcSimple(doorbanDataKuanRequestInPs.Name, psLstAuto);
            doorbanDataChangDownLoadSuccessOutPs = ConstantMethod.getPlcSimple(doorbanDataChangDownLoadSuccessOutPs.Name, psLstAuto);
            doorbanDataKuanDownLoadSuccessOutPs = ConstantMethod.getPlcSimple(doorbanDataKuanDownLoadSuccessOutPs.Name, psLstAuto);
            doorbanCurrentStatusInPs = ConstantMethod.getPlcSimple(doorbanCurrentStatusInPs.Name, psLstAuto);
            doorbanCurrentDoorIdInPs = ConstantMethod.getPlcSimple(doorbanCurrentDoorIdInPs.Name, psLstAuto);
            doorbanDoorCountInOutPs = ConstantMethod.getPlcSimple(doorbanDoorCountInOutPs.Name, psLstAuto);
            doorshellLenthChangInOutPs = ConstantMethod.getPlcSimple(doorshellLenthChangInOutPs.Name, psLstAuto);
            doorshellLenthKuanInOutPs = ConstantMethod.getPlcSimple(doorshellLenthKuanInOutPs.Name, psLstAuto);
            doorshellDataChangRequestInPs = ConstantMethod.getPlcSimple(doorshellDataChangRequestInPs.Name, psLstAuto);
            doorshellDataKuanRequestInPs = ConstantMethod.getPlcSimple(doorshellDataKuanRequestInPs.Name, psLstAuto);
            doorshellDataChangDownLoadSuccessOutPs = ConstantMethod.getPlcSimple(doorshellDataChangDownLoadSuccessOutPs.Name, psLstAuto);
            doorshellDataKuanDownLoadSuccessOutPs = ConstantMethod.getPlcSimple(doorshellDataKuanDownLoadSuccessOutPs.Name, psLstAuto);
            doorshellCurrentDoorIdInPs = ConstantMethod.getPlcSimple(doorshellCurrentDoorIdInPs.Name, psLstAuto);
            doorshellCurrentStatusInPs = ConstantMethod.getPlcSimple(doorshellCurrentStatusInPs.Name, psLstAuto);
            doorshellDoorIdInOutPs = ConstantMethod.getPlcSimple(doorshellDoorIdInOutPs.Name, psLstAuto);
            doorshellDataDoorIdRequestInPs = ConstantMethod.getPlcSimple(doorshellDataDoorIdRequestInPs.Name, psLstAuto);
            doorshellDataDoorIdDownLoadSuccessOutPs = ConstantMethod.getPlcSimple(doorshellDataDoorIdDownLoadSuccessOutPs.Name, psLstAuto);
            doorshellDoorCountInOutPs = ConstantMethod.getPlcSimple(doorshellDoorCountInOutPs.Name, psLstAuto);
            pos1OutInPs = ConstantMethod.getPlcSimple(pos1OutInPs.Name, psLstAuto);
            pos2OutInPs = ConstantMethod.getPlcSimple(pos2OutInPs.Name, psLstAuto);
            pos1EnOutPs = ConstantMethod.getPlcSimple(pos1EnOutPs.Name, psLstAuto);
            pos2EnOutPs = ConstantMethod.getPlcSimple(pos2EnOutPs.Name, psLstAuto);
            pos1EnInPs = ConstantMethod.getPlcSimple(pos1EnInPs.Name, psLstAuto);
            pos2EnInPs = ConstantMethod.getPlcSimple(pos2EnInPs.Name, psLstAuto);
            posMode = ConstantMethod.getPlcSimple(posMode.Name, psLstAuto);
            AutoPosDataOutInPs = ConstantMethod.getPlcSimple(AutoPosDataOutInPs.Name, psLstAuto);
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

            sdjDataDownLoadEnd =ConstantMethod.getPlcSimple(sdjDataDownLoadEnd.Name, psLstAuto);
            sdjLeftDataAddress = ConstantMethod.getPlcSimple(sdjLeftDataAddress.Name, psLstAuto);
            sdjRightDataAddress = ConstantMethod.getPlcSimple(sdjRightDataAddress.Name, psLstAuto);
            sdjLeftEnable = ConstantMethod.getPlcSimple(sdjLeftEnable.Name, psLstAuto);
            sdjRightEnable = ConstantMethod.getPlcSimple(sdjRightEnable.Name, psLstAuto);
            SetPage(1);
            SetPage(2);
            SetPage(3);
            ParamFile = ConstantMethod.configFileBak(Constant.ConfigParamFilePath);
            if (!int.TryParse(ParamFile.ReadConfig(Constant.printBarcodeMode), out printBarCodeMode))
            {
                MessageBox.Show(Constant.ErrorParamConfigFile);
                Application.Exit();
                Environment.Exit(0);
            }
            CutSelMode = ReadCutMode();
            minPrinterName = ParamFile.ReadConfig(Constant.minPrinterName);
            string[] logs = new string[] { DeviceName + Constant.Start };

            LogManager.WriteProgramLog(logs);

            InitControl();
            if (!evokDevice.getDeviceData())
            {
                MessageBox.Show(DeviceName + Constant.ConnectMachineFail);
            }

            DeviceUser = ParamFile.ReadConfig(Constant.DeviceUser);

            ShiftPage(0);

            ErrorList = new List<string>();
            WarningList = new List<string>();
            optSize = new OptSize();          

            InitBarCodeTimer();

           

            string isarrange = ParamFile.ReadConfig(Constant.OptMode);

            if (isarrange.Equals(Constant.OptModeOptNo))
            {
                IsArrangeOpt = true;
            }
            else
            {
                IsArrangeOpt = false;
            }

            SetSplitData();

        }

        public void SetIsArrangeOpt(bool s)
        {
            if (s)
            {
                IsArrangeOpt = true;
                ParamFile.WriteConfig(Constant.OptMode, Constant.OptModeOptNo);
            }
            else
            {
                IsArrangeOpt = false;
                ParamFile.WriteConfig(Constant.OptMode, Constant.OptModeOpt);

            }
        }
        //Ŀǰ�ֵ�����˾�׵ĸ��ݵ��� �������� ����ĳһ�в��ϲ�ͬ��������
        //���ݲ���Ҫ���зָ�� �Ǽ��� 
        void SetSplitData()
        {
           List<string>  paramStr = new List<string>();

           int i = 1;
           string s = "";
           while (!string.IsNullOrWhiteSpace(s = ParamFile.ReadConfig(Constant.strParam + i.ToString())))
           {
                string ss = s;
                paramStr.Add(ss);
                i++;
            }

            if(paramStr.Count>0)
            optSize.SplitParam = paramStr;




        }
        public string simi_SQL_DataTableName;
        void InitSimi()
        {
            if (DeviceName.Equals(Constant.simiDeivceName))
            {
                pSize = new patternSize();
                pSize.xiepoWidth = 4.0;
                pSize.patternWith = 12.0;
                pSize.patternHeight = 12.0;
                pSize.XBottomMargin = 2.5;
                pSize.YBottomMargin = 2.5;
                pSize.XNOPatternWidth = 8.5;
                optSize.PsSize = pSize;

                InitSql
                (
                   ParamFile.ReadConfig(Constant.SQL_ServerName),
                   ParamFile.ReadConfig(Constant.SQL_DatabaseName),
                   ParamFile.ReadConfig(Constant.SQL_UserName),
                   ParamFile.ReadConfig(Constant.SQL_Passwd)
                );

                simi_SQL_DataTableName = ParamFile.ReadConfig(Constant.SQL_Tablename);

                ReadSimiWlst();

                InitSimiSplit();

                InitPaint();

            }
        }
        PictureBox showCutPictureBox;
        public System.Windows.Forms.PictureBox ShowCutPictureBox
        {
            get { return showCutPictureBox; }
            set { showCutPictureBox = value; }
        }
        //��ʾ��ǰ�и��ͼƬ
        PictureBox showCurrentCutPictureBox;
        public System.Windows.Forms.PictureBox ShowCurrentCutPictureBox
        {
            get { return showCurrentCutPictureBox; }
            set { showCurrentCutPictureBox = value; }
        }


        void InitPaint()
        {

            Point p = new Point(10, 60);
            Simi_painR = new PaintRect(1000, p);
            if (optSize.ProdInfoLst.Count > 0)
            Simi_painR.SetRation(optSize.ProdInfoLst[0]);
            

        }

        public void Simi_Show(int id)
        {
            if (DeviceName.Equals(Constant.simiDeivceName)&&optSize.ProdInfoLst.Count > id && id >= 0  && showCutPictureBox !=null)
            {
                Bitmap btmap = null;
                // for (int i = 0; i < op.ProdInfoLst.Count; i++)
                // {
                Simi_painR.ProdDrawPloygon(optSize.ProdInfoLst[id], ref btmap,0, showCutPictureBox, id);
               // }

            }
        }
        public void Simi_Show(int id,int currentIndexId)
        {
            if (DeviceName.Equals(Constant.simiDeivceName) && optSize.ProdInfoLst.Count > id && id >= 0 && ShowCurrentCutPictureBox != null)
            {
                Bitmap btmap = null;
                // for (int i = 0; i < op.ProdInfoLst.Count; i++)
                // {
                Simi_painR.ProdDrawPloygon(optSize.ProdInfoLst[id], ref btmap, 0, ShowCurrentCutPictureBox, id, currentIndexId);
                // }

            }
        }

        void InitSimiSplit()
        {
            string splitcount= ParamFile.ReadConfig(Constant.SplitCount);
            int splitC = 0;
            if (int.TryParse(splitcount, out splitC))
            {
                if(splitC<Constant.SplitMinTaskCount)
                {
                    splitC = Constant.SplitMinTaskCount;
                }
                optSize.Simi_Splitcount = splitC;
            }
        }

        public void SetSimiSplitCount(int splitcount)
        {          
            ParamFile.WriteConfig(Constant.SplitCount,splitcount.ToString());
            optSize.Simi_Splitcount = splitcount;
        }
        //����˾�׽Ƕȹ���
        public void SetSimiTloranceAngle()
        {
            double tolerance = 0;
            string s =
            ParamFile.ReadConfig(Constant.ToleranceAngle);

            if (double.TryParse(s, out tolerance))
            {
                optSize.ToleranceAngle = tolerance;
            }


        }
        public void ReadSimiWlst()
        {
           
           List<int> wldata = new List<int>();
            for (int i = 0; i < Constant.MaxWlNearCount; i++)
            {

                int intwl = 0;
                string s=
                ParamFile.ReadConfig(Constant.WlNear1Str+i.ToString());


                if (string.IsNullOrWhiteSpace(s))   //�����հ׵� ֤����û���� �Ǿ�
                {
                    break;
                }
                if (int.TryParse(s,out intwl))
                {
                    if (intwl >= Constant.MaxWlNearSize)
                    {
                        wldata.Add(intwl);
                    }
                }
            }

            optSize.SetWl(wldata.ToArray());


        }
        void InitSql(string serverName,string databaseName,string userName,string passwd)
        {


            lo_conn = new SqlConnection("Server=" + serverName + ";Database=" + databaseName + ";uid=" + userName + ";pwd=" + passwd);
          //  com = lo_conn.CreateCommand();   //�����������

            try
            {
                lo_conn.Open();

                 DataTable dt = lo_conn.GetSchema("Tables");
                ConstantMethod.ShowInfo(rtbWork,"���ݿ����ӳɹ���");
                //showInfo(databaseName + "�������¼������");
                /**
                foreach (DataRow row in dt.Rows)
                {
                    //�õ�����
                    string table = row[2].ToString();

                    //���ֱ������������ݿ��µ����б�����ֱ����ӣ�
                    showInfo(table);
                }

                ***/
                //MessageBox.Show("��¼�ɹ�");
            }
            catch (Exception ex)
            {
                MessageBox.Show("���ݿ��¼ʧ�ܣ�����" + ex.Message);
            }


        }

        public bool isDownLoading(int id)
        {
            switch (id)
            {
                case 0:
                    return (runflag_XialiaoJu || getXialiaoJuStatus().Equals(xialiaojuStatus0[0]));

                case 1:
                    return ((runflag_DoorBanKuan || runflag_DoorBanChang) || getDoorBanStatus().Equals(xialiaojuStatus0[0]));

                case 2:
                    return (((runflag_DoorShellChang || runflag_DoorShellDoorId) || runflag_DoorShellKuan) || getDoorShellStatus().Equals(xialiaojuStatus0[0]));
            }
            return false;
        }

        public bool IsLineReady(List<int> deviceIdLst)
        {
            if (deviceIdLst.Contains(0) && !getDeviceStatusIsReady(0))
            {
                MessageBox.Show(Constant.tataLineDeviceName[0] + Constant.errorDeviceStatus);
                return false;
            }
            if (deviceIdLst.Contains(1) && !getDeviceStatusIsReady(1))
            {
                MessageBox.Show(Constant.tataLineDeviceName[1] + Constant.errorDeviceStatus);
                return false;
            }
            if (deviceIdLst.Contains(2) && !getDeviceStatusIsReady(2))
            {
                MessageBox.Show(Constant.tataLineDeviceName[2] + Constant.errorDeviceStatus);
                return false;
            }
            return true;
        }

        public bool IsMaterialExist() { return (optSize.MaterialId > 0); } 
          

        public bool IsPause()
        { 
               
                   return  (pauseInPs.ShowValue == 1);             
        } 
           

        public void IsShow(Label lbl)
        {
            showMxkSel(lbl);
            mxkShowInPs.visibleControl();
            zxShowInPs.visibleControl();
            zkShowInPs.visibleControl();
            smxShowInPs.visibleControl();
            mxkShowInPs.visibleControl();
            leftRightShowInPs.visibleControl();
            liWaiInPs.visibleControl();
            showDataWith2Ps(zxShowInPs, zxShowOutPs);
            showDataWith2Ps(zkShowInPs, zkShowOutPs);
            showDataWith2Ps(smxShowInPs, smxShowOutPs);
            showDataWith2Ps(leftRightShowInPs, leftRightShowOutPs);
            showDataWith2Ps(liWaiInPs, liWaiOutPs);
        }

        public bool LinedownLoadData(PlcInfoSimple stp, OptSize op, ref int pos, int count, int id)
        {
            if (pos >= op.ProdInfoLst.Count)
            {
                return false;
            }
            List<int> list = new List<int>();
            int addr = stp.Addr;
            int num2 = op.ProdInfoLst.Count - pos;
            int num3 = (num2 > count) ? count : num2;
            if ((id == 1) || (id == 2))
            {
                string[] logs = new string[] { "���Ͼ�/��о�峤���ݴ�����ܹ�" + ((int)(pos + num3)).ToString() + "��������" };
                LogManager.WriteProgramLog(logs);
                for (int i = pos; i < (pos + num3); i++)
                {
                    list.Clear();
                    list.Add(op.ProdInfoLst[i].WL);
                    list.Add(op.ProdInfoLst[i].Cut.Count);
                    list.AddRange(op.ProdInfoLst[i].Cut);
                    if (op.ProdInfoLst[i].Cut.Count > 13)
                    {
                        string[] textArray2 = new string[] { "���Ͼ�/��о�峤���ݹ��࣬ȡ������" };
                        LogManager.WriteProgramLog(textArray2);
                        return false;
                    }
                    if (!evokDevice.SetMultiPleDValue(stp, list.ToArray()))
                    {
                        evokDevice.SetMultiPleDValue(stp, list.ToArray());
                    }
                    string[] textArray3 = new string[] { "���Ͼ�/��о�峤���ݷ������" + i.ToString() };
                    LogManager.WriteProgramLog(textArray3);
                    stp.Addr = addr + (((i - pos) + 1) * 30);
                    stp.Addr -= 2;
                    int result = 0;
                    if (int.TryParse(op.ProdInfoLst[i].Param10[0].ToString(), out result))
                    {
                        evokDevice.SetDValue(stp, result);
                        string[] textArray4 = new string[] { "���Ͼ�/��о�峤����ID�������" + i.ToString() };
                        LogManager.WriteProgramLog(textArray4);
                    }
                    else
                    {
                        return false;
                    }
                    stp.Addr += 2;
                }
                stp.Addr = addr + 400;
                evokDevice.SetDValue(stp, num3);
            }
            if ((id == 3) || (id == 5))
            {
                string[] textArray5 = new string[] { "�Ű�/��Ƥ���ݴ����һ�����·�" };
                LogManager.WriteProgramLog(textArray5);
                list.Clear();
                list.Add(num3);
                for (int j = pos; j < (pos + num3); j++)
                {
                    if (op.ProdInfoLst[j].Cut.Count != 1)
                    {
                        return false;
                    }
                    int num8 = 0;
                    if ((op.ProdInfoLst[pos].Param1.Count > 0) && !int.TryParse(op.ProdInfoLst[j].Param1[0], out num8))
                    {
                        string[] textArray6 = new string[] { "�Ű�/��Ƥ����������" };
                        LogManager.WriteProgramLog(textArray6);
                        return false;
                    }
                    list.Add(num8 * Constant.dataMultiple);
                }
                if (!evokDevice.SetMultiPleDValue(stp, list.ToArray()))
                {
                    evokDevice.SetMultiPleDValue(stp, list.ToArray());
                }
                string[] textArray7 = new string[] { "�Ű�/��Ƥ��� ���ݷ������" };
                LogManager.WriteProgramLog(textArray7);
            }
            if (id == 4)
            {
                string[] textArray8 = new string[] { "��Ƥ�����ݴ��,һ�����·�" };
                LogManager.WriteProgramLog(textArray8);
                list.Clear();
                list.Add(num3);
                for (int k = pos; k < (pos + num3); k++)
                {
                    list.AddRange(op.ProdInfoLst[k].Cut);
                }
                if (!evokDevice.SetMultiPleDValue(stp, list.ToArray()))
                {
                    evokDevice.SetMultiPleDValue(stp, list.ToArray());
                }
                string[] textArray9 = new string[] { "��Ƥ�����ݷ������" };
                LogManager.WriteProgramLog(textArray9);
            }
            if (id == 6)
            {
                list.Clear();
                list.Add(num3);
                string[] textArray10 = new string[] { "��ƤID���ݴ����һ�����·�" };
                LogManager.WriteProgramLog(textArray10);
                for (int m = pos; m < (pos + num3); m++)
                {
                    if (op.ProdInfoLst[m].Cut.Count != 1)
                    {
                        return false;
                    }
                    int num11 = 0;
                    if ((op.ProdInfoLst[pos].Param1.Count > 0) && !int.TryParse(op.ProdInfoLst[m].Param10[0], out num11))
                    {
                        MessageBox.Show("����������");
                        return false;
                    }
                    list.Add(num11);
                }
                if (!evokDevice.SetMultiPleDValue(stp, list.ToArray()))
                {
                    evokDevice.SetMultiPleDValue(stp, list.ToArray());
                }
                string[] textArray11 = new string[] { "��ƤID���ݷ������" };
                LogManager.WriteProgramLog(textArray11);
            }
            Thread.Sleep(100);
            stp.Addr = addr;
            pos += num3;
            return true;
        }

        public void LineEmgStop()
        {
            evokDevice.SetMValueON2OFF(lineEmgStopOutPs);
            string[] logs = new string[] { "�߼�ͣ�źŷ��ͣ�" };
            LogManager.WriteProgramLog(logs);
        }

        public void LinePause()
        {
            evokDevice.SetMValueON2OFF(linePauseOutPs);
            string[] logs = new string[] { "����ͣ�źŷ��ͣ�" };
            LogManager.WriteProgramLog(logs);
        }

        public void LineReset()
        {
            evokDevice.SetMValueON2OFF(lineResetOutPs);
            string[] logs = new string[] { "�߸�λ�źŷ��ͣ�" };
            LogManager.WriteProgramLog(logs);
        }

        public void LineStart()
        {
            ConstantMethod.Delay(0xbb8);
            evokDevice.SetMValueON2OFF(lineStartOutPs);
            string[] logs = new string[] { "��PLC������������" };
            LogManager.WriteProgramLog(logs);
        }

        public void LineStop()
        {
            evokDevice.SetMValueON2OFF(lineStopOutPs);
            string[] logs = new string[] { "��ֹͣ�źŷ��ͣ�" };
            LogManager.WriteProgramLog(logs);
            RunFlag = false;
            xialiaoSingleStop();
            doorBanSingleStop();
            doorShellSingleStop();
        }

        public void ListSmallSizePrinter(ComboBox cb)
        {
            List<string> localPrinter = ConstantMethod.GetLocalPrinter();
            cb.DataSource = localPrinter;
            if (localPrinter.Contains(minPrinterName))
            {
                cb.Text = minPrinterName;
            }
            else
            {
                cb.Text = ConstantMethod.DefaultPrinter;
            }
        }

        public void lliaoOFF()
        {
            evokDevice.SetMValueOFF(lliaoOutInPs);
        }

        public void lliaoON()
        {
            evokDevice.SetMValueON(lliaoOutInPs);
        }
        public bool SetLen(int lenvalue)
        {
                
            return evokDevice.SetDValue(lcOutInPs, lenvalue);
        }
        public bool LoadCsvData(string filename)
        {
            if (lcOutInPs !=null)
            {
                optSize.Len = lcOutInPs.ShowValue;
            }
            if (dbcOutInPs !=null)
            {
                optSize.Dbc = dbcOutInPs.ShowValue;
            }
            if (ltbcOutInPs !=null)
            {
                optSize.Ltbc = ltbcOutInPs.ShowValue;
            }
            if (safeOutInPs !=null)
            {
                optSize.Safe = safeOutInPs.ShowValue;
            }

            
            optSize.WlMiniValue = wlMiniSizeOutInPs.ShowValue;

            showPath(filename);

            return optSize.LoadCsvData(filename);

        }
        #region ͨ��ʽ˫�˾�
        public void Sdj_DownLoadData(List<HySkCutParam> hyskLst)
        {
         
            List<int> aLst = new List<int>();
            List<int> eLst = new List<int>();
            List<int> fleftLst = new List<int>();
            List<int> gleftLst = new List<int>();
            List<int> driilDepthLst = new List<int>();
            List<int> knifeLst = new List<int>();
            List<int> cutmodeLst = new List<int>();
            List<int> jDepthLst = new List<int>();
            List<int> roundSWLst = new List<int>();

            if (hyskLst.Count <=0) return;
                #region 
                //����·�
            for (int i = 0; i<hyskLst.Count;i++)
            {
                if (hyskLst[i].Dir == 0)
                {
                    aLst.Add((int)(hyskLst[i].PosX*1000));
                    eLst.Add((int)(hyskLst[i].PosY * 1000));
                    fleftLst.Add((int)(hyskLst[i].Hysk.Len * 1000));
                    gleftLst.Add((int)(hyskLst[i].Hysk.Width * 1000));
                    driilDepthLst.Add((int)(hyskLst[i].DrillDepth * 1000));
                    knifeLst.Add((int)(hyskLst[i].Knife));
                    cutmodeLst.Add((int)(hyskLst[i].CutMode));
                    jDepthLst.Add((int)(hyskLst[i].JdDepth));
                    roundSWLst.Add((int)(hyskLst[i].RoundSwitch));
                }
            }
            int address = sdjLeftDataAddress.Addr;
           
            evokDevice.SetMultiPleDValue(sdjLeftDataAddress,aLst.ToArray());
            sdjLeftDataAddress.Addr += 30;
            evokDevice.SetMultiPleDValue(sdjLeftDataAddress, eLst.ToArray());
            sdjLeftDataAddress.Addr += 30;
            evokDevice.SetMultiPleDValue(sdjLeftDataAddress, fleftLst.ToArray());
            sdjLeftDataAddress.Addr += 30;
            evokDevice.SetMultiPleDValue(sdjLeftDataAddress, gleftLst.ToArray());
            sdjLeftDataAddress.Addr += 30;
            evokDevice.SetMultiPleDValue(sdjLeftDataAddress, driilDepthLst.ToArray());
            sdjLeftDataAddress.Addr += 30;
            evokDevice.SetMultiPleDValue(sdjLeftDataAddress, knifeLst.ToArray());
            sdjLeftDataAddress.Addr += 30;
            evokDevice.SetMultiPleDValue(sdjLeftDataAddress, cutmodeLst.ToArray());
            sdjLeftDataAddress.Addr += 30;
            evokDevice.SetMultiPleDValue(sdjLeftDataAddress, jDepthLst.ToArray());
            sdjLeftDataAddress.Addr += 30;
            evokDevice.SetMultiPleDValue(sdjLeftDataAddress, roundSWLst.ToArray());
            sdjLeftDataAddress.Addr += 30;

            int addressEn = sdjLeftEnable.Addr;
            for (int i = 0; i < 6; i++)
            {
              

                if (i < hyskLst.Count)
                {

                    evokDevice.SetMValueON(sdjLeftEnable);
                }
                else
                {
                    evokDevice.SetMValueOFF(sdjLeftEnable);
                }

                sdjLeftEnable.Addr++;
            }
            sdjLeftEnable.Addr = addressEn;

            #endregion
            #region 
            aLst.Clear(); 
            eLst.Clear(); 
            fleftLst.Clear(); 
            gleftLst.Clear(); 
            driilDepthLst.Clear(); 
            knifeLst.Clear(); 
            cutmodeLst.Clear(); 
            jDepthLst.Clear(); 
            roundSWLst.Clear();

            //�ұ��·�
            sdjLeftDataAddress.Addr = address + 300;

            for (int i = 0; i < hyskLst.Count; i++)
            {
                if (hyskLst[i].Dir == 1)
                {
                    aLst.Add((int)(hyskLst[i].PosX * 1000));
                    eLst.Add((int)(hyskLst[i].PosY * 1000));
                    fleftLst.Add((int)(hyskLst[i].Hysk.Len * 1000));
                    gleftLst.Add((int)(hyskLst[i].Hysk.Width * 1000));
                    driilDepthLst.Add((int)(hyskLst[i].DrillDepth * 1000));
                    knifeLst.Add((int)(hyskLst[i].Knife));
                    cutmodeLst.Add((int)(hyskLst[i].CutMode));
                    jDepthLst.Add((int)(hyskLst[i].JdDepth));
                    roundSWLst.Add((int)(hyskLst[i].RoundSwitch));
                }
            }

           

            evokDevice.SetMultiPleDValue(sdjLeftDataAddress, aLst.ToArray());
            sdjLeftDataAddress.Addr += 30;
            evokDevice.SetMultiPleDValue(sdjLeftDataAddress, eLst.ToArray());
            sdjLeftDataAddress.Addr += 30;
            evokDevice.SetMultiPleDValue(sdjLeftDataAddress, fleftLst.ToArray());
            sdjLeftDataAddress.Addr += 30;
            evokDevice.SetMultiPleDValue(sdjLeftDataAddress, gleftLst.ToArray());
            sdjLeftDataAddress.Addr += 30;
            evokDevice.SetMultiPleDValue(sdjLeftDataAddress, driilDepthLst.ToArray());
            sdjLeftDataAddress.Addr += 30;
            evokDevice.SetMultiPleDValue(sdjLeftDataAddress, knifeLst.ToArray());
            sdjLeftDataAddress.Addr += 30;
            evokDevice.SetMultiPleDValue(sdjLeftDataAddress, cutmodeLst.ToArray());
            sdjLeftDataAddress.Addr += 30;
            evokDevice.SetMultiPleDValue(sdjLeftDataAddress, jDepthLst.ToArray());
            sdjLeftDataAddress.Addr += 30;
            evokDevice.SetMultiPleDValue(sdjLeftDataAddress, roundSWLst.ToArray());
            sdjLeftDataAddress.Addr += 30;

            addressEn = sdjRightEnable.Addr;
            for (int i = 0; i < 6; i++)
            {
                

                if (i < hyskLst.Count)
                {

                    evokDevice.SetMValueON(sdjRightEnable);
                }
                else
                {
                    evokDevice.SetMValueOFF(sdjRightEnable);
                }

                sdjRightEnable.Addr++;
            }
            sdjRightEnable.Addr = addressEn;
            #endregion
            sdjLeftDataAddress.Addr = address ;

            evokDevice.SetMValueON(sdjDataDownLoadEnd);

        }
        #endregion
        /***
         * ����/����=����
        1.����ֻ��ȡ1��2��3��4��5��6��8��10��������ȡ��ֵ
        2.������Ϊ��λ��ʱ������ֻȡ1��2��3��4��5��6��8��10
        3.������Ϊ��λ��ʱ������ֻȡ5��6��8��10
        4.����/����=����������������С��ʱȡ��
        5.�����ܼ�����ĵ���*����������+10
         * 
         * 
         ****/
        Dictionary<int, int>  GetShenAoCount(int count,int id )
        {
            Dictionary<int, int> cLst = new Dictionary<int, int>();

            int ou = 0;
            if (count > 1)
            {
                           
                for (int i = 2; i < count; i++)
                {
                   
                    if (count % i == 0 && (count/i)<=10)
                    {
                        ou = count / i;
                        if (ou == 9 || ou == 7) continue;
                        if(count>=100 && ou<5) continue;

                        cLst.Add(i, (ou)); //��������� �Ǿͼ�1 ��Ϊż��
                    }
                }



            }

            return cLst;

        }
        //ʥ�»���Ҫ����������
        //�����趨������12 ��Ҫ��ֳ� �趨����Ϊ2 ����1��6 �趨������Ҫ̫�� Ȼ�����1 ��������� �Ǿͼ�1 ��Ϊż��
        /***
         * ����/����=����
        1.����ֻ��ȡ1��2��3��4��5��6��8��10��������ȡ��ֵ
        2.������Ϊ��λ��ʱ������ֻȡ1��2��3��4��5��6��8��10
        3.������Ϊ��λ��ʱ������ֻȡ5��6��8��10
        4.����/����=����������������С��ʱȡ��
        5.�����ܼ�����ĵ���*����������+10
         * 
         * 
         ****/
        public void ShenAo(int maxmargin)
        {

           if (!DeviceUser.Equals(Constant.DeviceUserShenAo)) return;

           DataTable dt = optSize.DtData;
           
           if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    int needcount = 0;
                    if (int.TryParse(dr[4].ToString(), out needcount))
                    {

                       
                        Dictionary<int, int> cLst= GetShenAoCount(needcount, 0);
                        for (int i = 0; i < maxmargin; i++)
                        {

                            if (cLst.Count <= 0)
                            {
                                needcount++;
                                cLst = GetShenAoCount(needcount, 0);
                            }
                            else
                            {
                                dr[1] = cLst.First().Key.ToString();
                                dr[3] = cLst.First().Value.ToString();
                                break;
                            }                        
                                                       
                        }                                                                  
                    }
                }
            }
        }
        public bool LoadSimiCsvData(string filename)
        {
            if (lcOutInPs != null)
            {
                optSize.Len =(int) (lcOutInPs.ShowValueDouble*Constant.dataMultiple);
            }
            if (dbcOutInPs != null)
            {
                optSize.Dbc =  (int)(dbcOutInPs.ShowValue * Constant.dataMultiple);
            }
            if (ltbcOutInPs != null)
            {
                optSize.Ltbc =  (int)(ltbcOutInPs.ShowValue * Constant.dataMultiple);
            }
            if (safeOutInPs != null)
            {
                optSize.Safe =  (int)(safeOutInPs.ShowValue * Constant.dataMultiple);
            }


            optSize.WlMiniValue = wlMiniSizeOutInPs.ShowValue;
            showPath(filename);
            return optSize.LoadCsvData(filename);
        }

        public bool LoadCsvData(string filename, int id)
        {
            if (lcOutInPs !=null)
            {
                optSize.Len = lcOutInPs.ShowValue;
            }
            if (dbcOutInPs !=null)
            {
                optSize.Dbc = dbcOutInPs.ShowValue;
            }
            if (ltbcOutInPs !=null)
            {
                optSize.Ltbc = ltbcOutInPs.ShowValue;
            }
            if (safeOutInPs !=null)
            {
                optSize.Safe = safeOutInPs.ShowValue;
            }
            optSize.WlMiniValue = wlMiniSizeOutInPs.ShowValue;
            showPath(filename);
            if (id == Constant.OpenCsvWithOutCheck)
            {
                return optSize.LoadCsvDataWithOutCheck(filename);
            }
            return optSize.LoadCsvData(filename);
        }

        public void LoadCsvData0(string filename)
        {
            optSize.Len = lcOutInPs.ShowValue;
            optSize.Dbc = dbcOutInPs.ShowValue;
            optSize.Ltbc = ltbcOutInPs.ShowValue;
            optSize.Safe = safeOutInPs.ShowValue;
            optSize.WlMiniValue = wlMiniSizeOutInPs.ShowValue;
            showPath(filename);
            optSize.LoadCsvData0(filename);
        }

        public void LoadExcelData(string filename)
        {
            optSize.Len = lcOutInPs.ShowValue;
            optSize.Dbc = dbcOutInPs.ShowValue;
            optSize.Ltbc = ltbcOutInPs.ShowValue;
            optSize.Safe = safeOutInPs.ShowValue;

            if (wlMiniSizeOutInPs !=null)
            {
                optSize.WlMiniValue = wlMiniSizeOutInPs.ShowValue;
            }

            showPath(filename);

            if (DeviceUser.Equals(Constant.DeviceUserJingPai))
            {
                optSize.LoadExcelDataWithJinPai(filename);
            }
            else
            optSize.LoadExcelData(filename);


        }

        public void LoadSimiData(string filename)
        {
            optSize.Len = lcOutInPs.ShowValue;
            optSize.Dbc = dbcOutInPs.ShowValue;
            optSize.Ltbc = ltbcOutInPs.ShowValue;   
            optSize.Safe = safeOutInPs.ShowValue;
            if (wlMiniSizeOutInPs !=null)
            {
                optSize.WlMiniValue = wlMiniSizeOutInPs.ShowValue;
            }
            showPath(filename);
            optSize.LoadSimiData(filename);
            optSize.ProdInfoLst.Clear() ;
           // SetPLCMaterialWidth();
        }

        void PrintUnCuttAbleBarCode(int rowindex)
        {
          
            ReLoadReport();
            if (printReport != null)
            {

                List<string> list = new List<string>();
                if ((optSize.DtData != null) && (optSize.DtData.Rows.Count > 0))
                {
                    DataRow row = optSize.DtData.Rows[rowindex];
                    for (int i = 3; i < optSize.DtData.Columns.Count; i++)
                    {
                        list.Add(row[i].ToString());
                    }
                    printBarcode(printReport, list.ToArray());
                }
                else
                {
                    MessageBox.Show("�����ݣ����ȵ������ݣ�");
                }
            }
            else
            {
                MessageBox.Show("������ش���");
            }
        

    }
    public void PrintUnCuttable(int id )
        {
         
            if (optSize.UnCuttableDataLst.Count > 0 && optSize.UnCuttableDataLst.Contains(id))
            {
                DialogResult dr = MessageBox.Show("��ӡ��ʾ",
                    "��ӡ��ǰ/ȫ�����ɼӹ���ǩ��", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Information);//�����¼�������ʾ
                if (dr == DialogResult.No)
                {
                    PrintUnCuttAbleBarCode(id);
                    
                }
                else
                {
                    foreach (int idx in optSize.UnCuttableDataLst)
                    {
                        PrintUnCuttAbleBarCode(idx);
                        ConstantMethod.Delay(500);
                    }
                }
            }
        }
        public void LoadSimiData(string[] filenames)
        {
            optSize.Len = (int)(lcOutInPs.ShowValueDouble*Constant.dataMultiple);
            optSize.Dbc = (int)(dbcOutInPs.ShowValue * Constant.dataMultiple);
            optSize.Ltbc = (int)(ltbcOutInPs.ShowValue * Constant.dataMultiple);
            optSize.Safe = (int)(safeOutInPs.ShowValue * Constant.dataMultiple);
            if (wlMiniSizeOutInPs !=null)
            {
                optSize.WlMiniValue = wlMiniSizeOutInPs.ShowValue;
            }
            optSize.LoadSimiData(filenames);
            SetPLCMaterialWidth();
        }

        private bool needBreak(int id)
        {
            switch (id)
            {
                case 1:
                    return errorStatus(getXialiaoJuStatus());

                case 2:
                    return errorStatus(getDoorBanStatus());

                case 3:
                    return errorStatus(getDoorBanStatus());

                case 4:
                    return errorStatus(getDoorShellStatus());

                case 5:
                    return errorStatus(getDoorShellStatus());

                case 6:
                    return errorStatus(getDoorShellStatus());
            }
            return false;
        }

        public void noSizeToCut()
        {
            evokDevice.SetMValueON(thisLenGoOutOutInPs);           
        }

        public void OpenDataNotEnough()
        {
            if (dataNotEnoughOutInPs !=null)
            {
                evokDevice.SetMValueON(dataNotEnoughOutInPs);
            }
        }

        public void oppositeBitClick(string str1, string str2, List<PlcInfoSimple> pLst)
        {
            PlcInfoSimple p = getPsFromPslLst(str1, str2, pLst);
            if (p !=null)
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
            else
            {
                MessageBox.Show(Constant.SetDataFail);
            }
        }

        public void oppositeBitClick(string str1, string str2, int id)
        {
            List<PlcInfoSimple> pslLst = new List<PlcInfoSimple>();
            if ((id < AllPlcSimpleLst.Count<List<PlcInfoSimple>>()) && (id >= 0))
            {
                pslLst = AllPlcSimpleLst[id];
                PlcInfoSimple p = getPsFromPslLst(str1, str2, pslLst);
                if (p !=null)
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
                else
                {
                    MessageBox.Show(Constant.SetDataFail);
                }
            }
        }
        public void optReady(int OPTID)
        {
            rtbResult.Clear();
            if (!DeviceStatus)
            {
                collectUserInputData();
            }
            else
            {
                collectDeviceData();
            }
            if (optSize.Len < 100)
            {
                MessageBox.Show(Constant.optFail);
            }
            else
            {

                optSize.Safe += (optSize.Dbc * 2 + 100);

                int num = OPTID;

                switch (num)
                {
                    case 0:
                        ConstantMethod.ShowInfo(rtbResult, optSize.OptNormal(rtbResult));
                        return;
                    case 1:
                        return;

                    case 2:
                        ConstantMethod.ShowInfo(rtbResult, optSize.OptNormal(rtbResult, 2));
                        return;

                    case 3:
                        ConstantMethod.ShowInfo(rtbResult, optSize.OptNormal(rtbResult, 3));
                        return;

                    case 4:
                        ConstantMethod.ShowInfo(rtbResult, optSize.OptNormal(rtbResult, 4));
                        return;
                }
                if (num == 10)
                {
                    ConstantMethod.ShowInfo(rtbResult, optSize.OptSimi(rtbResult, 10));
                }
            }
        }
        //˾��ר���Ż�
        public void optReadySimi(int OPTID,int id)
        {
            rtbResult.Clear();
           
            if (!DeviceStatus )
            {
                if(id !=0)
                collectUserInputData();
            }
            else
            {
                collectDeviceData();
            }

            if (optSize.Len < 100)
            {
                MessageBox.Show(Constant.optFail);
            }
            else
            {

                optSize.Safe += (optSize.Dbc * 2+100);

                int num = OPTID;

                switch (num)
                {
                    case 0:
                          ConstantMethod.ShowInfo(rtbResult, optSize.OptNormalSimi(rtbResult));
                        return;
                    case 1:

                        ConstantMethod.ShowInfo(rtbResult, optSize.OptNormal(rtbResult, num));
                        return;

                    case 2:
                          ConstantMethod.ShowInfo(rtbResult, optSize.OptNormal(rtbResult, num));
                        return;

                    case 3:
                        ConstantMethod.ShowInfo(rtbResult, optSize.OptNormal(rtbResult, num));
                        return;

                    case 4:
                        ConstantMethod.ShowInfo(rtbResult, optSize.OptNormal(rtbResult, num));
                        return;
                    case 10:
                        ConstantMethod.ShowInfo(rtbResult, optSize.OptSimi(rtbResult, num));
                        return;
                }
                
            }
        }

        public bool ParamParamTxt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                double num;
                if (double.TryParse(((TextBox)sender).Text, out num) && (num > -1.0))
                {
                    SetDValue(((TextBox)sender).Tag.ToString(), Constant.Write, PsLstParam, ((TextBox)sender).Text);
                }
                return true;
            }
            return false;
        }

        public bool pause()
        {
            if ((((deviceStatusId == Constant.constantStatusId[1]) || (deviceStatusId == Constant.constantStatusId[3])) || ((deviceStatusId == Constant.constantStatusId[4]) || (deviceStatusId == Constant.constantStatusId[6]))) || ((deviceStatusId == Constant.constantStatusId[7]) && (deviceStatusId < Constant.constantStatusStr.Count<string>())))
            {
                showWorkInfo();
                return false;
            }
            if (!RunFlag)
            {
                showWorkInfo(Constant.DeviceStop);
                return false;
            }
            if (pauseOutPs.ShowValue == 0)
            {
                evokDevice.opposite(pauseOutPs);
                int num = 1;
                ConstantMethod.DelayWriteCmdOk(0x2710, ref num, ref pauseInPs, ref emgStopInPs);
                string[] logs = new string[] { DeviceName + Constant.DevicePause };
                LogManager.WriteProgramLog(logs);
                if (pauseInPs.ShowValue == num)
                {
                    showWorkInfo(Constant.pauseOk);
                    return true;
                }
                showWorkInfo(Constant.pauseWrong);
                return false;
            }
            int valueOld = 0;
            evokDevice.opposite(pauseOutPs);
            ConstantMethod.DelayWriteCmdOk(0x2710, ref valueOld, ref pauseInPs);
            if (pauseInPs.ShowValue == valueOld)
            {
                showWorkInfo(Constant.contiOk);
                return true;
            }
            showWorkInfo(Constant.contiWrong);
            return false;
        }

        public void plcHandleBarCodeOFF()
        {
            evokDevice.SetMValueOFF(plcHandlebarCodeOutInPs);
        }

        public void plcHandleBarCodeON()
        {
            evokDevice.SetMValueON(plcHandlebarCodeOutInPs);
        }

        public void pressShift()
        {
            evokDevice.opposite(pressOutInPs);
        }

        private void PrintBarCheck(SingleSize ss)
        {
            if (ss.Barc.Contains("�ü�")) return;

                if (((printMiniSizeOutInPs != null) && (printMiniSizeOutInPs.ShowValue > ss.Cut)) && (PrintBarCodeMode == Constant.AutoBarCode))
            {
                if (((minPrinterName != "") && (printMiniSizeOutInPs.ShowValue > ss.Cut)) && ConstantMethod.GetLocalPrinter().Contains(minPrinterName))
                {
                    printReport.PrintSettings.Printer = minPrinterName;
                    printBarcode(printReport, ss.ParamStrLst.ToArray());
                }
                else
                {
                    printReport.PrintSettings.Printer = ConstantMethod.DefaultPrinter;
                }
                evokDevice.SetMValueOFF(plcHandlebarCodeOutInPs);
            }
            else if ((ss.ParamStrLst.ToArray().Length != 0) && ss.ParamStrLst.ToArray()[0].ToString().Equals(Constant.ScarId))
            {
                evokDevice.SetMValueOFF(plcHandlebarCodeOutInPs);
            }
            else
            {
                if (OldPrintBarCodeMode == Constant.AutoBarCode)
                {
                    evokDevice.SetMValueON(plcHandlebarCodeOutInPs);
                }
                printBarcode(printReport, ss.ParamStrLst.ToArray());
            }
        }

        public void printBarcode(Report rp1, object s2)
        {
            string[] strArray = (string[])s2;
           
            if (strArray.Length >= 1)
            {
                try
                {
                    if (((strArray != null) && IsPrintBarCode &&(printReport != null)))
                    {
                        Application.DoEvents();
                        if (rp1.FindObject("Barcode1") !=null)
                        {
                            (rp1.FindObject("Barcode1") as BarcodeObject).Text = strArray[0];
                        }
                        for (int i = 1; i < strArray.Length; i++)
                        {
                            if ((rp1.FindObject("Text" + i.ToString()) != null) && string.IsNullOrWhiteSpace(strArray[i]))
                            {
                                (rp1.FindObject("Text" + i.ToString()) as TextObject).Text = "";
                            }
                            else if ((rp1.FindObject("Text" + i.ToString()) != null) && !string.IsNullOrWhiteSpace(strArray[i]))
                            {
                                strArray[i] = ConstantMethod.ShiftString(strArray[i]);
                                (rp1.FindObject("Text" + i.ToString()) as TextObject).Text = strArray[i];
                            }
                        }
                        rp1.SetParameterValue("Parameter", strArray);
                        rp1.Prepare();
                        rp1.Print();
                    }
                }
                catch (Exception)
                {
                    RunFlag = false;
                }
                if (!printReport.PrintSettings.Printer.Equals(ConstantMethod.DefaultPrinter))
                {
                    printReport.PrintSettings.Printer = ConstantMethod.DefaultPrinter;
                }
            }
        }

        public void printBarcodeOffline(Report rp1, object s2)
        {
            string[] strArray = (string[])s2;
            string str = "";
            foreach (string str2 in strArray)
            {
                str = str + str2 + "/";
            }
            if (strArray.Length >= 1)
            {
                try
                {
                    if (((strArray != null) && (printReport != null)) && IsPrintBarCode)
                    {
                        Application.DoEvents();
                        if (rp1.FindObject("Barcode1") != null)
                        {
                            (rp1.FindObject("Barcode1") as BarcodeObject).Text = strArray[0];
                        }
                        for (int i = 1; i < strArray.Length; i++)
                        {
                            if ((rp1.FindObject("Text" + i.ToString()) != null) && string.IsNullOrWhiteSpace(strArray[i]))
                            {
                                (rp1.FindObject("Text" + i.ToString()) as TextObject).Text = "";
                            }
                            else if ((rp1.FindObject("Text" + i.ToString()) != null) && !string.IsNullOrWhiteSpace(strArray[i]))
                            {
                                strArray[i] = ConstantMethod.ShiftString(strArray[i]);
                                (rp1.FindObject("Text" + i.ToString()) as TextObject).Text = strArray[i];
                            }
                        }
                        rp1.SetParameterValue("Parameter", strArray);
                        rp1.Prepare();
                        rp1.Print();
                    }
                }
                catch (Exception)
                {
                    RunFlag = false;
                }
                if (!printReport.PrintSettings.Printer.Equals(ConstantMethod.DefaultPrinter))
                {
                    printReport.PrintSettings.Printer = ConstantMethod.DefaultPrinter;
                }
            }
        }


        public void printBarcode(Report rp1, object s2, int show)
        {
            string[] strArray = (string[])s2;
           
            if ((strArray != null) && (printReport !=null))
            {
                try
                {
                    OldPrintBarCodeMode = PrintBarCodeMode;
                    Application.DoEvents();
                    if (rp1.FindObject("Barcode1") !=null)
                    {
                        (rp1.FindObject("Barcode1") as BarcodeObject).Text = strArray[0];
                    }
                    for (int i = 1; i < strArray.Length; i++)
                    {
                        if ((rp1.FindObject("Text" + i.ToString()) != null) && string.IsNullOrWhiteSpace(strArray[i]))
                        {
                            (rp1.FindObject("Text" + i.ToString()) as TextObject).Text = "";
                        }
                        else if ((rp1.FindObject("Text" + i.ToString()) != null) && !string.IsNullOrWhiteSpace(strArray[i]))
                        {
                            strArray[i] = ConstantMethod.ShiftString(strArray[i]);
                            (rp1.FindObject("Text" + i.ToString()) as TextObject).Text = strArray[i];
                        }
                    }
                    rp1.SetParameterValue("Parameter", strArray);
                    rp1.Prepare();
                    rp1.Show();
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.ToString());
                }
            }
        }

       

        private void printRestMaterial(string[] s)
        {
            if (DeviceName.Equals(Constant.simiDeivceName))
            {

                SetPrintReport(Constant.BarCode2);

                printBarcode(printReport, s ,1);

            }

            SetPrintReport(Constant.BarCode1);
        }

        private void printTimerCheck(object sender, EventArgs e)
        {
            /***
            if (printThreadLst.Count > 0)
            {
                List<Thread> list = new List<Thread>(printThreadLst.Keys);
                if (list[0].ThreadState == ThreadState.Unstarted)
                {
                    list[0].Start(printThreadLst[list[0]]);
                }
                else if ((list[0].ThreadState == ThreadState.Aborted) || !list[0].IsAlive)
                {
                    printThreadLst.Remove(list[0]);
                }
            }
            ***/
        }

        public void ProClr()
        {
            evokDevice.SetDValue(prodOutInPs, 0);
            optSize.prodClear();
        }

        private string ReadBarCodeSourceFolder() { return ParamFile.ReadConfig(Constant.barCodePath); }
        

        public int ReadCSVDataDefault()
        {
            if (!LoadCsvData(Constant.userdata))
            {
                optSize.buildDefaultCsvFile(Constant.userdata);
                LoadCsvData(Constant.userdata);
            }
            return 0;
        }
        public int ReadCSVDataDefault(int id)
        {
            if (id.Equals(Constant.simiDeivceId))
            {
                if (!LoadSimiCsvData(Constant.userdata))
                {
                    optSize.buildDefaultCsvFile(Constant.userdata);
                    LoadSimiCsvData(Constant.userdata);
                }
            }
            else
            {
                ReadCSVDataDefault();
            
            }
            return 0;
        }
        public int ReadCutMode()
        {
            int result = 0;
            if (int.TryParse(ParamFile.ReadConfig(Constant.cutSelMode), out result))
            {
                return result;
            }
            return 0;
        }
        #region ˮƽ��׻�
        const int HoleMode = 0;
        const int GrovveMode = 1;
        const string  YunXi_GrovveMode = "T2";
        const string  YunXi_HoleMode   = "T1";
        private bool readData(string fileName)
        {
            DataTable table = new DataTable();
            FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(stream, System.Text.Encoding.Default);
            string source = "";
            Dictionary<string, List<double>> dictionary = new Dictionary<string, List<double>>();
            List<double> list = new List<double>();
            string str2 = "";
            while ((source = reader.ReadLine()) !=null)
            {
                if (source.Contains<char>('M'))
                {
                    str2 = source.Trim();
                    if (!dictionary.Keys.Contains<string>(str2))
                    {
                        list = new List<double>();
                        dictionary.Add(str2, list);
                    }
                }
                if (source.Contains<char>('X'))
                {
                    double result = 0.0;
                    char[] separator = new char[] { 'X' };
                    string[] strArray2 = source.Split(separator);
                    if ((strArray2.Length == 2) && double.TryParse(strArray2[1], out result))
                    {
                        double num2 = 0.0;
                        if (double.TryParse(strArray2[1], out num2) && !dictionary[str2].Contains(num2))
                        {
                            dictionary[str2].Add(num2);
                        }
                    }
                }
            }
            foreach (string str3 in dictionary.Keys.ToList<string>())
            {
                if (dictionary[str3].Count == 0)
                {
                    dictionary.Remove(str3);
                }
                else
                {
                    dictionary[str3].Sort();
                    string[] valueCol = new string[] { "λ��", "�۳�", "ľм��" };
                    DataTable item = ConstantMethod.getDataTableByString(valueCol);
                    item.TableName = str3;
                    for (int i = 0; i < dictionary[str3].Count; i++)
                    {
                        DataRow row = item.NewRow();
                        row[0] = dictionary[str3][i];
                        row[1] = "0";
                        row[2] = "0";
                        item.Rows.Add(row);
                    }
                    HoleDataLst.Add(item);
                }
            }
            return true;
        }
        //ˮƽ��׻� �۳� ����ʼλ��
        struct GrooveData
        {
           public  double StartPos;
           public  double Len;
        }
        private bool readDataYunXi(string fileName)
        {
            //T2 ���ۿ�ʼ
            //T1 ��׿�ʼ
           
            DataTable table = new DataTable();
            FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(stream, System.Text.Encoding.Default);
            string source = "";
            Dictionary<string, List<double>> dictionaryHole = new Dictionary<string, List<double>>();
            Dictionary<string, List<double>> dictionaryGroove = new Dictionary<string, List<double>>();

            List<double> listHole = new List<double>();

            List<double> listGrovve = new List<double>();


            string str2 = "";
       
            int CutMode = -1;

            while ((source = reader.ReadLine()) != null)
            {
                if (source.Contains("M6"))
                {
                    str2 = source.Trim();
                    if (!dictionaryHole.Keys.Contains<string>(str2))
                    {
                        listHole = new List<double>();
                        listGrovve = new List<double>();
                        dictionaryHole.Add(str2, listHole);
                        dictionaryGroove.Add(str2, listGrovve);
                    }
                }

                CutMode = source.Contains(YunXi_GrovveMode) ? GrovveMode : CutMode;
                CutMode = source.Contains(YunXi_HoleMode)   ? HoleMode : CutMode;
                                           
                if (source.Contains('X') && source.Contains('Y'))
                {
                    double result = 0.0;

                    string[] strArray2 = source.Split(' ');

                    string data = strArray2[2].Replace('Y',' ').Trim(); 

                    if (CutMode==HoleMode && (strArray2.Length == 3) && double.TryParse(data, out result))
                    {              
                        if (!dictionaryHole[str2].Contains(result))
                        {
                            dictionaryHole[str2].Add(result);
                        }
                    }
                    else
                    if (CutMode == GrovveMode && (strArray2.Length == 3) && double.TryParse(data, out result))
                    {

                        if (!dictionaryGroove[str2].Contains(result))
                        {
                            dictionaryGroove[str2].Add(result);
                        }                             
                    }

                }
            }
            //�������
            foreach (string str3 in dictionaryHole.Keys.ToList<string>())
            {
                if (dictionaryHole[str3].Count == 0)
                {
                    dictionaryHole.Remove(str3);
                }
                else
                {

                    dictionaryHole[str3].Sort();
                    if(dictionaryGroove[str3].Count>0)
                    dictionaryHole[str3].Reverse();
                    string[] valueCol = new string[] { "λ��", "�۳�", "ľм��" };
                    DataTable item = ConstantMethod.getDataTableByString(valueCol);
                    item.TableName = str3;
                    for (int i = 0; i < dictionaryHole[str3].Count; i++)
                    {
                        DataRow row = item.NewRow();
                        row[0] = dictionaryHole[str3][i];
                        row[1] = "0";
                        row[2] = "0";
                        item.Rows.Add(row);
                    }
                    HoleDataLst.Add(item);
                }
            }

            //��Ҫ������ʼλ�úͲ۳�
            Dictionary<string, List<GrooveData> > GrooveListDic = new Dictionary<string, List<GrooveData>>();

            //������������ ������Ҫ��������
            foreach (string str3 in dictionaryGroove.Keys.ToList<string>())
            {
                if (dictionaryGroove[str3].Count % 2 == 0)
                {
                    List<double> GrooveDataLst = new List<double>();
                    List<GrooveData> GrooveList = new List<GrooveData>();
                    GrooveListDic.Add(str3, GrooveList);
                    for (int i = 0; i < dictionaryGroove[str3].Count; i += 2)
                    {
                        if ((i + 1) < dictionaryGroove[str3].Count)
                        {
                            //���ݴ���0 ��Ч  С��0 ���������
                            double datag = 0;
                            if ((datag = (dictionaryGroove[str3][i + 1] - dictionaryGroove[str3][i])) != 0)
                            {
                                GrooveData gd = new GrooveData();

                                gd.Len = Math.Abs(datag);
                                gd.StartPos = dictionaryGroove[str3][i + 1] > dictionaryGroove[str3][i] ?dictionaryGroove[str3][i]: dictionaryGroove[str3][i + 1];
                                GrooveDataLst.Add(Math.Abs(datag));

                                if (GrooveListDic[str3].Count==0|| gd.StartPos > GrooveListDic[str3].Last().StartPos)
                                {
                                    GrooveListDic[str3].Add(gd);
                                }
                                else
                                {

                                    //����
                                    if (gd.StartPos < GrooveListDic[str3].First().StartPos)
                                    {
                                        GrooveListDic[str3].Insert(0, gd);

                                    }
                                     else                             
                                    for (int j = 0; j < GrooveListDic[str3].Count-1; j++)
                                    {

                                        if (GrooveListDic[str3][j].StartPos <= gd.StartPos && GrooveListDic[str3][j + 1].StartPos >= gd.StartPos)

                                        { GrooveListDic[str3].Insert(j, gd); break; }
                                    }                                    
                                }
                                                                                         
                            }
                            else
                                GrooveDataLst.Clear();
                        }
                    }
                    dictionaryGroove[str3].Clear();
                    
                    if(GrooveDataLst.Count>0)
                    dictionaryGroove[str3] = GrooveDataLst;

                  }
            }
           

            //��������      
            foreach (string str3 in dictionaryGroove.Keys.ToList<string>())
            {
                if (dictionaryGroove[str3].Count == 0)
                {
                    dictionaryGroove.Remove(str3);
                }
                else
                {
                    dictionaryGroove[str3].Sort();

                    DataTable dt = ConstantMethod.getDatatByTableName(HoleDataLst,str3);

                    // �����񲻴���
                    if (dt == null)
                    {
                        string[] valueCol = new string[] { "λ��", "�۳�", "ľм��" };

                        DataTable item = ConstantMethod.getDataTableByString(valueCol);

                        item.TableName = str3;

                        for (int i = 0; i < GrooveListDic[str3].Count; i++)
                        {
                            DataRow row = dt.NewRow();
                            row[0] = GrooveListDic[str3][i].StartPos;
                            row[1] = GrooveListDic[str3][i].Len;
                            row[2] = "0";
                            dt.Rows.Add(row);
                        }

                        HoleDataLst.Add(item);
                    }
                    else
                    {
                        // ���������
                        for (int i = GrooveListDic[str3].Count-1; i >=0; i--)
                        {
                           // if (i < dt.Rows.Count)
                           // {
                               // dt.Rows[i][1] = dictionaryGroove[str3][i];
                           // }
                           // else
                           // {
                                DataRow row = dt.NewRow();
                                row[0] = GrooveListDic[str3][i].StartPos;
                                row[1] = GrooveListDic[str3][i].Len;
                                row[2] = "0";
                                dt.Rows.InsertAt(row,0);
                           // }
                        }                                                                      
                    }
                }
            }
        
            return true;
        }
        
        #endregion
        public bool reset()
        {
            if (!DeviceStatus)
            {
                RunFlag = false;
                return true;
            }
            if ((((deviceStatusId == Constant.constantStatusId[1]) || (deviceStatusId == Constant.constantStatusId[2])) || ((deviceStatusId == Constant.constantStatusId[3]) || (deviceStatusId == Constant.constantStatusId[4]))) || ((deviceStatusId == Constant.constantStatusId[5]) && (deviceStatusId < Constant.constantStatusStr.Count<string>())))
            {
                showWorkInfo();
                return false;
            }
            evokDevice.SetMValueOFF2ON2OFF(resetOutPs);
            ConstantMethod.Delay(0x3e8);
            int valueOld = 0;
            ConstantMethod.DelayWriteCmdOk(0xea60, ref valueOld, ref resetInPs, ref emgStopInPs);
            string[] logs = new string[] { DeviceName + Constant.DeviceReset };
            LogManager.WriteProgramLog(logs);
            if (resetInPs.ShowValue == valueOld)
            {
                showWorkInfo(Constant.resetOk);
                stopOperation();
                return true;
            }
            showWorkInfo(Constant.resetWrong);
            return false;
        }

        public bool RestartDevice(int id)
        {
            evokDevice.RestartConneect(evokDevice.DataFormLst[id]);
            FindPlcSimpleInPlcInfoLst(id);
            return evokDevice.getDeviceData();
        }

        public void SaveFile()
        {
            optSize.SaveCsv();

            if(!DeviceUser.Equals(Constant.DeviceUserJingPai))
            optSize.SaveExcel();

            if (Rsm != null)
            {
                Rsm.SaveMaterial();
            }
        }

        public void SaveFileToUserData()
        {
            optSize.SaveCsv(Constant.SaveCsvToUserData);
        }

        public bool SaveOptParam1(string id)
        {
            int result = 0;
            if (int.TryParse(id, out result) && (result > -1))
            {
                try
                {
                    ParamFile.WriteConfig(Constant.optParam1, id);
                }
                catch (Exception)
                {
                }
                return true;
            }
            return false;
        }

        public void SaveProdDataLog(ProdInfo p, int id)
        {
            if (IsSaveProdLog && (optSize.ProdInfoLst.Count > 0))
            {
                string str = "";
                string str2 = $"�ϳ�:{optSize.Len}  ��ͷ����:{optSize.Ltbc}  ������:{optSize.Dbc}  ��ȫ����:{optSize.Safe} β��:{p.WL}";
                int num = id + 1;
                str = str + "  ��" + num.ToString() + "��:";
                for (int i = 0; i < p.Cut.Count; i++)
                {
                    string[] textArray1 = new string[6];
                    textArray1[0] = str;
                    textArray1[1] = "  ��";
                    num = i + 1;
                    textArray1[2] = num.ToString();
                    textArray1[3] = "���ߴ�";
                    textArray1[4] = p.Cut[i].ToString();
                    textArray1[5] = "\n";
                    str = string.Concat(textArray1);
                }
                string[] logs = new string[] { str2 + str };
                LogManager.WriteProgramLogProdData(logs);
            }
        }
        void DataClear()
        {
            if (HoleDataLst == null)
            {
                HoleDataLst = new List<DataTable>();
            }
            if (GrooveDataLst == null)
            {
                GrooveDataLst = new List<DataTable>();
            }
            HoleDataLst.Clear();
            GrooveDataLst.Clear();
        }
        public void ScanCode(string barcode)
        {

         //   if(IsRuninng)
           // StopRunning();

            string path = FindBarCodeFile(barcode);

            if (File.Exists(path))
            {

                DataClear();
                //��Ϫ���ɨ�� +  �������Ӽ�����
                if (!readDataYunXi(path))
                {
                    MessageBox.Show(Constant.dataConvertError);
                }

                IsRuninng = true;

                enterRunning();

            }
            else
            {
                MessageBox.Show("��ȡ����ʧ�ܣ�");
            }
            StopRunning();
        }

        public void scarModeSelect()
        {
            evokDevice.SetMValueOFF2ON(scarModeOutPs);
        }

        private void SelectCutThread(int cutid)
        {
            switch (cutid)
            {
                case 0:
                    if (CutThreadStart == null)
                    {
                        CutThreadStart = new ThreadStart(CutWorkThread);
                    }
                    if (CutThread == null)
                    {
                        CutThread = new Thread(CutThreadStart);
                    }
                    break;

                case 1:
                    if (CutThreadStart == null)
                    {
                        CutThreadStart = new ThreadStart(CutWorkThread);
                    }
                    if (CutThread == null)
                    {
                        CutThread = new Thread(CutThreadStart);
                    }
                    break;

                case 2:
                    if (CutThreadStart == null)
                    {
                        CutThreadStart = new ThreadStart(CutRotateWithHoleThread);
                    }
                    if (CutThread == null)
                    {
                        CutThread = new Thread(CutThreadStart);
                    }
                    break;

                case 3:
                    if (CutThreadStart == null)
                    {
                        CutThreadStart = new ThreadStart(CutRotateWithHoleThread);
                    }
                    if (CutThread == null)
                    {
                        CutThread = new Thread(CutThreadStart);
                    }
                    break;

                case 4:
                    if (CutThreadStart == null)
                    {
                        CutThreadStart = new ThreadStart(CutWorkThread);
                    }
                    if (CutThread == null)
                    {
                        CutThread = new Thread(CutThreadStart);
                    }
                    break;

                case 5:
                    if (CutThreadStart == null)
                    {
                        CutThreadStart = new ThreadStart(CutDoorShellThread);
                    }
                    if (CutThread == null)
                    {
                        CutThread = new Thread(CutThreadStart);
                    }
                    break;

                case 6:
                    if (CutThreadStart == null)
                    {
                        CutThreadStart = new ThreadStart(CutWorkThreadWithShuchi);
                    }
                    if (CutThread == null)
                    {
                        CutThread = new Thread(CutThreadStart);
                    }
                    break;

                case 7:
                    if (CutThreadStart == null)
                    {
                        CutThreadStart = new ThreadStart(CutDoorBanThread);
                    }
                    if (CutThread == null)
                    {
                        CutThread = new Thread(CutThreadStart);
                    }
                    break;

                case 8:
                    if (CutThreadStart == null)
                    {
                        CutThreadStart = new ThreadStart(CutWorkThreadWithAngle);
                    }
                    if (CutThread == null)
                    {
                        CutThread = new Thread(CutThreadStart);
                    }
                    break;

                case 9:
                    if (CutThreadStart == null)
                    {
                        CutThreadStart = new ThreadStart(CutSimenSiPlcThread);
                    }
                    if (CutThread == null)
                    {
                        CutThread = new Thread(CutThreadStart);
                    }
                    break;
                case 10: //�ֶ������Զ�ģʽ
                    if (CutThreadStart == null)
                    {
                        CutThreadStart = new ThreadStart(CutSimenSiPlcThread_AutoPos);
                    }
                    if (CutThread == null)
                    {
                        CutThread = new Thread(CutThreadStart);
                    }
                    break;
            }
        }

        public bool SetBarCodeSourceFolder(string folderName)
        {
            if (!Directory.Exists(folderName))
            {
                return false;
            }
            ParamFile.WriteConfig(Constant.barCodePath, folderName);
            return true;
        }

        public bool SetCutProCnt(int cnt)
        {
            if (CutProCnt >= optSize.ProdInfoLst.Count)
            {
                CutProCnt = 0;
            }
            if (cnt == 0)
            {
                CutProCnt = 0;
                return true;
            }
            if ((cnt <= optSize.ProdInfoLst.Count) && (cnt >= 1))
            {
                CutProCnt = cnt - 1;
                return true;
            }

            MessageBox.Show(Constant.startTips7);

            return false;
        }

        public void SetDataShow(int id)
        {
            optSize.ChooseDataLst();
            if (DeviceName.Equals(Constant.simiDeivceName))
            {
                if (optSize.SetSimiMaterial())
                {
                    SetSimiReady();
                }
                else
                {
                    MessageBox.Show("��������ʧ�ܣ�");
                }
            }
        }

        public void SetDataShowCb(ListBox cb)
        {
            if (cb !=null)
            {
                optSize.DataShowCb = cb;
            }
        }

        public void SetDataShowLbl(Label lbl1)
        {
            if (lbl1 !=null)
            {
                optSize.DataShowLbl = lbl1;
            }
        }

        private void SetDoorTypeCutCount(int value)
        {
            evokDevice.SetMultiPleDValue(doorTypeCutCountOutInPs, new List<int> {
                value,
                1,
                1
            }.ToArray());
        }

        public bool SetDValue(PlcInfoSimple p, string num)
        {
            double result = 0.0;
            if (double.TryParse(num, out result))
            {
                if ((result > p.MaxValue) || (result < p.MinValue))
                {
                    MessageBox.Show(Constant.dataOutOfRange + p.MinValue.ToString() + "--" + p.MaxValue.ToString());
                    return false;
                }
                result *= p.Ration;
            }
            if (p !=null)
            {
                return evokDevice.SetDValue(p, (int)result);
            }
            MessageBox.Show(Constant.SetDataFail);
            return false;
        }

        public void SetDValue(string str1, string str2, List<PlcInfoSimple> pLst, int num)
        {
            PlcInfoSimple p = getPsFromPslLst(str1, str2, pLst);
            if ((num > p.MaxValue) || (num < p.MinValue))
            {
                MessageBox.Show(Constant.dataOutOfRange + p.MinValue.ToString() + "--" + p.MaxValue.ToString());
            }
            else if (p !=null)
            {
                evokDevice.SetDValue(p, num);
            }
            else
            {
                MessageBox.Show(Constant.SetDataFail);
            }
        }

        public bool SetDValue(string str1, string str2, List<PlcInfoSimple> pLst, string num)
        {
            PlcInfoSimple p = getPsFromPslLst(str1, str2, pLst);
            double result = 0.0;
            if (double.TryParse(num, out result))
            {
                if ((result > p.MaxValue) || (result < p.MinValue))
                {
                    MessageBox.Show(Constant.dataOutOfRange + p.MinValue.ToString() + "--" + p.MaxValue.ToString());
                    return false;
                }
                result *= p.Ration;
            }
            if (p !=null)
            {
                return evokDevice.SetDValue(p, (int)result);
            }
            MessageBox.Show(Constant.SetDataFail);
            return false;
        }

        public void SetEvokDevice(EvokXJDevice evokDevice0)
        {
            evokDevice = evokDevice0;
        }

        public void SetInEdit(string str1, string str2, List<PlcInfoSimple> pLst)
        {
            PlcInfoSimple simple = getPsFromPslLst(str1, str2, pLst);
            if (simple !=null)
            {
                simple.IsInEdit = true;
            }
            else
            {
                MessageBox.Show(Constant.SetDataFail);
            }
        }

        public void SetInEdit(string str1, string str2, int id)
        {
            List<PlcInfoSimple> pslLst = new List<PlcInfoSimple>();
            if ((id < AllPlcSimpleLst.Count<List<PlcInfoSimple>>()) && (id >= 0))
            {
                pslLst = AllPlcSimpleLst[id];
                PlcInfoSimple simple = getPsFromPslLst(str1, str2, pslLst);
                if (simple !=null)
                {
                    simple.IsInEdit = true;
                }
                else
                {
                    MessageBox.Show(Constant.SetDataFail);
                }
            }
        }

        public void SetLblStatus(Label lblstatus0)
        {
            lblStatus = lblstatus0;
        }

        public void SetLtbc()
        {
            evokDevice.SetDValue(ltbcOutInPs, optSize.Ltbc);
        }

        public void SetMode(int index)
        {
            evokDevice.SetDValue(posMode, index);
        }

        public void SetMPsOff(string str1, string str2, List<PlcInfoSimple> pLst)
        {
            PlcInfoSimple p = getPsFromPslLst(str1, str2, pLst);
            if (p !=null)
            {
                evokDevice.SetMValueOFF(p);
            }
            else
            {
                MessageBox.Show(Constant.SetDataFail);
            }
        }

        public void SetMPsOff(string str1, string str2, int id)
        {
            List<PlcInfoSimple> pslLst = new List<PlcInfoSimple>();
            if ((id < AllPlcSimpleLst.Count<List<PlcInfoSimple>>()) && (id >= 0))
            {
                pslLst = AllPlcSimpleLst[id];
                PlcInfoSimple p = getPsFromPslLst(str1, str2, pslLst);
                if (p !=null)
                {
                    evokDevice.SetMValueOFF(p);
                }
                else
                {
                    MessageBox.Show(Constant.SetDataFail);
                }
            }
        }

        public void SetMPsOFFToOn(string str1, string str2, List<PlcInfoSimple> pLst)
        {
            PlcInfoSimple p = getPsFromPslLst(str1, str2, pLst);
            if (p !=null)
            {
                evokDevice.SetMValueOFF2ON(p);
            }
            else
            {
                MessageBox.Show(Constant.SetDataFail);
            }
        }

        public void SetMPsOFFToOn(string str1, string str2, int id)
        {
            List<PlcInfoSimple> pslLst = new List<PlcInfoSimple>();
            if ((id < AllPlcSimpleLst.Count<List<PlcInfoSimple>>()) && (id >= 0))
            {
                pslLst = AllPlcSimpleLst[id];
                PlcInfoSimple p = getPsFromPslLst(str1, str2, pslLst);
                if (p !=null)
                {
                    evokDevice.SetMValueOFF2ON(p);
                }
                else
                {
                    MessageBox.Show(Constant.SetDataFail);
                }
            }
        }

        public void SetMPsOn(string str1, string str2, List<PlcInfoSimple> pLst)
        {
            PlcInfoSimple p = getPsFromPslLst(str1, str2, pLst);
            if (p !=null)
            {
                evokDevice.SetMValueON(p);
            }
            else
            {
                MessageBox.Show(Constant.SetDataFail);
            }
        }

        public void SetMPsOn(string str1, string str2, int id)
        {
            List<PlcInfoSimple> pslLst = new List<PlcInfoSimple>();
            if ((id < AllPlcSimpleLst.Count<List<PlcInfoSimple>>()) && (id >= 0))
            {
                pslLst = AllPlcSimpleLst[id];
                PlcInfoSimple p = getPsFromPslLst(str1, str2, pslLst);
                if (p !=null)
                {
                    evokDevice.SetMValueON(p);
                }
                else
                {
                    MessageBox.Show(Constant.SetDataFail);
                }
            }
        }

        public void SetMPsONToOFF(PlcInfoSimple p)
        {
            if (p !=null)
            {
                evokDevice.SetMValueON2OFF(p);
            }
            else
            {
                ConstantMethod.ShowInfo(rtbWork, Constant.SetDataFail);
            }
        }

        public void SetMPsONToOFF(string str1, string str2, int id)
        {
            List<PlcInfoSimple> pslLst = new List<PlcInfoSimple>();
            if ((id < AllPlcSimpleLst.Count<List<PlcInfoSimple>>()) && (id >= 0))
            {
                pslLst = AllPlcSimpleLst[id];
                PlcInfoSimple p = getPsFromPslLst(str1, str2, pslLst);
                if (p !=null)
                {
                    evokDevice.SetMValueON2OFF(p);
                }
                else
                {
                    ConstantMethod.ShowInfo(rtbWork, Constant.SetDataFail);
                }
            }
        }

        public void SetOptParamShowCombox(ComboBox cbOptParam0)
        {
            cbOptParam1 = cbOptParam0;
            int result = 0;
            try
            {
                if (!int.TryParse(ParamFile.ReadConfig(Constant.optParam1), out result))
                {
                    MessageBox.Show(Constant.ErrorParamConfigFile);
                    Application.Exit();
                    Environment.Exit(0);
                }
            }
            catch (Exception)
            {
            }
            if ((cbOptParam1 != null) && (result > -1))
            {
                cbOptParam1.Text = result.ToString();
            }
        }

        public void SetOptSize(OptSize optSize0)
        {
            optSize = optSize0;
        }

        public bool SetOptSizeData(DataTable dtSize)
        {
            if ((dtSize != null) && (dtSize.Rows.Count > 0))
            {
                optSize.DtData = dtSize;
                return true;
            }
            return false;
        }

        public void SetOptSizeParam1(int value1)
        {
            if (value1 > -1)
            {
                SaveOptParam1(value1.ToString());
                optSize.OptParam1 = value1;
            }
        }

        public void SetOptSizeParam1(string value1)
        {
            if (SaveOptParam1(value1.ToString()))
            {
                optSize.OptParam1 = int.Parse(value1);
            }
        }

        public void SetOutEdit(string str1, string str2, List<PlcInfoSimple> pLst)
        {
            PlcInfoSimple simple = getPsFromPslLst(str1, str2, pLst);
            if (simple !=null)
            {
                simple.IsInEdit = false;
            }
            else
            {
                MessageBox.Show(Constant.SetDataFail);
            }
        }

        public void SetOutEdit(string str1, string str2, int id)
        {
            List<PlcInfoSimple> pslLst = new List<PlcInfoSimple>();
            if ((id < AllPlcSimpleLst.Count<List<PlcInfoSimple>>()) && (id >= 0))
            {
                pslLst = AllPlcSimpleLst[id];
                PlcInfoSimple simple = getPsFromPslLst(str1, str2, pslLst);
                if (simple !=null)
                {
                    simple.IsInEdit = false;
                }
                else
                {
                    MessageBox.Show(Constant.SetDataFail);
                }
            }
        }

        public void SetPage(int id)
        {
            if ((evokDevice.DataFormLst.Count > 1) && (evokDevice.DataFormLst[id].Rows.Count > 0))
            {
                AllPlcSimpleLst[id].Clear();
                foreach (DataRow row in evokDevice.DataFormLst[id].Rows)
                {
                    if (row != null)
                    {
                        string str = row["bin"].ToString();
                        if (!string.IsNullOrWhiteSpace(str))
                        {
                            PlcInfoSimple item = new PlcInfoSimple(str);
                            try
                            {
                                item.Mode = row["mode"].ToString();
                                item.RowIndex = evokDevice.DataFormLst[id].Rows.IndexOf(row);
                                item.BelongToDataform = evokDevice.DataFormLst[id];
                                int addr = 0;
                                string area = "D";
                                string userdata = row["addr"].ToString();
                                string str4 = row["param3"].ToString();
                                string str5 = row["param4"].ToString();
                                if (!string.IsNullOrWhiteSpace(str4))
                                {
                                    item.ShowStr.Add(str4);
                                }
                                if (!string.IsNullOrWhiteSpace(str5))
                                {
                                    item.ShowStr.Add(str5);
                                }
                                ConstantMethod.SplitAreaAndAddr(userdata, ref addr, ref area);
                                item.Area = area;
                                item.Addr = addr;
                                if (row.Table.Columns.Contains("param7"))
                                {
                                    string str6 = row["param7"].ToString();
                                    if (!string.IsNullOrWhiteSpace(str6))
                                    {
                                        item.Ration = int.Parse(str6);
                                    }
                                }
                                if (row.Table.Columns.Contains("param8"))
                                {
                                    string str7 = row["param8"].ToString();
                                    if (!string.IsNullOrWhiteSpace(str7))
                                    {
                                        item.MaxValue = int.Parse(str7);
                                    }
                                }
                                if (row.Table.Columns.Contains("param9"))
                                {
                                    string str8 = row["param9"].ToString();
                                    if (!string.IsNullOrWhiteSpace(str8))
                                    {
                                        item.MinValue = int.Parse(str8);
                                    }
                                }
                                AllPlcSimpleLst[id].Add(item);
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }
            }
        }

        private bool SetPLCMaterialWidth()
        {
            if (optSize.DownSizeMaterialWidth == 0.0)
            {
                evokDevice.SetDValue(widht1InOutPs, (int)(optSize.MaterialWidth * widht1InOutPs.Ration));
                ConstantMethod.Delay(200);
                evokDevice.SetDValue(widht2InOutPs, 0);
                ConstantMethod.Delay(200);
                evokDevice.SetDValue(materialIdInOutPs, optSize.MaterialId);
                ConstantMethod.Delay(200);
                return evokDevice.SetMValueON(materialSetEnInOutPs);
            }
            return (((optSize.DownSizeMaterialWidth > 0.0) && (optSize.UpSizeMaterialWidth > 0.0)) && (((evokDevice.SetDValue(widht1InOutPs, (int)(optSize.DownSizeMaterialWidth * widht1InOutPs.Ration)) && evokDevice.SetDValue(widht2InOutPs, (int)(optSize.UpSizeMaterialWidth * widht2InOutPs.Ration))) && evokDevice.SetDValue(materialIdInOutPs, optSize.MaterialId)) && evokDevice.SetMValueON(materialSetEnInOutPs)));
        }

        public bool SetPos(string s)
        {
            double result = 0.0;
            if ((double.TryParse(s, out result) && !SetPos1(s)) && !SetPos2(s))
            {
                return false;
            }
            return true;
        }

        public bool SetPos1(string index)
        {
            if (pos1EnInPs.ShowValue == Constant.M_OFF)
            {
                if (SetDValue(pos1OutInPs, index))
                {
                    ConstantMethod.Delay(500);
                    SetMPsONToOFF(pos1EnOutPs);
                }
                return true;
            }
            return false;
        }

        public bool SetPos2(string index)
        {
            if (pos2EnInPs.ShowValue == Constant.M_OFF)
            {
                if (SetDValue(pos2OutInPs, index))
                {
                    ConstantMethod.Delay(500);
                    SetMPsONToOFF(pos2EnOutPs);
                }
                return true;
            }
            return false;
        }

        public void SetPrintReport()
        {
            if (printReport == null)
            {
                printReport = new Report();
            }
            string searchPattern = "*.frx";
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string[] files = Directory.GetFiles(baseDirectory, searchPattern);
            if (Directory.GetFiles(baseDirectory, searchPattern).Length == 0)
            {
                MessageBox.Show(Constant.barCodeError);
            }
            else
            {
                if (Directory.GetFiles(baseDirectory, searchPattern).Length > 1)
                {
                    MessageBox.Show(Constant.barCodeError1);
                }
                if (Directory.GetFiles(baseDirectory, searchPattern).Length == 1)
                {
                    printReport.Load(files[0]);
                    printReport.PrintSettings.ShowDialog = false;
                }
            }
        }

        public void SetPrintReport(string fileName)
        {
            if (printReport == null)
            {
                printReport = new Report();
            }
            if (File.Exists(fileName))
            {
                printReport.Load(fileName);
                printReport.PrintSettings.ShowDialog = false;
            }
        }

        public void SetRtbResult(RichTextBox richrtbWork0)
        {
            rtbResult = richrtbWork0;
        }

        public void SetRtbWork(RichTextBox richrtbWork0)
        {
            rtbWork = richrtbWork0;
            if (MainForm !=null)
            {
                ConstantMethod.ChangeIco(MainForm);
            }
        }

        public void SetShowCnt(ComboBox cb1)
        {
            if (cb1 !=null)
            {
                optSize.CbResultCnt = cb1;
            }
        }

        private void SetSimiPLCMode()
        {
            if (optSize.MaterialId == Constant.patternMaterialId)
            {
                evokDevice.SetMValueON(inspectPatternModeInOutPs);
            }
            else
            {
                evokDevice.SetMValueOFF(inspectPatternModeInOutPs);
            }
        }

        private void SetSimiReady()
        {
            if (DeviceName.Equals(Constant.simiDeivceName))
            {
                SetPLCMaterialWidth();
                SetSimiPLCMode();
            }
        }


        public void SetSmallSizePrinter(string printerName)
        {
            if (ConstantMethod.GetLocalPrinter().Contains(printerName))
            {
                ParamFile.WriteConfig(Constant.minPrinterName, printerName);
                minPrinterName = printerName;
            }
        }

        public void SetUserDataGridView(DataGridView dgv1)
        {
            optSize.UserDataView = dgv1;
        }

        private void shiftColor()
        {
            ComboBox box = new ComboBox();
        }

        public bool shiftDataFormSplit(int formid, int rowSt, int count)
        {
            evokDevice.shiftDataFormSplit(formid, rowSt, count + 1);
            return true;
        }

        public void ShiftDgvParamLang(DataGridView dgvParam, int id)
        {
            if (id == 0)
            {
                dgvParam.Columns[0].DataPropertyName = evokDevice.DataFormLst[2].Columns[Constant.Bin].ToString();
            }
            else
            {
                dgvParam.Columns[0].DataPropertyName = evokDevice.DataFormLst[2].Columns[Constant.strParam10].ToString();
                dgvParam.Columns[0].HeaderText = "";
            }
            ConstantMethod.
            NoSortDatagridView(ref dgvParam);
        }

        public bool ShiftPage(int pageid)
        {
            
            if (CurrentPageId == pageid)
            {
                return true;
            }
            if (evokDevice.Status == 0)
            {
                if (pageid == 0)
                {
                    if (DeviceUser.Equals(Constant.DeviceUserOpeeinSimensi))
                    {
                        Constant.AutoPageID = Constant.Simensi_AutoPageID;
                    }
                  
                    evokDevice.SetDValue(pageShiftOutPs, Constant.AutoPageID);
                }   
                if (pageid == 1)
                {
                    if (DeviceUser.Equals(Constant.DeviceUserOpeeinSimensi))
                    {
                        Constant.HandPageID = Constant.Simensi_HandPageID;
                    }
                                     
                   evokDevice.SetDValue(pageShiftOutPs, Constant.HandPageID);

                }
                if (((pageid == 2) && (CurrentPageId < 4)) && !ConstantMethod.UserPassWd(Constant.PwdNoOffSet))
                {
                    return false;
                }
                evokDevice.shiftDataForm(pageid);
                FindPlcSimpleInPlcInfoLst(pageid);
                CurrentPageId = pageid;
                return true;
            }
            return false;
        }

        public void shiftToLine()
        {
            PlcInfoSimple startOutPs = new PlcInfoSimple(0, "D");
            startOutPs = startOutPs;
            startOutPs = lineStartOutPs;
            lineStartOutPs = startOutPs;
            startOutPs = startInPs;
            startInPs = lineStartInPs;
            lineStartInPs = startOutPs;
        }
        void ReLoadReport()
        {
            if(printReport !=null)
            {
                string fileName = printReport.FileName;
                printReport.Dispose();
                printReport = null;
                SetPrintReport(fileName);

            }
        }
      
        public void ShowBarCode(int rowindex)
        {
            ReLoadReport();
            if (printReport !=null)
            {

                List<string> list = new List<string>();
                if ((optSize.DtData != null) && (optSize.DtData.Rows.Count > 0))
                {
                    DataRow row = optSize.DtData.Rows[rowindex];
                    for (int i = 3; i < optSize.DtData.Columns.Count; i++)
                    {
                        list.Add(row[i].ToString());
                    }
                    printBarcode(printReport, list.ToArray(), 0);
                }
                else
                {
                    MessageBox.Show("�����ݣ����ȵ������ݣ�");
                }
            }
            else
            {
                MessageBox.Show("������ش���");
            }
        }

        public void ShowBarCode(Report rp1, int rowindex)
        {
            List<string> list = new List<string>();
            if ((optSize.DtData != null) && (optSize.DtData.Rows.Count > 0))
            {
                DataRow row = optSize.DtData.Rows[rowindex];
                for (int i = 3; i < optSize.DtData.Columns.Count; i++)
                {
                    list.Add(row[i].ToString());
                }
                printBarcode(rp1, list.ToArray(), 0);
            }
            else
            {
                MessageBox.Show("�����ݣ����ȵ������ݣ�");
            }
        }

        private void showDataWith2Ps(PlcInfoSimple psIn, PlcInfoSimple psOut)
        {
            if (((psOut.ShowValue > -1) && (psOut.ShowValue < psOut.ShowStr.Count)) && (psIn.ShowControl !=null))
            {
                ((Button)psIn.ShowControl).Text = psOut.ShowStr[psOut.ShowValue];
            }
        }

        public void ShowLblStatus()
        {
            if ((lblStatus !=null) && (deviceStatusId < Constant.constantStatusStr.Count<string>()))
            {
                lblStatus.Text = Constant.constantStatusStr[deviceStatusId];
                lblStatus.BackColor = Constant.colorRgb[deviceStatusId];
            }
        }

        public void showMxkSel(Label lbl)
        {
            if ((((lbl != null) && (mxkStringShowInPs.ShowStr != null)) && (mxkStringShowInPs.ShowStr.Count != 0)) && ((mxkStringShowInPs.ShowStr.Count > 0) && !string.IsNullOrWhiteSpace(mxkStringShowInPs.ShowStr[0])))
            {
                char[] separator = new char[] { '/' };
                string[] source = mxkStringShowInPs.ShowStr[0].Split(separator);
                if ((mxkStringShowInPs.ShowValue < source.Count<string>()) && (mxkStringShowInPs.ShowValue > -1))
                {
                    lbl.Text = source[mxkStringShowInPs.ShowValue];
                }
                else
                {
                    lbl.Text = Constant.prodLstNoData;
                }
            }
        }

        public void ShowNowLog(string filename0)
        {
            if (!File.Exists(filename0))
            {
                MessageBox.Show(Constant.DeviceNoLogFile);
            }
            else
            {
                LogForm form = new LogForm {
                    fileName = filename0
                };
                form.LoadData();
                form.ShowDialog();
            }
        }

        private void showPath(string pathname)
        {
            if (showFilePathLabel !=null)
            {              
                showFilePathLabel.Text = Path.GetFileName(pathname);
            }
        }

        public void showWorkInfo()
        {
            if (deviceStatusId < Constant.constantStatusStr.Count<string>())
            {
                ConstantMethod.ShowInfo(rtbWork, DeviceName + Constant.constantStatusStr[deviceStatusId]);
            }
        }

        public void showWorkInfo(string str)
        {
            ConstantMethod.ShowInfo(rtbWork, DeviceName + str);
        }

        public void SimiDataDownLoadSel(int id)
        {

            downLoadSizeId = id;
            optSize.simiDownLoadSizeId = id;

        }

        public bool start(int id)
        {
            evokDevice.SetMValueOFF2ON2OFF(startOutPs);
            ConstantMethod.Delay(200);
            CutReady(id);
            OldPrintBarCodeMode = PrintBarCodeMode;
            RunFlag = true;
            string[] logs = new string[] { DeviceName + Constant.DeviceStartCut };
            LogManager.WriteProgramLog(logs);
            return GetStartStatus();
        }

        public void StartCountClr()
        {
            if (!evokDevice.SetMValueOFF(startCountInOutPs))
            {
                ConstantMethod.Delay(100);
            }
        }

        public bool startCutDoor(int id)
        {
            ConstantMethod.Delay(300);
            evokDevice.SetMValueOFF2ON(startOutPs);
            CutReady();
            OldPrintBarCodeMode = PrintBarCodeMode;
            RunFlag = true;
            string[] logs = new string[] { DeviceName + Constant.DeviceStartCut };
            LogManager.WriteProgramLog(logs);
            return GetStartStatus();
        }

        public void StartWithOutDevice(int id)
        {
            showWorkInfo("��������");
            RunFlag = true;
            if (optSize.ProdInfoLst.Count > 0)
            {
                for (int i = CutProCnt; i < optSize.ProdInfoLst.Count; i++)
                {
                    if (!RunFlag) break;
                    Simi_Show(i);
                    SaveProdDataLog(optSize.ProdInfoLst[i], i);
                    ConstantMethod.ShowInfo(rtbWork, Constant.resultTip5 + ((i + 1)).ToString() + Constant.startTips4);
                    showWorkInfo("���PLC������");
                    ConstantMethod.Delay(0x3e8);
                    showWorkInfo("��������������");
                    CutLoopWithDevice(i);
                    SimicutoffProcess(optSize.ProdInfoLst[i]);
                    if (id > 100) break;
                }
            }
            else
            {
                MessageBox.Show(Constant.noData);
            }
            RunFlag = false;
        }

        PaintRect simi_painR;
        public xjplc.PaintRect Simi_painR
        {
            get { return simi_painR; }
            set { simi_painR = value; }
        }


        public void StartWithOutDeviceWithPattern(double patternDistance)
        {
            showWorkInfo("��������");
            RunFlag = true;
            double xoffset = 0;
            collectUserInputData();
            pSize = new patternSize();
            pSize.xiepoWidth = 4.0;
            pSize.patternWith = 12.0;
            pSize.patternHeight = 12.0;
            pSize.XBottomMargin = 2.5;
            pSize.YBottomMargin = 2.5;
            pSize.XNOPatternWidth = 8.5;
            optSize.PsSize = pSize;
            optSize.SetSimiMaterial();
            //��ȡ�ⳤ����
            optSize.OptMeasureWithSimiPattern
             (
                rtbResult, 
                optSize.DtData,
                GetPatternAllPos(patternDistance*Constant.dataMultiple,
                                 optSize.Len,                               
                                 pSize), 
                                 ref xoffset);


            if (optSize.ProdInfoLst.Count > 0)
            {
                for (int i = CutProCnt; i < optSize.ProdInfoLst.Count; i++)
                {
                    if (!RunFlag) break;
                    Simi_Show(i);
                    SaveProdDataLog(optSize.ProdInfoLst[i], i);
                    ConstantMethod.ShowInfo(rtbWork, Constant.resultTip5 + ((i + 1)).ToString() + Constant.startTips4);

                    showWorkInfo("���PLC������");

                    ConstantMethod.Delay(1000);

                    showWorkInfo("��������������");

                   

                    CutLoopWithDevice(i);

                    SimicutoffProcess(optSize.ProdInfoLst[i]);
                }
            }
            else
            {
                MessageBox.Show(Constant.noData);
            }
            RunFlag = false;
        }

        public bool stop()
        {
            if ((((deviceStatusId == Constant.constantStatusId[1]) || (deviceStatusId == Constant.constantStatusId[3])) || ((deviceStatusId == Constant.constantStatusId[4]) || (deviceStatusId == Constant.constantStatusId[6]))) || (deviceStatusId == Constant.constantStatusId[7]))
            {
                stopOperation();
                showWorkInfo();
                return true;
            }
            int num = 0;
            if (!string.IsNullOrWhiteSpace(stopOutPs.Area))
            {
                while (!evokDevice.SetMValueON(stopOutPs))
                {
                    num++;
                    Application.DoEvents();
                    if (num > 5)
                    {
                        break;
                    }
                }
            }
            int valueOld = 1;
            if (deviceStatusId != Constant.constantStatusId[0])
            {
                ConstantMethod.DelayWriteCmdOk(0xbb8, ref valueOld, ref stopInPs);
            }
            string[] logs = new string[] { DeviceName + Constant.DeviceStop };
            LogManager.WriteProgramLog(logs);
            try
            {
                if (deviceStatusId != Constant.constantStatusId[0])
                {
                    if (stopInPs.ShowValue == valueOld)
                    {
                        showWorkInfo(Constant.stopOk);
                        return true;
                    }
                    showWorkInfo(Constant.stopWrong);
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                stopOperation();
                string[] textArray2 = new string[] { DeviceName + Constant.DeviceStop };
                LogManager.WriteProgramLog(textArray2);
            }
            return false;
        }

        private void stopOperation()
        {
            RunFlag = false;

            CloseDataNotEnough();

            ConstantMethod.Delay(200);

            if ((CutThread != null) && CutThread.IsAlive)
            {
                CutThread.Abort();
            }

            if (stopOutPs != null)
            {
                evokDevice.SetMValueON(stopOutPs);
            }


            optSize.SingleSizeLst.Clear();
            optSize.ProdInfoLst.Clear();
            tCheckPrint.Enabled = false;


        }

        public void StopRunning() 
        {

            IsRuninng = false;

            if (updateData !=null)
            {
                updateData("");
            }
        }

        public bool testGetScarData(DataTable scarTable)
        {
            int showValue = scarInPs.ShowValue;
            if ((((showValue % 2) != 0) || (showValue < 1)) || (showValue > Constant.MaxScarCount))
            {
                return false;
            }
            showValue /= 2;
            scarTable.Rows.Clear();
            int scarStartAddress = Constant.ScarStartAddress;
            for (int i = 0; i < showValue; i++)
            {
                scarStartAddress += 2;
                DataRow row = scarTable.NewRow();
                row[0] = Constant.strDMArea[0] + scarStartAddress.ToString();
                row[1] = Constant.DoubleMode;
                row[2] = Constant.ScarName + i.ToString() + "-0";
                row[3] = "1";
                scarStartAddress += 2;
                DataRow row2 = scarTable.NewRow();
                row2[0] = Constant.strDMArea[0] + scarStartAddress.ToString();
                row2[1] = Constant.DoubleMode;
                row2[2] = Constant.ScarName + i.ToString() + "-1";
                row2[3] = "1";
                scarTable.Rows.Add(row);
                scarTable.Rows.Add(row2);
            }
            ShiftPage(Constant.ScarPage);
            ConstantMethod.Delay(0x3e8);
            ShiftPage(0);
            return true;
        }

        public void updateColName(DataGridView dgvParam)
        {
            dgvParam.Columns[0].HeaderText = Constant.paramHeader1;
            dgvParam.Columns[1].HeaderText = Constant.paramHeader2;
        }

        public void xialiaoSingleStart(OptSize op)
        {
            if (!op.IsDataValueAble)
            {
                MessageBox.Show(Constant.tataLineDeviceName[0] + Constant.optResultNoData);
            }
            else if (!getDeviceStatusIsReady(0))
            {
                MessageBox.Show(Constant.tataLineDeviceName[0] + Constant.errorDeviceStatus);
            }
            else
            {
                xialiaoStart(op);
            }
        }

        public void xialiaoSingleStop()
        {
            runflag_XialiaoJu = false;
        }

        private void xialiaoStart(OptSize op)
        {
            string[] logs = new string[] { "��ʼ�������Ͼ����������߳�" };
            LogManager.WriteProgramLog(logs);
            new Thread(new ParameterizedThreadStart(downLoadXiaLiaoju)).Start(op);
        }

        public List<List<PlcInfoSimple>> AllPlcSimpleLst
        {
            get
            {
                return allPlcSimpleLst;
            }
            set
            {
                allPlcSimpleLst = value;
            }
        }

        public bool AutoMes
        {
            get
            {
                if (autoMesOutInPs.Area == null)
                {
                    return false;
                }
                return (autoMesOutInPs.ShowValue == Constant.M_OFF);
            }
        }

        public int CurrentPageId
        {
            get { return currentPageId; }
         
            set
            {
                currentPageId = value;
            }
        }

        public int CutSelMode
        {
            get { return cutSelMode; }
        
            set
            {
                cutSelMode = value;
            }
        }

        public int DataFormCount { get { return evokDevice.DataFormLst.Count; } }
  

        public int DeviceMode
        {
            get
            {
                if (modeSelectOutInPs !=null)
                {
                    return modeSelectOutInPs.ShowValue;
                }
                return 0;
            }
        }

        public string DeviceName
        {
            get { return deviceName; }
        
            set
            {
                evokDevice.setDeviceId(value);
                deviceName = value;

                InitSimi();
            }
        }

        public int DeviceProperty
        {
            get { return deviceProperty; }
         
            set
            {
                deviceProperty = value;
            }
        }
        private string deviceUser = "";
        public string DeviceUser
        {
            get { return deviceUser; }
            set { deviceUser = value; }
        }

        public bool DeviceStatus { get { return (evokDevice.Status == 0); } }

        public bool IsOffLineMode { get { return evokDevice.Status < 0; } }
        public int deviceStatusId
        {
            get
            {
                if (deviceStatusOutInPs == null)
                {
                    return 0;
                }
                return deviceStatusOutInPs.ShowValue;
            }
        }

        public DataGridView DgvIO
        {
            get { return dgvIO; }
       
            set
            {
                dgvIO = value;
            }
        }

        public int doorbanCurrentId { get { return doorbanCurrentDoorIdInPs.ShowValue; } }
       

        public int doorshellCurrentId { get { return doorshellCurrentDoorIdInPs.ShowValue; } }
       

        public List<string> ErrorList
        {
            get { return errorList; }

            set
            {
                errorList = value;
            }
        }
        #region ˾������
        #endregion
        #region  ˮƽ��׻�
        public List<DataTable> HoleDataLst
        {
            get { return holeDataLst; }
           
            set
            {
                holeDataLst = value;
            }
        }

        public List<DataTable> GrooveDataLst
        {
            get { return grooveDataLst; }

            set
            {
                grooveDataLst = value;
            }
        }
        #endregion
        public bool IsInNoSafe
        {
            get { return ((emgStopInPs.ShowValue == Constant.M_ON) || (ErrorList.Count > 0)); }
        }

        public bool IsPrintBarCode { get { return (PrintBarCodeMode != Constant.NoPrintBarCode); } }
          

        public bool IsSaveProdLog
        {
            get { return isSaveProdLog; }

            set
            {
                isSaveProdLog = value;
            }
        }

        public bool lliao
        {
            get
            {
                return
                    (lliaoOutInPs.ShowValue == Constant.M_ON);
            }
        }

        public Form MainForm
        {
            get { return mainForm; }

            set
            {
                mainForm = value;
            }
        }

        public int OldPrintBarCodeMode
        {
            get { return oldPrintBarCodeMode; }

            set
            {
                oldPrintBarCodeMode = value;
            }
        }

        public ConfigFileManager ParamFile
        {
            get { return paramFile; }

            set
            {
                paramFile = value;
            }
        }

        public int PrintBarCodeMode
        {
            get { return printBarCodeMode; }

            set
            {
                printBarCodeMode = value;
            }
        }

        public List<PlcInfoSimple> PsLstAuto
        {
            get { return psLstAuto; }

            set
            {
                psLstAuto = value;
            }
        }

        public List<PlcInfoSimple> PsLstEx1
        {
            get { return psLstEx1; }

            set
            {
                psLstEx1 = value;
            }
        }

        public List<PlcInfoSimple> PsLstEx2
        {
            get { return psLstEx2; }
           
            set
            {
                psLstEx2 = value;
            }
        }

        public List<PlcInfoSimple> PsLstEx3
        {
            get { return psLstEx3; }
           
            set
            {
                psLstEx3 = value;
            }
        }

        public List<PlcInfoSimple> PsLstEx4
        {
            get { return psLstEx4; }
           
            set
            {
                psLstEx4 = value;
            }
        }

        public List<PlcInfoSimple> PsLstEx5
        {
            get { return psLstEx5; }
          
            set
            {
                psLstEx5 = value;
            }
        }

        public List<PlcInfoSimple> PsLstEx6
        {
            get { return psLstEx6; }
            
            set
            {
                psLstEx6 = value;
            }
        }

        public List<PlcInfoSimple> PsLstEx7
        {
            get { return psLstEx7; }
           
            set
            {
                psLstEx7 = value;
            }
        }

        public List<PlcInfoSimple> PsLstEx8
        {
            get { return psLstEx8; }
            
            set
            {
                psLstEx8 = value;
            }
        }

        public List<PlcInfoSimple> PsLstEx9
        {
            get { return psLstEx9; }

            set
            {
                psLstEx9 = value;
            }
        }

        public List<PlcInfoSimple> PsLstHand
        {
            get { return psLstHand; }

            set
            {
                psLstHand = value;
            }
        }

        public List<PlcInfoSimple> PsLstIO
        {
            get { return psLstIO; }

            set
            {
                psLstIO = value;
            }
        }

        public List<PlcInfoSimple> PsLstParam
        {
            get { return psLstParam; }

            set
            {
                psLstParam = value;
            }
        }

        public bool RunFlag
        {
            get { return mRunFlag; }

            set
            {
                mRunFlag = value;
            }
        }

        public bool ScarMode
        {
            get 
            {
                return (scarInPs.ShowValue > 0);
            }
        }
           

        public int SimimaterialId
        {
            get { return optSize.MaterialId; }
            
        }
        

        public bool startTip {
            get
            {
                return ((lineStartTipInPs !=null) && (lineStartTipInPs.ShowValue == 1));
            }
        }
           

        public DataTable UserDataTable
        {
            get { return userDataTable; }
            
            set
            {
                userDataTable = value;
            }
        }

        public int xialiaoCurrentId { get { return xialiaoCurrentDoorIdInPs.ShowValue; } }
       
    }

}                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              