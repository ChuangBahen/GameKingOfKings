using System.ComponentModel.DataAnnotations;

namespace KingOfKings.Backend.Models;

/// <summary>
/// Represents a zone/area in the game world.
/// 代表遊戲世界中的區域。
/// </summary>
public class Zone
{
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// The name of the zone.
    /// 區域名稱。
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of the zone.
    /// 區域描述。
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Minimum recommended level for this zone.
    /// 此區域的最低建議等級。
    /// </summary>
    public int MinLevel { get; set; } = 1;

    /// <summary>
    /// Maximum recommended level for this zone.
    /// 此區域的最高建議等級。
    /// </summary>
    public int MaxLevel { get; set; } = 10;
}
