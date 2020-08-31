# Чтобы добавить права на объект нужно использовать такую конструкцию:
# $path = "e:\temp\Music"
# $user = "BUILTIN\Пользователи"
# $Rights = "Read, ReadAndExecute, ListDirectory"
# $INHERITSETTINGS = "Containerinherit, ObjectInherit"
# $PropogationSettings = "None"
# $RuleType = "Allow"
# $acl = Get-Acl $path
# $perm = $user, $Rights, $InheritSettings, $PropogationSettings, $RuleType
# $rule = New-Object -TypeName System.Security.AccessControl.FileSystemAccessRule -ArgumentList $perm
# $acl.SetAccessRule($rule)
# $acl | Set-Acl -Path $path

# # Чтобы убрать NTFS доступ к папке для пользователя или группы:
# $path = "e:\temp\Music"
# $acl = Get-Acl $path
# $rules = $acl.Access | Where-Object IsInherited -eq $false
# $targetrule = $rules | Where-Object IdentityReference -eq "BUILTIN\Пользователи"
# $acl.RemoveAccessRule($targetrule)
# $acl | Set-Acl -Path $path

# # Чтобы отключить наследование для папки из PowerShell:
# $path = 'E:\Temp\Pictures\Abstraction'
# $acl = Get-ACL -Path $path
# $acl.SetAccessRuleProtection($False, $False) # первый $True указывает, является ли данный каталог защищенным, второй $True – нужно ли скопировать текущие NTFS разрешения
# Set-Acl -Path $path -AclObject $acl

foreach ($item in Get-ChildItem -Path 'E:\Temp\Pictures\*\' -Recurse)
{
    $acl = Get-ACL -Path $item.FullName
    $acl.SetAccessRuleProtection($False, $False)
    Set-Acl -Path $path -AclObject $acl
}

Get-ChildItem -Path 'E:\Temp\Pictures\*' -Recurse | Add-NTFSAccess -Account 'NT AUTHORITY\Система' -AccessRight 'FullControl'
Get-ChildItem -Path 'E:\Temp\Pictures\*\' -Recurse -Force | Set-NTFSOwner -Account 'ALEX-COMP\Администратор'


