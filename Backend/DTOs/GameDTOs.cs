namespace KingOfKings.Backend.DTOs;

/// <summary>
/// 完整角色狀態 DTO
/// </summary>
public class PlayerFullStatsDto
{
    public string Name { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public int Level { get; set; }
    public long Exp { get; set; }
    public long ExpRequired { get; set; }
    public int CurrentHp { get; set; }
    public int MaxHp { get; set; }
    public int CurrentMp { get; set; }
    public int MaxMp { get; set; }
    public CharacterStatsDto Stats { get; set; } = new();
    public EquipmentBonusesDto EquipmentBonuses { get; set; } = new();

    /// <summary>
    /// 套裝加成（與裝備加成分離計算）
    /// </summary>
    public SetBonusesDto SetBonuses { get; set; } = new();

    /// <summary>
    /// 啟用的套裝詳細資訊
    /// </summary>
    public List<ActiveSetDto> ActiveSets { get; set; } = new();
}

public class CharacterStatsDto
{
    public int Str { get; set; }
    public int Dex { get; set; }
    public int Int { get; set; }
    public int Wis { get; set; }
    public int Con { get; set; }
}

public class EquipmentBonusesDto
{
    public int Atk { get; set; }
    public int Def { get; set; }
    public int Str { get; set; }
    public int Dex { get; set; }
    public int Int { get; set; }
    public int Wis { get; set; }
    public int Con { get; set; }
}

/// <summary>
/// 套裝加成 DTO（與裝備加成結構一致）
/// </summary>
public class SetBonusesDto
{
    public int Atk { get; set; }
    public int Def { get; set; }
    public int Str { get; set; }
    public int Dex { get; set; }
    public int Int { get; set; }
    public int Wis { get; set; }
    public int Con { get; set; }
}

/// <summary>
/// 背包資料 DTO
/// </summary>
public class InventoryDataDto
{
    public List<InventoryItemDto> Items { get; set; } = new();
    public int Gold { get; set; }
}

public class InventoryItemDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public bool IsEquipped { get; set; }
    public string EquippedSlot { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Dictionary<string, int> Properties { get; set; } = new();

    /// <summary>
    /// 裝備品質 (0=Common, 1=Uncommon, 2=Rare, 3=Legendary)
    /// </summary>
    public int Quality { get; set; }

    /// <summary>
    /// 所屬套裝 ID (null = 非套裝裝備)
    /// </summary>
    public int? SetId { get; set; }

    /// <summary>
    /// 所屬套裝名稱
    /// </summary>
    public string? SetName { get; set; }
}

/// <summary>
/// 地圖資料 DTO
/// </summary>
public class MapDataDto
{
    public RoomInfoDto CurrentRoom { get; set; } = new();
    public List<ExitInfoDto> Exits { get; set; } = new();
}

public class RoomInfoDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Monsters { get; set; } = new();
}

public class ExitInfoDto
{
    public string Direction { get; set; } = string.Empty;
    public int RoomId { get; set; }
    public string RoomName { get; set; } = string.Empty;
    public bool HasMonsters { get; set; }
}

/// <summary>
/// 技能資料 DTO
/// </summary>
public class SkillsDataDto
{
    public List<SkillDto> LearnedSkills { get; set; } = new();
    public List<SkillDto> LockedSkills { get; set; } = new();
}

public class SkillDto
{
    public string SkillId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int MpCost { get; set; }
    public int RequiredLevel { get; set; }
    public bool IsLearned { get; set; }
}

/// <summary>
/// 套裝資訊 DTO
/// </summary>
public class EquipmentSetDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int TotalPieces { get; set; }
    public List<SetBonusDto> Bonuses { get; set; } = new();
    public List<SetPieceDto> Pieces { get; set; } = new();
}

/// <summary>
/// 套裝加成 DTO
/// </summary>
public class SetBonusDto
{
    public int RequiredPieces { get; set; }
    public string Description { get; set; } = string.Empty;
    public Dictionary<string, double> Bonuses { get; set; } = new();
    public bool IsActive { get; set; }
}

/// <summary>
/// 套裝部件 DTO
/// </summary>
public class SetPieceDto
{
    public int ItemId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsOwned { get; set; }
    public bool IsEquipped { get; set; }
}

/// <summary>
/// 啟用的套裝加成 DTO
/// </summary>
public class ActiveSetBonusesDto
{
    public List<ActiveSetDto> ActiveSets { get; set; } = new();
}

/// <summary>
/// 單一啟用套裝資訊
/// </summary>
public class ActiveSetDto
{
    public string SetName { get; set; } = string.Empty;
    public int EquippedPieces { get; set; }
    public int TotalPieces { get; set; }
    public List<ActiveBonusDto> ActiveBonuses { get; set; } = new();
}

/// <summary>
/// 單一啟用加成
/// </summary>
public class ActiveBonusDto
{
    public int RequiredPieces { get; set; }
    public string Description { get; set; } = string.Empty;
}
