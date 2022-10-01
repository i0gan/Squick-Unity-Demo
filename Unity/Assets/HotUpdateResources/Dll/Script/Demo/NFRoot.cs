//-----------------------------------------------------------------------
// <copyright file="NFILogicClassModule.cs">
//     Copyright (C) 2015-2019 lvsheng.huang <https://github.com/ketoo/SquickProtocol>
// </copyright>
//-----------------------------------------------------------------------
using UnityEngine;
using System.Collections;
using Squick;
using SquickProtocol;

public enum GAME_MODE
{
    GAME_MODE_NONE,
    GAME_MODE_2D,
    GAME_MODE_3D,
};

public class NFRoot : MonoBehaviour 
{
    public bool mbShowCMDGUI = false;
    public int port = 14001;

    private GAME_MODE mGameMode = GAME_MODE.GAME_MODE_NONE;

    private ObjectElement mxObjectElement;
    private bool mbShowElement = false;
    private bool mbShowServer = true;

	private IClassModule mClassModule;
	private IKernelModule mKernelModule;
	private NetModule mNetModule;
	private NFUIModule mUIModule;
	private LogModule mLogModule;

	private NFConfig mConfig = new NFConfig();

    private PluginManager mPluginManager;
    private static NFRoot _instance = null;
	public static NFRoot Instance()
    {
        return _instance;
    }

    public GAME_MODE GetGameMode()
    {
        return this.mGameMode;
    }
    public void SetGameMode(GAME_MODE mode)
    {
        this.mGameMode = mode;
    }

    public IPluginManager GetPluginManager()
    {
        return mPluginManager;
    }

    private void Awake()
	{
		mPluginManager = new PluginManager(); // 创建插件管理器
		mxObjectElement = new ObjectElement(); // 获取对象元素
	}

	void Start()
    {
        _instance = this;

        Debug.Log("Root Start " + Application.platform);
        RenderSettings.fog = false;

        mConfig.Load(); // 加载配置文件


        mPluginManager.Registered(new SquickPlugin(mPluginManager));   // 注册SDK插件
		mPluginManager.Registered(new NFUIPlugin(mPluginManager));    // 注册UI插件
		mPluginManager.Registered(new NFScenePlugin(mPluginManager)); // 注册场景插件

        // 获取基本模块
		mKernelModule = mPluginManager.FindModule<IKernelModule>();
		mClassModule = mPluginManager.FindModule<IClassModule>();
		mNetModule = mPluginManager.FindModule<NetModule>();
		mUIModule = mPluginManager.FindModule<NFUIModule>();
		mLogModule = mPluginManager.FindModule<LogModule>();

        // 设置类模块路径
		mClassModule.SetDataPath(mConfig.GetDataPath());

		mPluginManager.Awake();
        mPluginManager.Init();
        mPluginManager.AfterInit();

		mUIModule.ShowUI<NFUILogin>(); // 显示登录UI界面

		if (mConfig.GetServerList().Count > 1)
		{
			mbShowServer = true;
		}
		else
		{
            Debug.Log("选择服务器...");
            // 连接代理服务器 127.0.0.1 15001
			string strTargetIP = "1.14.123.62";
            if (mConfig.GetSelectServer(ref strTargetIP))
            {
                mNetModule.StartConnect(strTargetIP, port);
            }
		}

        DontDestroyOnLoad(gameObject);
	}
	
    void OnDestroy()
    {
        Debug.Log("Root OnDestroy");
        mPluginManager.BeforeShut();
        mPluginManager.Shut();
        mPluginManager = null;
    }
	
	void Update () 
    {
		mPluginManager.Execute();
	}

	private UnityEngine.Vector2 scrollPosition = UnityEngine.Vector2.zero;
    private string strIP = "";


    private float sliderValue = 1.0f;
    private bool bStopTime = false;
    private void OnGUI()
    {

        if (mbShowServer)
        {
            ArrayList arrayList = mConfig.GetServerList();
            scrollPosition = GUI.BeginScrollView(new Rect(Screen.width / 2 - 200, 0, 400, 600), scrollPosition, new Rect(0, 0, 400, arrayList.Count * 100));

            //all object
            for (int i = 0; i < arrayList.Count; i++)
            {
                NFConfig.Server server = (NFConfig.Server)arrayList[i];

                if (GUI.Button(new Rect(0, i * 100, 400, 100), server.strName + " " + server.strIP))
                {
                    mbShowServer = false;
                    mNetModule.StartConnect(server.strIP, port);
                }
            }

            if (strIP.Length == 0)
            {
                strIP = PlayerPrefs.GetString("IP");
            }

            strIP = GUI.TextField(new Rect(0, arrayList.Count * 100, 300, 100), strIP);
            if (GUI.Button(new Rect(300, arrayList.Count * 100, 100, 100), "connect"))
            {
                if (strIP.Length > 0)
                {
                    mbShowServer = false;
                    mNetModule.StartConnect(strIP, port);

                    PlayerPrefs.SetString("IP", strIP);
                }
            }

            GUI.EndScrollView();
        }
        else
        {
            if (mNetModule.GetState() == NetState.Disconnected)
            {
                mbShowServer = true;

            }
        }

        if (!mbShowCMDGUI)
        {
            return;
        }
        //if (Application.platform == RuntimePlatform.OSXEditor
        //    || Application.platform == RuntimePlatform.OSXPlayer)
        {
            mLogModule.PrintGUILog();

            GUI.Label(new Rect(0, 0, 200, 20), "Speed:" + sliderValue.ToString());
            sliderValue = GUI.HorizontalSlider(new Rect(80, 0, Screen.width - 80 * 2, 20), sliderValue, 0.0f, 1.0f);
            if (!bStopTime)
            {
                Time.timeScale = sliderValue;
            }
            if (GUI.Button(new Rect(Screen.width - 80, 0, 40, 20), "暂停"))
            {
                if (bStopTime)
                {
                    Time.timeScale = sliderValue;
                    bStopTime = false;
                }
                else
                {
                    Time.timeScale = 0.0f;
                    bStopTime = true;
                }
            }
            if (mbShowCMDGUI)
            {
                if (GUI.Button(new Rect(0, 0, 40, 20), "ROLE"))
                {
                    mbShowElement = !mbShowElement;
                }
            }
		}

		if (mbShowElement)
		{
			mxObjectElement.OnGUI(mKernelModule, 750, 1334);
		}

	}
}
