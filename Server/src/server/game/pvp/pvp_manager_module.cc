#include "pvp_manager_module.h"
#include <squick/struct/protocol_define.h>
// #include <squick/plugin/lua/export.h>
// #include <third_party/nlohmann/json.hpp>
namespace game::pvp {
bool PvpManagerModule::Start()
{
	m_pNetModule = pPluginManager->FindModule<INetModule>();
	m_pKernelModule = pPluginManager->FindModule<IKernelModule>();
	m_pClassModule = pPluginManager->FindModule<IClassModule>();
	m_pElementModule = pPluginManager->FindModule<IElementModule>();
	m_pLuaScriptModule = pPluginManager->FindModule<ILuaScriptModule>();
	m_pLogModule = pPluginManager->FindModule<ILogModule>();
	m_pNetClientModule = pPluginManager->FindModule<INetClientModule>();
	m_pGameServerNet_ServerModule = pPluginManager->FindModule<IGameServerNet_ServerModule>();
	return true;
}

bool PvpManagerModule::Destory()
{
	return true;
}

bool PvpManagerModule::AfterStart()
{
	return true;
}

bool PvpManagerModule::Update()
{
	return true;
}

// 创建PVP实例
void PvpManagerModule::PvpInstanceCreate(const string& instance_id, const string& key) {
	dout << "玩家请求创建实例\n";
	SquickStruct::ReqPvpInstanceCreate xMsg;
	xMsg.set_instance_id(instance_id);
	xMsg.set_game_id(pPluginManager->GetAppID()); // 获取当前Game ID
	m_pGameServerNet_ServerModule->SendMsgPBToPvpManager(SquickStruct::REQ_PVP_INSTANCE_CREATE, xMsg);
}


}