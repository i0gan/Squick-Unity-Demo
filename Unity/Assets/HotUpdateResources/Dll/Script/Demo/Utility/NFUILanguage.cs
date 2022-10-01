using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using SquickProtocol;
using Squick;

public class NFUILanguage : MonoBehaviour 
{
	//maybe for text, maybe for sprite(sprite_name)
	public string strText;

	private LanguageModule mLanguageModule;

	void Awake()
	{
		mLanguageModule = NFRoot.Instance().GetPluginManager().FindModule<LanguageModule>();
		mLanguageModule.AddLanguageUI (this.gameObject);
	}

	// Use this for initialization
	void Start () 
	{
		RefreshUIData ();
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	void OnDestroy()
	{
		mLanguageModule.RemLanguageUI (this.gameObject);
	}

	public void RefreshUIData()
	{
		//language option
		string strData = mLanguageModule.GetLocalLanguage(strText);
		Text xText = GetComponent<Text> ();
		if (xText) 
		{
			xText.text = strData.Replace("\\n", "\n");
		} 
		else		
		{
			//image
		}
	}

}
