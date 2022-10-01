using UnityEngine;
using System.Collections;

namespace Squick
{
    public class NFScenePlugin : IPlugin
    {
		public NFScenePlugin(IPluginManager pluginManager)
        {
            mPluginManager = pluginManager;
        }
        public override string GetPluginName()
        {
			return "NFScenePlugin";
        }

        public override void Install()
        {
            AddModule<NFSceneModule>(new NFSceneModule(mPluginManager));
        }
        public override void Uninstall()
        {
            mPluginManager.RemoveModule<NFSceneModule>();

            mModules.Clear();
        }
    }
}
