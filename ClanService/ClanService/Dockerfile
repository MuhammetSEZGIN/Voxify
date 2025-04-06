# 1) Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source

# Copy project file and restore dependencies
COPY ClanService/*.csproj ./
RUN dotnet restore

# Copy the rest of the files and publish
COPY ClanService/. ./
RUN dotnet publish -c Release -o /app/publish

# 2) Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Copy the published output
COPY --from=build /app/publish .

# Copy configuration files
COPY ClanService/appsettings*.json ./

# Listen on port 80 instead of 5074
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80

ENTRYPOINT ["dotnet", "ClanService.dll"]