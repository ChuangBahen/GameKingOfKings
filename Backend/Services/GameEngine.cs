using KingOfKings.Backend.Data;
using KingOfKings.Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace KingOfKings.Backend.Services;

public class GameEngine : IGameEngine
{
    private readonly IServiceProvider _serviceProvider;

    public GameEngine(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<string> ProcessCommandAsync(Guid playerId, string command)
    {
        if (string.IsNullOrWhiteSpace(command)) return string.Empty;

        var parts = command.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var action = parts[0].ToLower();
        var args = parts.Length > 1 ? string.Join(" ", parts.Skip(1)) : string.Empty;

        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var combatManager = scope.ServiceProvider.GetRequiredService<ICombatManager>();

        var player = await db.PlayerCharacters
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == playerId);

        if (player == null) return "錯誤：找不到玩家。";

        // 檢查是否在戰鬥中（限制移動）
        var inCombat = await combatManager.IsInCombatAsync(playerId);

        switch (action)
        {
            case "look":
            case "l":
                return await HandleLook(db, player, inCombat);
            case "move":
            case "go":
            case "n":
            case "s":
            case "e":
            case "w":
            case "north":
            case "south":
            case "east":
            case "west":
                if (inCombat) return "<span class='text-yellow-400'>戰鬥中無法移動！輸入 'flee' 逃離戰鬥。</span>";
                return await HandleMove(db, player, action, args);
            case "say":
                return $"你說：「{args}」";
            case "kill":
            case "attack":
            case "k":
                if (string.IsNullOrEmpty(args)) return "攻擊什麼？用法：kill <目標>";
                return await combatManager.StartCombatAsync(playerId, args);
            case "flee":
            case "run":
                return await combatManager.FleeAsync(playerId);
            case "cast":
            case "skill":
                if (string.IsNullOrEmpty(args)) return await HandleSkillList(db, player);
                return await combatManager.UseSkillAsync(playerId, args);
            case "status":
            case "stats":
            case "st":
                return await HandleStatus(db, player, inCombat);
            case "inventory":
            case "inv":
            case "i":
                return await HandleInventory(db, playerId);
            case "equip":
                if (string.IsNullOrEmpty(args)) return "裝備什麼？用法：equip <物品名稱>";
                return await HandleEquip(db, playerId, args);
            case "unequip":
                if (string.IsNullOrEmpty(args)) return "卸下什麼？用法：unequip <物品名稱>";
                return await HandleUnequip(db, playerId, args);
            case "use":
                if (string.IsNullOrEmpty(args)) return "使用什麼？用法：use <物品名稱>";
                return await HandleUseItem(db, playerId, args);
            case "rest":
                if (inCombat) return "<span class='text-yellow-400'>戰鬥中無法休息！</span>";
                return await HandleRest(db, player);
            case "help":
            case "?":
                return HandleHelp();
            default:
                return "未知指令。輸入 'help' 查看指令列表。";
        }
    }

    private async Task<string> HandleLook(AppDbContext db, PlayerCharacter player, bool inCombat)
    {
        var room = await db.Rooms.FindAsync(player.CurrentRoomId);
        if (room == null) return "你置身於虛空之中。";

        var result = $"<div class='text-yellow-400 font-bold'>{room.Name}</div><div>{room.Description}</div>";

        // 顯示房間中的怪物
        var monsters = await db.Monsters.Where(m => m.LocationId == player.CurrentRoomId).ToListAsync();
        if (monsters.Any())
        {
            result += "\n<div class='text-red-300 mt-2'>這裡的怪物：" +
                string.Join("、", monsters.Select(m => m.Name)) + "</div>";
        }

        // 顯示出口
        var exits = JsonSerializer.Deserialize<Dictionary<string, int>>(room.ExitsJson);
        if (exits != null && exits.Any())
        {
            var exitNames = exits.Keys.Select(e => e switch
            {
                "n" => "北",
                "s" => "南",
                "e" => "東",
                "w" => "西",
                _ => e
            });
            result += $"\n<div class='text-green-300'>出口：{string.Join("、", exitNames)}</div>";
        }

        if (inCombat)
        {
            result += "\n<div class='text-red-400'>⚔️ 你正在戰鬥中！</div>";
        }

        return result;
    }

    private async Task<string> HandleMove(AppDbContext db, PlayerCharacter player, string action, string args)
    {
        // 標準化方向
        string direction = action;
        if (action == "move" || action == "go") direction = args;

        if (string.IsNullOrEmpty(direction)) return "往哪走？(n/s/e/w)";

        direction = direction.ToLower().Substring(0, 1); // n, s, e, w

        var room = await db.Rooms.FindAsync(player.CurrentRoomId);
        if (room == null) return "你被困住了。";

        var exits = JsonSerializer.Deserialize<Dictionary<string, int>>(room.ExitsJson);

        if (exits != null && exits.ContainsKey(direction))
        {
            player.CurrentRoomId = exits[direction];
            await db.SaveChangesAsync();
            return await HandleLook(db, player, false);
        }

        return "那個方向走不通。";
    }

    private async Task<string> HandleSkillList(AppDbContext db, PlayerCharacter player)
    {
        var skills = await db.Skills
            .Where(s => (!s.RequiredClass.HasValue || s.RequiredClass == player.Class) && s.RequiredLevel <= player.Level)
            .ToListAsync();

        if (!skills.Any())
            return "你還沒學會任何技能。";

        var result = "<div class='text-cyan-400 font-bold'>可用技能：</div>";
        foreach (var skill in skills)
        {
            string typeColor = skill.Type switch
            {
                SkillType.Physical => "text-orange-400",
                SkillType.Magical => "text-blue-400",
                SkillType.Healing => "text-green-400",
                _ => "text-white"
            };
            result += $"\n<div class='{typeColor}'>• {skill.Name} ({skill.SkillId}) - 魔力消耗: {skill.MpCost} - {skill.Description}</div>";
        }
        result += "\n<div class='text-gray-400'>用法：cast <技能代碼></div>";

        return result;
    }

    private async Task<string> HandleStatus(AppDbContext db, PlayerCharacter player, bool inCombat)
    {
        var className = player.Class switch
        {
            ClassType.Warrior => "戰士",
            ClassType.Mage => "法師",
            ClassType.Priest => "牧師",
            _ => player.Class.ToString()
        };

        // 計算裝備加成
        var eq = await GetEquipmentBonusesAsync(db, player.Id);
        int totalStr = player.Stats.Str + eq["Str"];
        int totalDex = player.Stats.Dex + eq["Dex"];
        int totalCon = player.Stats.Con + eq["Con"];
        int totalInt = player.Stats.Int + eq["Int"];
        int totalWis = player.Stats.Wis + eq["Wis"];
        int totalAtk = eq["Atk"];
        int totalDef = eq["Def"];

        // 格式化屬性顯示（有加成時顯示綠色 +N）
        string FormatStat(string name, int baseVal, int bonus) =>
            bonus > 0 ? $"{name}: {baseVal}<span class='text-green-400'>+{bonus}</span>" : $"{name}: {baseVal}";

        var result = $@"<div class='text-yellow-400 font-bold'>═══ {player.Name} ═══</div>
<div>職業: <span class='text-cyan-400'>{className}</span> | 等級: <span class='text-green-400'>{player.Level}</span></div>
<div>經驗值: <span class='text-yellow-300'>{player.Exp}/{player.Level * 100}</span></div>
<div class='mt-2'>生命值: <span class='text-red-400'>{player.CurrentHp}/{player.MaxHp}</span></div>
<div>魔力值: <span class='text-blue-400'>{player.CurrentMp}/{player.MaxMp}</span></div>
<div class='mt-2 text-gray-300'>═══ 屬性 ═══</div>
<div>{FormatStat("力量", player.Stats.Str, eq["Str"])} | {FormatStat("敏捷", player.Stats.Dex, eq["Dex"])} | {FormatStat("體質", player.Stats.Con, eq["Con"])}</div>
<div>{FormatStat("智力", player.Stats.Int, eq["Int"])} | {FormatStat("智慧", player.Stats.Wis, eq["Wis"])}</div>";

        // 顯示裝備攻防加成
        if (totalAtk > 0 || totalDef > 0)
        {
            result += $"\n<div class='text-orange-400'>攻擊: +{totalAtk} | 防禦: +{totalDef}</div>";
        }

        if (inCombat)
        {
            result += "\n<div class='text-red-400 mt-2'>⚔️ 目前正在戰鬥中！</div>";
        }

        return result;
    }

    private async Task<string> HandleInventory(AppDbContext db, Guid playerId)
    {
        var items = await db.InventoryItems
            .Include(i => i.Item)
            .Where(i => i.PlayerId == playerId)
            .OrderBy(i => i.SlotIndex)
            .ToListAsync();

        if (!items.Any())
            return "<div class='text-gray-400'>你的背包是空的。</div>";

        var result = "<div class='text-yellow-400 font-bold'>═══ 背包 ═══</div>";

        // 先顯示已裝備的物品
        var equipped = items.Where(i => i.IsEquipped).ToList();
        if (equipped.Any())
        {
            result += "\n<div class='text-cyan-400'>已裝備：</div>";
            foreach (var item in equipped)
            {
                var slotName = item.EquippedSlot switch
                {
                    EquipmentSlot.Weapon => "武器",
                    EquipmentSlot.Body => "防具",
                    EquipmentSlot.Head => "頭部",
                    EquipmentSlot.Accessory => "飾品",
                    _ => item.EquippedSlot.ToString()
                };
                result += $"\n<div class='text-green-400'>  [{slotName}] {item.Item?.Name}</div>";
            }
        }

        // 顯示背包物品
        var backpack = items.Where(i => !i.IsEquipped).ToList();
        if (backpack.Any())
        {
            result += "\n<div class='text-gray-300 mt-2'>背包物品：</div>";
            foreach (var item in backpack)
            {
                string typeColor = item.Item?.Type switch
                {
                    ItemType.Weapon => "text-orange-400",
                    ItemType.Armor => "text-blue-400",
                    ItemType.Consumable => "text-green-400",
                    ItemType.Quest => "text-purple-400",
                    _ => "text-white"
                };
                result += $"\n<div class='{typeColor}'>  • {item.Item?.Name} x{item.Quantity}</div>";
            }
        }

        return result;
    }

    private async Task<string> HandleEquip(AppDbContext db, Guid playerId, string itemName)
    {
        var inventoryItem = await db.InventoryItems
            .Include(i => i.Item)
            .FirstOrDefaultAsync(i =>
                i.PlayerId == playerId &&
                !i.IsEquipped &&
                i.Item != null &&
                i.Item.Name.ToLower().Contains(itemName.ToLower()));

        if (inventoryItem?.Item == null)
            return $"背包中找不到 '{itemName}'。";

        if (inventoryItem.Item.Type != ItemType.Weapon &&
            inventoryItem.Item.Type != ItemType.Armor &&
            inventoryItem.Item.Type != ItemType.Accessory)
            return "只有武器、防具和飾品可以裝備。";

        // Determine slot based on item type and name
        // 使用名稱判斷飾品（向後相容舊資料）
        var isAccessory = inventoryItem.Item.Type == ItemType.Accessory ||
                          inventoryItem.Item.Name.Contains("戒") ||
                          inventoryItem.Item.Name.Contains("項鍊") ||
                          inventoryItem.Item.Name.Contains("耳環");

        var slot = inventoryItem.Item.Type switch
        {
            ItemType.Weapon => EquipmentSlot.Weapon,
            _ when isAccessory => EquipmentSlot.Accessory,
            _ => EquipmentSlot.Body  // Armor
        };

        // Check if something is already equipped in that slot
        var currentEquipped = await db.InventoryItems
            .FirstOrDefaultAsync(i => i.PlayerId == playerId && i.IsEquipped && i.EquippedSlot == slot);

        if (currentEquipped != null)
        {
            currentEquipped.IsEquipped = false;
            currentEquipped.EquippedSlot = EquipmentSlot.None;
        }

        inventoryItem.IsEquipped = true;
        inventoryItem.EquippedSlot = slot;

        await db.SaveChangesAsync();

        return $"<span class='text-green-400'>你裝備了 {inventoryItem.Item.Name}。</span>";
    }

    private async Task<string> HandleUnequip(AppDbContext db, Guid playerId, string itemName)
    {
        var inventoryItem = await db.InventoryItems
            .Include(i => i.Item)
            .FirstOrDefaultAsync(i =>
                i.PlayerId == playerId &&
                i.IsEquipped &&
                i.Item != null &&
                i.Item.Name.ToLower().Contains(itemName.ToLower()));

        if (inventoryItem?.Item == null)
            return $"你沒有裝備 '{itemName}'。";

        inventoryItem.IsEquipped = false;
        inventoryItem.EquippedSlot = EquipmentSlot.None;

        await db.SaveChangesAsync();

        return $"<span class='text-yellow-400'>你卸下了 {inventoryItem.Item.Name}。</span>";
    }

    private async Task<string> HandleUseItem(AppDbContext db, Guid playerId, string itemName)
    {
        var inventoryItem = await db.InventoryItems
            .Include(i => i.Item)
            .FirstOrDefaultAsync(i =>
                i.PlayerId == playerId &&
                !i.IsEquipped &&
                i.Item != null &&
                i.Item.Type == ItemType.Consumable &&
                i.Item.Name.ToLower().Contains(itemName.ToLower()));

        if (inventoryItem?.Item == null)
            return $"背包中找不到可使用的 '{itemName}'。";

        var player = await db.PlayerCharacters.FindAsync(playerId);
        if (player == null) return "找不到玩家。";

        // Parse item properties
        var props = JsonSerializer.Deserialize<Dictionary<string, int>>(inventoryItem.Item.PropertiesJson);
        string result = "";

        if (props != null)
        {
            if (props.TryGetValue("HealHp", out int healHp))
            {
                int oldHp = player.CurrentHp;
                player.CurrentHp = Math.Min(player.MaxHp, player.CurrentHp + healHp);
                int actualHeal = player.CurrentHp - oldHp;
                result = $"<span class='text-green-400'>使用了 {inventoryItem.Item.Name}，恢復了 {actualHeal} 點生命值！</span>";
            }
            else if (props.TryGetValue("HealMp", out int healMp))
            {
                int oldMp = player.CurrentMp;
                player.CurrentMp = Math.Min(player.MaxMp, player.CurrentMp + healMp);
                int actualHeal = player.CurrentMp - oldMp;
                result = $"<span class='text-blue-400'>使用了 {inventoryItem.Item.Name}，恢復了 {actualHeal} 點魔力！</span>";
            }
        }

        // Reduce quantity
        inventoryItem.Quantity--;
        if (inventoryItem.Quantity <= 0)
        {
            db.InventoryItems.Remove(inventoryItem);
        }

        await db.SaveChangesAsync();

        return string.IsNullOrEmpty(result) ? $"使用了 {inventoryItem.Item.Name}。" : result;
    }

    private async Task<string> HandleRest(AppDbContext db, PlayerCharacter player)
    {
        // Restore 25% HP and MP
        int hpRestore = player.MaxHp / 4;
        int mpRestore = player.MaxMp / 4;

        int oldHp = player.CurrentHp;
        int oldMp = player.CurrentMp;

        player.CurrentHp = Math.Min(player.MaxHp, player.CurrentHp + hpRestore);
        player.CurrentMp = Math.Min(player.MaxMp, player.CurrentMp + mpRestore);

        await db.SaveChangesAsync();

        int actualHpRestore = player.CurrentHp - oldHp;
        int actualMpRestore = player.CurrentMp - oldMp;

        return $"<span class='text-green-400'>你休息了一會兒...</span>\n" +
               $"<span class='text-red-300'>生命值恢復：+{actualHpRestore}</span>\n" +
               $"<span class='text-blue-300'>魔力恢復：+{actualMpRestore}</span>";
    }

    private string HandleHelp()
    {
        return @"<div class='text-yellow-400 font-bold'>═══ 指令列表 ═══</div>
<div class='text-cyan-400'>移動指令：</div>
<div>  north (n), south (s), east (e), west (w) - 往指定方向移動</div>
<div>  look (l) - 查看周圍環境</div>
<div class='text-red-400 mt-2'>戰鬥指令：</div>
<div>  kill <目標> - 攻擊怪物</div>
<div>  flee - 嘗試逃離戰鬥</div>
<div>  cast <技能> - 施放技能</div>
<div>  skills - 查看技能列表</div>
<div class='text-green-400 mt-2'>角色指令：</div>
<div>  status (st) - 查看角色狀態</div>
<div>  inventory (i) - 查看背包</div>
<div>  equip <物品> - 裝備物品</div>
<div>  unequip <物品> - 卸下裝備</div>
<div>  use <物品> - 使用消耗品</div>
<div>  rest - 休息恢復生命和魔力</div>
<div class='text-gray-400 mt-2'>其他指令：</div>
<div>  say <訊息> - 說話</div>
<div>  help - 顯示此說明</div>";
    }

    public async Task TickAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // HP/MP regeneration for players not in combat
        var playersNotInCombat = await db.PlayerCharacters
            .Where(p => !db.ActiveCombats.Any(c => c.PlayerId == p.Id))
            .ToListAsync();

        foreach (var player in playersNotInCombat)
        {
            // Regenerate 1% HP and MP per tick
            if (player.CurrentHp < player.MaxHp)
            {
                player.CurrentHp = Math.Min(player.MaxHp, player.CurrentHp + Math.Max(1, player.MaxHp / 100));
            }
            if (player.CurrentMp < player.MaxMp)
            {
                player.CurrentMp = Math.Min(player.MaxMp, player.CurrentMp + Math.Max(1, player.MaxMp / 50));
            }
        }

        await db.SaveChangesAsync();
    }

    /// <summary>
    /// 計算玩家裝備的總屬性加成
    /// </summary>
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
                    // 標準化屬性名稱
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
            catch (JsonException)
            {
                // 忽略無效的 JSON
            }
        }

        return bonuses;
    }
}
