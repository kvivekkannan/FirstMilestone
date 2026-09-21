# FirstMilestone

A deliberately small **Senior .NET / Angular practice application** designed to exercise the stack you are preparing for interviews: .NET 8, ASP.NET Core Web API, Angular, SQL Server, Entity Framework Core, Docker, REST APIs, testing, cloud deployment and an extensible module pattern.

## What is included

- **Angular 20** dashboard UI.
- **.NET 8 ASP.NET Core Web API** backend.
- **SQL Server + EF Core** for dashboard module configuration and weather-search history.
- **Open-Meteo** weather integration. It requires no API key for non-commercial use and asks for attribution. citeturn0search0turn0search5
- A simple **module catalog** with Weather installed by default and Stocks/News available as future modules.
- Unit-test project using **xUnit**.
- Dockerfiles for:
  - backend only
  - frontend only
  - the complete application in one container
- Docker Compose for local SQL Server + separate frontend/backend containers.
- Docker Compose for the single-container version.
- AWS/ECS deployment notes and a task-definition template.
- Health endpoint: `/health`.
- Swagger in Development: `/swagger`.

## Architecture

```text
                         +----------------------+
                         |      Angular UI      |
                         |  Dashboard / Modules  |
                         +----------+-----------+
                                    |
                                  HTTP
                                    |
                         +----------v-----------+
                         |   ASP.NET Core API   |
                         | .NET 8 / Controllers |
                         +----+------------+----+
                              |            |
                         EF Core        HTTP API
                              |            |
                    +---------v--+   +----v----------------+
                    | SQL Server |   | Open-Meteo           |
                    | modules +  |   | geocoding + forecast |
                    | search log |   +----------------------+
                    +------------+
```

The dashboard is intentionally module-oriented. The dashboard shell asks the API which modules are installed, and module-specific UI is rendered from that configuration. The catalog provides a small install path for future modules.

## Project structure

```text
FirstMilestone/
├── src/
│   ├── backend/
│   │   └── FirstMilestone.Api/
│   │       ├── Controllers/
│   │       ├── Data/
│   │       ├── DTOs/
│   │       ├── Entities/
│   │       ├── Services/
│   │       └── Program.cs
│   └── frontend/
│       └── first-milestone/
│           └── src/app/
├── tests/
│   └── FirstMilestone.Api.Tests/
├── deploy/aws/
├── docs/
├── Dockerfile
├── Dockerfile.backend
├── Dockerfile.frontend
├── docker-compose.yml
├── docker-compose.single.yml
└── README.md
```

## Run locally without Docker

### Backend

Prerequisites:

- .NET 8 SDK
- SQL Server locally, or change `DefaultConnection` in `appsettings.json`.

```bash
cd src/backend/FirstMilestone.Api
dotnet restore
dotnet run
```

Backend: `http://localhost:5080`

Swagger: `http://localhost:5080/swagger`

### Frontend

Prerequisites:

- Node.js 22+
- npm

```bash
cd src/frontend/first-milestone
npm install
npm start
```

Frontend: `http://localhost:4200`

The Angular dev server proxies `/api` to the local .NET API using `proxy.conf.json`.

## Run the full stack with Docker

This is the recommended local practice path because it exercises containers and SQL Server together.

```bash
docker compose up --build
```

Open:

- Dashboard: `http://localhost:4200`
- API: `http://localhost:8080`
- Swagger: `http://localhost:8080/swagger` (only if the backend environment is changed to Development)
- Health: `http://localhost:8080/health`
- SQL Server: `localhost:1433`

The first startup can take a little longer because SQL Server needs to initialize before the API creates its schema.

> The sample SQL password is intentionally only for local practice. Replace it before any shared or cloud deployment.

## Run the single-container version

This version builds Angular and copies the generated static files into ASP.NET Core `wwwroot`.

```bash
docker compose -f docker-compose.single.yml up --build
```

Open `http://localhost:8080`.

This is the simplest deployment shape for the first ECS exercise: one application container plus a managed SQL Server database.

## API endpoints

| Method | Endpoint | Purpose |
|---|---|---|
| GET | `/health` | Application/database health check |
| GET | `/api/dashboard/summary` | Dashboard summary |
| GET | `/api/modules` | List installed/available modules |
| POST | `/api/modules/{key}/install` | Install a module |
| POST | `/api/modules/{key}/uninstall` | Uninstall a module |
| GET | `/api/weather?city=Chennai` | Current weather + 5-day forecast |

## SQL Server usage

SQL Server is used for two deliberately simple concerns:

1. **DashboardModules** — module registry and installation state.
2. **WeatherSearches** — a lightweight audit/history of weather searches.

EF Core creates the local schema with `EnsureCreated()` to keep this practice application easy to start. For a production version, move to EF Core migrations and an explicit migration pipeline.

## Weather integration

The backend calls Open-Meteo's geocoding endpoint first and then its forecast endpoint. Open-Meteo documents a no-key API for non-commercial use, including current and forecast weather data. Attribution is required for the data. citeturn0search0turn0search6

The browser does **not** call Open-Meteo directly. The .NET API acts as the integration boundary. That gives you a useful place to add:

- caching
- retries/timeouts
- structured logging
- rate limiting
- provider abstraction/fallbacks
- telemetry
- contract tests

## Testing

```bash
dotnet test tests/FirstMilestone.Api.Tests/FirstMilestone.Api.Tests.csproj
```

The test project currently demonstrates xUnit-based unit testing around the weather-code mapping. Expand it as you evolve the project.

## Docker images

### Backend

```bash
docker build -f Dockerfile.backend -t firstmilestone-backend .
```

### Frontend

```bash
docker build -f Dockerfile.frontend -t firstmilestone-frontend .
```

### Complete application

```bash
docker build -t firstmilestone .
```

## Suggested AWS evolution

The next exercise should be to move the same application to AWS without changing its core business code.

A good learning path is:

```text
GitHub
  |
  v
GitHub Actions
  |
  +---- Build/test .NET
  +---- Build/test Angular
  +---- Build Docker image
  |
  v
Amazon ECR
  |
  v
Amazon ECS + Fargate
  |
  +---- Application container
  |
  v
Application Load Balancer
  |
  v
Internet

SQL Server ---> Amazon RDS for SQL Server
Secrets -----> AWS Secrets Manager
Logs --------> CloudWatch
```

Amazon ECS is a managed container orchestration service, and Fargate lets ECS run containers without managing the underlying servers. AWS documents ECS/Fargate task definitions, load balancing and monitoring as the main deployment building blocks. citeturn0search1turn0search9

For this practice application, the **single-container Dockerfile** is the shortest path to ECS. After that works, split the frontend/backend into independent ECS services to practice service discovery, ALB routing and independent scaling.

See [`deploy/aws/README.md`](deploy/aws/README.md) for the next steps.

## Interview-practice extensions

Once this baseline is working, use the application to deliberately add senior-level engineering concerns:

1. Add JWT authentication and role-based access.
2. Add Redis caching for weather responses.
3. Replace `EnsureCreated()` with EF Core migrations.
4. Add global exception handling and Problem Details.
5. Add structured logging with correlation IDs.
6. Add OpenTelemetry traces/metrics.
7. Add Polly resilience policies around external APIs.
8. Add integration tests with a disposable SQL Server container.
9. Add GitHub Actions CI/CD.
10. Push images to ECR and deploy to ECS Fargate.
11. Move the SQL database to RDS.
12. Store secrets in AWS Secrets Manager.
13. Add CloudWatch dashboards and alarms.
14. Add a Stock module.
15. Add a News module.
16. Add an AI module using an LLM API.
17. Add feature flags for module installation.
18. Add API versioning.
19. Add rate limiting.
20. Add background processing for scheduled module refreshes.

These extensions turn the app from a simple demo into a compact interview laboratory for .NET, cloud, DevOps, system design and AI integration.
