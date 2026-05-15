# CodeNameTwang - Meeting Room Booking System

A meeting room booking system built for **TayMark** (a fictional marketing agency) as part of the **CMP307 Software Engineering Practice** module at Abertay University.

## Architecture

| Layer | Technology | Purpose |
|---|---|---|
| **Client App** | Xamarin Forms (C#) | Cross-platform mobile/desktop booking UI |
| **Admin Panel** | Unity (C#) | Windows desktop admin area for CRUD on users, rooms, bookings |
| **REST API** | Node.js / Express | Mediates between clients and the database |
| **Database** | Microsoft SQL Server (T-SQL) | Relational schema in `Twang` namespace |

## Features

- Employee login via name + employee ID
- View today's bookings in a sidebar/master-detail layout
- Book meeting rooms with participant selection
- Real-time P2P notifications via custom TCP socket service
- Admin CRUD for users, rooms, and bookings
- CSV export of booking data (admin panel)
- Generic REST API with caching layer and permission checking

## Project Structure

```
├── Documentation/        # Diagrams, requirements, briefs (30+ files)
├── Project/
│   ├── Admin Area/       # Unity admin panel
│   ├── CodeNameTwang/    # Xamarin Forms client app (+ unit tests)
│   ├── RESTAPI/          # Node.js/Express backend
│   └── TSQLdb/           # Database schema (All.sql)
└── Precompiled bins/     # Prebuilt binaries
```

## Running the REST API

```bash
cd Project/RESTAPI
npm install
npm test          # Run unit tests (Jest)
node index.js     # Start server on port 80
```

## REST API Endpoints

| Endpoint | Method | Description |
|---|---|---|
| `/login` | GET | Employee login (name + ID) |
| `/loginadmin` | GET | Admin login (auth header) |
| `/logout` | GET | Logout (token) |
| `/DATA/:key/:table` | GET/PUT | CRUD on any table |
| `/DATA/:key/:table/:pk` | GET/POST/DELETE | CRUD on specific row |

## Notes

- Database credentials are hardcoded in `RESTAPI/crudOps/interface.js` for a university MSSQL server — these should be replaced with environment variables for production use.
- The Docker approach was deprecated because it interfered with IP tracking for online status.
- SSL setup is stubbed but not active.
