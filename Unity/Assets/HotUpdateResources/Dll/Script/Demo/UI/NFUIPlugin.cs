using UnityEngine;
using System.Collections;

namespace Squick
{
    public class NFUIPlugin : IPlugin
    {
        public NFUIPlugin(IPluginManager pluginManager)
        {
            mPluginManager = pluginManager;
        }
        public override string GetPluginName()
        {
			return "NFUIPlugin";
        }

        public override void Install()
        {
            AddModule<NFUIModule>(new NFUIModule(mPluginManager));
        }
        public override void Uninstall()
        {
			mPluginManager.RemoveModule<NFUIModule>();

            mModules.Clear();
        }
    }
}
