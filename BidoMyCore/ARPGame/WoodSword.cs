using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARPGame
{
    public class WoodSword : IAttachStrategy
    {
        public void AttackTarget(Monster monster)
        {
            monster.Notify(20);
        }
    }
}
