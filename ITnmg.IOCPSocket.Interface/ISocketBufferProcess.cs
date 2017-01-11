using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITnmg.IOCPSocket
{
	/// <summary>
	/// Socket 协议
	/// </summary>
	public interface ISocketBufferProcess
	{
		/// <summary>
		/// 处理 Socket 接收到的数据, 返回处理结果, 同时输出未能处理的数据(因为粘包或数据不全等原因)
		/// </summary>
		/// <param name="buffer">收到的数据</param>
		/// <param name="effectiveLength">buffer 中有效数据的长度</param>
		/// <returns>处理结果</returns>
		bool ProcessReceive( byte[] buffer, int effectiveLength );

		/// <summary>
		/// 返回下一个要发送的数据
		/// </summary>
		/// <param name="buffer">要发送的数据, 为避免频繁创建对象影响性能, 应直接向 buffer 写入数据, 不要用 new buffer[]. 
		/// 如果 buffer 容纳不下当前要发送的数据则先写入 buffer 最大容量的数据, 其他的数据下次再发. </param>
		/// <param name="effectiveLength">buffer 中有效数据的长度</param>
		void GetNextSendBuffer( ref byte[] buffer, ref int effectiveLength );
	}
}
