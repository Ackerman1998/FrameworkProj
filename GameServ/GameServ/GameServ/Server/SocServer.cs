using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using GameServ.Utils;
namespace GameServ.Server
{
    class SocServer
    {
        private Socket socket = null;
        private Dictionary<int, Client> clients=null;
        private IQueque<MessageNode> receiveQueue = new ReceiveMessageQueue<MessageNode>();//接收queue
        private IQueque<MessageNode> sendQueue = new SendMessageQueue<MessageNode>();//发送queue
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public SocServer(string ip, int port)
        {
            //Console.WriteLine("Log:"+this);
            //开启接收
            receiveQueue.Start();
            //sendQueue.Start();
            clients = new Dictionary<int, Client>();
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
            // socket.Bind(new IPEndPoint(IPAddress.Parse("113.89.233.34"), port));
            socket.Listen(100);
            Console.WriteLine("Soc Service Start...");
        }
       
        public void StartAccept()
        {
            socket.BeginAccept(new AsyncCallback(NetCallBack), socket);
        }
        private void NetCallBack(IAsyncResult ar)
        {
            Socket server = ar.AsyncState as Socket;
            Socket client = server.EndAccept(ar);
            IPEndPoint iPEndPoint = client.RemoteEndPoint as IPEndPoint;
            string clientIP = iPEndPoint.Address.ToString();
            Client cc = new Client(client,clientIP,this);
            Console.WriteLine(clientIP + "connecting...");
            server.BeginAccept(new AsyncCallback(NetCallBack), socket);
            //client.Send(Encoding.UTF8.GetBytes("connect success"));

            //byte[] buffer = new byte[1024 * 1024];
            //int count = client.Receive(buffer);
            //string str = Encoding.UTF8.GetString(buffer, 0, count);


        }
        /// <summary>
        /// 异步处理接收到的消息
        /// </summary>
        /// <param name="mn"></param>
        public void AddDataToReceive(MessageNode mn) {
            receiveQueue.AppendMessage(mn);
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        public void AddDataToSend()
        {

        }
    }
    class Client {
        private Socket clientSoc = null;
        private SocServer server = null;
        private string name = null;
        int header = 0;
        int readIndex = 0;
        byte[] buff = new byte[1024];
        byte[] buffHead = new byte[4];
        Bufferbyte bufferbyte = new Bufferbyte(1024*1024);
        public Client(Socket soc,string name,SocServer server) {
            this.clientSoc = soc;
            this.name = name;
            this.server = server;
            Receive();
        }
        private void Receive() {
            //开始读取接收到的消息
            clientSoc.BeginReceive(buffHead, readIndex, 4,SocketFlags.None ,ReceiveCallBack, clientSoc);
        }
        private void ReceiveCallBack(IAsyncResult ar)
        {
          
            Socket ss = ar.AsyncState as Socket;
            try
            {
                int count = clientSoc.EndReceive(ar);
                if (count == 0)
                {
                    Close();
                }
                else
                {
                    #region test code
                    /*
                    // Console.WriteLine("buff.length=" + Encoding.UTF8.GetString(buff));
                    //bufferbyte.WriteBytes(buff);
                    ////string method = bufferbyte.ReadString();
                    ////EventDispatch.DispatchEvent(method,bufferbyte);
                    //CreateMessage(bufferbyte);
                    //bufferbyte.Clear();
                    //clientSoc.BeginReceive(buff, 0, buff.Length, SocketFlags.None, ReceiveCallBack, clientSoc);
                    */
                    #endregion
                    readIndex += count;
                    if (header == 0)
                    {
                            //读取字符串长度
                            bufferbyte.WriteBytes(buffHead);
                            header = bufferbyte.ReadInt();
                            bufferbyte.Clear();
                            readIndex = 0;
                            clientSoc.BeginReceive(bufferbyte.GetByteBuffer(), readIndex, header, SocketFlags.None, ReceiveCallBack, clientSoc);
                    }
                    else {
                        if (header == readIndex)
                        {
                            bufferbyte.WriteIndex += header;
                            CreateMessage(bufferbyte);
                            readIndex = 0;
                            header = 0;
                            bufferbyte.Clear();
                            Receive();
                        }
                        else {
                           // Receive();
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("接收消息异常:"+e);
                //Close();
                readIndex = 0;
                header = 0;
                bufferbyte.Clear();
                Receive();
            }
            finally {
                
            }
         
        }
        /// <summary>
        /// handle message
        /// </summary>
        /// <param name="bufferbyte"></param>
        private void CreateMessage(Bufferbyte buffer) {
                string methodName = buffer.ReadString();       
                Bufferbyte bufferbyte = new Bufferbyte(buffer.GetBytes().Length);
                bufferbyte.WriteBytes(buffer.GetBytes(), buffer.ReadIndex);
                MessageNode messageNode = new MessageNode();
                messageNode.methodName = methodName;
                messageNode.bufferbyte = bufferbyte;
                messageNode.client = clientSoc;
                server.AddDataToReceive(messageNode);
            
           
        }
        public void Close() {
            Console.WriteLine(name+" disconnect...");
            clientSoc.Close();
            clientSoc.Dispose();
        }
    }
}
/*读取规则
通信数据的格式：
方法名+若干个参数 （例如：string + int + int ==> "Login" + 123456 + 123456）

读取规则：
定义两个字节数组，分别用来读取第一段的长度，第二段的数据部分。
首先读取长度，读取成功后用header存放数据部分的长度
继续读取，读取到数据存放到特定的数据结构中，再入消息队列进行消息读取处理执行；
完毕后初始化之前的参数，再递归开启消息读取。
套接字异步开启消息读取
  clientSoc.BeginReceive(buffHead, readIndex, 4,SocketFlags.None ,ReceiveCallBack, clientSoc);
  public IAsyncResult BeginReceive(byte[] buffer, int offset, int size, SocketFlags socketFlags, AsyncCallback callback, object state);
  buffer:接收消息的数组
  offset:起始位置
  size:消息长度
  例如  clientSoc.BeginReceive(buffHead[10], 0, 5,SocketFlags.None ,ReceiveCallBack, clientSoc);
  接收到的消息：JNMXCAAAAA,A代表空白
     */
