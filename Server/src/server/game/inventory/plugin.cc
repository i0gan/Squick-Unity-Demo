


#include "plugin.h"
#include "inventory_module.h"


SQUICK_EXPORT void SquickPluginLoad(IPluginManager* pm)
{
    CREATE_PLUGIN(pm, InventoryPlugin)
};

SQUICK_EXPORT void SquickPluginUnload(IPluginManager* pm)
{
    DESTROY_PLUGIN(pm, InventoryPlugin)
};




const int InventoryPlugin::GetPluginVersion()
{
    return 0;
}

const std::string InventoryPlugin::GetPluginName()
{
	return GET_CLASS_NAME(InventoryPlugin);
}

void InventoryPlugin::Install()
{
	REGISTER_MODULE(pPluginManager, IInventoryModule, InventoryModule)
}

void InventoryPlugin::Uninstall()
{

	UNREGISTER_MODULE(pPluginManager, IInventoryModule, InventoryModule)
}