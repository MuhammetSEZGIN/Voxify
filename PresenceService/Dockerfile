# 1) Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY PresenceService/PresenceService.csproj PresenceService/
COPY Shared.Contracts/Shared.Contracts.csproj Shared.Contracts/
RUN dotnet restore PresenceService/PresenceService.csproj

# Copy the rest of the files and publish
COPY PresenceService/. PresenceService/
COPY Shared.Contracts/. Shared.Contracts/
WORKDIR /src/PresenceService
RUN dotnet publish PresenceService.csproj -c Release -o /app/publish

# 2) Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Copy the published output
COPY --from=build /app/publish .

# Copy configuration files
COPY PresenceService/appsettings*.json ./

# Listen on port 80
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80

ENTRYPOINT ["dotnet", "PresenceService.dll"]
