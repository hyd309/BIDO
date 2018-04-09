using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppCoreTest
{
    public class Master : IObserver
    {
        public Master(Cat c)
        {
            c.attach(this);
        }
        public void DoSomething()
        {
            Console.WriteLine("主人醒了");
        }
    }
}
