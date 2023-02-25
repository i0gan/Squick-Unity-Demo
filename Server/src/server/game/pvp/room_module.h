// Author: i0gan
// Email : l418894113@gmail.com
// Date  : 2023-01-04
// Description: PVP 房间模块

#pragma once
#include <squick/struct/struct.h>
#include <squick/plugin/kernel/export.h>
#include <squick/plugin/config/export.h>
#include <squick/plugin/net/export.h>
#include <squick/plugin/lua/export.h>
#include <squick/plugin/log/export.h>
#include <squick/core/base.h>
#include "i_pvp_manager_module.h"

#include "../scene/i_scene_process_module.h"
#include "../server/i_server_module.h"
#include "../client/i_db_module.h"


#include <map>

namespace game::pvp {
class IRoomModule : public IModule
{

};

class RoomModule 
	: public IRoomModule
{
public:
	RoomModule(IPluginManager* p)
	{
		pPluginManager = p;
        m_bIsUpdate = true;
	}

	virtual bool Start();
	virtual bool Destory();

	virtual bool AfterStart();
	virtual bool Update();

protected:
	void OnReqRoomCreate(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len);
	void OnReqRoomJoin(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len);
	void OnReqRoomList(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len);
	void OnReqRoomDetails(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len);
	void OnReqRoomQuit(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len);
	void OnReqRoomPlayerEvent(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len);

	void OnReqStartPvpGame(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len);
	void OnReqJoinPvpGame(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len);
	void OnReqQuitPvpGame(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len);

	void OnReqPvpGameInit(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len);
	std::map<int, SQUICK_SHARE_PTR<SquickStruct::RoomDetails>> m_rooms;
private:
	IKernelModule* m_pKernelModule;
	INetModule* m_pNetModule;
	IClassModule* m_pClassModule;
	IElementModule* m_pElementModule;
	ILuaScriptModule* m_pLuaScriptModule;
	ILogModule* m_pLogModule;
	IPvpManagerModule* m_pPvpManagerModule;

	ISceneModule* m_pSceneModule;
	IGameServerNet_ServerModule* m_pGameServerNet_ServerModule;
	ISceneProcessModule* m_pSceneProcessModule;
	INetClientModule* m_pNetClientModule;
	IScheduleModule* m_pScheduleModule;
	IDataTailModule* m_pDataTailModule;
	IEventModule* m_pEventModule;
};

}
