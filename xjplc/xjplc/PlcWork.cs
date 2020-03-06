using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace xjplc
{
    public class XJPlcWork
    {

        string deviceName = "NA";
        public string DeviceName
        {
            get { return deviceName; }
            set { deviceName = value; }
        }
        EvokXJDevice evokDevice;

        List<List<PlcInfoSimple>> allPlcSimpleLst;
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
        ConfigFileManager paramFile;
        public ConfigFileManager ParamFile
        {
            get { return paramFile; }

            set
            {
                paramFile = value;
            }
        }

        int currentPageId;
        public int CurrentPageId
        {
            get { return currentPageId; }

            set
            {
                currentPageId = value;
            }
        }

        public XJPlcWork(string[] PlcdataFileLst, string configPort)
        {
            Init(PlcdataFileLst, configPort);
        }
        void Init(string[] PlcdataFileLst,string configPort)
        {
            List<string> strfile = new List<string>();
            strfile.AddRange(PlcdataFileLst) ;
            evokDevice = new EvokXJDevice(strfile, ConstantMethod.LoadPortParam(configPort));

            for (int i = 0; i < strfile.Count; i++)
            {
                SetPage(i);
            }

            string[] logs = new string[] { Constant.Start };

            LogManager.WriteProgramLog(logs);

            if (evokDevice.DataFormLst.Count > 0 && AllPlcSimpleLst.Count == evokDevice.DataFormLst.Count)
            {
                for (int i = 0; i < evokDevice.DataFormLst.Count; i++)
                {
                    ConstantMethod.FindPos(evokDevice.DataFormLst[i], AllPlcSimpleLst[i]);
                }
            }

            if (!evokDevice.getDeviceData())
            {
                MessageBox.Show(DeviceName + Constant.ConnectMachineFail);
            }

            ShiftPLCPage(0);

        }
        #region 复用函数
        public bool ShiftPLCPage(int pageid)
        {

            if (CurrentPageId == pageid)
            {
                return true;
            }

            evokDevice.shiftDataForm(pageid);
            FindPlcSimpleInPlcInfoLst(pageid);
            CurrentPageId = pageid;
            return true;
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
       #endregion
    }
}
