using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GameServ.Utils
{
    class AESEncrypt
    {
        /// <summary>
        /// AES加密字节流
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="keyStr"></param>
        /// <param name="ivStr"></param>
        /// <returns></returns>
        public static byte [] Encrypt(byte [] buffer, string keyStr= "abcdef0123456789abcdef0123456789", string ivStr= "0000000000000000") {
            if (buffer.Length==0 || string.IsNullOrEmpty(keyStr) || string.IsNullOrEmpty(ivStr))
            {
                return null;
            }
            else {
                string base64Str = Convert.ToBase64String(buffer);//转base64
                Console.WriteLine("base64str : "+base64Str);
                //Console.WriteLine("bufferStr : " + Encoding.UTF8.GetString(buffer));
                byte[] encryptBuffer;
                Aes aes = Aes.Create();
                aes.Key = Encoding.UTF8.GetBytes(keyStr);
                aes.IV = Encoding.UTF8.GetBytes(ivStr);
                ICryptoTransform cryptoTransform = aes.CreateEncryptor();
                using (MemoryStream ms =new MemoryStream()) {
                    using (CryptoStream cryptoStream=new CryptoStream(ms,cryptoTransform,CryptoStreamMode.Write)) {
                        using (StreamWriter streamWriter=new StreamWriter(cryptoStream)) {
                            streamWriter.Write(base64Str);//将字符串加密
                        }
                        encryptBuffer = ms.ToArray();
                    }
                }
             
               Console.WriteLine("Encrypt str=" + Convert.ToBase64String(encryptBuffer));//得到加密后的字符串
                return encryptBuffer; 
            }
        }

        /// <summary>
        /// AES解密字节流
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="keyStr"></param>
        /// <param name="ivStr"></param>
        /// <returns></returns>
        public static byte[] Decrypt(byte[] buffer, string keyStr = "abcdef0123456789abcdef0123456789", string ivStr = "0000000000000000") {
            if (buffer.Length == 0 || string.IsNullOrEmpty(keyStr) || string.IsNullOrEmpty(ivStr))
            {
                return null;
            }
            else
            {

                string str;
               // byte[] encryptBuffer;
                Aes aes = Aes.Create();
                aes.Key = Encoding.UTF8.GetBytes(keyStr);
                aes.IV = Encoding.UTF8.GetBytes(ivStr);
                ICryptoTransform cryptoTransform = aes.CreateDecryptor();
                using (MemoryStream ms = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(ms, cryptoTransform, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader=new StreamReader(cryptoStream)) {
                            str = streamReader.ReadToEnd();
                           
                        }
                    }
                    Console.WriteLine("Decrypt str=" + Encoding.UTF8.GetString(Convert.FromBase64String(str)));
                }

                return Convert.FromBase64String(str);
            }
        }
    }
}
