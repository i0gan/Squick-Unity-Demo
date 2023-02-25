#include "game_manager_module.h"

bool GameManagerModule::Start()
{
	m_pKernelModule = pPluginManager->FindModule<IKernelModule>();
	m_pClassModule = pPluginManager->FindModule<IClassModule>();
	m_pSceneProcessModule = pPluginManager->FindModule<ISceneProcessModule>();
	m_pElementModule = pPluginManager->FindModule<IElementModule>();
	m_pLogModule = pPluginManager->FindModule<ILogModule>();
	m_pEventModule = pPluginManager->FindModule<IEventModule>();
	m_pSceneModule = pPluginManager->FindModule<ISceneModule>();
	m_pScheduleModule = pPluginManager->FindModule<IScheduleModule>();

	m_pNetModule = pPluginManager->FindModule<INetModule>();
	m_pNetClientModule = pPluginManager->FindModule<INetClientModule>();
	m_pGameServerNet_ServerModule = pPluginManager->FindModule<IGameServerNet_ServerModule>();

	return true;
}

bool GameManagerModule::Destory()
{
	return true;
}

bool GameManagerModule::Update()
{
	return true;
}

bool GameManagerModule::AfterStart()
{
	m_pNetModule->AddReceiveCallBack(SquickStruct::ACK_PROPERTY_INT, this, &GameManagerModule::OnClientPropertyIntProcess);
	m_pNetModule->AddReceiveCallBack(SquickStruct::ACK_PROPERTY_FLOAT, this, &GameManagerModule::OnClientPropertyFloatProcess);
	m_pNetModule->AddReceiveCallBack(SquickStruct::ACK_PROPERTY_STRING, this, &GameManagerModule::OnClientPropertyStringProcess);
	m_pNetModule->AddReceiveCallBack(SquickStruct::ACK_PROPERTY_OBJECT, this, &GameManagerModule::OnClientPropertyObjectProcess);
	m_pNetModule->AddReceiveCallBack(SquickStruct::ACK_PROPERTY_VECTOR2, this, &GameManagerModule::OnClientPropertyVector2Process);
	m_pNetModule->AddReceiveCallBack(SquickStruct::ACK_PROPERTY_VECTOR3, this, &GameManagerModule::OnClientPropertyVector3Process);

	m_pNetModule->AddReceiveCallBack(SquickStruct::ACK_ADD_ROW, this, &GameManagerModule::OnClientAddRowProcess);
	m_pNetModule->AddReceiveCallBack(SquickStruct::ACK_REMOVE_ROW, this, &GameManagerModule::OnClientRemoveRowProcess);
	m_pNetModule->AddReceiveCallBack(SquickStruct::ACK_SWAP_ROW, this, &GameManagerModule::OnClientSwapRowProcess);
	m_pNetModule->AddReceiveCallBack(SquickStruct::ACK_RECORD_INT, this, &GameManagerModule::OnClientRecordIntProcess);
	m_pNetModule->AddReceiveCallBack(SquickStruct::ACK_RECORD_FLOAT, this, &GameManagerModule::OnClientRecordFloatProcess);
	m_pNetModule->AddReceiveCallBack(SquickStruct::ACK_RECORD_STRING, this, &GameManagerModule::OnClientRecordStringProcess);
	m_pNetModule->AddReceiveCallBack(SquickStruct::ACK_RECORD_OBJECT, this, &GameManagerModule::OnClientRecordObjectProcess);
	m_pNetModule->AddReceiveCallBack(SquickStruct::ACK_RECORD_VECTOR2, this, &GameManagerModule::OnClientRecordVector2Process);
	m_pNetModule->AddReceiveCallBack(SquickStruct::ACK_RECORD_VECTOR3, this, &GameManagerModule::OnClientRecordVector3Process);

	return true;
}

void GameManagerModule::OnClientPropertyIntProcess(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len)
{
	CLIENT_MSG_PROCESS( msgID, msg, len, SquickStruct::ObjectPropertyInt)

	int nGMLevel = 0;
	const int scene = pObject->GetPropertyInt(SquickProtocol::IObject::SceneID());
	const int group = pObject->GetPropertyInt(SquickProtocol::IObject::GroupID());

	for (int i = 0; i < xMsg.property_list_size(); i++)
	{
		const SquickStruct::PropertyInt &xProperty = xMsg.property_list().Get(i);
		Guid objectID = INetModule::ProtobufToStruct(xMsg.player_id());

		auto pProperty = CalProperty(pObject, objectID, xProperty.property_name(), nGMLevel);
		if (pProperty)
		{
			if (pProperty->GetUpload() || nGMLevel > 0)
			{
				m_pLogModule->LogInfo(objectID, "Upload From Client int set " + xProperty.property_name() + std::to_string(xProperty.data()), __FUNCTION__, __LINE__);

				if (!objectID.IsNull())
				{
					if (objectID == pObject->Self())
					{
						m_pKernelModule->SetPropertyInt(objectID, xProperty.property_name(), xProperty.data());
					}
					else if (nGMLevel > 1)
					{
						m_pKernelModule->SetPropertyInt(objectID, xProperty.property_name(), xProperty.data());
					}
				}
				else
				{
					m_pSceneModule->SetPropertyInt(scene, group, xProperty.property_name(), xProperty.data());
				}
			}
			else
			{
				m_pLogModule->LogError(nPlayerID, "Upload From Client int set Upload error " + xProperty.property_name(), __FUNCTION__, __LINE__);
			}
		}
		else
		{
			m_pLogModule->LogError(nPlayerID, "Upload From Client int set Property error " + xProperty.property_name(), __FUNCTION__, __LINE__);
		}
	}
}

void GameManagerModule::OnClientPropertyFloatProcess(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len)
{
	CLIENT_MSG_PROCESS( msgID, msg, len, SquickStruct::ObjectPropertyFloat)

	int nGMLevel = 0;
	const int scene = pObject->GetPropertyInt(SquickProtocol::IObject::SceneID());
	const int group = pObject->GetPropertyInt(SquickProtocol::IObject::GroupID());

	for (int i = 0; i < xMsg.property_list_size(); i++)
	{
		const SquickStruct::PropertyFloat &xProperty = xMsg.property_list().Get(i);
		Guid objectID = INetModule::ProtobufToStruct(xMsg.player_id());

		auto pProperty = CalProperty(pObject, objectID, xProperty.property_name(), nGMLevel);
		if (pProperty)
		{
			//judge upload then set value
			if (pProperty->GetUpload() || nGMLevel > 0)
			{
				if (!objectID.IsNull())
				{
					if (objectID == pObject->Self())
					{
						m_pKernelModule->SetPropertyFloat(objectID, xProperty.property_name(), xProperty.data());
					}
					else if (nGMLevel > 1)
					{
						m_pKernelModule->SetPropertyFloat(objectID, xProperty.property_name(), xProperty.data());
					}
				}
				else
				{
					m_pSceneModule->SetPropertyInt(scene, group, xProperty.property_name(), xProperty.data());
				}
			}
			else
			{
				m_pLogModule->LogError(nPlayerID, "Upload From Client float set Upload error " + xProperty.property_name(), __FUNCTION__, __LINE__);
			}
		}
		else
		{
			m_pLogModule->LogError(nPlayerID, "Upload From Client float set Property error " + xProperty.property_name(), __FUNCTION__, __LINE__);
		}
	}
}

void GameManagerModule::OnClientPropertyStringProcess(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len)
{
	CLIENT_MSG_PROCESS( msgID, msg, len, SquickStruct::ObjectPropertyString)

	int nGMLevel = 0;
	const int scene = pObject->GetPropertyInt(SquickProtocol::IObject::SceneID());
	const int group = pObject->GetPropertyInt(SquickProtocol::IObject::GroupID());

	for (int i = 0; i < xMsg.property_list_size(); i++)
	{
		const SquickStruct::PropertyString &xProperty = xMsg.property_list().Get(i);
		Guid objectID = INetModule::ProtobufToStruct(xMsg.player_id());

		auto pProperty = CalProperty(pObject, objectID, xProperty.property_name(), nGMLevel);
		if (pProperty)
		{
			//judge upload then set value
			if (pProperty->GetUpload() || nGMLevel > 0)
			{
				if (!objectID.IsNull())
				{
					if (objectID == pObject->Self())
					{
						m_pKernelModule->SetPropertyString(objectID, xProperty.property_name(), xProperty.data());
					}
					else if (nGMLevel > 1)
					{
						m_pKernelModule->SetPropertyString(objectID, xProperty.property_name(), xProperty.data());
					}
				}
				else
				{
					m_pSceneModule->SetPropertyString(scene, group, xProperty.property_name(), xProperty.data());
				}
			}
			else
			{
				m_pLogModule->LogError(nPlayerID, "Upload From Client string set Upload error " + xProperty.property_name(), __FUNCTION__, __LINE__);
			}
		}
		else
		{
			m_pLogModule->LogError(nPlayerID, "Upload From Client string set Property error" + xProperty.property_name(), __FUNCTION__, __LINE__);
		}
	}
}

void GameManagerModule::OnClientPropertyObjectProcess(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len)
{
	CLIENT_MSG_PROCESS( msgID, msg, len, SquickStruct::ObjectPropertyObject)

	int nGMLevel = 0;
	const int scene = pObject->GetPropertyInt(SquickProtocol::IObject::SceneID());
	const int group = pObject->GetPropertyInt(SquickProtocol::IObject::GroupID());

	for (int i = 0; i < xMsg.property_list_size(); i++)
	{
		const SquickStruct::PropertyObject &xProperty = xMsg.property_list().Get(i);
		Guid objectID = INetModule::ProtobufToStruct(xMsg.player_id());

		auto pProperty = CalProperty(pObject, objectID, xProperty.property_name(), nGMLevel);
		if (pProperty)
		{
			//judge upload then set value
			if (pProperty->GetUpload() || nGMLevel > 0)
			{
				if (!objectID.IsNull())
				{
					if (objectID == pObject->Self())
					{
						m_pKernelModule->SetPropertyObject(objectID, xProperty.property_name(), INetModule::ProtobufToStruct(xProperty.data()));
					}
					else if (nGMLevel > 1)
					{
						m_pKernelModule->SetPropertyObject(objectID, xProperty.property_name(), INetModule::ProtobufToStruct(xProperty.data()));
					}
				}
				else
				{
					m_pSceneModule->SetPropertyObject(scene, group, xProperty.property_name(), INetModule::ProtobufToStruct(xProperty.data()));
				}
			}
			else
			{
				m_pLogModule->LogError(nPlayerID, "Upload From Client object set Upload error " + xProperty.property_name(), __FUNCTION__, __LINE__);
			}
		}
		else
		{
			m_pLogModule->LogError(nPlayerID, "Upload From Client object set Property error " + xProperty.property_name(), __FUNCTION__, __LINE__);
		}
	}
}


void GameManagerModule::OnClientPropertyVector2Process(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len)
{
	CLIENT_MSG_PROCESS( msgID, msg, len, SquickStruct::ObjectPropertyVector2)

	int nGMLevel = 0;
	const int scene = pObject->GetPropertyInt(SquickProtocol::IObject::SceneID());
	const int group = pObject->GetPropertyInt(SquickProtocol::IObject::GroupID());

	for (int i = 0; i < xMsg.property_list_size(); i++)
	{
		SquickStruct::PropertyVector2 xProperty = xMsg.property_list().Get(i);
		Guid objectID = INetModule::ProtobufToStruct(xMsg.player_id());

		auto pProperty = CalProperty(pObject, objectID, xProperty.property_name(), nGMLevel);
		if (pProperty)
		{
			//judge upload then set value
			if (pProperty->GetUpload() || nGMLevel > 0)
			{
				if (!objectID.IsNull())
				{
					if (objectID == pObject->Self())
					{
						m_pKernelModule->SetPropertyVector2(objectID, xProperty.property_name(), INetModule::ProtobufToStruct(xProperty.data()));
					}
					else if (nGMLevel > 1)
					{
						m_pKernelModule->SetPropertyVector2(objectID, xProperty.property_name(), INetModule::ProtobufToStruct(xProperty.data()));
					}
				}
				else
				{
					m_pSceneModule->SetPropertyVector2(scene, group, xProperty.property_name(), INetModule::ProtobufToStruct(xProperty.data()));
				}
			}
			else
			{
				m_pLogModule->LogError(nPlayerID, "Upload From Client object set Upload error " + xProperty.property_name(), __FUNCTION__, __LINE__);
			}
		}
		else
		{
			m_pLogModule->LogError(nPlayerID, "Upload From Client object set Property error " + xProperty.property_name(), __FUNCTION__, __LINE__);
		}
	}
}

void GameManagerModule::OnClientPropertyVector3Process(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len)
{
	CLIENT_MSG_PROCESS( msgID, msg, len, SquickStruct::ObjectPropertyVector3)

	int nGMLevel = 0;
	const int scene = pObject->GetPropertyInt(SquickProtocol::IObject::SceneID());
	const int group = pObject->GetPropertyInt(SquickProtocol::IObject::GroupID());

	for (int i = 0; i < xMsg.property_list_size(); i++)
	{
		SquickStruct::PropertyVector3 xProperty = xMsg.property_list().Get(i);
		Guid objectID = INetModule::ProtobufToStruct(xMsg.player_id());

		auto pProperty = CalProperty(pObject, objectID, xProperty.property_name(), nGMLevel);
		if (pProperty)
		{
			//judge upload then set value
			if (pProperty->GetUpload() || nGMLevel > 0)
			{
				if (!objectID.IsNull())
				{
					if (objectID == pObject->Self())
					{
						m_pKernelModule->SetPropertyVector3(objectID, xProperty.property_name(), INetModule::ProtobufToStruct(xProperty.data()));
					}
					else if (nGMLevel > 1)
					{
						m_pKernelModule->SetPropertyVector3(objectID, xProperty.property_name(), INetModule::ProtobufToStruct(xProperty.data()));
					}
				}
				else
				{
					m_pSceneModule->SetPropertyVector3(scene, group, xProperty.property_name(), INetModule::ProtobufToStruct(xProperty.data()));
				}
			}
			else
			{
				m_pLogModule->LogError(nPlayerID, "Upload From Client object set Upload error " + xProperty.property_name(), __FUNCTION__, __LINE__);
			}
		}
		else
		{
			m_pLogModule->LogError(nPlayerID, "Upload From Client object set Property error " + xProperty.property_name(), __FUNCTION__, __LINE__);
		}
	}
}

void GameManagerModule::OnClientAddRowProcess(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len)
{
	CLIENT_MSG_PROCESS( msgID, msg, len, SquickStruct::ObjectRecordAddRow)

	int nGMLevel = 0;
	const int scene = pObject->GetPropertyInt(SquickProtocol::IObject::SceneID());
	const int group = pObject->GetPropertyInt(SquickProtocol::IObject::GroupID());

	Guid objectID = INetModule::ProtobufToStruct(xMsg.player_id());
	auto pRecord = CalRecord(pObject, objectID, xMsg.record_name(), nGMLevel);
	if (!pRecord)
	{
		m_pLogModule->LogError(nPlayerID, "Upload From Client add row record error " + xMsg.record_name(), __FUNCTION__, __LINE__);
		return;
	}
	if (pRecord->GetUpload() || nGMLevel > 0)
	{
		for (int i = 0; i < xMsg.row_data_size(); i++)
		{
			const SquickStruct::RecordAddRowStruct &xAddRowStruct = xMsg.row_data().Get(i);
			int row = xAddRowStruct.row();

			std::map<int, SquickData> colDataMap;
			for (int j = 0; j < xAddRowStruct.record_int_list_size(); j++)
			{
				const SquickStruct::RecordInt &xRecordInt = xAddRowStruct.record_int_list().Get(j);
				SquickData data;
				data.SetInt(xRecordInt.data());
				colDataMap[xRecordInt.col()] = data;
			}
			for (int j = 0; j < xAddRowStruct.record_float_list_size(); j++)
			{
				const SquickStruct::RecordFloat &xRecordFloat = xAddRowStruct.record_float_list().Get(j);
				SquickData data;
				data.SetFloat(xRecordFloat.data());
				colDataMap[xRecordFloat.col()] = data;
			}
			for (int j = 0; j < xAddRowStruct.record_string_list_size(); j++)
			{
				const SquickStruct::RecordString &xRecordString = xAddRowStruct.record_string_list().Get(j);
				SquickData data;
				data.SetString(xRecordString.data());
				colDataMap[xRecordString.col()] = data;
			}
			for (int j = 0; j < xAddRowStruct.record_object_list_size(); j++)
			{
				const SquickStruct::RecordObject &xRecordObject = xAddRowStruct.record_object_list().Get(j);
				SquickData data;
				data.SetObject(INetModule::ProtobufToStruct(xRecordObject.data()));
				colDataMap[xRecordObject.col()] = data;
			}

			for (int j = 0; j < xAddRowStruct.record_vector2_list_size(); j++)
			{
				const SquickStruct::RecordVector2 &xRecordObject = xAddRowStruct.record_vector2_list().Get(j);
				SquickData data;
				data.SetVector2(INetModule::ProtobufToStruct(xRecordObject.data()));
				colDataMap[xRecordObject.col()] = data;
			}

			for (int j = 0; j < xAddRowStruct.record_vector3_list_size(); j++)
			{
				const SquickStruct::RecordVector3 &xRecordObject = xAddRowStruct.record_vector3_list().Get(j);
				SquickData data;
				data.SetVector3(INetModule::ProtobufToStruct(xRecordObject.data()));
				colDataMap[xRecordObject.col()] = data;
			}

			DataList xDataList;
			for (int j = 0; j < colDataMap.size(); j++)
			{
				if (colDataMap.find(j) != colDataMap.end())
				{
					xDataList.Append(colDataMap[j]);
				}
				else
				{
					m_pLogModule->LogInfo(nPlayerID, "Upload From Client add row record error " + xMsg.record_name(), __FUNCTION__, __LINE__);
					return;
				}
			}

			if (pRecord->AddRow(row, xDataList) >= 0)
			{
				m_pLogModule->LogInfo(nPlayerID, "Upload From Client add row record " + xMsg.record_name(), __FUNCTION__, __LINE__);
			}
			else
			{
				m_pLogModule->LogInfo(nPlayerID, "Upload From Client add row record error " + xMsg.record_name(), __FUNCTION__, __LINE__);
			}
		}
	}
}

void GameManagerModule::OnClientRemoveRowProcess(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len)
{
	CLIENT_MSG_PROCESS( msgID, msg, len, SquickStruct::ObjectRecordRemove)

	int nGMLevel = 0;
	const int scene = pObject->GetPropertyInt(SquickProtocol::IObject::SceneID());
	const int group = pObject->GetPropertyInt(SquickProtocol::IObject::GroupID());

	Guid objectID = INetModule::ProtobufToStruct(xMsg.player_id());
	auto pRecord = CalRecord(pObject, objectID, xMsg.record_name(), nGMLevel);
	if (!pRecord)
	{
		m_pLogModule->LogError(nPlayerID, "Upload From Client remove row record error " + xMsg.record_name(), __FUNCTION__, __LINE__);
		return;
	}

	if (pRecord->GetUpload() || nGMLevel > 0)
	{
		for (int i = 0; i < xMsg.remove_row_size(); i++)
		{
			if (pRecord->Remove(xMsg.remove_row().Get(i)))
			{
				m_pLogModule->LogInfo(nPlayerID, "Upload From Client remove row record " + xMsg.record_name(), __FUNCTION__, __LINE__);
			}
			else
			{
				m_pLogModule->LogInfo(nPlayerID, "Upload From Client remove row record error " + xMsg.record_name(), __FUNCTION__, __LINE__);
			}
		}
	}
}

void GameManagerModule::OnClientSwapRowProcess(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len)
{

}

void GameManagerModule::OnClientRecordIntProcess(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len)
{
	CLIENT_MSG_PROCESS( msgID, msg, len, SquickStruct::ObjectRecordInt)

	int nGMLevel = 0;
	const int scene = pObject->GetPropertyInt(SquickProtocol::IObject::SceneID());
	const int group = pObject->GetPropertyInt(SquickProtocol::IObject::GroupID());

	Guid objectID = INetModule::ProtobufToStruct(xMsg.player_id());
	auto pRecord = CalRecord(pObject, objectID, xMsg.record_name(), nGMLevel);
	if (!pRecord)
	{
		m_pLogModule->LogError(nPlayerID, "Upload From Client int set record error " + xMsg.record_name(), __FUNCTION__, __LINE__);
		return;
	}

	if (pRecord->GetUpload() || nGMLevel > 0)
	{

		for (int i = 0; i < xMsg.property_list_size(); i++)
		{
			const SquickStruct::RecordInt &xRecordInt = xMsg.property_list().Get(i);
			pRecord->SetInt(xRecordInt.row(), xRecordInt.col(), xRecordInt.data());
			m_pLogModule->LogInfo(nPlayerID, "Upload From Client int set record " + xMsg.record_name(), __FUNCTION__, __LINE__);
		}
	}

}


void GameManagerModule::OnClientRecordFloatProcess(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len)
{
	CLIENT_MSG_PROCESS( msgID, msg, len, SquickStruct::ObjectRecordFloat)

	int nGMLevel = 0;
	const int scene = pObject->GetPropertyInt(SquickProtocol::IObject::SceneID());
	const int group = pObject->GetPropertyInt(SquickProtocol::IObject::GroupID());

	Guid objectID = INetModule::ProtobufToStruct(xMsg.player_id());
	auto pRecord = CalRecord(pObject, objectID, xMsg.record_name(), nGMLevel);
	if (!pRecord)
	{
		m_pLogModule->LogError(nPlayerID, "Upload From Client float set record error " + xMsg.record_name(), __FUNCTION__, __LINE__);
		return;
	}

	if (pRecord->GetUpload() || nGMLevel > 0)
	{

		for (int i = 0; i < xMsg.property_list_size(); i++)
		{
			const SquickStruct::RecordFloat &xRecordFloat = xMsg.property_list().Get(i);
			pRecord->SetFloat(xRecordFloat.row(), xRecordFloat.col(), xRecordFloat.data());
			m_pLogModule->LogInfo(nPlayerID, "Upload From Client float set record " + xMsg.record_name(), __FUNCTION__, __LINE__);
		}
	}

}

void GameManagerModule::OnClientRecordStringProcess(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len)
{
	CLIENT_MSG_PROCESS( msgID, msg, len, SquickStruct::ObjectRecordString)

	int nGMLevel = 0;
	const int scene = pObject->GetPropertyInt(SquickProtocol::IObject::SceneID());
	const int group = pObject->GetPropertyInt(SquickProtocol::IObject::GroupID());

	Guid objectID = INetModule::ProtobufToStruct(xMsg.player_id());
	auto pRecord = CalRecord(pObject, objectID, xMsg.record_name(), nGMLevel);
	if (!pRecord)
	{
		m_pLogModule->LogError(nPlayerID, "Upload From Client String set record error " + xMsg.record_name(), __FUNCTION__, __LINE__);
		return;
	}

	if (pRecord->GetUpload() || nGMLevel > 0)
	{

		for (int i = 0; i < xMsg.property_list_size(); i++)
		{
			const SquickStruct::RecordString &xRecordString = xMsg.property_list().Get(i);
			pRecord->SetString(xRecordString.row(), xRecordString.col(), xRecordString.data());
			m_pLogModule->LogInfo(nPlayerID, "Upload From Client String set record " + xMsg.record_name(), __FUNCTION__, __LINE__);
		}
	}

}

void GameManagerModule::OnClientRecordObjectProcess(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len)
{
	CLIENT_MSG_PROCESS( msgID, msg, len, SquickStruct::ObjectRecordObject)

	int nGMLevel = 0;
	const int scene = pObject->GetPropertyInt(SquickProtocol::IObject::SceneID());
	const int group = pObject->GetPropertyInt(SquickProtocol::IObject::GroupID());

	Guid objectID = INetModule::ProtobufToStruct(xMsg.player_id());
	auto pRecord = CalRecord(pObject, objectID, xMsg.record_name(), nGMLevel);
	if (!pRecord)
	{
		m_pLogModule->LogError(nPlayerID, "Upload From Client Object set record error " + xMsg.record_name(), __FUNCTION__, __LINE__);
		return;
	}

	if (pRecord->GetUpload() || nGMLevel > 0)
	{

		for (int i = 0; i < xMsg.property_list_size(); i++)
		{
			const SquickStruct::RecordObject &xRecordObject = xMsg.property_list().Get(i);
			pRecord->SetObject(xRecordObject.row(), xRecordObject.col(), INetModule::ProtobufToStruct(xRecordObject.data()));
			m_pLogModule->LogInfo(nPlayerID, "Upload From Client Object set record " + xMsg.record_name(), __FUNCTION__, __LINE__);
		}
	}
}

void GameManagerModule::OnClientRecordVector2Process(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len)
{
	CLIENT_MSG_PROCESS( msgID, msg, len, SquickStruct::ObjectRecordVector2)

	int nGMLevel = 0;
	const int scene = pObject->GetPropertyInt(SquickProtocol::IObject::SceneID());
	const int group = pObject->GetPropertyInt(SquickProtocol::IObject::GroupID());

	Guid objectID = INetModule::ProtobufToStruct(xMsg.player_id());
	auto pRecord = CalRecord(pObject, objectID, xMsg.record_name(), nGMLevel);
	if (!pRecord)
	{
		m_pLogModule->LogError(nPlayerID, "Upload From Client vector2 set record error " + xMsg.record_name(), __FUNCTION__, __LINE__);
		return;
	}
	if (pRecord->GetUpload() || nGMLevel > 0)
	{
		for (int i = 0; i < xMsg.property_list_size(); i++)
		{
			const SquickStruct::RecordVector2 &xRecordVector2 = xMsg.property_list().Get(i);
			pRecord->SetVector2(xRecordVector2.row(), xRecordVector2.col(), INetModule::ProtobufToStruct(xRecordVector2.data()));
			m_pLogModule->LogInfo(nPlayerID, "Upload From Client vector2 set record " + xMsg.record_name(), __FUNCTION__, __LINE__);
		}
	}
}

void GameManagerModule::OnClientRecordVector3Process(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len)
{
	CLIENT_MSG_PROCESS( msgID, msg, len, SquickStruct::ObjectRecordVector3)

	int nGMLevel = 0;
	const int scene = pObject->GetPropertyInt(SquickProtocol::IObject::SceneID());
	const int group = pObject->GetPropertyInt(SquickProtocol::IObject::GroupID());

	Guid objectID = INetModule::ProtobufToStruct(xMsg.player_id());
	auto pRecord = CalRecord(pObject, objectID, xMsg.record_name(), nGMLevel);
	if (!pRecord)
	{
		m_pLogModule->LogError(nPlayerID, "Upload From Client vector3 set record error " + xMsg.record_name(), __FUNCTION__, __LINE__);
		return;
	}

	if (pRecord->GetUpload() || nGMLevel > 0)
	{

		for (int i = 0; i < xMsg.property_list_size(); i++)
		{
			const SquickStruct::RecordVector3 &xRecordVector3 = xMsg.property_list().Get(i);
			pRecord->SetVector3(xRecordVector3.row(), xRecordVector3.col(), INetModule::ProtobufToStruct(xRecordVector3.data()));
			m_pLogModule->LogInfo(nPlayerID, "Upload From Client vector3 set record " + xMsg.record_name(), __FUNCTION__, __LINE__);
		}
	}
}

std::shared_ptr<IProperty> GameManagerModule::CalProperty(SQUICK_SHARE_PTR<IObject> pObject, const Guid objectID, const std::string& propertyName, int &gmLevel)
{
	const std::string& account = pObject->GetPropertyString(SquickProtocol::Player::Account());
	gmLevel = m_pElementModule->GetPropertyInt(account, SquickProtocol::GM::Level());
#ifdef SQUICK_DEBUG_MODE
	gmLevel = 2;
#endif

	const int scene = pObject->GetPropertyInt(SquickProtocol::IObject::SceneID());
	const int group = pObject->GetPropertyInt(SquickProtocol::IObject::GroupID());

	SQUICK_SHARE_PTR<IProperty> pProperty;
	if (!objectID.IsNull())
	{
		auto gameObject = m_pKernelModule->GetObject(objectID);
		if (gameObject)
		{
			pProperty = gameObject->GetPropertyManager()->GetElement(propertyName);
		}
	}
	else
	{
		auto propertyManager = m_pSceneModule->FindPropertyManager(scene, group);
		if (propertyManager)
		{
			pProperty = propertyManager->GetElement(propertyName);
		}
	}

	return pProperty;
}

std::shared_ptr<IRecord> GameManagerModule::CalRecord(SQUICK_SHARE_PTR<IObject> pObject, const Guid objectID, const std::string &recordName, int &gmLevel)
{
	const std::string& account = pObject->GetPropertyString(SquickProtocol::Player::Account());
	gmLevel = m_pElementModule->GetPropertyInt(account, SquickProtocol::GM::Level());
#ifdef SQUICK_DEBUG_MODE
	gmLevel = 1;
#endif

	const int scene = pObject->GetPropertyInt(SquickProtocol::IObject::SceneID());
	const int group = pObject->GetPropertyInt(SquickProtocol::IObject::GroupID());

	SQUICK_SHARE_PTR<IRecord> pRecord;
	if (!objectID.IsNull())
	{
		auto gameObject = m_pKernelModule->GetObject(objectID);
		if (gameObject)
		{
			pRecord = gameObject->GetRecordManager()->GetElement(recordName);
		}
	}
	else
	{
		auto recordManager = m_pSceneModule->FindRecordManager(scene, group);
		if (recordManager)
		{
			pRecord = recordManager->GetElement(recordName);
		}
	}

	return pRecord;
}
