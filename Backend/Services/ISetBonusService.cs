namespace KingOfKings.Backend.Services;

/// <summary>
/// 套裝加成服務介面
/// </summary>
public interface ISetBonusService
{
    /// <summary>
    /// 計算玩家當前裝備的套裝加成
    /// </summary>
    /// <param name="playerId">玩家 ID</param>
    /// <returns>套裝加成字典 (屬性名稱 -> 加成值)</returns>
    Task<Dictionary<string, double>> CalculateSetBonusesAsync(Guid playerId);

    /// <summary>
    /// 取得玩家已啟用的套裝加成資訊
    /// </summary>
    /// <param name="playerId">玩家 ID</param>
    /// <returns>啟用的套裝加成 DTO 列表</returns>
    Task<List<ActiveSetInfo>> GetActiveSetBonusesAsync(Guid playerId);
}

/// <summary>
/// 啟用的套裝資訊
/// </summary>
public class ActiveSetInfo
{
    public int SetId { get; set; }
    public string SetName { get; set; } = string.Empty;
    public int EquippedPieces { get; set; }
    public int TotalPieces { get; set; }
    public List<ActiveBonusInfo> ActiveBonuses { get; set; } = new();
}

/// <summary>
/// 啟用的加成資訊
/// </summary>
public class ActiveBonusInfo
{
    public int RequiredPieces { get; set; }
    public string Description { get; set; } = string.Empty;
    public Dictionary<string, double> Bonuses { get; set; } = new();
}
