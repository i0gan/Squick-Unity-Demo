#include "server_module.h"
#include <squick/struct/protocol_define.h>
#include <squick/plugin/kernel/i_event_module.h>
#include <squick/plugin/kernel/scene_module.h>

bool GameServerNet_ServerModule::Start()
{
	this->pPluginManager->SetAppType(SQUICK_SERVER_TYPES::SQUICK_ST_GAME);

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
	m_pThreadPoolModule = pPluginManager->FindModule<IThreadPoolModule>();
	m_pSyncPosModule = pPluginManager->FindModule<ISyncPosModule>();

	return true;
}

bool GameServerNet_ServerModule::AfterStart()
{

	m_pNetModule->AddReceiveCallBack(SquickStruct::PROXY_TO_GAME_REFRESH, this, &GameServerNet_ServerModule::OnRefreshProxyServerInfoProcess);
	m_pNetModule->AddReceiveCallBack(SquickStruct::PROXY_TO_GAME_REGISTERED, this, &GameServerNet_ServerModule::OnProxyServerRegisteredProcess);
	m_pNetModule->AddReceiveCallBack(SquickStruct::PROXY_TO_GAME_UNREGISTERED, this, &GameServerNet_ServerModule::OnProxyServerUnRegisteredProcess);
	
	m_pNetModule->AddReceiveCallBack(SquickStruct::PVP_MANAGER_TO_GAME_REFRESH, this, &GameServerNet_ServerModule::OnRefreshPvpManagerServerInfoProcess);
	m_pNetModule->AddReceiveCallBack(SquickStruct::PVP_MANAGER_TO_GAME_REGISTERED, this, &GameServerNet_ServerModule::OnPvpManagerServerRegisteredProcess);
	m_pNetModule->AddReceiveCallBack(SquickStruct::PVP_MANAGER_TO_GAME_UNREGISTERED, this, &GameServerNet_ServerModule::OnPvpManagerServerUnRegisteredProcess);
	
	m_pNetModule->AddReceiveCallBack(SquickStruct::REQ_LEAVE_GAME, this, &GameServerNet_ServerModule::OnClientLeaveGameProcess);

	m_pNetModule->AddReceiveCallBack(SquickStruct::REQ_SWAP_SCENE, this, &GameServerNet_ServerModule::OnClientSwapSceneProcess);
	m_pNetModule->AddReceiveCallBack(SquickStruct::REQ_ENTER_GAME_FINISH, this, &GameServerNet_ServerModule::OnClientEnterGameFinishProcess);
	

	//EGMI_ACK_RECORD_CLEAR = 228,
	//EGMI_ACK_RECORD_SORT = 229,
	//的绑定事件
	m_pNetModule->AddReceiveCallBack(SquickStruct::REQ_MOVE, this, &GameServerNet_ServerModule::OnClientReqMoveProcess);
	m_pNetModule->AddReceiveCallBack(SquickStruct::REQ_LAG_TEST, this, &GameServerNet_ServerModule::OnLagTestProcess);


	m_pNetModule->AddEventCallBack(this, &GameServerNet_ServerModule::OnSocketPSEvent);


	/////////////////////////////////////////////////////////////////////////

	SQUICK_SHARE_PTR<IClass> xLogicClass = m_pClassModule->GetElement(SquickProtocol::Server::ThisName());
	if (xLogicClass)
	{
		const std::vector<std::string>& strIdList = xLogicClass->GetIDList();
		for (int i = 0; i < strIdList.size(); ++i)
		{
			const std::string& strId = strIdList[i];

			const int serverType = m_pElementModule->GetPropertyInt32(strId, SquickProtocol::Server::Type());
			const int serverID = m_pElementModule->GetPropertyInt32(strId, SquickProtocol::Server::ServerID());
			if (serverType == SQUICK_SERVER_TYPES::SQUICK_ST_GAME && pPluginManager->GetAppID() == serverID)
			{
				const int nPort = m_pElementModule->GetPropertyInt32(strId, SquickProtocol::Server::Port());
				const int maxConnect = m_pElementModule->GetPropertyInt32(strId, SquickProtocol::Server::MaxOnline());
				const int nCpus = m_pElementModule->GetPropertyInt32(strId, SquickProtocol::Server::CpuCount());
				//const std::string& name = m_pElementModule->GetPropertyString(strId, SquickProtocol::Server::ID());
				//const std::string& ip = m_pElementModule->GetPropertyString(strId, SquickProtocol::Server::IP());
				int nRet = m_pNetModule->Startialization(maxConnect, nPort, nCpus);
				if (nRet < 0)
				{
					std::ostringstream strLog;
					strLog << "Cannot init server net, Port = " << nPort;
					m_pLogModule->LogError(NULL_OBJECT, strLog, __FUNCTION__, __LINE__);
					NFASSERT(nRet, "Cannot init server net", __FILE__, __FUNCTION__);
					exit(0);
				}
			}
		}
	}

	return true;
}

bool GameServerNet_ServerModule::Destory()
{

	return true;
}

bool GameServerNet_ServerModule::Update()
{
	return true;
}

void GameServerNet_ServerModule::OnSocketPSEvent(const SQUICK_SOCKET sockIndex, const SQUICK_NET_EVENT eEvent, INet* pNet)
{
	if (eEvent & SQUICK_NET_EVENT_EOF)
	{
		m_pLogModule->LogInfo(Guid(0, sockIndex), "SQUICK_NET_EVENT_EOF Connection closed", __FUNCTION__, __LINE__);
		OnClientDisconnect(sockIndex);
	}
	else if (eEvent & SQUICK_NET_EVENT_ERROR)
	{
		m_pLogModule->LogInfo(Guid(0, sockIndex), "SQUICK_NET_EVENT_ERROR Got an error on the connection", __FUNCTION__, __LINE__);
		OnClientDisconnect(sockIndex);
	}
	else if (eEvent & SQUICK_NET_EVENT_TIMEOUT)
	{
		m_pLogModule->LogInfo(Guid(0, sockIndex), "SQUICK_NET_EVENT_TIMEOUT read timeout", __FUNCTION__, __LINE__);
		OnClientDisconnect(sockIndex);
	}
	else  if (eEvent & SQUICK_NET_EVENT_CONNECTED)
	{
		m_pLogModule->LogInfo(Guid(0, sockIndex), "SQUICK_NET_EVENT_CONNECTED connected success", __FUNCTION__, __LINE__);
		OnClientConnected(sockIndex);
	}
}

void GameServerNet_ServerModule::OnClientDisconnect(const SQUICK_SOCKET nAddress)
{
	int serverID = 0;
	SQUICK_SHARE_PTR<GateServerInfo> pServerData = mProxyMap.First();
	while (pServerData)
	{
		if (nAddress == pServerData->xServerData.nFD)
		{
			serverID = pServerData->xServerData.pData->server_id();
			break;
		}

		pServerData = mProxyMap.Next();
	}

	mProxyMap.RemoveElement(serverID);
}

void GameServerNet_ServerModule::OnClientConnected(const SQUICK_SOCKET nAddress)
{

}

void GameServerNet_ServerModule::OnClientLeaveGameProcess(const SQUICK_SOCKET sockIndex, const int msgID, const char *msg, const uint32_t len)
{
	Guid nPlayerID;
	SquickStruct::ReqLeaveGameServer xMsg;
	if (!m_pNetModule->ReceivePB( msgID, msg, len, xMsg, nPlayerID))
	{
		return;
	}

	if (nPlayerID.IsNull())
	{
		return;
	}

	m_pKernelModule->SetPropertyInt(nPlayerID, SquickProtocol::IObject::Connection(), 0);

	m_pKernelModule->DestroyObject(nPlayerID);

	RemovePlayerGateInfo(nPlayerID);
}

// 客户端进入游戏完成处理
void GameServerNet_ServerModule::OnClientEnterGameFinishProcess(const SQUICK_SOCKET sockIndex, const int msgID, const char *msg, const uint32_t len)
{
#ifdef SQUICK_DEV
		std::cout << "GameServerNet_ServerModule::OnClientEnterGameFinishProcess 玩家进入游戏\n";
#endif

	CLIENT_MSG_PROCESS( msgID, msg, len, SquickStruct::ReqAckEnterGameSuccess);
	m_pKernelModule->DoEvent(nPlayerID, SquickProtocol::Player::ThisName(), CLASS_OBJECT_EVENT::COE_CREATE_CLIENT_FINISH, DataList::Empty());
	
	m_pNetModule->SendMsgPB(SquickStruct::ACK_ENTER_GAME_FINISH, xMsg, sockIndex, nPlayerID);
}

void GameServerNet_ServerModule::OnLagTestProcess(const SQUICK_SOCKET sockIndex, const int msgID, const char * msg, const uint32_t len)
{
	CLIENT_MSG_PROCESS(msgID, msg, len, SquickStruct::ReqAckLagTest);
	this->SendMsgPBToGate(SquickStruct::ACK_GAME_LAG_TEST, xMsg, nPlayerID);
}

void GameServerNet_ServerModule::OnClientSwapSceneProcess(const SQUICK_SOCKET sockIndex, const int msgID, const char *msg, const uint32_t len)
{
	CLIENT_MSG_PROCESS(msgID, msg, len, SquickStruct::ReqAckSwapScene)

	const SquickStruct::ESceneType sceneType = (SquickStruct::ESceneType)m_pElementModule->GetPropertyInt(std::to_string(xMsg.scene_id()), SquickProtocol::Scene::Type());
	const int nowSceneID = m_pKernelModule->GetPropertyInt(nPlayerID, SquickProtocol::Player::SceneID());
	const int nowGroupID = m_pKernelModule->GetPropertyInt(nPlayerID, SquickProtocol::Player::GroupID());

	if (sceneType == SquickStruct::ESceneType::NORMAL_SCENE)
	{
		const Vector3& pos = m_pSceneModule->GetRelivePosition(xMsg.scene_id());
		m_pSceneProcessModule->RequestEnterScene(pObject->Self(), xMsg.scene_id(), 1, 0, pos, DataList::Empty());
	}
	else if (sceneType == SquickStruct::ESceneType::SINGLE_CLONE_SCENE)
	{
		const Vector3& pos = m_pSceneModule->GetRelivePosition(xMsg.scene_id());
		m_pSceneProcessModule->RequestEnterScene(pObject->Self(), xMsg.scene_id(), m_pKernelModule->RequestGroupScene(xMsg.scene_id()), 0, pos, DataList::Empty());
	}
}

void GameServerNet_ServerModule::OnClientReqMoveProcess(const SQUICK_SOCKET sockIndex, const int msgID, const char *msg,  const uint32_t len)
{
	CLIENT_MSG_PROCESS_NO_OBJECT(msgID, msg, len, SquickStruct::ReqAckPlayerPosSync)

	if (xMsg.sync_unit_size() > 0)
	{
		SquickStruct::PosSyncUnit* syncUnit = xMsg.mutable_sync_unit(0);
		if (syncUnit)
		{
			const Guid& xMover = INetModule::ProtobufToStruct(syncUnit->mover());
			if (xMover != nPlayerID)
			{
				const Guid masterID = m_pKernelModule->GetPropertyObject(xMover, SquickProtocol::NPC::MasterID());
				if (masterID != nPlayerID)
				{
					m_pLogModule->LogError(xMover, "Message come from player " + nPlayerID.ToString());
					return;
				}
				return;
			}
			else
			{
				const int sceneID = m_pKernelModule->GetPropertyInt32(xMover, SquickProtocol::Player::SceneID());
				const int groupID = m_pKernelModule->GetPropertyInt32(xMover, SquickProtocol::Player::GroupID());

				PosSyncUnit posSyncUnit;

				posSyncUnit.mover = xMover;
				posSyncUnit.pos = INetModule::ProtobufToStruct(syncUnit->pos());
				posSyncUnit.orientation = INetModule::ProtobufToStruct(syncUnit->orientation());
				posSyncUnit.status = syncUnit->status();
				posSyncUnit.type = syncUnit->type();

				m_pSyncPosModule->RequireMove(Guid(sceneID, groupID), posSyncUnit);
				m_pKernelModule->SetPropertyVector3(nPlayerID, SquickProtocol::IObject::Position(), posSyncUnit.pos);
				//this->SendGroupMsgPBToGate(SquickStruct::ACK_MOVE, xMsg, sceneID, groupID);
			}
		}
	}
}

void GameServerNet_ServerModule::OnProxyServerRegisteredProcess(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len)
{
	Guid nPlayerID;
	SquickStruct::ServerInfoReportList xMsg;
	if (!INetModule::ReceivePB( msgID, msg, len, xMsg, nPlayerID))
	{
		return;
	}

	for (int i = 0; i < xMsg.server_list_size(); ++i)
	{
		const SquickStruct::ServerInfoReport& xData = xMsg.server_list(i);
		SQUICK_SHARE_PTR<GateServerInfo> pServerData = mProxyMap.GetElement(xData.server_id());
		if (!pServerData)
		{
			pServerData = SQUICK_SHARE_PTR<GateServerInfo>(SQUICK_NEW GateServerInfo());
			mProxyMap.AddElement(xData.server_id(), pServerData);
		}

		pServerData->xServerData.nFD = sockIndex;
		*(pServerData->xServerData.pData) = xData;

		m_pLogModule->LogInfo(Guid(0, xData.server_id()), xData.server_name(), "Proxy Registered");
	}

	return;
}

void GameServerNet_ServerModule::OnProxyServerUnRegisteredProcess(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len)
{
	Guid nPlayerID;
	SquickStruct::ServerInfoReportList xMsg;
	if (!m_pNetModule->ReceivePB( msgID, msg, len, xMsg, nPlayerID))
	{
		return;
	}

	for (int i = 0; i < xMsg.server_list_size(); ++i)
	{
		const SquickStruct::ServerInfoReport& xData = xMsg.server_list(i);
		mProxyMap.RemoveElement(xData.server_id());


		m_pLogModule->LogInfo(Guid(0, xData.server_id()), xData.server_name(), "Proxy UnRegistered");
	}

	return;
}

void GameServerNet_ServerModule::OnRefreshProxyServerInfoProcess(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len)
{
	Guid nPlayerID;
	SquickStruct::ServerInfoReportList xMsg;
	if (!m_pNetModule->ReceivePB( msgID, msg, len, xMsg, nPlayerID))
	{
		return;
	}

	for (int i = 0; i < xMsg.server_list_size(); ++i)
	{
		const SquickStruct::ServerInfoReport& xData = xMsg.server_list(i);
		SQUICK_SHARE_PTR<GateServerInfo> pServerData = mProxyMap.GetElement(xData.server_id());
		if (!pServerData)
		{
			pServerData = SQUICK_SHARE_PTR<GateServerInfo>(SQUICK_NEW GateServerInfo());
			mProxyMap.AddElement(xData.server_id(), pServerData);
		}

		pServerData->xServerData.nFD = sockIndex;
		*(pServerData->xServerData.pData) = xData;

		m_pLogModule->LogInfo(Guid(0, xData.server_id()), xData.server_name(), "Proxy Registered");
	}

	return;
}



void GameServerNet_ServerModule::OnPvpManagerServerRegisteredProcess(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len)
{
	Guid nPlayerID;
	SquickStruct::ServerInfoReportList xMsg;
	if (!INetModule::ReceivePB(msgID, msg, len, xMsg, nPlayerID))
	{
		return;
	}

	for (int i = 0; i < xMsg.server_list_size(); ++i)
	{
		const SquickStruct::ServerInfoReport& xData = xMsg.server_list(i);
		SQUICK_SHARE_PTR<GateServerInfo> pServerData = mPvpManagerMap.GetElement(xData.server_id());
		if (!pServerData)
		{
			pServerData = SQUICK_SHARE_PTR<GateServerInfo>(SQUICK_NEW GateServerInfo());
			mPvpManagerMap.AddElement(xData.server_id(), pServerData);
		}

		pServerData->xServerData.nFD = sockIndex;
		*(pServerData->xServerData.pData) = xData;

		m_pLogModule->LogInfo(Guid(0, xData.server_id()), xData.server_name(), "Pvp Manager Registered");
	}

	return;
}

void GameServerNet_ServerModule::OnPvpManagerServerUnRegisteredProcess(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len)
{
	Guid nPlayerID;
	SquickStruct::ServerInfoReportList xMsg;
	if (!m_pNetModule->ReceivePB(msgID, msg, len, xMsg, nPlayerID))
	{
		return;
	}

	for (int i = 0; i < xMsg.server_list_size(); ++i)
	{
		const SquickStruct::ServerInfoReport& xData = xMsg.server_list(i);
		mPvpManagerMap.RemoveElement(xData.server_id());

		m_pLogModule->LogInfo(Guid(0, xData.server_id()), xData.server_name(), "Pvp Manager Registered");
	}

	return;
}

void GameServerNet_ServerModule::OnRefreshPvpManagerServerInfoProcess(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len)
{
	Guid nPlayerID;
	SquickStruct::ServerInfoReportList xMsg;
	if (!m_pNetModule->ReceivePB(msgID, msg, len, xMsg, nPlayerID))
	{
		return;
	}

	for (int i = 0; i < xMsg.server_list_size(); ++i)
	{
		const SquickStruct::ServerInfoReport& xData = xMsg.server_list(i);
		SQUICK_SHARE_PTR<GateServerInfo> pServerData = mPvpManagerMap.GetElement(xData.server_id());
		if (!pServerData)
		{
			pServerData = SQUICK_SHARE_PTR<GateServerInfo>(SQUICK_NEW GateServerInfo());
			mPvpManagerMap.AddElement(xData.server_id(), pServerData);
		}

		pServerData->xServerData.nFD = sockIndex;
		*(pServerData->xServerData.pData) = xData;

		m_pLogModule->LogInfo(Guid(0, xData.server_id()), xData.server_name(), "Pvp Manager Registered");
	}

	return;
}




void GameServerNet_ServerModule::SendMsgPBToGate(const uint16_t msgID, google::protobuf::Message& xMsg, const Guid& self)
{
	SQUICK_SHARE_PTR<GateBaseInfo> pData = mRoleBaseData.GetElement(self);
	if (pData)
	{
		SQUICK_SHARE_PTR<GateServerInfo> pProxyData = mProxyMap.GetElement(pData->gateID);
		if (pProxyData)
		{
			m_pNetModule->SendMsgPB(msgID, xMsg, pProxyData->xServerData.nFD, pData->xClientID);
		}
	}
}

void GameServerNet_ServerModule::SendMsgToGate(const uint16_t msgID, const std::string& msg, const Guid& self)
{
	SQUICK_SHARE_PTR<GateBaseInfo> pData = mRoleBaseData.GetElement(self);
	if (pData)
	{
		SQUICK_SHARE_PTR<GateServerInfo> pProxyData = mProxyMap.GetElement(pData->gateID);
		if (pProxyData)
		{
			m_pNetModule->SendMsg(msgID, msg, pProxyData->xServerData.nFD, pData->xClientID);
		}
	}
}
// 发送给 PVP Manager 服务器
void GameServerNet_ServerModule::SendMsgPBToPvpManager(const uint16_t msgID, google::protobuf::Message& xMsg)
{
	// 选择PVP转发表中的第一个PVP Manager进行发送
	GateServerInfo* pGameData = mPvpManagerMap.FirstNude();
	if (pGameData) {
		dout << "发送给 PVP Manager 服务器: " << pGameData->xServerData.pData << std::endl;
		m_pNetModule->SendMsgPB(msgID, xMsg, pGameData->xServerData.nFD);
	}
	else {
		dout << "未找到 PVP Manager 服务器\n";
	}
}

void GameServerNet_ServerModule::SendMsgToPvpManager(const uint16_t msgID, const std::string& msg)
{
	// 选择PVP转发表中的第一个PVP Manager进行发送
	GateServerInfo* pGameData = mPvpManagerMap.FirstNude();
	if (pGameData) {
		m_pNetModule->SendMsg(msgID, msg, pGameData->xServerData.nFD);
	}
	else {
		dout << "未找到 PVP Manager 服务器\n";
	}
}

// 发送给PVP服务器
void GameServerNet_ServerModule::SendMsgPBToPvp(const uint16_t msgID, google::protobuf::Message& xMsg, const Guid& self)
{
	SQUICK_SHARE_PTR<GateBaseInfo> pData = mRoleBaseData.GetElement(self);
	if (pData)
	{
		SQUICK_SHARE_PTR<GateServerInfo> pProxyData = mPvpManagerMap.GetElement(pData->gateID);
		if (pProxyData)
		{
			m_pNetModule->SendMsgPB(msgID, xMsg, pProxyData->xServerData.nFD, pData->xClientID);
		}
	}
}

void GameServerNet_ServerModule::SendMsgToPvp(const uint16_t msgID, const std::string& msg, const Guid& self)
{
	SQUICK_SHARE_PTR<GateBaseInfo> pData = mRoleBaseData.GetElement(self);
	if (pData)
	{
		SQUICK_SHARE_PTR<GateServerInfo> pProxyData = mPvpManagerMap.GetElement(pData->gateID);
		if (pProxyData)
		{
			m_pNetModule->SendMsg(msgID, msg, pProxyData->xServerData.nFD, pData->xClientID);
		}
	}
}
// ---


void GameServerNet_ServerModule::SendGroupMsgPBToGate(const uint16_t msgID, google::protobuf::Message & xMsg, const int sceneID, const int groupID)
{
	//care: batch
	DataList xList;
	if (m_pKernelModule->GetGroupObjectList(sceneID, groupID, xList, true))
	{
		for (int i = 0; i < xList.GetCount(); ++i)
		{
			Guid xObject = xList.Object(i);
			this->SendMsgPBToGate(msgID, xMsg, xObject);
		}
	}
}

void GameServerNet_ServerModule::SendGroupMsgPBToGate(const uint16_t msgID, google::protobuf::Message & xMsg, const int sceneID, const int groupID, const Guid exceptID)
{
	DataList xList;
	if (m_pKernelModule->GetGroupObjectList(sceneID, groupID, xList, true))
	{
		for (int i = 0; i < xList.GetCount(); ++i)
		{
			Guid xObject = xList.Object(i);
			if (xObject != exceptID)
			{
				this->SendMsgPBToGate(msgID, xMsg, xObject);
			}
		}
	}
}

void GameServerNet_ServerModule::SendGroupMsgPBToGate(const uint16_t msgID, const std::string & msg, const int sceneID, const int groupID)
{
	//care: batch
	DataList xList;
	if (m_pKernelModule->GetGroupObjectList(sceneID, groupID, xList, true))
	{
		for (int i = 0; i < xList.GetCount(); ++i)
		{
			Guid xObject = xList.Object(i);
			this->SendMsgToGate(msgID, msg, xObject);
		}
	}
}

void GameServerNet_ServerModule::SendGroupMsgPBToGate(const uint16_t msgID, const std::string & msg, const int sceneID, const int groupID, const Guid exceptID)
{
	DataList xList;
	if (m_pKernelModule->GetGroupObjectList(sceneID, groupID, xList, true))
	{
		for (int i = 0; i < xList.GetCount(); ++i)
		{
			Guid xObject = xList.Object(i);
			if (xObject != exceptID)
			{
				this->SendMsgToGate(msgID, msg, xObject);
			}
		}
	}
}

bool GameServerNet_ServerModule::AddPlayerGateInfo(const Guid& roleID, const Guid& clientID, const int gateID)
{
    if (gateID <= 0)
    {
        
        return false;
    }

    if (clientID.IsNull())
    {
        return false;
    }

    SQUICK_SHARE_PTR<GameServerNet_ServerModule::GateBaseInfo> pBaseData = mRoleBaseData.GetElement(roleID);
    if (nullptr != pBaseData)
    {
        
        m_pLogModule->LogError(clientID, "player is exist, cannot enter game", __FUNCTION__, __LINE__);
        return false;
    }

    SQUICK_SHARE_PTR<GateServerInfo> pServerData = mProxyMap.GetElement(gateID);
    if (nullptr == pServerData)
    {
        return false;
    }

    if (!pServerData->xRoleInfo.insert(std::make_pair(roleID, pServerData->xServerData.nFD)).second)
    {
        return false;
    }

    if (!mRoleBaseData.AddElement(roleID, SQUICK_SHARE_PTR<GateBaseInfo>(SQUICK_NEW GateBaseInfo(gateID, clientID))))
    {
        pServerData->xRoleInfo.erase(roleID) ;
        return false;
    }

    return true;
}

bool GameServerNet_ServerModule::RemovePlayerGateInfo(const Guid& roleID)
{
    SQUICK_SHARE_PTR<GateBaseInfo> pBaseData = mRoleBaseData.GetElement(roleID);
    if (nullptr == pBaseData)
    {
        return false;
    }

    mRoleBaseData.RemoveElement(roleID);

    SQUICK_SHARE_PTR<GateServerInfo> pServerData = mProxyMap.GetElement(pBaseData->gateID);
    if (nullptr == pServerData)
    {
        return false;
    }

    pServerData->xRoleInfo.erase(roleID);
    return true;
}

SQUICK_SHARE_PTR<IGameServerNet_ServerModule::GateBaseInfo> GameServerNet_ServerModule::GetPlayerGateInfo(const Guid& roleID)
{
    return mRoleBaseData.GetElement(roleID);
}

SQUICK_SHARE_PTR<IGameServerNet_ServerModule::GateServerInfo> GameServerNet_ServerModule::GetGateServerInfo(const int gateID)
{
    return mProxyMap.GetElement(gateID);
}

SQUICK_SHARE_PTR<IGameServerNet_ServerModule::GateServerInfo> GameServerNet_ServerModule::GetGateServerInfoBySockIndex(const SQUICK_SOCKET sockIndex)
{
    int gateID = -1;
    SQUICK_SHARE_PTR<GateServerInfo> pServerData = mProxyMap.First();
    while (pServerData)
    {
        if (sockIndex == pServerData->xServerData.nFD)
        {
            gateID = pServerData->xServerData.pData->server_id();
            break;
        }

        pServerData = mProxyMap.Next();
    }

    if (gateID == -1)
    {
        return nullptr;
    }

    return pServerData;
}

void GameServerNet_ServerModule::OnTransWorld(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len)
{
	std::string msgData;
	Guid nPlayer;
	int64_t nHasKey = 0;
	if (INetModule::ReceivePB( msgID, msg, len, msgData, nPlayer))
	{
		nHasKey = nPlayer.nData64;
	}

	m_pNetClientModule->SendBySuitWithOutHead(SQUICK_SERVER_TYPES::SQUICK_ST_WORLD, nHasKey, msgID, std::string(msg, len));
}