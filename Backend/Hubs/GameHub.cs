using KingOfKings.Backend.Data;
using KingOfKings.Backend.DTOs;
using KingOfKings.Backend.Models;
using KingOfKings.Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;

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
        await Clients.All.SendAsync("ReceiveMessage", user, message, "general");
    }

    public async Task SendCommand(string command)
    {
        Console.WriteLine($"[GameHub] SendCommand received: '{command}'");

        if (!Context.Items.TryGetValue("PlayerId", out var idObj) || idObj is not Guid playerId)
        {
            Console.WriteLine("[GameHub] SendCommand - PlayerId not found in Context.Items");
            await Clients.Caller.SendAsync("ReceiveMessage", "System", "請先加入遊戲。", "system");
            return;
        }

        Console.WriteLine($"[GameHub] Processing command for player: {playerId}");
        var result = await _gameEngine.ProcessCommandAsync(playerId, command);
        Console.WriteLine($"[GameHub] Command result: {result?.Substring(0, Math.Min(100, result?.Length ?? 0))}...");

        // 判斷訊息類型
        var messageType = DetermineMessageType(command, result);
        await Clients.Caller.SendAsync("ReceiveMessage", "Game", result, messageType);

        // 根據指令類型推送相關資料更新
        await PushUpdatesBasedOnCommand(playerId, command);
    }

    /// <summary>
    /// 判斷訊息類型
    /// </summary>
    private static string DetermineMessageType(string command, string result)
    {
        var cmd = command.ToLower().Split(' ')[0];

        // 戰鬥相關指令
        if (cmd is "kill" or "attack" or "k" or "flee" or "run" or "cast" or "skill")
            return "combat";

        // 系統相關
        if (cmd is "help" or "?")
            return "system";

        // 根據結果內容判斷
        if (result.Contains("⚔️") || result.Contains("傷害") || result.Contains("戰鬥") ||
            result.Contains("damage") || result.Contains("打倒"))
            return "combat";

        return "general";
    }

    /// <summary>
    /// 根據指令推送相關資料更新
    /// </summary>
    private async Task PushUpdatesBasedOnCommand(Guid playerId, string command)
    {
        var cmd = command.ToLower().Split(' ')[0];

        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var player = await db.PlayerCharacters.FindAsync(playerId);
        if (player == null) return;

        switch (cmd)
        {
            case "move" or "go" or "n" or "s" or "e" or "w" or "north" or "south" or "east" or "west":
                await PushMapDataAsync(db, player);
                break;
            case "equip" or "unequip":
                await PushFullStatsAsync(db, player);
                await PushInventoryAsync(db, playerId);
                break;
            case "use":
                await PushFullStatsAsync(db, player);
                await PushInventoryAsync(db, playerId);
                break;
            case "rest":
                await PushFullStatsAsync(db, player);
                break;
        }
    }

    /// <summary>
    /// 加入遊戲 - 使用 JWT 認證的使用者資訊
    /// </summary>
    public async Task JoinGame()
    {
        var userId = GetAuthenticatedUserId();
        var username = GetAuthenticatedUsername();

        Console.WriteLine($"[GameHub] JoinGame - UserId: {userId}, Username: {username}");

        if (userId == Guid.Empty || string.IsNullOrEmpty(username))
        {
            await Clients.Caller.SendAsync("ReceiveMessage", "System", "認證失敗，請重新登入。", "system");
            return;
        }

        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var player = await db.PlayerCharacters.FirstOrDefaultAsync(p => p.UserId == userId);

        if (player == null)
        {
            Console.WriteLine($"[GameHub] Player not found, requesting class selection");
            await Clients.Caller.SendAsync("RequireClassSelection", new
            {
                Classes = new[]
                {
                    new { Type = 0, Name = "戰士", Description = "前線戰士，擅長物理攻擊與承受傷害", Stats = "STR 14 / DEX 10 / INT 6 / WIS 6 / CON 14 / HP 120 / MP 30" },
                    new { Type = 1, Name = "法師", Description = "魔法師，擅長法術攻擊但體質較弱", Stats = "STR 6 / DEX 10 / INT 14 / WIS 10 / CON 6 / HP 70 / MP 100" },
                    new { Type = 2, Name = "牧師", Description = "治療者，擅長恢復和輔助魔法", Stats = "STR 8 / DEX 8 / INT 10 / WIS 14 / CON 10 / HP 90 / MP 80" }
                }
            });
            return;
        }

        Context.Items["PlayerId"] = player.Id;
        await Clients.Caller.SendAsync("ReceiveMessage", "System", $"歡迎回來，{username}！你已進入萬王之王的世界。", "system");

        // 推送所有初始資料
        await PushFullStatsAsync(db, player);
        await PushInventoryAsync(db, player.Id);
        await PushMapDataAsync(db, player);
        await PushSkillsAsync(db, player);

        // 顯示目前房間
        var lookResult = await _gameEngine.ProcessCommandAsync(player.Id, "look");
        await Clients.Caller.SendAsync("ReceiveMessage", "Game", lookResult, "general");
    }

    /// <summary>
    /// 建立角色 - 玩家選擇職業後呼叫
    /// </summary>
    public async Task CreateCharacter(int classType)
    {
        var userId = GetAuthenticatedUserId();
        var username = GetAuthenticatedUsername();

        Console.WriteLine($"[GameHub] CreateCharacter - UserId: {userId}, ClassType: {classType}");

        if (userId == Guid.Empty || string.IsNullOrEmpty(username))
        {
            await Clients.Caller.SendAsync("ReceiveMessage", "System", "認證失敗，請重新登入。", "system");
            return;
        }

        var selectedClass = (ClassType)classType;
        if (!Enum.IsDefined(typeof(ClassType), selectedClass))
        {
            await Clients.Caller.SendAsync("ReceiveMessage", "System", "無效的職業選擇。", "system");
            return;
        }

        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (await db.PlayerCharacters.AnyAsync(p => p.UserId == userId))
        {
            await Clients.Caller.SendAsync("ReceiveMessage", "System", "你已經有角色了。", "system");
            return;
        }

        var (stats, hp, mp) = GetInitialStats(selectedClass);
        var player = new PlayerCharacter
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = username,
            Class = selectedClass,
            CurrentRoomId = 1,
            Stats = stats,
            MaxHp = hp,
            CurrentHp = hp,
            MaxMp = mp,
            CurrentMp = mp
        };

        db.PlayerCharacters.Add(player);
        await db.SaveChangesAsync();

        Context.Items["PlayerId"] = player.Id;

        var className = selectedClass switch
        {
            ClassType.Warrior => "戰士",
            ClassType.Mage => "法師",
            ClassType.Priest => "牧師",
            _ => "冒險者"
        };

        Console.WriteLine($"[GameHub] Character created: {username} as {className}");

        await Clients.Caller.SendAsync("CharacterCreated", new { ClassName = className });
        await Clients.Caller.SendAsync("ReceiveMessage", "System", $"歡迎，{className} {username}！你已進入萬王之王的世界。", "system");

        // 推送所有初始資料
        await PushFullStatsAsync(db, player);
        await PushInventoryAsync(db, player.Id);
        await PushMapDataAsync(db, player);
        await PushSkillsAsync(db, player);

        var lookResult = await _gameEngine.ProcessCommandAsync(player.Id, "look");
        await Clients.Caller.SendAsync("ReceiveMessage", "Game", lookResult, "general");
    }

    #region Push Methods

    /// <summary>
    /// 推送完整角色狀態
    /// </summary>
    public async Task PushFullStatsAsync(AppDbContext db, PlayerCharacter player)
    {
        var equipment = await GetEquipmentBonusesAsync(db, player.Id);

        var className = player.Class switch
        {
            ClassType.Warrior => "戰士",
            ClassType.Mage => "法師",
            ClassType.Priest => "牧師",
            _ => "冒險者"
        };

        var dto = new PlayerFullStatsDto
        {
            Name = player.Name,
            ClassName = className,
            Level = player.Level,
            Exp = player.Exp,
            ExpRequired = player.Level * 100,
            CurrentHp = player.CurrentHp,
            MaxHp = player.MaxHp,
            CurrentMp = player.CurrentMp,
            MaxMp = player.MaxMp,
            Stats = new CharacterStatsDto
            {
                Str = player.Stats.Str,
                Dex = player.Stats.Dex,
                Int = player.Stats.Int,
                Wis = player.Stats.Wis,
                Con = player.Stats.Con
            },
            EquipmentBonuses = new EquipmentBonusesDto
            {
                Atk = equipment["Atk"],
                Def = equipment["Def"],
                Str = equipment["Str"],
                Dex = equipment["Dex"],
                Int = equipment["Int"],
                Wis = equipment["Wis"],
                Con = equipment["Con"]
            }
        };

        await Clients.Caller.SendAsync("FullStatsUpdate", dto);
    }

    /// <summary>
    /// 推送背包資料
    /// </summary>
    public async Task PushInventoryAsync(AppDbContext db, Guid playerId)
    {
        var items = await db.InventoryItems
            .Include(i => i.Item)
            .Where(i => i.PlayerId == playerId)
            .OrderBy(i => i.SlotIndex)
            .ToListAsync();

        var dto = new InventoryDataDto
        {
            Gold = 0,
            Items = items.Where(i => i.Item != null).Select(i => new InventoryItemDto
            {
                Id = i.Id.ToString(),
                Name = i.Item!.Name,
                Type = i.Item.Type.ToString(),
                Icon = GetItemIcon(i.Item.Type, i.Item.Name),
                Quantity = i.Quantity,
                IsEquipped = i.IsEquipped,
                EquippedSlot = i.EquippedSlot.ToString(),
                Description = i.Item.Description ?? "",
                Properties = ParseItemProperties(i.Item.PropertiesJson)
            }).ToList()
        };

        await Clients.Caller.SendAsync("InventoryUpdate", dto);
    }

    /// <summary>
    /// 推送地圖資料
    /// </summary>
    public async Task PushMapDataAsync(AppDbContext db, PlayerCharacter player)
    {
        var room = await db.Rooms.FindAsync(player.CurrentRoomId);
        if (room == null) return;

        var monsters = await db.Monsters
            .Where(m => m.LocationId == player.CurrentRoomId)
            .Select(m => m.Name)
            .ToListAsync();

        var exits = new List<ExitInfoDto>();
        var exitsDict = JsonSerializer.Deserialize<Dictionary<string, int>>(room.ExitsJson);

        if (exitsDict != null)
        {
            foreach (var exit in exitsDict)
            {
                var targetRoom = await db.Rooms.FindAsync(exit.Value);
                var hasMonsters = await db.Monsters.AnyAsync(m => m.LocationId == exit.Value);

                exits.Add(new ExitInfoDto
                {
                    Direction = exit.Key,
                    RoomId = exit.Value,
                    RoomName = targetRoom?.Name ?? "未知",
                    HasMonsters = hasMonsters
                });
            }
        }

        var dto = new MapDataDto
        {
            CurrentRoom = new RoomInfoDto
            {
                Id = room.Id,
                Name = room.Name,
                Description = room.Description,
                Monsters = monsters
            },
            Exits = exits
        };

        await Clients.Caller.SendAsync("MapUpdate", dto);
    }

    /// <summary>
    /// 推送技能資料
    /// </summary>
    public async Task PushSkillsAsync(AppDbContext db, PlayerCharacter player)
    {
        var allSkills = await db.Skills
            .Where(s => !s.RequiredClass.HasValue || s.RequiredClass == player.Class)
            .ToListAsync();

        var learnedSkills = allSkills
            .Where(s => s.RequiredLevel <= player.Level)
            .Select(s => new SkillDto
            {
                SkillId = s.SkillId,
                Name = s.Name,
                Description = s.Description,
                Type = s.Type.ToString(),
                MpCost = s.MpCost,
                RequiredLevel = s.RequiredLevel,
                IsLearned = true
            }).ToList();

        var lockedSkills = allSkills
            .Where(s => s.RequiredLevel > player.Level)
            .Select(s => new SkillDto
            {
                SkillId = s.SkillId,
                Name = s.Name,
                Description = s.Description,
                Type = s.Type.ToString(),
                MpCost = s.MpCost,
                RequiredLevel = s.RequiredLevel,
                IsLearned = false
            }).ToList();

        var dto = new SkillsDataDto
        {
            LearnedSkills = learnedSkills,
            LockedSkills = lockedSkills
        };

        await Clients.Caller.SendAsync("SkillsUpdate", dto);
    }

    #endregion

    #region Helper Methods

    private static (CharacterStats Stats, int Hp, int Mp) GetInitialStats(ClassType classType)
    {
        return classType switch
        {
            ClassType.Warrior => (new CharacterStats { Str = 14, Dex = 10, Int = 6, Wis = 6, Con = 14 }, 120, 30),
            ClassType.Mage => (new CharacterStats { Str = 6, Dex = 10, Int = 14, Wis = 10, Con = 6 }, 70, 100),
            ClassType.Priest => (new CharacterStats { Str = 8, Dex = 8, Int = 10, Wis = 14, Con = 10 }, 90, 80),
            _ => (new CharacterStats { Str = 10, Dex = 10, Int = 10, Wis = 10, Con = 10 }, 100, 50)
        };
    }

    private async Task<Dictionary<string, int>> GetEquipmentBonusesAsync(AppDbContext db, Guid playerId)
    {
        var bonuses = new Dictionary<string, int>
        {
            { "Atk", 0 }, { "Def", 0 }, { "Str", 0 },
            { "Dex", 0 }, { "Int", 0 }, { "Wis", 0 }, { "Con", 0 }
        };

        var equippedItems = await db.InventoryItems
            .Include(i => i.Item)
            .Where(i => i.PlayerId == playerId && i.IsEquipped)
            .ToListAsync();

        foreach (var equipped in equippedItems)
        {
            if (equipped.Item == null) continue;

            try
            {
                var props = JsonSerializer.Deserialize<Dictionary<string, int>>(
                    equipped.Item.PropertiesJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (props == null) continue;

                foreach (var kvp in props)
                {
                    var key = kvp.Key.ToLower() switch
                    {
                        "atk" or "attack" => "Atk",
                        "def" or "defense" => "Def",
                        "str" or "strength" => "Str",
                        "dex" or "dexterity" => "Dex",
                        "int" or "intelligence" => "Int",
                        "wis" or "wisdom" => "Wis",
                        "con" or "constitution" => "Con",
                        _ => kvp.Key
                    };

                    if (bonuses.ContainsKey(key))
                        bonuses[key] += kvp.Value;
                }
            }
            catch (JsonException) { }
        }

        return bonuses;
    }

    private static string GetItemIcon(ItemType type, string name)
    {
        return type switch
        {
            ItemType.Weapon => "🗡️",
            ItemType.Armor => "🛡️",
            ItemType.Consumable => name.Contains("藥水") ? "🧪" : "📦",
            ItemType.Accessory => "💍",
            ItemType.Quest => "📜",
            _ => "📦"
        };
    }

    private static Dictionary<string, int> ParseItemProperties(string? json)
    {
        if (string.IsNullOrEmpty(json)) return new();
        try
        {
            return JsonSerializer.Deserialize<Dictionary<string, int>>(json) ?? new();
        }
        catch { return new(); }
    }

    #endregion

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
