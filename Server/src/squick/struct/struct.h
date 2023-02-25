#pragma once

#include <squick/core/platform.h>
#include <squick/core/i_record_manager.h>
#include <squick/core/i_property_manager.h>
#include <squick/core/list.h>
#include "limit.h"
#include "define.pb.h"
#include "base.pb.h"
#include "game.pb.h"
#include "share.pb.h"
#include "server.pb.h"
#include "protocol_define.h"

// -------------------------------------------------------------------------
#pragma pack(push,1)

enum E_CHECK_TYPE
{
    ECT_SAVE        = 0, 
    ECT_PRIVATE     = 1, 
    ECT_PUBLIC      = 2, 
};
#pragma pack(pop)
