using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Squick
{
    public class PluginManager : IPluginManager
    {
        public PluginManager()
        {
        }
        //------------- 接口 -------------------//

        public override void Awake()
        {
            foreach (IPlugin plugin in mPlugins.Values)
            {
                if (plugin != null)
                {
                    plugin.Awake();
                }
            }
        }

        public override void Init()
        {
            mInitTime = DateTime.Now.Ticks / 10000;
            foreach (IPlugin plugin in mPlugins.Values)
            {
                if (plugin != null)
                {
                    plugin.Init();
                }
            }
        }

        public override void AfterInit()
        {
            foreach (IPlugin plugin in mPlugins.Values)
            {
                if (plugin != null)
                {
                    plugin.AfterInit();
                }
            }
        }
        public override void Execute()
        {
            mNowTime = DateTime.Now.Ticks / 10000;

            foreach (IPlugin plugin in mPlugins.Values)
            {
                if (plugin != null)
                {
					plugin.Execute();
                }
            }
        }

        public override void BeforeShut()
        {
            foreach (IPlugin plugin in mPlugins.Values)
            {
                if (plugin != null)
                {
                    plugin.BeforeShut();
                }
            }
        }

        public override void Shut()
        {
            foreach (IPlugin plugin in mPlugins.Values)
            {
                if (plugin != null)
                {
                    plugin.Shut();
                }
            }
        }


        public override T _FindModule<T>()
        {
            IModule module = _FindModule(typeof(T).ToString());

            return (T)module;
        }

        public override IModule _FindModule(string strModuleName)
        {
            IModule module;
            mModules.TryGetValue(strModuleName, out module);
            return module;
        }
        public override void Registered(IPlugin plugin)
        {
            mPlugins.Add(plugin.GetPluginName(), plugin);
            plugin.Install();
        }
        public override void UnRegistered(IPlugin plugin)
        {
            mPlugins.Remove(plugin.GetPluginName());
            plugin.Uninstall();
        }
        public override void AddModule(string strModuleName, IModule pModule)
        {
            mModules.Add(strModuleName, pModule);
        }
        public override void RemoveModule(string strModuleName)
        {
            mModules.Remove(strModuleName);
        }

		public override void _RemoveModule<T>()
        {
			RemoveModule(typeof(T).ToString());
        }

        public override Int64 GetInitTime()
        {
            return mInitTime;
        }
        public override Int64 GetNowTime()
        {
            return mNowTime;
        }

        protected Int64 mInitTime;
        protected Int64 mNowTime;
        protected Dictionary<string, IPlugin> mPlugins = new Dictionary<string, IPlugin>();
        protected Dictionary<string, IModule> mModules = new Dictionary<string, IModule>();
    };
}