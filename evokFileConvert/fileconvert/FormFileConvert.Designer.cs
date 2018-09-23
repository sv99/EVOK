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
            this.导入CSV文件ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.导入CSVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.设置导出模板ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.保存配置文件ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.加载文件模板ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.barCodeButton = new System.Windows.Forms.ToolStripMenuItem();
            this.加载条码ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pBar1 = new System.Windows.Forms.ProgressBar();
            this.demoLoadDialog = new System.Windows.Forms.OpenFileDialog();
            this.printReport = new FastReport.Report();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.printReport)).BeginInit();
            this.SuspendLayout();
            // 
            // dgv
            // 
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv.Location = new System.Drawing.Point(12, 31);
            this.dgv.Name = "dgv";
            this.dgv.RowTemplate.Height = 27;
            this.dgv.Size = new System.Drawing.Size(909, 447);
            this.dgv.TabIndex = 0;
            this.dgv.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_CellDoubleClick);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(12, 484);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(909, 61);
            this.button3.TabIndex = 1;
            this.button3.Text = "导出机器文件";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // rtbResult
            // 
            this.rtbResult.Location = new System.Drawing.Point(927, 31);
            this.rtbResult.Name = "rtbResult";
            this.rtbResult.Size = new System.Drawing.Size(300, 507);
            this.rtbResult.TabIndex = 4;
            this.rtbResult.Text = "";
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.文件ToolStripMenuItem,
            this.设置ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1242, 28);
            this.menuStrip1.TabIndex = 5;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 文件ToolStripMenuItem
            // 
            this.文件ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.导入excel文件ToolStripMenuItem,
            this.导入CSV文件ToolStripMenuItem,
            this.导入CSVToolStripMenuItem});
            this.文件ToolStripMenuItem.Name = "文件ToolStripMenuItem";
            this.文件ToolStripMenuItem.Size = new System.Drawing.Size(49, 24);
            this.文件ToolStripMenuItem.Text = "文件";
            // 
            // 导入excel文件ToolStripMenuItem
            // 
            this.导入excel文件ToolStripMenuItem.Name = "导入excel文件ToolStripMenuItem";
            this.导入excel文件ToolStripMenuItem.Size = new System.Drawing.Size(174, 26);
            this.导入excel文件ToolStripMenuItem.Text = "导入excel文件";
            this.导入excel文件ToolStripMenuItem.Click += new System.EventHandler(this.导入excel文件ToolStripMenuItem_Click);
            // 
            // 导入CSV文件ToolStripMenuItem
            // 
            this.导入CSV文件ToolStripMenuItem.Name = "导入CSV文件ToolStripMenuItem";
            this.导入CSV文件ToolStripMenuItem.Size = new System.Drawing.Size(174, 26);
            this.导入CSV文件ToolStripMenuItem.Text = "导入CSV(,)";
            this.导入CSV文件ToolStripMenuItem.Click += new System.EventHandler(this.导入CSV文件ToolStripMenuItem_Click);
            // 
            // 导入CSVToolStripMenuItem
            // 
            this.导入CSVToolStripMenuItem.Name = "导入CSVToolStripMenuItem";
            this.导入CSVToolStripMenuItem.Size = new System.Drawing.Size(174, 26);
            this.导入CSVToolStripMenuItem.Text = "导入CSV(;)";
            this.导入CSVToolStripMenuItem.Click += new System.EventHandler(this.导入CSVToolStripMenuItem_Click);
            // 
            // 设置ToolStripMenuItem
            // 
            this.设置ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.设置导出模板ToolStripMenuItem,
            this.保存配置文件ToolStripMenuItem,
            this.加载文件模板ToolStripMenuItem,
            this.barCodeButton,
            this.加载条码ToolStripMenuItem});
            this.设置ToolStripMenuItem.Name = "设置ToolStripMenuItem";
            this.设置ToolStripMenuItem.Size = new System.Drawing.Size(49, 24);
            this.设置ToolStripMenuItem.Text = "设置";
            // 
            // 设置导出模板ToolStripMenuItem
            // 
            this.设置导出模板ToolStripMenuItem.Name = "设置导出模板ToolStripMenuItem";
            this.设置导出模板ToolStripMenuItem.Size = new System.Drawing.Size(168, 26);
            this.设置导出模板ToolStripMenuItem.Text = "设置导出模板";
            this.设置导出模板ToolStripMenuItem.Click += new System.EventHandler(this.设置导出模板ToolStripMenuItem_Click);
            // 
            // 保存配置文件ToolStripMenuItem
            // 
            this.保存配置文件ToolStripMenuItem.Name = "保存配置文件ToolStripMenuItem";
            this.保存配置文件ToolStripMenuItem.Size = new System.Drawing.Size(168, 26);
            this.保存配置文件ToolStripMenuItem.Text = "保存配置模板";
            this.保存配置文件ToolStripMenuItem.Click += new System.EventHandler(this.保存配置文件ToolStripMenuItem_Click);
            // 
            // 加载文件模板ToolStripMenuItem
            // 
            this.加载文件模板ToolStripMenuItem.Name = "加载文件模板ToolStripMenuItem";
            this.加载文件模板ToolStripMenuItem.Size = new System.Drawing.Size(168, 26);
            this.加载文件模板ToolStripMenuItem.Text = "加载文件模板";
            this.加载文件模板ToolStripMenuItem.Click += new System.EventHandler(this.加载文件模板ToolStripMenuItem_Click);
            // 
            // barCodeButton
            // 
            this.barCodeButton.Name = "barCodeButton";
            this.barCodeButton.Size = new System.Drawing.Size(168, 26);
            this.barCodeButton.Text = "查看条码模板";
            this.barCodeButton.Click += new System.EventHandler(this.查看条码模板ToolStripMenuItem_Click);
            // 
            // 加载条码ToolStripMenuItem
            // 
            this.加载条码ToolStripMenuItem.Name = "加载条码ToolStripMenuItem";
            this.加载条码ToolStripMenuItem.Size = new System.Drawing.Size(168, 26);
            this.加载条码ToolStripMenuItem.Text = "加载条码模板";
            this.加载条码ToolStripMenuItem.Click += new System.EventHandler(this.加载条码ToolStripMenuItem_Click);
            // 
            // pBar1
            // 
            this.pBar1.Location = new System.Drawing.Point(12, 551);
            this.pBar1.Name = "pBar1";
            this.pBar1.Size = new System.Drawing.Size(1218, 23);
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
            // FormFileConvert
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1242, 586);
            this.Controls.Add(this.pBar1);
            this.Controls.Add(this.rtbResult);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.dgv);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
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
        private System.Windows.Forms.ToolStripMenuItem 导入CSV文件ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 设置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 设置导出模板ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 保存配置文件ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 导入CSVToolStripMenuItem;
        private System.Windows.Forms.ProgressBar pBar1;
        private System.Windows.Forms.ToolStripMenuItem 加载文件模板ToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog demoLoadDialog;
        private FastReport.Report printReport;
        private System.Windows.Forms.ToolStripMenuItem barCodeButton;
        private System.Windows.Forms.ToolStripMenuItem 加载条码ToolStripMenuItem;
    }
}

