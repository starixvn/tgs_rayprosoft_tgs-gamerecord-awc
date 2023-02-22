#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BuildConfiguration=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Tgs.GameRecord.AwcService/Tgs.GameRecord.AwcService.csproj", "Tgs.GameRecord.AwcService/"]
COPY ["Tgs.GameRecord.AwcService.Core/Tgs.GameRecord.AwcService.Core.csproj", "Tgs.GameRecord.AwcService.Core/"]
RUN dotnet restore "Tgs.GameRecord.AwcService/Tgs.GameRecord.AwcService.csproj"
COPY . .
WORKDIR "/src/Tgs.GameRecord.AwcService"
RUN dotnet build "Tgs.GameRecord.AwcService.csproj" -c $BuildConfiguration -o /app/build

FROM build AS publish
ARG BuildConfiguration
RUN dotnet publish "Tgs.GameRecord.AwcService.csproj" -c $BuildConfiguration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.GameRecord.AwcService.dll"]