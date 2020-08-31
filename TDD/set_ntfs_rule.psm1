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
$admins = 'BUILTIN\Администраторы'
$users = 'BUILTIN\Пользователи'
$user = [System.Security.Principal.WindowsIdentity]::GetCurrent().Name
$system = 'NT AUTHORITY\Система'
$network = 'NT AUTHORITY\Сеть'
$profile1 = $users, $admins
$profile2 = $admins
$profile3 = $system, $network
$profile4 = $user, $system
$profile5 = $users
$profile6 = $user
$profGroup = @($profile1, $profile2, $profile6, $profile3, $profile4, $profile6, $profile5, $profile6)
$privGroup = @($read, $write, $modify, $fullControl, $fullControl, $fullControl, $readAndExe)

# Gives a random permissions for random users.
function SetAccess { [CmdletBinding()]
    param (
        [Parameter()]
        [string]$Path
    )
    $prof = Get-Random -InputObject $profGroup
    $priv = Get-Random -InputObject $privGroup
    Add-NTFSAccess -Path $Path -Account $prof -AccessRights $priv
}