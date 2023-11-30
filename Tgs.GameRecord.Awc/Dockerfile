#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Tgs.GameRecord.Awc/Tgs.GameRecord.Awc.csproj", "Tgs.GameRecord.Awc/"]
COPY ["Tgs.GameRecord.Awc.Core/Tgs.GameRecord.Awc.Core.csproj", "Tgs.GameRecord.Awc.Core/"]
COPY ["Tgs.GameRecord.Awc.Common/Tgs.GameRecord.Awc.Common.csproj", "Tgs.GameRecord.Awc.Common/"]
RUN dotnet restore "./Tgs.GameRecord.Awc/./Tgs.GameRecord.Awc.csproj"
COPY . .
WORKDIR "/src/Tgs.GameRecord.Awc"
RUN dotnet build "./Tgs.GameRecord.Awc.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Tgs.GameRecord.Awc.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.GameRecord.Awc.dll"]