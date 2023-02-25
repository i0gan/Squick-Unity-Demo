
#include "plugin.h"
#include "auto_broadcast_module.h"
#include "npc_refresh_module.h"
#include "scene_auto_broadcast_module.h"
#include "scene_process_module.h"
#include "sync_pos_module.h"
#include "property_module.h"
#include "property_config_module.h"

namespace game::scene {
SQUICK_EXPORT void SquickPluginLoad(IPluginManager* pm)
{
	CREATE_PLUGIN(pm, Plugin)
};

SQUICK_EXPORT void SquickPluginUnload(IPluginManager* pm)
{
	DESTROY_PLUGIN(pm, Plugin)
};


//////////////////////////////////////////////////////////////////////////

const int Plugin::GetPluginVersion()
{
	return 0;
}

const std::string Plugin::GetPluginName()
{
	return GET_CLASS_NAME(Plugin);
}

void Plugin::Install()
{
	REGISTER_MODULE(pPluginManager, IAutoBroadcastModule, AutoBroadcastModule)
	REGISTER_MODULE(pPluginManager, ISceneAutoBroadcastModule, SceneAutoBroadcastModule)

	REGISTER_MODULE(pPluginManager, IPropertyModule, PropertyModule)
    REGISTER_MODULE(pPluginManager, IPropertyConfigModule, PropertyConfigModule)
    REGISTER_MODULE(pPluginManager, ISceneProcessModule, SceneProcessModule)
	REGISTER_MODULE(pPluginManager, INPCRefreshModule, NPCRefreshModule)
	REGISTER_MODULE(pPluginManager, ISyncPosModule, SyncPosModule)
}

void Plugin::Uninstall()
{
	UNREGISTER_MODULE(pPluginManager, ISceneAutoBroadcastModule, SceneAutoBroadcastModule)
	UNREGISTER_MODULE(pPluginManager, IAutoBroadcastModule, AutoBroadcastModule)

    UNREGISTER_MODULE(pPluginManager, ISyncPosModule, NFSyncModule)
    UNREGISTER_MODULE(pPluginManager, INPCRefreshModule, NPCRefreshModule)
    UNREGISTER_MODULE(pPluginManager, ISceneProcessModule, SceneProcessModule)
    UNREGISTER_MODULE(pPluginManager, IPropertyConfigModule, PropertyConfigModule)
    UNREGISTER_MODULE(pPluginManager, IPropertyModule, PropertyModule)
}

}