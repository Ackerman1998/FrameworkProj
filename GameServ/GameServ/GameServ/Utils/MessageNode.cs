using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GameServ.Utils
{
    class MessageNode
    {
       public Bufferbyte bufferbyte;
       public string methodName;
        public Socket client;
    }
}
