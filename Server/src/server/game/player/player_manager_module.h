#pragma once

#include <squick/core/consistent_hash.h>
#include <squick/plugin/no_sql/export.h>
#include <squick/plugin/net/export.h>
#include <squick/plugin/kernel/export.h>
#include <squick/plugin/config/export.h>
#include <squick/core/base.h>

#include "../scene/i_scene_process_module.h"
#include "../server/i_server_module.h"
#include "../client/i_db_module.h"

#include "player.h"

#include <unordered_map>

namespace game::player {

class IPlayerManagerModule : public IModule
{
public:
	virtual void SetDefaultSceneID(const int sceneID) = 0;
};

class PlayerManagerModule
    : public IPlayerManagerModule
{
public:
    PlayerManagerModule(IPluginManager* p)
    {
        pPluginManager = p;
    }
    virtual ~PlayerManagerModule() {};

    virtual bool Start();
    virtual bool Destory();
	virtual bool ReadyUpdate();
    virtual bool Update();
    virtual bool AfterStart();

	virtual void SetDefaultSceneID(const int sceneID);
	// 发送数据到客户端
	virtual void OnSendToClient(const uint16_t msgID, google::protobuf::Message& xMsg, const Guid& client_id);

protected:
	void OnRequireRoleListProcess(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len);
	void OnResponseRoleListProcess(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len);
	
	void OnCreateRoleGameProcess(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len);
	void OnDeleteRoleGameProcess(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len);
	void OnClientEnterGameProcess(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len);
	
	void OnDBLoadRoleDataProcess(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len);
	int OnObjectPlayerEvent(const Guid & self, const std::string & className, const CLASS_OBJECT_EVENT classEvent, const DataList & var);

	
private:

	void LoadDataFromDb(const Guid& self);
	void SaveDataToDb(const Guid& self);

	int SaveDataOnTime(const Guid& self, const std::string& name, const float fIntervalTime, const int count);
	
private:

	int defaultSceneID = 1;
	// 待优化为 unordered_map , 查找时间复杂度为 O(1)
	std::map<Guid, SquickStruct::RoleDataPack> mxObjectDataCache;
	std::map<Guid, SQUICK_SHARE_PTR<Player>> m_players; // 所有玩家

private:
	INetModule* m_pNetModule;
	IClassModule* m_pClassModule;
	IElementModule* m_pElementModule;
	IKernelModule* m_pKernelModule;
	ISceneModule* m_pSceneModule;
	IGameServerNet_ServerModule* m_pGameServerNet_ServerModule;
	IGameServerToDBModule* m_pGameToDBModule;
	ISceneProcessModule* m_pSceneProcessModule;
	INetClientModule* m_pNetClientModule;
	IScheduleModule* m_pScheduleModule;
	IDataTailModule* m_pDataTailModule;
	IEventModule* m_pEventModule;
};

}