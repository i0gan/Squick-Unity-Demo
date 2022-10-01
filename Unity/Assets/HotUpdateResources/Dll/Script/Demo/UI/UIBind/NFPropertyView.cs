using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using SquickProtocol;
using Squick;
using System;

public class PropertyView : MonoBehaviour {

	public enum ViewType
	{
		ORIGINAL = 0,
		HERO_GUID_ICON,
		NPC_CNFID_ICON,
		ITEM_CNFID_ICON,
		SKILL_CNFID_ICON
	};
    public string propertyName;
    public bool group = false;
	public ViewType type = ViewType.ORIGINAL;

    private Squick.Guid bindID = new Squick.Guid();
	private IKernelModule mkernelModule;
	private IClassModule mClassModule;
	private IElementModule mElementModule;
	private LoginModule mLoginModule;

	// Use this for initialization
	private void Awake()
	{
		mkernelModule = NFRoot.Instance().GetPluginManager().FindModule<IKernelModule>();
		mClassModule = NFRoot.Instance().GetPluginManager().FindModule<IClassModule>();
		mLoginModule = NFRoot.Instance().GetPluginManager().FindModule<LoginModule>();
		mElementModule = NFRoot.Instance().GetPluginManager().FindModule<IElementModule>();
	}

    private void OnDestroy()
    {
        //TODO
        //unregister callback
    }

    void Start () 
	{
        bindID = group ? Squick.Guid.Zero : mLoginModule.mRoleID;

        //register callback
        mkernelModule.RegisterClassCallBack(SquickProtocol.Player.ThisName, OnClassPlayerEventHandler);

		//generally speaking, this object will be created after the player login (be created)
        //as a result, we must add the data when the UI object creating to show the data at the UI.

		IProperty xProperty = mkernelModule.FindProperty(bindID, propertyName);
		if (xProperty != null)
        {
            mkernelModule.RegisterPropertyCallback(bindID, propertyName, PropertyEventHandler);
            mkernelModule.RegisterGroupPropertyCallback(propertyName, PropertyEventHandler);

            IProperty property = mkernelModule.FindProperty(bindID, propertyName);
            if (property != null)
            {
                DataList.TData data = property.GetData();
                PropertyEventHandler(bindID, propertyName, null, data, 0);
            }
        }
        else
        {
			Debug.LogError("there have not a property named: " + propertyName + "  " + this.transform.parent.parent.name + "/" + this.transform.parent.name + "/" + this.gameObject.name);
        }
        /*
		switch (xProperty.GetType())
		{
			case DataList.VARIANT_TYPE.VTYPE_INT:
				
			
		}
		*/



		if (type == ViewType.ORIGINAL)
		{
			Text xText = gameObject.GetComponent<Text> ();
			if (xText != null)
			{
				if (xText.text == "0" || xText.text.Length <= 0)
				{
					//xText.enabled = false;
				}
			}
		}


	}

	// Update is called once per frame
	void Update () 
	{

	}

	void OnClassPlayerEventHandler(Squick.Guid self, int nContainerID, int nGroupID, Squick.IObject.CLASS_EVENT_TYPE eType, string strClassName, string strConfigIndex)
	{
		{
			if (eType == IObject.CLASS_EVENT_TYPE.OBJECT_CREATE)
			{
				//mkernelModule.RegisterPropertyCallback (self, propertyName, PropertyEventHandler);
			}
		}
	}

	void PropertyEventHandler(Squick.Guid self, string strProperty, DataList.TData oldVar, DataList.TData newVar, Int64 reason)
	{
		IProperty xProperty = mkernelModule.FindProperty(self, propertyName);
		DataList.TData data = xProperty.GetData();
		switch (type)
		{
			case ViewType.ITEM_CNFID_ICON:
				{
                    /*
					Image xImage = gameObject.GetComponent<Image> ();
					if (xImage != null)
					{
						string strIconName = mElementModule.QueryPropertyString (data.ToString(), SquickProtocol.Item.Icon);
						string strIconFileName = mElementModule.QueryPropertyString (data.ToString(), SquickProtocol.Item.SpriteFile);
						Sprite xSprite = NFTexturePacker.Instance.GetSprit(strIconFileName, strIconName);
						if (xSprite != null)
						{
							xImage.overrideSprite = xSprite;
						}
					}
                    */
				}
				break;
			case ViewType.HERO_GUID_ICON:
				{
                    /*
					Image xImage = gameObject.GetComponent<Image> ();
					if (xImage != null)
					{
						IRecord xRecord = mkernelModule.FindRecord (self, SquickProtocol.Player.PlayerHero.ThisName);
						int nRow = xRecord.FindObject ((int)SquickProtocol.Player.PlayerHero.GUID, data.ObjectVal ());
						if (nRow >= 0)
						{
							string strCnfID = xRecord.QueryString (nRow, (int)SquickProtocol.Player.PlayerHero.ConfigID);
							string strIconName = mElementModule.QueryPropertyString (strCnfID, SquickProtocol.Item.Icon);
							string strIconFileName = mElementModule.QueryPropertyString (strCnfID, SquickProtocol.Item.SpriteFile);
							Sprite xSprite = NFTexturePacker.Instance.GetSprit (strIconFileName, strIconName);
							if (xSprite != null)
							{
								xImage.overrideSprite = xSprite;
							}
						}
						else
						{
							xImage.enabled = false;
						}
					}
                    */
				}
				break;
			case ViewType.NPC_CNFID_ICON:
				{
					Image xImage = gameObject.GetComponent<Image> ();
					if (xImage != null)
					{
						string strCnfID = data.StringVal ();
						string strIconName = mElementModule.QueryPropertyString (strCnfID, SquickProtocol.NPC.Icon);
						string strIconFileName = mElementModule.QueryPropertyString (strCnfID, SquickProtocol.NPC.SpriteFile);
						Sprite xSprite = NFTexturePacker.Instance.GetSprit (strIconFileName, strIconName);
						if (xSprite != null)
						{
							xImage.overrideSprite = xSprite;
						}
					}
				}
				break;
			case ViewType.SKILL_CNFID_ICON:
				{
					Image xImage = gameObject.GetComponent<Image> ();
					if (xImage != null)
                    {
                        string strCnfID = data.StringVal();
                        string strIconName = mElementModule.QueryPropertyString (strCnfID, SquickProtocol.Skill.Icon);
						string strIconFileName = mElementModule.QueryPropertyString (strCnfID, SquickProtocol.Skill.SpriteFile);
						Sprite xSprite = NFTexturePacker.Instance.GetSprit (strIconFileName, strIconName);
						if (xSprite != null)
						{
							xImage.overrideSprite = xSprite;
						}
					}
				}
				break;
			default:
				{
					Text xText = gameObject.GetComponent<Text> ();
					if (xText != null)
					{
						xText.text = data.ToString ();
						if (xText.text == "0" || xText.text.Length <= 0)
						{
							//xText.enabled = false;
						}
					}
				}
				break;
		}
	}
}