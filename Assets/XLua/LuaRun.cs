using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XLua;

[LuaCallCSharp]
public class LuaRun : MonoBehaviour
{
    [Header("测试")]
    public bool isTest = false;//是否测试，测试读取本地文件，若不是测试读取ab包中的lua文件
    string luaResPath = "lua/";
    public TextAsset textLua;//Lua txt文件
    public string luaFileName;//Lua文件名
    private string luaName;//Lua名字
    private string luaString;//lua内容
    private LuaEnv luaenv = new LuaEnv();
    private LuaTable table;
    public MonoBehaviour Mono;
    private Action luaStart;
    private Action luaUpdate;
    private Action luaDestroy;

    private void Awake()
    {
       
        Mono = this;
        table = luaenv.NewTable();
        //init lua
        if (textLua == null|| !isTest)
        {
         
            if (luaFileName == null)
            {
                Debug.LogError("lua not exist...");
                return;
            }
            else
            {
                //如果texlua等于空，直接从ab中根据表名加载lua
                AssetBundle ab = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/lua");
                luaString = ab.LoadAsset<TextAsset>(luaFileName.Substring(luaFileName.IndexOf('/')+1)).text;
                ab.Unload(false);
                ab = null;
                Debug.Log("从ab中加载lua字符串成功..." + luaString);
                luaName = textLua.name.Substring(0, textLua.name.IndexOf('.'));
            }
        }
        else {
            luaString = textLua.text;
            luaName = textLua.name.Substring(0, textLua.name.IndexOf('.'));
        }
        luaenv.DoString(luaString);
        luaenv.Global.Get(luaName,out table);
        //设置一些参数
        table.Set("self",this);         
        table.Set("transform",transform);
        //获取一些通用的方法
        Action awake = table.Get<Action>("Awake");
        table.Get("Start", out luaStart);
        table.Get("Update",out luaUpdate);
        table.Get("Destroy",out luaDestroy);      
        if (awake != null) {
            awake(); 
        }
        
        #region test code
    //LuaTable meta = luaenv.NewTable();
    //meta.Set("__index", luaenv.Global);
    //table.SetMetaTable(meta);
    //meta.Dispose();
    //table.Set("self",this);
    //luaenv.DoString(textLua.text, "TestLua", table);

    //luaAwake = table.Get<Action>("Awake");
    #endregion
    }
    // Start is called before the first frame update
    void Start()
    {
        if (luaStart != null)
        {
            luaStart();
        }
    }
    /// <summary>
    /// C#调用lua方法
    /// </summary>
    /// <param name="name"></param>
    /// <param name="objs"></param>
    public void CallLuaFunc(string name , params object [] objs) {
        LuaFunction functions = table.Get<LuaFunction>(name);
        functions.Call(objs);
    }
    // Update is called once per frame
    void Update()
    {
        if (luaUpdate != null)
        {
            luaUpdate();
        }
    }
    private void OnDestroy()
    {
        if (luaDestroy != null)
        {
            luaDestroy();
        }
  
        luaStart = null;
        luaUpdate = null;
        luaDestroy = null;
        table.Dispose();
    }
    [Obsolete]
    private void LuaVoidTest() {
        Button button = this.GetComponent<Button>();
        button.onClick.AddListener(()=> {
            GameObject o;
            Transform t;
        });
    }
}

/*
 各个类间的调用:
 为了能互相调用，lua文件最好都放在Resources文件夹下
 exp: 要调用Test.lua,把该文件放在Resources/lua下，使用local test = require("lua/Test")即可获取Test
 协程
 list dictionary
 数组
 代码整体向左移动 shift+tab
     
     */