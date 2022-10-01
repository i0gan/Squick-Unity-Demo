using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;
using Squick;
using SquickProtocol;

public class RecordRowView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	public delegate void RowClickEventHandler(RecordRowData data);
	public delegate void RowPressDownEventHandler(RecordRowData data);
	public delegate void RowPressUpEventHandler(RecordRowData data);
    public delegate void RowViewUpdateEventHandler(Squick.Guid self, string recordName, int nRow, RecordRowView view);

	//must working as the col-view manager
	private Dictionary<int, RecordColView> colViewList = new Dictionary<int, RecordColView>();
	private RecordRowData data;
	private RecordController controller;
	private List<RowClickEventHandler> eventHandler = new List<RowClickEventHandler>();

	private List<RowPressDownEventHandler> eventDwonHandler = new List<RowPressDownEventHandler>();
	private List<RowPressUpEventHandler> eventUpHandler = new List<RowPressUpEventHandler>();

    public GameObject selectPanel;
	private static GameObject lastSelect;
    //public Text text;

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

    void Start()
    {
    	Button btn = this.gameObject.GetComponent<Button>();
    	if (btn == null)
    	{
    		btn = this.gameObject.AddComponent<Button> ();
    	}
    
    	btn.enabled = true;
    	btn.onClick.AddListener(delegate () { this.OnClick(this.gameObject); });
    }
    
    public void AddColView(int col, RecordColView colView)
    {
    	colViewList.Add (col, colView);
    }
    
    public void RemoveAllClickEvent()
    {
    	eventHandler.Clear ();
    }
    
    public void RegisterClickEvent(RowClickEventHandler handler)
    {
    	eventHandler.Add (handler);
    }

    public void RegisterPressDownEvent(RowPressDownEventHandler handler)
    {
        eventDwonHandler.Add(handler);
    }
    public void RegisterPressUpEvent(RowPressUpEventHandler handler)
    {
        eventUpHandler.Add(handler);
    }

    public RecordRowData GetData()
    {
        return data;
    }

    public void SetData(Squick.Guid xGUID, string strRecordName, RecordController xController, RecordRowData xData)
    {
		data = xData;
		controller = xController;

		if (data != null)
		{
			foreach (KeyValuePair<int, RecordColView> entry in colViewList)
			{
			    IRecord xRecord = mkernelModule.FindRecord (xGUID, strRecordName);

				entry.Value.Refresh (xGUID, xRecord.QueryRowCol (data.row, entry.Key));
			}

			xController.UpdateEvent (xData.id, xData.recordName, xData.row, this);
		}
    }

    void OnClick(GameObject go)
    {
    	for (int i = 0; i < eventHandler.Count; ++i)
    	{
    		RowClickEventHandler handler = eventHandler [i];
    		handler(data);
    	}
    
    	controller.ClickEvent (data);

    	if (lastSelect != null)
    	{
    		lastSelect.SetActive (false);
    	}
    
    	if (selectPanel != null)
    	{
    		selectPanel.SetActive (true);
    	}
    
    	lastSelect = selectPanel;
    }

    public void OnMouseEnter()
    {
        OnPointerDown(null);
    }

    public void OnMouseExit()
    {
        OnPointerUp(null);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        for (int i = 0; i < eventDwonHandler.Count; ++i)
        {
            RowPressDownEventHandler handler = eventDwonHandler[i];
            handler(data);
        }

        controller.DownEvent(data);

        if (lastSelect != null)
        {
            lastSelect.SetActive(false);
        }

        if (selectPanel != null)
        {
            selectPanel.SetActive(true);
        }

        lastSelect = selectPanel;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        for (int i = 0; i < eventUpHandler.Count; ++i)
        {
            RowPressUpEventHandler handler = eventUpHandler[i];
            handler(data);
        }

        controller.UpEvent(data);
    }
}