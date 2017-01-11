using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITnmg.IOCPSocket
{
	public class TextProtocol : ISocketBufferProcess
	{
		public void GetNextSendBuffer( ref byte[] buffer, ref int effectiveLength )
		{
		}

		public bool ProcessReceive( byte[] buffer, int effectiveLength )
		{
			return true;
		}
	}
}
