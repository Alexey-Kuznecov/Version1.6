# Особые права:
$executeFile = 'ExecuteFile'
$readData = 'ReadData'
$readAttributes = 'ReadAttributes'
$readExtendedAttributes = 'ReadExtendedAttributes'
$createFiles = 'CreateFiles'
$appendData = 'AppendData'
$writeAttributes = 'WriteAttributes'
$writeExtendedAttributes = 'WriteExtendedAttributes'
$deleteSubdirectoriesAndFiles = 'DeleteSubdirectoriesAndFiles'
$delete = 'Delete'
$readPermissions = 'ReadPermissions'
$changePermissions = 'ChangePermissions'
$takeOwnership = 'TakeOwnership'

# Группы прав:
$read, $write, $modify, $fullControl, $readAndExe
$read = $readData, $readAttributes, $readExtendedAttributes, $readPermissions
$write = $createFiles, $appendData, $writeAttributes, $writeExtendedAttributes
$modify = $executeFile, $readData, $readAttributes, $readExtendedAttributes, $readPermissions, $writeAttributes, $writeExtendedAttributes, $createFiles, $appendData, $delete
$readAndExe = $executeFile, $readData, $readAttributes, $readExtendedAttributes, $readPermissions
$fullControl =  $executeFile, $readData, $readAttributes, $readExtendedAttributes, $readPermissions, $writeAttributes, $writeExtendedAttributes, $createFiles, $appendData, $delete, $deleteSubdirectoriesAndFiles, $changePermissions, $takeOwnership

# Профили:
$admin = 'ALEX-COMP\Администратор'
$admins = 'BUILTIN\Администраторы'
$users = 'BUILTIN\Пользователи'
$user = 'ALEX-COMP\Пользователь'
$curUser = 'ALEX-COMP\Alexey'
$system = 'NT AUTHORITY\Система'
$network = 'NT AUTHORITY\Сеть'
$guest = 'NT AUTHORITY\Сеть'
$profile1 = $users, $admins,
$profile2 = $admin, $user
$profile3 = $system, $network
$profile4 = $guest, $user, $admin, $user
$profile5 = $system
$profile6 = $admin, $curUser

$profGroup = @($profile1, $profile2, $profile3, $profile4, $profile5, $profile6)
$privGroup = @($read, $write, $modify, $fullControl, $readAndExe)

function Set-For-System-Access { [CmdletBinding()]
    param (
        [Parameter()]
        [string]
        $Path
    )
    $prof = Get-Random -InputObject $profGroup
    $priv = Get-Random -InputObject $privGroup
    Add-NTFSAccess -Path $Path -Account $prof -AccessRights $priv
}
