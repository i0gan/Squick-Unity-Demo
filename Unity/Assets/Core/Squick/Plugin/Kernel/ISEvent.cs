using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Squick
{
	public abstract class ISEvent
	{
		public delegate void EventHandler(int eventId, DataList valueList);

		public abstract void RegisterCallback(ISEvent.EventHandler handler);
		public abstract void DoEvent(DataList valueList);
	}
}
