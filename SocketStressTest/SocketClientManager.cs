using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SocketStressTestInterface;

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
        /// 客户端集合
        /// </summary>
        private Dictionary<int, SocketClient> socketTab;


        /// <summary>
        /// 异常事件
        /// </summary>
        public event Action<Exception> ErrorEvent;

        /// <summary>
        /// 已连接的连接数
        /// </summary>
        public int TotalConnected
        {
            get;
            private set;
        }

        public int TotalClosed
        {
            get;
            private set;
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
            this.socketTab = new Dictionary<int, SocketClient>( this.connCount );
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
                    this.OnError( f.Exception.GetBaseException() );
                    return;
                }

                if ( f.Result == null || f.Result.Length == 0 )
                {
                    this.OnError( new Exception( "域名或ip地址不正确,未能解析." ) );
                    return;
                }

                IPAddress addr = f.Result[0];
                IPEndPoint point = new IPEndPoint( addr, this.port );

                for ( int i = 0; i < this.connCount; i++ )
                {
                    SocketClient s = new SocketClient( i, point, sendTimeOut, receiveTimeOut );
                    s.SocketErrorEvent += S_SocketErrorEvent;
                    this.socketTab.Add( i, s );
                }

                StartConnect();
            } );
        }

        private void S_SocketErrorEvent( int id, SocketError err )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 停止客户端任务
        /// </summary>
        /// <returns></returns>
        public async Task StopAsync()
        {
            for ( int i = 0; i < this.socketTab.Count; i++ )
            {
                this.socketTab[i].Connect();
            }
        }


        protected void OnError( Exception ex )
        {
            if ( this.ErrorEvent != null )
            {
                this.ErrorEvent( ex );
            }
        }


        private void StartConnect()
        {
            for ( int i = 0; i < this.socketTab.Count; i++ )
            {
                this.socketTab[i].Connect();
            }
        }
    }
}

