@echo off
netsh advfirewall firewall delete rule name="Sync-ODDR"
netsh advfirewall firewall add rule name="Sync-ODDR" protocol=TCP dir=in localport=10800,10801 action=allow
pause