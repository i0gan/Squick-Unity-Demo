#pragma once

#include <iostream>
#include <squick/core/i_module.h>
class ISceneProcessModule
    : public IModule
{
public:

	virtual bool RequestEnterScene(const Guid& self, const int sceneID, const int nGrupID, const int type, const Vector3& pos, const DataList& argList) = 0;
};
