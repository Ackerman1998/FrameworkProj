using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
struct Version
{
    public double versionCode;
    public double downloadCode;
}
public class LoginLoad : MonoBehaviour
{

    [Header("测试")]
    public bool isTest = false;
    public Image slider;
    public Text tips;
    public Button restartScene;//当网络发生错误时，点击按钮重载场景
    double currentVersionCode;
    double serverVersionCode;
    public GameObject checkVersionUI;
    DownLoad downLoad = null;
    private Dictionary<string, string> dictAssets = new Dictionary<string, string>();
    public bool over = false;

    public double ServerVersionCode { get => serverVersionCode; set => serverVersionCode = value; }

    public void HideLoadUI()
    {
        over = true;
    }
    public void ChangeUIText(string str)
    {
        tips.text = str;
    }
    private void Awake()
    {
        dictAssets.Add("http://www.joelee.top:1998/ackerman/ui", Application.streamingAssetsPath + "/AssetBundles/Windows/ui");
        dictAssets.Add("http://www.joelee.top:1998/ackerman/lua", Application.streamingAssetsPath + "/AssetBundles/Windows/lua");

        restartScene.gameObject.SetActive(false);
        GetCurrentVersion();
        StartCoroutine(UpdateUI());

        //Version version = new Version();
        //version.versionCode = 1.0f;
        //version.downloadCode = 1.0f;
        //print(JsonMapper.ToJson(version));

    }
    IEnumerator UpdateUI()
    {
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(0.2f);
            slider.fillAmount += 0.1f;
        }
        Init();
    }
    //version:1.1
    private void GetCurrentVersion()
    {
        string path = Application.streamingAssetsPath + "/version.txt";
        string version = File.ReadAllText(path);
        Version ver = JsonMapper.ToObject<Version>(version);
        currentVersionCode = ver.versionCode;

        Debug.Log("CurrentCode=" + currentVersionCode);
    }
    private void Init()
    {

        string url = "http://www.joelee.top:1998/ackerman/version.txt";
        string urlAB = "http://www.joelee.top:1998/ackerman/lua";
        StartCoroutine(NetGet(url, () =>
        {
            //请求错误回调
            restartScene.gameObject.SetActive(true);
            tips.text = "NetError,Please Check Net or Refresh...";
        }, (text) =>
        {
            //请求成功回调

            serverVersionCode = double.Parse(text.Substring(text.IndexOf(':') + 1));
            tips.text = "Last Version = " + serverVersionCode;
            if (serverVersionCode > currentVersionCode)
            {
                //version need update 下载目标文件
                downLoad = new DownLoad(this);
                downLoad.VerificatAndDownloadFile(dictAssets, () => {

                    HideLoadUI();
                });

            }
            else
            {
                //version is last
                ChangeUIText("当前版本为最新");
                HideLoadUI();
            }
        }));
    }

    public void RestartScene()
    {
        SceneManager.LoadScene("Login");
    }
    /// <summary>
    /// Get请求
    /// </summary>
    /// <param name="url"></param>
    /// <param name="errorAction"></param>
    /// <param name="callBack"></param>
    /// <returns></returns>
    IEnumerator NetGet(string url, Action errorAction, Action<string> callBack)
    {

        UnityWebRequest webRequest = UnityWebRequest.Get(url);
        yield return webRequest.SendWebRequest();
        if (webRequest.error != null)
        {
            Debug.Log("Net is error ,please check net" + webRequest.error);
            errorAction();
        }
        else
        {
            try
            {
                callBack(webRequest.downloadHandler.text);
            }
            catch (Exception e)
            {
                Debug.Log("Version is error,Please check net or refresh" + e);
                errorAction();
            }
        }
    }
    /// <summary>
    /// 加载ui
    /// </summary>
    private void Load()
    {
#if UNITY_EDITOR
        //测试用,直接从resource加载
        if (isTest)
        {
            Instantiate(Resources.Load<GameObject>("ui/LoginUI"));
        }
        else
        {
            //直接从AB中加载
            AssetBundle ab = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/AssetBundles/Windows/ui");
            Instantiate(ab.LoadAsset<GameObject>("LoginUI"));
            ab.Unload(false);
            ab = null;
        }
#else
        //发布后加载
         AssetBundle ab = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/AssetBundles/Windows/ui");
         Instantiate(ab.LoadAsset<GameObject>("LoginUI"));
         ab.Unload(false);
         ab = null;
#endif
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //if (downLoad != null && downLoad.unityWebRequest != null)
        //{
        //    if (downLoad.unityWebRequest.downloadProgress >= 1)
        //    {
        //        ChangeUIText("下载进度:" + 100 + "%");
        //    }
        //    else
        //    {
        //        print("下载进度:" + downLoad.unityWebRequest.downloadProgress);
        //        string str = (downLoad.unityWebRequest.downloadProgress * 100).ToString("0.00");
        //        ChangeUIText("下载进度:" + str + "%");
        //    }

        //}
        if (downLoad != null && downLoad.thread != null)
        {
            // Debug.Log("进度："+downLoad.progress);
            slider.fillAmount = downLoad.progress;
            ChangeUIText("下载进度:" + (downLoad.progress * 100).ToString("0.00") + "%");
        }
        if (over)
        {
            over = false;
            checkVersionUI.SetActive(false);
            Load();
            this.gameObject.SetActive(false);
        }


    }
    private void OnDestroy()
    {
        if (downLoad != null)
        {
            downLoad.Destroy();
        }
    }
}
interface IDownLoad
{
    void VerificatAndDownloadFile(Dictionary<string, string> dic, Action okAction = null);

}
abstract class DownLoadBase
{
    public Thread thread = null;
    public UnityWebRequest unityWebRequest = null;
    public float progress;


}
/// <summary>
///下载文件
/// </summary>
class DownLoad : DownLoadBase, IDownLoad
{
    HttpWebRequest webRequest = null;
    private LoginLoad loginLoad = null;
    private Dictionary<string, string> downLoadDict = new Dictionary<string, string>();//需要下载的任务
    private int downloadCount = 0;//已经下载好的任务数

    private bool isStop = false;
    public Thread Thread { get => thread; set => thread = value; }
    public DownLoad(LoginLoad login)
    {
        this.loginLoad = login;

    }
    /// <summary>
    /// 释放资源
    /// </summary>
    public void Destroy()
    {
        isStop = true;//Unity 客户端关闭
        webRequest = null;
        if (thread != null)
        {
            thread.Abort();
            thread = null;
        }
        loginLoad = null;
    }

    /// <summary>
    /// 校验并下载文件
    /// </summary>
    /// <param name="url"></param>
    /// <param name="filePath"></param>
    /// <param name="callback"></param>
    public void VerificatAndDownloadFile(Dictionary<string, string> dic, Action okAction = null)
    {//key=url,value=filepath
        loginLoad.ChangeUIText("正在校验文件...");
        foreach (var temp in dic)
        {
            FileStream fs = new FileStream(temp.Value, FileMode.OpenOrCreate, FileAccess.Write);
            long fileLength = fs.Length;//本地文件大小
            //Debug.Log(temp.Value + "长度=" + fileLength);
            //Debug.Log("从服务器上获取文件大小=" + GetFileLenForServ(temp.Key));
            //进行文件大小比对
            if (fileLength == GetFileLenForServ(temp.Key))
            {
                //文件大小相同,不下载
                fs.Close();
                fs.Dispose();
            }
            else
            {
                //文件大小不相同,添加到下载队列中
                downLoadDict.Add(temp.Key, temp.Value);
                fs.Close();
                fs.Dispose();
            }
        }
        #region 覆盖下载
        /*
         覆盖下载
        if (downLoadDict.Count != 0)
        {
            loginLoad.ChangeUIText("正在下载文件...");
            foreach (var temp in downLoadDict)
            {
                loginLoad.StartCoroutine(DownLoadFile(temp.Key, temp.Value));
            }
            //loginLoad.ChangeUIText("下载成功...");
            loginLoad.ChangeUIText("下载更新包完成");
        }
        else {
           loginLoad.ChangeUIText("校验完成,暂无下载");
        }
        */
        #endregion
        Debug.Log("新的资源包个数:" + downLoadDict.Count);
        //非覆盖下载
        if (downLoadDict.Count != 0)
        {

            foreach (var temp in downLoadDict)
            {
                loginLoad.ChangeUIText("正在下载文件...");
                AddDownLoadFile(temp.Key, temp.Value);
            }

            loginLoad.ChangeUIText("下载更新包完成...");
            //if (okAction!=null) {
            //    okAction();
            //}
        }
        else
        {
            loginLoad.ChangeUIText("校验完成,暂无下载");
            if (okAction != null) { okAction(); }

        }
    }
    private long fileLength = 0;
    private long totalLength = 0;
    /// <summary>
    /// 下载文件(非覆盖下载)
    /// </summary>
    private void AddDownLoadFile(string url, string path)
    {
        thread = new Thread(() => {
            //Debug.Log("线程内存地址:"+thread.GetHashCode());
            //Debug.Log("正在下载文件" + url);
            if (GetDownloadCode() == 1.0)
            {
                //下载文件，给定一个标记表示在下载，下载完成时，再将标记还原
                File.Delete(path);
                SetVerDownloadCode(1.1);
            }
            FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
            long fileLen = fs.Length;
            long totalLen = GetFileLenForServ(url);
            Debug.Log("totalLenddd=" + totalLen);
            totalLength += totalLen;
            //Debug.Log("totalLen=" + totalLength);
            if (fileLen < totalLen)
            {
                fs.Seek(fileLen, SeekOrigin.Begin);//设置本地文件写入起点
                HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;
                request.AddRange((int)fileLen);//设置请求文件读取起点
                Stream stream = request.GetResponse().GetResponseStream();
                byte[] buffer = new byte[1024];
                int length = stream.Read(buffer, 0, buffer.Length);
                fileLength += fileLen;
                while (length > 0)
                {
                    if (isStop) return;
                    fs.Write(buffer, 0, length);
                    fileLen += length;
                    fileLength += length;
                    //Debug.Log("totalLength=" + totalLength);
                    //Debug.Log("fileLength=" + fileLength);
                    progress = (float)fileLength / (float)totalLength;
                    length = stream.Read(buffer, 0, buffer.Length);
                }
                stream.Close();
                stream.Dispose();
            }
            else
            {
                //下载完成
            }
            Debug.Log("下载文件成功" + url);
            downloadCount++;
            fs.Close();
            fs.Dispose();
            if (downloadCount >= downLoadDict.Count)
            {
                Debug.Log(" SetVerDownloadCode(1.0);");
                SetVerDownloadCode(1.0);
                loginLoad.HideLoadUI();
                SetVersionCode();
            }

        });
        thread.Start();
    }

    /// <summary>
    /// 下载文件(覆盖下载)
    /// </summary>
    IEnumerator DownLoadFile(string url, string path)
    {
        unityWebRequest = UnityWebRequest.Get(url);

        yield return unityWebRequest.SendWebRequest();
        //Debug.Log("下载进度:" + unityWebRequest.downloadProgress);
        if (unityWebRequest.error != null)
        {
            Debug.Log("下载更新包时出现错误:" + unityWebRequest.error);
            loginLoad.ChangeUIText("下载失败,请刷新...");
            loginLoad.restartScene.gameObject.SetActive(true);
        }
        else
        {
            byte[] buffer = unityWebRequest.downloadHandler.data;
            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
            fs.Write(buffer, 0, buffer.Length);
            fs.Close();
            fs.Dispose();
            unityWebRequest = null;
            Debug.Log("下载更新包" + path + "成功...");

        }
    }
    /// <summary>
    /// 是否开始下载，开始下载1.0,没下载1.1
    /// </summary>
    private double GetDownloadCode()
    {
        string path = Application.streamingAssetsPath + "/version.txt";
        string version = File.ReadAllText(path);
        Version ver = JsonMapper.ToObject<Version>(version);
        return ver.downloadCode;
    }
    /// <summary>
    /// 设置下载code
    /// </summary>
    /// <param name="versioncode"></param>
    /// <param name="downloadcode"></param>
    private void SetVerDownloadCode(double downloadcode)
    {
        lock (this)
        {
            Debug.Log("设置下载号" + downloadcode);
            string path = Application.streamingAssetsPath + "/version.txt";
            string version = File.ReadAllText(path);
            Version ver = JsonMapper.ToObject<Version>(version);
            Version v = new Version();
            v.versionCode = ver.versionCode;
            v.downloadCode = downloadcode;
            File.WriteAllText(path, JsonMapper.ToJson(v));
            Debug.Log("设置下载号...");
        }

    }

    private void SetVersionCode()
    {

        Debug.Log("设置版本号" + loginLoad.ServerVersionCode);
        string path = Application.streamingAssetsPath + "/version.txt";
        string version = File.ReadAllText(path);
        Version ver = JsonMapper.ToObject<Version>(version);
        Version v = new Version();
        v.versionCode = loginLoad.ServerVersionCode;
        v.downloadCode = ver.downloadCode;
        File.WriteAllText(path, JsonMapper.ToJson(v));
        Debug.Log("设置版本号...");


    }
    /// <summary>
    /// 从服务器获取文件大小
    /// </summary>
    /// <param name="url"></param>
    public long GetFileLenForServ(string url)
    {
        HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;
        request.Method = "HEAD";
        HttpWebResponse response = request.GetResponse() as HttpWebResponse;
        return response.ContentLength;
    }
}
/*
 客户端下载ab包进行更新：
 --功能
    多线程下载文件，断点续传下载文件,下载进度条UI
    下载文件时断网,或其他情况     
     
     
     */
