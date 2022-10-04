using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Collections;
using System.IO;
using UnityEngine;
using System.Globalization;

namespace Squick
{
    public class ElementModule : IElementModule
    {

        public ElementModule(IPluginManager pluginManager)
        {
            mPluginManager = pluginManager;
            mhtObject = new Dictionary<string, IElement>();
        }

        public override void Awake() 
		{  
		}

        public override void Init()
        {
            mxLogicClassModule = mPluginManager.FindModule<IClassModule>();
            mstrRootPath = mxLogicClassModule.GetDataPath();
        }

        public override void AfterInit()
        {
            Load();
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

        public override bool Load()
        {
            ClearInstanceElement();

            Dictionary<string, ISClass> xTable = mxLogicClassModule.GetElementList();
            foreach (KeyValuePair<string, ISClass> kv in xTable)
            {
                LoadInstanceElement(kv.Value);
            }

            return false;
        }

        public override bool Clear()
        {
            return false;
        }

        public override bool ExistElement(string strConfigName)
        {
            if (mhtObject.ContainsKey(strConfigName))
            {
                return true;
            }

            return false;
        }

        public override Int64 QueryPropertyInt(string strConfigName, string strPropertyName)
        {
            IElement xElement = GetElement(strConfigName);
            if (null != xElement)
            {
                return xElement.QueryInt(strPropertyName);
            }

            UnityEngine.Debug.LogError("ERROR: " + strConfigName + " HAS NO " + strPropertyName);

            return 0;
        }

        public override double QueryPropertyFloat(string strConfigName, string strPropertyName)
        {
            IElement xElement = GetElement(strConfigName);
            if (null != xElement)
            {
                return xElement.QueryFloat(strPropertyName);
            }

            UnityEngine.Debug.LogError("ERROR: " + strConfigName + " HAS NO " + strPropertyName);

            return 0.0;
        }

        public override string QueryPropertyString(string strConfigName, string strPropertyName)
        {
            IElement xElement = GetElement(strConfigName);
            if (null != xElement)
            {
                return xElement.QueryString(strPropertyName);
            }

            UnityEngine.Debug.LogError("ERROR: " + strConfigName + " HAS NO " + strPropertyName);

            return DataList.NULL_STRING;
        }

        public override bool AddElement(string strName, IElement xElement)
        {
            if (!mhtObject.ContainsKey(strName))
            {
                mhtObject.Add(strName, xElement);

                return true;
            }

            return false;
        }

        public override IElement GetElement(string strConfigName)
        {
            if (mhtObject.ContainsKey(strConfigName))
            {
                return (IElement)mhtObject[strConfigName];
            }

            return null;
        }

        private void ClearInstanceElement()
        {
            mhtObject.Clear();
        }

        private void LoadInstanceElement(ISClass xLogicClass)
        {
            string strLogicPath = mstrRootPath;
            strLogicPath += xLogicClass.GetInstance();

            strLogicPath = strLogicPath.Replace(".xml", "");

            TextAsset textAsset = (TextAsset)Resources.Load(strLogicPath);

            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(textAsset.text);
            XmlNode xRoot = xmldoc.SelectSingleNode("XML");

            XmlNodeList xNodeList = xRoot.SelectNodes("Object");
            for (int i = 0; i < xNodeList.Count; ++i)
            {
                //NFCLog.Instance.Log("Class:" + xLogicClass.GetName());

                XmlNode xNodeClass = xNodeList.Item(i);
                XmlAttribute strID = xNodeClass.Attributes["Id"];

                //NFCLog.Instance.Log("ClassID:" + strID.Value);

                IElement xElement = GetElement(strID.Value);
                if (null == xElement)
                {
                    xElement = new Element();
                    AddElement(strID.Value, xElement);
                    xLogicClass.AddConfigName(strID.Value);

                    XmlAttributeCollection xCollection = xNodeClass.Attributes;
                    for (int j = 0; j < xCollection.Count; ++j)
                    {
                        XmlAttribute xAttribute = xCollection[j];
                        IProperty xProperty = xLogicClass.GetPropertyManager().GetProperty(xAttribute.Name);
                        if (null != xProperty)
                        {
                            DataList.VARIANT_TYPE eType = xProperty.GetType();
                            switch (eType)
                            {
                                case DataList.VARIANT_TYPE.VTYPE_INT:
                                    {
                                        try
                                        {
                                            DataList.TData xValue = new DataList.TData(DataList.VARIANT_TYPE.VTYPE_INT);
                                            xValue.Set(int.Parse(xAttribute.Value));
                                            IProperty property = xElement.GetPropertyManager().AddProperty(xAttribute.Name, xValue);
                                            property.SetUpload(xProperty.GetUpload());
                                        }
                                        catch
                                        {
                                            Debug.LogError("ID:" + strID.Value + " Property Name:" + xAttribute.Name + " Value:" + xAttribute.Value);
                                        }
                                    }
                                    break;
                                case DataList.VARIANT_TYPE.VTYPE_FLOAT:
                                    {
                                        try
                                        {
                                            DataList.TData xValue = new DataList.TData(DataList.VARIANT_TYPE.VTYPE_FLOAT);
                                            xValue.Set(float.Parse(xAttribute.Value, CultureInfo.InvariantCulture.NumberFormat));
                                            IProperty property = xElement.GetPropertyManager().AddProperty(xAttribute.Name, xValue);
                                            property.SetUpload(xProperty.GetUpload());
                                        }
                                        catch
                                        {

                                            Debug.LogError("ID:" + strID.Value + " Property Name:" + xAttribute.Name + " Value:" + xAttribute.Value);
                                        }
                                    }
                                    break;
                                case DataList.VARIANT_TYPE.VTYPE_STRING:
                                    {
                                        DataList.TData xValue = new DataList.TData(DataList.VARIANT_TYPE.VTYPE_STRING);
                                        xValue.Set(xAttribute.Value);
                                        IProperty property = xElement.GetPropertyManager().AddProperty(xAttribute.Name, xValue);
                                        property.SetUpload(xProperty.GetUpload());
                                    }
                                    break;
                                case DataList.VARIANT_TYPE.VTYPE_OBJECT:
                                    {
                                        try
                                        {
                                            DataList.TData xValue = new DataList.TData(DataList.VARIANT_TYPE.VTYPE_OBJECT);
                                            xValue.Set(new Guid(0, int.Parse(xAttribute.Value)));
                                            IProperty property = xElement.GetPropertyManager().AddProperty(xAttribute.Name, xValue);
                                            property.SetUpload(xProperty.GetUpload());
                                        }
                                        catch
                                        {

                                            Debug.LogError("ID:" + strID.Value + " Property Name:" + xAttribute.Name + " Value:" + xAttribute.Value);
                                        }
                                    }
                                    break;
                                case DataList.VARIANT_TYPE.VTYPE_VECTOR2:
                                    {
                                        try
                                        {
                                            DataList.TData xValue = new DataList.TData(DataList.VARIANT_TYPE.VTYPE_VECTOR2);
                                            //xValue.Set(new Guid(0, int.Parse(xAttribute.Value)));
                                            IProperty property = xElement.GetPropertyManager().AddProperty(xAttribute.Name, xValue);
                                            property.SetUpload(xProperty.GetUpload());
                                        }
                                        catch
                                        {

                                            Debug.LogError("ID:" + strID.Value + " Property Name:" + xAttribute.Name + " Value:" + xAttribute.Value);
                                        }
                                    }
                                    break;
                                case DataList.VARIANT_TYPE.VTYPE_VECTOR3:
                                    {
                                        try
                                        {
                                            DataList.TData xValue = new DataList.TData(DataList.VARIANT_TYPE.VTYPE_VECTOR3);
                                            //xValue.Set(new Guid(0, int.Parse(xAttribute.Value)));
                                            IProperty property = xElement.GetPropertyManager().AddProperty(xAttribute.Name, xValue);
                                            property.SetUpload(xProperty.GetUpload());

                                        }
                                        catch
                                        {
                                            Debug.LogError("ID:" + strID.Value + " Property Name:" + xAttribute.Name + " Value:" + xAttribute.Value);
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

        /////////////////////////////////////////
        private IClassModule mxLogicClassModule;
        /////////////////////////////////////////
        private Dictionary<string, IElement> mhtObject;
        private string mstrRootPath;
    }
}