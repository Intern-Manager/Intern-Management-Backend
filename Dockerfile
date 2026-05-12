# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY InternManagement.sln .
COPY InternManagement.API/InternManagement.API.csproj InternManagement.API/
COPY InternManagement.Application/InternManagement.Application.csproj InternManagement.Application/
COPY InternManagement.Domain/InternManagement.Domain.csproj InternManagement.Domain/
COPY InternManagement.Infrastructure/InternManagement.Infrastructure.csproj InternManagement.Infrastructure/

# Restore dependencies
RUN dotnet restore InternManagement.API/InternManagement.API.csproj

# Copy all source code
COPY . .

# Build and publish
WORKDIR /src/InternManagement.API
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Create non-root user for security
RUN adduser --disabled-password --gecos "" appuser

# Copy published files
COPY --from=build /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Switch to non-root user
USER appuser

EXPOSE 8080

ENTRYPOINT ["dotnet", "InternManagement.API.dll"]
