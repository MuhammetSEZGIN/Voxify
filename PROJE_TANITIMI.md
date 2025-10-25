# SESVER - Mikroservis Tabanlı İletişim Platformu

## Proje Genel Bakış

**Sesver**, Discord benzeri bir gerçek zamanlı iletişim platformudur. Proje, modern yazılım mimarisi prensiplerine uygun olarak **mikroservis mimarisi** kullanılarak geliştirilmiştir. .NET Core teknolojisi ile yazılmış, ölçeklenebilir ve modüler bir yapıya sahiptir.

## Teknik Stack

### Backend Teknolojileri
- **Framework**: .NET Core (C#)
- **Veritabanı**: PostgreSQL
- **Message Broker**: RabbitMQ
- **API Gateway**: Ocelot
- **Gerçek Zamanlı İletişim**: SignalR
- **Authentication**: JWT (JSON Web Tokens)
- **ORM**: Entity Framework Core
- **Containerization**: Docker & Docker Compose

### Mimari Desenler
- Mikroservis Mimarisi
- API Gateway Pattern
- Event-Driven Architecture (RabbitMQ ile)
- Repository Pattern
- Dependency Injection
- CQRS (kısmen, mesaj servisleri için)

## Mikroservis Yapısı

Proje 5 ana mikroservisten oluşmaktadır:

### 1. **IdentityService** (Port: 5158)
**Sorumluluklar:**
- Kullanıcı kaydı ve kimlik doğrulama
- JWT token üretimi ve yönetimi
- Rol tabanlı yetkilendirme (Role-Based Access Control)
- Kullanıcı profil yönetimi

**Temel Özellikler:**
- ASP.NET Core Identity kullanımı
- JWT Bearer Authentication
- Password hashing ve güvenlik
- RabbitMQ ile event publishing (kullanıcı oluşturma, güncelleme gibi)

**API Endpoints:**
- `/api/Auth` - Login, Register, Token işlemleri
- `/api/User` - Kullanıcı CRUD operasyonları
- `/api/Role` - Rol yönetimi

### 2. **ClanService** (Port: 5074)
**Sorumluluklar:**
- Clan (sunucu/topluluk) yönetimi
- Kanal (text ve voice) yönetimi
- Clan üyelik yönetimi
- Clan içi roller ve izinler
- Davet sistemi

**Temel Özellikler:**
- Clan oluşturma, silme, güncelleme
- Text ve voice channel yönetimi
- Üye ekleme/çıkarma/banlama
- Hiyerarşik rol sistemi
- AutoMapper ile DTO mapping
- Unit test coverage (MSTest)

**API Endpoints:**
- `/api/Clan` - Clan CRUD işlemleri
- `/api/Channel` - Text kanal yönetimi
- `/api/VoiceChannel` - Voice kanal yönetimi
- `/api/ClanMembership` - Üyelik yönetimi
- `/api/Role` - Clan rolleri yönetimi

### 3. **MessageService** (Port: 5107)
**Sorumluluklar:**
- Mesaj gönderme ve saklama
- Gerçek zamanlı mesajlaşma (SignalR Hub)
- Mesaj geçmişi yönetimi
- Mesaj silme ve düzenleme

**Temel Özellikler:**
- SignalR Hub ile WebSocket desteği
- Real-time mesaj broadcasting
- Kanal bazlı mesaj grupları
- Rate limiting (dakikada 20 mesaj sınırı)
- Background task queue ile asenkron işlemler
- Health checks
- CORS desteği

**API Endpoints:**
- `/api/Message` - Mesaj CRUD işlemleri
- SignalR Hub: `/messageHub` - Real-time mesajlaşma

**SignalR Hub Methods:**
- `SendMessage()` - Mesaj gönder
- `JoinChannel()` - Kanala katıl
- `LeaveChannel()` - Kanaldan ayrıl
- `ReceiveMessage` (client event) - Mesaj al

### 4. **VoiceService** (Port: 5044)
**Sorumluluklar:**
- Sesli iletişim yönetimi (geliştirilme aşamasında)
- Voice channel connection yönetimi

**Not:** Bu servis henüz temel yapıdadır ve geliştirilmeye devam etmektedir.

### 5. **ApiGateway** (Port: 5000)
**Sorumluluklar:**
- Tüm mikroservislere tek giriş noktası
- Routing ve load balancing
- Request/Response dönüşümleri
- Swagger UI aggregation

**Ocelot Configuration:**
- `/identity/*` → IdentityService
- `/message/*` → MessageService
- `/clan/*`, `/channel/*`, `/role/*`, `/voiceChannel/*`, `/clanMembership/*` → ClanService
- `/voice/*` → VoiceService

## Veritabanı Mimarisi

Her mikroservis kendi bağımsız PostgreSQL veritabanını kullanır (Database per Service pattern):

1. **identityservicedb** - Kullanıcılar ve kimlik bilgileri
2. **clanservicedb** - Clan'lar, kanallar, üyelikler
3. **messageservicedb** - Mesajlar ve mesaj geçmişi
4. **voiceservicedb** - Voice channel bilgileri

### Entity Framework Migrations
Her servis kendi migration'larını yönetir, bu sayede:
- Bağımsız deployment mümkündür
- Schema değişiklikleri izole edilmiştir
- Version control ile takip edilir

## Event-Driven Architecture (RabbitMQ)

Mikroservisler arası iletişim için RabbitMQ kullanılmaktadır:

**Publisher-Subscriber Pattern:**
- IdentityService yeni kullanıcı oluşturduğunda event publish eder
- ClanService ve MessageService bu event'i dinler ve kendi veritabanlarında kullanıcı kaydı oluşturur

**Avantajları:**
- Loose coupling (Gevşek bağlılık)
- Asenkron iletişim
- Eventual consistency
- Servisler arası bağımsızlık

**RabbitMQ Queues:**
- `user.created` - Yeni kullanıcı oluşturma eventi
- `user.updated` - Kullanıcı güncelleme eventi
- `user.deleted` - Kullanıcı silme eventi

## Güvenlik

### Authentication & Authorization
- **JWT Token** based authentication
- Symmetric key encryption (HS256)
- Token içinde: UserId, UserName, Roles
- Token expiration ve refresh mekanizması
- CORS policy yapılandırması

### Password Security
- ASP.NET Core Identity ile password hashing
- Minimum password requirements
- Password complexity rules

### API Security
- Rate limiting (MessageService'de)
- Input validation
- DTO pattern ile data exposure kontrolü

## Containerization & Deployment

### Docker Compose Yapısı
Tüm servisler Docker container'larında çalışır:

```yaml
services:
  - postgres (Port: 5432)
  - rabbitmq (Port: 5672, Management: 15672)
  - apigateway (Port: 5000)
  - identityservice (Port: 5158)
  - clanservice (Port: 5074)
  - messageservice (Port: 5107)
  - voiceservice (Port: 5044)
```

**Avantajları:**
- Tek komutla tüm sistemi ayağa kaldırma: `docker-compose up`
- Environment izolasyonu
- Kolay deployment
- Tutarlı development/production ortamları

## Test Stratejisi

**ClanService için Unit Tests:**
- Service layer testleri
- Controller testleri
- MSTest framework kullanımı
- Moq library ile mocking
- Repository pattern testleri

**Test Coverage Alanları:**
- ClanService
- ChannelService
- VoiceChannelService
- RoleService
- ClanMembershipService

## Kullanılan Design Patterns

1. **Repository Pattern** - Data access layer abstraction
2. **Dependency Injection** - IoC container kullanımı
3. **DTO Pattern** - Data transfer ve validation
4. **Factory Pattern** - Service creation
5. **Observer Pattern** - RabbitMQ event handling
6. **Gateway Pattern** - API Gateway (Ocelot)
7. **Circuit Breaker** - Potansiyel hata yönetimi için altyapı

## Ölçeklenebilirlik

Proje ölçeklenebilir olacak şekilde tasarlanmıştır:

### Horizontal Scaling
- Her mikroservis bağımsız olarak scale edilebilir
- Stateless servis tasarımı
- Load balancer desteği (API Gateway üzerinden)

### Database Scaling
- Her servis kendi DB'sine sahip (Database per Service)
- PostgreSQL connection pooling
- Read replica potansiyeli

### Message Queue
- RabbitMQ cluster desteği
- Asenkron işlem kuyruğu
- Message persistence

## API Dokümantasyonu

Her servis kendi Swagger UI'ına sahiptir:
- IdentityService: `http://localhost:5158/swagger`
- ClanService: `http://localhost:5074/swagger`
- MessageService: `http://localhost:5107/swagger`
- VoiceService: `http://localhost:5044/swagger`

API Gateway üzerinden: `http://localhost:5000/swagger`

## Rate Limiting

MessageService'de rate limiting uygulanmıştır:
- Fixed window limiter
- 20 request per window
- 429 Too Many Requests response

## Health Checks

MessageService'de health check endpoint'leri mevcuttur:
- Database connectivity check
- Service status monitoring

## Logging

Her serviste structured logging mevcuttur:
- Console logger
- Debug logger
- ILogger<T> dependency injection

## Geliştirme Ortamı

### Çalıştırma
```bash
# Tüm servisleri başlat
docker-compose up

# Belirli bir servisi build et
dotnet build Sesver.sln

# Testleri çalıştır
dotnet test ClanService/ClanServiceTests/UnitTests/UnitTests.csproj
```

### Configuration
- `appsettings.json` - Production ayarları
- `appsettings.Development.json` - Development ayarları (git'e commit edilmez)
- Environment variables - Docker compose'da tanımlı

## Proje Yapısı Özeti

```
Sesver/
├── ApiGateway/              # Ocelot API Gateway
├── IdentityService/         # Kimlik doğrulama servisi
├── ClanService/             # Clan ve kanal yönetimi
│   ├── ClanService/         # Ana servis
│   └── ClanServiceTests/    # Unit testler
├── MessageService/          # Mesajlaşma servisi (SignalR)
├── VoiceService/            # Voice servisi (in progress)
├── postgres-init/           # DB initialization scripts
├── docker-compose.yml       # Container orchestration
└── Sesver.sln              # Solution file
```

## Gelecek Geliştirmeler

1. **VoiceService Completion** - WebRTC ile sesli iletişim
2. **File Sharing** - Dosya upload/download
3. **Video Calls** - Video konferans özelliği
4. **Mobile App** - React Native veya Flutter ile mobile client
5. **Notification Service** - Push notification servisi
6. **Analytics Service** - Kullanım istatistikleri ve analytics
7. **Admin Panel** - Yönetim dashboard'u
8. **Kubernetes Deployment** - K8s ile production deployment
9. **CI/CD Pipeline** - GitHub Actions ile otomatik deployment
10. **Monitoring & APM** - Prometheus, Grafana, ELK Stack

## Teknolojik Güçlü Yönler

1. **Modern Mimari**: Mikroservis yaklaşımı ile maintainability ve scalability
2. **Event-Driven**: RabbitMQ ile loosely coupled servisler
3. **Real-time**: SignalR ile WebSocket desteği
4. **Containerization**: Docker ile kolay deployment
5. **Security**: JWT, Identity, CORS, Rate Limiting
6. **Testing**: Unit test infrastructure
7. **API Design**: RESTful API best practices
8. **Documentation**: Swagger/OpenAPI

## Görüşme İçin Temel Noktalar

### Mimari Kararlar
- Neden mikroservis? → Ölçeklenebilirlik, bağımsız deployment, teknoloji çeşitliliği
- Neden RabbitMQ? → Asenkron iletişim, loose coupling, eventual consistency
- Neden API Gateway? → Tek giriş noktası, routing, aggregation

### Teknik Zorluklar ve Çözümler
- **Data Consistency**: Event-driven architecture ile eventual consistency
- **Service Communication**: RabbitMQ messaging ve REST API'ler
- **Real-time Messaging**: SignalR hub implementation
- **Authentication**: JWT token flow ve mikroservisler arası token validation

### Ölçeklenebilirlik Stratejisi
- Horizontal scaling capability
- Database per service pattern
- Stateless service design
- Message queue for async processing

### Güvenlik Önlemleri
- JWT authentication
- Password hashing (Identity)
- Rate limiting
- Input validation
- CORS policy

---

**Proje Durumu**: Aktif geliştirme aşamasında
**Son Güncelleme**: 2025
**Teknoloji Seviyesi**: Production-ready (VoiceService hariç)
