cd /d %~dp0
echo OFF
net.exe session 1>NUL 2>NUL && (
    goto as_admin
) || (
    goto not_admin
)
:not_admin
echo ���Թ���Ա����������е�ǰ�ű���
goto end

:as_admin
SET basePath=%cd%
SET serviceName=Hua.DDNS
SET displayName="Hua.DDNS Demo"
SET description="Hua.DDNS Demo"
SET servicePath="%basePath%\%serviceName%.exe"
ECHO %servicePath%
net stop %serviceName%
sc delete %serviceName%
ping 1.1.1.1 -n 1 -w 10 > nul
nssm install %serviceName% %servicePath%
nssm set %serviceName% AppDirectory %basePath%
nssm set %serviceName% AppStopMethodSkip 6
nssm set %serviceName% AppStopMethodConsole 1000
nssm set %serviceName% AppThrottle 5000
ping 1.1.1.1 -n 1 -w 10 > nul
nssm start %serviceName%
Echo ��װ�ɹ�
goto end
:end
Pause