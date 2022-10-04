
using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace Squick
{
	public class KernelModule : IKernelModule
    {
        public KernelModule(IPluginManager pluginManager)
        {
            mPluginManager = pluginManager;
            mhtObject = new Dictionary<Guid, IObject>();
            mhtClassHandleDel = new Dictionary<string, ClassHandleDel>();
        }


        private Dictionary<Guid, IObject> mhtObject;
        private Dictionary<string, ClassHandleDel> mhtClassHandleDel;
        private IElementModule mxElementModule;
        private IClassModule mxLogicClassModule;

        private Random mRandom = new Random();


        ~KernelModule()
		{
			mhtObject = null;
        }

        public override void Awake()
        {
        }

        public override void Init()
        {
            mxElementModule = mPluginManager.FindModule<IElementModule>();
            mxLogicClassModule = mPluginManager.FindModule<IClassModule>();
        }

        public override void AfterInit()
        {
            CreateObject(new Guid(0, 0), 0, 0, SquickProtocol.Group.ThisName, "", new DataList());
        }

        public override void BeforeShut()
        {
        }

		public override void Shut()
		{
		}

        public override void Execute()
        {
        }

        public override void RegisterGroupPropertyCallback(string strPropertyName, IProperty.PropertyEventHandler handler)
        {
            IObject xGameObject = GetObject(new Guid(0, 0));
            if (null != xGameObject)
            {
                xGameObject.GetPropertyManager().RegisterCallback(strPropertyName, handler);
            }
        }

        public override void RegisterGroupRecordCallback(string strRecordName, IRecord.RecordEventHandler handler)
        {
            IObject xGameObject = GetObject(new Guid(0, 0));
            if (null != xGameObject)
            {
                xGameObject.GetRecordManager().RegisterCallback(strRecordName, handler);
            }
        }

		public override void RegisterPropertyCallback(Guid self, string strPropertyName, IProperty.PropertyEventHandler handler)
		{
			IObject xGameObject = GetObject(self);
			if (null != xGameObject)
			{
				xGameObject.GetPropertyManager().RegisterCallback(strPropertyName, handler);
			}
		}

		public override void RegisterRecordCallback(Guid self, string strRecordName, IRecord.RecordEventHandler handler)
		{
			IObject xGameObject = GetObject(self);
			if (null != xGameObject)
			{
				xGameObject.GetRecordManager().RegisterCallback(strRecordName, handler);
			}
		}

		public override void RegisterClassCallBack(string strClassName, IObject.ClassEventHandler handler)
		{
			if(mhtClassHandleDel.ContainsKey(strClassName))
			{
				ClassHandleDel xHandleDel = (ClassHandleDel)mhtClassHandleDel[strClassName];
				xHandleDel.AddDel(handler);
				
			}
			else
			{
				ClassHandleDel xHandleDel = new ClassHandleDel();
				xHandleDel.AddDel(handler);
				mhtClassHandleDel[strClassName] = xHandleDel;
			}
		}

		public override void RegisterEventCallBack(Guid self, int nEventID, ISEvent.EventHandler handler)
		{
			IObject xGameObject = GetObject(self);
			if (null != xGameObject)
			{
				//xGameObject.GetEventManager().RegisterCallback(nEventID, handler, valueList);
			}
		}

		public override IObject GetObject(Guid ident)
		{
            if (null != ident && mhtObject.ContainsKey(ident))
			{
				return (IObject)mhtObject[ident];
			}

			return null;
		}

		public override IObject CreateObject(Guid self, int nContainerID, int nGroupID, string strClassName, string strConfigIndex, DataList arg)
		{
			if (!mhtObject.ContainsKey(self))
			{
				IObject xNewObject = new Object(self, nContainerID, nGroupID, strClassName, strConfigIndex);
				mhtObject.Add(self, xNewObject);

                DataList.TData varConfigID = new DataList.TData(DataList.VARIANT_TYPE.VTYPE_STRING);
                varConfigID.Set(strConfigIndex);
                xNewObject.GetPropertyManager().AddProperty("ConfigID", varConfigID);

                DataList.TData varConfigClass = new DataList.TData(DataList.VARIANT_TYPE.VTYPE_STRING);
                varConfigClass.Set(strClassName);
                xNewObject.GetPropertyManager().AddProperty("ClassName", varConfigClass);

                if (arg.Count() % 2 == 0)
                {
                    for (int i = 0; i < arg.Count() - 1; i += 2)
                    {
                        string strPropertyName = arg.StringVal(i);
                        DataList.VARIANT_TYPE eType = arg.GetType(i + 1);
                        switch (eType)
                        {
                            case DataList.VARIANT_TYPE.VTYPE_INT:
                                {
                                    DataList.TData var = new DataList.TData(DataList.VARIANT_TYPE.VTYPE_INT);
                                    var.Set(arg.IntVal(i+1));
                                    xNewObject.GetPropertyManager().AddProperty(strPropertyName, var);
                                }
                                break;
                            case DataList.VARIANT_TYPE.VTYPE_FLOAT:
                                {
                                    DataList.TData var = new DataList.TData(DataList.VARIANT_TYPE.VTYPE_FLOAT);
                                    var.Set(arg.FloatVal(i + 1));
                                    xNewObject.GetPropertyManager().AddProperty(strPropertyName, var);
                                }
                                break;
                            case DataList.VARIANT_TYPE.VTYPE_STRING:
                                {
                                    DataList.TData var = new DataList.TData(DataList.VARIANT_TYPE.VTYPE_STRING);
                                    var.Set(arg.StringVal(i + 1));
                                    xNewObject.GetPropertyManager().AddProperty(strPropertyName, var);
                                }
                                break;
                            case DataList.VARIANT_TYPE.VTYPE_OBJECT:
                                {
                                    DataList.TData var = new DataList.TData(DataList.VARIANT_TYPE.VTYPE_OBJECT);
                                    var.Set(arg.ObjectVal(i + 1));
                                    xNewObject.GetPropertyManager().AddProperty(strPropertyName, var);
                                }
                                break;
                            case DataList.VARIANT_TYPE.VTYPE_VECTOR2:
                                {
                                    DataList.TData var = new DataList.TData(DataList.VARIANT_TYPE.VTYPE_VECTOR2);
                                    var.Set(arg.Vector2Val(i + 1));
                                    xNewObject.GetPropertyManager().AddProperty(strPropertyName, var);
                                }
                                break;
                            case DataList.VARIANT_TYPE.VTYPE_VECTOR3:
                                {
                                    DataList.TData var = new DataList.TData(DataList.VARIANT_TYPE.VTYPE_VECTOR3);
                                    var.Set(arg.Vector3Val(i + 1));
                                    xNewObject.GetPropertyManager().AddProperty(strPropertyName, var);
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }

                InitProperty(self, strClassName);
                InitRecord(self, strClassName);

                if (mhtClassHandleDel.ContainsKey(strClassName))
                {
                    ClassHandleDel xHandleDel = (ClassHandleDel)mhtClassHandleDel[strClassName];
                    if (null != xHandleDel && null != xHandleDel.GetHandler())
                    {
                        IObject.ClassEventHandler xHandlerList = xHandleDel.GetHandler();
                        xHandlerList(self, nContainerID, nGroupID, IObject.CLASS_EVENT_TYPE.OBJECT_CREATE, strClassName, strConfigIndex);
                        xHandlerList(self, nContainerID, nGroupID, IObject.CLASS_EVENT_TYPE.OBJECT_LOADDATA, strClassName, strConfigIndex);
                        xHandlerList(self, nContainerID, nGroupID, IObject.CLASS_EVENT_TYPE.OBJECT_CREATE_FINISH, strClassName, strConfigIndex);
                    }

                }

                //NFCLog.Instance.Log(NFCLog.LOG_LEVEL.DEBUG, "Create object: " + self.ToString() + " ClassName: " + strClassName + " SceneID: " + nContainerID + " GroupID: " + nGroupID);
                return xNewObject;
			}

			return null;
		}


		public override bool DestroyObject(Guid self)
		{
			if (mhtObject.ContainsKey(self))
			{
				IObject xGameObject = (IObject)mhtObject[self];

				string strClassName = xGameObject.ClassName();
                if (mhtClassHandleDel.ContainsKey(strClassName))
                {
                    ClassHandleDel xHandleDel = (ClassHandleDel)mhtClassHandleDel[strClassName];
                    if (null != xHandleDel && null != xHandleDel.GetHandler())
                    {
                        IObject.ClassEventHandler xHandlerList = xHandleDel.GetHandler();
                        xHandlerList(self, xGameObject.ContainerID(), xGameObject.GroupID(), IObject.CLASS_EVENT_TYPE.OBJECT_DESTROY, xGameObject.ClassName(), xGameObject.ConfigIndex());
                    }

                    mhtObject.Remove(self);
                }


                //NFCLog.Instance.Log(NFCLog.LOG_LEVEL.DEBUG, "Destroy object: " + self.ToString() + " ClassName: " + strClassName + " SceneID: " + xGameObject.ContainerID() + " GroupID: " + xGameObject.GroupID());

                return true;
			}

			return false;
		}

		public override IProperty FindProperty(Guid self, string strPropertyName)
		{
			if (mhtObject.ContainsKey(self))
			{
				IObject xGameObject = (IObject)mhtObject[self];
				return xGameObject.FindProperty(strPropertyName);
			}

			return null;
		}

        public override bool SetPropertyInt(Guid self, string strPropertyName, Int64 nValue)
		{
			if (mhtObject.ContainsKey(self))
			{
				IObject xGameObject = (IObject)mhtObject[self];
				return xGameObject.SetPropertyInt(strPropertyName, nValue);
			}

			return false;
		}

		public override bool SetPropertyFloat(Guid self, string strPropertyName, double fValue)
		{
			if (mhtObject.ContainsKey(self))
			{
				IObject xGameObject = (IObject)mhtObject[self];
				return xGameObject.SetPropertyFloat(strPropertyName, fValue);
			}

			return false;
		}

		public override bool SetPropertyString(Guid self, string strPropertyName, string strValue)
		{
			if (mhtObject.ContainsKey(self))
			{
				IObject xGameObject = (IObject)mhtObject[self];
				return xGameObject.SetPropertyString(strPropertyName, strValue);
			}

			return false;
		}

		public override bool SetPropertyObject(Guid self, string strPropertyName, Guid objectValue)
		{
			if (mhtObject.ContainsKey(self))
			{
				IObject xGameObject = (IObject)mhtObject[self];
				return xGameObject.SetPropertyObject(strPropertyName, objectValue);
			}

			return false;
		}

        public override bool SetPropertyVector2(Guid self, string strPropertyName, SVector2 objectValue)
        {
            if (mhtObject.ContainsKey(self))
            {
                IObject xGameObject = (IObject)mhtObject[self];
                return xGameObject.SetPropertyVector2(strPropertyName, objectValue);
            }

            return false;
        }

        public override bool SetPropertyVector3(Guid self, string strPropertyName, SVector3 objectValue)
        {
            if (mhtObject.ContainsKey(self))
            {
                IObject xGameObject = (IObject)mhtObject[self];
                return xGameObject.SetPropertyVector3(strPropertyName, objectValue);
            }

            return false;
        }

        public override Int64 QueryPropertyInt(Guid self, string strPropertyName)
		{
			if (mhtObject.ContainsKey(self))
			{
				IObject xGameObject = (IObject)mhtObject[self];
				return xGameObject.QueryPropertyInt(strPropertyName);
			}

			return 0;
		}

		public override double QueryPropertyFloat(Guid self, string strPropertyName)
		{
			if (mhtObject.ContainsKey(self))
			{
				IObject xGameObject = (IObject)mhtObject[self];
				return xGameObject.QueryPropertyFloat(strPropertyName);
			}

			return 0.0;
		}

		public override string QueryPropertyString(Guid self, string strPropertyName)
		{
			if (mhtObject.ContainsKey(self))
			{
				IObject xGameObject = (IObject)mhtObject[self];
				return xGameObject.QueryPropertyString(strPropertyName);
			}

			return "";
		}

		public override Guid QueryPropertyObject(Guid self, string strPropertyName)
		{
			if (mhtObject.ContainsKey(self))
			{
				IObject xGameObject = (IObject)mhtObject[self];
				return xGameObject.QueryPropertyObject(strPropertyName);
			}

			return new Guid();
		}

        public override SVector2 QueryPropertyVector2(Guid self, string strPropertyName)
        {
            if (mhtObject.ContainsKey(self))
            {
                IObject xGameObject = (IObject)mhtObject[self];
                return xGameObject.QueryPropertyVector2(strPropertyName);
            }

            return new SVector2();
        }

        public override SVector3 QueryPropertyVector3(Guid self, string strPropertyName)
        {
            if (mhtObject.ContainsKey(self))
            {
                IObject xGameObject = (IObject)mhtObject[self];
                return xGameObject.QueryPropertyVector3(strPropertyName);
            }

            return new SVector3();
        }


        public override IRecord FindRecord(Guid self, string strRecordName)
		{
			if (mhtObject.ContainsKey(self))
			{
				IObject xGameObject = (IObject)mhtObject[self];
				return xGameObject.FindRecord(strRecordName);
			}

			return null;
		}


        public override bool SetRecordInt(Guid self, string strRecordName, int nRow, int nCol, Int64 nValue)
		{
			if (mhtObject.ContainsKey(self))
			{
				IObject xGameObject = (IObject)mhtObject[self];
				return xGameObject.SetRecordInt(strRecordName, nRow, nCol, nValue);
			}

			return false;
		}

		public override bool SetRecordFloat(Guid self, string strRecordName, int nRow, int nCol, double fValue)
		{
			if (mhtObject.ContainsKey(self))
			{
				IObject xGameObject = (IObject)mhtObject[self];
				return xGameObject.SetRecordFloat(strRecordName, nRow, nCol, fValue);
			}

			return false;
		}


		public override bool SetRecordString(Guid self, string strRecordName, int nRow, int nCol, string strValue)
		{
			if (mhtObject.ContainsKey(self))
			{
				IObject xGameObject = (IObject)mhtObject[self];
				return xGameObject.SetRecordString(strRecordName, nRow, nCol, strValue);
			}

			return false;
		}

		public override bool SetRecordObject(Guid self, string strRecordName, int nRow, int nCol, Guid objectValue)
		{
			if (mhtObject.ContainsKey(self))
			{
				IObject xGameObject = (IObject)mhtObject[self];
				return xGameObject.SetRecordObject(strRecordName, nRow, nCol, objectValue);
			}

			return false;
		}

        public override bool SetRecordVector2(Guid self, string strRecordName, int nRow, int nCol, SVector2 objectValue)
        {
            if (mhtObject.ContainsKey(self))
            {
                IObject xGameObject = (IObject)mhtObject[self];
                return xGameObject.SetRecordVector2(strRecordName, nRow, nCol, objectValue);
            }

            return false;
        }

        public override bool SetRecordVector3(Guid self, string strRecordName, int nRow, int nCol, SVector3 objectValue)
        {
            if (mhtObject.ContainsKey(self))
            {
                IObject xGameObject = (IObject)mhtObject[self];
                return xGameObject.SetRecordVector3(strRecordName, nRow, nCol, objectValue);
            }

            return false;
        }


        public override Int64 QueryRecordInt(Guid self, string strRecordName, int nRow, int nCol)
		{
			if (mhtObject.ContainsKey(self))
			{
				IObject xGameObject = (IObject)mhtObject[self];
				return xGameObject.QueryRecordInt(strRecordName, nRow, nCol);
			}

			return 0;
		}

		public override double QueryRecordFloat(Guid self, string strRecordName, int nRow, int nCol)
		{
			if (mhtObject.ContainsKey(self))
			{
				IObject xGameObject = (IObject)mhtObject[self];
				return xGameObject.QueryRecordFloat(strRecordName, nRow, nCol);
			}

			return 0.0;
		}

		public override string QueryRecordString(Guid self, string strRecordName, int nRow, int nCol)
		{
			if (mhtObject.ContainsKey(self))
			{
				IObject xGameObject = (IObject)mhtObject[self];
				return xGameObject.QueryRecordString(strRecordName, nRow, nCol);
			}

			return "";
		}

		public override Guid QueryRecordObject(Guid self, string strRecordName, int nRow, int nCol)
		{
			if (mhtObject.ContainsKey(self))
			{
				IObject xGameObject = (IObject)mhtObject[self];
				return xGameObject.QueryRecordObject(strRecordName, nRow, nCol);
			}

			return new Guid();
		}

        public override SVector2 QueryRecordVector2(Guid self, string strRecordName, int nRow, int nCol)
        {
            if (mhtObject.ContainsKey(self))
            {
                IObject xGameObject = (IObject)mhtObject[self];
                return xGameObject.QueryRecordVector2(strRecordName, nRow, nCol);
            }

            return new SVector2();
        }

        public override SVector3 QueryRecordVector3(Guid self, string strRecordName, int nRow, int nCol)
        {
            if (mhtObject.ContainsKey(self))
            {
                IObject xGameObject = (IObject)mhtObject[self];
                return xGameObject.QueryRecordVector3(strRecordName, nRow, nCol);
            }

            return new SVector3();
        }

        public override DataList GetObjectList()
		{
			DataList varData = new DataList();
            foreach (KeyValuePair<Guid, IObject> kv in mhtObject)
            {
                varData.AddObject(kv.Key);				
            }

			return varData;
		}

        public override int FindRecordRow(Guid self, string strRecordName, int nCol, int nValue)
        {
            if (mhtObject.ContainsKey(self))
            {
                IObject xGameObject = (IObject)mhtObject[self];
                IRecord xRecord = xGameObject.GetRecordManager().GetRecord(strRecordName);
                if (null != xRecord)
                {
                    return xRecord.FindInt(nCol, nValue);
                }
            }
            return -1;
        }

        public override int FindRecordRow(Guid self, string strRecordName, int nCol, double value)
        {
            if (mhtObject.ContainsKey(self))
            {
                IObject xGameObject = (IObject)mhtObject[self];
                IRecord xRecord = xGameObject.GetRecordManager().GetRecord(strRecordName);
                if (null != xRecord)
                {
                    return xRecord.FindFloat(nCol, value);
                }
            }
            return -1;

        }

        public override int FindRecordRow(Guid self, string strRecordName, int nCol, string value)
        {
            if (mhtObject.ContainsKey(self))
            {
                IObject xGameObject = (IObject)mhtObject[self];
                IRecord xRecord = xGameObject.GetRecordManager().GetRecord(strRecordName);
                if (null != xRecord)
                {
                    return xRecord.FindString(nCol, value);
                }
            }
            return -1;

        }

        public override int FindRecordRow(Guid self, string strRecordName, int nCol, Guid value)
        {
            if (mhtObject.ContainsKey(self))
            {
                IObject xGameObject = (IObject)mhtObject[self];
                IRecord xRecord = xGameObject.GetRecordManager().GetRecord(strRecordName);
                if (null != xRecord)
                {
                    return xRecord.FindObject(nCol, value);
                }
            }
            return -1;

        }

        public override int FindRecordRow(Guid self, string strRecordName, int nCol, SVector2 value)
        {
            if (mhtObject.ContainsKey(self))
            {
                IObject xGameObject = (IObject)mhtObject[self];
                IRecord xRecord = xGameObject.GetRecordManager().GetRecord(strRecordName);
                if (null != xRecord)
                {
                    return xRecord.FindVector2(nCol, value);
                }
            }
            return -1;

        }

        public override int FindRecordRow(Guid self, string strRecordName, int nCol, SVector3 value)
        {
            if (mhtObject.ContainsKey(self))
            {
                IObject xGameObject = (IObject)mhtObject[self];
                IRecord xRecord = xGameObject.GetRecordManager().GetRecord(strRecordName);
                if (null != xRecord)
                {
                    return xRecord.FindVector3(nCol, value);
                }
            }
            return -1;

        }


        public override int FindRecordRow(Guid self, string strRecordName, int nCol, int nValue, ref DataList xDatalist)
        {
            if (mhtObject.ContainsKey(self))
            {
                IObject xGameObject = (IObject)mhtObject[self];
                IRecord xRecord = xGameObject.GetRecordManager().GetRecord(strRecordName);
                if (null != xRecord)
                {
                    return xRecord.FindInt(nCol, nValue, ref xDatalist);
                }
            }

            return -1;
        }

        public override int FindRecordRow(Guid self, string strRecordName, int nCol, double fValue, ref DataList xDatalist)
        {
            if (mhtObject.ContainsKey(self))
            {
                IObject xGameObject = (IObject)mhtObject[self];
                IRecord xRecord = xGameObject.GetRecordManager().GetRecord(strRecordName);
                if (null != xRecord)
                {
                    return xRecord.FindFloat(nCol, fValue, ref xDatalist);
                }
            }

            return -1;
        }

        public override int FindRecordRow(Guid self, string strRecordName, int nCol, string strValue, ref DataList xDatalist)
        {
            if (mhtObject.ContainsKey(self))
            {
                IObject xGameObject = (IObject)mhtObject[self];
                IRecord xRecord = xGameObject.GetRecordManager().GetRecord(strRecordName);
                if (null != xRecord)
                {
                    return xRecord.FindString(nCol, strValue, ref xDatalist);
                }
            }

            return -1;
        }

        public override int FindRecordRow(Guid self, string strRecordName, int nCol, Guid nValue, ref DataList xDatalist)
        {
            if (mhtObject.ContainsKey(self))
            {
                IObject xGameObject = (IObject)mhtObject[self];
                IRecord xRecord = xGameObject.GetRecordManager().GetRecord(strRecordName);
                if (null != xRecord)
                {
                    return xRecord.FindObject(nCol, nValue, ref xDatalist);
                }
            }

            return -1;
        }

        public override int FindRecordRow(Guid self, string strRecordName, int nCol, SVector2 nValue, ref DataList xDatalist)
        {
            if (mhtObject.ContainsKey(self))
            {
                IObject xGameObject = (IObject)mhtObject[self];
                IRecord xRecord = xGameObject.GetRecordManager().GetRecord(strRecordName);
                if (null != xRecord)
                {
                    return xRecord.FindVector2(nCol, nValue, ref xDatalist);
                }
            }

            return -1;
        }

        public override int FindRecordRow(Guid self, string strRecordName, int nCol, SVector3 nValue, ref DataList xDatalist)
        {
            if (mhtObject.ContainsKey(self))
            {
                IObject xGameObject = (IObject)mhtObject[self];
                IRecord xRecord = xGameObject.GetRecordManager().GetRecord(strRecordName);
                if (null != xRecord)
                {
                    return xRecord.FindVector3(nCol, nValue, ref xDatalist);
                }
            }

            return -1;
        }

        public override int Random(int start, int end)
		{
            return mRandom.Next(start, end);
		}

        void InitProperty(Guid self, string strClassName)
        {
            ISClass xLogicClass = mxLogicClassModule.GetElement(strClassName);
            DataList xDataList = xLogicClass.GetPropertyManager().GetPropertyList();
            for (int i = 0; i < xDataList.Count(); ++i )
            {
                string strPropertyName = xDataList.StringVal(i);
                IProperty xProperty = xLogicClass.GetPropertyManager().GetProperty(strPropertyName);
  
                IObject xObject = GetObject(self);
                IPropertyManager xPropertyManager = xObject.GetPropertyManager();

                IProperty property = xPropertyManager.AddProperty(strPropertyName, xProperty.GetData());
                //if property==null ,means this property alreay exist in manager
                if (property != null)
                {
                    property.SetUpload(xProperty.GetUpload());
                }
            }
        }

        void InitRecord(Guid self, string strClassName)
        {
			ISClass xLogicClass = mxLogicClassModule.GetElement(strClassName);
            DataList xDataList = xLogicClass.GetRecordManager().GetRecordList();
            for (int i = 0; i < xDataList.Count(); ++i)
            {
                string strRecordyName = xDataList.StringVal(i);
                IRecord xRecord = xLogicClass.GetRecordManager().GetRecord(strRecordyName);

                IObject xObject = GetObject(self);
                IRecordManager xRecordManager = xObject.GetRecordManager();

                IRecord record = xRecordManager.AddRecord(strRecordyName, xRecord.GetRows(), xRecord.GetColsData(), xRecord.GetTagData());
                if (record != null)
                {
                    record.SetUpload(xRecord.GetUpload());
                }
            }
        }
      
        class ClassHandleDel
		{
			public ClassHandleDel()
			{
                mhtHandleDelList = new Dictionary<IObject.ClassEventHandler, string>();
			}
			
			public void AddDel(IObject.ClassEventHandler handler)
			{
				if (!mhtHandleDelList.ContainsKey(handler))
				{
					mhtHandleDelList.Add(handler, handler.ToString());
					mHandleDel += handler;
				}
			}
			
			public IObject.ClassEventHandler GetHandler()
			{
				return mHandleDel;
			}
         
			
			private IObject.ClassEventHandler mHandleDel;
            private Dictionary<IObject.ClassEventHandler, string> mhtHandleDelList;


        }
	}
}