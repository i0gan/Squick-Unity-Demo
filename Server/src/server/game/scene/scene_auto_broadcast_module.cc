

#include "scene_auto_broadcast_module.h"
#include <squick/struct/protocol_define.h>
//#include "squick/base/event.h"

bool SceneAutoBroadcastModule::Start()
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

bool SceneAutoBroadcastModule::AfterStart()
{
	m_pSceneModule->AddGroupPropertyCommCallBack(this, &SceneAutoBroadcastModule::OnPropertyEvent);
	m_pSceneModule->AddGroupRecordCommCallBack(this, &SceneAutoBroadcastModule::OnRecordEvent);

	m_pSceneModule->AddBeforeLeaveSceneGroupCallBack(this, &SceneAutoBroadcastModule::OnBeforeLeaveSceneEvent);
	m_pSceneModule->AddAfterEnterSceneGroupCallBack(this, &SceneAutoBroadcastModule::OnAfterEntrySceneEvent);
	return true;
}

bool SceneAutoBroadcastModule::Destory()
{
	return true;
}

bool SceneAutoBroadcastModule::Update()
{
	return true;
}

int SceneAutoBroadcastModule::OnPropertyEvent(const Guid & self, const std::string & propertyName, const SquickData & oldVar, const SquickData & newVar)
{
	DataList players;
	m_pKernelModule->GetGroupObjectList(self.nHead64, self.nData64, players, true);

	switch (oldVar.GetType())
	{
	case TDATA_INT:
	{
		SquickStruct::ObjectPropertyInt xPropertyInt;
		SquickStruct::Ident* pIdent = xPropertyInt.mutable_player_id();
		*pIdent = INetModule::StructToProtobuf(Guid());

		SquickStruct::PropertyInt* pDataInt = xPropertyInt.add_property_list();
		pDataInt->set_property_name(propertyName);
		pDataInt->set_data(newVar.GetInt());

		for (int i = 0; i < players.GetCount(); i++)
		{
			Guid identOld = players.Object(i);

			m_pGameServerNet_ServerModule->SendMsgPBToGate(SquickStruct::ACK_PROPERTY_INT, xPropertyInt, identOld);
		}
	}
	break;

	case TDATA_FLOAT:
	{
		SquickStruct::ObjectPropertyFloat xPropertyFloat;
		SquickStruct::Ident* pIdent = xPropertyFloat.mutable_player_id();
		*pIdent = INetModule::StructToProtobuf(Guid());

		SquickStruct::PropertyFloat* pDataFloat = xPropertyFloat.add_property_list();
		pDataFloat->set_property_name(propertyName);
		pDataFloat->set_data(newVar.GetFloat());

		for (int i = 0; i < players.GetCount(); i++)
		{
			Guid identOld = players.Object(i);

			m_pGameServerNet_ServerModule->SendMsgPBToGate(SquickStruct::ACK_PROPERTY_FLOAT, xPropertyFloat, identOld);
		}
	}
	break;

	case TDATA_STRING:
	{
		SquickStruct::ObjectPropertyString xPropertyString;
		SquickStruct::Ident* pIdent = xPropertyString.mutable_player_id();
		*pIdent = INetModule::StructToProtobuf(Guid());

		SquickStruct::PropertyString* pDataString = xPropertyString.add_property_list();
		pDataString->set_property_name(propertyName);
		pDataString->set_data(newVar.GetString());

		for (int i = 0; i < players.GetCount(); i++)
		{
			Guid identOld = players.Object(i);

			m_pGameServerNet_ServerModule->SendMsgPBToGate(SquickStruct::ACK_PROPERTY_STRING, xPropertyString, identOld);
		}
	}
	break;

	case TDATA_OBJECT:
	{
		SquickStruct::ObjectPropertyObject xPropertyObject;
		SquickStruct::Ident* pIdent = xPropertyObject.mutable_player_id();
		*pIdent = INetModule::StructToProtobuf(Guid());

		SquickStruct::PropertyObject* pDataObject = xPropertyObject.add_property_list();
		pDataObject->set_property_name(propertyName);
		*pDataObject->mutable_data() = INetModule::StructToProtobuf(newVar.GetObject());

		for (int i = 0; i < players.GetCount(); i++)
		{
			Guid identOld = players.Object(i);

			m_pGameServerNet_ServerModule->SendMsgPBToGate(SquickStruct::ACK_PROPERTY_OBJECT, xPropertyObject, identOld);
		}
	}
	break;
	case TDATA_VECTOR2:
	{
		SquickStruct::ObjectPropertyVector2 xPropertyVector2;
		SquickStruct::Ident* pIdent = xPropertyVector2.mutable_player_id();
		*pIdent = INetModule::StructToProtobuf(Guid());

		SquickStruct::PropertyVector2* pDataObject = xPropertyVector2.add_property_list();
		pDataObject->set_property_name(propertyName);
		*pDataObject->mutable_data() = INetModule::StructToProtobuf(newVar.GetVector2());

		for (int i = 0; i < players.GetCount(); i++)
		{
			Guid identOld = players.Object(i);

			m_pGameServerNet_ServerModule->SendMsgPBToGate(SquickStruct::ACK_PROPERTY_VECTOR2, xPropertyVector2, identOld);
		}
	}
	break;
	case TDATA_VECTOR3:
	{
		SquickStruct::ObjectPropertyVector3 xPropertyVector3;
		SquickStruct::Ident* pIdent = xPropertyVector3.mutable_player_id();
		*pIdent = INetModule::StructToProtobuf(Guid());

		SquickStruct::PropertyVector3* pDataObject = xPropertyVector3.add_property_list();
		pDataObject->set_property_name(propertyName);
		*pDataObject->mutable_data() = INetModule::StructToProtobuf(newVar.GetVector3());

		for (int i = 0; i < players.GetCount(); i++)
		{
			Guid identOld = players.Object(i);

			m_pGameServerNet_ServerModule->SendMsgPBToGate(SquickStruct::ACK_PROPERTY_VECTOR3, xPropertyVector3, identOld);
		}
	}
	break;
	default:
		break;
	}

	return 0;
}

int SceneAutoBroadcastModule::OnRecordEvent(const Guid & self, const RECORD_EVENT_DATA & eventData, const SquickData & oldVar, const SquickData & newVar)
{
	DataList players;
	m_pKernelModule->GetGroupObjectList(self.nHead64, self.nData64, players, true);

	switch (eventData.nOpType)
	{
	case RECORD_EVENT_DATA::Add:
	{
		SquickStruct::ObjectRecordAddRow xAddRecordRow;
		SquickStruct::Ident* pIdent = xAddRecordRow.mutable_player_id();
		*pIdent = INetModule::StructToProtobuf(Guid());

		xAddRecordRow.set_record_name(eventData.recordName);

		SquickStruct::RecordAddRowStruct* pAddRowData = xAddRecordRow.add_row_data();
		pAddRowData->set_row(eventData.row);


		SQUICK_SHARE_PTR<IRecord> xRecord = m_pSceneModule->FindRecord(self.nHead64, self.nData64, eventData.recordName);
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

				for (int i = 0; i < players.GetCount(); i++)
				{
					Guid identOther = players.Object(i);

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
		*pIdent = INetModule::StructToProtobuf(Guid());

		xReoveRecordRow.set_record_name(eventData.recordName);
		xReoveRecordRow.add_remove_row(eventData.row);

		for (int i = 0; i < players.GetCount(); i++)
		{
			Guid identOther = players.Object(i);

			m_pGameServerNet_ServerModule->SendMsgPBToGate(SquickStruct::ACK_REMOVE_ROW, xReoveRecordRow, identOther);
		}
	}
	break;
	case RECORD_EVENT_DATA::Swap:
	{

		SquickStruct::ObjectRecordSwap xSwapRecord;
		*xSwapRecord.mutable_player_id() = INetModule::StructToProtobuf(Guid());

		xSwapRecord.set_origin_record_name(eventData.recordName);
		xSwapRecord.set_target_record_name(eventData.recordName);
		xSwapRecord.set_row_origin(eventData.row);
		xSwapRecord.set_row_target(eventData.col);

		for (int i = 0; i < players.GetCount(); i++)
		{
			Guid identOther = players.Object(i);

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
			*xRecordChanged.mutable_player_id() = INetModule::StructToProtobuf(Guid());

			xRecordChanged.set_record_name(eventData.recordName);
			SquickStruct::RecordInt* recordProperty = xRecordChanged.add_property_list();
			recordProperty->set_row(eventData.row);
			recordProperty->set_col(eventData.col);
			int64_t nData = newVar.GetInt();
			recordProperty->set_data(nData);

			for (int i = 0; i < players.GetCount(); i++)
			{
				Guid identOther = players.Object(i);

				m_pGameServerNet_ServerModule->SendMsgPBToGate(SquickStruct::ACK_RECORD_INT, xRecordChanged, identOther);
			}
		}
		break;

		case TDATA_FLOAT:
		{
			SquickStruct::ObjectRecordFloat xRecordChanged;
			*xRecordChanged.mutable_player_id() = INetModule::StructToProtobuf(Guid());

			xRecordChanged.set_record_name(eventData.recordName);
			SquickStruct::RecordFloat* recordProperty = xRecordChanged.add_property_list();
			recordProperty->set_row(eventData.row);
			recordProperty->set_col(eventData.col);
			recordProperty->set_data(newVar.GetFloat());

			for (int i = 0; i < players.GetCount(); i++)
			{
				Guid identOther = players.Object(i);

				m_pGameServerNet_ServerModule->SendMsgPBToGate(SquickStruct::ACK_RECORD_FLOAT, xRecordChanged, identOther);
			}
		}
		break;
		case TDATA_STRING:
		{
			SquickStruct::ObjectRecordString xRecordChanged;
			*xRecordChanged.mutable_player_id() = INetModule::StructToProtobuf(Guid());

			xRecordChanged.set_record_name(eventData.recordName);
			SquickStruct::RecordString* recordProperty = xRecordChanged.add_property_list();
			recordProperty->set_row(eventData.row);
			recordProperty->set_col(eventData.col);
			recordProperty->set_data(newVar.GetString());

			for (int i = 0; i < players.GetCount(); i++)
			{
				Guid identOther = players.Object(i);

				m_pGameServerNet_ServerModule->SendMsgPBToGate(SquickStruct::ACK_RECORD_STRING, xRecordChanged, identOther);
			}
		}
		break;
		case TDATA_OBJECT:
		{
			SquickStruct::ObjectRecordObject xRecordChanged;
			*xRecordChanged.mutable_player_id() = INetModule::StructToProtobuf(Guid());

			xRecordChanged.set_record_name(eventData.recordName);
			SquickStruct::RecordObject* recordProperty = xRecordChanged.add_property_list();
			recordProperty->set_row(eventData.row);
			recordProperty->set_col(eventData.col);
			*recordProperty->mutable_data() = INetModule::StructToProtobuf(newVar.GetObject());

			for (int i = 0; i < players.GetCount(); i++)
			{
				Guid identOther = players.Object(i);

				m_pGameServerNet_ServerModule->SendMsgPBToGate(SquickStruct::ACK_RECORD_OBJECT, xRecordChanged, identOther);
			}
		}
		break;
		case TDATA_VECTOR2:
		{
			SquickStruct::ObjectRecordVector2 xRecordChanged;
			*xRecordChanged.mutable_player_id() = INetModule::StructToProtobuf(Guid());

			xRecordChanged.set_record_name(eventData.recordName);
			SquickStruct::RecordVector2* recordProperty = xRecordChanged.add_property_list();
			recordProperty->set_row(eventData.row);
			recordProperty->set_col(eventData.col);
			*recordProperty->mutable_data() = INetModule::StructToProtobuf(newVar.GetVector2());

			for (int i = 0; i < players.GetCount(); i++)
			{
				Guid identOther = players.Object(i);

				m_pGameServerNet_ServerModule->SendMsgPBToGate(SquickStruct::ACK_RECORD_VECTOR2, xRecordChanged, identOther);
			}
		}
		break;
		case TDATA_VECTOR3:
		{
			SquickStruct::ObjectRecordVector3 xRecordChanged;
			*xRecordChanged.mutable_player_id() = INetModule::StructToProtobuf(Guid());

			xRecordChanged.set_record_name(eventData.recordName);
			SquickStruct::RecordVector3* recordProperty = xRecordChanged.add_property_list();
			recordProperty->set_row(eventData.row);
			recordProperty->set_col(eventData.col);
			*recordProperty->mutable_data() = INetModule::StructToProtobuf(newVar.GetVector3());

			for (int i = 0; i < players.GetCount(); i++)
			{
				Guid identOther = players.Object(i);

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

int SceneAutoBroadcastModule::OnBeforeLeaveSceneEvent(const Guid & self, const int sceneID, const int groupID, const int type, const DataList & argList)
{
	ClearProperty(self, sceneID, groupID);
	ClearRecord(self, sceneID, groupID);

	return 0;
}

int SceneAutoBroadcastModule::OnAfterEntrySceneEvent(const Guid & self, const int sceneID, const int groupID, const int type, const DataList & argList)
{
	DataList argVar;
	argVar << self;

	OnPropertyEnter(argVar, sceneID, groupID);
	OnRecordEnter(argVar, sceneID, groupID);

	return 0;
}

int SceneAutoBroadcastModule::OnPropertyEnter(const DataList & argVar, const int sceneID, const int groupID)
{
	SquickStruct::MultiObjectPropertyList xPublicMsg;
	SQUICK_SHARE_PTR<IPropertyManager> pPropertyManager = m_pSceneModule->FindPropertyManager(sceneID, groupID);
	if (pPropertyManager)
	{
		SquickStruct::ObjectPropertyList* pPublicData = xPublicMsg.add_multi_player_property();

		*(pPublicData->mutable_player_id()) = INetModule::StructToProtobuf(Guid(0, 0));

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
			m_pGameServerNet_ServerModule->SendMsgPBToGate(SquickStruct::ACK_OBJECT_PROPERTY_ENTRY, xPublicMsg, identOther);
		}
	}

	return 0;
}

bool OnSceneRecordEnterPack(SQUICK_SHARE_PTR<IRecord> pRecord, SquickStruct::ObjectRecordBase* pObjectRecordBase)
{
	if (!pRecord || !pObjectRecordBase)
	{
		return false;
	}

	for (int i = 0; i < pRecord->GetRows(); i++)
	{
		if (pRecord->IsUsed(i))
		{

			SquickStruct::RecordAddRowStruct* pAddRowStruct = pObjectRecordBase->add_row_struct();
			pAddRowStruct->set_row(i);

			for (int j = 0; j < pRecord->GetCols(); j++)
			{

				DataList valueList;
				DATA_TYPE eType = pRecord->GetColType(j);
				switch (eType)
				{
				case DATA_TYPE::TDATA_INT:
				{
					INT64 nValue = pRecord->GetInt(i, j);
					//if ( 0 != nValue )
					{
						SquickStruct::RecordInt* pAddData = pAddRowStruct->add_record_int_list();
						pAddData->set_row(i);
						pAddData->set_col(j);
						pAddData->set_data(nValue);
					}
				}
				break;
				case DATA_TYPE::TDATA_FLOAT:
				{
					double dwValue = pRecord->GetFloat(i, j);
					//if ( dwValue < -0.01f || dwValue > 0.01f )
					{
						SquickStruct::RecordFloat* pAddData = pAddRowStruct->add_record_float_list();
						pAddData->set_row(i);
						pAddData->set_col(j);
						pAddData->set_data(dwValue);
					}
				}
				break;
				case DATA_TYPE::TDATA_STRING:
				{
					const std::string& strData = pRecord->GetString(i, j);
					//if ( !strData.empty() )
					{
						SquickStruct::RecordString* pAddData = pAddRowStruct->add_record_string_list();
						pAddData->set_row(i);
						pAddData->set_col(j);
						pAddData->set_data(strData);
					}
				}
				break;
				case DATA_TYPE::TDATA_OBJECT:
				{
					Guid ident = pRecord->GetObject(i, j);
					//if ( !ident.IsNull() )
					{
						SquickStruct::RecordObject* pAddData = pAddRowStruct->add_record_object_list();
						pAddData->set_row(i);
						pAddData->set_col(j);
						*(pAddData->mutable_data()) = INetModule::StructToProtobuf(ident);
					}
				}
				break;
				case DATA_TYPE::TDATA_VECTOR2:
				{
					Vector2 vPos = pRecord->GetVector2(i, j);
					//if ( !ident.IsNull() )
					{
						SquickStruct::RecordVector2* pAddData = pAddRowStruct->add_record_vector2_list();
						pAddData->set_row(i);
						pAddData->set_col(j);
						*(pAddData->mutable_data()) = INetModule::StructToProtobuf(vPos);
					}
				}
				break;
				case DATA_TYPE::TDATA_VECTOR3:
				{
					Vector3 vPos = pRecord->GetVector3(i, j);
					//if ( !ident.IsNull() )
					{
						SquickStruct::RecordVector3* pAddData = pAddRowStruct->add_record_vector3_list();
						pAddData->set_row(i);
						pAddData->set_col(j);
						*(pAddData->mutable_data()) = INetModule::StructToProtobuf(vPos);
					}
				}
				break;

				default:
					break;
				}
			}
		}
	}
	return 0;
}

int SceneAutoBroadcastModule::OnRecordEnter(const DataList & argVar, const int sceneID, const int groupID)
{
	SquickStruct::MultiObjectRecordList xPublicMsg;

	SQUICK_SHARE_PTR<IRecordManager> pRecordManager = m_pSceneModule->FindRecordManager(sceneID, groupID);
	if (pRecordManager)
	{
		SquickStruct::ObjectRecordList* pPublicData = NULL;

		SQUICK_SHARE_PTR<IRecord> pRecord = pRecordManager->First();
		while (pRecord)
		{
			if (!pRecord->GetPublic())
			{
				pRecord = pRecordManager->Next();
				continue;
			}

			SquickStruct::ObjectRecordBase* pPublicRecordBase = NULL;
			if (pRecord->GetPublic())
			{
				if (!pPublicData)
				{
					pPublicData = xPublicMsg.add_multi_player_record();
					*(pPublicData->mutable_player_id()) = INetModule::StructToProtobuf(Guid(0, 0));
				}
				pPublicRecordBase = pPublicData->add_record_list();
				pPublicRecordBase->set_record_name(pRecord->GetName());

				OnSceneRecordEnterPack(pRecord, pPublicRecordBase);
			}

			pRecord = pRecordManager->Next();
		}


		for (int i = 0; i < argVar.GetCount(); i++)
		{
			Guid identOther = argVar.Object(i);
			if (xPublicMsg.multi_player_record_size() > 0)
			{
				m_pGameServerNet_ServerModule->SendMsgPBToGate(SquickStruct::ACK_OBJECT_RECORD_ENTRY, xPublicMsg, identOther);
			}
		}
	}

	return 0;
}

int SceneAutoBroadcastModule::ClearProperty(const Guid & self, const int sceneID, const int groupID)
{
	SquickStruct::MultiObjectPropertyList xPublicMsg;
	SQUICK_SHARE_PTR<IPropertyManager> pPropertyManager = m_pSceneModule->FindPropertyManager(sceneID, groupID);
	if (pPropertyManager)
	{
		SquickStruct::ObjectPropertyList* pPublicData = xPublicMsg.add_multi_player_property();

		*(pPublicData->mutable_player_id()) = INetModule::StructToProtobuf(Guid(0, 0));


		m_pGameServerNet_ServerModule->SendMsgPBToGate(SquickStruct::ACK_PROPERTY_CLEAR, xPublicMsg, self);
	}

	return 0;
}

int SceneAutoBroadcastModule::ClearRecord(const Guid & self, const int sceneID, const int groupID)
{
	SquickStruct::MultiObjectRecordList xPublicMsg;
	SQUICK_SHARE_PTR<IRecordManager> pRecordManager = m_pSceneModule->FindRecordManager(sceneID, groupID);
	if (pRecordManager)
	{
		SquickStruct::ObjectRecordList* pPublicData = xPublicMsg.add_multi_player_record();
		*(pPublicData->mutable_player_id()) = INetModule::StructToProtobuf(Guid(0, 0));

		SQUICK_SHARE_PTR<IRecord> pRecord = pRecordManager->First();
		while (pRecord)
		{
			if (!pRecord->GetPublic())
			{
				pRecord = pRecordManager->Next();
				continue;
			}

			if (pRecord->GetPublic())
			{
				SquickStruct::ObjectRecordBase* pPublicRecordBase = pPublicData->add_record_list();
				pPublicRecordBase->set_record_name(pRecord->GetName());
			}

			pRecord = pRecordManager->Next();
		}


		if (xPublicMsg.multi_player_record_size() > 0)
		{
			m_pGameServerNet_ServerModule->SendMsgPBToGate(SquickStruct::ACK_RECORD_CLEAR, xPublicMsg, self);
		}
	}

	return 0;
}