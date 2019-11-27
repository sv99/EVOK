using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using xjplc;

namespace xjplc
{
    public class PaintRect
    {
         double ration;
         int xmargin = 5;   //锯片厚度
         int ymargin = 60; //每一行的间距
         int height;
         int len = 0;
          Point OrigninP;
        
        public PaintRect(int len, Point OP)
        {

            OrigninP = OP;

            this.len = len;
           

        }

        public void SetRation(ProdInfo p)
        {

            ration = 0.3098;// len * 100 / (double)p.Len;

            xmargin = (int)(p.DBC * ration / 100);

            double het = 0;

           if(double.TryParse(p.Param16[0],out het)) //材料宽度
            height = (int)(het * ration);

        }
        private void DrawRect(Bitmap bt, Point p, int width, int height,int ColorId)
        {
            Graphics g = Graphics.FromImage(bt);
            //出现一个画笔  
            Pen pen = new Pen(Brushes.Red,7);
            //因为创建矩形需要point对象与size对象  
            Size s = new Size(width, height);
            Rectangle r = new Rectangle(p, s);

            g.DrawRectangle(pen, r);
            switch (ColorId)
            {
                case 1:
                    { g.FillRectangle(new SolidBrush(Color.Green), p.X, p.Y, width, height); }
                    break;
                case 2:
                    { g.FillRectangle(new SolidBrush(Color.Red), p.X, p.Y, width, height); }
                    break;
            }
            
        }
        /***
        public void ProdDrawRect(ProdInfoDraw prodinfo, ref  Bitmap bt,int px,int count)
        {
            double ration = 1000 / (double)prodinfo.Len;
            Point xtop = new Point(OrigninP.X, OrigninP.Y);

            if (bt == null)
            {
                bt = new Bitmap(OrigninP.X + 1000 + xmargin * (prodinfo.Cut.Count + 1) , 2*OrigninP.Y + count*(height + ymargin));
            }
                      
            
            if (prodinfo.Cut.Count > 0)
            {
                xtop.Y = xtop.Y + px * height + ymargin* px;
                for (int i = 0; i < prodinfo.Cut.Count; i++)
                {
                    int xwidth = (int)(ration * prodinfo.Cut[i]);
                    DrawRect(bt, xtop, xwidth, height,1);
                    xtop.X = xtop.X  + xmargin+ xwidth ;
                }
                int xwl = (int)(ration * prodinfo.WL);
                DrawRect(bt, xtop, xwl, height,2);

            }
        }
        ***/
        //这里考虑特殊情况 比如 一个尺寸左边角度和上一个尺寸右边角度 都是正的 但是左边角度<右边
        Point[] pointArrayGet(ref Point  xtop ,double leftAngle,double rightAngle,double nextleftAngle, double prerightAngleint,int upSize,int downSize)
        {
            List<Point> pLst = new List<Point>();

            Point p0 = new Point();  //左上
            Point p1 = new Point();  //左下
            Point p2 = new Point();  //右上
            Point p3 = new Point();  //右下

            ConstantMethod.PointCopy(ref p0, xtop);
            ConstantMethod.PointCopy(ref p1, p0);
            p1.Y += height;


            ConstantMethod.PointCopy(ref p2, p0);
            ConstantMethod.PointCopy(ref p3, p1);
           
            //分三种情况  左角度为90或-90   正 负         
            //正的度数
            if (leftAngle > 0 && leftAngle < 90)
            {
                if (prerightAngleint > 0 && prerightAngleint > leftAngle &prerightAngleint<90)
                {
                    p1.X -= (int)(height / Math.Tan(leftAngle / 180 * Math.PI));
                }
                else
                p0.X += (int)(height / Math.Tan(leftAngle / 180 * Math.PI));
            }
            else
            {
                //负的度数
                if (leftAngle < 0 && leftAngle > -90)
                {
                    //if (prerightAngleint < 0 && prerightAngleint < leftAngle & prerightAngleint > -90)
                    //{
                        p1.X = (p1.X+(int)(Math.Abs(height / Math.Tan(leftAngle / 180 * Math.PI))));
                   // }
                    //else
                   // p1.X += (int)(height / Math.Tan(leftAngle / 180 * Math.PI));
                }

            }
            //在一个方向上加上尺寸  就可以了                
            p2.X = (p0.X+upSize);
            p3.X = (p1.X+downSize);

            if (((nextleftAngle >= 0 && rightAngle >= 0) || (nextleftAngle < 0 && rightAngle < 0)) &&
                (rightAngle != 90 && nextleftAngle!=90)
                )
            {
                if (Math.Abs(Math.Abs(nextleftAngle) - Math.Abs(rightAngle)) < 2)
                {
                    if (p2.X >= p3.X)
                    {
                        xtop.X = (p3.X + calcuXMargin(rightAngle));
                    }
                    else
                    {
                        xtop.X = (p2.X + calcuXMargin(rightAngle));
                    }
                }
                else
                {

                    if (rightAngle < 0 && rightAngle > nextleftAngle)
                    {

                        xtop.X = (int)(calcuXMargin(rightAngle) + p2.X +

                            Math.Abs(height / Math.Tan(rightAngle / 180 * Math.PI)) -
                            Math.Abs(height / Math.Tan(nextleftAngle / 180 * Math.PI)))
                            ;
                    }
                    if (rightAngle<90 &&rightAngle > 0 && nextleftAngle>0&& rightAngle > nextleftAngle && nextleftAngle <90 )
                    {

                        xtop.X = (int)(calcuXMargin(rightAngle) + p2.X 

                            
                            +Math.Abs(height / Math.Tan(nextleftAngle / 180 * Math.PI)
                            - Math.Abs(height / Math.Tan(rightAngle / 180 * Math.PI))))
                            ;
                    }
                }

               
            }
            else
            {
                if (p2.X >= p3.X)
                {
                    
                    xtop.X = (calcuXMargin(rightAngle) + p2.X);
                }
                else
                {
                    xtop.X = (calcuXMargin(rightAngle) + p3.X);
                }
            }
            //90度 直接相等
            if (leftAngle == 90)
            {
                p0.X = p1.X;
                
            }
            if (rightAngle == 90)
            {
                p2.X = p3.X;

            }

            pLst.Add(p0);
            pLst.Add(p2);
            pLst.Add(p3);
            pLst.Add(p1);
          
                      
            return pLst.ToArray();

        }

        //根据角度和中线长度进行计算点位
        //这里为了 计算下一个距离 需要返回一个起始点距离 int NextMargin = 0;
        Point[] pointArrayGet(Point stPoint,double angleLeft,double angleRight,int height,int width,ref int nextMargin)
        {
            List<Point> pLst = new List<Point>();

            Point p0 = new Point();
            Point p1 = new Point();
            Point p2 = new Point();
            Point p3 = new Point();
          
            if (angleLeft < 90 && angleLeft >=0)
            {
                p0.X = stPoint.X;

                p0.Y = stPoint.Y + height;

                p1.X = (int)(p0.X + (height)/Math.Tan(Math.PI/(180/angleLeft)));

                p1.Y = p0.Y - height;

            }

            if (angleLeft < 0 && angleLeft >-90)
            {
                p0.X = stPoint.X+height;

                p0.Y = stPoint.Y + height;

                p1.X =(int) (p0.X + (height) / Math.Tan(Math.PI / (180 / angleLeft)));
                p1.Y = p0.Y - height;

            }

            if (angleLeft == 90  || angleLeft == -90)
            {
                p0.X = stPoint.X;

                p0.Y = stPoint.Y + height;

                p1.X = p0.X;

                p1.Y = p0.Y - height;
            }

            if ( angleRight < 90 && angleRight >= 0)
            {
                p2.Y = p1.Y;
                if (angleLeft<90 && angleLeft >= 0)
                {
                    p2.X = p1.X + width;
                }
                if (angleLeft < 0 && angleLeft > -90)
                {
                    p2.X = p1.X + width+height;
                }

                if (angleLeft == 90 || angleLeft == -90)
                {
                    p2.X = p1.X + width + height/2;
                }


                p3.X = p2.X - height;

                p3.Y = p2.Y + height;

            }

            if (angleRight < 0 && angleRight >-90)
            {
                p2.Y = p1.Y;

                if (angleLeft < 90 && angleLeft >= 0)
                {
                    p2.X = p1.X + width;
                }
                if (angleLeft < 0 && angleLeft > -90)
                {
                    p2.X = p1.X + width - height; 
                }

                if (angleLeft == 90 || angleLeft == -90)
                {
                    p2.X = p1.X + width - height / 2;
                }

                p3.X = p2.X + height;

                p3.Y = p2.Y + height;

            }


            if (angleRight == 90)
            {
                p2.Y = p1.Y;

                if (angleLeft < 90 && angleLeft >= 0)
                {
                    p2.X = p1.X + width-height/2;
                }
                if (angleLeft < 0 && angleLeft > -90)
                {
                    p2.X = p1.X + width + height/2;
                }

                if (angleLeft == 90 || angleLeft == -90)
                {
                    p2.X = p1.X + width;
                }

                p3.X = p2.X;
                p3.Y = p2.Y + height;

            }


            pLst.Add(p0);
            pLst.Add(p1);
            pLst.Add(p2);
            pLst.Add(p3);

            int max = 0;
            int min = 1000000;
            foreach (Point ps in pLst)
            {
                if (ps.X > max)
                {
                    max = ps.X;
                }
                if (ps.X < min)
                {
                    min = ps.X; 
                }
            }
            nextMargin = max - min;

            return pLst.ToArray();
                             
        }

        public int calcuXMargin(double angle)
        {
            if (angle == 90 || angle == -90) return xmargin;
            return Math.Abs((int)(xmargin / Math.Sin(angle/180*Math.PI)));
        }
        public void DrawLen(Bitmap bt,int id,int len)
        {
                    
            Graphics g = Graphics.FromImage(bt);
        
            //出现一个画笔  
            Pen pen = new Pen(Brushes.Red, 5);
     
            g.DrawRectangle(pen, OrigninP.X, OrigninP.Y+(height+ymargin)*id,len, height);


        }
        string  PackShowStr(int xuhao,ProdInfo prodinfo)
        {
            string str = "";
            double liaochang = 0;

            liaochang = prodinfo.Len / 100;
    
            str = "切割序号："+ (xuhao+1).ToString()+"  料长：" + liaochang.ToString("0.00");

            int countId = 1;
            foreach (int size in prodinfo.Cut)
            {

                double sizedouble =(double)size / 100;

                if (countId > 3)
                {
                    str += "\r\n";
                }

                str += 
                ("  "+countId.ToString() + "段: " +
                 sizedouble.ToString("0.00") +
                " L" + prodinfo.leftAngle[countId-1]
                 + " R" + prodinfo.rightAngle[countId - 1]
                    );
                countId++;
            }
            double wlDouble = ((double)prodinfo.WL / 100);
            str += ("  余料：" + wlDouble.ToString("0.00"));
            return str;
        }
        public void ProdDrawPloygon(ProdInfo prodinfo, ref Bitmap bt, int heightId, PictureBox p1)
        {

            int count = prodinfo.Cut.Count;
            SetRation(prodinfo);
            Point xtop = new Point(OrigninP.X, OrigninP.Y);

            if (bt == null)
            {
                bt = new Bitmap(OrigninP.X + len + xmargin * (prodinfo.Cut.Count + 1) + 100 * xmargin + 100, 2 * OrigninP.Y + (count + 50) * (height + ymargin));
                //p1.Height = bt.Height + 100;
            }

            DrawLen(bt, heightId, (int)(prodinfo.Len * ration / Constant.dataMultiple));


            Point[] pArray = new Point[4];
            Point[] pLine = new Point[2];
            if (prodinfo.Cut.Count > 0)
            {


                xtop.X = xtop.X + calcuXMargin(prodinfo.leftAngle[0]);
                xtop.Y = xtop.Y + heightId * height + ymargin * heightId;
                pLine[0] = xtop;
                pLine[1] = xtop;

                pLine[1].Y += height;

                DrawLine(bt, pLine);

                //开始画图
                for (int i = 0; i < prodinfo.Cut.Count; i++)
                {
                    double upSize = 0;
                    double downSize = 0;
                    if (!double.TryParse(prodinfo.Param5[i], out upSize))
                    {
                        MessageBox.Show(Constant.convertError + Constant.resultTip5 + heightId.ToString() +
                            Constant.resultTip6 + Constant.resultTip5 + i.ToString() + Constant.resultTip7);
                        return;
                    }
                    if (!double.TryParse(prodinfo.Param6[i], out downSize))
                    {
                        MessageBox.Show(Constant.convertError + Constant.resultTip5 + heightId.ToString() +
                            Constant.resultTip6 + Constant.resultTip5 + i.ToString() + Constant.resultTip7);
                        return;
                    }

                    upSize = upSize * (ration);
                    downSize = downSize * (ration);

                    if (i < (prodinfo.Cut.Count() - 1))
                    {
                        if (i != 0)
                            pArray =
                        pointArrayGet(ref xtop, prodinfo.leftAngle[i], prodinfo.rightAngle[i], prodinfo.leftAngle[i + 1], prodinfo.rightAngle[i - 1], (int)upSize, (int)downSize);
                        else
                            pArray =
                        pointArrayGet(ref xtop, prodinfo.leftAngle[i], prodinfo.rightAngle[i], prodinfo.leftAngle[i + 1], 90, (int)upSize, (int)downSize);

                    }
                    else
                    {
                        if (i > 0)
                            pArray =
                            pointArrayGet(ref xtop, prodinfo.leftAngle[i], prodinfo.rightAngle[i], 0, prodinfo.rightAngle[i - 1], (int)upSize, (int)downSize);
                        else
                            pArray =
                            pointArrayGet(ref xtop, prodinfo.leftAngle[i], prodinfo.rightAngle[i], 0, 0, (int)upSize, (int)downSize);

                    }
                    if (i != 0)
                        DrawPolygon(bt, pArray, 1, "");
                    else
                    {
                        DrawPolygon(bt, pArray, 1, PackShowStr(heightId, prodinfo));
                    }
                }

                if (bt != null)
                    p1.Image = bt;

            }
        }

        public void ProdDrawPloygon(ProdInfo prodinfo, ref Bitmap bt, int heightId,PictureBox p1,int xuhao)
        {
            int count = prodinfo.Cut.Count;
             SetRation(prodinfo);
            Point xtop = new Point(OrigninP.X, OrigninP.Y);

            if (bt == null)
            {
                bt = new Bitmap(OrigninP.X + len + xmargin * (prodinfo.Cut.Count + 1)+100*xmargin+100, 2 * OrigninP.Y + (count+50) * (height + ymargin));
                //p1.Height = bt.Height + 100;
            }

            DrawLen(bt,heightId, (int)(prodinfo.Len*ration/Constant.dataMultiple));


            Point[] pArray = new Point[4];
            Point[] pLine = new Point[2];
            if (prodinfo.Cut.Count > 0)
            {
             
                  
                xtop.X = xtop.X + calcuXMargin(prodinfo.leftAngle[0]);                                                         
                xtop.Y = xtop.Y + heightId * height + ymargin * heightId;
                pLine[0] = xtop;
                pLine[1] = xtop;

                pLine[1].Y += height;

                DrawLine(bt, pLine);
                
                //开始画图
                for (int i = 0; i < prodinfo.Cut.Count; i++)
                {
                    double upSize = 0; 
                    double downSize = 0;
                    if (!double.TryParse(prodinfo.Param5[i], out upSize))
                    {
                        MessageBox.Show(Constant.convertError+Constant.resultTip5+heightId.ToString()+
                            Constant.resultTip6 + Constant.resultTip5 + i.ToString()+ Constant.resultTip7);
                        return;
                    }
                    if (!double.TryParse(prodinfo.Param6[i], out downSize))
                    {
                        MessageBox.Show(Constant.convertError + Constant.resultTip5 + heightId.ToString() +
                            Constant.resultTip6 + Constant.resultTip5 + i.ToString() + Constant.resultTip7);
                        return;
                    }

                    upSize = upSize*(ration);
                    downSize = downSize*(ration);

                    if (i < (prodinfo.Cut.Count() - 1))
                    {
                        if(i!=0)
                            pArray =
                        pointArrayGet(ref xtop, prodinfo.leftAngle[i], prodinfo.rightAngle[i], prodinfo.leftAngle[i + 1], prodinfo.rightAngle[i-1],(int)upSize, (int)downSize);
                        else
                            pArray =
                        pointArrayGet(ref xtop, prodinfo.leftAngle[i], prodinfo.rightAngle[i], prodinfo.leftAngle[i + 1], 90, (int)upSize, (int)downSize);

                    }
                    else
                    {
                        if(i>0)
                        pArray =
                        pointArrayGet(ref xtop, prodinfo.leftAngle[i], prodinfo.rightAngle[i], 0, prodinfo.rightAngle[i - 1],(int)upSize, (int)downSize);
                        else
                        pArray =
                        pointArrayGet(ref xtop, prodinfo.leftAngle[i], prodinfo.rightAngle[i], 0, 0, (int)upSize, (int)downSize);

                    }
                    if (i != 0)
                        DrawPolygon(bt, pArray, 1, "");
                    else
                    {
                        DrawPolygon(bt, pArray, 1, PackShowStr(xuhao,prodinfo));
                    }                  
                }

                if(bt !=null)
                p1.Image = bt;            

            }
        }
        /***
        //px 是高度间隙      
        public void  ProdDrawPloygon(ProdInfoDraw prodinfo, ref Bitmap bt, int heightId, int count)
        {
            
            double ration = 800 / (double)prodinfo.Len;

            Point xtop = new Point(OrigninP.X, OrigninP.Y);

            if (bt == null)
            {
                bt = new Bitmap(OrigninP.X + 1300 + xmargin * (prodinfo.Cut.Count + 1), 2 * OrigninP.Y + count * (height + ymargin));
            }

            int nextmargin=0;

            string drawstr = "";

            Point[] pArray= new Point[4];

            if (prodinfo.Cut.Count > 0)
            {

                xtop.Y = xtop.Y + heightId * height + ymargin * heightId;

                for (int i = 0; i < prodinfo.Cut.Count; i++)
                {
                    int xwidth = (int)(ration * prodinfo.Cut[i]);
                    pArray=
                    pointArrayGet(xtop, prodinfo.leftAngle[i], prodinfo.rightAngle[i], height, xwidth,ref nextmargin);
                    DrawPolygon(bt, pArray,1,"");
                    if ((i + 1) < (prodinfo.Cut.Count) && (prodinfo.rightAngle[i] == prodinfo.leftAngle[i+1]))
                    {
                        xtop.X = xtop.X + xmargin + nextmargin-(int)(height/Math.Tan(Math.PI / (180 / Math.Abs(prodinfo.rightAngle[i]))));
                    }
                    else
                    xtop.X = xtop.X + xmargin +nextmargin;
                    double showstr = ((double)prodinfo.Cut[i]) / 100;
                    drawstr += showstr + "--";
                }
                              
                    double xwl = ration * prodinfo.WL;
                    drawstr += "尾料：" + xwl.ToString("0.00");
                    Point[] pArrayWl = 
                    pointArrayGet(xtop, prodinfo.rightAngle[prodinfo.Cut.Count - 1], 90, height, (int)xwl, ref nextmargin);
                    xtop.X = xtop.X + xmargin + nextmargin;
                    //显示序号和尺寸排布
                    if (pArray.Count() > 0)
                    DrawPolygon(bt, pArrayWl, 2, (heightId + 1) + ":" + drawstr);
               
            }
        }
        ***/
        public void DrawLine(Bitmap bt, Point[] pLst)
        {
            Graphics g = Graphics.FromImage(bt);
            //出现一个画笔  
            Pen pen = new Pen(Brushes.Red,5);
            //因为创建矩形需要point对象与size对象  
            if (pLst.Count() == 2)
            {              
                    g.DrawLine(pen, pLst[0].X, pLst[0].Y, pLst[1].X, pLst[1].Y);
            }

        }
        public void DrawPolygon(Bitmap bt, Point[] pLst,int colorId,string str)
        {
            Graphics g = Graphics.FromImage(bt);
            //出现一个画笔  
            Pen pen = new Pen(Brushes.Red,5);
            //因为创建矩形需要point对象与size对象  
          
            g.DrawPolygon(pen, pLst);
          
            switch (colorId)
            {
                case 1:
                    { g.FillPolygon(new SolidBrush(Color.Green), pLst); }
                    break;
                case 2:
                    { g.FillPolygon(new SolidBrush(Color.Red), pLst); }
                    break;
            }

            if (!string.IsNullOrWhiteSpace(str))
            {
                int max = 0;
                int min = 1000000;
                foreach (Point ps in pLst)
                {
                    if (ps.X > max)
                    {
                        max = ps.X;
                    }
                    if (ps.X < min)
                    {
                        min = ps.X;
                    }
                }
                int m = (max - min) / 2-10;
                //写文字
                Font drawFont = new Font("Arial", 12);
                SolidBrush drawBrush = new SolidBrush(Color.Black);
                if(str.Contains("4段"))
                g.DrawString(str, drawFont, drawBrush, 10, pLst[0].Y-55);
                else
                g.DrawString(str, drawFont, drawBrush, 10, pLst[0].Y-30);
            }

        }


    }
}
