
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Squick
{
	public abstract class ISEventModule : IModule
	{

		public abstract void RegisterCallback(int nEventID, ISEvent.EventHandler handler);
		public abstract void DoEvent(int nEventID);
		public abstract void DoEvent(int nEventID, DataList valueList);
	}
}
