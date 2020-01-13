namespace fileconvert
{
    partial class FormFileConvert
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormFileConvert));
            this.dgv = new System.Windows.Forms.DataGridView();
            this.button3 = new System.Windows.Forms.Button();
            this.rtbResult = new System.Windows.Forms.RichTextBox();
            this.DialogExcelDataLoad = new System.Windows.Forms.OpenFileDialog();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.文件ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.导入excel文件ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.csv1 = new System.Windows.Forms.ToolStripMenuItem();
            this.csv2 = new System.Windows.Forms.ToolStripMenuItem();
            this.menu = new System.Windows.Forms.ToolStripMenuItem();
            this.设置导出模板ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.保存配置文件ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.加载文件模板ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.barCodeButton = new System.Windows.Forms.ToolStripMenuItem();
            this.加载条码ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pBar1 = new System.Windows.Forms.ProgressBar();
            this.demoLoadDialog = new System.Windows.Forms.OpenFileDialog();
            this.printReport = new FastReport.Report();
            this.button1 = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.listBox2 = new System.Windows.Forms.ListBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.printReport)).BeginInit();
            this.SuspendLayout();
            // 
            // dgv
            // 
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv.Location = new System.Drawing.Point(9, 53);
            this.dgv.Margin = new System.Windows.Forms.Padding(2);
            this.dgv.Name = "dgv";
            this.dgv.RowTemplate.Height = 27;
            this.dgv.Size = new System.Drawing.Size(548, 330);
            this.dgv.TabIndex = 0;
            this.dgv.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_CellDoubleClick);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(9, 387);
            this.button3.Margin = new System.Windows.Forms.Padding(2);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(548, 49);
            this.button3.TabIndex = 1;
            this.button3.Text = "导出机器文件";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // rtbResult
            // 
            this.rtbResult.Location = new System.Drawing.Point(562, 25);
            this.rtbResult.Margin = new System.Windows.Forms.Padding(2);
            this.rtbResult.Name = "rtbResult";
            this.rtbResult.Size = new System.Drawing.Size(226, 434);
            this.rtbResult.TabIndex = 4;
            this.rtbResult.Text = "";
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.文件ToolStripMenuItem,
            this.menu});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(1099, 25);
            this.menuStrip1.TabIndex = 5;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 文件ToolStripMenuItem
            // 
            this.文件ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.导入excel文件ToolStripMenuItem,
            this.csv1,
            this.csv2});
            this.文件ToolStripMenuItem.Name = "文件ToolStripMenuItem";
            this.文件ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.文件ToolStripMenuItem.Text = "文件";
            // 
            // 导入excel文件ToolStripMenuItem
            // 
            this.导入excel文件ToolStripMenuItem.Name = "导入excel文件ToolStripMenuItem";
            this.导入excel文件ToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.导入excel文件ToolStripMenuItem.Text = "导入excel文件";
            this.导入excel文件ToolStripMenuItem.Click += new System.EventHandler(this.导入excel文件ToolStripMenuItem_Click);
            // 
            // csv1
            // 
            this.csv1.Name = "csv1";
            this.csv1.Size = new System.Drawing.Size(153, 22);
            this.csv1.Text = "导入CSV(,)";
            this.csv1.Click += new System.EventHandler(this.导入CSV文件ToolStripMenuItem_Click);
            // 
            // csv2
            // 
            this.csv2.Name = "csv2";
            this.csv2.Size = new System.Drawing.Size(153, 22);
            this.csv2.Text = "导入CSV(;)";
            this.csv2.Click += new System.EventHandler(this.导入CSVToolStripMenuItem_Click);
            // 
            // menu
            // 
            this.menu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.设置导出模板ToolStripMenuItem,
            this.保存配置文件ToolStripMenuItem,
            this.加载文件模板ToolStripMenuItem,
            this.barCodeButton,
            this.加载条码ToolStripMenuItem});
            this.menu.Name = "menu";
            this.menu.Size = new System.Drawing.Size(44, 21);
            this.menu.Text = "设置";
            // 
            // 设置导出模板ToolStripMenuItem
            // 
            this.设置导出模板ToolStripMenuItem.Name = "设置导出模板ToolStripMenuItem";
            this.设置导出模板ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.设置导出模板ToolStripMenuItem.Text = "设置导出模板";
            this.设置导出模板ToolStripMenuItem.Click += new System.EventHandler(this.设置导出模板ToolStripMenuItem_Click);
            // 
            // 保存配置文件ToolStripMenuItem
            // 
            this.保存配置文件ToolStripMenuItem.Name = "保存配置文件ToolStripMenuItem";
            this.保存配置文件ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.保存配置文件ToolStripMenuItem.Text = "保存配置模板";
            this.保存配置文件ToolStripMenuItem.Click += new System.EventHandler(this.保存配置文件ToolStripMenuItem_Click);
            // 
            // 加载文件模板ToolStripMenuItem
            // 
            this.加载文件模板ToolStripMenuItem.Name = "加载文件模板ToolStripMenuItem";
            this.加载文件模板ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.加载文件模板ToolStripMenuItem.Text = "加载文件模板";
            this.加载文件模板ToolStripMenuItem.Click += new System.EventHandler(this.加载文件模板ToolStripMenuItem_Click);
            // 
            // barCodeButton
            // 
            this.barCodeButton.Name = "barCodeButton";
            this.barCodeButton.Size = new System.Drawing.Size(148, 22);
            this.barCodeButton.Text = "查看条码模板";
            this.barCodeButton.Click += new System.EventHandler(this.查看条码模板ToolStripMenuItem_Click);
            // 
            // 加载条码ToolStripMenuItem
            // 
            this.加载条码ToolStripMenuItem.Name = "加载条码ToolStripMenuItem";
            this.加载条码ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.加载条码ToolStripMenuItem.Text = "加载条码模板";
            this.加载条码ToolStripMenuItem.Click += new System.EventHandler(this.加载条码ToolStripMenuItem_Click);
            // 
            // pBar1
            // 
            this.pBar1.Location = new System.Drawing.Point(9, 441);
            this.pBar1.Margin = new System.Windows.Forms.Padding(2);
            this.pBar1.Name = "pBar1";
            this.pBar1.Size = new System.Drawing.Size(548, 18);
            this.pBar1.TabIndex = 6;
            // 
            // demoLoadDialog
            // 
            this.demoLoadDialog.FileName = "openFileDialog1";
            // 
            // printReport
            // 
            this.printReport.ReportResourceString = resources.GetString("printReport.ReportResourceString");
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(791, 25);
            this.button1.Margin = new System.Windows.Forms.Padding(2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(74, 43);
            this.button1.TabIndex = 7;
            this.button1.Text = "撤销";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 12;
            this.listBox1.Items.AddRange(new object[] {
            "尺寸",
            "设定数量",
            "条码",
            "参数1",
            "参数2",
            "参数3",
            "参数4",
            "参数5",
            "参数6",
            "参数7",
            "参数8",
            "参数9",
            "参数10",
            "参数11",
            "参数12",
            "参数13",
            "参数14",
            "参数15",
            "参数16",
            "参数17",
            "参数18",
            "参数19",
            "参数20",
            "参数21",
            "参数22",
            "参数23",
            "参数24",
            "参数25",
            "参数26",
            "参数27",
            "参数28",
            "参数29",
            "参数30",
            "参数31",
            "参数32",
            "参数33",
            "参数34"});
            this.listBox1.Location = new System.Drawing.Point(869, 25);
            this.listBox1.Margin = new System.Windows.Forms.Padding(2);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(101, 436);
            this.listBox1.TabIndex = 8;
            // 
            // listBox2
            // 
            this.listBox2.FormattingEnabled = true;
            this.listBox2.ItemHeight = 12;
            this.listBox2.Location = new System.Drawing.Point(974, 25);
            this.listBox2.Margin = new System.Windows.Forms.Padding(2);
            this.listBox2.Name = "listBox2";
            this.listBox2.Size = new System.Drawing.Size(109, 436);
            this.listBox2.TabIndex = 8;
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(9, 28);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(339, 20);
            this.comboBox1.TabIndex = 9;
            this.comboBox1.Text = "1";
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // FormFileConvert
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1099, 486);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.listBox2);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.pBar1);
            this.Controls.Add(this.rtbResult);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.dgv);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FormFileConvert";
            this.Text = "意利欧设备文件格式转换";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.FormFileConvert_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.printReport)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgv;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.RichTextBox rtbResult;
        private System.Windows.Forms.OpenFileDialog DialogExcelDataLoad;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 文件ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 导入excel文件ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem csv1;
        private System.Windows.Forms.ToolStripMenuItem menu;
        private System.Windows.Forms.ToolStripMenuItem 设置导出模板ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 保存配置文件ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem csv2;
        private System.Windows.Forms.ProgressBar pBar1;
        private System.Windows.Forms.ToolStripMenuItem 加载文件模板ToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog demoLoadDialog;
        private FastReport.Report printReport;
        private System.Windows.Forms.ToolStripMenuItem barCodeButton;
        private System.Windows.Forms.ToolStripMenuItem 加载条码ToolStripMenuItem;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.ListBox listBox2;
        private System.Windows.Forms.ComboBox comboBox1;
    }
}

