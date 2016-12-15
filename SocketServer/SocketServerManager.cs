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
		private string domainOrIP;

		/// <summary>
		/// 当使用域名连接时, 如果返回多个ip, 是否首选ipv4, 否则用ipv6.
		/// </summary>
		private bool firstIPv4;

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
		/// 信号量,初始设为 maxConnCount
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
		/// 服务端运行状态变化事件
		/// </summary>
		public event EventHandler<bool> ServerStateChangeEvent;

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
		/// <param name="domainOrIP">要监听的域名或IP</param>
		/// <param name="port">端口</param>
		/// <param name="maxConnectionCount">允许的最大连接数</param>
		/// <param name="maxBufferSize">socket 读写缓存大小</param>
		/// <param name="firstIPv4">如果用域名初始化,可能返回多个ipv4和ipv6地址,指定是否首选ipv4地址.</param>
		/// <param name="sendTimeOut"></param>
		/// <param name="receiveTimeOut"></param>
		public void Init( string domainOrIP, int port, int maxConnectionCount
			, int maxBufferSize = 32 * 1024, bool firstIPv4 = true, int sendTimeOut = 6000, int receiveTimeOut = 6000 )
		{
			if ( domainOrIP != null )
			{
				this.domainOrIP = domainOrIP.Trim();
			}
			this.firstIPv4 = firstIPv4;
			this.port = port;
			this.sendTimeOut = sendTimeOut;
			this.receiveTimeOut = receiveTimeOut;
			this.maxConnCount = maxConnectionCount;
			this.maxBufferSize = maxBufferSize;
			this.semaphore = new Semaphore( this.maxConnCount, this.maxConnCount );
			this.waitConnectionList = new Dictionary<int, Socket>( this.maxConnCount );
			this.connectedList = new Dictionary<int, Socket>( this.maxConnCount );
			//读写分离, 每个socket连接需要2个SocketAsyncEventArgs.
			this.argsPool = new SocketServer.SocketAsyncEventArgsPool( maxConnectionCount * 2, ArgsSendAndReceive_Completed, 1 );
		}

		/// <summary>
		/// 开始监听连接
		/// </summary>
		/// <returns>返回实际监听的 EndPoint</returns>
		public async Task<IPEndPoint> StartListen()
		{
			IPEndPoint result = null;

			try
			{
				if ( this.socket == null )
				{
					if ( !string.IsNullOrWhiteSpace( this.domainOrIP ) )
					{
						IPAddress ipAddr = null;

						if ( !IPAddress.TryParse( domainOrIP, out ipAddr ) )
						{
							var addrs = await Dns.GetHostAddressesAsync( this.domainOrIP );

							if ( addrs == null || addrs.Length == 0 )
							{
								throw new Exception( "域名或ip地址不正确,未能解析." );
							}

							ipAddr = addrs.FirstOrDefault( k => k.AddressFamily == (firstIPv4 ? AddressFamily.InterNetwork : AddressFamily.InterNetworkV6) );
							ipAddr = ipAddr ?? addrs.First();
						}

						result = new IPEndPoint( ipAddr, this.port );
					}
					else
					{
						result = new IPEndPoint( firstIPv4 ? IPAddress.Any : IPAddress.IPv6Any, port );
					}

					this.socket = new Socket( result.AddressFamily, SocketType.Stream, ProtocolType.Tcp );
					this.socket.Bind( result );
					this.socket.Listen( maxConnCount );

					SocketAsyncEventArgs args = new SocketAsyncEventArgs();
					args.Completed += Args_Completed;

					if ( !this.socket.AcceptAsync( args ) )
					{
						Args_Completed( this.socket, args );
					}

					OnServerStateChange( this, true );
				}
				else
				{
					result = this.socket.LocalEndPoint as IPEndPoint;
					OnError( this, new Exception( "服务端已经在运行中" ) );
				}
			}
			catch ( Exception ex )
			{
				this.Close();
				this.OnError( this, ex );
			}

			return result;
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
					try
					{
						this.socket.Shutdown( SocketShutdown.Both );
					}
					catch
					{
					}

					this.socket.Close( sendTimeOut );
					this.semaphore.Close();
				}
				catch ( Exception ex )
				{
					OnError( this, ex );
				}
				finally
				{
					if ( this.socket != null )
					{
						this.socket.Close();
						this.socket = null;
					}
					this.semaphore = new Semaphore( this.maxConnCount, this.maxConnCount );
					OnServerStateChange( this, false );
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
				//
				this.semaphore.WaitOne();
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
						ProcessReceive( e );
						break;
					case SocketAsyncOperation.Send:
						ProcessSend( e );
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


		private void ProcessReceive( SocketAsyncEventArgs e )
		{
		}

		private void ProcessSend( SocketAsyncEventArgs e )
		{
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
		/// 引发 ServerStateChange 事件
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void OnServerStateChange( object sender, bool e )
		{
			if ( ServerStateChangeEvent != null )
			{
				ServerStateChangeEvent( sender, e );
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
