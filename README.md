# Appointment Booking System

A full-stack appointment booking system with:
- **Backend:** ASP.NET Core (C#) + Entity Framework Core + SQL Server
- **Frontend:** Angular + Angular Material
- **Authentication:** JWT-based, user-specific bookings
- **Deployment:** Docker Compose

---

## Features
- User registration and login (JWT authentication)
- Bookings are user-specific (users only see/manage their own)
- Modern, responsive dashboard (Angular Material)
- Secure API (CORS, JWT, EF Core)
- Easy local development with Docker Compose

---

## Quick Start (with Docker)

### 1. Prerequisites
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) installed
- [Git](https://git-scm.com/) installed

### 2. Clone the Repository
```bash
git clone <your-repo-url>
cd Appointment-Booking-System
```

### 3. Build and Run All Services
```bash
docker-compose build --no-cache
docker-compose up -d
```
- Frontend: [http://localhost:4200](http://localhost:4200)
- Backend API: [http://localhost:5000](http://localhost:5000)
- SQL Server: localhost:1433

### 4. Stop All Services
```bash
docker-compose down
```

---

## Manual Development (Optional)

### Backend (ASP.NET Core)
- Location: `backend/`
- Requirements: .NET 8 SDK, SQL Server

```bash
cd backend
# Update connection string in appsettings.json if needed
# Run migrations (if not using Docker):
dotnet ef database update
# Run the API
 dotnet run
```

### Frontend (Angular)
- Location: `front-end/angular-app/`
- Requirements: Node.js 18+, npm

```bash
cd front-end/angular-app
npm install
ng serve --open
```

---

## API Overview

### Authentication
- `POST /api/auth/register` — Register a new user
- `POST /api/auth/login` — Login, returns JWT and user info

### Bookings (JWT required)
- `GET /api/bookings` — List all bookings for current user
- `GET /api/bookings/{id}` — Get a single booking (must belong to user)
- `POST /api/bookings` — Create a booking (date, time, description)
- `PUT /api/bookings/{id}` — Update a booking (must belong to user)
- `DELETE /api/bookings/{id}` — Delete a booking (must belong to user)

---

## Troubleshooting

- **Database connection errors:**
  - Ensure Docker is running and port 1433 is not blocked.
  - If running locally, check your connection string in `backend/appsettings.json`.
- **CORS errors:**
  - The backend is configured for CORS. If you change ports, update allowed origins.
- **JWT errors:**
  - Ensure the JWT secret in `appsettings.json` is at least 256 bits (32 chars).
- **Frontend build errors:**
  - Make sure Node.js and npm versions are compatible (Node 18+ recommended).
- **API 400 errors on booking:**
  - Make sure you are not sending `userId` in the booking creation payload. The backend sets it automatically.

---

## License
MIT
