using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace UDPClient
{
    public class UDPSliceClient
    {
        int bufferSize = 50;
        private static int _staticid;
        int headInfoLength = 12;
        static Dictionary<int, Dictionary<int, byte[]>> listSend=new Dictionary<int, Dictionary<int, byte[]>>();        public static int NewPacketID        {
            get
            {
                if (_staticid > int.MaxValue - 2)
                {
                    _staticid = 0;
                }
                return ++_staticid;
            }        }
        public void DoMain()
        {
            //BufferProcesser(System.Text.Encoding.Default.GetBytes("abcdefghijklmnopqrstuvwxyz0123456789:abcdefghijklmnopqrstuvwxyz0123456789:abcdefghijklmnopqrstuvwxyz0123456789"));
            //BufferProcesser(System.Text.Encoding.Default.GetBytes("0123456789ABCDEFGHI"));

            Thread thread = new Thread(new ThreadStart(RemoveReceivedFromSend));
            thread.Start();


            while (true)
            {
                BufferProcesser(System.Text.Encoding.Default.GetBytes(Console.ReadLine()));
                foreach (var item in listSend)
                {
                    foreach (var dic in item.Value)
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
                        SendMeaage(socket, remoteEP, dic.Value);
                        Console.WriteLine("Send 一次");
                        //System.Threading.Thread.Sleep(2000);
                    }
                }
            }
        }
        private void SendMeaage(Socket socket, EndPoint remoteEndPoint, byte[] bytes)
        {
            int sc = socket.SendTo(bytes, remoteEndPoint);
        }
        private void BufferProcesser(byte[] buffer)        {
            int packId = NewPacketID;
            //分割后的 字节片段 数组
            if (buffer.Length <= bufferSize - 20)
            {
                byte[] bs = new byte[buffer.Length + headInfoLength];
                byte[] bytePacketMetaID = BitConverter.GetBytes(packId);
                byte[] bytetotalBuffSec = BitConverter.GetBytes(1);//总片段数量
                byte[] lengthIndex = BitConverter.GetBytes(0);//第几个片段（片段索引）
                Array.Copy(bytePacketMetaID, 0, bs, 0, bytePacketMetaID.Length);//0-3存放 包ID
                Array.Copy(bytetotalBuffSec, 0, bs, 4, bytetotalBuffSec.Length);//4-7存放片段 总量
                Array.Copy(lengthIndex, 0, bs, 8, lengthIndex.Length);//第8-11位存放 片段索引
                Array.Copy(buffer, 0, bs, headInfoLength, buffer.Length);
                Dictionary<int, byte[]> dic = new Dictionary<int, byte[]>();
                dic.Add(0, bs);
                listSend.Add(packId, dic);
            }
            else
            {
                //一共划分的 片段 数量
                int totalBuffSec = 0;
                totalBuffSec = (int)(buffer.Length / (bufferSize - 2));
                if (buffer.Length % (bufferSize - 2) > 0)
                {
                    totalBuffSec++;
                }
                int offset = 0;
                Dictionary<int, byte[]> dic = new Dictionary<int, byte[]>();
                for (int i = 0; ; i++)
                {
                    /*
                     * 数据包id    片段数量  索引        数据区域
                     * 0_________4_________8__________12______________________________
                    */
                    byte[] bs = new byte[(buffer.Length - offset > bufferSize - 2 ? bufferSize : buffer.Length - offset) + headInfoLength];
                    byte[] bytePacketMetaID = BitConverter.GetBytes(packId);
                    byte[] bytetotalBuffSec = BitConverter.GetBytes(totalBuffSec);//总片段数量
                    byte[] lengthIndex = BitConverter.GetBytes(i);//第几个片段（片段索引）
                    Array.Copy(bytePacketMetaID, 0, bs, 0, bytePacketMetaID.Length);//0-3存放 包ID
                    Array.Copy(bytetotalBuffSec, 0, bs, 4, bytetotalBuffSec.Length);//4-7存放片段 总量
                    Array.Copy(lengthIndex, 0, bs, 8, lengthIndex.Length);//第8-11位存放 片段索引
                    Array.Copy(buffer, offset, bs, headInfoLength, bs.Length - headInfoLength);//第12位开始存放 数据
                    dic.Add(i,bs);

                    offset += bs.Length - headInfoLength;
                    if (offset >= buffer.Length)
                    {
                        break;
                    }
                }
                listSend.Add(packId, dic);
            }        }

        //声明一个接收到的dic集合
        static Dictionary<int, Dictionary<int, bool>> listReceive = new Dictionary<int, Dictionary<int, bool>>();


        //异步接收回调函数
        private void EndReceiveFromCallback(IAsyncResult iar)
        {
            State state = iar.AsyncState as State;
            Socket socket = state.Socket;
            try
            {
                //完成接收
                int byteRead = socket.EndReceiveFrom(iar, ref state.RemoteEP);

                int packetMetaID = BitConverter.ToInt32(state.Buffer, 0);
                int totalBuffSec = BitConverter.ToInt32(state.Buffer, 4);
                int index = BitConverter.ToInt32(state.Buffer, 8);
                Console.WriteLine("packetMetaID=" + packetMetaID);
                Console.WriteLine("totalBuffSec=" + totalBuffSec);
                Console.WriteLine("index=" + index);
                Dictionary<int, bool> dic=new Dictionary<int, bool>();
                dic.Add(index, true);
                if (listReceive.ContainsKey(packetMetaID))
                {
                    listReceive[packetMetaID].Add(index, true);
                }
                else
                {
                    listReceive.Add(packetMetaID, dic);
                }
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
            }
        }

        /// <summary>
        /// 单独启动一个线程，去处理接收和发送集合
        /// </summary>
        private void RemoveReceivedFromSend()
        {
            while (true)
            {
                foreach (var key in listSend.Keys.ToArray<int>())
                {
                    if (listReceive.ContainsKey(key))
                    {
                        var receiveIndexBool = listReceive[key];
                        bool isAllReceived = true;
                        //判断所有此包下面所有片段接收完成，则移除
                        foreach (var sendIndexByte in listSend[key])
                        {
                            if (receiveIndexBool.ContainsKey(sendIndexByte.Key) && receiveIndexBool[sendIndexByte.Key])
                            {
                                //发送包的次片段如果为true则表示已经传递成功
                            }
                            else
                            {
                                isAllReceived = false;
                                break;
                            }
                        }
                        if (isAllReceived)
                        {
                            Console.WriteLine("移除集合:"+key);
                            listSend.Remove(key);
                            listReceive.Remove(key);
                        }
                    }
                }
            }
        }
    }
}
