using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace UDPServer
{
    public class UDPSliceServer
    {
        static Dictionary<int, ReceiveModel> dicReceive = new Dictionary<int, ReceiveModel>();
        int headInfoLength = 12;

        public void DoMain()
        {
            Socket socket = new Socket(AddressFamily.InterNetwork,
                     SocketType.Dgram,
                     ProtocolType.Udp);
            EndPoint localEP = new IPEndPoint(IPAddress.Parse("192.168.1.76"), 8002);
            socket.Bind(localEP);

            Console.WriteLine("开始接收");
            //启动异步接收
            State state = new State(socket);
            socket.BeginReceiveFrom(
                state.Buffer, 0, state.Buffer.Length,
                SocketFlags.None,
                ref state.RemoteEP,
                EndReceiveFromCallback,
                state);

            Thread thread = new Thread(new ThreadStart(HandleMessage));
            thread.Start();

            Console.ReadLine();
            socket.Close();
        }

        private void EndReceiveFromCallback(IAsyncResult iar)
        {
            Console.WriteLine("收到一次");
            State state = iar.AsyncState as State;
            Socket socket = state.Socket;
            int byteRead = socket.EndReceiveFrom(iar, ref state.RemoteEP);
            try
            {
                /*
                 * 数据包id    片段数量  索引        数据区域
                 * 0_________4_________8__________12______________________________
                */
                int packetMetaID = BitConverter.ToInt32(state.Buffer, 0);
                int totalBuffSec = BitConverter.ToInt32(state.Buffer, 4);
                Console.WriteLine("packetMetaID="+ packetMetaID+ " 和 totalBuffSec="+ totalBuffSec);
                int lengthIndex = BitConverter.ToInt32(state.Buffer, 8);
                byte[] data = new byte[byteRead- headInfoLength];
                Array.Copy(state.Buffer, headInfoLength, data, 0, byteRead- headInfoLength);
                if (dicReceive.ContainsKey(packetMetaID))
                {
                    if (dicReceive[packetMetaID].indexByteData.ContainsKey(lengthIndex))
                    {

                    }
                    else
                    {
                        dicReceive[packetMetaID].lengthBuff += data.Length;
                        dicReceive[packetMetaID].indexByteData.Add(lengthIndex, data);
                    }
                }
                else
                {
                    ReceiveModel model = new ReceiveModel();
                    model.packetMetaID = packetMetaID;
                    model.totalBuffSec = totalBuffSec;
                    model.lengthBuff += data.Length;
                    Dictionary<int, byte[]> dic = new Dictionary<int, byte[]>();
                    dic.Add(lengthIndex,data);
                    model.indexByteData = dic;
                    dicReceive.Add(packetMetaID,model);
                }

                byte[] backSend = new byte[headInfoLength];
                Array.Copy(state.Buffer, 0, backSend, 0, headInfoLength);//前12位字节数，发送给客户端

                socket.SendTo(backSend, state.RemoteEP);//向客户端发送信息
            }
            catch (Exception e)
            {
                Console.WriteLine("发生异常！异常信息：");
                Console.WriteLine(e.Message);
            }
            finally
            {
                //非常重要：继续异步接收
                socket.BeginReceiveFrom(
                    state.Buffer, 0, state.Buffer.Length,
                    SocketFlags.None,
                    ref state.RemoteEP,
                    EndReceiveFromCallback,
                    state);
            }
        }

        private void HandleMessage()
        {
            while (true)
            {
                foreach (var key in dicReceive.Keys.ToList<int>())
                {
                    ReceiveModel model = dicReceive[key];
                    if (model.totalBuffSec == model.indexByteData.Count)
                    {
                        //说明数据报接收完成
                        byte[] dataAll = new byte[model.lengthBuff];
                        int offIndex = 0;
                        for (int i = 0; i < model.totalBuffSec; i++)
                        {
                            //string msg1 = Encoding.Default.GetString(model.indexByteData[i]);
                            //Console.WriteLine(i+" -msg=" + msg1);
                            //从0开始-总长度，遍历 合并所有数据块
                            Array.Copy(model.indexByteData[i], 0, dataAll, offIndex, model.indexByteData[i].Length);
                            offIndex += model.indexByteData[i].Length;
                        }
                        string msg = Encoding.Default.GetString(dataAll);
                        Console.WriteLine("结果是：" + msg);
                        dicReceive.Remove(key);
                    }
                }
            }
        }
    }


    public class ReceiveModel
    {
        /// <summary>
        /// 数据报id
        /// </summary>
        public int packetMetaID { get; set; }
        /// <summary>
        /// 数据报总分割数量
        /// </summary>
        public int totalBuffSec { get; set; }
        /// <summary>
        /// 数据总字节长度
        /// </summary>
        public int lengthBuff { get; set; }
        /// <summary>
        /// 索引，数据片段(纯数据片段，不含头信息)
        /// </summary>
        public Dictionary<int, byte[]> indexByteData { get; set; }
    }
}
