FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["LimasIotDevices.API/LimasIoTDevices.API.csproj", "LimasIotDevices.API/"]
COPY ["LimasIotDevices.Application/LimasIotDevices.Application.csproj", "LimasIotDevices.Application/"]
COPY ["LimasIoTDevices.Facade/LimasIoTDevices.Facade.csproj", "LimasIoTDevices.Facade/"]
COPY ["LimasIoTDevices.Shared/LimasIoTDevices.Shared.csproj", "LimasIoTDevices.Shared/"]
COPY ["LimasIotDevices.Infrastructure/LimasIotDevices.Infrastructure.csproj", "LimasIotDevices.Infrastructure/"]
COPY ["LimasIotDevices.Domain/LimasIotDevices.Domain.csproj", "LimasIotDevices.Domain/"]
RUN dotnet restore "./LimasIotDevices.API/LimasIoTDevices.API.csproj"
COPY . .
WORKDIR "/src/LimasIotDevices.API"
RUN dotnet build "./LimasIoTDevices.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./LimasIoTDevices.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LimasIoTDevices.API.dll"]