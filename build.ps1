Import-Module BuildScripts

Build-BSDotNetPackage -ProjectName AmeliaRSVP.Web -DockerImage mcr.microsoft.com/dotnet/sdk:6.0.101-alpine3.14
