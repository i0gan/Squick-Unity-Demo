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


#include "../client/i_world_module.h"
#include "../server/i_server_module.h"
#include "i_scene_auto_broadcast_module.h"
#include "i_scene_process_module.h"
////////////////////////////////////////////////////////////////////////////

class SceneAutoBroadcastModule
    : public ISceneAutoBroadcastModule
{
public:
	SceneAutoBroadcastModule(IPluginManager* p)
    {
        pPluginManager = p;
    }
    virtual bool Start();
    virtual bool Destory();
    virtual bool Update();
    virtual bool AfterStart();

private:
	int ClearProperty(const Guid& self, const int sceneID, const int groupID);
	int ClearRecord(const Guid& self, const int sceneID, const int groupID);

	int OnPropertyEvent(const Guid& self, const std::string& propertyName, const SquickData& oldVar, const SquickData& newVar);
	int OnRecordEvent(const Guid& self, const RECORD_EVENT_DATA& eventData, const SquickData& oldVar, const SquickData& newVar);

	int OnBeforeLeaveSceneEvent(const Guid & self, const int sceneID, const int groupID, const int type, const DataList& argList);
	int OnAfterEntrySceneEvent(const Guid & self, const int sceneID, const int groupID, const int type, const DataList& argList);
	
	//broad the data of self to argvar 
	int OnPropertyEnter(const DataList& argVar, const int sceneID, const int groupID);
	int OnRecordEnter(const DataList& argVar, const int sceneID, const int groupID);

private:

    IKernelModule* m_pKernelModule;
    IClassModule* m_pClassModule;
    ILogModule* m_pLogModule;
    ISceneProcessModule* m_pSceneProcessModule;
    IElementModule* m_pElementModule;
	INetModule* m_pNetModule;
	IEventModule* m_pEventModule;
	ISceneModule* m_pSceneModule;
    INetClientModule* m_pNetClientModule;
	IGameServerNet_ServerModule* m_pGameServerNet_ServerModule;
};