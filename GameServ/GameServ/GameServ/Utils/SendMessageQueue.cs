using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameServ.Utils
{

    /// <summary>
    /// 发送queue
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class SendMessageQueue<T> : MessageQueue<T>, IQueque<T>
    {
        delegate void CallBack();
        CallBack callBack;
        public SendMessageQueue() {
            
        }
        public  void Start() {
      
            if (!isRunning) {
                isRunning = true;
                queue = new ConcurrentQueue<T>();
                thread = new Thread(RunStart);
                thread.Start();
                callBack = new CallBack(DoRun);
                RunStart();
                
            }
        }
        public  void AppendMessage(T node) {
            queue.Enqueue(node);
            if (!isWait) {
                eventWaitHandle.Set();
            }
        }
        private void RunStart() {
            callBack.BeginInvoke(RunEnd, callBack);
        }
        private void RunEnd(IAsyncResult ac) {
            callBack.EndInvoke(ac);
            if (queue.Count==0) {
                isWait = false;
                eventWaitHandle.WaitOne();
                isWait = true;
            }
            RunStart();
        }
        private void DoRun() {
            T t;
            bool b  = queue.TryDequeue(out t) ;
            if (b) {
                if (t.GetType() == Type.GetType("GameServ.Utils.MessageNode"))
                {
                    MessageNode node = t as MessageNode;
                    node.bufferbyte.SendMessage(node.client);

                }
            }
            
        }
    }
}
