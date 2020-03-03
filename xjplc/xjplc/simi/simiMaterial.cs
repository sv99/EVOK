using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xjplc.simi
{
    public class simiMaterial
    {


        string materialName;
        public string MaterialName
        {
            get { return materialName; }
            set { materialName = value; }
        }
        double len;
        public double Len
        {
            get { return len; }
            set { len = value; }
        }
        double height;
        public double Height
        {
            get { return height; }
            set { height = value; }
        }
        double width;
        public double Width
        {
            get { return width; }
            set { width = value; }
        }

        //参数1 定位基准开始
        //参数2 宽度系数
        List<string> paramLst;
        public System.Collections.Generic.List<string> ParamLst
        {
            get { return paramLst; }
            set { paramLst = value; }
        }

        DataTable materialData;


        CsvStreamReader csvop;

        void Init()
        {
            paramLst = new List<string>();
        }
        void Init(string name)
        {
            paramLst = new List<string>();
            csvop = new CsvStreamReader();
            materialData = csvop.OpenCSV(name);
        }

        public string[] materialLst
        {
            get
            {
                if (materialData != null && materialData.Rows.Count > 0)
                {
                    List<string> sLst = new List<string>();
                    foreach (DataRow dr in materialData.Rows)
                    {
                        sLst.Add(dr[0].ToString());
                    }
                    return sLst.ToArray();
                }
                return null;
              
            }
        }
                
        //第7 是代号
        //第6 是 width0
        
        public bool setMaterial(string mcode)
        {
            if (materialData != null && materialData.Rows.Count > 0)
            {
                foreach (DataRow dr in materialData.Rows)
                {
                    if (dr[0].ToString().Equals(mcode))
                    {
                        MaterialName = mcode;
                        string lenStr   = dr[2].ToString();
                        string widthStr = dr[3].ToString();
                        string thickStr = dr[4].ToString();
                       
                        if (double.TryParse(lenStr, out len)
                            && double.TryParse(widthStr, out width)
                            && double.TryParse(thickStr, out height))                      
                        {
                            paramLst.Clear();

                            for (int i = 5; i < materialData.Columns.Count; i++)
                            {
                                paramLst.Add(dr[i].ToString());
                            }

                            return true;
                        }                                              
                    }
                }
            }

            return false;
        }

        const int noShift = 0; //
        const int upShift = 1;
        const int downShift = 2;       
        //四边形的数据 上下移不一样 
        //根据客户的尺寸计算 任意一边的尺寸
        //输入尺寸名称 尺寸 左角度  右角度 尺寸上下移选择 
        //尺寸宽度分割系数 系数为下移宽度在总宽度所占的系数
        //现在统一默认换算尺寸是顶边,
        //接下来如果是底边那就换算成底边
        public  double calculateSize(string size, string leftAngle, string rightAngle,ref string oppositeSize,ref string maxSize)
        {
            double sized = 0;
            double leftAngled = 45;
            double rightAngled = 45;

            double widthDown = 1;
            double id = 2;

            double opposite=0;
            
            if (double.TryParse(size, out sized))
            {

            }
            else
                return 0;

            if (double.TryParse(leftAngle, out leftAngled))
            {

            }
            else
                return 0;

            if (double.TryParse(rightAngle, out rightAngled))
            {

            }
            else
                return 0;


            if (paramLst.Count <2) return 0;

            if (double.TryParse(paramLst[0], out id))
            {

            }
            else
            return 0;

            if (double.TryParse(paramLst[1], out widthDown))
            {

            }
            else
            return 0;

           // if (widthDown = 1) return 0;

            if (id == upShift)
            {
                double tanleftAngle = Math.Tan(leftAngled / 180 * Math.PI);
                double tanRightAngle = Math.Tan(rightAngled / 180 * Math.PI);

                double leftSize = 0;// widthDown / tanleftAngle;
                double rightSize = 0;
                if(tanleftAngle<1000)
                leftSize =  widthDown / tanleftAngle;

                if(tanRightAngle<1000)
                rightSize = widthDown / tanRightAngle;


                opposite =sized + leftSize - rightSize;
                oppositeSize = opposite.ToString("0.00");

               leftSize = 0;// widthDown / tanleftAngle;
               rightSize = 0;

                if (tanleftAngle < 1000)
                    leftSize = (Width - widthDown) / tanleftAngle;

                if (tanRightAngle < 1000)
                    rightSize = (Width - widthDown) / tanRightAngle;

                sized = sized - leftSize + rightSize;
            
             }
            else
            if (id == downShift)
            {

                opposite = sized - (Width - widthDown) / Math.Tan(leftAngled / 180 * Math.PI) + (Width - widthDown) / Math.Tan(rightAngled / 180 * Math.PI);
                oppositeSize = opposite.ToString("0.00");
                sized = sized + widthDown / Math.Tan(leftAngled / 180 * Math.PI) - widthDown / Math.Tan(rightAngled / 180 * Math.PI);

             }

            maxSize = sized.ToString("0.00");

            if ((leftAngled!=90 && rightAngled!=90)&&((leftAngled > 0 && rightAngled > 0 ) || (leftAngled < 0 && rightAngled < 0)))
            {
                double max = sized + width;
                maxSize = max.ToString("0.00");
            }
            else if (opposite > sized) maxSize = opposite.ToString("0.00");

            return sized;
        }


        public double calculateSize0(string topsize, string leftAngle, string rightAngle, ref string oppositeSize, ref string usersize)
        {
            double sized = 0;
            double leftAngled = 45;
            double rightAngled = 45;

            double widthDown = 1;
            double id = 2;


            if (double.TryParse(topsize, out sized))
            {

            }
            else
                return 0;

            if (double.TryParse(leftAngle, out leftAngled))
            {

            }
            else
                return 0;

            if (double.TryParse(rightAngle, out rightAngled))
            {

            }
            else
                return 0;


            if (paramLst.Count < 2) return 0;

            if (double.TryParse(paramLst[0], out id))
            {

            }
            else
                return 0;

            if (double.TryParse(paramLst[1], out widthDown))
            {

            }
            else
                return 0;

            if (leftAngled == 90 && rightAngled == 90)
            {
                usersize = sized.ToString();
                oppositeSize = sized.ToString();
            }
            if (leftAngled == 90 || rightAngled == 90)
            {
                if (leftAngled != 90)
                {
                    oppositeSize =
                   (sized + (int)((Width / Math.Tan(leftAngled / 180 * Math.PI)))).ToString();
                    usersize = (sized + (int)((Width  - widthDown) / Math.Tan(leftAngled / 180 * Math.PI))).ToString();
                }
                if (rightAngled != 90)
                {
                    oppositeSize =
                   (sized - (int)((Width / Math.Tan(leftAngled / 180 * Math.PI)))).ToString();
                    usersize = 
                   (sized - (int)((Width  - widthDown) / Math.Tan(leftAngled / 180 * Math.PI))).ToString();
                }
            }
            else
            {
                oppositeSize =
                  (sized +
                   Width/ Math.Tan(leftAngled / 180 * Math.PI)-Width / Math.Tan(leftAngled / 180 * Math.PI)
                   ).ToString();
                usersize =
               (sized +
                  ( Width-widthDown) / Math.Tan(leftAngled / 180 * Math.PI) - (Width - widthDown) / Math.Tan(leftAngled / 180 * Math.PI)
                   ).ToString();
            }

         

            return sized;
        }

        public simiMaterial(string filename)
        {
            Init(filename);
        }
        public simiMaterial()
        {
            Init();
        }

    }
}
