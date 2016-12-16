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
			this.cob_FirsIPType.SelectedIndex = 0;
			this.server = new SocketServerManager();
			this.server.ErrorEvent += Server_ErrorEvent;
			this.server.ServerStateChangeEvent += Server_ServerStateChangeEvent;
		}

		private void bt_Start_Click( object sender, EventArgs e )
		{
			try
			{
				SetBtns( true );
				string ip = this.tb_DomainOrIP.Text.Trim();
				int port = Convert.ToInt32( this.tb_Port.Text.Trim() );
				int maxConn = Convert.ToInt32( this.tb_MaxConnection.Text.Trim() );
				bool firstIPType = this.cob_FirsIPType.SelectedIndex == 0;

				server.Init( ip, port, maxConn, maxConn / 2, 16 * 1024, 10000, 10000, firstIPType );
				server.StartListen().ContinueWith( f =>
				{
					if ( f.IsCompleted )
					{
						this.AsyncPost( k =>
						{
							this.tb_DomainOrIP.Text = k == null ? "" : k.Address.ToString();
							SetBtns( true );
						}, f.Result );
					}
					else
					{
						SetBtns( false );
					}
				} );
			}
			catch ( Exception ex )
			{
				if ( server != null )
				{
					server.Close();
				}

				SetBtns( true );
			}
		}

		private void bt_Stop_Click( object sender, EventArgs e )
		{
			try
			{
				SetBtns( false );
				server.Close();
			}
			catch ( Exception ex )
			{
				SetBtns( false );
			}
		}

		private void Server_ErrorEvent( object sender, Exception e )
		{
			ShowMessageBox( e.Message );
		}

		private void Server_ServerStateChangeEvent( object sender, bool e )
		{
			this.SetBtns( e );
		}


		private void SetBtns( bool start )
		{
			this.bt_Start.Enabled = !start;
			this.bt_Stop.Enabled = start;
		}

		private void ShowMessageBox( string msg )
		{
			this.AsyncPost( f =>
			{
				MessageBox.Show( msg );
			}, msg );
		}
		
		private void AsyncPost<T>( Action<T> method, T state )
		{
			if ( this.sync != null )
			{
				this.sync.Post( f =>
				{
					method( (T)f );
				}, state );
			}
		}
	}
}
