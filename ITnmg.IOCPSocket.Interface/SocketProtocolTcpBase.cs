using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITnmg.IOCPSocket
{
	public abstract class SocketBufferProcessTcpBase : ISocketBufferProcess
	{
		/// <summary>
		/// 接收缓存锁定用
		/// </summary>
		private object receiveLock = new object();

		private SocketBufferQueue receiveBufferQueue;

		private SocketBufferQueue sendBufferQueue;


		/// <summary>
		/// 处理接收到的数据
		/// </summary>
		/// <returns></returns>
		public virtual bool ProcessReceive( byte[] buffer, int effectiveLength )
		{
			bool result = false;

			lock ( receiveLock )
			{
				//待发给协议处理的数据
				//byte[] buffer;

				////如果当前缓存有效数据长度大于0
				//if ( receiveBufferLength > 0 )
				//{
				//	//从缓存池中取一段缓存, 长度等于当前有效长度+新收到的有效长度
				//	buffer = bufferManager.TakeBuffer( ReceiveArgs.BytesTransferred + receiveBufferLength );
				//	//先把有当前缓存有效数据写入新的大缓存
				//	Buffer.BlockCopy( receiveBuffer, 0, buffer, 0, receiveBufferLength );
				//	//再将新收到的数据写入大缓存,接在之前添加的数据后
				//	Buffer.BlockCopy( ReceiveArgs.Buffer, ReceiveArgs.Offset, buffer, receiveBufferLength, ReceiveArgs.BytesTransferred );
				//	//归还缓冲区
				//	bufferManager.ReturnBuffer( receiveBuffer );
				//	//清除有效长度
				//	receiveBufferLength = 0;
				//}
				//else
				//{
				//	//申请一段能放下新收到数据的缓存
				//	buffer = bufferManager.TakeBuffer( ReceiveArgs.BytesTransferred );
				//	//拷贝有效数据
				//	Buffer.BlockCopy( ReceiveArgs.Buffer, ReceiveArgs.Offset, buffer, 0, ReceiveArgs.BytesTransferred );
				//	//归还缓冲区
				//	bufferManager.ReturnBuffer( receiveBuffer );
				//	//将新的缓存赋给接收缓存
				//	receiveBuffer = buffer;
				//	//设置有效长度
				//	receiveBufferLength = ReceiveArgs.BytesTransferred;
				//}


				//Protocol.ProcessReceive( receiveBuffer, receiveBufferLength, out
			}

			return result;
		}

		public void GetNextSendBuffer( ref byte[] buffer, ref int effectiveLength )
		{
			throw new NotImplementedException();
		}
	}
}
