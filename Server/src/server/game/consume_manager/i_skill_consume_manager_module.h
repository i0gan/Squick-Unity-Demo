#pragma once

#include <iostream>
#include <squick/core/i_module.h>

class ISkillConsumeProcessModule
		: public IModule
{
public:

	virtual int ConsumeLegal(const Guid& self, const std::string& skillID, const DataList& other) = 0;
	virtual int ConsumeProcess(const Guid& self, const std::string& strSkillName, const int64_t index, const DataList& other, DataList& damageListValue, DataList& damageResultList) = 0;
};

class ISkillConsumeManagerModule
    : public IModule
{
public:
	virtual bool SetConsumeModule(const int skillType, ISkillConsumeProcessModule* pModule) = 0;
	virtual bool SetConsumeModule(const int skillType, const int skillSubType, ISkillConsumeProcessModule* pModule) = 0;

	virtual ISkillConsumeProcessModule* GetConsumeModule(const int skillType) = 0;
	virtual ISkillConsumeProcessModule* GetConsumeModule(const int skillType, const int skillSubType) = 0;

};
