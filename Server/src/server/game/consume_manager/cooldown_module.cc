
#include "cooldown_module.h"
#include <squick/struct/protocol_define.h>

bool CooldownModule::AfterStart()
{
    m_pKernelModule = pPluginManager->FindModule<IKernelModule>();
    m_pElementModule = pPluginManager->FindModule<IElementModule>();

    return true;
}

void CooldownModule::AddCooldown(const Guid& self, const std::string& configID )
{
    //skillCnfID, usedTime
    SQUICK_SHARE_PTR<IRecord> xRecord = m_pKernelModule->FindRecord(self, SquickProtocol::NPC::Cooldown::ThisName());
    const int row = xRecord->FindString(SquickProtocol::NPC::Cooldown::ConfigID, configID);
    if (row >= 0)
    {
        //reset the time
        xRecord->SetInt(row, SquickProtocol::NPC::Cooldown::ConfigID, SquickGetTimeMS());
    }
    else
    {
        SQUICK_SHARE_PTR<DataList> xDataList = xRecord->GetStartData();
        xDataList->SetString(SquickProtocol::NPC::Cooldown::ConfigID, configID);
        xDataList->SetInt(SquickProtocol::NPC::Cooldown::Time, SquickGetTimeMS());

        xRecord->AddRow(row, *xDataList);
    }

	//for common skill cd, if you dont have a common skill CD, the monster will use multiple skills in one sec when meet players
	const int nRowCommon = xRecord->FindString(SquickProtocol::NPC::Cooldown::ConfigID, SquickProtocol::NPC::Cooldown::ThisName());
	if (nRowCommon >= 0)
	{
		//reset the time
		xRecord->SetInt(nRowCommon, SquickProtocol::NPC::Cooldown::ConfigID, SquickGetTimeMS());
	}
	else
	{
		SQUICK_SHARE_PTR<DataList> xDataList = xRecord->GetStartData();
		xDataList->SetString(SquickProtocol::NPC::Cooldown::ConfigID, SquickProtocol::NPC::Cooldown::ThisName());
		xDataList->SetInt(SquickProtocol::NPC::Cooldown::Time, SquickGetTimeMS());

		xRecord->AddRow(nRowCommon, *xDataList);
	}
}

bool CooldownModule::ExistCooldown(const Guid& self, const std::string& configID )
{
	SQUICK_SHARE_PTR<IRecord> xRecord = m_pKernelModule->FindRecord(self, SquickProtocol::NPC::Cooldown::ThisName());

	//for common skill if you dont have a common skill CD, the monster will use multiple skills in one sec when meet the players
	const int nRowCommon = xRecord->FindString(SquickProtocol::NPC::Cooldown::ConfigID, SquickProtocol::NPC::Cooldown::ThisName());
	if (nRowCommon >= 0)
	{
		float fCDTime = 1.0f;
		int64_t nLastTime = xRecord->GetInt(nRowCommon, SquickProtocol::NPC::Cooldown::Time);
		int64_t nNowTime = SquickGetTimeMS();
		if ((nNowTime - nLastTime) < fCDTime * 1000)
		{
			return true;
		}
	}

    const int row = xRecord->FindString(SquickProtocol::NPC::Cooldown::ConfigID, configID);
    if (row >= 0)
    {
        //compare the time with the cooldown time
        double fCDTime = m_pElementModule->GetPropertyFloat(configID, SquickProtocol::Skill::CoolDownTime());
        int64_t nLastTime = xRecord->GetInt(row, SquickProtocol::NPC::Cooldown::Time);
        if ((SquickGetTimeMS() - nLastTime) < fCDTime * 1000)
        {
            return true;
        }
    }

    return false;
}
