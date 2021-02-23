using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using System.Text;

namespace Framework {
    public class PackEditorWindow : EditorWindow
    {
        [MenuItem("Framework/打开窗口")]
        public static void PackWindow() {
            PackEditorWindow editorWindow = EditorWindow.GetWindow(typeof(PackEditorWindow),false,"开发工具") as PackEditorWindow;
            editorWindow.position = new Rect(1785,325,550,550);
            editorWindow.Show();

        }
        /// <summary>
        /// init 
        /// </summary>
        private void OnEnable()
        {
            //Debug.Log("Init...");
            
        }
        private void OnDestroy()
        {
            //Debug.Log("Destroy...");
            list.Clear();
            list =null;
        }
        string luaPath = "F:/Ackerman/FrameworkProj_Copy/Assets/XLua/Resources/xlua";
        string luaName = "model";
        string luaDemoPath = "F:/Ackerman/FrameworkProj_Copy/Assets/XLua/Resources/xlua/model.lua.txt";
        /// <summary>
        /// GUI draw Editor window
        /// </summary>
        private void OnGUI()
        {
            list = new List<string>();
            GUIStyle style = new GUIStyle();
            style.fontStyle = FontStyle.Bold;
            style.fontSize = 15;
            GUILayout.Label("PackKit");
            GUILayout.BeginVertical("BOX");
            GUILayout.Label("AssetBundle管理", style);
            PackSettings.SimulateAssetBundle= GUILayout.Toggle(PackSettings.SimulateAssetBundle, LocaleText.SimulationMode);
            PackSettings.ABPath = EditorGUILayout.TextField("打包路径", PackSettings.ABPath);

            if (GUILayout.Button("打包")) {
                //打AB包
                SignAssets.PackageAbs();
                //生成config文件，AB文件索引
                Asset asset = new Asset();
                asset.dict = new Dictionary<string, string>();
                string[] allABNames = AssetDatabase.GetAllAssetBundleNames();
                foreach (string s in allABNames)
                {
                    string[] allAssetsPath = AssetDatabase.GetAssetPathsFromAssetBundle(s);

                    foreach (string s2 in allAssetsPath)
                    {
                        
                        string str1 = s2.Substring(s2.LastIndexOf("/") + 1, s2.LastIndexOf(".") - 1 - s2.LastIndexOf("/"));
                        string str2 = PackSettings.ABPath + "/" + s;
                        asset.dict.Add(str1, str2);
                    }
                }
                string str = JsonConvert.SerializeObject(asset);
                byte[] buffer = Encoding.UTF8.GetBytes(str);
                FileStream fs = File.Create(Application.streamingAssetsPath + "/Config");
                fs.Write(buffer, 0, buffer.Length);
                fs.Flush();
                fs.Dispose();           
                asset.dict.Clear();
                Debug.Log("Pack Success,Generate Config.");

            }
            if (GUILayout.Button("清除所有")) {
                SignAssets.ClearAbs();
            }
            style.fontSize = 12;
            GUILayout.Label("已标记",style);
            LoadABList();

            GUILayout.EndVertical();
            GUILayout.Label("LuaKit");
            GUILayout.BeginVertical("BOX");
            luaPath = EditorGUILayout.TextField("Lua生成路径",luaPath);
            luaName = EditorGUILayout.TextField("Lua名字", luaName);
            if (GUILayout.Button("生成") ){
                if (File.Exists(luaPath + "/" + luaName + ".lua.txt"))
                {
                    Debug.Log("Lua文件" + luaPath + "/" + luaName + ".lua.txt" + "已存在");

                }
                else {
                    string luaText = File.ReadAllText(luaDemoPath);
                    string newText = luaText.Replace("model", luaName);
                    FileStream fs = File.Create(luaPath + "/" + luaName + ".lua.txt");
                    byte[] buffer = Encoding.UTF8.GetBytes(newText);
                    fs.Write(buffer, 0, buffer.Length);
                    fs.Close();
                    fs.Dispose();
                    Debug.Log("生成Lua代码成功,路径=" + luaPath + "/" + luaName + ".lua.txt");
                }
            }
            GUILayout.EndVertical();
            list.Clear();
        }
        /// <summary>
        /// file is mark?
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private bool IsMark(string path) {
            var re = AssetImporter.GetAtPath(path);
            DirectoryInfo info = new DirectoryInfo(path);
            if (list.Contains(path)) {
                return false;
            }
            list.Add(path);
            return string.Equals(re.assetBundleName, info.Name.Replace(".", "_").ToLower());
        }
        List<string> list;
        /// <summary>
        /// get parent folder
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private string GetParentFolder(string path) {
            if (path.Equals(string.Empty)) {
                return string.Empty;
            }

            return Path.GetDirectoryName(path);
        }
        /// <summary>
        /// Load AssetBundle List
        /// </summary>
        private void LoadABList() {
            var abs = AssetDatabase.GetAllAssetBundleNames();
            foreach (string ab in abs) {
                var result = AssetDatabase.GetAssetPathsFromAssetBundle(ab);
           
                foreach (string r in result) {
                   
                    if (IsMark(r)) {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(r);
                        if (GUILayout.Button("选择", GUILayout.Width(80), GUILayout.Height(20))) {
                            Selection.objects = new[]
                                {
                                    AssetDatabase.LoadAssetAtPath<Object>(r)
                                };
                        }
                        if (GUILayout.Button("取消标记", GUILayout.Width(80), GUILayout.Height(20)))
                        {
                            SignAssets.MarkAB(r);
                        }
                        
                        GUILayout.EndHorizontal();
                    }
                    if (IsMark(GetParentFolder(r))) {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(GetParentFolder(r));
                        if (GUILayout.Button("选择", GUILayout.Width(80), GUILayout.Height(20)))
                        {
                            Selection.objects = new[]
                                {
                                    AssetDatabase.LoadAssetAtPath<Object>(r)
                                };
                        }
                        if (GUILayout.Button("取消标记", GUILayout.Width(80), GUILayout.Height(20)))
                        {
                            SignAssets.MarkAB(r,true);
                        }
                    
                      
                        GUILayout.EndHorizontal();
                    }
                }
         
            }
        }
        private void GetGUIStyle() {

            GUIStyle style = new GUIStyle();
            
        }
    }
    /// <summary>
    /// const text
    /// </summary>
    public class LocaleText
    {

        
        public static string SimulationMode
        {
            get
            {
                return "Simulation Mode";
            }
        }
    }
}