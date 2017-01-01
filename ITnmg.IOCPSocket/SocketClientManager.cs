using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ITnmg.IOCPSocket
{
	/// <summary>
	/// Socket压力测试客户端
	/// </summary>
	public class SocketClientManager : SocketManagerBase
	{
		private IPEndPoint serverPoint;

		/// <summary>
		/// 客户端运行状态变化事件
		/// </summary>
		public event EventHandler<bool> ClientManagerStatusChangeEvent;



		/// <summary>
		/// 构造函数
		/// </summary>
		public SocketClientManager()
		{
		}


		
		/// <summary>
		/// 启动客户端任务
		/// </summary>
		public async Task StartAsync( string domainOrIP, int port, bool preferredIPv4 = true )
		{
			serverPoint = await GetIPEndPoint( domainOrIP, port, preferredIPv4 );

			Parallel.For( 0, maxConnCount, i =>
			{
				SocketAsyncEventArgs args = saePool.Pop();
				args.RemoteEndPoint = serverPoint;
				args.Completed += ConnectArgs_Completed;

				try
				{
					if ( !Socket.ConnectAsync( SocketType.Stream, ProtocolType.Tcp, args ) )
					{
						ConnectArgs_Completed( serverPoint, args );
					}
				}
				catch ( Exception ex )
				{
					OnError( this, ex );
				}
			} );

			OnClientManagerStatusChange( this, true );
		}

		/// <summary>
		/// 停止客户端任务
		/// </summary>
		/// <returns></returns>
		public async Task StopAsync()
		{
			await CloseConnectList();
			OnClientManagerStatusChange( this, false );
		}

		
		/// <summary>
		/// 收到新连接
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ConnectArgs_Completed( object sender, SocketAsyncEventArgs e )
		{
			if ( e.SocketError == SocketError.Success )
			{
				e.UserToken = ToConnCompletedSuccess( e.ConnectSocket );
			}
			else
			{
				ToConnCompletedError( e.ConnectSocket, e.SocketError, e.UserToken as SocketUserToken );
			}

			//监听下一个请求
			e.UserToken = null;
			e.Dispose();
			e = null;
		}

		/// <summary>
		/// 引发 ClientManagerStatusChangeEvent 事件
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OnClientManagerStatusChange( object sender, bool e )
		{
			if ( ClientManagerStatusChangeEvent != null )
			{
				ClientManagerStatusChangeEvent( sender, e );
			}
		}
	}
}

