using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
namespace Framework { 
     class SocClient 
    {
        private bool isConnect = false;//是否连接上
        Socket _socket = null;
        MessageQueue<MessageNode> messageQueue = null;
        byte[] bf = new byte[1024*64];
        public SocClient(string ip,int port) {
           
            if (isConnect == true) return;
            EventRegister();
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.Connect(new IPEndPoint(IPAddress.Parse(ip), port));
            isConnect = true;
            messageQueue = new MessageQueue<MessageNode>();
            messageQueue.Start();//开启消息读取
            _socket.BeginReceive(bf, 0, bf.Length, SocketFlags.None, callback, null);

        }
   
        private void EventRegister() {
            EventDispatch.AddEventListener("Framework.MessageHandler","LoginSuccess");
        }
        private void callback(IAsyncResult ar)
        {
        
            int cc = _socket.EndReceive(ar);
            if (cc == 0)
            {
                Debug.Log("Server is disconnect...");
                _socket.Close();
            }
            else
            {
                Bufferbyte bufferbyte = new Bufferbyte();
                bufferbyte.WriteBytes(bf);
                CreateMessage(bufferbyte);
                bufferbyte.Clear();
                _socket.BeginReceive(bf, 0, bf.Length, SocketFlags.None, callback, _socket);
               
            }

        }
        /// <summary>
        /// handle message
        /// </summary>
        /// <param name="bufferbyte"></param>
        private void CreateMessage(Bufferbyte buffer)
        {
            string methodName = buffer.ReadString();
            Bufferbyte bufferbyte = new Bufferbyte();
            bufferbyte.WriteBytes(buffer.GetBytes(), buffer.ReadIndex);
            MessageNode messageNode = new MessageNode();
            messageNode.methodName = methodName;
            messageNode.bufferbyte = bufferbyte;
            messageNode.client = _socket;
            AddDataToReceive(messageNode);
        }

        private void AddDataToReceive(MessageNode messageNode)
        {
            messageQueue.AppendMessage(messageNode);
        }

        public void SendMessage(Bufferbyte bufferbyte)
        {
            if (!isConnect)
            {
                Debug.Log("Network error");
            }
            else
            {
                bufferbyte.SendMessage(_socket);
            }
      
        }
        public void Dispose() {
            //if (!isConnect)
            //{
            //    Debug.Log("Network error");
            //}
            //else {
                _socket.Close();
                _socket.Dispose();
                _socket = null;
               messageQueue.Dispose();
            //Debug.Log("释放资源");
            //}
        
        }
    }
}