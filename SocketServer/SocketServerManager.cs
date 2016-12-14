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
		/// <summary>
		/// SocketAsyncEventArgs 池
		/// </summary>
		private SocketAsyncEventArgsPool argsPool;

		/// <summary>
		/// 监听用的 socket
		/// </summary>
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
		/// 允许的最大连接数量
		/// </summary>
		private int maxConnCount;

		/// <summary>
		/// 一次读写socket的最大缓存字节数
		/// </summary>
		private int maxBufferSize;

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

		/// <summary>
		/// Socket 连接发生的异常
		/// </summary>
		public event EventHandler<SocketError> SocketConnectionErrorEvent;

		public event EventHandler<int> ConnectedCountChangeEvent;


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
				return this.maxConnCount;
			}
		}



		public SocketServerManager()
		{
		}




		/// <summary>
		/// 初始化服务端
		/// </summary>
		/// <param name="domainOrIP"></param>
		/// <param name="port"></param>
		/// <param name="maxConnectionCount"></param>
		/// <param name="maxBufferSize"></param>
		/// <param name="sendTimeOut"></param>
		/// <param name="receiveTimeOut"></param>
		public void Init( string domainOrIP, int port, int maxConnectionCount, int maxBufferSize, int sendTimeOut = 6000, int receiveTimeOut = 6000 )
		{
			if ( string.IsNullOrWhiteSpace( domainOrIP ) )
			{
				throw new ArgumentNullException( "domainOrIP" );
			}
			
			this.ip = domainOrIP;
			this.port = port;
			this.sendTimeOut = sendTimeOut;
			this.receiveTimeOut = receiveTimeOut;
			this.maxConnCount = maxConnectionCount;
			this.maxBufferSize = maxBufferSize;
			this.semaphore = new Semaphore( 0, this.maxConnCount );
			this.waitConnectionList = new Dictionary<int, Socket>( this.maxConnCount );
			this.connectedList = new Dictionary<int, Socket>( this.maxConnCount );
			//读写分离, 每个socket连接需要2个SocketAsyncEventArgs.
			this.argsPool = new SocketServer.SocketAsyncEventArgsPool( maxConnectionCount * 2, ArgsSendAndReceive_Completed, 1 );
		}

		/// <summary>
		/// 开始监听连接
		/// </summary>
		/// <returns></returns>
		public async Task StartListen()
		{
			if ( this.socket == null )
			{
				await Dns.GetHostAddressesAsync( this.ip ).ContinueWith( f =>
				{
					if ( f.Result == null || f.Result.Length == 0 )
					{
						throw new Exception( "域名或ip地址不正确,未能解析." );
					}

					IPAddress addr = f.Result[0];
					IPEndPoint point = new IPEndPoint( addr, this.port );

					this.socket = new Socket( point.AddressFamily, SocketType.Stream, ProtocolType.Tcp );
					this.socket.Bind( point );
					this.socket.Listen( maxConnCount );

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
		}

		/// <summary>
		/// 关闭监听
		/// </summary>
		public void Close()
		{
			if ( this.socket != null )
			{
				try
				{
					this.socket.Shutdown( SocketShutdown.Both );
					this.socket.Close( sendTimeOut );
				}
				catch ( Exception ex )
				{
				}
				finally
				{
					this.socket = null;
				}
			}
		}

		/// <summary>
		/// 监听 socket 接收到新连接
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Args_Completed( object sender, SocketAsyncEventArgs e )
		{
			if ( e.SocketError == SocketError.Success )
			{
				var receiveArgs = argsPool.Pop();

				if ( !e.AcceptSocket.ReceiveAsync( receiveArgs ) )
				{
					ArgsSendAndReceive_Completed( e.AcceptSocket, receiveArgs );
				}

				var sendArgs = argsPool.Pop();

				if ( !e.AcceptSocket.SendAsync( sendArgs ) )
				{
					ArgsSendAndReceive_Completed( e.AcceptSocket, sendArgs );
				}
			}
			else
			{
				OnSocketConnectionError( e.AcceptSocket, e.SocketError );
			}
		}

		/// <summary>
		/// Socket 发送与接收完成事件执行的方法
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ArgsSendAndReceive_Completed( object sender, SocketAsyncEventArgs e )
		{
			if ( e.SocketError == SocketError.Success )
			{
				switch ( e.LastOperation )
				{
					case SocketAsyncOperation.Receive:
						break;
					case SocketAsyncOperation.Send:
						break;
					default:
						break;
				}
			}
			else
			{
				OnSocketConnectionError( e.AcceptSocket, e.SocketError );
			}
		}



		/// <summary>
		/// 引发 Error 事件
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void OnError( object sender, Exception e )
		{
			if ( this.ErrorEvent != null )
			{
				this.ErrorEvent( sender, e );
			}
		}

		/// <summary>
		/// 引发 SocketConnectionError 事件
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void OnSocketConnectionError( object sender, SocketError e )
		{
			if ( this.SocketConnectionErrorEvent != null )
			{
				this.SocketConnectionErrorEvent( sender, e );
			}
		}
	}
}
