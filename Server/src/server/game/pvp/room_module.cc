#include "room_module.h"
#include <squick/struct/protocol_define.h>
// #include <squick/plugin/lua/export.h>
// #include <third_party/nlohmann/json.hpp>

namespace game::pvp {
bool RoomModule::Start()
{
	m_pNetModule = pPluginManager->FindModule<INetModule>();
	
	m_pKernelModule = pPluginManager->FindModule<IKernelModule>();
	m_pClassModule = pPluginManager->FindModule<IClassModule>();
	m_pElementModule = pPluginManager->FindModule<IElementModule>();
	m_pLuaScriptModule = pPluginManager->FindModule<ILuaScriptModule>();
	m_pLogModule = pPluginManager->FindModule<ILogModule>();
	m_pPvpManagerModule = pPluginManager->FindModule<IPvpManagerModule>();

	m_pGameServerNet_ServerModule = pPluginManager->FindModule<IGameServerNet_ServerModule>();
	m_pScheduleModule = pPluginManager->FindModule<IScheduleModule>();
	m_pDataTailModule = pPluginManager->FindModule<IDataTailModule>();
	m_pSceneModule = pPluginManager->FindModule<ISceneModule>();
	m_pSceneProcessModule = pPluginManager->FindModule<ISceneProcessModule>();
	m_pEventModule = pPluginManager->FindModule<IEventModule>();

	return true;
}

bool RoomModule::Destory()
{
	return true;
}

bool RoomModule::AfterStart()
{
    dout << "启动房间模块\n";
	m_pNetModule->AddReceiveCallBack(SquickStruct::EGameMsgID::REQ_ROOM_CREATE, this, &RoomModule::OnReqRoomCreate);
	m_pNetModule->AddReceiveCallBack(SquickStruct::EGameMsgID::REQ_ROOM_LIST, this, &RoomModule::OnReqRoomList);
	m_pNetModule->AddReceiveCallBack(SquickStruct::EGameMsgID::REQ_ROOM_DETAILS, this, &RoomModule::OnReqRoomDetails);
	m_pNetModule->AddReceiveCallBack(SquickStruct::EGameMsgID::REQ_ROOM_JOIN, this, &RoomModule::OnReqRoomJoin);
	m_pNetModule->AddReceiveCallBack(SquickStruct::EGameMsgID::REQ_ROOM_QUIT, this, &RoomModule::OnReqRoomQuit);
	m_pNetModule->AddReceiveCallBack(SquickStruct::EGameMsgID::REQ_ROOM_PLAYER_EVENT, this, &RoomModule::OnReqRoomPlayerEvent);

	m_pNetModule->AddReceiveCallBack(SquickStruct::EGameMsgID::REQ_START_PVP_GAME, this, &RoomModule::OnReqStartPvpGame);
	m_pNetModule->AddReceiveCallBack(SquickStruct::EGameMsgID::REQ_JOIN_PVP_GAME, this, &RoomModule::OnReqJoinPvpGame);
	m_pNetModule->AddReceiveCallBack(SquickStruct::EGameMsgID::REQ_QUIT_PVP_GAME, this, &RoomModule::OnReqQuitPvpGame);

	m_pNetModule->AddReceiveCallBack(SquickStruct::ServerMsgId::REQ_PVP_GAME_INIT, this, &RoomModule::OnReqPvpGameInit);
	
	return true;
}

bool RoomModule::Update()
{
	return true;
}
// 创建房间时，房主申请进入一个PVP场景创建一个公共组，其他玩家可获取这个group id
void RoomModule::OnReqRoomCreate(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len) {
	int sceneId = 3; // PVP 场景

	Guid clientID;
	SquickStruct::ReqRoomCreate xMsg;
	if (!m_pNetModule->ReceivePB(msgID, msg, len, xMsg, clientID)) {
		return;
	}

	dout << " OnReqRoomCreate: \n" << xMsg.name();
	
	// 获取玩家对象
	SQUICK_SHARE_PTR<IObject> pObject = m_pKernelModule->GetObjectA(clientID);
	if (pObject == nullptr) {
		dout << " No this player: \n" << xMsg.name();
		return;
	}

	// 将玩家加入组
	const Vector3& pos = m_pSceneModule->GetRelivePosition(sceneId, 0); // 生成出生点
	// 测试启动
	//m_pPvpManagerModule->PvpInstanceCreate("123456", "test_key");
	const int groupID = m_pKernelModule->RequestGroupScene(sceneId); // 在PVP场景中申请一个groupID
	auto roomDetails = SQUICK_SHARE_PTR<SquickStruct::RoomDetails>(SQUICK_NEW SquickStruct::RoomDetails());

	// .......
	roomDetails->set_id(groupID);
	roomDetails->set_name(xMsg.name());
	roomDetails->set_nplayers(1); // 房间人数
	roomDetails->set_status(SquickStruct::RoomStatus::ROOM_PLAYERS_PREPARE); // 房间准备中
	m_rooms[groupID] = roomDetails;

	// 通知玩家创建组成功
	SquickStruct::AckRoomCreate ack;
	ack.set_room_id(groupID);
	m_pGameServerNet_ServerModule->SendMsgPBToGate(SquickStruct::ACK_ROOM_CREATE, ack, clientID);

	dout << "申请Group id: " << groupID << " \n";
	m_pSceneModule->SetPropertyObject(sceneId, groupID, SquickProtocol::Group::MasterID(), pObject->Self());
	m_pSceneProcessModule->RequestEnterScene(pObject->Self(), sceneId, groupID, 0, pos, DataList::Empty());
}


// 请求加入房间
void RoomModule::OnReqRoomJoin(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len) {
    dout << "OnReqRoomJoin\n";

}

// 获取房间列表
void RoomModule::OnReqRoomList(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len) {
	dout << "OnReqRoomList\n";
	
	Guid clientID;
	SquickStruct::ReqRoomList xMsg;
	if (!m_pNetModule->ReceivePB(msgID, msg, len, xMsg, clientID)) {
		return;
	}

	int start = xMsg.start();
	int limit = xMsg.limit();
	if (start >= m_rooms.size()) {
		dout << "bad request\n";
		return;
	}

	SquickStruct::AckRoomList ack;
	SquickStruct::RoomSimple* room;

	int i = 0;
	for (auto iter = m_rooms.begin(); iter != m_rooms.end(); ++iter) {
		if (i > 100) {
			break;
		}
		room = ack.add_list();
		room->set_name(iter->second->name());
		room->set_id(iter->second->id());
		dout << "遍历: " << iter->second->name() << std::endl;
	}

	m_pGameServerNet_ServerModule->SendMsgPBToGate(SquickStruct::ACK_ROOM_LIST, ack, clientID);
}

void RoomModule::OnReqRoomDetails(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len) {
	dout << "OnReqRoomDetails\n";
}


void RoomModule::OnReqRoomQuit(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len) {
	dout << "OnReqRoomQuit\n";
}

void RoomModule::OnReqRoomPlayerEvent(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len) {
	dout << "OnReqRoomQuit\n";
}

void RoomModule::OnReqStartPvpGame(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len) {
	dout << "OnReqStartGame\n";
	// 启动 PVP 服务器
	m_pPvpManagerModule->PvpInstanceCreate("123456", "test_key");
}

void RoomModule::OnReqJoinPvpGame(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len) {
	dout << "OnReqJoinPvpGame\n";

}

void RoomModule::OnReqQuitPvpGame(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len) {
	dout << "OnReqQuitPvpGame\n";
}

// 有PVP Server调用
void RoomModule::OnReqPvpGameInit(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len) {
	dout << "OnReqPvpGameInit\n";

}


}