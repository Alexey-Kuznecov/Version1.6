Import-Module .\set_ntfs_rule.psm1
# Set-ExecutionPolicy Unrestricted
# $file = New-Object -TypeName System.IO.FileStream -ArgumentList E:\File.txt,Create,ReadWrite
# $file.SetLength(1000Mb)
# $file.Close()

# $months = @('January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December')

# New-Item -Path 'E:\temp' -ItemType Directory

# for ($i=2000; $i -le 2021; $i++)
# {
#     New-Item -Path "E:\temp\$i" -ItemType Directory
#     foreach ($month in $months)
#     {
#         $random = Get-Random -max 12Mb -min 10Mb
#         $file = New-Object -TypeName System.IO.FileStream -ArgumentList "E:\temp\$i\$month.txt",Create,ReadWrite
#         $file.SetLength($random)
#         $file.Close()
#     }
# }

# $f_genres = @('Anime','Biographical','Thriller','Western','Military','Detective','Child','Documentary','Drama','Historical','Film comics','Comedy','Concert','Short','Crime','Melodrama','Mystic','Music','Cartoon','Musical','Scientific','Adventures','Reality show','Family','Sport','Talk show','Horrors','Fantastic','Film noir','Fantasy','Erotica')
# $v_formats = @('dv','avi','mpeg','mov','dvd','flv','dvdrip','bdrip','hdrip')

# foreach ($genre in $f_genres)
# {
#     New-Item -Path "E:\temp\Films\$genre" -ItemType Directory

#     for ($i=0; $i -le $v_formats.Length; $i++)
#     {
#         $random_count = Get-Random -max 10 -min 5

#         for ($i = 0; $i -lt $random_count; $i++)
#         {
#             $format = Get-Random -InputObject $v_formats
#             $random = Get-Random -max 20Mb -min 10Mb
#             $file = New-Object -TypeName System.IO.FileStream -ArgumentList "E:\temp\Films\$genre\film$i.$format",Create,ReadWrite
#             $file.SetLength($random)
#             $file.Close()
#         }
#     }
# }

# $m_genres = @('Disco','House','Techno','Country','Lounge','Trance','Electro','Jazz','R & B','Rap','Hip-Hop','Rock','Pop music','Dubstep','Drum & Bass')
# $m_formats = @('mp3','wav','wma','ogg','flac','aac','aiff','alac')

# foreach ($genre in $m_genres)
# {
#     New-Item -Path "E:\temp\Music\$genre" -ItemType Directory

#     for ($i=0; $i -le $m_formats.Length; $i++)
#     {
#         $random_count = Get-Random -max 30 -min 5

#         for ($i = 0; $i -lt $random_count; $i++)
#         {
#             $format = Get-Random -InputObject $m_formats
#             $random = Get-Random -max 24Mb -min 3Mb
#             $file = New-Object -TypeName System.IO.FileStream -ArgumentList "E:\temp\Music\$genre\film$i.$format",Create,ReadWrite
#             $file.SetLength($random)
#             $file.Close()
#         }
#     }
# }

# Creates a file that will throw exceptions when deleted or copied.
function CreateProblemFile {
    param (
        [Parameter()]
        [string]$Path
    )
    # To prevent inheritance of access rights.
    $acl = Get-ACL -Path $Path
    $acl.SetAccessRuleProtection($True, $True)
    Set-Acl -Path $Path -AclObject $acl
    # # To delete permissions for all users.
}


# $p_cat = @('Art Materials','Downloads','FlatImages','Abstraction','Avatar',
#     'Animation','Cities','Girls','Other','Games','Icons','Illustrations',
#     'Infographics','Clipart','Beautiful Pictures','Creative Pictures','Faces',
#     'Logos','Macro','Markers','Materials','Cars','Memes','Miniatures','Models',
#     'Music','Covers','Wallpaper','Pasters','Landscapes','Posters','Jokes','Various',
#     'Drawings','Sarcasm','Site Assembly','Silhouettes','Screenshots','With Meaning',
#     'Scheme Sand Hierarchies','Textures','Folders Background','Photoshop')

# $i_format = @('raw','jpeg','tiff','psd','bmp','gif','png')

# foreach ($cat in $p_cat)
# {
#     # $folderex = Test-Path -Path E:\temp\Pictures

#     New-Item -Path "E:\temp\Pictures\$cat" -ItemType Directory

#     for ($i=0; $i -le $i_format.Length; $i++)
#     {
#         $random_count = Get-Random -max 10 -min 5

#         for ($i = 0; $i -lt $random_count; $i++)
#         {
#             $format = Get-Random -InputObject $i_format
#             $random = Get-Random -max 10Mb -min 3Mb
#             $path = "E:\temp\Pictures\$cat\film$i.$format"
#             $file = New-Object -TypeName System.IO.FileStream -ArgumentList $path,Create,ReadWrite
#             $file.SetLength($random)
#             $file.Close()
#             SetAccess -Path $path
#             CreateProblemFile -Path $path
#             # if ($count -eq 15) {
#             #     # Remove-NTFSAccess -Path $Path -Account 'Все' -AccessRights $fullControl
#             #     $count = 0
#             # }
#             # $count++
#         }
#     }
# }

$files = Get-ChildItem -Path E:\temp\Pictures -File -Recurse  

foreach ($item in $files) 
{
    Remove-NTFSAccess -Path $item.FullName -Account 'Все' -AccessRights 'FullControl'
}

# Remove-Item -Path E:\temp\Pictures -Force -Confirm