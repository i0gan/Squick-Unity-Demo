

#include "auto_broadcast_module.h"
#include <squick/struct/protocol_define.h>
#include <squick/plugin/kernel/i_event_module.h>
#include <server/db/logic/common_redis_module.h>


bool AutoBroadcastModule::Start()
{
	m_pKernelModule = pPluginManager->FindModule<IKernelModule>();
	m_pClassModule = pPluginManager->FindModule<IClassModule>();
	m_pSceneProcessModule = pPluginManager->FindModule<ISceneProcessModule>();
	m_pElementModule = pPluginManager->FindModule<IElementModule>();
	m_pLogModule = pPluginManager->FindModule<ILogModule>();
	m_pEventModule = pPluginManager->FindModule<IEventModule>();
	m_pSceneModule = pPluginManager->FindModule<ISceneModule>();

	m_pNetModule = pPluginManager->FindModule<INetModule>();
	m_pNetClientModule = pPluginManager->FindModule<INetClientModule>();
	m_pGameServerNet_ServerModule = pPluginManager->FindModule<IGameServerNet_ServerModule>();
	
	return true;
}

bool AutoBroadcastModule::AfterStart()
{
	m_pKernelModule->AddClassCallBack(SquickProtocol::Player::ThisName(), this, &AutoBroadcastModule::OnObjectClassEvent);

	m_pSceneModule->AddObjectEnterCallBack(this, &AutoBroadcastModule::OnObjectListEnter);
	m_pSceneModule->AddObjectDataFinishedCallBack(this, &AutoBroadcastModule::OnObjectDataFinished);
	m_pSceneModule->AddObjectLeaveCallBack(this, &AutoBroadcastModule::OnObjectListLeave);
	m_pSceneModule->AddPropertyEnterCallBack(this, &AutoBroadcastModule::OnPropertyEnter);
	m_pSceneModule->AddRecordEnterCallBack(this, &AutoBroadcastModule::OnRecordEnter);

	m_pSceneModule->AddPropertyEventCallBack(this, &AutoBroadcastModule::OnPropertyEvent);
	m_pSceneModule->AddRecordEventCallBack(this, &AutoBroadcastModule::OnRecordEvent);

	m_pSceneModule->AddSwapSceneEventCallBack(this, &AutoBroadcastModule::OnSceneEvent);

	return true;
}

bool AutoBroadcastModule::Destory()
{

	return true;
}

bool AutoBroadcastModule::Update()
{
	return true;
}


// 广播对象
int AutoBroadcastModule::OnObjectClassEvent(const Guid& self, const std::string& className, const CLASS_OBJECT_EVENT classEvent, const DataList& var)
{
	if (CLASS_OBJECT_EVENT::COE_CREATE_NODATA == classEvent)
	{
		//only just to tell client, now player can enter world(without data) and you can start to load scene or mesh
		SQUICK_SHARE_PTR<IGameServerNet_ServerModule::GateBaseInfo> pDataBase = m_pGameServerNet_ServerModule->GetPlayerGateInfo(self);
		if (pDataBase)
		{
			SquickStruct::AckEventResult xMsg;
			xMsg.set_event_code(SquickStruct::EGameEventCode::SUCCESS);

			*xMsg.mutable_event_client() = INetModule::StructToProtobuf(pDataBase->xClientID);
			*xMsg.mutable_event_object() = INetModule::StructToProtobuf(self);
			
			m_pGameServerNet_ServerModule->SendMsgPBToGate(SquickStruct::ACK_ENTER_GAME, xMsg, self);
		}
	}

	return 0;
}

int AutoBroadcastModule::OnSceneEvent(const Guid & self, const int sceneID, const int groupID, const int type, const DataList& argList)
{
	Vector3 vRelivePos = m_pSceneModule->GetRelivePosition(sceneID);

	SquickStruct::ReqAckSwapScene xAckSwapScene;
	xAckSwapScene.set_scene_id(sceneID);
	xAckSwapScene.set_transfer_type(type);
	xAckSwapScene.set_line_id(0);
	xAckSwapScene.set_x(vRelivePos.X());
	xAckSwapScene.set_y(vRelivePos.Y());
	xAckSwapScene.set_z(vRelivePos.Z());
	xAckSwapScene.set_data("");

	//buildings

	m_pGameServerNet_ServerModule->SendMsgPBToGate(SquickStruct::ACK_SWAP_SCENE, xAckSwapScene, self);

	return 0;
}

int AutoBroadcastModule::OnPropertyEnter(const DataList& argVar, const Guid& self)
{
	if (argVar.GetCount() <= 0 || self.IsNull())
	{
		return 0;
	}

	SquickStruct::MultiObjectPropertyList xPublicMsg;
	SquickStruct::MultiObjectPropertyList xPrivateMsg;
	SQUICK_SHARE_PTR<IObject> pObject = m_pKernelModule->GetObject(self);
	if (pObject)
	{
		SquickStruct::ObjectPropertyList* pPublicData = xPublicMsg.add_multi_player_property();
		SquickStruct::ObjectPropertyList* pPrivateData = xPrivateMsg.add_multi_player_property();

		*(pPublicData->mutable_player_id()) = INetModule::StructToProtobuf(self);
		*(pPrivateData->mutable_player_id()) = INetModule::StructToProtobuf(self);

		SQUICK_SHARE_PTR<IPropertyManager> pPropertyManager = pObject->GetPropertyManager();
		//std::cout << "OnPropertyEnter auto " << pPropertyManager->Self().ToString() << std::endl;
		//std::cout << pPropertyManager->ToString() << std::endl;

		SQUICK_SHARE_PTR<IProperty> pPropertyInfo = pPropertyManager->First();
		while (pPropertyInfo)
		{
			if (pPropertyInfo->Changed())
			{
				switch (pPropertyInfo->GetType())
				{
				case TDATA_INT:
				{
					if (pPropertyInfo->GetPublic())
					{
						SquickStruct::PropertyInt* pDataInt = pPublicData->add_property_int_list();
						pDataInt->set_property_name(pPropertyInfo->GetKey());
						pDataInt->set_data(pPropertyInfo->GetInt());
					}

					if (pPropertyInfo->GetPrivate())
					{
						SquickStruct::PropertyInt* pDataInt = pPrivateData->add_property_int_list();
						pDataInt->set_property_name(pPropertyInfo->GetKey());
						pDataInt->set_data(pPropertyInfo->GetInt());
					}
				}
				break;

				case TDATA_FLOAT:
				{
					if (pPropertyInfo->GetPublic())
					{
						SquickStruct::PropertyFloat* pDataFloat = pPublicData->add_property_float_list();
						pDataFloat->set_property_name(pPropertyInfo->GetKey());
						pDataFloat->set_data(pPropertyInfo->GetFloat());
					}

					if (pPropertyInfo->GetPrivate())
					{
						SquickStruct::PropertyFloat* pDataFloat = pPrivateData->add_property_float_list();
						pDataFloat->set_property_name(pPropertyInfo->GetKey());
						pDataFloat->set_data(pPropertyInfo->GetFloat());
					}
				}
				break;

				case TDATA_STRING:
				{
					if (pPropertyInfo->GetPublic())
					{
						SquickStruct::PropertyString* pDataString = pPublicData->add_property_string_list();
						pDataString->set_property_name(pPropertyInfo->GetKey());
						pDataString->set_data(pPropertyInfo->GetString());
					}

					if (pPropertyInfo->GetPrivate())
					{
						SquickStruct::PropertyString* pDataString = pPrivateData->add_property_string_list();
						pDataString->set_property_name(pPropertyInfo->GetKey());
						pDataString->set_data(pPropertyInfo->GetString());
					}
				}
				break;

				case TDATA_OBJECT:
				{
					if (pPropertyInfo->GetPublic())
					{
						SquickStruct::PropertyObject* pDataObject = pPublicData->add_property_object_list();
						pDataObject->set_property_name(pPropertyInfo->GetKey());
						*(pDataObject->mutable_data()) = INetModule::StructToProtobuf(pPropertyInfo->GetObject());
					}

					if (pPropertyInfo->GetPrivate())
					{
						SquickStruct::PropertyObject* pDataObject = pPrivateData->add_property_object_list();
						pDataObject->set_property_name(pPropertyInfo->GetKey());
						*(pDataObject->mutable_data()) = INetModule::StructToProtobuf(pPropertyInfo->GetObject());
					}
				}
				break;

				case TDATA_VECTOR2:
				{
					if (pPropertyInfo->GetPublic())
					{
						SquickStruct::PropertyVector2* pDataObject = pPublicData->add_property_vector2_list();
						pDataObject->set_property_name(pPropertyInfo->GetKey());
						*(pDataObject->mutable_data()) = INetModule::StructToProtobuf(pPropertyInfo->GetVector2());
					}

					if (pPropertyInfo->GetPrivate())
					{
						SquickStruct::PropertyVector2* pDataObject = pPrivateData->add_property_vector2_list();
						pDataObject->set_property_name(pPropertyInfo->GetKey());
						*(pDataObject->mutable_data()) = INetModule::StructToProtobuf(pPropertyInfo->GetVector2());
					}
				}
				break;
				case TDATA_VECTOR3:
				{
					if (pPropertyInfo->GetPublic())
					{
						SquickStruct::PropertyVector3* pDataObject = pPublicData->add_property_vector3_list();
						pDataObject->set_property_name(pPropertyInfo->GetKey());
						*(pDataObject->mutable_data()) = INetModule::StructToProtobuf(pPropertyInfo->GetVector3());
					}

					if (pPropertyInfo->GetPrivate())
					{
						SquickStruct::PropertyVector3* pDataObject = pPrivateData->add_property_vector3_list();
						pDataObject->set_property_name(pPropertyInfo->GetKey());
						*(pDataObject->mutable_data()) = INetModule::StructToProtobuf(pPropertyInfo->GetVector3());
					}
				}
				break;
				default:
					break;
				}
			}

			pPropertyInfo = pPropertyManager->Next();
		}

		for (int i = 0; i < argVar.GetCount(); i++)
		{
			Guid identOther = argVar.Object(i);
			if (self == identOther)
			{
				m_pGameServerNet_ServerModule->SendMsgPBToGate(SquickStruct::ACK_OBJECT_PROPERTY_ENTRY, xPrivateMsg, identOther);
			}
			else
			{
				m_pGameServerNet_ServerModule->SendMsgPBToGate(SquickStruct::ACK_OBJECT_PROPERTY_ENTRY, xPublicMsg, identOther);
			}
		}
	}

	return 0;
}

int AutoBroadcastModule::OnRecordEnter(const DataList& argVar, const Guid& self)
{
	if (argVar.GetCount() <= 0 || self.IsNull())
	{
		return 0;
	}

	SquickStruct::MultiObjectRecordList xPublicMsg;
	SquickStruct::MultiObjectRecordList xPrivateMsg;

	SQUICK_SHARE_PTR<IObject> pObject = m_pKernelModule->GetObject(self);
	if (pObject)
	{
		SquickStruct::ObjectRecordList* pPublicData = NULL;
		SquickStruct::ObjectRecordList* pPrivateData = NULL;

		SQUICK_SHARE_PTR<IRecordManager> pRecordManager = pObject->GetRecordManager();
		SQUICK_SHARE_PTR<IRecord> pRecord = pRecordManager->First();
		while (pRecord)
		{
			if (!pRecord->GetPublic() && !pRecord->GetPrivate())
			{
				pRecord = pRecordManager->Next();
				continue;
			}

			SquickStruct::ObjectRecordBase* pPrivateRecordBase = NULL;
			SquickStruct::ObjectRecordBase* pPublicRecordBase = NULL;
			if (pRecord->GetPublic())
			{
				if (!pPublicData)
				{
					pPublicData = xPublicMsg.add_multi_player_record();
					*(pPublicData->mutable_player_id()) = INetModule::StructToProtobuf(self);
				}
				pPublicRecordBase = pPublicData->add_record_list();
				pPublicRecordBase->set_record_name(pRecord->GetName());

				CommonRedisModule::ConvertRecordToPB(pRecord, pPublicRecordBase);
			}

			if (pRecord->GetPrivate())
			{
				if (!pPrivateData)
				{
					pPrivateData = xPrivateMsg.add_multi_player_record();
					*(pPrivateData->mutable_player_id()) = INetModule::StructToProtobuf(self);
				}
				pPrivateRecordBase = pPrivateData->add_record_list();
				pPrivateRecordBase->set_record_name(pRecord->GetName());

				CommonRedisModule::ConvertRecordToPB(pRecord, pPrivateRecordBase);
			}

			pRecord = pRecordManager->Next();
		}


		for (int i = 0; i < argVar.GetCount(); i++)
		{
			Guid identOther = argVar.Object(i);
			if (self == identOther)
			{
				if (xPrivateMsg.multi_player_record_size() > 0)
				{
					m_pGameServerNet_ServerModule->SendMsgPBToGate(SquickStruct::ACK_OBJECT_RECORD_ENTRY, xPrivateMsg, identOther);
				}
			}
			else
			{
				if (xPublicMsg.multi_player_record_size() > 0)
				{
					m_pGameServerNet_ServerModule->SendMsgPBToGate(SquickStruct::ACK_OBJECT_RECORD_ENTRY, xPublicMsg, identOther);
				}
			}
		}
	}

	return 0;
}

int AutoBroadcastModule::OnPropertyEvent(const Guid & self, const std::string & propertyName, const SquickData & oldVar, const SquickData & newVar, const DataList & argVar, const INT64 reason)
{
	if (SquickProtocol::Player::ThisName() == m_pKernelModule->GetPropertyString(self, SquickProtocol::Player::ClassName()))
	{
		SQUICK_SHARE_PTR<IObject> xObject = m_pKernelModule->GetObject(self);
		if (xObject->GetState() != CLASS_OBJECT_EVENT::COE_CREATE_FINISH)
		{
			return 0;
		}
	}

	switch (oldVar.GetType())
	{
	case TDATA_INT:
	{
		SquickStruct::ObjectPropertyInt xPropertyInt;
		SquickStruct::Ident* pIdent = xPropertyInt.mutable_player_id();
		*pIdent = INetModule::StructToProtobuf(self);

		SquickStruct::PropertyInt* pDataInt = xPropertyInt.add_property_list();
		pDataInt->set_property_name(propertyName);
		pDataInt->set_data(newVar.GetInt());
		pDataInt->set_reason(reason);

		for (int i = 0; i < argVar.GetCount(); i++)
		{
			Guid identOld = argVar.Object(i);

			m_pGameServerNet_ServerModule->SendMsgPBToGate(SquickStruct::ACK_PROPERTY_INT, xPropertyInt, identOld);
		}
	}
	break;

	case TDATA_FLOAT:
	{
		SquickStruct::ObjectPropertyFloat xPropertyFloat;
		SquickStruct::Ident* pIdent = xPropertyFloat.mutable_player_id();
		*pIdent = INetModule::StructToProtobuf(self);

		SquickStruct::PropertyFloat* pDataFloat = xPropertyFloat.add_property_list();
		pDataFloat->set_property_name(propertyName);
		pDataFloat->set_data(newVar.GetFloat());
		pDataFloat->set_reason(reason);

		for (int i = 0; i < argVar.GetCount(); i++)
		{
			Guid identOld = argVar.Object(i);

			m_pGameServerNet_ServerModule->SendMsgPBToGate(SquickStruct::ACK_PROPERTY_FLOAT, xPropertyFloat, identOld);
		}
	}
	break;

	case TDATA_STRING:
	{
		SquickStruct::ObjectPropertyString xPropertyString;
		SquickStruct::Ident* pIdent = xPropertyString.mutable_player_id();
		*pIdent = INetModule::StructToProtobuf(self);

		SquickStruct::PropertyString* pDataString = xPropertyString.add_property_list();
		pDataString->set_property_name(propertyName);
		pDataString->set_data(newVar.GetString());
		pDataString->set_reason(reason);

		for (int i = 0; i < argVar.GetCount(); i++)
		{
			Guid identOld = argVar.Object(i);

			m_pGameServerNet_ServerModule->SendMsgPBToGate(SquickStruct::ACK_PROPERTY_STRING, xPropertyString, identOld);
		}
	}
	break;

	case TDATA_OBJECT:
	{
		SquickStruct::ObjectPropertyObject xPropertyObject;
		SquickStruct::Ident* pIdent = xPropertyObject.mutable_player_id();
		*pIdent = INetModule::StructToProtobuf(self);

		SquickStruct::PropertyObject* pDataObject = xPropertyObject.add_property_list();
		pDataObject->set_property_name(propertyName);
		*pDataObject->mutable_data() = INetModule::StructToProtobuf(newVar.GetObject());
		pDataObject->set_reason(reason);

		for (int i = 0; i < argVar.GetCount(); i++)
		{
			Guid identOld = argVar.Object(i);

			m_pGameServerNet_ServerModule->SendMsgPBToGate(SquickStruct::ACK_PROPERTY_OBJECT, xPropertyObject, identOld);
		}
	}
	break;
	case TDATA_VECTOR2:
	{
		SquickStruct::ObjectPropertyVector2 xPropertyVector2;
		SquickStruct::Ident* pIdent = xPropertyVector2.mutable_player_id();
		*pIdent = INetModule::StructToProtobuf(self);

		SquickStruct::PropertyVector2* pDataObject = xPropertyVector2.add_property_list();
		pDataObject->set_property_name(propertyName);
		*pDataObject->mutable_data() = INetModule::StructToProtobuf(newVar.GetVector2());
		pDataObject->set_reason(reason);

		for (int i = 0; i < argVar.GetCount(); i++)
		{
			Guid identOld = argVar.Object(i);

			m_pGameServerNet_ServerModule->SendMsgPBToGate(SquickStruct::ACK_PROPERTY_VECTOR2, xPropertyVector2, identOld);
		}
	}
	break;
	case TDATA_VECTOR3:
	{
		SquickStruct::ObjectPropertyVector3 xPropertyVector3;
		SquickStruct::Ident* pIdent = xPropertyVector3.mutable_player_id();
		*pIdent = INetModule::StructToProtobuf(self);

		SquickStruct::PropertyVector3* pDataObject = xPropertyVector3.add_property_list();
		pDataObject->set_property_name(propertyName);
		*pDataObject->mutable_data() = INetModule::StructToProtobuf(newVar.GetVector3());
		pDataObject->set_reason(reason);

		for (int i = 0; i < argVar.GetCount(); i++)
		{
			Guid identOld = argVar.Object(i);

			m_pGameServerNet_ServerModule->SendMsgPBToGate(SquickStruct::ACK_PROPERTY_VECTOR3, xPropertyVector3, identOld);
		}
	}
	break;
	default:
		break;
	}

	return 0;
}

int AutoBroadcastModule::OnRecordEvent(const Guid & self, const std::string& recordName, const RECORD_EVENT_DATA & eventData, const SquickData & oldVar, const SquickData & newVar, const DataList & argVar)
{
	if (SquickProtocol::Player::ThisName() == m_pKernelModule->GetPropertyString(self, SquickProtocol::Player::ClassName()))
	{
		SQUICK_SHARE_PTR<IObject> xObject = m_pKernelModule->GetObject(self);
		if (xObject->GetState() != CLASS_OBJECT_EVENT::COE_CREATE_FINISH)
		{
			return 0;
		}
	}

	switch (eventData.nOpType)
	{
	case RECORD_EVENT_DATA::Add:
	{
		SquickStruct::ObjectRecordAddRow xAddRecordRow;
		SquickStruct::Ident* pIdent = xAddRecordRow.mutable_player_id();
		*pIdent = INetModule::StructToProtobuf(self);

		xAddRecordRow.set_record_name(recordName);

		SquickStruct::RecordAddRowStruct* pAddRowData = xAddRecordRow.add_row_data();
		pAddRowData->set_row(eventData.row);


		SQUICK_SHARE_PTR<IRecord> xRecord = m_pKernelModule->FindRecord(self, recordName);
		if (xRecord)
		{
			DataList rowDataList;
			if (xRecord->QueryRow(eventData.row, rowDataList))
			{
				for (int i = 0; i < rowDataList.GetCount(); i++)
				{
					switch (rowDataList.Type(i))
					{
					case TDATA_INT:
					{

						int64_t nValue = rowDataList.Int(i);

						SquickStruct::RecordInt* pAddData = pAddRowData->add_record_int_list();
						pAddData->set_col(i);
						pAddData->set_row(eventData.row);
						pAddData->set_data(nValue);
					}
					break;
					case TDATA_FLOAT:
					{
						double fValue = rowDataList.Float(i);

						SquickStruct::RecordFloat* pAddData = pAddRowData->add_record_float_list();
						pAddData->set_col(i);
						pAddData->set_row(eventData.row);
						pAddData->set_data(fValue);
					}
					break;
					case TDATA_STRING:
					{
						const std::string& str = rowDataList.String(i);
						SquickStruct::RecordString* pAddData = pAddRowData->add_record_string_list();
						pAddData->set_col(i);
						pAddData->set_row(eventData.row);
						pAddData->set_data(str);
					}
					break;
					case TDATA_OBJECT:
					{
						Guid identValue = rowDataList.Object(i);
						SquickStruct::RecordObject* pAddData = pAddRowData->add_record_object_list();
						pAddData->set_col(i);
						pAddData->set_row(eventData.row);

						*pAddData->mutable_data() = INetModule::StructToProtobuf(identValue);
					}
					break;
					case TDATA_VECTOR2:
					{
						Vector2 vPos = rowDataList.Vector2At(i);
						SquickStruct::RecordVector2* pAddData = pAddRowData->add_record_vector2_list();
						pAddData->set_col(i);
						pAddData->set_row(eventData.row);
						*pAddData->mutable_data() = INetModule::StructToProtobuf(vPos);
					}
					break;
					case TDATA_VECTOR3:
					{
						Vector3 vPos = rowDataList.Vector3At(i);
						SquickStruct::RecordVector3* pAddData = pAddRowData->add_record_vector3_list();
						pAddData->set_col(i);
						pAddData->set_row(eventData.row);
						*pAddData->mutable_data() = INetModule::StructToProtobuf(vPos);
					}
					break;

					default:
						break;
					}
				}

				for (int i = 0; i < argVar.GetCount(); i++)
				{
					Guid identOther = argVar.Object(i);

					m_pGameServerNet_ServerModule->SendMsgPBToGate(SquickStruct::ACK_ADD_ROW, xAddRecordRow, identOther);
				}
			}
		}
	}
	break;
	case RECORD_EVENT_DATA::Del:
	{
		SquickStruct::ObjectRecordRemove xReoveRecordRow;

		SquickStruct::Ident* pIdent = xReoveRecordRow.mutable_player_id();
		*pIdent = INetModule::StructToProtobuf(self);

		xReoveRecordRow.set_record_name(recordName);
		xReoveRecordRow.add_remove_row(eventData.row);

		for (int i = 0; i < argVar.GetCount(); i++)
		{
			Guid identOther = argVar.Object(i);

			m_pGameServerNet_ServerModule->SendMsgPBToGate(SquickStruct::ACK_REMOVE_ROW, xReoveRecordRow, identOther);
		}
	}
	break;
	case RECORD_EVENT_DATA::Swap:
	{

		SquickStruct::ObjectRecordSwap xSwapRecord;
		*xSwapRecord.mutable_player_id() = INetModule::StructToProtobuf(self);

		xSwapRecord.set_origin_record_name(recordName);
		xSwapRecord.set_target_record_name(recordName);
		xSwapRecord.set_row_origin(eventData.row);
		xSwapRecord.set_row_target(eventData.col);

		for (int i = 0; i < argVar.GetCount(); i++)
		{
			Guid identOther = argVar.Object(i);

			m_pGameServerNet_ServerModule->SendMsgPBToGate(SquickStruct::ACK_SWAP_ROW, xSwapRecord, identOther);
		}
	}
	break;
	case RECORD_EVENT_DATA::Update:
	{
		switch (oldVar.GetType())
		{
		case TDATA_INT:
		{
			SquickStruct::ObjectRecordInt xRecordChanged;
			*xRecordChanged.mutable_player_id() = INetModule::StructToProtobuf(self);

			xRecordChanged.set_record_name(recordName);
			SquickStruct::RecordInt* recordProperty = xRecordChanged.add_property_list();
			recordProperty->set_row(eventData.row);
			recordProperty->set_col(eventData.col);
			int64_t nData = newVar.GetInt();
			recordProperty->set_data(nData);

			for (int i = 0; i < argVar.GetCount(); i++)
			{
				Guid identOther = argVar.Object(i);

				m_pGameServerNet_ServerModule->SendMsgPBToGate(SquickStruct::ACK_RECORD_INT, xRecordChanged, identOther);
			}
		}
		break;

		case TDATA_FLOAT:
		{
			SquickStruct::ObjectRecordFloat xRecordChanged;
			*xRecordChanged.mutable_player_id() = INetModule::StructToProtobuf(self);

			xRecordChanged.set_record_name(recordName);
			SquickStruct::RecordFloat* recordProperty = xRecordChanged.add_property_list();
			recordProperty->set_row(eventData.row);
			recordProperty->set_col(eventData.col);
			recordProperty->set_data(newVar.GetFloat());

			for (int i = 0; i < argVar.GetCount(); i++)
			{
				Guid identOther = argVar.Object(i);

				m_pGameServerNet_ServerModule->SendMsgPBToGate(SquickStruct::ACK_RECORD_FLOAT, xRecordChanged, identOther);
			}
		}
		break;
		case TDATA_STRING:
		{
			SquickStruct::ObjectRecordString xRecordChanged;
			*xRecordChanged.mutable_player_id() = INetModule::StructToProtobuf(self);

			xRecordChanged.set_record_name(recordName);
			SquickStruct::RecordString* recordProperty = xRecordChanged.add_property_list();
			recordProperty->set_row(eventData.row);
			recordProperty->set_col(eventData.col);
			recordProperty->set_data(newVar.GetString());

			for (int i = 0; i < argVar.GetCount(); i++)
			{
				Guid identOther = argVar.Object(i);

				m_pGameServerNet_ServerModule->SendMsgPBToGate(SquickStruct::ACK_RECORD_STRING, xRecordChanged, identOther);
			}
		}
		break;
		case TDATA_OBJECT:
		{
			SquickStruct::ObjectRecordObject xRecordChanged;
			*xRecordChanged.mutable_player_id() = INetModule::StructToProtobuf(self);

			xRecordChanged.set_record_name(recordName);
			SquickStruct::RecordObject* recordProperty = xRecordChanged.add_property_list();
			recordProperty->set_row(eventData.row);
			recordProperty->set_col(eventData.col);
			*recordProperty->mutable_data() = INetModule::StructToProtobuf(newVar.GetObject());

			for (int i = 0; i < argVar.GetCount(); i++)
			{
				Guid identOther = argVar.Object(i);

				m_pGameServerNet_ServerModule->SendMsgPBToGate(SquickStruct::ACK_RECORD_OBJECT, xRecordChanged, identOther);
			}
		}
		break;
		case TDATA_VECTOR2:
		{
			SquickStruct::ObjectRecordVector2 xRecordChanged;
			*xRecordChanged.mutable_player_id() = INetModule::StructToProtobuf(self);

			xRecordChanged.set_record_name(recordName);
			SquickStruct::RecordVector2* recordProperty = xRecordChanged.add_property_list();
			recordProperty->set_row(eventData.row);
			recordProperty->set_col(eventData.col);
			*recordProperty->mutable_data() = INetModule::StructToProtobuf(newVar.GetVector2());

			for (int i = 0; i < argVar.GetCount(); i++)
			{
				Guid identOther = argVar.Object(i);

				m_pGameServerNet_ServerModule->SendMsgPBToGate(SquickStruct::ACK_RECORD_VECTOR2, xRecordChanged, identOther);
			}
		}
		break;
		case TDATA_VECTOR3:
		{
			SquickStruct::ObjectRecordVector3 xRecordChanged;
			*xRecordChanged.mutable_player_id() = INetModule::StructToProtobuf(self);

			xRecordChanged.set_record_name(recordName);
			SquickStruct::RecordVector3* recordProperty = xRecordChanged.add_property_list();
			recordProperty->set_row(eventData.row);
			recordProperty->set_col(eventData.col);
			*recordProperty->mutable_data() = INetModule::StructToProtobuf(newVar.GetVector3());

			for (int i = 0; i < argVar.GetCount(); i++)
			{
				Guid identOther = argVar.Object(i);

				m_pGameServerNet_ServerModule->SendMsgPBToGate(SquickStruct::ACK_RECORD_VECTOR3, xRecordChanged, identOther);
			}
		}
		break;
		default:
			return 0;
			break;
		}
	}
	break;
	case RECORD_EVENT_DATA::Create:
		return 0;
		break;
	case RECORD_EVENT_DATA::Cleared:
	{
		//             SquickStruct::ObjectRecordObject xRecordChanged;
		//             xRecordChanged.set_player_id( self.nData64 );
		//             xRecordChanged.set_record_name( recordName );
		//
		//             for ( int i = 0; i < valueBroadCaseList.GetCount(); i++ )
		//             {
		//                 Guid identOther = valueBroadCaseList.Object( i );
		//                 SendMsgPB(SquickStruct::ACK_RECORD_CLEAR, xRecordChanged, 0);
		//             }
	}
	break;
	default:
		break;
	}
	return 0;
}

int AutoBroadcastModule::OnObjectListEnter(const DataList& self, const DataList& argVar)
{
	if (self.GetCount() <= 0 || argVar.GetCount() <= 0)
	{
		return 0;
	}

	SquickStruct::AckPlayerEntryList xPlayerEntryInfoList;
	for (int i = 0; i < argVar.GetCount(); i++)
	{
		Guid identOld = argVar.Object(i);

		if (identOld.IsNull())
		{
			continue;
		}

		SquickStruct::PlayerEntryInfo* pEntryInfo = xPlayerEntryInfoList.add_object_list();
		*(pEntryInfo->mutable_object_guid()) = INetModule::StructToProtobuf(identOld);

		Vector3 vPos = m_pKernelModule->GetPropertyVector3(identOld, SquickProtocol::IObject::Position());
		pEntryInfo->set_x(vPos.X());
		pEntryInfo->set_y(vPos.Y());
		pEntryInfo->set_z(vPos.Z());
		pEntryInfo->set_career_type(m_pKernelModule->GetPropertyInt32(identOld, SquickProtocol::Player::Job()));
		pEntryInfo->set_player_state(0);
		pEntryInfo->set_config_id(m_pKernelModule->GetPropertyString(identOld, SquickProtocol::Player::ConfigID()));
		pEntryInfo->set_scene_id(m_pKernelModule->GetPropertyInt32(identOld, SquickProtocol::Player::SceneID()));
		pEntryInfo->set_class_id(m_pKernelModule->GetPropertyString(identOld, SquickProtocol::Player::ClassName()));

	}

	if (xPlayerEntryInfoList.object_list_size() <= 0)
	{
		return 0;
	}

	for (int i = 0; i < self.GetCount(); i++)
	{
		Guid ident = self.Object(i);
		if (ident.IsNull())
		{
			continue;
		}


		m_pGameServerNet_ServerModule->SendMsgPBToGate(SquickStruct::ACK_OBJECT_ENTRY, xPlayerEntryInfoList, ident);
	}

	return 1;
}

int AutoBroadcastModule::OnObjectDataFinished(const DataList & self, const DataList & argVar)
{
	if (self.GetCount() <= 0 || argVar.GetCount() <= 0)
	{
		return 0;
	}

	SquickStruct::AckPlayerEntryList xPlayerEntryInfoList;
	for (int i = 0; i < argVar.GetCount(); i++)
	{
		Guid identOld = argVar.Object(i);

		if (identOld.IsNull())
		{
			continue;
		}

		SquickStruct::PlayerEntryInfo* pEntryInfo = xPlayerEntryInfoList.add_object_list();
		*(pEntryInfo->mutable_object_guid()) = INetModule::StructToProtobuf(identOld);

		Vector3 vPos = m_pKernelModule->GetPropertyVector3(identOld, SquickProtocol::IObject::Position());
		pEntryInfo->set_x(vPos.X());
		pEntryInfo->set_y(vPos.Y());
		pEntryInfo->set_z(vPos.Z());
		pEntryInfo->set_career_type(m_pKernelModule->GetPropertyInt32(identOld, SquickProtocol::Player::Job()));
		pEntryInfo->set_player_state(0);
		pEntryInfo->set_config_id(m_pKernelModule->GetPropertyString(identOld, SquickProtocol::Player::ConfigID()));
		pEntryInfo->set_scene_id(m_pKernelModule->GetPropertyInt32(identOld, SquickProtocol::Player::SceneID()));
		pEntryInfo->set_class_id(m_pKernelModule->GetPropertyString(identOld, SquickProtocol::Player::ClassName()));

	}

	if (xPlayerEntryInfoList.object_list_size() <= 0)
	{
		return 0;
	}

	for (int i = 0; i < self.GetCount(); i++)
	{
		Guid ident = self.Object(i);
		if (ident.IsNull())
		{
			continue;
		}


		m_pGameServerNet_ServerModule->SendMsgPBToGate(SquickStruct::ACK_DATA_FINISHED, xPlayerEntryInfoList, ident);
	}

	return 1;
}

int AutoBroadcastModule::OnObjectListLeave(const DataList& self, const DataList& argVar)
{
	if (self.GetCount() <= 0 || argVar.GetCount() <= 0)
	{
		return 0;
	}

	SquickStruct::AckPlayerLeaveList xPlayerLeaveInfoList;
	for (int i = 0; i < argVar.GetCount(); i++)
	{
		Guid identOld = argVar.Object(i);

		if (identOld.IsNull())
		{
			continue;
		}

		SquickStruct::Ident* pIdent = xPlayerLeaveInfoList.add_object_list();
		*pIdent = INetModule::StructToProtobuf(argVar.Object(i));
	}

	for (int i = 0; i < self.GetCount(); i++)
	{
		Guid ident = self.Object(i);
		if (ident.IsNull())
		{
			continue;
		}

		m_pGameServerNet_ServerModule->SendMsgPBToGate(SquickStruct::ACK_OBJECT_LEAVE, xPlayerLeaveInfoList, ident);
	}

	return 1;
}
