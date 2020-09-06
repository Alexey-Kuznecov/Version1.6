# Gets hash summ of the file
Get-FileHash "H:\NodeJSPortable\NodeJSPortable.exe" -Algorithm MD5 | Format-List
# Makes symbolic link.
New-Item -Path "Target" -ItemType symboliclink -Value "Source"
# Install chacolatey
Set-ExecutionPolicy Bypass -Scope Process -Force; [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072; iex ((New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))

