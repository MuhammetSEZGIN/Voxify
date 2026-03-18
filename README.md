# Voxify - Real-Time Communication Platform (Backend)

Voxify is a Discord-like real-time communication platform built with a **Microservices Architecture** using **C# / .NET Core**. It provides text messaging, user presence tracking, and voice channel capabilities.

> 🖥️ **Frontend Repository:** You can find the highly-optimized, memory-friendly React client here: [MuhammetSEZGIN/sesver-react](https://github.com/MuhammetSEZGIN/voxify-react)

## 🚀 Features

- **Microservices Architecture:** Scalable services routed through an **Ocelot API Gateway**.
- **Real-Time Text & Presence:** Powered by **SignalR** for instant messaging and online/offline status tracking.
- **Voice Channels:** Integrated with **LiveKit** for high-quality, real-time audio streaming.
- **Event-Driven Communication:** Asynchronous inter-service messaging using **RabbitMQ** and **MassTransit** (ensuring eventual consistency).
- **Authentication & Authorization:** Secure access using **JWT** and Role-Based Access Control (RBAC).
- **Database per Service:** Independent **PostgreSQL** databases for each microservice to ensure loose coupling.
- **Containerized:** Fully deployable using **Docker Compose**.
- **CI/CD Integration:** Automated pipelines for seamless deployment. Currently deployed and running actively in a production environment (Turkey).

## 🛠️ Tech Stack

- **Framework:** .NET 8 / C#
- **Gateway:** Ocelot
- **Real-Time:** SignalR, LiveKit (WebRTC)
- **Message Broker:** RabbitMQ + MassTransit
- **Database:** PostgreSQL, Entity Framework Core
- **Testing:** xUnit, Moq
- **Containerization & DevOps:** Docker, Docker Compose, GitHub Actions (CI/CD)

## 🏗️ Architecture Overview

The system is designed with independent microservices communicating over an API Gateway for synchronous requests and a Message Broker (RabbitMQ) for asynchronous events (e.g., propagating clan/channel deletion to presence/voice services).

### Services
* **Identity Service:** Manages users, JWT generation, and RBAC.
* **Message Service:** Handles text messages via SignalR.
* **Presence Service:** Tracks user online/offline status and manages clan activities.
* **Voice / Clan Service:** Manages rooms, LiveKit tokens, and voice channel logic.

## ⚙️ Getting Started (Local Development)

### Prerequisites
- [Docker & Docker Compose](https://www.docker.com/)
- [.NET 8 SDK](https://dotnet.microsoft.com/download)

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/MuhammetSEZGIN/Voxify.git
   cd Voxify
   ```

2. Set up environment variables:
   - Configure your `appsettings.json` or `.env` files for PostgreSQL credentials, RabbitMQ, and LiveKit API keys.

3. Run the infrastructure and services using Docker Compose:
   ```bash
   docker-compose up -d
   ```
   *This will spin up PostgreSQL, RabbitMQ, LiveKit (if configured), and the .NET microservices.*

4. Apply database migrations:
   - Ensure Entity Framework migrations are applied for each service's database.

## ✅ Testing
The project uses `xUnit` and `Moq` for unit testing. To run the tests:
```bash
dotnet test
```

## 📄 License
This project is licensed under the MIT License.
