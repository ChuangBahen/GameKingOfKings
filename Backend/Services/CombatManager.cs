using KingOfKings.Backend.Data;
using KingOfKings.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace KingOfKings.Backend.Services;

/// <summary>
/// Combat management service implementation.
/// 戰鬥管理服務實作。
/// </summary>
public class CombatManager : ICombatManager
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Random _random = new();
    private const int COMBAT_TICK_INTERVAL_MS = 3000; // 3 seconds between auto-attacks

    public CombatManager(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<string> StartCombatAsync(Guid playerId, string targetName)
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var player = await db.PlayerCharacters.FindAsync(playerId);
        if (player == null) return "找不到玩家。";

        // Check if already in combat
        var existingCombat = await db.ActiveCombats.FirstOrDefaultAsync(c => c.PlayerId == playerId);
        if (existingCombat != null) return "你已經在戰鬥中了！";

        // Find monster spawn in the same room
        var monsterSpawn = await db.MonsterSpawns
            .Include(ms => ms.MonsterTemplate)
            .FirstOrDefaultAsync(ms =>
                ms.RoomId == player.CurrentRoomId &&
                ms.IsAlive &&
                !ms.InCombat &&
                ms.MonsterTemplate != null &&
                ms.MonsterTemplate.Name.ToLower().Contains(targetName.ToLower()));

        if (monsterSpawn == null)
        {
            // Try to find by exact monster template name
            var monster = await db.Monsters.FirstOrDefaultAsync(m =>
                m.LocationId == player.CurrentRoomId &&
                m.Name.ToLower().Contains(targetName.ToLower()));

            if (monster == null)
                return $"這裡找不到 '{targetName}'。";

            // Spawn a new instance of this monster
            monsterSpawn = new MonsterSpawn
            {
                Id = Guid.NewGuid(),
                MonsterTemplateId = monster.Id,
                RoomId = player.CurrentRoomId,
                CurrentHp = monster.MaxHp,
                InCombat = false,
                SpawnedAt = DateTime.UtcNow
            };
            db.MonsterSpawns.Add(monsterSpawn);
            await db.SaveChangesAsync();

            // Reload with template
            monsterSpawn = await db.MonsterSpawns
                .Include(ms => ms.MonsterTemplate)
                .FirstAsync(ms => ms.Id == monsterSpawn.Id);
        }

        // Start combat
        monsterSpawn.InCombat = true;
        monsterSpawn.EngagedPlayerId = playerId;

        var combat = new ActiveCombat
        {
            Id = Guid.NewGuid(),
            PlayerId = playerId,
            MonsterSpawnId = monsterSpawn.Id,
            StartedAt = DateTime.UtcNow,
            LastTick = DateTime.UtcNow,
            NextTick = DateTime.UtcNow.AddMilliseconds(COMBAT_TICK_INTERVAL_MS)
        };
        db.ActiveCombats.Add(combat);

        await db.SaveChangesAsync();

        var monsterName = monsterSpawn.MonsterTemplate?.Name ?? "怪物";
        return $"<span class='text-red-400'>⚔️ 開始與 {monsterName} 戰鬥！</span>\n" +
               $"<span class='text-gray-400'>{monsterName} 生命值：{monsterSpawn.CurrentHp}/{monsterSpawn.MonsterTemplate?.MaxHp}</span>";
    }

    public async Task<string> FleeAsync(Guid playerId)
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var combat = await db.ActiveCombats
            .Include(c => c.MonsterSpawn)
            .ThenInclude(ms => ms!.MonsterTemplate)
            .FirstOrDefaultAsync(c => c.PlayerId == playerId);

        if (combat == null) return "你不在戰鬥中。";

        // 50% chance to flee successfully
        if (_random.Next(100) < 50)
        {
            // Failed to flee - monster gets a free attack
            var player = await db.PlayerCharacters.FindAsync(playerId);
            if (player != null && combat.MonsterSpawn?.MonsterTemplate != null)
            {
                int damage = Math.Max(1, combat.MonsterSpawn.MonsterTemplate.Attack - (player.Stats.Con / 2));
                player.CurrentHp -= damage;
                if (player.CurrentHp < 0) player.CurrentHp = 0;
                await db.SaveChangesAsync();

                return $"<span class='text-yellow-400'>逃跑失敗！</span>\n" +
                       $"<span class='text-red-400'>{combat.MonsterSpawn.MonsterTemplate.Name} 對你造成 {damage} 點傷害！</span>";
            }
            return "<span class='text-yellow-400'>逃跑失敗！</span>";
        }

        // Successfully fled
        if (combat.MonsterSpawn != null)
        {
            combat.MonsterSpawn.InCombat = false;
            combat.MonsterSpawn.EngagedPlayerId = null;
        }
        db.ActiveCombats.Remove(combat);
        await db.SaveChangesAsync();

        return "<span class='text-green-400'>成功逃離戰鬥！</span>";
    }

    public async Task<string> UseSkillAsync(Guid playerId, string skillId)
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var player = await db.PlayerCharacters.FindAsync(playerId);
        if (player == null) return "找不到玩家。";

        var combat = await db.ActiveCombats
            .Include(c => c.MonsterSpawn)
            .ThenInclude(ms => ms!.MonsterTemplate)
            .FirstOrDefaultAsync(c => c.PlayerId == playerId);

        var skill = await db.Skills.FirstOrDefaultAsync(s => s.SkillId == skillId.ToLower());
        if (skill == null) return $"找不到技能 '{skillId}'。";

        // Check if player can use this skill
        if (skill.RequiredClass.HasValue && skill.RequiredClass.Value != player.Class)
            return $"此技能需要 {GetClassName(skill.RequiredClass.Value)} 職業。";

        if (skill.RequiredLevel > player.Level)
            return $"此技能需要等級 {skill.RequiredLevel}。";

        if (player.CurrentMp < skill.MpCost)
            return $"魔力不足！(目前 {player.CurrentMp}/需要 {skill.MpCost})";

        // Deduct MP
        player.CurrentMp -= skill.MpCost;

        // Calculate skill effect
        int statValue = skill.ScalingStat switch
        {
            "STR" => player.Stats.Str,
            "INT" => player.Stats.Int,
            "WIS" => player.Stats.Wis,
            "DEX" => player.Stats.Dex,
            "CON" => player.Stats.Con,
            _ => 10
        };

        int power = (int)(skill.BasePower + (statValue * skill.ScalingMultiplier));
        power = (int)(power * (0.9 + _random.NextDouble() * 0.2)); // 90-110% variance

        string result = "";

        switch (skill.Type)
        {
            case SkillType.Physical:
            case SkillType.Magical:
                if (combat == null || combat.MonsterSpawn == null)
                {
                    player.CurrentMp += skill.MpCost; // Refund MP
                    return "需要在戰鬥中才能使用攻擊技能！";
                }

                int damage = Math.Max(1, power - combat.MonsterSpawn.MonsterTemplate!.Defense);
                combat.MonsterSpawn.CurrentHp -= damage;

                string colorClass = skill.Type == SkillType.Magical ? "text-blue-400" : "text-orange-400";
                result = $"<span class='{colorClass}'>⚡ 施放 {skill.Name}！造成 {damage} 點傷害！</span>";

                if (combat.MonsterSpawn.CurrentHp <= 0)
                {
                    combat.MonsterSpawn.CurrentHp = 0;
                    result += $"\n<span class='text-yellow-400'>🎉 打倒了 {combat.MonsterSpawn.MonsterTemplate.Name}！</span>";
                }
                break;

            case SkillType.Healing:
                int healAmount = power;
                int oldHp = player.CurrentHp;
                player.CurrentHp = Math.Min(player.MaxHp, player.CurrentHp + healAmount);
                int actualHeal = player.CurrentHp - oldHp;

                result = $"<span class='text-green-400'>💚 施放 {skill.Name}！恢復 {actualHeal} 點生命值！</span>";
                break;
        }

        await db.SaveChangesAsync();
        return result;
    }

    public async Task<CombatTickResult> ProcessCombatTickAsync(Guid combatId)
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var combat = await db.ActiveCombats
            .Include(c => c.MonsterSpawn)
            .ThenInclude(ms => ms!.MonsterTemplate)
            .Include(c => c.Player)
            .FirstOrDefaultAsync(c => c.Id == combatId);

        if (combat == null || combat.Player == null || combat.MonsterSpawn?.MonsterTemplate == null)
        {
            return new CombatTickResult { CombatEnded = true, Message = "找不到戰鬥。" };
        }

        var player = combat.Player;
        var monsterSpawn = combat.MonsterSpawn;
        var monster = monsterSpawn.MonsterTemplate;

        var result = new CombatTickResult
        {
            PlayerId = player.Id,
            PlayerCurrentHp = player.CurrentHp,
            PlayerMaxHp = player.MaxHp,
            MonsterCurrentHp = monsterSpawn.CurrentHp,
            MonsterMaxHp = monster.MaxHp
        };

        // Player auto-attack
        int playerDamage = CalculatePlayerDamage(player, monster);
        monsterSpawn.CurrentHp -= playerDamage;
        result.Message = GenerateDynamicCombatMessage(
            player.Name,
            monster.Name,
            playerDamage,
            monsterSpawn.CurrentHp,
            monster.MaxHp,
            isPlayerAttacking: true);
        result.MonsterCurrentHp = monsterSpawn.CurrentHp;

        // Check monster death
        if (monsterSpawn.CurrentHp <= 0)
        {
            monsterSpawn.CurrentHp = 0;
            monsterSpawn.KilledAt = DateTime.UtcNow;
            monsterSpawn.InCombat = false;

            // Award exp
            player.Exp += monster.ExpReward;
            result.ExpGained = monster.ExpReward;

            // Process loot
            var lootEntries = await db.LootTableEntries
                .Include(l => l.Item)
                .Where(l => l.MonsterId == monster.Id)
                .ToListAsync();

            foreach (var lootEntry in lootEntries)
            {
                if (_random.NextDouble() * 100 <= lootEntry.DropRate)
                {
                    int quantity = _random.Next(lootEntry.MinQuantity, lootEntry.MaxQuantity + 1);

                    // Add to player inventory
                    var existingItem = await db.InventoryItems
                        .FirstOrDefaultAsync(i => i.PlayerId == player.Id && i.ItemId == lootEntry.ItemId && !i.IsEquipped);

                    if (existingItem != null)
                    {
                        existingItem.Quantity += quantity;
                    }
                    else
                    {
                        var newItem = new InventoryItem
                        {
                            Id = Guid.NewGuid(),
                            PlayerId = player.Id,
                            ItemId = lootEntry.ItemId,
                            Quantity = quantity,
                            SlotIndex = await GetNextInventorySlot(db, player.Id)
                        };
                        db.InventoryItems.Add(newItem);
                    }

                    result.Loot.Add(new LootDrop
                    {
                        ItemId = lootEntry.ItemId,
                        ItemName = lootEntry.Item?.Name ?? "Unknown",
                        Quantity = quantity
                    });
                }
            }

            result.Message += $"\n<span class='text-yellow-400'>🎉 打倒了 {monster.Name}！獲得 {monster.ExpReward} 經驗值</span>";

            if (result.Loot.Any())
            {
                result.Message += "\n<span class='text-cyan-400'>📦 掉落物品：" +
                    string.Join("、", result.Loot.Select(l => $"{l.ItemName} x{l.Quantity}")) + "</span>";
            }

            result.CombatEnded = true;
            result.MonsterDied = true;

            // Check level up
            var (didLevelUp, levelUpMsg) = await CheckLevelUp(db, player);
            if (didLevelUp)
            {
                result.Message += $"\n{levelUpMsg}";
            }

            // Remove combat and monster spawn
            db.ActiveCombats.Remove(combat);
            db.MonsterSpawns.Remove(monsterSpawn);
        }
        else
        {
            // Monster counter-attack
            int monsterDamage = Math.Max(1, monster.Attack - (player.Stats.Con / 2));
            monsterDamage = (int)(monsterDamage * (0.9 + _random.NextDouble() * 0.2));
            player.CurrentHp -= monsterDamage;
            result.PlayerCurrentHp = player.CurrentHp;

            result.Message += "\n" + GenerateDynamicCombatMessage(
                monster.Name,
                player.Name,
                monsterDamage,
                player.CurrentHp,
                player.MaxHp,
                isPlayerAttacking: false);

            // 加入玩家 HP 狀態警示
            var playerHpWarning = GetHpStatusDescription(player.CurrentHp, player.MaxHp, isPlayer: true);
            if (!string.IsNullOrEmpty(playerHpWarning))
                result.Message += $"\n{playerHpWarning}";

            // Check player death
            if (player.CurrentHp <= 0)
            {
                player.CurrentHp = 0;
                result.PlayerDied = true;
                result.CombatEnded = true;

                // Handle player death (respawn at village)
                player.CurrentHp = player.MaxHp / 2;
                player.CurrentMp = player.MaxMp / 2;
                player.CurrentRoomId = 1; // Village Square
                player.Exp = Math.Max(0, player.Exp - monster.ExpReward); // Lose some exp

                result.Message += "\n<span class='text-red-500'>💀 你陣亡了！在新手村廣場復活...</span>";

                // Clean up combat
                monsterSpawn.InCombat = false;
                monsterSpawn.EngagedPlayerId = null;
                db.ActiveCombats.Remove(combat);
            }
            else
            {
                // Update next tick time
                combat.LastTick = DateTime.UtcNow;
                combat.NextTick = DateTime.UtcNow.AddMilliseconds(COMBAT_TICK_INTERVAL_MS);
            }
        }

        await db.SaveChangesAsync();
        return result;
    }

    public async Task<List<ActiveCombat>> GetActiveCombatsAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        return await db.ActiveCombats
            .Where(c => c.NextTick <= DateTime.UtcNow)
            .ToListAsync();
    }

    public async Task<bool> IsInCombatAsync(Guid playerId)
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        return await db.ActiveCombats.AnyAsync(c => c.PlayerId == playerId);
    }

    public async Task<ActiveCombat?> GetPlayerCombatAsync(Guid playerId)
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        return await db.ActiveCombats
            .Include(c => c.MonsterSpawn)
            .ThenInclude(ms => ms!.MonsterTemplate)
            .FirstOrDefaultAsync(c => c.PlayerId == playerId);
    }

    private int CalculatePlayerDamage(PlayerCharacter player, Monster monster)
    {
        int baseDamage = player.Stats.Str * 2;
        int damage = Math.Max(1, baseDamage - monster.Defense);
        return (int)(damage * (0.9 + _random.NextDouble() * 0.2));
    }

    private static string GetClassName(ClassType classType)
    {
        return classType switch
        {
            ClassType.Warrior => "戰士",
            ClassType.Mage => "法師",
            ClassType.Priest => "牧師",
            _ => classType.ToString()
        };
    }

    /// <summary>
    /// 根據 HP 百分比取得狀態描述
    /// </summary>
    private static string GetHpStatusDescription(int currentHp, int maxHp, bool isPlayer)
    {
        double hpPercent = (double)currentHp / maxHp * 100;

        if (isPlayer)
        {
            return hpPercent switch
            {
                >= 75 => "",  // 狀態良好，不需提示
                >= 50 => "<span class='text-yellow-300'>⚠️ 你略顯疲態...</span>",
                >= 25 => "<span class='text-orange-400'>⚠️ 你傷痕累累！</span>",
                >= 10 => "<span class='text-red-400'>💔 你命懸一線！！</span>",
                _ => "<span class='text-red-500'>☠️ 你瀕臨死亡！！！</span>"
            };
        }
        else
        {
            return hpPercent switch
            {
                >= 75 => "",  // 怪物狀態良好
                >= 50 => "它看起來有些疲憊",
                >= 25 => "它搖搖晃晃",
                >= 10 => "它快撐不住了！",
                _ => "它奄奄一息！"
            };
        }
    }

    /// <summary>
    /// 根據傷害值取得攻擊描述
    /// </summary>
    private static string GetDamageDescription(int damage, int targetMaxHp, bool isCritical = false)
    {
        double damagePercent = (double)damage / targetMaxHp * 100;

        if (isCritical)
            return "💥 致命一擊！";

        return damagePercent switch
        {
            >= 25 => "💪 重擊！",
            >= 15 => "👊 有效攻擊！",
            >= 5 => "",  // 普通攻擊
            _ => "輕輕擦過..."
        };
    }

    /// <summary>
    /// 生成動態戰鬥訊息
    /// </summary>
    private string GenerateDynamicCombatMessage(
        string attackerName,
        string defenderName,
        int damage,
        int defenderCurrentHp,
        int defenderMaxHp,
        bool isPlayerAttacking)
    {
        var damageDesc = GetDamageDescription(damage, defenderMaxHp);
        var hpStatus = GetHpStatusDescription(defenderCurrentHp, defenderMaxHp, !isPlayerAttacking);

        string baseMessage;
        if (isPlayerAttacking)
        {
            baseMessage = !string.IsNullOrEmpty(damageDesc)
                ? $"<span class='text-orange-400'>{damageDesc} 你對 {defenderName} 造成 {damage} 點傷害！</span>"
                : $"<span class='text-orange-400'>你對 {defenderName} 造成 {damage} 點傷害！</span>";

            if (!string.IsNullOrEmpty(hpStatus))
                baseMessage += $" <span class='text-gray-400'>({hpStatus})</span>";
        }
        else
        {
            baseMessage = !string.IsNullOrEmpty(damageDesc)
                ? $"<span class='text-red-400'>{damageDesc} {attackerName} 對你造成 {damage} 點傷害！</span>"
                : $"<span class='text-red-400'>{attackerName} 對你造成 {damage} 點傷害！</span>";
        }

        return baseMessage;
    }

    private async Task<int> GetNextInventorySlot(AppDbContext db, Guid playerId)
    {
        var usedSlots = await db.InventoryItems
            .Where(i => i.PlayerId == playerId)
            .Select(i => i.SlotIndex)
            .ToListAsync();

        for (int i = 0; i < 25; i++)
        {
            if (!usedSlots.Contains(i)) return i;
        }
        return -1; // Inventory full
    }

    private async Task<(bool, string)> CheckLevelUp(AppDbContext db, PlayerCharacter player)
    {
        // Simple level formula: ExpRequired = Level * 100
        long expRequired = player.Level * 100;

        if (player.Exp >= expRequired)
        {
            player.Level++;
            player.Exp -= expRequired;

            // Increase stats based on class
            switch (player.Class)
            {
                case ClassType.Warrior:
                    player.Stats.Str += 3;
                    player.Stats.Con += 2;
                    player.Stats.Dex += 1;
                    player.MaxHp += 15;
                    player.MaxMp += 5;
                    break;
                case ClassType.Mage:
                    player.Stats.Int += 3;
                    player.Stats.Wis += 2;
                    player.Stats.Dex += 1;
                    player.MaxHp += 8;
                    player.MaxMp += 15;
                    break;
                case ClassType.Priest:
                    player.Stats.Wis += 3;
                    player.Stats.Int += 2;
                    player.Stats.Con += 1;
                    player.MaxHp += 10;
                    player.MaxMp += 12;
                    break;
            }

            // Restore HP/MP on level up
            player.CurrentHp = player.MaxHp;
            player.CurrentMp = player.MaxMp;

            return (true, $"<span class='text-yellow-300'>🌟 升級了！你現在是等級 {player.Level}！</span>");
        }

        return (false, "");
    }
}
