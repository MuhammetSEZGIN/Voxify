# 1) Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY ApiGateway/ApiGateway.csproj ApiGateway/
RUN dotnet restore ApiGateway/ApiGateway.csproj

# Copy the rest of the files and publish
COPY ApiGateway/. ApiGateway/
WORKDIR /src/ApiGateway
RUN dotnet publish ApiGateway.csproj -c Release -o /app/publish

# 2) Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Copy the published output
COPY --from=build /app/publish .

# Copy configuration files (needed for Ocelot)
COPY ApiGateway/ocelot.json ./
COPY ApiGateway/ocelot.docker.json ./
COPY ApiGateway/appsettings*.json ./

# Listen on port 5000
EXPOSE 5000
ENV ASPNETCORE_ENVIRONMENT=Docker
ENV ASPNETCORE_URLS=http://+:5000

ENTRYPOINT ["dotnet", "ApiGateway.dll"]