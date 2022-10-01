
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Squick
{
    public abstract class IProperty
    {
        public delegate void PropertyEventHandler(Guid self, string strProperty, DataList.TData oldVar, DataList.TData newVar, Int64 reason);

        public abstract string GetKey();

        public abstract DataList.VARIANT_TYPE GetType();
        public abstract DataList.TData GetData();

        public abstract void SetUpload(bool upload);

        public abstract bool GetUpload();

        public abstract Int64 QueryInt();

        public abstract double QueryFloat();

        public abstract string QueryString();

        public abstract Guid QueryObject();

        public abstract SVector2 QueryVector2();

        public abstract SVector3 QueryVector3();

        public abstract bool SetInt(Int64 value, Int64 reason = 0);

        public abstract bool SetFloat(double value, Int64 reason = 0);

        public abstract bool SetString(string value, Int64 reason = 0);

        public abstract bool SetObject(Guid value, Int64 reason = 0);

        public abstract bool SetVector2(SVector2 value, Int64 reason = 0);

        public abstract bool SetVector3(SVector3 value, Int64 reason = 0);

        public abstract bool SetData(DataList.TData x);

        public abstract void RegisterCallback(PropertyEventHandler handler);
    }
}