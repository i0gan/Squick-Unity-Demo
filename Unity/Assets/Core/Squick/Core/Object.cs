
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Squick
{
    public class Object : IObject
    {
        public Object(Guid self, int nContainerID, int nGroupID, string strClassName, string strConfigIndex)
        {
            mSelf = self;
            mstrClassName = strClassName;
            mstrConfigIndex = strConfigIndex;
            mnContainerID = nContainerID;
            mnGroupID = nGroupID;
            Init();
        }

        ~Object()
        {
            Shut();
        }

        public override void Init()
        {
            mRecordManager = new RecordManager(mSelf);
            mPropertyManager = new PropertyManager(mSelf);

            return;
        }

        public override void Shut()
        {
            DataList xRecordList = mRecordManager.GetRecordList();
            if (null != xRecordList)
            {
                for (int i = 0; i < xRecordList.Count(); ++i)
                {
                    string strRecordName = xRecordList.StringVal(i);
                    IRecord xRecord = mRecordManager.GetRecord(strRecordName);
                    if (null != xRecord)
                    {
                        xRecord.Clear();
                    }
                }
            }

            mRecordManager = null;
            mPropertyManager = null;

            return;
        }

        ///////////////////////////////////////////////////////////////////////
        public override Guid Self()
        {
            return mSelf;
        }

        public override int ContainerID()
        {
            return mnContainerID;
        }

        public override int GroupID()
        {
            return mnGroupID;
        }

        public override string ClassName()
        {
            return mstrClassName;
        }

        public override string ConfigIndex()
        {
            return mstrConfigIndex;
        }

		public override IProperty FindProperty(string strPropertyName)
        {
			return mPropertyManager.GetProperty(strPropertyName);
        }

        public override bool SetPropertyInt(string strPropertyName, Int64 nValue)
        {
            IProperty property = mPropertyManager.GetProperty(strPropertyName);
            if (null == property)
            {
                DataList.TData xValue = new DataList.TData(DataList.VARIANT_TYPE.VTYPE_INT);
                property = mPropertyManager.AddProperty(strPropertyName, xValue);
            }

            property.SetInt(nValue);
            return true;
        }

        public override bool SetPropertyFloat(string strPropertyName, double fValue)
        {
            IProperty property = mPropertyManager.GetProperty(strPropertyName);
            if (null == property)
            {
                DataList.TData xValue = new DataList.TData(DataList.VARIANT_TYPE.VTYPE_FLOAT);
                property = mPropertyManager.AddProperty(strPropertyName, xValue);
            }

            property.SetFloat(fValue);
            return true;
        }

        public override bool SetPropertyString(string strPropertyName, string strValue)
        {
            IProperty property = mPropertyManager.GetProperty(strPropertyName);
            if (null == property)
            {
                DataList.TData xValue = new DataList.TData(DataList.VARIANT_TYPE.VTYPE_STRING);
                property = mPropertyManager.AddProperty(strPropertyName, xValue); ;
            }

            property.SetString(strValue);
            return true;
        }

        public override bool SetPropertyObject(string strPropertyName, Guid obj)
        {
            IProperty property = mPropertyManager.GetProperty(strPropertyName);
            if (null == property)
            {
                DataList.TData xValue = new DataList.TData(DataList.VARIANT_TYPE.VTYPE_OBJECT);
                property = mPropertyManager.AddProperty(strPropertyName, xValue);
            }

            property.SetObject(obj);
            return true;

        }

        public override bool SetPropertyVector2(string strPropertyName, SVector2 obj)
        {
            IProperty property = mPropertyManager.GetProperty(strPropertyName);
            if (null == property)
            {
                DataList.TData xValue = new DataList.TData(DataList.VARIANT_TYPE.VTYPE_VECTOR2);
                property = mPropertyManager.AddProperty(strPropertyName, xValue);
            }

            property.SetVector2(obj);
            return true;

        }

        public override bool SetPropertyVector3(string strPropertyName, SVector3 obj)
        {
            IProperty property = mPropertyManager.GetProperty(strPropertyName);
            if (null == property)
            {
                DataList.TData xValue = new DataList.TData(DataList.VARIANT_TYPE.VTYPE_VECTOR3);
                property = mPropertyManager.AddProperty(strPropertyName, xValue);
            }

            property.SetVector3(obj);
            return true;

        }

        public override Int64 QueryPropertyInt(string strPropertyName)
        {
            IProperty property = mPropertyManager.GetProperty(strPropertyName);
            if (null != property)
            {
                return property.QueryInt();
            }

            return 0;
        }

        public override double QueryPropertyFloat(string strPropertyName)
        {
            IProperty property = mPropertyManager.GetProperty(strPropertyName);
            if (null != property)
            {
                return property.QueryFloat();
            }

            return 0.0;
        }

        public override string QueryPropertyString(string strPropertyName)
        {
            IProperty property = mPropertyManager.GetProperty(strPropertyName);
            if (null != property)
            {
                return property.QueryString();
            }

            return DataList.NULL_STRING;
        }

        public override Guid QueryPropertyObject(string strPropertyName)
        {
            IProperty property = mPropertyManager.GetProperty(strPropertyName);
            if (null != property)
            {
                return property.QueryObject();
            }

            return DataList.NULL_OBJECT;
        }

        public override SVector2 QueryPropertyVector2(string strPropertyName)
        {
            IProperty property = mPropertyManager.GetProperty(strPropertyName);
            if (null != property)
            {
                return property.QueryVector2();
            }

            return DataList.NULL_VECTOR2;
        }

        public override SVector3 QueryPropertyVector3(string strPropertyName)
        {
            IProperty property = mPropertyManager.GetProperty(strPropertyName);
            if (null != property)
            {
                return property.QueryVector3();
            }

            return DataList.NULL_VECTOR3;
        }

		public override IRecord FindRecord(string strRecordName)
        {
            return mRecordManager.GetRecord(strRecordName);
        }

        public override bool SetRecordInt(string strRecordName, int nRow, int nCol, Int64 nValue)
        {
            IRecord record = mRecordManager.GetRecord(strRecordName);
            if (null != record)
            {
                record.SetInt(nRow, nCol, nValue);
                return true;
            }

            return false;
        }

        public override bool SetRecordFloat(string strRecordName, int nRow, int nCol, double fValue)
        {
            IRecord record = mRecordManager.GetRecord(strRecordName);
            if (null != record)
            {
                record.SetFloat(nRow, nCol, fValue);
                return true;
            }

            return false;
        }

        public override bool SetRecordString(string strRecordName, int nRow, int nCol, string strValue)
        {
            IRecord record = mRecordManager.GetRecord(strRecordName);
            if (null != record)
            {
                record.SetString(nRow, nCol, strValue);
                return true;
            }

            return false;
        }

        public override bool SetRecordObject(string strRecordName, int nRow, int nCol, Guid obj)
        {
            IRecord record = mRecordManager.GetRecord(strRecordName);
            if (null != record)
            {
                record.SetObject(nRow, nCol, obj);
                return true;
            }

            return false;
        }

        public override bool SetRecordVector2(string strRecordName, int nRow, int nCol, SVector2 obj)
        {
            IRecord record = mRecordManager.GetRecord(strRecordName);
            if (null != record)
            {
                record.SetVector2(nRow, nCol, obj);
                return true;
            }

            return false;
        }

        public override bool SetRecordVector3(string strRecordName, int nRow, int nCol, SVector3 obj)
        {
            IRecord record = mRecordManager.GetRecord(strRecordName);
            if (null != record)
            {
                record.SetVector3(nRow, nCol, obj);
                return true;
            }

            return false;
        }

        public override Int64 QueryRecordInt(string strRecordName, int nRow, int nCol)
        {
            IRecord record = mRecordManager.GetRecord(strRecordName);
            if (null != record)
            {
                return record.QueryInt(nRow, nCol);
            }

            return 0;
        }

        public override double QueryRecordFloat(string strRecordName, int nRow, int nCol)
        {
            IRecord record = mRecordManager.GetRecord(strRecordName);
            if (null != record)
            {
                return record.QueryFloat(nRow, nCol);
            }

            return 0.0;
        }

        public override string QueryRecordString(string strRecordName, int nRow, int nCol)
        {
            IRecord record = mRecordManager.GetRecord(strRecordName);
            if (null != record)
            {
                return record.QueryString(nRow, nCol);
            }

            return DataList.NULL_STRING;
        }

        public override Guid QueryRecordObject(string strRecordName, int nRow, int nCol)
        {
            IRecord record = mRecordManager.GetRecord(strRecordName);
            if (null != record)
            {
                return record.QueryObject(nRow, nCol);
            }

            return null;
        }

        public override SVector2 QueryRecordVector2(string strRecordName, int nRow, int nCol)
        {
            IRecord record = mRecordManager.GetRecord(strRecordName);
            if (null != record)
            {
                return record.QueryVector2(nRow, nCol);
            }

            return null;
        }

        public override SVector3 QueryRecordVector3(string strRecordName, int nRow, int nCol)
        {
            IRecord record = mRecordManager.GetRecord(strRecordName);
            if (null != record)
            {
                return record.QueryVector3(nRow, nCol);
            }

            return null;
        }

        public override IRecordManager GetRecordManager()
        {
            return mRecordManager;
        }

        public override IPropertyManager GetPropertyManager()
        {
            return mPropertyManager;
        }

        Guid mSelf;
        int mnContainerID;
        int mnGroupID;

        string mstrClassName;
        string mstrConfigIndex;

        IRecordManager mRecordManager;
        IPropertyManager mPropertyManager;
    }
}