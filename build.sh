#!/usr/bin/env sh


echo =======================================

echo = ����ϲ����� github.com/jsix/devfw =

echo =======================================


set megdir=%dir%\dll\

if exist "%megdir%merge.exe" (

  echo ������,���Ե�...
  cd %dir%dist\dll\

echo  /keyfile:%dir%j6.devfw.snk>nul

"%megdir%merge.exe" /closed /keyfile:%dir%/src/core/J6.DevFw.Core/j6.devfw.snk /ndebug /targetplatform:v4 /target:dll /out:%dir%dist\jrdev.dll^
 JR.DevFw.Core.dll JR.DevFw.PluginKernel.dll JR.DevFw.Data.dll JR.DevFw.Template.dll JR.DevFw.Web.dll JR.DevFw.Toolkit.Data.dll
  


  echo ���!�����:%dir%dist\jrdev.dll

)


pause
