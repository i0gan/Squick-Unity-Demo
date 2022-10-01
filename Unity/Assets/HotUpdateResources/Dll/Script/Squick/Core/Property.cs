
using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace Squick
{
    public class Property : IProperty
    {
		public Property( Guid self, string strPropertyName, DataList varData)
        {
            mSelf = self;
            msPropertyName = strPropertyName;
            mxData = new DataList.TData(varData.GetType(0));

            switch (varData.GetType(0))
            {
                case DataList.VARIANT_TYPE.VTYPE_INT:
                    mxData.Set(varData.IntVal(0));
                    break;
                case DataList.VARIANT_TYPE.VTYPE_FLOAT:
                    mxData.Set(varData.FloatVal(0));
                    break;
                case DataList.VARIANT_TYPE.VTYPE_OBJECT:
                    mxData.Set(varData.ObjectVal(0));
                    break;
                case DataList.VARIANT_TYPE.VTYPE_STRING:
                    mxData.Set(varData.StringVal(0));
                    break;
                case DataList.VARIANT_TYPE.VTYPE_VECTOR2:
                    mxData.Set(varData.Vector2Val(0));
                    break;
                case DataList.VARIANT_TYPE.VTYPE_VECTOR3:
                    mxData.Set(varData.Vector3Val(0));
                    break;
                default:
                    break;
            }
        }

        public Property(Guid self, string strPropertyName, DataList.TData varData)
        {
            mSelf = self;
            msPropertyName = strPropertyName;
            mxData = new DataList.TData(varData);
        }

        public override string GetKey()
        {
            return msPropertyName;
        }
		
		public override DataList.VARIANT_TYPE GetType()
		{
            return mxData.GetType();
		}

        public override DataList.TData GetData()
        {
            return mxData;
        }

        public override void SetUpload(bool upload)
        {
            mbUpload = upload;
        }

        public override bool GetUpload()
        {
            return mbUpload;
        }

        public override Int64 QueryInt()
        {
            if (DataList.VARIANT_TYPE.VTYPE_INT == mxData.GetType())
            {
                return mxData.IntVal();
            }

			UnityEngine.Debug.LogError (this.msPropertyName + " is " + mxData.GetType().ToString());

            return DataList.NULL_INT;
        }

        public override double QueryFloat()
        {
            if (DataList.VARIANT_TYPE.VTYPE_FLOAT == mxData.GetType())
            {
                return (double)mxData.FloatVal();
            }

			UnityEngine.Debug.LogError (this.msPropertyName + " is " + mxData.GetType().ToString());

            return DataList.NULL_DOUBLE;
        }

        public override string QueryString()
        {
            if (DataList.VARIANT_TYPE.VTYPE_STRING == mxData.GetType())
            {
                return mxData.StringVal();
            }

			UnityEngine.Debug.LogError (this.msPropertyName + " is " + mxData.GetType().ToString());

            return DataList.NULL_STRING;
        }

        public override Guid QueryObject()
        {
			if (DataList.VARIANT_TYPE.VTYPE_OBJECT == mxData.GetType())
            {
                return (Guid)mxData.ObjectVal();
            }

			UnityEngine.Debug.LogError (this.msPropertyName + " is " + mxData.GetType().ToString());

            return DataList.NULL_OBJECT;
        }

        public override SVector2 QueryVector2()
        {
            if (DataList.VARIANT_TYPE.VTYPE_VECTOR2 == mxData.GetType())
            {
                return (SVector2)mxData.Vector2Val();
            }

			UnityEngine.Debug.LogError (this.msPropertyName + " is " + mxData.GetType().ToString());

            return DataList.NULL_VECTOR2;
        }

        public override SVector3 QueryVector3()
        {
            if (DataList.VARIANT_TYPE.VTYPE_VECTOR3 == mxData.GetType())
            {
                return (SVector3)mxData.Vector3Val();
            }

			UnityEngine.Debug.LogError (this.msPropertyName + " is " + mxData.GetType().ToString());

            return DataList.NULL_VECTOR3;
        }

        public override bool SetInt(Int64 value, Int64 reason = 0)
		{
			if (DataList.VARIANT_TYPE.VTYPE_INT != mxData.GetType ())
			{
				UnityEngine.Debug.LogError (this.msPropertyName + " is " + mxData.GetType().ToString() + " but you set type " + DataList.VARIANT_TYPE.VTYPE_INT.ToString());

				return false;
			}

            if (value == -1)
            {
                int i = 0;
                ++i;
            }

            if (mxData.IntVal() != value)
            {
                DataList.TData oldValue = new DataList.TData(mxData);
                DataList.TData newValue = new DataList.TData(DataList.VARIANT_TYPE.VTYPE_INT);
                newValue.Set(value);

                mxData.Set(value);

                if (null != doHandleDel)
                {
                    doHandleDel(mSelf, msPropertyName, oldValue, newValue, reason);
                }
				
			}

			return true;
		}

		public override bool SetFloat(double value, Int64 reason = 0)
		{
			if (DataList.VARIANT_TYPE.VTYPE_FLOAT != mxData.GetType ())
			{
				UnityEngine.Debug.LogError (this.msPropertyName + " is " + mxData.GetType().ToString() + " but you set type " + DataList.VARIANT_TYPE.VTYPE_FLOAT.ToString());

				return false;
			}

            if (mxData.FloatVal() - value > DataList.EPS_DOUBLE
                || mxData.FloatVal() - value < -DataList.EPS_DOUBLE)
            {
                DataList.TData oldValue = new DataList.TData(mxData);
                DataList.TData newValue = new DataList.TData(DataList.VARIANT_TYPE.VTYPE_FLOAT);
                newValue.Set(value);

                mxData.Set(value);

                if (null != doHandleDel)
                {
                    doHandleDel(mSelf, msPropertyName, oldValue, newValue, reason);
                }
			}

			return true;
		}

		public override bool SetString(string value, Int64 reason = 0)
		{
			if (DataList.VARIANT_TYPE.VTYPE_STRING != mxData.GetType ())
			{
				UnityEngine.Debug.LogError (this.msPropertyName + " is " + mxData.GetType().ToString() + " but you set type " + DataList.VARIANT_TYPE.VTYPE_STRING.ToString());

				return false;
			}
            if (mxData.StringVal() != value)
            {
                DataList.TData oldValue = new DataList.TData(mxData);
                DataList.TData newValue = new DataList.TData(DataList.VARIANT_TYPE.VTYPE_STRING);
                newValue.Set(value);

                mxData.Set(value);

                if (null != doHandleDel)
                {
                    doHandleDel(mSelf, msPropertyName, oldValue, newValue, reason);
                }
            }

			return true;
		}

		public override bool SetObject(Guid value, Int64 reason = 0)
		{
			if (DataList.VARIANT_TYPE.VTYPE_OBJECT != mxData.GetType ())
			{
				UnityEngine.Debug.LogError (this.msPropertyName + " is " + mxData.GetType().ToString() + " but you set type " + DataList.VARIANT_TYPE.VTYPE_OBJECT.ToString());

				return false;
			}

            if (mxData.ObjectVal() != value)
            {
                DataList.TData oldValue = new DataList.TData(mxData);
                DataList.TData newValue = new DataList.TData(DataList.VARIANT_TYPE.VTYPE_OBJECT);
                newValue.Set(value);

                mxData.Set(value);

                if (null != doHandleDel)
                {
                    doHandleDel(mSelf, msPropertyName, oldValue, newValue, reason);
                }
            }

			return true;
		}

        public override bool SetVector2(SVector2 value, Int64 reason = 0)
        {
			if (DataList.VARIANT_TYPE.VTYPE_VECTOR2 != mxData.GetType ())
			{
				UnityEngine.Debug.LogError (this.msPropertyName + " is " + mxData.GetType().ToString() + " but you set type " + DataList.VARIANT_TYPE.VTYPE_VECTOR2.ToString());

				return false;
			}

            if (mxData.Vector2Val() != value)
            {
                DataList.TData oldValue = new DataList.TData(mxData);
                DataList.TData newValue = new DataList.TData(DataList.VARIANT_TYPE.VTYPE_VECTOR2);
                newValue.Set(value);

                mxData.Set(value);

                if (null != doHandleDel)
                {
                    doHandleDel(mSelf, msPropertyName, oldValue, newValue, reason);
                }
            }

            return true;
        }

        public override bool SetVector3(SVector3 value, Int64 reason = 0)
        {
			if (DataList.VARIANT_TYPE.VTYPE_VECTOR3 != mxData.GetType ())
			{
				UnityEngine.Debug.LogError (this.msPropertyName + " is " + mxData.GetType().ToString() + " but you set type " + DataList.VARIANT_TYPE.VTYPE_VECTOR3.ToString());

				return false;
			}

            if (mxData.Vector3Val() != value)
            {
                DataList.TData oldValue = new DataList.TData(mxData);
                DataList.TData newValue = new DataList.TData(DataList.VARIANT_TYPE.VTYPE_VECTOR3);
                newValue.Set(value);

                mxData.Set(value);

                if (null != doHandleDel)
                {
                    doHandleDel(mSelf, msPropertyName, oldValue, newValue, reason);
                }
            }

            return true;
        }

        public override bool SetData(DataList.TData x)
        {
            if (DataList.VARIANT_TYPE.VTYPE_UNKNOWN == mxData.GetType()
                || x.GetType() == mxData.GetType())
            {
                switch (mxData.GetType())
                {
                    case DataList.VARIANT_TYPE.VTYPE_INT:
                        SetInt(x.IntVal());
                        break;
                    case DataList.VARIANT_TYPE.VTYPE_STRING:
                        SetString(x.StringVal());
                        break;
                    case DataList.VARIANT_TYPE.VTYPE_FLOAT:
                        SetFloat(x.FloatVal());
                        break;
                    case DataList.VARIANT_TYPE.VTYPE_OBJECT:
                        SetObject(x.ObjectVal());
                        break;
                    case DataList.VARIANT_TYPE.VTYPE_VECTOR2:
                        SetVector2(x.Vector2Val());
                        break;
                    case DataList.VARIANT_TYPE.VTYPE_VECTOR3:
                        SetVector3(x.Vector3Val());
                        break;
                    default:
                        break;
                }

                return true;
            }

            return false;
        }

		public override void RegisterCallback(PropertyEventHandler handler)
		{
			doHandleDel += handler;

            if (null != handler)
            {
                DataList.TData oldValue = new DataList.TData(mxData);
                handler(mSelf, msPropertyName, oldValue, oldValue, 0);
            }
        }

		PropertyEventHandler doHandleDel;

		Guid mSelf;
		string msPropertyName;
        DataList.TData mxData;
        bool mbUpload;
    }
}