using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Squick
{
    public abstract class IKernelModule : IModule
    {
        public abstract DataList GetObjectList();

        /////////////////////////////////////////////////////////////
        public abstract void RegisterGroupPropertyCallback(string strPropertyName, IProperty.PropertyEventHandler handler);
        public abstract void RegisterGroupRecordCallback(string strRecordName, IRecord.RecordEventHandler handler);

        /////////////////////////////////////////////////////////////
        public abstract void RegisterPropertyCallback(Guid self, string strPropertyName, IProperty.PropertyEventHandler handler);

        public abstract void RegisterRecordCallback(Guid self, string strRecordName, IRecord.RecordEventHandler handler);

        public abstract void RegisterClassCallBack(string strClassName, IObject.ClassEventHandler handler);

        public abstract void RegisterEventCallBack(Guid self, int nEventID, ISEvent.EventHandler handler);

        /////////////////////////////////////////////////////////////////

        public abstract IObject GetObject(Guid ident);

        public abstract IObject CreateObject(Guid self, int nContainerID, int nGroupID, string strClassName, string strConfigIndex, DataList arg);

        public abstract bool DestroyObject(Guid self);

		public abstract IProperty FindProperty(Guid self, string strPropertyName);

        public abstract bool SetPropertyInt(Guid self, string strPropertyName, Int64 nValue);
        public abstract bool SetPropertyFloat(Guid self, string strPropertyName, double fValue);
        public abstract bool SetPropertyString(Guid self, string strPropertyName, string strValue);
        public abstract bool SetPropertyObject(Guid self, string strPropertyName, Guid objectValue);
        public abstract bool SetPropertyVector2(Guid self, string strPropertyName, SVector2 objectValue);
        public abstract bool SetPropertyVector3(Guid self, string strPropertyName, SVector3 objectValue);

        public abstract Int64 QueryPropertyInt(Guid self, string strPropertyName);
        public abstract double QueryPropertyFloat(Guid self, string strPropertyName);
        public abstract string QueryPropertyString(Guid self, string strPropertyName);
        public abstract Guid QueryPropertyObject(Guid self, string strPropertyName);
        public abstract SVector2 QueryPropertyVector2(Guid self, string strPropertyName);
        public abstract SVector3 QueryPropertyVector3(Guid self, string strPropertyName);

        public abstract IRecord FindRecord(Guid self, string strRecordName);

        public abstract bool SetRecordInt(Guid self, string strRecordName, int nRow, int nCol, Int64 nValue);
        public abstract bool SetRecordFloat(Guid self, string strRecordName, int nRow, int nCol, double fValue);
        public abstract bool SetRecordString(Guid self, string strRecordName, int nRow, int nCol, string strValue);
        public abstract bool SetRecordObject(Guid self, string strRecordName, int nRow, int nCol, Guid objectValue);
        public abstract bool SetRecordVector2(Guid self, string strRecordName, int nRow, int nCol, SVector2 objectValue);
        public abstract bool SetRecordVector3(Guid self, string strRecordName, int nRow, int nCol, SVector3 objectValue);

        public abstract Int64 QueryRecordInt(Guid self, string strRecordName, int nRow, int nCol);
        public abstract double QueryRecordFloat(Guid self, string strRecordName, int nRow, int nCol);
        public abstract string QueryRecordString(Guid self, string strRecordName, int nRow, int nCol);
        public abstract Guid QueryRecordObject(Guid self, string strRecordName, int nRow, int nCol);
        public abstract SVector2 QueryRecordVector2(Guid self, string strRecordName, int nRow, int nCol);
        public abstract SVector3 QueryRecordVector3(Guid self, string strRecordName, int nRow, int nCol);

        public abstract int FindRecordRow(Guid self, string strRecordName, int nCol, int value);
        public abstract int FindRecordRow(Guid self, string strRecordName, int nCol, double value);
        public abstract int FindRecordRow(Guid self, string strRecordName, int nCol, string value);
        public abstract int FindRecordRow(Guid self, string strRecordName, int nCol, Guid value);
        public abstract int FindRecordRow(Guid self, string strRecordName, int nCol, SVector2 value);
        public abstract int FindRecordRow(Guid self, string strRecordName, int nCol, SVector3 value);

        public abstract int FindRecordRow(Guid self, string strRecordName, int nCol, int nValue, ref DataList xDatalist);
        public abstract int FindRecordRow(Guid self, string strRecordName, int nCol, double fValue, ref DataList xDatalist);
        public abstract int FindRecordRow(Guid self, string strRecordName, int nCol, string strValue, ref DataList xDatalist);
        public abstract int FindRecordRow(Guid self, string strRecordName, int nCol, Guid nValue, ref DataList xDatalist);
        public abstract int FindRecordRow(Guid self, string strRecordName, int nCol, SVector2 nValue, ref DataList xDatalist);
        public abstract int FindRecordRow(Guid self, string strRecordName, int nCol, SVector3 nValue, ref DataList xDatalist);


		public abstract int Random(int start, int end);
    }
}