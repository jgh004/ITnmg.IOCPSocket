namespace SocketServer.WinForm
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
			this.tb_ConnectedCount = new System.Windows.Forms.TextBox();
			this.button1 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// tb_ConnectedCount
			// 
			this.tb_ConnectedCount.Location = new System.Drawing.Point(68, 64);
			this.tb_ConnectedCount.Name = "tb_ConnectedCount";
			this.tb_ConnectedCount.Size = new System.Drawing.Size(100, 25);
			this.tb_ConnectedCount.TabIndex = 0;
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(460, 63);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 1;
			this.button1.Text = "button1";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// F_Main
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(725, 414);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.tb_ConnectedCount);
			this.Name = "F_Main";
			this.Text = "Socket Server";
			this.Load += new System.EventHandler(this.F_Main_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox tb_ConnectedCount;
		private System.Windows.Forms.Button button1;
	}
}

