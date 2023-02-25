#pragma once
#include <squick/core/base.h>

namespace game::pvp {
class IPvpManagerModule : public IModule
{
public:
    virtual void PvpInstanceCreate(const string& instance_id, const string& key) = 0;
};

}