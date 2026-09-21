# FirstMilestone single-container image.
# Angular is built first, then copied into ASP.NET Core wwwroot.
FROM node:22-alpine AS frontend-build
WORKDIR /frontend
COPY src/frontend/first-milestone/package*.json ./
RUN npm install --no-audit --no-fund
COPY src/frontend/first-milestone/ ./
RUN npm run build

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS backend-build
WORKDIR /src
COPY src/backend/FirstMilestone.Api/FirstMilestone.Api.csproj src/backend/FirstMilestone.Api/
RUN dotnet restore src/backend/FirstMilestone.Api/FirstMilestone.Api.csproj
COPY src/backend/FirstMilestone.Api/ src/backend/FirstMilestone.Api/
RUN dotnet publish src/backend/FirstMilestone.Api/FirstMilestone.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
COPY --from=backend-build /app/publish .
COPY --from=frontend-build /frontend/dist/first-milestone/browser ./wwwroot
ENTRYPOINT ["dotnet", "FirstMilestone.Api.dll"]
