$key = Get-ItemPropertyValue "HKCU:\SOFTWARE\Classes\.dll" "(Default)"
[Microsoft.Win32.Registry]::SetValue('HKEY_CURRENT_USER\SOFTWARE\Classes\' + $key + '\shell\AssemblyInformation\command', '', '"' + $PSScriptRoot + '\AssemblyInformation.Launcher.exe" "%1"')
[Microsoft.Win32.Registry]::SetValue('HKEY_CURRENT_USER\SOFTWARE\Classes\' + $key + '\shell\AssemblyInformation', 'icon', '"' + $PSScriptRoot + '\Icon.ico"')

$key = Get-ItemPropertyValue "HKCU:\SOFTWARE\Classes\.exe" "(Default)"
[Microsoft.Win32.Registry]::SetValue('HKEY_CURRENT_USER\SOFTWARE\Classes\' + $key + '\shell\AssemblyInformation\command', '', '"' + $PSScriptRoot + '\AssemblyInformation.Launcher.exe" "%1"')
[Microsoft.Win32.Registry]::SetValue('HKEY_CURRENT_USER\SOFTWARE\Classes\' + $key + '\shell\AssemblyInformation', 'icon', '"' + $PSScriptRoot + '\Icon.ico"')