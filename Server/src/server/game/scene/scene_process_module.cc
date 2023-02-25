

#include "scene_process_module.h"
#include "squick/struct/protocol_define.h"
#include "squick/struct/struct.h"

bool SceneProcessModule::Start()
{
	m_pKernelModule = pPluginManager->FindModule<IKernelModule>();
	m_pElementModule = pPluginManager->FindModule<IElementModule>();
	m_pClassModule = pPluginManager->FindModule<IClassModule>();
	m_pLogModule = pPluginManager->FindModule<ILogModule>();
	m_pEventModule = pPluginManager->FindModule<IEventModule>();
	m_pSceneModule = pPluginManager->FindModule<ISceneModule>();
	m_pCellModule = pPluginManager->FindModule<ICellModule>();
	m_pGameServerNet_ServerModule = pPluginManager->FindModule<IGameServerNet_ServerModule>();
	
    return true;
}

bool SceneProcessModule::Destory()
{
    return true;
}

bool SceneProcessModule::Update()
{
    return true;
}

bool SceneProcessModule::AfterStart()
{
    m_pKernelModule->AddClassCallBack(SquickProtocol::Player::ThisName(), this, &SceneProcessModule::OnObjectClassEvent);

	m_pSceneModule->AddEnterSceneConditionCallBack(this, &SceneProcessModule::EnterSceneConditionEvent);

	m_pSceneModule->AddBeforeEnterSceneGroupCallBack(this, &SceneProcessModule::BeforeEnterSceneGroupEvent);
	m_pSceneModule->AddAfterEnterSceneGroupCallBack(this, &SceneProcessModule::AfterEnterSceneGroupEvent);
	m_pSceneModule->AddBeforeLeaveSceneGroupCallBack(this, &SceneProcessModule::BeforeLeaveSceneGroupEvent);
	m_pSceneModule->AddAfterLeaveSceneGroupCallBack(this, &SceneProcessModule::AfterLeaveSceneGroupEvent);
    //////////////////////////////////////////////////////////////////////////

    return true;
}

bool SceneProcessModule::ReadyUpdate()
{
	SQUICK_SHARE_PTR<IClass> xLogicClass = m_pClassModule->GetElement(SquickProtocol::Scene::ThisName());
	if (xLogicClass)
	{
		const std::vector<std::string>& strIdList = xLogicClass->GetIDList();
		for (int i = 0; i < strIdList.size(); ++i)
		{
			const std::string& strId = strIdList[i];

			//load first
			LoadSceneResource(strId);
			//create second
			CreateSceneBaseGroup(strId);
		}
	}

	return true;
}

bool SceneProcessModule::RequestEnterScene(const Guid & self, const int sceneID, const int groupID, const int type, const Vector3& pos, const DataList & argList)
{
	SquickStruct::ESceneType eSceneType = (SquickStruct::ESceneType)m_pElementModule->GetPropertyInt32(std::to_string(sceneID), SquickProtocol::Scene::Type());
	if (eSceneType == SquickStruct::ESceneType::SINGLE_CLONE_SCENE)
	{
		if (groupID < 0)
		{
			int nNewGroupID = m_pKernelModule->RequestGroupScene(sceneID);
			if (nNewGroupID > 0)
			{
				return m_pSceneModule->RequestEnterScene(self, sceneID, nNewGroupID, type, pos, argList);
			}
		}
		else if (groupID > 0)
		{
			return m_pSceneModule->RequestEnterScene(self, sceneID, groupID, type, pos, argList);
		}
	}
	else if (eSceneType == SquickStruct::ESceneType::NORMAL_SCENE)
	{
		return m_pSceneModule->RequestEnterScene(self, sceneID, 1, type, pos, argList);
	}
	else
	{
		if (groupID > 0)
		{
			return m_pSceneModule->RequestEnterScene(self, sceneID, groupID, type, pos, argList);
		}
		else
		{
			int nNewGroupID = m_pKernelModule->RequestGroupScene(sceneID);
			if (nNewGroupID > 0)
			{
				return m_pSceneModule->RequestEnterScene(self, sceneID, nNewGroupID, type, pos, argList);
			}
		}
	}

	return false;
}

int SceneProcessModule::BeforeLeaveSceneGroupEvent(const Guid & self, const int sceneID, const int groupID, const int type, const DataList & argList)
{
	//change AI onwer

	return 0;
}

int SceneProcessModule::AfterLeaveSceneGroupEvent(const Guid & self, const int sceneID, const int groupID, const int type, const DataList & argList)
{
	SquickStruct::ESceneType eSceneType = (SquickStruct::ESceneType)m_pElementModule->GetPropertyInt32(std::to_string(sceneID), SquickProtocol::Scene::Type());
	if (eSceneType == SquickStruct::ESceneType::NORMAL_SCENE)
	{

	}
	else if (eSceneType == SquickStruct::ESceneType::SINGLE_CLONE_SCENE)
	{
		m_pKernelModule->ReleaseGroupScene(sceneID, groupID);

		return 0;
	}
	else
	{
		const Guid masterID = m_pSceneModule->GetPropertyObject(sceneID, groupID, SquickProtocol::Group::MasterID());
		if (masterID.IsNull())
		{
			DataList varObjectList;
			if (m_pKernelModule->GetGroupObjectList(sceneID, groupID, varObjectList, true) && varObjectList.GetCount() <= 0)
			{
				m_pKernelModule->ReleaseGroupScene(sceneID, groupID);

				m_pLogModule->LogInfo(self, "DestroyCloneSceneGroup " + std::to_string(groupID), __FUNCTION__ , __LINE__);
			}
		}
	}

	return 0;
}

int SceneProcessModule::OnObjectClassEvent(const Guid& self, const std::string& className, const CLASS_OBJECT_EVENT classEvent, const DataList& var)
{
    if (className == SquickProtocol::Player::ThisName())
    {
        if (CLASS_OBJECT_EVENT::COE_DESTROY == classEvent)
        {
			int sceneID = m_pKernelModule->GetPropertyInt32(self, SquickProtocol::Player::SceneID());
			int groupID = m_pKernelModule->GetPropertyInt32(self, SquickProtocol::Player::GroupID());
			SquickStruct::ESceneType eSceneType = (SquickStruct::ESceneType)m_pElementModule->GetPropertyInt32(std::to_string(sceneID), SquickProtocol::Scene::Type());

            if (eSceneType == SquickStruct::ESceneType::SINGLE_CLONE_SCENE)
            {
            	//wa need to wait for this player for a moment if the player disconnected from server.
                m_pKernelModule->ReleaseGroupScene(sceneID, groupID);

                m_pLogModule->LogInfo(self, "DestroyCloneSceneGroup " + std::to_string(groupID), __FUNCTION__ , __LINE__);
            }
			else if (eSceneType == SquickStruct::ESceneType::MULTI_CLONE_SCENE)
			{
				//wa need to wait for this player if the player disconnected from server.
				DataList varObjectList;
				if (m_pKernelModule->GetGroupObjectList(sceneID, groupID, varObjectList, true) && varObjectList.GetCount() <= 0)
				{
					m_pKernelModule->ReleaseGroupScene(sceneID, groupID);

					m_pLogModule->LogInfo(self, "DestroyCloneSceneGroup " + std::to_string(groupID), __FUNCTION__ , __LINE__);
				}
			}
        }
    }

    return 0;
}

int SceneProcessModule::EnterSceneConditionEvent(const Guid & self, const int sceneID, const int groupID, const int type, const DataList & argList)
{
	return 0;
}

int SceneProcessModule::BeforeEnterSceneGroupEvent(const Guid & self, const int sceneID, const int groupID, const int type, const DataList & argList)
{
	//you can use object pool to enhance performance
	SquickStruct::ESceneType eSceneType = (SquickStruct::ESceneType)m_pElementModule->GetPropertyInt32(std::to_string(sceneID), SquickProtocol::Scene::Type());
	if (eSceneType == SquickStruct::ESceneType::SINGLE_CLONE_SCENE)
	{
		m_pSceneModule->CreateSceneNPC(sceneID, groupID, DataList::Empty());
	}
	else if (eSceneType == SquickStruct::ESceneType::MULTI_CLONE_SCENE)
	{
		DataList varObjectList;
		if (m_pKernelModule->GetGroupObjectList(sceneID, groupID, varObjectList, true) && varObjectList.GetCount() <= 0)
		{
			m_pSceneModule->CreateSceneNPC(sceneID, groupID, DataList::Empty());
		}
	}

	return 0;
}

int SceneProcessModule::AfterEnterSceneGroupEvent(const Guid & self, const int sceneID, const int groupID, const int type, const DataList & argList)
{
	SquickStruct::ESceneType eSceneType = (SquickStruct::ESceneType)m_pElementModule->GetPropertyInt32(std::to_string(sceneID), SquickProtocol::Scene::Type());
	if (eSceneType == SquickStruct::ESceneType::SINGLE_CLONE_SCENE)
	{
	}
	else if (eSceneType == SquickStruct::ESceneType::MULTI_CLONE_SCENE)
	{
	}
	else if (eSceneType == SquickStruct::ESceneType::NORMAL_SCENE)
	{
		
	}

	return 1;
}

// 加载场景资源
bool SceneProcessModule::LoadSceneResource(const std::string& strSceneIDName)
{
    const std::string& strSceneFilePath = m_pElementModule->GetPropertyString(strSceneIDName, SquickProtocol::Scene::FilePath());
 
    rapidxml::file<> xFileSource(strSceneFilePath.c_str());
    rapidxml::xml_document<>  xFileDoc;
    xFileDoc.parse<0>(xFileSource.data());

	int sceneID = lexical_cast<int>(strSceneIDName);

    rapidxml::xml_node<>* pSeedFileRoot = xFileDoc.first_node();
    for (rapidxml::xml_node<>* pSeedFileNode = pSeedFileRoot->first_node(); pSeedFileNode; pSeedFileNode = pSeedFileNode->next_sibling())
    {
        std::string seedID = pSeedFileNode->first_attribute(SquickProtocol::IObject::ID().c_str())->value();
		std::string configID = pSeedFileNode->first_attribute(SquickProtocol::IObject::ConfigID().c_str())->value();
        float fSeedX = lexical_cast<float>(pSeedFileNode->first_attribute("X")->value());
        float fSeedY = lexical_cast<float>(pSeedFileNode->first_attribute("Y")->value());
        float fSeedZ = lexical_cast<float>(pSeedFileNode->first_attribute("Z")->value());
		int nWeight = lexical_cast<int>(pSeedFileNode->first_attribute("Weight")->value());
		m_pSceneModule->AddSeedData(sceneID, seedID, configID, Vector3(fSeedX, fSeedY, fSeedZ), nWeight);

	}

	const std::string& strRelivePosition = m_pElementModule->GetPropertyString(strSceneIDName, SquickProtocol::Scene::RelivePos());
	DataList xRelivePositionList;
	xRelivePositionList.Split(strRelivePosition, ";");
	for (int i = 0; i < xRelivePositionList.GetCount(); ++i)
	{
		const std::string& strPos = xRelivePositionList.String(i);
		if (!strPos.empty())
		{
			DataList xPosition;
			xPosition.Split(strPos, ",");
			if (xPosition.GetCount() == 3)
			{
				float fPosX = lexical_cast<float>(xPosition.String(0));
				float fPosY = lexical_cast<float>(xPosition.String(1));
				float fPosZ = lexical_cast<float>(xPosition.String(2));
				m_pSceneModule->AddRelivePosition(sceneID, i, Vector3(fPosX, fPosY, fPosZ));
			}
		}
	}

	const std::string& strTagPosition = m_pElementModule->GetPropertyString(strSceneIDName, SquickProtocol::Scene::RelivePosEx());
	DataList xTagPositionList;
	xTagPositionList.Split(strTagPosition, ";");
	for (int i = 0; i < xTagPositionList.GetCount(); ++i)
	{
		const std::string& strPos = xTagPositionList.String(i);
		if (!strPos.empty())
		{
			DataList xPosition;
			xPosition.Split(strPos, ",");
			if (xPosition.GetCount() == 3)
			{
				float fPosX = lexical_cast<float>(xPosition.String(0));
				float fPosY = lexical_cast<float>(xPosition.String(1));
				float fPosZ = lexical_cast<float>(xPosition.String(2));
				m_pSceneModule->AddTagPosition(sceneID, i, Vector3(fPosX, fPosY, fPosZ));
			}
		}
	}


    return true;
}

bool SceneProcessModule::CreateSceneBaseGroup(const std::string & strSceneIDName)
{
	const int sceneID = lexical_cast<int>(strSceneIDName);
	SquickStruct::ESceneType eSceneType = (SquickStruct::ESceneType)m_pElementModule->GetPropertyInt32(std::to_string(sceneID), SquickProtocol::Scene::Type());
	const int nMaxGroup = m_pElementModule->GetPropertyInt32(std::to_string(sceneID), SquickProtocol::Scene::MaxGroup());
	if (eSceneType == SquickStruct::ESceneType::NORMAL_SCENE)
	{
		//line 10
		for (int i = 0; i < 1; ++i)
		{
			int groupID = m_pKernelModule->RequestGroupScene(sceneID);
			if (groupID > 0)
			{
				m_pSceneModule->CreateSceneNPC(sceneID, groupID);
			}
			else
			{
				m_pLogModule->LogError("CreateSceneBaseGroup error " + strSceneIDName);
			}
		}

		return true;
	}
	return false;
}
