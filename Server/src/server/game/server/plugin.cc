#include "server_module.h"
#include "game_manager_module.h"
#include "plugin.h"
//
//


SQUICK_EXPORT void SquickPluginLoad(IPluginManager* pm)
{

    CREATE_PLUGIN(pm, GameServerNet_ServerPlugin)

};

SQUICK_EXPORT void SquickPluginUnload(IPluginManager* pm)
{
    DESTROY_PLUGIN(pm, GameServerNet_ServerPlugin)
};


//////////////////////////////////////////////////////////////////////////

const int GameServerNet_ServerPlugin::GetPluginVersion()
{
    return 0;
}

const std::string GameServerNet_ServerPlugin::GetPluginName()
{
	return GET_CLASS_NAME(GameServerNet_ServerPlugin);
}

void GameServerNet_ServerPlugin::Install()
{
	REGISTER_MODULE(pPluginManager, IGameManagerModule, GameManagerModule)
	REGISTER_MODULE(pPluginManager, IGameServerNet_ServerModule, GameServerNet_ServerModule)
}

void GameServerNet_ServerPlugin::Uninstall()
{
	UNREGISTER_MODULE(pPluginManager, IGameServerNet_ServerModule, GameServerNet_ServerModule)
	UNREGISTER_MODULE(pPluginManager, IGameManagerModule, GameManagerModule)
}