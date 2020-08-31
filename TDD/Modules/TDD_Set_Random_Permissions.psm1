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
function SetRandomPerm { [CmdletBinding()]
    param (
        [Parameter()]
        [string]$Path
    )
    $prof = Get-Random -InputObject $profGroup
    $priv = Get-Random -InputObject $privGroup
    Add-NTFSAccess -Path $Path -Account $prof -AccessRights $priv
}


function SetRandomPermAll { [CmdletBinding()]
    param (
        [Parameter()]
        [string]$Path
    )

    foreach ($item in Get-ChildItem -Path $Path -File -Recurse) {
        $prof = Get-Random -InputObject $profGroup
        $priv = Get-Random -InputObject $privGroup
        Add-NTFSAccess -Path $item.FullName -Account $prof -AccessRights $priv
    }
}

function RemoveInheritance {
    param (
        [CmdletBinding()]
        [Parameter(ValueFromPipeLine = $true)][string]$PipeLine,
        [Parameter()]
        [string]$Path
    )

    if ($Path -eq "") {
        $Path = $PipeLine
    }

    foreach ($item in Get-ChildItem -Path $Path -File -Recurse) {
         # To prevent inheritance of access rights.
        $acl = Get-ACL -Path $item.FullName
        $acl.SetAccessRuleProtection($true, $true)
        Set-Acl -Path $item.FullName -AclObject $acl
        # # To delete permissions for all users.
    }
}

function RemoveAll {
    param (
        [CmdletBinding()]
        [string]$Path,
        [string]$User,
        [string]$Perm
    )

    foreach ($item in Get-ChildItem -Path $Path -File -Recurse) {
        $item | Remove-NTFSAccess -Account $User -AccessRights $Perm
    }
}

# SIG # Begin signature block
# MIIFkQYJKoZIhvcNAQcCoIIFgjCCBX4CAQExCzAJBgUrDgMCGgUAMGkGCisGAQQB
# gjcCAQSgWzBZMDQGCisGAQQBgjcCAR4wJgIDAQAABBAfzDtgWUsITrck0sYpfvNR
# AgEAAgEAAgEAAgEAAgEAMCEwCQYFKw4DAhoFAAQUcwGBs0oDcKRQpdaUKml5Ir40
# gw2gggMqMIIDJjCCAg6gAwIBAgIQXI2Y/4WK6KlBlFsjmPpZszANBgkqhkiG9w0B
# AQsFADAcMRowGAYDVQQDDBFsZXhhMXRoQGdtYWlsLmNvbTAeFw0yMDA4MjUwODE1
# MzhaFw0yMTA4MjUwODM1MzhaMBwxGjAYBgNVBAMMEWxleGExdGhAZ21haWwuY29t
# MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAudlTYCSLWSHR/tUtF7el
# n8O7UyS4+tIYXGNXra/uzYB6k3mgiV91Sl/ukE9iOU6/qVpj2leyjGkygNH5wWa1
# OuY/4HTfkTKt+tWr+vl+ZMViIMfM1TdYHNAgeU4fvPrMukOG8TEUAVt6fRTn8Lx2
# 9SzpUgP3ttInfASqJVJexZMicKBBxCnbqdenYVqkgXspUM0+qUJDgq9i5EPDJDCh
# zZj1+IJRlLeqmWkk5Obddj/FfYsJIAhPLl+wUbQ5BoDmDUlI9jvUfX2urbuJ+uzn
# NrtdqeuW8B0t+6s9eFslGtZTbbxO/+bdwDOMvVZnS3vIwZbf/LJvw1PNXmS7ZxRB
# dQIDAQABo2QwYjAOBgNVHQ8BAf8EBAMCB4AwEwYDVR0lBAwwCgYIKwYBBQUHAwMw
# HAYDVR0RBBUwE4IRbGV4YTF0aEBnbWFpbC5jb20wHQYDVR0OBBYEFIKYlgcGRtlb
# NohSqiwQan0A6J3mMA0GCSqGSIb3DQEBCwUAA4IBAQCki89phTOt/NGdqQDXqBOv
# ieK6Pvwtb1WlaZ2pldh740UQKIAaXsrGR4FMyWBGuuy8vUv6zQGgWsMkXglYz1W1
# qBgAAm6usuRi7TnBpLq87g291moM4USNyZBwxq3TP1tZTpN3CtGTSzL1Oj6vSm7W
# 24FrqiUbMcojnrIjbjen+V21s4U95czpL6HCWIpRcM2otW8J9nYD1B7xFcb82Meq
# uefdXBMSedSa399mj71YeP2hLVolgDzzK/0qW4V1ZvIWy5SAa4yN/Nv0m6+86a+d
# laOaPWeLfs6GjBn0hm7ip8ppS4o6wYi1Bq/2tLfhQi2dytsk3MJw8QiskEwZ6uF8
# MYIB0TCCAc0CAQEwMDAcMRowGAYDVQQDDBFsZXhhMXRoQGdtYWlsLmNvbQIQXI2Y
# /4WK6KlBlFsjmPpZszAJBgUrDgMCGgUAoHgwGAYKKwYBBAGCNwIBDDEKMAigAoAA
# oQKAADAZBgkqhkiG9w0BCQMxDAYKKwYBBAGCNwIBBDAcBgorBgEEAYI3AgELMQ4w
# DAYKKwYBBAGCNwIBFTAjBgkqhkiG9w0BCQQxFgQU636IiT8K9eAudZDHkd5ilcQQ
# mSIwDQYJKoZIhvcNAQEBBQAEggEAM94QRdDdXQv6PE+U2kOpLh8X6q3FtFYeE0m/
# RQeAYud/k/eH9C5j28CDga/mSIXtpA+N6LZGihneFtEpaSA85fGrciO+992HpHiV
# qcpvdqzGmzO0+lVnV3peT1OByp4c4SAFPPrGdgDljXp7xCcjITXD78wX7MXwNwkf
# TcwODpoHeWxoFhaa+NpHjmgm31ONKUkDss8AbkfODX/vwZya7gu3YuobmGx2K4Pl
# M/REgln4/A2oiDl9Z2wcdx52aNxJAHuSLLbpg/moeTUGqk7c6a20sP1q07QwPGWP
# TUsoq8jW92adU1ZX60PAlP26ljrCmaoaJjMujvdJ45eLPNsBzQ==
# SIG # End signature block
