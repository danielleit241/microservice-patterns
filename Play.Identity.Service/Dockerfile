FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Play.Identity.Service/Play.Identity.Service.csproj", "Play.Identity.Service/"]
COPY ["nuget.config", "."]
COPY ["packages", "./packages"]

RUN dotnet restore "./Play.Identity.Service/Play.Identity.Service.csproj" --configfile nuget.config
COPY . .
WORKDIR "/src/Play.Identity.Service"
RUN dotnet build "./Play.Identity.Service.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Play.Identity.Service.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Play.Identity.Service.dll"]