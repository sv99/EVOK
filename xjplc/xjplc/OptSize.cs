using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
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
        public string Barc;  //条码
        public int Cut;    //切割长度
        public string ParamStr1;
        public string ParamStr2;
        public string ParamStr3;
        public string ParamStr4;
        public string ParamStr5;
        public string ParamStr6;
        public string ParamStr7;
        public string ParamStr8;
        public string ParamStr9;
        public string ParamStr10;
        public string ParamStr11;
        public string ParamStr12;
        public string ParamStr13;
        public string ParamStr14;
        public string ParamStr15;
        public string ParamStr16;
        public string ParamStr17;
        public string ParamStr18;
        public string ParamStr19;
        public string ParamStr20;

        public int Xuhao; //在dttable里是哪个行号

        public List<string> ParamStrLst;
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

            DtUser = dt;
            Xuhao = xuhao0;

        }
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

            return true;
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public bool LoadCsvData(string filename)
        {

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
            resultOpt = OptModuleMeasure(dataOpt.ToList<int>(), OptRealLen, dbc, ltbc, safe);
                                                        
            if (resultOpt.Count > 0)
            {
                List<SingleSize> resultSingleSize = new List<SingleSize>();

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

                if (resultSingleSize.Count > 0)
                {
                    ProdInfo prodInfo = new ProdInfo(resultSingleSize);
                    prodInfo.DBC = dbc;
                    prodInfo.LBC = ltbc;
                    prodInfo.Len = len;
                    ConstantMethod.ShowInfo(rt1, "尾料：" + prodInfo.WL.ToString());
                    ProdInfoLst.Add(prodInfo);
                    singleSizeLst.Add(resultSingleSize);
                    ConstantMethod.ShowInfo(rt1, "--------------");
                    ConstantMethod.ShowInfo(rt1, "--------------");
                }          
        
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
            if (rt1 != null) rt1.Clear();
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
            resultOpt = OptModuleNormal(dataOpt.ToList<int>(), OptRealLen, dbc, ltbc, safe);

                    
            if (resultOpt.Count > 0 )
            {
                ConstantMethod.ShowInfo(rt1, "需要料数："+resultOpt.Count.ToString()+ "根");
                ConstantMethod.ShowInfo(rt1, "--------------");
                for (int i = 0; i < resultOpt.Count; i++)
                {
                    ConstantMethod.ShowInfo(rt1, "第" + (i + 1).ToString() + "根：");
                    List<SingleSize> resultSingleSize = new List<SingleSize>();
                                        
                    for (int j = 0; j < resultOpt[i].Count; j++)
                    {                       
                        for (int k = 0; k < prodLst.Count; k++)
                        {
                            if (prodLst[k].Cut == resultOpt[i][j])
                            {
                                resultSingleSize.Add(prodLst[k]);
                                ConstantMethod.ShowInfo(rt1, "第" + (j+1).ToString() + "刀:" + resultOpt[i][j].ToString() + "---------条码：" + prodLst[k].Barc);
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
                        ConstantMethod.ShowInfo(rt1, "尾料：" + prodInfo.WL.ToString());
                        ProdInfoLst.Add(prodInfo);
                        singleSizeLst.Add(resultSingleSize);
                        ConstantMethod.ShowInfo(rt1, "--------------");
                        ConstantMethod.ShowInfo(rt1, "--------------");
                    }
                }
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
                            if (dt.Rows[i][4 + m] != null)  //参数
                                pi.ParamStrLst[m] = dt.Rows[i][4 + m].ToString();
                        }
                        

                    }
                }

            }      
    }
               
        
    }
}
