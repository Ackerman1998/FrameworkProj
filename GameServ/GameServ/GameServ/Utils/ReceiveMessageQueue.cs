using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameServ.Utils
{
    class ReceiveMessageQueue<T> : MessageQueue<T>,IQueque<T>
    {
        private void Go() {

            if (!isRunning)
            {
                isRunning = true;
                action = new Action(DoRun);
                RunAction();
            }
        }
        private void RunAction() {
             action.BeginInvoke(RunEnd, action);
        }
        private void RunEnd(IAsyncResult ar)
        {
            action.EndInvoke(ar);
            if (queue.IsEmpty) {
                isWait = true;            
                eventWaitHandle.WaitOne();// 阻止当前线程，直到当前 System.Threading.WaitHandle 收到信号。      
                isWait = false;
            }       
            RunAction();
        }
        private void DoRun() {
            try {

                T str;
                bool b = queue.TryDequeue(out str);

                if (b)
                {

                    //这里对数据进行处理
                    if (str.GetType() == Type.GetType("GameServ.Utils.MessageNode"))
                    {
                        MessageNode node = str as MessageNode;
                        Console.WriteLine("执行方法：" + node.methodName);
                        EventDispatch.DispatchEvent(node.methodName, node);
                       // Console.WriteLine("执行方法：" + node.methodName+"完毕");
                    }
                    else {
                        Console.WriteLine("错误的数据类型...");

                    }
                }
            } catch (Exception e) {
                Console.WriteLine("[Queue Exception :"+e+"]"  );
            } finally {

            }
           
          
        }
        /// <summary>
        /// message Enqueue
        /// </summary>
        /// <param name="node"></param>
        public  void AppendMessage(T node) {
            queue.Enqueue(node);
            if (isWait)
            {
                //将事件状态设置为有信号，从而允许一个或多个等待线程继续执行。
                eventWaitHandle.Set();
            }
  
        }
        public  void Start() {
            if (thread==null) {
                queue = new ConcurrentQueue<T>();
                thread = new Thread(Go);
                thread.Start();
            }
        }

        
    }
   
}
/*
 数据处理类：
     开启线程处理接收到的数据
     使用Action异步执行处理数据的方法，若队列为空，则阻塞当前线程，不再读取数据。当有数据入队时
     停止线程堵塞，继续读取数据。这里我们用到EventWaitHandle来作为线程是否阻塞的信号
     EventWaitHandle使用：
     WaitOne()阻止当前线程，直到当前 System.Threading.WaitHandle 收到信号。
     Set()将事件状态设置为有信号，从而允许一个或多个等待线程继续执行。
              
     */
