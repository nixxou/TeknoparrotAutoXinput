@echo off
set sourceFolder=.\config
set destinationFolder=.\TeknoparrotAutoXinput\bin\x64\Debug\net6.0-windows\config

echo Copie du répertoire config en cours...

rem Créer le répertoire de destination s'il n'existe pas
if not exist "%destinationFolder%" mkdir "%destinationFolder%"

rem Copier le contenu du répertoire source vers le répertoire destination
xcopy /e /i /y "%sourceFolder%" "%destinationFolder%"

echo Copie terminée.

set sourceFolder=.\img
set destinationFolder=.\TeknoparrotAutoXinput\bin\x64\Debug\net6.0-windows\img

echo Copie du répertoire config en cours...

rem Créer le répertoire de destination s'il n'existe pas
if not exist "%destinationFolder%" mkdir "%destinationFolder%"

rem Copier le contenu du répertoire source vers le répertoire destination
xcopy /e /i /y "%sourceFolder%" "%destinationFolder%"

echo Copie terminée.

set sourceFolder=.\TeknoparrotAutoXinput\lib
set destinationFolder=.\TeknoparrotAutoXinput\bin\x64\Debug\net6.0-windows

echo Copie du répertoire config en cours...

rem Créer le répertoire de destination s'il n'existe pas
if not exist "%destinationFolder%" mkdir "%destinationFolder%"

rem Copier le contenu du répertoire source vers le répertoire destination
xcopy /e /i /y "%sourceFolder%" "%destinationFolder%"

echo Copie terminée.
pause