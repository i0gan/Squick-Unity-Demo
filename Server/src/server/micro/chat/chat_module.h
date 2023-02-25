#pragma once

#include <memory>
#include <squick/plugin/net/i_net_module.h>
#include <squick/plugin/log/i_log_module.h>
#include <squick/plugin/kernel/i_kernel_module.h>

#include "i_chat_module.h"
#include <squick/struct/protocol_define.h>
#include <server/game/server/i_server_module.h>


class ChatModule
    : public IChatModule
{
public:
	ChatModule(IPluginManager* p)
    {
        pPluginManager = p;
    }
    virtual bool Start() override;
    virtual bool Destory() override;
    virtual bool Update() override;

    virtual bool AfterStart() override;

private:
	void OnClientChatProcess(const SQUICK_SOCKET sockIndex, const int msgID, const char* msg, const uint32_t len);

protected:

    //////////////////////////////////////////////////////////////////////////
    ILogModule* m_pLogModule;
	INetModule* m_pNetModule;
	IKernelModule* m_pKernelModule;
	IGameServerNet_ServerModule* m_pGameServerNet_ServerModule;
};
