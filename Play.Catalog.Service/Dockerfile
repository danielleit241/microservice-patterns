FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Play.Catalog.Service/Play.Catalog.Service.csproj", "Play.Catalog.Service/"]
COPY ["Play.Catalog.Contracts/Play.Catalog.Contracts.csproj", "Play.Catalog.Contracts/"]
COPY ["nuget.config", "."]
COPY ["packages", "./packages"]

RUN dotnet restore "./Play.Catalog.Service/Play.Catalog.Service.csproj" --configfile nuget.config
COPY . .
WORKDIR "/src/Play.Catalog.Service"
RUN dotnet build "./Play.Catalog.Service.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Play.Catalog.Service.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Play.Catalog.Service.dll"]