using System;
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
    class SendMessageQueue<T>
    {
        Thread thread = null;
        bool isRunning = false;
        bool isWait = false;

        delegate void CallBack();
        private Queue<T> queue = null;
     
        CallBack callBack;
        EventWaitHandle eventWaitHandle = new AutoResetEvent(false);
        public SendMessageQueue() {

        }
        public void Start() {
      
            if (!isRunning) {
                isRunning = true;
                queue = new Queue<T>();
                thread = new Thread(RunStart);
                thread.Start();
                callBack = new CallBack(DoRun);
                RunStart();

            }
        }
        public void AppednMessage(T t) {
            queue.Enqueue(t);
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
            
            T t = queue.Dequeue() ;
            if (t.GetType() == Type.GetType("GameServ.Utils.MessageNode"))
            {
                MessageNode node = t as MessageNode;
                node.bufferbyte.SendMessage(node.client);

            }
        }
    }
}
