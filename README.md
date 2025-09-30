# Play Economy API

Play Economy API is a small microservices sample solution implemented in C# on the .NET platform. It demonstrates a typical microservice layout with separate services, shared contracts, and an API gateway.

## Key Technologies
- Language / Platform: C# on .NET (solution: `microservice-patterns.sln`)
- HTTP APIs: ASP.NET Core (implied by project layout)
- Architecture: Microservices (independent projects for Catalog, Inventory, Identity)
- API Gateway: Play.ApiGateway (central entrypoint / routing)
- Shared Contracts / DTOs: Play.Catalog.Contracts
- Shared Libraries / Infrastructure: Play.Common, Play.Infra
- Tooling: dotnet CLI, Visual Studio / VS Code
- Typical patterns: Dependency Injection, configuration via appsettings, separation of concerns

## Repository Projects
- Play.ApiGateway
- Play.Catalog.Service
- Play.Catalog.Contracts
- Play.Inventory.Service
- Play.Identity.Service
- Play.Common
- Play.Infra
- microservice-patterns.sln

## Quick start (developer)
1. Clone:
   git clone https://github.com/danielleit241/play-economy-api.git
2. Restore and build:
   dotnet restore
   dotnet build microservice-patterns.sln
3. Run a service:
   dotnet run --project Play.Catalog.Service
   dotnet run --project Play.Inventory.Service
   dotnet run --project Play.Identity.Service
   dotnet run --project Play.ApiGateway

Check each project's `appsettings.json` or environment variables for ports, connection strings and other runtime settings before running multiple services simultaneously.

## Notes & Next Steps
- This README focuses on technologies and structure. For a full developer guide you can add Dockerfiles, docker-compose for local integration, CI workflows, and explicit .NET target versions.
- No license file is present in the repository—add a LICENSE if you plan to publish or share this project publicly.
