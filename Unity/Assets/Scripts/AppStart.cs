using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
public class AppStart : MonoBehaviour
{
    // Start is called before the first frame update
    //public VideoPlayer videoPlayer;
    void Start()
    {
        //Debug.Log("Video Length");
        Invoke("LoadScene", 2.0f);
        if (UnityEngine.RuntimePlatform.Android == Application.platform)
        {
            KeepScreenOn(); // 不息屏
        }
    }

    void LoadScene()
    {
        
        SceneManager.LoadScene("Init");
    }
    
    public static AndroidJavaObject Activity
    {
        get
        {
            AndroidJavaClass jcPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            return jcPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        }
    }
    
    const int FLAG_KEEP_SCREEN_ON = 128;
    public static void KeepScreenOn()
    {
        Debug.Log("Click KeepscreenOn");
        try
        {
            Activity.Call("runOnUiThread", new AndroidJavaRunnable(() => {
                //需要在UI线程中调用
                Activity.Call<AndroidJavaObject>("getWindow").Call("addFlags", FLAG_KEEP_SCREEN_ON);
            }));
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
