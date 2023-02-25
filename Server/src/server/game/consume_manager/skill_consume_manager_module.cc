

#include "skill_consume_manager_module.h"

bool SkillConsumeManagerModule::Start()
{

    return true;
}

bool SkillConsumeManagerModule::Destory()
{

    return true;
}

bool SkillConsumeManagerModule::Update()
{

    return true;
}

bool SkillConsumeManagerModule::AfterStart()
{

    return true;
}

bool SkillConsumeManagerModule::SetConsumeModule(const int skillType, ISkillConsumeProcessModule* pModule)
{
	mSkillConsumeProcess[Guid(skillType, 0)] = pModule;
    return true;
}

ISkillConsumeProcessModule* SkillConsumeManagerModule::GetConsumeModule(const int skillType)
{
	auto it = mSkillConsumeProcess.find(Guid(skillType, 0));
	if (it != mSkillConsumeProcess.end())
	{
		return it->second;
	}

	return nullptr;
}

bool SkillConsumeManagerModule::SetConsumeModule(const int skillType, const int skillSubType, ISkillConsumeProcessModule *pModule)
{
	mSkillConsumeProcess[Guid(skillType, skillSubType)] = pModule;
	return false;
}

ISkillConsumeProcessModule *SkillConsumeManagerModule::GetConsumeModule(const int skillType, const int skillSubType)
{
	auto it = mSkillConsumeProcess.find(Guid(skillType, skillSubType));
	if (it != mSkillConsumeProcess.end())
	{
		return it->second;
	}
	else
	{
		it = mSkillConsumeProcess.find(Guid(skillType, 0));
		if (it != mSkillConsumeProcess.end())
		{
			return it->second;
		}
	}

	return nullptr;
}
