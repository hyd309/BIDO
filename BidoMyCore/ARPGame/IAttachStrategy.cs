using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARPGame
{
    interface IAttachStrategy
    {
        void AttackTarget(Monster monster);
    }
}
