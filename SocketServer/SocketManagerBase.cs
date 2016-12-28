using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IOCPSocket
{
	/// <summary>
	/// IPCP Socket 服务端,客户端通用管理基类
	/// </summary>
	public abstract class SocketManagerBase
	{
		/// <summary>
		/// SocketAsyncEventArgs 池
		/// </summary>
		protected SocketAsyncEventArgsPool saePool;

		/// <summary>
		/// 允许的最大连接数量
		/// </summary>
		protected int maxConnCount;

		/// <summary>
		/// 启动时初始化多少个连接的资源
		/// </summary>
		protected int initConnectionResourceCount;

		/// <summary>
		/// 一次读写socket的最大缓存字节数
		/// </summary>
		protected int singleBufferMaxSize;

		/// <summary>
		/// 发送超时时间, 以毫秒为单位.
		/// </summary>
		protected int sendTimeOut;

		/// <summary>
		/// 接收超时时间, 以毫秒为单位.
		/// </summary>
		protected int receiveTimeOut;

		/// <summary>
		/// 信号量,初始设为 maxConnCount
		/// </summary>
		protected Semaphore semaphore;

		/// <summary>
		/// userToken 缓存集合
		/// </summary>
		protected ConcurrentStack<SocketUserToken> entityPool;

		/// <summary>
		/// 已连接的集合
		/// </summary>
		protected ConcurrentDictionary<int, SocketUserToken> connectedEntityList;


		/// <summary>
		/// 异常事件
		/// </summary>
		public event EventHandler<Exception> ErrorEvent;

		/// <summary>
		/// Socket 连接状态改变事件
		/// </summary>
		public event EventHandler<SocketStatusChangeArgs> ConnectedStatusChangeEvent;


		/// <summary>
		/// 获取已连接的连接数
		/// </summary>
		public int TotalConnectedCount
		{
			get
			{
				return this.connectedEntityList.Count;
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
		public SocketManagerBase()
		{
		}



		/// <summary>
		/// 初始化管理
		/// </summary>
		/// <param name="maxConnectionCount">允许的最大连接数</param>
		/// <param name="initConnectionResourceCount">启动时初始化多少个连接的资源</param>
		/// <param name="singleBufferMaxSize">每个 socket 读写缓存最大字节数, 默认为8k</param>
		/// <param name="sendTimeOut">socket 发送超时时长, 以毫秒为单位</param>
		/// <param name="receiveTimeOut">socket 接收超时时长, 以毫秒为单位</param>
		public virtual async Task InitAsync( int maxConnectionCount, int initConnectionResourceCount, int singleBufferMaxSize = 8 * 1024
			, int sendTimeOut = 10000, int receiveTimeOut = 10000 )
		{
			this.maxConnCount = maxConnectionCount;
			this.initConnectionResourceCount = initConnectionResourceCount;
			this.singleBufferMaxSize = singleBufferMaxSize;
			this.sendTimeOut = sendTimeOut;
			this.receiveTimeOut = receiveTimeOut;

			await Task.Run( () =>
			{
				//设置初始线程数为cpu核数*2
				this.connectedEntityList = new ConcurrentDictionary<int, IOCPSocket.SocketUserToken>( Environment.ProcessorCount * 2, this.maxConnCount );
				//读写分离, 每个socket连接需要2个SocketAsyncEventArgs.
				saePool = new SocketAsyncEventArgsPool( this.initConnectionResourceCount * 2, SendAndReceiveArgs_Completed, this.singleBufferMaxSize );

				this.entityPool = new ConcurrentStack<SocketUserToken>();

				Parallel.For( 0, maxConnCount, i =>
				{
					SocketUserToken token = new SocketUserToken( i );
					token.CurrentSocket = new Socket( SocketType.Stream, ProtocolType.Tcp );
					token.ReceiveArgs = saePool.Pop();
					token.SendArgs = saePool.Pop();
					this.entityPool.Push( token );
				} );
			} );
		}



		/// <summary>
		/// 引发 Error 事件
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OnError( object sender, Exception e )
		{
			if ( this.ErrorEvent != null )
			{
				this.ErrorEvent( sender, e );
			}
		}

		/// <summary>
		/// 引发 ConnectedStatusChangeEvent 事件
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="tokenId"></param>
		/// <param name="status"></param>
		/// <param name="error"></param>
		protected virtual void OnConnectedStatusChange( object sender, int tokenId, bool status, SocketError error )
		{
			if ( this.ConnectedStatusChangeEvent != null )
			{
				var arg = new SocketStatusChangeArgs();
				arg.UserTokenId = tokenId;
				arg.Status = status;
				arg.Error = error;
				arg.ConnectedCount = this.connectedEntityList.Count;
				this.ConnectedStatusChangeEvent( sender, arg );
			}
		}



		/// <summary>
		/// 分析ip或域名,返回 IPEndPoint 实例
		/// </summary>
		/// <param name="domainOrIP">要监听的域名或IP</param>
		/// <param name="port">端口</param>
		/// <param name="preferredIPv4">如果用域名初始化,可能返回多个ipv4和ipv6地址,指定是否首选ipv4地址.</param>
		/// <returns>返回 IPEndPoint 实例</returns>
		protected virtual async Task<IPEndPoint> GetIPEndPoint( string domainOrIP, int port, bool preferredIPv4 = true )
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
		protected virtual async Task CloseConnectList()
		{
			await Task.Run( () =>
			{
				if ( this.connectedEntityList != null )
				{
					connectedEntityList.AsParallel().ForAll( f =>
					{
						FreeUserToken( f.Value );
					} );

					this.connectedEntityList.Clear();
				}
			} );
		}

		/// <summary>
		/// 执行 socket 连接成功时的处理
		/// </summary>
		/// <param name="s"></param>
		/// <returns>返回 userToken</returns>
		protected virtual SocketUserToken ToConnCompletedSuccess( Socket s )
		{
			SocketUserToken result = null;
			SocketError error = SocketError.Success;

			try
			{
				if ( !entityPool.TryPop( out result ) )
				{
					result = new SocketUserToken( (int)s.Handle );
					result.CurrentSocket = new Socket( SocketType.Stream, ProtocolType.Tcp );
					result.ReceiveArgs = saePool.Pop();
					result.SendArgs = saePool.Pop();
				}

				result.CurrentSocket = s;
				result.ReceiveArgs.UserToken = result;
				result.SendArgs.UserToken = result;

				if ( !s.ReceiveAsync( result.ReceiveArgs ) )
				{
					SendAndReceiveArgs_Completed( s, result.ReceiveArgs );
				}

				if ( !s.SendAsync( result.SendArgs ) )
				{
					SendAndReceiveArgs_Completed( s, result.SendArgs );
				}

				if ( connectedEntityList.TryAdd( result.Id, result ) )
				{
					OnConnectedStatusChange( this, result.Id, true, error );
				}
				else
				{
					CloseSocket( s );
					throw new Exception( $"无法建立 Socket 连接: {error.ToString()}, id:{result.Id} handle:{s?.Handle}" );
				}
			}
			catch ( Exception ex )
			{
				CloseSocket( s );
				OnConnectedStatusChange( this, result == null ? (int)s.Handle : result.Id, false, error );
			}

			return result;
		}

		/// <summary>
		/// 执行 socket 连接异常时的处理
		/// </summary>
		protected virtual void ToConnCompletedError( Socket s, SocketError error, SocketUserToken token )
		{
			try
			{
				if ( token != null )
				{
					if ( connectedEntityList.TryRemove( token.Id, out token ) )
					{
						saePool.Push( token.ReceiveArgs );
						saePool.Push( token.SendArgs );
						token.Reset();
						entityPool.Push( token );
						OnConnectedStatusChange( this, token.Id, false, error );
					}
				}

				CloseSocket( s );
			}
			catch ( Exception ex )
			{
				CloseSocket( s );
				OnConnectedStatusChange( this, token == null ? (int)s.Handle : token.Id, false, error );
			}
		}

		/// <summary>
		/// Socket 发送与接收完成事件执行的方法
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void SendAndReceiveArgs_Completed( object sender, SocketAsyncEventArgs e )
		{
			if ( e.SocketError == SocketError.Success )
			{
				switch ( e.LastOperation )
				{
					case SocketAsyncOperation.Receive:
						break;
					case SocketAsyncOperation.Send:
						break;
					case SocketAsyncOperation.Accept:
						break;
					case SocketAsyncOperation.Connect:
						break;
					case SocketAsyncOperation.Disconnect:
						break;
					default:
						break;
				}
			}
			else
			{
			}
		}

		/// <summary>
		/// 关闭 socket
		/// </summary>
		/// <param name="s"></param>
		protected virtual void CloseSocket( Socket s )
		{
			if ( s != null )
			{
				try
				{
					s.Shutdown( SocketShutdown.Both );
				}
				catch ( Exception ex )
				{
				}

				s.Close();
			}
		}

		/// <summary>
		/// 释放 token 资源, 将 token 放回池
		/// </summary>
		/// <param name="token"></param>
		protected virtual void FreeUserToken( SocketUserToken token )
		{
			CloseSocket( token.CurrentSocket );

			this.saePool.Push( token.ReceiveArgs );
			this.saePool.Push( token.SendArgs );
			token.CurrentSocket = null;
			token.ReceiveArgs = null;
			token.SendArgs = null;
			this.entityPool.Push( token );
		}
	}
}
