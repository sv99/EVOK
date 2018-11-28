//单个寄存器起始地址  个数最大值=62 不能超过62
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace xjplc
{


    public partial class FormMain0 : Form
    {
      
        XJDevice  device; 
   
        PortParam portparam ;

        CsvStreamReader CSVData;

        List<string> strDataFormPath;

        public FormMain0()
        {
            InitializeComponent();
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            strDataFormPath = new List<string>();
            strDataFormPath.Add(Constant.PlcDataFilePathAuto);
            strDataFormPath.Add(Constant.PlcDataFilePathHand);
            strDataFormPath.Add(Constant.PlcDataFilePathParam);
            CSVData = new CsvStreamReader();
            test();        

            return;
        }
        private void test()
        {
            portparam = new PortParam();
                   
            if (!ConstantMethod.XJFindPort())
            {
                MessageBox.Show( Constant.ConnectMachineFail);
                Application.Exit();
            }

            portparam = ConstantMethod.LoadPortParam(Constant.ConfigSerialportFilePath);

            device = new XJDevice(strDataFormPath);

            if (device.DataFormLst.Count > 0)
            {
               
                dgv.DataSource = device.DataFormLst[0];
              
                dataGridView2.DataSource = device.DataFormLst[1];
                
                dataGridView3.DataSource = device.DataFormLst[2];
                //dgvIO.DataSource = evokDevice.DataFormLst[2];
            }
            //device.SetShowDataGridView(dgv);
            //获取其他监控窗口的数据
            // device.GetPlcDataTableFromFile(strDataFormPath);

            if (device.getDeviceData())
            {              
                ConstantMethod.ShowInfo(rtbResult, "成功 ");
               // device.StartUpdateUI();
           
            }
            else
            {
                ConstantMethod.ShowInfo(rtbResult, "连接设备失败！ ");
            }
        }       
                    
        //01 19 00 02 03 13 86 00 04 13 10 00 04 3a 20
        private void button3_Click(object sender, EventArgs e)
        {
            // int i = Convert.ToInt32("8", 8);
            //测试D区域 单字 一个写 单字 多个写 双字 单个写 双字多个写
            int addr ;
            int[] value= new int[3];
            value[0] = 300;
            value[1] = 400;
            value[2] = 500;
            if (int.TryParse(txtd4880.Text, out addr) && int.TryParse(dvalue.Text, out value[0]))
                if (device.WriteMultiPleDMData(addr, value, "D","双字"))
                {
                    ConstantMethod.ShowInfo(rtbResult, "设置" + txtd4880.Text + "成功！");
                }                           

        }
       
        
        private void button1_Click(object sender, EventArgs e)
        {
           
            
        }
        public System.Timers.Timer PlcCheckTimer;



        private void button2_Click(object sender, EventArgs e)
        {
            PortParam p = ConstantMethod.LoadPortParam(Constant.ConfigSerialportFilePath);

            device.RestartConneect(device.DataFormLst[0]);

            if (device.getDeviceData())
            {                             
                ConstantMethod.ShowInfo(rtbResult, "成功 ");
                //device.StartUpdateUI();
               
            }
            else
            {
                ConstantMethod.ShowInfo(rtbResult, "连接设备失败！ ");
            }
            
            GC.Collect();
            GC.WaitForPendingFinalizers();


        }
      
        private void button4_Click(object sender, EventArgs e)
        {          

        }       
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(device !=null)
            device.DeviceShutDown();
            ConstantMethod.Delay(100);
        }                       

        private void button1_Click_1(object sender, EventArgs e)
        {        
            
            //写入M区域
            int addr;
            //多个M测试
           int[] value = new int[9];
            value[0] = 1;
            value[1] = 1;
            value[2] = 1;
            value[3] = 1;
            value[4] = 1;
            value[5] = 1;
            value[6] = 1;
            value[7] = 1;
            value[8] = 1;

           if (int.TryParse(bitAddr.Text, out addr))
               if (device.WriteMultiPleDMData(addr, value, arecb.Text,""))
               {
                   ConstantMethod.ShowInfo(rtbResult, "设置" + bitAddr.Text + "成功！");
               }
            
            /***单个M测试
             *  int[] value = new int[1];
            if (int.TryParse(bitAddr.Text, out addr) && (int.TryParse(bitvalue.Text, out value[0])))
                if (device.WriteMarea(addr, value.Count(), value, arecb.Text))
                {
                    ConstantMethod.ShowInfo(rtbResult, "设置" + bitAddr.Text + "成功！");
                }
                **/

        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            device.shiftDataForm(1);
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            device.shiftDataForm(2);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            device.shiftDataForm(0);
        }

        private void dataGridView1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)//回车键
            {
                string ID = dgv.SelectedCells[0].Value.ToString();
                rtbResult.AppendText(ID);
            }
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            string valueStr = dgv.SelectedCells[0].Value.ToString();
            int rowindex = dgv.SelectedCells[0].RowIndex;
            string userdata= dgv.Rows[rowindex].Cells["addr"].Value.ToString();
            int addr=0;
            string area="D";
            string mode= dgv.Rows[rowindex].Cells["mode"].Value.ToString();
            ConstantMethod.SplitAreaAndAddr(userdata, ref addr, ref area);          
            
            int valueInt;
            if (int.TryParse(valueStr, out valueInt))
            {
                if (XJPLCPackCmdAndDataUnpack.AreaGetFromStr(area) > -1 && XJPLCPackCmdAndDataUnpack.AreaGetFromStr(area) < Constant.M_ID)
                {
                    device.WriteSingleDData(addr, valueInt, area, mode);
                }
                else
                {
                    if (XJPLCPackCmdAndDataUnpack.AreaGetFromStr(area) > Constant.HD_ID && XJPLCPackCmdAndDataUnpack.AreaGetFromStr(area) < (Constant.Y_ID + 1))
                    {
                        if (valueInt == 1 || valueInt == 0)
                        {
                            device.WriteSingleMData(addr, valueInt, area, mode);
                        }
                        else MessageBox.Show(Constant.ErrorMValue);
                        
                    }
               

                }
            }          

        }

        private void dataGridView1_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            dgv.EndEdit();
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        private void dataGridView1_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            
        }

        private void dataGridView1_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
           
        }

        private void dataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            
        }
        
        private void button4_Click_2(object sender, EventArgs e)
        {
            //FindPort();
        }
    }
       
}
