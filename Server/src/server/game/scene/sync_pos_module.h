


#ifndef SQUICK_SYNC_MODULE_H
#define SQUICK_SYNC_MODULE_H

#include <squick/core/i_plugin_manager.h>
#include <squick/plugin/kernel/i_schedule_module.h>
#include <squick/plugin/config/i_class_module.h>
#include <squick/plugin/config/i_element_module.h>
#include <squick/plugin/kernel/i_kernel_module.h>

#include <squick/plugin/kernel/i_scene_module.h>
#include <squick/plugin/log/i_log_module.h>

#include "i_sync_pos_module.h"
#include "../server/i_server_module.h"
class SyncPosModule
    : public ISyncPosModule
{
public:
	SyncPosModule(IPluginManager* p)
	{
		pPluginManager = p;
		m_bIsUpdate = true;
	}

	virtual ~SyncPosModule() {};

    virtual bool Start();
    virtual bool Destory();
    virtual bool Update();
    virtual bool AfterStart();

    virtual bool RequireMove(const Guid scene_group, const PosSyncUnit& syncUnit) override;

protected:

	int OnNPCClassEvent(const Guid& self, const std::string& className, const CLASS_OBJECT_EVENT classEvent, const DataList& var);
	int OnNPCGMPositionEvent(const Guid& self, const std::string& propertyName, const SquickData& oldVar, const SquickData& newVar);

	int OnPlayerClassEvent(const Guid& self, const std::string& className, const CLASS_OBJECT_EVENT classEvent, const DataList& var);
	int OnPlayerGMPositionEvent(const Guid& self, const std::string& propertyName, const SquickData& oldVar, const SquickData& newVar, const INT64 reason);

private:

private:
	IScheduleModule* m_pScheduleModule;
	INetModule* m_pNetModule;
	IClassModule* m_pClassModule;
	ILogModule* m_pLogModule;
	IKernelModule* m_pKernelModule;
	IElementModule* m_pElementModule;
	ISceneModule* m_pSceneModule;
	IGameServerNet_ServerModule* m_pGameServerNet_ServerModule;
};


#endif
