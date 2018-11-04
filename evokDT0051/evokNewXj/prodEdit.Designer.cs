namespace evokNewXJ
{
    partial class prodEdit
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(prodEdit));
            this.UserData = new System.Windows.Forms.DataGridView();
            this.bx = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.hy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.printcb = new System.Windows.Forms.ComboBox();
            this.textBox21 = new System.Windows.Forms.TextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.stbtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.UserData)).BeginInit();
            this.SuspendLayout();
            // 
            // UserData
            // 
            this.UserData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.UserData.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.bx,
            this.sc,
            this.hy});
            this.UserData.Location = new System.Drawing.Point(30, 69);
            this.UserData.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.UserData.Name = "UserData";
            this.UserData.RowTemplate.Height = 23;
            this.UserData.Size = new System.Drawing.Size(403, 499);
            this.UserData.TabIndex = 117;
            // 
            // bx
            // 
            this.bx.HeaderText = "步序";
            this.bx.Name = "bx";
            // 
            // sc
            // 
            this.sc.HeaderText = "锁槽工位";
            this.sc.Name = "sc";
            // 
            // hy
            // 
            this.hy.HeaderText = "合页工位";
            this.hy.Name = "hy";
            // 
            // printcb
            // 
            this.printcb.Font = new System.Drawing.Font("宋体", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.printcb.FormattingEnabled = true;
            this.printcb.Location = new System.Drawing.Point(195, 15);
            this.printcb.Name = "printcb";
            this.printcb.Size = new System.Drawing.Size(164, 35);
            this.printcb.TabIndex = 127;
            // 
            // textBox21
            // 
            this.textBox21.Font = new System.Drawing.Font("宋体", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox21.Location = new System.Drawing.Point(619, 69);
            this.textBox21.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.textBox21.Name = "textBox21";
            this.textBox21.Size = new System.Drawing.Size(112, 38);
            this.textBox21.TabIndex = 129;
            this.textBox21.Tag = "原料长度";
            this.textBox21.Text = "100";
            this.textBox21.TextChanged += new System.EventHandler(this.textBox21_TextChanged);
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Font = new System.Drawing.Font("宋体", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label24.Location = new System.Drawing.Point(36, 17);
            this.label24.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(152, 28);
            this.label24.TabIndex = 128;
            this.label24.Text = "程序编号：";
            // 
            // stbtn
            // 
            this.stbtn.Font = new System.Drawing.Font("宋体", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.stbtn.Location = new System.Drawing.Point(478, 202);
            this.stbtn.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.stbtn.Name = "stbtn";
            this.stbtn.Size = new System.Drawing.Size(164, 52);
            this.stbtn.TabIndex = 130;
            this.stbtn.Tag = "H1000";
            this.stbtn.Text = "H1000";
            this.stbtn.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(473, 69);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(138, 28);
            this.label1.TabIndex = 131;
            this.label1.Text = "工进速度:";
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("宋体", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox1.Location = new System.Drawing.Point(619, 115);
            this.textBox1.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(112, 38);
            this.textBox1.TabIndex = 129;
            this.textBox1.Tag = "原料长度";
            this.textBox1.Text = "100";
            this.textBox1.TextChanged += new System.EventHandler(this.textBox21_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(473, 118);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(138, 28);
            this.label2.TabIndex = 131;
            this.label2.Text = "加工速度:";
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("宋体", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button1.Location = new System.Drawing.Point(671, 202);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(164, 52);
            this.button1.TabIndex = 130;
            this.button1.Tag = "H1001";
            this.button1.Text = "H1001";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("宋体", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button2.Location = new System.Drawing.Point(478, 273);
            this.button2.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(164, 52);
            this.button2.TabIndex = 130;
            this.button2.Tag = "H1002";
            this.button2.Text = "H1002";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("宋体", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button3.Location = new System.Drawing.Point(671, 273);
            this.button3.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(164, 52);
            this.button3.TabIndex = 130;
            this.button3.Tag = "H1003";
            this.button3.Text = "H1003";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Font = new System.Drawing.Font("宋体", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button4.Location = new System.Drawing.Point(478, 350);
            this.button4.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(164, 52);
            this.button4.TabIndex = 130;
            this.button4.Tag = "H1004";
            this.button4.Text = "H1004";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(739, 69);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(96, 28);
            this.label3.TabIndex = 131;
            this.label3.Text = "(mm/s)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(739, 118);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(96, 28);
            this.label4.TabIndex = 131;
            this.label4.Text = "(mm/s)";
            // 
            // prodEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(849, 615);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.stbtn);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.printcb);
            this.Controls.Add(this.textBox21);
            this.Controls.Add(this.label24);
            this.Controls.Add(this.UserData);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "prodEdit";
            this.Text = "生产程序编辑";
            ((System.ComponentModel.ISupportInitialize)(this.UserData)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView UserData;
        private System.Windows.Forms.DataGridViewTextBoxColumn bx;
        private System.Windows.Forms.DataGridViewTextBoxColumn sc;
        private System.Windows.Forms.DataGridViewTextBoxColumn hy;
        private System.Windows.Forms.ComboBox printcb;
        private System.Windows.Forms.TextBox textBox21;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Button stbtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}