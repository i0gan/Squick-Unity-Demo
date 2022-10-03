using System;
using Game;
using UnityEngine;
using Uquick.Core;
namespace HotUpdateScripts.Game;

public class AppStart : MonoBehaviour
{
    private void Awake()
    {
       Debug.Log("Hello World! 这里执行的是热更代码");
    }

    private void Start()
    {
        Invoke("NextScene", 3.0f);
        
    }

    void NextScene()
    {
        AssetMgr.LoadSceneAsync(BM.BPath.Assets_HotUpdateResources_Scene_Home__unity);
    }
    
    private void Update()
    {
        
    }
}