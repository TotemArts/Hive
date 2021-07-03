FROM mcr.microsoft.com/dotnet/core/sdk:3.1.201-bionic AS build

WORKDIR /src
COPY . .
COPY ./nuget.build.config ./nuget.config
ENTRYPOINT ["/bin/bash", "/src/tests/entrypoint.sh"]