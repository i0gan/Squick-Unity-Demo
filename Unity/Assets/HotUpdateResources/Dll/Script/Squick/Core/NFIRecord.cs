//-----------------------------------------------------------------------
// <copyright file="IRecord.cs">
//     Copyright (C) 2015-2015 lvsheng.huang <https://github.com/ketoo/SquickProtocol>
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Squick
{
    public abstract class IRecord
    {

        public enum ERecordOptype
        {
            Add = 0,
            Del,
            Swap,
            Create,
            Update,
            Cleared,
            Sort,
            Cover,

        };

        public delegate void RecordEventHandler(Guid self, string strRecordName, IRecord.ERecordOptype eType, int nRow, int nCol, DataList.TData oldVar, DataList.TData newVar);

        public abstract void SetUpload(bool upload);
        public abstract bool GetUpload();
        public abstract bool IsUsed(int nRow);
        public abstract int GetUsedRows();
        public abstract int GetRows();
        public abstract int GetCols();
		public abstract DataList.VARIANT_TYPE GetColType(int nCol);
        public abstract string GetColTag(int nCol);
		public abstract DataList GetColsData();
        public abstract DataList GetTagData();

        // add data
        public abstract int AddRow(int nRow);
        public abstract int AddRow(int nRow, DataList var);

        // set data
        public abstract int SetValue(int nRow, DataList var);

        public abstract bool SetInt(int nRow, int nCol, Int64 value);
        public abstract bool SetFloat(int nRow, int nCol, double value);
        public abstract bool SetString(int nRow, int nCol, string value);
        public abstract bool SetObject(int nRow, int nCol, Guid value);
        public abstract bool SetVector2(int nRow, int nCol, SVector2 value);
        public abstract bool SetVector3(int nRow, int nCol, SVector3 value);

        // query data
		public abstract DataList QueryRow(int nRow);
		public abstract DataList.TData QueryRowCol(int nRow, int nCol);
        public abstract bool SwapRow(int nOriginRow, int nTargetRow);

        public abstract Int64 QueryInt(int nRow, int nCol);
        public abstract double QueryFloat(int nRow, int nCol);
        public abstract string QueryString(int nRow, int nCol);
        public abstract Guid QueryObject(int nRow, int nCol);
        public abstract SVector2 QueryVector2(int nRow, int nCol);
        public abstract SVector3 QueryVector3(int nRow, int nCol);

        //public abstract int FindRow( int nRow );
        public abstract int FindColValue(int nCol, DataList var, ref DataList varResult);

        public abstract int FindRecordRow(int nCol, int value);
        public abstract int FindRecordRow(int nCol, double value);
        public abstract int FindRecordRow(int nCol, string value);
        public abstract int FindRecordRow(int nCol, Guid value);
        public abstract int FindRecordRow(int nCol, SVector2 value);
        public abstract int FindRecordRow(int nCol, SVector3 value);

        public abstract int FindInt(int nCol, Int64 value, ref DataList varResult);
        public abstract int FindFloat(int nCol, double value, ref DataList varResult);
        public abstract int FindString(int nCol, string value, ref DataList varResult);
        public abstract int FindObject(int nCol, Guid value, ref DataList varResult);
        public abstract int FindVector2(int nCol, SVector2 value, ref DataList varResult);
        public abstract int FindVector3(int nCol, SVector3 value, ref DataList varResult);

		public abstract int FindInt(int nCol, Int64 value);
		public abstract int FindFloat(int nCol, double value);
		public abstract int FindString(int nCol, string value);
		public abstract int FindObject(int nCol, Guid value);
		public abstract int FindVector2(int nCol, SVector2 value);
		public abstract int FindVector3(int nCol, SVector3 value);

        public abstract bool Remove(int nRow);
        public abstract bool Clear();

        public abstract void RegisterCallback(RecordEventHandler handler);

        public abstract string GetName();
        public abstract void SetName(string strName);
    }
}