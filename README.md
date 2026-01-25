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
- **Real-time Updates**: WebSocket integration with Home Assistant for live state updates
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
  -e DB_CONNECTION_STRING="Host=your-postgres-host;Database=limasiot;Username=your_user;Password=your_password" \
  -e HOME_ASSISTANT_HOST_URL="http://your-homeassistant-instance:8123" \
  -e HOME_ASSISTANT_TOKEN="your-long-lived-access-token" \
  -e DEVICE_EVENT_DEBOUNCE_MILLISECONDS=1500 \
  limasiot-devices-api
```

#### Using Docker Compose

> **Note**: The PostgreSQL database should be created and managed separately by the developer. You can use a dedicated PostgreSQL container, a cloud-hosted database, or a local installation.

The `docker-compose.yml` file uses environment variables that you pass at runtime:

```yaml
version: "3.8"

services:
  api:
    build:
      context: .
      dockerfile: LimasIotDevices.API/Dockerfile
    ports:
      - "${HOST_PORT:-8080}:8080"
    environment:
      DB_CONNECTION_STRING: ${DB_CONNECTION_STRING}
      HOME_ASSISTANT_HOST_URL: ${HOME_ASSISTANT_HOST_URL}
      HOME_ASSISTANT_TOKEN: ${HOME_ASSISTANT_TOKEN}
      DEVICE_EVENT_DEBOUNCE_MILLISECONDS: ${DEVICE_EVENT_DEBOUNCE_MILLISECONDS:-1500}
```

Run with Docker Compose by passing environment variables directly:

```bash
# Start the API service with environment variables
HOST_PORT=8080 \
DB_CONNECTION_STRING="Host=your-postgres-host;Database=limasiot;Username=your_user;Password=your_password" \
HOME_ASSISTANT_HOST_URL="http://your-homeassistant-instance:8123" \
HOME_ASSISTANT_TOKEN="your-long-lived-access-token" \
DEVICE_EVENT_DEBOUNCE_MILLISECONDS=1500 \
docker-compose up -d

# View logs
docker-compose logs -f api

# Stop the service
docker-compose down
```

The API will be available at:
- HTTP: `http://localhost:8080` (or the port defined in `HOST_PORT`)
- Swagger UI: `http://localhost:8080/swagger`

## Docker Configuration

### Dockerfile

The project includes a multi-stage Dockerfile optimized for production:

- **Base stage**: Uses .NET 10 ASP.NET runtime image
- **Build stage**: Compiles the application using .NET 10 SDK
- **Publish stage**: Publishes optimized release build
- **Final stage**: Creates minimal production image

### Environment Variables

When running in Docker, configure the following environment variables:

| Variable | Description | Default | Example |
|----------|-------------|---------|----------|
| `DB_CONNECTION_STRING` | PostgreSQL connection string | (required) | `Host=postgres;Database=limasiot;Username=user;Password=pass` |
| `HOME_ASSISTANT_HOST_URL` | Home Assistant URL | (required) | `http://homeassistant:8123` |
| `HOME_ASSISTANT_TOKEN` | Home Assistant access token | (required) | `your-long-lived-access-token` |
| `DEVICE_EVENT_DEBOUNCE_MILLISECONDS` | Debounce time for device events | `1500` | `1500` |
| `HOST_PORT` | Host port to expose the API | `8080` | `8080` |

### Docker Networking

When running with Docker Compose alongside Home Assistant:

```yaml
services:
  api:
    # ... other configuration ...
    networks:
      - homeassistant-network  # If Home Assistant is in a different network
    
networks:
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
- **Key**: Unique identifier within the device (required attribute: "main")
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
- **FluentMigrator**: Database migration framework
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

### LimasIotDevices.Shared
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

## Deployment

### Production Considerations

1. **Never commit sensitive data**: Pass environment variables through your deployment platform (GitHub Actions, Azure DevOps, Kubernetes secrets, etc.)

2. **Use secure connection strings**: Ensure your PostgreSQL database uses SSL/TLS in production

3. **Protect your Home Assistant token**: Use your platform's secret management system

