using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServ.Utils
{
    abstract class MessageQueue<T> 
    {
        protected System.Collections.Concurrent.ConcurrentQueue<T> queue = null;//用来存放数据的容器
        protected bool isRunning = false;//线程是否启动
        protected bool isWait = false;//信号
        protected System.Threading.Thread thread = null;
        protected Action action = null;
        protected System.Threading.EventWaitHandle eventWaitHandle = new System.Threading.AutoResetEvent(false);
       
    }
}
