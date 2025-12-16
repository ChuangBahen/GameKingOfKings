using KingOfKings.Backend.Data;
using KingOfKings.Backend.Models;
using KingOfKings.Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KingOfKings.Backend.Hubs;

/// <summary>
/// 遊戲 SignalR Hub - 處理即時遊戲通訊
/// </summary>
[Authorize]
public class GameHub : Hub
{
    private readonly IGameEngine _gameEngine;
    private readonly IServiceProvider _serviceProvider;

    public GameHub(IGameEngine gameEngine, IServiceProvider serviceProvider)
    {
        _gameEngine = gameEngine;
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// 從 JWT Claims 取得已認證的 UserId
    /// </summary>
    private Guid GetAuthenticatedUserId()
    {
        // 優先使用自訂的 UserId claim
        var userIdClaim = Context.User?.FindFirst("UserId")?.Value
                       ?? Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (Guid.TryParse(userIdClaim, out var userId))
        {
            return userId;
        }
        return Guid.Empty;
    }

    /// <summary>
    /// 從 JWT Claims 取得已認證的 Username
    /// </summary>
    private string GetAuthenticatedUsername()
    {
        return Context.User?.FindFirst(ClaimTypes.Name)?.Value
            ?? Context.User?.Identity?.Name
            ?? string.Empty;
    }

    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }

    public async Task SendCommand(string command)
    {
        Console.WriteLine($"[GameHub] SendCommand received: '{command}'");

        if (!Context.Items.TryGetValue("PlayerId", out var idObj) || idObj is not Guid playerId)
        {
            Console.WriteLine("[GameHub] SendCommand - PlayerId not found in Context.Items");
            await Clients.Caller.SendAsync("ReceiveMessage", "System", "請先加入遊戲。");
            return;
        }

        Console.WriteLine($"[GameHub] Processing command for player: {playerId}");
        var result = await _gameEngine.ProcessCommandAsync(playerId, command);
        Console.WriteLine($"[GameHub] Command result: {result?.Substring(0, Math.Min(100, result?.Length ?? 0))}...");
        await Clients.Caller.SendAsync("ReceiveMessage", "Game", result);
    }

    /// <summary>
    /// 加入遊戲 - 使用 JWT 認證的使用者資訊
    /// </summary>
    public async Task JoinGame()
    {
        var userId = GetAuthenticatedUserId();
        var username = GetAuthenticatedUsername();

        // 除錯：輸出認證資訊
        Console.WriteLine($"[GameHub] JoinGame - UserId: {userId}, Username: {username}");
        Console.WriteLine($"[GameHub] User IsAuthenticated: {Context.User?.Identity?.IsAuthenticated}");
        if (Context.User != null)
        {
            foreach (var claim in Context.User.Claims)
            {
                Console.WriteLine($"[GameHub] Claim: {claim.Type} = {claim.Value}");
            }
        }

        if (userId == Guid.Empty || string.IsNullOrEmpty(username))
        {
            await Clients.Caller.SendAsync("ReceiveMessage", "System", "認證失敗，請重新登入。");
            return;
        }

        // 從資料庫取得或建立玩家角色
        var playerId = await GetOrCreatePlayerAsync(userId, username);

        if (playerId == Guid.Empty)
        {
            await Clients.Caller.SendAsync("ReceiveMessage", "System", "無法加入遊戲，請重試。");
            return;
        }

        Context.Items["PlayerId"] = playerId;
        await Clients.Caller.SendAsync("ReceiveMessage", "System", $"歡迎回來，{username}！你已進入萬王之王的世界。");

        // 顯示目前房間
        var lookResult = await _gameEngine.ProcessCommandAsync(playerId, "look");
        await Clients.Caller.SendAsync("ReceiveMessage", "Game", lookResult);
    }

    /// <summary>
    /// 取得或建立玩家角色
    /// </summary>
    private async Task<Guid> GetOrCreatePlayerAsync(Guid userId, string username)
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var player = await db.PlayerCharacters.FirstOrDefaultAsync(p => p.UserId == userId);

        if (player == null)
        {
            // 為已認證的使用者建立新角色
            player = new PlayerCharacter
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = username,
                Class = ClassType.Warrior, // 預設職業，未來可加入選擇流程
                CurrentRoomId = 1,
                Stats = new CharacterStats { Str = 12, Dex = 10, Int = 8, Wis = 8, Con = 12 }
            };
            db.PlayerCharacters.Add(player);
            await db.SaveChangesAsync();
        }

        return player.Id;
    }

    public override async Task OnConnectedAsync()
    {
        var username = GetAuthenticatedUsername();
        Console.WriteLine($"[GameHub] 使用者連線: {username}");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var username = GetAuthenticatedUsername();
        Console.WriteLine($"[GameHub] 使用者斷線: {username}");
        await base.OnDisconnectedAsync(exception);
    }
}
