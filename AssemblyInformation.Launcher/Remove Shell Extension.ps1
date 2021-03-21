$key = Get-ItemPropertyValue "HKCU:\SOFTWARE\Classes\.dll" "(Default)"
Remove-Item -Path ('HKCU:\SOFTWARE\Classes\' + $key + '\shell\AssemblyInformation') -Recurse

$key = Get-ItemPropertyValue "HKCU:\SOFTWARE\Classes\.exe" "(Default)"
Remove-Item -Path ('HKCU:\SOFTWARE\Classes\' + $key + '\shell\AssemblyInformation') -Recurse