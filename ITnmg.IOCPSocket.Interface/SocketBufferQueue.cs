using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITnmg.IOCPSocket
{
	/// <summary>
	/// Socket 缓存队列
	/// </summary>
	public class SocketBufferQueue
	{
		/// <summary>
		/// 获取或设置不完整的缓存数组中有效数据长度
		/// </summary>
		public int FragmentaryBufferLength
		{
			get; set;
		}

		/// <summary>
		/// 获取或设置处理过后返回的不完整的数据
		/// </summary>
		public byte[] FragmentaryBuffer
		{
			get; set;
		}

		/// <summary>
		/// Socket 缓存队列
		/// </summary>
		public ConcurrentQueue<SocketBuffer> BufferQueue
		{
			get; set;
		}


		public SocketBufferQueue()
		{
			BufferQueue = new ConcurrentQueue<SocketBuffer>();
		}
	}
}
