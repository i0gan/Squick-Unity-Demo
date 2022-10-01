
using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace Squick
{
	public class RecordManager : IRecordManager
	{
		public RecordManager(Guid ident)
		{
			mSelf = ident;
            mhtRecord = new Dictionary<string, IRecord>();
		}

		public override void RegisterCallback(string strRecordName, IRecord.RecordEventHandler handler)
		{
			if (mhtRecord.ContainsKey(strRecordName))
			{
				IRecord record = (IRecord)mhtRecord[strRecordName];
				record.RegisterCallback(handler);
			}
		}

		public override IRecord AddRecord(string strRecordName, int nRow, DataList varData, DataList varTag)
		{
			IRecord record = new Record (mSelf, strRecordName, nRow, varData, varTag);
			mhtRecord.Add(strRecordName, record);

			return record;
		}

		public override IRecord GetRecord(string strPropertyName)
		{
			IRecord record = null;

			if (mhtRecord.ContainsKey(strPropertyName))
			{
				record = (IRecord)mhtRecord[strPropertyName];
			}

			return record;
		}
		
		public override DataList GetRecordList()
		{
			DataList varData = new DataList();
            foreach (KeyValuePair<string, IRecord> de in mhtRecord) 
			{
				varData.AddString(de.Key);				
			}
			
			return varData;
		}
		
		Guid mSelf;
        //Hashtable mhtRecord;
        Dictionary<string, IRecord> mhtRecord;
	}
}