# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["src/DevTrack.Api/DevTrack.Api.csproj", "src/DevTrack.Api/"]
COPY ["src/DevTrack.Application/DevTrack.Application.csproj", "src/DevTrack.Application/"]
COPY ["src/DevTrack.Domain/DevTrack.Domain.csproj", "src/DevTrack.Domain/"]
COPY ["src/DevTrack.Infrastructure/DevTrack.Infrastructure.csproj", "src/DevTrack.Infrastructure/"]
RUN dotnet restore "src/DevTrack.Api/DevTrack.Api.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/src/DevTrack.Api"
RUN dotnet build "DevTrack.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "DevTrack.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Add non-root user
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DevTrack.Api.dll"]