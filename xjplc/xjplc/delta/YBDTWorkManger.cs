using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using xjplc.delta;

namespace xjplc
{

    public class YBDTWorkInfo
    {
        public YBDTWorkInfo()
        {

        }
        //按照组合好的列名称来
        public YBDTWorkInfo(DataRow dr)
        {
            DanHao = dr[Constant.strformatYB[0]].ToString();
            DateTimeDanhao = dr[Constant.strformatYB[1]].ToString();
            Department = dr[Constant.strformatYB[2]].ToString();
            TuHao = dr[Constant.strformatYB[3]].ToString();
            ProdName = dr[Constant.strformatYB[4]].ToString();
            GongXu = dr[Constant.strformatYB[5]].ToString();
            GyTx = dr[Constant.strformatYB[6]].ToString();
            OperatorName = dr[Constant.strformatYB[7]].ToString();
            OperatorTx = dr[Constant.strformatYB[8]].ToString();
            DeviceClass = dr[Constant.strformatYB[9]].ToString();
            DeviceId = dr[Constant.strformatYB[10]].ToString();
            DeviceIP = dr[Constant.strformatYB[11]].ToString();
            DeviceTx = dr[Constant.strformatYB[12]].ToString();
            CadPath = dr[Constant.strformatYB[13]].ToString();
            Ddsm = dr[Constant.strformatYB[14]].ToString();
            SetProdQuantity = dr[Constant.strformatYB[15]].ToString();
            Speed = dr[Constant.strformatYB[16]].ToString();
            Jshu = dr[Constant.strformatYB[17]].ToString();
            Gmj = dr[Constant.strformatYB[18]].ToString();

        }
        string danHao;
        public string DanHao
        {
            get { return danHao; }
            set { danHao = value; }
        }
        string dateTimeDanhao;
        public string DateTimeDanhao
        {
            get { return dateTimeDanhao; }
            set { dateTimeDanhao = value; }
        }
        string department;
        public string Department
        {
            get { return department; }
            set { department = value; }
        }
        string tuHao;
        public string TuHao
        {
            get { return tuHao; }
            set { tuHao = value; }
        }
        string prodName;
        public string ProdName
        {
            get { return prodName; }
            set { prodName = value; }
        }
        string gongXu;
        public string GongXu
        {
            get { return gongXu; }
            set { gongXu = value; }
        }
        string gyTx;
        public string GyTx
        {
            get { return gyTx; }
            set { gyTx = value; }
        }
        string operatorName;
        public string OperatorName
        {
            get { return operatorName; }
            set { operatorName = value; }
        }
        string operatorTx;
        public string OperatorTx
        {
            get { return operatorTx; }
            set { operatorTx = value; }
        }
        string deviceClass;
        public string DeviceClass
        {
            get { return deviceClass; }
            set { deviceClass = value; }
        }
        string deviceId;
        public string DeviceId
        {
            get { return deviceId; }
            set { deviceId = value; }
        }
        string deviceIP;
        public string DeviceIP
        {
            get { return deviceIP; }
            set { deviceIP = value; }
        }
        string deviceTx;
        public string DeviceTx
        {
            get { return deviceTx; }
            set { deviceTx = value; }
        }
        string cadPath;
        public string CadPath
        {
            get { return cadPath; }
            set { cadPath = value; }
        }
        string ddsm;
        public string Ddsm
        {
            get { return ddsm; }
            set { ddsm = value; }
        }
        string setProdQuantity;
        public string SetProdQuantity
        {
            get { return setProdQuantity; }
            set { setProdQuantity = value; }
        }
        string speed;
        public string Speed
        {
            get { return speed; }
            set { speed = value; }
        }
        string jshu;
        public string Jshu
        {
            get { return jshu; }
            set { jshu = value; }
        }
        string gmj;
        public string Gmj
        {
            get { return gmj; }
            set { gmj = value; }
        }



    }

    public class YBDTWork
    {
                  
        YBDTDevice ybtdDevice;
        public xjplc.delta.YBDTDevice YbtdDevice
        {
            get { return ybtdDevice; }
            set { ybtdDevice = value; }
        }

        ExcelNpoi excelop;
        private List<string> strDataFormPath;
        System.Timers.Timer UpdateTimer;
        DateTime startTime;
        public DateTime StartTime
        {
            get { return startTime; }
            set { startTime = value; }
        }
        int readSpeed=1;
        public int ReadSpeed
        {
            get { return readSpeed; }
            set { readSpeed = value; }
        }
        DateTime endRealTime = DateTime.Now;
        public System.DateTime EndRealTime
        {
            get {

                int setQuantityInt = 0;

                if (int.TryParse(ybdtWorkInfo.SetProdQuantity, out setQuantityInt))
                {
                    if (setQuantityInt > ProdQuantity && ReadSpeed >0)
                    {
                        int timeSec = (setQuantityInt - ProdQuantity) * ReadSpeed;
                        endRealTime = StartTime.AddSeconds(timeSec);
                    }
                }
                
                
                return endRealTime;
            }
            
        }
        
        //理论结束时间
        DateTime endNeedTime ;
        public DateTime EndNeedTime
        {
            get
            {
                int speedInt=0;
                int setQuantityInt=0;
                if (int.TryParse(ybdtWorkInfo.Speed, out speedInt)&&(int.TryParse(ybdtWorkInfo.SetProdQuantity, out setQuantityInt)))
                {
                    if (ProdQuantity < setQuantityInt && speedInt > 0)
                    {
                        endNeedTime = StartTime.AddSeconds((setQuantityInt - ProdQuantity) * speedInt);
                    }
                    else
                    {
                        endNeedTime = StartTime;
                    }
                }
                return endNeedTime;
            }            
        }

        int prodQuantity;
        public int ProdQuantity
        {
            get {
                prodQuantity = proQuantityInOutPs.ShowValue;
                return prodQuantity;
            }
           
        }


        DataTable sqlDeiviceInfoDatatable;
        public System.Data.DataTable SqlDeiviceInfoDatatable
        {
            get { return sqlDeiviceInfoDatatable; }
            set { sqlDeiviceInfoDatatable = value; }
        }
        List<int> stopTime;
        public System.Collections.Generic.List<int> StopTime
        {
            get { return stopTime; }
            set { stopTime = value; }
        }
        public System.Collections.Generic.List<string> StrDataFormPath
        {
            get { return strDataFormPath; }
            set { strDataFormPath = value; }
        }
        public DTPlcInfoSimple startInPs = new DTPlcInfoSimple("车床1启动读");
        public DTPlcInfoSimple startInPs1 = new DTPlcInfoSimple("车床2启动读");
        public DTPlcInfoSimple startInPs2 = new DTPlcInfoSimple("工件校准气缸读");
        public DTPlcInfoSimple startInPs3 = new DTPlcInfoSimple("夹头开读");
        public DTPlcInfoSimple startInPs4 = new DTPlcInfoSimple("夹头锁紧读");
        public DTPlcInfoSimple startInPs5 = new DTPlcInfoSimple("检测气缸2读");
        public DTPlcInfoSimple startInPs6 = new DTPlcInfoSimple("开夹头读");
        public DTPlcInfoSimple startInPs7 = new DTPlcInfoSimple("关夹头读");
        public DTPlcInfoSimple startInPs8 = new DTPlcInfoSimple("1#机械手停止读");
        public DTPlcInfoSimple startInPs9 = new DTPlcInfoSimple("2#机械手停止读");
        public DTPlcInfoSimple startInPs18 = new DTPlcInfoSimple("机械手离开信号1读");
        public DTPlcInfoSimple startInPs10 = new DTPlcInfoSimple("机械手离开离开2读");
        public DTPlcInfoSimple startInPs11 = new DTPlcInfoSimple("车床1产品位置检测读");
        public DTPlcInfoSimple startInPs12 = new DTPlcInfoSimple("车床2产品位置检测读");
        public DTPlcInfoSimple startInPs13 = new DTPlcInfoSimple("气缸回位信号1读"); 
        public DTPlcInfoSimple startInPs14 = new DTPlcInfoSimple("气缸回位信号2读");
        public DTPlcInfoSimple startInPs15 = new DTPlcInfoSimple("复位读");
        public DTPlcInfoSimple startInPs16 = new DTPlcInfoSimple("1#数控车床报警读");
        public DTPlcInfoSimple startInPs17 = new DTPlcInfoSimple("2#数控车床报警读");
        public DTPlcInfoSimple proQuantityInOutPs = new DTPlcInfoSimple("产量读写");
        public YBDTWork(Socket soc,YBDTWorkInfo y0)
        {
                   
            //加载数据如果不对 那就用户自己加载           
            StrDataFormPath = new List<string>();
            StrDataFormPath.Add(Constant.PlcDataFilePathAuto);
            StrDataFormPath.Add(Constant.PlcDataFilePathHand);
            StrDataFormPath.Add(Constant.PlcDataFilePathParam);
            StrDataFormPath.Add(Constant.PlcDataFilePathIO);
            startTime = DateTime.Now;
            for (int i = StrDataFormPath.Count - 1; i >= 0; i--)
            {
                if (!File.Exists(StrDataFormPath[i]))
                {
                    StrDataFormPath.RemoveAt(i);
                    MessageBox.Show(Constant.ErrorPlcFile);
                    return;
                }
            }

            YbtdDevice = new YBDTDevice(StrDataFormPath,soc);

            excelop = new ExcelNpoi();

            excelop.FileName = string.Concat(
            Constant.AppFilePath, DateTime.Now.ToString("yyyMMdd"), Constant.prodResult,".xlsx"); ////"\\configParam.xml"
          
            YbdtWorkInfo = y0;

            AllPlcSimpleLst = new List<List<DTPlcInfoSimple>>();
            PsLstAuto = new List<DTPlcInfoSimple>();
            PsLstAuto.Add(startInPs);
            PsLstAuto.Add(startInPs1);
            PsLstAuto.Add(startInPs2);
            PsLstAuto.Add(startInPs3);
            PsLstAuto.Add(startInPs4);
            PsLstAuto.Add(startInPs5);
            PsLstAuto.Add(startInPs6);
            PsLstAuto.Add(startInPs7);
            PsLstAuto.Add(startInPs8);
            PsLstAuto.Add(startInPs9);
            PsLstAuto.Add(startInPs18);
            PsLstAuto.Add(startInPs10);
            PsLstAuto.Add(startInPs11);
            PsLstAuto.Add(startInPs12);
            PsLstAuto.Add(startInPs13);
            PsLstAuto.Add(startInPs14);
            PsLstAuto.Add(startInPs15);
            PsLstAuto.Add(startInPs16);
            PsLstAuto.Add(startInPs17);
            PsLstAuto.Add(startInPs12);
            PsLstAuto.Add(startInPs13);
            PsLstAuto.Add(startInPs14);
            PsLstAuto.Add(startInPs15);
            PsLstAuto.Add(startInPs16);
            PsLstAuto.Add(startInPs17);
            PsLstAuto.Add(proQuantityInOutPs);
            AllPlcSimpleLst.Add(PsLstAuto);
            StopTime = new List<int>();
            SaveData();
            //设置数据库定时更新
            UpdateTimer = new System.Timers.Timer();
            UpdateTimer.Elapsed += UpdateSqlTimeEvent;
            UpdateTimer.Interval = 1000;
            UpdateTimer.AutoReset = true;

            if (!YbtdDevice.getDeviceData())
            {
                //MessageBox.Show(Constant.ConnectMachineFail);
                Dispose();              
                return;
            }
            SqlDeiviceInfoDatatable = new DataTable();
           
            UpdateTimer.Enabled = true;
        }
            
        YBDTWorkInfo ybdtWorkInfo;
               
        public xjplc.YBDTWorkInfo YbdtWorkInfo
        {
            get { return ybdtWorkInfo; }
            set { ybdtWorkInfo = value; }
        }

        private void UpdateSqlTimeEvent(object source, System.Timers.ElapsedEventArgs e)
        {
            UpdateSql();
            CreateSql();
            SaveDeviceToSql();
        }

        private List<DTPlcInfoSimple> psLstAuto;
        public System.Collections.Generic.List<xjplc.DTPlcInfoSimple> PsLstAuto
        {
            get { return psLstAuto; }
            set { psLstAuto = value; }
        }

        
        public List<List<DTPlcInfoSimple>> AllPlcSimpleLst { get; private set; }
        public void InitControl()
        {
            if ((ybtdDevice.DataFormLst.Count > 0) && (ybtdDevice.DataFormLst[0] != null))
            {
                ConstantMethod.FindPos(ybtdDevice.DataFormLst[0], PsLstAuto);
            }           
        }
        bool isSaving;
        public bool IsSaving
        {
            get { return isSaving; }
            set { isSaving = value; }
        }
        public void Dispose()
        {
            if (YbtdDevice != null || YbtdDevice.Status == Constant.DeviceConnected)
            {
                YbtdDevice.SocManager.Dispose();             
            }
            UpdateTimer.Enabled = false;
         
        }
        #region 数据保存

        public void WriteDataToFile(string filename,string[] data)
        {
            if (!string.IsNullOrWhiteSpace(filename)
                && File.Exists(filename)
                && data.Length>0)
            {
                IsSaving = true;
                excelop.ChangeCellValue(data,data[7]);                             
            }
            else
            {
                if (!File.Exists(filename))
                {
                    CreateDataTable(filename, Constant.strformatYBSave.ToArray());
                }               
            }
        }
        public List<string> PackDr()
        {
            List<string> workDataRow = new List<string>();

            workDataRow.Add(ybdtWorkInfo.DanHao);
            workDataRow.Add(ybdtWorkInfo.DateTimeDanhao);
            workDataRow.Add(ybdtWorkInfo.Department);
            workDataRow.Add(ybdtWorkInfo.TuHao);
            workDataRow.Add(ybdtWorkInfo.ProdName);
            workDataRow.Add(ybdtWorkInfo.GongXu);
            workDataRow.Add(ybdtWorkInfo.OperatorName);
            workDataRow.Add(ybdtWorkInfo.DeviceId);
            workDataRow.Add(ProdQuantity.ToString());
            workDataRow.Add(StartTime.ToLocalTime().ToString());
            workDataRow.Add(EndRealTime.ToLocalTime().ToString());
            workDataRow.Add("0");
            workDataRow.Add("null");

            return workDataRow;
        }
        public bool CreateDataTable(string Filename,string[] headerName)
        {
            if (!File.Exists(Filename))
            {
                DataTable dt = new DataTable();
                for (int i = 0; i < headerName.Length; i++)
                {
                    dt.Columns.Add(headerName[i], Type.GetType("System.String"));
                }               

                dt.Rows.Add(PackDr().ToArray());

                excelop.FileName = Filename;

                excelop.ExportDataToExcelNoDialog(dt,Filename,null,null);

                return true;
            }
            return false;                                 
        }      
        public void SaveData()
        {                      
            try
            {
                isSaving = true;
                WriteDataToFile(excelop.FileName, PackDr().ToArray());
            }
            catch
            {

            }
            finally
            {
                isSaving = false;
            }
        }
        #endregion
        private void FindPlcSimpleInPlcInfoLst(int m)
        {

            foreach (List<DTPlcInfoSimple> pLst in AllPlcSimpleLst)
            {
                foreach (DTPlcInfoSimple p in pLst)
                {
                    FindPlcInfo(p, ybtdDevice.DPlcInfo, ybtdDevice.MPlcInfoAll);
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
        public bool ShiftPage(int pageid)
        {
            if (YbtdDevice.Status == Constant.DeviceConnected)
            {


                YbtdDevice.shiftDataForm(pageid);

                FindPlcSimpleInPlcInfoLst(pageid);

                ConstantMethod.Delay(50);

                return true;
            }

            return false;

        }


        /// <summary>
        /// 在数据库中 建立以设备编号为名称的数据库表格
        /// </summary>
        public void CreateSql()
        {
            if (!SqlHelper.IsTableExist(Constant.sqlDb, YbdtWorkInfo.DeviceId))
            {
                Dictionary<string, string> sDic = new Dictionary<string, string>();
               
                foreach (string s in Constant.plcDataFile)
                {
                    sDic.Add(s, Constant.sqlChar20);
                }               

                SqlHelper.CreateDataTable(Constant.sqlDb, YbdtWorkInfo.DeviceId, sDic);
                
            }
        }
        public bool SaveDeviceToSql()
        {
            //获取数据库表格数据
            //获取是否有数据操作
            //将现有数据更新到表格

            string sql = " SELECT * FROM "+YbdtWorkInfo.DeviceId;
            DataTable tempDataTable = SqlHelper.ExecuteDataTable(sql);

            foreach (DataRow dr in tempDataTable.Rows)
            {
                int opInt=0;
                int valueInt=0;
                string binStr = dr[Constant.Bin].ToString();

               if (int.TryParse(dr[Constant.strParam3].ToString(), out opInt) && (int.TryParse(dr[Constant.strParam4].ToString(),out valueInt)))
               {
                    if (opInt == Constant.sqlWrite)
                    {

                        DTPlcInfoSimple p = getPsFromPslLst(binStr,Constant.Read,PsLstAuto);
                        if (p.Area.Equals(Constant.strDMArea[0]) || p.Area.Equals(Constant.strDMArea[1]))
                        {
                            YbtdDevice.SetDValue(p, valueInt);
                        }
                        else
                        {
                            if (p.Area.Equals(Constant.strDMArea[3]) ||
                                p.Area.Equals(Constant.strDMArea[4]) ||
                                p.Area.Equals(Constant.strDMArea[5]) ||
                                p.Area.Equals(Constant.strDMArea[6]) 
                                )
                            {
                                if (valueInt == Constant.M_ON)
                                {
                                    YbtdDevice.SetMValueON(p);
                                }
                                if (valueInt == Constant.M_OFF)
                                {
                                    YbtdDevice.SetMValueOFF(p);
                                }
                            }
                        }
                    }
                }
            }


            UpdatePlcStatus(YbtdDevice.DataFormLst[0]);

            return true;
        }

        private void UpdatePlcStatus(DataTable  dt)
        {
            if (dt != null && dt.Rows.Count > 0)
            {
                SqlHelper.UpdateFromDtByName(YbdtWorkInfo.DeviceId.Trim(), dt);
            }
        }

        public void UpdateSql()
        {
            //获取数据库数据
            //获取加载不需要改的数据
            //将datarow写回去更新          

            string sql = " SELECT * FROM deviceinfo where 设备地址="+"'"+YbdtWorkInfo.DeviceIP+"'";

            SqlDeiviceInfoDatatable = SqlHelper.ExecuteDataTable(sql);
            try
            {
                if (SqlDeiviceInfoDatatable.Rows.Count == 1)
                {
                    YbdtWorkInfo.DanHao = SqlDeiviceInfoDatatable.Rows[0][Constant.strformatYB[0]].ToString();
                    YbdtWorkInfo.DateTimeDanhao = SqlDeiviceInfoDatatable.Rows[0][Constant.strformatYB[1]].ToString();
                    YbdtWorkInfo.Department = SqlDeiviceInfoDatatable.Rows[0][Constant.strformatYB[2]].ToString();
                    YbdtWorkInfo.TuHao = SqlDeiviceInfoDatatable.Rows[0][Constant.strformatYB[3]].ToString();
                    YbdtWorkInfo.ProdName = SqlDeiviceInfoDatatable.Rows[0][Constant.strformatYB[4]].ToString();
                    YbdtWorkInfo.GongXu = SqlDeiviceInfoDatatable.Rows[0][Constant.strformatYB[5]].ToString();
                    YbdtWorkInfo.GyTx = SqlDeiviceInfoDatatable.Rows[0][Constant.strformatYB[6]].ToString();
                    YbdtWorkInfo.OperatorName = SqlDeiviceInfoDatatable.Rows[0][Constant.strformatYB[7]].ToString();
                    YbdtWorkInfo.OperatorTx = SqlDeiviceInfoDatatable.Rows[0][Constant.strformatYB[8]].ToString();
                    YbdtWorkInfo.DeviceClass = SqlDeiviceInfoDatatable.Rows[0][Constant.strformatYB[9]].ToString();
                    YbdtWorkInfo.DeviceId = SqlDeiviceInfoDatatable.Rows[0][Constant.strformatYB[10]].ToString();
                    YbdtWorkInfo.DeviceIP = SqlDeiviceInfoDatatable.Rows[0][Constant.strformatYB[11]].ToString();
                    YbdtWorkInfo.DeviceTx = SqlDeiviceInfoDatatable.Rows[0][Constant.strformatYB[12]].ToString();
                    YbdtWorkInfo.CadPath = SqlDeiviceInfoDatatable.Rows[0][Constant.strformatYB[13]].ToString();
                    YbdtWorkInfo.Ddsm = SqlDeiviceInfoDatatable.Rows[0][Constant.strformatYB[14]].ToString();
                    YbdtWorkInfo.SetProdQuantity = SqlDeiviceInfoDatatable.Rows[0][Constant.strformatYB[15]].ToString();
                    YbdtWorkInfo.Speed = SqlDeiviceInfoDatatable.Rows[0][Constant.strformatYB[16]].ToString();
                    YbdtWorkInfo.Jshu = SqlDeiviceInfoDatatable.Rows[0][Constant.strformatYB[17]].ToString();
                    YbdtWorkInfo.Gmj = SqlDeiviceInfoDatatable.Rows[0][Constant.strformatYB[18]].ToString();

                    SqlDeiviceInfoDatatable.Rows[0]["当前产量"] = ProdQuantity;
                    SqlDeiviceInfoDatatable.Rows[0]["实际节拍"] = ReadSpeed;
                    SqlDeiviceInfoDatatable.Rows[0]["开始时间"] = StartTime;
                    SqlDeiviceInfoDatatable.Rows[0]["理论结束时间"] = EndNeedTime;
                    SqlDeiviceInfoDatatable.Rows[0]["实际结束时间"] = EndRealTime;

                }

                UpdateDeviceInfoLst(SqlDeiviceInfoDatatable);
            }
            catch(Exception ex)
            { }
            

        }
        private void UpdateDeviceInfoLst(DataTable dt)
        {

            if (dt != null && dt.Rows.Count > 0)
            {
                SqlHelper.UpdateFromDeviceInfo(dt);
            }

        }
        #region 设置寄存器数据
        public void TestSetMON()
        {

            ybtdDevice.SetMValueON(startInPs);

        }
        public void TestSetMOFF()
        {

            ybtdDevice.SetMValueOFF(startInPs);
        }

        public void ClrQuantity()
        {
            ybtdDevice.SetDValue(proQuantityInOutPs,0);
        }
        public void SetDValue(string str1, string str2, List<DTPlcInfoSimple> pLst, int num)
        {
            DTPlcInfoSimple p = getPsFromPslLst(str1, str2, pLst);

            if (p != null)
            {
                YbtdDevice.SetDValue(p, num);
            }
            else
            {
               // MessageBox.Show(Constant.SetDataFail);
            }

        }

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

        public void SetMPsOFFToOn(string str1, string str2, List<DTPlcInfoSimple> pLst)
        {
            DTPlcInfoSimple p = getPsFromPslLst(str1, str2, pLst);
            if (p != null)
            {
                YbtdDevice.SetMValueOFF2ON(p);
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
                YbtdDevice.SetMValueON(p);
            }
            else
            {
                MessageBox.Show(Constant.SetDataFail);
            }
        }
        #endregion
    }

    //一切从这里出发 socserver 负责修改这个worklst集合 传输到 socclient里 进行删除
    public class YBDTWorkManger
    {
        string UserDtFileName = string.Concat(
            Constant.AppFilePath, DateTime.Now.ToString("yyyMMdd"), ".xlsx");

        ExcelNpoi Excelop;

        List<YBDTWork> ybdtWorkLst;

        List<YBDTWorkInfo> ybdtWorkInfoLst;
        public System.Collections.Generic.List<xjplc.YBDTWorkInfo> YbdtWorkInfoLst
        {
            get { return ybdtWorkInfoLst; }
            set { ybdtWorkInfoLst = value; }
        }

        
        bool isLoadData;
        public bool IsLoadData
        {
            get { return isLoadData; }
            set { isLoadData = value; }
        }
        public System.Collections.Generic.List<xjplc.YBDTWork> YbdtWorkLst
        {
            get { return ybdtWorkLst; }
            set { ybdtWorkLst = value; }
        }
        DataTable userDt;
        public System.Data.DataTable UserDt
        {
            get { return userDt; }
            set { userDt = value; }
        }

        List<string> itemShowStr;
        public System.Collections.Generic.List<string> ItemShowStr
        {
            get { return itemShowStr; }
            set { itemShowStr = value; }
        }
        DataTable sqlShowDataTable;
        public System.Data.DataTable SqlShowDataTable
        {
            get { return sqlShowDataTable; }
            set { sqlShowDataTable = value; }
        }
        SocServer socServer;

        public event ydtdWorkChanged ydtdWorkChangedEvent;//利用委托来声明事件
        public YBDTWorkManger()
        {
            YbdtWorkLst = new List<YBDTWork>();
                         
            socServer = new SocServer();
            socServer.newClientIn += WorkClientAdd;
                       
            UserDt = new DataTable(Constant.sqlDataTableName);
            Excelop = new ExcelNpoi();
            YbdtWorkInfoLst = new List<YBDTWorkInfo>();

            if (!LoadExcelData(UserDtFileName))
            {

                MessageBox.Show(Constant.ErrorPlcFile);
                ConstantMethod.AppExit();
            }

            //获取用户导入信息 导入到数据库
            if (!GetDeviceData(UserDt))
            {
                MessageBox.Show(Constant.ErrorPlcFile);
                ConstantMethod.AppExit();
            }

            UserDt.TableName = Constant.sqlDataTableName;
            LoadItemShow();
            UpdateDeviceInfoLstToSql(UserDt);

           
            socServer.startServer();

            socServer.YbWorkLst = YbdtWorkLst;

        }
        bool isInEdit;
        public bool IsInEdit
        {
            get { return isInEdit; }
            set { isInEdit = value; }
        }
        private void LoadItemShow()
        {
            ExcelNpoi excelop = new ExcelNpoi();
            DataTable showItemDataTable = excelop.ImportExcel(Constant.showItemPath);
            ItemShowStr = new List<string>();
            if (showItemDataTable != null && showItemDataTable.Rows.Count > 0)
            {
                foreach (DataRow dr in showItemDataTable.Rows)
                {
                    
                    if (!string.IsNullOrWhiteSpace(dr[1].ToString()))
                        ItemShowStr.Add(dr[1].ToString());
                };
            }
        }
        //从软件的数据表格格式转化为sql数据格式
        public DataTable DataTableShift(DataTable desT)
        {

            DataTable dest = new DataTable();
            dest = desT.Clone();
            for (int m = 0; m < dest.Columns.Count; m++)
            {
                dest.Columns[m].DataType = Type.GetType(Constant.strformatSqlType[m]);
            }
            for (int i = 0; i < desT.Rows.Count; i++)
            {
                try
                {

                    DataRow dr = dest.NewRow();

                    for (int j = 0; j < desT.Columns.Count; j++)
                    {
                        dr[j] = desT.Rows[i][j];
                    }

                    dest.Rows.Add(dr);
                }
                catch(Exception ex)
                {

                }

            }
            return dest;

        }
        private void UpdateDeviceInfoLstToSql(DataTable dt)
        {
            if (dt != null && dt.Rows.Count > 0)
            {
                SqlHelper.UpdateFromDeviceInfo(DataTableShift(dt));
            }
        }

        private void DataTableShowItem(DataTable dt,string[] itemShow)
        {
            List<string> colstrLst = new List<string>();
            foreach (DataColumn dc in dt.Columns)
            {
                colstrLst.Add(dc.ColumnName);
            }
            foreach(string s in colstrLst)
            {
                if (!itemShow.Contains(s))
                {
                    dt.Columns.Remove(s);
                }   
            }
        }
        public void GetDataFromSql(DataGridView dg1)
        {
            string sqlStr = "SELECT * FROM deviceinfo" + " WHERE ";
                      
            if (ybdtWorkLst.Count == 0)
            {
                dg1.DataSource = null;
                if(SqlShowDataTable !=null)
                SqlShowDataTable.Rows.Clear();
                return;
            }

            for(int i=0;i<YbdtWorkLst.Count;i++)
            {
                

                if (i == YbdtWorkLst.Count - 1)
                {
                    sqlStr += "设备地址=" + "'" + YbdtWorkLst[i].YbdtWorkInfo.DeviceIP + "'";
                }
                else
                {
                    sqlStr += "设备地址=" + "'" + YbdtWorkLst[i].YbdtWorkInfo.DeviceIP + "'" + " or ";
                }
                              
            }
        
            DataTable dt0 = SqlHelper.ExecuteDataTable(sqlStr);
                      

            if (SqlShowDataTable == null || dt0.Rows.Count != SqlShowDataTable.Rows.Count || SqlShowDataTable.Rows.Count ==0)
            {
                SqlShowDataTable = dt0.Copy();
                DataTableShowItem(SqlShowDataTable, itemShowStr.ToArray());
                dg1.DataSource = SqlShowDataTable;
            }
            else
            {
                for (int i = 0; i < dt0.Rows.Count; i++)
                {
                    for (int j = 0; j < SqlShowDataTable.Columns.Count; j++)
                    {
                        if (j < dt0.Columns.Count)
                        {
                            SqlShowDataTable.Rows[i][j] = dt0.Rows[i][SqlShowDataTable.Columns[j].ColumnName];
                        }
                    }
                }
            }
           
        }

        public YBDTWorkInfo GetWorkInfoFromLst(List<YBDTWorkInfo> workInfoLst, string deviceIp)
        {
            foreach (YBDTWorkInfo y in workInfoLst)
            {
                if (y.DeviceIP.Trim().Equals(deviceIp.Trim()))
                {
                    return y;
                }
            }
            return null;
        }
        public YBDTWork GetYBDTWorkFromLst(List<YBDTWork> workLst, string deviceIp)
        {
            foreach (YBDTWork y in workLst)
            {
                if (y.YbdtWorkInfo.DeviceIP.Trim().Equals(deviceIp.Trim()))
                {
                    return y;
                }         
            }
            return null;

        }
        void WorkClientAdd(object sender, Socket e)
        {
            string strIp = (e.RemoteEndPoint as IPEndPoint).Address.ToString();
            YBDTWorkInfo yw = GetWorkInfoFromLst(YbdtWorkInfoLst, strIp);
            if (yw != null)
            {
                YBDTWork ybdtwork = new YBDTWork(e, yw);
                if (ybdtwork.YbtdDevice.Status == Constant.DeviceConnected)
                    YBDTWorkChanged(Constant.AddWork, ybdtwork);
                else
                    ybdtwork.Dispose();
            }
                      
        }

        public void YBDTWorkChanged(int op,YBDTWork ybdtwork)
        {
            switch (op)
           {
                case Constant.AddWork:
                    {
                        if (YbdtWorkLst != null)
                        {
                           
                            //按照正产逻辑 消息 是一层一层往上报的
                            ybdtwork.YbtdDevice.SocManager.WorkManger = this;
                            ybdtwork.InitControl();
                            ybdtwork.ShiftPage(0);
                            YbdtWorkLst.Add(ybdtwork);
                            if (ydtdWorkChangedEvent != null)
                            {
                                ydtdWorkChangedEvent(this, ybdtwork);
                            }
                        }

                        break;
                    }
                case Constant.DelWork:
                    {
                        if (YbdtWorkLst != null)
                        {
                            YbdtWorkLst.Remove(ybdtwork);
                            if (ydtdWorkChangedEvent != null)
                            {
                                ydtdWorkChangedEvent(this, ybdtwork);
                            }                                                      
                        }
                        break;
                    }
            }
            

        }
        public bool GetDeviceData(DataTable dt)
        {
            
            if (dt != null )
            {
                foreach (DataRow dr in dt.Rows)
                {
                    for (int j = 0; j < dr.ItemArray.Length; j++)
                    {
                        if (string.IsNullOrWhiteSpace(dr[j].ToString()))
                        {
                            continue;
                        }
                       
                    }

                    YBDTWorkInfo ybWorkInfo = new YBDTWorkInfo(dr);
                    
                    YbdtWorkInfoLst.Add(ybWorkInfo);
                }
                
            }
            if (YbdtWorkInfoLst.Count < 1) return false;
            return true;
        }
        public bool LoadExcelData(string filename)
        {
            LogManager.WriteProgramLog(Constant.LoadFileSt + filename);
            if (!File.Exists(filename))
            {
                return false;
            }
            IsLoadData = true;
            UserDt = Excelop.ImportExcel(filename);

            if (UserDt.Rows.Count < 1) return false;

            string[] str = ConstantMethod.GetColumnsByDataTable(UserDt);

            if (str == null) return false;

            if (!ConstantMethod.compareString(str, Constant.strformatYB))
            {
                return false;
            }

            Excelop.FileName = filename;

            UserDt.Columns.Add("实际节拍");
            UserDt.Columns.Add("当前产量");
            UserDt.Columns.Add("开始时间");
            UserDt.Columns.Add("理论结束时间");
            UserDt.Columns.Add("实际结束时间");

            IsLoadData = false;

            LogManager.WriteProgramLog(Constant.LoadFileEd);
            return true;
        }
    }
}
