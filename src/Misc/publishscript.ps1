#Install-Module -Name Posh-SSH
$compress = @{
  Path = "c:\max-projects\Bonanza-1-without-app-service\bonanza\src\Bonanza.Storage.Benchmark\bin\Release\netcoreapp3.1\win-x64\"
  CompressionLevel = "Optimal"
  DestinationPath = "c:\max-projects\Bonanza-1-without-app-service\bonanza\src\Bonanza.Storage.Benchmark\bin\Release\netcoreapp3.1\win-x64.Zip"
}
Compress-Archive @compress -Force

#ssh -i "E:\documents\Passwords\sshkeys-dev01\bonanza-dev01_key.pem" azurecat@20.102.99.1
#Set-SCPFile -ComputerName azurecat@20.102.99.1 -Credential "E:\documents\Passwords\sshkeys-dev01\bonanza-dev01_key.pem" -RemotePath "c:\app\" -LocalFile 'c:\max-projects\Bonanza-1-without-app-service\bonanza\src\Bonanza.Storage.Benchmark\bin\Release\netcoreapp3.1\win-x64.Zip'