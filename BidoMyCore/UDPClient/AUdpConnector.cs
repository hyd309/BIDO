using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace UDPClient
{
    public class AUdpConnector
    {        private static AUdpConnector sinstance;        public static AUdpConnector Instance        {
            get
            {
                if (sinstance == null)
                {
                    sinstance = new AUdpConnector();
                }
                return sinstance;
            }        }

        /// <summary>
        /// 每个片段的 字节大小 
        /// </summary>
        int bufferSize = 4096;        UdpClient connector;        Action<byte[], IPEndPoint, int> delReceive;        public void Init(Action<byte[], IPEndPoint, int> receiveDel, int port)        {
            delReceive = receiveDel;
            OnBind(port);        }        private void OnBind(int port)        {
            connector = new UdpClient(port, AddressFamily.InterNetwork);
            OnStartReceive(connector);//启动异步接收调用
            OnStartToSend();//循环开始接收        }

        private void OnStartToSend()        {
            new Thread(new ThreadStart(DoSend)).Start();        }

        public static bool bQuit = false;        private void DoSend()        {
            while (true)
            {
                if (bQuit)
                {
                    break;
                }
                lock (sendLock)
                {
                    List<int> remove = new List<int>();
                    foreach (var pv in packetsToSendPool)
                    {
                        var p = pv.Value;
                        if (p.bOutOfTime)
                        {
                            remove.Add(pv.Key);
                            continue;
                        }
                        if (p.NeedRetry)
                        {
                            //超过200ms需要重发
                            byte[] buffer = new byte[p.buffer.Length + 8];
                            byte[] packetMetaIDBuff = BitConverter.GetBytes(p.packetMetaID);
                            byte[] packetIDBuff = BitConverter.GetBytes(p.packetID);
                            Array.ConstrainedCopy(packetMetaIDBuff, 0, buffer, 0, packetMetaIDBuff.Length);
                            Array.ConstrainedCopy(packetIDBuff, 0, buffer, 4, packetIDBuff.Length);
                            Array.ConstrainedCopy(p.buffer, 0, buffer, 8, p.buffer.Length);
                            connector.Send(buffer, buffer.Length, p.endpoint);

                            if (!p.bReliable)//广播形式发送的时候bReliable=false;其他发送形式都为true
                            {
                                remove.Add(pv.Key);
                            }
                        }
                    }
                    foreach (var r in remove)
                    {
                        //移除需要重发的数据包
                        packetsToSendPool.Remove(r);
                    }
                }

                Thread.Sleep(1);
            }        }

        #region 异步接收调用

        public void OnStartReceive(UdpClient s)        {
            if (s == null)
            {
                s = connector;
            }

            byte[] bs = new byte[bufferSize];
            s.BeginReceive(OnReceived, new object[] { s, bs });        }                /// <summary>
        /// 接收主函数
        /// </summary>
        /// <param name="ar"></param>        private void OnReceived(IAsyncResult ar)        {
            object[] objs = ar.AsyncState as object[];
            UdpClient s = objs[0] as UdpClient;
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            byte[] bs = s.EndReceive(ar, ref sender);
            int buffr = bs.Length;
            if (buffr == 0)
            {
                if (delReceive != null)
                {
                    delReceive(null, sender, 0);
                }
            }
            else
            {
                if (delReceive != null)
                {
                    lock (sendLock)
                    {
                        int packetMetaID = BitConverter.ToInt32(bs, 0);//总包的ID数量，toint32 默认转化从0开始的4位字节为整数
                        int packetID = BitConverter.ToInt32(bs, 4);//片段索引 转化从第4位开始，长度为4位的字节为整数；如果toLong则需要转化8位字节为long类型

                        int totalSec = BitConverter.ToInt32(bs, 8);//这个才是此包的分片数量
                        if (totalSec == 0)
                        {
                            int def = BitConverter.ToInt32(bs, 16);

                            byte[] packetBuff = new byte[bs.Length - 20];
                            Array.Copy(bs, 20, packetBuff, 0, packetBuff.Length);
                            if (def == (int)EPacketID.EUDPPacketReceived)
                            {
                                string sp = System.Text.Encoding.UTF8.GetString(packetBuff);
                                int ipid = 0;
                                int.TryParse(sp, out ipid);
                                if (packetsToSendPool.ContainsKey(ipid))
                                {
                                    packetsToSendPool.Remove(ipid);
                                }
                            }
                            else
                            {
                                OnSend("" + packetMetaID, sender.Address, sender.Port, (int)EPacketID.EUDPPacketReceived, false);
                                delReceive(packetBuff, sender, def);
                            }
                        }
                        else
                        {
                            OnSend("" + packetMetaID, sender.Address, sender.Port, (int)EPacketID.EUDPPacketReceived, false);

                            if (!dPendingPackets.ContainsKey(packetID))
                            {
                                dPendingPackets.Add(packetID, new Dictionary<int, byte[]>());
                            }
                            int index = BitConverter.ToInt32(bs, 12);
                            byte[] packetBuff = new byte[bs.Length - 16];
                            Array.Copy(bs, 16, packetBuff, 0, packetBuff.Length);
                            if (dPendingPackets[packetID].ContainsKey(index))
                            {
                                dPendingPackets[packetID][index] = packetBuff;
                            }
                            else
                            {
                                dPendingPackets[packetID].Add(index, packetBuff);
                            }
                            if (dPendingPackets[packetID].Count == totalSec)
                            {
                                ProcessingPacket(dPendingPackets[packetID], sender);
                            }
                        }
                    }
                }
                OnStartReceive(s);
            }        }

        #endregion

        private void ProcessingPacket(Dictionary<int, byte[]> dictionary, IPEndPoint ep)        {
            byte[] buffer = new byte[bufferSize * dictionary.Count];
            int total = 0;
            for (int i = 0; i < dictionary.Count; i++)
            {
                Array.Copy(dictionary[i], 0, buffer, total, dictionary[i].Length);
                total += dictionary[i].Length;
            }
            int def = BitConverter.ToInt32(buffer, 0);
            byte[] result = new byte[total - 4];
            Array.Copy(buffer, 4, result, 0, result.Length);
            delReceive(result, ep, def);        }

        Dictionary<int, Dictionary<int, byte[]>> dPendingPackets = new Dictionary<int, Dictionary<int, byte[]>>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer">整体传输字节大小</param>
        /// <returns></returns>
        private List<byte[]> BufferProcesser(byte[] buffer)        {
            //分割后的 字节片段 数组
            List<byte[]> result = new List<byte[]>();
            if (buffer.Length <= bufferSize - 20)
            {
                byte[] bs = new byte[buffer.Length + 8];
                Array.Copy(buffer, 0, bs, 8, buffer.Length);
                result.Add(bs);
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
                for (int i = 0; ; i++)
                {
                    byte[] bs = new byte[(buffer.Length - offset > bufferSize - 2 ? bufferSize : buffer.Length - offset) + 8];
                    byte[] lengthBuff = BitConverter.GetBytes(totalBuffSec);//总片段数量
                    byte[] lengthIndex = BitConverter.GetBytes(i);//第几个片段（片段索引）
                    Array.Copy(lengthBuff, 0, bs, 0, lengthBuff.Length);//0-3存放片段 总量
                    Array.Copy(lengthIndex, 0, bs, 4, lengthIndex.Length);//第4-7位存放 片段索引
                    Array.Copy(buffer, offset, bs, 8, bs.Length - 8);//第8位开始存放 数据
                    result.Add(bs);
                    offset += bs.Length - 8;
                    if (offset >= buffer.Length)
                    {
                        break;
                    }
                }
            }
            return result;        }        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="def">始终是=EUDPPacketReceived=1</param>
        /// <param name="bReliable"></param>        public void OnSend(string s, IPAddress ip, int port, int def, bool bReliable = true)        {
            OnSend(Encoding.UTF8.GetBytes(s), ip, port, def, bReliable);        }        private object sendLock = new object();        /// <summary>
        /// 数据报发送池
        /// </summary>        private Dictionary<int, UDPPacket> packetsToSendPool = new Dictionary<int, UDPPacket>();        /// <summary>
        /// 
        /// </summary>
        /// <param name="bs"></param>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="def"></param>
        /// <param name="bReliable"></param>        public void OnSend(byte[] bs, IPAddress ip, int port, int def, bool bReliable = true)        {
            lock (sendLock)
            {
                byte[] defbuff = BitConverter.GetBytes(def);
                byte[] buff = new byte[bs.Length + defbuff.Length];
                Array.Copy(defbuff, buff, defbuff.Length);
                Array.Copy(bs, 0, buff, 4, bs.Length);

                List<byte[]> buffers = BufferProcesser(buff);
                int packetID = UDPPacket.NewPacketID;
                foreach (var b in buffers)
                {
                    var p = new UDPPacket(b, new IPEndPoint(ip, port), packetID, bReliable);
                    packetsToSendPool.Add(p.packetMetaID, p);
                }
            }        }

        /// <summary>
        /// 广播形式发送
        /// </summary>
        /// <param name="s"></param>
        /// <param name="port"></param>
        public void OnSendOnBroadcast(string s, int port)        {
            OnSendOnBroadcast(Encoding.UTF8.GetBytes(s), port);        }
        /// <summary>
        /// 广播形式发送
        /// </summary>
        /// <param name="bs"></param>
        /// <param name="port"></param>        public void OnSendOnBroadcast(byte[] bs, int port)        {
            OnSend(bs, IPAddress.Broadcast, port, (int)EPacketID.EBroadcast, false);        }

        public bool server { get; set; }        public bool client { get; set; }
    }

    // packetID, total section count, index
    public class UDPPacket
    {        private static int _staticid;        public static int NewPacketID        {
            get
            {
                if (_staticid > int.MaxValue - 2)
                {
                    _staticid = 0;
                }
                return ++_staticid;
            }        }

        public byte[] buffer { get; private set; }        public IPEndPoint endpoint { get; private set; }        public DateTime tryTime { get; private set; }
        /// <summary>
        /// 包的序列号从0-int.max循环；==NewPacketID
        /// </summary>        public int packetID { get; private set; }        public int packetMetaID { get; private set; }        public int packetDef { get; private set; }        public int retryCount { get; private set; }        const int retryMS = 200;        const int maxRetryCount = 10;        public bool bReliable { get; private set; }        public UDPPacket(byte[] b, IPEndPoint ep, int pid, bool reliable)        {
            buffer = b;
            endpoint = ep;
            tryTime = DateTime.Now.AddMilliseconds(-retryMS);
            packetID = pid;
            bReliable = reliable;
            packetMetaID = NewPacketID;        }        /// <summary>
        /// 依赖重发次数，超过10次就=true算超时
        /// </summary>        public bool bOutOfTime { get; private set; }        /// <summary>
        /// 200毫秒内的不需要重发，否则更新tryTime重发，重发次数++
        /// 累计超过最大重发次数，则返回false
        /// </summary>        public bool NeedRetry        {
            get
            {
                if ((DateTime.Now - tryTime).TotalMilliseconds < retryMS)
                {
                    return false;
                }
                tryTime = DateTime.Now;
                retryCount++;
                if (retryCount > maxRetryCount)
                {
                    bOutOfTime = true;
                    return false;
                }
                return true;
            }        }
    }
    public enum EPacketID
    {        EUDPPacketReceived = 1,        EBroadcast,
    }
}
