using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SquickProtocol;
using Squick;
namespace Squick
{

	public class LanguageModule : IModule
	{
		private Dictionary<GameObject, string> mxUIGO = new Dictionary<GameObject, string>();
		private string mstrLocalLanguage = SquickProtocol.Language.Chinese;


		private IElementModule mElementModule;

		public LanguageModule(IPluginManager pluginManager)
		{
			mPluginManager = pluginManager;
		}

		public override void Awake(){}
		public override void Init()
		{
			mElementModule = mPluginManager.FindModule<IElementModule>();
		}

		public override void AfterInit(){ }
		public override void Execute(){ }
		public override void BeforeShut(){ }
		public override void Shut(){ }

		public string GetLocalLanguage(string strLanguageID)
		{
			//SquickProtocol.Language.Chinese
			return mElementModule.QueryPropertyString(strLanguageID, mstrLocalLanguage);
		}

		public void SetLocalLanguage(string strLanguageName)
		{
			mstrLocalLanguage = strLanguageName;

			RefreshUILanguage();
		}

		public void AddLanguageUI(GameObject go)
		{
			mxUIGO.Add(go, "");
		}

		public void RemLanguageUI(GameObject go)
		{
			mxUIGO.Remove(go);
		}

		void RefreshUILanguage()
		{
			foreach (var x in mxUIGO)
			{
				GameObject go = x.Key;
				UILanguage xLanguage = go.GetComponent<UILanguage>();
				if (xLanguage)
				{
					xLanguage.RefreshUIData();
				}
			}
		}
	}
}
