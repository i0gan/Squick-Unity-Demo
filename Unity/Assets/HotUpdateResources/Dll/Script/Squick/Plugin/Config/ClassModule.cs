using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using UnityEngine;

namespace Squick
{
    public class SClassModule : IClassModule
    {

		public SClassModule(IPluginManager pluginManager)
        {
            mPluginManager = pluginManager;
        }

		public override void Awake() { }


        public override void Init()
        {
            Load();
        }

		public override void AfterInit() { }
        public override void Execute() { }
        public override void BeforeShut() { }
        public override void Shut() { }

        public override void SetDataPath(string strDataPath)
        {
            mstrPath = strDataPath;
        }

        public override string GetDataPath()
        {
            return mstrPath;
        }

        private bool Load()
        {
            ClearLogicClass();
            

            string strLogicPath = mstrPath + "config/struct/logic_class";
			TextAsset textAsset = (TextAsset) Resources.Load(strLogicPath); 

			XmlDocument xmldoc = new XmlDocument ();
			xmldoc.LoadXml ( textAsset.text );
            XmlNode root = xmldoc.SelectSingleNode("XML");

            LoadLogicClass(root);
            LoadLogicClassProperty();
            LoadLogicClassRecord();
            LoadLogicClassInclude();


            ProcessLogicClassIncludeFiles();

            return false;
        }

        public override bool ExistElement(string strClassName)
        {
            if (mhtObject.ContainsKey(strClassName))
            {
                return true;
            }

            return false;
        }

        public override bool AddElement(string strName)
        {
            if (!mhtObject.ContainsKey(strName))
            {
                ISClass xElement = new SClass();
                xElement.SetName(strName);
                xElement.SetEncrypt(false);

                mhtObject.Add(strName, xElement);

                return true;
            }

            return false;
        }

        public override ISClass GetElement(string strClassName)
        {
            if (mhtObject.ContainsKey(strClassName))
            {
                return (ISClass)mhtObject[strClassName];
            }

            return null;
        }
        /////////////////////////////////////////
        private void LoadLogicClass(XmlNode xNode)
        {
            XmlNodeList xNodeList = xNode.SelectNodes("Class");
            for (int i = 0; i < xNodeList.Count; ++i)
            {
                XmlNode xNodeClass = xNodeList.Item(i);
                XmlAttribute strID = xNodeClass.Attributes["Id"];
                XmlAttribute strPath = xNodeClass.Attributes["Path"];
                XmlAttribute strInstancePath = xNodeClass.Attributes["InstancePath"];

                ISClass xLogicClass = new SClass();
                mhtObject.Add(strID.Value, xLogicClass);

                xLogicClass.SetName(strID.Value);
                xLogicClass.SetPath(strPath.Value);
                xLogicClass.SetInstance(strInstancePath.Value);
                xLogicClass.SetEncrypt(false);

                XmlNodeList xNodeSubClassList = xNodeClass.SelectNodes("Class");
                if (xNodeSubClassList.Count > 0)
                {
                    LoadLogicClass(xNodeClass);
                }
            }
        }
        
        private void ClearLogicClass()
        {
            mhtObject.Clear();
        }

        private void LoadLogicClassProperty()
        {
            Dictionary<string, ISClass> xTable = GetElementList();
            foreach (KeyValuePair<string, ISClass> kv in xTable)
            {
                LoadLogicClassProperty(kv.Value, mstrPath + kv.Value.GetPath());
            }

            //再为每个类加载iobject的属性
            foreach (KeyValuePair<string, ISClass> kv in xTable)
            {
                if (kv.Key != "IObject")
                {
                    AddBasePropertyFormOther(kv.Value, "IObject");
                }
            }
        }

        private void LoadLogicClassRecord()
        {
            Dictionary<string, ISClass> xTable = GetElementList();
            foreach (KeyValuePair<string, ISClass> kv in xTable)
            {
                LoadLogicClassRecord(kv.Value, mstrPath + kv.Value.GetPath());
            }
        }

        private void LoadLogicClassProperty(ISClass xLogicClass, string strLogicPath)
        {
            if (null != xLogicClass)
            {
				strLogicPath = strLogicPath.Replace (".xml", "");

				TextAsset textAsset = (TextAsset) Resources.Load(strLogicPath); 

				XmlDocument xmldoc = new XmlDocument ();
				xmldoc.LoadXml ( textAsset.text );
				XmlNode xRoot = xmldoc.SelectSingleNode("XML");

                XmlNode xNodePropertys = xRoot.SelectSingleNode("Propertys");
                XmlNodeList xNodeList = xNodePropertys.SelectNodes("Property");
                for (int i = 0; i < xNodeList.Count; ++i)
                {
                    XmlNode xPropertyNode = xNodeList.Item(i);
                    XmlAttribute strID = xPropertyNode.Attributes["Id"];
                    XmlAttribute strType = xPropertyNode.Attributes["Type"];
                    XmlAttribute strUpload = xPropertyNode.Attributes["Upload"];
                    bool bUpload = strUpload.Value.Equals("1");

                    switch (strType.Value)
                    {
                        case "int":
                            {
                                DataList.TData xValue = new DataList.TData(DataList.VARIANT_TYPE.VTYPE_INT);
                                IProperty xProperty = xLogicClass.GetPropertyManager().AddProperty(strID.Value, xValue);
                                xProperty.SetUpload(bUpload);
                            }
                            break;
                        case "float":
                            {

                                DataList.TData xValue = new DataList.TData(DataList.VARIANT_TYPE.VTYPE_FLOAT);
                                IProperty xProperty = xLogicClass.GetPropertyManager().AddProperty(strID.Value, xValue);
                                xProperty.SetUpload(bUpload);
                            }
                            break;
                        case "string":
                            {

                                DataList.TData xValue = new DataList.TData(DataList.VARIANT_TYPE.VTYPE_STRING);
                                IProperty xProperty = xLogicClass.GetPropertyManager().AddProperty(strID.Value, xValue);
                                xProperty.SetUpload(bUpload);
                            }
                            break;
                        case "object":
                            {
                                DataList.TData xValue = new DataList.TData(DataList.VARIANT_TYPE.VTYPE_OBJECT);
                                IProperty xProperty = xLogicClass.GetPropertyManager().AddProperty(strID.Value, xValue);
                                xProperty.SetUpload(bUpload);
                            }
                            break;
                        case "vector2":
                            {
                                DataList.TData xValue = new DataList.TData(DataList.VARIANT_TYPE.VTYPE_VECTOR2);
                                IProperty xProperty = xLogicClass.GetPropertyManager().AddProperty(strID.Value, xValue);
                                xProperty.SetUpload(bUpload);
                            }
                            break;
                        case "vector3":
                            {
                                DataList.TData xValue = new DataList.TData(DataList.VARIANT_TYPE.VTYPE_VECTOR3);
                                IProperty xProperty = xLogicClass.GetPropertyManager().AddProperty(strID.Value, xValue);
                                xProperty.SetUpload(bUpload);
                            }
                            break;
                        default:
                            break;

                    }
                }
            }
        }

        private void LoadLogicClassRecord(ISClass xLogicClass, string strLogicPath)
        {
            if (null != xLogicClass)
            {
				strLogicPath = strLogicPath.Replace (".xml", "");

				TextAsset textAsset = (TextAsset) Resources.Load(strLogicPath); 

				XmlDocument xmldoc = new XmlDocument ();
				xmldoc.LoadXml ( textAsset.text );
				XmlNode xRoot = xmldoc.SelectSingleNode("XML");

                XmlNode xNodePropertys = xRoot.SelectSingleNode("Records");
                if (null != xNodePropertys)
                {
                    XmlNodeList xNodeList = xNodePropertys.SelectNodes("Record");
                    if (null != xNodeList)
                    {
                        for (int i = 0; i < xNodeList.Count; ++i)
                        {
                            XmlNode xRecordNode = xNodeList.Item(i);

                            string strID = xRecordNode.Attributes["Id"].Value;
                            string strRow = xRecordNode.Attributes["Row"].Value;
                            string strUpload = xRecordNode.Attributes["Upload"].Value;
                            bool bUpload = strUpload.Equals("1");
							DataList xValue = new DataList();
                            DataList xTag = new DataList();

                            XmlNodeList xTagNodeList = xRecordNode.SelectNodes("Col");
                            for (int j = 0; j < xTagNodeList.Count; ++j)
                            {
                                XmlNode xColTagNode = xTagNodeList.Item(j);

                                XmlAttribute strTagID = xColTagNode.Attributes["Tag"];
                                XmlAttribute strTagType = xColTagNode.Attributes["Type"];

								xTag.AddString (strTagID.Value);

                                switch (strTagType.Value)
                                {
                                    case "int":
                                        {
                                            xValue.AddInt(0);
                                        }
                                        break;
                                    case "float":
                                        {
                                            xValue.AddFloat(0.0);
                                        }
                                        break;
                                    case "string":
                                        {
                                            xValue.AddString("");
                                        }
                                        break;
                                    case "object":
                                        {
                                            xValue.AddObject(new Guid(0, 0));
                                        }
                                        break;
										case "vector2":
										{
											xValue.AddVector2(SVector2.Zero());
										}
										break;
										case "vector3":
										{
											xValue.AddVector3(SVector3.Zero());
										}
										break;
                                    default:
                                        break;

                                }
                            }
							IRecord xRecord = xLogicClass.GetRecordManager().AddRecord(strID, int.Parse(strRow), xValue, xTag);
                            xRecord.SetUpload(bUpload);
                        }
                    }
                }
            }
        }

        private void LoadLogicClassInclude()
        {
            Dictionary<string, ISClass> xTable = GetElementList();
            foreach (KeyValuePair<string, ISClass> kv in xTable)
            {
                LoadLogicClassInclude(kv.Key);
            }

        }

        private void LoadLogicClassInclude(string strName)
        {
            ISClass xLogicClass = GetElement(strName);
            if (null != xLogicClass)
            {
                string strLogicPath = mstrPath + xLogicClass.GetPath();

                strLogicPath = strLogicPath.Replace(".xml", "");

                TextAsset textAsset = (TextAsset)Resources.Load(strLogicPath);

                XmlDocument xmldoc = new XmlDocument();
                xmldoc.LoadXml(textAsset.text);
                XmlNode xRoot = xmldoc.SelectSingleNode("XML");

                XmlNode xNodePropertys = xRoot.SelectSingleNode("Includes");
                XmlNodeList xNodeList = xNodePropertys.SelectNodes("Include");
                for (int i = 0; i < xNodeList.Count; ++i)
                {
                    XmlNode xPropertyNode = xNodeList.Item(i);
                    XmlAttribute strID = xPropertyNode.Attributes["Id"];
                    //int start = strID.Value.LastIndexOf('/') + 1;
                    //int end = strID.Value.LastIndexOf('.');
                    //string className = strID.Value.Substring(start, end - start);

                    xLogicClass.AddIncludeFile(strID.Value);
                }
            }
        }

        void AddBasePropertyFormOther(ISClass xLogicClass, string strOther)
        {
            ISClass xOtherClass = GetElement(strOther);
            if (null != xLogicClass && null != xOtherClass)
            {
                DataList xValue = xOtherClass.GetPropertyManager().GetPropertyList();
                for (int i = 0; i < xValue.Count(); ++i)
                {
                    IProperty xProperty = xOtherClass.GetPropertyManager().GetProperty(xValue.StringVal(i));
                    xLogicClass.GetPropertyManager().AddProperty(xValue.StringVal(i), xProperty.GetData());
                }
            }
        }

        void AddBaseRecordFormOther(ISClass xLogicClass, string strOther)
        {
            ISClass xOtherClass = GetElement(strOther);
            if (null != xLogicClass && null != xOtherClass)
            {
                DataList xValue = xOtherClass.GetRecordManager().GetRecordList();
                for (int i = 0; i < xValue.Count(); ++i)
                {
                    IRecord record = xOtherClass.GetRecordManager().GetRecord(xValue.StringVal(i));

                    xLogicClass.GetRecordManager().AddRecord(xValue.StringVal(i), record.GetRows(), record.GetColsData(), record.GetTagData());
                }
            }
        }

        private void ProcessLogicClassIncludeFiles()
        {
            Dictionary<string, ISClass> xTable = GetElementList();
            foreach (KeyValuePair<string, ISClass> kv in xTable)
            {
                ProcessLogicClassIncludeFiles(kv.Value);
            }
        }

        private void ProcessLogicClassIncludeFiles(ISClass classObject)
        {
            List<string> includeFiles = classObject.GetIncludeFileList();
            foreach (var item in includeFiles)
            {
                LoadLogicClassProperty(classObject, item);
                LoadLogicClassRecord(classObject, item);
            }
        }

        public override Dictionary<string, ISClass> GetElementList()
        {
            return mhtObject;
        }
        /////////////////////////////////////////
        private Dictionary<string, ISClass> mhtObject = new Dictionary<string, ISClass>();
        private string mstrPath = "";
    }
}