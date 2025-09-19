@echo off
SETLOCAL

REM Ange Anaconda installation
SET "CONDA_PATH=C:\Users\ricka\anaconda3"
SET "ENV_NAME=judotech"
SET "PYTHON_VERSION=3"
SET "NODE_VERSION=24"

REM Initiera conda
CALL "%CONDA_PATH%\Scripts\activate.bat"

REM Kontrollera om miljön redan finns
conda info --envs | findstr /C:"%ENV_NAME%" >nul
IF %ERRORLEVEL% NEQ 0 (
    echo Skapar miljön %ENV_NAME%...
    conda create -y -n %ENV_NAME% python=%PYTHON_VERSION% nodejs=%NODE_VERSION%
) ELSE (
    echo Miljön %ENV_NAME% finns redan.
)

REM Aktivera miljön
CALL conda activate %ENV_NAME%

REM Starta VS Code i nuvarande katalog
code .

ENDLOCAL
