# 🎱 Bingora
[![Ask DeepWiki](https://deepwiki.com/badge.svg)](https://deepwiki.com/MinorPorras/Bingora)
[![.NET](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![React](https://img.shields.io/badge/React-18-61DAFB?logo=react&logoColor=black)](https://reactjs.org/)
[![TypeScript](https://img.shields.io/badge/TypeScript-5-3178C6?logo=typescript&logoColor=white)](https://www.typescriptlang.org/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-15-336791?logo=postgresql&logoColor=white)](https://www.postgresql.org/)
[![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?logo=docker&logoColor=white)](https://www.docker.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

**Bingora** es una plataforma web moderna para la creación, configuración y ejecución de juegos de bingo de forma digital y en tiempo real. 

El sistema está diseñado para resolver un problema real: digitalizar la experiencia del bingo en eventos familiares o comunitarios, permitiendo que los administradores controlen la partida desde una pantalla principal, mientras los jugadores participan escaneando un código QR desde sus dispositivos móviles, sin necesidad de crear cuentas.

> 🚧 **Estado del Proyecto:** En desarrollo activo (Fase MVP). Este repositorio sirve también como proyecto de portafolio para demostrar prácticas de arquitectura de software de nivel Mid/Senior.

---

## ✨ Características Principales (MVP)

### 👨‍💼 Para Administradores
- **Autenticación Segura:** Sistema de login con JWT y Refresh Tokens.
- **Gestión de Bingos:** Creación de juegos con valores personalizados y configuración de columnas/rangos.
- **Diseño de Patrones:** Editor visual de grid 5x5 para definir formas ganadoras (líneas, esquinas, cartón lleno).
- **Generación de Cartones:** Algoritmo que genera cartones únicos, con códigos QR listos para imprimir.
- **Tablero de Juego en Tiempo Real:** Pantalla principal con modo de pantalla completa (ideal para proyectores/TV), mostrando el número actual, historial y estado de la partida.
- **Detección Automática de Ganadores:** Validación instantánea de cartones al generar cada número.

### 📱 Para Jugadores
- **Acceso sin Fricción:** Escaneo del código QR del cartón físico para unirse a la partida.
- **Vinculación Rápida:** Ingreso de nombre para asociar la identidad al cartón en la partida activa.
- **Sincronización en Vivo:** Actualización automática del estado del cartón mediante WebSockets (SignalR).

---

## 🏗️ Arquitectura y Decisiones Técnicas

Este proyecto no es solo un CRUD; está construido con un enfoque en la mantenibilidad, escalabilidad y buenas prácticas de la industria:

- **Clean Architecture:** Separación estricta en capas (`Domain`, `Application`, `Infrastructure`, `Api`) para garantizar la independencia del framework y la facilidad de testing.
- **ASP.NET Core Minimal APIs:** Agrupadas en *Endpoint Groups* para mantener un `Program.cs` limpio y una estructura modular.
- **Patrón Result:** Manejo de errores de negocio esperados sin abusar de excepciones, mejorando el rendimiento y la predictibilidad del código.
- **Validación en la Frontera:** Uso de `FluentValidation` para garantizar la integridad de los datos antes de que toquen la lógica de negocio.
- **Base de Datos Moderna:** PostgreSQL con uso de **Esquemas** (`bingo.usuario`) en lugar de prefijos legacy, y columnas `jsonb` para configuraciones flexibles (ej. rangos de columnas).
- **Algoritmos Optimizados:** Representación de cartones y patrones ganadores mediante **Bitmasking** (operaciones a nivel de bit) para validar victorias en complejidad O(1).

---

## 🛠️ Stack Tecnológico

| Categoría | Tecnologías |
| :--- | :--- |
| **Backend** | C# 12, .NET 10, ASP.NET Core Minimal APIs, Entity Framework Core |
| **Frontend** | React 18, TypeScript, Vite, pnpm, TailwindCSS |
| **Tiempo Real** | SignalR (WebSockets) |
| **Base de Datos** | PostgreSQL 15 (Dockerizado para desarrollo local) |
| **Herramientas** | Docker, Docker Compose, Git, GitHub, Scalar (OpenAPI) |
| **Librerías Clave** | FluentValidation, BCrypt.Net, QRCoder, Microsoft.IdentityModel |

---

## 🚀 Guía de Inicio Rápido (Desarrollo Local)

Sigue estos pasos para levantar el proyecto en tu máquina.

### Prerrequisitos
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) (v18+) y `pnpm` (`npm install -g pnpm`)
- [Docker](https://www.docker.com/) y Docker Compose

### 1. Clonar el repositorio
```bash
git clone git@github.com:tu-usuario/bingora.git
cd bingora
```

### 2. Configurar Variables de Entorno
Copia el archivo de ejemplo y ajusta las variables si es necesario (especialmente las cadenas de conexión y claves JWT).
```bash
cp src/Bingora.Api/.env.example src/Bingora.Api/.env
```
(Nota: Nunca subas archivos .env reales al repositorio).

### 3. Levantar la Infraestructura (Base de Datos)
```bash
docker compose up -d
```
Esto iniciará un contenedor de PostgreSQL en el puerto 5432.

### 4. Ejecutar Migraciones y Backend
```bash
cd src/Bingora.Api
dotnet ef database update
dotnet run
```
La API estará disponible en https://localhost:5001 (o el puerto configurado). La documentación interactiva de la API se encuentra en /scalar.

### 5. Ejecutar el Frontend
Abre una nueva terminal:
```bash
cd frontend
pnpm install
pnpm dev
```
La aplicación web estará disponible en http://localhost:5173.

## 🗺️ Roadmap
- Configuración de Clean Architecture y Docker.
- Modelo de Dominio y Base de Datos (PostgreSQL + Esquemas).
- Autenticación (JWT, BCrypt, Result Pattern).
- Generación de cartones con algoritmo de columnas y QR.
- Lógica de Partidas y Detección de Ganadores (Bitmasking).
- Integración de SignalR para tiempo real.
- Pruebas Unitarias y de Integración (xUnit + Testcontainers).

## 🤝 Contribuciones
Aunque este es un proyecto personal de portafolio, las sugerencias, reportes de bugs o ideas de mejora son bienvenidos. Por favor, abre un Issue o un Pull Request siguiendo las buenas prácticas de Git.

💡 Desarrollado con ❤️ y mucho ☕ por [MinorPorras]
> [LinkedIn](https://www.linkedin.com/in/minor-porras/) | [Portafolio](https://minorporras.vercel.app)
