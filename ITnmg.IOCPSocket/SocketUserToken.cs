using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Sockets;
using System.ServiceModel.Channels;
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
		private BufferManager bufferManager;
		

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
		/// 获取缓存数据处理对象
		/// </summary>
		public ISocketBufferProcess BufferProcess
		{
			get;
			private set;
		}



		/// <summary>
		/// 初始化 SocketUserToken 实例
		/// </summary>
		public SocketUserToken( ISocketBufferProcess bufferProcess, int singleBufferMaxSize )
		{
			this.BufferProcess = bufferProcess ?? throw new ArgumentNullException( "bufferProcess" );
			this.bufferManager = BufferManager.CreateBufferManager( 2, singleBufferMaxSize );
			this.Reset();
		}



		/// <summary>
		/// 重置为初始值
		/// </summary>
		public void Reset()
		{
			this.Id = -1;
			this.CurrentSocket = null;
		}

		/// <summary>
		/// 处理接收到的数据
		/// </summary>
		public void ProcessReceive()
		{
			BufferProcess.ProcessReceive( ReceiveArgs.Buffer, ReceiveArgs.BytesTransferred );
		}

		public void ProcessSend()
		{
		}
	}
}
