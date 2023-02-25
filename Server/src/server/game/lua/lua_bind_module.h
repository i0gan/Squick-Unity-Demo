#pragma once

#include <map>
#include <iostream>

#include <squick/struct/struct.h>
#include <squick/core/platform.h>
#include <squick/plugin/kernel/export.h>
#include <squick/plugin/config/export.h>
#include <squick/plugin/net/export.h>
#include <squick/plugin/lua/export.h>

#include "../player/player_manager_module.h"
namespace game::lua {
class ILuaBindModule : public IModule
{

};

class LuaBindModule
	: public ILuaBindModule
{
public:
	LuaBindModule(IPluginManager* p)
	{
		pPluginManager = p;
        m_bIsUpdate = true;
	}

	virtual bool Start();
	virtual bool Destory();

	virtual bool AfterStart();
	virtual bool Update();

protected:
	bool OnGetServerList(SQUICK_SHARE_PTR<HttpRequest> req);
	bool Bind();

	void SendToPlayer(string& player_guid_str, uint16_t msgID, std::string& data);
	void Test(const uint16_t msgID, string& msg, int a);
private:
	ILuaScriptModule* m_pLuaScriptModule;
	player::IPlayerManagerModule* m_pPlayerManagerModule;
	IGameServerNet_ServerModule* m_pGameServerNet_ServerModule;
};

}
