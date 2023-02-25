

#include "chat_module.h"
#include <squick/struct/protocol_define.h>

bool ChatModule::Start()
{
	m_pNetModule = pPluginManager->FindModule<INetModule>();
	m_pKernelModule = pPluginManager->FindModule<IKernelModule>();
	m_pLogModule = pPluginManager->FindModule<ILogModule>();
	m_pGameServerNet_ServerModule = pPluginManager->FindModule<IGameServerNet_ServerModule>();

	return true;
}

bool ChatModule::AfterStart()
{
	m_pNetModule->AddReceiveCallBack(SquickStruct::REQ_CHAT, this, &ChatModule::OnClientChatProcess);

	return true;
}

bool ChatModule::Destory()
{

	return true;
}

bool ChatModule::Update()
{
	return true;
}

void ChatModule::OnClientChatProcess(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len)
{
	CLIENT_MSG_PROCESS( msgID, msg, len, SquickStruct::ReqAckPlayerChat);

	switch (xMsg.chat_channel())
	{
	case SquickStruct::ReqAckPlayerChat::EGCC_GLOBAL:
	{
		//this code means the game server will sends a message to all players who playing game
		m_pNetModule->SendMsgPBToAllClient(SquickStruct::ACK_CHAT, xMsg);
	}
	break;
	case SquickStruct::ReqAckPlayerChat::EGCC_ROOM:
	{
		const int sceneID = m_pKernelModule->GetPropertyInt(nPlayerID, SquickProtocol::Player::SceneID());
		const int groupID = m_pKernelModule->GetPropertyInt(nPlayerID, SquickProtocol::Player::GroupID());

		//this code means the game server will sends a message to all players who in the same room
		m_pGameServerNet_ServerModule->SendGroupMsgPBToGate(SquickStruct::ACK_CHAT, xMsg, sceneID, groupID);
	}
	break;
	default:
	{
		//this code means the game server will sends a message yourself(nPlayerID)
		m_pGameServerNet_ServerModule->SendMsgPBToGate(SquickStruct::ACK_CHAT, xMsg, nPlayerID);
	}
	break;;
	}
}