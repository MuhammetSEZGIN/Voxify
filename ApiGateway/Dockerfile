# 1) Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source

# Copy project file and restore dependencies
COPY ApiGateway/*.csproj ./
RUN dotnet restore

# Copy the rest of the files and publish
COPY ApiGateway/. ./
RUN dotnet publish -c Release -o /app/publish

# 2) Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Copy the published output
COPY --from=build /app/publish .

# Copy configuration files (needed for Ocelot)
COPY ApiGateway/ocelot.docker.json ./ocelot.json
COPY ApiGateway/swagger.json ./swagger.json
COPY ApiGateway/appsettings*.json ./

# Listen on port 5000
EXPOSE 5000
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:5000

ENTRYPOINT ["dotnet", "ApiGateway.dll"]