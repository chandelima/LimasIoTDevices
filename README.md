# Lima's IoT Devices API

A .NET 10 Web API that acts as an integration layer between your applications and Home Assistant, providing simplified device management and control through a RESTful interface.

> **Personal Project**: This is a personal project created to solve specific pain points I encountered while working with Home Assistant:
> 
> - **Simpler Device Management**: I needed a cleaner way to manage and control multiple Home Assistant entities as unified devices, with a better API abstraction layer that could be easily integrated into my other applications and automation workflows.
> 
> - **Decoupling Automations from Physical Devices**: Every time I needed to replace a physical relay module or change IoT hardware, I had to update the Home Assistant entity IDs across all my automations, scripts, and integrations. This API solves that by creating a stable virtual device layer - when hardware changes, I only update the entity mapping in one place, and all my automations continue to work seamlessly without any modifications.

## Overview

Lima's IoT Devices API is designed to simplify the interaction with Home Assistant devices by providing a cleaner abstraction layer. It allows you to manage virtual devices that map to one or more Home Assistant entities, enabling easier device control through a unified API.

## Features

- **Device Management**: Create, read, update, and delete virtual devices that represent Home Assistant entities
- **Device Attributes**: Manage multiple attributes per device, each mapped to Home Assistant entities
- **Service Calls**: Execute Home Assistant services through simplified API endpoints
- **Real-time Updates**: WebSocket integration with Home Assistant for live state updates using SSE
- **Event System**: Track and query device state change events
- **Localization Support**: Multi-language error messages and validation
- **Resilience**: Built-in HTTP resilience with Polly for reliable external communications
- **PostgreSQL Database**: Persistent storage for device configurations and event history
- **Docker Support**: Ready-to-use containerization for easy deployment

## Architecture

The project follows Clean Architecture principles with clear separation of concerns:

```
LimasIoTDevices/
├── LimasIoTDevices.API/                # API layer (controllers, middleware)
├── LimasIotDevices.Application/        # Business logic and use cases
├── LimasIotDevices.Domain/             # Domain models and interfaces
├── LimasIotDevices.Infrastructure/     # Data access and external services
├── LimasIoTDevices.Facade/             # DTOs and public interfaces
└── LimasIoTDevices.Shared/             # Shared utilities and cross-cutting concerns
```

### Key Components

- **API Layer**: RESTful endpoints with Swagger documentation
- **Application Layer**: Use cases implementing business logic
- **Domain Layer**: Core business entities and validation rules
- **Infrastructure Layer**: PostgreSQL database access and Home Assistant gateway
- **Facade Layer**: Public contracts (DTOs and interfaces)
- **Shared Layer**: Common utilities, middleware, and extensions

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (for local development)
- [Docker](https://www.docker.com/get-started) (for containerized deployment)
- [PostgreSQL](https://www.postgresql.org/download/) database
- [Home Assistant](https://www.home-assistant.io/) instance
- Home Assistant long-lived access token

## Getting Started

### Option 1: Running with Docker (Recommended)

#### Using Docker Run

1. **Build the Docker image:**

```bash
docker build -t limasiot-devices-api -f LimasIotDevices.API/Dockerfile .
```

2. **Run the container:**

```bash
docker run -d \
  --name limasiot-api \
  -p 8080:8080 \
  -p 8081:8081 \
  -e ConnectionStrings__LimasIotDevices="Host=your-postgres-host;Database=limasiot;Username=your_user;Password=your_password" \
  -e HomeAssistantData__HostUrl="http://your-homeassistant-instance:8123" \
  -e HomeAssistantData__Token="your-long-lived-access-token" \
  limasiot-devices-api
```

#### Using Docker Compose

Create a `docker-compose.yml` file in the root directory:

```yaml
version: '3.8'

services:
  api:
    build:
      context: .
      dockerfile: LimasIotDevices.API/Dockerfile
    container_name: limasiot-api
    ports:
      - "8080:8080"
      - "8081:8081"
    environment:
      - ConnectionStrings__LimasIotDevices=Host=postgres;Database=limasiot;Username=limasiot;Password=your_secure_password
      - HomeAssistantData__HostUrl=http://homeassistant:8123
      - HomeAssistantData__Token=your-long-lived-access-token
      - ASPNETCORE_ENVIRONMENT=Production
    depends_on:
      - postgres
    networks:
      - limasiot-network
    restart: unless-stopped

  postgres:
    image: postgres:17-alpine
    container_name: limasiot-postgres
    environment:
      - POSTGRES_DB=limasiot
      - POSTGRES_USER=limasiot
      - POSTGRES_PASSWORD=your_secure_password
    volumes:
      - postgres-data:/var/lib/postgresql/data
    ports:
      - "5432:5432"
    networks:
      - limasiot-network
    restart: unless-stopped

volumes:
  postgres-data:
    driver: local

networks:
  limasiot-network:
    driver: bridge
```

Run with Docker Compose:

```bash
# Start all services
docker-compose up -d

# View logs
docker-compose logs -f api

# Stop all services
docker-compose down

# Stop and remove volumes
docker-compose down -v
```

The API will be available at:
- HTTP: `http://localhost:8080`
- HTTPS: `http://localhost:8081`
- Swagger UI: `http://localhost:8080/swagger`

### Option 2: Running Locally

#### 1. Clone the Repository

```bash
git clone <repository-url>
cd LimasIoTDevices
```

#### 2. Configure Application Settings

Update `appsettings.json` or use User Secrets:

```json
{
  "ConnectionStrings": {
    "LimasIotDevices": "Host=localhost;Database=limasiot;Username=your_user;Password=your_password"
  },
  "HomeAssistantData": {
    "HostUrl": "http://your-homeassistant-instance:8123",
    "Token": "your-long-lived-access-token"
  }
}
```

#### Using User Secrets (Recommended for Development)

```bash
cd LimasIotDevices.API
dotnet user-secrets set "ConnectionStrings:LimasIotDevices" "Host=localhost;Database=limasiot;Username=your_user;Password=your_password"
dotnet user-secrets set "HomeAssistantData:HostUrl" "http://your-homeassistant-instance:8123"
dotnet user-secrets set "HomeAssistantData:Token" "your-long-lived-access-token"
```

#### 3. Setup Database

Ensure PostgreSQL is running and create the database. Migrations will be applied automatically.


#### 4. Run the Application

```bash
cd LimasIotDevices.API
dotnet run
```

The API will be available at:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger UI: `https://localhost:5001/swagger`

## Docker Configuration

### Dockerfile

The project includes a multi-stage Dockerfile optimized for production:

- **Base stage**: Uses .NET 10 ASP.NET runtime image
- **Build stage**: Compiles the application using .NET 10 SDK
- **Publish stage**: Publishes optimized release build
- **Final stage**: Creates minimal production image

### Environment Variables

When running in Docker, configure the following environment variables:

| Variable | Description | Example |
|----------|-------------|---------|
| `ConnectionStrings__LimasIotDevices` | PostgreSQL connection string | `Host=postgres;Database=limasiot;Username=user;Password=pass` |
| `HomeAssistantData__HostUrl` | Home Assistant URL | `http://homeassistant:8123` |
| `HomeAssistantData__Token` | Home Assistant access token | `your-long-lived-access-token` |
| `ASPNETCORE_ENVIRONMENT` | Environment name | `Production`, `Development` |
| `ASPNETCORE_URLS` | URLs to bind | `http://+:8080;http://+:8081` |

### Docker Networking

When running with Docker Compose alongside Home Assistant:

```yaml
services:
  api:
    # ... other configuration ...
    networks:
      - limasiot-network
      - homeassistant-network  # If Home Assistant is in a different network
    
networks:
  limasiot-network:
    driver: bridge
  homeassistant-network:
    external: true  # Connect to existing Home Assistant network
```

## API Endpoints

### Device Management

#### Search Devices
```http
GET /api/v1/devices/search?searchTerm=living
```

#### Get Device by Key
```http
GET /api/v1/devices/{key}
```

#### Create Device
```http
POST /api/v1/devices
Content-Type: application/json

{
  "key": "living_room_light",
  "name": "Living Room Light",
  "description": "Main ceiling light",
  "attributes": [
    {
      "key": "main",
      "name": "Main",
      "entities": ["light.living_room_ceiling"]
    },
    {
      "key": "brightness",
      "name": "Brightness",
      "entities": ["light.living_room_ceiling"]
    }
  ]
}
```

#### Update Device
```http
PUT /api/v1/devices
Content-Type: application/json

{
  "key": "living_room_light",
  "name": "Living Room Light Updated",
  "description": "Updated description",
  "attributes": [...]
}
```

#### Delete Device
```http
DELETE /api/v1/devices/{key}
```

### Service Calls

#### Call Service on Device Attribute
```http
POST /api/v1/devices/{deviceKey}/{attributeKey}/call-service/{service}
Content-Type: application/json

{
  "brightness": 255,
  "color_temp": 370
}
```

#### Call Service on Main Device Attribute
```http
POST /api/v1/devices/{deviceKey}/call-service/{service}
Content-Type: application/json

{
  "entity_id": "light.living_room"
}
```

### Events

Event endpoints are available through the `EventController` (check Swagger for details).

## Key Concepts

### Devices
Virtual devices that represent one or more Home Assistant entities. Each device has:
- **Key**: Unique identifier
- **Name**: Display name
- **Description**: Optional description
- **Attributes**: Collection of device attributes

### Attributes
Represent specific properties or controls of a device:
- **Key**: Unique identifier within the device (required attribute: "state")
- **Name**: Display name
- **Entities**: List of Home Assistant entity IDs that this attribute controls

### Service Calls
Execute Home Assistant services on device entities:
- Services like `turn_on`, `turn_off`, `toggle`, `set_brightness`, etc.
- Pass additional data as JSON payload
- Automatically calls the service on all mapped entities

## Technology Stack

- **.NET 10**: Latest .NET framework
- **ASP.NET Core**: Web API framework
- **Entity Framework Core 10**: ORM for database access
- **PostgreSQL**: Relational database
- **Npgsql**: PostgreSQL provider for EF Core
- **Swashbuckle**: API documentation (Swagger/OpenAPI)
- **System.Reactive**: Reactive extensions
- **Polly**: Resilience and transient-fault-handling
- **WebSockets**: Real-time communication with Home Assistant

## Project Structure Details

### LimasIoTDevices.API
- Controllers for device and event management
- Middleware for exception handling and translation
- Swagger/OpenAPI configuration
- Startup configuration

### LimasIotDevices.Application
- Use cases implementing business logic
- Background services (WebSocket connection to Home Assistant)
- Validation services
- Translation management

### LimasIotDevices.Domain
- Core domain models
- Business interfaces
- Validation rules
- Translation models

### LimasIotDevices.Infrastructure
- Database context and configurations
- Home Assistant gateway implementation
- External service integrations
- Data persistence

### LimasIoTDevices.Facade
- Public DTOs (Data Transfer Objects)
- Use case interfaces
- Service contracts

### LimasIoTDevices.Shared
- Exception handling middleware
- Extension methods
- Common utilities
- HTTP resilience configuration

## Development

### Building the Solution

```bash
dotnet build
```

### Building Docker Image

```bash
# From the solution root directory
docker build -t limasiot-devices-api -f LimasIotDevices.API/Dockerfile .
```

### Running Tests

```bash
dotnet test
```

### Code Coverage

```bash
dotnet tool install -g dotnet-coverage
dotnet-coverage collect -f cobertura -o coverage.cobertura.xml dotnet test
```

## Deployment

### Production Considerations

1. **Use Docker Secrets or Environment Files**: Never commit sensitive data
   
   ```bash
   # Create .env file (add to .gitignore)
   echo "POSTGRES_PASSWORD=secure_password" > .env
   echo "HA_TOKEN=your_token" >> .env
   
   # Reference in docker-compose.yml
   env_file:
     - .env
   ```
