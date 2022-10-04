
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Squick
{
    public abstract class IObject
    {
        public enum CLASS_EVENT_TYPE
        {
            OBJECT_DESTROY,
            OBJECT_CREATE,
            OBJECT_LOADDATA,
            OBJECT_CREATE_FINISH,
        }

        public delegate void ClassEventHandler(Guid self, int nContainerID, int nGroupID, CLASS_EVENT_TYPE eType, string strClassName, string strConfigIndex);

        public abstract void Init();
        public abstract void Shut();

        ///////////////////////////////////////////////////////////////////////
        public abstract Guid Self();
        public abstract int ContainerID();
        public abstract int GroupID();
        public abstract string ClassName();
        public abstract string ConfigIndex();

        /////////////////////////////////////////////////////////////////

		public abstract IProperty FindProperty(string strPropertyName);

        public abstract bool SetPropertyInt(string strPropertyName, Int64 nValue);
        public abstract bool SetPropertyFloat(string strPropertyName, double fValue);
        public abstract bool SetPropertyString(string strPropertyName, string strValue);
        public abstract bool SetPropertyObject(string strPropertyName, Guid obj);
        public abstract bool SetPropertyVector2(string strPropertyName, SVector2 obj);
        public abstract bool SetPropertyVector3(string strPropertyName, SVector3 obj);

        public abstract Int64 QueryPropertyInt(string strPropertyName);
        public abstract double QueryPropertyFloat(string strPropertyName);
        public abstract string QueryPropertyString(string strPropertyName);
        public abstract Guid QueryPropertyObject(string strPropertyName);
        public abstract SVector2 QueryPropertyVector2(string strPropertyName);
        public abstract SVector3 QueryPropertyVector3(string strPropertyName);

		public abstract IRecord FindRecord(string strRecordName);

        public abstract bool SetRecordInt(string strRecordName, int nRow, int nCol, Int64 nValue);
        public abstract bool SetRecordFloat(string strRecordName, int nRow, int nCol, double fValue);
        public abstract bool SetRecordString(string strRecordName, int nRow, int nCol, string strValue);
        public abstract bool SetRecordObject(string strRecordName, int nRow, int nCol, Guid obj);
        public abstract bool SetRecordVector2(string strRecordName, int nRow, int nCol, SVector2 obj);
        public abstract bool SetRecordVector3(string strRecordName, int nRow, int nCol, SVector3 obj);

        public abstract Int64 QueryRecordInt(string strRecordName, int nRow, int nCol);
        public abstract double QueryRecordFloat(string strRecordName, int nRow, int nCol);
        public abstract string QueryRecordString(string strRecordName, int nRow, int nCol);
        public abstract Guid QueryRecordObject(string strRecordName, int nRow, int nCol);
        public abstract SVector2 QueryRecordVector2(string strRecordName, int nRow, int nCol);
        public abstract SVector3 QueryRecordVector3(string strRecordName, int nRow, int nCol);

        public abstract IRecordManager GetRecordManager();
        public abstract IPropertyManager GetPropertyManager();
    }
}