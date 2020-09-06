$executePaths = Get-Content -Path "C:\Users\Alexey\Desktop\TDD\ExecutePaths.txt"
$ProgramNames = Get-Content -Path "C:\Users\Alexey\Desktop\TDD\ProgramNames.txt"
$count = 0

foreach ($item in $executePaths)
{
    $source = "H:\AppPortable\AppLinks\" + $ProgramNames[$count]
    $exist = Test-Path $source

    if ($exist -ne $true)
    {
        try
        {
            New-Item -Path $source -ItemType symboliclink -Value $item -ErrorAction Stop
        }
        catch
        {
            try {
                $Install_Path = "h:\AppPortable\AppLinks"
                $WSShell = New-Object -com WScript.Shell
                $ShortcutPath = Join-Path -Path $Install_Path -ChildPath $ProgramNames[$count].Replace('.exe','.lnk') -ErrorAction
                $NewShortcut = $WSShell.CreateShortcut($ShortcutPath)
                $NewShortcut.TargetPath = $item
                $NewShortcut.Save()
            }
            catch
            {
                $ProgramNames[$count] | Out-Host
            }
        }
    }

    $count++
}