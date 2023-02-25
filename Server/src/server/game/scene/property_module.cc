

#include "property_module.h"

bool PropertyModule::Start()
{
	m_pKernelModule = pPluginManager->FindModule<IKernelModule>();
    m_pElementModule = pPluginManager->FindModule<IElementModule>();
    m_pClassModule = pPluginManager->FindModule<IClassModule>();
    m_pPropertyConfigModule = pPluginManager->FindModule<IPropertyConfigModule>();
    m_pLogModule = pPluginManager->FindModule<ILogModule>();

    return true;
}

bool PropertyModule::Destory()
{
    return true;
}

bool PropertyModule::Update()
{
    return true;
}

bool PropertyModule::AfterStart()
{
	m_pKernelModule->AddClassCallBack(SquickProtocol::Player::ThisName(), this, &PropertyModule::OnObjectClassEvent);
	m_pKernelModule->AddClassCallBack(SquickProtocol::NPC::ThisName(), this, &PropertyModule::OnObjectClassEvent);

    return true;
}

int64_t PropertyModule::GetPropertyValue(const Guid& self, const std::string& propertyName, const PropertyGroup eGroupType)
{
    if (PropertyGroup::NPG_ALL != eGroupType)
    {
        return m_pKernelModule->GetRecordInt(self, SquickProtocol::Player::CommValue::ThisName(), eGroupType, propertyName);
    }

    return 0;
}

int PropertyModule::SetPropertyValue(const Guid& self, const std::string& propertyName, const PropertyGroup eGroupType, const int64_t nValue)
{
    if (PropertyGroup::NPG_ALL != eGroupType)
    {
        SQUICK_SHARE_PTR<IObject> pObject = m_pKernelModule->GetObject(self);
        if (pObject)
        {
            SQUICK_SHARE_PTR<IRecord> pRecord = m_pKernelModule->FindRecord(self, SquickProtocol::Player::CommValue::ThisName());
            if (pRecord)
            {
                pRecord->SetUsed(eGroupType, true);
                return pRecord->SetInt(eGroupType, propertyName, nValue);
            }
        }
    }

    return 0;
}

int PropertyModule::OnObjectLevelEvent(const Guid& self, const std::string& propertyName, const SquickData& oldVar, const SquickData& newVar, const INT64 reason)
{
    const int job = m_pKernelModule->GetPropertyInt32(self, SquickProtocol::Player::Job());
    const int level = m_pKernelModule->GetPropertyInt32(self, SquickProtocol::Player::Level());
    const std::string& initPropertyID = m_pPropertyConfigModule->GetInitPropertyID(job, level);
    const std::string& configID = m_pElementModule->GetPropertyString(initPropertyID, SquickProtocol::InitProperty::HeroConfigID());

    if (configID.empty() || !m_pElementModule->ExistElement(configID))
    {
        m_pLogModule->LogError(self, configID + " configID not exist!!!", __FUNCTION__, __LINE__);
        return 1;
    }
    //normally, we modify the config id by hero module, so we don't need to modify the config id by job and level
    //but if you don't have a hero system, you could active this code
    if (!activeExtraController)
	{
		m_pKernelModule->SetPropertyString(self, SquickProtocol::Player::ConfigID(), configID);
	}

    RefreshBaseProperty(self);

    FullHPMP(self);
    FullSP(self);

    return 0;
}

int PropertyModule::OnObjectMAXHPEvent(const Guid& self, const std::string& propertyName, const SquickData& oldVar, const SquickData& newVar, const INT64 reason)
{
	const int hp = m_pKernelModule->GetPropertyInt32(self, SquickProtocol::Player::HP());
	if (hp > newVar.GetInt())
	{
		m_pKernelModule->SetPropertyInt(self, SquickProtocol::Player::HP(), newVar.GetInt());
	}

	return 0;
}

int PropertyModule::OnObjectConfigIDEvent(const Guid& self, const std::string& propertyName, const SquickData& oldVar, const SquickData& newVar, const INT64 reason)
{
	//for appearance
	return 0;
}

int PropertyModule::OnRecordEvent(const Guid& self, const RECORD_EVENT_DATA& eventData, const SquickData& oldVar, const SquickData& newVar)
{
	const std::string& recordName = eventData.recordName;
    const int nOpType = eventData.nOpType;
    const int row = eventData.row;
    const int col = eventData.col;

    int nAllValue = 0;
    SQUICK_SHARE_PTR<IRecord> pRecord = m_pKernelModule->FindRecord(self, SquickProtocol::Player::CommValue::ThisName());
    for (int i = 0; i < (int)(PropertyGroup::NPG_ALL); i++)
    {
		if (activeExtraController && i == PropertyGroup::NPG_JOB_LEVEL)
		{
			continue;
		}

		if (i < pRecord->GetRows())
        {
            int nValue = pRecord->GetInt32(i, col);
            nAllValue += nValue;
        }
    }

    m_pKernelModule->SetPropertyInt(self, pRecord->GetColTag(col), nAllValue);

    return 0;
}

int PropertyModule::OnObjectClassEvent(const Guid& self, const std::string& className, const CLASS_OBJECT_EVENT classEvent, const DataList& var)
{
    dout << "加载属性\n";
    if (className == SquickProtocol::Player::ThisName()) {
		if (CLASS_OBJECT_EVENT::COE_CREATE_NODATA == classEvent)
		{
			SQUICK_SHARE_PTR<IRecord> pRecord = m_pKernelModule->FindRecord(self, SquickProtocol::Player::CommValue::ThisName());
			if (pRecord)
			{
				for (int i = 0; i < PropertyGroup::NPG_ALL; i++)
				{
					pRecord->AddRow(-1);
				}
			}

		}
		else if (CLASS_OBJECT_EVENT::COE_CREATE_BEFORE_ATTACHDATA == classEvent)
        {
           //cant attach the level event here as we will reset the property configID and Level by sequence
           //as a result, the level event will be triggered first, then configID event triggered late, or the trigger sequence in reverse
           //that means if we added attach the level event here, we cant get the correct result
        }
		else if (CLASS_OBJECT_EVENT::COE_CREATE_LOADDATA == classEvent)
        {
           

        }
		else if (CLASS_OBJECT_EVENT::COE_CREATE_AFTER_ATTACHDATA == classEvent)
        {
            int onlineCount = m_pKernelModule->GetPropertyInt32(self, SquickProtocol::Player::OnlineCount());
            m_pKernelModule->SetPropertyInt(self, SquickProtocol::Player::OnlineCount(), (onlineCount + 1));

            if (onlineCount <= 0 && m_pKernelModule->GetPropertyInt32(self, SquickProtocol::Player::SceneID()) > 0)
            {
                //first time online
                // 第一次登录，从等级1开始
                m_pKernelModule->SetPropertyInt(self, SquickProtocol::Player::Level(), 1);
				OnObjectLevelEvent(self, SquickProtocol::Player::Level(), 1, 1, 0);
            }
            else
            {
                int level = m_pKernelModule->GetPropertyInt32(self, SquickProtocol::Player::Level());
                OnObjectLevelEvent(self, SquickProtocol::Player::Level(), level, level, 0);
            }
		}
        else if (CLASS_OBJECT_EVENT::COE_CREATE_AFTER_EFFECT == classEvent)
        {
        }
        else if (CLASS_OBJECT_EVENT::COE_CREATE_READY == classEvent)
        {
			RefreshAllProperty(self);
			FullHPMP(self);

	        m_pKernelModule->AddPropertyCallBack(self, SquickProtocol::Player::Level(), this, &PropertyModule::OnObjectLevelEvent);
	        m_pKernelModule->AddPropertyCallBack(self, SquickProtocol::Player::MAXHP(), this, &PropertyModule::OnObjectMAXHPEvent);
			m_pKernelModule->AddPropertyCallBack(self, SquickProtocol::Player::ConfigID(), this, &PropertyModule::OnObjectConfigIDEvent);
			m_pKernelModule->AddRecordCallBack(self, SquickProtocol::Player::CommValue::ThisName(), this, &PropertyModule::OnRecordEvent);
        }
		else if (CLASS_OBJECT_EVENT::COE_CREATE_HASDATA == classEvent)
		{
		}
        else if (CLASS_OBJECT_EVENT::COE_CREATE_FINISH == classEvent)
        {
        }
    }
    else
    {
	    if (CLASS_OBJECT_EVENT::COE_CREATE_FINISH == classEvent)
	    {
		    m_pKernelModule->AddPropertyCallBack(self, SquickProtocol::Player::MAXHP(), this, &PropertyModule::OnObjectMAXHPEvent);
	    }
    }

    return 0;
}

void PropertyModule::RefreshBaseProperty(const Guid& self)
{
	const int job = m_pKernelModule->GetPropertyInt32(self, SquickProtocol::Player::Job());
	const int level = m_pKernelModule->GetPropertyInt32(self, SquickProtocol::Player::Level());
	const std::string& initPropertyID = m_pPropertyConfigModule->GetInitPropertyID(job, level);
	const std::string& configID = m_pElementModule->GetPropertyString(initPropertyID, SquickProtocol::InitProperty::HeroConfigID());

    const std::string& effectData = m_pElementModule->GetPropertyString(configID, SquickProtocol::NPC::EffectData());
    if (effectData.empty() || !m_pElementModule->ExistElement(effectData))
    {
        m_pLogModule->LogError(self, effectData + " effectData not exist!!!", __FUNCTION__, __LINE__);
        return;
    }

    SQUICK_SHARE_PTR<IRecord> pRecord = m_pKernelModule->FindRecord(self, SquickProtocol::Player::CommValue::ThisName());
    if (!pRecord)
    {
        return;
    }

    for (int i = 0; i < pRecord->GetCols(); ++i)
    {
        const std::string& colTag = pRecord->GetColTag(i);
        INT64 nValue = m_pElementModule->GetPropertyInt(effectData, colTag);

		pRecord->SetUsed(PropertyGroup::NPG_JOB_LEVEL, true);
		pRecord->SetInt(PropertyGroup::NPG_JOB_LEVEL, colTag, nValue);
    }
}

void PropertyModule::RefreshAllProperty(const Guid& self)
{
    SQUICK_SHARE_PTR<IRecord> pRecord = m_pKernelModule->FindRecord(self, SquickProtocol::Player::CommValue::ThisName());
    for (int col = 0; col < pRecord->GetCols(); col++)
    {
        int64_t nAllValue = 0;

        for (int i = 0; i < (int)(PropertyGroup::NPG_ALL); i++)
        {
        	if (activeExtraController && i == PropertyGroup::NPG_JOB_LEVEL)
			{
				continue;
			}

            if (i < pRecord->GetRows())
            {
                int64_t nValue = pRecord->GetInt(i, col);
                nAllValue += nValue;
            }
        }

        m_pKernelModule->SetPropertyInt(self, pRecord->GetColTag(col), nAllValue);
    }
}

bool PropertyModule::AddExp(const Guid& self, const int64_t exp)
{
    int eJobType = m_pKernelModule->GetPropertyInt32(self, SquickProtocol::Player::Job());
    int64_t nCurExp = m_pKernelModule->GetPropertyInt(self, SquickProtocol::Player::EXP());
    int nLevel = m_pKernelModule->GetPropertyInt32(self, SquickProtocol::Player::Level());
    const std::string& heroConfigID = m_pElementModule->GetPropertyString(m_pPropertyConfigModule->GetInitPropertyID(eJobType, nLevel), SquickProtocol::InitProperty::HeroConfigID());
    int64_t nMaxExp = m_pElementModule->GetPropertyInt(heroConfigID, SquickProtocol::Player::MAXHP());

    nCurExp += exp;

    int64_t nRemainExp = nCurExp - nMaxExp;
    while (nRemainExp >= 0)
    {
        
        nLevel++;
        
        m_pKernelModule->SetPropertyInt(self, SquickProtocol::Player::Level(), nLevel);

        nCurExp = nRemainExp;

        const std::string& nextHeroConfigID = m_pElementModule->GetPropertyString(m_pPropertyConfigModule->GetInitPropertyID(eJobType, nLevel), SquickProtocol::InitProperty::HeroConfigID());
        nMaxExp = m_pElementModule->GetPropertyInt(heroConfigID, SquickProtocol::Player::MAXHP());
        if (nMaxExp <= 0)
        {
            break;
        }

        nRemainExp -= nMaxExp;
    }

    m_pKernelModule->SetPropertyInt(self, SquickProtocol::Player::EXP(), nCurExp);

    return true;
}

bool PropertyModule::FullHPMP(const Guid& self)
{
    INT64 nMaxHP = m_pKernelModule->GetPropertyInt(self, SquickProtocol::Player::MAXHP());
    if (nMaxHP > 0)
    {
        m_pKernelModule->SetPropertyInt(self, SquickProtocol::Player::HP(), nMaxHP);
    }

    INT64 nMaxMP = m_pKernelModule->GetPropertyInt(self, SquickProtocol::Player::MAXMP());
    if (nMaxMP > 0)
    {
        m_pKernelModule->SetPropertyInt(self, SquickProtocol::Player::MP(), nMaxMP);
    }

    return true;
}

bool PropertyModule::AddHP(const Guid& self, const int nValue)
{
    if (nValue <= 0)
    {
        return false;
    }

	int nCurValue = m_pKernelModule->GetPropertyInt32(self, SquickProtocol::Player::HP());
	int nMaxValue = m_pKernelModule->GetPropertyInt32(self, SquickProtocol::Player::MAXHP());

    if (nCurValue > 0)
    {
        nCurValue += nValue;
        if (nCurValue > nMaxValue)
        {
            nCurValue = nMaxValue;
        }

        m_pKernelModule->SetPropertyInt(self, SquickProtocol::Player::HP(), nCurValue);
    }

    return true;
}

bool PropertyModule::EnoughHP(const Guid& self, const int nValue)
{
    INT64 nCurValue = m_pKernelModule->GetPropertyInt(self, SquickProtocol::Player::HP());
    if ((nCurValue > 0) && (nCurValue - nValue >= 0))
    {
        return true;
    }

    return false;
}

bool PropertyModule::DamageHP(const Guid & self, const int nValue)
{
	int nCurValue = m_pKernelModule->GetPropertyInt32(self, SquickProtocol::Player::HP());
	if (nCurValue > 0)
	{
		nCurValue -= nValue;
		nCurValue = (nCurValue >= 0) ? nCurValue : 0;

		m_pKernelModule->SetPropertyInt(self, SquickProtocol::Player::HP(), nCurValue);

		return true;
	}

	return false;
}

bool PropertyModule::ConsumeHP(const Guid& self, const int nValue)
{
	int nCurValue = m_pKernelModule->GetPropertyInt32(self, SquickProtocol::Player::HP());
    if ((nCurValue > 0) && (nCurValue - nValue >= 0))
    {
        nCurValue -= nValue;
        m_pKernelModule->SetPropertyInt(self, SquickProtocol::Player::HP(), nCurValue);

        return true;
    }

    return false;
}

bool PropertyModule::AddMP(const Guid& self, const int nValue)
{
    if (nValue <= 0)
    {
        return false;
    }

	int nCurValue = m_pKernelModule->GetPropertyInt32(self, SquickProtocol::Player::MP());
	int nMaxValue = m_pKernelModule->GetPropertyInt32(self, SquickProtocol::Player::MAXMP());

    nCurValue += nValue;
    if (nCurValue > nMaxValue)
    {
        nCurValue = nMaxValue;
    }

    m_pKernelModule->SetPropertyInt(self, SquickProtocol::Player::MP(), nCurValue);

    return true;
}

bool PropertyModule::ConsumeMP(const Guid& self, const int nValue)
{
	int nCurValue = m_pKernelModule->GetPropertyInt32(self, SquickProtocol::Player::MP());
    if ((nCurValue > 0) && (nCurValue - nValue >= 0))
    {
        nCurValue -= nValue;
        m_pKernelModule->SetPropertyInt(self, SquickProtocol::Player::MP(), nCurValue);

        return true;
    }

    return false;
}

bool PropertyModule::EnoughMP(const Guid& self, const int nValue)
{
	int nCurValue = m_pKernelModule->GetPropertyInt32(self, SquickProtocol::Player::MP());
    if ((nCurValue > 0) && (nCurValue - nValue >= 0))
    {
        return true;
    }

    return false;
}

bool PropertyModule::DamageMP(const Guid & self, const int nValue)
{
	int nCurValue = m_pKernelModule->GetPropertyInt32(self, SquickProtocol::Player::MP());
	if (nCurValue > 0)
	{
		nCurValue -= nValue;
		nCurValue = (nCurValue >= 0) ? nCurValue : 0;

		m_pKernelModule->SetPropertyInt(self, SquickProtocol::Player::MP(), nCurValue);

		return true;
	}

	return false;
}

bool PropertyModule::FullSP(const Guid& self)
{
	int nMAXCSP = m_pKernelModule->GetPropertyInt32(self, SquickProtocol::Player::MAXSP());
    if (nMAXCSP > 0)
    {
        m_pKernelModule->SetPropertyInt(self, SquickProtocol::Player::SP(), nMAXCSP);

        return true;
    }

    return false;
}

bool PropertyModule::AddSP(const Guid& self, const int nValue)
{
    if (nValue <= 0)
    {
        return false;
    }

	int nCurValue = m_pKernelModule->GetPropertyInt32(self, SquickProtocol::Player::SP());
	int nMaxValue = m_pKernelModule->GetPropertyInt32(self, SquickProtocol::Player::MAXSP());

    nCurValue += nValue;
    if (nCurValue > nMaxValue)
    {
        nCurValue = nMaxValue;
    }

    m_pKernelModule->SetPropertyInt(self, SquickProtocol::Player::SP(), nCurValue);

    return true;
}

bool PropertyModule::ConsumeSP(const Guid& self, const int nValue)
{
	int nCSP = m_pKernelModule->GetPropertyInt32(self, SquickProtocol::Player::SP());
    if ((nCSP > 0) && (nCSP - nValue >= 0))
    {
        nCSP -= nValue;
        m_pKernelModule->SetPropertyInt(self, SquickProtocol::Player::SP(), nCSP);

        return true;
    }

    return false;
}

bool PropertyModule::EnoughSP(const Guid& self, const int nValue)
{
	int nCurValue = m_pKernelModule->GetPropertyInt32(self, SquickProtocol::Player::SP());
    if ((nCurValue > 0) && (nCurValue - nValue >= 0))
    {
        return true;
    }

    return false;
}

bool PropertyModule::AddGold(const Guid& self, const int64_t nValue)
{
	int64_t nCurValue = m_pKernelModule->GetPropertyInt(self, SquickProtocol::Player::Gold());
    nCurValue += nValue;
	if (nCurValue < 0)
	{
		nCurValue = 0;
	}

    m_pKernelModule->SetPropertyInt(self, SquickProtocol::Player::Gold(), nCurValue);

    return false;
}

bool PropertyModule::ConsumeGold(const Guid& self, const int64_t nValue)
{
    if (nValue <= 0)
    {
        return false;
    }

	int64_t nCurValue = m_pKernelModule->GetPropertyInt(self, SquickProtocol::Player::Gold());
    nCurValue -= nValue;
    if (nCurValue >= 0)
    {
        m_pKernelModule->SetPropertyInt(self, SquickProtocol::Player::Gold(), nCurValue);

        return true;
    }

    return false;
}

bool PropertyModule::EnoughGold(const Guid& self, const int64_t nValue)
{
	int64_t nCurValue = m_pKernelModule->GetPropertyInt(self, SquickProtocol::Player::Gold());
    if ((nCurValue > 0) && (nCurValue - nValue >= 0))
    {
        return true;
    }

    return false;
}

bool PropertyModule::AddDiamond(const Guid& self, const int nValue)
{
	int nCurValue = m_pKernelModule->GetPropertyInt32(self, SquickProtocol::Player::Diamond());
    nCurValue += nValue;
	if (nCurValue < 0)
	{
		nCurValue = 0;
	}

    m_pKernelModule->SetPropertyInt(self, SquickProtocol::Player::Diamond(), nCurValue);

    return false;
}

bool PropertyModule::ConsumeDiamond(const Guid& self, const int nValue)
{
    if (nValue <= 0)
    {
        return false;
    }

	int nCurValue = m_pKernelModule->GetPropertyInt32(self, SquickProtocol::Player::Diamond());
    nCurValue -= nValue;
    if (nCurValue >= 0)
    {
        m_pKernelModule->SetPropertyInt(self, SquickProtocol::Player::Diamond(), nCurValue);

        return true;
    }

    return false;
}

bool PropertyModule::EnoughDiamond(const Guid& self, const int nValue)
{
	int nCurValue = m_pKernelModule->GetPropertyInt32(self, SquickProtocol::Player::Diamond());
    if ((nCurValue > 0) && (nCurValue - nValue >= 0))
    {
        return true;
    }

    return false;
}

void PropertyModule::ActiveExtraController()
{
	activeExtraController = true;
}
