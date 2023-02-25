#pragma once

#include <iostream>
#include <squick/core/i_module.h>

class IInventoryModule : public IModule
{
public:
    virtual Guid CreateEquip(const Guid& self, const std::string& configName, const int count = 1) = 0;
    virtual bool CreateItem(const Guid& self, const std::string& configName, const int count) = 0;
	virtual int ItemCount(const Guid& self, const std::string& strItemConfigID) = 0;

    virtual bool DeleteEquip(const Guid& self, const Guid& id) = 0;
	virtual bool DeleteItem(const Guid& self, const std::string& strItemConfigID, const int count) = 0;
    virtual bool EnoughItem(const Guid& self, const std::string& strItemConfigID, const int count) = 0;
};
