set build_version="debug"
set project_path= %~dp0\..
set build_path=%project_path%\cache

mkdir %build_path%

start cmd /c "proto2code.bat"
start cmd /c "generate_config.bat"

rem cmake 
cd %build_path%\
echo %project_path%
cmake %project_path%\src -DMODE=dev

rem Project has generated at %build_path%
pause