using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace xjplc
{
   public class workManager
    {

        FileConvertToMachine fileConvert;

        List<doorTypeInfo> doorLst;
        public System.Collections.Generic.List<xjplc.doorTypeInfo> DoorLst
        {
            get { return doorLst; }
            set { doorLst = value; }
        }
        OpenFileDialog op1 = new OpenFileDialog();
     
        public workManager()
        {
            fileConvert = new FileConvertToMachine();
            DoorLst = new List<doorTypeInfo>();

            op1.InitialDirectory = ConstantMethod.GetAppPath();
            op1.Filter = "文件(*.xls,*.xlsx)|*.xls;*.xlsx";
            op1.FileName = "请选择数据文件";

        }
        //获取指定数据的表格集合
       public  DataTable getUserData(int id)
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
                        foreach (doorTypeInfo dr in DoorLst)
                        {
                            foreach (DataRow drr in dr.Door_Size.Rows)
                                dtUser.ImportRow(drr);
                        }
                        break;
                    }
                case Constant.doorBanId:
                    {
                        foreach (doorTypeInfo dr in DoorLst)
                        {
                            foreach (DataRow drr in dr.Door_Ban.Rows)
                                dtUser.ImportRow(drr);
                        }
                        break;
                    }
                case Constant.doorShellId:
                    {
                        foreach (doorTypeInfo dr in DoorLst)
                        {
                            foreach (DataRow drr in dr.Door_shell.Rows)
                                dtUser.ImportRow(drr);
                        }
                        break;
                    }
                
            }

            return dtUser;


        }
        public void doorLstReverse()
        {
            DoorLst.Reverse();
        }
        //将需要的数据 码头 门板 门边 数据显示在表格中
        public void ShowDoor(int id,DataGridView userShow)
        {
            //按照要求倒个序
            //if (id == Constant.doorShellId)            
            //doorLst.Reverse();
            DataTable dt = getUserData(id);  
            if(dt.Rows.Count>0)
            userShow.DataSource = dt;
                       
            // ConstantMethod.ShowInfo(rtbresult, "总共" + doorLst.Count.ToString() + "扇门！");

        }
        //选择多行了之后 显示数据20181110
        public void ShowDoor(string doorName, DataGridView dgSize, DataGridView dgDoorBan, DataGridView dgDoorShell)
        {
            if (string.IsNullOrWhiteSpace(doorName)) return;
            if (DoorLst.Count < 1) return;
            if (dgSize == null  || dgDoorShell ==null || dgDoorBan==null) return;
            DataTable dtDoorSize = DoorLst[0].Door_Size.Clone();
            DataTable dtDoorBan = DoorLst[0].Door_Ban.Clone();
            DataTable dtDoorShell = DoorLst[0].Door_shell.Clone();
            foreach (doorTypeInfo dTI in DoorLst)
            {
                if (dTI.Name.Equals(doorName))
                {
                    foreach (DataRow drSize in dTI.Door_Size.Rows)
                    {
                        DataRow dtNew = dtDoorSize.NewRow();
                        dtNew.ItemArray = drSize.ItemArray.ToArray();
                        dtDoorSize.Rows.Add(dtNew);
                    }
                    foreach (DataRow drShell in dTI.Door_shell.Rows)
                    {
                        DataRow dtNew = dtDoorShell.NewRow();
                        dtNew.ItemArray = drShell.ItemArray.ToArray();
                        dtDoorShell.Rows.Add(dtNew);
                    }
                    foreach (DataRow drBan in dTI.Door_Ban.Rows)
                    {
                        DataRow dtNew = dtDoorBan.NewRow();
                        dtNew.ItemArray = drBan.ItemArray.ToArray();
                        dtDoorBan.Rows.Add(dtNew);
                    }
                }

            }

            if (dtDoorSize.Rows.Count > 0)
            {
                dgSize.DataSource = dtDoorSize;
                dgDoorBan.DataSource = dtDoorBan;
                dgDoorShell.DataSource = dtDoorShell;
            }

            // ConstantMethod.ShowInfo(rtbresult, "总共" + doorLst.Count.ToString() + "扇门！");

        }
        public void ShowDoor(int id, DataGridView dgSize, DataGridView dgDoorBan, DataGridView dgDoorShell)
        {
            if (id < DoorLst.Count)
            {
                dgSize.DataSource = DoorLst[id].Door_Size;
                dgDoorBan.DataSource = DoorLst[id].Door_Ban;
                dgDoorShell.DataSource = DoorLst[id].Door_shell;
            }      

            // ConstantMethod.ShowInfo(rtbresult, "总共" + doorLst.Count.ToString() + "扇门！");

        }
        public void ShowResult(ListView listresult)
        {
            int j = 0;
            listresult.Clear();
            listresult.View = View.Details;

            ColumnHeader c1h;

            c1h = new ColumnHeader();

            c1h.Text = "序号";

            listresult.Columns.Add(c1h);

            c1h = new ColumnHeader();

            c1h.Text = "门型";
            c1h.Width = 600;
            listresult.Columns.Add(c1h);

            for (int i = 0; i <DoorLst.Count; i++)
            {
                if (DoorLst[i].Door_Size.Rows.Count == 0)
                {
                    DoorLst.RemoveAt(i);
                }
                else
                {
                  
                    ListViewItem item = listresult.Items.Add(DoorLst[i].Xuhao.ToString());
                    item.SubItems.Add(DoorLst[i].Name);          
                    //ConstantMethod.ShowInfo(rtbresult, "第" + j.ToString() + "扇门:" + doorLst[i].Name);
                }
            }

           // ConstantMethod.ShowInfo(rtbresult, "总共" + doorLst.Count.ToString() + "扇门！");

        }

        public void showDoorTypeList(ComboBox cb1, int id)
        {
            List<string> typeNameLst = new List<string>();
            if (cb1 != null)
            {
                cb1.Items.Clear();
                foreach (doorTypeInfo dti in DoorLst)
                {
                    if (!typeNameLst.Contains(dti.Name))
                    {
                        typeNameLst.Add(dti.Name);
                    }
                }

                if (typeNameLst.Count > 0)
                {
                    cb1.Items.AddRange(typeNameLst.ToArray());
                    cb1.SelectedIndex = 0;
                }
            }
        }

        public void ShowResult(RichTextBox rtbresult)
        {
            int j = 0;
            for(int i=DoorLst.Count-1;i>-1;i--)            
            {
                if (DoorLst[i].Door_Size.Rows.Count == 0)
                {
                    DoorLst.RemoveAt(i);
                }
                else
                {
                    j++;
                    ConstantMethod.ShowInfo(rtbresult, "第"+j.ToString()+"扇门:"+DoorLst[i].Name);
                }             
            }

            ConstantMethod.ShowInfo(rtbresult, "总共"+ DoorLst.Count.ToString()+"扇门！");


        }
      
        public bool LoadData()
        {
           

            if (op1.ShowDialog() == DialogResult.OK)
            {
                op1.InitialDirectory = Path.GetDirectoryName(op1.FileName);
                return fileConvert.LoadDoorTypeDataTable(op1.FileName,DoorLst);
            }

            return false;
        }
    }
    public class doorTypeInfo
    {
        string name;


        string xuhao;
        public string Xuhao
        {
            get { return xuhao; }
            set { xuhao = value; }
        }
        string gwId = "1";
        public string GwId
        {
            get { return gwId; }
            set { gwId = value; }
        }
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
