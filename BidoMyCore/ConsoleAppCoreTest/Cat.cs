using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppCoreTest
{
    public class Cat
    {
        private List<IObserver> list = new List<IObserver>();

        public void attach(IObserver ob)
        {
            list.Add(ob);
        }

        public void Warning()
        {
            Console.WriteLine("Hello World!");
            foreach (IObserver ob in list)
            {
                ob.DoSomething();
            }
        }
    }
}
