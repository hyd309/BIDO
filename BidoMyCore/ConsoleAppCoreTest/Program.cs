using System;
using System.Collections.Generic;

namespace ConsoleAppCoreTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Cat cat = new Cat();
            new Master(cat);
            new Mouse(cat);
            cat.Warning();
            Console.ReadKey();
        }       
}
}
