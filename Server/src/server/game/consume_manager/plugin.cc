#include "plugin.h"
#include "cooldown_module.h"
#include "item_consume_manager_module.h"
#include "skill_consume_manager_module.h"

#ifdef SQUICK_DYNAMIC_PLUGIN

SQUICK_EXPORT void SquickPluginLoad(IPluginManager* pm)
{
    CREATE_PLUGIN(pm, ConsumeManagerPlugin)
};

SQUICK_EXPORT void SquickPluginUnload(IPluginManager* pm)
{
    DESTROY_PLUGIN(pm, ConsumeManagerPlugin)
};

#endif
//////////////////////////////////////////////////////////////////////////

const int ConsumeManagerPlugin::GetPluginVersion()
{
    return 0;
}

const std::string ConsumeManagerPlugin::GetPluginName()
{
	return GET_CLASS_NAME(ConsumeManagerPlugin);
}

void ConsumeManagerPlugin::Install()
{
	REGISTER_MODULE(pPluginManager, ICooldownModule, CooldownModule)
	REGISTER_MODULE(pPluginManager, IItemConsumeManagerModule, ItemConsumeManagerModule)
	REGISTER_MODULE(pPluginManager, ISkillConsumeManagerModule, SkillConsumeManagerModule)
}

void ConsumeManagerPlugin::Uninstall()
{
	UNREGISTER_MODULE(pPluginManager, ICooldownModule, CooldownModule)
	UNREGISTER_MODULE(pPluginManager, ISkillConsumeManagerModule, SkillConsumeManagerModule)
	UNREGISTER_MODULE(pPluginManager, IItemConsumeManagerModule, ItemConsumeManagerModule)
}