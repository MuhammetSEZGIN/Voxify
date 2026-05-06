FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /workspace

COPY VersionControlService/VersionControlService.csproj VersionControlService/
RUN dotnet restore VersionControlService/VersionControlService.csproj

COPY VersionControlService/. VersionControlService/
WORKDIR /workspace/VersionControlService
RUN dotnet publish VersionControlService.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

RUN mkdir -p /app/data

EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80

ENTRYPOINT ["dotnet", "VersionControlService.dll"]
