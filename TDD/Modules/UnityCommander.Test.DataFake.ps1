$files = Get-ChildItem -Path E:\temp\Pictures -File -Recurse  

foreach ($item in $files) 
{
	Remove-NTFSAccess -Path $item.FullName -Account 'Все' -AccessRights 'FullControl'
}