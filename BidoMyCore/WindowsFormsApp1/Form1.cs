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

namespace WinClient
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

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(richTextBox1.Text))
            {
                MessageBox.Show("请先输入待发送内容");
                return;
            }

            // 匿名发送  
            //udpcSend = new UdpClient(0);             // 自动分配本地IPv4地址  
            // 实名发送  
            IPEndPoint localIpep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12345); // 本机IP，指定的端口号  
            udpcSend = new UdpClient(localIpep);
            Thread thrSend = new Thread(SendMessage);
            thrSend.Start(richTextBox1.Text);
        }

        /// <summary>  
        /// 发送信息  
        /// </summary>  
        /// <param name="obj"></param>  
        private void SendMessage(object obj)
        {
            string message = (string)obj;
            byte[] sendbytes = Encoding.Unicode.GetBytes(message);
            IPEndPoint remoteIpep = new IPEndPoint(IPAddress.Parse(textBox1.Text), 8848); // 发送到的IP地址和端口号  
            var result= udpcSend.SendAsync(sendbytes, sendbytes.Length, remoteIpep);
            udpcSend.Close();
            bool isNotReturn = true;
            ResetTextBox(richTextBox1);
        }

        // 清空指定RichTextBox中的文本  
        delegate void ResetTextBoxDelegate(RichTextBox txtbox);
        private void ResetTextBox(RichTextBox txtbox)
        {
            if (txtbox.InvokeRequired)
            {
                ResetTextBoxDelegate resetTextBoxDelegate = ResetTextBox;
                txtbox.Invoke(resetTextBoxDelegate, new object[] { txtbox });
            }
            else
            {
                txtbox.Text = "";
            }
        }


    }
}
