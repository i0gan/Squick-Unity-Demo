using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;
using Squick;
using SquickProtocol;
using UnityEngine.UI;
using Uquick.Core;

public class UILoading : UIDialog
{
	public Sprite[] xTextureList;
    public Image GUIHolder;
    public Slider slider;
    public Image bar;

    private int mnSceneID = 0;

	private LoginModule mLoginModule;
    private UIModule mUIModule;
    private SceneModule mSceneModule;
	private ISEventModule mEventModule;
	private IElementModule mElementModule;

    private void Awake()
    {
        IPluginManager xPluginManager = SquickRoot.Instance().GetPluginManager();
        mLoginModule = xPluginManager.FindModule<LoginModule>();
        mUIModule = xPluginManager.FindModule<UIModule>();
        mSceneModule = xPluginManager.FindModule<SceneModule>();

        mEventModule = xPluginManager.FindModule<ISEventModule>();
        mElementModule = xPluginManager.FindModule<IElementModule>();

    }

    // Use this for initialization
    public override void Init()
	{
	}
    
	void Start()
    {
    }

    void OnGUI()
    {
    }

    private void Update()
    {


    }

    public void LoadLevel(int nSceneID, Vector3 vector)
    {
   
        mnSceneID = nSceneID;
        bar.fillAmount = 0;

		Squick.IElement xElement = mElementModule.GetElement(nSceneID.ToString());
		if (null != xElement) 
		{
			string strName = xElement.QueryString (SquickProtocol.Scene.SceneName);
			string strUIName = xElement.QueryString (SquickProtocol.Scene.LoadingUI);

			UnityEngine.SceneManagement.Scene xSceneInfo = SceneManager.GetActiveScene ();
			if (xSceneInfo.name == strName)
			{
				//Debug.LogWarning ("begin the same scene" + strSceneID);
			}
            StartCoroutine(LoadLevel (nSceneID, strName, vector, strUIName));
		}
		else 
		{
			//Debug.LogError ("LoadLevel error: " + nSceneID);
		}

    }

    private IEnumerator LoadLevel(int nSceneID, string strSceneID, Vector3 vector, string strUI)
    {
        yield return new WaitForEndOfFrame();
        AssetMgr.LoadSceneAsync("Assets/HotUpdateResources/Scene/" + strSceneID + ".unity");
        mSceneModule.LoadSceneEnd(mnSceneID);
        mUIModule.CloseUI<UILoading>();
    }
}