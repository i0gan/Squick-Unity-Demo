set(ProjectName "plugin_server_micro_email")
file(GLOB plugin_server_micro_email_cc
	*.cc)
	
file(GLOB plugin_server_micro_email_h 
	*.h)

add_library(plugin_server_micro_email SHARED
	${plugin_server_micro_email_cc}
	${plugin_server_micro_email_h})
	
set_target_properties( plugin_server_micro_email PROPERTIES OUTPUT_NAME "email")
set_target_properties( plugin_server_micro_email PROPERTIES PREFIX "")
set_target_properties( plugin_server_micro_email PROPERTIES
	ARCHIVE_OUTPUT_DIRECTORY ${SQUICK_BUILD_PLUGIN_SERVER_PATH}/micro
	RUNTIME_OUTPUT_DIRECTORY ${SQUICK_BUILD_PLUGIN_SERVER_PATH}/micro
	LIBRARY_OUTPUT_DIRECTORY ${SQUICK_BUILD_PLUGIN_SERVER_PATH}/micro )

add_definitions(-D_X64 -D_CONSOLE)
