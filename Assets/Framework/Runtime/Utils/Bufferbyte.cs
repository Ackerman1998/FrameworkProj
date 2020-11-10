using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

class Bufferbyte
{
    int startIndex = 0;
    int readIndex = 0;
    byte[] buffer;
    public Bufferbyte(int bufferLen = 1024)
    {
        buffer = new byte[bufferLen];
    }
    public void WriteInt(int i)
    {
        byte[] buf = BitConverter.GetBytes(i);
        Array.Copy(buf, 0, buffer, startIndex, buf.Length);
        startIndex += buf.Length;
    }
    public void WriteFloat(float f)
    {
        byte[] buf = BitConverter.GetBytes(f);
        Array.Copy(buf, 0, buffer, startIndex, buf.Length);
        startIndex += buf.Length;
    }
    public void WriteString(string str)
    {
        byte[] buf = Encoding.UTF8.GetBytes(str);
        WriteInt(buf.Length);
        Array.Copy(buf, 0, buffer, startIndex, buf.Length);
        startIndex += buf.Length;
    }
    public void WriteBytes(byte[] buf)
    {
        Array.Copy(buf, 0, buffer, startIndex, buf.Length);
        startIndex += buf.Length;
    }
    public int ReadInt()
    {
        int i = BitConverter.ToInt32(buffer, readIndex);
        readIndex += 4;
        return i;
    }
    public float ReadFloat()
    {
        float i = BitConverter.ToInt64(buffer, readIndex);
        readIndex += 4;
        return i;
    }
    public string ReadString()
    {
        int len = ReadInt();
        string str = Encoding.UTF8.GetString(buffer, readIndex, len);
        readIndex += len;
        return str;
    }
    public void Clear()
    {
        Array.Clear(buffer, 0, startIndex);
    }
    public void SendMessage(Socket soc)
    {
       // Debug.Log(startIndex+","+buffer.Length);
        byte[] b = new byte[startIndex];

        Array.Copy(buffer, 0, b, 0, startIndex);
       
        soc.Send(b,SocketFlags.None);
    }
}