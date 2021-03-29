function Add-ShellExtension($fileExtension) {
	$key = Get-ItemPropertyValue "Registry::HKCR\.$fileExtension" "(default)"
	New-Item -Path "Registry::HKCU\SOFTWARE\Classes\$key\shell\AssemblyInformation\command" -Value """$PSScriptRoot\AssemblyInformation.Launcher.exe"" ""%1""" -Force
	New-ItemProperty -Path "Registry::HKCU\SOFTWARE\Classes\$key\shell\AssemblyInformation" -Name "icon" -Value """$PSScriptRoot\AssemblyInformation.ico""" -Force
}

Add-ShellExtension("dll")
Add-ShellExtension("exe")