#pragma
// Author: i0gan
// Email : l418894113@gmail.com
// Date  : 2022-12-03
// Github: https://github.com/i0gan/Squick
// Description: Export base headers in a single header file

#include "i_plugin.h"
#include "i_module.h"
#include "i_plugin_manager.h"

#define dout std::cout << "SQUICK_DEV LOG:" << __FILE__ << ":" << __LINE__ << " "
#define dlog(x) "SQUICK_DEV LOG:" + std::string(__FILE__) + ":" + std::to_string(__LINE__) + "\n" + std::string(x) + "\n"