using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Squick;
using SquickProtocol;
using System.Text;

public class NFUILogin : NFUIDialog
{
	private ISEventModule mEventModule;


	private NetModule mNetModule;
	private LoginModule mLoginModule;
	private NFUIModule mUIModule;
    private HelpModule mHelpModule;

	public InputField mAccount;
	public InputField mPassword;
	public Button mLogin;
	// Use this for initialization


	public override void Init()
	{

        // 查找基本模块
		mEventModule = NFRoot.Instance().GetPluginManager().FindModule<ISEventModule>();

		mNetModule = NFRoot.Instance().GetPluginManager().FindModule<NetModule>();
        mLoginModule = NFRoot.Instance().GetPluginManager().FindModule<LoginModule>();
        mUIModule = NFRoot.Instance().GetPluginManager().FindModule<NFUIModule>();
		mHelpModule = NFRoot.Instance().GetPluginManager().FindModule<HelpModule>();

        // 监听登录按钮
        mLogin.onClick.AddListener(OnLoginClick); 

        // 注册回调函数
        mEventModule.RegisterCallback((int)LoginModule.Event.LoginSuccess, OnLoginSuccess);
	}

	void Start () 
	{
        mAccount.text = PlayerPrefs.GetString("account");
        mPassword.text = PlayerPrefs.GetString("password");
    }

    // UI Event
    private void OnLoginClick()
    {

        Debug.Log("验证key");
        // 点击登录
        PlayerPrefs.SetString("account", mAccount.text);
        PlayerPrefs.SetString("password", mPassword.text);
        //mLoginModule.LoginPB(mAccount.text, mPassword.text, "");
       
        mLoginModule.RequireVerifyWorldKey(mAccount.text, mPassword.text);
    }
    
    // Logic Event
	public void OnLoginSuccess(int eventId, DataList valueList)
    {

        Debug.Log("登录成功！");
		//mUIModule.ShowUI<NFUISelectServer>();

        //mLoginModule.RequireWorldList();
    }
}
