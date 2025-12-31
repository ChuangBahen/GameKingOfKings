using System.ComponentModel.DataAnnotations;

namespace KingOfKings.Backend.Models;

/// <summary>
/// 套裝定義
/// </summary>
public class EquipmentSet
{
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// 套裝名稱
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 套裝描述
    /// </summary>
    [MaxLength(200)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 套裝總件數
    /// </summary>
    [Range(2, 10)]
    public int TotalPieces { get; set; }

    /// <summary>
    /// 套裝包含的物品
    /// </summary>
    public ICollection<Item> Items { get; set; } = new List<Item>();

    /// <summary>
    /// 套裝加成
    /// </summary>
    public ICollection<SetBonus> Bonuses { get; set; } = new List<SetBonus>();
}
