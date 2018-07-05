﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace xjplc
{
    public class DTPlcInfoSimple
    {
        DTPlcInfo pInfo;

        Control showControl;

        List<string> showStr;
        public List<string> ShowStr
        {
            get { return showStr; }
            set { showStr = value;}
        }
        public Control ShowControl
        {
            get { return showControl; }
            set { showControl = value; }
        }

        public void SetPlcInfo(DTPlcInfo pInfo0)
        {
            pInfo = pInfo0;
        }
        string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public DTPlcInfoSimple(int addr0, string area0)
        {
            addr = addr0;
            area = area0;
            showStr = new List<string>();
        }
        public DTPlcInfoSimple(string name0)
        {
            name = name0;
            showStr = new List<string>();
        }

        bool isInEdit;//如果在编辑 就不去相应更新控件
        public bool IsInEdit
        {
            get { return isInEdit; }
            set { isInEdit = value; }
        }
        int showvalue;
        public int ShowValue //从表格读取数据回来
        {
            get
            {
                if (pInfo != null)
                {
                    if (showvalue != pInfo.PlcValue || showvalue == 0)
                    {
                        showvalue = pInfo.PlcValue;
                        if (!IsInEdit)
                        {
                            if (showControl != null && showControl is Button)
                            {
                                if (showvalue == 0)
                                {
                                    showControl.BackColor = System.Drawing.Color.Transparent;
                                    if (showStr.Count > showvalue)
                                    {
                                        showControl.Text = showStr[showvalue];
                                    }
                                }
                                else
                                {
                                    showControl.BackColor = System.Drawing.Color.Red;
                                    if (showStr.Count > showvalue)
                                    {
                                        showControl.Text = showStr[showvalue];
                                    }
                                }
                            }

                            if (showControl != null && (showControl is TextBox || showControl is Label))
                            {
                                showControl.Invoke((EventHandler)(delegate
                                {
                                    showControl.Text = showvalue.ToString();

                                }));
                            }
                        }
                    }
                }                                       
                return showvalue;
            }


            
        }
        int addr;
        public int Addr
        {
            get {             
                return addr; }
            set
            {
                if (area.Equals(Constant.strDMArea[5]) || (area.Equals(Constant.strDMArea[6])))
                {
                    addr = ConstantMethod.GetXYAddr8To10(value);              
                }
                else addr = value;

            }
        }
        string area;
        public string Area
        {
            get { return area; }
            set { area = value; }
        }
        string mode;
        public string Mode
        {
            get { return mode; }
            set { mode = value; }
        }
        DataTable belongToDataform;
        public System.Data.DataTable BelongToDataform
        {
            get { return belongToDataform; }
            set { belongToDataform = value; }
        }
        int rowIndex;
        public int RowIndex
        {
            get { return rowIndex; }
            set { rowIndex = value; }
        }

    }
    public class DTPlcInfo: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        //传入相对地址 和区域 单双字
        public DTPlcInfo(int addr, string areaIn, string valuemode)
        {
            this.relativeaddr = addr;
       
            this.StrArea = areaIn;
            absAddr = DTPLCPackCmdAndDataUnpack.AreaGetFromStr(relativeaddr, StrArea);
            intArea = DTPLCPackCmdAndDataUnpack.AreaGetFromStr(StrArea);
          
            valueMode = valuemode;
            //D区两个字节 
            if (intArea < Constant.M_ID)
                ByteValue = new byte[2];
            else ByteValue = new byte[1];

        }
        public DTPlcInfo()
        {

        }
        bool isInEdit;//如果在编辑 就不去相应更新监控数据表格
        public bool IsInEdit
        {
            get { return isInEdit; }
            set { isInEdit = value; }
        }
        //属于哪张表格
        DataTable belongToDT;
        public System.Data.DataTable BelongToDT
        {
            get { return belongToDT; }
            set { belongToDT = value; }
        }
        int col;
        public int Col
        {
            get { return col; }
            set { col = value; }
        }
        int row;//在监控数据表格中哪一行
        public int Row     
        {
            get { return row; }
            set { row = value; }
        }

        //单字还是双字
        string valueMode;
        public string ValueMode
        {
            get { return valueMode; }
            set { valueMode = value; }
        }
        //如果是双字 高字节 是哪个哦
        DTPlcInfo doubleModeHigh;
        public DTPlcInfo DoubleModeHigh
        {
            get { return doubleModeHigh; }
            set { doubleModeHigh = value; }
        }
        private string strArea;
        public string StrArea
        {
            get { return strArea; }
            set { strArea = value; }
        }
        //显示的控件 
        Control btn = null;
        //最小值
        private int minValue;
        public int MinValue
        {
            get { return minValue; }
            set { minValue = value; }
        }
        //最大值
        private int maxValue;
        public int MaxValue
        {
            get { return maxValue; }
            set {
                maxValue = value;

            }
        }
        //数据是否正常
        public bool IsValueNormal
        {
            get
            {
              return ((plcValue > (minValue + 1)) && (plcValue < (maxValue + 1))) ? true : false;
            }

        }
        //xuhao 在接收到数据中处个位置
        //如果是位的话 还有所在字节
        private Byte[] byteValue;
        public Byte[] ByteValue
        {
            get { return byteValue; }
            set {
                 byteValue = value;
       
            }
        }
        //所在字节 位置
        private int xuhao;  //对于M 是buffer的 第几个字节开始 对于d的话 没啥用 只要byte是两个数量就好了
        public int Xuhao
        {
            get { return xuhao; }
            set { xuhao = value; }
        }
        //哪个区的
        private int intArea;
        public int IntArea
        {
            set
            {
                intArea = value;
                if (intArea < Constant.strDMArea.Count())
                    StrArea = Constant.strDMArea[intArea];
            }
            get { return intArea; }
        }
        //地址是多少
        private int relativeaddr;
        public int RelAddr
        {
            set
            {
                relativeaddr = value;
                absAddr = XJPLCPackCmdAndDataUnpack.AreaGetFromStr(relativeaddr, StrArea);
            }
            get { return relativeaddr; }
        }

        private int absAddr;
        public int AbsAddr
        {
            get
            {
                return absAddr;
            }
            set
            {
                absAddr = value;
                relativeaddr = DTPLCPackCmdAndDataUnpack.RelAbsGet(absAddr, intArea);
            }

        }
           
        //显示的值 目前十进制
        private int plcValue;
        /// <summary>
        /// PlcVALUE 需要计算 每次get比较麻烦
        /// </summary>
        public int PlcValue
        {
            get
            {            
                if (ByteValue != null)
                    if (intArea < Constant.M_ID)
                    {
                        //如果是双字
                        if (valueMode.Equals(Constant.DoubleMode) && (DoubleModeHigh != null))
                        {                           
                            int value0 = ((int)(ByteValue[0] << 8) | (int)(ByteValue[1]));
                            int value1 = ((int)(doubleModeHigh.ByteValue[0] << 8) | (int)(doubleModeHigh.ByteValue[1]));
                            plcValue = ConstantMethod.Pack4BytesToInt(value0, value1);
                        }
                        else
                        {
                            plcValue = ((int)(ByteValue[0] << 8) | (int)(ByteValue[1]));
                        }
                    }
                    else
                    {
                        int duibi = 0;
                        duibi = (int)Math.Pow(2, Xuhao);
                        plcValue = (ByteValue[0] & duibi) == duibi ? 1 : 0;
                    }
            
                return plcValue;
            }
            set
            {                
                plcValue = value;             
            }
        }


    }
}