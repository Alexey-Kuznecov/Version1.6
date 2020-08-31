Import-Module .\TDD_Create_Dircetory_Fake.psm1
Import-Module .\TDD_Set_Random_Permissions.psm1

# $months = @('January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December')
$f_genres = @('Anime','Biographical','Thriller','Western','Military','Detective','Child','Documentary','Drama','Historical','Film comics','Comedy','Concert','Short','Crime','Melodrama','Mystic','Music','Cartoon','Musical','Scientific','Adventures','Reality show','Family','Sport','Talk show','Horrors','Fantastic','Film noir','Fantasy','Erotica')
$v_formats = @('dv','avi','mpeg','mov','dvd','flv','dvdrip','bdrip','hdrip')
$m_genres = @('Disco','House','Techno','Country','Lounge','Trance','Electro','Jazz','R & B','Rap','Hip-Hop','Rock','Pop music','Dubstep','Drum & Bass')
$m_formats = @('mp3','wav','wma','ogg','flac','aac','aiff','alac')

$p_cat = @('Art Materials','Downloads','FlatImages','Abstraction','Avatar',
    'Animation','Cities','Girls','Other','Games','Icons','Illustrations',
    'Infographics','Clipart','Beautiful Pictures','Creative Pictures','Faces',
    'Logos','Macro','Markers','Materials','Cars','Memes','Miniatures','Models',
    'Music','Covers','Wallpaper','Pasters','Landscapes','Posters','Jokes','Various',
    'Drawings','Sarcasm','Site Assembly','Silhouettes','Screenshots','With Meaning',
    'Scheme Sand Hierarchies','Textures','Folders Background','Photoshop')
$i_format = @('raw','jpeg','tiff','psd','bmp','gif','png')

# SetRandomPermAll -Path "E:\Temp\Pictures"
CreateDirFaked -d "H:\Works\UnitTests\Source\Pictures" -ln $p_cat -lf $i_format -perm $True
CreateDirFaked -d "H:\Works\UnitTests\Source\Films" -ln $f_genres -lf $v_formats -perm $True
CreateDirFaked -d "H:\Works\UnitTests\Source\Music" -ln $m_genres -lf $m_formats -perm $True

"H:\Works\UnitTests\Source" | RemoveInheritance
RemoveAll -Path "H:\Works\UnitTests\Source" -User "Все" -Perm Full

# Get-ChildItem -Path "E:\Temp" | Where-Object -Property PSIsContainer | Select-Object -Property FullName
# Remove-Item -Path E:\temp\Pictures -Force -Confirm