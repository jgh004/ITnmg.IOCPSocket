namespace ITnmg.IOCPSocket.WinFormClient
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
			this.bt_Init = new System.Windows.Forms.Button();
			this.bt_Stop = new System.Windows.Forms.Button();
			this.bt_Start = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.tb_ConnectionCount = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.tb_Port = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.tb_DomainOrIP = new System.Windows.Forms.TextBox();
			this.panel2 = new System.Windows.Forms.Panel();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.tb_Console = new System.Windows.Forms.TextBox();
			this.toolStrip1.SuspendLayout();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
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
			this.toolStrip1.Size = new System.Drawing.Size(1182, 27);
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
			this.panel1.Controls.Add(this.bt_Init);
			this.panel1.Controls.Add(this.bt_Stop);
			this.panel1.Controls.Add(this.bt_Start);
			this.panel1.Controls.Add(this.label3);
			this.panel1.Controls.Add(this.tb_ConnectionCount);
			this.panel1.Controls.Add(this.label2);
			this.panel1.Controls.Add(this.tb_Port);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Controls.Add(this.tb_DomainOrIP);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 27);
			this.panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(1182, 54);
			this.panel1.TabIndex = 1;
			// 
			// bt_Init
			// 
			this.bt_Init.Location = new System.Drawing.Point(770, 9);
			this.bt_Init.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.bt_Init.Name = "bt_Init";
			this.bt_Init.Size = new System.Drawing.Size(84, 31);
			this.bt_Init.TabIndex = 106;
			this.bt_Init.Text = "Init";
			this.bt_Init.UseVisualStyleBackColor = true;
			this.bt_Init.Click += new System.EventHandler(this.bt_Init_Click);
			// 
			// bt_Stop
			// 
			this.bt_Stop.Location = new System.Drawing.Point(988, 7);
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
			this.bt_Start.Location = new System.Drawing.Point(879, 7);
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
			// tb_DomainOrIP
			// 
			this.tb_DomainOrIP.Location = new System.Drawing.Point(44, 11);
			this.tb_DomainOrIP.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.tb_DomainOrIP.Name = "tb_DomainOrIP";
			this.tb_DomainOrIP.Size = new System.Drawing.Size(213, 27);
			this.tb_DomainOrIP.TabIndex = 0;
			this.tb_DomainOrIP.Text = "127.0.0.1";
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.splitContainer1);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel2.Location = new System.Drawing.Point(0, 81);
			this.panel2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(1182, 552);
			this.panel2.TabIndex = 2;
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.textBox1);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.tb_Console);
			this.splitContainer1.Size = new System.Drawing.Size(1182, 552);
			this.splitContainer1.SplitterDistance = 360;
			this.splitContainer1.TabIndex = 108;
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(59, 26);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(119, 27);
			this.textBox1.TabIndex = 104;
			// 
			// tb_Console
			// 
			this.tb_Console.BackColor = System.Drawing.Color.Black;
			this.tb_Console.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tb_Console.ForeColor = System.Drawing.Color.White;
			this.tb_Console.Location = new System.Drawing.Point(0, 0);
			this.tb_Console.Multiline = true;
			this.tb_Console.Name = "tb_Console";
			this.tb_Console.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.tb_Console.Size = new System.Drawing.Size(1182, 188);
			this.tb_Console.TabIndex = 0;
			// 
			// F_Main
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1182, 633);
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
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
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
        private System.Windows.Forms.TextBox tb_DomainOrIP;
        private System.Windows.Forms.Button bt_Stop;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ToolStripButton ts_SendData;
		private System.Windows.Forms.Button bt_Init;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.TextBox tb_Console;
	}
}

