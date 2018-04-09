using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARPGame
{
    public class Monster
    {
        public Monster(int hp,string name)
        {
            HP = hp;
            Name = name;
        }

        public int HP { get; set; }
        public String Name { get; set; }

        public void Notify(int hpOff)
        {

            if (HP <= 0)
            {
                Console.WriteLine(this.Name + "死亡");
            }
            else {
                this.HP = this.HP - hpOff;
                Console.WriteLine(Name +"持续"+ HP);
            }
        }

    }
}
