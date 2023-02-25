#pragma once

#include <iostream>
#include <squick/core/i_module.h>

class PosSyncUnit;

class ISyncPosModule
    : public IModule
{
public:
    virtual bool RequireMove(const Guid scene_group, const PosSyncUnit& syncUnit) = 0;
};
