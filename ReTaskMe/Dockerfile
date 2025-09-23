# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS base

# Install globalization libraries
RUN apk add --no-cache icu-libs

WORKDIR /app
EXPOSE 8181

# Environment variables
ENV DOTNET_VERSION=9.0.0
ENV ASPNET_VERSION=9.0.0
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV ASPNETCORE_URLS=http://+:8181
ENV ASPNETCORE_HTTP_PORTS=8181
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV ASPNETCORE_ENVIRONMENT=Production

# Build image
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build

WORKDIR /src

# Copy project files (for caching restore layers)
COPY ["ReTaskMe/ReTaskMe.csproj", "ReTaskMe/"]
COPY ["BusinessLogicLayerCore/BusinessLogicLayerCore.csproj", "BusinessLogicLayerCore/"]
COPY ["DataAccessLayerCore/DataAccessLayerCore.csproj", "DataAccessLayerCore/"]
COPY ["HelperLayer/HelperLayer.csproj", "HelperLayer/"]

# Restore dependencies
RUN dotnet restore "ReTaskMe/ReTaskMe.csproj"

# Copy everything
COPY . .

# Build the main project
WORKDIR "/src/ReTaskMe"
RUN dotnet build "ReTaskMe.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "ReTaskMe.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "ReTaskMe.dll"]
