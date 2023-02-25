
#include "master_module.h"
#include "plugin.h"
#include <squick/struct/struct.h>
#include <squick/struct/protocol_define.h>

bool LoginToMasterModule::Start()
{
	m_pNetClientModule = pPluginManager->FindModule<INetClientModule>();
	m_pKernelModule = pPluginManager->FindModule<IKernelModule>();
	m_pLogModule = pPluginManager->FindModule<ILogModule>();
	m_pClassModule = pPluginManager->FindModule<IClassModule>();
	m_pElementModule = pPluginManager->FindModule<IElementModule>();
	m_pLoginNet_ServerModule = pPluginManager->FindModule<ILoginNet_ServerModule>();

    return true;
}

bool LoginToMasterModule::Destory()
{
    return true;
}

bool LoginToMasterModule::AfterStart()
{
	m_pNetClientModule->AddReceiveCallBack(SQUICK_SERVER_TYPES::SQUICK_ST_MASTER, SquickStruct::ACK_CONNECT_WORLD, this, &LoginToMasterModule::OnSelectServerResultProcess);
	m_pNetClientModule->AddReceiveCallBack(SQUICK_SERVER_TYPES::SQUICK_ST_MASTER, SquickStruct::STS_NET_INFO, this, &LoginToMasterModule::OnWorldInfoProcess);
	
	m_pNetClientModule->AddEventCallBack(SQUICK_SERVER_TYPES::SQUICK_ST_MASTER, this, &LoginToMasterModule::OnSocketMSEvent);

	m_pNetClientModule->ExpandBufferSize();

    SQUICK_SHARE_PTR<IClass> xLogicClass = m_pClassModule->GetElement(SquickProtocol::Server::ThisName());
    if (xLogicClass)
    {
		const std::vector<std::string>& strIdList = xLogicClass->GetIDList();
		for (int i = 0; i < strIdList.size(); ++i)
		{
			const std::string& strId = strIdList[i];

			const int serverType = m_pElementModule->GetPropertyInt32(strId, SquickProtocol::Server::Type());
			const int serverID = m_pElementModule->GetPropertyInt32(strId, SquickProtocol::Server::ServerID());
			if (serverType == SQUICK_SERVER_TYPES::SQUICK_ST_MASTER)
			{
				const int nPort = m_pElementModule->GetPropertyInt32(strId, SquickProtocol::Server::Port());
				//const int maxConnect = m_pElementModule->GetPropertyInt32(strId, SquickProtocol::Server::MaxOnline());
				//const int nCpus = m_pElementModule->GetPropertyInt32(strId, SquickProtocol::Server::CpuCount());
				const std::string& name = m_pElementModule->GetPropertyString(strId, SquickProtocol::Server::ID());
				const std::string& ip = m_pElementModule->GetPropertyString(strId, SquickProtocol::Server::IP());

				ConnectData xServerData;

				xServerData.nGameID = serverID;
				xServerData.eServerType = (SQUICK_SERVER_TYPES)serverType;
				xServerData.ip = ip;
				xServerData.nPort = nPort;
				xServerData.name = strId;

				m_pNetClientModule->AddServer(xServerData);
			}
		}
    }



    return true;
}

bool LoginToMasterModule::BeforeDestory()
{

    return false;
}

bool LoginToMasterModule::Update()
{
	ServerReport();
	return true;
}

void LoginToMasterModule::Register(INet* pNet)
{
    SQUICK_SHARE_PTR<IClass> xLogicClass = m_pClassModule->GetElement(SquickProtocol::Server::ThisName());
    if (xLogicClass)
    {
		const std::vector<std::string>& strIdList = xLogicClass->GetIDList();
		for (int i = 0; i < strIdList.size(); ++i)
		{
			const std::string& strId = strIdList[i];

            const int serverType = m_pElementModule->GetPropertyInt32(strId, SquickProtocol::Server::Type());
            const int serverID = m_pElementModule->GetPropertyInt32(strId, SquickProtocol::Server::ServerID());
            if (serverType == SQUICK_SERVER_TYPES::SQUICK_ST_LOGIN && pPluginManager->GetAppID() == serverID)
            {
                const int nPort = m_pElementModule->GetPropertyInt32(strId, SquickProtocol::Server::Port());
                const int maxConnect = m_pElementModule->GetPropertyInt32(strId, SquickProtocol::Server::MaxOnline());
                //const int nCpus = m_pElementModule->GetPropertyInt32(strId, SquickProtocol::Server::CpuCount());
                const std::string& name = m_pElementModule->GetPropertyString(strId, SquickProtocol::Server::ID());
                const std::string& ip = m_pElementModule->GetPropertyString(strId, SquickProtocol::Server::IP());

                SquickStruct::ServerInfoReportList xMsg;
                SquickStruct::ServerInfoReport* pData = xMsg.add_server_list();

                pData->set_server_id(serverID);
                pData->set_server_name(strId);
                pData->set_server_cur_count(0);
                pData->set_server_ip(ip);
                pData->set_server_port(nPort);
                pData->set_server_max_online(maxConnect);
                pData->set_server_state(SquickStruct::EST_NARMAL);
                pData->set_server_type(serverType);

                SQUICK_SHARE_PTR<ConnectData> pServerData = m_pNetClientModule->GetServerNetInfo(pNet);
                if (pServerData)
                {
                    int nTargetID = pServerData->nGameID;
					m_pNetClientModule->SendToServerByPB(nTargetID, SquickStruct::ServerMsgId::LOGIN_TO_MASTER_REGISTERED, xMsg);
                    m_pLogModule->LogInfo(Guid(0, pData->server_id()), pData->server_name(), "Register");
                }
            }
        }
    }
}

void LoginToMasterModule::ServerReport()
{
	if (mLastReportTime + 10 > pPluginManager->GetNowTime())
	{
		return;
	}
	mLastReportTime = pPluginManager->GetNowTime();
	std::shared_ptr<IClass> xLogicClass = m_pClassModule->GetElement(SquickProtocol::Server::ThisName());
	if (xLogicClass)
	{
		const std::vector<std::string>& strIdList = xLogicClass->GetIDList();
		for (int i = 0; i < strIdList.size(); ++i)
		{
			const std::string& strId = strIdList[i];

			const int serverType = m_pElementModule->GetPropertyInt32(strId, SquickProtocol::Server::Type());
			const int serverID = m_pElementModule->GetPropertyInt32(strId, SquickProtocol::Server::ServerID());
			if (pPluginManager->GetAppID() == serverID)
			{
				const int nPort = m_pElementModule->GetPropertyInt32(strId, SquickProtocol::Server::Port());
				const int maxConnect = m_pElementModule->GetPropertyInt32(strId, SquickProtocol::Server::MaxOnline());
				const std::string& name = m_pElementModule->GetPropertyString(strId, SquickProtocol::Server::ID());
				const std::string& ip = m_pElementModule->GetPropertyString(strId, SquickProtocol::Server::IP());

				SquickStruct::ServerInfoReport reqMsg;

				reqMsg.set_server_id(serverID);
				reqMsg.set_server_name(strId);
				reqMsg.set_server_cur_count(0);
				reqMsg.set_server_ip(ip);
				reqMsg.set_server_port(nPort);
				reqMsg.set_server_max_online(maxConnect);
				reqMsg.set_server_state(SquickStruct::EST_NARMAL);
				reqMsg.set_server_type(serverType);

				m_pNetClientModule->SendToAllServerByPB(SQUICK_SERVER_TYPES::SQUICK_ST_MASTER, SquickStruct::STS_SERVER_REPORT, reqMsg, Guid());
			}
		}
	}
}

void LoginToMasterModule::OnSelectServerResultProcess(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len)
{
    Guid nPlayerID;
    SquickStruct::AckConnectWorldResult xMsg;
    if (!INetModule::ReceivePB( msgID, msg, len, xMsg, nPlayerID))
    {
        return;
    }

    m_pLoginNet_ServerModule->OnSelectWorldResultsProcess(xMsg.world_id(), INetModule::ProtobufToStruct(xMsg.sender()), xMsg.login_id(), xMsg.account(), xMsg.world_ip(), xMsg.world_port(), xMsg.world_key());
}

void LoginToMasterModule::OnSocketMSEvent(const SQUICK_SOCKET sockIndex, const SQUICK_NET_EVENT eEvent, INet* pNet)
{
    if (eEvent & SQUICK_NET_EVENT_EOF)
    {
        m_pLogModule->LogInfo(Guid(0, sockIndex), "SQUICK_NET_EVENT_EOF Connection closed", __FUNCTION__, __LINE__);
    }
    else if (eEvent & SQUICK_NET_EVENT_ERROR)
    {
        m_pLogModule->LogInfo(Guid(0, sockIndex), "SQUICK_NET_EVENT_ERROR Got an error on the connection", __FUNCTION__, __LINE__);
    }
    else if (eEvent & SQUICK_NET_EVENT_TIMEOUT)
    {
        m_pLogModule->LogInfo(Guid(0, sockIndex), "SQUICK_NET_EVENT_TIMEOUT read timeout", __FUNCTION__, __LINE__);
    }
    else  if (eEvent & SQUICK_NET_EVENT_CONNECTED)
    {
        m_pLogModule->LogInfo(Guid(0, sockIndex), "SQUICK_NET_EVENT_CONNECTED connected success", __FUNCTION__, __LINE__);
        Register(pNet);
    }
}

void LoginToMasterModule::OnWorldInfoProcess(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len)
{
    Guid nPlayerID ;
    SquickStruct::ServerInfoReportList xMsg;
    if (!INetModule::ReceivePB( msgID, msg, len, xMsg, nPlayerID))
    {
        return;
    }

    for (int i = 0; i < xMsg.server_list_size(); ++i)
    {
        const SquickStruct::ServerInfoReport& xData = xMsg.server_list(i);

        SQUICK_SHARE_PTR<SquickStruct::ServerInfoReport> pServerData = mWorldMap.GetElement(xData.server_id());
        if (!pServerData)
        {
            pServerData = SQUICK_SHARE_PTR<SquickStruct::ServerInfoReport>(SQUICK_NEW SquickStruct::ServerInfoReport());
            *pServerData = xData;

            mWorldMap.AddElement(xData.server_id(), pServerData);
        }

    }

    m_pLogModule->LogInfo(Guid(0, xMsg.server_list_size()), "", "WorldInfo");
}

INetClientModule* LoginToMasterModule::GetClusterModule()
{
	return m_pNetClientModule;
}

MapEx<int, SquickStruct::ServerInfoReport>& LoginToMasterModule::GetWorldMap()
{
    return mWorldMap;
}