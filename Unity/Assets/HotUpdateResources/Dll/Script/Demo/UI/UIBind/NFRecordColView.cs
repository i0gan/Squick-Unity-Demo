using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Squick;
using SquickProtocol;

public class RecordColView : MonoBehaviour 
{
	public enum ViewType
	{
		ORIGINAL = 0,
		HERO_GUID_ICON,
		NPC_CNFID_ICON,
		ITEM_CNFID_ICON
	};

	public int col;
	public ViewType type = ViewType.ORIGINAL;

	private RecordRowView rowView;

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

		int iNum = 0;
		Transform tParent = this.transform.parent;
		while(tParent)
		{
			rowView = tParent.GetComponent<RecordRowView> ();
			if (rowView != null)
			{
				rowView.AddColView (col, this);
				break;
			}
		
			iNum++;
			if (iNum > 3)
			{
				break;
			}

			tParent = tParent.parent;
		}
	}

	// Use this for initialization
	void Start () 
	{
		
	}
	
	public void Refresh(Guid self, DataList.TData data)
	{
		switch (type)
		{
			case ViewType.ITEM_CNFID_ICON:
				{
					Image xImage = gameObject.GetComponent<Image> ();
					if (xImage != null)
					{
                        /*
						string strIconName = mElementModule.QueryPropertyString (data.ToString(), SquickProtocol.Item.Icon);
						string strIconFileName = mElementModule.QueryPropertyString (data.ToString(), SquickProtocol.Item.SpriteFile);
						Sprite xSprite = NFTexturePacker.Instance.GetSprit(strIconFileName, strIconName);
						if (xSprite != null)
						{
							xImage.overrideSprite = xSprite;
							if (xImage.sprite == null) 
							{
								xImage.enabled = false;
							}
							else 
							{
								xImage.enabled = true;
							}
						}
                        */
					}
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
								if (xImage.sprite == null)
								{
									xImage.enabled = false;
								}
								else
								{
									xImage.enabled = true;
								}
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

				}
				break;
			default:
				{
					Text xText = gameObject.GetComponent<Text> ();
					if (xText != null)
					{
						xText.text = data.ToString ();
					}
				}
				break;
		}
	}
}