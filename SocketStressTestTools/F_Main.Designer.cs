namespace SocketStressTestTools
{
    partial class F_Main
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose( bool disposing )
        {
            if ( disposing && (components != null) )
            {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(F_Main));
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.ts_Settings = new System.Windows.Forms.ToolStripButton();
			this.ts_SendData = new System.Windows.Forms.ToolStripButton();
			this.panel1 = new System.Windows.Forms.Panel();
			this.bt_Stop = new System.Windows.Forms.Button();
			this.bt_Start = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.tb_ConnectionCount = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.tb_Port = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.tb_IP = new System.Windows.Forms.TextBox();
			this.panel2 = new System.Windows.Forms.Panel();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.toolStrip1.SuspendLayout();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStrip1
			// 
			this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ts_Settings,
            this.ts_SendData});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(1098, 27);
			this.toolStrip1.TabIndex = 0;
			// 
			// ts_Settings
			// 
			this.ts_Settings.Image = ((System.Drawing.Image)(resources.GetObject("ts_Settings.Image")));
			this.ts_Settings.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.ts_Settings.Name = "ts_Settings";
			this.ts_Settings.Size = new System.Drawing.Size(93, 24);
			this.ts_Settings.Text = "Settings";
			// 
			// ts_SendData
			// 
			this.ts_SendData.Image = ((System.Drawing.Image)(resources.GetObject("ts_SendData.Image")));
			this.ts_SendData.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.ts_SendData.Name = "ts_SendData";
			this.ts_SendData.Size = new System.Drawing.Size(103, 24);
			this.ts_SendData.Text = "SendData";
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.bt_Stop);
			this.panel1.Controls.Add(this.bt_Start);
			this.panel1.Controls.Add(this.label3);
			this.panel1.Controls.Add(this.tb_ConnectionCount);
			this.panel1.Controls.Add(this.label2);
			this.panel1.Controls.Add(this.tb_Port);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Controls.Add(this.tb_IP);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 27);
			this.panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(1098, 54);
			this.panel1.TabIndex = 1;
			// 
			// bt_Stop
			// 
			this.bt_Stop.Location = new System.Drawing.Point(941, 7);
			this.bt_Stop.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.bt_Stop.Name = "bt_Stop";
			this.bt_Stop.Size = new System.Drawing.Size(84, 34);
			this.bt_Stop.TabIndex = 7;
			this.bt_Stop.Text = "Stop";
			this.bt_Stop.UseVisualStyleBackColor = true;
			this.bt_Stop.Click += new System.EventHandler(this.bt_Stop_Click);
			// 
			// bt_Start
			// 
			this.bt_Start.Location = new System.Drawing.Point(823, 7);
			this.bt_Start.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.bt_Start.Name = "bt_Start";
			this.bt_Start.Size = new System.Drawing.Size(84, 34);
			this.bt_Start.TabIndex = 6;
			this.bt_Start.Text = "Start";
			this.bt_Start.UseVisualStyleBackColor = true;
			this.bt_Start.Click += new System.EventHandler(this.bt_Start_Click);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(467, 14);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(145, 20);
			this.label3.TabIndex = 5;
			this.label3.Text = "Connection Count:";
			// 
			// tb_ConnectionCount
			// 
			this.tb_ConnectionCount.Location = new System.Drawing.Point(618, 11);
			this.tb_ConnectionCount.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.tb_ConnectionCount.Name = "tb_ConnectionCount";
			this.tb_ConnectionCount.Size = new System.Drawing.Size(128, 27);
			this.tb_ConnectionCount.TabIndex = 4;
			this.tb_ConnectionCount.Text = "2";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(296, 14);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(44, 20);
			this.label2.TabIndex = 3;
			this.label2.Text = "Port:";
			// 
			// tb_Port
			// 
			this.tb_Port.Location = new System.Drawing.Point(346, 11);
			this.tb_Port.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.tb_Port.Name = "tb_Port";
			this.tb_Port.Size = new System.Drawing.Size(82, 27);
			this.tb_Port.TabIndex = 2;
			this.tb_Port.Text = "9000";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 14);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(26, 20);
			this.label1.TabIndex = 1;
			this.label1.Text = "IP:";
			// 
			// tb_IP
			// 
			this.tb_IP.Location = new System.Drawing.Point(44, 11);
			this.tb_IP.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.tb_IP.Name = "tb_IP";
			this.tb_IP.Size = new System.Drawing.Size(213, 27);
			this.tb_IP.TabIndex = 0;
			this.tb_IP.Text = "127.0.0.1";
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.textBox1);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel2.Location = new System.Drawing.Point(0, 81);
			this.panel2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(1098, 515);
			this.panel2.TabIndex = 2;
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(151, 97);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(116, 27);
			this.textBox1.TabIndex = 0;
			// 
			// F_Main
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1098, 596);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.toolStrip1);
			this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.Name = "F_Main";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Socket Stress Test";
			this.Load += new System.EventHandler(this.F_Main_Load);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripButton ts_Settings;
        private System.Windows.Forms.Button bt_Start;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tb_ConnectionCount;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tb_Port;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tb_IP;
        private System.Windows.Forms.Button bt_Stop;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ToolStripButton ts_SendData;
		private System.Windows.Forms.TextBox textBox1;
	}
}

