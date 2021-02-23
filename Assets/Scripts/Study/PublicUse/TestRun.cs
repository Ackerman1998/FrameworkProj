using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.IO;
using System;

using Newtonsoft.Json;
using System.Text;
using Object = UnityEngine.Object;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using UnityEngine.Networking;

public class TestRun : MonoBehaviour, IDisposable
{
    public AudioSource audioSource;
    string[] allAssetBundleNames;
    public Sprite sprite;
    public Image img;
    private void OnDestroy()
    {
        ///PackKit.Instance.Destroy();
       //print("Destroy");
        if (socClient != null) {
            socClient.Dispose();
            thread.Abort();
            thread = null;
        }
        //if (thread != null) {
        //    thread.Abort();
        //    thread = null;
        //}
        if (t!=null) {
            t.Abort();
            thread = null;
        }
    }

    static Socket _socket = null;
    Thread thread=null ;
    Thread t =null ;

    SocClient socClient;
    byte[] bf = new byte[1024 * 1024];
    private void Run() {

        thread = new Thread(() =>
        {
            socClient = new SocClient("127.0.0.1", 8081);
        });
        thread.Start();
        //thread =  new System.Threading.Thread(TestThread);
        //  thread.Start();
    }
    private void TestThread()
    {
        //int i = 0;
        //while (i < 100)
        //{
        //    Debug.Log("TestThread running : " + i);
        //    System.Threading.Thread.Sleep(1000);
        //    i++;
        //}
    }
    
    // Start is called before the first frame update
    void Start()
    {
        Run();
        //_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //_socket.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8081));

        //new Thread(() =>
        //{
        //    HttpWebRequest httpWeb = HttpWebRequest.Create("http://127.0.0.1:8081") as HttpWebRequest;
        //    httpWeb.Method = "POST";
        //    print("end");
        //    HttpWebResponse wr = httpWeb.GetResponse() as HttpWebResponse;

        //    StreamReader streamReader = new StreamReader(wr.GetResponseStream(), false);
        //    string base64str= streamReader.ReadToEnd();

        //    FileStream fs = new FileStream("C:/Users/Administrator/Desktop/JDKSJADKLA.png", FileMode.CreateNew);
        //    fs.Write(Convert.FromBase64String(base64str), 0, Convert.FromBase64String(base64str).Length);
        //    fs.Flush();
        //    fs.Close();
        //    fs.Dispose();
        //}).Start();

        //byte[] buffer = Encoding.UTF8.GetBytes("djaskjdklasjdlkjalsjdkalsjdkals");
        //using (FileStream fs = File.Create(Application.persistentDataPath + "/Config")) {

        //    fs.Write(buffer, 0, buffer.Length);
        //}
        //ProductFactory.Create("mouse").Sell();
        //ProductFactory.Create("cup").Get();
        //Observer observer = new Observer();
        //Enemy enemy=  new Enemy(observer);
        //Hero hero =  new Hero(observer);
        //observer.NoticeAll();
        //TestPackKit();
        // TestLoadAB();
        //TestLoadAB();
        //TestJson();
        //StartCoroutine("Get");
    }
    //  [ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
    //[SecuritySafeCritical]
    //public static void Copy(Array sourceArray, int sourceIndex, Array destinationArray, int destinationIndex, int length);
    //[ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
    //public static void Copy(Array sourceArray, long sourceIndex, Array destinationArray, long destinationIndex, long length);
    /// <summary>
    /// 发送消息实例
    /// </summary>
    public void SendMessage() {
         t = new Thread(()=> {
            int i = 0;
            //字节长度+字节数组
            while (i <= 100)
            {
                Bufferbyte bufferbyte = new Bufferbyte();
                bufferbyte.WriteString("Login");
                bufferbyte.WriteInt(123456);
                bufferbyte.WriteString("zrm123");

                socClient.SendMessage(bufferbyte);
         //    Debug.Log(bufferbyte.ReadString());
                //  string str = "Hello" + i;
                //string str = "Login";
                //    Bufferbyte bufferbyte = new Bufferbyte();
                //    bufferbyte.WriteString(str);
                //    bufferbyte.WriteInt(123456);
                //    bufferbyte.WriteString("zrm123");
                //    bufferbyte.SendMessage(_socket);
                // byte [] bufferLen = Encoding.UTF8.GetBytes((Encoding.UTF8.GetBytes(str).Length).ToString());
                // print(Encoding.UTF8.GetBytes(str).Length);
                //==================普通发送方式
                //byte [] bufferLen = BitConverter.GetBytes(Encoding.UTF8.GetBytes(str).Length);

                //byte[] buffer = Encoding.UTF8.GetBytes(str);
                //byte[] newBuff = new byte[buffer.Length+bufferLen.Length];
                //Array.Copy(bufferLen,0,newBuff,0,bufferLen.Length);
                //Array.Copy(buffer,0, newBuff, bufferLen.Length, buffer.Length);
                //    print("发送消息" + str + "成功");
                //    _socket.Send(newBuff);
                //=================
                i++;
            }
            // Array arr = new Array();
            //Array.Copy(bufferLen,newBuff,0, bufferLen.Length);

        });
        t.Start();
    }
    private void callback(IAsyncResult ar)
    {
      
        //_socket = ar.AsyncState as Socket;
   
        int cc = _socket.EndReceive(ar);
        if (cc == 0)
        {
            print("server is diconnect...");
            _socket.Close();
        }
        else {
           
            _socket.Send(Encoding.UTF8.GetBytes("connect ..."));
            print(Encoding.UTF8.GetString(bf,0,cc));
        
            _socket.BeginReceive(bf, 0, bf.Length, SocketFlags.None, callback, _socket);
            _socket.Close();
 
        }
     
    }

    IEnumerator Get() {
        UnityWebRequest webRequest =UnityWebRequest.Get("http://127.0.0.1:8081");
        yield return webRequest.downloadHandler;
        if (webRequest.error != null)
        {

        }
        else {
            byte[] buffer = webRequest.downloadHandler.data;
            Debug.Log(Encoding.UTF8.GetString(buffer));
        }
    }
    void TestPackKit()
    {
        //PackKit.Instance.Init(this);
        //audioSource.clip = PackKit.Instance.LoadAssetSync<AudioClip>("AudioTrainingSub1_1");
        //audioSource.Play();
        //img.sprite = PackKit.Instance.LoadAssetSync<Sprite>("fire");
        //PackKit.Instance.LoadPrefabSync("Cube").Create();
    }
    void TestLoadAB() {
        //string abPaths = Application.streamingAssetsPath;
        //string[] fileNames = Directory.GetFiles(abPaths);
        //foreach (string s in fileNames)
        //{
        //    print(s);
        //}
        //AssetBundle asset = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/sound");
        //foreach (Object obj in asset.LoadAllAssets())
        //{
        //    print(obj.name);
        //}
    }
    /// <summary>
    /// 将ab中的资源文件的名字作为值，路径作为键。打包时写入到配置表中json格式。
    /// </summary>
    void TestJson() {
        //PackKit.Instance.AddPrefabLoader(new List<string>() { "fire","Cube"});
        //初始化
        PackKit.Instance.Init(this);
        //测试预加载资源功能
        PackKit.Instance.AddPrefabLoader(new List<string>() { "Sphere", "Cube" });
        //加载并实例化预制体
        PackKit.Instance.LoadAssetSync<Object>("Cube").Create();
        //加载图片资源
        img.sprite= PackKit.Instance.LoadAssetSync<Sprite>("fire");
    }
    void TestAssets() {
//#if UNITY_EDITOR
//        //string []str= AssetDatabase.GetAssetPathsFromAssetBundle("cube");
//        //string []str= AssetDatabase.GetAllAssetBundleNames();获取所有的ab包的名字
//        //string []str= AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName("cube_prefab", "cube");
//        //foreach (string s in str) {
//        //    print(s);
//        //    AssetDatabase.LoadAssetAtPath<Object>(s).Create();
//        //}
//        print("解包");
//        allAssetBundleNames = AssetDatabase.GetAllAssetBundleNames();//获取所有的ab包的名字 ui cube sphere


//        audioSource.clip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Prefabs/Sound/AudioTrainingSub1_1.mp3");
//        audioSource.Play();
//        if (!string.IsNullOrEmpty(FindAB("cube"))) {
//            AssetDatabase.LoadAssetAtPath<Object>(FindAB("cube")).Create();
//        }
       
//#endif
    }
    /// <summary>
    ///通过遍历ab包，给定资源名来获取资源路径
    /// </summary>
    /// <param name="assetName"></param>
    string FindAB(string assetName) {
        //if (allAssetBundleNames.Length != 0)
        //{
        //    foreach (string s in allAssetBundleNames)
        //    {
        //        string[] s1 = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(s, assetName);
        //        if (s1.Length != 0)
        //        {
        //            return s1[0];
        //        }
        //        else
        //        {
        //            return string.Empty;
        //        }
        //    }
        //}
        //else {
        //    return string.Empty;
        //}
        return string.Empty;
    }
    void TestFF() {
        //string str = "hjellldjsakldjklsajdlkajsdkjaskldjklasjdkljaskljdl1";
        //FFactory.WriteStrToFile(str,Application.dataPath+"/Data/","zrm",()=> {
        //    Debug.Log("Write Success!");
        //});
        //FFactory.ReadStrToFile(Application.dataPath + "/Data/","zrm").Print();
        //FFactory.ReadStrToFile(Application.dataPath + "/Data/zrm").Print();
        //FFactory.WriteByteToFile(FFactory.ReadImgToFile(Application.dataPath + "/colorformat.png"), Application.dataPath + "/Data/", "zrm.png",()=> {
        //    Debug.Log("Write Imgs Success!");
        //});
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void Dispose()
    {
      
        print("dispose ...");
    }
}
/*
 AssetDatabase类：对资源进行读取的类
 不能被实例化，可用来读取asset，只能在editor下使用
     方法：
     GetAllAssetBundleNames() 获取所有的ab包的名字
     GetAssetPathsFromAssetBundle(string abName) 获取ab包中的所有资源路径
     GetAssetPathsFromAssetBundleAndAssetName(string abName,string assetName)给定ab包名，资源名字，获取到资源的路径
     LoadAssetAtPath<T>(string assetPath)指定要加载的资源类型T,给定资源的路径，加载资源
     GetAssetBundleDependencies()获取资源引用（暂无使用）
     */
