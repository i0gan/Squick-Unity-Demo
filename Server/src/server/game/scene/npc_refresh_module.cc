#include <squick/struct/struct.h>
#include "npc_refresh_module.h"

bool NPCRefreshModule::Start()
{
    return true;
}


bool NPCRefreshModule::Destory()
{
    return true;
}

bool NPCRefreshModule::Update()
{
    return true;
}

bool NPCRefreshModule::AfterStart()
{
	m_pScheduleModule = pPluginManager->FindModule<IScheduleModule>();
	m_pEventModule = pPluginManager->FindModule<IEventModule>();
    m_pKernelModule = pPluginManager->FindModule<IKernelModule>();
    m_pSceneProcessModule = pPluginManager->FindModule<ISceneProcessModule>();
    m_pElementModule = pPluginManager->FindModule<IElementModule>();
	m_pLogModule = pPluginManager->FindModule<ILogModule>();
	m_pPropertyModule = pPluginManager->FindModule<IPropertyModule>();
	m_pSceneModule = pPluginManager->FindModule<ISceneModule>();
	
	m_pKernelModule->AddClassCallBack(SquickProtocol::NPC::ThisName(), this, &NPCRefreshModule::OnObjectClassEvent);

    return true;
}

int NPCRefreshModule::OnObjectClassEvent( const Guid& self, const std::string& className, const CLASS_OBJECT_EVENT classEvent, const DataList& var )
{
    SQUICK_SHARE_PTR<IObject> pSelf = m_pKernelModule->GetObject(self);
    if (nullptr == pSelf)
    {
        return 1;
    }

    if (className == SquickProtocol::NPC::ThisName())
    {
        if ( CLASS_OBJECT_EVENT::COE_CREATE_LOADDATA == classEvent )
        {
            const std::string& configIndex = m_pKernelModule->GetPropertyString(self, SquickProtocol::NPC::ConfigID());
			const std::string& strEffectPropertyID = m_pElementModule->GetPropertyString(configIndex, SquickProtocol::NPC::EffectData());
			const int npcType = m_pElementModule->GetPropertyInt32(configIndex, SquickProtocol::NPC::NPCType());
			SQUICK_SHARE_PTR<IPropertyManager> pSelfPropertyManager = pSelf->GetPropertyManager();

			//effect data
			//normal npc
			SQUICK_SHARE_PTR<IPropertyManager> pConfigPropertyManager = m_pElementModule->GetPropertyManager(strEffectPropertyID);
			if (pConfigPropertyManager)
			{
				std::string strProperName;
				for (IProperty* property = pConfigPropertyManager->FirstNude(strProperName); property != NULL; property = pConfigPropertyManager->NextNude(strProperName))
				{
					if (pSelfPropertyManager && property->Changed()
						&& strProperName != SquickProtocol::IObject::ID()
						&& strProperName != SquickProtocol::IObject::ConfigID()
						&& strProperName != SquickProtocol::IObject::ClassName()
						&& strProperName != SquickProtocol::IObject::SceneID()
						&& strProperName != SquickProtocol::IObject::GroupID())
					{
						pSelfPropertyManager->SetProperty(property->GetKey(), property->GetValue());
					}
				}
			}

			if (npcType == SquickStruct::ENPCType::HERO_NPC)
			{
				//star & level
			}
        }
        else if ( CLASS_OBJECT_EVENT::COE_CREATE_HASDATA == classEvent )
        {
			int nHPMax = m_pKernelModule->GetPropertyInt32(self, SquickProtocol::NPC::MAXHP());
            m_pKernelModule->SetPropertyInt(self, SquickProtocol::NPC::HP(), nHPMax);

            m_pKernelModule->AddPropertyCallBack( self, SquickProtocol::NPC::HP(), this, &NPCRefreshModule::OnObjectHPEvent);
        }
    }

    return 0;
}

int NPCRefreshModule::OnObjectHPEvent( const Guid& self, const std::string& propertyName, const SquickData& oldVar, const SquickData& newVar, const INT64 reason)
{
    if ( newVar.GetInt() <= 0 )
    {
        const Guid& identAttacker = m_pKernelModule->GetPropertyObject( self, SquickProtocol::NPC::LastAttacker());
        if (!identAttacker.IsNull())
		{
			OnObjectBeKilled(self, identAttacker);
        }

		m_pScheduleModule->AddSchedule( self, "OnNPCDeadDestroyHeart", this, &NPCRefreshModule::OnNPCDeadDestroyHeart, 1.0f, 1 );
    }

    return 0;
}

int NPCRefreshModule::OnNPCDeadDestroyHeart( const Guid& self, const std::string& heartBeat, const float time, const int count)
{
    //and create new object
	int sceneID = m_pKernelModule->GetPropertyInt32( self, SquickProtocol::NPC::SceneID());
	int groupID = m_pKernelModule->GetPropertyInt32(self, SquickProtocol::NPC::GroupID());
	int npcType = m_pKernelModule->GetPropertyInt32(self, SquickProtocol::NPC::NPCType());

	if (npcType == SquickStruct::ENPCType::NORMAL_NPC)
	{

		const std::string& className = m_pKernelModule->GetPropertyString( self, SquickProtocol::NPC::ClassName());
		const std::string& seedID = m_pKernelModule->GetPropertyString( self, SquickProtocol::NPC::SeedID());
		const std::string& configID = m_pKernelModule->GetPropertyString( self, SquickProtocol::NPC::ConfigID());
		const Guid masterID = m_pKernelModule->GetPropertyObject(self, SquickProtocol::NPC::MasterID());
		const Guid camp = m_pKernelModule->GetPropertyObject(self, SquickProtocol::NPC::CampID());
		int refresh = m_pKernelModule->GetPropertyInt32(self, SquickProtocol::NPC::Refresh());

		m_pKernelModule->DestroySelf(self);

		const Vector3& seedPos = m_pSceneModule->GetSeedPos(sceneID, seedID);
		if (refresh > 0)
		{
			DataList arg;
			arg << SquickProtocol::NPC::Position() << seedPos;
			arg << SquickProtocol::NPC::SeedID() << seedID;
			arg << SquickProtocol::NPC::MasterID() << masterID;
			arg << SquickProtocol::NPC::CampID() << camp;
			arg << SquickProtocol::NPC::Refresh() << refresh;

			m_pKernelModule->CreateObject(Guid(), sceneID, groupID, className, configID, arg);
		}
	}
	else
	{
		m_pKernelModule->DestroySelf(self);
	}

    return 0;
}

int NPCRefreshModule::OnBuildingDeadDestroyHeart(const Guid & self, const std::string & heartBeat, const float time, const int count)
{
	//and create new object
	const std::string& className = m_pKernelModule->GetPropertyString(self, SquickProtocol::NPC::ClassName());
	const std::string& seedID = m_pKernelModule->GetPropertyString(self, SquickProtocol::NPC::SeedID());
	const std::string& configID = m_pKernelModule->GetPropertyString(self, SquickProtocol::NPC::ConfigID());
	const Guid masterID = m_pKernelModule->GetPropertyObject(self, SquickProtocol::NPC::MasterID());
	int npcType = m_pKernelModule->GetPropertyInt32(self, SquickProtocol::NPC::NPCType());
	int sceneID = m_pKernelModule->GetPropertyInt32(self, SquickProtocol::NPC::SceneID());
	int groupID = m_pKernelModule->GetPropertyInt32(self, SquickProtocol::NPC::GroupID());

	Vector3 fSeedPos = m_pKernelModule->GetPropertyVector3(self, SquickProtocol::NPC::Position());

	if (npcType == SquickStruct::ENPCType::TURRET_NPC)
	{
		m_pKernelModule->DestroySelf(self);

		DataList arg;
		arg << SquickProtocol::NPC::Position() << fSeedPos;
		arg << SquickProtocol::NPC::SeedID() << seedID;
		arg << SquickProtocol::NPC::MasterID() << masterID;

		m_pKernelModule->CreateObject(Guid(), sceneID, groupID, className, configID, arg);
	}
	
	return 0;
}

int NPCRefreshModule::OnObjectBeKilled( const Guid& self, const Guid& killer )
{
	if ( m_pKernelModule->GetObject(killer) )
	{
		const int64_t exp = m_pKernelModule->GetPropertyInt(self, SquickProtocol::NPC::EXP());
		const int64_t gold = m_pKernelModule->GetPropertyInt(self, SquickProtocol::NPC::Gold());

		m_pPropertyModule->AddExp(killer, exp);
		m_pPropertyModule->AddGold(killer, gold);
	}

	return 0;
}