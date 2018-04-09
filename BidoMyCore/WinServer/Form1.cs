using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace WinServer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        /// <summary>  
        /// 用于UDP发送的网络服务类  
        /// </summary>  
        private UdpClient udpcSend;
        /// <summary>  
        /// 用于UDP接收的网络服务类  
        /// </summary>  
        private UdpClient udpcRecv;

        delegate void ShowMessageDelegate(RichTextBox txtbox, string message);
        private void ShowMessage(RichTextBox txtbox, string message)
        {
            if (txtbox.InvokeRequired)
            {
                ShowMessageDelegate showMessageDelegate = ShowMessage;
                txtbox.Invoke(showMessageDelegate, new object[] { txtbox, message });
            }
            else
            {
                txtbox.Text += message + "\r\n";
            }
        }
        /// <summary>  
        /// 开关：在监听UDP报文阶段为true，否则为false  
        /// </summary>  
        bool IsUdpcRecvStart = false;
        /// <summary>  
        /// 线程：不断监听UDP报文  
        /// </summary>  
        Thread thrRecv;

        private void button2_Click(object sender, EventArgs e)
        {
            if (!IsUdpcRecvStart) // 未监听的情况，开始监听  
            {
                IPEndPoint localIpep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8848); // 本机IP和监听端口号  
                udpcRecv = new UdpClient(localIpep);
                thrRecv = new Thread(ReceiveMessage);
                ShowMessage(richTextBox2, "n:" + thrRecv.ThreadState.ToString());
                thrRecv.Start();
                IsUdpcRecvStart = true;
                ShowMessage(richTextBox2, "UDP监听器已成功启动");
            }
            else // 正在监听的情况，终止监听  
            {
                ShowMessage(richTextBox2, "S:" + thrRecv.ThreadState.ToString());
                thrRecv.Abort(); // 必须先关闭这个线程，否则会异常  
                udpcRecv.Close();
                IsUdpcRecvStart = false;
                ShowMessage(richTextBox2, "P:" + thrRecv.ThreadState.ToString());
                ShowMessage(richTextBox2, "UDP监听器已成功关闭");
            }
        }
        private void ReceiveMessage(object obj)
        {
            IPEndPoint remoteIpep = new IPEndPoint(IPAddress.Any, 0);
            while (true)
            {
                try
                {
                    byte[] bytRecv = udpcRecv.Receive(ref remoteIpep);
                    string message = Encoding.Unicode.GetString(bytRecv, 0, bytRecv.Length);
                    ShowMessage(richTextBox2, string.Format("{0}[{1}]", remoteIpep, message));
                }
                catch (Exception ex)
                {
                    ShowMessage(richTextBox2, ex.Message);
                    break;
                }
            }
        }
    }
}
