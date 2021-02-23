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
        byte[] bf = new byte[1024* 64];
      
        int header = 0;
        int readIndex = 0;
        byte[] buffHead = new byte[4];
        //byte[] buff = new byte[1024];
        Bufferbyte bufferbyte = new Bufferbyte(1024 * 1024);
        public SocClient(string ip,int port) {
           
            if (isConnect == true) return;
            EventRegister();
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.Connect(new IPEndPoint(IPAddress.Parse(ip), port));
            isConnect = true;
            //if (_socket.Connected) {
                messageQueue = new MessageQueue<MessageNode>();
                messageQueue.Start();//开启消息读取
                                     //_socket.BeginReceive(bf, 0, bf.Length, SocketFlags.None, callback, null);
                Receive();
                //_socket.BeginReceive(buffHead, readIndex, 1, SocketFlags.None, callback, null);
            //}

        }
        private void Receive()
        {
          
            //开始读取接收到的消息
            _socket.BeginReceive(buffHead, readIndex, 1, SocketFlags.None, callback, null);
        }
        private void EventRegister() {
            EventDispatch.AddEventListener("Framework.MessageHandler","LoginSuccess");
        }
        private void callback(IAsyncResult ar)
        {
            //try {
                int count = _socket.EndReceive(ar);
             ///   Debug.Log(header+"start read message" + count);
                if (count == 0)
                {
                   // Debug.Log("Server is disconnect...");
                    _socket.Close();
                }
                else
                {
                    #region test
                    //byte[] b = new byte[1024];

                    // receiveCount += cc;
                    //  Debug.Log(Convert.ToBase64String(bf) );
                    //  byte[] decryptBuffer = AESEncrypt.Decrypt(bf);


                    //Bufferbyte bufferbyte = new Bufferbyte();
                    //bufferbyte.WriteBytes(bf);
                    // Debug.Log("receiveCount:"+ receiveCount + ",Len=" + bf.Length + ",cc=" + cc);


                    //Bufferbyte bufferbyte = new Bufferbyte();
                    //bufferbyte.WriteBytes(bf);
                    //CreateMessage(bufferbyte);
                    //bufferbyte.Clear();
                    //_socket.BeginReceive(bf, 0, bf.Length, SocketFlags.None, callback, _socket);
                    #endregion
                 //   Debug.Log("receive readIndex" + readIndex);
                    readIndex += count;
                    if (header == 0)
                    {
                    
                        //读取字符串长度
                        if (readIndex < 4)
                        {

                            Receive();
                        }
                        else
                        {
                            bufferbyte.WriteBytes(buffHead);
                            header = bufferbyte.ReadInt();
                           // Debug.Log("receive header" + header);
                            bufferbyte.Clear();
                            readIndex = 0;
                            _socket.BeginReceive(bufferbyte.GetByteBuffer(), readIndex, header, SocketFlags.None, callback, _socket);
                        }
                    }
                    else
                    {
                        if (header == readIndex)
                        {
                            bufferbyte.WriteIndex += readIndex;

                            CreateMessage(bufferbyte);
                            readIndex = 0;
                            header = 0;
                            bufferbyte.Clear();
                            Receive();
                        }
                        else
                        {
                            // Receive();
                        }
                    }
                }

            //}
            //catch (Exception e) {
            //    Debug.Log("Exception:{" + e);
            //}
            //finally {

            //}
          

        }
       
        /// <summary>
        /// handle message
        /// </summary>
        /// <param name="bufferbyte"></param>
        private void CreateMessage(Bufferbyte buffer)
        {
            string methodName = buffer.ReadString();
            Bufferbyte bufferbyte = new Bufferbyte(buffer.GetBytes().Length);
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