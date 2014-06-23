@ECHO OFF
SET bindir="bin\Release"
SET objdir="COMPASS\obj"
SET devenv="C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\devenv.exe"
SET zip="7z.exe"
SET version="1.4.1"

rmdir /s /q %bindir%
rmdir /s /q %objdir%

mkdir %bindir%

@ECHO ON

%devenv% "COMPASS\COMPASS.sln" /rebuild "Release" /project "COMPASS"
%devenv% "COMPASS\COMPASS.sln" /rebuild "Release|x86" /project "COMPASS"
%devenv% "COMPASS\COMPASS.sln" /rebuild "Release|x64" /project "COMPASS"

%zip% a -tzip "%bindir%\COMPASS-%version% (32-bit).zip" ".\%bindir%\32-bit\*"
%zip% a -tzip "%bindir%\COMPASS-%version% (64-bit).zip" ".\%bindir%\64-bit\*"
%zip% a -tzip "%bindir%\COMPASS-%version%.zip" ".\%bindir%\Any Cpu\*"

PAUSE