using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppCoreTest
{
    public class Mouse : IObserver
    {

        public Mouse(Cat c)
        {
            c.attach(this);
        }

        public void DoSomething()
        {
            Console.WriteLine("老鼠跑了");
        }
    }
}
