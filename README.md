# DevTrack - A Modern .NET Issue Tracker

A professional-grade issue tracking system built with Clean Architecture, CQRS, and modern .NET practices.

## 🚀 Features

- **Clean Architecture**: Domain-driven design with clear separation of concerns
- **CQRS with MediatR**: Command Query Responsibility Segregation pattern
- **JWT Authentication**: Secure token-based authentication
- **Role-based Authorization**: Project-level access control
- **Redis Caching**: High-performance caching with cache-aside pattern
- **Domain Events**: Decoupled event-driven architecture
- **PostgreSQL Database**: Robust relational data storage
- **Docker Support**: Fully containerized application
- **Integration Testing**: Comprehensive tests with Testcontainers
- **Health Checks**: Production-ready health monitoring
- **CI/CD Pipeline**: Automated builds and tests with GitHub Actions

## 🛠️ Technology Stack

- **Backend**: .NET 8, ASP.NET Core Web API
- **Architecture**: Clean Architecture with CQRS (MediatR)
- **Database**: PostgreSQL with Entity Framework Core
- **Caching**: Redis with StackExchange.Redis
- **Authentication**: JWT Bearer tokens
- **Testing**: xUnit, FluentAssertions, Testcontainers
- **Documentation**: OpenAPI/Swagger
- **Containerization**: Docker & Docker Compose
- **CI/CD**: GitHub Actions

## 📋 Prerequisites

- Docker Desktop
- .NET 8 SDK (for local development)
- Git

## 🚀 Getting Started

### Using Docker (Recommended)

1. Clone the repository:
   ```bash
   git clone https://github.com/codewhizzz/DevTrack.git
   cd DevTrack

2. Start the application:
    docker-compose up -d

3. Access the application:

    API: http://localhost:8081
    Swagger UI: http://localhost:8081/swagger
    Health Check: http://localhost:8081/health
    PostgreSQL: localhost:5440
    Redis: localhost:6380