using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Squick;
using SquickProtocol;

public class RecordController : MonoBehaviour
{
    private List<RecordRowData> _data;

	private Guid xGUID;
	private List<RecordRowView.RowClickEventHandler> clickEventHandler = new List<RecordRowView.RowClickEventHandler>();
	private List<RecordRowView.RowPressDownEventHandler> pressDownEventHandler = new List<RecordRowView.RowPressDownEventHandler>();
	private List<RecordRowView.RowPressUpEventHandler> pressUpEventHandler = new List<RecordRowView.RowPressUpEventHandler>();
    private List<RecordRowView.RowViewUpdateEventHandler> updateEventHandler = new List<RecordRowView.RowViewUpdateEventHandler>();

	public RecordRowView rowViewItem;
    public bool group = false;

    public string recordName = "";

	[HideInInspector]
	public bool ColValueCondition = false;
	[HideInInspector]
	public bool ColPropertyCondition = false;
	[HideInInspector]
	public int ColConditionNum= 0;
	[HideInInspector]
	public string ColConditionContent = "";
	[HideInInspector]
	public string ColConditionPropertyName = "";
	[HideInInspector]
	public string ColConditionPropertyValue = "";


	private IKernelModule mkernelModule;
    private IClassModule mClassModule;
    private IElementModule mElementModule;
    private LoginModule mLoginModule;

	// Use this for initialization
	private void Awake()
	{
		mkernelModule = SquickRoot.Instance().GetPluginManager().FindModule<IKernelModule>();
		mClassModule = SquickRoot.Instance().GetPluginManager().FindModule<IClassModule>();
		mLoginModule = SquickRoot.Instance().GetPluginManager().FindModule<LoginModule>();
		mElementModule = SquickRoot.Instance().GetPluginManager().FindModule<IElementModule>();
	}

    void Start()
    {
        // tell the scroller that this script will be its delegate
        _data = new List<RecordRowData>();

		mkernelModule.RegisterClassCallBack(SquickProtocol.Player.ThisName, OnClassPlayerEventHandler);

        //generally speaking, this object will be created after the player login (be created)
        //as a result, we must add the data when the UI object creating to show the data at the UI.
        if (!group)
        {
            xGUID = mLoginModule.mRoleID;
        }
        else
        {
            xGUID = new Guid();
        }
        {
			IRecord xRecord = mkernelModule.FindRecord(mLoginModule.mRoleID, recordName);
            if (xRecord != null)
            {
				mkernelModule.RegisterRecordCallback(mLoginModule.mRoleID, recordName, RecordEventHandler);
				for (int i = 0; i < xRecord.GetRows(); ++i)
				{
					if (xRecord.IsUsed(i))
					{
						RecordEventHandler(mLoginModule.mRoleID, recordName, IRecord.ERecordOptype.Add, i, 0, null, null);
					}
				}
            }
            else
            {
                Debug.LogError("no this record " + recordName);
            }
        }
        
        // load in a large set of data
        //LoadData();
    }


    /////////////////////////////////////////


	public void RemoveAllClickEvent()
	{
		clickEventHandler.Clear ();
	}
	public void RemoveAllUpdateClickEvent()
	{
		updateEventHandler.Clear ();
	}
	public void RegisterUpdateEvent(RecordRowView.RowViewUpdateEventHandler handler)
	{
		updateEventHandler.Add (handler);
	}

	public void RegisterPressDownEvent(RecordRowView.RowPressDownEventHandler handler)
	{
        pressDownEventHandler.Add (handler);
	}

    public void RegisterPressUpEvent(RecordRowView.RowPressUpEventHandler handler)
    {
        pressUpEventHandler.Add(handler);
    }

    public void RegisterClickEvent(RecordRowView.RowClickEventHandler handler)
    {
        clickEventHandler.Add(handler);
    }



    public void UpdateEvent(Guid self, string recordName, int nRow, RecordRowView view)
	{
		for (int i = 0; i < updateEventHandler.Count; ++i)
		{
			RecordRowView.RowViewUpdateEventHandler handler = updateEventHandler [i];
			handler(self, recordName, nRow, view);
		}
	}
	public void ClickEvent(RecordRowData data)
	{
		for (int i = 0; i < clickEventHandler.Count; ++i)
		{
			RecordRowView.RowClickEventHandler handler = clickEventHandler [i];
			handler(data);
		}
	}
    public void DownEvent(RecordRowData data)
    {
        for (int i = 0; i < pressDownEventHandler.Count; ++i)
        {
            RecordRowView.RowPressDownEventHandler handler = pressDownEventHandler[i];
            handler(data);
        }
    }
    public void UpEvent(RecordRowData data)
    {
        for (int i = 0; i < pressUpEventHandler.Count; ++i)
        {
            RecordRowView.RowPressUpEventHandler handler = pressUpEventHandler[i];
            handler(data);
        }
    }

    void OnClassPlayerEventHandler(Guid self, int nContainerID, int nGroupID, Squick.IObject.CLASS_EVENT_TYPE eType, string strClassName, string strConfigIndex)
	{
		if (mLoginModule.mRoleID == self)
		{
			xGUID = self;

			if (eType == Squick.IObject.CLASS_EVENT_TYPE.OBJECT_CREATE)
			{
				IRecord xRecord = mkernelModule.FindRecord(self, recordName);
				if (xRecord != null)
				{
					mkernelModule.RegisterRecordCallback(self, recordName, RecordEventHandler);
				}
				else
				{
					Debug.LogError("no this record " + recordName);
				}
			}
		}
	}

	void RecordEventHandler(Guid self, string strRecordName, IRecord.ERecordOptype eType, int nRow, int nCol, DataList.TData oldVar, DataList.TData newVar)
	{
		if (ColValueCondition)
		{
            if (eType == IRecord.ERecordOptype.Add)
            {
                if (ColConditionNum >= 0 && ColConditionContent.Length > 0)
                {
                    IRecord xRecord = mkernelModule.FindRecord(self, recordName);
                    if (xRecord != null)
                    {
                        if (xRecord.GetCols() > ColConditionNum)
                        {
                            switch (xRecord.GetColType(ColConditionNum))
                            {
                                case DataList.VARIANT_TYPE.VTYPE_INT:
                                    {
                                        long value = xRecord.QueryInt(nRow, ColConditionNum);
                                        if (value.ToString() != ColConditionContent)
                                        {
                                            //remove
                                            return;
                                        }
                                    }
                                    break;
                                case DataList.VARIANT_TYPE.VTYPE_STRING:
                                    {
                                        string value = xRecord.QueryString(nRow, ColConditionNum);
                                        if (value != ColConditionContent)
                                        {
                                            //remove
                                            return;
                                        }
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
            else if (eType == IRecord.ERecordOptype.Update)
            {
                if (nCol == ColConditionNum)
                {
                    if (ColConditionNum >= 0 && ColConditionContent.Length > 0)
                    {
                        IRecord xRecord = mkernelModule.FindRecord(self, recordName);
                        if (xRecord != null)
                        {
                            if (xRecord.GetCols() > ColConditionNum)
                            {
                                switch (xRecord.GetColType(ColConditionNum))
                                {
                                    case DataList.VARIANT_TYPE.VTYPE_INT:
                                        {
                                            long value = newVar.IntVal();
                                            if (value.ToString() != ColConditionContent)
                                            {
                                                //remove
                                                DestroyObject(nRow);
                                                return;
                                            }
                                        }
                                        break;
                                    case DataList.VARIANT_TYPE.VTYPE_STRING:
                                        {
                                            string value = newVar.StringVal();
                                            if (value != ColConditionContent)
                                            {
                                                //remove
                                                DestroyObject(nRow);
                                                return;
                                            }
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                }
            }

        }

		if (ColPropertyCondition)
        {
			if (ColConditionNum >= 0 && ColConditionPropertyName.Length > 0 && ColConditionPropertyValue.Length > 0)
            {
                IRecord xRecord = mkernelModule.FindRecord(self, recordName);
                if (xRecord != null)
                {
                    if (xRecord.GetCols() > ColConditionNum)
                    {
						switch (xRecord.GetColType(ColConditionNum))
                        {
                            case DataList.VARIANT_TYPE.VTYPE_INT:
                                {
									long value = xRecord.QueryInt(nRow, ColConditionNum);
                                    IElement xElement = mElementModule.GetElement(value.ToString());
                                    if (xElement == null)
                                    {
                                        Debug.LogError("Col:" + ColConditionNum.ToString() + " Value:" + value.ToString());
                                        return;
                                    }

                                    IProperty xProperty = xElement.GetPropertyManager().GetProperty(ColConditionPropertyName);
                                    if (xProperty == null)
                                    {
                                        Debug.LogError("Col:" + ColConditionNum.ToString() + " Value:" + value.ToString() + " Property:" + ColConditionPropertyName);
                                        return;
                                    }

                                    if (xProperty.GetData() != null)
                                    {
                                        switch (xProperty.GetData().GetType())
                                        {
                                            case DataList.VARIANT_TYPE.VTYPE_INT:
                                                {
                                                    if (xProperty.QueryInt().ToString() != ColConditionPropertyValue)
                                                    {
                                                        return;
                                                    }
                                                }
                                                break;
                                            case DataList.VARIANT_TYPE.VTYPE_STRING:
                                                {
                                                    if (xProperty.QueryString() != ColConditionPropertyValue)
                                                    {
                                                        return;
                                                    }
                                                }
                                                break;
                                        }
                                    }
                                }
                                break;
                            case DataList.VARIANT_TYPE.VTYPE_STRING:
                                {
                                    string value = xRecord.QueryString(nRow, ColConditionNum);
									IElement xElement = mElementModule.GetElement(value);
                                    if (xElement == null)
                                    {
                                        Debug.LogError("Col:" + ColConditionNum.ToString() + " Value:" + value);
                                        return;
                                    }

                                    IProperty xProperty = xElement.GetPropertyManager().GetProperty(ColConditionPropertyName);
                                    if (xProperty == null)
                                    {
                                        Debug.LogError("Col:" + ColConditionNum.ToString() + " Value:" + value + " Property:" + ColConditionPropertyName);
                                        return;
                                    }

                                    if (xProperty.GetData() != null)
                                    {
                                        switch (xProperty.GetData().GetType())
                                        {
                                            case DataList.VARIANT_TYPE.VTYPE_INT:
                                                {
                                                    if (xProperty.QueryInt().ToString() != ColConditionPropertyValue)
                                                    {
                                                        return;
                                                    }
                                                }
                                                break;
                                            case DataList.VARIANT_TYPE.VTYPE_STRING:
                                                {
                                                    if (xProperty.QueryString() != ColConditionPropertyValue)
                                                    {
                                                        return;
                                                    }
                                                }
                                                break;
                                        }
                                    }
                                }
                                break;
                            default:
                                break;
                        }


						            
                    }
                }

            }
        }

		switch(eType)
		{
			case IRecord.ERecordOptype.Add:
				{
					RecordRowData rowData = new RecordRowData ();
					rowData.row = nRow;
					rowData.recordName = strRecordName;
					rowData.id = self;

					_data.Add (rowData);


                    SquickRoot.Instance().StartCoroutine(CreateObject(self, rowViewItem, rowData));

				}
				break;
			case IRecord.ERecordOptype.Del:
				{
					int nIndex = -1;
					for (int i = 0; i < _data.Count; ++i)
					{
						RecordRowData rowData = _data[i];
						if (rowData.row == nRow)
						{
                            nIndex = i;
                            break;
						}
					}

                    if (nIndex >= 0)
                    {
                        _data.RemoveAt(nIndex);
                    }

                    RecordRowView[] rowViews = this.GetComponentsInChildren<RecordRowView>();
                    for (int i = 0; i < rowViews.Length; ++i)
                    {
                        RecordRowData rowData = rowViews[i].GetData();
                        if (rowData != null
                           && rowData.row == nRow)
                        {
                            GameObject.Destroy(rowViews[i].gameObject);
                            break;
                        }
                    }
                }
				break;
			case IRecord.ERecordOptype.Update:
				{
                    if (ColValueCondition)
                    {
                        if (nCol == ColConditionNum)
                        {
                            int nIndex = -1;
                            for (int i = 0; i < _data.Count; ++i)
                            {
                                RecordRowData rowData = _data[i];
                                if (rowData.row == nRow)
                                {
                                    nIndex = i;
                                    break;
                                }
                            }

                            if (nIndex < 0)
                            {
                                //add a new one
                                RecordRowData rowData = new RecordRowData();
                                rowData.row = nRow;
                                rowData.recordName = strRecordName;
                                rowData.id = self;

                                _data.Add(rowData);


                                SquickRoot.Instance().StartCoroutine(CreateObject(self, rowViewItem, rowData));
                            }
                            
                        }
                    }

          
                    {
                        RecordRowView[] rowViews = this.GetComponentsInChildren<RecordRowView>();
                        for (int i = 0; i < rowViews.Length; ++i)
                        {
                            RecordRowData rowData = rowViews[i].GetData();
                            if (rowData != null
                               && rowData.row == nRow)
                            {
                                rowViews[i].SetData(self, recordName, this, rowData);
                                break;
                            }
                        }
                    }
				}
				break;
			case IRecord.ERecordOptype.Create:
				break;
			case IRecord.ERecordOptype.Cleared:
                _data.Clear();
                break;
			default:
				break;
		}

	}

    private IEnumerator CreateObject(Guid self, RecordRowView go, RecordRowData rowData)
    {
        yield return 0;

        RecordRowView rowObject = GameObject.Instantiate(go);
        if (rowObject)
        {
            rowObject.transform.SetParent(this.transform);
            rowObject.transform.localScale = Vector3.one;
            rowObject.SetData(self, recordName, this, rowData);
        }
    }

    private void DestroyObject(int row)
    {
        int nIndex = -1;
        for (int i = 0; i < _data.Count; ++i)
        {
            RecordRowData rowData = _data[i];
            if (rowData.row == row)
            {
                nIndex = i;
                break;
            }
        }

        if (nIndex >= 0)
        {
            _data.RemoveAt(nIndex);
        }

        RecordRowView[] rowViews = this.GetComponentsInChildren<RecordRowView>();
        for (int i = 0; i < rowViews.Length; ++i)
        {
            RecordRowData rowData = rowViews[i].GetData();
            if (rowData != null
               && rowData.row == row)
            {
                GameObject.Destroy(rowViews[i].gameObject);
                break;
            }
        }
    }
}
