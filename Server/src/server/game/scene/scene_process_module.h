
#ifndef SQUICK_SCENEPROCESS_MODULE_H
#define SQUICK_SCENEPROCESS_MODULE_H

#include <string>
#include <map>
#include <iostream>
#include "squick/core/map.h"
#include "squick/core/list.h"
#include "third_party/rapidxml/rapidxml.hpp"
#include "third_party/rapidxml/rapidxml_iterators.hpp"
#include "third_party/rapidxml/rapidxml_print.hpp"
#include "third_party/rapidxml/rapidxml_utils.hpp"
#include "squick/struct/protocol_define.h"

#include <squick/plugin/config/i_class_module.h>
#include <squick/plugin/config/i_element_module.h>
#include <squick/plugin/log/i_log_module.h>

#include <squick/core/i_plugin_manager.h>
#include <squick/plugin/kernel/i_event_module.h>
#include <squick/plugin/kernel/i_scene_module.h>
#include <squick/plugin/kernel/i_cell_module.h>
#include <squick/plugin/kernel/i_kernel_module.h>

//#include "i_property_module.h"
#include "i_scene_process_module.h"
#include "../server/i_server_module.h"

class SceneProcessModule
    : public ISceneProcessModule
{
public:
    SceneProcessModule(IPluginManager* p)
    {
        pPluginManager = p;
    }
    virtual ~SceneProcessModule() {};

    virtual bool Start();
    virtual bool Destory();
    virtual bool Update();
    virtual bool AfterStart();
	virtual bool ReadyUpdate();

	virtual bool RequestEnterScene(const Guid& self, const int sceneID, const int groupID, const int type, const Vector3& pos, const DataList& argList);

protected:
	bool LoadSceneResource(const std::string& strSceneIDName);
	bool CreateSceneBaseGroup(const std::string& strSceneIDName);

protected:

    int OnObjectClassEvent(const Guid& self, const std::string& className, const CLASS_OBJECT_EVENT classEvent, const DataList& var);
	
	int EnterSceneConditionEvent(const Guid& self, const int sceneID, const int groupID, const int type, const DataList& argList);
	
	int BeforeEnterSceneGroupEvent(const Guid& self, const int sceneID, const int groupID, const int type, const DataList& argList);
	int AfterEnterSceneGroupEvent(const Guid& self, const int sceneID, const int groupID, const int type, const DataList& argList);
	int BeforeLeaveSceneGroupEvent(const Guid& self, const int sceneID, const int groupID, const int type, const DataList& argList);
	int AfterLeaveSceneGroupEvent(const Guid& self, const int sceneID, const int groupID, const int type, const DataList& argList);

private:

    IElementModule* m_pElementModule;
    IClassModule* m_pClassModule;
    IKernelModule* m_pKernelModule;
    ILogModule* m_pLogModule;
	IEventModule* m_pEventModule;
	ISceneModule* m_pSceneModule;
	ICellModule* m_pCellModule;
    IGameServerNet_ServerModule* m_pGameServerNet_ServerModule;
};

#endif
