using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace IOCPSocket
{
	/// <summary>
	/// SocketAsyncEventArgs 管理池
	/// </summary>
	public class SocketAsyncEventArgsPool : IDisposable
	{
		/// <summary>
		/// SocketAsyncEventArgs 池
		/// </summary>
		private ConcurrentStack<SocketAsyncEventArgs> pool;

		/// <summary>
		/// 缓存管理
		/// </summary>
		private ConcurrentBufferManager bufferManager;

		/// <summary>
		/// SocketAsyncEventArgs 完成时的方法
		/// </summary>
		private EventHandler<SocketAsyncEventArgs> socketAsyncCompleted;

		/// <summary>
		/// SocketAsyncEventArgs 缓存最大字节数
		/// </summary>
		private int singleMaxBufferSize;


		/// <summary>
		/// 获取池中 SocketAsyncEventArgs 的数量
		/// </summary>
		public int Count
		{
			get
			{
				return pool == null ? 0 : pool.Count;
			}
		}



		/// <summary>
		/// 初始化 SocketAsyncEventArgs 池, 并设置 SocketAsyncEventArgs.Buffer 缓存.
		/// </summary>
		/// <param name="capacity">初始状态容量大小</param>
		/// <param name="completed">SocketAsyncEventArgs.Completed 事件执行的方法</param>
		/// <param name="singleBufferMaxSize">SocketAsyncEventArgs.Buffer 的最大 Length, 默认为8K</param>
		public SocketAsyncEventArgsPool( int capacity, EventHandler<SocketAsyncEventArgs> completed, int singleBufferMaxSize = 8 * 1024 )
		{
			socketAsyncCompleted = completed;
			singleMaxBufferSize = singleBufferMaxSize;
			//缓存池大小与SocketAsyncEventArgs池大小相同,因为每个SocketAsyncEventArgs只用一个缓存
			bufferManager = new ConcurrentBufferManager( singleBufferMaxSize * capacity, singleBufferMaxSize );
			pool = new ConcurrentStack<SocketAsyncEventArgs>();

			for ( int i = 0; i < capacity; i++ )
			{
				SocketAsyncEventArgs arg = TryCreateNew();

				if ( arg == null )
				{
					break;
				}

				pool.Push( arg );
			}
		}

		/// <summary>
		/// 初始化 SocketAsyncEventArgs 池, 并设置 SocketAsyncEventArgs.Buffer 缓存.
		/// </summary>
		/// <param name="capacity">初始状态容量大小</param>
		/// <param name="singleMaxBufferSize">SocketAsyncEventArgs.Buffer 的最大 Length, 默认为32K</param>
		public SocketAsyncEventArgsPool( int capacity, int singleMaxBufferSize = 32 * 1024 ) : this( capacity, null, singleMaxBufferSize )
		{
		}



		/// <summary>
		/// 入栈
		/// </summary>
		/// <param name="item">SocketAsyncEventArgs 实例, 不可为null</param>
		public void Push( SocketAsyncEventArgs item )
		{
			if ( item == null )
			{
				throw new ArgumentNullException( "item" );
			}

			lock ( this )
			{
				item.AcceptSocket = null;
				item.RemoteEndPoint = null;
				item.UserToken = null;
				item.DisconnectReuseSocket = true;
			}

			pool.Push( item );
		}

		/// <summary>
		/// 出栈, 如果为空则创建新的 SocketAsyncEventArgs 并设置初始值返回
		/// </summary>
		/// <returns>SocketAsyncEventArgs 实例</returns>
		public SocketAsyncEventArgs Pop()
		{
			SocketAsyncEventArgs result;

			if ( !pool.TryPop( out result ) )
			{
				lock ( this )
				{
					result = TryCreateNew();
				}
			}

			return result;
		}

		/// <summary>
		/// 清空堆栈
		/// </summary>
		public void Clear()
		{
			if ( pool != null )
			{
				pool.Clear();
			}

			if ( bufferManager != null )
			{
				bufferManager.Clear();
			}
		}

		/// <summary>
		/// 释放资源
		/// </summary>
		public void Dispose()
		{
			this.Clear();
			this.pool = null;

			if ( this.bufferManager != null )
			{
				this.bufferManager.Dispose();
				this.bufferManager = null;
			}
			
			GC.SuppressFinalize( this );
		}



		/// <summary>
		/// 创建新 SocketAsyncEventArgs
		/// </summary>
		/// <returns></returns>
		private SocketAsyncEventArgs TryCreateNew()
		{
			SocketAsyncEventArgs item = null;
			var buffer = bufferManager.TakeBuffer( singleMaxBufferSize );

			if ( buffer != null )
			{
				item = new SocketAsyncEventArgs();
				item.DisconnectReuseSocket = true;
				item.SetBuffer( buffer, 0, buffer.Length );

				if ( socketAsyncCompleted != null )
				{
					item.Completed += socketAsyncCompleted;
				}
			}

			return item;
		}
	}
}
