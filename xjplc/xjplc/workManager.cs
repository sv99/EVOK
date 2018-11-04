using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace xjplc
{
   public class workManager
    {

        FileConvertToMachine fileConvert;

        List<doorTypeInfo> doorLst;

        int shell_height_offset=0;

        int shell_width_offset=0;
        public workManager()
        {
            fileConvert = new FileConvertToMachine();
            doorLst = new List<doorTypeInfo>();
        }
        //获取指定数据的表格集合
        DataTable getUserData(int id)
        {
            DataTable dtUser = new DataTable();

            for (int i = 0; i < Constant.strformatZh.Length; i++)
            {
                dtUser.Columns.Add(Constant.strformatZh[i]);               
            }

            switch (id)
            {
                case Constant.doorSizeId:
                    {
                        foreach (doorTypeInfo dr in doorLst)
                        {
                            foreach (DataRow drr in dr.Door_Size.Rows)
                                dtUser.ImportRow(drr);
                        }
                        break;
                    }
                case Constant.doorBanId:
                    {
                        foreach (doorTypeInfo dr in doorLst)
                        {
                            foreach (DataRow drr in dr.Door_Ban.Rows)
                                dtUser.ImportRow(drr);
                        }
                        break;
                    }
                case Constant.doorShellId:
                    {
                        foreach (doorTypeInfo dr in doorLst)
                        {
                            foreach (DataRow drr in dr.Door_shell.Rows)
                                dtUser.ImportRow(drr);
                        }
                        break;
                    }
                
            }

            return dtUser;


        }
        //将需要的数据 码头 门板 门边 数据显示在表格中
        public void ShowDoor(int id,DataGridView userShow)
        {
            //按照要求倒个序
            if (id == Constant.doorShellId)            
                doorLst.Reverse();
            DataTable dt = getUserData(id);  
            if(dt.Rows.Count>0)
            userShow.DataSource = dt;
            
            // ConstantMethod.ShowInfo(rtbresult, "总共" + doorLst.Count.ToString() + "扇门！");

        }
        public void ShowDoor(int id, DataGridView dgSize, DataGridView dgDoorBan, DataGridView dgDoorShell)
        {
            if (id < doorLst.Count)
            {
                dgSize.DataSource = doorLst[id].Door_Size;
                dgDoorBan.DataSource = doorLst[id].Door_Ban;
                dgDoorShell.DataSource = doorLst[id].Door_shell;
            }      
            // ConstantMethod.ShowInfo(rtbresult, "总共" + doorLst.Count.ToString() + "扇门！");

        }
        public void ShowResult(ListBox listresult)
        {
            int j = 0;
            for (int i = doorLst.Count - 1; i > -1; i--)
            {
                if (doorLst[i].Door_Size.Rows.Count == 0)
                {
                    doorLst.RemoveAt(i);
                }
                else
                {
                    j++;
                    listresult.Items.Add(doorLst[i].Name);
                    //ConstantMethod.ShowInfo(rtbresult, "第" + j.ToString() + "扇门:" + doorLst[i].Name);
                }
            }

           // ConstantMethod.ShowInfo(rtbresult, "总共" + doorLst.Count.ToString() + "扇门！");

        }

        public void ShowResult(RichTextBox rtbresult)
        {
            int j = 0;
            for(int i=doorLst.Count-1;i>-1;i--)            
            {
                if (doorLst[i].Door_Size.Rows.Count == 0)
                {
                    doorLst.RemoveAt(i);
                }
                else
                {
                    j++;
                    ConstantMethod.ShowInfo(rtbresult, "第"+j.ToString()+"扇门:"+doorLst[i].Name);
                }             
            }

            ConstantMethod.ShowInfo(rtbresult, "总共"+ doorLst.Count.ToString()+"扇门！");


        }
        public bool LoadData()
        {
            OpenFileDialog op1 = new OpenFileDialog();
            op1.InitialDirectory = ConstantMethod.GetAppPath();
            op1.Filter = "文件(*.xls,*.xlsx)|*.xls;*.xlsx";
            op1.FileName = "请选择数据文件";

            if (op1.ShowDialog() == DialogResult.OK)
            {
                return fileConvert.LoadDoorTypeDataTable(op1.FileName,doorLst);
            }

            return false;
        }
    }
    public class doorTypeInfo
    {
        string name;

        int height;
        public int Height
        {
            get { return height; }
            set { height = value; }
        }
        int width;
        public int Width
        {
            get { return width; }
            set { width = value; }
        }
        int thickness;
        public int Thickness
        {
            get { return thickness; }
            set { thickness = value; }
        }

        int cntdone;
        public int Cntdone
        {
            get { return cntdone; }
            set { cntdone = value; }
        }
        int count;
        public int Count
        {
            get { return count; }
            set { count = value; }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        List<string> barCodeStr;
        public System.Collections.Generic.List<string> BarCodeStr
        {
            get { return barCodeStr; }
            set { barCodeStr = value; }
        }
        DataTable door_Ban;
        public System.Data.DataTable Door_Ban
        {
            get { return door_Ban; }
            set { door_Ban = value; }
        }
        DataTable door_shell;
        public System.Data.DataTable Door_shell
        {
            get { return door_shell; }
            set { door_shell = value; }
        }
        DataTable door_Size;
        public System.Data.DataTable Door_Size
        {
            get { return door_Size; }
            set { door_Size = value; }
        }
        public doorTypeInfo()
        {
            BarCodeStr = new List<string>();
            cntdone = 0;
            count = 0;
            Door_Ban = new DataTable();
            Door_shell = new DataTable();
            Door_Size = new DataTable();

            for (int i = 0; i < Constant.strformatZh.Length; i++)
            {
                Door_Ban.Columns.Add(Constant.strformatZh[i]);
                Door_shell.Columns.Add(Constant.strformatZh[i]);
                Door_Size.Columns.Add(Constant.strformatZh[i]);
            }              
        }
        
    }
}
