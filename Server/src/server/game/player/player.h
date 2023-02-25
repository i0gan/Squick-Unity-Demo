// Author: i0gan
// Email : l418894113@gmail.com
// Date  : 2023-01-05
// Description: 玩家进入游戏后，由玩家管理器创建出来的对象
#include <squick/core/guid.h>
#include <squick/struct/game.pb.h>
#include <squick/core/base.h>

#pragma once
namespace game::player {

class Player {
public:
    Player();
    virtual ~Player();
    void OnEnterGame();
    void OnOffline();
    void OnDestroy();
    void OnEnterScene();
    void OnExitScene();
    void OnChangeScene();

private:
    SquickStruct::RoleDataPack data;
    Guid guid;
};

}