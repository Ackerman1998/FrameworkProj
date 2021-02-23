using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServ.Utils
{
    interface IQueque<T>
    {
       void AppendMessage(T node);
       void Start();
     
    }
}
