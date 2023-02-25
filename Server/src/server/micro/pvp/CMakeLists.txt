set(ProjectName "plugin_server_micro_pvp")
file(GLOB plugin_server_micro_pvp_cc
	*.cc)
	
file(GLOB plugin_server_micro_pvp_h 
	*.h)

add_library(plugin_server_micro_chat SHARED
	${plugin_server_micro_pvp_cc}
	${plugin_server_micro_pvp_h})
	
set_target_properties( plugin_server_micro_pvp PROPERTIES OUTPUT_NAME "pvp")
set_target_properties( plugin_server_micro_pvp PROPERTIES PREFIX "")
set_target_properties( plugin_server_micro_pvp PROPERTIES
	ARCHIVE_OUTPUT_DIRECTORY ${SQUICK_BUILD_PLUGIN_SERVER_PATH}/micro
	RUNTIME_OUTPUT_DIRECTORY ${SQUICK_BUILD_PLUGIN_SERVER_PATH}/micro
	LIBRARY_OUTPUT_DIRECTORY ${SQUICK_BUILD_PLUGIN_SERVER_PATH}/micro )

add_definitions(-D_X64 -D_CONSOLE)
