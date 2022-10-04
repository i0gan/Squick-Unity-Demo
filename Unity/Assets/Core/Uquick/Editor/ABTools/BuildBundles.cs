using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using BM;
using Uquick.Core;
using UnityEditor;
using UnityEngine;

namespace Uquick.Editor
{
    public class BuildBundles
    {
        [MenuItem("Tools/BuildAsset/Build Asset Bundle %#&B")]
        private static void BuildAssetBundles()
        {
            DLLMgr.Delete("Assets/HotUpdate/Dll/HotScriptss.bytes");
            Action<string> buildAct = async s =>
            {
                var watch = new Stopwatch();
                watch.Start();
                var bytes = DLLMgr.FileToByte(DLLMgr.DllPath);
                var result = DLLMgr.ByteToFile(CryptoHelper.AesEncrypt(bytes,s), "Assets/HotUpdate/Dll/HotScriptss.bytes");
                watch.Stop();
                Log.Print("Convert Dlls in: " + watch.ElapsedMilliseconds + " ms.");
                if (!result)
                {
                    Log.PrintError("DLL to Byte[] Error!");
                    return;
                }
            
                Setting.EncryptPassword = s;

                await Task.Delay(3);
                AssetDatabase.Refresh();

                watch = new Stopwatch();
                watch.Start();
                BuildAssets.BuildAllBundle();
                watch.Stop();
                Log.Print("Build AssetBundles in: " + watch.ElapsedMilliseconds + " ms."); 
            };
            
            if (string.IsNullOrEmpty(Setting.EncryptPassword))
            {
                CryptoWindow.ShowWindow();
                CryptoWindow.Build = buildAct;
            }
            else
            {
                buildAct.Invoke(Setting.EncryptPassword);
            }
        }
        
        private const string KViewCachePath = "Tools/BuildAsset/View/Caches";
        private const string KViewDataPath = "Tools/BuildAsset/View/Build";
        
        [MenuItem(KViewDataPath)]
        private static void ViewDataPath()
        {
            if(Directory.Exists(Directory.GetParent(Application.dataPath).FullName + "/Build/"))
            {
                EditorUtility.OpenWithDefaultApp(Directory.GetParent(Application.dataPath).FullName + "/Build/");
            }
            else 
            {
                Log.PrintError("Unable to View Bundles: Please Build Bundles First");
            }
        }
        
        [MenuItem(KViewCachePath)]
        private static void ViewCachePath()
        {
            EditorUtility.OpenWithDefaultApp(Application.persistentDataPath);
        }
        
    }
}