namespace evokNewXJ
{
    partial class MSizeForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MSizeForm));
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.skinButton18 = new CCWin.SkinControl.SkinButton();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox1.Location = new System.Drawing.Point(196, 42);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(143, 31);
            this.textBox1.TabIndex = 0;
            // 
            // skinButton18
            // 
            this.skinButton18.BackColor = System.Drawing.Color.Transparent;
            this.skinButton18.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.skinButton18.DownBack = null;
            this.skinButton18.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.skinButton18.ForeColor = System.Drawing.Color.Black;
            this.skinButton18.Location = new System.Drawing.Point(139, 102);
            this.skinButton18.MouseBack = null;
            this.skinButton18.Name = "skinButton18";
            this.skinButton18.NormlBack = null;
            this.skinButton18.Size = new System.Drawing.Size(122, 46);
            this.skinButton18.TabIndex = 9;
            this.skinButton18.Text = "确认修改";
            this.skinButton18.UseVisualStyleBackColor = false;
            this.skinButton18.Click += new System.EventHandler(this.skinButton18_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Location = new System.Drawing.Point(75, 42);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(106, 24);
            this.label5.TabIndex = 10;
            this.label5.Text = "原料长度";
            // 
            // MSizeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::evokNewXJ.Properties.Resources.bk;
            this.ClientSize = new System.Drawing.Size(408, 164);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.skinButton18);
            this.Controls.Add(this.textBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MSizeForm";
            this.Text = "原料尺寸确认";
            this.VisibleChanged += new System.EventHandler(this.MSizeForm_VisibleChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private CCWin.SkinControl.SkinButton skinButton18;
        private System.Windows.Forms.Label label5;
    }
}