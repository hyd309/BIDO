using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ;
using RabbitMQ.Client;
     
namespace RabbitMQClient
{
    class Program
    {
        private static String QUEUE_NAME = "q1";
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.HostName ="192.168.1.76";
            factory.UserName = "whldz";//guest 账户不具备远程访问的权限
            factory.Password = "123456";
            using (var connection=factory.CreateConnection())
            {
                using (var channel=connection.CreateModel())
                {
                    channel.QueueDeclare(QUEUE_NAME, false, false, false, null);
                    string msg = "device:00000";
                    byte[] data = Encoding.UTF8.GetBytes(msg);
                    channel.BasicPublish("", QUEUE_NAME, null,data);
                }
            }
        }
    }
}
