#pragma once

#include <squick/plugin/kernel/i_kernel_module.h>
#include <squick/plugin/config/i_element_module.h>
#include "i_cooldown_module.h"


class CooldownModule
        : public ICooldownModule
{
public:

    CooldownModule(IPluginManager* p )
    {
        pPluginManager = p;
    }

    virtual bool AfterStart() override;

    virtual void AddCooldown(const Guid& self, const std::string& configID );
    virtual bool ExistCooldown(const Guid& self, const std::string& configID );

private:

    IKernelModule* m_pKernelModule;
    IElementModule* m_pElementModule;
};
