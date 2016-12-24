using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace SocketServer
{
	/// <summary>
	/// 线程安全的缓存管理类
	/// </summary>
	public class ConcurrentBufferManager : IDisposable
	{
		/// <summary>
		/// 非线程安全的 BufferManager 实例
		/// </summary>
		private BufferManager manager;



		/// <summary>
		/// 初始化 ConcurrentBufferManager 类的新实例
		/// </summary>
		/// <param name="maxBufferPoolSize">缓冲池的最大字节数</param>
		/// <param name="singleBufferMaxSize">单个缓冲区的最大字节数</param>
		public ConcurrentBufferManager( long maxBufferPoolSize, int singleBufferMaxSize )
		{
			manager = BufferManager.CreateBufferManager( maxBufferPoolSize, singleBufferMaxSize );
		}



		/// <summary>
		/// 从缓冲池获取一个至少为指定大小的缓冲区
		/// </summary>
		/// <param name="bufferSize">所请求缓冲区的大小（以字节为单位）</param>
		/// <returns></returns>
		public byte[] TakeBuffer(int bufferSize )
		{
			lock ( this )
			{
				byte[] result = null;

				try
				{
					result = manager.TakeBuffer( bufferSize );
				}
				catch ( Exception ex )
				{
					GC.Collect( GC.GetGeneration(this), GCCollectionMode.Forced, false );
				}

				return result;
			}
		}

		/// <summary>
		/// 将缓冲区返回到缓冲池
		/// </summary>
		/// <param name="buffer">要返回的缓冲区引用</param>
		public void ReturnBuffer( byte[] buffer )
		{
			lock ( this )
			{
				manager.ReturnBuffer( buffer );
			}
		}

		/// <summary>
		/// 释放目前在管理器中缓存的缓冲区
		/// </summary>
		public void Clear()
		{
			lock ( this )
			{
				if ( manager != null )
				{
					manager.Clear();
				}
			}
		}

		/// <summary>
		/// 释放资源
		/// </summary>
		public void Dispose()
		{
			this.Clear();
			this.manager = null;
			GC.SuppressFinalize( this );
		}
	}
}
