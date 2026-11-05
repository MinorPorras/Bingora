# 🎱 Bingora

[![Ask DeepWiki](https://deepwiki.com/badge.svg)](https://deepwiki.com/MinorPorras/Bingora)
[![.NET](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![React](https://img.shields.io/badge/React-18-61DAFB?logo=react&logoColor=black)](https://reactjs.org/)
[![TypeScript](https://img.shields.io/badge/TypeScript-5-3178C6?logo=typescript&logoColor=white)](https://www.typescriptlang.org/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-15-336791?logo=postgresql&logoColor=white)](https://www.postgresql.org/)
[![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?logo=docker&logoColor=white)](https://www.docker.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

**Bingora** is a modern web platform designed for the creation, configuration, and real-time execution of digital bingo games. 

The system solves a real-world problem: digitizing the bingo experience for family or community events. It allows administrators to control the game from a main dashboard, while players join seamlessly by scanning a QR code from their mobile devices, without the need to create accounts.

> 🚧 **Project Status:** Active Development (MVP Phase). This repository also serves as a portfolio project to demonstrate Mid/Senior-level software architecture practices, Clean Architecture, and Domain-Driven Design (DDD).
> 
> 💡 **Note on Tech Evolution:** Originally architected in .NET 10 to establish a robust baseline. It is currently undergoing a strategic refactoring to **Java and Spring Boot** to demonstrate mastery of enterprise-grade design patterns across multiple ecosystems.

---

## ✨ Key Features (MVP)

### 👨‍💼 For Administrators
- **Secure Authentication:** JWT and Refresh Token-based login system.
- **Bingo Management:** Creation of games with custom values and flexible column/range configurations.
- **Pattern Design:** Visual 5x5 grid editor to define winning shapes (lines, corners, full card).
- **Card Generation:** Algorithm that generates unique cards with ready-to-print, dynamic QR codes.
- **Real-Time Game Dashboard:** Main screen with full-screen mode (ideal for projectors/TVs), displaying the current number, history, and game status.
- **Automated Winner Detection:** Instant card validation using bitwise operations upon each number draw.

### 📱 For Players
- **Frictionless Access:** Scan the physical card's QR code to join the active game instantly.
- **Quick Identity Binding:** Simple name entry to associate the player with their specific card in the active session.
- **Live Synchronization:** Automatic, real-time card state updates via WebSockets (SignalR).

---

## 🏗️ Architecture & Technical Decisions

This project goes beyond a simple CRUD application; it is built with a strong focus on maintainability, scalability, and industry best practices:

- **Clean Architecture & DDD:** Strict separation of concerns across layers (`Domain`, `Application`, `Infrastructure`, `Api`) to ensure framework independence, testability, and pure business logic.
- **ASP.NET Core Minimal APIs:** Organized into *Endpoint Groups* to maintain a clean `Program.cs` and a modular, scalable structure.
- **Result Pattern:** Handling expected business errors without abusing exceptions, improving performance and control flow predictability.
- **Boundary Validation:** Utilizing `FluentValidation` to guarantee data integrity before it reaches the business logic.
- **Modern Database Design:** PostgreSQL utilizing **Schemas** (e.g., `bingo.user`) instead of legacy table prefixes, and `jsonb` columns for flexible configurations. Includes persisted computed columns and dedicated indexes for O(1) case/accent-insensitive searches.
- **Optimized Algorithms:** Representation of cards and winning patterns using **Bitmasking** (bitwise operations) to validate wins in O(1) time complexity.

---

## 🛠️ Tech Stack

| Category | Technologies |
| :--- | :--- |
| **Backend** | C# 12, .NET 10, ASP.NET Core Minimal APIs, Entity Framework Core |
| **Frontend** | React 18, TypeScript, Vite, pnpm, TailwindCSS |
| **Real-Time** | SignalR (WebSockets) |
| **Database** | PostgreSQL 15 (Dockerized for local development) |
| **Tools** | Docker, Docker Compose, Git, GitHub, Scalar (OpenAPI) |
| **Key Libraries** | FluentValidation, BCrypt.Net, QRCoder, Microsoft.IdentityModel |

---

## 🚀 Quick Start Guide (Local Development)

Follow these steps to run the project on your local machine.

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) (v18+) and `pnpm` (`npm install -g pnpm`)
- [Docker](https://www.docker.com/) and Docker Compose

### 1. Clone the repository
```bash
git clone git@github.com:tu-usuario/bingora.git
cd bingora
```

### 2. Configure Environment Variables
Copy the example file and adjust the variables if necessary (especially database connection strings and JWT keys).
```bash
cp src/Bingora.Api/.env.example src/Bingora.Api/.env
```
*(Note: Never commit actual `.env` files to the repository).*

### 3. Start Infrastructure (Database)
```bash
docker compose up -d
```
This will start a PostgreSQL container on port 5432.

### 4. Run Migrations and Backend
```bash
cd src/Bingora.Api
dotnet ef database update
dotnet run
```
The API will be available at https://localhost:5001 (or your configured port). Interactive API documentation is available at /scalar.

### 5. Run the Frontend
Open a new terminal:
```bash
cd frontend
pnpm install
pnpm dev
```
The web application will be available at http://localhost:5173.

## 🗺️ Roadmap
- Clean Architecture & Docker setup.
- Domain Model & Database (PostgreSQL + Schemas + Bitmasking).
- Authentication (JWT, BCrypt, Result Pattern).
- Card generation with column algorithms and automated QRs.
- Game logic and Automated Winner Detection.
- Real-time communication with SignalR.
- 🔄 Strategic refactoring to Java & Spring Boot (In Progress).
- 🧪 Unit and Integration Testing (xUnit + Testcontainers).
- 🚀 Cloud Deployment (CI/CD pipeline).

## 🤝 Contributing
Although this is a personal portfolio project, suggestions, bug reports, or improvement ideas are highly welcome. Please open an Issue or a Pull Request following standard Git best practices (semantic commits, descriptive PRs).

💡 Developed with ❤️ and lots of ☕ by MinorPorras
> [LinkedIn](https://www.linkedin.com/in/minor-porras/) | [Portfolio](https://minorporras.vercel.app)
