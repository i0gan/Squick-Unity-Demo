#pragma once
#include <squick/plugin/kernel/i_kernel_module.h>

#include <squick/plugin/config/i_class_module.h>
#include <squick/core/i_plugin_manager.h>
#include <squick/plugin/log/i_log_module.h>
#include <squick/plugin/config/i_common_config_module.h>
#include <squick/plugin/kernel/i_event_module.h>
#include <squick/plugin/kernel/i_schedule_module.h>
#include <squick/plugin/kernel/i_scene_module.h>
#include <squick/plugin/config/i_element_module.h>

#include "i_npc_refresh_module.h"
#include "i_property_module.h"
#include "i_scene_process_module.h"
class NPCRefreshModule
    : public INPCRefreshModule
{
public:
    NPCRefreshModule( IPluginManager* p )
    {
        pPluginManager = p;
    }
    virtual ~NPCRefreshModule() {};

    virtual bool Start();
    virtual bool Destory();
    virtual bool Update();
    virtual bool AfterStart();

protected:
    int OnObjectClassEvent( const Guid& self, const std::string& className, const CLASS_OBJECT_EVENT classEvent, const DataList& var );

    int OnObjectHPEvent( const Guid& self, const std::string& propertyName, const SquickData& oldVar, const SquickData& newVar, const INT64 reason);

	int OnNPCDeadDestroyHeart(const Guid& self, const std::string& heartBeat, const float time, const int count);
	int OnBuildingDeadDestroyHeart( const Guid& self, const std::string& heartBeat, const float time, const int count);

protected:
	int OnObjectBeKilled( const Guid& self, const Guid& killer );

private:
	IEventModule* m_pEventModule;
	IScheduleModule* m_pScheduleModule;
    IElementModule* m_pElementModule;
    IKernelModule* m_pKernelModule;
    ISceneProcessModule* m_pSceneProcessModule;
	ILogModule* m_pLogModule;
	IPropertyModule* m_pPropertyModule;
    ISceneModule* m_pSceneModule;
};
