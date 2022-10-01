
using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace Squick
{
    public abstract class IRecordManager
    {
		public abstract IRecord AddRecord(string strRecordName,  int nRow, DataList varData, DataList varTag);
		public abstract IRecord GetRecord(string strRecordName);
		public abstract DataList GetRecordList();
		
		public abstract void RegisterCallback(string strRecordName, IRecord.RecordEventHandler handler);
    }
}