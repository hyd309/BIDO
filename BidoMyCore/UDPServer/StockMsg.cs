using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDPServer
{
    public abstract class StockMsg
    {
        public abstract void StockIn(string stockMsg);
    }
}
