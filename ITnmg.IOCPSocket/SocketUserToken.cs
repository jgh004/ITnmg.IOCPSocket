using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ITnmg.IOCPSocket
{
	/// <summary>
	/// 异步 socket 关联的用户程序对象
	/// </summary>
	public class SocketUserToken
	{
		/// <summary>
		/// 缓存管理
		/// </summary>
		private ConcurrentBufferManager bufferManager;


		/// <summary>
		/// 获取或设置唯一Id
		/// </summary>
		public int Id
		{
			get; set;
		}

		/// <summary>
		/// 获取或设置当前 socket 连接
		/// </summary>
		public Socket CurrentSocket
		{
			get; set;
		}

		/// <summary>
		/// 获取或设置发送数据用的SocketAsyncEventArgs
		/// </summary>
		public SocketAsyncEventArgs ReceiveArgs
		{
			get; set;
		}

		/// <summary>
		/// 获取或设置接收数据用的SocketAsyncEventArgs
		/// </summary>
		public SocketAsyncEventArgs SendArgs
		{
			get; set;
		}

		/// <summary>
		/// 接收数用的缓存
		/// </summary>
		public SocketReceiveBuffer ReceiveBuffer
		{
			get; set;
		}

		/// <summary>
		/// 发送数据队列
		/// </summary>
		public ConcurrentQueue<BufferMap> SendBuffer
		{
			get; set;
		}

		/// <summary>
		/// 获取通信协议
		/// </summary>
		public ISocketProtocol Protocol
		{
			get;
			private set;
		}



		/// <summary>
		/// 初始化 SocketUserToken 实例
		/// </summary>
		public SocketUserToken( ISocketProtocol protocol, int singleBufferMaxSize )
		{
			this.Protocol = protocol;
			this.bufferManager = ConcurrentBufferManager.CreateBufferManager( 2, singleBufferMaxSize );
			this.ReceiveBuffer = new SocketReceiveBuffer();
			this.SendBuffer = new ConcurrentQueue<IOCPSocket.BufferMap>();
			this.Reset();
		}



		/// <summary>
		/// 重置为初始值
		/// </summary>
		public void Reset()
		{
			this.Id = -1;
			this.CurrentSocket = null;
			this.bufferManager.Clear();
		}

		/// <summary>
		/// 取出接收到的数据放到缓存队列
		/// </summary>
		/// <returns></returns>
		public void ProcessReceiveBuffer()
		{
			//从缓存池中取一段缓存
			var buffer = bufferManager.TakeBuffer( ReceiveArgs.BytesTransferred );
			Buffer.BlockCopy( ReceiveArgs.Buffer, ReceiveArgs.Offset, buffer, 0, buffer.Length );
			//将数据添加到队列
			ReceiveBuffer.Enqueue( buffer );
		}
	}

	/// <summary>
	/// Socket 接收数据用的缓存
	/// </summary>
	public class SocketReceiveBuffer
	{
		/// <summary>
		/// 接收的不完整的粘包数据
		/// </summary>
		public byte[] fragmentaryBuffer;

		/// <summary>
		/// 数据缓存队列
		/// </summary>
		public ConcurrentQueue<BufferMap> BufferQueue
		{
			get; set;
		}


		public SocketReceiveBuffer()
		{
			this.BufferQueue = new ConcurrentQueue<IOCPSocket.BufferMap>();
		}
	}

	/// <summary>
	/// byte[] 缓存
	/// </summary>
	public class BufferMap
	{
		/// <summary>
		/// 带有数据的字节数组
		/// </summary>
		public byte[] Buffer
		{
			get; set;
		}

		/// <summary>
		/// 数组中有效数据的长度
		/// </summary>
		public int Length
		{
			get; set;
		}
	}
}
