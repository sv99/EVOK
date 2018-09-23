using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace xjplc
{

    public class sizeFormula
    {
        private string name;

        private string strexe;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public sizeFormula(string str)
        {
            name = str;
        }

        public string Strexe
        {
            get { return strexe; }
            set { strexe = value; }
        }

    }
    //单个尺寸 包含尺寸名 和 相应的公式
    public class singleSize
    {
        private string name;
        private List<sizeFormula> formula;

        private int count;


        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public singleSize(string str)
        {
            name = str;
            count = -1;
            formula = new List<sizeFormula>();
        }

        public List<sizeFormula> Formula
        {
            get { return formula; }
            set { formula = value; }
        }

        public int Count
        {
            get { return count; }
            set { count = value; }
        }

    }
    //一个尺寸集合 每个尺寸由相应的公式计算出来
    public class sizeLstByDoorType
    {
        private string name;

        private List<singleSize> size;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public List<singleSize> Size
        {
            get
            {
                return size;
            }
            set
            {
                size = value;
            }
        }

        public sizeLstByDoorType(string str)
        {
            name = str;
            size = new List<singleSize>();
        }

    }

    /// <summary>
    /// 获取机器可执行的表格
    /// 导入数据表格
    /// </summary>
    public class FileConvertToMachine
    {
        Dictionary<string,int> valCol;
        List<sizeLstByDoorType> UserSplitType = null;

        List<sizeFormula> FormulaType = null;

        ExcelNpoi exop;
        CsvStreamReader csvOp;

        public FileConvertToMachine()
        {
            csvOp = new CsvStreamReader();
            UserSplitType = new List<sizeLstByDoorType>();
            FormulaType = new List<sizeFormula>();
            exop = new ExcelNpoi();
            if (ReadSplitTypeFromCsvData() != 0)
            {
                MessageBox.Show("门型计算数据导入错误！");
            }

            valCol = new Dictionary<string, int>();

            valCol.Add("总序号",0);
            valCol.Add("生产总编号", 1);
            valCol.Add("分厂日期", 3);
            valCol.Add("材质", 4);
            valCol.Add("序号", 5);
            valCol.Add("产品名称", 6);
            valCol.Add("高", 7);
            valCol.Add("宽", 8);
            valCol.Add("厚", 9);
            valCol.Add("门扇条码", 15);
            valCol.Add("线别", 18);
            valCol.Add("查找项", 19);
            valCol.Add("数量", 10);

        }

        public int ReadSplitTypeFromCsvData()
        {

            int offset = 0;
            if (!File.Exists(Constant.SplitTypeFile)) return -1;
            DataTable dtFormula =exop.ImportExcel(Constant.SplitTypeFile); 


            if (dtFormula == null) return -2;
            if (offset > dtFormula.Rows.Count) return -3;

            int i = offset;
            //读取分类之前先清空类保存器
            if (UserSplitType.Count > 0)
                UserSplitType.Clear();

            //依次读取表格行数
            while (i < dtFormula.Rows.Count)
            {
                //从第offset行开始读取 因为第一行是标题
                //第一列门型
                string ProdType = dtFormula.Rows[i][0].ToString();
                sizeLstByDoorType sptype = new sizeLstByDoorType(ProdType);
                //门型为空 则退出
                if (ProdType == "") return 1;
                int tmp = -1;

                string sizename = "";
                string formulaname = "";
                string formulastr = "";

                i++;

                while (!string.Equals(dtFormula.Rows[i - offset][0].ToString(), ProdType))
                {
                    sizename = dtFormula.Rows[i - offset][1].ToString();
                    singleSize singlesize = new singleSize(sizename);


                    //公式 长 宽 
                    formulaname = dtFormula.Rows[i - offset][1].ToString();
                    sizeFormula formula = new sizeFormula(formulaname);

                    formulastr = dtFormula.Rows[i - offset][1].ToString();
                    formula.Strexe = formulastr;
                    singlesize.Formula.Add(formula); 


                    formulastr = dtFormula.Rows[i - offset][2].ToString();
                    if (!string.IsNullOrWhiteSpace(formulastr))
                    {
                        sizeFormula formula0 = new sizeFormula(formulaname);
                        formula0.Strexe = formulastr;
                        singlesize.Formula.Add(formula0);
                    }

                    if (int.TryParse(dtFormula.Rows[i - offset][4].ToString(), out tmp))
                    {
                        singlesize.Count = tmp;                     
                    }
                    else
                    {
                        MessageBox.Show("数量数据错误,第" + (i - offset).ToString() + "行");                       
                    }

                    sptype.Size.Add(singlesize);

                    i++;
                }

                i++;
                if (sptype.Size.Count > 0)
                {
                    UserSplitType.Add(sptype);
                }
                else
                {
                    MessageBox.Show(ProdType + "计算规则添加错误！请检查数据！");
                }

            }
            return 0;
        }

        public bool LoadDoorTypeDataTable(string filename,List<doorTypeInfo> doorLst )
        {
            DataTable doorType = exop.ImportExcel(filename);
            if (doorType.Rows.Count > 0)
            {
                doorLst.Clear();
            }
            else
            {
                return false;
            }
            foreach (DataRow dr in doorType.Rows)
            {
                doorTypeInfo door = new doorTypeInfo(); 
                 
                door.Name = dr[valCol["产品名称"]].ToString() ;

                double height = 0;
                if (!double.TryParse(dr[valCol["高"]].ToString(), out height))
                {
                    MessageBox.Show("门高数据错误！");
                    return false;
                }

                door.Height = (int)height;

                double width = 0;
                if (!double.TryParse(dr[valCol["宽"]].ToString(), out width))
                {
                    MessageBox.Show("门宽数据错误！");
                    return false;
                }
                door.Width = (int)width;

                double thickness = 0;
                if (!double.TryParse(dr[valCol["厚"]].ToString(), out width))
                {
                    MessageBox.Show("门厚数据错误！");
                    return false;
                }
                door.Thickness = (int)thickness;


                int cnt = 0;
                if (!int.TryParse(dr[valCol["数量"]].ToString(), out cnt))
                {
                    MessageBox.Show("门数量数据错误！");
                    return false;
                }
                door.Count = cnt;


                door.BarCodeStr.Add(dr[valCol["数量"]].ToString());

                foreach (string key in valCol.Keys)
                {
                    door.BarCodeStr.Add(dr[valCol[key]].ToString());
                }
                doorLst.Add(door);

                getSizeDataTable(door);

            }
            return true;                                               
        }
        public int GetFormulaResult(string formulaStr, doorTypeInfo door)
        {
            DataTable dt = new DataTable();
            string formstr = formulaStr;
            formstr = formstr.Replace("W", door.Width.ToString());
            formstr = formstr.Replace("H", door.Height.ToString());
            object result = dt.Compute(formstr, "");
            int resultInt=0;
            if (int.TryParse(result.ToString(),out resultInt))
            {
              

            }

            return resultInt;
        }
        public bool getSizeDataTable(doorTypeInfo door)
        {
            string doorType = ConstantMethod.getCharacter(door.Name)+ ConstantMethod.getNumber(door.Name); 

            foreach (sizeLstByDoorType split in UserSplitType)
            {
                //匹配成功 找到公式  找到尺寸
                if (split.Name.Contains(doorType))
                {
                    //添加门皮
                    DataRow dr0 = door.Door_shell.NewRow();
                    dr0[Constant.strformatZh[0]] = door.Height;
                    dr0[Constant.strformatZh[1]] = "2";// GetFormulaResult(size.Formula[0].Strexe, door);
                    dr0[Constant.strformatZh[2]] = "0";
                    dr0[Constant.strformatZh[3]] = door.Name;
                    dr0[Constant.strformatZh[4]] = door.Width;// GetFormulaResult(size.Formula[1].Strexe, door);
                    door.Door_shell.Rows.Add(dr0);


                    foreach (singleSize size in split.Size)
                    {                      

                        //刨花板 桥洞力学板
                        if (size.Formula.Count == 2)
                        {
                            DataRow dr = door.Door_Ban.NewRow();
                            
                            dr[Constant.strformatZh[0]] = GetFormulaResult(size.Formula[0].Strexe, door);
                            dr[Constant.strformatZh[1]] = size.Count;// GetFormulaResult(size.Formula[0].Strexe, door);
                            dr[Constant.strformatZh[2]] = "0";
                            dr[Constant.strformatZh[3]] = size.Name;
                            dr[Constant.strformatZh[4]] = GetFormulaResult(size.Formula[1].Strexe, door);

                            door.Door_Ban.Rows.Add(dr);

                        }
                        else
                        {
                            //添加尺寸 条子
                            if (size.Formula.Count == 1)
                            {
                                DataRow dr = door.Door_Size.NewRow();
                                dr[Constant.strformatZh[0]] = GetFormulaResult(size.Formula[0].Strexe, door);
                                dr[Constant.strformatZh[1]] = size.Count;// GetFormulaResult(size.Formula[0].Strexe, door);
                                dr[Constant.strformatZh[2]] = "0";
                                dr[Constant.strformatZh[3]] = size.Name;
                                dr[Constant.strformatZh[4]] = size.Name;

                                door.Door_Size.Rows.Add(dr);


                            }

                        }                                            
                        
                    }
                }             
            }
            
            return false;
        }   
    }
}
