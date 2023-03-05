#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["CollisionDetector/CollisionDetector.csproj", "CollisionDetector/"]
COPY ["ApiLibrary/ApiLibrary.csproj", "ApiLibrary/"]
COPY ["CoreLibrary/CoreLibrary.csproj", "CoreLibrary/"]
COPY ["DatabaseHandlerLibrary/DatabaseHandlerLibrary.csproj", "DatabaseHandlerLibrary/"]
COPY ["WebSocketLibrary/WebSocketLibrary.csproj", "WebSocketLibrary/"]
RUN dotnet restore "CollisionDetector/CollisionDetector.csproj"
COPY . .
WORKDIR "/src/CollisionDetector"
RUN dotnet build "CollisionDetector.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CollisionDetector.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CollisionDetector.dll"]