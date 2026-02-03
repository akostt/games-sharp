# GamesSharp - Architecture Documentation

## Application Architecture

This ASP.NET Core MVC web application follows a three-tier architecture:

### 1. Presentation Layer (Views)
- **Razor Views (.cshtml)**: Dynamic HTML pages with C# code
- **Layout**: Shared layout with Bootstrap 5 for responsive design
- **Controllers**: Handle HTTP requests and return views

### 2. Business Logic Layer (Controllers & Services)
- **Controllers**: Process user input, interact with data layer
  - `GamesController`: Manages board games CRUD operations
  - `PlayersController`: Manages player registration and profiles
  - `GameSessionsController`: Handles game session scheduling and management
  - `HomeController`: Serves the main dashboard

### 3. Data Access Layer (Models & DbContext)
- **Entity Framework Core**: ORM for database operations
- **Models**: Entity classes representing database tables
- **DbContext**: Database context for querying and saving data

## Database Schema

```
┌─────────────┐         ┌──────────────────┐         ┌─────────────┐
│    Game     │         │  GameSession     │         │   Player    │
├─────────────┤         ├──────────────────┤         ├─────────────┤
│ Id (PK)     │◄───────┤│ Id (PK)          │         │ Id (PK)     │
│ Name        │         │ GameId (FK)      │         │ Name        │
│ Description │         │ ScheduledDate    │         │ Email       │
│ MinPlayers  │         │ ActualStartTime  │         │ Phone       │
│ MaxPlayers  │         │ ActualEndTime    │         │ Registered  │
│ Duration    │         │ Location         │         └─────────────┘
│ Genre       │         │ Status           │               ▲
└─────────────┘         │ Notes            │               │
                        └──────────────────┘               │
                                ▲                           │
                                │                           │
                                │                           │
                        ┌───────┴─────────┐                │
                        │ SessionPlayer   │                │
                        ├─────────────────┤                │
                        │ Id (PK)         │                │
                        │ GameSessionId(FK)├───────────────┘
                        │ PlayerId (FK)   ├────────────────┘
                        │ Score           │
                        │ IsWinner        │
                        └─────────────────┘
```

## Key Relationships

1. **Game → GameSession** (One-to-Many)
   - A game can have multiple sessions
   - A session belongs to one game

2. **Player ↔ GameSession** (Many-to-Many via SessionPlayer)
   - A player can participate in multiple sessions
   - A session can have multiple players
   - SessionPlayer stores additional information (score, winner status)

## Application Flow

1. **User visits homepage** → Sees dashboard with links to Games, Players, and Sessions
2. **User manages games** → Can add, edit, view, or delete board games
3. **User registers players** → Can add player profiles with contact information
4. **User schedules session** → Selects game, date/time, location, and players
5. **User tracks sessions** → Can update status, record start/end times, and results

## Technology Stack

- **Framework**: ASP.NET Core 10.0 (MVC)
- **ORM**: Entity Framework Core 10.0
- **Database**: Microsoft SQL Server (LocalDB for development)
- **Frontend**: Bootstrap 5, jQuery
- **Validation**: jQuery Validation & Unobtrusive Validation

## Configuration

### Connection Strings
Configured in `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=GamesSharpDb;..."
}
```

### Dependency Injection
Registered in `Program.cs`:
- DbContext with SQL Server provider
- MVC Controllers with Views

## Security Considerations

1. **Anti-Forgery Tokens**: Used in all forms to prevent CSRF attacks
2. **Input Validation**: Model validation on both client and server side
3. **Connection Strings**: Should be stored in User Secrets or environment variables in production
4. **SQL Injection**: Protected by EF Core parameterized queries

## Future Enhancements

- User authentication and authorization
- Role-based access control
- Game statistics and analytics
- Real-time updates with SignalR
- Mobile responsive design improvements
- Search and filtering capabilities
- Email notifications for scheduled sessions
- API endpoints for mobile app integration
