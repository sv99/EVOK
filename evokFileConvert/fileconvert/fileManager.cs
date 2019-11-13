using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using xjplc;

namespace fileconvert
{
    //设立此类的目的 为 用户的数据格式多样，所以需要进行格式转换
    //同时用户希望进行数据分割
    public static class fileManager
    {
        private static void SaveFile(DataTable dt, string filename)
        {
            CsvStreamReader csvop = new CsvStreamReader();
            csvop.SaveCSV(dt, filename);
        }
        //带多个参数文件名
        public static void SaveFileWithDefault(DataTable dt, string PathStr,string param1)
        {
            string dir = Path.GetDirectoryName(PathStr);
            string filename = Path.GetFileNameWithoutExtension(PathStr)+"_"+param1 + "_";
            string f0 = dir + "\\" + filename + "Machine.csv";
            SaveFile(dt, f0);

        }
        //根据转换公式 转换表格 最后一个参数代表是用默认规则保存文件还是用带参数名的
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rtbResult"></param> 显示结果
        /// <param name="PathStr"></param>  
        /// <param name="pBar1"></param>
        /// <param name="UserDt"></param>  数据源
        /// <param name="valueCol"></param> 数据转换规则
        /// <param name="isNameUseDefault"></param> 是否使用默认命名规则
        /// <param name="paramLst"></param>  如果不是使用默认命名规则，提供命名规则 使用表格中哪一列作为文件名
        /// <returns></returns>
        public static DataTable  saveDataTableToFile(RichTextBox rtbResult,string PathStr,ProgressBar pBar1,DataTable UserDtTmp, List<int> valueCol,bool isNameUseDefault,List<string> paramLst)
        {
         
            DataTable dtOutPutTmp=new DataTable("file");
            rtbResult.Clear();
            pBar1.Value = 0;
            pBar1.Minimum = 0;
            for (int i = 0; i < valueCol.Count(); i++)
            {
                if (valueCol[i] >= UserDtTmp.Columns.Count) return null;
            }
            //收集数据 保存
            if (valueCol.Count > 3 )
            {
                pBar1.Maximum = UserDtTmp.Rows.Count;

                DataColumn dtcolSize = new DataColumn("尺寸");

                DataColumn dtcolCnt = new DataColumn("设定数量");

                DataColumn dtcolCntDone = new DataColumn("已切数量");

                DataColumn dtcolBarCode = new DataColumn("条码");

                dtOutPutTmp.Columns.Add(dtcolSize);
                dtOutPutTmp.Columns.Add(dtcolCnt);
                dtOutPutTmp.Columns.Add(dtcolCntDone);
                dtOutPutTmp.Columns.Add(dtcolBarCode);
                ConstantMethod.ShowInfo(rtbResult, UserDtTmp.Columns[valueCol[0]].ColumnName + "=====>" + dtOutPutTmp.Columns[0].ColumnName);
                ConstantMethod.ShowInfo(rtbResult, UserDtTmp.Columns[valueCol[1]].ColumnName + "=====>" + dtOutPutTmp.Columns[1].ColumnName);
                ConstantMethod.ShowInfo(rtbResult, UserDtTmp.Columns[valueCol[2]].ColumnName + "=====>" + dtOutPutTmp.Columns[3].ColumnName);

                //增加列  ConstantMethod.ShowInfo(rtbResult,"开始转换，转换规则如下");
                for (int i = 0; i < (valueCol.Count - 3); i++)
                {
                    DataColumn dtcolParm = new DataColumn("参数" + (i + 1).ToString());
                    dtOutPutTmp.Columns.Add(dtcolParm);
                    ConstantMethod.ShowInfo(rtbResult, UserDtTmp.Columns[valueCol[i + 3]].ColumnName + "=====>" + dtOutPutTmp.Columns[i + 4].ColumnName);
                }

                //增加行
                foreach (DataRow row in UserDtTmp.Rows)
                {
                    DataRow dr2 = dtOutPutTmp.NewRow();

                    pBar1.Value = pBar1.Value + 1;

                    for (int i = 0; i < dr2.ItemArray.Length; i++)
                    {
                        if (i == 2)
                        {
                            dr2[i] = "0";
                        }
                        else
                        {
                            if (i < 2)
                            {
                                dr2[i] = row[valueCol[i]];
                            }
                            else
                            {
                                dr2[i] = row[valueCol[i - 1]];
                            }
                        }

                    }

                    dtOutPutTmp.Rows.Add(dr2);
                }
                if (isNameUseDefault)
                {
                    string dir = Path.GetDirectoryName(PathStr); 
                    string filename = Path.GetFileNameWithoutExtension(PathStr);
                    string f0= dir + "\\" + filename + "Machine.csv";
                    SaveFile(dtOutPutTmp, f0);
                }
                else
                {
                    //用带参数的名称去保存
                    if (paramLst != null && dtOutPutTmp.Rows.Count>0)
                    {

                        string filename = "";
                        for (int i = 0; i < paramLst.Count; i++)
                        {
                            filename = filename+
                               UserDtTmp.Rows[0][paramLst[i]].ToString() + "_";
                        }
                        //先建立文件夹
                        string dirCreateName = Path.GetFileNameWithoutExtension(PathStr);
                        
                        //
                        string dir = Path.GetDirectoryName(PathStr);

                        Directory.CreateDirectory(dir+"\\"+dirCreateName);

                        filename = filename + dtOutPutTmp.Rows.Count.ToString();

                        string f0 = dir + "\\" + dirCreateName+"\\"+filename + "_Machine.csv";
                        SaveFile(dtOutPutTmp,f0);
                    }
                }
                return dtOutPutTmp;
            }
            else
            {
                MessageBox.Show("规则错误，数据源列数<规则的列数");
            }


            return null;
            

        }
    }
}
