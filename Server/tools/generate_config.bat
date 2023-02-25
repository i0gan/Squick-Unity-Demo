rem Author: i0gan
rem Email : l418894113@gmail.com
rem Date  : 2022-11-27
rem Github: https://github.com/i0gan/Squick
rem Description: Generate configuration files

set config_path=..\config
set config_path_gen=../config
set excel_path=..\resource\excel
set excel_path_gen=../resource/excel
set struct_path=..\src\squick\struct
set lua_proto_path=
set client_config_path=..\client


rem 生成配置文件
start .\proto2code.bat

mkdir %config_path%\proto
mkdir %config_path%\struct
mkdir %config_path%\ini

.\xlsx2need %excel_path_gen% %config_path_gen%

rem 拷贝 \proto\protocol_define.h 
copy ..\config\proto\protocol_define.h %struct_path%


mkdir %client_config_path%\ini
mkdir %client_config_path%\proto
mkdir %client_config_path%\struct
mkdir %client_config_path%\lua
mkdir %client_config_path%\csharp

copy %config_path%\proto\ProtocolDefine.cs %client_config_path%\csharp
xcopy /s /e /y %config_path%\ini %client_config_path%\ini
xcopy /s /e /y %config_path%\struct %client_config_path%\struct


rem rd /s/q %config_path%\proto

rem 生成Lua文件
python proto_enum_to_lua.py
python proto_to_lua_str.py
copy ..\src\lua\proto\enum.lua %client_config_path%\lua
