

#include "plugin.h"
#include "game_server_module.h"



#ifdef SQUICK_DYNAMIC_PLUGIN

SQUICK_EXPORT void SquickPluginLoad(IPluginManager* pm)
{
    CREATE_PLUGIN(pm, GameServerPlugin)
};

SQUICK_EXPORT void SquickPluginUnload(IPluginManager* pm)
{
    DESTROY_PLUGIN(pm, GameServerPlugin)
};

#endif
//////////////////////////////////////////////////////////////////////////

const int GameServerPlugin::GetPluginVersion()
{
    return 0;
}

const std::string GameServerPlugin::GetPluginName()
{
	return GET_CLASS_NAME(GameServerPlugin);
}

void GameServerPlugin::Install()
{

    REGISTER_MODULE(pPluginManager, IGameServerModule, GameServerModule)
    
}

void GameServerPlugin::Uninstall()
{
	
	
	
	
    
    UNREGISTER_MODULE(pPluginManager, IGameServerModule, GameServerModule)




    
}
