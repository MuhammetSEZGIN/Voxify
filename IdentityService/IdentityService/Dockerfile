# 1) Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /workspace

# Copy project file and restore dependencies
COPY IdentityService/IdentityService/IdentityService.csproj IdentityService/IdentityService/
COPY Shared.Contracts/Shared.Contracts.csproj Shared.Contracts/
RUN dotnet restore IdentityService/IdentityService/IdentityService.csproj

# Copy the rest of the files and publish
COPY IdentityService/IdentityService/. IdentityService/IdentityService/
COPY Shared.Contracts/. Shared.Contracts/
WORKDIR /workspace/IdentityService/IdentityService
RUN dotnet publish IdentityService.csproj -c Release -o /app/publish

# 2) Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Copy the published output
COPY --from=build /app/publish .

# Copy configuration files
COPY IdentityService/IdentityService/appsettings*.json ./

# Listen on port 80
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80

ENTRYPOINT ["dotnet", "IdentityService.dll"]