#pragma once

#include <iostream>
#include <squick/core/i_module.h>

class IItemConsumeProcessModule
		: public IModule
{
public:

	// > 0, error code
	virtual int ConsumeLegal(const Guid& self, const std::string& strItemID, const DataList& targetID, const Vector3& vector) = 0;

	//> 0, error code
	virtual int ConsumeProcess(const Guid& self, const std::string& strItemID, const DataList& targetID, const Vector3& vector) = 0;

};

class IItemConsumeManagerModule
    : public IModule
{
public:
	virtual bool SetConsumeModule(const int itemType, IItemConsumeProcessModule* pModule) = 0;
    virtual bool SetConsumeModule(const int itemType, const int itemSubType, IItemConsumeProcessModule* pModule) = 0;

    virtual IItemConsumeProcessModule* GetConsumeModule(const int itemType) = 0;
	virtual IItemConsumeProcessModule* GetConsumeModule(const int itemType, const int itemSubType) = 0;
};

