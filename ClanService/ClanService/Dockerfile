# 1) Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /workspace

# Copy project file and restore dependencies
COPY ClanService/ClanService/ClanService.csproj ClanService/ClanService/
COPY Shared.Contracts/Shared.Contracts.csproj Shared.Contracts/
RUN dotnet restore ClanService/ClanService/ClanService.csproj

# Copy the rest of the files and publish
COPY ClanService/ClanService/. ClanService/ClanService/
COPY Shared.Contracts/. Shared.Contracts/
WORKDIR /workspace/ClanService/ClanService
RUN dotnet publish ClanService.csproj -c Release -o /app/publish

# 2) Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Copy the published output
COPY --from=build /app/publish .

# Copy configuration files
COPY ClanService/ClanService/appsettings*.json ./

# Listen on port 80 instead of 5074
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80

ENTRYPOINT ["dotnet", "ClanService.dll"]