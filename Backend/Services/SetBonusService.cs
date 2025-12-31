using System.Text.Json;
using KingOfKings.Backend.Data;
using Microsoft.EntityFrameworkCore;

namespace KingOfKings.Backend.Services;

/// <summary>
/// 套裝加成服務實作
/// </summary>
public class SetBonusService : ISetBonusService
{
    private readonly IServiceProvider _serviceProvider;

    public SetBonusService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc />
    public async Task<Dictionary<string, double>> CalculateSetBonusesAsync(Guid playerId)
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var totalBonuses = new Dictionary<string, double>();

        // 取得玩家已裝備的物品
        var equippedItems = await db.InventoryItems
            .Include(i => i.Item)
            .Where(i => i.PlayerId == playerId && i.IsEquipped && i.Item != null && i.Item.SetId != null)
            .Select(i => i.Item!.SetId!.Value)
            .ToListAsync();

        if (!equippedItems.Any())
            return totalBonuses;

        // 統計每個套裝已裝備的件數
        var setEquippedCounts = equippedItems
            .GroupBy(setId => setId)
            .ToDictionary(g => g.Key, g => g.Count());

        // 取得相關套裝的加成規則
        var setIds = setEquippedCounts.Keys.ToList();
        var setBonuses = await db.SetBonuses
            .Where(sb => setIds.Contains(sb.SetId))
            .OrderBy(sb => sb.SetId)
            .ThenBy(sb => sb.RequiredPieces)
            .ToListAsync();

        // 計算啟用的加成
        foreach (var setBonus in setBonuses)
        {
            if (!setEquippedCounts.TryGetValue(setBonus.SetId, out int equippedCount))
                continue;

            // 檢查是否達到啟用門檻
            if (equippedCount >= setBonus.RequiredPieces)
            {
                try
                {
                    var bonuses = JsonSerializer.Deserialize<Dictionary<string, double>>(
                        setBonus.BonusJson,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (bonuses == null) continue;

                    foreach (var kvp in bonuses)
                    {
                        // 累加加成
                        if (totalBonuses.ContainsKey(kvp.Key))
                            totalBonuses[kvp.Key] += kvp.Value;
                        else
                            totalBonuses[kvp.Key] = kvp.Value;
                    }
                }
                catch (JsonException)
                {
                    // 忽略無效的 JSON
                }
            }
        }

        return totalBonuses;
    }

    /// <inheritdoc />
    public async Task<List<ActiveSetInfo>> GetActiveSetBonusesAsync(Guid playerId)
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var result = new List<ActiveSetInfo>();

        // 取得玩家已裝備的套裝物品
        var equippedSetItems = await db.InventoryItems
            .Include(i => i.Item)
                .ThenInclude(item => item!.EquipmentSet)
            .Where(i => i.PlayerId == playerId && i.IsEquipped && i.Item != null && i.Item.SetId != null)
            .Select(i => new { i.Item!.SetId, i.Item.EquipmentSet })
            .ToListAsync();

        if (!equippedSetItems.Any())
            return result;

        // 統計每個套裝已裝備的件數
        var setInfos = equippedSetItems
            .Where(x => x.SetId.HasValue && x.EquipmentSet != null)
            .GroupBy(x => x.SetId!.Value)
            .Select(g => new
            {
                SetId = g.Key,
                Set = g.First().EquipmentSet!,
                EquippedCount = g.Count()
            })
            .ToList();

        // 取得相關套裝的加成規則
        var setIds = setInfos.Select(s => s.SetId).ToList();
        var allBonuses = await db.SetBonuses
            .Where(sb => setIds.Contains(sb.SetId))
            .OrderBy(sb => sb.RequiredPieces)
            .ToListAsync();

        foreach (var setInfo in setInfos)
        {
            var activeSetInfo = new ActiveSetInfo
            {
                SetId = setInfo.SetId,
                SetName = setInfo.Set.Name,
                EquippedPieces = setInfo.EquippedCount,
                TotalPieces = setInfo.Set.TotalPieces,
                ActiveBonuses = new List<ActiveBonusInfo>()
            };

            // 找出該套裝已啟用的加成
            var setBonuses = allBonuses.Where(b => b.SetId == setInfo.SetId);
            foreach (var bonus in setBonuses)
            {
                if (setInfo.EquippedCount >= bonus.RequiredPieces)
                {
                    var bonusDict = new Dictionary<string, double>();
                    try
                    {
                        bonusDict = JsonSerializer.Deserialize<Dictionary<string, double>>(
                            bonus.BonusJson,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
                    }
                    catch (JsonException) { }

                    activeSetInfo.ActiveBonuses.Add(new ActiveBonusInfo
                    {
                        RequiredPieces = bonus.RequiredPieces,
                        Description = bonus.Description,
                        Bonuses = bonusDict
                    });
                }
            }

            // 只有至少有一個啟用加成才加入結果
            if (activeSetInfo.ActiveBonuses.Any())
            {
                result.Add(activeSetInfo);
            }
        }

        return result;
    }
}
