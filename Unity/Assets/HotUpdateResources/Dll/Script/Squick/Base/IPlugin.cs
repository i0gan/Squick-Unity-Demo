using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Squick
{
    public abstract class IPlugin : IModule
    {
        //------------- 接口 -------------------//
        public abstract string GetPluginName();
        public abstract void Install();
        public abstract void Uninstall();
        public override void Awake()
        {
            foreach (IModule module in mModules.Values)
            {
                if (module != null)
                {
                    module.Awake();
                }
            }
        }

        public override void Init()
        {
            foreach (IModule module in mModules.Values)
            {
                if (module != null)
                {
                    module.Init();
                }
            }
        }

        public override void AfterInit()
        {
            foreach (IModule module in mModules.Values)
            {
                if (module != null)
                {
                    module.AfterInit();
                }
            }
        }

        public override void Execute()
        {
            foreach (IModule module in mModules.Values)
            {
                if (module != null)
                {
					module.Execute();
                }
            }
        }

        public override void BeforeShut()
        {
            foreach (IModule module in mModules.Values)
            {
                if (module != null)
                {
                    module.BeforeShut();
                }
            }
        }

        public override void Shut()
        {
            foreach (IModule module in mModules.Values)
            {
                if (module != null)
                {
                    module.Shut();
                }
            }
        }

        public void AddModule<T1>(IModule module)
        {
            string strName = typeof(T1).ToString();
            mPluginManager.AddModule(strName, module);
            mModules.Add(strName, module);
        }

        protected Dictionary<string, IModule> mModules = new Dictionary<string, IModule>();
    };
}