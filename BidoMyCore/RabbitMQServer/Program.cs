using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RabbitMQ.Client;
namespace RabbitMQServer
{
    class Program
    {
        private static String QUEUE_NAME = "q1";
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.HostName = "192.168.1.76";
            factory.UserName = "whldz";//guest 账户不具备远程访问的权限
            factory.Password = "123456";
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    var consumer = new QueueingBasicConsumer(channel);
                    channel.BasicConsume(QUEUE_NAME,true,consumer);
                    while (true)
                    {
                        var ea = consumer.Queue.Dequeue();
                        var data = ea.Body;
                        var message = Encoding.UTF8.GetString(data);
                        Console.WriteLine(message);
                    }
                }
            }
        }
    }
}
