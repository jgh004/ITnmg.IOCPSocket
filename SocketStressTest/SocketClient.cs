using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocketStressTest
{
    /// <summary>
    /// socket 客户端
    /// </summary>
    internal class SocketClient
    {
        /// <summary>
        /// 客户端id
        /// </summary>
        private readonly int id = 0;

        /// <summary>
        /// 连接状态
        /// </summary>
        private volatile bool connStatus;

		/// <summary>
		/// 连接重试次数
		/// </summary>
		private volatile int connectionRetryCount = 0;

        /// <summary>
        /// 内部使用的socket
        /// </summary>
        private Socket socket;

        /// <summary>
        /// socket异步连接完成事件参数
        /// </summary>
        private SocketAsyncEventArgs connArgs;

        /// <summary>
        /// socket异步发送数据完成事件参数
        /// </summary>
        private SocketAsyncEventArgs sendArgs;

        /// <summary>
        /// socket异步接收数据完成事件参数
        /// </summary>
        private SocketAsyncEventArgs recvArgs;



        /// <summary>
        /// socket 连接状态变更事件
        /// </summary>
        public event Action<int, bool> StatusChangeEvent;

        /// <summary>
        /// socket 异常事件
        /// </summary>
        public event Action<int, SocketError> ErrorEvent;

		/// <summary>
		/// 获取连接重试次数
		/// </summary>
		public int ConnectionRetryCount
		{
			get
			{
				return this.connectionRetryCount;
			}
			private set
			{
				lock ( this )
				{
					this.connectionRetryCount = value;
				}
			}
		}
        
        /// <summary>
        /// 获取连接的状态
        /// </summary>
        public bool ConnStatus
        {
            get
            {
                return this.connStatus;
            }
            private set
			{
				lock ( this )
				{
					this.connStatus = value;
				}

                this.OnStatusChange( this.id, value );
            }
        }



        /// <summary>
        /// 创建socket客户端
        /// </summary>
        /// <param name="endPoint">ip终结点</param>
        /// <param name="sendTimeOut">发送数据超时时间,以毫秒为单位.</param>
        /// <param name="receiveTimeOut">接收数据超时时间,以毫秒为单位.</param>
        public SocketClient( int id, IPEndPoint endPoint, int sendTimeOut = 60000, int receiveTimeOut = 60000 )
        {
            this.id = id;
			this.socket = new Socket( endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp );

			this.socket.SendTimeout = sendTimeOut;
            this.socket.ReceiveTimeout = receiveTimeOut;

			this.connArgs = new SocketAsyncEventArgs();
			this.connArgs.RemoteEndPoint = endPoint;
            this.connArgs.Completed += Connect_Completed;

            this.sendArgs = new SocketAsyncEventArgs();
			this.sendArgs.RemoteEndPoint = endPoint;
			this.sendArgs.Completed += SendEve_Completed;

			this.recvArgs = new SocketAsyncEventArgs();
			this.recvArgs.RemoteEndPoint = endPoint;
			this.recvArgs.Completed += ReceiveEve_Completed;
        }


        /// <summary>
        /// 连接到服务器
        /// </summary>
        public void Connect()
        {
            if ( !this.socket.ConnectAsync( connArgs ) )
            {
                Connect_Completed( this.socket, connArgs );
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        public void SendData()
        {
            if ( !this.socket.SendAsync( sendArgs ) )
            {
                this.SendEve_Completed( this.socket, sendArgs );
            }
        }

        /// <summary>
        /// 接收数据
        /// </summary>
        public void ReceiveData()
        {
            if ( !this.socket.ReceiveAsync( recvArgs ) )
            {
                this.ReceiveEve_Completed( this.socket, recvArgs );
            }
        }


        /// <summary>
        /// 引发 StatusChangeEvent 事件 
        /// </summary>
        /// <param name="id">当前 client id</param>
        /// <param name="status">当前 client 连接状态</param>
        protected void OnStatusChange( int id, bool status )
        {
            if ( this.StatusChangeEvent != null )
            {
                this.StatusChangeEvent( id, status );
            }
        }

        /// <summary>
        /// 引发 SocketErrorEvent 事件
        /// </summary>
        /// <param name="id">当前 client id</param>
        /// <param name="error"></param>
        protected void OnError( int id, SocketError error )
        {
            if ( this.ErrorEvent != null )
            {
				this.ConnStatus = false;
				this.socket.Close();
                this.ErrorEvent( id, error );
            }
        }



		/// <summary>
		/// 异步连接完成处理
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Connect_Completed( object sender, SocketAsyncEventArgs e )
		{
			if ( e.SocketError == SocketError.Success )
			{
				this.ConnStatus = true;
			}
			else
			{
				this.ConnStatus = false;

				this.OnError( this.id, e.SocketError );
			}

			this.ConnectionRetryCount++;
		}

        /// <summary>
        /// 异步发送完成处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendEve_Completed( object sender, SocketAsyncEventArgs e )
        {
            if ( e.SocketError == SocketError.Success )
            {
            }
            else
            {
                this.OnError( this.id, e.SocketError );
            }
        }

        /// <summary>
        /// 异步接收完成处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReceiveEve_Completed( object sender, SocketAsyncEventArgs e )
        {
            if ( e.SocketError == SocketError.Success )
            {
            }
            else
            {
                this.OnError( this.id, e.SocketError );
            }
        }
    }
}

