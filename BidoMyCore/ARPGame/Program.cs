using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARPGame
{
    class Program
    {
        static void Main(string[] args)
        {
            Monster m3 = new Monster(1000, "UU");
            Role role = new Role();
            role.Weapon = new WoodSword();
            role.Attack(m3);
            Console.ReadLine();
        }
    }
}
