function Set-Owner ($user, $Path) {
    if (!(Test-Path -LiteralPath $Path)) {Write-Warning "Указан неверный путь к папке"}
    else {
        # преобразовываем путь вида C:\Folder в C:\\Folder (к слешу пути добавляем ещё один
        # для корректной работы класса Win32_LogicalFileSecuritySetting и эскейпим другие символы
        $path = $path -replace "\\|'",'\$0'
        $Path = $Path -replace '\[', "$([char]91)"
        $Path = $Path -replace '\]', "$([char]93)"
        # т.к. DACL мы не записываем, то объявляем только классы SecurityDescriptor и Trustee
        $SD = ([WMIClass] "Win32_SecurityDescriptor").CreateInstance()
        $Trustee = ([WMIClass] "Win32_Trustee").CreateInstance()
        # преобразовываем имя пользователя в SID и заполняем необходимые поля в Trustee
        $SID = (new-object System.Security.Principal.NTAccount $user).translate([System.Security.Principal.NTAccount])
        [byte[]] $SIDArray = ,0 * $SID.BinaryLength
        $SID.GetBinaryForm($SIDArray,0)
        $Trustee.Name = $user
        $Trustee.SID = $SIDArray
        $SD.Owner = $Trustee
        # здесь мы добавляем флаг управления
        $SD.ControlFlags="0x8000"
        # выбираем сведения о безопасности необходимой папки
        $wPrivilege = Get-WmiObject Win32_LogicalFileSecuritySetting -filter "path='$path'"
        # включаем привилегия для WMI. Для Windows Vista/Windows Server 2008,
        # при запуске скрипта с повышенными привилегиями данная строка не обязательна
        $wPrivilege.psbase.Scope.Options.EnablePrivileges = $true
        # записываем SecurityDescriptor с новым владельцем в папку
        $Return = $wPrivilege.setsecuritydescriptor($SD)
        # преобразовываем возвращаемый код в текстовое значение
        switch ($Return.ReturnValue) {
            "0" {"Успешно"}
            "2" {Write-Warning "Отказано в доступе"}
            "8" {Write-Warning "Неизвестная ошибка"}
            "9" {Write-Warning "Отсутствуют привилегии"}
            "21" {Write-Warning "Указан неправильный параметр"}
            "1307" {Write-Warning "Указанный пользователь не может быть владельцем данного объекта"}
            default {Write-Warning "Произошла неизвестная ошибка с кодом:" $Return.Value}
        }
    }
}
# Set-Owner -user 'ALEX-COMP\Alexey' -Path 'E:\Temp\TxGameDownload'
Set-Owner -user 'BUILTIN\Администраторы' -Path 'E:\Temp\Pictures'
Set-Owner -user 'ALEX-COMP\Администратор' -Path 'E:\Temp\Films'
Set-Owner -user 'ALEX-COMP\Администратор' -Path 'E:\Temp\Music'
Set-Owner -user 'ALEX-COMP\Администратор' -Path 'E:\Temp\Music'
Set-Owner -user 'NT AUTHORITY\СИСТЕМА' -Path 'E:\Temp\Films\Film noir\film2.avi'
Set-Owner -user 'СЕТЬ' -Path 'E:\Temp\Music'
Set-Owner -user 'BUILTIN\Пользователи' -Path 'E:\Temp\Music'
Set-Owner -user 'ALEX-COMP\Alexey' -Path 'E:\Temp\Music'

# Set-Owner -user 'ALEX-COMP\Адмнинстраторы' -Path 'E:\Temp\Pictures'

# эта часть совсем необязательна, я её включилc лишь для наглядности
# и полноты скрипта
function Get-Owner ($path) {(Get-Acl $path).owner}