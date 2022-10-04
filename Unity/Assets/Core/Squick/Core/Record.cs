
//-----------------------------------------------------------------------
using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace Squick
{
    public class Record : IRecord
    {

		public Record(Guid self, string strRecordName, int nRow, DataList varData, DataList varTag)
		{
			mSelf = self;
			mnRow = nRow;
			mstrRecordName = strRecordName;
            mVarRecordType = new DataList(varData);
			mxTag = new DataList(varTag);
		}

        public override void SetUpload(bool upload)
        {
            mbUpload = upload;
        }

        public override bool GetUpload()
        {
            return mbUpload;
        }

        //==============================================
        public override bool IsUsed(int nRow)
		{
			if (mhtRecordVec.ContainsKey(nRow))
			{
				return true;
			}
			
			return false;
		}

        public override int GetUsedRows()
        {
            return mhtRecordVec.Count;
        }

        public override int GetRows()
        {
			return mnRow;
        }
		
        public override int GetCols()
        {
			return mVarRecordType.Count();
        }

        public override DataList.VARIANT_TYPE GetColType(int nCol)
        {
			return mVarRecordType.GetType(nCol);
        }

        public override DataList GetColsData()
        {
            return mVarRecordType;
        }

		public override DataList GetTagData()
		{
			return mxTag;
		}
		public override string GetColTag(int nCol)
		{
			return mxTag.StringVal (nCol);
		}

        // add data
        public override int AddRow(int nRow)
        {
			if(nRow >= 0 && nRow < mnRow)
			{
				return AddRow(nRow, mVarRecordType);
			}

            UnityEngine.Debug.LogError(this.mstrRecordName + " AddRow Failed:" + nRow.ToString());
            return -1;
        }

        public override int AddRow(int nRow, DataList var)
        {
			if(nRow >= 0 && nRow < mnRow)
			{
				if (!mhtRecordVec.ContainsKey(nRow))
				{
					mhtRecordVec[nRow] = new DataList(var);

                    if (null != doHandleDel)
                    {
                        doHandleDel(mSelf, mstrRecordName, ERecordOptype.Add, nRow, 0, DataList.NULL_TDATA, DataList.NULL_TDATA);
                    }
					return nRow;
				}
			}


            UnityEngine.Debug.LogError(this.mstrRecordName + " AddRow Failed:" + nRow.ToString());
            return -1;
        }

        // set data
        public override int SetValue(int nRow, DataList var)
        {
			if(nRow >= 0 && nRow < mnRow)
			{
				if (!mhtRecordVec.ContainsKey(nRow))
                {
                    return -1;
                }
				
				mhtRecordVec[nRow] = var;
				return nRow;
			}
            return -1;
        }

        public override bool SetInt(int nRow, int nCol, Int64 value)
        {
			if(nRow >= 0 && nRow < mnRow)
			{
				if (!mhtRecordVec.ContainsKey(nRow))
                {
                    return false;
                }
				DataList valueList = (DataList)mhtRecordVec[nRow];
				if (valueList.GetType(nCol) == DataList.VARIANT_TYPE.VTYPE_INT)
				{
					if (valueList.IntVal(nCol) != value)
					{
                        DataList.TData oldValue = new DataList.TData(DataList.VARIANT_TYPE.VTYPE_INT);
                        DataList.TData newValue = new DataList.TData(DataList.VARIANT_TYPE.VTYPE_INT);

                        oldValue.Set(valueList.IntVal(nCol));
                        newValue.Set(value);

                        valueList.SetInt(nCol, value);
	                   
	                    if (null != doHandleDel)
	                    {
	                        doHandleDel(mSelf, mstrRecordName, ERecordOptype.Update, nRow, nCol, oldValue, newValue);
	                    }
	                }
				}
				return true;
				
			}
            return false;
        }

        public override bool SetFloat(int nRow, int nCol, double value)
        {
			if(nRow >= 0 && nRow < mnRow)
			{
				if (!mhtRecordVec.ContainsKey(nRow))
                {
                    return false;
                }
				DataList valueList = (DataList)mhtRecordVec[nRow];
				if (valueList.GetType(nCol) == DataList.VARIANT_TYPE.VTYPE_FLOAT)
				{
					if (valueList.FloatVal(nCol) - value > DataList.EPS_DOUBLE
						|| valueList.FloatVal(nCol) - value < -DataList.EPS_DOUBLE)
					{
                        DataList.TData oldValue = new DataList.TData(DataList.VARIANT_TYPE.VTYPE_FLOAT);
                        DataList.TData newValue = new DataList.TData(DataList.VARIANT_TYPE.VTYPE_FLOAT);

                        oldValue.Set(valueList.FloatVal(nCol));
                        newValue.Set(value);

	                    valueList.SetFloat(nCol, value);
	
	                    if (null != doHandleDel)
	                    {
	                        doHandleDel(mSelf, mstrRecordName, ERecordOptype.Update, nRow, nCol, oldValue, newValue);
	                    }
	                }
	
				}
	
				return true;
			}
			return false;
        }

        public override bool SetString(int nRow, int nCol, string value)
        {
			if(nRow >= 0 && nRow < mnRow)
			{
				if (!mhtRecordVec.ContainsKey(nRow))
                {
                    return false;
                }
				DataList valueList = (DataList)mhtRecordVec[nRow];
				if (valueList.GetType(nCol) == DataList.VARIANT_TYPE.VTYPE_STRING)
				{
					if (valueList.StringVal(nCol) != value)
					{
                        DataList.TData oldValue = new DataList.TData(DataList.VARIANT_TYPE.VTYPE_STRING);
                        DataList.TData newValue = new DataList.TData(DataList.VARIANT_TYPE.VTYPE_STRING);

                        oldValue.Set(valueList.StringVal(nCol));
                        newValue.Set(value);

                        valueList.SetString(nCol, value);
	
	                    if (null != doHandleDel)
	                    {
	                        doHandleDel(mSelf, mstrRecordName, ERecordOptype.Update, nRow, nCol, oldValue, newValue);
	                    }
	                }
				}
	
				return true;
			}

			return false;
        }

        public override bool SetObject(int nRow, int nCol, Guid value)
        {
			if(nRow >= 0 && nRow < mnRow)
			{
				if (!mhtRecordVec.ContainsKey(nRow))
                {
                    return false;
                }
				DataList valueList = (DataList)mhtRecordVec[nRow];
				if (valueList.GetType(nCol) == DataList.VARIANT_TYPE.VTYPE_OBJECT)
				{
					if (valueList.ObjectVal(nCol) != value)
					{
                        DataList.TData oldValue = new DataList.TData(DataList.VARIANT_TYPE.VTYPE_OBJECT);
                        DataList.TData newValue = new DataList.TData(DataList.VARIANT_TYPE.VTYPE_OBJECT);

                        oldValue.Set(valueList.ObjectVal(nCol));
                        newValue.Set(value);

                        valueList.SetObject(nCol, value);

                        if (null != doHandleDel)
                        {
                            doHandleDel(mSelf, mstrRecordName, ERecordOptype.Update, nRow, nCol, oldValue, newValue);
                        }
                    }
				}

				return true;
			}

			return false;
        }

        public override bool SetVector2(int nRow, int nCol, SVector2 value)
        {
            if (nRow >= 0 && nRow < mnRow)
            {
                if (!mhtRecordVec.ContainsKey(nRow))
                {
                    return false;
                }
                DataList valueList = (DataList)mhtRecordVec[nRow];
                if (valueList.GetType(nCol) == DataList.VARIANT_TYPE.VTYPE_VECTOR2)
                {
                    if (valueList.Vector2Val(nCol) != value)
                    {
                        DataList.TData oldValue = new DataList.TData(DataList.VARIANT_TYPE.VTYPE_VECTOR2);
                        DataList.TData newValue = new DataList.TData(DataList.VARIANT_TYPE.VTYPE_VECTOR2);

                        oldValue.Set(valueList.Vector2Val(nCol));
                        newValue.Set(value);

                        valueList.SetVector2(nCol, value);

                        if (null != doHandleDel)
                        {
                            doHandleDel(mSelf, mstrRecordName, ERecordOptype.Update, nRow, nCol, oldValue, newValue);
                        }
                    }
                }

                return true;
            }

            return false;
        }

        public override bool SetVector3(int nRow, int nCol, SVector3 value)
        {
            if (nRow >= 0 && nRow < mnRow)
            {
                if (!mhtRecordVec.ContainsKey(nRow))
                {
                    return false;
                }
                DataList valueList = (DataList)mhtRecordVec[nRow];
                if (valueList.GetType(nCol) == DataList.VARIANT_TYPE.VTYPE_VECTOR3)
                {
                    if (valueList.Vector3Val(nCol) != value)
                    {
                        DataList.TData oldValue = new DataList.TData(DataList.VARIANT_TYPE.VTYPE_VECTOR3);
                        DataList.TData newValue = new DataList.TData(DataList.VARIANT_TYPE.VTYPE_VECTOR3);

                        oldValue.Set(valueList.Vector3Val(nCol));
                        newValue.Set(value);

                        valueList.SetVector3(nCol, value);

                        if (null != doHandleDel)
                        {
                            doHandleDel(mSelf, mstrRecordName, ERecordOptype.Update, nRow, nCol, oldValue, newValue);
                        }
                    }
                }

                return true;
            }

            return false;
        }

        // query data
        public override DataList QueryRow(int nRow)
        {
			if (mhtRecordVec.ContainsKey(nRow))
			{
				return  (DataList)mhtRecordVec[nRow];
			}

            return null;
        }

		public override DataList.TData QueryRowCol(int nRow, int nCol)
		{
			if (mhtRecordVec.ContainsKey(nRow))
			{
				DataList dataList =  (DataList)mhtRecordVec[nRow];
				if (dataList != null)
				{
					return dataList.GetData (nCol);
				}
			}

			return null;
		}

        public override bool SwapRow(int nOriginRow, int nTargetRow)
        {
			if(nOriginRow >= 0 && nOriginRow < mnRow && nTargetRow >= 0 && nTargetRow < mnRow)
			{
	            DataList valueOriginList = null;
	            DataList valueTargetList = null;
	           
	            if (mhtRecordVec.ContainsKey(nOriginRow))
	            {
	                valueOriginList = (DataList)mhtRecordVec[nOriginRow];
	            }
	            if (mhtRecordVec.ContainsKey(nTargetRow))
	            {
	                valueTargetList = (DataList)mhtRecordVec[nOriginRow];
	            }
	
	            if (null == valueTargetList)
	            {
	                if (mhtRecordVec.ContainsKey(nOriginRow))
	                {
	                    mhtRecordVec.Remove(nOriginRow);
	                }
	            }
	            else
	            {
	               mhtRecordVec[nOriginRow] = valueTargetList;
	            }
	            
	            if (null == valueOriginList)
	            {
	                if (mhtRecordVec.ContainsKey(nTargetRow))
	                {
	                    mhtRecordVec.Remove(nTargetRow);
	                }
	            }
	            else
	            {
	                mhtRecordVec[nTargetRow] = valueOriginList;
	            }
	           
	            if (null != doHandleDel)
	             {
	                 doHandleDel(mSelf, mstrRecordName, ERecordOptype.Swap, nOriginRow, nTargetRow, DataList.NULL_TDATA, DataList.NULL_TDATA);
	             }
	            return true;
			}
			return false;
        }

        public override Int64 QueryInt(int nRow, int nCol)
        {
			DataList valueList = QueryRow(nRow);
			if (null != valueList)
			{
				return valueList.IntVal(nCol);
			}

			return 0;
        }

        public override double QueryFloat(int nRow, int nCol)
        {
			DataList valueList = QueryRow(nRow);
			if (null != valueList)
			{
				return valueList.FloatVal(nCol);
			}

            return 0.0;
        }

        public override string QueryString(int nRow, int nCol)
        {
			DataList valueList = QueryRow(nRow);
			if (null != valueList)
			{
				return valueList.StringVal(nCol);
			}

            return DataList.NULL_STRING;
        }

        public override Guid QueryObject(int nRow, int nCol)
        {
			DataList valueList = QueryRow(nRow);
			if (null != valueList)
			{
				return valueList.ObjectVal(nCol);
			}

            return DataList.NULL_OBJECT;
        }

        public override SVector2 QueryVector2(int nRow, int nCol)
        {
            DataList valueList = QueryRow(nRow);
            if (null != valueList)
            {
                return valueList.Vector2Val(nCol);
            }

            return DataList.NULL_VECTOR2;
        }

        public override SVector3 QueryVector3(int nRow, int nCol)
        {
            DataList valueList = QueryRow(nRow);
            if (null != valueList)
            {
                return valueList.Vector3Val(nCol);
            }

            return DataList.NULL_VECTOR3;
        }

        //public override int FindRow( int nRow );
        public override int FindColValue(int nCol, DataList var, ref DataList varResult)
        {
			for (int i = 0; i < mhtRecordVec.Count; i++ )
			{
				DataList valueList = (DataList)mhtRecordVec[i];
				switch (valueList.GetType(0))
				{
					case DataList.VARIANT_TYPE.VTYPE_INT:
						return FindInt(nCol, var.IntVal(0), ref varResult);

					case DataList.VARIANT_TYPE.VTYPE_FLOAT:
						return FindInt(nCol, var.IntVal(0), ref varResult);

					case DataList.VARIANT_TYPE.VTYPE_STRING:
						return FindInt(nCol, var.IntVal(0), ref varResult);

					case DataList.VARIANT_TYPE.VTYPE_OBJECT:
						return FindObject(nCol, var.ObjectVal(0), ref varResult);

                    case DataList.VARIANT_TYPE.VTYPE_VECTOR2:
                        return FindVector2(nCol, var.Vector2Val(0), ref varResult);

                    case DataList.VARIANT_TYPE.VTYPE_VECTOR3:
                        return FindVector3(nCol, var.Vector3Val(0), ref varResult);
                    default:
					break;
				}
			}


            return -1;
        }

        public override int FindRecordRow(int nCol, int value)
        {
            foreach (int i in mhtRecordVec.Keys)
            {
                DataList valueList = (DataList)mhtRecordVec[i];
                if (valueList.IntVal(nCol) == value)
                {
                    return i;
                }
            }

            return -1;
        }

        public override int FindRecordRow(int nCol, double value)
        {
            foreach (int i in mhtRecordVec.Keys)
            {
                DataList valueList = (DataList)mhtRecordVec[i];
                if (Math.Abs(valueList.FloatVal(nCol) - value) < 0.01)
                {
                    return i;
                }
            }

            return -1;
        }

        public override int FindRecordRow(int nCol, string value)
        {
            foreach (int i in mhtRecordVec.Keys)
            {
                DataList valueList = (DataList)mhtRecordVec[i];
                if (valueList.StringVal(nCol) == value)
                {
                    return i;
                }
            }

            return -1;
        }

        public override int FindRecordRow(int nCol, Guid value)
        {
            foreach (int i in mhtRecordVec.Keys)
            {
                DataList valueList = (DataList)mhtRecordVec[i];
                if (valueList.ObjectVal(nCol) == value)
                {
                    return i;
                }
            }

            return -1;
        }

        public override int FindRecordRow(int nCol, SVector2 value)
        {
            foreach (int i in mhtRecordVec.Keys)
            {
                DataList valueList = (DataList)mhtRecordVec[i];
                if (valueList.Vector2Val(nCol) == value)
                {
                    return i;
                }
            }

            return -1;
        }

        public override int FindRecordRow(int nCol, SVector3 value)
        {
            foreach (int i in mhtRecordVec.Keys)
            {
                DataList valueList = (DataList)mhtRecordVec[i];
                if (valueList.Vector3Val(nCol) == value)
                {
                    return i;
                }
            }

            return -1;
        }

        public override int FindInt(int nCol, Int64 value, ref DataList varResult)
        {
			foreach (int i in mhtRecordVec.Keys)
			{
				DataList valueList = (DataList)mhtRecordVec[i];
				if (valueList.IntVal(nCol) == value)
				{
                    varResult.AddInt(i);
                }
            }

            return varResult.Count();
        }

        public override int FindFloat(int nCol, double value, ref DataList varResult)
        {
			foreach (int i in mhtRecordVec.Keys)
			{
				DataList valueList = (DataList)mhtRecordVec[i];
				if (valueList.FloatVal(nCol) == value)
				{
                    varResult.AddInt(i);
                }
            }

            return varResult.Count();
        }

        public override int FindString(int nCol, string value, ref DataList varResult)
        {
			foreach (int i in mhtRecordVec.Keys)
			{
				DataList valueList = (DataList)mhtRecordVec[i];
				if (valueList.StringVal(nCol) == value)
				{
                    varResult.AddInt(i);
                }
            }

            return varResult.Count();
        }

        public override int FindObject(int nCol, Guid value, ref DataList varResult)
        {
			foreach (int i in mhtRecordVec.Keys)
			{
				DataList valueList = (DataList)mhtRecordVec[i];
				if (valueList.ObjectVal(nCol) == value)
				{
                    varResult.AddInt(i);
                }
			}

            return varResult.Count();
        }

        public override int FindVector2(int nCol, SVector2 value, ref DataList varResult)
        {
            foreach (int i in mhtRecordVec.Keys)
            {
                DataList valueList = (DataList)mhtRecordVec[i];
                if (valueList.Vector2Val(nCol) == value)
                {
                    varResult.AddInt(i);
                }
            }

            return varResult.Count();
        }

        public override int FindVector3(int nCol, SVector3 value, ref DataList varResult)
        {
            foreach (int i in mhtRecordVec.Keys)
            {
                DataList valueList = (DataList)mhtRecordVec[i];
                if (valueList.Vector3Val(nCol) == value)
                {
                    varResult.AddInt(i);
                }
            }

            return varResult.Count();
        }
		public override int FindInt(int nCol, Int64 value)
		{
			DataList varResult = new DataList ();
			int nCount = FindInt (nCol, value, ref varResult);
			if (nCount > 0 && varResult.Count() > 0)
			{
				return (int)varResult.IntVal (0);
			}

			return -1;
		}

		public override int FindFloat(int nCol, double value)
		{
			DataList varResult = new DataList ();
			int nCount = FindFloat (nCol, value, ref varResult);
			if (nCount > 0 && varResult.Count() > 0)
			{
				return (int)varResult.IntVal (0);
			}

			return -1;
		}

		public override int FindString(int nCol, string value)
		{
			DataList varResult = new DataList ();
			int nCount = FindString (nCol, value, ref varResult);
			if (nCount > 0 && varResult.Count() > 0)
			{
				return (int)varResult.IntVal (0);
			}

			return -1;
		}

		public override int FindObject(int nCol, Guid value)
		{
			DataList varResult = new DataList ();
			int nCount = FindObject (nCol, value, ref varResult);
			if (nCount > 0 && varResult.Count() > 0)
			{
				return (int)varResult.IntVal (0);
			}

			return -1;
		}

		public override int FindVector2(int nCol, SVector2 value)
		{
			DataList varResult = new DataList ();
			int nCount = FindVector2 (nCol, value, ref varResult);
			if (nCount > 0 && varResult.Count() > 0)
			{
				return (int)varResult.IntVal (0);
			}

			return -1;
		}

		public override int FindVector3(int nCol, SVector3 value)
		{
			DataList varResult = new DataList ();
			int nCount = FindVector3 (nCol, value, ref varResult);
			if (nCount > 0 && varResult.Count() > 0)
			{
				return (int)varResult.IntVal (0);
			}

			return -1;
		}

        public override bool Remove(int nRow)
        {
			if (mhtRecordVec.Contains(nRow))
            {
				if (null != doHandleDel)
                {
                    doHandleDel(mSelf, mstrRecordName, ERecordOptype.Del, nRow, 0, DataList.NULL_TDATA, DataList.NULL_TDATA);
                }
				mhtRecordVec.Remove(nRow);
				return true;
            }

            return false;
        }

        public override bool Clear()
        {
			mhtRecordVec.Clear();

            return true;
        }

		public override void RegisterCallback(RecordEventHandler handler)
		{
			doHandleDel += handler;
		}

        public override string GetName()
        {
            return mstrRecordName;
        }

        public override void SetName(string strName)
        {
            mstrRecordName = strName;
        }

		RecordEventHandler doHandleDel;

		DataList mVarRecordType;
        Hashtable mhtRecordVec = new Hashtable();
		DataList mxTag;

		Guid mSelf;
		string mstrRecordName;
		int mnRow;
        bool mbUpload;
    }
}