using UnityEngine;
using System.Collections;

namespace Squick
{
	public abstract class IModule
	{
		public abstract void Awake();
		public abstract void Init();
		public abstract void AfterInit();
		public abstract void Execute();
		public abstract void BeforeShut();
		public abstract void Shut();

        public IPluginManager mPluginManager;
        public string mName;
    };
}