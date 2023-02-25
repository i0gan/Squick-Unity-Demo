#include "plugin.h"
#include <squick/struct/protocol_define.h>

#include "server_module.h"

bool ProxyServerNet_ServerModule::Start()
{
	this->pPluginManager->SetAppType(SQUICK_SERVER_TYPES::SQUICK_ST_PROXY);

	m_pNetModule = pPluginManager->FindModule<INetModule>();
	m_pKernelModule = pPluginManager->FindModule<IKernelModule>();
	m_pClassModule = pPluginManager->FindModule<IClassModule>();
	m_pNetClientModule = pPluginManager->FindModule<INetClientModule>();
	m_pLogModule = pPluginManager->FindModule<ILogModule>();
	m_pElementModule = pPluginManager->FindModule<IElementModule>();
	m_pProxyToWorldModule = pPluginManager->FindModule<IProxyServerToWorldModule>();
	m_pSecurityModule = pPluginManager->FindModule<ISecurityModule>();
	m_pWsModule = pPluginManager->FindModule<IWSModule>();
    m_pThreadPoolModule = pPluginManager->FindModule<IThreadPoolModule>();

    return true;
}

bool ProxyServerNet_ServerModule::AfterStart()
{
	m_pNetModule->AddReceiveCallBack(SquickStruct::REQ_CONNECT_KEY, this, &ProxyServerNet_ServerModule::OnConnectKeyProcess);
	m_pWsModule->AddReceiveCallBack(SquickStruct::REQ_CONNECT_KEY, this, &ProxyServerNet_ServerModule::OnConnectKeyProcessWS);
	m_pNetModule->AddReceiveCallBack(SquickStruct::REQ_WORLD_LIST, this, &ProxyServerNet_ServerModule::OnReqServerListProcess);
	m_pNetModule->AddReceiveCallBack(SquickStruct::REQ_SELECT_SERVER, this, &ProxyServerNet_ServerModule::OnSelectServerProcess);
	m_pNetModule->AddReceiveCallBack(SquickStruct::REQ_ROLE_LIST, this, &ProxyServerNet_ServerModule::OnReqRoleListProcess);
	m_pNetModule->AddReceiveCallBack(SquickStruct::REQ_CREATE_ROLE, this, &ProxyServerNet_ServerModule::OnReqCreateRoleProcess);
	m_pNetModule->AddReceiveCallBack(SquickStruct::REQ_DELETE_ROLE, this, &ProxyServerNet_ServerModule::OnReqDelRoleProcess);
	m_pNetModule->AddReceiveCallBack(SquickStruct::REQ_ENTER_GAME, this, &ProxyServerNet_ServerModule::OnReqEnterGameServer);
	m_pNetModule->AddReceiveCallBack(this, &ProxyServerNet_ServerModule::OnOtherMessage);


    // 绑定Call back以及转发去向
	m_pNetModule->AddEventCallBack(this, &ProxyServerNet_ServerModule::OnSocketClientEvent);
	m_pNetModule->ExpandBufferSize(1024*1024*2);

    SQUICK_SHARE_PTR<IClass> xLogicClass = m_pClassModule->GetElement(SquickProtocol::Server::ThisName());
    if (xLogicClass)
    {
		const std::vector<std::string>& strIdList = xLogicClass->GetIDList();
		for (int i = 0; i < strIdList.size(); ++i)
		{
			const std::string& strId = strIdList[i];

            const int serverType = m_pElementModule->GetPropertyInt32(strId, SquickProtocol::Server::Type());
            const int serverID = m_pElementModule->GetPropertyInt32(strId, SquickProtocol::Server::ServerID());
            if (serverType == SQUICK_SERVER_TYPES::SQUICK_ST_PROXY && pPluginManager->GetAppID() == serverID)
            {
                const int nPort = m_pElementModule->GetPropertyInt32(strId, SquickProtocol::Server::Port());
                const int maxConnect = m_pElementModule->GetPropertyInt32(strId, SquickProtocol::Server::MaxOnline());
                const int nCpus = m_pElementModule->GetPropertyInt32(strId, SquickProtocol::Server::CpuCount());
                //const std::string& name = m_pElementModule->GetPropertyString(strId, SquickProtocol::Server::ID());
                //const std::string& ip = m_pElementModule->GetPropertyString(strId, SquickProtocol::Server::IP());
                
                // 绑定端口
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

bool ProxyServerNet_ServerModule::Destory()
{
    return true;
}

bool ProxyServerNet_ServerModule::Update()
{
	return true;
}

void ProxyServerNet_ServerModule::OnOtherMessage(const SQUICK_SOCKET sockIndex, const int msgID, const char * msg, const uint32_t len)
{
	NetObject* pNetObject = m_pNetModule->GetNet()->GetNetObject(sockIndex);
	if (!pNetObject || pNetObject->GetConnectKeyState() <= 0 || pNetObject->GetGameID() <= 0)
	{
		//state error
		return;
	}

	std::string strMsgData = m_pSecurityModule->DecodeMsg(pNetObject->GetAccount(), pNetObject->GetSecurityKey(), msgID, msg, len);
	if (strMsgData.empty())
	{
		//decode failed
		m_pLogModule->LogError(Guid(0, sockIndex), "DecodeMsg failed", __FUNCTION__, __LINE__);
		return;
	}

	SquickStruct::MsgBase xMsg;
	if (!xMsg.ParseFromString(strMsgData))
	{
		char szData[MAX_PATH] = { 0 };
		sprintf(szData, "Parse Message Failed from Packet to MsgBase, MessageID: %d\n", msgID);

		m_pLogModule->LogError(Guid(0, sockIndex), szData, __FUNCTION__, __LINE__);
		return;
	}


	//real user id
	*xMsg.mutable_player_id() = INetModule::StructToProtobuf(pNetObject->GetUserID());


	std::string msgData;
	if (!xMsg.SerializeToString(&msgData))
	{
		return;
	}

	if (xMsg.has_hash_ident())
	{
		//special for distributed
		if (!pNetObject->GetHashIdentID().IsNull())
		{
			m_pNetClientModule->SendBySuitWithOutHead(SQUICK_SERVER_TYPES::SQUICK_ST_GAME, pNetObject->GetHashIdentID().ToString(), msgID, msgData);
		}
		else
		{
			Guid xHashIdent = INetModule::ProtobufToStruct(xMsg.hash_ident());
			m_pNetClientModule->SendBySuitWithOutHead(SQUICK_SERVER_TYPES::SQUICK_ST_GAME, xHashIdent.ToString(), msgID, msgData);
		}
	}
	else
	{
        if (msgID >= 50000)
        {
			m_pNetClientModule->SendBySuitWithOutHead(SQUICK_SERVER_TYPES::SQUICK_ST_WORLD, pNetObject->GetUserID().ToString(), msgID, msgData);
        }
        else
        {
		    m_pNetClientModule->SendByServerIDWithOutHead(pNetObject->GetGameID(), msgID, msgData);
        }
        
	}
}

void ProxyServerNet_ServerModule::OnConnectKeyProcessWS(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len)
{
    Guid nPlayerID;
    SquickStruct::ReqAccountLogin xMsg;
    if (!m_pNetModule->ReceivePB( msgID, msg, len, xMsg, nPlayerID))
    {
        return;
    }
	bool bRet = m_pSecurityModule->VerifySecurityKey(xMsg.account(), xMsg.security_code());
    //bool bRet = m_pProxyToWorldModule->VerifyConnectData(xMsg.account(), xMsg.security_code());
    if (bRet)
    {
        NetObject* pNetObject = m_pWsModule->GetNet()->GetNetObject(sockIndex);
        if (pNetObject)
        {
            //this net-object verify successful and set state as true
            pNetObject->SetConnectKeyState(1);
			pNetObject->SetSecurityKey(xMsg.security_code());

            //this net-object bind a user's account
            pNetObject->SetAccount(xMsg.account());

            SquickStruct::AckEventResult xSendMsg;
            xSendMsg.set_event_code(SquickStruct::VERIFY_KEY_SUCCESS);
            *xSendMsg.mutable_event_client() = INetModule::StructToProtobuf(pNetObject->GetClientID());
			m_pWsModule->SendMsgPB(SquickStruct::EGameMsgID::ACK_CONNECT_KEY, xSendMsg, sockIndex);
        }
    }
    else
    {
        //if verify failed then close this connect
		m_pWsModule->GetNet()->CloseNetObject(sockIndex);
    }
}

void ProxyServerNet_ServerModule::OnConnectKeyProcess(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len)
{
    Guid nPlayerID;
    SquickStruct::ReqAccountLogin xMsg;
    if (!m_pNetModule->ReceivePB( msgID, msg, len, xMsg, nPlayerID))
    {
        return;
    }
    
    // 验证Token
	bool bRet = m_pSecurityModule->VerifySecurityKey(xMsg.account(), xMsg.security_code());
    //bool bRet = m_pProxyToWorldModule->VerifyConnectData(xMsg.account(), xMsg.security_code());
    if (bRet)
    {
        NetObject* pNetObject = m_pNetModule->GetNet()->GetNetObject(sockIndex);
        if (pNetObject)
        {
            //this net-object verify successful and set state as true
            pNetObject->SetConnectKeyState(1);
			pNetObject->SetSecurityKey(xMsg.security_code());

            //this net-object bind a user's account
            pNetObject->SetAccount(xMsg.account());

            SquickStruct::AckEventResult xSendMsg;
            xSendMsg.set_event_code(SquickStruct::VERIFY_KEY_SUCCESS);
            *xSendMsg.mutable_event_client() = INetModule::StructToProtobuf(pNetObject->GetClientID());

			m_pNetModule->SendMsgPB(SquickStruct::EGameMsgID::ACK_CONNECT_KEY, xSendMsg, sockIndex);
        }
    }
    else
    {
        //if verify failed then close this connect
		m_pNetModule->GetNet()->CloseNetObject(sockIndex);
    }
}

void ProxyServerNet_ServerModule::OnSocketClientEvent(const SQUICK_SOCKET sockIndex, const SQUICK_NET_EVENT eEvent, INet* pNet)
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

void ProxyServerNet_ServerModule::OnClientDisconnect(const SQUICK_SOCKET nAddress)
{
    NetObject* pNetObject = m_pNetModule->GetNet()->GetNetObject(nAddress);
    if (pNetObject)
    {
        int nGameID = pNetObject->GetGameID();
        if (nGameID > 0)
        {
            //when a net-object bind a account then tell that game-server
            if (!pNetObject->GetUserID().IsNull())
            {
                SquickStruct::ReqLeaveGameServer xData;
				xData.set_arg(nGameID);

                SquickStruct::MsgBase xMsg;

				//real user id
                *xMsg.mutable_player_id() = INetModule::StructToProtobuf(pNetObject->GetUserID());

                if (!xData.SerializeToString(xMsg.mutable_msg_data()))
                {
                    return;
                }

                std::string msg;
                if (!xMsg.SerializeToString(&msg))
                {
                    return;
                }

				m_pNetClientModule->SendByServerIDWithOutHead(nGameID, SquickStruct::EGameMsgID::REQ_LEAVE_GAME, msg);
            }
        }

        mxClientIdent.RemoveElement(pNetObject->GetClientID());
    }
}

// 选择服务器
void ProxyServerNet_ServerModule::OnSelectServerProcess(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len)
{
	NetObject* pNetObject = m_pNetModule->GetNet()->GetNetObject(sockIndex);
	if (!pNetObject)
	{
		return;
	}

    // 验证
	std::string strMsgData = m_pSecurityModule->DecodeMsg(pNetObject->GetAccount(), pNetObject->GetSecurityKey(), msgID, msg, len);
	if (strMsgData.empty())
	{
		// decode failed
		return;
	}

    Guid nPlayerID;
    SquickStruct::ReqSelectServer xMsg;
    if (!m_pNetModule->ReceivePB( msgID, strMsgData, xMsg, nPlayerID))
    {
        return;
    }

#ifdef SQUICK_DEV
    std::cout << "客户端选择服务器: " << xMsg.world_id() << std::endl;
#endif

    SQUICK_SHARE_PTR<ConnectData> pServerData = m_pNetClientModule->GetServerNetInfo(xMsg.world_id());
    if (pServerData && ConnectDataState::NORMAL == pServerData->eState)
    {
	    //Modify: not need check pNetObject again by wenmin
        //NetObject* pNetObject = m_pNetModule->GetNet()->GetNetObject(sockIndex);
        //if (pNetObject)
        //{
        //now this client bind a game server, all message will be sent to this game server whom bind with client
            std::cout << " Set Game ID: " << xMsg.world_id() << " status: " << pServerData->eState << std::endl;
            pNetObject->SetGameID(xMsg.world_id());

            SquickStruct::AckEventResult xMsg;
            xMsg.set_event_code(SquickStruct::EGameEventCode::SELECTSERVER_SUCCESS);
			m_pNetModule->SendMsgPB(SquickStruct::EGameMsgID::ACK_SELECT_SERVER, xMsg, sockIndex);
            return;
        //}
    }else {
        std::cout << " 查找存在问题: " << pServerData << std::endl;
        if(pServerData) {
            std::cout << " status: " << pServerData->eState << std::endl;
        }
    }
    // 查找最小负载的服务器

    //actually, if you want the game server working with a good performance then we need to find the game server with lowest workload
	int nWorkload = 999999;
	int nGameID = 0;
    MapEx<int, ConnectData>& xServerList = m_pNetClientModule->GetServerList();
    ConnectData* pGameData = xServerList.FirstNude();
    while (pGameData)
    {
        if (ConnectDataState::NORMAL == pGameData->eState
            && SQUICK_SERVER_TYPES::SQUICK_ST_GAME == pGameData->eServerType)
        {
			if (pGameData->nWorkLoad < nWorkload)
			{
				nWorkload = pGameData->nWorkLoad;
				nGameID = pGameData->nGameID;
			}
        }
        //std::cout << "在代理服务器上已注册的服务器：" << pGameData->nGameID  << " Status: " << pGameData->eState << std::endl;

        pGameData = xServerList.NextNude();
    }

	if (nGameID > 0)
	{
#ifdef SQUICK_DEV
        std::cout << " 根据工作负载选择服务器: Server ID: " << nGameID << std::endl;
#endif
		pNetObject->SetGameID(nGameID);

		SquickStruct::AckEventResult xMsg;
		xMsg.set_event_code(SquickStruct::EGameEventCode::SELECTSERVER_SUCCESS);
		m_pNetModule->SendMsgPB(SquickStruct::EGameMsgID::ACK_SELECT_SERVER, xMsg, sockIndex);
		return;
	}
	
    SquickStruct::AckEventResult xSendMsg;
    xSendMsg.set_event_code(SquickStruct::EGameEventCode::SELECTSERVER_FAIL);
	m_pNetModule->SendMsgPB(SquickStruct::EGameMsgID::ACK_SELECT_SERVER, xMsg, sockIndex);
}

void ProxyServerNet_ServerModule::OnReqServerListProcess(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len)
{
	NetObject* pNetObject = m_pNetModule->GetNet()->GetNetObject(sockIndex);
	if (!pNetObject)
	{
		return;
	}

	std::string strMsgData = m_pSecurityModule->DecodeMsg(pNetObject->GetAccount(), pNetObject->GetSecurityKey(), msgID, msg, len);
	if (strMsgData.empty())
	{
		//decode failed
		return;
	}

    if (pNetObject->GetConnectKeyState() > 0)
    {
		Guid nPlayerID; //no value
		SquickStruct::ReqServerList xMsg;
		if (!m_pNetModule->ReceivePB( msgID, strMsgData, xMsg, nPlayerID))
		{
			return;
		}

		if (xMsg.type() != SquickStruct::RSLT_GAMES_ERVER)
		{
			return;
		}

        //ack all gameserver data
        SquickStruct::AckServerList xData;
        xData.set_type(SquickStruct::RSLT_GAMES_ERVER);

        MapEx<int, ConnectData>& xServerList = m_pNetClientModule->GetServerList();
        ConnectData* pGameData = xServerList.FirstNude();
        while (pGameData)
        {
            if (ConnectDataState::NORMAL == pGameData->eState
				&& SQUICK_SERVER_TYPES::SQUICK_ST_GAME == pGameData->eServerType)
            {
                SquickStruct::ServerInfo* pServerInfo = xData.add_info();

                pServerInfo->set_name(pGameData->name);
                pServerInfo->set_status(SquickStruct::EServerState::EST_NARMAL);
                pServerInfo->set_server_id(pGameData->nGameID);
                pServerInfo->set_wait_count(0);
            }

            pGameData = xServerList.NextNude();
        }
		m_pNetModule->SendMsgPB(SquickStruct::EGameMsgID::ACK_WORLD_LIST, xData, sockIndex);
    }
}

int ProxyServerNet_ServerModule::Transport(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len)
{
    SquickStruct::MsgBase xMsg;
    if (!xMsg.ParseFromArray(msg, len))
    {
        char szData[MAX_PATH] = { 0 };
        sprintf(szData, "Parse Message Failed from Packet to MsgBase, MessageID: %d\n", msgID);

        return false;
    }

    // broadcast many palyers
    for (int i = 0; i < xMsg.player_client_list_size(); ++i)
    {
        SQUICK_SHARE_PTR<SQUICK_SOCKET> pFD = mxClientIdent.GetElement(INetModule::ProtobufToStruct(xMsg.player_client_list(i)));
        if (pFD)
        {
            if (xMsg.has_hash_ident())
            {
                NetObject* pNetObject = m_pNetModule->GetNet()->GetNetObject(*pFD);
                if (pNetObject)
                {
                    pNetObject->SetHashIdentID(INetModule::ProtobufToStruct(xMsg.hash_ident()));
                }
            }
			m_pNetModule->SendMsgWithOutHead(msgID, std::string(msg, len), *pFD);
        }
    }

    //send message to one player
    if (xMsg.player_client_list_size() <= 0)
    {
		Guid xClientIdent = INetModule::ProtobufToStruct(xMsg.player_id());
        SQUICK_SHARE_PTR<SQUICK_SOCKET> pFD = mxClientIdent.GetElement(xClientIdent);
        if (pFD)
        {
            if (xMsg.has_hash_ident())
            {
                NetObject* pNetObject = m_pNetModule->GetNet()->GetNetObject(*pFD);
                if (pNetObject)
                {
                    pNetObject->SetHashIdentID(INetModule::ProtobufToStruct(xMsg.hash_ident()));
                }
            }

			m_pNetModule->SendMsgWithOutHead(msgID, std::string(msg, len), *pFD);
        }
		else if(xClientIdent.IsNull())
		{
			//send this msessage to all clientss
			m_pNetModule->GetNet()->SendMsgToAllClientWithOutHead(msgID, msg, len);
		}
		//pFD is empty means end of connection, no need to send message to this client any more. And,
		//we should never send a message that specified to a player to all clients here.
		else
		{
		}
    }

    return true;
}

void ProxyServerNet_ServerModule::OnClientConnected(const SQUICK_SOCKET nAddress)
{
    std::cout << "Client Connected.... \n";
	//bind client'id with socket id
    Guid xClientIdent = m_pKernelModule->CreateGUID();
    NetObject* pNetObject = m_pNetModule->GetNet()->GetNetObject(nAddress);
    if (pNetObject)
    {
        pNetObject->SetClientID(xClientIdent);
    }

    mxClientIdent.AddElement(xClientIdent, SQUICK_SHARE_PTR<SQUICK_SOCKET>(new SQUICK_SOCKET(nAddress)));
}

void ProxyServerNet_ServerModule::OnReqRoleListProcess(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len)
{
    dout << "ProxyServerNet_ServerModule::OnReqRoleListProcess: 获取角色列表中\n";
	NetObject* pNetObject = m_pNetModule->GetNet()->GetNetObject(sockIndex);
	if (!pNetObject)
	{
		return;
	}
	std::string strMsgData = m_pSecurityModule->DecodeMsg(pNetObject->GetAccount(), pNetObject->GetSecurityKey(), msgID, msg, len);
	if (strMsgData.empty())
	{
		//decode failed
		return;
	}
    Guid nPlayerID;
    SquickStruct::ReqRoleList xData;
    if (!m_pNetModule->ReceivePB( msgID, msg, len, xData, nPlayerID))
    {
        return;
    }
    std::cout << "ProxyServerNet_ServerModule::OnReqRoleListProcess Player.nHead64" << nPlayerID.nHead64 << "Player.nData64" << nPlayerID.nData64 << "\n";
    SQUICK_SHARE_PTR<ConnectData> pServerData = m_pNetClientModule->GetServerNetInfo(xData.game_id());
    if (!pServerData)
    {
        pServerData = m_pNetClientModule->GetServerNetInfo(SQUICK_SERVER_TYPES::SQUICK_ST_GAME);
        if (pServerData)
        {
            pNetObject->SetGameID(pServerData->nGameID);
        }
    }

    if (pServerData && ConnectDataState::NORMAL == pServerData->eState)
    {
        if (pNetObject->GetConnectKeyState() > 0
            && pNetObject->GetGameID() == pServerData->nGameID
            && pNetObject->GetAccount() == xData.account())
        {
            SquickStruct::MsgBase xMsg;
            if (!xData.SerializeToString(xMsg.mutable_msg_data()))
            {
                return;
            }

			//clientid
            xMsg.mutable_player_id()->CopyFrom(INetModule::StructToProtobuf(pNetObject->GetClientID()));

            std::string msg;
            if (!xMsg.SerializeToString(&msg))
            {
                return;
            }

			m_pNetClientModule->SendByServerIDWithOutHead(pNetObject->GetGameID(), SquickStruct::EGameMsgID::REQ_ROLE_LIST, msg);
        }else {
            std::cout << "ProxyServerNet_ServerModule::OnReqRoleListProcess 失败...\n" << pServerData->nGameID << " / " <<  pNetObject->GetGameID()
            << "\n" << pNetObject->GetAccount()  << " " << xData.account();
        }
    }
    else
    {
        m_pLogModule->LogError(pNetObject->GetClientID(), "account cant get a game server:" + xData.account(), __FILE__, __LINE__);
    }
}

void ProxyServerNet_ServerModule::OnReqCreateRoleProcess(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len)
{
	NetObject* pNetObject = m_pNetModule->GetNet()->GetNetObject(sockIndex);
	if (!pNetObject)
	{
		return;
	}
    
	std::string strMsgData = m_pSecurityModule->DecodeMsg(pNetObject->GetAccount(), pNetObject->GetSecurityKey(), msgID, msg, len);
	if (strMsgData.empty())
	{
		//decode failed
		return;
	}

    Guid nPlayerID; //no value
    SquickStruct::ReqCreateRole xData;
    if (!m_pNetModule->ReceivePB( msgID, msg, len, xData, nPlayerID))
    {
        return;
    }
    std::cout << "创建角色\n";
    SQUICK_SHARE_PTR<ConnectData> pServerData = m_pNetClientModule->GetServerNetInfo(pNetObject->GetGameID());
    if (pServerData && ConnectDataState::NORMAL == pServerData->eState)
    {
        if (pNetObject->GetConnectKeyState() > 0
            && pNetObject->GetAccount() == xData.account())
        {
            SquickStruct::MsgBase xMsg;
            if (!xData.SerializeToString(xMsg.mutable_msg_data()))
            {
                return;
            }
            
            //the clientid == playerid before the player entre the game-server
            xMsg.mutable_player_id()->CopyFrom(INetModule::StructToProtobuf(pNetObject->GetClientID()));

            std::string msg;
            if (!xMsg.SerializeToString(&msg))
            {
                return;
            }
            std::cout << "创建角色发送中 " << pNetObject->GetGameID();
			m_pNetClientModule->SendByServerIDWithOutHead(pNetObject->GetGameID(), msgID, msg);
        }
    }
}

void ProxyServerNet_ServerModule::OnReqDelRoleProcess(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len)
{
	NetObject* pNetObject = m_pNetModule->GetNet()->GetNetObject(sockIndex);
	if (!pNetObject)
	{
		return;
	}

	std::string strMsgData = m_pSecurityModule->DecodeMsg(pNetObject->GetAccount(), pNetObject->GetSecurityKey(), msgID, msg, len);
	if (strMsgData.empty())
	{
		//decode failed
		return;
	}

    Guid nPlayerID;// no value
    SquickStruct::ReqDeleteRole xData;
    if (!m_pNetModule->ReceivePB( msgID, msg, len, xData, nPlayerID))
    {
        return;
    }

    SQUICK_SHARE_PTR<ConnectData> pServerData = m_pNetClientModule->GetServerNetInfo(xData.game_id());
	if (pServerData && ConnectDataState::NORMAL == pServerData->eState)
    {
        if (pNetObject->GetConnectKeyState() > 0
            && pNetObject->GetGameID() == xData.game_id()
            && pNetObject->GetAccount() == xData.account())
        {
			SquickStruct::MsgBase xMsg;
			if (!xData.SerializeToString(xMsg.mutable_msg_data()))
			{
				return;
			}

			//clientid
			xMsg.mutable_player_id()->CopyFrom(INetModule::StructToProtobuf(pNetObject->GetClientID()));

			std::string msg;
			if (!xMsg.SerializeToString(&msg))
			{
				return;
			}

			m_pNetClientModule->SendByServerIDWithOutHead(pNetObject->GetGameID(), msgID, msg);
        }
    }
}

// 请求进入游戏
void ProxyServerNet_ServerModule::OnReqEnterGameServer(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len)
{
	NetObject* pNetObject = m_pNetModule->GetNet()->GetNetObject(sockIndex);
	if (!pNetObject)
	{
		return;
	}

	std::string strMsgData = m_pSecurityModule->DecodeMsg(pNetObject->GetAccount(), pNetObject->GetSecurityKey(), msgID, msg, len);
	if (strMsgData.empty())
	{
		//decode failed
		return;
	}

    Guid nPlayerID;//no value
    SquickStruct::ReqEnterGameServer xData;
    if (!m_pNetModule->ReceivePB( msgID, msg, len, xData, nPlayerID))
    {
        return;
    }
    
    SQUICK_SHARE_PTR<ConnectData> pServerData = m_pNetClientModule->GetServerNetInfo(pNetObject->GetGameID());
    if (pServerData && ConnectDataState::NORMAL == pServerData->eState)
    {
        if (pNetObject->GetConnectKeyState() > 0
            && pNetObject->GetAccount() == xData.account()
            && !xData.name().empty()
            && !xData.account().empty())
        {
            SquickStruct::MsgBase xMsg;
            if (!xData.SerializeToString(xMsg.mutable_msg_data()))
            {
                return;
            }

			//clientid
            xMsg.mutable_player_id()->CopyFrom(INetModule::StructToProtobuf(pNetObject->GetClientID()));
            std::string msg;
            if (!xMsg.SerializeToString(&msg))
            {
                return;
            }
			m_pNetClientModule->SendByServerIDWithOutHead(pNetObject->GetGameID(), SquickStruct::EGameMsgID::REQ_ENTER_GAME, msg);
        }
    }
}

int ProxyServerNet_ServerModule::EnterGameSuccessEvent(const Guid xClientID, const Guid xPlayerID)
{
    SQUICK_SHARE_PTR<SQUICK_SOCKET> pFD = mxClientIdent.GetElement(xClientID);
    if (pFD)
    {
        NetObject* pNetObeject = m_pNetModule->GetNet()->GetNetObject(*pFD);
        if (pNetObeject)
        {
            pNetObeject->SetUserID(xPlayerID);
        }
    }

    return 0;
}
