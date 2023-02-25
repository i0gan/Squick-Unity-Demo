


#include "property_config_module.h"
#include <squick/core/i_plugin_manager.h>

bool PropertyConfigModule::Start()
{
    return true;
}

bool PropertyConfigModule::Destory()
{
    return true;
}

bool PropertyConfigModule::Update()
{
    return true;
}

bool PropertyConfigModule::AfterStart()
{
    m_pClassModule = pPluginManager->FindModule<IClassModule>();
    m_pElementModule = pPluginManager->FindModule<IElementModule>();

    Load();

    return true;
}

const std::string& PropertyConfigModule::GetInitPropertyID(const int nJob,  const int nLevel)
{
    std::map<Guid, std::string>& propertyData = GetData();
	auto it = propertyData.find(Guid(nJob, nLevel));
	if (it != propertyData.end())
	{
        return it->second;
	}

    return NULL_STR;
}

void PropertyConfigModule::ClearInitPropertyData()
{
    std::map<Guid, std::string>& propertyData = GetData();
    propertyData.clear();
}

void PropertyConfigModule::AddInitPropertyID(const int nJob, const int nLevel, const std::string& data)
{
    std::map<Guid, std::string>& propertyData = GetData();

	auto it = propertyData.find(Guid(nJob, nLevel));
	if (it != propertyData.end())
	{
        propertyData.insert(std::make_pair(Guid(nJob, nLevel), data));
	}
}

void PropertyConfigModule::SetEx(const bool b)
{
    mbExtra = b;
}

void PropertyConfigModule::Load()
{
    SQUICK_SHARE_PTR<IClass> xLogicClass = m_pClassModule->GetElement(SquickProtocol::InitProperty::ThisName());
    if (xLogicClass)
    {
		const std::vector<std::string>& strIdList = xLogicClass->GetIDList();
		for (int i = 0; i < strIdList.size(); ++i)
		{
			const std::string& strId = strIdList[i];

            SQUICK_SHARE_PTR<IPropertyManager> pPropertyManager = m_pElementModule->GetPropertyManager(strId);
            if (pPropertyManager)
            {
                const int nJob = m_pElementModule->GetPropertyInt32(strId, SquickProtocol::InitProperty::Job());
                const int nLevel = m_pElementModule->GetPropertyInt32(strId, SquickProtocol::InitProperty::Level());

                std::map<Guid, std::string>& propertyData = GetData();

				auto it = propertyData.find(Guid(nJob, nLevel));
				if (it == propertyData.end())
				{
                    propertyData.insert(std::make_pair(Guid(nJob, nLevel), strId));
				}
            }
        }
    }
}

std::map<Guid, std::string>& PropertyConfigModule::GetData()
{
    return mhtCoefficientData;
}

bool PropertyConfigModule::LegalLevel(const int nJob, const int nLevel)
{
	auto it = mhtCoefficientData.find(Guid(nJob, nLevel));
	if (it != mhtCoefficientData.end())
	{
		return true;
	}


    return false;
}
