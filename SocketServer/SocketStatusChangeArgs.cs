using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace IOCPSocket
{
	/// <summary>
	/// Socket 连接状态变更事件参数
	/// </summary>
	public class SocketStatusChangeArgs : EventArgs
	{
		/// <summary>
		/// 获取引发事件的 SocketUserToken Id
		/// </summary>
		public int UserTokenId
		{
			get;
			internal set;
		}

		/// <summary>
		/// 获取 Socket 当前状态
		/// </summary>
		public bool Status
		{
			get;
			internal set;
		}

		/// <summary>
		/// 获取 Socket 异常信息
		/// </summary>
		public SocketError Error
		{
			get;
			internal set;
		}

		/// <summary>
		/// 获取当前已连接 Socket 数量
		/// </summary>
		public int ConnectedCount
		{
			get;
			internal set;
		}
	}
}
