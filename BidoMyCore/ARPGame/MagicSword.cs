using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARPGame
{
    public class MagicSword
    {
        private Random _random = new Random();

        public void AttackTarget(Monster monster)
        {
            int loss = (_random.NextDouble() < 0.5) ? 100 : 200;
            if (loss==200)
            {
                Console.WriteLine("暴击...");
            }
            monster.Notify(loss);
        }
    }
}
