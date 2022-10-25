FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /DP/CoreLibrary

COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /DP
COPY --from=build-env /DP/CoreLibrary .
ENTRYPOINT ["dotnet", "DotNet.Docker.dll"]