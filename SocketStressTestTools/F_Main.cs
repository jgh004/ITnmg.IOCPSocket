using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SocketStressTest;

namespace SocketStressTestTools
{
	public partial class F_Main : Form
	{
		private SynchronizationContext sync = null;
		SocketClientManager clientManager;

		public F_Main()
		{
			InitializeComponent();
		}

		private void F_Main_Load( object sender, EventArgs e )
		{
			this.sync = SynchronizationContext.Current;
			clientManager = new SocketClientManager();
			clientManager.ConnectedStatusChangeEvent += ( a, b ) =>
			{
				this.sync.Post( f =>
				{
					this.textBox1.Text = b.ConnectedCount.ToString();
				}, b );
			};
		}

		private void bt_Start_Click( object sender, EventArgs e )
		{
			var max = Convert.ToInt32( this.tb_ConnectionCount.Text.Trim() );
			clientManager.InitAsync( max, max, 8 * 1024, 10000, 10000 ).ContinueWith( m =>
			{
				clientManager.StartAsync( this.tb_IP.Text.Trim(), Convert.ToInt32( this.tb_Port.Text.Trim() ) ).Wait();
			} );
		}

		private void bt_Stop_Click( object sender, EventArgs e )
		{
			if ( clientManager != null )
			{
				clientManager.StopAsync();
			}
		}
	}
}
