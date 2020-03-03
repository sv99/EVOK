using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Data;
using System.IO;
using Modbus.Device;
//#----------------针对MODBUS tcp进行数据采集
namespace xjplc.ky
{
    public  class DeviceManager
    {

        Device dev1;
        Device dev2;
        Device dev3;
        Device dev4;
        Device dev5;
        
        System.Timers.Timer deviceMointor;

        private List<Device> devLst;
        public System.Collections.Generic.List<xjplc.ky.Device> DevLst
        {
            get { return devLst; }
            set { devLst = value; }
        }
        public DeviceManager()
        {
            deviceMointor = new System.Timers.Timer(5000);  

            deviceMointor.Enabled = false;

            deviceMointor.AutoReset = true;

            deviceMointor.Elapsed += new System.Timers.ElapsedEventHandler(deviceMointorEvent);

            DevLst = new List<Device>();
            
            if (File.Exists(Constant.ConfigDevice1))
            {
                dev1 = new Device(Constant.ConfigDevice1, Constant.ConfigServerPortFilePath1, Constant.deltaPlc);
                dev1.DeviceName = "双头合页机";
            }

            /***
            if (File.Exists(Constant.ConfigDevice2))
            {
                dev2 = new Device(Constant.ConfigDevice2, Constant.ConfigServerPortFilePath2, Constant.deltaPlc);
                dev2.DeviceName = "锁槽拉手机";
            }

            if (File.Exists(Constant.ConfigDevice3))
            {
               dev3 = new Device(Constant.ConfigDevice3, Constant.ConfigServerPortFilePath3, Constant.deltaPlc);
               dev3.DeviceName = "锁槽机";
            }
          
           if (File.Exists(Constant.ConfigDevice4))
           {
               dev4 = new Device(Constant.ConfigDevice4, Constant.ConfigServerPortFilePath4, Constant.schneiderPlc);
               dev4.DeviceName = "四边锯";
            }
           ***/
            if (dev1!=null)
            DevLst.Add(dev1);
            if (dev2 != null)
                DevLst.Add(dev2);

            if (dev3 != null)
                DevLst.Add(dev3);

            if (dev4 != null)
                 DevLst.Add(dev4);
                deviceMointor.Enabled = true;

           MessageBox.Show("设备连接成功！");


        }
        public Device getDeviceByName(string name)
        {
            foreach (Device  dev in DevLst)
            {
                if (dev.DeviceName.Equals(name))
                {
                    return dev;
                }
            }

            return null;
        }

        private void deviceMointorEvent(object sender, EventArgs e)
        {
            foreach (Device dev in DevLst)
            {
                if (dev.Status < 0)
                {
                    dev.ResetDev();
                }
            }
        }

        public void Dispose()
        {
            getDeviceByName(Constant.devSBJ).Dispose();
        }
        #region 锁槽机
        public void setSCDATACode()
        {

            Device dev = getDeviceByName(Constant.devSC);

            string mc="0"; string mk = "0"; string mh = "0"; string peifang = "0";

            if (dev.DtData.Rows.Count > 0)
            {
                mc = dev.DtData.Rows[0][0].ToString();
                mk = dev.DtData.Rows[0][1].ToString();
                mh = dev.DtData.Rows[0][2].ToString();
                peifang = dev.DtData.Rows[0][3].ToString();
                mc = ConstantMethod.getDataMultipleZero(mc);
                mk = ConstantMethod.getDataMultipleZero(mk);
                mh = ConstantMethod.getDataMultipleZero(mh);
            }
            else return;

            dev.DtData.Rows.RemoveAt(0);

            byte[] mcBytes = ConstantMethod.convertSingleToBytesFromString(mc);
            byte[] mkBytes = ConstantMethod.convertSingleToBytesFromString(mk);
            byte[] mhBytes = ConstantMethod.convertSingleToBytesFromString(mh);
            byte[] pfBytes = ConstantMethod.convertSingleToBytesFromString(peifang);


            dev.
            setValueFormDLst("门长读写", ConstantMethod.convert4BytesTo2Bytes(mcBytes));

            dev.
            setValueFormDLst("门宽读写", ConstantMethod.convert4BytesTo2Bytes(mkBytes));

            dev.
            setValueFormDLst("门厚读写", ConstantMethod.convert4BytesTo2Bytes(mhBytes));

            dev.
            setValueFormDLst("配方读写", ConstantMethod.convert4BytesTo2Bytes(pfBytes));





        }       
        public void checkSCGotoSetData()
        {
            string scCodeDownLoad = getDeviceByName(Constant.devSC).getValueFormDLst("扫码触发读写");
            int value = 0;
            if (int.TryParse(scCodeDownLoad, out value))
            {
                if (value > 0)
                {
                    //CloseCodeScanMode(Constant.devSC);
                    setSCDATACode();
                }
            }
        }


        #endregion
        #region 锁槽拉手机
        public void setSCLSDATACode()
        {

            Device dev = getDeviceByName(Constant.devSCLS);
            string mc="0"; string mk = "0"; string mh = "0"; string peifang1 = "0"; string peifang2 = "0";

            if (dev.DtData.Rows.Count > 0)
            {
                mc = dev.DtData.Rows[0][0].ToString();
                mk = dev.DtData.Rows[0][1].ToString();
                mh = dev.DtData.Rows[0][2].ToString();               
                peifang1 = dev.DtData.Rows[0][3].ToString();
                peifang2 = dev.DtData.Rows[0][4].ToString();

                mc = ConstantMethod.getDataMultipleZero(mc);
                mk = ConstantMethod.getDataMultipleZero(mk);
                mh = ConstantMethod.getDataMultipleZero(mh);
            }
            else return;

            dev.DtData.Rows.RemoveAt(0);

            byte[] mcBytes = ConstantMethod.convertSingleToBytesFromString(mc);
            byte[] mkBytes = ConstantMethod.convertSingleToBytesFromString(mk);
            byte[] mhBytes = ConstantMethod.convertSingleToBytesFromString(mh);
            byte[] pf1Bytes = ConstantMethod.convertSingleToBytesFromString(peifang1);
            byte[] pf2Bytes = ConstantMethod.convertSingleToBytesFromString(peifang2);


           
            dev.
            setValueFormDLst("门长读写", ConstantMethod.convert4BytesTo2Bytes(mcBytes));

            dev.
            setValueFormDLst("门宽读写", ConstantMethod.convert4BytesTo2Bytes(mkBytes));

            dev.
            setValueFormDLst("门厚读写", ConstantMethod.convert4BytesTo2Bytes(mhBytes));

            dev.
            setValueFormDLst("配方1读写", ConstantMethod.convert4BytesTo2Bytes(pf1Bytes));

            dev.
            setValueFormDLst("配方2读写", ConstantMethod.convert4BytesTo2Bytes(pf2Bytes));


           


        }

        public void checkSCLSGotoSetData()
        {
            string sclsCodeDownLoad = getDeviceByName(Constant.devSCLS).getValueFormDLst("扫码触发读写");
            int value = 0;
            if (int.TryParse(sclsCodeDownLoad, out value))
            {
                if (value > 0)
                {
                   // CloseCodeScanMode(Constant.devSCLS);
                    setSCLSDATACode();
                }
            }
        }

        #endregion
        #region 双头合页机
      
        public void setSTHYDATACode()
        {

            Device dev = getDeviceByName(Constant.devSTHY);
            string mc="0"; string mk= "0"; string mh= "0"; string jdf= "0"; string peifang= "0";
           
            if (dev.DtData.Rows.Count > 0)
            {
                mc = dev.DtData.Rows[0][0].ToString();
                mk = dev.DtData.Rows[0][1].ToString();
                mh = dev.DtData.Rows[0][2].ToString();
                jdf = dev.DtData.Rows[0][3].ToString();
                peifang = dev.DtData.Rows[0][4].ToString();

                mc = ConstantMethod.getDataMultipleZero(mc);
                mk = ConstantMethod.getDataMultipleZero(mk);
                mh = ConstantMethod.getDataMultipleZero(mh);

            }
            else return;          

            byte[] mcBytes = ConstantMethod.convertSingleToBytesFromString(mc);
            byte[] mkBytes = ConstantMethod.convertSingleToBytesFromString(mk);
            byte[] mhBytes = ConstantMethod.convertSingleToBytesFromString(mh);
            byte[] jdfBytes = ConstantMethod.convertSingleToBytesFromString(jdf);
            byte[] pfBytes = ConstantMethod.convertSingleToBytesFromString(peifang);          

            dev.
            setValueFormDLst("门长读写", ConstantMethod.convert4BytesTo2Bytes(mcBytes));

            dev.
            setValueFormDLst("门宽读写", ConstantMethod.convert4BytesTo2Bytes(mkBytes));

            dev.
            setValueFormDLst("门厚读写", ConstantMethod.convert4BytesTo2Bytes(mhBytes));

            dev.
            setValueFormDLst("角度阀读写", ConstantMethod.convert4BytesTo2Bytes(jdfBytes));
            dev.
            setValueFormDLst("配方读写", ConstantMethod.convert4BytesTo2Bytes(pfBytes));



            dev.DtData.Rows.RemoveAt(0);


        }

        
        public void checkSTHYGotoSetData()
        {
            string sthyCodeDownLoad = getDeviceByName(Constant.devSTHY).getValueFormDLst("扫码触发读写");
            int value = 0;
            if (int.TryParse(sthyCodeDownLoad, out value))
            {
                if (value > 0)
                {
                 //  CloseCodeScanMode(Constant.devSTHY);
                   setSTHYDATACode();
                }
            }
        }

        #endregion
        #region 四边锯
        public void setSBJData()
        {
            Device dev = getDeviceByName(Constant.devSBJ);
            string ylc = "0"; string ylk = "0"; string jgc = "0"; string jgk = "0";string mh = "0";

            if (dev.DtData.Rows.Count > 0)
            {
                ylc = dev.DtData.Rows[0][0].ToString();
                ylk = dev.DtData.Rows[0][1].ToString();
                jgc = dev.DtData.Rows[0][2].ToString();
                jgk = dev.DtData.Rows[0][3].ToString();
                mh = dev.DtData.Rows[0][4].ToString();


                ylc = ConstantMethod.getDataMultipleZero(ylc, 100);
                ylk = ConstantMethod.getDataMultipleZero(ylk, 100);
                jgc = ConstantMethod.getDataMultipleZero(jgc, 100);
                jgk = ConstantMethod.getDataMultipleZero(jgk, 100);
                mh = ConstantMethod.getDataMultipleZero(mh, 100);
            }
            else return;

                byte[] ylcBytes = BitConverter.GetBytes(int.Parse(ylc)); 
                byte[] ylkBytes = BitConverter.GetBytes(int.Parse(ylk));
                byte[] jgcBytes = BitConverter.GetBytes(int.Parse(jgc));
                byte[] jgkBytes = BitConverter.GetBytes(int.Parse(jgk));
                byte[] mhBytes = BitConverter.GetBytes(int.Parse(mh));

            

            dev.
            setValueFormDLst("原料长度读写", ConstantMethod.convert4BytesTo2Bytes(ylcBytes));

            dev.
            setValueFormDLst("原料宽度读写", ConstantMethod.convert4BytesTo2Bytes(ylkBytes));

            dev.
           setValueFormDLst("加工宽度读写", ConstantMethod.convert4BytesTo2Bytes(jgkBytes));

            dev.
           setValueFormDLst("加工长度读写", ConstantMethod.convert4BytesTo2Bytes(jgcBytes));

           dev.
           setValueFormDLst("门厚读写", ConstantMethod.convert4BytesTo2Bytes(mhBytes));

            dev.DtData.Rows.RemoveAt(0);


        }

        public void checkSBJGotoSetData()
        {
            string sbjCodeDownLoad = getDeviceByName(Constant.devSBJ).getValueFormDLst("扫码触发读写");
            int value = 0;
            if (int.TryParse(sbjCodeDownLoad, out value))
            {
                if (value > 0)
                {
                    setSBJData();
                }
            }
        }

        #endregion

    }


   public  class Device
    {

        int deviceid = 0;
        public int Deviceid
        {
            get { return deviceid; }
            set { deviceid = value; }
        }
        string deviceName;


        public string DeviceName
        {
            get { return deviceName; }
            set
            {

                deviceName = value;
                if (deviceName.Equals(Constant.devSTHY))
                {
                    DtData = ConstantMethod.getDataTableByString(Constant.sthyDataCol);
                    DtData.Rows.Add("2000", "60", "500", "1", "1");
                    DtData.Rows.Add("2020", "60", "500", "1", "2");
                    DtData.Rows.Add("2030", "60", "500", "0", "3");
                    DtData.Rows.Add("2040", "60", "500", "0", "4");
                    DtData.Rows.Add("2000", "60", "500", "1", "1");
                    DtData.Rows.Add("2020", "60", "500", "1", "2");
                    DtData.Rows.Add("2030", "60", "500", "0", "3");
                    DtData.Rows.Add("2040", "60", "500", "0", "4");
                    DtData.Rows.Add("2000", "60", "500", "1", "1");
                    DtData.Rows.Add("2020", "60", "500", "1", "2");
                    DtData.Rows.Add("2030", "60", "500", "0", "3");
                    DtData.Rows.Add("2040", "60", "500", "0", "4");
                    DtData.Rows.Add("2000", "60", "500", "1", "1");
                    DtData.Rows.Add("2020", "60", "500", "1", "2");
                    DtData.Rows.Add("2030", "60", "500", "0", "3");
                    DtData.Rows.Add("2040", "60", "500", "0", "4");
                    DtData.Rows.Add("2000", "60", "500", "1", "1");
                    DtData.Rows.Add("2020", "60", "500", "1", "2");
                    DtData.Rows.Add("2030", "60", "500", "0", "3");
                    DtData.Rows.Add("2040", "60", "500", "0", "4");
                    DtData.Rows.Add("2000", "60", "500", "1", "1");
                    DtData.Rows.Add("2020", "60", "500", "1", "2");
                    DtData.Rows.Add("2030", "60", "500", "0", "3");
                    DtData.Rows.Add("2040", "60", "500", "0", "4");
                    DtData.Rows.Add("2000", "60", "500", "1", "1");
                    DtData.Rows.Add("2020", "60", "500", "1", "2");
                    DtData.Rows.Add("2030", "60", "500", "0", "3");
                    DtData.Rows.Add("2040", "60", "500", "0", "4");
                }

                if (deviceName.Equals(Constant.devSCLS))
                {
                    // DtData = ConstantMethod.getDataTableByString(Constant.sclsDataCol);
                    DtData = ConstantMethod.getDataTableByString(Constant.sclsDataCol);
                    DtData.Rows.Add("2000", "60", "500", "1", "1");
                    DtData.Rows.Add("2020", "60", "500", "2", "2");
                    DtData.Rows.Add("2030", "60", "500", "3", "3");
                    DtData.Rows.Add("2040", "60", "500", "4", "4");
                    DtData.Rows.Add("2000", "60", "500", "1", "1");
                    DtData.Rows.Add("2020", "60", "500", "2", "2");
                    DtData.Rows.Add("2030", "60", "500", "3", "3");
                    DtData.Rows.Add("2040", "60", "500", "4", "4");
                    DtData.Rows.Add("2000", "60", "500", "1", "1");
                    DtData.Rows.Add("2020", "60", "500", "2", "2");
                    DtData.Rows.Add("2030", "60", "500", "3", "3");
                    DtData.Rows.Add("2040", "60", "500", "4", "4");
                    DtData.Rows.Add("2000", "60", "500", "1", "1");
                    DtData.Rows.Add("2020", "60", "500", "2", "2");
                    DtData.Rows.Add("2030", "60", "500", "3", "3");
                    DtData.Rows.Add("2040", "60", "500", "4", "4");

                    DtData.Rows.Add("2000", "60", "500", "1", "1");
                    DtData.Rows.Add("2020", "60", "500", "2", "2");
                    DtData.Rows.Add("2030", "60", "500", "3", "3");
                    DtData.Rows.Add("2040", "60", "500", "4", "4");
                }

                if (deviceName.Equals(Constant.devSC))
                {
                    DtData = ConstantMethod.getDataTableByString(Constant.scDataCol);
                    DtData.Rows.Add("2000", "60", "500", "1");
                    DtData.Rows.Add("2020", "60", "500", "2");
                    DtData.Rows.Add("2030", "60", "500", "3");
                    DtData.Rows.Add("2040", "60", "500", "4");
                    DtData.Rows.Add("2000", "60", "500", "1");
                    DtData.Rows.Add("2020", "60", "500", "2");
                    DtData.Rows.Add("2030", "60", "500", "3");
                    DtData.Rows.Add("2040", "60", "500", "4");
                    DtData.Rows.Add("2000", "60", "500", "1");
                    DtData.Rows.Add("2020", "60", "500", "2");
                    DtData.Rows.Add("2030", "60", "500", "3");
                    DtData.Rows.Add("2040", "60", "500", "4");
                    DtData.Rows.Add("2000", "60", "500", "1");
                    DtData.Rows.Add("2020", "60", "500", "2");
                    DtData.Rows.Add("2030", "60", "500", "3");
                    DtData.Rows.Add("2040", "60", "500", "4");
                    DtData.Rows.Add("2000", "60", "500", "1");
                    DtData.Rows.Add("2020", "60", "500", "2");
                    DtData.Rows.Add("2030", "60", "500", "3");
                    DtData.Rows.Add("2040", "60", "500", "4");
                    DtData.Rows.Add("2000", "60", "500", "1");
                    DtData.Rows.Add("2020", "60", "500", "2");
                    DtData.Rows.Add("2030", "60", "500", "3");
                    DtData.Rows.Add("2040", "60", "500", "4");
                    DtData.Rows.Add("2000", "60", "500", "1");
                    DtData.Rows.Add("2020", "60", "500", "2");
                    DtData.Rows.Add("2030", "60", "500", "3");
                    DtData.Rows.Add("2040", "60", "500", "4");
                    DtData.Rows.Add("2000", "60", "500", "1");
                    DtData.Rows.Add("2020", "60", "500", "2");
                    DtData.Rows.Add("2030", "60", "500", "3");
                    DtData.Rows.Add("2040", "60", "500", "4");
                    DtData.Rows.Add("2000", "60", "500", "1");
                    DtData.Rows.Add("2020", "60", "500", "2");
                    DtData.Rows.Add("2030", "60", "500", "3");
                    DtData.Rows.Add("2040", "60", "500", "4");
                    DtData.Rows.Add("2000", "60", "500", "1");
                    DtData.Rows.Add("2020", "60", "500", "2");
                    DtData.Rows.Add("2030", "60", "500", "3");
                    DtData.Rows.Add("2040", "60", "500", "4");
                }

                if (deviceName.Equals(Constant.devSBJ))
                {
                    DtData = ConstantMethod.getDataTableByString(Constant.sbjDataCol);
                    DtData.Rows.Add("2040", "1000", "2020", "980","45");

                }
            }
        }
        int status=0;

        DataTable dtData;
        public System.Data.DataTable DtData
        {
            get { return dtData; }
            set { dtData = value; }
        }
        public void SetShowDtData(DataGridView dgv)
        {
            if (DtData != null)
            {
                dgv.DataSource = DtData;
            }
        }
        public int Status
        {
            get {
                if (dtTool.Status < 0)
                    return -1;
                else
                {
                    string st = getValueFormDLst("状态读写");
                    if (int.TryParse(st, out status))
                        return status;
                    else return 0;
                }

            }
            set { status = value; }
        }
        int plcId = -1;
        public int PlcId
        {
            get { return plcId; }
            set { plcId = value; }
        }

        public string getValueFormDLst(string str)
        {
            foreach(PlcSimple p in plcLstD)
            {
                if (p.Strname.Equals(str)) return p.ShowValue;
            }

            return null;
        }

        public void setValueFormDLst(string str,ushort[] value)
        {
            if (value == null || value.Count() == 0) return;
            List<PlcSimple> ALLPLST = new List<PlcSimple>();
            ALLPLST.AddRange(plcLstD);
            ALLPLST.AddRange(plcLstM);
            foreach (PlcSimple p in ALLPLST)
            {
                if (p.Strname.Equals(str))
                {
                    if (p.Area.Equals(Constant.D_ID))
                        dtTool.WriteDData(p, value);
                    else
                    {
                        if (p.Area.Equals(Constant.M_ID))
                        {
                            if(value[0]>0)
                              dtTool.SetMData(p, true);
                            else
                              dtTool.SetMData(p, false);
                        }
                    }
                }
            }
        }

        DataTransform dtTool;
        List<PlcSimple> plcLstD;
        List<PlcSimple> plcLstM;

        ServerInfo serverParam ;
        //寄存器保存在表格里
        public Device(string filename,string serverfile,int plcid)
        {
            CsvStreamReader csvFile = new CsvStreamReader();
            DataTable dt = csvFile.OpenCSV(filename);
            plcId = plcid;
            plcLstD = new List<PlcSimple>();
            plcLstM = new List<PlcSimple>();

            if (dt != null && dt.Rows.Count > 0)
                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    PlcSimple plcSimple = new PlcSimple(dt.Rows[i].ItemArray[1].ToString(), dt.Rows[i].ItemArray[0].ToString());
                    plcSimple.SetParam(getStrArr(dt.Rows[i].ItemArray));
                    if(plcSimple.Area.Equals(Constant.D_ID))
                      plcLstD.Add(plcSimple);
                    else
                      plcLstM.Add(plcSimple);
                    plcSimple.SetPlcAddressOffset(plcId);
                }
            else
            {
                MessageBox.Show(Constant.ErrorPlcFile + filename);
                ConstantMethod.AppExit();
            }

            //获取serverip 和 port
            serverParam = ConstantMethod.LoadServerParam(serverfile);
            dtTool      = new DataTransform(serverParam);
            dtTool.SetDPlcSimple(plcLstD);
            dtTool.SetMPlcSimple(plcLstM);
            dtTool.StartGetData();          

        }
        public Device(string filename, ServerInfo serverParam, int plcid)
        {
            CsvStreamReader csvFile = new CsvStreamReader();
            DataTable dt = csvFile.OpenCSV(filename);
            plcId = plcid;
            plcLstD = new List<PlcSimple>();
            plcLstM = new List<PlcSimple>();

            if (dt != null && dt.Rows.Count > 0)
                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    PlcSimple plcSimple = new PlcSimple(dt.Rows[i].ItemArray[1].ToString(), dt.Rows[i].ItemArray[0].ToString());
                    plcSimple.SetParam(getStrArr(dt.Rows[i].ItemArray));
                    if (plcSimple.Area.Equals(Constant.D_ID))
                        plcLstD.Add(plcSimple);
                    else
                        plcLstM.Add(plcSimple);
                    plcSimple.SetPlcAddressOffset(plcId);
                }
            else
            {
                MessageBox.Show(Constant.ErrorPlcFile + filename);
                ConstantMethod.AppExit();
            }

            //获取serverip 和 port
            //serverParam = ConstantMethod.LoadServerParam(serverfile);
            dtTool = new DataTransform(serverParam);
            dtTool.SetDPlcSimple(plcLstD);
            dtTool.SetMPlcSimple(plcLstM);
            dtTool.StartGetData();

        }

        public void ResetDev()
        {   
            dtTool.Reset(serverParam);
        }
        public string[] getStrArr(object[] objArray)
        {

            List<string> strLst = new List<string>();

            for (int i = 0; i < objArray.Length; i++)
            {
                strLst.Add(objArray[i].ToString());
            }

            return strLst.ToArray();

        }


        public void Dispose()
        {
            dtTool.StopGetData();
            
        }

    }
    //只存储D
   public  class PlcSimple
    {

        int addressOffset=0;
        //数据是否正常
        public bool IsValueNormal
        {
            get
            {
                double v=0;
                if (double.TryParse(ShowValue, out v))
                {
                    return ((v > (minValue - 1)) && (v < (maxValue + 1))) ? true : false;
                }
                else
                {
                    return false;
                }

            }

        }
        
        Control showControl;

        string strname;
        public string Strname
        {
            get { return strname; }
            set { strname = value; }
        }
        int address;
        public int Address
        {
            get { return address; }
            set { address = value; }
        }
        int area;
        public int Area
        {
            get { return area; }
            set { area = value; }
        }
        string showValue;
        public string ShowValue
        {
            get { return showValue; }
            set { showValue = value; }
        }
        public PlcSimple()
        {

        }
       
        public PlcSimple(string name0, string  addressAndArea0)
        {
            param = new List<string>();
            strname = name0;
            if (addressAndArea0.Contains(Constant.strDMArea[0]))
            {
                Area = Constant.D_ID;
            }
            else
            {
                Area = Constant.M_ID;
            }
            string addrstr = ConstantMethod.RemoveNotNumber(addressAndArea0);
            if (!int.TryParse(addrstr ,out address))
            {
                MessageBox.Show(Constant.ErrorPlcFile);
                ConstantMethod.AppExit();
            }

        }
        public void SetParam(string[] paraLst)
        {
            param.AddRange(paraLst);
            
            if (!(int.TryParse(paraLst[3], out maxValue)))
            {             

            }
            if (!(int.TryParse(paraLst[4], out minValue)))
            {

            }
            if (!(double.TryParse(paraLst[5], out ration)))
            {

            }

        }
        public void SetPlcAddressOffset(int id )
        {
            if (Area == Constant.D_ID)
            {
                address = address+ Constant.PlcDAddressOffset[id];
            }
            else
            {
                address = address+ Constant.PlcMAddressOffset[id];
            }
        }
        //最小值
        private int minValue = 0;
        public int MinValue
        {
            get { return minValue; }
            set { minValue = value; }
        }
        //最大值
        private int maxValue = 100000000;
        public int MaxValue
        {
            get { return maxValue; }
            set
            {
                maxValue = value;

            }
        }
        //数据一般是要缩小下的
        double ration = 1;
        public double Ration
        {
            get { return ration; }
            set { ration = value; }
        }

        List<string> param;

    }
   public class DataTransform
    {
        TcpClient client;

        Modbus.Device.ModbusIpMaster master;
        int slaveId = 1;
        public int SlaveId
        {
            get { return slaveId; }
            set { slaveId = value; }
        }
        int status =-1;
        public int Status
        {
            get { return status; }
            set { status = value; }
        }
        System.Timers.Timer DataGetTimer;
           
        List<PlcSimple> pDLst;
        List<PlcSimple> pMLst;

        public DataTransform(ServerInfo ser)
        {
            try
            {
                client = new TcpClient(ser.server_Ip, int.Parse(ser.server_Port));
                master = ModbusIpMaster.CreateIp(client);
                Status = 0;
              }
            catch(Exception ex)
            {
                Status = -1;
            }

            DataGetTimer = new System.Timers.Timer(Constant.XJConnectTimeOut);  //这里0.3 秒别改 加到常量里 工控机性能不行 

            DataGetTimer.Enabled = false;

            DataGetTimer.AutoReset = true;

            DataGetTimer.Elapsed += new System.Timers.ElapsedEventHandler(GetDataEvent);


        }

        public void Reset(ServerInfo ser)
        {
            try
            {
                if (client != null && client.Connected)
                {
                    client.Close();
                }


                if (master != null) master.Dispose();

                StopGetData();

                client = new TcpClient(ser.server_Ip, int.Parse(ser.server_Port));
                master = ModbusIpMaster.CreateIp(client);
              
                Status = 0;
            }
            catch(Exception ex)
            {
                Status = -1;
            }

        }
        private void GetDataEvent(object sender, EventArgs e)
        {
            ReadDData();
            ReadMData();
        }

        void ReadDData()
        {
            if(status>=0)
            if (pDLst!=null && pDLst.Count > 0)             
            {
                ushort[] inputs = master.ReadHoldingRegisters((ushort)pDLst[0].Address, (ushort)pDLst.Count);
                if (inputs != null &&inputs.Count() == pDLst.Count)
                {
                    for (int i = 0; i < pDLst.Count; i++)
                    {
                        pDLst[i].ShowValue = inputs[i].ToString();
                    }
                }
            }
        }
        void ReadMData()
        {
            if (status < 1)
            if (pMLst != null && pMLst.Count > 0)
            {
                bool[] inputs = master.ReadCoils((ushort)pMLst[0].Address, (ushort)pDLst.Count);
                if (inputs != null && inputs.Count() == pDLst.Count)
                {
                    for (int i = 0; i < pMLst.Count; i++)
                    {
                        pMLst[i].ShowValue = inputs[i].ToString();
                    }
                }
            }
        }
        public void WriteDData(PlcSimple pc,ushort[] value)
        {
            // List<ushort> valueShort = new List<ushort>();
            //for(int i=0;i<90;i++)
            //valueShort.Add((ushort)value);
            // for (int i = 0; i < 10; i++)
            // {
            //master.WriteSingleRegister((ushort)(pc.Address, ushort value);
             master.WriteMultipleRegisters((byte)SlaveId, (ushort)(pc.Address), value);
           // }

            //master.WriteSingleRegisterAsync((ushort)pc.Address, (ushort)90);
        }
        public void SetMData(PlcSimple pc ,bool value)
        {

            List<bool> valueShort = new List<bool>();
            valueShort.Add((bool)value);
            master.WriteMultipleCoils((byte)SlaveId,(ushort)pc.Address,valueShort.ToArray());

        }
        public void SetDPlcSimple(List<PlcSimple> plcLst)
        {
            pDLst = plcLst;
        }
        public void SetMPlcSimple(List<PlcSimple> plcLst)
        {
            pMLst = plcLst;
        }

        public void StartGetData()
        {
            DataGetTimer.Enabled = true;
        }

        public void StopGetData()
        {
            DataGetTimer.Enabled =false;
        }

        public void Dispose()
        {
            if (client != null && client.Connected)
            {
                client.Close();
            }


            if (master != null) master.Dispose();

            StopGetData();


        }



    }


}
