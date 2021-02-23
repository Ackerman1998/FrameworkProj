using GameServ.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LitJson;
using GameServ.Utils;
using GameServ.Test;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
namespace GameServ
{
    class Program
    {
        static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }

        #region test void
        /// <summary>
        /// TcpClient
        /// </summary>
        //static void Main(string[] args)
        //{
        //    HttpServer httpServer = new MyHttpServer(8080);

        //    Thread thread = new Thread(new ThreadStart(httpServer.listen));

        //    thread.Start();
        //    Console.ReadKey();
        //}
        //static Socket _socket = null;

        //static void Main(string[] args)
        //{
        //    _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //    _socket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8082));
        //    _socket.Listen(100);
        //    _socket.BeginAccept(callback, _socket);
        //    Console.ReadKey();
        //}

        //private static void callback(IAsyncResult ar)
        //{
        //    Socket socket = ar.AsyncState as Socket;
        //    Socket client = socket.EndAccept(ar);
        //    client.Send(Encoding.UTF8.GetBytes("hello!"));
        //    socket.BeginAccept(callback, socket);
        //    byte[] receive_buffer = new byte[1024 * 60];
        //    int recv = client.Receive(receive_buffer);
        //    string recv_str = Encoding.UTF8.GetString(receive_buffer);
        //    Console.WriteLine("接收消息：" + recv_str);
        //}
        #endregion
        static Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);  //侦听socket
        static void Main(string[] args)
        {
            #region test code
            //test Messagequeue

            //MessageQueue<string> message = new MessageQueue<string>();
            //message.Start();
            //Console.WriteLine("5秒后开启线程1，2...");
            //Thread.Sleep(5000);
            //new Thread(() =>
            //{
            //    for (int i = 0; i < 100; i++)
            //    {
            //        Thread.Sleep(100);
            //        message.AppendMessage("test void1 " + i);
            //    }

            //}).Start();
            //new Thread(() =>
            //{
            //    for (int i = 0; i < 100; i++)
            //    {
            //        Thread.Sleep(200);
            //        message.AppendMessage("test void2 " + i);
            //    }

            //}).Start();
            //for (int i = 0; i < 100; i++)
            //{
            //    message.AppendMessage("test void2 " + i);
            //}
            //MessageHandler message = new MessageHandler();
            //message.Start();
            //for (int i=0;i<100;i++) {
            //    message.AppendMessage("test void " + i);
            //}
            //while (true) {
            //    Console.WriteLine("请输入一个邮箱");
            //    string email = Console.ReadLine();
            //    Regex regex = new Regex(@"^(\w)+(\.\w)*@(\w)+((\.\w+)+)$");
            //    if (regex.IsMatch(email))
            //    {
            //        Console.WriteLine("邮箱格式正确。");
            //    }
            //    else
            //    {
            //        Console.WriteLine("邮箱格式不正确。");
            //    }
            //}
            #endregion

            #region Net 
            RegisterVoid();//注册一些方法
            GameServ.Server.SocServer hs = new Server.SocServer("127.0.0.1", 8081);
            hs.StartAccept();
            #endregion

            #region code Security
            byte [] b= AESEncrypt.Encrypt(Encoding.UTF8.GetBytes("Here is some data to encrypt!"), "abcdef0123456789abcdef0123456789", "0000000000000000");
            AESEncrypt.Decrypt(b, "abcdef0123456789abcdef0123456789", "0000000000000000");
            //TestSecurity("Here is some data to encrypt!");
            //TestSecurity("Here is some data to encrypt!");
            //string original = "Here is some data to encrypt!";

            //// Create a new instance of the Aes
            //// class.  This generates a new key and initialization
            //// vector (IV).
            //using (Aes myAes = Aes.Create())
            //{

            //    // Encrypt the string to an array of bytes.
            //    byte[] encrypted = EncryptStringToBytes_Aes(original, myAes.Key, myAes.IV);

            //    // Decrypt the bytes to a string.
            //    string roundtrip = DecryptStringFromBytes_Aes(encrypted, myAes.Key, myAes.IV);

            //    //Display the original data and the decrypted data.
            //    Console.WriteLine("Original:   {0}", original);
            //    Console.WriteLine("Round Trip: {0}", Encoding.UTF8.GetString(encrypted));
            //}
            #endregion
            Console.ReadKey();
        }
        private static void TestSecurity(string str) {
            Aes myAes = Aes.Create();
            //指定key，若不指定key则每次自动生成key
            myAes.Key = Encoding.UTF8.GetBytes("abcdef0123456789abcdef0123456789");//32位
            myAes.IV = Encoding.UTF8.GetBytes("0000000000000000");//16位
            byte[] encryStr = EncryStringToBytes_AES(str,myAes.Key,myAes.IV);
            Console.WriteLine("AES String ="+Encoding.UTF8.GetString(encryStr));
        }
        private static byte[] EncryStringToBytes_AES(string str,byte [] key,byte [] iv) {
            if (string.IsNullOrEmpty(str) || key.Length == 0 || iv.Length == 0)
            {
                Console.Write("null");
                return null;
            }
            else {
                byte[] returnBuffer;
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = key;
                    aesAlg.IV = iv;
                    Console.WriteLine("key="+Encoding.UTF8.GetString(key));
                    Console.WriteLine("iv=" + Encoding.UTF8.GetString(iv));
                    // Create an encryptor to perform the stream transform.
                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                    // Create the streams used for encryption.
                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            {
                                swEncrypt.Write(str);                         
                            }
                            returnBuffer = msEncrypt.ToArray();
                        }
                    }
                }
                return returnBuffer;
            }
        }
        static void RegisterVoid() {
            EventDispatch.AddEventListener("GameServ.Logic.ClientLogic","Login");
            EventDispatch.AddEventListener("GameServ.Logic.ClientLogic","SendSuccess");
          
        }
        static void StartSoc() {
            Console.WriteLine("Http start...");
            _socket.Bind(new IPEndPoint(IPAddress.Any, 8081));
            _socket.Listen(100);
            _socket.BeginAccept(new AsyncCallback(OnAccept), _socket);  //开始接收来自浏览器的http请求（其实是socket连接请求）
            Console.Read();
        }
        static void OnAccept(IAsyncResult ar)
        {
            try
            {
                Socket socket = ar.AsyncState as Socket;
                Socket new_client = socket.EndAccept(ar);  //接收到来自浏览器的代理socket
                //NO.1  并行处理http请求
                //new_client.Send(Encoding.UTF8.GetBytes("hello!"));
                socket.BeginAccept(new AsyncCallback(OnAccept), socket); //开始下一次http请求接收   （此行代码放在NO.2处时，就是串行处理http请求，前一次处理过程会阻塞下一次请求处理）

                byte[] recv_buffer = new byte[1024 * 640];
                int real_recv = new_client.Receive(recv_buffer);  //接收浏览器的请求数据
                string recv_request = Encoding.UTF8.GetString(recv_buffer, 0, real_recv);
                int i = recv_request.IndexOf("HTTP");
              
                //Console.WriteLine("*********************");
                Console.WriteLine(recv_request);  //将请求显示到界面
                Console.WriteLine("i="+i);  //将请求显示到界面
                //Console.WriteLine("*********************");
                // Resolve(recv_request, new_client);  //解析、路由、处理

                //NO.2  串行处理http请求
                Console.WriteLine("有人请求了页面...");
                //发送一个文件
                SendFile(new_client);
            }
            catch
            {

            }
        }

        /// <summary>
        /// 按照HTTP协议格式 解析浏览器发送的请求字符串
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        static void Resolve(string request, Socket response)
        {
            //浏览器发送的请求字符串request格式类似这样：
            //GET /index.html HTTP/1.1
            //Host: 127.0.0.1:8081
            //Connection: keep-alive
            //Cache-Control: max-age=0
            //
            //id=123&pass=123       （post方式提交的表单数据，get方式提交数据直接在url中）

            string[] strs = request.Split(new string[] { "\r\n" }, StringSplitOptions.None);  //以“换行”作为切分标志
            if (strs.Length > 0)  //解析出请求路径、post传递的参数(get方式传递参数直接从url中解析)
            {
                string[] items = strs[0].Split(' ');  //items[1]表示请求url中的路径部分（不含主机部分）
                Dictionary<string, string> param = new Dictionary<string, string>();

                if (strs.Contains(""))  //包含空行  说明存在post数据
                {
                    string post_data = strs[strs.Length - 1]; //最后一项
                    if (post_data != "")
                    {
                        string[] post_datas = post_data.Split('&');
                        foreach (string s in post_datas)
                        {
                            param.Add(s.Split('=')[0], s.Split('=')[1]);
                        }
                    }
                }
                //路由处理 返回一个html
                 Route(items[1], param, response);
               
            }
        }
        //application/octet-stream
        static void SendFile(Socket new_client) {
            StringBuilder buf = new StringBuilder();

            Console.WriteLine("开始发送...");
            string statusline = "HTTP/1.1 200 OK\r\n";
           // string statusline = "POST   " + "http://127.0.0.1:8081" + " HTTP/1.1 \r\n";
            // new_client.Send(Encoding.UTF8.GetBytes(statusline));

            //string path = "C:/Users/Administrator/Desktop/kkk.txt";
            // string path = "C:/Users/Administrator/Desktop/socket_webServer.rar";
            string path = "C:/Users/Administrator/Desktop/1111.jpg";
           // byte [] buffer = File.ReadAllBytes(path);
            byte [] buffer = SaveImage(path);

            //测试下载文档Compeleted
            // string header = string.Format("Content-Disposition:attachment;filename=ByteBuffer.cs\r\ncharset=UTF-8\r\nContent-Length:{0}\r\n", buffer.Length);

            //测试下载压缩包
            // string header = string.Format("Content-Disposition:attachment;filename=socket_webServer.rar\r\ncharset=UTF-8\r\nContent-Length:{0}\r\n", buffer.Length);
           
            //测试传输PNG 只能传输base64字符串（下载功能待完善）
            //string header = string.Format("Content-Type:image/png;charset=UTF-8\r\nContent-Length:{0}\r\n", Encoding.UTF8.GetBytes(base64str).Length);


            string base64str = Convert.ToBase64String(buffer);
            #region 将base64字符串转化为图片
            //============================================将base64字符串转化为图片============================================string64
            //byte[] img = Convert.FromBase64String(File.ReadAllText("C:/Users/Administrator/Desktop/string64.txt"));
            //FileStream fs = new FileStream("C:/Users/Administrator/Desktop/wwww.png", FileMode.CreateNew);
            //fs.Write(img, 0, img.Length);
            //fs.Flush();
            //fs.Close();
            //fs.Dispose();
            //FileStream fs2 = new FileStream("C:/Users/Administrator/Desktop/createImg2.txt", FileMode.CreateNew);
            //fs2.Write(Encoding.UTF8.GetBytes(base64str), 0, Encoding.UTF8.GetBytes(base64str).Length);
            //fs2.Flush();
            //fs2.Close();
            //fs2.Dispose();
            //============================================将base64字符串转化为图片============================================
            #endregion




            Console.WriteLine("********************************************************");
            //string json = JsonMapper.ToJson(base64str);
            //Content-Disposition:attachment inline ISO-8859-1
            //  Console.WriteLine(base64str);
            //  new_client.Send(Encoding.UTF8.GetBytes(header));
            //string header = string.Format("Content-Type:image/jpg;charset=UTF-8\r\nContent-Length:{0}\r\n", Encoding.UTF8.GetBytes(base64str).Length);
            string header = string.Format("Content-Type:image/jpeg;filename=my.jpg\r\ncharset=UTF-8\r\nContent-Length:{0}\r\n", Encoding.UTF8.GetBytes(base64str).Length);
            buf.Append(statusline).Append(header).Append("\r\n").Append(base64str);


           // Console.WriteLine(buf.ToString());
            byte[] ms = Encoding.UTF8.GetBytes(buf.ToString());
            new_client.Send(ms);
            new_client.Close();

            //Console.WriteLine("发送成功..."+ buf.ToString());
        }

        public static string ImgToBase64(string path) {
            if (!string.IsNullOrEmpty(path)) {
                MemoryStream ms = new MemoryStream();
                
            }
            return null;
        }

        public static byte[] SaveImage(String path)
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read); //将图片以文件流的形式进行保存
            BinaryReader br = new BinaryReader(fs);
            byte[] imgBytesIn = br.ReadBytes((int)fs.Length); //将流读入到字节数组中
            return imgBytesIn;
        }


        /// <summary>
        /// 按照请求路径（不包括主机部分）  进行路由处理
        /// </summary>
        /// <param name="path"></param>
        /// <param name="param"></param>
        /// <param name="response"></param>
        static void Route(string path, Dictionary<string, string> param, Socket response)
        {
            //Console.WriteLine("************************************************");
            //Console.WriteLine(path);
            //Console.WriteLine("************************************************");
            if (path.EndsWith("index.html") || path.EndsWith("/"))  //请求首页
            {
                Home.HomePage(response);
            }
            else if (path.EndsWith("login.zsp"))  //登录 处理页面
            {
                User.LoginCheck(param["id"], param["pass"], response);
            }
            //...
        }
    }
    

    #region 处理请求 并按照HTTP协议格式发送数据到浏览器
    /// <summary>
    /// 请求首页
    /// </summary>
    class Home
    {
        public static void HomePage(Socket response)
        {
            string statusline = "HTTP/1.1 200 OK\r\n";   //状态行
            byte[] statusline_to_bytes = Encoding.UTF8.GetBytes(statusline);

            string content =
            "<html>" +
                "<head>" +
                    "<title>socket webServer  -- Login</title>" +
                "</head>" +
                "<body>" +
                   "<div style=\"text-align:center\">" +
                       "<form method=\"post\" action=\"/login.zsp\">" +
                           "用户名:&nbsp;<input name=\"id\" /><br />" +
                           "密码:&nbsp;&nbsp;&nbsp;<input name=\"pass\" type=\"password\"/><br />" +
                           "<input  type=\"submit\" value=\"登录\"/>" +
                       "</form>" +
                   "</div>" +
                "</body>" +
            "</html>";  //内容
            byte[] content_to_bytes = Encoding.UTF8.GetBytes(content);

            string header = string.Format("Content-Type:text/html;charset=UTF-8\r\nContent-Length:{0}\r\n", content_to_bytes.Length);
            byte[] header_to_bytes = Encoding.UTF8.GetBytes(header);  //应答头
            response.Send(statusline_to_bytes);  //发送状态行
            response.Send(header_to_bytes);  //发送应答头
            response.Send(new byte[] { (byte)'\r', (byte)'\n' });  //发送空行
            response.Send(content_to_bytes);  //发送正文（html）

            response.Close();
        }
        //...
    }

    /// <summary>
    /// 登录(post方式提交表单)  
    /// </summary>
    class User
    {
        public static void LoginCheck(string name, string pass, Socket response)
        {
            //访问数据库，检查用户名密码是否正确（此处略）...
            //假设用户名密码正确  返回登录成功页面

            //System.Threading.Thread.Sleep(10000);  //模拟耗时处理

            string statusline = "HTTP/1.1 200 OK\r\n";   //状态行
            byte[] statusline_to_bytes = Encoding.UTF8.GetBytes(statusline);

            string content =
            "<html>" +
                "<head>" +
                    "<title>socket webServer  -- Login</title>" +
                "</head>" +
                "<body>" +
                   "<div style=\"text-align:center\">" +
                       "欢迎您！" + name + ",今天是 " + DateTime.Now.ToLongDateString() +
                   "</div>" +
                "</body>" +
            "</html>";  //内容
            byte[] content_to_bytes = Encoding.UTF8.GetBytes(content);

            string header = string.Format("Content-Type:text/html;charset=UTF-8\r\nContent-Length:{0}\r\n", content_to_bytes.Length);
            byte[] header_to_bytes = Encoding.UTF8.GetBytes(header);  //应答头

            response.Send(statusline_to_bytes);  //发送状态行
            response.Send(header_to_bytes);  //发送应答头
            response.Send(new byte[] { (byte)'\r', (byte)'\n' });  //发送空行
            response.Send(content_to_bytes);  //发送正文（html）

            response.Close();
        }
        //...
    }
    #endregion
}
#region 响应报文示例
// GET / HTTP/1.1
//Host: 127.0.0.1:8081
//Connection: keep-alive
//Upgrade-Insecure-Requests: 1
//User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.111 Safari/537.36
//Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9
//Sec-Fetch-Site: none
//Sec-Fetch-Mode: navigate
//Sec-Fetch-User: ?1
//Sec-Fetch-Dest: document
//Accept-Encoding: gzip, deflate, br
//Accept-Language: zh,en-US;q=0.9,en;q=0.8,zh-TW;q=0.7,zh-CN;q=0.6


//GET /favicon.ico HTTP/1.1
//Host: 127.0.0.1:8081
//Connection: keep-alive
//User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.111 Safari/537.36
//Accept: image/avif,image/webp,image/apng,image/*,*/*;q=0.8
//Sec-Fetch-Site: same-origin
//Sec-Fetch-Mode: no-cors
//Sec-Fetch-Dest: image
//Referer: http://127.0.0.1:8081/
//Accept-Encoding: gzip, deflate, br
//Accept-Language: zh,en-US;q=0.9,en;q=0.8,zh-TW;q=0.7,zh-CN;q=0.6

//==============================================================================================================================================

//POST /login.zsp HTTP/1.1
//Host: 127.0.0.1:8081
//Connection: keep-alive
//Content-Length: 11
//Cache-Control: max-age=0
//Upgrade-Insecure-Requests: 1
//Origin: http://127.0.0.1:8081
//Content-Type: application/x-www-form-urlencoded
//User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.111 Safari/537.36
//Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9
//Sec-Fetch-Site: same-origin
//Sec-Fetch-Mode: navigate
//Sec-Fetch-User: ?1
//Sec-Fetch-Dest: document
//Referer: http://127.0.0.1:8081/
//Accept-Encoding: gzip, deflate, br
//Accept-Language: zh,en-US;q=0.9,en;q=0.8,zh-TW;q=0.7,zh-CN;q=0.6

//id=1&pass=1
//GET /favicon.ico HTTP/1.1
//Host: 127.0.0.1:8081
//Connection: keep-alive
//User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.111 Safari/537.36
//Accept: image/avif,image/webp,image/apng,image/*,*/*;q=0.8
//Sec-Fetch-Site: same-origin
//Sec-Fetch-Mode: no-cors
//Sec-Fetch-Dest: image
//Referer: http://127.0.0.1:8081/login.zsp
//Accept-Encoding: gzip, deflate, br
//Accept-Language: zh,en-US;q=0.9,en;q=0.8,zh-TW;q=0.7,zh-CN;q=0.6
#endregion

//===================================================http响应报文==============================================================
//HTTP/1.1 200 OK

//Content-Type:text/html;charset=UTF-8
//Content-Length:310



//<html><head><title>socket webServer  -- Login</title></head><body><div style = "text-align:center" >
//    < form method="post" action="/login.zsp">用户名:&nbsp;<input name = "id" />< br /> 密码:&nbsp;&nbsp;
//    &nbsp;<input name = "pass" type="password"/><br /><input type = "submit" value="登录"/></form>
//    </div></body></html>
//===================================================http响应报文==============================================================


//===================================================正确的http响应报文(发送json)==============================================================
//发送成功...
//HTTP/1.1 200 OK
//Content-Type:application/json;charset=UTF-8
//Content-Length:745
//
//{"dict":{"fire":"F:/Ackerman/FrameworkProj/Assets/StreamingAssets/art","Directional Light":"F:/Ackerman/FrameworkProj/Assets/Strea
//    mingAssets/directional light_prefab","Button":"F:/Ackerman/FrameworkProj/Assets/StreamingAssets/prefabs","Cube":"F:/Ackerman/Fra
//    meworkProj/Assets/StreamingAssets/prefabs","GameObject":"F:/Ackerman/FrameworkProj/Assets/StreamingAssets/prefabs","Sphere":"F:/A
//    ckerman/FrameworkProj/Assets/StreamingAssets/prefabs","Text":"F:/Ackerman/FrameworkProj/Assets/StreamingAssets/prefabs","AudioTrain
//    ingSub1_1":"F:/Ackerman/FrameworkProj/Assets/StreamingAssets/sound","AudioTrainingSub1_2":"F:/Ackerman/FrameworkProj/Assets/Streaming
//    Assets/sound","AudioTrainingSub1_3":"F:/Ackerman/FrameworkProj/Assets/StreamingAssets/sound"}}

//===================================================正确的http响应报文(发送json)==============================================================


#region 浏览器发送请求 url="http://http://127.0.0.1:8081/ackerman/1.jpg"
//GET /ackerman/1.jpg HTTP/1.1
//Host: 127.0.0.1:8081
//Connection: keep-alive
//Upgrade-Insecure-Requests: 1
//User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.111 Safari/537.36
//Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9
//Sec-Fetch-Site: none
//Sec-Fetch-Mode: navigate
//Sec-Fetch-User: ?1
//Sec-Fetch-Dest: document
//Accept-Encoding: gzip, deflate, br
//Accept-Language: zh,en-US;q=0.9,en;q=0.8,zh-TW;q=0.7,zh-CN;q=0.6
#endregion
