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
using SocketServer;

namespace SocketServer.WinForm
{
	public partial class F_Main : Form
	{
		SynchronizationContext sync = null;
		SocketServerManager server = null;

		public F_Main()
		{
			InitializeComponent();
		}

		private void F_Main_Load( object sender, EventArgs e )
		{
			sync = SynchronizationContext.Current;
			this.server = new SocketServerManager();

		}

		private void button1_Click( object sender, EventArgs e )
		{
			server.Init( "localhost", 9000, 100, 32 * 1024 );
			server.StartListen().ContinueWith( f =>
			{
				if ( f.IsFaulted )
				{
				}
			} );
		}
	}
}
