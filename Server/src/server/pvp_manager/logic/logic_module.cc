
#include "plugin.h"
#include "logic_module.h"
namespace pvp_manager::logic {
bool LogicModule::Start()
{
    return true;
}

bool LogicModule::Destory()
{
    return true;
}

bool LogicModule::Update()
{
    return true;
}


bool LogicModule::AfterStart()
{
    m_pKernelModule = pPluginManager->FindModule<IKernelModule>();
    m_pClassModule = pPluginManager->FindModule<IClassModule>();
	m_pNetModule = pPluginManager->FindModule<INetModule>();
	m_pNetClientModule = pPluginManager->FindModule<INetClientModule>();

	// 来自客户端
	m_pNetModule->AddReceiveCallBack(SquickStruct::REQ_LAG_TEST, this, &LogicModule::OnLagTestProcess);


	// 来自Game 服务器
	m_pNetClientModule->AddReceiveCallBack(SQUICK_SERVER_TYPES::SQUICK_ST_GAME, SquickStruct::REQ_PVP_INSTANCE_CREATE, this, &LogicModule::OnReqPvpInstanceCreate);

    return true;
}

void LogicModule::OnLagTestProcess(const SQUICK_SOCKET sockIndex, const int msgID, const char * msg, const uint32_t len)
{
	std::string msgDatag(msg, len);
	m_pNetModule->SendMsgWithOutHead(SquickStruct::EGameMsgID::ACK_GATE_LAG_TEST, msgDatag, sockIndex);

	//TODO improve performance
	NetObject* pNetObject = m_pNetModule->GetNet()->GetNetObject(sockIndex);
	if (pNetObject)
	{
		const int gameID = pNetObject->GetGameID();
		// 避免不必要的开销，心跳包与PVP Manager进行跳动就行
		//m_pNetClientModule->SendByServerIDWithOutHead(gameID, msgID, msgDatag);
	}
}

// Game Server请求创建PVP服务器实例
void LogicModule::OnReqPvpInstanceCreate(const SQUICK_SOCKET sockIndex, const int msgID, const char * msg, const uint32_t len)
{

	Guid tmpID; // 服务端之间推送，ID值无效
	SquickStruct::ReqPvpInstanceCreate xMsg;
	if (!m_pNetModule->ReceivePB(msgID, msg, len, xMsg, tmpID)) {
		return;
	}

	dout << "Game Server 请求创建PVP实例 from " << xMsg.game_id() << std::endl;
	string cmd;
	// 为了测试方便，先暂时采用system来启动PVP服务器，后期采用docker进行管理 PVP 服务器
#if SQUICK_PLATFORM == SQUICK_PLATFORM_WIN
	cmd = "start ../../client/Build/Server/GunVR.exe";
	cmd += " -instance_id=" + xMsg.instance_id();
	cmd += " -key=" + string("abcd");
	cmd += " -game_id=" + string("16001");
	cmd += " -mip=" + string("192.168.0.196"); // PVP Manager IP
	cmd += " -mport=" + string("20001");       // PVP Manager 端口
	cmd += " -ip=" + string("192.168.0.196");  // PVP IP地址
#else
	cmd = "docker run -d gunvr_pvp pvp_1";
#endif
	dout << "执行: " << cmd << std::endl;
	system(cmd.c_str());
}

}