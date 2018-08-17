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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.csvsplitCombox = new System.Windows.Forms.ComboBox();
            this.button3 = new System.Windows.Forms.Button();
            this.rtbResult = new System.Windows.Forms.RichTextBox();
            this.DialogExcelDataLoad = new System.Windows.Forms.OpenFileDialog();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.SuspendLayout();
            // 
            // dgv
            // 
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv.Location = new System.Drawing.Point(26, 91);
            this.dgv.Name = "dgv";
            this.dgv.RowTemplate.Height = 27;
            this.dgv.Size = new System.Drawing.Size(511, 391);
            this.dgv.TabIndex = 0;
            this.dgv.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_CellDoubleClick);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(26, 26);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(126, 36);
            this.button1.TabIndex = 1;
            this.button1.Text = "导入EXCEL文件";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(182, 26);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(135, 36);
            this.button2.TabIndex = 1;
            this.button2.Text = "导入CSV文件";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // csvsplitCombox
            // 
            this.csvsplitCombox.FormattingEnabled = true;
            this.csvsplitCombox.Items.AddRange(new object[] {
            ",",
            ";"});
            this.csvsplitCombox.Location = new System.Drawing.Point(347, 26);
            this.csvsplitCombox.Name = "csvsplitCombox";
            this.csvsplitCombox.Size = new System.Drawing.Size(121, 23);
            this.csvsplitCombox.TabIndex = 3;
            this.csvsplitCombox.Text = ",";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(570, 26);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(135, 36);
            this.button3.TabIndex = 1;
            this.button3.Text = "导出机器文件";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // rtbResult
            // 
            this.rtbResult.Location = new System.Drawing.Point(570, 91);
            this.rtbResult.Name = "rtbResult";
            this.rtbResult.Size = new System.Drawing.Size(300, 349);
            this.rtbResult.TabIndex = 4;
            this.rtbResult.Text = "";
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(347, 49);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(135, 36);
            this.button4.TabIndex = 1;
            this.button4.Text = "加载导出模板";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Visible = false;
            this.button4.Click += new System.EventHandler(this.button3_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(570, 446);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(135, 36);
            this.button5.TabIndex = 1;
            this.button5.Text = "设置导出模板";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(735, 446);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(135, 36);
            this.button6.TabIndex = 1;
            this.button6.Text = "保存配置文件";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // FormFileConvert
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(891, 512);
            this.Controls.Add(this.rtbResult);
            this.Controls.Add(this.csvsplitCombox);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.dgv);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormFileConvert";
            this.Text = "格式转换";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgv;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ComboBox csvsplitCombox;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.RichTextBox rtbResult;
        private System.Windows.Forms.OpenFileDialog DialogExcelDataLoad;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
    }
}

