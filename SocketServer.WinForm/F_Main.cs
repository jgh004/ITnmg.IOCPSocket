using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using IOCPSocket;

namespace SocketServer.WinForm
{
	public partial class F_Main : Form
	{
		SynchronizationContext sync;
		SocketServerManager server;
		bool isInit = false;

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
			this.server.ServerStatusChangeEvent += Server_ServerStateChangeEvent;
			this.server.ConnectedStatusChangeEvent += Server_ConnectedStatusChangeEvent;
		}

		private void bt_Init_Click( object sender, EventArgs e )
		{
			InitAsync();
		}

		private void bt_Start_Click( object sender, EventArgs e )
		{
			StartAsync();
		}

		private void bt_Stop_Click( object sender, EventArgs e )
		{
			try
			{
				SetBtns( false );
				server.StopAsync();
			}
			catch ( Exception ex )
			{
				SetBtns( false );
			}
		}

		private async Task InitAsync()
		{
			this.bt_Init.Enabled = false;
			int maxCount = Convert.ToInt32( this.tb_MaxConnection.Text.Trim() );

			await this.server.InitAsync( maxCount, maxCount, 4 * 1024, 100000, 100000 ).ContinueWith( f =>
			{
				if ( f.Exception == null )
				{
					this.isInit = true;
				}
				else
				{
					this.ShowMessageBox( "初始化失败: " + f.Exception.GetBaseException().Message );
				}

				this.AsyncPost( k =>
				{
					this.bt_Init.Enabled = true;
				}, "" );
			} );
		}

		private async Task StartAsync()
		{
			try
			{
				if ( !isInit )
				{
					await InitAsync();
				}

				SetBtns( true );
				string ip = this.tb_DomainOrIP.Text.Trim();
				int port = Convert.ToInt32( this.tb_Port.Text.Trim() );
				int maxConn = Convert.ToInt32( this.tb_MaxConnection.Text.Trim() );
				bool firstIPType = this.cob_FirsIPType.SelectedIndex == 0;

				await server.StartAsync( ip, port, firstIPType ).ContinueWith( f =>
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
					await server.StopAsync();
				}

				SetBtns( true );
			}
		}

		private void Server_ErrorEvent( object sender, Exception e )
		{
			WriteConsole( $"Error: {e.Message}" );
		}

		private void Server_ServerStateChangeEvent( object sender, bool e )
		{
			this.SetBtns( e );
			WriteConsole( $"Server: {(e ? "启动" : "停止")}" );
		}

		private void Server_ConnectedStatusChangeEvent( object sender, SocketStatusChangeArgs e )
		{
			AsyncPost( f =>
			{
				this.textBox1.Text = e.ConnectedCount.ToString();
			}, e );

			if ( e.Error != null )
			{
				WriteConsole( $"UserTokenId:{e.UserTokenId}, Status:{e.Status}, ConnCount:{e.ConnectedCount}, Error:{e.Error}" );
			}
		}


		private void SetBtns( bool start )
		{
			this.bt_Init.Enabled = !start;
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

		private void WriteConsole( string s )
		{
			this.AsyncPost( f =>
			{
				if ( this.tb_Console.Lines.Length > 20000 )
				{
					this.tb_Console.Text = "";
				}

				this.tb_Console.Text += (this.tb_Console.Text.Length > 0 ? "\r\n" : "") + s;
				this.tb_Console.SelectionStart = this.tb_Console.Text.Length;
			}, s );
		}
	}
}
