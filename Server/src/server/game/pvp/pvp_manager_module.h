// Author: i0gan
// Email : l418894113@gmail.com
// Date  : 2023-01-04
// Description: PVP 游戏状态模块，用于管理游戏开始，游戏结束，游戏结算等。

#pragma once
#include <squick/struct/struct.h>
#include <squick/plugin/kernel/export.h>
#include <squick/plugin/config/export.h>
#include <squick/plugin/net/export.h>
#include <squick/plugin/lua/export.h>
#include <squick/plugin/log/export.h>
#include <squick/core/base.h>
#include "../server/i_server_module.h"
#include "i_pvp_manager_module.h"

namespace game::pvp {

class PvpManagerModule 
	: public IPvpManagerModule
{
public:
	PvpManagerModule(IPluginManager* p)
	{
		pPluginManager = p;
        m_bIsUpdate = true;
	}

	virtual bool Start();
	virtual bool Destory();

	virtual bool AfterStart();
	virtual bool Update();
	virtual void PvpInstanceCreate(const string& instance_id, const string& key);

protected:
	
private:
	IKernelModule* m_pKernelModule;
	INetModule* m_pNetModule;
	IClassModule* m_pClassModule;
	IElementModule* m_pElementModule;
	ILuaScriptModule* m_pLuaScriptModule;
	ILogModule* m_pLogModule;
    INetClientModule* m_pNetClientModule;
	IGameServerNet_ServerModule* m_pGameServerNet_ServerModule;
};

}
