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
	/// Socket服务端
	/// </summary>
	public class SocketServerManager : SocketManagerBase
	{
		/// <summary>
		/// 监听用的 socket
		/// </summary>
		protected Socket listenSocket;

		/// <summary>
		/// 服务端运行状态变化事件
		/// </summary>
		public event EventHandler<bool> ServerStatusChangeEvent;



		/// <summary>
		/// 创建服务端实例
		/// </summary>
		public SocketServerManager()
		{
		}


		
		/// <summary>
		/// 开始监听连接
		/// </summary>
		/// <param name="domainOrIP">要监听的域名或IP</param>
		/// <param name="port">端口</param>
		/// <param name="preferredIPv4">如果用域名初始化,可能返回多个ipv4和ipv6地址,指定是否首选ipv4地址.</param>
		/// <returns>返回实际监听的 EndPoint</returns>
		public virtual async Task<IPEndPoint> StartAsync( string domainOrIP, int port, bool preferredIPv4 = true )
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
					args.Completed += AcceptArgs_Completed;

					if ( !this.listenSocket.AcceptAsync( args ) )
					{
						AcceptArgs_Completed( this.listenSocket, args );
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
				await this.StopAsync();
				this.OnError( this, ex );
			}

			return result;
		}

		/// <summary>
		/// 关闭监听
		/// </summary>
		public virtual async Task StopAsync()
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
		/// 监听 socket 接收到新连接
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void AcceptArgs_Completed( object sender, SocketAsyncEventArgs e )
		{
			if ( e.SocketError == SocketError.Success )
			{
				if ( this.semaphore != null )
				{
					this.semaphore.WaitOne();
				}
				e.UserToken = ToConnCompletedSuccess( e.AcceptSocket );
			}
			else
			{
				ToConnCompletedError( e.AcceptSocket, e.SocketError, e.UserToken as SocketUserToken );
				if ( this.semaphore != null )
				{
					this.semaphore.Release();
				}
			}

			//监听下一个请求
			e.AcceptSocket = null;
			e.UserToken = null;

			if ( this.listenSocket != null && !this.listenSocket.AcceptAsync( e ) )
			{
				AcceptArgs_Completed( this.listenSocket, e );
			}
		}

		/// <summary>
		/// 引发 ServerStateChange 事件
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OnServerStateChange( object sender, bool e )
		{
			if ( ServerStatusChangeEvent != null )
			{
				ServerStatusChangeEvent( sender, e );
			}
		}
	}
}
