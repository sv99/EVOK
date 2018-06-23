namespace xjplc
{
    partial class FormMain0
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbFrequency = new System.Windows.Forms.TextBox();
            this.rtbResult = new System.Windows.Forms.RichTextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.cbPortList = new System.Windows.Forms.ComboBox();
            this.dgv = new System.Windows.Forms.DataGridView();
            this.bitAddr = new System.Windows.Forms.TextBox();
            this.arecb = new System.Windows.Forms.ComboBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.txtd4880 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.dvalue = new System.Windows.Forms.TextBox();
            this.bitvalue = new System.Windows.Forms.TextBox();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.dataGridView3 = new System.Windows.Forms.DataGridView();
            this.button3 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView3)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "串口";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "频率";
            // 
            // tbFrequency
            // 
            this.tbFrequency.Location = new System.Drawing.Point(55, 35);
            this.tbFrequency.Name = "tbFrequency";
            this.tbFrequency.Size = new System.Drawing.Size(112, 25);
            this.tbFrequency.TabIndex = 4;
            this.tbFrequency.Text = "19200";
            // 
            // rtbResult
            // 
            this.rtbResult.Location = new System.Drawing.Point(12, 567);
            this.rtbResult.Name = "rtbResult";
            this.rtbResult.Size = new System.Drawing.Size(124, 300);
            this.rtbResult.TabIndex = 5;
            this.rtbResult.Text = "";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(15, 66);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(152, 60);
            this.button2.TabIndex = 0;
            this.button2.Text = "重新连接";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(18, 161);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(121, 36);
            this.button7.TabIndex = 0;
            this.button7.Text = "发送";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button3_Click);
            // 
            // cbPortList
            // 
            this.cbPortList.FormattingEnabled = true;
            this.cbPortList.Location = new System.Drawing.Point(55, 9);
            this.cbPortList.Name = "cbPortList";
            this.cbPortList.Size = new System.Drawing.Size(112, 23);
            this.cbPortList.TabIndex = 6;
            // 
            // dgv
            // 
            this.dgv.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv.Location = new System.Drawing.Point(173, 9);
            this.dgv.Name = "dgv";
            this.dgv.RowTemplate.Height = 27;
            this.dgv.Size = new System.Drawing.Size(545, 410);
            this.dgv.TabIndex = 7;
            this.dgv.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellEndEdit);
            this.dgv.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellEnter);
            this.dgv.CellLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellLeave);
            this.dgv.CurrentCellDirtyStateChanged += new System.EventHandler(this.dataGridView1_CurrentCellDirtyStateChanged);
            this.dgv.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView1_DataError);
            this.dgv.RowStateChanged += new System.Windows.Forms.DataGridViewRowStateChangedEventHandler(this.dataGridView1_RowStateChanged);
            this.dgv.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.dataGridView1_KeyPress);
            // 
            // bitAddr
            // 
            this.bitAddr.Location = new System.Drawing.Point(15, 479);
            this.bitAddr.Name = "bitAddr";
            this.bitAddr.Size = new System.Drawing.Size(121, 25);
            this.bitAddr.TabIndex = 8;
            this.bitAddr.Text = "0";
            // 
            // arecb
            // 
            this.arecb.FormattingEnabled = true;
            this.arecb.Items.AddRange(new object[] {
            "X",
            "Y",
            "M",
            "HM"});
            this.arecb.Location = new System.Drawing.Point(15, 440);
            this.arecb.Name = "arecb";
            this.arecb.Size = new System.Drawing.Size(121, 23);
            this.arecb.TabIndex = 9;
            this.arecb.Text = "Y";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "D",
            "HD"});
            this.comboBox1.Location = new System.Drawing.Point(18, 217);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 23);
            this.comboBox1.TabIndex = 9;
            this.comboBox1.Text = "D";
            // 
            // txtd4880
            // 
            this.txtd4880.Location = new System.Drawing.Point(18, 256);
            this.txtd4880.Name = "txtd4880";
            this.txtd4880.Size = new System.Drawing.Size(118, 25);
            this.txtd4880.TabIndex = 8;
            this.txtd4880.Text = "30";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(15, 387);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(124, 32);
            this.button1.TabIndex = 0;
            this.button1.Text = "发送";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // dvalue
            // 
            this.dvalue.Location = new System.Drawing.Point(18, 287);
            this.dvalue.Name = "dvalue";
            this.dvalue.Size = new System.Drawing.Size(118, 25);
            this.dvalue.TabIndex = 8;
            this.dvalue.Text = "30";
            // 
            // bitvalue
            // 
            this.bitvalue.Location = new System.Drawing.Point(15, 510);
            this.bitvalue.Name = "bitvalue";
            this.bitvalue.Size = new System.Drawing.Size(121, 25);
            this.bitvalue.TabIndex = 8;
            this.bitvalue.Text = "0";
            // 
            // dataGridView2
            // 
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Location = new System.Drawing.Point(771, 12);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.RowTemplate.Height = 27;
            this.dataGridView2.Size = new System.Drawing.Size(537, 247);
            this.dataGridView2.TabIndex = 10;
            // 
            // dataGridView3
            // 
            this.dataGridView3.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView3.Location = new System.Drawing.Point(771, 287);
            this.dataGridView3.Name = "dataGridView3";
            this.dataGridView3.RowTemplate.Height = 27;
            this.dataGridView3.Size = new System.Drawing.Size(545, 247);
            this.dataGridView3.TabIndex = 10;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(771, 637);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(102, 51);
            this.button3.TabIndex = 11;
            this.button3.Text = "切换表格1";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click_1);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(771, 705);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(102, 51);
            this.button5.TabIndex = 11;
            this.button5.Text = "切换表格2";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(771, 567);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(102, 51);
            this.button6.TabIndex = 11;
            this.button6.Text = "切换表格0";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(173, 439);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(110, 42);
            this.button4.TabIndex = 12;
            this.button4.Text = "button4";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click_2);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1320, 901);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.dataGridView3);
            this.Controls.Add(this.dataGridView2);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.arecb);
            this.Controls.Add(this.dvalue);
            this.Controls.Add(this.txtd4880);
            this.Controls.Add(this.bitAddr);
            this.Controls.Add(this.bitvalue);
            this.Controls.Add(this.dgv);
            this.Controls.Add(this.cbPortList);
            this.Controls.Add(this.rtbResult);
            this.Controls.Add(this.tbFrequency);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button7);
            this.Name = "Form1";
            this.Text = "20180604";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView3)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbFrequency;
        private System.Windows.Forms.RichTextBox rtbResult;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.ComboBox cbPortList;
        private System.Windows.Forms.DataGridView dgv;
        private System.Windows.Forms.TextBox bitAddr;
        private System.Windows.Forms.ComboBox arecb;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.TextBox txtd4880;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox dvalue;
        private System.Windows.Forms.TextBox bitvalue;
        private System.Windows.Forms.DataGridView dataGridView2;
        private System.Windows.Forms.DataGridView dataGridView3;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button4;
    }
}

