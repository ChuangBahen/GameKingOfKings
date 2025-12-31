using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KingOfKings.Backend.Models;

/// <summary>
/// 套裝加成定義
/// </summary>
public class SetBonus
{
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// 所屬套裝 ID
    /// </summary>
    public int SetId { get; set; }

    /// <summary>
    /// 所屬套裝
    /// </summary>
    [ForeignKey(nameof(SetId))]
    public EquipmentSet? EquipmentSet { get; set; }

    /// <summary>
    /// 啟用所需件數 (2, 3, 4...)
    /// </summary>
    [Range(2, 10)]
    public int RequiredPieces { get; set; }

    /// <summary>
    /// 加成屬性 JSON (e.g., {"MaxHp":20,"Atk":5})
    /// </summary>
    [Required]
    public string BonusJson { get; set; } = "{}";

    /// <summary>
    /// 加成描述 (e.g., "HP+20, 攻擊+5")
    /// </summary>
    [MaxLength(100)]
    public string Description { get; set; } = string.Empty;
}
