----------------------------------------------------------------------------------
-- don't edit it, generated from .proto files by tools
----------------------------------------------------------------------------------

proto_code = [[
syntax = "proto3";

package SquickStruct; 

message Ident//The base protocol can not be transfer directly
{ 
    int64	   svrid = 1;
    int64      index = 2;
}

message Vector2//The base protocol can not be transfer directly
{ 
    float      x = 1;
    float      y = 2;
}

message Vector3//The base protocol can not be transfer directly
{ 
    float	   x = 1;
    float      y = 2;
    float      z = 3;
}

////////////////////////BaseCommon/////////////////////////////
message PropertyInt//The base protocol can not be transfer directly
{ 
    bytes     property_name = 1;
    int64     data = 2;
	int64     reason = 3;
}

message PropertyFloat//The base protocol can not be transfer directly
{ 
    bytes     property_name = 1;
    float     data = 2;
	int64     reason = 3;
}

message PropertyString//The base protocol can not be transfer directly
{ 
    bytes     property_name = 1;
    bytes     data = 2;
    int64     reason = 3;
}

message PropertyObject//The base protocol can not be transfer directly
{ 
	bytes     property_name = 1;
	Ident     data = 2;
	int64     reason = 3;
}

message PropertyVector2//The base protocol can not be transfer directly
{ 
	bytes     property_name = 1;
	Vector2   data = 2;
	int64     reason = 3;
}

message PropertyVector3//The base protocol can not be transfer directly
{ 
	bytes     property_name = 1;
	Vector3   data = 2;
	int64     reason = 3;
}

///////////////////////////////////////////////

message RecordInt//The base protocol can not be transfer directly
{
    int32      row = 1;
	int32      col = 2;
	int64      data = 3;
}


message RecordFloat//The base protocol can not be transfer directly
{
    int32      row = 1;
	int32      col = 2;
	float      data = 3;
}

message RecordString//The base protocol can not be transfer directly
{ 
    int32      row = 1;
	int32      col = 2;
	bytes     data = 3;
}

message RecordObject//The base protocol can not be transfer directly
{
    int32      row = 1;
	int32      col = 2;
	Ident      data = 3;
}

message RecordVector2//The base protocol can not be transfer directly
{
    int32      row = 1;
	int32      col = 2;
	Vector2      data = 3;
}

message RecordVector3//The base protocol can not be transfer directly
{
    int32      row = 1;
	int32      col = 2;
	Vector3      data = 3;
}

message RecordAddRowStruct//The base protocol can not be transfer directly
{
	int32 				row = 1;
	repeated RecordInt			record_int_list = 2;
	repeated RecordFloat		record_float_list = 3;
	repeated RecordString		record_string_list = 4;
	repeated RecordObject		record_object_list = 5;
	repeated RecordVector2      record_vector2_list = 6;
	repeated RecordVector3      record_vector3_list = 7;
}
message ObjectRecordBase//The base protocol can not be transfer directly
{ 
	bytes  record_name = 1;
	repeated RecordAddRowStruct row_struct = 2;
}

/////////////////////////////////////////////////

message ObjectPropertyInt//when data's driver want to transfer data to client independently
{
	Ident  player_id = 1;
    repeated PropertyInt	property_list = 2;
} 

message ObjectPropertyFloat//when data's driver want to transfer data to client independently
{
	Ident  player_id = 1;
    repeated PropertyFloat  	property_list = 2;
} 

message ObjectPropertyString//when data's driver want to transfer data to client independently
{
	Ident  player_id = 1;
    repeated PropertyString  	property_list = 2;
} 

message ObjectPropertyObject//when data's driver want to transfer data to client independently
{
	Ident  player_id = 1;
    repeated PropertyObject  	property_list = 2;
}

message ObjectPropertyVector2//when data's driver want to transfer data to client independently
{
	Ident  player_id = 1;
    repeated PropertyVector2  	property_list = 2;
}

message ObjectPropertyVector3//when data's driver want to transfer data to client independently
{
	Ident  player_id = 1;
    repeated PropertyVector3  	property_list = 2;
}

message ObjectRecordInt//when data's driver want to transfer data to client independently
{
	Ident  player_id = 1;
	bytes  record_name = 2;
    repeated RecordInt  	property_list = 3;
} 

message ObjectRecordFloat//when data's driver want to transfer data to client independently
{
	Ident  player_id = 1;
	bytes     	record_name = 2;
    repeated RecordFloat  	property_list = 3;
}

message ObjectRecordString//when data's driver want to transfer data to client independently
{
	Ident       player_id = 1;
	bytes     	record_name = 2;
    repeated RecordString  	property_list = 3;
}

message ObjectRecordObject//when data's driver want to transfer data to client independently
{
	Ident       player_id = 1;
	bytes     	record_name = 2;    
    repeated RecordObject  property_list = 3;
}

message ObjectRecordVector2//when data's driver want to transfer data to client independently
{
	Ident       player_id = 1;
	bytes     	record_name = 2;    
    repeated RecordVector2  property_list = 3;
}

message ObjectRecordVector3//when data's driver want to transfer data to client independently
{
	Ident       player_id = 1;
	bytes     	record_name = 2;    
    repeated RecordVector3  property_list = 3;
}

message ObjectRecordSwap//when data's driver want to transfer data to client independently
{
	Ident  	    player_id = 1;
	bytes    	origin_record_name = 2;
	bytes		target_record_name = 3;
	int32 		row_origin = 4;
	int32 		row_target = 5;
}

message ObjectRecordAddRow//when data's driver want to transfer data to client independently
{
	Ident       player_id = 1;
	bytes       record_name = 2;
	repeated RecordAddRowStruct    row_data = 3;
}

message ObjectRecordRemove//when data's driver want to transfer data to client independently
{
	Ident     	player_id = 1;
	bytes    	record_name = 2;  
	repeated int32 		remove_row = 3;
}

///////////////////////////////////////////////////////////////////

message ObjectPropertyList //send all properties as a pack when client log in or log off
{
	Ident  player_id = 1;
	repeated PropertyInt property_int_list = 2;
	repeated PropertyFloat property_float_list = 3;
	repeated PropertyString property_string_list = 4;
	repeated PropertyObject property_object_list = 5;
	repeated PropertyVector2 property_vector2_list = 6;
	repeated PropertyVector3 property_vector3_list = 7;
}
 
message MultiObjectPropertyList//send all client's properties as a pack when client log in or log off
{
	repeated ObjectPropertyList multi_player_property = 1;
}

message ObjectRecordList//send all records as a pack when client log in or log off
{
	Ident  player_id = 1;
	repeated ObjectRecordBase record_list = 2;
}

 message MultiObjectRecordList//send all client's records as a pack when client log in or log off
 {
	repeated ObjectRecordList multi_player_record = 1;
 }

///////////////////////////////////////////////////////////////////
message MsgBase
{
	Ident  player_id = 1; // only be used between proxy-server and game-server
	bytes  msg_data = 2;
	repeated Ident  player_Client_list = 3;
	Ident  hash_ident = 4;
}

message ReqAckLagTest
{	
	int32 index = 1;
}

message ReqCommand // Game command
{
	enum EGameCommandType
	{
		EGCT_MODIY_PROPERTY		= 0;//[property_name,value]
		EGCT_MODIY_ITEM			= 1;//[item_id,count]
		EGCT_CREATE_OBJECT		= 2;//[object_index,count]
		EGCT_ADD_ROLE_EXP		= 3;//
	}
	Ident control_id = 1;
	EGameCommandType command_id = 2;
	bytes command_str_value = 3;
	int64 command_value_int = 4;
	double command_value_float = 5;
	bytes command_value_str = 6;
	Ident command_value_object = 7;
	int32 row = 8;
	int32 col = 9;
}

//events
enum EGameEventCode
{
	SUCCESS									= 0;        //
	UNKOWN_ERROR							= 1;		//
	ACCOUNT_EXIST							= 2;        //
	ACCOUNTPWD_INVALID						= 3;        //
	ACCOUNT_USING							= 4;        //
	ACCOUNT_LOCKED							= 5;        //
	ACCOUNT_LOGIN_SUCCESS					= 6;        //
	VERIFY_KEY_SUCCESS						= 7;        //
	VERIFY_KEY_FAIL							= 8;        //
	SELECTSERVER_SUCCESS					= 9;        //
	SELECTSERVER_FAIL						= 10;       //
	
	CHARACTER_EXIST							= 110;       //
	SVRZONEID_INVALID						= 111;       //
	CHARACTER_NUMOUT						= 112;       //
	CHARACTER_INVALID						= 113;       //
	CHARACTER_NOTEXIST						= 114;       //
	CHARACTER_USING							= 115;       //
	CHARACTER_LOCKED						= 116;       //
	ZONE_OVERLOAD							= 117;       //
	NOT_ONLINE								= 118;       //

	INSUFFICIENT_DIAMOND					= 200;       //
	INSUFFICIENT_GOLD					    = 201;       //
	INSUFFICIENT_SP					        = 202;       //
}


// Servers RPC 
enum ServerMsgId {

	SERVER_MSG_ID_NONE						= 0;
	WORLD_TO_MASTER_REGISTERED				= 1;
	WORLD_TO_MASTER_UNREGISTERED			= 2;
	WORLD_TO_MASTER_REFRESH					= 3;

	LOGIN_TO_MASTER_REGISTERED				= 4;
	LOGIN_TO_MASTER_UNREGISTERED			= 5;
	LOGIN_TO_MASTER_REFRESH					= 6;

	PROXY_TO_WORLD_REGISTERED				= 7;
	PROXY_TO_WORLD_UNREGISTERED				= 8;
	PROXY_TO_WORLD_REFRESH					= 9;

	PROXY_TO_GAME_REGISTERED				= 10;
	PROXY_TO_GAME_UNREGISTERED				= 11;
	PROXY_TO_GAME_REFRESH					= 12;

	GAME_TO_WORLD_REGISTERED				= 13;
	GAME_TO_WORLD_UNREGISTERED				= 14;
	GAME_TO_WORLD_REFRESH					= 15;

	DB_TO_WORLD_REGISTERED					= 16;
	DB_TO_WORLD_UNREGISTERED				= 17;
	DB_TO_WORLD_REFRESH						= 18;

	PVP_MANAGER_TO_WORLD_REGISTERED         = 19; // 将PVP管理服务器注册到 World 服务器
	PVP_MANAGER_TO_WORLD_UNREGISTERED       = 20;
	PVP_MANAGER_TO_WORLD_REFRESH            = 21;

	PVP_MANAGER_TO_GAME_REGISTERED         = 22; // 将PVP管理服务器注册到 GAME 服务器, PVP Manager
	PVP_MANAGER_TO_GAME_UNREGISTERED       = 23;
	PVP_MANAGER_TO_GAME_REFRESH            = 24;


	// Pvp Manager API
	// PVP 管理服 接口
    REQ_PVP_INSTANCE_CREATE                 = 30; // 创建 PVP 对战服实例
	ACK_PVP_INSTANCE_CREATE                 = 31;
    REQ_PVP_INSTANCE_DESTROY                = 32;
	ACK_PVP_INSTANCE_DESTROY                = 33;
    REQ_PVP_INSTANCE_STATUS                 = 34;
	ACK_PVP_INSTANCE_STATUS                 = 35;
    REQ_PVP_INSTANCE_LIST                   = 36; // 
	ACK_PVP_INSTANCE_LIST                   = 37; 

	// Pvp Manager API
	// PVP 接口, 由PVP -> PVP Manager -> Game
	// 在PVP游戏中，以Game服务器为主，PVP服务器只负责当前对局网络同步和数据结算，不做其他逻辑功能
	REQ_PVP_STATUS                         = 50;
	ACK_PVP_STATUS                         = 51;
	REQ_PVP_GAME_INIT                      = 52; // 创建完毕PVP服务器后， PVP服务器向Game服务器初始化对战服数据和各玩家数据
	ACK_PVP_GAME_INIT                      = 53; 
	REQ_PLAYER_INFO                        = 54; // 加入玩家，
	ACK_PLAYER_INFO                        = 55; // 
	ACK_NEW_PLAYER                         = 56; //
	
	REQ_CONNECT_GAME_SERVER                = 60; // PVP请求连接Game Server
	ACK_CONNECT_GAME_SERVER                = 61; 
}

// Client RPC 
enum EGameMsgID
{
	UNKNOW									= 0;         //
	EVENT_RESULT							= 1;         // for events
	EVENT_TRANSPORT							= 2;         // for events
	CLOSE_SOCKET							= 3;         // want to close some one

	STS_NET_INFO							= 70;
	
	REQ_LAG_TEST							= 80; // LAG_TEST
	ACK_GATE_LAG_TEST						= 81; // 代理服务器响应
	ACK_GAME_LAG_TEST						= 82; // 游戏服务器响应
	
	STS_SERVER_REPORT						= 90;  // 服务端报告服务状态
	STS_HEART_BEAT							= 100; // 服务端之间心跳包
	
	//////////////////////////////////////////////////////////////////////////////////////
	REQ_LOGIN								= 101;     	//
	ACK_LOGIN								= 102;     	//
	REQ_LOGOUT								= 103;		//

	REQ_WORLD_LIST							= 110;			//
	ACK_WORLD_LIST							= 111;			//
	REQ_CONNECT_WORLD						= 112;			//
	ACK_CONNECT_WORLD						= 113;
	REQ_KICKED_FROM_WORLD					= 114;			//

	REQ_CONNECT_KEY							= 120;         // 先获取  Connect key 才能建立连接
	ACK_CONNECT_KEY							= 122;         // 

	REQ_SELECT_SERVER						= 130;			//
	ACK_SELECT_SERVER						= 131;			//
	REQ_ROLE_LIST							= 132;			//
	ACK_ROLE_LIST							= 133;			//
	REQ_CREATE_ROLE							= 134;			//
	REQ_DELETE_ROLE							= 135;			//
	REQ_RECOVER_ROLE						= 136;			//

	REQ_LOAD_ROLE_DATA						= 140;			// 加载玩家数据
	ACK_LOAD_ROLE_DATA						= 141;			//
	REQ_SAVE_ROLE_DATA						= 142;			// 请求保存玩家数据
	ACK_SAVE_ROLE_DATA						= 143;			// 

	REQ_ENTER_GAME							= 150;			// 进入游戏
	ACK_ENTER_GAME							= 151;			// 
	REQ_LEAVE_GAME							= 152;			// 离开游戏
	ACK_LEAVE_GAME							= 153;			// 
	REQ_ENTER_GAME_FINISH					= 154;			//
	ACK_ENTER_GAME_FINISH					= 155;			//

	REQ_ENTER_SCENE							= 160;          // 请求加入场景
	ACK_ENTER_SCENE							= 161;
	REQ_LEAVE_SCENE							= 162;			// 离开场景
	ACK_LEAVE_SCENE							= 163;			// 
	REQ_ENTER_SCENE_FINISH					= 164;			//
	ACK_ENTER_SCENE_FINISH					= 165;			//

	REQ_SWAP_SCENE							= 170;			// 切换场景
	ACK_SWAP_SCENE							= 171;			//  
	REQ_SWAP_HOME_SCENE						= 172;			// 
	ACK_SWAP_HOME_SCENE						= 173;			// 

/*
进入游戏后，服务端返回如下：
LogUnLua: 不存在该Msg, msg id: 200
LogUnLua: 不存在该Msg, msg id: 202
LogUnLua: 不存在该Msg, msg id: 203
LogUnLua: 不存在该Msg, msg id: 260
LogUnLua: 不存在该Msg, msg id: 217
LogUnLua: 不存在该Msg, msg id: 250
LogUnLua: 不存在该Msg, msg id: 171
LogUnLua: 不存在该Msg, msg id: 301
LogUnLua: 不存在该Msg, msg id: 202*/

	// 场景对象
	ACK_OBJECT_ENTRY						= 200;			// 对象在服务端上创建成功
	ACK_OBJECT_LEAVE						= 201;			// 

	ACK_OBJECT_PROPERTY_ENTRY				= 202;			// 对象属性
	ACK_OBJECT_RECORD_ENTRY					= 203;			// 对象记录值

	ACK_PROPERTY_INT						= 210;			//
	ACK_PROPERTY_FLOAT					    = 211;			//
	ACK_PROPERTY_STRING						= 212;			//
	//EGMI_ACK_PROPERTY_DOUBLE				= 213;			//
	ACK_PROPERTY_OBJECT						= 214;			//
	ACK_PROPERTY_VECTOR2        			= 215;
	ACK_PROPERTY_VECTOR3        			= 216;
	ACK_PROPERTY_CLEAR          			= 217;  // 属性清除

	ACK_ADD_ROW								= 220;
	ACK_REMOVE_ROW							= 221;
	ACK_SWAP_ROW							= 222;
	ACK_RECORD_INT							= 223;
	ACK_RECORD_FLOAT						= 224;
	//EGMI_ACK_RECORD_DOUBLE				= 225;
	ACK_RECORD_STRING						= 226;
	ACK_RECORD_OBJECT						= 227;
	ACK_RECORD_VECTOR2						= 228;
	ACK_RECORD_VECTOR3						= 229;

	ACK_RECORD_CLEAR						= 250; // 记录值清除
	ACK_RECORD_SORT							= 251;

	ACK_DATA_FINISHED						= 260; // 服务端发送对象数据完成

	REQ_MOVE								= 300;
	ACK_MOVE								= 301; // 移动

	REQ_CHAT								= 350;
	ACK_CHAT								= 351;

	REQ_SKILL_OBJECTX						= 400;
	ACK_SKILL_OBJECTX						= 401;
	REQ_SKILL_POS							= 402;
	ACK_SKILL_POS							= 403;

	ACK_ONLINE_NOTIFY						= 600;
	ACK_OFFLINE_NOTIFY						= 601;

	//game logic message id, start from 1000

	// 玩家房间逻辑
	REQ_ROOM_CREATE                         = 1000;
	ACK_ROOM_CREATE                         = 1001;

	REQ_ROOM_DETAILS                        = 1002;
	ACK_ROOM_DETAILS                        = 1003;

	REQ_ROOM_JOIN                           = 1004;
	ACK_ROOM_JOIN                           = 1005;
	ACK_ROOM_JOIN_NOTICE                    = 1006;
	REQ_ROOM_QUIT                           = 1007; // 离开房间
	ACK_ROOM_QUIT                           = 1008;
	ACK_ROOM_QUIT_NOTICE                    = 1009;
	REQ_ROOM_LIST                           = 1010; // 获取房间列表
	ACK_ROOM_LIST                           = 1011; // 

	REQ_ROOM_PLAYER_EVENT                   = 1020; // 在房间里互动以及事件，广播形式发送给房间内所有玩家
	ACK_ROOM_PLAYER_EVENT                   = 1021; 

	// 对战逻辑
	REQ_START_PVP_GAME                   = 1032; // 请求多人在线游戏，需要提前创建好房间
	ACK_START_PVP_GAME                   = 1033; 

	REQ_JOIN_PVP_GAME                    = 1034; // 请求加入对战中的游戏
	ACK_JOIN_PVP_GAME                    = 1035;

	REQ_QUIT_PVP_GAME                    = 1036; // 玩家请求退出当前对局，由 Player -> Proxy -> Game -> PvpManager -> Pvp
	ACK_QUIT_PVP_GAME                    = 1037;

	ACK_PVP_GAME_OVER                    = 1038; // 游戏结束

}

///////////////////////////////////////////////////////////////////////////////////////////////////

enum EItemType
{
	EIT_EQUIP   			= 0; //the equipment which can add props
	EIT_GEM   				= 1; //the gem ca be embed to the equipment
	EIT_SUPPLY   			= 2; //expendable items for player, such as a medicine that cures
	EIT_SCROLL   			= 3; //special items that can call a hero or others, special items can do what you want to do
}

enum ESkillType
{
	BRIEF_SINGLE_SKILL 			= 0;//this kind of skill just can damage one object
	BRIEF_GROUP_SKILL			= 1;//this kind of skill can damage multiple objects
	BULLET_SINGLE_SKILL			= 2;//this kind of bullet just can damage one object
	BULLET_REBOUND_SKILL		= 3;//this kind of bullet can damage multiple objects via rebound
	BULLET_TARGET_BOMB_SKILL	= 4;//this kind of bullet can damage multiple objects who around the target when the bullet touched the target object
	BULLET_POS_BOMB_SKILL		= 5;//this kind of bullet can damage multiple objects  who around the target when the bullet arrived the position
	FUNC_SKILL					= 6;
};

// 场景形式，玩家进入后，自己开一个独立场景
enum ESceneType
{
	NORMAL_SCENE 			= 0; //public town, only has one group available for players is 1
	SINGLE_CLONE_SCENE 		= 1; //private room, only has one player per group and it will be destroyed if the player leaved from group.
	MULTI_CLONE_SCENE 		= 2; //private room, only has more than one player per group and it will be destroyed if all players leaved from group.
}

enum ENPCType
{
    NORMAL_NPC	= 0;			  //
    HERO_NPC 	= 1;              //
    TURRET_NPC 	= 2;              //
    FUNC_NPC 	= 3;              //
};



enum EServerState
{
	EST_CRASH = 0;
	EST_NARMAL = 1;
	EST_BUSY = 2;
	EST_FIRE = 3;
	EST_MAINTEN = 4;
}

enum ELoginMode
{
	ELM_LOGIN = 0;
	ELM_REGISTER = 1;
	ELM_AUTO_REGISTER_LOGIN = 2; // login, or register if account dosen't exsit
}

message ServerInfoReport
{
	
	int32   server_id = 1;
	bytes  server_name = 2;
	bytes  server_ip = 3;
	int32   server_port = 4;
	int32   server_max_online = 5;
	int32   server_cur_count = 6;
	EServerState  server_state = 7;
	int32   server_type = 8;
}

message ServerInfoReportList
{
	repeated ServerInfoReport server_list = 1;
}

message AckEventResult
{
	EGameEventCode event_code = 1;
	Ident event_object = 2;//playerID
	Ident event_client = 3;//clientID
}

////////////////////////////////////////////////////
message ReqAccountLogin
{
	bytes 			account = 2;
	bytes 			password = 3;
	bytes 			security_code = 4;
    bytes 			signBuff = 5;
    int32 			clientVersion = 6;
    ELoginMode		loginMode = 7;
    int32 			clientIP = 8;
    int64 			clientMAC = 9;
	bytes 			device_info = 10;
	bytes 			extra_info = 11;
	int32 			platform_type = 12;
}

message ReqAccountLogout
{ 
	bytes 		account = 2;
	bytes 		extra_info = 3;
}

message ServerInfo
{
    int32  		server_id = 1;
    bytes 			name = 2;
	int32			wait_count = 3;
	EServerState	status = 4;
}

enum ReqServerListType
{
	RSLT_WORLD_SERVER = 0;
	RSLT_GAMES_ERVER = 1;
};

message ReqServerList
{
	ReqServerListType type = 1;
}

message AckServerList
{
	ReqServerListType type = 1;
	repeated ServerInfo info = 2;
}

message ReqConnectWorld
{
	int32   world_id = 1;
	bytes  account = 2;
	Ident  sender = 3;
	int32   login_id = 4;
}

message AckConnectWorldResult
{
	int32  world_id = 1;
	Ident  sender = 2;
	int32  login_id = 3;
	bytes  account = 4;
	bytes  world_ip = 5;
	int32  world_port = 6;
	bytes  world_key = 7;
}

message ReqSelectServer
{
	int32   world_id = 1;
}

message ReqKickFromWorld
{
	int32   world_id = 1;
	bytes   account = 2;
}

////////////////////////////////////////////
message ReqRoleList
{
	int32   game_id = 1;
	bytes   account = 2;	
}

message RoleLiteInfo
{
	Ident 		id = 1;
	int32 		career = 2;
    int32 		sex = 3;
    int32 		race = 4;
    bytes 		noob_name = 5;
    int32 		game_id = 6;
	int32 		role_level = 7;
	int32		delete_time = 8;
	int32		reg_time = 9;
	int32		last_offline_time = 10;
	int32		last_offline_ip = 11;
	bytes		view_record = 12;
}

message AckRoleLiteInfoList
{
	repeated RoleLiteInfo char_data = 1;
	bytes   account = 2;
}

message ReqCreateRole
{
	bytes 		account = 1;
	int32 		career = 2;
    int32 		sex = 3;
    int32 		race = 4;
    bytes 		noob_name = 5;
}

message ReqDeleteRole
{
	bytes 	account = 1;
    bytes	name = 2;
	int32 	game_id = 3;
}

message ReqRecoverRole
{
	bytes 	account = 1;
    bytes	name = 2;
	int32 	game_id = 3;
}

message ServerHeartBeat
{
	int32 count = 1;
}

message RoleOnlineNotify
{
	Ident self = 1;
	int32 game = 3;
	int32 proxy = 4;
	bytes name = 5;
	int32 bp = 6;
		
	repeated PropertyInt property_int_list = 20;
	repeated PropertyFloat property_float_list = 21;
	repeated PropertyString property_string_list = 22;
	repeated PropertyObject property_object_list = 23;
	repeated PropertyVector2 property_vector2_list = 24;
	repeated PropertyVector3 property_vector3_list = 25;
}

message RoleOfflineNotify
{
	Ident self = 1;
	Ident clan = 2;
	int32 game = 3;
	int32 proxy = 4;
}

message RoleDataPack
{
	Ident 		id = 1;
	ObjectPropertyList 	property = 2;
	ObjectRecordList 	record = 3;
}// 各服务器之间的通信



// 服务器类型
// c++代码对应在 squick/plugin/net/i_net_module.h :  SQUICK_SERVER_TYPES
enum ServerType
{
    SQUICK_ST_NONE          = 0;    // NONE
    SQUICK_ST_REDIS         = 1;    // 
    SQUICK_ST_MYSQL         = 2;    //
    SQUICK_ST_MASTER        = 3;    // 不支持横向拓展
    SQUICK_ST_LOGIN         = 4;    //
    SQUICK_ST_PROXY         = 5;    // 支持横向拓展
    SQUICK_ST_GAME          = 6;    // 支持横向拓展
	SQUICK_ST_WORLD			= 7;    // 支持横向拓展
	SQUICK_ST_DB			= 8;    // 暂不支持横向拓展
	SQUICK_ST_MAX			= 9;    //
	SQUICK_ST_GATEWAY		= 10;   // 支持横向拓展
	SQUICK_ST_PVP_MANAGER	= 11;   // PVP实例管理服务器
    SQUICK_ST_MICRO         = 12;   // 微服务，支持横向拓展
};

message RegisterServer {
    int32       server_id       = 1;
    ServerType  server_type     = 2;
    string      public_addr     = 3;
    int32       server_load     = 4;
}

message RegisterServerReply {
    int32 error           = 1;
}

message OnServerDisconnected {
    int32 server_id   = 1;
    int32 server_type = 2;
}

message PingRequest {
    int32       reserved        = 1;
    int32       server_load     = 2;
}

message PingResponse {
    int32       reserved        = 1;
    int32       server_load     = 2;
}

// 新加入服务器通知
message NewServerNotice {
    repeated RegisterServer servers = 1;
};

// 请求服务器列表
message ServerListRequest {
    ServerType server_type      = 1;
}

// 服务器列表返回
message ServerListResponse {
    repeated RegisterServer servers = 1;
}


// PVP服务器在Pvp Manager服务器上 请求连接 Game Server
// ACK_CONNECT_GAME_SERVER                = 61;
message ReqConnectGameServer {
    bytes instance_id = 1;
    bytes instance_key = 2;
    bytes name = 3;
    bytes security_code = 4;
    int32 platform_type = 5;
    int32 game_id = 6;
}

// 
message AckConnectGameServer {
    int32 code = 1;
}

// 在Game服务器上请求 Pvp Manager服务器创建PVP服务器实例
message ReqPvpInstanceCreate { 
    bytes instance_id = 1;
    bytes instance_key = 2;
    int32 game_id = 3;
}

// 
message AckPvpInstanceCreate {
    int32 code = 1;
}

message ReqPvpGameInit {
    bytes instance_id = 1;  // instance id
    bytes instance_key = 2; 
    int32 room_id = 3;
}



//除去基础对象身上的属性外，这里全部游戏中的逻辑协议

// 进入游戏
message ReqEnterGameServer
{
	Ident 		id = 1;      // 角色ID
	bytes 		account = 2; // 账号
    int32 		game_id = 3; // 游戏服务器ID
	bytes 		name = 4;    // 
}

message ReqAckEnterGameSuccess
{
    int32 		arg = 1;
}

message ReqHeartBeat
{
    int32 		arg = 1;
}

message ReqLeaveGameServer
{
    int32 		arg = 1;
}

message PlayerEntryInfo //对象出现基本信息
{
	Ident	object_guid = 1;
	float	x = 2;
	float	y = 3;
	float	z = 4;
	int32  career_type = 5;
	int32  player_state = 6;
	bytes  config_id = 7;
	int32  scene_id = 8;
	bytes  class_id = 9;
}

message AckPlayerEntryList //对象出现列表
{
	repeated PlayerEntryInfo object_list = 1;
}

message AckPlayerLeaveList //对象离去列表
{
	repeated Ident 	object_list = 1;
}

////////////////////////////////////////////////////////////////////////////////////////////////////

message PosSyncUnit
{	
	enum EMoveType
	{
		EMT_WALK = 0;
		EET_SPEEDY = 1;
		EET_TELEPORT = 2;
	}

	Ident 	mover = 1;
    Vector3 pos = 2;
    Vector3 orientation = 3;
    int32 status = 4;
	EMoveType type = 5;
}

message ReqAckPlayerPosSync
{
    int32 sequence = 1;
	repeated PosSyncUnit sync_unit = 2;
}

////////////////////////////

message EffectData
{
	enum EResultType
	{
		EET_FAIL = 0;
		EET_SUCCESS = 1;
		EET_REFUSE = 2;
		EET_MISS = 3;	
		EET_CRIT = 4;	
		EET_ULTI = 5;	
	}
	Ident 		effect_ident = 1;
	int32  		effect_value = 2;
	EResultType  effect_rlt = 3;
}

message ReqAckUseSkill
{
	Ident 	user = 1;
	bytes skill_id = 2;
	int32  client_index = 3;//因为客户端要先展示
	int64  server_index = 4;//因为客户端要先展示
	repeated EffectData 	effect_data = 5;
}

message ReqAckSwapScene
{
	int32 	transfer_type = 1;
	int32	scene_id = 2;
	int32	line_id = 3;
	float 	x = 4;
	float 	y = 5;
	float 	z = 6;
	bytes 	data = 7;
}

/////////////////////////////////////////
message ReqAckPlayerChat
{
	enum EGameChatChannel
	{
		EGCC_GLOBAL = 0;
		EGCC_CLAN = 1;
		EGCC_ROOM = 2;
		EGCC_TEAM = 3;
	}
	enum EGameChatType
	{
		EGCT_TEXT= 0;
		EGCT_VOICE = 1;
		EGCT_EMOJI = 2;
	}

	Ident	player_id = 1;
	bytes 	player_name = 2;
	EGameChatChannel  chat_channel = 3;
	EGameChatType  chat_type = 4;
	bytes chat_info = 5;
}

// 客户端请求创建房间
message ReqRoomCreate
{
	bytes name = 1;
}

message AckRoomCreate
{
	int32 room_id = 1;
}

// 请求加入房间
message ReqRoomJoin
{
	int32 room_id = 1;
}

message AckRoomJoin
{
	int32 code = 1;
}

enum RoomStatus {
	ROOM_PLAYERS_PREPARE = 0; // 玩家准备中
	ROOM_PLAYERS_GAME = 1;    // 已经开始游戏
}

message RoomSimple {
	int32 id = 1;          // 房间id
	bytes name = 2;        // 房间名称
	bytes game_mode = 3;   // 游戏模式
	RoomStatus status = 4; // 房间状态
	int32 nplayers = 5;     // 当前房间内玩家人数
	int32 max_players = 6; // 最多人数
}

message RoomDetails {
	int32 id = 1;          // 房间id
	bytes name = 2;        // 房间名称
	bytes game_mode = 3;   // 游戏模式
	RoomStatus status = 4; // 房间状态
	int32 nplayers = 5;    // 当前房间内玩家人数
	int32 max_players = 6; // 最多人数
	Ident owner = 7;       // 房主
	repeated Ident players = 8; // 房间内所有玩家
}

// 玩家获取房间列表
message ReqRoomList {
	int32 start = 1;
	int32 limit = 2;
}

message AckRoomList {
	repeated RoomSimple list = 1;
}

// 请求获取房间详细信息
message ReqRoomDetails {
	int32 room_id = 1;
}

message AckRoomDetails {
	RoomDetails room = 1;
}

// 请求开始游戏
message ReqStartGame {
	int32 room_id = 1;
}

message AckStartGame {
	int32 code = 1;
}

// 客户端加入房间后，通过房间id获取PVP服务器信息
message ReqPlayerJoin {
	int32 room_id = 1;
}

// PVP 服务器响应结构
message PvpServer {
	int32 instance_id = 1;
	bytes name = 2;
	bytes ip = 3;
	int32 port = 4;
	bytes key = 5;
}

// 如果是已经在房间里的人，开始游戏创建PVP服务器后，会自动通知房间里的其他玩家。
message AckPlayerJoin {
	int32 code = 1;
	PvpServer server = 4;
}
]]