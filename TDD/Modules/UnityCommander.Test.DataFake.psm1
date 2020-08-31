Import-Module NTFSAccess
function DataFaker { 
    [CmdletBinding()]
    param (
        [parameter(Mandatory=$TRUE,Position=0,HelpMessage="Directory to compare left.")]
        [alias("n")]
        [string]$Name
      )

    $files = Get-ChildItem -Path E:\temp\Pictures -File -Recurse  

    foreach ($item in $files) 
    {
        Remove-NTFSAccess -Path $item.FullName -Account 'Все' -AccessRights 'FullControl'
    }
}

DataFaker -Name 'dasd'

# SIG # Begin signature block
# MIIFkQYJKoZIhvcNAQcCoIIFgjCCBX4CAQExCzAJBgUrDgMCGgUAMGkGCisGAQQB
# gjcCAQSgWzBZMDQGCisGAQQBgjcCAR4wJgIDAQAABBAfzDtgWUsITrck0sYpfvNR
# AgEAAgEAAgEAAgEAAgEAMCEwCQYFKw4DAhoFAAQULCw1yem64ImA5NajM+Ql+3qR
# kAygggMqMIIDJjCCAg6gAwIBAgIQejt+1DFMRJNF0fWWJera3TANBgkqhkiG9w0B
# AQsFADAcMRowGAYDVQQDDBFsZXhhMXRoQGdtYWlsLmNvbTAeFw0yMDA4MjAxNjI1
# MTJaFw0yMTA4MjAxNjQ1MTJaMBwxGjAYBgNVBAMMEWxleGExdGhAZ21haWwuY29t
# MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAoIYA/5hvYIy9PE7GxIj5
# y6YCxuHYR5JCPzcdIxG75YggaShrFha1xlIJcoio/FNRHn0BVLb2ZQ/Gz+y1C+PF
# eDoz2pNx82lHXOOTPo5Jhcq2fhVWOHiy24zntZzksnZ+jkOsKHwxrbhPFjtlxiWF
# Gm3U6s2RdbSb+u+OmQUVLjYhPq2y/KeWq4qdqkaG44aj4NSIXAX7gfd5uFUA4fjv
# p4t/XMby+Sh7VWgTOtSL5+5Nbs7/9EGmvDW+ojktXY4vjlMsZhq2GcZJ9dAlye9a
# yaZ16u6DYbA3cRwjGDe4awl4bqoKMEHGxanOOioqj3PLTLI0RAMy/Gd2Nd9WCENp
# 0QIDAQABo2QwYjAOBgNVHQ8BAf8EBAMCB4AwEwYDVR0lBAwwCgYIKwYBBQUHAwMw
# HAYDVR0RBBUwE4IRbGV4YTF0aEBnbWFpbC5jb20wHQYDVR0OBBYEFGCcF4QyeoRn
# B0jgQhkjYy5R+CujMA0GCSqGSIb3DQEBCwUAA4IBAQBno3JpH+b0/tfRw2Mz6ga7
# 57p25qOcvM3EuDdsUskWYSl+cNNmjsm9FKpnvETP15SY/uwJk097PlYO60Hhw0FC
# wsmburypOwfkAfu07I5q2Ya41iYOp6Esxdyd9XXLTeGqTIbfqgZvyrKS/WKrQJCU
# e5uKIPUhAQaY2KImSCXJRT/xhuRMSpQVcJwTdutLjOx/pVWqCytbg32R8dlna5QO
# tmOpr9AaFSPBJDhfRa0SnhmeNNBj1xwSpu+QfR6gmzEpwodVrhEHpJd8aX0nCt3+
# WZyoSuML/jG4czPh4/GGD5SvsYD47Gir1ZGkiUEU3EaoJS12wT3S57JyFnA6js+N
# MYIB0TCCAc0CAQEwMDAcMRowGAYDVQQDDBFsZXhhMXRoQGdtYWlsLmNvbQIQejt+
# 1DFMRJNF0fWWJera3TAJBgUrDgMCGgUAoHgwGAYKKwYBBAGCNwIBDDEKMAigAoAA
# oQKAADAZBgkqhkiG9w0BCQMxDAYKKwYBBAGCNwIBBDAcBgorBgEEAYI3AgELMQ4w
# DAYKKwYBBAGCNwIBFTAjBgkqhkiG9w0BCQQxFgQUkImi+JoQ3hXd2Yk6G9cljT4Y
# yxIwDQYJKoZIhvcNAQEBBQAEggEAe4zBQRapVVajoHvbtv3AJHOjq0ghsVm2u2rt
# uPxsT3iVzdbr7yYmpt77yajH16DJSczDEWDTGQFFXXumMjbdi4a/IFinqzjlOH6g
# T3Mok1b8cnS+dIUAdXbsdkxN6KKpHn5cPA4UFOiyXk60IxWRUDJRP3t1GYMXMxOk
# bzajuT+nVdVtyOvyCbzUBUujxR/e928QCVhPSRUIqyj93o1C7hA8wDrLUR8qKzy0
# Gb0l+Nd31sh8Rm/paxaVoAROnfXcs5pLo7IguCnl1Y4ptb5F1fW68WsKFGcv7dlC
# +bapwJcIXAG0pnpMrY3THurbqnFkWtnm7EkW3tl3G9pZWrrprg==
# SIG # End signature block
