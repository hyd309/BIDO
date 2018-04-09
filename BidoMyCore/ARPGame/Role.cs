using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARPGame
{
    internal sealed class Role
    {
        //用的什么剑
        public IAttachStrategy Weapon { get; set; }

        /// <summary>
        /// 攻击怪兽
        /// </summary>
        /// <param name="monster"></param>
        public void Attack(Monster monster)
        {
            this.Weapon.AttackTarget(monster);
        }
    }
}
