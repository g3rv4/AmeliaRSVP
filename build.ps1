Import-Module BuildScripts
Build-BSDotNetPackage -ProjectName AmeliaRSVP.Web -DockerImage mcr.microsoft.com/dotnet/sdk:6.0.101-alpine3.14

if ($env:GITHUB_ENV) {
    Write-Output "VERSION=$newVersion" | Out-File -FilePath $env:GITHUB_ENV -Encoding utf8 -Append
}