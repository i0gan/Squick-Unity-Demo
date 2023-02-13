using UnityEngine;
using System.Collections;

namespace Squick
{
    public class UIPlugin : IPlugin
    {
        public UIPlugin(IPluginManager pluginManager)
        {
            mPluginManager = pluginManager;
        }
        public override string GetPluginName()
        {
			return "UIPlugin";
        }

        public override void Install()
        {
            AddModule<UIModule>(new UIModule(mPluginManager));
        }
        public override void Uninstall()
        {
			mPluginManager.RemoveModule<UIModule>();

            mModules.Clear();
        }
    }
}
