using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
namespace Framework
{
    class MessageQueue<T> 
    {

        private ConcurrentQueue<T> queue=null;//用来存放数据的容器
        private bool isRunning = false;//线程是否启动
        private bool isWait = false;//信号
        private Thread thread = null;
        private Action action = null;
        private EventWaitHandle eventWaitHandle = new AutoResetEvent(false);
       
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
            if (queue.Count <= 0) return;
            try {

                T str;
                bool b = queue.TryDequeue(out str);

                if (b)
                {

                    //这里对数据进行处理
                    if (str.GetType() == Type.GetType("Framework.MessageNode"))
                    {
                        MessageNode node = str as MessageNode;
                        Debug.Log("执行方法：" + node.methodName);
                        EventDispatch.DispatchEvent(node.methodName, node);

                    }
                    else {
                        Debug.Log("错误的数据类型...");

                    }
                }
            } catch (Exception e) {
                Debug.Log("[ Exception :"+e+"]"  );
            } finally {

            }
           
          
        }
        /// <summary>
        /// message Enqueue
        /// </summary>
        /// <param name="node"></param>
        public void AppendMessage(T node) {
            queue.Enqueue(node);
            if (isWait)
            {
                //将事件状态设置为有信号，从而允许一个或多个等待线程继续执行。
                eventWaitHandle.Set();
            }
  
        }
        public void Start() {
            if (thread==null) {
                queue = new ConcurrentQueue<T>();
                thread = new Thread(Go);
                thread.Start();
            }
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose() {
            if (!isRunning) return;
           
            thread.Abort();
            thread = null;
            queue = null;
            eventWaitHandle.Set();//先关闭线程，解除堵塞
            Debug.Log("释放资源...");
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
