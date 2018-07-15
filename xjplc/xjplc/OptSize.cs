using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace xjplc
{
    //单个尺寸类 含 尺寸 条码 预留1~5
    public class SingleSize
    {
        DataTable dtUser;
        public System.Data.DataTable DtUser
        {
            get { return dtUser; }
            set { dtUser = value; }
        }
        private  string barc;  //条码
        public string Barc
        {
            get { return barc; }
            set { barc = value; }
        }
        private int cut;    //切割长度
        public int Cut
        {
            get { return cut; }
            set { cut = value; }
        }
        private string paramStr1;
        public string ParamStr1
        {
            get { return paramStr1; }
            set { paramStr1 = value; }
        }
        private string paramStr2;
        public string ParamStr2
        {
            get { return paramStr2; }
            set { paramStr2 = value; }
        }
        private string paramStr3;
        public string ParamStr3
        {
            get { return paramStr3; }
            set { paramStr3 = value; }
        }
        private string paramStr4;
        public string ParamStr4
        {
            get { return paramStr4; }
            set { paramStr4 = value; }
        }
        private string paramStr5;
        public string ParamStr5
        {
            get { return paramStr5; }
            set { paramStr5 = value; }
        }
        private string paramStr6;
        public string ParamStr6
        {
            get { return paramStr6; }
            set { paramStr6 = value; }
        }
        private string paramStr7;
        public string ParamStr7
        {
            get { return paramStr7; }
            set { paramStr7 = value; }
        }
        private string paramStr8;
        public string ParamStr8
        {
            get { return paramStr8; }
            set { paramStr8 = value; }
        }
        private string paramStr9;
        public string ParamStr9
        {
            get { return paramStr9; }
            set { paramStr9 = value; }
        }
        private string paramStr10;
        public string ParamStr10
        {
            get { return paramStr10; }
            set { paramStr10 = value; }
        }
        private string paramStr11;
        public string ParamStr11
        {
            get { return paramStr11; }
            set { paramStr11 = value; }
        }
        private string paramStr12;
        public string ParamStr12
        {
            get { return paramStr12; }
            set { paramStr12 = value; }
        }
        private string paramStr13;
        public string ParamStr13
        {
            get { return paramStr13; }
            set { paramStr13 = value; }
        }
        private string paramStr14;
        public string ParamStr14
        {
            get { return paramStr14; }
            set { paramStr14 = value; }
        }
        private string paramStr15;
        public string ParamStr15
        {
            get { return paramStr15; }
            set { paramStr15 = value; }
        }
        private string paramStr16;
        public string ParamStr16
        {
            get { return paramStr16; }
            set { paramStr16 = value; }
        }
        private string paramStr17;
        public string ParamStr17
        {
            get { return paramStr17; }
            set { paramStr17 = value; }
        }
        private string paramStr18;
        public string ParamStr18
        {
            get { return paramStr18; }
            set { paramStr18 = value; }
        }
        private string paramStr19;
        public string ParamStr19
        {
            get { return paramStr19; }
            set { paramStr19 = value; }
        }
        private string paramStr20;
        public string ParamStr20
        {
            get { return paramStr20; }
            set { paramStr20 = value; }
        }

        public int Xuhao; //在dttable里是哪个行号

        private List<string> paramStrLst;
        public List<string> ParamStrLst
        {
            get { return paramStrLst; }
            set {
                paramStrLst = value;
            }
        }
        public SingleSize()
        {
            Barc = "";
            Cut = 0;      //切割长度
            Xuhao = 0;
            ParamStr1 = "";
            ParamStr2 = "";
            ParamStr3 = "";
            ParamStr4 = "";
            ParamStr5 = "";
            ParamStr6 = "";
            ParamStr7 = "";
            ParamStr8 = "";
            ParamStr9 = "";
            ParamStr10 = "";
            ParamStr11 = "";
            ParamStr12 = "";
            ParamStr13 = "";
            ParamStr14 = "";
            ParamStr15 = "";
            ParamStr16 = "";
            ParamStr17 = "";
            ParamStr18 = "";
            ParamStr19 = "";
            ParamStr20 = "";
            paramStrLst = new List<string>();
            paramStrLst.Add(Barc);
            paramStrLst.Add(ParamStr1);
            paramStrLst.Add(ParamStr2);
            paramStrLst.Add(ParamStr3);
            paramStrLst.Add(ParamStr4);
            paramStrLst.Add(ParamStr5);
            paramStrLst.Add(ParamStr6);
            paramStrLst.Add(ParamStr7);
            paramStrLst.Add(ParamStr8);
            paramStrLst.Add(ParamStr9);
            paramStrLst.Add(ParamStr10);
            paramStrLst.Add(ParamStr11);
            paramStrLst.Add(ParamStr12);
            paramStrLst.Add(ParamStr12);
            paramStrLst.Add(ParamStr13);
            paramStrLst.Add(ParamStr14);
            paramStrLst.Add(ParamStr15);
            paramStrLst.Add(ParamStr16);
            paramStrLst.Add(ParamStr17);
            paramStrLst.Add(ParamStr18);
            paramStrLst.Add(ParamStr19);
            paramStrLst.Add(ParamStr20);
        }
        public SingleSize(DataTable dt,int xuhao0)
        {
            Barc = "";
            Cut = 0;      //切割长度
            Xuhao = 0;
            ParamStr1 = "";
            ParamStr2 = "";
            ParamStr3 = "";
            ParamStr4 = "";
            ParamStr5 = "";
            ParamStr6 = "";
            ParamStr7 = "";
            ParamStr8 = "";
            ParamStr9 = "";
            ParamStr10 = "";
            ParamStr11 = "";
            ParamStr12 = "";
            ParamStr13 = "";
            ParamStr14 = "";
            ParamStr15 = "";
            ParamStr16 = "";
            ParamStr17 = "";
            ParamStr18 = "";
            ParamStr19 = "";
            ParamStr20 = "";
            ParamStrLst = new List<string>();
            ParamStrLst.Add(Barc);
            ParamStrLst.Add(ParamStr1);
            ParamStrLst.Add(ParamStr2);
            ParamStrLst.Add(ParamStr3);
            ParamStrLst.Add(ParamStr4);
            ParamStrLst.Add(ParamStr5);
            ParamStrLst.Add(ParamStr6);
            ParamStrLst.Add(ParamStr7);
            ParamStrLst.Add(ParamStr8);
            ParamStrLst.Add(ParamStr9);
            ParamStrLst.Add(ParamStr10);
            ParamStrLst.Add(ParamStr11);
            ParamStrLst.Add(ParamStr12);
            ParamStrLst.Add(ParamStr12);
            ParamStrLst.Add(ParamStr13);
            ParamStrLst.Add(ParamStr14);
            ParamStrLst.Add(ParamStr15);
            ParamStrLst.Add(ParamStr16);
            ParamStrLst.Add(ParamStr17);
            ParamStrLst.Add(ParamStr18);
            ParamStrLst.Add(ParamStr19);
            ParamStrLst.Add(ParamStr20);


            DtUser = dt;
            Xuhao = xuhao0;

        }
    }

    //针对打孔的 在继承 切片带角度的
    public class SingleSizeWithHoleAngle : SingleSize
    {
        public SingleSizeWithHoleAngle(DataTable dt, int xuhao) : base(dt, xuhao)
        {

        }
        public SingleSizeWithHoleAngle() : base()
        {

        }



        int holeCount = 0;
        public int HoleCount
        {
            get
            {

                for (int i = 0; i < hole.Count(); i = i + 3)
                {
                    if (hole[i] > 0)
                        holeCount++;
                }
                return holeCount;
            }

        }
        int[] hole;
        public int[] Hole
        {
            get
            {
                List<string> strparam = new List<string>();
                string nullHoleSre = "0";
                strparam.Add(ParamStr3);
                strparam.Add(ParamStr4);
                strparam.Add(ParamStr5);
                strparam.Add(ParamStr6);
                strparam.Add(ParamStr7);
                strparam.Add(nullHoleSre);
                strparam.Add(nullHoleSre);
                strparam.Add(nullHoleSre);
                strparam.Add(nullHoleSre);
                strparam.Add(nullHoleSre);
                


                strparam.Add(ParamStr8);
                strparam.Add(ParamStr9);
                strparam.Add(ParamStr10);
                strparam.Add(ParamStr11);
                strparam.Add(ParamStr12);
                strparam.Add(nullHoleSre);
                strparam.Add(nullHoleSre);
                strparam.Add(nullHoleSre);
                strparam.Add(nullHoleSre);
                strparam.Add(nullHoleSre);


                hole = getHoleData(strparam.ToArray());


                return hole;
            }
        }
        int[] angle;
        public int[] Angle
        {
            get
            {
                angle = getAngleData(ParamStr2);

                return angle;
            }

        }



        #region 涉及打孔的一些参数函数
        //孔的格式 55-80-60 参数3 ~ 参数10
        private int[] getHoleData(string[] s)
        {
            List<int> data = new List<int>();

            int holeX = -1;
            int holeY = -1;
            int holeZ = -1;

            for (int i = 0; i < s.Count(); i++)
            {
                string[] sArray = Regex.Split(s[i], "-", RegexOptions.IgnoreCase);
                if (sArray.Count() != 3)
                {
                    data.Add(0);
                    data.Add(0);
                    data.Add(0);
                    continue;
                }
                else
                {
                    //孔乘以100
                    if (!int.TryParse(sArray[0], out holeX)) data.Add(0);
                    else data.Add(holeX*100);
                    if (!int.TryParse(sArray[1], out holeY)) data.Add(0);
                    else data.Add(holeY*100);
                    if (!int.TryParse(sArray[2], out holeZ)) data.Add(0);
                    else data.Add(holeZ*100);

                }
            }

            return data.ToArray();

        }
        //角度格式 L/45_R\45 参数2
        private int[] getAngleData(string s)
        {
            List<int> data = new List<int>();
            if (s.Count() != 9)
            {
                goto Last;
            }

            string strAngle =
             Regex.Replace(s, @"[^0-9]+", "");

            if (strAngle.Count() != 4) goto Last; //这里得出的角度暂时没有用起来
            //角度乘以1000
            int intLAngle = getAngleDirectionFromStr(s[1], strAngle.Substring(0, 2))*1000;
            int intRAngle = getAngleDirectionFromStr(s[6], strAngle.Substring(2, 2))*1000;

            if ((intLAngle > 0) && (intRAngle > 0))
            {
                data.Add(intLAngle);
                data.Add(intRAngle);
                return data.ToArray();
            }

            Last:
            {
                data.Add(Constant.Angle90Int);
                data.Add(Constant.Angle90Int);
                return data.ToArray();
            }



        }

        /// <summary>
        /// 通过字符 得出是 几度
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private int getAngleDirectionFromStr(char s, string strangle)
        {
            if (s.Equals(Constant.Angle90)) return 90;
            if (s.Equals(Constant.Angle135)) return (int.Parse(strangle) + 90);
            if (s.Equals(Constant.Angle45)) return int.Parse(strangle);
            return -1;

        }
        #endregion 涉及打孔的一些参数函数

    }
    public  class ProdInfo
    {

        int id;              //方案序号
        public int ID
        {
            set { id = value; }
            get { return id; }
        }
        int len;            //整料长度
        public int Len
        {
            set { len = value; }
            get { return len; }
        }
        int dbc;            //刀补偿
        public int DBC
        {
            set { dbc = value; }
            get
            {
                return dbc;
            }
        }
        int lbc;            //料补偿 
        public int LBC
        {
            set { lbc = value; }
            get { return lbc; }
        }

        int wl;            //尾料长度= 料长-刀补偿*总尺寸个数（含齐头）-料补偿
        public int WL
        {
            get
            {
                if ((len > 0) && (Cut.Count > 0))
                {
                    int jfbc = (Cut.Count + 1) * dbc;
                    int ladd = 0;
                    for (int i = 0; i < Cut.Count; i++)
                    {
                        ladd = ladd + Cut[i];
                    }
                    wl = len - ladd - jfbc - lbc;
                }
                return wl;
            }
        }

        public List<int> Xuhao;    //如果存在Userdata 这个控件 那切割时需要统计已切数量
        public List<string> Barc;  //条码
        public List<int> Cut;      //切割长度

        public List<string> Param1; //参数1
        public List<string> Param2; //参数2
        public List<string> Param3; //参数3
        public List<string> Param4; //参数4
        public List<string> Param5; //参数5
        public List<string> Param6; //参数6
        public List<string> Param7; //参数7
        public List<string> Param8; //参数8
        public List<string> Param9; //参数9
        public List<string> Param10;    //参数10
        public List<string> Param11;    //参数11
        public List<string> Param12;    //参数12
        public List<string> Param13;    //参数13
        public List<string> Param14;    //参数14
        public List<string> Param15;    //参数15
        public List<string> Param16;    //参数16
        public List<string> Param17;    //参数17
        public List<string> Param18;    //参数18
        public List<string> Param19;    //参数19
        public List<string> Param20;	//参数20


        public List<int[]> angle;
        public List<int[]> hole;

        public ProdInfo(List<SingleSize> ssLst)
        {
            Barc = new List<string>();  //条码
            Xuhao = new List<int>();   //在dataform中的位置 方便切割的时候统计
            Cut = new List<int>();      //切割长度
            Param1 = new List<string>();    //参数1
            Param2 = new List<string>();    //参数2
            Param3 = new List<string>();    //参数3
            Param4 = new List<string>();    //参数4
            Param5 = new List<string>();    //参数5
            Param6 = new List<string>();    //参数6
            Param7 = new List<string>();    //参数7
            Param8 = new List<string>();    //参数8
            Param9 = new List<string>();    //参数9
            Param10 = new List<string>();   //参数10
            Param11 = new List<string>();   //参数11
            Param12 = new List<string>();   //参数12
            Param13 = new List<string>();   //参数13
            Param14 = new List<string>();   //参数14
            Param15 = new List<string>();   //参数15
            Param16 = new List<string>();   //参数16
            Param17 = new List<string>();   //参数17
            Param18 = new List<string>();   //参数18
            Param19 = new List<string>();   //参数19
            Param20 = new List<string>();   //参数20

            angle = new List<int[]>();
            hole = new List<int[]>();

            if (ssLst.Count > 0)
            {
                foreach (SingleSize s in ssLst)
                {
                    Cut.Add(s.Cut);
                    Barc.Add(s.Barc);
                    Xuhao.Add(s.Xuhao);
                    Param1.Add(s.ParamStr1);
                    Param2.Add(s.ParamStr2);
                    Param3.Add(s.ParamStr3);
                    Param4.Add(s.ParamStr4);
                    Param5.Add(s.ParamStr5);
                    Param6.Add(s.ParamStr6);
                    Param7.Add(s.ParamStr7);
                    Param8.Add(s.ParamStr8);
                    Param9.Add(s.ParamStr9);
                    Param10.Add(s.ParamStr10);
                    Param11.Add(s.ParamStr11);
                    Param12.Add(s.ParamStr12);
                    Param13.Add(s.ParamStr13);
                    Param14.Add(s.ParamStr14);
                    Param15.Add(s.ParamStr15);
                    Param16.Add(s.ParamStr16);
                    Param17.Add(s.ParamStr17);
                    Param18.Add(s.ParamStr18);
                    Param19.Add(s.ParamStr19);
                    Param20.Add(s.ParamStr20);
                }
            }

        }

        public int delete(int id)
        {
            if (Cut.Count > id)
            {
                Cut.RemoveAt(id);
                Barc.RemoveAt(id);
                Param1.RemoveAt(id);
                Param2.RemoveAt(id);
                Param3.RemoveAt(id);
                Param4.RemoveAt(id);
                Param5.RemoveAt(id);
                Param6.RemoveAt(id);
                Param7.RemoveAt(id);
                Param8.RemoveAt(id);
                Param9.RemoveAt(id);
                Param10.RemoveAt(id);
                Param11.RemoveAt(id);
                Param12.RemoveAt(id);
                Param13.RemoveAt(id);
                Param14.RemoveAt(id);
                Param15.RemoveAt(id);
                Param16.RemoveAt(id);
                Param17.RemoveAt(id);
                Param18.RemoveAt(id);
                Param19.RemoveAt(id);
                Param20.RemoveAt(id);
                Xuhao.RemoveAt(id);

                return 0;
            }
            else
            {
                return -1;
            }
        }

    }
    
    public class OptSize
    {
        CsvStreamReader CSVop;

        ExcelNpoi Excelop;

        DataGridView UserDataView;

        DataTable dtData;

        bool IsLoadData;

        bool IsSaving;
        public DataTable DtData
        {
            get { return dtData; }
            set { dtData = value; }
        }
        int dbc;  //刀补偿
        public int Dbc
        {
            get { return dbc; }
            set { dbc = value; }
        }
        int ltbc;//料头补偿
        public int Ltbc
        {
            get { return ltbc; }
            set { ltbc = value; }
        }
        int len; //料补偿
        public int Len
        {
            get { return len; }
            set { len = value; }
        }

        //要切的尺寸都在里面了
        List<List<SingleSize>> singleSizeLst;
        //上面的类 组合成一个二维表格List 下发
        List<ProdInfo> prodInfoLst;
        public List<ProdInfo> ProdInfoLst
        {
            get { return prodInfoLst; }
            set { prodInfoLst = value; }
        }
        public List<List<SingleSize>> SingleSizeLst
        {
            get { return singleSizeLst; }
            set { singleSizeLst = value; }
        }


        private  
        int optRealLen;//真实可用原料
        public int OptRealLen
        {
            get {
                 optRealLen=Len - dbc - ltbc - safe;

                return optRealLen; }
           
        }
        int safe;  //安全距离
        public int Safe
        {
            get { return safe; }
            set { safe = value; }
        }
        #region 优化

        /// <summary>
        /// 测量结果显示
        /// </summary>
        /// <param name="resultOpt 数据结果"></param>
        /// <param name="prodLst 单行数据的总和 "></param>
        /// <param name="rt1"></param>
        private void ShowMeasureResult(List<int> resultOpt, List<SingleSize> prodLst, RichTextBox rt1)
        {
            List<SingleSize> resultSingleSize = new List<SingleSize>();
            resultOpt.Sort();
            for (int i = 0; i < resultOpt.Count; i++)
            {
                for (int k = 0; k < prodLst.Count; k++)
                {
                    if (prodLst[k].Cut == resultOpt[i])
                    {
                        resultSingleSize.Add(prodLst[k]);
                        ConstantMethod.ShowInfo(rt1, "第" + (i + 1).ToString() + "刀:" + resultOpt[i].ToString() + "---------条码：" + prodLst[k].Barc);
                        prodLst.RemoveAt(k);
                        break;
                    }

                }
            }
            //一根 一根进行汇总
            if (resultSingleSize.Count > 0)
            {
                ProdInfo prodInfo = new ProdInfo(resultSingleSize);
                prodInfo.DBC = dbc;
                prodInfo.LBC = ltbc;
                prodInfo.Len = len;
                ConstantMethod.ShowInfoNoScrollEnd(rt1, "尾料：" + prodInfo.WL.ToString());
                ProdInfoLst.Add(prodInfo);
                singleSizeLst.Add(resultSingleSize);
                ConstantMethod.ShowInfoNoScrollEnd(rt1, "--------------");
                ConstantMethod.ShowInfoNoScrollEnd(rt1, "--------------");
            }
        }
        /// <summary>
        /// 测量结果显示
        /// </summary>
        /// <param name="resultOpt 数据结果"></param>
        /// <param name="prodLst 单行数据的总和 "></param>
        /// <param name="rt1"></param>
        private void ShowNormalResult(List<List<int>> resultOpt, List<SingleSize> prodLst, RichTextBox rt1)
        {
            ConstantMethod.ShowInfo(rt1, "--------------");

            for (int i = 0; i < resultOpt.Count; i++)
            {
                ConstantMethod.ShowInfoNoScrollEnd(rt1, "第" + (i + 1).ToString() + "根：");
                List<SingleSize> resultSingleSize = new List<SingleSize>();
                //排个序
                resultOpt[i].Sort();
                for (int j = 0; j < resultOpt[i].Count; j++)
                {
                    for (int k = 0; k < prodLst.Count; k++)
                    {
                        if (prodLst[k].Cut == resultOpt[i][j])
                        {
                            resultSingleSize.Add(prodLst[k]);
                            ConstantMethod.ShowInfoNoScrollEnd(rt1, "第" + (j + 1).ToString() + "刀:" + resultOpt[i][j].ToString() + "---------条码：" + prodLst[k].Barc);
                            prodLst.RemoveAt(k);
                            break;
                        }
                    }

                }

                if (resultSingleSize.Count > 0)
                {
                    ProdInfo prodInfo = new ProdInfo(resultSingleSize);
                    prodInfo.DBC = dbc;
                    prodInfo.LBC = ltbc;
                    prodInfo.Len = len;
                    ConstantMethod.ShowInfoNoScrollEnd(rt1, "尾料：" + prodInfo.WL.ToString());
                    ProdInfoLst.Add(prodInfo);
                    singleSizeLst.Add(resultSingleSize);
                    ConstantMethod.ShowInfoNoScrollEnd(rt1, "--------------");
                    ConstantMethod.ShowInfoNoScrollEnd(rt1, "--------------");
                }

            }

            ConstantMethod.ShowInfo(rt1, "需要料数：" + resultOpt.Count.ToString() + "根");

        }
        public OptSize(DataGridView UserData0)
        {
            CSVop = new CsvStreamReader();
            SingleSizeLst = new List<List<SingleSize>>();
            ProdInfoLst = new List<ProdInfo>();
            UserDataView = UserData0;
            Excelop = new ExcelNpoi();
         
        }
        #region 优化DLL 引用


        #endregion
        public void SaveExcel()
        {
            if (!IsLoadData
                && !string.IsNullOrWhiteSpace(Excelop.FileName)
                && File.Exists(Excelop.FileName)
                && dtData != null
                && dtData.Rows.Count > 0)
            {
                IsSaving = true;
                Excelop.ExportDataToExcelNoDialog(dtData, Excelop.FileName, null, null);
                IsSaving = false;
            }
        }

        public void SaveCsv()
        {
            if (!IsLoadData
                && !string.IsNullOrWhiteSpace(CSVop.FileName)
                && File.Exists(CSVop.FileName)
                && dtData != null
                && dtData.Rows.Count > 0)
            {
                IsSaving = true;
                CSVop.SaveCSV(dtData, CSVop.FileName);
                IsSaving = false;
            }
        }
        public void ShowErrorRow()
        {
            List<int> errorId = new List<int>();
            //检查datagridview数据是否违法 得出错误列
            errorId = CheckDataGridViewData(OptRealLen, DtData);
            for (int i = 0; i < UserDataView.Rows.Count; i++)
            {
                UserDataView.Rows[i].DefaultCellStyle.BackColor = UserDataView.RowsDefaultCellStyle.ForeColor;
            }

            for (int i = errorId.Count - 1; i >= 0; i--)
            {
                UserDataView.Rows[errorId[i]].DefaultCellStyle.BackColor = Color.Red;
            }
        }
        #region 加载数据

        public bool LoadExcelData(string filename)
        {
            LogManager.WriteProgramLog(Constant.LoadFileSt+filename);
            while (IsSaving)
            {
                Application.DoEvents();
            }
            IsLoadData = true;
            dtData = Excelop.ImportExcel(filename);

            if (dtData.Rows.Count < 2) return false;

            string[] str = ConstantMethod.GetColumnsByDataTable(dtData);

            if (str == null) return false;

            if (! ConstantMethod.compareString(str, Constant.strformatZh))
            {
                return false;
            }
            CSVop.FileName =null;

            Excelop.FileName = filename;

            UserDataView.DataSource = dtData;
            ShowErrorRow();

            IsLoadData = false;

            LogManager.WriteProgramLog(Constant.LoadFileEd);
            return true;
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public bool LoadCsvData(string filename)
        {
            LogManager.WriteProgramLog(Constant.LoadFileSt+filename);
            while (IsSaving)
            {
                Application.DoEvents();
            }

            IsLoadData = true;
            CSVop.DataClear(); 
            if(dtData != null)          
            dtData.Clear();

            CSVop.FileName = filename;
            if (!CSVop.CheckCSVFile(Constant.strformatZh)) return false;
          
            dtData = CSVop.OpenCSV(filename);

            if (dtData.Rows.Count < 2) return false;
            Excelop.FileName = null;
            UserDataView.DataSource= dtData;
            ShowErrorRow();
            IsLoadData = false;
            LogManager.WriteProgramLog(Constant.LoadFileEd);
            return true;
        }
        #endregion
        public void prodClear()
        {
            if (DtData != null && DtData.Rows.Count > 0)
                foreach (DataRow dr in DtData.Rows)
                {
                    dr["已切数量"] = 0;
                }
        }
       
        public string OptMeasure(RichTextBox rt1)
        {

            //干活之前 先清空数据 做好准备工作
            if (rt1 != null) rt1.Clear();
            singleSizeLst.Clear();
            ProdInfoLst.Clear();                     
                       
            if (dtData == null || dtData.Rows.Count < 1) return Constant.prodLstNoData;

            //检查错误行
            ShowErrorRow();
            //获取数据
            List<SingleSize> prodLst = new List<SingleSize>();
            GetDataFromDtByDoorType(DtData, prodLst);

            //如果无数据 则返回-1
            if (prodLst.Count < 1) //
                return Constant.prodLstNoData;


            //进行优化 变成单个模块
            List<int> resultOpt = new List<int>();
            List<int> dataOpt = new List<int>();

            //进行完整的优化
            if (prodLst.Count > 0)
                foreach (SingleSize sss in prodLst)
                {
                    dataOpt.Add(sss.Cut);
                }

            //----返回的结果 只有一组数据
            resultOpt = OptModuleMeasure(dataOpt.ToList<int>(), len, dbc, ltbc, safe);
                                                        
            if (resultOpt.Count > 0)
            {
                 ShowMeasureResult(resultOpt, prodLst, rt1);
            }
            else return Constant.optResultNoData;


            resultOpt = null;
            prodLst = null;
            dataOpt = null;
            //GC.Collect();
            //GC.WaitForPendingFinalizers();     
            return Constant.optSuccess;
        }
        public string OptNormal(RichTextBox rt1)
        {
            //干活之前 先清空数据 做好准备工作          
            singleSizeLst.Clear();
            ProdInfoLst.Clear();          
           
            if (dtData == null || dtData.Rows.Count < 1) return Constant.prodLstNoData;

            //检查错误行
            ShowErrorRow();
            List<SingleSize> prodLst = new List<SingleSize>();
            GetDataFromDtByDoorType(DtData,prodLst);

            //如果无数据 则返回-1
            if (prodLst.Count < 1) //
                return Constant.prodLstNoData;


            //进行优化 变成单个模块
            List<List<int>> resultOpt = new List<List<int>>();
            List<int> dataOpt = new List<int>();


            //进行完整的优化
            if (prodLst.Count > 0)
            foreach (SingleSize sss in prodLst)
            {
               dataOpt.Add(sss.Cut);
            }

            //----
            resultOpt = OptModuleNormal(dataOpt.ToList<int>(),len, dbc, ltbc, safe);

                    
            if (resultOpt.Count > 0 )
            {
                ShowNormalResult(resultOpt, prodLst, rt1);              
            }
            else return Constant.optResultNoData; 
                
            resultOpt = null;
            prodLst = null;
            dataOpt = null;
            //GC.Collect();
            //GC.WaitForPendingFinalizers();     
            return Constant.optSuccess;

        }        
    
        private void GetSmallBigData(int c, List<int> data, List<int> dataBig, List<int> dataSmall, double index)
        {
            int len = (int)(c * index);

            if (len > -1)
            {
                foreach (int dataSize in data)
                {
                    if (dataSize > len && dataSize < c)
                    {
                        dataBig.Add(dataSize);
                    }
                    else
                    {
                        if (dataSize > 0 && dataSize < c) dataSmall.Add(dataSize);
                    }
                }
            }

        }

        /// <summary>
        /// //自动测长情况下 只优化一根出来就可以了 
        //选一个最靠近料长的 尺寸
        //剩下的拿来优化
        /// </summary>
        /// <param name="data"></param>
        /// <param name="c"></param>
        /// <param name="dbc_tmp"></param>
        /// <param name="ltbc_tmp"></param>
        /// <param name="safe_tmp"></param>
        /// <returns></returns>
        private List<int> OptModuleMeasure(List<int> data, int c, int dbc_tmp, int ltbc_tmp, int safe_tmp)
        {

            List<int> dataTmp = data.ToList<int>();

            List<int> dataResult = new List<int>();

            //设置一个参数 如果数据在上面都没有 也就是大家都是小类 尺寸  也没有必要再进行优化了 都是同一类尺寸
            double indexBigDouble = (double)dataTmp.ToArray().Max() / c + 0.1;

            List<int> dataBig = new List<int>();
            List<int> dataSmall = new List<int>();
            List<int> dataRes = new List<int>();
            int[] dataResL;
            //找到最靠近料长的尺寸
            dataBig.Add(dataTmp.ToArray().Max());
            if(dataBig.Count>0)
            dataTmp.Remove(dataBig[0]);
            //剩下全是短料
            dataSmall = dataTmp.ToList();
            //只进行一次优化
            dataResL = OptModule0(dataSmall, c - dataBig[0] - dbc_tmp, dbc_tmp, ltbc_tmp, safe_tmp);
            //统计结果
            dataResult.AddRange(dataResL);
            //把长料增加进去
            if (dataBig.Count > 0)
                dataResult.Add(dataBig[0]); 
                      
            return dataResult.ToList();

        }
        /// <summary>
        /// 优化主程序
        /// 先进行大小分类，再进行优化排版 
        /// 分类
        ///  排版
        /// 统计根数
        //选择最优根数和其他参数
        /// </summary>
        /// <param name="data"></param>
        /// <param name="selectedData"></param>
        /// <param name="c"></param>
        /// <param name="dbc_tmp"></param>
        /// <param name="ltbc_tmp"></param>
        /// <param name="safe_tmp"></param>
        private List<List<int>> OptModuleNormal(List<int> data, int c, int dbc_tmp, int ltbc_tmp, int safe_tmp)
        {        
            double index = 0;//取值是0~1 0.1 0.2 0.3 0.4 0.5 0.6 0.7 0.8 0.9 
                                
            List<int> dataTmp = data.ToList<int>();

            List<List<int>> dataResult = new List<List<int>>();

            //设置一个参数 如果数据在上面都没有 也就是大家都是小类 尺寸  也没有必要再进行优化了 都是同一类尺寸
            double indexBigDouble = (double)dataTmp.ToArray().Max() / c + 0.1;
            
            //优化9次
            for (int i = 1; i <10; i++)
            {
                              
                index = 0.1 * i;
                if (index > indexBigDouble) break;
                dataTmp = data.ToList<int>();
                List<List<int>> dataResultM = new List<List<int>>();

                //每次优化把数据优化完
                while (dataTmp.Count > 0)
                {
                    Application.DoEvents();
                    List<int> dataBig = new List<int>();
                    List<int> dataSmall = new List<int>();
                    List<int> dataRes = new List<int>();
                    int[] dataResL;
                    //区分长短料
                    GetSmallBigData(c, dataTmp, dataBig, dataSmall, index);
                    //分三种情况 1.长料 短料 都有 2.长料有 短料无 3.短料有 长料无
                    if (dataBig.Count > 0 && dataSmall.Count > 0)
                    {
                        //短料优化
                        dataResL=OptModule0(dataSmall, c - dataBig[0] - dbc_tmp, dbc_tmp, ltbc_tmp, safe_tmp);
                        dataRes.AddRange(dataResL);
                        dataRes.Add(dataBig[0]);
                    }
                    else
                    {
                        //长料有 短料无
                        if (dataBig.Count > 0)
                        {
                            dataResL=OptModule0(dataBig, c, dbc_tmp, ltbc_tmp, safe_tmp);
                            dataRes.AddRange(dataResL);         
                        }
                        else
                        {
                            //短料有 长料无
                            if (dataSmall.Count > 0)
                            {
                                dataResL = OptModule0(dataSmall, c, dbc_tmp, ltbc_tmp, safe_tmp);
                                dataRes.AddRange(dataResL);
                            }
                        }
                    }
                    //每一根的优化结果 然后在 datatmp中删除已经选中的数据
                    if (dataRes.Count > 0)
                    {
                        dataResultM.Add(dataRes.ToList<int>());
                        //删除已经选中的数据
                        for (int j = 0; j < dataRes.Count; j++)
                        {
                            int FindSize = dataTmp.IndexOf(dataRes[j]);
                            if (FindSize > -1)
                                dataTmp.RemoveAt(FindSize);
                        }
                    }
                    //
                    dataBig = null;
                    dataSmall = null;
                    dataRes = null;
                    //GC.Collect();
                    //GC.WaitForPendingFinalizers();
                }
               
                dataTmp = null;
                if (dataResultM.Count > 0)
                {
                    if (dataResult.Count > 0)
                    {
                        if (dataResult.Count > dataResultM.Count)
                        {
                            dataResult = dataResultM.ToList();
                        }
                    }
                    else
                    {
                        dataResult = dataResultM.ToList();
                    }                                       
                }

                dataResultM = null;

            }
            //GC.Collect();
            //GC.WaitForPendingFinalizers();
            return dataResult.ToList();          
          
        }

        [DllImport("opt3.dll", EntryPoint = "Optimize", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Optimize(int n, int c, int[] w, int[] v, int[] x);


        //单个模块进行排版
        private int[] OptModule0(List<int> data,  int c, int dbc_tmp, int ltbc_tmp, int safe_tmp)
        {
            List<int> selectedData = new List<int>();
            data.Insert(0, 0);
            int n = data.Count();
            int[] w = data.ToArray();// data.ToArray();//new int[15]{0,34000,20000,7000,15000,12000,2000,25000,25000,10000,18000,16000,12000,15000,14000};
            int[] x = new int[n];
            int[] v = new int[n];
            double valuedouble;
            n = data.Count();//有效数据个数

            valuedouble = (c - dbc_tmp - ltbc_tmp - safe_tmp) / 100;
            c = (int)(Math.Floor(valuedouble));

            //参与运算的尺寸减去刀补偿
            for (int i = 1; i < n; i++)
            {
                w[i] = w[i] + dbc_tmp;
                valuedouble = ((double)w[i]) / 100;
                w[i] = (int)Math.Ceiling(valuedouble); ;
            }
            for (int i = 0; i < n; i++)
            {
                v[i] = w[i];
            }
            try
            {
               // ConstantMethod.
               Optimize(n, c,w, v, x);//UserData.RowCount;
            }
            catch (Exception)
            {
              //  Optimize(n, w, v, x, c);
            }
            finally
            {
                
            }
           
            for (int i = 1; i < x.Count(); i++)
            {
                int k = i;

                if (x[i] == 1)
                {
                    selectedData.Add(data[k]);
                }

            }

            w = null;
            x = null;
            v = null;

            data.RemoveAt(0);
            //GC.Collect();
            //GC.WaitForPendingFinalizers();

            return selectedData.ToArray();                        
                       
        }
        
        #endregion
        //主要针对第一列数据进行检查TaT
        /// <summary>
        /// 第一列尺寸 第二列设定数量 第三列 已切数量
        /// </summary>
        /// <param name="reallen"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        private List<int> CheckDataGridViewData(int reallen,DataTable dt)
        {
            List<int> errRow = new List<int>();
            double dblevaluesize; //尺寸
            int intcounttocut; //要切的数量
            int intcountdone;//已切数量

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int m = i;
                                        
                if (dt.Rows[i][Constant.strformatZh[0]] == null)
                {
                    errRow.Add(m);
                    continue;
                }
                if (dt.Rows[i][Constant.strformatZh[1]] == null)
                {
                    errRow.Add(m);
                    continue;
                }
                if (dt.Rows[i][Constant.strformatZh[2]] == null)
                {
                    errRow.Add(m);
                    continue;
                }
                if (!Double.TryParse(dt.Rows[i][Constant.strformatZh[0]].ToString(), out dblevaluesize))
                {
                    errRow.Add(m);
                    continue;
                }
                if (!int.TryParse(dt.Rows[i][Constant.strformatZh[1]].ToString(), out intcounttocut))
                {
                    errRow.Add(m);
                    continue;
                }
                //已切数量没有 默认为0
                if (!int.TryParse(dt.Rows[i][Constant.strformatZh[2]].ToString(), out intcountdone))
                {
                    dt.Rows[i][Constant.strformatZh[2]] = 0;
                    intcountdone = 0;
                    // errRow.Add(m);
                }

                if (intcounttocut < intcountdone)
                {
                    errRow.Add(m);
                    continue;
                }

                if ((dblevaluesize > 0) && (intcounttocut > -1) && (intcountdone > -1))
                {

                    int size = (int)(dblevaluesize * 100);
                    if (size > reallen)
                    {
                        errRow.Add(m);
                    }
                }
                else
                { errRow.Add(m); continue; }
            }

            return errRow;
        }

        /// <summary>
        /// 在给定的dt里寻找数据
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="ProdLst"></param>
        private void GetDataFromDtByDoorType(DataTable dt, List<SingleSize> ProdLst)
        {
            double dblevaluesize; //尺寸
            int intcounttocut; //要切的数量
            int intcountdone;//已切数量

            ProdLst.Clear();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                //判断 尺寸 设定数量 已切数量
                if (!Double.TryParse(dt.Rows[i][Constant.strformatZh[0]].ToString(), out dblevaluesize)) continue;
                if (!int.TryParse(dt.Rows[i][Constant.strformatZh[1]].ToString(), out intcounttocut)) continue;
                if (!int.TryParse(dt.Rows[i][Constant.strformatZh[2]].ToString(), out intcountdone)) continue;

                if (intcounttocut <= intcountdone) continue;

                //尺寸需要扩大一下 有精度要求 小数点后面两位            
                int size = (int)(dblevaluesize * 100);

                if (!(size < OptRealLen)) continue;

                if ((dblevaluesize > 0) && (intcounttocut > 0) && (intcountdone > -1))
                {
                    for (int j = 0; j < (intcounttocut - intcountdone); j++)
                    {                    
                        SingleSize pi = new SingleSize(dt,i);
                        ProdLst.Add(pi);
                        
                        //设定尺寸 条码
                        pi.Cut = size;                       
                        pi.Barc = dt.Rows[i][Constant.strformatZh[3]].ToString();
                        pi.DtUser = dt;
                        //添加参数                      
                        for (int m = 0; m < pi.ParamStrLst.Count; m++)
                        {
                            if ((m+4)<dt.Rows[i].ItemArray.Count() && dt.Rows[i][4 + m] != null)  //参数
                                pi.ParamStrLst[m] = dt.Rows[i][4 + m].ToString();
                        }

                        if (pi.ParamStrLst.Count > 20)
                        {
                            pi.ParamStr1 = pi.ParamStrLst[0];
                            pi.ParamStr2 = pi.ParamStrLst[1];
                            pi.ParamStr3 = pi.ParamStrLst[2];
                            pi.ParamStr4 = pi.ParamStrLst[3];
                            pi.ParamStr5 = pi.ParamStrLst[4];
                            pi.ParamStr6 = pi.ParamStrLst[5];
                            pi.ParamStr7 = pi.ParamStrLst[6];
                            pi.ParamStr8 = pi.ParamStrLst[7];
                            pi.ParamStr9 = pi.ParamStrLst[8];
                            pi.ParamStr10 = pi.ParamStrLst[9];
                            pi.ParamStr11 = pi.ParamStrLst[10];
                            pi.ParamStr12 = pi.ParamStrLst[11];
                            pi.ParamStr13 = pi.ParamStrLst[12];
                            pi.ParamStr14 = pi.ParamStrLst[13];
                            pi.ParamStr15 = pi.ParamStrLst[14];
                            pi.ParamStr16 = pi.ParamStrLst[15];
                            pi.ParamStr17 = pi.ParamStrLst[16];
                            pi.ParamStr18 = pi.ParamStrLst[17];
                            pi.ParamStr19 = pi.ParamStrLst[18];
                            pi.ParamStr20 = pi.ParamStrLst[19];


                        }
                        pi.ParamStrLst.Insert(0,pi.Barc);

                    }
                }

            }      
    }
               
        
    }
}
