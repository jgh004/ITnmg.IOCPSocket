using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocketServer
{
	/// <summary>
	/// Socket服务端
	/// </summary>
	public class SocketServerManager
    {
		private Socket socket;

		/// <summary>
		/// 服务端ip
		/// </summary>
		private string ip;

		/// <summary>
		/// 服务端端口
		/// </summary>
		private int port;

		/// <summary>
		/// 要创建的连接数量
		/// </summary>
		private int connCount;

		/// <summary>
		/// 发送超时时间, 以毫秒为单位.
		/// </summary>
		private int sendTimeOut;

		/// <summary>
		/// 接收超时时间, 以毫秒为单位.
		/// </summary>
		private int receiveTimeOut;

		/// <summary>
		/// 信号量,初始设为0,让所有线程等待
		/// </summary>
		private Semaphore semaphore;

		/// <summary>
		/// 待连接客户端集合
		/// </summary>
		private Dictionary<int, Socket> waitConnectionList;

		/// <summary>
		/// 已连接的集合
		/// </summary>
		private Dictionary<int, Socket> connectedList;


		/// <summary>
		/// 异常事件
		/// </summary>
		public event EventHandler<Exception> ErrorEvent;

		public event EventHandler<int> ConnectedCountChange;


		/// <summary>
		/// 获取已连接的连接数
		/// </summary>
		public int TotalConnectedCount
		{
			get
			{
				return this.connectedList.Count;
			}
		}

		/// <summary>
		/// 获取总连接数
		/// </summary>
		public int TotalCount
		{
			get
			{
				return this.connCount;
			}
		}



		public SocketServerManager()
		{
		}


		public void Init( string domainOrIP, int port, int connectionCount, int sendTimeOut = 6000, int receiveTimeOut = 6000 )
		{
			if ( string.IsNullOrWhiteSpace( domainOrIP ) )
			{
				throw new ArgumentNullException( "domainOrIP" );
			}
			
			this.ip = domainOrIP;
			this.port = port;
			this.sendTimeOut = sendTimeOut;
			this.receiveTimeOut = receiveTimeOut;
			this.connCount = connectionCount;
			this.semaphore = new Semaphore( 0, this.connCount );
			this.waitConnectionList = new Dictionary<int, Socket>( this.connCount );
			this.connectedList = new Dictionary<int, Socket>( this.connCount );
		}


		public async Task StartListen()
		{
			await Dns.GetHostAddressesAsync( this.ip ).ContinueWith( f =>
			{
				if ( f.Exception != null )
				{
					this.OnError( this, f.Exception.GetBaseException() );
					return;
				}

				if ( f.Result == null || f.Result.Length == 0 )
				{
					this.OnError( this, new Exception( "域名或ip地址不正确,未能解析." ) );
					return;
				}

				IPAddress addr = f.Result[0];
				IPEndPoint point = new IPEndPoint( addr, this.port );

				this.socket = new Socket( point.AddressFamily, SocketType.Stream, ProtocolType.Tcp );
				this.socket.Bind( point );
				this.socket.Listen( this.TotalCount );
				SocketAsyncEventArgs args = new SocketAsyncEventArgs();
				args.Completed += Args_Completed;

				if ( !this.socket.AcceptAsync( args ) )
				{
					Args_Completed( this.socket, args );
				}
			} ).ContinueWith( f =>
			{
				if ( f.Exception != null )
				{
					this.OnError( this, f.Exception.GetBaseException() );
				}
			} );
		}

		private void Args_Completed( object sender, SocketAsyncEventArgs e )
		{
			e.AcceptSocket.ReceiveAsync( new SocketAsyncEventArgs() );
			e.AcceptSocket.SendAsync( new SocketAsyncEventArgs() );
		}



		/// <summary>
		/// 引发 Error 事件
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="ex"></param>
		protected void OnError( object sender, Exception ex )
		{
			if ( this.ErrorEvent != null )
			{
				this.ErrorEvent( sender, ex );
			}
		}
	}
}
