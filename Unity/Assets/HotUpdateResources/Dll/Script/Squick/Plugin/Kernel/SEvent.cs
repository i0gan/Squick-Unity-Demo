using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Squick
{
	class SEvent : ISEvent
	{
		public SEvent(int nEventID, DataList valueList)
		{
			mnEventID = nEventID;
            mArgValueList = valueList;
		}

		public override void RegisterCallback(ISEvent.EventHandler handler)
		{
			mHandlerDel += handler;
		}

		public override void DoEvent(DataList valueList)
		{
			if (null != mHandlerDel)
			{
				//mHandlerDel(mSelf, mnEventID, mArgValueList, valueList);
				mHandlerDel(mnEventID, valueList);
			}
		}

		Guid mSelf;
		int mnEventID;
		DataList mArgValueList;
		ISEvent.EventHandler mHandlerDel;
	}
}
