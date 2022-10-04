
using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace Squick
{
    public class DataList
    {
        private Dictionary<int, TData> mValueObject = new Dictionary<int, TData>();

        //==============================================
        public enum VARIANT_TYPE
        {
            VTYPE_UNKNOWN,  // 未知
            VTYPE_INT,              // 64
            VTYPE_FLOAT,            // 单精度浮点数
            VTYPE_STRING,       // 字符串
            VTYPE_OBJECT,       // 对象ID
            VTYPE_VECTOR2,
            VTYPE_VECTOR3,
            VTYPE_MAX,
        };

        public class TData
        {
            private TData()
            {
                mData = new System.Object();
                nType = VARIANT_TYPE.VTYPE_UNKNOWN;
            }

            public TData(TData x)
            {
                nType = x.nType;
                mData = x.mData;
            }


            public TData(VARIANT_TYPE eType)
            {
                mData = new System.Object();
                nType = eType;
                switch (eType)
                {
                    case VARIANT_TYPE.VTYPE_INT:
                        mData = (Int64)0;
                        break;
                    case VARIANT_TYPE.VTYPE_FLOAT:
                        mData = (double)0;
                        break;
                    case VARIANT_TYPE.VTYPE_OBJECT:
                        mData = new Squick.Guid();
                        break;
                    case VARIANT_TYPE.VTYPE_STRING:
                        mData = "";
                        break;
                    case VARIANT_TYPE.VTYPE_VECTOR2:
                        mData = new SVector2();
                        break;
                    case VARIANT_TYPE.VTYPE_VECTOR3:
                        mData = new SVector3();
                        break;
                    default:
                        break;
                }
            }

            public VARIANT_TYPE GetType()
            {
                return nType;
            }

            public bool Set(Int64 value)
            {
                if (nType == VARIANT_TYPE.VTYPE_UNKNOWN)
                {
                    nType = VARIANT_TYPE.VTYPE_INT;
                }

                if (nType == VARIANT_TYPE.VTYPE_INT)
                {
                    mData = value;
                    return true;
                }

                return false;
            }

            public bool Set(double value)
            {
                if (nType == VARIANT_TYPE.VTYPE_UNKNOWN)
                {
                    nType = VARIANT_TYPE.VTYPE_FLOAT;
                }

                if (nType == VARIANT_TYPE.VTYPE_FLOAT)
                {
                    mData = value;
                    return true;
                }

                return false;
            }

            public bool Set(string value)
            {
                if (nType == VARIANT_TYPE.VTYPE_UNKNOWN)
                {
                    nType = VARIANT_TYPE.VTYPE_STRING;
                }

                if (nType == VARIANT_TYPE.VTYPE_STRING)
                {
                    mData = value;
                    return true;
                }

                return false;
            }

            public bool Set(Guid value)
            {
                if (nType == VARIANT_TYPE.VTYPE_UNKNOWN)
                {
                    nType = VARIANT_TYPE.VTYPE_OBJECT;
                }

                if (nType == VARIANT_TYPE.VTYPE_OBJECT)
                {
                    mData = value;
                    return true;
                }

                return false;
            }

            public bool Set(SVector2 value)
            {
                if (nType == VARIANT_TYPE.VTYPE_UNKNOWN)
                {
                    nType = VARIANT_TYPE.VTYPE_VECTOR2;
                }

                if (nType == VARIANT_TYPE.VTYPE_VECTOR2)
                {
                    mData = value;
                    return true;
                }

                return false;
            }

            public bool Set(SVector3 value)
            {
                if (nType == VARIANT_TYPE.VTYPE_UNKNOWN)
                {
                    nType = VARIANT_TYPE.VTYPE_VECTOR3;
                }

                if (nType == VARIANT_TYPE.VTYPE_VECTOR3)
                {
                    mData = value;
                    return true;
                }

                return false;
            }

            public Int64 IntVal()
            {
                if (nType == VARIANT_TYPE.VTYPE_INT)
                {

                    return (Int64)mData;
                }

                return DataList.NULL_INT;
            }

            public double FloatVal()
            {
                if (nType == VARIANT_TYPE.VTYPE_FLOAT)
                {

                    return (double)mData;
                }

                return DataList.NULL_DOUBLE;
            }

            public string StringVal()
            {
                if (nType == VARIANT_TYPE.VTYPE_STRING)
                {

                    return (string)mData;
                }

                return DataList.NULL_STRING;
            }

            public Guid ObjectVal()
            {
                if (nType == VARIANT_TYPE.VTYPE_OBJECT)
                {

                    return (Guid)mData;
                }

                return DataList.NULL_OBJECT;
            }

            public SVector2 Vector2Val()
            {
                if (nType == VARIANT_TYPE.VTYPE_VECTOR2)
                {

                    return (SVector2)mData;
                }

                return DataList.NULL_VECTOR2;
            }

            public SVector3 Vector3Val()
            {
                if (nType == VARIANT_TYPE.VTYPE_VECTOR3)
                {

                    return (SVector3)mData;
                }

                return DataList.NULL_VECTOR3;
            }

            public override string ToString()
            {
                switch (nType)
                {
                    case VARIANT_TYPE.VTYPE_INT:
                        return IntVal().ToString();
                        break;
                    case VARIANT_TYPE.VTYPE_FLOAT:
                        return FloatVal().ToString();
                        break;
                    case VARIANT_TYPE.VTYPE_OBJECT:
                        return ObjectVal().ToString();
                        break;
                    case VARIANT_TYPE.VTYPE_STRING:
                        return StringVal();
                        break;
                    case VARIANT_TYPE.VTYPE_VECTOR2:
                        return Vector2Val().ToString();
                        break;
                    case VARIANT_TYPE.VTYPE_VECTOR3:
                        return Vector3Val().ToString();
                        break;
                    default:
                        break;
                }

                return DataList.NULL_STRING;
            }

            private VARIANT_TYPE nType;
            private System.Object mData;
        }

        public static readonly Int64 NULL_INT = 0;
        public static readonly double NULL_DOUBLE = 0.0;
        public static readonly string NULL_STRING = "";
        public static readonly Guid NULL_OBJECT = new Guid();
        public static readonly SVector2 NULL_VECTOR2 = new SVector2();
        public static readonly SVector3 NULL_VECTOR3 = new SVector3();
        public static readonly double EPS_DOUBLE = 0.0001;
        public static readonly TData NULL_TDATA = new TData(DataList.VARIANT_TYPE.VTYPE_UNKNOWN);

        public DataList(string str, char c)
        {
            string[] strSub = str.Split(c);
            foreach (string strDest in strSub)
            {
                AddString(strDest);
            }
        }

        public DataList(DataList src)
        {
            for (int i = 0; i < src.Count(); i++)
            {
                switch (src.GetType(i))
                {
                    case VARIANT_TYPE.VTYPE_INT:
                        AddInt(src.IntVal(i));
                        break;
                    case VARIANT_TYPE.VTYPE_FLOAT:
                        AddFloat(src.FloatVal(i));
                        break;
                    case VARIANT_TYPE.VTYPE_STRING:
                        AddString(src.StringVal(i));
                        break;
                    case VARIANT_TYPE.VTYPE_OBJECT:
                        AddObject(src.ObjectVal(i));
                        break;
                    case VARIANT_TYPE.VTYPE_VECTOR2:
                        AddVector2(src.Vector2Val(i));
                        break;
                    case VARIANT_TYPE.VTYPE_VECTOR3:
                        AddVector3(src.Vector3Val(i));
                        break;
                    default:
                        break;
                }
            }

        }

        public DataList()
        {
        }

        public bool AddInt(Int64 value)
        {
            TData data = new TData(VARIANT_TYPE.VTYPE_INT);
            data.Set(value);

            return AddDataObject(ref data);
        }

        public bool AddFloat(double value)
        {
            TData data = new TData(VARIANT_TYPE.VTYPE_FLOAT);
            data.Set(value);

            return AddDataObject(ref data);
        }

        public bool AddString(string value)
        {
            TData data = new TData(VARIANT_TYPE.VTYPE_STRING);
            data.Set(value);

            return AddDataObject(ref data);
        }

        public bool AddObject(Guid value)
        {
            TData data = new TData(VARIANT_TYPE.VTYPE_OBJECT);
            data.Set(value);

            return AddDataObject(ref data);
        }

        public bool AddVector2(SVector2 value)
        {
            TData data = new TData(VARIANT_TYPE.VTYPE_VECTOR2);
            data.Set(value);

            return AddDataObject(ref data);
        }

        public bool AddVector3(SVector3 value)
        {
            TData data = new TData(VARIANT_TYPE.VTYPE_VECTOR3);
            data.Set(value);

            return AddDataObject(ref data);
        }

        public bool SetInt(int index, Int64 value)
        {
            TData data = GetData(index);
            if (data != null && data.GetType() == VARIANT_TYPE.VTYPE_INT)
            {
                data.Set(value);

                return true;
            }

            return false;
        }

        public bool SetFloat(int index, double value)
        {
            TData data = GetData(index);
            if (data != null && data.GetType() == VARIANT_TYPE.VTYPE_FLOAT)
            {
                data.Set(value);

                return true;
            }

            return false;
        }

        public bool SetString(int index, string value)
        {
            TData data = GetData(index);
            if (data != null && data.GetType() == VARIANT_TYPE.VTYPE_STRING)
            {
                data.Set(value);

                return true;
            }

            return false;
        }

        public bool SetObject(int index, Guid value)
        {
            TData data = GetData(index);
            if (data != null && data.GetType() == VARIANT_TYPE.VTYPE_OBJECT)
            {
                data.Set(value);

                return true;
            }

            return false;
        }

        public bool SetVector2(int index, SVector2 value)
        {
            TData data = GetData(index);
            if (data != null && data.GetType() == VARIANT_TYPE.VTYPE_VECTOR2)
            {
                data.Set(value);

                return true;
            }

            return false;
        }

        public bool SetVector3(int index, SVector3 value)
        {
            TData data = GetData(index);
            if (data != null && data.GetType() == VARIANT_TYPE.VTYPE_VECTOR3)
            {
                data.Set(value);

                return true;
            }

            return false;
        }

        public Int64 IntVal(int index)
        {
            TData data = GetData(index);
            if (data != null && data.GetType() == VARIANT_TYPE.VTYPE_INT)
            {
                return data.IntVal();
            }

            return DataList.NULL_INT;
        }

        public double FloatVal(int index)
        {
            TData data = GetData(index);
            if (data != null && data.GetType() == VARIANT_TYPE.VTYPE_FLOAT)
            {
                return (double)data.FloatVal();
            }

            return (float)DataList.NULL_DOUBLE;
        }

        public string StringVal(int index)
        {
            TData data = GetData(index);
            if (data != null && data.GetType() == VARIANT_TYPE.VTYPE_STRING)
            {
                return data.StringVal();
            }

            return DataList.NULL_STRING;
        }

        public Guid ObjectVal(int index)
        {
            TData data = GetData(index);
            if (data != null && data.GetType() == VARIANT_TYPE.VTYPE_OBJECT)
            {
                return data.ObjectVal();
            }

            return DataList.NULL_OBJECT;
        }

        public SVector2 Vector2Val(int index)
        {
            TData data = GetData(index);
            if (data != null && data.GetType() == VARIANT_TYPE.VTYPE_VECTOR3)
            {
                return data.Vector2Val();
            }

            return DataList.NULL_VECTOR2;
        }

        public SVector3 Vector3Val(int index)
        {
            TData data = GetData(index);
            if (data != null && data.GetType() == VARIANT_TYPE.VTYPE_VECTOR3)
            {
                return data.Vector3Val();
            }

            return DataList.NULL_VECTOR3;
        }

        public int Count()
        {
            return mValueObject.Count;
        }

        public void Clear()
        {
            mValueObject.Clear();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected bool AddDataObject(ref TData data)
        {
            int nCount = mValueObject.Count;
            mValueObject.Add(nCount, data);

            return true;
        }

        public VARIANT_TYPE GetType(int index)
        {
            if (mValueObject.Count > index)
            {
                TData data = (TData)mValueObject[index];

                return data.GetType();
            }

            return VARIANT_TYPE.VTYPE_UNKNOWN;
        }

        public TData GetData(int index)
        {
            if (mValueObject.ContainsKey(index))
            {
                return (TData)mValueObject[index];
            }

            return null;
        }
    }
}
