using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using xjplc;

namespace evokNewXJ
{
    public class HySkDevice
    {
        public string[] DataColStr = 
        {
           "长度定位距离A", "宽度定位距离E", "长度F","宽度G","加工深度","刀号选择1大刀/2小刀",
           "外圈铣=1/全铣=2",
           "进刀量",
           "圆弧开=1/圆弧关=0",
           "左边=1/右边=0"
        };

        double MaxA=1000000;
        double MinA=0;

        double MaxE=1000000;
        double MinE=0;

        double MaxDrillDepth=100000;
        double MinDrillDepth=0;

        double MaxJdDepth=1000000;
        double MinJdDepth=0;


        public List<HySkCutParam> HySkLst;
        DataTable UserData;
        public HySkDevice()
        {
            Init();
        }


        void tempDataAdd(int id)
        {
            for (int i = 0; i < id; i++)
            {
                DataRow dr = UserData.NewRow();
                dr[0] = 800;
                dr[1] = 20;
                dr[2] = 60;
                dr[3] = 70;
                dr[4] = 30;
                dr[5] = 1;
                dr[6] = 1;
                dr[7] = 1;
                dr[8] = 1;
                dr[9] = 0;
                UserData.Rows.Add(dr);
            }



            for (int i = 0; i < id; i++)
            {
                DataRow dr = UserData.NewRow();
                dr[0] = 700;
                dr[1] = 10;
                dr[2] = 50;
                dr[3] = 60;
                dr[4] = 20;
                dr[5] = 1;
                dr[6] = 1;
                dr[7] = 1;
                dr[8] = 1;
                dr[9] = 1;
                UserData.Rows.Add(dr);
            }
        }
        DataGridView dgv;
        void Init()
        {
            HySkLst = new List<HySkCutParam>();
            UserData = ConstantMethod.getDataTableByString(DataColStr);
            tempDataAdd(6);


        }

        public void ShowUserData(DataGridView dgv1)
        {
            this.dgv = dgv1;
            
            dgv.DataSource = UserData;
            ConstantMethod.NoSortDatagridView(ref dgv);
        }
        void SetColor(int id,int ColorId)
        {
            if (id >= 0 && id < UserData.Rows.Count)
            {
                 if(ColorId==0)
                dgv.Rows[id].DefaultCellStyle.BackColor = Color.Red;
                if (ColorId == 1)
                dgv.Rows[id].DefaultCellStyle.BackColor = Color.Green;
            }
        }
        public bool DataOK = true;
        public void ReadData()
        {
            if (UserData.Rows.Count > 0)
            {
                int id = -1;
                DataOK = true; 
                HySkCutParam hk = new HySkCutParam();
                foreach (DataRow  dr in UserData.Rows)
                {
                    id++;
                    hk = new HySkCutParam();
                    SetColor(id, 0);
                    double posX;  //位置X
                    double posY;//位置Y
                    HySk hysk = new HySk();  //合页尺寸
                    double drillDepth;  //钻孔深度
                    int knife;        //刀号
                    int cutMode;       //切割模式--全铣 半铣
                    double jdDepth;    //进刀量
                    int roundSwitch;   //圆弧开关
                    int dir;           //左边 右边   

                    if (
                        double.TryParse(dr[0].ToString(), out posX) &&
                        posX < MaxA && posX > MinA
                        )
                    {
                        hk.PosX = posX;

                    }
                    else { DataOK = false; continue;  }

                    if (double.TryParse(dr[1].ToString(), out posY) &&
                        posY < MaxA && posY > MinA
                        )
                    {
                        hk.PosY = posY;
                       
                    }
                    else
                    
                        { DataOK = false; continue; }
                    


                    if (
                        double.TryParse(dr[2].ToString(), out hysk.Len)&&
                        double.TryParse(dr[3].ToString(), out hysk.Width)

                        )
                    {
                        hk.Hysk = hysk;

                    }
                    else { DataOK = false; continue; }

                    if (
                        double.TryParse(dr[4].ToString(), out drillDepth) 
                                               
                        )
                    {
                        hk.DrillDepth = drillDepth;

                    }
                    else { DataOK = false; continue; }

                    if (
                       int.TryParse(dr[5].ToString(), out knife)

                       )
                    {
                        hk.Knife = knife;
                    }
                    else { DataOK = false; continue; }

                    if (
                       int.TryParse(dr[6].ToString(), out cutMode)

                       )
                    {
                        hk.CutMode = cutMode;
                    }
                    else { DataOK = false; continue; }


                    if (
                       double.TryParse(dr[7].ToString(), out jdDepth)

                       )
                    {
                        hk.JdDepth = jdDepth;
                    }
                    else { DataOK = false; continue; }

                    if (
                       int.TryParse(dr[8].ToString(), out roundSwitch)

                       )
                    {
                        hk.RoundSwitch = roundSwitch;
                    }
                    else { DataOK = false; continue; }

                    if (
                       int.TryParse(dr[9].ToString(), out dir)

                       )
                    {
                        hk.Dir = dir;
                    }
                    else { DataOK = false; continue; }
                    HySkLst.Add(hk);
                    SetColor(id, 1);
                     
                }
            }
        }

        public  Action<List<HySkCutParam>> DownLoadData;
       

    }
    public struct HySk
    {
        public double Len;

        public double Width;


        public double R;
       
    }
    public struct HySkCutParam
    {
        double posX;  //位置X
        public double PosX
        {
            get { return posX; }
            set { posX = value; }
        }
        double posY;//位置Y
        public double PosY
        {
            get { return posY; }
            set { posY = value; }
        }
        HySk hysk;  //合页尺寸
        public evokNewXJ.HySk Hysk
        {
            get { return hysk; }
            set { hysk = value; }
        }
        double drillDepth;  //钻孔深度
        public double DrillDepth
        {
            get { return drillDepth; }
            set { drillDepth = value; }
        }
        int knife;        //刀号
        public int Knife
        {
            get { return knife; }
            set { knife = value; }
        }
        int cutMode;       //切割模式--全铣 半铣
        public int CutMode
        {
            get { return cutMode; }
            set { cutMode = value; }
        }
        double jdDepth;    //进刀量
        public double JdDepth
        {
            get { return jdDepth; }
            set { jdDepth = value; }
        }
        int roundSwitch;   //圆弧开关
        public int RoundSwitch
        {
            get { return roundSwitch; }
            set { roundSwitch = value; }
        }
        int dir;           //左边 右边             
        public int Dir
        {
            get { return dir; }
            set { dir = value; }
        }
    }

}
