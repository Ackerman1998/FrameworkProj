using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameServ.Test
{
    // 模拟一个处理消息队列的类
    class MessageHandler
    {
        // 消息队列
        private Queue<string> messageQue = new Queue<string>();
        private Thread th = null;
        private bool can = true;
        System.Collections.Concurrent.ConcurrentQueue<string> ts=new System.Collections.Concurrent.ConcurrentQueue<string>();
        // 处理消息队列的方法
        void HandlerMessage()
        {
            while (can)
            {
                if (messageQue.Count > 0)
                {
                    Thread.Sleep(1000);
                    Console.WriteLine(messageQue.Dequeue());
                }
            }
        }

        // 向消息队列中增加消息
        public void AppendMessage(string message)
        {
            messageQue.Enqueue(message);
        }

        // 开始 处理消息的线程
        public void Start()
        {
            if (th == null)
            {
                th = new Thread(HandlerMessage);
                th.Name = "HandlerMessage";
            }

            if (!th.IsAlive)
            {
                th.Start();
            }
        }

        // 结束 处理消息的线程
        public void Stop()
        {
            can = false;
        }
    }

}
