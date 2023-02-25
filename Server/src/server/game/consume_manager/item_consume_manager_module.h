#pragma once

#include <iostream>
#include <squick/core/i_plugin_manager.h>

#include "i_item_consume_manager_module.h"
class ItemConsumeManagerModule
    : public IItemConsumeManagerModule
{
public:

    ItemConsumeManagerModule( IPluginManager* p )
    {
        pPluginManager = p;
    }
    virtual bool Start() override;
    virtual bool Destory() override;
    virtual bool Update() override;
    virtual bool AfterStart() override;

	virtual bool SetConsumeModule(const int itemType, IItemConsumeProcessModule* pModule);
	virtual bool SetConsumeModule(const int itemType, const int itemSubType, IItemConsumeProcessModule* pModule);

	virtual IItemConsumeProcessModule* GetConsumeModule(const int itemType);
	virtual IItemConsumeProcessModule* GetConsumeModule(const int itemType, const int itemSubType);

private:

    std::map<Guid, IItemConsumeProcessModule*> mItemConsumeModule;
};

