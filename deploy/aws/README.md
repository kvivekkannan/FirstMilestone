# FirstMilestone on AWS ECS/Fargate

This folder is a starting point for the next stage of the practice project.

## Recommended first AWS topology

For the first deployment, keep the application as one container using the root `Dockerfile`:

```text
Route 53 / DNS (optional)
        |
        v
Application Load Balancer
        |
        v
ECS Service (Fargate)
        |
        +--> FirstMilestone container :8080
                 |
                 +--> Amazon RDS for SQL Server
                 |
                 +--> Open-Meteo over HTTPS
```

ECS/Fargate is suitable for this exercise because AWS manages the container infrastructure while you define the task's CPU, memory, networking and IAM requirements. citeturn0search1turn0search4

## Deployment sequence

### 1. Build the application image

```bash
docker build -t firstmilestone .
```

### 2. Create an ECR repository

Create an ECR repository such as `firstmilestone` and authenticate Docker to ECR.

### 3. Tag and push

```bash
docker tag firstmilestone:latest <account-id>.dkr.ecr.<region>.amazonaws.com/firstmilestone:latest
docker push <account-id>.dkr.ecr.<region>.amazonaws.com/firstmilestone:latest
```

### 4. Create RDS for SQL Server

Create an RDS SQL Server instance in the same VPC as the ECS service. Keep the database private. Allow inbound database traffic only from the ECS task security group.

### 5. Store the database secret

Use AWS Secrets Manager instead of putting a password directly into the ECS task definition.

Recommended secret shape:

```json
{
  "username": "...",
  "password": "...",
  "host": "...",
  "port": "1433",
  "database": "FirstMilestone"
}
```

### 6. Create the ECS cluster

Create an ECS cluster using Fargate capacity.

### 7. Register the task definition

Use `task-definition.template.json` as a starting point. Replace the placeholders and wire the secret into the container environment.

### 8. Create an ALB

The ALB should forward HTTP/HTTPS traffic to the ECS task on port 8080. Use `/health` as the target-group health check.

### 9. Create the ECS service

Start with one task. Once the baseline works, practice desired-count scaling and deployment configuration.

### 10. Add observability

Send container logs to CloudWatch Logs and create alarms around errors, latency and task health.

## After the first successful deployment

Move to a two-service architecture:

```text
ALB
 |
 +-- /api/* ------> ECS backend service
 |
 +-- /* ----------> ECS frontend service
```

Then practice:

- independent scaling
- separate deployments
- blue/green or rolling deployments
- service-to-service networking
- centralized configuration
- secrets management
- container vulnerability scanning
- CloudWatch dashboards
- GitHub Actions -> ECR -> ECS deployment

AWS's ECS documentation covers Fargate task definitions, load balancing, capacity and monitoring; use the current AWS docs when you perform the actual deployment because console screens and service options can change. citeturn0search1turn0search3
