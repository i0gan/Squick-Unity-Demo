

#include "chat_module.h"
#include "plugin.h"

//
//

SQUICK_EXPORT void SquickPluginLoad(IPluginManager* pm)
{

    CREATE_PLUGIN(pm, ChatPlugin)

};

SQUICK_EXPORT void SquickPluginUnload(IPluginManager* pm)
{
    DESTROY_PLUGIN(pm, ChatPlugin)
};



const int ChatPlugin::GetPluginVersion()
{
    return 0;
}

const std::string ChatPlugin::GetPluginName()
{
	return GET_CLASS_NAME(ChatPlugin);
}

void ChatPlugin::Install()
{
    REGISTER_MODULE(pPluginManager, IChatModule, ChatModule)

}

void ChatPlugin::Uninstall()
{
    UNREGISTER_MODULE(pPluginManager, IChatModule, ChatModule)
}