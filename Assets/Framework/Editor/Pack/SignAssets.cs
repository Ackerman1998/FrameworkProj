using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Framework;
namespace Framework { 
public class SignAssets 
{
    //标记ab包
    [MenuItem("Assets/快速标记")]
    public static void Abs()
    {
        MarkAB(GetSelectedPath());
    }
    public static string GetSelectedPath()
    {
        var path = string.Empty;
        foreach (var obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
        {
            path = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                return path;
            }
        }
        return path;
    }
    public static void MarkAB(string path,bool isFolder=false)
    {
        //Debug.Log("MarkABPath="+path);
        if (!string.IsNullOrEmpty(path))
        {
            if (isFolder)
            {
                path = Path.GetDirectoryName(path);
            }
            else {
               
            }
            var ai = AssetImporter.GetAtPath(path);
            var dir = new DirectoryInfo(path);
            if (ai.assetBundleName == "" && ai.assetBundleVariant == "")
            {
                ai.assetBundleName = dir.Name.Replace(".", "_");
                ai.assetBundleVariant = "";
                Debug.Log("标记" + ai.assetBundleName + "成功");
            }
            else
            {
                Debug.Log("取消标记" + ai.assetBundleName);
                ai.assetBundleVariant = "";
                ai.assetBundleName = "";
            }
            AssetDatabase.RemoveUnusedAssetBundleNames();
        }
    }
   // [MenuItem("Ackerman/Tools/打包")]
    public static void PackageAbs()
    {
        string directory = PackSettings.ABPath;
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        AssetBundleManifest assetBundleManifest = BuildPipeline.BuildAssetBundles(directory, BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
        if (assetBundleManifest != null)
        {
            Debug.Log("打包成功,打包路径：" + directory);
        }
        // DirectoryInfo代表文件夹的一个类 可实例化 ,Directory 静态类 不可实例化
        DirectoryInfo directoryInfo = new DirectoryInfo(directory);
        FileInfo[] files = directoryInfo.GetFiles();
        foreach (FileInfo file in files)
        {
            //清除manifest文件，打包目录
            if (file.Name.EndsWith("manifest")||directory.Contains(file.Name) )
            {
              File.Delete(file.FullName);
            }
        }
    }
   // [MenuItem("Ackerman/Tools/清除AB包")]
    public static void ClearAbs()
    {
        string directory =PackSettings.ABPath;
        DirectoryInfo directoryInfo = new DirectoryInfo(directory);
        FileInfo[] files = directoryInfo.GetFiles();
        for (int i = 0; i < files.Length; i++)
        {
            File.Delete(files[i].FullName);
        }
        Debug.Log("清除成功!");
    }
}
}
/*
 BuildAssetBundleOptions(构建AssetBundle的方式)
 None:默认方式,使用LZMA压缩，压缩包小，加载时间长
 UncompressedAssetBundle:不压缩数据，包大，加载快
 ChunkBaseCompression:LZ4压缩，压缩包比Node稍大，但加载时间快
     
     */
