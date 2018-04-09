using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace UDPClient
{
    class Program
    {
        static void Main(string[] args)
        {
            UDPSliceClient client = new UDPSliceClient();
            client.DoMain();
            Console.ReadLine();

            return;
            AddListMsg("device:123123123");
            Thread thread = new Thread(new ThreadStart(SendMsg));
            thread.Start();

            Thread th = new Thread(new ThreadStart(()=> {
                while (true)
                {
                    string msg = Console.ReadLine();
                    if (msg == "exit")
                    {
                        break;
                    }
                    else
                    {
                        AddListMsg(msg);
                    }
                }
            }));
            th.Start();
        }

        #region XXXXXXX可删除测试代码

        public static int func1(decimal num)
        {
            List<int> list = new List<int>();

            if (num < 1)
            {
                while (num != 0)
                {
                    num = num * 10;
                    int val = (int)(num / 1);
                    list.Add(val);
                    num = num - val;
                }
            }
            else
            {
                //处理为小于1的小数0.XX
            }

            int result = 0;
            for (int i = 0; i < list.Count; i++)
            {
                int aa = (list.Count - i - 1);
                for (int j = 0; j < aa; j++)
                {
                    list[i] = list[i] * 10;
                }
                result += list[i];
            }
            return result;
        }

        #endregion

        static Dictionary<string,string> listMsg = new Dictionary<string, string>();

        static void AddListMsg(string msg)
        {
            if (listMsg.ContainsValue(msg))
            {
                return;
            }
            else
            {
                System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
                string t = ((DateTime.Now.Ticks - startTime.Ticks)/ 10000).ToString();   //除10000调整为13位(精确到毫秒)
                while (listMsg.ContainsKey(t))
                {
                    Random random = new Random();
                    t = t + ""+ random.Next(0, 100);
                }
                listMsg.Add(t, msg);
            }
        }

        static void SendMsg()
        {
            while (true)
            {
                foreach (var dic in listMsg)
                {
                    Socket socket = new Socket(AddressFamily.InterNetwork,
                        SocketType.Dgram,
                        ProtocolType.Udp);
                    //客户端使用的终结点
                    EndPoint localEP = new IPEndPoint(IPAddress.Any, 0);
                    socket.Bind(localEP);

                    //启动异步接收
                    State state = new State(socket);
                    socket.BeginReceiveFrom(
                        state.Buffer, 0, state.Buffer.Length,
                        SocketFlags.None,
                        ref state.RemoteEP,
                        EndReceiveFromCallback,
                        state);

                    //向服务器发送信息
                    EndPoint remoteEP = new IPEndPoint(IPAddress.Parse("192.168.1.76"), 8002);
                    string sendMsg = dic.Key + "┥" + dic.Value;
                    SendMeaage(socket, remoteEP, sendMsg);
                }
                Console.WriteLine("消息队列："+listMsg.Count);
                Thread.Sleep(1000);
            }
        }

        //异步接收回调函数
        static void EndReceiveFromCallback(IAsyncResult iar)
        {
            State state = iar.AsyncState as State;
            Socket socket = state.Socket;
            try
            {
                //完成接收
                int byteRead = socket.EndReceiveFrom(iar, ref state.RemoteEP);
                ////显示服务器地址和端口
                //Console.WriteLine("服务器终结点：{0}", state.RemoteEP.ToString());
                ////显示接收信息
                //Console.WriteLine("接收数据字节数：{0}", byteRead);
                string message = Encoding.Default.GetString(state.Buffer, 0, byteRead);
                Console.WriteLine("来着服务器的信息：{0}", message);
                listMsg.Remove(message);
            }
            catch (Exception e)
            {
                Console.WriteLine("发生异常！异常信息：");
                Console.WriteLine(e.Message);
            }
            finally
            {
                socket.Close();
                socket.Dispose();
                ////非常重要：继续异步接收
                //socket.BeginReceiveFrom(
                //    state.Buffer, 0, state.Buffer.Length,
                //    SocketFlags.None,
                //    ref state.RemoteEP,
                //    EndReceiveFromCallback,
                //    state);
            }
        }

        /// <summary>
        /// 向服务器发送信息
        /// </summary>
        /// <param name="socket">本地Socket</param>
        /// <param name="remoteEndPoint">服务器终结点</param>
        /// <param name="Message">信息</param>
        static void SendMeaage(Socket socket, EndPoint remoteEndPoint, string Message)
        {
            byte[] bytes = Encoding.Default.GetBytes(Message);
            int sc=socket.SendTo(bytes, remoteEndPoint);
        }

        #region UDPClient异步通讯

        //static UdpClient udpClient;

        //static void UDPClientFunc()
        //{
        //    IPEndPoint localIpep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12345);
        //    udpClient = new UdpClient(localIpep);
        //    udpClient.BeginReceive(new AsyncCallback(CallBackFun),udpClient);
        //}

        //static void CallBackFun(IAsyncResult ar)
        //{
        //    try
        //    {
        //        IPEndPoint ip = new IPEndPoint(IPAddress.Any, 0);
        //        byte[] data = udpClient.EndReceive(ar,ref ip);
        //        State state=(State)ar.AsyncState;


        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}
        #endregion
    }

    /// <summary>
    /// 用于异步接收处理的辅助类
    /// </summary>
    class State
    {
        public State(Socket socket)
        {
            this.Buffer = new byte[1024];
            this.Socket = socket;
            this.RemoteEP = new IPEndPoint(IPAddress.Any, 0);
        }
        /// <summary>
        /// 获取本机Socket
        /// </summary>
        public Socket Socket { get; private set; }
        /// <summary>
        /// 获取接收缓冲区
        /// </summary>
        public byte[] Buffer { get; private set; }
        /// <summary>
        /// 获取/设置客户端终结点
        /// </summary>
        public EndPoint RemoteEP;
    }
}
