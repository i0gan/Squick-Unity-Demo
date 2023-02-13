
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

public class SquickRoot : MonoBehaviour
{
    public bool mbShowCMDGUI = false;
    public int port = 15001;

    private GAME_MODE mGameMode = GAME_MODE.GAME_MODE_NONE;

    private ObjectElement mxObjectElement;

    public IClassModule mClassModule;
    public IKernelModule mKernelModule;
    public NetModule mNetModule;
    public UIModule mUIModule;
    public LogModule mLogModule;
    public SquickConfig mConfig = new SquickConfig();
    public PluginManager mPluginManager;

    private static SquickRoot _instance = null;
    public static SquickRoot Instance()
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
        mPluginManager = new PluginManager();  // 创建插件管理器
        mxObjectElement = new ObjectElement(); // 获取对象元素
    }

    void Start()
    {
        _instance = this;
        RenderSettings.fog = false;

        mConfig.Load(); // 加载配置文件

        mPluginManager.Registered(new SquickPlugin(mPluginManager));   // 注册SDK插件
        mPluginManager.Registered(new UIPlugin(mPluginManager));       // 注册UI插件
        mPluginManager.Registered(new ScenePlugin(mPluginManager));    // 注册场景插件

        // 获取基本模块
        mKernelModule = mPluginManager.FindModule<IKernelModule>();
        mClassModule = mPluginManager.FindModule<IClassModule>();
        mNetModule = mPluginManager.FindModule<NetModule>();
        mUIModule = mPluginManager.FindModule<UIModule>();
        mLogModule = mPluginManager.FindModule<LogModule>();

        // 设置类模块路径
        mClassModule.SetDataPath(mConfig.GetDataPath());

        mPluginManager.Awake();
        mPluginManager.Init();
        mPluginManager.AfterInit();


        // 显示登录UI界面
        mUIModule.ShowUI<UILogin>();

        string strTargetIP = "";        
        if (mConfig.GetSelectServer(ref strTargetIP))
        {
            Debug.Log("连接服务器: ..." + strTargetIP + ":" + port);
            mNetModule.StartConnect(strTargetIP, port);
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

    void Update()
    {
        mPluginManager.Execute();
    }
}
