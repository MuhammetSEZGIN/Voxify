# 1) Build aşaması
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source

# Proje dosyalarını kopyala ve restore et
COPY *.csproj .
RUN dotnet restore

# Kalan tüm dosyaları kopyala ve publish et
COPY . .
RUN dotnet publish -c Release -o /app/publish

# 2) Runtime aşaması
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Derlenmiş çıktıyı kopyala
COPY --from=build /app/publish .

# Uygulamayı 5074 portundan dinleyelim
EXPOSE 5074
ENV ASPNETCORE_URLS=http://+:5074

ENTRYPOINT ["dotnet", "ClanService.dll"]
