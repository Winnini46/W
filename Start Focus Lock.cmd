@echo off
start "" powershell.exe -NoProfile -STA -WindowStyle Hidden -ExecutionPolicy RemoteSigned -File "%~dp0FocusLock.ps1"
