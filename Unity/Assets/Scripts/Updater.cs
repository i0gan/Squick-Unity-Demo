using System;
using System.Collections.Generic;
using System.IO;
using BM;
using ET;
using Uquick.Core;
using UnityEngine;
using System.Threading;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public interface IUpdater
{
    void OnMessage(string msg);

    void OnProgress(float progress);

    void OnVersion(string ver);

    void OnLoadSceneProgress(float progress);

    void OnLoadSceneFinish();
}

public class BaseUpdater : IUpdater
{
    public Action<string> onMessage;

    public void OnMessage(string msg)
    {
        onMessage?.Invoke(msg);
    }

    public Action<float> onProgress;

    public void OnProgress(float progress)
    {
        onProgress?.Invoke(progress);
    }

    public Action<string> onVersion;

    public void OnVersion(string ver)
    {
        onVersion?.Invoke(ver);
    }

    public Action<float> onLoadSceneProgress;

    public void OnLoadSceneProgress(float progress)
    {
        onLoadSceneProgress?.Invoke(progress);
    }

    public Action onLoadSceneFinish;

    public void OnLoadSceneFinish()
    {
        onLoadSceneFinish?.Invoke();
    }

    public BaseUpdater(Action<string> onMessage, Action<float> onProgress, Action<string> onVersion,
        Action<float> onLoadSceneProgress, Action onLoadSceneFinish)
    {
        this.onMessage = onMessage;
        this.onProgress = onProgress;
        this.onVersion = onVersion;
        this.onLoadSceneProgress = onLoadSceneProgress;
        this.onLoadSceneFinish = onLoadSceneFinish;
    }
}

public class Updater : MonoBehaviour
{
    [SerializeField] private string baseURL = "http://127.0.0.1:7888/dlc/";
    [SerializeField] private string gameScene = "Assets/HotUpdate/Scene/AppStart.unity";
    [SerializeField] private string mainPackageName = "Main";

    [Tooltip("主包秘钥，如果加密了的话需要填写")]
    [SerializeField]
    private string mainPackageKey = "";

    [Tooltip("主包是否需要校验CRC")]
    [SerializeField]
    private bool mainPackageCheckCRC = true;

    [Tooltip("Develop是开发模式，Local是离线模式，Build是真机模式")]
    [SerializeField]
    private AssetLoadMode mode;

    private float tickTime = 0.0f;
    enum Process
    {
        None,
        CheckApplicationVersion,
        UpdateApplication,
        UpdatePackage
    }

    Process process = Updater.Process.CheckApplicationVersion;
    bool isProcessing = false;


    /// <summary>
    /// 获取分包信息
    /// </summary>
    /// <param name="bundlePackageName">包名</param>
    /// <param name="checkCRC"></param>
    public static async ETTask<UpdateBundleDataInfo> CheckPackage(string bundlePackageName, bool checkCRC = true)
    {
        return await AssetComponent.CheckAllBundlePackageUpdate(new Dictionary<string, bool>()
        {
            { bundlePackageName, checkCRC }
        });
    }

    /// <summary>
    /// 获取本地包版本信息（没下载过就是0）
    /// </summary>
    /// <param name="bundlePackageName">包名</param>
    /// <param name="package">分包信息（可以留空，自动根据包名获取）</param>
    /// <returns></returns>
    public static async ETTask<int> GetLocalPackageVersion(string bundlePackageName,
        UpdateBundleDataInfo package = null)
    {
        package = package ?? await CheckPackage(bundlePackageName, false);
        return package.GetVersion(bundlePackageName)[0];
    }

    /// <summary>
    /// 获取远程包版本信息
    /// </summary>
    /// <param name="bundlePackageName">包名</param>
    /// <param name="package">分包信息（可以留空，自动根据包名获取）</param>
    /// <returns></returns>
    public static async ETTask<int> GetRemotePackageVersion(string bundlePackageName,
        UpdateBundleDataInfo package = null)
    {
        package = package ?? await CheckPackage(bundlePackageName, false);
        return package.GetVersion(bundlePackageName)[1];
    }

    /// <summary>
    /// 下载包
    /// </summary>
    /// <param name="bundlePackageName">包名</param>
    /// <param name="updater"></param>
    /// <param name="checkCRC"></param>
    /// <param name="package"></param>
    /// <param name="key"></param>
    /// <param name="nextScene"></param>
    public static async void UpdatePackage(string bundlePackageName, IUpdater updater, bool checkCRC = true,
        UpdateBundleDataInfo package = null, string key = null, string nextScene = null)
    {
        // 检查资源包更新
        if (string.IsNullOrEmpty(key)) key = null;
        //MessageBox.Dispose();
        package = package ?? await CheckPackage(bundlePackageName, checkCRC);
        updater.OnVersion("版本:" + Application.version + "RES" +
                          await GetRemotePackageVersion(bundlePackageName, package));

        async void Init()
        {
            updater.OnProgress(1);
            updater.OnMessage("更新完成");
            await AssetComponent.Initialize(bundlePackageName, key);
            if (string.IsNullOrEmpty(nextScene)) return;
            updater.OnMessage("正在进入");
            //Debug.Log("最初加载常见" + nextScene);
            AssetMgr.LoadSceneAsync(nextScene, false, bundlePackageName, updater.OnLoadSceneProgress,
                ao => updater.OnLoadSceneFinish());
        }


        // 更新资源
        if (package.NeedUpdate && AssetComponentConfig.AssetLoadMode == AssetLoadMode.Build)
        {
            //updater.OnMessage($"发现{package.NeedDownLoadBundleCount}个更新资源，总需要更新: {Tools.GetDisplaySize(package.NeedUpdateSize)}");
            //var tips = $"发现{package.NeedDownLoadBundleCount}个资源有更新，总计需要更新 {Tools.GetDisplaySize(package.NeedUpdateSize)}";
            //var mb = MessageBox.Show("提示", tips, "更新", "退出");
            //long now = Tools.TimeStamp;
            package.OnProgress = info =>
            {
                //float diff = Tools.TimeStamp - now;
                //diff = diff < 1 ? 1 : diff;
                //diff *= 1000;//ms -> s
                //var speed = info.FinishUpdateSize / diff;
                updater.OnMessage(
                    //$"下载中...{Tools.GetDisplaySpeed(speed)}, 进度：{Math.Round(info.Progress, 2)}%");
                    $"正在更新中： {Tools.GetDisplaySize(info.FinishUpdateSize)} / {Tools.GetDisplaySize(package.NeedUpdateSize)}    {Math.Round(info.Progress, 2)}% ");
                updater.OnProgress(info.Progress / 100f);
            };
            await AssetComponent.DownLoadUpdate(package);
            Init();
            /*
            async void ONComplete(MessageBox.EventId ok)
            {
                if (ok == MessageBox.EventId.Ok)
                {
                    
                }
                else
                {
                    Quit();
                }
            }
            mb.onComplete = ONComplete;*/
        }
        else
        {
            Init();
        }
    }

    /// <summary>
    /// 下载包
    /// </summary>
    /// <param name="bundlePackageName"></param>
    /// <param name="updater"></param>
    /// <param name="checkCRC"></param>
    /// <param name="key"></param>
    /// <param name="nextScene"></param>
    public static void UpdatePackage(string bundlePackageName, IUpdater updater, bool checkCRC = true,
        string key = null,
        string nextScene = null) // 入口
    {
        UpdatePackage(bundlePackageName, updater, checkCRC, null, key, nextScene);
    }


    void CheckApplicationVersion(IUpdater updater)
    {
        // 检测apk是否需要更新

        Debug.Log("正在检查安装包版本中");
        HttpRestful.Instance.Get(AssetComponentConfig.BundleServerUrl + "/version", new Action<bool, string>(
            (bl, str) =>
            {
                if (bl)
                {
                    // 错误
                    updater.OnMessage("网络连接失败");
                    Debug.Log("网络连接失败");
                    StartCoroutine(WaitTimeTodo(1.0f, () =>
                    {
                        isProcessing = false;
                        process = Process.CheckApplicationVersion;
                    }));
                }

                else
                {
                    JObject o;
                    float version;
                    try
                    {
                        o = JObject.Parse(str);
                        version = (float)o["Version"];
                    }
                    catch (Exception)
                    {
                        version = 0.0f;
                    }

                    Debug.Log("远程版本：" + version.ToString());

                    if (version > float.Parse(Application.version))
                    {
                        // 需要更新
                        Debug.Log("安装包更新");
                        updater.OnMessage($"安装包更新至： " + version.ToString());
                        StartCoroutine(WaitTimeTodo(2.0f, () =>
                        {
                            isProcessing = false;
                            process = Process.UpdateApplication;
                        }));
                    }
                    else
                    {
                        // 无需更新
                        updater.OnMessage($"安装包已达到最新版本");
                        Debug.Log("安装包已达到最新版本");
                        StartCoroutine(WaitTimeTodo(1.0f, () =>
                        {
                            updater.OnMessage($"正在检查热更资源文件");
                            Debug.Log("正在检查热更资源文件");
                            isProcessing = false;
                            process = Process.UpdatePackage;
                        }));
                    }
                }
            }));
    }


    IEnumerator UpdateApplicationVersion(IUpdater updater)
    {
#if UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            Debug.Log("下载安装包中");
            string downloadURL = AssetComponentConfig.BundleServerUrl + "/application.apk";

            UnityWebRequest headRequest = UnityWebRequest.Head(downloadURL);
            //开始与远程服务器通信。
            yield return headRequest.SendWebRequest();

            if (!string.IsNullOrEmpty(headRequest.error))
            {
                Debug.LogError("获取不到资源文件");
                yield break;
            }

            //获取文件总大小
            ulong totalLength = ulong.Parse(headRequest.GetResponseHeader("Content-Length"));
            Debug.Log("获取大小" + totalLength);
            headRequest.Dispose();


            using (UnityWebRequest request = UnityWebRequest.Get(downloadURL))
            {
                request.SendWebRequest();
                while (!request.isDone)
                {
                    updater.OnMessage(
                        $"安装包更新中   {Tools.GetDisplaySize((long)request.downloadedBytes)} / {Tools.GetDisplaySize((long)totalLength)}    {Math.Round(request.downloadProgress * 100, 2)}%");
                    updater.OnProgress(request.downloadProgress * 100 / 100f);
                    yield return new WaitForSeconds(0.5f);
                }

                if (request.isNetworkError || request.isHttpError)
                {
                    updater.OnMessage("网络连接失败");
                    Debug.Log("网络连接失败");
                    StartCoroutine(WaitTimeTodo(1.0f, () =>
                    {
                        isProcessing = false;
                        process = Process.UpdateApplication;
                    }));
                }
                else
                {
                    updater.OnMessage("更新完成，正在保存安装包");
                    yield return new WaitForSeconds(0.5f);
                    string path = "/storage/emulated/0" + "/Download/" + Application.productName + "_" +
                                  Application.version + ".apk";
                    Debug.Log("更新完成，请求安装中，保存路径： " + path);
                    DownloadHandler fileHandler = request.downloadHandler;
                    using (MemoryStream memory = new MemoryStream(fileHandler.data))
                    {
                        byte[] buffer = new byte[1024 * 1024];
                        FileStream file = File.Open(path, FileMode.OpenOrCreate);
                        int readBytes;
                        while ((readBytes = memory.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            file.Write(buffer, 0, readBytes);
                        }

                        file.Close();
                    }

                    updater.OnMessage("正在请求安装");
                    yield return new WaitForSeconds(0.5f);
                    InstallApk(path);
                }
            }
        }
#endif
        yield return new WaitForSeconds(0.5f);

    }

    // 安装apk
    public static bool InstallApk(string apkPath)
    {
        AndroidJavaClass javaClass = new AndroidJavaClass("com.pwnsky.tflash.unity3d.Main");
        return javaClass.CallStatic<bool>("InstallApk", apkPath);
    }


    /// <summary>
    /// 下载包
    /// </summary>
    /// <param name="bundlePackageName">包名</param>
    /// <param name="checkCRC"></param>
    /// <param name="package"></param>
    /// <param name="key"></param>
    /// <param name="nextScene"></param>
    /// <param name="onMessage"></param>
    /// <param name="onProgress"></param>
    /// <param name="onVersion"></param>
    /// <param name="onLoadSceneProgress"></param>
    /// <param name="onLoadSceneFinished"></param>
    /// 
    public static void UpdatePackage(string bundlePackageName, bool checkCRC = true,
        UpdateBundleDataInfo package = null, string key = null,
        string nextScene = null,
        Action<string> onMessage = null, Action<float> onProgress = null, Action<string> onVersion = null,
        Action<float> onLoadSceneProgress = null, Action onLoadSceneFinished = null)
    {
        BaseUpdater updater =
            new BaseUpdater(onMessage, onProgress, onVersion, onLoadSceneProgress, onLoadSceneFinished);
        UpdatePackage(bundlePackageName, updater, checkCRC, package, key, nextScene);
    }

    /// <summary>
    /// 删除缓存的包
    /// </summary>
    /// <param name="bundlePackageName"></param>
    public static void ClearPackage(string bundlePackageName)
    {
        var dir = Path.Combine(Application.persistentDataPath, "bundlePackageName");
        if (Directory.Exists(dir))
        {
            Directory.Delete(dir);
        }
    }


    IEnumerator WaitTimeTodo(float waitTimer, UnityAction action)
    {
        if (waitTimer <= 0) waitTimer = 0;
        yield return new WaitForSeconds(waitTimer);
        action.Invoke();
    }


    /// <summary>
    /// 更新配置
    /// </summary>
    private void Awake()
    {
        // 播放背景音乐


        baseURL = baseURL.EndsWith("/") ? baseURL : baseURL + "/";
        process = Updater.Process.CheckApplicationVersion;
        isProcessing = false;
        tickTime = Time.time;
#if UNITY_EDITOR
        AssetComponentConfig.AssetLoadMode = mode;
#else
    // 其他平台
    if(AssetComponentConfig.AssetLoadMode == AssetLoadMode.Local) {
        AssetComponentConfig.AssetLoadMode = AssetLoadMode.Local;
    }else {
        // 其他一律采用Build模式
        AssetComponentConfig.AssetLoadMode = AssetLoadMode.Build;
    }
#endif
        AssetComponentConfig.BundleServerUrl = baseURL;
        AssetComponentConfig.DefaultBundlePackageName = mainPackageName;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (Time.time - tickTime > 1.0)
        {
            if (process == Process.CheckApplicationVersion && isProcessing == false) // 检验主程序
            {
#if UNITY_EDITOR || UNITY_STANDALONE
                isProcessing = false;
                process = Process.UpdatePackage;
#elif UNITY_ANDROID || UNITY_IOS
            isProcessing = true;
            CheckApplicationVersion(FindObjectOfType<UpdateScreen>());
#endif
            }
            else if (process == Process.UpdateApplication && isProcessing == false) // 更新主程序
            {
                isProcessing = true;
                StartCoroutine(UpdateApplicationVersion(FindObjectOfType<UpdateScreen>()));
            }
            else if (process == Process.UpdatePackage && isProcessing == false)
            {
                isProcessing = true;
                UpdatePackage(mainPackageName, FindObjectOfType<UpdateScreen>(), mainPackageCheckCRC, mainPackageKey,
                    gameScene);
            }
            tickTime = Time.time;
        }
    }

    private void Start()
    {
        Game.Audio.GetInstance.AB_PlayAudio("UpdateBgm");
        Application.runInBackground = true;
    }


    private void OnDestroy()
    {
        //MessageBox.Dispose();
    }


    public static async ETTask CheckAndroidVersion()
    {
    }


    private static void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}