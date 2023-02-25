
#include "inventory_module.h"
#include <squick/struct/protocol_define.h>
#include <squick/struct/define.pb.h>
#include <squick/struct/share.pb.h>

bool InventoryModule::Start()
{
	m_pKernelModule = pPluginManager->FindModule<IKernelModule>();
	m_pElementModule = pPluginManager->FindModule<IElementModule>();
	m_pSceneProcessModule = pPluginManager->FindModule<ISceneProcessModule>();
	m_pPropertyModule = pPluginManager->FindModule<IPropertyModule>();
	m_pLogModule = pPluginManager->FindModule<ILogModule>();

    return true;
}

bool InventoryModule::Destory()
{
    return true;
}

bool InventoryModule::Update()
{
    
    return true;
}

bool InventoryModule::AfterStart()
{

    return true;
}


Guid InventoryModule::CreateEquip(const Guid& self, const std::string& configName, const int count)
{
	SQUICK_SHARE_PTR<IObject> pObject = m_pKernelModule->GetObject( self );
	if ( NULL == pObject )
	{
		return NULL_OBJECT;
	}

	
	bool bExist = m_pElementModule->ExistElement( configName );
	if ( !bExist )
	{
		m_pLogModule->LogError(self, "has no this element:" + configName);
		return NULL_OBJECT;
	}

	int nItemType = m_pElementModule->GetPropertyInt32(configName, SquickProtocol::Item::ItemType());
	if ( SquickStruct::EItemType::EIT_EQUIP != nItemType )
	{
		m_pLogModule->LogError(self, configName + " has no this item type:" + std::to_string(nItemType));
		return NULL_OBJECT;
	}

	SQUICK_SHARE_PTR<IRecord> pRecord = pObject->GetRecordManager()->GetElement( SquickProtocol::Player::InventoryEquipment::ThisName() );
	if (!pRecord)
	{
		return NULL_OBJECT;
	}

	Guid ident = m_pKernelModule->CreateGUID();

	SQUICK_SHARE_PTR<DataList> var = pRecord->GetStartData();

	var->SetObject(SquickProtocol::Player::InventoryEquipment::GUID, ident);
	var->SetString(SquickProtocol::Player::InventoryEquipment::ConfigID, configName.c_str());
	var->SetInt(SquickProtocol::Player::InventoryEquipment::Date, pPluginManager->GetNowTime());

	if (count > 1)
	{
		for (int i = 0; i < count; ++i)
		{
			var->SetObject(SquickProtocol::Player::InventoryEquipment::GUID, m_pKernelModule->CreateGUID());
			pRecord->AddRow(-1, *var);
		}

		return NULL_OBJECT;
	}
	else if (count == 1)
	{
		int nAddRow = pRecord->AddRow(-1, *var);
		if (nAddRow >= 0)
		{
			return pRecord->GetObject(nAddRow, SquickProtocol::Player::InventoryEquipment::GUID);
		}
	}

	return NULL_OBJECT;
}

bool InventoryModule::CreateItem(const Guid& self, const std::string& configName, const int count)
{
	if (count <= 0)
	{
		return false;
	}

	SQUICK_SHARE_PTR<IObject> pObject = m_pKernelModule->GetObject( self );
	if ( NULL == pObject )
	{
		return false;
	}

	
	bool bExist = m_pElementModule->ExistElement(SquickProtocol::Item::ThisName(), configName );
	if ( !bExist )
	{
		m_pLogModule->LogError(self, "has no this element:" + configName);
		return false;
	}

	int nItemType = m_pElementModule->GetPropertyInt32(configName, SquickProtocol::Item::ItemType());
	if ( SquickStruct::EItemType::EIT_EQUIP == nItemType )
	{
		CreateEquip(self, configName, count);

		return false;
	}

	return CreateItemInNormalBag(self, configName, count);
}

bool InventoryModule::DeleteEquip(const Guid& self, const Guid& id )
{
	if (id.IsNull())
	{
		return false;
	}

	
	SQUICK_SHARE_PTR<IObject> pObject = m_pKernelModule->GetObject( self );
	if (nullptr == pObject)
	{
		return false;
	}

	SQUICK_SHARE_PTR<IRecord> pRecord = pObject->GetRecordManager()->GetElement( SquickProtocol::Player::InventoryEquipment::ThisName() );
	if (nullptr == pRecord)
	{
		return false;
	}

	DataList varFindResult;
	int nFindRowCount = pRecord->FindObject(SquickProtocol::Player::InventoryEquipment::GUID, id, varFindResult);
	if (nFindRowCount > 0)
	{
		//int nTotalCount = 0;
		for (int i = 0; i < varFindResult.GetCount(); ++i)
		{
			int nFindRow = varFindResult.Int32(i);
			pRecord->Remove(nFindRow);
		}
	}

	return true;
}

bool InventoryModule::DeleteItem(const Guid& self, const std::string& strItemConfigID, const int count )
{
	if(count <= 0)
	{
		return false;
	}

	SQUICK_SHARE_PTR<IObject> pObject = m_pKernelModule->GetObject( self );
	if ( NULL == pObject )
	{
		return false;
	}


	if (!m_pElementModule->ExistElement(SquickProtocol::Item::ThisName(), strItemConfigID))
	{
		m_pLogModule->LogError(self, "has no this element:" + strItemConfigID);
		return false;
	}

	SQUICK_SHARE_PTR<IRecord> pRecord = pObject->GetRecordManager()->GetElement( SquickProtocol::Player::Inventory::ThisName() );
	if (!pRecord)
	{
		return false;
	}

	int nFindRow = pRecord->FindString(SquickProtocol::Player::Inventory::ConfigID, strItemConfigID);
	if (nFindRow >= 0)
	{
		int nOldCount = pRecord->GetInt32(nFindRow, SquickProtocol::Player::Inventory::ItemCount);
		if (nOldCount > count)
		{
			int nNewCount = nOldCount - count;
			pRecord->SetInt(nFindRow, SquickProtocol::Player::Inventory::ItemCount, nNewCount);

			m_pLogModule->LogInfo(self, " DeleteItem:" + strItemConfigID + ", from " + std::to_string(nOldCount) + " to " + std::to_string(nNewCount));
			return true;
		}
		else if (nOldCount == count)
		{
			pRecord->Remove(nFindRow);
			m_pLogModule->LogInfo(self, " DeleteItem:" + strItemConfigID + ", from " + std::to_string(nOldCount) + " to 0");
			return true;
		}
	}

	return false;
}



bool InventoryModule::EnoughItem(const Guid& self, const std::string& strItemConfigID, const int count )
{
    if(count <= 0)
    {
        return false;
    }

    SQUICK_SHARE_PTR<IObject> pObject = m_pKernelModule->GetObject( self );
    if ( NULL == pObject )
    {
        return false;
    }

    
    bool bExist = m_pElementModule->ExistElement(SquickProtocol::Item::ThisName(), strItemConfigID );
    if ( !bExist )
    {
		m_pLogModule->LogError(self, "has no this element:" + strItemConfigID);
        return false;
    }

    SQUICK_SHARE_PTR<IRecord> pRecord = pObject->GetRecordManager()->GetElement( SquickProtocol::Player::Inventory::ThisName() );
    if (!pRecord)
    {
        return false;
    }

	int row = pRecord->FindString(SquickProtocol::Player::Inventory::ConfigID, strItemConfigID);
	if (row >= 0)
	{
		int itemCount = pRecord->GetInt32(row, SquickProtocol::Player::Inventory::ItemCount);

		if (itemCount >= count)
		{
			return true;
		}
	}

    return false;
}

bool InventoryModule::CreateItemInNormalBag(const Guid & self, const std::string & configName, const int count)
{
	SQUICK_SHARE_PTR<IRecord> pRecord = m_pKernelModule->FindRecord(self, SquickProtocol::Player::Inventory::ThisName());
	if (nullptr == pRecord)
	{
		return false;
	}

	int row = pRecord->FindString(SquickProtocol::Player::Inventory::ConfigID, configName);
	if (row < 0)
	{
		SQUICK_SHARE_PTR<DataList> xRowData = pRecord->GetStartData();

		xRowData->SetString(SquickProtocol::Player::Inventory::ConfigID, configName);
		xRowData->SetInt(SquickProtocol::Player::Inventory::ItemCount, count);

		int row = pRecord->AddRow(-1, *xRowData);
		if (row < 0)
		{
			m_pLogModule->LogError(self, " cant add item to bag " + configName);
			return false;
		}
	}
	else
	{
		int totalCount = pRecord->GetInt32(row, SquickProtocol::Player::Inventory::ItemCount) + count;
		pRecord->SetInt(row, SquickProtocol::Player::Inventory::ItemCount, totalCount);
	}


	m_pLogModule->LogInfo(self, "add item to bag:" + configName + ", count:" + std::to_string(count));

	return true;
}

int InventoryModule::ItemCount(const Guid &self, const std::string &strItemConfigID)
{

	SQUICK_SHARE_PTR<IObject> pObject = m_pKernelModule->GetObject( self );
	if ( NULL == pObject )
	{
		return 0;
	}


	if (!m_pElementModule->ExistElement(SquickProtocol::Item::ThisName(), strItemConfigID))
	{
		m_pLogModule->LogError(self, "has no this element:" + strItemConfigID);
		return 0;
	}

	SQUICK_SHARE_PTR<IRecord> pRecord = pObject->GetRecordManager()->GetElement( SquickProtocol::Player::Inventory::ThisName() );
	if (!pRecord)
	{
		return 0;
	}

	int nFindRow = pRecord->FindString(SquickProtocol::Player::Inventory::ConfigID, strItemConfigID);
	if (nFindRow >= 0)
	{
		return pRecord->GetInt32(nFindRow, SquickProtocol::Player::Inventory::ItemCount);
	}

	return 0;
}
