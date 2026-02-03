# GamesSharp - Board Game Sessions Manager

A web application built with ASP.NET Core and Entity Framework for managing board game sessions.

## Features

- **Game Management**: Add, edit, and manage your collection of board games with details like player count, duration, and genre
- **Player Management**: Register and track players with their contact information and gaming history
- **Session Management**: Schedule and organize game sessions with multiple players
- **Session Tracking**: Track session status (Planned, In Progress, Completed, Cancelled)
- **Player History**: View each player's gaming history and participation

## Technologies

- **ASP.NET Core 10.0** - Web framework
- **Entity Framework Core** - ORM for database access
- **SQL Server** - Database (LocalDB for development)
- **Bootstrap 5** - UI framework
- **MVC Pattern** - Application architecture

## Database Schema

The application uses the following main entities:

- **Game**: Stores board game information (name, description, player count, duration, genre)
- **Player**: Manages player profiles and registration details
- **GameSession**: Tracks scheduled game sessions with status and timing information
- **SessionPlayer**: Junction table for many-to-many relationship between sessions and players

## Getting Started

### Prerequisites

- .NET 10.0 SDK or later
- SQL Server or LocalDB

### Installation

1. Clone the repository:
```bash
git clone https://github.com/akostt/games-sharp.git
cd games-sharp
```

2. Restore dependencies:
```bash
dotnet restore
```

3. Update the connection string in `appsettings.json` if needed:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=GamesSharpDb;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

4. Create and apply database migrations:
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

5. Run the application:
```bash
dotnet run
```

6. Open your browser and navigate to `https://localhost:5001` or `http://localhost:5000`

## Project Structure

```
GamesSharp/
├── Controllers/        # MVC Controllers
│   ├── GamesController.cs
│   ├── PlayersController.cs
│   └── GameSessionsController.cs
├── Data/              # Database context
│   └── ApplicationDbContext.cs
├── Models/            # Data models
│   ├── Game.cs
│   ├── Player.cs
│   ├── GameSession.cs
│   └── SessionPlayer.cs
├── Views/             # Razor views
│   ├── Games/
│   ├── Players/
│   └── GameSessions/
└── wwwroot/           # Static files (CSS, JS, images)
```

## Usage

1. **Add Games**: Navigate to Games section and add your board games
2. **Register Players**: Add players who will participate in game sessions
3. **Schedule Sessions**: Create new game sessions, select a game, set date/time, and add players
4. **Track Progress**: Update session status and record results

## Sample Data

The application comes with sample games pre-seeded in the database:
- Catan (Strategy game, 3-4 players, 90 minutes)
- Ticket to Ride (Family game, 2-5 players, 60 minutes)
- Pandemic (Cooperative game, 2-4 players, 45 minutes)

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is open source and available under the MIT License.