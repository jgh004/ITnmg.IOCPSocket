using System;
using System.Collections.Concurrent;
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
		private SocketAsyncEventArgsPool saePool;

		/// <summary>
		/// 监听用的 socket
		/// </summary>
		private Socket listenSocket;

		/// <summary>
		/// 允许的最大连接数量
		/// </summary>
		private int maxConnCount;

		/// <summary>
		/// 启动时初始化多少个连接的资源
		/// </summary>
		private int initConnectionResourceCount;

		/// <summary>
		/// 一次读写socket的最大缓存字节数
		/// </summary>
		private int singleBufferMaxSize;

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
		/// 已连接的集合
		/// </summary>
		private ConcurrentDictionary<IntPtr, Socket> connectedList;


		/// <summary>
		/// 异常事件
		/// </summary>
		public event EventHandler<Exception> ErrorEvent;

		/// <summary>
		/// 服务端运行状态变化事件
		/// </summary>
		public event EventHandler<bool> ServerStateChangeEvent;

		/// <summary>
		/// Socket 连接数改变
		/// </summary>
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



		/// <summary>
		/// 创建服务端实例
		/// </summary>
		public SocketServerManager()
		{
		}



		/// <summary>
		/// 初始化服务端
		/// </summary>
		/// <param name="maxConnectionCount">允许的最大连接数</param>
		/// <param name="initConnectionResourceCount">启动时初始化多少个连接的资源</param>
		/// <param name="singleBufferMaxSize">每个 socket 读写缓存最大字节数, 默认为8k</param>
		/// <param name="sendTimeOut">socket 发送超时时长, 以毫秒为单位</param>
		/// <param name="receiveTimeOut">socket 接收超时时长, 以毫秒为单位</param>
		public async Task InitAsync( int maxConnectionCount, int initConnectionResourceCount, int singleBufferMaxSize = 8 * 1024
			, int sendTimeOut = 10000, int receiveTimeOut = 10000)
		{
			this.sendTimeOut = sendTimeOut;
			this.receiveTimeOut = receiveTimeOut;
			this.maxConnCount = maxConnectionCount;
			this.initConnectionResourceCount = initConnectionResourceCount;
			this.singleBufferMaxSize = singleBufferMaxSize;

			await Task.Run( () =>
			{
				//设置初始线程数为cpu核数*2
				this.connectedList = new ConcurrentDictionary<IntPtr, Socket>( Environment.ProcessorCount * 2, this.maxConnCount );
				//读写分离, 每个socket连接需要2个SocketAsyncEventArgs.
				saePool = new SocketAsyncEventArgsPool( this.initConnectionResourceCount * 2, ArgsSendAndReceive_Completed, this.singleBufferMaxSize );
			} );
		}

		/// <summary>
		/// 开始监听连接
		/// </summary>
		/// <param name="domainOrIP">要监听的域名或IP</param>
		/// <param name="port">端口</param>
		/// <param name="preferredIPv4">如果用域名初始化,可能返回多个ipv4和ipv6地址,指定是否首选ipv4地址.</param>
		/// <returns>返回实际监听的 EndPoint</returns>
		public async Task<IPEndPoint> StartListen( string domainOrIP, int port, bool preferredIPv4 = true )
		{
			IPEndPoint result = null;

			try
			{
				if ( this.listenSocket == null )
				{
					result = await GetIPEndPoint( domainOrIP, port, preferredIPv4 );
					this.semaphore = new Semaphore( this.maxConnCount, this.maxConnCount );
					this.listenSocket = new Socket( result.AddressFamily, SocketType.Stream, ProtocolType.Tcp );
					this.listenSocket.SendTimeout = this.sendTimeOut;
					this.listenSocket.ReceiveTimeout = this.receiveTimeOut;
					this.listenSocket.SendBufferSize = this.singleBufferMaxSize;
					this.listenSocket.ReceiveBufferSize = this.singleBufferMaxSize;
					this.listenSocket.Bind( result );
					this.listenSocket.Listen( 100 );

					SocketAsyncEventArgs args = new SocketAsyncEventArgs();
					args.SetBuffer( new byte[this.singleBufferMaxSize], 0, this.singleBufferMaxSize );
					args.DisconnectReuseSocket = true;
					args.Completed += Args_Completed;

					if ( !this.listenSocket.AcceptAsync( args ) )
					{
						Args_Completed( this.listenSocket, args );
					}

					OnServerStateChange( this, true );
				}
				else
				{
					result = this.listenSocket.LocalEndPoint as IPEndPoint;
					OnError( this, new Exception( "服务端已在运行" ) );
				}
			}
			catch ( Exception ex )
			{
				await this.CloseAsync();
				this.OnError( this, ex );
			}

			return result;
		}

		/// <summary>
		/// 关闭监听
		/// </summary>
		public async Task CloseAsync()
		{
			if ( this.listenSocket != null )
			{
				try
				{
					await CloseConnectList();

					try
					{
						this.listenSocket.Shutdown( SocketShutdown.Both );
					}
					catch ( Exception ex )
					{
					}
					
					OnServerStateChange( this, false );
				}
				catch ( Exception ex )
				{
					OnError( this, ex );
				}
				finally
				{
					this.listenSocket.Close();
					this.listenSocket = null;
					this.semaphore.Close();
					this.semaphore.Dispose();
					this.semaphore = null;
				}
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
		/// 引发 ConnectedCountChangeEvent 事件
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void OnConnectedCountChange( object sender, int e )
		{
			if ( this.ConnectedCountChangeEvent != null )
			{
				this.ConnectedCountChangeEvent( sender, e );
			}
		}



		/// <summary>
		/// 分析ip或域名,返回 IPEndPoint 实例
		/// </summary>
		/// <param name="domainOrIP">要监听的域名或IP</param>
		/// <param name="port">端口</param>
		/// <param name="preferredIPv4">如果用域名初始化,可能返回多个ipv4和ipv6地址,指定是否首选ipv4地址.</param>
		/// <returns>返回 IPEndPoint 实例</returns>
		private async Task<IPEndPoint> GetIPEndPoint( string domainOrIP, int port, bool preferredIPv4 = true )
		{
			IPEndPoint result = null;

			if ( !string.IsNullOrWhiteSpace( domainOrIP ) )
			{
				string ip = domainOrIP.Trim();
				IPAddress ipAddr = null;

				if ( !IPAddress.TryParse( ip, out ipAddr ) )
				{
					var addrs = await Dns.GetHostAddressesAsync( ip );

					if ( addrs == null || addrs.Length == 0 )
					{
						throw new Exception( "域名或ip地址不正确,未能解析." );
					}

					ipAddr = addrs.FirstOrDefault( k => k.AddressFamily == (preferredIPv4 ? AddressFamily.InterNetwork : AddressFamily.InterNetworkV6) );
					ipAddr = ipAddr ?? addrs.First();
				}

				result = new IPEndPoint( ipAddr, port );
			}
			else
			{
				result = new IPEndPoint( preferredIPv4 ? IPAddress.Any : IPAddress.IPv6Any, port );
			}

			return result;
		}

		/// <summary>
		/// 关闭已连接socket
		/// </summary>
		/// <returns></returns>
		private async Task CloseConnectList()
		{
			await Task.Run( () =>
			{
				if ( this.connectedList != null )
				{
					foreach ( var c in connectedList )
					{
						try
						{
							c.Value.Shutdown( SocketShutdown.Both );
						}
						catch ( Exception ex )
						{
						}

						c.Value.Close();
					}

					this.connectedList.Clear();
				}
			} );
		}

		/// <summary>
		/// 监听 socket 接收到新连接
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Args_Completed( object sender, SocketAsyncEventArgs e )
		{
			try
			{
				if ( e.SocketError == SocketError.Success )
				{
					this.semaphore.WaitOne();

					//var s = new SocketEntity();
					//s.SetSocket( e.AcceptSocket );

					//var receiveArgs = SocketAsyncEventArgsPool.Pop();

					//if ( !e.AcceptSocket.ReceiveAsync( receiveArgs ) )
					//{
					//	ArgsSendAndReceive_Completed( receiveArgs.AcceptSocket, receiveArgs );
					//}

					//var sendArgs = SocketAsyncEventArgsPool.Pop();

					//if ( !e.AcceptSocket.SendAsync( sendArgs ) )
					//{
					//	ArgsSendAndReceive_Completed( sendArgs.AcceptSocket, sendArgs );
					//}

					if ( connectedList.TryAdd( e.AcceptSocket.Handle, e.AcceptSocket ) )
					{
						//引发 socket 状态变更事件
						OnConnectedCountChange( this, this.connectedList.Count );
					}
					else
					{
						OnError( this, new Exception( "Socket 重复连接" ) );
					}

					//if ( this.socket != null && !this.socket.AcceptAsync( e ) )
					//{
					//	Args_Completed( this.socket, e );
					//}
				}
				else
				{
					if ( connectedList != null )
					{
						OnConnectedCountChange( this, this.connectedList.Count );
					}

					if ( e.AcceptSocket != null )
					{
						if ( connectedList != null )
						{
							Socket s = null;
							connectedList.TryRemove( e.AcceptSocket.Handle, out s );
							s = null;
						}

						e.AcceptSocket.Close( this.sendTimeOut );
					}
				}
			}
			catch ( Exception ex )
			{
				e.AcceptSocket = null;
				OnError( this, ex );
			}
		}

		/// <summary>
		/// Socket 发送与接收完成事件执行的方法
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ArgsSendAndReceive_Completed( object sender, SocketAsyncEventArgs e )
		{
		}

	}
}
