function CreateDirFaked {
    param (
        [alias("d")]
        [string]$Destination,
        [alias("ln")]
        [array]$FolderLsName,
        [alias("lf")]
        [array]$FileLsFormat,
        [alias("perm")]
        [bool]$IncludePerm
    )
    $store = $Destination
    foreach ($cat in $FolderLsName)
    {
        $Destination += RandomPath
        $ex = Test-Path -Path $Destination

        if ($ex -eq $false) {
            New-Item -Path $Destination -ItemType Directory
        }

        $random_count = Get-Random -max 5 -min 1

        for ($i = 0; $i -lt $random_count; $i++)
        {
            $format = Get-Random -InputObject $FileLsFormat
            $random = Get-Random -max 10Mb -min 3Mb
            $path = "$Destination\img$i.$format"
            $file = New-Object -TypeName System.IO.FileStream -ArgumentList $path,Create,ReadWrite
            $file.SetLength($random)
            $file.Close()

            if ($IncludePerm) {
                SetRandomPerm -Path $path
            }
        }

        $Destination = $store
    }
}

function RandomPath {

    $random = Get-Random -max 3 -min 0
    $ranPath = ""

    for ($i = 0; $i -lt $random; $i++) {
        $fname = Get-Random -InputObject $FolderLsName
        $ranPath += $fname + "\"
    }
    return "\" + $ranPath
}

# SIG # Begin signature block
# MIIFkQYJKoZIhvcNAQcCoIIFgjCCBX4CAQExCzAJBgUrDgMCGgUAMGkGCisGAQQB
# gjcCAQSgWzBZMDQGCisGAQQBgjcCAR4wJgIDAQAABBAfzDtgWUsITrck0sYpfvNR
# AgEAAgEAAgEAAgEAAgEAMCEwCQYFKw4DAhoFAAQUT5zsuV4lhxpbeeUZi8yHs6p3
# GCSgggMqMIIDJjCCAg6gAwIBAgIQXI2Y/4WK6KlBlFsjmPpZszANBgkqhkiG9w0B
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
# DAYKKwYBBAGCNwIBFTAjBgkqhkiG9w0BCQQxFgQU9PWLgxNGitqaruISuBxj0ItK
# mXIwDQYJKoZIhvcNAQEBBQAEggEARXSPFsN8q7vzFYAatGpX2T4voJMgLlAfvj6B
# uXF08Sfbp+U+8CT3Y2CDYBeKkslHMmCOVUdl2gkSzy5M4Z2WJeLf3UyXmErwhJl5
# +2LVuLgkzmyJcl6zkk5VeEqG6y3ypiUZlwMzrHilC5m48OQnWlEdRMyLwyNwg2ea
# 8kC/pjEiiwRkqd+XCBAxPPzbp27mxy+Qf9DPnmc2PPw/42av3nzGeF4eB5ME1777
# XgLKINM/lNvrOwZQoq1D7RaG7SFETKzus+A2H7edVwxd4Cl8cjETIR4pUBh+AAuW
# hrqp83MPQpi5zA92KJcf3CqxM0Ic9lGD8bB1z8vDSw413VKYXQ==
# SIG # End signature block
