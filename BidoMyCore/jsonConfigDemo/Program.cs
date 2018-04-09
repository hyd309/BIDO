using System;

using Microsoft.Extensions.Configuration;

namespace jsonConfigDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("Class.json");

            var configuartion = builder.Build();

            Console.WriteLine($"ClassNo:{configuartion["ClassNo"]}");
            Console.WriteLine($"ClassDesc:{configuartion["ClassDesc"]}");

            Console.WriteLine($"Students");

            Console.Write(configuartion["Students:0:name"]);
            Console.WriteLine(configuartion["Students:0:age"]);

            Console.Write(configuartion["Students:1:name"]);
            Console.WriteLine(configuartion["Students:1:age"]);

            Console.Write(configuartion["Students:2:name"]);
            Console.WriteLine(configuartion["Students:2:age"]);

            Console.ReadLine();
        }
    }
}
