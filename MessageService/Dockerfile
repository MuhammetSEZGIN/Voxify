# 1) Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source

# Copy project file and restore dependencies
COPY MessageService/*.csproj ./
RUN dotnet restore

# Copy the rest of the files and publish
COPY MessageService/. ./
RUN dotnet publish -c Release -o /app/publish

# 2) Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Copy the published output
COPY --from=build /app/publish .

# Copy configuration files
COPY MessageService/appsettings*.json ./

# Listen on port 80
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80

ENTRYPOINT ["dotnet", "MessageService.dll"]