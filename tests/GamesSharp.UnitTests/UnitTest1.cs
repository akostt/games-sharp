using GamesSharp.Data;
using GamesSharp.Helpers;
using GamesSharp.Models;
using GamesSharp.Services.GameSessions;
using Microsoft.EntityFrameworkCore;

namespace GamesSharp.UnitTests;

public class GameSessionServiceTests
{
    [Fact]
    public async Task ValidateSessionInputAsync_ShouldReturnError_WhenEndTimeEarlierThanStart()
    {
        // Arrange
        await using var context = CreateContext();
        var service = new GameSessionService(context);

        var session = new GameSession
        {
            GameId = 1,
            ScheduledDate = new DateTime(2026, 4, 20),
            ActualStartTime = new DateTime(2026, 4, 20, 12, 0, 0),
            ActualEndTime = new DateTime(2026, 4, 20, 11, 0, 0)
        };

        // Act
        var errors = await service.ValidateSessionInputAsync(session, selectedPlayers: null);

        // Assert
        Assert.Contains(errors, e => e.Key == nameof(GameSession.ActualEndTime));
    }

    [Fact]
    public async Task CreateSessionAsync_ShouldNormalizeTimes_SetScheduledStatus_AndAttachDistinctPlayers()
    {
        // Arrange
        await using var context = CreateContext();
        context.Games.Add(new Game
        {
            Id = 10,
            Name = "Terraforming Mars",
            MinPlayers = 1,
            MaxPlayers = 5,
            AverageDuration = 120
        });
        context.Players.AddRange(
            new Player { Id = 101, Name = "Alice", RegisteredDate = DateTime.UtcNow },
            new Player { Id = 102, Name = "Bob", RegisteredDate = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var service = new GameSessionService(context);
        var session = new GameSession
        {
            GameId = 10,
            ScheduledDate = new DateTime(2026, 5, 1),
            ActualStartTime = new DateTime(2026, 1, 1, 18, 30, 0),
            ActualEndTime = new DateTime(2026, 1, 1, 21, 0, 0),
            SessionStatusId = 99
        };

        // Act
        await service.CreateSessionAsync(session, new[] { 101, 101, 102 });
        var created = await context.GameSessions
            .Include(gs => gs.SessionPlayers)
            .SingleAsync();

        // Assert
        Assert.Equal(Constants.SessionStatus.ScheduledId, created.SessionStatusId);
        Assert.Equal(new DateTime(2026, 5, 1, 18, 30, 0), created.ActualStartTime);
        Assert.Equal(new DateTime(2026, 5, 1, 21, 0, 0), created.ActualEndTime);
        Assert.Equal(2, created.SessionPlayers.Count);
        Assert.Contains(created.SessionPlayers, sp => sp.PlayerId == 101);
        Assert.Contains(created.SessionPlayers, sp => sp.PlayerId == 102);
    }

    [Fact]
    public async Task SaveResultsAsync_ShouldUpdateScores_AndMarkSessionCompleted_WhenResultsProvided()
    {
        // Arrange
        await using var context = CreateContext();
        context.GameSessions.Add(new GameSession
        {
            Id = 200,
            GameId = 1,
            ScheduledDate = new DateTime(2026, 4, 20),
            SessionStatusId = Constants.SessionStatus.ScheduledId,
            SessionPlayers =
            {
                new SessionPlayer { Id = 301, PlayerId = 1 },
                new SessionPlayer { Id = 302, PlayerId = 2 }
            }
        });
        await context.SaveChangesAsync();

        var service = new GameSessionService(context);
        var input = new List<SessionPlayerResultInput>
        {
            new() { SessionPlayerId = 301, Score = 15, IsWinner = true },
            new() { SessionPlayerId = 302, Score = 10, IsWinner = false }
        };

        // Act
        var saved = await service.SaveResultsAsync(200, input);
        var updated = await context.GameSessions
            .Include(gs => gs.SessionPlayers)
            .SingleAsync(gs => gs.Id == 200);

        // Assert
        Assert.True(saved);
        Assert.Equal(Constants.SessionStatus.CompletedId, updated.SessionStatusId);
        Assert.Equal(15, updated.SessionPlayers.Single(sp => sp.Id == 301).Score);
        Assert.True(updated.SessionPlayers.Single(sp => sp.Id == 301).IsWinner);
    }

    [Fact]
    public async Task GetEquipmentAvailabilityAsync_ShouldReturnRequiredAndAvailable_WithEnoughFlag()
    {
        // Arrange
        await using var context = CreateContext();
        context.Equipments.AddRange(
            new Equipment { Id = 1, Name = "Dice" },
            new Equipment { Id = 2, Name = "Timer" });

        context.GameEquipments.AddRange(
            new GameEquipment { Id = 1, GameId = 1, EquipmentId = 1, RequiredQuantity = 3 },
            new GameEquipment { Id = 2, GameId = 1, EquipmentId = 2, RequiredQuantity = 1 });

        context.VenueEquipments.AddRange(
            new VenueEquipment { Id = 1, VenueId = 5, EquipmentId = 1, Quantity = 2 },
            new VenueEquipment { Id = 2, VenueId = 5, EquipmentId = 2, Quantity = 5 });

        await context.SaveChangesAsync();
        var service = new GameSessionService(context);

        // Act
        var availability = await service.GetEquipmentAvailabilityAsync(gameId: 1, venueId: 5);

        // Assert
        Assert.Equal(2, availability.Count);
        Assert.False(availability.Single(x => x.EquipmentId == 1).IsEnough);
        Assert.Equal(2, availability.Single(x => x.EquipmentId == 1).AvailableQuantity);
        Assert.True(availability.Single(x => x.EquipmentId == 2).IsEnough);
    }

    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"games-sharp-tests-{Guid.NewGuid()}")
            .Options;

        return new ApplicationDbContext(options);
    }
}
