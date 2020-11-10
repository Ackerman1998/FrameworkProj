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
        private MessageQueue<MessageNode> receiveQueue = new MessageQueue<MessageNode>();//接收queue
        private SendMessageQueue<MessageNode> sendQueue = new SendMessageQueue<MessageNode>();//发送queue
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public SocServer(string ip, int port)
        {
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
        byte[] buff = new byte[1024];
        Bufferbyte bufferbyte = new Bufferbyte(1024*1024);
        public Client(Socket soc,string name,SocServer server) {
            this.clientSoc = soc;
            this.name = name;
            this.server = server;
            Receive();
        }
        private void Receive() {
            clientSoc.BeginReceive(buff, 0,buff.Length,SocketFlags.None ,ReceiveCallBack, clientSoc);
        }
        private void ReceiveCallBack(IAsyncResult ar)
        {
            Socket ss = ar.AsyncState as Socket;
            int count =  clientSoc.EndReceive(ar);
            if (count == 0)
            {
                Close();
            }
            else {
               
                bufferbyte.WriteBytes(buff);
                //string method = bufferbyte.ReadString();
                //EventDispatch.DispatchEvent(method,bufferbyte);
                CreateMessage(bufferbyte);
                bufferbyte.Clear();
                clientSoc.BeginReceive(buff, 0, buff.Length, SocketFlags.None, ReceiveCallBack, clientSoc);
            }
        }
        /// <summary>
        /// handle message
        /// </summary>
        /// <param name="bufferbyte"></param>
        private void CreateMessage(Bufferbyte buffer) {
            string methodName = buffer.ReadString();
            Bufferbyte bufferbyte = new Bufferbyte();
            bufferbyte.WriteBytes(buffer.GetBytes(),buffer.ReadIndex);
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
/*
通信数据的格式：
方法名+若干个参数 （例如：string + int + int ==> "Login" + 123456 + 123456）
     
     */