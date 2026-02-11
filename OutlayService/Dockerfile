# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY OutlayService.csproj .
RUN dotnet restore OutlayService.csproj --disable-parallel

# Copy everything else and publish
COPY . .
RUN dotnet publish OutlayService.csproj -c Release -o /outlay-service

# Final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /outlay-service

# Copy published output
COPY --from=build /outlay-service .

# Environment defaults (non‑secret)
ENV DOTNET_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "OutlayService.dll"]