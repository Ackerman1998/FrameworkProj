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
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public SocServer(string ip, int port)
        {
            Console.WriteLine(GetIP());
            clients = new Dictionary<int, Client>();
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
            // socket.Bind(new IPEndPoint(IPAddress.Parse("113.89.233.34"), port));
            socket.Listen(100);
            Console.WriteLine("Soc Service Start...");
        }
        private static string GetIP()
        {
            string tempip = "";
            try
            {
                WebRequest wr = WebRequest.Create("http://www.ip138.com/ips138.asp");
                Stream s = wr.GetResponse().GetResponseStream();
                StreamReader sr = new StreamReader(s, Encoding.Default);
                string all = sr.ReadToEnd(); //读取网站的数bai据
                int start = all.IndexOf("您的IP地址是：[") + 9;
                int end = all.IndexOf("]", start);
                tempip = all.Substring(start, end - start);
                sr.Close();
                s.Close();
            }
            catch
            {
            }
            return tempip;
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
       
      
    }
    class Client {
        private Socket clientSoc = null;
        private SocServer server = null;
        private string name = null;
        byte[] buff = new byte[1024];
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
                Bufferbyte bufferbyte = new Bufferbyte(1024);
                bufferbyte.WriteBytes(buff);
                string method = bufferbyte.ReadString();
                EventDispatch.DispatchEvent(method,bufferbyte);
                
                clientSoc.BeginReceive(buff, 0, buff.Length, SocketFlags.None, ReceiveCallBack, clientSoc);
            }
        }

        public void Close() {
            Console.WriteLine(name+" disconnect...");
            clientSoc.Close();
            clientSoc.Dispose();
        }
    }
}
