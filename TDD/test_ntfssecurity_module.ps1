$path = 'e:\temp\Music'

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
$read, $write, $modify, $full, $readAndExe
$read = $readData, $readAttributes, $readExtendedAttributes, $readPermissions
$write = $createFiles, $appendData, $writeAttributes, $writeExtendedAttributes
$modify = $executeFile, $readData, $readAttributes, $readExtendedAttributes, $readPermissions, $writeAttributes, $writeExtendedAttributes, $createFiles, $appendData, $delete
$readAndExe = $executeFile, $readData, $readAttributes, $readExtendedAttributes, $readPermissions
$fullControl =  $executeFile, $readData, $readAttributes, $readExtendedAttributes, $readPermissions, $writeAttributes, $writeExtendedAttributes, $createFiles, $appendData, $delete, $deleteSubdirectoriesAndFiles, $changePermissions, $takeOwnership

# Профили:
$admin = 'ALEX-COMP\Администратор'
$admins = 'BUILTIN\Администраторы'
$users = 'BUILTIN\Пользователи'
$user = 'ALEX-COMP\Alexey'
$sys = 'NT AUTHORITY\Система'
$userGroup = $users, $user, $admins

$exUsers = $users

# Чтобы предоставить права только на верхнем уровне и не изменять разрешения на вложенные объекты
# (только на папку), используйте команду: -AppliesTo ThisFolderOnly
Add-NTFSAccess $path -Account $user -AccessRights $read -PassThru

# Удалить назначенные NTFS разрешения:
Remove-NTFSAccess $path -Account $user -AccessRight $fullControl -PassThru

# Следующей командой можно лишить указанную учетную прав на все вложенные объекты в указанной папке
# (наследованные разрешения будут пропущены):
Get-ChildItem -Path $path -Recurse | Get-NTFSAccess -Account $exUsers -ExcludeInherited | Remove-NTFSAccess -PassThru

# Следующей командой можно назначить учетную запись Administrator владельцем всех
# вложенных объектов в каталоге:
Get-ChildItem -Path $path -Recurse -Force | Set-NTFSOwner -Account $user

# Чтобы очистить все разрешения, назначенные на объекты каталога вручную
# (не будет удалены унаследованные разрешения):
Get-ChildItem -Path $path -Recurse -Force | Clear-NTFSAccess

# Включить NTFS наследование для всех объектов в каталоге:
Get-ChildItem -Path $path -Recurse -Force | Enable-NTFSAccessInheritance

# Чтобы вывести все разрешения, которые назначены вручную, исключая унаследованные разрешения:
Get-ChildItem $path -recurse | Get-NTFSAccess –ExcludeInherited

# Можно вывести разрешения, назначенные для определенного аккаунта
# (не путайте с эффективными разрешениями, речь о них ниже):
Get-ChildItem $path -recurse | Get-NTFSAccess -Account $users