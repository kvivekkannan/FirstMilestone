# FirstMilestone as an Interview Lab

Use this application as a living example when preparing for senior interviews.

## .NET

- Dependency injection and service lifetimes
- Middleware pipeline
- Async/await and cancellation
- REST API design
- validation and Problem Details
- logging and exception handling

## Architecture

- module boundaries
- Clean Architecture evolution
- CQRS as an optional extension
- separation of external integrations
- DTO/entity boundaries

## SQL Server / EF Core

- indexes
- query plans
- tracking vs no-tracking
- transactions
- migrations
- connection pooling
- concurrency

## Cloud

- ECR
- ECS/Fargate
- ALB
- RDS
- Secrets Manager
- CloudWatch
- IAM

## DevOps

- Docker multi-stage builds
- GitHub Actions
- CI quality gates
- artifact/image promotion
- deployment strategies
- rollback

## System design questions to practice

1. How would you scale the weather module to 100K users?
2. How would you prevent repeated calls to Open-Meteo?
3. How would you handle Open-Meteo downtime?
4. How would you make module installation tenant-specific?
5. How would you split this into microservices?
6. When would you keep it as a modular monolith?
7. How would you deploy this to ECS with zero-downtime releases?
8. How would you secure the database connection?
9. How would you add authentication and authorization?
10. How would you introduce Redis?
