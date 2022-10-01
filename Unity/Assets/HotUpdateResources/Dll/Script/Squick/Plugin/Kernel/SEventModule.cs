using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace Squick
{
	public class SEventModule : ISEventModule
    {
        public override void Awake() {}
        public override void Init() {}
        public override void AfterInit() {}
        public override void Execute() { }
        public override void BeforeShut() { }
        public override void Shut() {  }

        public SEventModule(IPluginManager pluginManager)
        {
            mPluginManager = pluginManager;
            mhtEvent = new Dictionary<int, ISEvent>();
		}
  
  
        public override void RegisterCallback(int nEventID, ISEvent.EventHandler handler)
        {
            if (!mhtEvent.ContainsKey(nEventID))
            {
				mhtEvent.Add(nEventID, new SEvent(nEventID, new DataList()));
            }

			ISEvent identEvent = (ISEvent)mhtEvent[nEventID];
            identEvent.RegisterCallback(handler);
        }

        public override void DoEvent(int nEventID, DataList valueList)
        {
            if (mhtEvent.ContainsKey(nEventID))
            {
                ISEvent identEvent = (ISEvent)mhtEvent[nEventID];
                identEvent.DoEvent(valueList);
            }
        }

        public override void DoEvent(int nEventID)
        {
			DataList valueList = new DataList();
			if (mhtEvent.ContainsKey(nEventID))
            {
				ISEvent identEvent = (ISEvent)mhtEvent[nEventID];
                identEvent.DoEvent(valueList);
            }
        }

        Dictionary<int, ISEvent> mhtEvent;
    }
}