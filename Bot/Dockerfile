#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Bot/Bot.csproj", "Bot/"]
COPY ["Library/Library.csproj", "Library/"]
RUN dotnet restore "Bot/Bot.csproj"
COPY . .
WORKDIR "/src/Bot"
RUN dotnet build "Bot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Bot.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Bot.dll"]