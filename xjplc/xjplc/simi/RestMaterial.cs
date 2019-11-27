using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace xjplc.simi
{
     public  class RestMaterial
    {
            
        List<simiMaterial> restMLst; //余料列表
        public System.Collections.Generic.List<xjplc.simi.simiMaterial> RestMLst
        {
            get { return restMLst; }
            set { restMLst = value; }
        }
        DataTable materialDataTable;
        public RestMaterial()
        {
            Init();
        }
        DataGridView dgv;
        public System.Windows.Forms.DataGridView Dgv
        {
            get { return dgv; }
            set { dgv = value; }
        }

        void UpdateMaterialList()
        {
            RestMLst.Clear();
            int countId = 0;
            foreach (DataRow dr in materialDataTable.Rows)
            {
                simiMaterial ss = new simiMaterial(Constant.ConfigSimiRestMaterialFile);
                ss.setMaterial(dr[0].ToString());
                ss.Len = double.Parse(dr[2].ToString());
                RestMLst.Add(ss);

                countId++;
            }
            if(dgv !=null)
            dgv.DataSource = materialDataTable;
            

        }
        void Init()
        {

            materialDataTable = new CsvStreamReader().OpenCSV(Constant.ConfigSimiRestMaterialFile);

            RestMLst = new List<simiMaterial>();
            UpdateMaterialList();



        }
        public void updateDgv()
        {
                       
            UpdateMaterialList();         
        }

        // 提取 物料编号/长度/宽度/厚度/尺寸定位/宽度分割 生成条码
        public string GetMaterialBarCode(simiMaterial ss)
        {
            string s="";
            s = ss.MaterialName        +"/"
                + ss.Len.ToString()    +"/"
                + ss.Width.ToString()  + "/"
                + ss.Height.ToString() + "/"
                ;

            foreach (string param in ss.ParamLst)
            {
                if (!string.IsNullOrWhiteSpace(param))
                {
                    s += (param + "/");
                }
            }

            return s;

        }
        //增加材料选项
        //通过扫码进来的数据
        //解析后 添加到扫码枪中
        public bool AddMaterial(string s)
        {
            //物料编号/长度/宽度/厚度/尺寸定位/宽度分割
            //TFR0009/1000/ 119 /80/1 /50
            DataRow dr = materialDataTable.NewRow();

            string[] ss = s.Split('/');

            if (ss.Count() > 5)
            {
                dr.ItemArray = ss;
                materialDataTable.Rows.Add(ss);
            }
            else return false;


            UpdateMaterialList();


            return true;
        }
        //删除材料选项
        public bool DeleteMaterial(int idRow)
        {
            if (idRow >= 0 && idRow < materialDataTable.Rows.Count)
            {
                materialDataTable.Rows.RemoveAt(idRow);
            }
            else return false;

            UpdateMaterialList();

            return true;
        }
        //删除材料选项
        public bool DeleteMaterial(string name,string size)
        {
            
            foreach (DataRow dr in materialDataTable.Rows)
            {
                if (dr[0].ToString().Equals(name) && size.Equals(dr[2].ToString()))
                {
                    materialDataTable.Rows.Remove(dr);
                    break;
                }
            }

            UpdateMaterialList();

            return true;
        }

        //清除所有材料
        public bool DeleteAllMaterial()
        {

            DialogResult dr = MessageBox.Show("是否继续清空？", "关闭提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information);//触发事件进行提示
            if (dr == DialogResult.No)
            {
                
                return false;
            }
            else
            {
                materialDataTable.Rows.Clear();
            }

            UpdateMaterialList();

            return true;
        }

        public bool SaveMaterial()
        {


            new CsvStreamReader().SaveCSV(materialDataTable,Constant.ConfigSimiRestMaterialFile) ;


            return true;
        }


    }
}
