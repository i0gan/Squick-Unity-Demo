#pragma once

#include <iostream>
#include <squick/core/i_module.h>

class ICooldownModule
    : public IModule
{
public:

    virtual void AddCooldown(const Guid& self, const std::string& configID) = 0;
    virtual bool ExistCooldown(const Guid& self, const std::string& configID) = 0;
};
