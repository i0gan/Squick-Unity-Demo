using UnityEngine;
using System.Collections;
using System;

namespace Squick
{
    public abstract class IPluginManager : IModule
    {
        public abstract T _FindModule<T>() where T : IModule;
        public abstract IModule _FindModule(string strModuleName);
		public abstract void Registered(IPlugin plugin);
        public abstract void UnRegistered(IPlugin plugin);
        public abstract void AddModule(string strModuleName, IModule pModule);
		public abstract void RemoveModule(string strModuleName);
		public abstract void _RemoveModule<T>() where T : IModule;

        public abstract Int64 GetInitTime();
        public abstract Int64 GetNowTime();


		public T FindModule<T>() where T : IModule
		{
			return _FindModule<T>();
		}

		public IModule FindModule(string strModuleName)
		{
			return _FindModule(strModuleName);
		}

		public void RemoveModule<T>() where T : IModule
		{
			_RemoveModule<T>();
		}
	};
}