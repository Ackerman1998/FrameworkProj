using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GameServ.Utils
{
    /// <summary>
    /// 工具类：处理byte
    /// </summary>
    class Bufferbyte
    {
        private int writeIndex = 0;
        private int readIndex = 0;
        byte[] buffer;

        public int WriteIndex { get => writeIndex; set => writeIndex = value; }
        public int ReadIndex { get => readIndex; set => readIndex = value; }

        public Bufferbyte(int bufferLen=1024*64) {
            buffer = new byte[bufferLen];
        }
        /// <summary>
        /// header=整个字符串长度,bufferLen数组长度=字符串长度+4
        /// </summary>
        /// <param name="header"></param>
        /// <param name="bufferLen"></param>
        public Bufferbyte(int header,int bufferLen)
        {
            buffer = new byte[bufferLen];
            WriteInt(header);
            writeIndex += 4;
        }
        public void WriteInt(int i)
        {
            byte [] buf = BitConverter.GetBytes(i);
            Array.Copy(buf,0,buffer, writeIndex, buf.Length);
            writeIndex += buf.Length;
        }
        public void WriteFloat(float f)
        {
            byte[] buf = BitConverter.GetBytes(f);
            Array.Copy(buf, 0, buffer, writeIndex, buf.Length);
            writeIndex += buf.Length;
        }
        public void WriteString(string str)
        {
            byte[] buf = Encoding.UTF8.GetBytes(str);
            WriteInt(buf.Length);
            Array.Copy(buf,0,buffer, writeIndex, buf.Length);
            writeIndex += buf.Length;
        }
        public void WriteBytes(byte [] buf) {
           // Console.WriteLine(buf.Length);
            Array.Copy(buf, 0, buffer, writeIndex, buf.Length);
            writeIndex += buf.Length;
        }
        /// <summary>
        /// 将目标字符写入
        /// </summary>
        /// <param name="buf"></param>
        /// <param name="startIndex"></param>
        public void WriteBytes(byte[] buf,int startIndex)
        {
            Array.Copy(buf, startIndex, buffer, writeIndex, buf.Length- startIndex);
            writeIndex += buf.Length - startIndex;
        }

        public byte[] GetBytes() {
            byte[] b = new byte[writeIndex];
            Array.Copy(buffer, 0, b, 0, writeIndex);
            Console.WriteLine("b="+b.Length);
            return b;
        }
        public byte[] GetByteBuffer()
        {
            //byte[] b = new byte[writeIndex];
            //Array.Copy(buffer, 0, b, 0, writeIndex);
            return buffer;
        }
        public int ReadInt() {
            int i= BitConverter.ToInt32(buffer,readIndex);
            readIndex += 4;
            return i;
        }
        public float ReadFloat()
        {
            float i = BitConverter.ToInt64(buffer, readIndex);
            readIndex += 4;
            return i;
        }
        public string ReadString() {
            int len = ReadInt();
            string str = Encoding.UTF8.GetString(buffer,readIndex,len);
            readIndex += len;
            return str;
        }
        public void Clear() {
            Array.Clear(buffer,0, writeIndex);
            writeIndex = 0;
            readIndex = 0;
        }

        public void SendMessage(Socket soc)
        {
            int len = WriteIndex;
            //Bufferbyte sendBuffer = new Bufferbyte(4,4+writeIndex);
            //sendBuffer.WriteBytes(buffer);
            byte[] b = new byte[writeIndex];
            Array.Copy(buffer, 0, b, 0, writeIndex);
            //  Console.WriteLine("准备发送:"+Encoding.UTF8.GetString(b));

            //在首部加入整个字符串长度


            //Bufferbyte bufferbyte = new Bufferbyte();
            //bufferbyte.WriteBytes(AESEncrypt.Encrypt(b));
            // soc.Send(AESEncrypt.Encrypt(b), SocketFlags.None);
         
            soc.Send(BitConverter.GetBytes(b.Length));
            soc.Send(b, SocketFlags.None);
            Console.WriteLine("发送消息" + b.Length+"成功");
        }
    }
}
/*
 消息发送规则：
 每段字符前加上字符的长度（除int类型数据外）
 发送整个字符串时，先发送字符串长度，再发送字符串的字节
 例如：发送一则消息("Login"+"hello"+123456)
       14
       5-Byte("Login")-5-Byte("hello")-Byte(123456)
     */
