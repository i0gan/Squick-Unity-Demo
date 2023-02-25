#include "manager_module.h"
#include <squick/struct/protocol_define.h>
// #include <squick/plugin/lua/export.h>
// #include <third_party/nlohmann/json.hpp>
namespace pvp_manager::logic {
bool ManagerModule::Start()
{
	m_pKernelModule = pPluginManager->FindModule<IKernelModule>();
	m_pClassModule = pPluginManager->FindModule<IClassModule>();
	m_pElementModule = pPluginManager->FindModule<IElementModule>();
	m_pLuaScriptModule = pPluginManager->FindModule<ILuaScriptModule>();
	m_pLogModule = pPluginManager->FindModule<ILogModule>();
	return true;
}

bool ManagerModule::Destory()
{
	return true;
}

bool ManagerModule::AfterStart()
{
    std::cout << "启动PVP管理服务器模块\n";
	return true;
}

bool ManagerModule::Update()
{
	return true;
}

}