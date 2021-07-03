#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
COPY ["endpoints/Server", "shared" "/src"]
WORKDIR "/src/endpoints/Server/src/Worker"
RUN dotnet build "Hive.Endpoints.Server.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Hive.Endpoints.Server.Worker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Hive.Endpoints.Server.Worker.dll"]