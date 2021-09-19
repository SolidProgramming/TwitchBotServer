#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443



FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["Solid Twitch Bot Server/Solid Twitch Bot Server.csproj", "Solid Twitch Bot Server/"]
COPY ["Bot Manager/BotManager.csproj", "Bot Manager/"]
COPY ["Logger/Logger.csproj", "Logger/"]
COPY ["OBSWebsocketController/OBSWebsocketController.csproj", "OBSWebsocketController/"]
COPY ["Shares/Shares.csproj", "Shares/"]
RUN dotnet restore "Solid Twitch Bot Server/Solid Twitch Bot Server.csproj"
COPY . .
WORKDIR "/src/Solid Twitch Bot Server"
RUN dotnet build "Solid Twitch Bot Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Solid Twitch Bot Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Solid Twitch Bot Server.dll"]