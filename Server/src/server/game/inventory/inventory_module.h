#pragma once

#include <squick/plugin/kernel/i_kernel_module.h>
#include <squick/plugin/config/i_element_module.h>
#include <squick/plugin/log/i_log_module.h>
#include <squick/core/i_plugin_manager.h>


#include <server/game/scene/i_property_module.h>
#include <server/game/scene/i_scene_process_module.h>

#include "i_inventory_module.h"

class InventoryModule
    : public IInventoryModule
{
public:

	InventoryModule(IPluginManager* p )
    {
        pPluginManager = p;
    }
    virtual ~InventoryModule() {};

    virtual bool Start() override;
    virtual bool Destory() override;
    virtual bool Update() override;
    virtual bool AfterStart() override;

	///////////
	virtual Guid CreateEquip( const Guid& self, const std::string& configName, const int count = 1);
	virtual bool CreateItem(const Guid& self, const std::string& configName, const int count);

	virtual int ItemCount(const Guid& self, const std::string& strItemConfigID);

	virtual bool DeleteEquip(const Guid& self, const Guid& id);
    virtual bool DeleteItem(const Guid& self, const std::string& strItemConfigID, const int count);
    virtual bool EnoughItem(const Guid& self, const std::string& strItemConfigID, const int count);

protected:
	bool CreateItemInNormalBag(const Guid& self, const std::string& configName, const int count);

private:
    IKernelModule* m_pKernelModule;
    ILogModule* m_pLogModule;
    IElementModule* m_pElementModule;
    ISceneProcessModule* m_pSceneProcessModule;
    IPropertyModule* m_pPropertyModule;
};

