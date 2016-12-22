using System;
using System.Collections;
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
    /// Socket压力测试客户端
    /// </summary>
    public class SocketClientManager
    {
        /// <summary>
        /// 服务端ip
        /// </summary>
        private string ip;

        /// <summary>
        /// 服务端端口
        /// </summary>
        private int port;

        /// <summary>
        /// 要创建的连接数量
        /// </summary>
        private int connCount;

        /// <summary>
        /// 发送超时时间, 以毫秒为单位.
        /// </summary>
        private int sendTimeOut;

        /// <summary>
        /// 接收超时时间, 以毫秒为单位.
        /// </summary>
        private int receiveTimeOut;

        /// <summary>
        /// 信号量,初始设为0,让所有线程等待
        /// </summary>
        private Semaphore semaphore;

        /// <summary>
        /// 待连接客户端集合
        /// </summary>
        private Dictionary<int, SocketClient> waitConnectionList;

		/// <summary>
		/// 已连接的集合
		/// </summary>
		private Dictionary<int, SocketClient> connectedList;


        /// <summary>
        /// 异常事件
        /// </summary>
        public event EventHandler<Exception> ErrorEvent;

		public event EventHandler<int> ConnectedCountChange;


		/// <summary>
		/// 获取已连接的连接数
		/// </summary>
		public int TotalConnectedCount
        {
            get
            {
                return this.connectedList.Count;
            }
        }

        /// <summary>
        /// 获取总连接数
        /// </summary>
        public int TotalCount
        {
            get
            {
                return this.connCount;
            }
        }



		/// <summary>
		/// 构造函数
		/// </summary>
		public SocketClientManager()
        {
        }



		/// <summary>
		/// 初始化客户端参数
		/// </summary>
		/// <param name="domainOrIP">域名或ip</param>
		/// <param name="port">端口</param>
		/// <param name="connectionCount">要创建的连接数</param>
		/// <param name="sendTimeOut">发送数据超时时间</param>
		/// <param name="receiveTimeOut">接收数据超时时间</param>
		public void Init( string domainOrIP, int port, int connectionCount, int sendTimeOut = 6000, int receiveTimeOut = 6000 )
        {
            if ( string.IsNullOrWhiteSpace( domainOrIP ) )
            {
                throw new ArgumentNullException( "domainOrIP" );
            }

            this.ip = domainOrIP;
            this.port = port;
            this.sendTimeOut = sendTimeOut;
            this.receiveTimeOut = receiveTimeOut;
            this.connCount = connectionCount;
            this.semaphore = new Semaphore( 0, this.connCount );
            this.waitConnectionList = new Dictionary<int, SocketClient>( this.connCount );
			this.connectedList = new Dictionary<int, SocketClient>( this.connCount );
		}

        /// <summary>
        /// 启动客户端任务
        /// </summary>
        public async Task StartAsync()
        {
            await Dns.GetHostAddressesAsync( this.ip ).ContinueWith( f =>
            {
                if ( f.Exception != null )
                {
                    this.OnError( this, f.Exception.GetBaseException() );
                    return;
                }

                if ( f.Result == null || f.Result.Length == 0 )
                {
                    this.OnError(this, new Exception( "域名或ip地址不正确,未能解析." ) );
                    return;
                }

                IPAddress addr = f.Result[0];

                for ( int i = 0; i < this.connCount; i++ )
				{
					IPEndPoint point = new IPEndPoint( addr, this.port );
					SocketClient s = new SocketClient( i, point, sendTimeOut, receiveTimeOut );
                    s.ErrorEvent += S_ErrorEvent;
                    s.StatusChangeEvent += S_StatusChangeEvent;
                    this.waitConnectionList.Add( i, s );
                }

                StartConnect();
            } ).ContinueWith(f=>
			{
				if ( f.Exception != null )
				{
					this.OnError( this, f.Exception.GetBaseException() );
				}
			} );
        }

		/// <summary>
		/// 停止客户端任务
		/// </summary>
		/// <returns></returns>
		public void StopAsync()
        {
            for ( int i = 0; i < this.waitConnectionList.Count; i++ )
            {
                this.waitConnectionList[i].Connect();
            }
        }


        /// <summary>
        /// 引发 Error 事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ex"></param>
        protected void OnError( object sender, Exception ex )
        {
            if ( this.ErrorEvent != null )
            {
                this.ErrorEvent( sender, ex );
            }
        }



        /// <summary>
        /// 开始连接
        /// </summary>
        private void StartConnect()
        {
            for ( int i = 0; i < this.waitConnectionList.Count; i++ )
            {
                this.waitConnectionList[i].Connect();
            }
        }
		
        /// <summary>
        /// socket 异常处理
        /// </summary>
        /// <param name="id"></param>
        /// <param name="err"></param>
        private void S_ErrorEvent( int id, SocketError err )
        {
        }

		/// <summary>
		/// socket 连接状态变更
		/// </summary>
		/// <param name="id"></param>
		/// <param name="status"></param>
        private void S_StatusChangeEvent( int id, bool status )
        {
			lock ( this )
			{
				if ( status )
				{
					if ( this.waitConnectionList.ContainsKey( id ) )
					{
						var s = this.waitConnectionList[id];

						if ( this.waitConnectionList.Remove( id ) )
						{
							this.connectedList.Add( id, s );
						}
					}
					else
					{
						throw new Exception();
					}
				}
				else
				{
					if ( this.connectedList.ContainsKey( id ) )
					{
						var s = this.connectedList[id];

						if ( this.connectedList.Remove( id ) )
						{
							if ( s.ConnectionRetryCount <= 3 )
							{
								this.waitConnectionList.Add( id, s );
							}
						}
					}
				}

				if ( this.ConnectedCountChange != null )
				{
					this.ConnectedCountChange(this, this.connectedList.Count );
				}
			}
        }
    }
}

