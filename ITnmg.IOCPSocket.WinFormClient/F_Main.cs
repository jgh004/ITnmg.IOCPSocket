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

namespace ITnmg.IOCPSocket.WinFormClient
{
	public partial class F_Main : Form
	{
		private SynchronizationContext sync;
		SocketClientManager clientManager;
		bool isInit = false;

		public F_Main()
		{
			InitializeComponent();
		}

		private void F_Main_Load( object sender, EventArgs e )
		{
			this.sync = SynchronizationContext.Current;
			clientManager = new SocketClientManager();
			clientManager.ErrorEvent += ClientManager_ErrorEvent;
			clientManager.ClientManagerStatusChangeEvent += ClientManager_ClientManagerStatusChangeEvent;
			clientManager.ConnectedStatusChangeEvent += ClientManager_ConnectedStatusChangeEvent;
			;
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
				clientManager.StopAsync();
			}
			catch ( Exception ex )
			{
				SetBtns( false );
			}
		}

		private async Task InitAsync()
		{
			var max = Convert.ToInt32( this.tb_ConnectionCount.Text.Trim() );
			await clientManager.InitAsync( max, max, new TextProtocol(), 4 * 1024, 10000, 10000 ).ContinueWith( f =>
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
				await clientManager.StartAsync( this.tb_DomainOrIP.Text.Trim(), Convert.ToInt32( this.tb_Port.Text.Trim() ) ).ContinueWith( f =>
				{
					if ( !f.IsCompleted )
					{
						SetBtns( false );
					}
				} );

			}
			catch ( Exception ex )
			{
				if ( clientManager != null )
				{
					await clientManager.StopAsync();
				}

				SetBtns( true );
			}
		}

		private void ClientManager_ErrorEvent( object sender, Exception e )
		{
			WriteConsole( $"Error: {e.Message}" );
		}
		
		private void ClientManager_ClientManagerStatusChangeEvent( object sender, bool e )
		{
			this.SetBtns( e );
			WriteConsole( $"Server: {(e ? "启动" : "停止")}" );
		}

		private void ClientManager_ConnectedStatusChangeEvent( object sender, SocketStatusChangeArgs e )
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
