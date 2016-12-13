using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketServer
{
	/// <summary>
	/// SocketAsyncEventArgs 池
	/// </summary>
	public class SocketAsyncEventArgsPool
	{
		/// <summary>
		/// SocketAsyncEventArgs 池
		/// </summary>
		private static Stack<SocketAsyncEventArgs> pool;
		
		/// <summary>
		/// 获取池中 SocketAsyncEventArgs 的数量
		/// </summary>
		public static int Count
		{
			get
			{
				return pool.Count;
			}
		}



		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="capacity">初始容量</param>
		private SocketAsyncEventArgsPool()
		{
		}

		/// <summary>
		/// 静态构造函数
		/// </summary>
		static SocketAsyncEventArgsPool()
		{
			pool = new Stack<SocketAsyncEventArgs>();
		}



		/// <summary>
		/// 初始化池容量, 只有当池为空时才能初始化.
		/// </summary>
		/// <param name="capacity">初始状态容量大小</param>
		/// <returns>设置成功返回 true, 否则返回 false.</returns>
		public static bool InitPoolCapacity( int capacity )
		{
			bool result = false;

			lock ( pool )
			{
				if ( pool.Count == 0 )
				{
					pool = new Stack<SocketAsyncEventArgs>( capacity );
					result = true;
				}
			}

			return result;
		}


		/// <summary>
		/// 入栈
		/// </summary>
		/// <param name="item">SocketAsyncEventArgs 实例</param>
		public static void Push( SocketAsyncEventArgs item )
		{
			lock ( pool )
			{
				pool.Push( item );
			}
		}

		/// <summary>
		/// 出栈
		/// </summary>
		/// <returns>SocketAsyncEventArgs 实例</returns>
		public static SocketAsyncEventArgs Pop()
		{
			lock ( pool )
			{
				return pool.Pop();
			}
		}
	}
}
