using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using xjplc.delta;

namespace xjplc
{
    public class DTPlcInfoSimple
    {
        DTPlcInfo pInfo;
        DTTcpPlcInfo pTcpInfo;
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
        bool isParam = true;
        public bool IsParam
        {
            get { return isParam; }
            set { isParam = value; }
        }
        public void SetPlcInfo(DTPlcInfo pInfo0)
        {
            pInfo = pInfo0;
        }
        public void SetPlcInfo(DTTcpPlcInfo pInfo0)
        {
            pTcpInfo = pInfo0;
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
        //最小值
        private int minValue=-1000000;
        public int MinValue
        {
            get { return minValue; }
            set { minValue = value; }
        }
        //最大值
        private int maxValue=100000000;
        public int MaxValue
        {
            get { return maxValue; }
            set
            {
                maxValue = value;

            }
        }
        //数据一般是要缩小下的
        double ration = 1;
        public double Ration
        {
            get { return ration; }
            set { ration = value; }
        }
        //数据是否正常
        public bool IsValueNormal
        {
            get
            {
            
                if (mode.Contains(Constant.REAL))
                {
                   
                   return ((showValueFloat >= minValue) && (showValueFloat <= maxValue)) ? true : false;
                    
                }
                else
                {
                    
                    if ((ConstantMethod.getModeCount(mode)) < 5 )
                    {

                        return ((showValue >=minValue) && (showValue <=maxValue)) ? true : false;
                    }
                }


                return true;
                
            }

        }
        bool isInEdit;//如果在编辑 就不去相应更新控件
        public bool IsInEdit
        {
            get { return isInEdit; }
            set { isInEdit = value; }
        }
        public string valueStr;

        //显示浮点数锯
        void controlValueShow(int id)
        {
        
            //没有在编辑状态
            if (!IsInEdit || showControl.Focused==false)
            {
                if (showControl != null && pTcpInfo != null && (showControl is TextBox || showControl is Label ))
                {
             
                    switch (id)
                    {
                        case Constant.floatShow:
                            {
                                //保留两位小数
                                if (IsValueNormal)
                                {                            
                                    showControl.Text = String.Format("{0:F}", ShowValueFloat);
                                }
                                else
                                    showControl.Text = Constant.dataOutOfRange;
                                break;
                            }
                        case Constant.intShow:
                            {
                                if (IsValueNormal)
                                {
                                    if (ration > 99)
                                    {
                                        showControl.Text = showValueFloat.ToString("0.00");
                                    }
                                    else
                                        showControl.Text = showValue.ToString();
                                }
                                else
                                    showControl.Text = Constant.dataOutOfRange;
                                break;
                            }
                    }


                }
                else
                {
                    if (showControl != null && (showControl is Button || showControl is ComboBox))
                    {
                        if (ShowControl is ComboBox && showValue < ((ComboBox)ShowControl).Items.Count)
                        {
                            (showControl as ComboBox).SelectedIndex = showValue;                        
                        }
                        else
                        if (showValue == 0)
                        {
                            showControl.BackColor = System.Drawing.Color.Transparent;
                            if (showStr.Count > showValue)
                            {
                                showControl.Text = showStr[showValue];
                            }
                        }
                        else
                        {
                            if (showValue > 0)
                            {
                                showControl.BackColor = System.Drawing.Color.Red;                              
                                if (showStr.Count > showValue)
                                {
                                    showControl.Text = showStr[showValue];
                                }
                            }
                        }
                    }
                }

            }
        }
        void controlValueShow()
        {
            
            //没有在编辑状态
            if (!IsInEdit)
            {
                if (showControl != null && showControl is Button)
                {
                    if (showValue == 0)
                    {
                        showControl.BackColor = System.Drawing.Color.Transparent;
                        if (showStr.Count > showValue)
                        {
                            showControl.Text = showStr[showValue];
                        }
                    }
                    else
                    {
                        if (showValue > 0)
                        {
                            showControl.BackColor = System.Drawing.Color.Red;
                            if (showStr.Count > showValue)
                            {
                                showControl.Text = showStr[showValue];
                            }
                        }
                    }
                }

                if (showControl != null && (showControl is TextBox || showControl is Label))
                {
                    if (IsParam)
                        ConstantMethod.
                        SetText(showControl, ((double)showValue / Constant.dataMultiple).ToString());
                    else
                        ConstantMethod.
                        SetText(showControl, showValue.ToString());
                    showControl.Text = showValue.ToString();

                }

            }
        }
        double showValueFloat;
        public double ShowValueFloat
        {
            get { return showValueFloat; }
            set { showValueFloat = value; }
        }
        int showValue;
        public int ShowValue //从表格读取数据回来
        {
            get
            {
                if(pTcpInfo !=null)
                {
                    //如果是浮点型数据 那就是4个字节 进行转换
                    //如果是整型数据 那一般是小于四个字节 下面那个参数5 就是这个意思
                    if (mode.Contains(Constant.REAL))
                    {
                        if (double.TryParse(pTcpInfo.PlcValue, out showValueFloat))
                        {
                            if (ration > 0)
                            {
                               showValueFloat = showValueFloat / ration;
                                
                            }
                            controlValueShow(0);
                        }                    
                    }
                    else
                    if ((ConstantMethod.getModeCount(mode)) < 5)
                    {

                        if (int.TryParse(pTcpInfo.PlcValue, out showValue))
                        {
                            if (ration > 0) {

                                showValueFloat = (double)showValue / ration;
                                showValue = (int)((double)showValue / ration);
                                
                            }
                            controlValueShow(1);
                        }                       
                    }
                }
                //这个是台达485 模式下的显示 
                if (pInfo != null)
                {
                    if (showValue != pInfo.PlcValue || showValue == 0)
                    {
                        showValue = pInfo.PlcValue;
                        controlValueShow();
                    }
                }                                       
                return showValue;
            }


            
        }
       
        int addr;
        public int Addr
        {
            get
            {             
                return addr;
            }
            set
            {
                //20190514
               //关于这里 需要考虑ES 地址不对的情况  ES情况下 XY 地址还是需要转换的 目前没用到 
               //只考虑到as DVP15mc的情况

               addr = value;

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


    //台达运动控制器DVP15MC DVP50MC
    public class DTTcpPlcInfo 
    {   
        
        int plcId =-1;
        public int PlcId
        {
            get { return plcId; }
            set { plcId = value; }
        }
        public DTTcpPlcInfo(int addr, string areaIn,string Mode)
        {
            this.relativeaddr = addr;

            this.StrArea = areaIn;
                      
            absAddr = DTTcpCmdPackAndDataUnpack.GetAbsAddrFromStr(relativeaddr, StrArea);
            intArea = DTTcpCmdPackAndDataUnpack.GetIntAreaFromStr(StrArea);
            ValueMode = Mode;


        }
        public DTTcpPlcInfo(int addr, string areaIn, string Mode,int plcId)
        {
            this.relativeaddr = addr;

            this.StrArea = areaIn;
            //区分 dvp15mc 和 As PLC
            if (plcId == Constant.xzjDeivceId)
            {
                intArea = XJPLCPackCmdAndDataUnpack.AreaGetFromStr(areaIn);
                absAddr = XJPLCPackCmdAndDataUnpack.RelAbsGet(addr, intArea,0);               

            }
            else
            {
                absAddr = DTTcpCmdPackAndDataUnpack.GetAbsAddrFromStr(relativeaddr, StrArea);
                intArea = DTTcpCmdPackAndDataUnpack.GetIntAreaFromStr(StrArea);
            }

            ValueMode = Mode;


        }
        public DTTcpPlcInfo()
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

        //单字还是双字 还是 4字 8字
        string valueMode;
        public string ValueMode
        {
            get { return valueMode; }
            set { valueMode = value; }
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
            set
            {
                maxValue = value;

            }
        }
        //数据是否正常
        public bool IsValueNormal
        {
            get
            {

                return true; //((plcValue > (minValue + 1)) && (plcValue < (maxValue + 1))) ? true : false;
            }

        }
       
        //如果是位的话 还有所在字节
        private Byte[] byteValue;
        public Byte[] ByteValue
        {
            get {

                return byteValue;
            }
            set
            {
                byteValue = new byte[value.Count()];
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
                absAddr = DTTcpCmdPackAndDataUnpack.GetAbsAddrFromInt(relativeaddr, intArea);// XJPLCPackCmdAndDataUnpack.AreaGetFromStr(relativeaddr, StrArea);
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

        //显示的值目前十进制
        private string plcValue="-1";
        /// <summary>
        /// PlcVALUE 需要计算 每次get比较麻烦
        /// </summary>
        public string PlcValue
        {
            get
            {
                if (ByteValue != null&& ByteValue.Count()>0)
                {

                        
                        plcValue =
                        ConstantMethod.getValueFromByte(valueMode, ByteValue);                                                         
                }                  
                return plcValue;
            }           
        }


    }

}
