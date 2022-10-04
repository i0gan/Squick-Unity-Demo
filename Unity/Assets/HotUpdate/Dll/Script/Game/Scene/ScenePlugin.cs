using UnityEngine;
using System.Collections;

namespace Squick
{
    public class ScenePlugin : IPlugin
    {
		public ScenePlugin(IPluginManager pluginManager)
        {
            mPluginManager = pluginManager;
        }
        public override string GetPluginName()
        {
			return "ScenePlugin";
        }

        public override void Install()
        {
            AddModule<SceneModule>(new SceneModule(mPluginManager));
        }
        public override void Uninstall()
        {
            mPluginManager.RemoveModule<SceneModule>();

            mModules.Clear();
        }
    }
}
