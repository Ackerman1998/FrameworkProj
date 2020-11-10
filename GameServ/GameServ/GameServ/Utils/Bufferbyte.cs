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
            return b;
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
            byte[] b = new byte[writeIndex];
            Array.Copy(buffer, 0, b, 0, writeIndex);
            soc.Send(b, SocketFlags.None);
        }
    }
}
