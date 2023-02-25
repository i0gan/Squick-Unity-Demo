
#pragma once

#include <list>
#include <squick/plugin/net/i_response.h>
#include <squick/struct/struct.h>

class ResponseWorldList : public IResponse
{
public:
	class NFWorld
	{
	public:
		int id;
		std::string name;
		SquickStruct::EServerState state;
		int count;
	};

	std::list<NFWorld> world;
};

AJSON(ResponseWorldList::NFWorld, id, name, state, count)
AJSON(ResponseWorldList, world, code, message)