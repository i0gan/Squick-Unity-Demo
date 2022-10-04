using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Uquick.Core;
using UnityEngine;

namespace Squick
{
	public class UIModule : IModule
    {
        private Dictionary<string, GameObject> mAllUIs = new Dictionary<string, GameObject>();
        private Queue<UIDialog> mDialogs = new Queue<UIDialog>();
        private UIDialog mCurrentDialog = null;

        public override void Awake() {}
        public override void AfterInit() {}
        public override void Execute() { }
        public override void BeforeShut() {}
        public override void Shut() { }

        public UIModule(IPluginManager pluginManager)
        {
            mPluginManager = pluginManager;
		}
        
        public override void Init() 
        { 
        }

		public T ShowUI<T>(bool bCloseLastOne = true, bool bPushHistory = true, DataList varList = null) where T : UIDialog
        {
            /*
            if (mCurrentDialog != null && bCloseLastOne)
            {
                Debug.Log("close ui " + mCurrentDialog.gameObject.name);

                mCurrentDialog.gameObject.SetActive(false);
                mCurrentDialog = null;
            }
            */

            string name = typeof(T).ToString();
            GameObject uiObject;
            if (!mAllUIs.TryGetValue(name, out uiObject))
            {
                //GameObject perfb = Resources.Load<GameObject>("UI/" + name);
                GameObject perfb = AssetMgr.Load<GameObject>("Assets/HotUpdate/UI/" + name + ".prefab");
                Debug.Log("Load UI: " + name);
                if (Application.platform == RuntimePlatform.Android
                    ||Application.platform == RuntimePlatform.IPhonePlayer
                    || Application.platform == RuntimePlatform.OSXEditor
                    || Application.platform == RuntimePlatform.WindowsEditor
                    || Application.platform == RuntimePlatform.LinuxEditor)
                {
                    //perfb = Resources.Load<GameObject>("UI/" + name);
                }
                else
                {
                    //perfb = Resources.Load<GameObject>("UI/PC/" + name);
                }

                Debug.Log(name);

                uiObject = GameObject.Instantiate(perfb);
                uiObject.name = name;

                uiObject.transform.SetParent(SquickRoot.Instance().transform);

                mAllUIs.Add(name, uiObject);

                T panel = uiObject.GetComponent<T>();
				panel.Init();
            }
            else
            {
                uiObject.SetActive(true);

				//Debug.Log("open ui " + uiObject.gameObject.name);
            }

            if (uiObject)
            {
                T panel = uiObject.GetComponent<T>();
                if (varList != null)
                    panel.mUserData = varList;
                
                mCurrentDialog = panel;
                
                uiObject.SetActive(true);

                //Debug.Log("open ui " + uiObject.gameObject.name);

                if (bPushHistory)
                {
                    mDialogs.Enqueue(panel);
                }

				return panel;
            }

			return null;
        }

        public T GetUI<T>() where T : UIDialog
        {
            string name = typeof(T).ToString();
            GameObject uiObject;
            if (mAllUIs.TryGetValue(name, out uiObject))
            {
                return uiObject.GetComponent<T>();
            }

            return null;
        }

        public void CloseUI<T>() where T : UIDialog
        {
            string name = typeof(T).ToString();
            GameObject uiObject;
            if (mAllUIs.TryGetValue(name, out uiObject))
            {
				uiObject.SetActive(false);

                Debug.Log("close ui " + uiObject.gameObject.name);
            }
        }

		public void CloseTopUI()
		{
			if (mCurrentDialog)
            {
                mCurrentDialog.gameObject.SetActive(false);
                mCurrentDialog = null;

				mDialogs.Peek();
			}
		}
      
        public void CloseAllUI()
        {
            if (mCurrentDialog)
            {
                mCurrentDialog.gameObject.SetActive(false);
                mCurrentDialog = null;
            }

            foreach (var item in mAllUIs.ToList())
            {
                item.Value.SetActive(false);
            }

            mDialogs.Clear();
        }

        public void DestroyAllUI()
        {
            foreach (var item in mAllUIs.ToList())
            {
                GameObject.DestroyImmediate(item.Value);
            }

            mAllUIs.Clear();
            mDialogs.Clear();
        }
    }
}