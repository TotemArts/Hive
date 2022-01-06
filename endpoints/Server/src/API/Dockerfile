#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 21337

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
COPY ["endpoints/Server", "shared" "/src"]
WORKDIR "/src/endpoints/Server/src/API"
RUN dotnet build "Hive.Endpoints.Server.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Hive.Endpoints.Server.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Hive.Endpoints.Server.API.dll"]