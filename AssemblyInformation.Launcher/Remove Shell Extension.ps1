function Remove-ShellExtension($fileExtension) {
	$key = Get-ItemPropertyValue "Registry::HKCR\.$fileExtension" "(default)"
	Remove-Item -Path "Registry::HKCU\SOFTWARE\Classes\$key\shell\AssemblyInformation" -Recurse
}

Remove-ShellExtension("dll")
Remove-ShellExtension("exe")