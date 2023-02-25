#pragma once

#include <memory>
#include <squick/struct/struct.h>
#include <squick/plugin/net/i_net_module.h>
#include <squick/plugin/net/i_net_client_module.h>

#include <squick/plugin/log/i_log_module.h>
#include <squick/plugin/kernel/i_kernel_module.h>
#include <squick/plugin/kernel/i_event_module.h>
#include <squick/plugin/kernel/i_scene_module.h>
#include <squick/plugin/config/i_class_module.h>
#include <squick/plugin/config/i_element_module.h>
#include <squick/plugin/kernel/i_schedule_module.h>
#include <squick/plugin/kernel/i_thread_pool_module.h>

#include "i_game_manager_module.h"
#include "../client/i_world_module.h"
#include "../scene/i_scene_process_module.h"
#include "i_server_module.h"

class GameManagerModule
		: public IGameManagerModule
{
public:
	GameManagerModule(IPluginManager* p)
	{
		pPluginManager = p;
	}

	virtual bool Start();
	virtual bool Destory();
	virtual bool Update();
	virtual bool AfterStart();

protected:
	void OnClientPropertyIntProcess(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len);
	void OnClientPropertyFloatProcess(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len);
	void OnClientPropertyStringProcess(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len);
	void OnClientPropertyObjectProcess(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len);
	void OnClientPropertyVector2Process(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len);
	void OnClientPropertyVector3Process(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len);

	void OnClientAddRowProcess(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len);
	void OnClientRemoveRowProcess(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len);
	void OnClientSwapRowProcess(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len);
	void OnClientRecordIntProcess(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len);
	void OnClientRecordFloatProcess(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len);
	void OnClientRecordStringProcess(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len);
	void OnClientRecordObjectProcess(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len);
	void OnClientRecordVector2Process(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len);
	void OnClientRecordVector3Process(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len);

protected:
	//get GM Level and what you want to modify
	std::shared_ptr<IProperty> CalProperty(SQUICK_SHARE_PTR<IObject> pObject, const Guid objectID, const std::string& propertyName, int& gmLevel);
	std::shared_ptr<IRecord> CalRecord(SQUICK_SHARE_PTR<IObject> pObject, const Guid objectID, const std::string& recordName, int& gmLevel);

private:

	//////////////////////////////////////////////////////////////////////////
	IKernelModule* m_pKernelModule;
	IClassModule* m_pClassModule;
	ILogModule* m_pLogModule;
	ISceneProcessModule* m_pSceneProcessModule;
	IElementModule* m_pElementModule;
	INetModule* m_pNetModule;
	IEventModule* m_pEventModule;
	ISceneModule* m_pSceneModule;
	INetClientModule* m_pNetClientModule;
	IScheduleModule* m_pScheduleModule;
	IGameServerNet_ServerModule* m_pGameServerNet_ServerModule;
};
