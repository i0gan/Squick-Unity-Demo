using UnityEngine;
using System.Collections;
using SquickProtocol;

namespace Squick
{
    public class SquickPlugin : IPlugin
    {
        public SquickPlugin(IPluginManager pluginManager)
        {
            mPluginManager = pluginManager;
        }
        public override string GetPluginName()
        {
			return "SquickPlugin";
        }

        public override void Install()
        {

            AddModule<IElementModule>(new ElementModule(mPluginManager));
            AddModule<IClassModule>(new SClassModule(mPluginManager));
            AddModule<IKernelModule>(new KernelModule(mPluginManager));
            AddModule<ISEventModule>(new SEventModule(mPluginManager));

            AddModule<NetEventModule>(new NetEventModule(mPluginManager));
            AddModule<LagTestModule>(new LagTestModule(mPluginManager));
            AddModule<LogModule>(new LogModule(mPluginManager));
            AddModule<LoginModule>(new LoginModule(mPluginManager));
            AddModule<NetHandlerModule>(new NetHandlerModule(mPluginManager));
            AddModule<NetModule>(new NetModule(mPluginManager));
            AddModule<HelpModule>(new HelpModule(mPluginManager));
            AddModule<LanguageModule>(new LanguageModule(mPluginManager));
            AddModule<UploadDataModule>(new UploadDataModule(mPluginManager));
        }
        public override void Uninstall()
        {

            mPluginManager.RemoveModule<UploadDataModule>();
            mPluginManager.RemoveModule<NetEventModule>();
            mPluginManager.RemoveModule<LanguageModule>();
            mPluginManager.RemoveModule<HelpModule>();
            mPluginManager.RemoveModule<NetModule>();
            mPluginManager.RemoveModule<NetHandlerModule>();
            mPluginManager.RemoveModule<LoginModule>();
            mPluginManager.RemoveModule<LogModule>();
            mPluginManager.RemoveModule<LagTestModule>();

            mPluginManager.RemoveModule<IElementModule>();
			mPluginManager.RemoveModule<IClassModule>();
			mPluginManager.RemoveModule<IKernelModule>();
			mPluginManager.RemoveModule<ISEventModule>();

            mModules.Clear();
        }
    }
}
