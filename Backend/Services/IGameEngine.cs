namespace KingOfKings.Backend.Services;

public interface IGameEngine
{
    Task<string> ProcessCommandAsync(Guid playerId, string command);
    Task TickAsync();
}
