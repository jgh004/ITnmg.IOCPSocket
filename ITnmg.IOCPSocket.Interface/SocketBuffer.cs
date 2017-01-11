using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITnmg.IOCPSocket
{
	/// <summary>
	/// Socket 收发数据的缓存
	/// </summary>
	public class SocketBuffer
	{
		/// <summary>
		/// 获取或设置缓存内容
		/// </summary>
		public byte[] Buffer
		{
			get; set;
		}

		/// <summary>
		/// 获取或设置缓存中有效数据的长度
		/// </summary>
		public int Length
		{
			get; set;
		}
	}
}
