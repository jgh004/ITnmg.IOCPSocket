using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketStressTest
{
    /// <summary>
    /// socket 客户端
    /// </summary>
    public class SocketClient
    {
        /// <summary>
        /// 客户端id
        /// </summary>
        private int id = 0;

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
        /// socket 连接完成事件
        /// </summary>
        public event Action<int> SocketConnectedEvent;

        /// <summary>
        /// socket 关闭事件
        /// </summary>
        public event Action<int> SocketClosedEvent;

        /// <summary>
        /// socket 异常事件
        /// </summary>
        public event Action<int, SocketError> SocketErrorEvent;


        /// <summary>
        /// 获取上一次收发操作成功后连接的状态
        /// </summary>
        public bool Connected
        {
            get
            {
                return this.socket.Connected;
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

            if ( endPoint.AddressFamily == AddressFamily.InterNetworkV6 )
            {
                this.socket = new Socket( endPoint.AddressFamily, SocketType.Stream, ProtocolType.IPv6 );
            }
            else
            {
                this.socket = new Socket( endPoint.AddressFamily, SocketType.Stream, ProtocolType.IPv4 );
            }

            this.socket.SendTimeout = sendTimeOut;
            this.socket.ReceiveTimeout = receiveTimeOut;
            this.connArgs = new SocketAsyncEventArgs();
            this.connArgs.Completed += Connect_Completed;
            this.sendArgs = new SocketAsyncEventArgs();
            this.sendArgs.Completed += SendEve_Completed;
            this.recvArgs = new SocketAsyncEventArgs();
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

        public void SendData()
        {
            if ( !this.socket.SendAsync( sendArgs ) )
            {
                this.SendEve_Completed( this.socket, sendArgs );
            }
        }

        public void ReceiveData()
        {
            if ( !this.socket.ReceiveAsync( recvArgs ) )
            {
                this.ReceiveEve_Completed( this.socket, recvArgs );
            }
        }


        /// <summary>
        /// 引发 SocketConnectedEvent 事件 
        /// </summary>
        /// <param name="id">当前 client id</param>
        protected void OnSocketConnected( int id )
        {
            if ( this.SocketConnectedEvent != null )
            {
                this.SocketConnectedEvent( id );
            }
        }

        /// <summary>
        /// 引发 SocketClosedEvent 事件
        /// </summary>
        /// <param name="id">当前 client id</param>
        protected void OnSocketClosed( int id )
        {
            if ( this.SocketClosedEvent != null )
            {
                this.SocketClosedEvent( id );
            }
        }

        /// <summary>
        /// 引发 SocketErrorEvent 事件
        /// </summary>
        /// <param name="id">当前 client id</param>
        /// <param name="error"></param>
        protected void OnSocketError( int id, SocketError error )
        {
            if ( this.SocketErrorEvent != null )
            {
                this.SocketErrorEvent( id, error );
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
                this.OnSocketConnected( this.id );
            }
            else
            {
                this.OnSocketError( this.id, e.SocketError );
            }
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
                this.OnSocketError( this.id, e.SocketError );
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
                this.OnSocketError( this.id, e.SocketError );
            }
        }
    }
}

