using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDPServer
{
    public class TableStoreMsg: StockMsg
    {
        public override void StockIn(string stockMsg)
        {
            Console.WriteLine("TableStoreMsg输出："+stockMsg);
        }
    }
}
