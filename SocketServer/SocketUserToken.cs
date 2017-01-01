using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace IOCPSocket
{
	/// <summary>
	/// 异步 socket 关联的用户程序对象
	/// </summary>
	public class SocketUserToken
	{
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
		/// 初始化 SocketUserToken 实例
		/// </summary>
		public SocketUserToken()
		{
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
	}
}
