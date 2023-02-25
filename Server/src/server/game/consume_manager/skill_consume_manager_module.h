#pragma once

#include <iostream>
#include "squick/core/map_ex.h"

#include "i_skill_consume_manager_module.h"

class SkillConsumeManagerModule
    : public ISkillConsumeManagerModule
{
public:
    SkillConsumeManagerModule( IPluginManager* p )
    {
        pPluginManager = p;
    }
    virtual bool Start() override;
    virtual bool Destory() override;
    virtual bool Update() override;
    virtual bool AfterStart() override;

    virtual bool SetConsumeModule(const int skillType, ISkillConsumeProcessModule* pModule);
	virtual bool SetConsumeModule(const int skillType, const int skillSubType, ISkillConsumeProcessModule* pModule);

	virtual ISkillConsumeProcessModule* GetConsumeModule( const int skillType);
	virtual ISkillConsumeProcessModule* GetConsumeModule( const int skillType, const int skillSubType);

private:
	std::map<Guid, ISkillConsumeProcessModule*> mSkillConsumeProcess;
};
